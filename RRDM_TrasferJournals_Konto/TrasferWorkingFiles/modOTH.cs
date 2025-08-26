using System;
using System.IO;
using Microsoft.VisualBasic;

static class modOTH
{
    public static DateTime dtFromDate;
    public static DateTime dtToDate;
    public static string sSourceDirectory;
    public static string sTargetDirectory;
    public static int iNoOfFiles;
    public static int iBackwardNoOfDays;
    public static int iForwardNoOfDays;
    public static string sErrorMessage;
    public static string sFilesNotProcessed = string.Empty;
    public static bool bErrorIndicator;

    public static void ProcessOthDirectory()
    {
        // ' The Structure of the Directory will be as follows
        // ' \Root\Files.DAT
        try
        {
            System.Configuration.AppSettingsReader appSettingsReaderX = new System.Configuration.AppSettingsReader();

            string sOthExtension = System.Convert.ToString(appSettingsReaderX.GetValue("OTH_Extension", typeof(System.String)));
            string sNewExtension = System.Convert.ToString(appSettingsReaderX.GetValue("OTH_Extension", typeof(System.String)));
            string sRenameCopy = System.Convert.ToString(appSettingsReaderX.GetValue("RenameCopy", typeof(System.String)));


            string sPath;
            string sWorkingPath;
            string sExtension = "";

            bool bRenameCopy = (sRenameCopy == "True");

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
                    sErrorMessage = "ModuleOTH(ProcessOthDirectory) - Target Directory not found.";
                    bErrorIndicator = true;
                    return;
                }
                else if (!TWF_PublicFn.PublicFn.DeleteFiles(sTargetDirectory))
                {
                    sErrorMessage = "ModuleOTH(ProcessOthDirectory) - The Files in the Target Directory could not be deleted. The Operation is aborted.";
                    bErrorIndicator = true;
                    return;
                }
                sExtension = sOthExtension;
                iNoOfFiles = 0;
                // ' Find the First level Directory - Atm
                System.IO.DirectoryInfo dirOthInfo = new System.IO.DirectoryInfo(sPath + @"\");
                FilesSearchOther(dirOthInfo, sExtension, sNewExtension, bRenameCopy);
            }
            else // ' Directory not Found
            {
                sErrorMessage = "ModuleOTH(ProcessOthDirectory) - Source Directory not found. The Operation is aborted.";
                bErrorIndicator = true;
                return;
            }
        }

        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleOTH(ProcessOthDirectory) - An exception has occured in the module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            sErrorMessage = sb.ToString();
            bErrorIndicator = true;
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
                sDestination = "";
                sDestination = sTargetDirectory + @"\" + resultFile.Name;
                // 'Rename File
                // 'If RenameCopy = False Then
                File.Move(resultFile.FullName, sDestination);
                // 'Else ''Copy File
                // 'File.Copy(resultFile.FullName, Destination, True)
                // 'End If

                iNoOfFiles++;
            }
        }

        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ModuleOTH(FilesSearchOther) - An exception has occured in the Module. The Operation is aborted.");
            sb.AppendLine("Exception Details:");
            sb.AppendLine(ex.Message);
            sErrorMessage = sb.ToString();
            bErrorIndicator = true;
            return;
        }
    }
}
