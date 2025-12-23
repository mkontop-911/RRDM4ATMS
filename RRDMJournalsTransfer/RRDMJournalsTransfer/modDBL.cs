using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

static class ModDBL
{
    //public static DateTime dtFromDate;
    //public static DateTime dtToDate;
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

    public static void ProcessDblDirectory(IConfiguration config)
    {

        // ' The Structure of the Directory will be as follows
        // ' \Root\Atm\Files.DAT

        FilesNotProcessed = string.Empty;

        try
        {
            string dblExtension = config["DBL_Extension"];
            string sNewExtension = config["New_Extension"];
            string sRenameCopy = config["RenameCopy"];
            int iNoOfBackwardMonths = int.Parse(config["BackwardNoOfMonths"]);

            bool bRenameCopy = (sRenameCopy == "True");
            ErrorMessage = string.Empty;
            ErrorIndicator = false;

            string sPath;

            string sExtension = string.Empty;

            // 'Find the main directory
            // Dim di As DirectoryInfo = New DirectoryInfo(path & "\")
            sPath = SourceDirectory;
            DirectoryInfo di = new DirectoryInfo(sPath + @"\");

            // 'Find if main directory exists
            if (System.IO.Directory.Exists(sPath + @"\"))
            {
                if (!System.IO.Directory.Exists(TargetDirectory + @"\"))
                {
                    ErrorMessage = "ModuleDBL(ProcessDblDirectory) - Target Directory not found.";
                    ErrorIndicator = true;
                    return;
                }

                sExtension = dblExtension;

                NoOfFiles = 0;

                // ' Find the First level Directory - Atm

                foreach (string sDirAtm in System.IO.Directory.GetDirectories(sPath + @"\"))
                {
                    System.IO.DirectoryInfo oDirAtmInfo = new System.IO.DirectoryInfo(sDirAtm);
                    string sAtmName = oDirAtmInfo.Name;

                    if (((("" + sAtmName).Trim()).Length) <= 7)
                    {
                        // 'Process the Files in the ATM Directory
                        sAtmName = sAtmName.Replace("-", "");
                        FilesSearchDbl(oDirAtmInfo, sAtmName, sExtension, sNewExtension, bRenameCopy, config);
                    }
                }
            }
            else // ' Source Directory not Found
            {
                ErrorMessage = "ModuleDBL(ProcessDblDirectory) - Source Directory not found. The Operation is aborted.";
                ErrorIndicator = true;
                return;
            }

        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleDBL(ProcessDblDirectory) - An exception has occured in the Module. The Operation is aborted.");
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

    private static void FilesSearchDbl(DirectoryInfo oDirInfo, string sAtmName, string sOldExtension, string sNewExtension, bool bRenameCopy, IConfiguration config)
    {

        // 'Get files of directory
        FileInfo[] resultFiles;
        string fileNameDateTime;
        string sDestination;

        DateTime dtFileCreationDateTime;
        string sBackDatedFileName;
        DateTime dtWFromDate;

        string sFileRegex = config["DBL_Regex"];
        Regex regex = new Regex(sFileRegex);
        try
        {
            dtWFromDate = ReferenceDate;

            //Assign Min Value to the Max Date
            MaxReturnDate = new DateTime(2000, 1, 1);

            sBackDatedFileName = (dtWFromDate.AddDays(-BackwardNoOfDays)).ToString("yyyyMMdd");

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
                            dtFileCreationDateTime = resultFile.CreationTime;
                            string[] atmParts = resultFile.Name.Split((new char[] { '_' }));

                            if ((atmParts.Length >= 1))
                                fileNameDateTime = "20" + atmParts[1].Substring(4, 2) + atmParts[1].Substring(2, 2) + atmParts[1].Substring(0, 2);
                            else
                                fileNameDateTime = "20000000";


                            if (dtFileCreationDateTime > ReferenceDate)
                            {

                                MaxReturnDate = ((MaxReturnDate > dtFileCreationDateTime)) ? MaxReturnDate : dtFileCreationDateTime;


                                sDestination = "";
                                sDestination = TargetDirectory + @"\" + RRDM4ATMs.PublicFn.fnRight(("00000000" + sAtmName), 8) + "_" + fileNameDateTime + "_EJ_DBL.00" + resultFile.Name.Substring(10, 1);

                                // 'Rename File
                                if (bRenameCopy == false)
                                    File.Move(resultFile.FullName, sDestination);
                                else
                                    File.Copy(resultFile.FullName, sDestination, true);

                                NoOfFiles++;
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
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleDBL(FilesSearchDbl) - An exception has occured in the Module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            ErrorMessage = sb.ToString();
            ErrorIndicator = true;
            return;
        }
    }

}


