using System;
using System.IO;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

static class ModNcrVision
{
    public static DateTime ReferenceDate;
    public static DateTime MaxReturnDate;
    public static string SourceDirectory;
    public static string TargetDirectory;
    public static int NoOfFiles;
    public static int BackwardNoOfDays;
    public static int ForwardNoOfDays;
    public static string ErrorMessage;
    public static string FilesNotProcessed;
    public static bool ErrorIndicator;

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
            DateTime dtMaxReturnDate;


            dtWFromDate = ReferenceDate.AddMonths(-iNoOfBackwardMonths);
            dtWToDate = DateTime.Now;

            dtMaxReturnDate = new DateTime(2000, 1, 1);


            string sRangeFromDir = System.Convert.ToString(dtWFromDate.Year) + RRDM4ATMs.PublicFn.fnRight(("00" + System.Convert.ToString(dtWFromDate.Month)), 2);
            string sRangeToDir = System.Convert.ToString(dtWToDate.Year) + RRDM4ATMs.PublicFn.fnRight(("00" + System.Convert.ToString(dtWToDate.Month)), 2);

            // 'Find the main directory
            sPath = SourceDirectory;
            sWorkingPath = sPath;
            DirectoryInfo di = new DirectoryInfo(sPath + @"\");


            // 'Find if exist main directory
            if (System.IO.Directory.Exists(sPath + @"\"))
            {
                if (!System.IO.Directory.Exists(TargetDirectory + @"\"))
                {
                    ErrorMessage = "ModuleNcrVision(ProcessNcrVisionDirectory) - Target Directory not found.";
                    ErrorIndicator = true;
                    return;
                }

                sExtension = ncrNewExtension;

                NoOfFiles = 0;

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
                ErrorMessage = "ModuleNcrVision(ProcessNcrVisionDirectory) - Source Directory not found. The Operation is aborted.";
                ErrorIndicator = true;
                return;
            }
        }

        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleNcrVision(ProcessNcrVisionDirectory) - An exception has occured in the module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            ErrorMessage = sb.ToString();
            ErrorIndicator = true;
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

        DateTime dtFileCreationDateTime;
        string sBackDatedFileName;
        DateTime dtWFromDate;


        // NCR_Vision_Regex = (?i)\d{8}_\d{8}.txt
        string sFileRegex = System.Convert.ToString(oAppSettingsReader.GetValue("NCR_Vision_Regex", typeof(System.String)));
        Regex regex = new Regex(sFileRegex);

        try
        {
            dtWFromDate = ReferenceDate;
            sBackDatedFileName = dtWFromDate.AddDays(-BackwardNoOfDays).ToString("yyyyMMdd");

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
                            dtFileCreationDateTime = resultFile.CreationTime;

                            string atmName = resultFile.Name.Substring(0, 8);

                            fileNameDateTime = resultFile.Name.Substring(9, 8);


                            if (dtFileCreationDateTime > ReferenceDate)
                            {
                                sDestination = "";
                                sDestination = TargetDirectory + @"\" + RRDM4ATMs.PublicFn.fnRight("00000000" + atmName, 8) + "_" + fileNameDateTime + "_EJ_NCR.00" + "0";
                                // 'Rename File
                                if (bRenameCopy == false)
                                    File.Move(resultFile.FullName, sDestination);
                                else
                                    File.Copy(resultFile.FullName, sDestination, true);

                                NoOfFiles++;
                                MaxReturnDate = ((MaxReturnDate > dtFileCreationDateTime)) ? MaxReturnDate : dtFileCreationDateTime;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine(FilesNotProcessed);
                    sb.Append("File In Error..:");
                    sb.AppendLine(resultFile.FullName.ToString());
                    FilesNotProcessed = sb.ToString();
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
            ErrorMessage = sb.ToString();
            ErrorIndicator = true;
            return;
        }
    }
}
