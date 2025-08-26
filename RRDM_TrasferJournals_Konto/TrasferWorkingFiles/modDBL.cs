using System;
using System.IO;
using System.Text.RegularExpressions;

static class modDBL
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

    public static void ProcessDblDirectory()
    {

        // ' The Structure of the Directory will be as follows
        // ' \Root\Atm\Files.DAT

        sFilesNotProcessed = string.Empty;

        try
        {
            System.Configuration.AppSettingsReader appSettingsReaderX = new System.Configuration.AppSettingsReader();

            string dblExtension = System.Convert.ToString(appSettingsReaderX.GetValue("DBL_Extension", typeof(System.String)));
            string sNewExtension = System.Convert.ToString(appSettingsReaderX.GetValue("New_Extension", typeof(System.String)));
            string sRenameCopy = System.Convert.ToString(appSettingsReaderX.GetValue("RenameCopy", typeof(System.String)));
            int iNoOfBackwardMonths = System.Convert.ToInt32(appSettingsReaderX.GetValue("BackwardNoOfMonths", typeof(System.String)));

            bool bRenameCopy = (sRenameCopy == "True");
            sErrorMessage = string.Empty;
            bErrorIndicator = false;

            string sPath;

            string sExtension = string.Empty;

            // 'Find the main directory
            // Dim di As DirectoryInfo = New DirectoryInfo(path & "\")
            sPath = sSourceDirectory;
            DirectoryInfo di = new DirectoryInfo(sPath + @"\");

            // 'Find if main directory exists
            if (System.IO.Directory.Exists(sPath + @"\"))
            {
                if (!System.IO.Directory.Exists(sTargetDirectory + @"\"))
                {
                    sErrorMessage = "ModuleDBL(ProcessDblDirectory) - Target Directory not found.";
                    bErrorIndicator = true;
                    return;
                }
                else
                {
                    if (!TWF_PublicFn.PublicFn.DeleteFiles(sTargetDirectory))
                    {
                        sErrorMessage = "ModuleDBL(ProcessDblDirectory) - The Files in the Target Directory could not be deleted. The Operation is aborted.";
                        bErrorIndicator = true;
                        return;
                    }
                }

                sExtension = dblExtension;

                iNoOfFiles = 0;

                // ' Find the First level Directory - Atm

                foreach (string sDirAtm in System.IO.Directory.GetDirectories(sPath + @"\"))
                {
                    System.IO.DirectoryInfo oDirAtmInfo = new System.IO.DirectoryInfo(sDirAtm);
                    string sAtmName = oDirAtmInfo.Name;

                    if (((("" + sAtmName).Trim()).Length) <= 7)
                    {
                        // 'Process the Files in the ATM Directory
                        sAtmName = sAtmName.Replace("-", "");
                        FilesSearchDbl(oDirAtmInfo, sAtmName, sExtension, sNewExtension, bRenameCopy);
                    }
                }
            }
            else // ' Source Directory not Found
            {
                sErrorMessage = "ModuleDBL(ProcessDblDirectory) - Source Directory not found. The Operation is aborted.";
                bErrorIndicator = true;
                return;
            }

        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleDBL(ProcessDblDirectory) - An exception has occured in the Module. The Operation is aborted.");
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

    private static void FilesSearchDbl(DirectoryInfo oDirInfo, string sAtmName, string sOldExtension, string sNewExtension, bool bRenameCopy)
    {

        // 'Get files of directory
        FileInfo[] resultFiles;
        string fileNameDateTime;
        string sDestination;

        DateTime dtCreationDate;
        string sBackDatedFileName;
        DateTime dtWFromDate;

        System.Configuration.AppSettingsReader oAppSettingsReader = new System.Configuration.AppSettingsReader();
        string sFileRegex = System.Convert.ToString(oAppSettingsReader.GetValue("DBL_Regex", typeof(System.String)));
        Regex regex = new Regex(sFileRegex);
        try
        {
            dtWFromDate = dtFromDate;
            sBackDatedFileName = (dtWFromDate.AddDays(-iBackwardNoOfDays)).ToString("yyyyMMdd");

            // ' Delete Files in Target Directory
            resultFiles = oDirInfo.GetFiles();

            // 'Find the files .000
            foreach (FileInfo resultFile in resultFiles)
            {
                Match oMatch = regex.Match(resultFile.Name);

                if (oMatch.Success)
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

                        // 'Check if files with this type exists - ''118@07-03-20190

                        // ' Result File Name is in the Following Format - XXX_ddmmyy_n 
                        // ' where
                        // ' XXX is the ATM Number
                        // ' dd is the day of the Month
                        // ' mm is the month
                        // ' yy is the year
                        // ' the file sequence within the day

                        if (resultFile.Extension == sOldExtension)
                        {
                            fileNameDateTime = "";
                            dtCreationDate = resultFile.CreationTime.Date;
                            string[] atmParts = resultFile.Name.Split((new char[] { '_' }));

                            if ((atmParts.Length >= 1))
                                fileNameDateTime = "20" + atmParts[1].Substring(4, 2) + atmParts[1].Substring(2, 2) + atmParts[1].Substring(0, 2);
                            else
                                fileNameDateTime = "20000000";


                            if ((Convert.ToInt64(dtCreationDate.ToString("yyyyMMdd")) >= Convert.ToInt64(dtFromDate.ToString("yyyyMMdd"))) & Convert.ToInt64(dtCreationDate.ToString("yyyyMMdd")) <= Convert.ToInt64(dtToDate.ToString("yyyyMMdd")))
                            {
                                sDestination = "";
                                sDestination = sTargetDirectory + @"\" + TWF_PublicFn.PublicFn.fnRight(("00000000" + sAtmName) , 8) + "_" + fileNameDateTime + "_EJ_DBL.00" + resultFile.Name.Substring(10, 1);

                                // 'Rename File
                                if (bRenameCopy == false)
                                    File.Move(resultFile.FullName, sDestination);
                                else
                                    File.Copy(resultFile.FullName, sDestination, true);

                                iNoOfFiles++;
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
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleDBL(FilesSearchDbl) - An exception has occured in the Module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            sErrorMessage = sb.ToString();
            bErrorIndicator = true;
            return;
        }
    }
   
}


