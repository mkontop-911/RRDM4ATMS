using System;
using System.IO;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

static class modNcrVision
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

    public static void ProcessNcrVisionDirectory()
    {
        try
        {
            System.Configuration.AppSettingsReader appSettingsReaderX = new System.Configuration.AppSettingsReader();

            string ncrNewExtension = System.Convert.ToString(appSettingsReaderX.GetValue("NCR_Vision_Extension", typeof(System.String)));
            string newExtension = System.Convert.ToString(appSettingsReaderX.GetValue("New_Extension", typeof(System.String)));
            string sRenameCopy = System.Convert.ToString(appSettingsReaderX.GetValue("RenameCopy", typeof(System.String)));

            int iNoOfBackwardMonths = System.Convert.ToInt32(appSettingsReaderX.GetValue("BackwardNoOfMonths", typeof(System.String)));

            string sPath;
            string sWorkingPath;
            string sExtension = "";

            bool bRenameCopy = (sRenameCopy == "True");
 
            DateTime dtWFromDate;
            DateTime dtWToDate;


            dtWFromDate = dtFromDate.AddMonths(-iNoOfBackwardMonths);
            dtWToDate = dtToDate;

            string sRangeFromDir = System.Convert.ToString(dtWFromDate.Year) + TWF_PublicFn.PublicFn.fnRight(("00" + System.Convert.ToString(dtWFromDate.Month)), 2);
            string sRangeToDir = System.Convert.ToString(dtWToDate.Year) + TWF_PublicFn.PublicFn.fnRight(("00" + System.Convert.ToString(dtWToDate.Month)), 2);

            // 'Find the main directory
            sPath = sSourceDirectory;
            sWorkingPath = sPath;
            DirectoryInfo di = new DirectoryInfo(sPath + @"\");


            // 'Find if exist main directory
            if (System.IO.Directory.Exists(sPath + @"\"))
            {
                if (!System.IO.Directory.Exists(sTargetDirectory + @"\"))
                {
                    sErrorMessage = "ModuleNcrVision(ProcessNcrVisionDirectory) - Target Directory not found.";
                    bErrorIndicator = true;
                    return;
                }
                else if (!TWF_PublicFn.PublicFn.DeleteFiles(sTargetDirectory))
                {
                    sErrorMessage = "ModuleNcrVision(ProcessNcrVisionDirectory) - The Files in the Target Directory could not be deleted. The Operation is aborted.";
                    bErrorIndicator = true;
                    return;
                }


                sExtension = ncrNewExtension;

                iNoOfFiles = 0;

                // ' Directory Structure is flat. Once you get in there you can find Files for all ATMs

                foreach (string sDir in System.IO.Directory.GetDirectories(sPath + @"\"))
                {
                    System.IO.DirectoryInfo oDirInfo = new System.IO.DirectoryInfo(sDir);

                    string sName = oDirInfo.Name;
                    sWorkingPath = sPath + @"\" + sName + @"\";
                    System.IO.DirectoryInfo oDirFileInfo = new System.IO.DirectoryInfo(sWorkingPath);
                    // Find the files in directory & Process them

                    FilesSearchNcrVision(oDirFileInfo, sExtension, newExtension, bRenameCopy);
                }
            }
            else // ' Directory not Found
            {
                sErrorMessage = "ModuleNcrVision(ProcessNcrVisionDirectory) - Source Directory not found. The Operation is aborted.";
                bErrorIndicator = true;
                return;
            }
        }

        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleNcrVision(ProcessNcrVisionDirectory) - An exception has occured in the module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            sErrorMessage = sb.ToString();
            bErrorIndicator = true;
        }
        finally
        {
        }
    }


    private static void FilesSearchNcrVision(DirectoryInfo oDirInfo, string sOldExtension, string sNewExtension, bool bRenameCopy)
    {
        System.Configuration.AppSettingsReader oAppSettingsReader = new System.Configuration.AppSettingsReader();
        FileInfo wFileName;

        // 'Get files of directory
        FileInfo[] resultFiles;
        string fileNameDateTime;
        string sDestination;

        DateTime dtCreationDate;
        string sBackDatedFileName;
        DateTime dtWFromDate;


        // NCR_Vision_Regex = (?i)\d{8}_\d{8}.txt
        string sFileRegex = System.Convert.ToString(oAppSettingsReader.GetValue("NCR_Vision_Regex", typeof(System.String)));
        Regex regex = new Regex(sFileRegex);

        try
        {
            dtWFromDate = dtFromDate;
            sBackDatedFileName = dtWFromDate.AddDays(-iBackwardNoOfDays).ToString("yyyyMMdd");

            // ' Delete Files in Target Directory
            resultFiles = oDirInfo.GetFiles();

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

                    // 'Check if files with this type exists - 00000063_20200215.txt
                    // '00000063 is the ATM Number
                    // '20200215 is the YYYYMMDD

                    Match oMatch = regex.Match(resultFile.Name);

                    if (oMatch.Success)
                    {
                        wFileName = resultFile;

                        if (resultFile.Extension == sOldExtension)
                        {
                            fileNameDateTime = "";
                            dtCreationDate = resultFile.CreationTime.Date;

                            string atmName = resultFile.Name.Substring(0, 8);

                            fileNameDateTime = resultFile.Name.Substring(9, 8);


                            if ((Int64.Parse(dtCreationDate.ToString("yyyyMMdd")) >= Int64.Parse(dtFromDate.ToString("yyyyMMdd"))) && (Int64.Parse(dtCreationDate.ToString("yyyyMMdd")) <= Int64.Parse(dtToDate.ToString("yyyyMMdd"))))
                            {
                                sDestination = "";
                                sDestination = sTargetDirectory + @"\" + TWF_PublicFn.PublicFn.fnRight("00000000" + atmName, 8) + "_" + fileNameDateTime + "_EJ_NCR.00" + "0";
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
            sb.AppendLine("ModuleNcrVision(FilesSearchNcrVision) - An exception has occured in the Module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            sErrorMessage = sb.ToString();
            bErrorIndicator = true;
            return;
        }
    }
}
