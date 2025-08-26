using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using RRDM4ATMs;
using System.Collections.Generic;
using System.Linq;

namespace RRDMRFMClasses
{
    public class ProcessSourceFile_Core_Banking_T24
    {
        #region Enumerations and structures
        public enum ProcessFileResult
        {
            Success,
            IsDuplicate,
            ReconciliationInProgress,
            ProcessingError,
            SQL_Error,
            IO_Error,
            IO_Error_SourceFleMove,
            IO_Error_ArchiveFile,
            RawImportError
        }
        #endregion

        #region Fields and properties
        private bool _IsSuccess;
        public bool IsSuccess { get { return _IsSuccess; } }

        private string _Origin;
        public string Origin { get { return _Origin; } }

        private string _SourceFileID;
        public string SourceFileID { get { return _SourceFileID; } }

        private string _Operator;
        public string Operator { get { return _Operator; } }

        private string _FileUID;
        public string FileUID { get { return _FileUID; } }

        private string _sourceFilePath;
        public string sourceFilePath { get { return _sourceFilePath; } }

        private string _importTable;
        public string importTable { get { return _importTable; } }

        private int _LinesProcessed;
        public int LinesProcessed { get { return _LinesProcessed; } }

        private string _CategoryId;
        public string CategoryId { get { return _CategoryId; } }

        private DataTable _BINsToCategories;
        public DataTable BINsToCategories { get { return _BINsToCategories; } }

        private DataTable _InMemDataTbl_UNIV;
        public DataTable InMemDataTbl_UNIV { get { return _InMemDataTbl_UNIV; } }

        private DataTable _InMemDataTbl_RAW;
        public DataTable InMemDataTbl_RAW { get { return _InMemDataTbl_RAW; } }
        #endregion

        #region Process Source File
        /// <summary>
        /// Process source File and return number of Lines processed..
        /// </summary>
        /// <param name="Origin"></param>
        /// <param name="FileUID"></param>
        /// <param name="Layout"></param>
        /// <param name="fullFileName"></param>
        /// <returns>-1:Error, #:LineCount</returns>
        public ProcessSourceFile_Core_Banking_T24(string argOperator, string argOrigin, string argFileUID,
                                                  string argSrcFileID, string fullFileName,
                                                  int LinesInHeadrer, int LinesInTrailer,
                                                  string argImportTable, DataTable AtmsTbl, int MaxPagingRows)
        {
            DataTable LayoutTbl;
            //RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();
            //// ********INSERT BY PANICOS**************
            //DateTime BeforeCallDtTime = DateTime.Now;
            //// *************************BY PANICOS********************************
            _Operator = argOperator;
            _Origin = argOrigin;
            _FileUID = argFileUID;
            _SourceFileID = argSrcFileID;
            _importTable = argImportTable;
            _sourceFilePath = fullFileName;

            #region Prepare for Matching Categories
            // Get the TableMatchingCategoriesVsBINs table 
            RRDMMatchingCategoriesVsBINs mCB = new RRDMMatchingCategoriesVsBINs();

            mCB.ReadReconcMatchingFieldsToFillDataTable_AllCategories(argOperator, "Our Atms"); // Selection Criteria?
            if (!mCB.ErrorFound)
            {
                if (mCB.RecordFound == true)
                {
                    _BINsToCategories = mCB.TableMatchingCategoriesVsBINs;
                }
                else
                {
                    _BINsToCategories = null;
                    //string msg = string.Format("Could not retrieve MatchingCategoriesVsBINs\nThe error reads:\n{0}", Mc.ErrorOutput);
                    //DisplayEventMsg(msg, EventLogEntryType.Error);
                    //return;
                }
            }
            else
            {
                // Database error.. abort
                string msg = string.Format("Database error while reading 'MatchingCategoriesVsBINs'.\nThe error reads:\n{0}", mCB.ErrorOutput);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                _LinesProcessed = -1;
                _IsSuccess = false;
                return;
            }
            // Get the Category ID to use when CardNumber is not present
            RRDMMatchingCategories mC = new RRDMMatchingCategories();
            mC.ReadMatchingCategorybyGetsAllOtherBINS(argOperator);
            if (mC.RecordFound)
            {
                _CategoryId = mC.CategoryId;
            }
            else
            {
                // Database error.. abort
                string msg = string.Format("Database error while reading 'MatchingCategories'.\nThe error reads:\n{0}", mC.ErrorOutput);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                _LinesProcessed = -1;
                _IsSuccess = false;
                return;
            }

            //RRDMMatchingCategoriesVsSourcesFiles Csf = new RRDMMatchingCategoriesVsSourcesFiles();
            //Csf.ReadReconcCategoriesVsSourcesBySourceFile(SourceFileID);

            //if (!Csf.ErrorFound)
            //{
            //    if (Csf.RecordFound && Csf.IsTargetSystem)
            //    {
            //        _CategoryId = Csf.CategoryId;
            //    }
            //}
            //else
            //{
            //    // Database error.. abort
            //    string msg = string.Format("Database error while reading 'MatchingCategoriesVsSourcesFiles'.\nThe error reads:\n{0}", Csf.ErrorOutput);
            //    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
            //    _LinesProcessed = -1;
            //    _IsSuccess = false;
            //    return;
            //}
            #endregion

            #region Read the File Layout record (returns a table, one row per field)
            RRDMMappingFileFieldsFromBankToRRDM sfl = new RRDMMappingFileFieldsFromBankToRRDM();
            sfl.ReadSourceFileLayout(argSrcFileID);
            if (sfl.ErrorFound)
            {
                string msg = string.Format("Processing stopped! " +
                                     "\nError reading Layout table for {0}! \nThe received message is: \n{1}", argSrcFileID, sfl.ErrorOutput);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                _LinesProcessed = -1;
                _IsSuccess = false;
            }
            LayoutTbl = sfl.SourceFilelayout;
            #endregion

            #region Create the In-Memory UNIV Data Table
            // Create In-Memory DataTable
            _InMemDataTbl_UNIV = RRDMMatchingTxns_InGeneralTables.CreateUNIV_MemTable();
            if (_InMemDataTbl_UNIV == null)
            {
                string msg = "Processing stopped! Could not create the In-Memory UNIV Data table.";
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                _LinesProcessed = -1;
                _IsSuccess = false;
                return;
            }
            _InMemDataTbl_UNIV.Clear();
            #endregion

            #region Create the In-Memory RAW Data Table
            // Create In-Memory DataTable
            _InMemDataTbl_RAW = ReconcCore_Banking_T24_RAW.CreateMemoryTable();
            if (_InMemDataTbl_RAW == null)
            {
                string msg = "Processing stopped! Could not create the In-Memory RAW Data table.";
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                _LinesProcessed = -1;
                _IsSuccess = false;
                return;
            }
            _InMemDataTbl_RAW.Clear();
            #endregion

            #region Read the file and process in batches of 'MaxPagingRows' lines
            ReconcCore_Banking_T24_RAW rawTbl = new ReconcCore_Banking_T24_RAW();
            RRDMMatchingTxns_InGeneralTables univTbl = new RRDMMatchingTxns_InGeneralTables();
            string _line;
            int TotalLines = 0;
            int LineCounter = 0;
            int LinesInError = 0;
            _IsSuccess = true;
            _LinesProcessed = 0;
            #region Try
            try
            {
                List<string> Lines = new List<string>();
                using (StreamReader file = new StreamReader(fullFileName))
                {
                    while ((_line = file.ReadLine()) != null)
                    {
                        Lines.Add(_line);
                    }
                    file.Close();
                }
                TotalLines = Lines.Count;

                #region Loop through pages
                for (int i = 0; i < TotalLines; i = i + MaxPagingRows)
                {
                    _InMemDataTbl_UNIV.Clear();
                    _InMemDataTbl_RAW.Clear();

                    //if(Environment.UserInteractive)
                    //{ 
                    //    Console.WriteLine("File: {0} - Lines {1} to {2}", _sourceFilePath, i + 1, i + MaxPagingRows);
                    //}

                    #region Single Paging
                    var page = Lines.Skip(i).Take(MaxPagingRows);
                    foreach (string Line in page)
                    {
                        if ((LineCounter >= LinesInHeadrer) && (LineCounter < TotalLines - LinesInTrailer))
                        {
                            //if (Environment.UserInteractive)
                            //{
                            //    Console.WriteLine("Line {0,6:000000}: {1}", LineCounter+1, Line.Substring(0, 64));
                            //}

                            bool errorsFound = ProcessLine_Core_Banking_T24.ProcessLineT2401(_Operator, _Origin, _FileUID,
                                                                                             LineCounter + 1, Line, LayoutTbl,
                                                                                             _BINsToCategories, _CategoryId,
                                                                                             _InMemDataTbl_RAW, _InMemDataTbl_UNIV,
                                                                                             fullFileName, AtmsTbl);
                            _LinesProcessed++;
                            if (errorsFound)
                            {
                                LinesInError++;
                            }
                        }
                        LineCounter++;
                    }
                    #endregion

                    if (LinesInError > 0)
                    {
                        rawTbl.DeleteRecordsByOriginFile(_FileUID);
                        univTbl.DeleteRecordsByOriginFile(_FileUID, _importTable);
                    }

                    if (LinesInError == 0 && _LinesProcessed > 0)
                    #region Process Single Page - OK
                    {
                        // At this point the In-Memory Data Tables (RAW and UNIV) contain the lines from the source file
                        #region Do a BULKINSERT in RAW
                        //ReconcCore_Banking_T24_RAW rawTbl = new ReconcCore_Banking_T24_RAW();
                        rawTbl.BulkInsertFromDataTable(_InMemDataTbl_RAW);
                        if (rawTbl.ErrorFound)
                        {
                            string msg = string.Format("Error while BULK INSERTing into Table [{0}]!\nThe message reads: {1}", _importTable + "_RAW", rawTbl.ErrorOutput);
                            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Information);
                            msg = string.Format("No rows were inserted in Table [{0}_RAW]!", _importTable);
                            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                            _LinesProcessed = -1;
                            _IsSuccess = false;
                        }
                        else
                        {
                            string msg = string.Format("Succesfully inserted into Table [{0}_RAW]!", _importTable);
                            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Information);
                            // _LinesProcessed = LineCounter;
                            _IsSuccess = true;
                        }

                        #region INSERT BY PANICOS
                        //// ****************INSERT BY PANICOS**************
                        //string Message = "Alecos_Insert T24 File - File Creation ";

                        //Pt.InsertPerformanceTrace("ETHNCY2N", "ETHNCY2N", 2, "LoadTrans", "NBG101", BeforeCallDtTime, DateTime.Now, Message);
                        //BeforeCallDtTime = DateTime.Now; 
                        //// *************************END INSERT BY PANICOS********************************
                        #endregion

                        #endregion
                        if (_IsSuccess)
                        {
                            // Do a BULKINSERT in UNIV
                            //RRDMMatchingTxns_InGeneralTables univTbl = new RRDMMatchingTxns_InGeneralTables();
                            univTbl.BulkInsertFromDataTable(_InMemDataTbl_UNIV, _importTable);
                            if (!univTbl.ErrorFound)
                            {
                                string msg = string.Format("Succesfully inserted into Table [{0}]!", _importTable);
                                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Information);
                                // _LinesProcessed = LineCounter;
                                _IsSuccess = true;

                                //// ****************INSERT BY PANICOS**************
                                //Message = "Alecos_Insert T24 File - For Bulk Insert";

                                //Pt.InsertPerformanceTrace("ETHNCY2N", "ETHNCY2N", 2, "LoadTrans", "NBG101", BeforeCallDtTime, DateTime.Now, Message);
                                ////BeforeCallDtTime = DateTime.Now;
                                //// *************************END INSERT BY PANICOS********************************
                            }
                            else
                            {
                                string msg = string.Format("Error while BULK INSERTing into Table [{0}]!\nThe message reads: {1}", _importTable, univTbl.ErrorOutput);
                                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                                msg = string.Format("No rows were inserted in Table [{0}]!", _importTable);
                                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                                // Delete already inserted records in RAW
                                rawTbl.DeleteRecordsByOriginFile(_FileUID);
                                if (!rawTbl.ErrorFound)
                                {
                                    msg = string.Format("Inserted rows in table {0} were deleted! Table [{0}] is restored to a consistent state!", _importTable + "_RAW");
                                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                                }
                                else
                                {
                                    msg = string.Format("Inserted rows in table {0} could not be deleted! Manual recovery may be needed!\r\nThe error message is:{1}", _importTable + "_RAW", rawTbl.ErrorOutput);
                                    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                                }
                                _LinesProcessed = -1;
                                _IsSuccess = false;
                            }
                        }
                        else
                        {
                            string msg = string.Format("No rows were inserted in Table [{0}]!", _importTable);
                            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                            _LinesProcessed = -1;
                            _IsSuccess = false;
                        }
                    }
                    #endregion
                    else
                    #region Process Single Page - NOT OK
                    {
                        string msg = string.Format("No rows were inserted in Table [{0}]!", _importTable);
                        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                        _LinesProcessed = -1;
                        _IsSuccess = false;
                        break;
                    }
                    #endregion
                }
                #endregion 

                return;
            }
            #endregion

            #region Catch
            catch (Exception ex)
            {
                // Delete records in "Initial Table" with FileUID...
                rawTbl.DeleteRecordsByOriginFile(_FileUID);
                univTbl.DeleteRecordsByOriginFile(_FileUID, _importTable);

                string msg = string.Format("Exception System.IO!\r\n{0}", ex.Message);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                _LinesProcessed = -1;
                _IsSuccess = false;
            }
            #endregion
            #endregion
        }
        /* ======= End of ProcessSourceFile() ================= */
        #endregion
    }
}
