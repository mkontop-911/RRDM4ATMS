using System;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.Extensions.Configuration;

static class ModOTH
{

    public static DateTime ReferenceDate;
    public static DateTime MaxReturnDate;
    public static string SourceDirectory;
    public static string TargetDirectory;
    public static int NoOfFiles;
    public static int BackwardNoOfDays;
    public static int ForwardNoOfDays;
    public static string ErrorMessage;
    public static string FilesNotProcessed = string.Empty;
    public static bool ErrorIndicator;

    public static void ProcessOthDirectory(IConfiguration config)
    {
        // ' The Structure of the Directory will be as follows
        // ' \Root\Files.DAT
        try
        {
            string sOthExtension = config["OTH_Extension"];
            // Original code read OTH_Extension into sNewExtension as well, preserving behavior (though logic seems unused)
            string sNewExtension = config["OTH_Extension"]; 
            string sRenameCopy = config["RenameCopy"];


            string sPath;
            string sWorkingPath;
            string sExtension = "";

            bool bRenameCopy = (sRenameCopy == "True");

            // 'Find the main directory
            // Dim di As DirectoryInfo = New DirectoryInfo(path & "\")
            sPath = SourceDirectory;
            sWorkingPath = sPath;
            DirectoryInfo di = new DirectoryInfo(sPath + @"\");


            // 'Find if exist main directory
            if (System.IO.Directory.Exists(sPath + @"\"))
            {
                if (!System.IO.Directory.Exists(TargetDirectory + @"\"))
                {
                    ErrorMessage = "ModuleOTH(ProcessOthDirectory) - Target Directory not found.";
                    ErrorIndicator = true;
                    return;
                }
                //else if (!RRDM4ATMs.PublicFn.DeleteFiles(TargetDirectory))
                //{
                //    ErrorMessage = "ModuleOTH(ProcessOthDirectory) - The Files in the Target Directory could not be deleted. The Operation is aborted.";
                //    ErrorIndicator = true;
                //    return;
                //}
                sExtension = sOthExtension;
                NoOfFiles = 0;
                // ' Find the First level Directory - Atm
                System.IO.DirectoryInfo dirOthInfo = new System.IO.DirectoryInfo(sPath + @"\");
                FilesSearchOther(dirOthInfo, sExtension, sNewExtension, bRenameCopy);
            }
            else // ' Directory not Found
            {
                ErrorMessage = "ModuleOTH(ProcessOthDirectory) - Source Directory not found. The Operation is aborted.";
                ErrorIndicator = true;
                return;
            }
        }

        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleOTH(ProcessOthDirectory) - An exception has occured in the module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            ErrorMessage = sb.ToString();
            ErrorIndicator = true;
        }
        finally
        {
        }
    }

    private static void FilesSearchOther(DirectoryInfo dirInfo, string oldExtension, string newExtension, bool renameCopy)
    {
        try
        {

            // 'Get files of directory
            FileInfo[] resultFiles;
            string sDestination;
            DateTime dtFileCreationDateTime;


            // ' Delete Files in Target Directory
            resultFiles = dirInfo.GetFiles();

            // 'Find the files .000
            foreach (FileInfo resultFile in resultFiles)
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

                // ' Move the File as it is. No conversion
                dtFileCreationDateTime = resultFile.CreationTime;
                if (dtFileCreationDateTime > ReferenceDate)
                {
                    sDestination = "";
                    sDestination = TargetDirectory + @"\" + resultFile.Name;
                    // 'Rename File
                    // 'If RenameCopy = False Then
                    File.Move(resultFile.FullName, sDestination);
                    // 'Else ''Copy File
                    // 'File.Copy(resultFile.FullName, Destination, True)
                    // 'End If

                    NoOfFiles++;
                    MaxReturnDate = ((MaxReturnDate > dtFileCreationDateTime)) ? MaxReturnDate : dtFileCreationDateTime;
                }
            }
        }

        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleOTH(FilesSearchOther) - An exception has occured in the Module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            ErrorMessage = sb.ToString();
            ErrorIndicator = true;
            return;
        }
    }
}
