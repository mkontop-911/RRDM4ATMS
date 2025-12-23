using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

static class ModNCR
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

    public static void ProcessNcrDirectory(IConfiguration config)
    {

        ErrorMessage = string.Empty;
        ErrorIndicator = false;

        try
        {
            string ncrPath = config["NCR_Path"];
            string ncrExtension = config["NCR_Extension"];
            string newExtension = config["New_Extension"];

            string sRenameCopy = config["RenameCopy"];
            int iNoOfBackwardMonths = int.Parse(config["BackwardNoOfMonths"]);

            string sExtension = "";

            bool bRenameCopy = (sRenameCopy == "True");
            string sWorkingPath = string.Empty;


            DateTime dtWFromDate;
            DateTime dtWToDate;

            DateTime dtMaxReturnDate;


            dtWFromDate = ReferenceDate.AddMonths(-iNoOfBackwardMonths);
            dtWToDate = DateTime.Now;

            dtMaxReturnDate = new DateTime(2000, 1, 1);

            string sRangeFromDir = System.Convert.ToString(dtWFromDate.Year) + RRDM4ATMs.PublicFn.fnRight(("00" + System.Convert.ToString(dtWFromDate.Month)), 2);
            string sRangeToDir = System.Convert.ToString(dtWToDate.Year) + RRDM4ATMs.PublicFn.fnRight(("00" + System.Convert.ToString(dtWToDate.Month)), 2);

            // 'Find the main directory
            // Dim di As DirectoryInfo = New DirectoryInfo(path & "\")
            sWorkingPath = SourceDirectory;
            DirectoryInfo di = new DirectoryInfo(SourceDirectory + @"\");


            // 'Find if exist main directory
            if (System.IO.Directory.Exists(SourceDirectory + @"\"))
            {
                if (!System.IO.Directory.Exists(TargetDirectory + @"\"))
                {
                    ErrorMessage = "ModuleNCR(ProcessNcrDirectory) - Target Directory not found.";
                    ErrorIndicator = true;
                    return;
                }

                sExtension = ncrExtension;

                NoOfFiles = 0;

                // ' Directory Structure is as follows \Root\Atm\Year\mmDDMMSS
                // ' Find the First level Directory - Atm

                foreach (string sDirAtm in System.IO.Directory.GetDirectories(SourceDirectory + @"\"))
                {
                    System.IO.DirectoryInfo dirAtmInfo = new System.IO.DirectoryInfo(sDirAtm);

                    string sAtmName = dirAtmInfo.Name;
                    sWorkingPath = SourceDirectory + @"\" + sAtmName + @"\";
                    // 'Find the Year Level Directory
                    foreach (string sDirAtmInfo in System.IO.Directory.GetDirectories(sWorkingPath))
                    {
                        System.IO.DirectoryInfo dirYearInfo = new System.IO.DirectoryInfo(sDirAtmInfo);
                        string sAtmYear = dirYearInfo.Name;

                        if ((Int32.Parse(sAtmYear) >= dtWFromDate.Year) && (Int32.Parse(sAtmYear) <= dtWToDate.Year))
                        {
                            sWorkingPath = SourceDirectory + @"\" + sAtmName + @"\" + sAtmYear + @"\";
                            // Find the files in directory & Process them
                            FilesSearchNcr(dirYearInfo, sAtmName, sAtmYear, sExtension, newExtension, bRenameCopy, config);
                        }
                    }
                }
            }
            else // ' Directory not Found
            {
                ErrorMessage = "ModuleNCR(ProcessNcrDirectory) - Source Directory not found. The Operation is aborted.";
                ErrorIndicator = true;
                return;
            }
        }

        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleNCR(ProcessNcrDirectory) - An exception has occured in the module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            ErrorMessage = sb.ToString();
            ErrorIndicator = true;
            return;
        }
        finally
        {
        }
    }

    private static void FilesSearchNcr(DirectoryInfo oDirInfo, string sAtmLevel, string sYearLevel, string sOldExtension, string sNewExtension, bool bRenameCopy, IConfiguration config)
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
            DateTime dtFileCreationDateTime;



            string sFileRegex = config["NCR_Regex"];
            Regex regex = new Regex(sFileRegex);

            dtWFromDate = ReferenceDate;
            sBackDatedFileName = dtWFromDate.AddDays(-BackwardNoOfDays).ToString("yyyyMMdd");
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
                            dtFileCreationDateTime = resultFile.CreationTime;

                            // If Filename lies between the range specified by the User OR
                            // Filename is greater than the Backdated File name and the Creation Date is equal to 
                            // the Current Date then process the File

                            if (dtFileCreationDateTime > ReferenceDate)

                            {
                                if (sSaveDate == fileNameDateTime)
                                    iExtensionCounter++;
                                else
                                    iExtensionCounter = 0;

                                sSaveDate = fileNameDateTime;

                                sDestination = TargetDirectory + @"\" + RRDM4ATMs.PublicFn.fnRight(("00000000" + sAtmLevel), 8) + "_" + sYearLevel + resultFile.Name.Substring(0, 4) + "_EJ_NCR." + iExtensionCounter.ToString("000");

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
            sb.AppendLine("ModuleNCR(FilesSearchNcr) - An exception has occured in the Module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            ErrorMessage = sb.ToString();
            ErrorIndicator = true;
            return;
        }
    }
}

