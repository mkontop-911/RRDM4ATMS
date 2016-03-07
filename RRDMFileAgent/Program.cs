using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Threading;
//
using System.Windows.Forms; // for MessageBox
using System.Diagnostics;
//
using RRDMFileAgentClasses;
using RRDM4ATMs;


namespace RRDMFileAgent
{
    public struct LayoutStruct
    {
        public bool Exists;
        public int SeqNo;
        public string LayoutID;
        public string FieldID;
        public string ColumnName;
        public string FieldType;
        public int StartPos;
        public int Length;
    };

    class Program
    {
        public static RRDMSourceFile rsf = new RRDMSourceFile();


        static void Main(string[] args)
        {
            string Origin;


            string SourceFileID;
            FileSystemWatcher RRDMAgentFileMonitor = new FileSystemWatcher();

            // Read arguments

            if (args.Length != 2)
            {
                if (args.Length == 1)
                {
                    if (args[0] == "ResetPOC")
                    {
                        ResetEnvironmentForPOCDemo();
                        Console.WriteLine("");
                        Console.WriteLine("  RRDM File Agent Environment has been RESET for POC Demo purposes!");
                        Console.WriteLine("");
                        return;
                    }
                }
                string msg = "Invalid number of arguments passed! Exiting...";
                MessageBox.Show(msg, "RRDM File Agent", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Console.WriteLine(msg); Console.ReadKey();
                return;
            }
            else
            {
                Origin = args[0];
                SourceFileID = args[1];
            }

            string m = string.Format("RRDM File Agent     System:{0}  Filetype:{1}", Origin, SourceFileID);
            Console.Title = m;

            // Read the record from from RRDMReconcSourceFile 
            rsf.ReadSourceFileRecord(Origin, SourceFileID);
            if (rsf.ErrorFound)
            {
                string msg = string.Format("Error reading FileID: {0}  of  Origin: {1} from the database! \nThe received error message is: \n{2}", SourceFileID, Origin, rsf.ErrorOutput);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                MessageBox.Show(msg, "RRDM File Agent", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if Source and Archive dirs exist
            if (!Directory.Exists(rsf.SourceDirectory))
            {
                string msg = string.Format("Could not locate the following directory on the disk:\n{0}", rsf.SourceDirectory);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                MessageBox.Show(msg, "RRDM File Agent", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(rsf.ArchiveDirectory))
            {
                string msg = string.Format("Could not locate the following directory on the disk:\n{0}", rsf.ArchiveDirectory);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                MessageBox.Show(msg, "RRDM File Agent", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create the File Watcher
            try
            {
                RRDMAgentFileMonitor = RRDMFileMonitor.CreateFileMonitor(rsf.SourceDirectory, rsf.FileNameMask);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Could not create File Monitor! \nThe received error message is: \n{0}", ex.Message);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                MessageBox.Show(msg, "RRDM File Agent", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            // Set the handler(s)
            RRDMAgentFileMonitor.Created += new FileSystemEventHandler(RRDMAgent_NewFileReceived);

            //Begin monitoring.
            RRDMAgentFileMonitor.EnableRaisingEvents = true;

            Console.WriteLine("");
            Console.WriteLine(string.Format("  Ready to process files masked as  {0}  coming into  {1}", rsf.FileNameMask, rsf.SourceDirectory));
            Console.WriteLine("");
            Console.WriteLine("  Press [ q + ENTER ] or [ CTRL + C ] to end this program....");
            Console.WriteLine("");
            {
                string q = "";
                while (q == "")
                {
                    q = Console.ReadLine();
                    if (q == "q")
                        return;
                    else
                        q = "";
                }
            }
            RRDMAgentFileMonitor.EnableRaisingEvents = false;
            RRDMAgentFileMonitor.Dispose();
        }

        /* ================= End of Main() ============ */


        /// <summary>
        /// RRDMAgent_NewFile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RRDMAgent_NewFileReceived(object sender, System.IO.FileSystemEventArgs e)
        {
            bool retValue;
            string msg;
            int fileSize = 0;
            int LineCount = 0;
            string HASHValue;
            string fileName = e.Name;
            string fileFullName = e.FullPath;
            string Origin = rsf.SystemOfOrigin;
            string Layout = rsf.LayoutID;
            int RFALId = 0;
            string CategoryId = "EWB311"; // ToDo .....


            RRDMAgentLog ral1 = new RRDMAgentLog();
            RRDMReconcCategoriesVsSourcesFiles rcs = new RRDMReconcCategoriesVsSourcesFiles();

            // Console.WriteLine("01\n");

            #region Record the event in the event log ..
            msg = string.Format("New File: {0} from OriginSystem: {1} and LayoutID: {2} was intercepted!", fileName, Origin, Layout);
            EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Information, msg);
            Console.WriteLine(msg);
            #endregion

            // Console.WriteLine("02\n");

            #region Check if the file has already been processed
            // Calculate SHA256 hash value
            HASHValue = FileHASH.BytesToString(FileHASH.GetHashSha256(fileFullName));

            // Console.WriteLine("03\n");

            // Use the HASH to retieve a row from table
            ral1.GetRecordByFileHASH(HASHValue);
            if (ral1.ErrorFound)
            {
                // error reading ... abort
                msg = string.Format("Processing stopped! " +
                                     "\nCould not validate the file HASH value because the database reports:\n{0}", ral1.ErrorOutput);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return;
            }
            else
            {
                if (ral1.RecordFound)
                {
                    // FileHASH already exists ... abort
                    msg = string.Format("Processing stopped! " +
                                         "\nThe file HASH value [{0}] of the intercepted file [{1}] already exists in the database for a file named [{2}].",
                                         HASHValue, fileName, ral1.FileName);
                    EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                    Console.WriteLine(msg);
                    return;
                }
            }
            #endregion

            // Console.WriteLine("04\n");

            #region Check if system is ready to accept the new file
            // Check in the ReconcCategoryVsSourceFiles
            rcs.ReadReconcCategoriesVsSources(CategoryId, rsf.SourceFileID);
            if (rcs.ErrorFound || !rcs.RecordFound)
            {
                // error reading ... abort
                msg = string.Format("Processing stopped! " +
                                     "\nCould not retrieve the corresponding ReconcCategoriesVsSources row for {0},{1}\nThe received message is:\n{2}",
                                     CategoryId, rsf.SourceFileID, rcs.ErrorOutput);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return;
            }
            // Check if ProcessMode is 1. If not ... abort
            if (rcs.ProcessMode != 1)
            {
                msg = string.Format("Processing stopped! " +
                                     "\nCannot continue because the process for {0},{1} has not finished yet!", CategoryId, rsf.SourceFileID);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return;
            }

            // The RCS retrieved will be used to update 'ProcessMode' to -1 at the end of our processing.

            #endregion

            // Console.WriteLine("05\n");

            #region TODO
            // ToDo : Validate, Transform file, ...
            #endregion

            #region Insert a row in the File Agent log table
            // This will be updated with the remaining values afer processing
            // We INSERT now in order to get the FileUID to save as OriginFileName in the corresponding TWtbl---Trans table
            // The FileUID is in the form 'FUID#######' where ####### is the SeqNo padded with zeroes
            // Fill in the column values
            RRDMAgentLog ral = new RRDMAgentLog();
            ral.SystemOfOrigin = rsf.SystemOfOrigin;
            ral.SourceFileID = rsf.SourceFileID;
            ral.FileName = fileName;
            ral.FileSize = fileSize;
            ral.DateTimeReceived = DateTime.Now;
            ral.FileHASH = HASHValue;
            ral.Status = 0; // 
            // ral.ArchivedPath = archFullPath;
            // ral.LineCount = LineCount;
            ral.ArchivedPath = " ";
            ral.LineCount = 0;

            RFALId = ral.Insert(); // Keep for following Update
            if (ral.ErrorFound)
            {
                msg = string.Format("Processing stopped! " +
                                    "An error occured while INSERTING in the ReconcAgentLog table. The error message is : {0}",
                                     ral.ErrorOutput);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return;
            }
            #endregion

            // Console.WriteLine("06\n");

            #region Process Source File

            string FileUID = string.Format("FUID{0}", RFALId.ToString("0000000"));
            LineCount = ProcessSourceFile(Origin, FileUID, Layout, fileFullName);
            if (LineCount == -1)
            {
                // Delete the Agent Log row already inserted
                ral.Delete(RFALId);
                return;
            }
            #endregion

            // Console.WriteLine("07\n");

            #region Update ProcessMode in the ReconccCategoriesVsSources table
            // Use the already retrieved RCS record and update 'ProcessMode' to -1 
            int PrMode = -1;
            int SeqNo = rcs.SeqNo;
            rcs.AgentUpdateReconcCategoryVsSourcesProcessMode(SeqNo, PrMode);
            if (rcs.ErrorFound)
            {
                msg = string.Format("Processing stopped! " +
                                     "\nCould not UPDATE the corresponding ReconccCategoriesVsSources row for {0},{1}\nThe received message is:\n{2}",
                                     CategoryId, rsf.SourceFileID, rcs.ErrorOutput);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return;
            }
            #endregion

            // Console.WriteLine("08\n");


            #region Archive the file and then delete
            // Get the file size
            FileInfo f = new FileInfo(fileFullName);
            fileSize = (int)f.Length;
            // Archive
            string archFullPath = rsf.ArchiveDirectory + "\\" + fileName;
            retValue = ArchiveFile(fileFullName, archFullPath);
            if (retValue)
            {
                // Delete source file
                try
                {
                    File.Delete(fileFullName);
                }
                catch (IOException)
                {
                    //ToDo
                }
            }
            else
            {
                // ToDo: failed to archive
            }
            #endregion

            // Console.WriteLine("09\n");

            #region Update the already inserted AgentLog row in the File Agent log table
            // Fill in the column values
            RRDMAgentLog ral2 = new RRDMAgentLog();
            ral2.SystemOfOrigin = rsf.SystemOfOrigin;
            ral2.SourceFileID = rsf.SourceFileID;
            ral2.FileName = fileName;
            ral2.FileSize = fileSize;
            ral2.DateTimeReceived = DateTime.Now;
            ral2.FileHASH = HASHValue;
            ral2.Status = 1; // 
            ral2.ArchivedPath = archFullPath;
            ral2.LineCount = LineCount;

            ral2.Update(RFALId);
            if (ral2.ErrorFound)
            {
                msg = string.Format("Processing stopped! " +
                                    "An error occured while INSERTING in the ReconcAgentLog table. The error message is : {0}",
                                     ral2.ErrorOutput);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return;
            }
            #endregion

            #region Finish up by writing an event in the Event Log
            msg = string.Format("Finished processing file: {0} from OriginSystem: {1}!", fileName, Origin);
            EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Information, msg);
            Console.WriteLine(msg);
            #endregion
        }

        /* ======== End of RRDMAgent_NewFile() ======= */

        /// <summary>
        /// Backup the File (Save a copy)
        /// </summary>
        /// <param name="srcFullPath"></param>
        /// <param name="archFullPath"></param>
        /// <returns>bool</returns>
        private static bool ArchiveFile(string srcFullPath, string archFullPath)
        {
            string msg = "";
            bool successArchive = true;

            try
            {
                File.Copy(srcFullPath, archFullPath);
            }
            catch (UnauthorizedAccessException ex)
            {
                successArchive = false;
                msg = string.Format("Error copying file {0} to {1} due to lack of required permissions!\nThe exact error message is:\n{2}", srcFullPath, archFullPath, ex.Message);
            }
            catch (IOException ex)
            {
                successArchive = false;
                msg = string.Format("I/O exception while copying file {0} to {1}!\nThe exception message is: \n{2}", srcFullPath, archFullPath, ex.Message);
            }
            catch (Exception ex)
            {
                successArchive = false;
                msg = string.Format("An exception occured while copying file {0} to {1}! \nThe exception message is: \n{2}", srcFullPath, archFullPath, ex.Message);
            }


            if (successArchive == true)
            {
                successArchive = true;
                msg = string.Format("File: {0} copied successfully to {1}!", srcFullPath, archFullPath);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Information, msg);
            }
            else
            {
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
            }
            return (successArchive);
        }

        private static bool ValidateFile(string FileName)
        {
            return (true);
        }


        /// <summary>
        /// Process source File and return number of Lines processed..
        /// </summary>
        /// <param name="Origin"></param>
        /// <param name="FileUID"></param>
        /// <param name="Layout"></param>
        /// <param name="fullFileName"></param>
        /// <returns>-1:Error, #:LineCount</returns>
        private static int ProcessSourceFile(string Origin, string FileUID, string Layout, string fullFileName)
        {
            string msg;
            string Line;
            int LineCounter = 0;
            DataTable LayoutTbl;


            // Read the File Layout record (returns a table, one row per field)
            ReconcSourceFileLayout sfl = new ReconcSourceFileLayout();
            sfl.GetLayoutTable(Layout);
            if (sfl.ErrorFound)
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError reading Layout table for {0}! \nThe received message is: \n{1}", Layout, sfl.ErrorOutput);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);

                return (-1);
            }
            LayoutTbl = sfl.LayoutTable;


            // Trancate the destination table
            ReconcWorkingTable rwt = new ReconcWorkingTable();
            rwt.TruncateTable(rsf.InitialTableName);


            // Read the file and process line by line. 
            // ToDo: re-write for bulk-INSERT
            try
            { 
            System.IO.StreamReader file = new System.IO.StreamReader(fullFileName);
            while ((Line = file.ReadLine()) != null)
            {
                bool ret = ProcessLine(Origin, FileUID, LineCounter + 1, Line, LayoutTbl);

                LineCounter++;
            }
            file.Close();
            }
            catch (Exception ex)
            {
                msg = string.Format("Exception System.IO! \n{0}", ex.Message);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
            }

            /*
            string msgtxt = string.Format("Lines read: {0}, File: {1}", counter, e.FullPath);
            Logging.WriteEventLog("RRDMAgent", EventLogEntryType.Information, msgtxt);
            */

            return (LineCounter);
        }
        /* ======= End of ProcessSourceFile() ================= */

        public static bool ProcessLine(string Origin, string FileUID, int LineNumber, string Line, DataTable LayoutTbl)
        {
            string msg;
            string expression;
            string extractedValue;
            DateTime dt;
            int i;
            decimal dec;

            DataRow[] LayoutRow;
            LayoutStruct FieldAttrib;

            ReconcWorkingTable WorkTbl = new ReconcWorkingTable();
            
            // Part of Header
            WorkTbl.OriginFileName = FileUID;
            WorkTbl.OriginalRecordId = LineNumber;

            // Get from input Line
            WorkTbl.TerminalId = "";
            WorkTbl.TransType = 0;
            WorkTbl.TransDescr = "";
            WorkTbl.CardNumber = "";
            WorkTbl.AccNumber = "";
            WorkTbl.TransCurr = "";
            WorkTbl.TransAmount = 0; // 
            WorkTbl.AtmTraceNo = 0;
            WorkTbl.RRNumber = 0;
            WorkTbl.ResponseCode = 0;
            WorkTbl.T24RefNumber = "";
            WorkTbl.OpenRecord = true;
            WorkTbl.Operator = "";

            // TODO 
            // For now, get from input Line
            WorkTbl.Origin = "";
            WorkTbl.RMCateg = "";
            WorkTbl.TransTypeAtOrigin = "";
            WorkTbl.Product = "";

            // TODO 
            // For now, fytefta!!
            WorkTbl.CostCentre = "Cost Center 402";
            WorkTbl.RMCycle = 0;
            // WorkTbl.UniqueRecordId = 1; // KONTO will calculate


            #region ToDo: generalize using --> FieldAttrib.FieldType
            /*
            switch (FieldAttrib.FieldType)
            {
                case "CHAR":
                    {

                        break;
                    }
                default:
                    {
                        // Error
                        break;
                    }
            }
            */
            #endregion

            #region Extract: TerminalID (CHAR)
            //          which is CHAR
            // Get the field attributes
            expression = "ColumnName = 'TerminalId'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            WorkTbl.TerminalId = extractedValue;
            #endregion

            #region Extract: TransType (INT)
            //          which is INT
            // Get the field attributes
            expression = "ColumnName = 'TransType'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            if (Int32.TryParse(extractedValue, out i))  // TODO exceptions
            {
                WorkTbl.TransType = i;
            }
            else
            {
                // TODO
                WorkTbl.TransType = 0;
            }
            #endregion

            #region Extract: TransDescr
            //          which is CHAR
            // Get the field attributes
            expression = "ColumnName = 'TransDescr'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            WorkTbl.TransDescr = extractedValue;
            #endregion

            #region Extract: TransTypeAtOrigin
            //          which is CHAR
            // Get the field attributes
            expression = "ColumnName = 'TransTypeAtOrigin'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            WorkTbl.TransTypeAtOrigin = extractedValue;
            #endregion

            #region Extract: CardNumber
            //          which is CHAR
            // Get the field attributes
            expression = "ColumnName = 'CardNumber'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            WorkTbl.CardNumber = extractedValue;
            #endregion

            #region Extract: AccNumber
            //          which is CHAR
            // Get the field attributes
            expression = "ColumnName = 'AccNumber'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            WorkTbl.AccNumber = extractedValue;
            #endregion

            #region Extract: TransCurr
            //          which is CHAR
            // Get the field attributes
            expression = "ColumnName = 'TransCurr'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            WorkTbl.TransCurr = extractedValue;
            #endregion

            #region Extract: TransAmount (DEC)
            //          which is DEC
            // Get the field attributes
            expression = "ColumnName = 'TransAmount'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            if (Decimal.TryParse(extractedValue, out dec))
            {
                WorkTbl.TransAmount = dec;
            }
            else
            {
                // TODO
                WorkTbl.TransAmount = 0;
            }
            #endregion

            #region Extract: TransDate (DATETIME)
            //          which is DATE
            // Get the field attributes
            expression = "ColumnName = 'TransDate'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            if (DateTime.TryParse(extractedValue, out dt))
            {
                WorkTbl.TransDate = dt;
            }
            else
            {
                // TODO
            }
            #endregion

            #region Extract: ATMTraceNo (INT)
            //          which is INT
            // Get the field attributes
            expression = "ColumnName = 'ATMTraceNo'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            if (Int32.TryParse(extractedValue, out i))
            {
                WorkTbl.AtmTraceNo = i;
            }
            else
            {
                // TODO
                WorkTbl.AtmTraceNo = 0;
            }
            #endregion

            #region Extract: RRNumber (INT)
            //          which is INT
            // Get the field attributes
            expression = "ColumnName = 'RRNumber'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            if (Int32.TryParse(extractedValue, out i))
            {
                WorkTbl.RRNumber = i;
            }
            else
            {
                // TODO
                WorkTbl.RRNumber = 0;
            }
            #endregion

            #region Extract: ResponseCode (INT)
            //          which is INT
            // Get the field attributes
            expression = "ColumnName = 'ResponseCode'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            if (Int32.TryParse(extractedValue, out i))
            {
                WorkTbl.ResponseCode = i;
            }
            else
            {
                // TODO
                WorkTbl.ResponseCode = 0;
            }
            #endregion

            #region Extract: T24RefNumber (CHAR)
            //          which is CHAR
            // Get the field attributes
            expression = "ColumnName = 'T24RefNumber'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            WorkTbl.T24RefNumber = extractedValue;
            #endregion

            #region Extract: RMCateg ( TODO )
            //          which is CHAR
            // Get the field attributes
            expression = "ColumnName = 'RMCateg'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            WorkTbl.RMCateg = extractedValue;
            #endregion

            #region Extract: Product ( TODO - currntly from CardNo(1-6))
            //          which is CHAR
            // Get the field attributes
            // expression = "ColumnName = 'Product'";
            expression = "ColumnName = 'CardNumber'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, 6); // fixed length of 6
            // TODO -- derive for source??
            switch (extractedValue)
            {
                case "437507":
                    {
                        WorkTbl.Product = "Debit Card";
                        break;
                    }
                case "457556":
                    {
                        WorkTbl.Product = "Prepaid Card";
                        break;
                    }
                case "450177":
                    {
                        WorkTbl.Product = "Gift Card";
                        break;
                    }
                case "457557":
                    {
                        WorkTbl.Product = "Travel Money Card";
                        break;
                    }
                default:
                    {
                        WorkTbl.Product = extractedValue;
                        break;
                    }
            }
            #endregion

            #region Extract: Origin
            //          which is CHAR
            // Get the field attributes
            expression = "ColumnName = 'Origin'";
            LayoutRow = LayoutTbl.Select(expression);
            FieldAttrib = GetFieldLayoutElements(LayoutRow);
            if (LayoutRow.Length != 1 || (!FieldAttrib.Exists))
            {
                msg = string.Format("Processing stopped! " +
                                     "\nError in reading the field attributes from the Source File Layout!\nThe Expression that failed is: '{0}'", expression);
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, msg);
                Console.WriteLine(msg);
                return (false);
            }
            extractedValue = Line.Substring(FieldAttrib.StartPos - 1, FieldAttrib.Length);
            WorkTbl.Origin = extractedValue;
            #endregion

            #region INSERT into the table
            WorkTbl.InsertSingle(rsf.InitialTableName);
            if (WorkTbl.ErrorFound)
            {
                EventLogging.WriteEventLog("RRDMAgent", EventLogEntryType.Error, WorkTbl.ErrorOutput);
                Console.WriteLine(WorkTbl.ErrorOutput);
                return (false);
            }
            #endregion

            return (true);
        }

        public static LayoutStruct GetFieldLayoutElements(DataRow[] LayoutRow)
        {
            LayoutStruct Elmnt = new LayoutStruct();
            try
            {
                Elmnt.LayoutID = LayoutRow[0]["LayoutID"].ToString();
                Elmnt.FieldID = LayoutRow[0]["FieldID"].ToString();
                Elmnt.ColumnName = LayoutRow[0]["ColumnName"].ToString(); // not really necessary
                Elmnt.StartPos = (int)LayoutRow[0]["StartPos"];
                Elmnt.Length = (int)LayoutRow[0]["Length"];
                Elmnt.FieldType = LayoutRow[0]["FieldType"].ToString();
            }
            catch 
            {
                Elmnt.Exists = false;
                Elmnt.LayoutID = "";
                Elmnt.FieldID = "";
                Elmnt.ColumnName = "";
                Elmnt.StartPos = 0;
                Elmnt.Length = 0;
                Elmnt.FieldType = "";
                return (Elmnt);
            }
            Elmnt.Exists = true;
            return (Elmnt);
        }


        public static void ResetEnvironmentForPOCDemo()
        {
            string msg;
            RRDMReconcCategoriesVsSourcesFiles rcs = new RRDMReconcCategoriesVsSourcesFiles();

            // Reset ProcessMode flas. Set to 1
            rcs.AgentReconcCategoryVsSourcesReset();
            if (rcs.ErrorFound)
            {
                msg = string.Format("An error occured while RESETing the ProcessMode flag.\nThe error message is : \n{0}",
                                     rcs.ErrorOutput);
                Console.WriteLine(msg);
                return;
            }


            // Truncate RecocFileAgenLog table
            RRDMAgentLog ral = new RRDMAgentLog();
            ral.TruncateTable();
            if (ral.ErrorFound)
            {
                msg = string.Format("An error occured while TRUNCATING the ReconcAgentLog table. \nThe error message is : \n{0}",
                                     ral.ErrorOutput);
                Console.WriteLine(msg);
                return;
            }

        }
    }
}
