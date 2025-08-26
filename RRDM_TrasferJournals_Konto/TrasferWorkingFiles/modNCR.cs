using System;
using System.IO;
using System.Text.RegularExpressions;

static class modNCR
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

    public static void ProcessNcrDirectory()
    {

        sErrorMessage = string.Empty;
        bErrorIndicator = false;

        try
        {
            System.Configuration.AppSettingsReader appSettingsReaderX = new System.Configuration.AppSettingsReader();

            string ncrPath = System.Convert.ToString(appSettingsReaderX.GetValue("NCR_Path", typeof(System.String)));
            string ncrExtension = System.Convert.ToString(appSettingsReaderX.GetValue("NCR_Extension", typeof(System.String)));
            string newExtension = System.Convert.ToString(appSettingsReaderX.GetValue("New_Extension", typeof(System.String)));

            string sRenameCopy = System.Convert.ToString(appSettingsReaderX.GetValue("RenameCopy", typeof(System.String)));
            int iNoOfBackwardMonths = System.Convert.ToInt32(appSettingsReaderX.GetValue("BackwardNoOfMonths", typeof(System.String)));

            string sExtension = "";

            bool bRenameCopy = (sRenameCopy == "True");
            string sWorkingPath = string.Empty;


            DateTime dtWFromDate;
            DateTime dtWToDate;


            dtWFromDate = dtFromDate.AddMonths(-iNoOfBackwardMonths);
            dtWToDate = dtToDate;

            string sRangeFromDir = System.Convert.ToString(dtWFromDate.Year) + TWF_PublicFn.PublicFn.fnRight(("00" + System.Convert.ToString(dtWFromDate.Month)), 2);
            string sRangeToDir = System.Convert.ToString(dtWToDate.Year) + TWF_PublicFn.PublicFn.fnRight(("00" + System.Convert.ToString(dtWToDate.Month)), 2);

            // 'Find the main directory
            // Dim di As DirectoryInfo = New DirectoryInfo(path & "\")
            sWorkingPath = sSourceDirectory;
            DirectoryInfo di = new DirectoryInfo(sSourceDirectory + @"\");


            // 'Find if exist main directory
            if (System.IO.Directory.Exists(sSourceDirectory + @"\"))
            {
                if (!System.IO.Directory.Exists(sTargetDirectory + @"\"))
                {
                    sErrorMessage = "ModuleNCR(ProcessNcrDirectory) - Target Directory not found.";
                    bErrorIndicator = true;
                    return;
                }
                else if (!TWF_PublicFn.PublicFn.DeleteFiles(sTargetDirectory))
                {
                    sErrorMessage = "ModuleNCR(ProcessNcrDirectory) - The Files in the Target Directory could not be deleted. The Operation is aborted.";
                    bErrorIndicator = true;
                    return;
                }

                sExtension = ncrExtension;

                iNoOfFiles = 0;

                // ' Directory Structure is as follows \Root\Atm\Year\mmDDMMSS
                // ' Find the First level Directory - Atm

                foreach (string sDirAtm in System.IO.Directory.GetDirectories(sSourceDirectory + @"\"))
                {
                    System.IO.DirectoryInfo dirAtmInfo = new System.IO.DirectoryInfo(sDirAtm);

                    string sAtmName = dirAtmInfo.Name;
                    sWorkingPath = sSourceDirectory + @"\" + sAtmName + @"\";
                    // 'Find the Year Level Directory
                    foreach (string sDirAtmInfo in System.IO.Directory.GetDirectories(sWorkingPath))
                    {
                        System.IO.DirectoryInfo dirYearInfo = new System.IO.DirectoryInfo(sDirAtmInfo);
                        string sAtmYear = dirYearInfo.Name;

                        if ((Int32.Parse(sAtmYear) >= dtWFromDate.Year) && (Int32.Parse(sAtmYear) <= dtWToDate.Year))
                        {
                            sWorkingPath = sSourceDirectory + @"\" + sAtmName + @"\" + sAtmYear + @"\";
                            // Find the files in directory & Process them
                            FilesSearchNcr(dirYearInfo, sAtmName, sAtmYear, sExtension, newExtension, bRenameCopy);
                        }
                    }
                }
            }
            else // ' Directory not Found
            {
                sErrorMessage = "ModuleNCR(ProcessNcrDirectory) - Source Directory not found. The Operation is aborted.";
                bErrorIndicator = true;
                return;
            }
        }

        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleNCR(ProcessNcrDirectory) - An exception has occured in the module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            sErrorMessage = sb.ToString();
            bErrorIndicator = true;
            return;
        }
        finally
        {
        }
    }

    private static void FilesSearchNcr(DirectoryInfo oDirInfo, string sAtmLevel, string sYearLevel, string sOldExtension, string sNewExtension, bool bRenameCopy)
    {
        try
        {

            // 'Get files of directory
            FileInfo[] resultFiles;

            string sSaveDate;
            string sDestination;
            string fileNameDateTime;
            string sBackDatedFileName;

            int iExtensionCounter = 0;

            DateTime dtWFromDate;
            DateTime dtCreationDate;



            System.Configuration.AppSettingsReader oAppSettingsReader = new System.Configuration.AppSettingsReader();

            string sFileRegex = System.Convert.ToString(oAppSettingsReader.GetValue("NCR_Regex", typeof(System.String)));
            Regex regex = new Regex(sFileRegex);

            dtWFromDate = dtFromDate;
            sBackDatedFileName = dtWFromDate.AddDays(-iBackwardNoOfDays).ToString("yyyyMMdd");
            // ' Delete Files in Target Directory
            resultFiles = oDirInfo.GetFiles();

            sSaveDate = "        ";


            // 'Find the files .000
            foreach (FileInfo resultFile in resultFiles)
            {
                try
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

                    Match oMatch = regex.Match(resultFile.Name);

                    if (oMatch.Success)
                    {

                        // 'Check if files with this type exists
                        if (resultFile.Extension == sOldExtension)
                        {
                            fileNameDateTime = sYearLevel + resultFile.Name.Substring(0, 4);
                            dtCreationDate = resultFile.CreationTime.Date;

                            // If Filename lies between the range specified by the User OR
                            // Filename is greater than the Backdated File name and the Creation Date is equal to 
                            // the Current Date then process the File

                            if ((Int64.Parse(dtCreationDate.ToString("yyyyMMdd")) >= Int64.Parse(dtFromDate.ToString("yyyyMMdd"))) &&
                               (Int64.Parse(dtCreationDate.ToString("yyyyMMdd")) <= Int64.Parse(dtToDate.ToString("yyyyMMdd"))))
                            {
                                if (sSaveDate == fileNameDateTime)
                                    iExtensionCounter++;
                                else
                                    iExtensionCounter = 0;

                                sSaveDate = fileNameDateTime;

                                sDestination = sTargetDirectory + @"\" + TWF_PublicFn.PublicFn.fnRight(("00000000" + sAtmLevel), 8) + "_" + sYearLevel + resultFile.Name.Substring(0, 4) + "_EJ_NCR." + iExtensionCounter.ToString("000");

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
            sb.AppendLine("ModuleNCR(FilesSearchNcr) - An exception has occured in the Module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            sErrorMessage = sb.ToString();
            bErrorIndicator = true;
            return;
        }
    }
}

