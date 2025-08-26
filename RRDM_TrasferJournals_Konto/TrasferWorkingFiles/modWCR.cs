using System;
using System.IO;
using System.Text.RegularExpressions;

static class modWCR
{
    public static DateTime dtFromDate;
    public static DateTime dtToDate;
    public static string sSourceDirectory;
    public static string sTargetDirectory;
    public static int iNoOfFiles;
    public static int iBackwardNoOfDays;
    public static int iForwardNoOfDays;
    public static string sErrorMessage;
    public static string sFilesNotProcessed;
    public static bool bErrorIndicator;

    public static void ProcessWcrDirectory()
    {

        // ' The Structure of the Directory will be as follows
        // ' \Root\Atm\Year\Month\Files.DAT
        string sWorkingPath = "";

        try
        {
            System.Configuration.AppSettingsReader appSettingsReaderX = new System.Configuration.AppSettingsReader();

            string WCRExtension = System.Convert.ToString(appSettingsReaderX.GetValue("WCR_Extension", typeof(System.String)));
            string sNewExtension = System.Convert.ToString(appSettingsReaderX.GetValue("New_Extension", typeof(System.String)));
            string sRenameCopy = System.Convert.ToString(appSettingsReaderX.GetValue("RenameCopy", typeof(System.String)));

            int iNoOfBackwardMonths = System.Convert.ToInt32(appSettingsReaderX.GetValue("BackwardNoOfMonths", typeof(System.String)));

            bool bRenameCopy = (sRenameCopy == "True");

            string sPath;
            // Dim sWorkingPath As String
            string sExtension = "";

            // 'Find the main directory
            // Dim di As DirectoryInfo = New DirectoryInfo(path & "\")
            sPath = sSourceDirectory;
            sWorkingPath = sPath;
            DirectoryInfo di = new DirectoryInfo(sPath + @"\");


            // 'Find if exist main directory
            if (System.IO.Directory.Exists(sPath + @"\"))
            {
                if (!System.IO.Directory.Exists(sTargetDirectory + @"\"))
                {
                    sErrorMessage = "ModuleWCR(ProcessWCRDirectory) - Target Directory not found.";
                    bErrorIndicator = true;
                    return;
                }
                else if (!TWF_PublicFn.PublicFn.DeleteFiles(sTargetDirectory))
                {
                    sErrorMessage = "ModuleWCR(ProcessWCRDirectory) - The Files in the Target Directory could not be deleted. The Operation is aborted.";
                    bErrorIndicator = true;
                    return;
                }

                sExtension = WCRExtension;
                iNoOfFiles = 0;

                // ' Using the From Date calculate the Backward & Forward Dates using the APPCONFIG Values (_iBackwardNoOfDays & _iForwardNoOfDays)
                // ' to be used for the Directory Filtering

                DateTime dtWFromDate;
                DateTime dtWToDate;

                dtWFromDate = dtFromDate.AddMonths(-iNoOfBackwardMonths);
                dtWToDate = dtToDate;

                string sRangeFromDir = System.Convert.ToString(dtWFromDate.Year) + TWF_PublicFn.PublicFn.fnRight(("00" + System.Convert.ToString(dtWFromDate.Month)), 2);
                string sRangeToDir = System.Convert.ToString(dtWToDate.Year) + TWF_PublicFn.PublicFn.fnRight(("00" + System.Convert.ToString(dtWToDate.Month)), 2);

                // ' Find the First level Directory - Atm

                foreach (string sDirAtm in System.IO.Directory.GetDirectories(sPath + @"\"))
                {
                    System.IO.DirectoryInfo dirAtmInfo = new System.IO.DirectoryInfo(sDirAtm);

                    string sAtmName = dirAtmInfo.Name;
                    sWorkingPath = sPath + @"\" + sAtmName + @"\";

                    // 'Find the Year Level Directory
                    foreach (string sDirAtmInfo in System.IO.Directory.GetDirectories(sWorkingPath))
                    {
                        System.IO.DirectoryInfo dirYearInfo = new System.IO.DirectoryInfo(sDirAtmInfo);
                        string sAtmYear = dirYearInfo.Name;

                        if ((Int32.Parse(sAtmYear) >= dtWFromDate.Year) && (Int32.Parse(sAtmYear) <= dtWToDate.Year))
                        {
                            sWorkingPath = sPath + @"\" + sAtmName + @"\" + sAtmYear + @"\";

                            // 'Find the Month Level Directory
                            foreach (string sDirYearInfo in System.IO.Directory.GetDirectories(sWorkingPath))
                            {
                                System.IO.DirectoryInfo oDirYearMonthInfo = new System.IO.DirectoryInfo(sDirYearInfo);
                                string sAtmYearMonth = oDirYearMonthInfo.Name;
                                string sWorkingDir = sAtmYear + TWF_PublicFn.PublicFn.fnRight(("00" + sAtmYearMonth), 2);

                                if ((Int64.Parse(sWorkingDir) >= Int64.Parse(sWorkingDir)) && (Int64.Parse(sWorkingDir) <= Int64.Parse(sRangeToDir)))
                                    FilesSearchWcr(oDirYearMonthInfo, sAtmName, sAtmYear, sAtmYearMonth, sExtension, sNewExtension, bRenameCopy);
                            }
                        }
                    }
                }
            }
            else // ' Directory not Found
            {
                sErrorMessage = "ModuleWCR(ProcessWCRDirectory) - Source Directory not found. The Operation is aborted.";
                bErrorIndicator = true;
                return;
            }
        }

        catch (Exception ex) // Throw New System.Exception(ex.Message)
        { 
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleWCR(ProcessWCRDirectory) - An exception has occured in the module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            sErrorMessage = sb.ToString();
            bErrorIndicator = true;
        }
        finally
        {
        }
    }

    private static void FilesSearchWcr(DirectoryInfo oDirInfo, string sAtmLevel, string sYearLevel, string sMonthLevel, string sOldExtension, string sNewExtension, bool bRenameCopy)
    {
        try
        {

            // 'Get files of directory
            FileInfo[] resultFiles;
            string fileNameDateTime;
            string sDestination;
           
            DateTime dtCreationDate;

            string sBackDatedFileName;
            string sForwardDatedFileName;

            DateTime dtWFromDate;
            DateTime dtWToDate;

            int iPos;
            string sDayLevel;

            System.Configuration.AppSettingsReader oAppSettingsReader = new System.Configuration.AppSettingsReader();
            string sFileRegex = System.Convert.ToString(oAppSettingsReader.GetValue("WCR_Regex", typeof(System.String)));
            Regex regex = new Regex(sFileRegex);


            dtWFromDate = dtFromDate;
            dtWToDate = dtToDate;

            sBackDatedFileName = dtWFromDate.ToString("yyyyMMdd");
            sForwardDatedFileName = dtWToDate.ToString("yyyyMMdd");


            // ' Delete Files in Target Directory
            resultFiles = oDirInfo.GetFiles();

         
            foreach (FileInfo resultFile in resultFiles)
            {
                try
                {
                    // ' FileName Format is 111@22-33-44445.dat where
                    // '  111 is the ATM Number
                    // '   22 is the Month
                    // '   33 is the Day of the Month
                    // ' 4444 is the Year 
                    // '    5 is the sequence of the file within the Day

                    Match oMatch = regex.Match(resultFile.Name);

                    if (oMatch.Success)
                    {
                        // Dim colItem(8) As String
                        // colItem(0) = (resultFile.Name)
                        // colItem(1) = (resultFile.DirectoryName)
                        // colItem(2) = (resultFile.LastAccessTime)
                        // colItem(3) = (resultFile.Length / 1000)
                        // colItem(4) = (resultFile.Attributes)
                        // colItem(5) = (resultFile.CreationTime)
                        // colItem(6) = (resultFile.Extension)
                        // colItem(7) = (resultFile.FullName)

                        // 'Check if files with this type exists
                        if (resultFile.Extension == sOldExtension)
                        {
                            // Strings.Right(("00000000" & AtmLevel), 8)
                            dtCreationDate = resultFile.CreationTime.Date;
                            fileNameDateTime = sYearLevel + TWF_PublicFn.PublicFn.fnRight(("00" + sMonthLevel), 2) + resultFile.Name.Substring(7, 2);


                            if ((Int64.Parse(dtCreationDate.ToString("yyyyMMdd")) >= Int64.Parse(dtFromDate.ToString("yyyyMMdd"))) && (Int64.Parse(dtCreationDate.ToString("yyyyMMdd")) <= Int64.Parse(dtToDate.ToString("yyyyMMdd"))))
                            {
                                iPos = 0;
                                iPos = resultFile.Name.IndexOf("@");
                                sDayLevel = resultFile.Name.Substring(iPos + 3, 2);

                                sDestination = sTargetDirectory + @"\" + TWF_PublicFn.PublicFn.fnRight(("00000000" + sAtmLevel), 8) + "_" + sYearLevel + TWF_PublicFn.PublicFn.fnRight(("00" + sMonthLevel), 2) + sDayLevel + "_EJ_WCR.00" + resultFile.Name.Substring(14, 1);

                                // 'Rename File
                                if (bRenameCopy == false)
                                    File.Move(resultFile.FullName, sDestination);
                                else
                                    File.Copy(resultFile.FullName, sDestination, true);

                                iNoOfFiles++;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine(sFilesNotProcessed);
                    sb.Append("File In Error..:");
                    sb.AppendLine(resultFile.FullName.ToString());
                    sFilesNotProcessed = sb.ToString();
                    continue;
                }
            }
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleWCR(FilesSearchWcr) - An exception has occured in the Module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            sErrorMessage = sb.ToString();
            bErrorIndicator = true;
            return;
        }
    }
}
