using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using RRDM4ATMs;
using System.Collections.Generic;
using System.Linq;

namespace RRDMRFMClasses
{
    public class ProcessSourceFile_Switch_Base24
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
        public ProcessSourceFile_Switch_Base24(string argOperator, string argOrigin, string argFileUID, 
                                               string argSrcFileID, string fullFileName,
                                               int LinesInHeadrer, int LinesInTrailer, 
                                               string argImportTable, DataTable AtmsTbl, int MaxPagingRows)
        {
            DataTable LayoutTbl;

            _Operator = argOperator;
            _Origin = argOrigin;
            _FileUID = argFileUID;
            _SourceFileID = argSrcFileID;
            _importTable = argImportTable;
            _sourceFilePath = fullFileName;

            RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();
            // ********INSERT BY PANICOS**************
            DateTime BeforeCallDtTime = DateTime.Now;
            // *************************BY PANICOS********************************

            #region Prepare for Matching Categories
            // Get the TableMatchingCategoriesVsBINs table 
            RRDMMatchingCategoriesVsBINs Mc = new RRDMMatchingCategoriesVsBINs();
            
            Mc.ReadReconcMatchingFieldsToFillDataTable_AllCategories(argOperator, "Our Atms"); // Selection Criteria?
            if (!Mc.ErrorFound)
            {
                if (Mc.RecordFound == true)
                {
                    _BINsToCategories = Mc.TableMatchingCategoriesVsBINs;
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
                string msg = string.Format("Database error while reading 'MatchingCategoriesVsBINs'.\nThe error reads:\n{0}", Mc.ErrorOutput);
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
            _InMemDataTbl_RAW = ReconcSwitch_Base24_RAW.CreateMemoryTable();
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
            ReconcSwitch_Base24_RAW rawTbl = new ReconcSwitch_Base24_RAW();
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

                    //if (Environment.UserInteractive)
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
                            //    Console.WriteLine("Line {0,6:000000}: {1}", LineCounter + 1, Line.Substring(0, 64));
                            //}

                            bool errorsFound = ProcessLine_Switch_Base24.ProcessLineFTOD_TLF(_Operator, _Origin, _FileUID,
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
                        // At this point the In-Memory Data Tables (RAW and UNIV) contain the page lines from the source file
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
                        // ****************INSERT BY PANICOS**************
                        string Message = "Alecos_Insert Switch File - File Creation Records" + _LinesProcessed.ToString();

                        Pt.InsertPerformanceTrace("ETHNCY2N", "ETHNCY2N", 2, "LoadTrans", "NBG101", BeforeCallDtTime, DateTime.Now, Message);
                        BeforeCallDtTime = DateTime.Now;
                        // *************************END INSERT BY PANICOS********************************
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
                                //Message = "Alecos_Insert Switch File - For Bulk Insert";

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

        #region // Create UNIV InMem DataTable
        //static DataTable CreateUNIV_MemTable()
        //{
        //    DataTable InMemDataTbl = new DataTable();
        //    try
        //    {
        //        InMemDataTbl.Columns.Add("SeqNo", typeof(int));
        //        InMemDataTbl.Columns.Add("OriginFileName", typeof(string));
        //        InMemDataTbl.Columns.Add("OriginalRecordId", typeof(int));
        //        InMemDataTbl.Columns.Add("MatchingCateg", typeof(string));
        //        InMemDataTbl.Columns.Add("Origin", typeof(string));
        //        InMemDataTbl.Columns.Add("TransTypeAtOrigin", typeof(string));
        //        InMemDataTbl.Columns.Add("TerminalId", typeof(string));
        //        InMemDataTbl.Columns.Add("TransType", typeof(int));
        //        InMemDataTbl.Columns.Add("TransDescr", typeof(string));
        //        InMemDataTbl.Columns.Add("CardNumber", typeof(string));
        //        InMemDataTbl.Columns.Add("AccNo", typeof(string));
        //        InMemDataTbl.Columns.Add("TransCurr", typeof(string));
        //        InMemDataTbl.Columns.Add("TransAmt", typeof(decimal));
        //        InMemDataTbl.Columns.Add("AmtFileBToFileC", typeof(decimal));
        //        InMemDataTbl.Columns.Add("TransDate", typeof(DateTime));
        //        InMemDataTbl.Columns.Add("TraceNo", typeof(int));
        //        InMemDataTbl.Columns.Add("RRNumber", typeof(int));
        //        InMemDataTbl.Columns.Add("ResponseCode", typeof(string));
        //        InMemDataTbl.Columns.Add("T24RefNumber", typeof(string));
        //        InMemDataTbl.Columns.Add("Processed", typeof(bool));
        //        InMemDataTbl.Columns.Add("ProcessedAtRMCycle", typeof(int));
        //        InMemDataTbl.Columns.Add("Operator", typeof(string));
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = string.Format("Exception encountered while creating the In-Memory table! The message reads: {0}", ex.Message);
        //        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
        //        return (null);
        //    }
        //    return InMemDataTbl;
        //}
        #endregion

        #region // Insert new row in UNIV InMem DataTable
        //public static bool InsertRowInMem(DataTable InMemdataTbl, ReconcInitialTable InitialTbl)
        //{
        //    bool rc = false;
        //    try
        //    {
        //        DataRow InMemRow = InMemdataTbl.NewRow();
        //        InMemRow["SeqNo"] = InitialTbl.SeqNo;
        //        InMemRow["OriginFileName"] = InitialTbl.OriginFileName;
        //        InMemRow["OriginalRecordId"] = InitialTbl.OriginalRecordId;
        //        InMemRow["MatchingCateg"] = InitialTbl.MatchingCateg;
        //        InMemRow["Origin"] = InitialTbl.Origin;
        //        InMemRow["TransTypeAtOrigin"] = InitialTbl.TransTypeAtOrigin;
        //        InMemRow["TerminalId"] = InitialTbl.TerminalId;
        //        InMemRow["TransType"] = InitialTbl.TransType;
        //        InMemRow["TransDescr"] = InitialTbl.TransDescr;
        //        InMemRow["CardNumber"] = InitialTbl.CardNumber;
        //        InMemRow["AccNo"] = InitialTbl.AccNo;
        //        InMemRow["TransCurr"] = InitialTbl.TransCurr;
        //        InMemRow["TransAmt"] = InitialTbl.TransAmt;
        //        InMemRow["AmtFileBToFileC"] = InitialTbl.AmtFileBToFileC;
        //        InMemRow["TransDate"] = InitialTbl.TransDate;
        //        InMemRow["TraceNo"] = InitialTbl.TraceNo;
        //        InMemRow["RRNumber"] = InitialTbl.RRNumber;
        //        InMemRow["ResponseCode"] = InitialTbl.ResponseCode;
        //        InMemRow["T24RefNumber"] = InitialTbl.T24RefNumber;
        //        InMemRow["Processed"] = InitialTbl.Processed;
        //        InMemRow["ProcessedAtRMCycle"] = InitialTbl.ProcessedAtRMCycle;
        //        InMemRow["Operator"] = InitialTbl.Operator;
        //        InMemdataTbl.Rows.Add(InMemRow);
        //        rc = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        RFMFunctions.RecordEventMsg(ex.Message, EventLogEntryType.Error);
        //        rc = false;
        //    }
        //    return (rc);
        //}
        #endregion

        #region // Get Field Layout Attributes
        //public static RRDMMatchingFileFieldsFromBankToRRDM.SourceFileLayoutStruct GetFieldLayoutAttributes(DataRow[] LayoutRow)
        //{
        //    RRDMMatchingFileFieldsFromBankToRRDM.SourceFileLayoutStruct FieldAttrib = new RRDMMatchingFileFieldsFromBankToRRDM.SourceFileLayoutStruct();
        //    try
        //    {
        //        FieldAttrib.SeqNo = (int)LayoutRow[0]["SeqNo"];
        //        FieldAttrib.SourceFileId = LayoutRow[0]["SourceFileId"].ToString();
        //        FieldAttrib.SourceFieldNm = LayoutRow[0]["SourceFieldNm"].ToString();
        //        FieldAttrib.SourceFieldValue = LayoutRow[0]["SourceFieldValue"].ToString();
        //        FieldAttrib.SourceFieldPositionStart = (int)LayoutRow[0]["SourceFieldPositionStart"];
        //        FieldAttrib.SourceFieldPositionEnd = (int)LayoutRow[0]["SourceFieldPositionEnd"];
        //        FieldAttrib.TargetFieldNm = LayoutRow[0]["TargetFieldNm"].ToString();
        //        FieldAttrib.TargetFieldType = LayoutRow[0]["TargetFieldType"].ToString();
        //        FieldAttrib.TargetFieldValue = LayoutRow[0]["TargetFieldValue"].ToString();
        //        //FieldAttrib.RoutineValidation = (bool)LayoutRow[0]["RoutineValidation"];
        //        //FieldAttrib.RoutineNm = LayoutRow[0]["RoutineNm"].ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        FieldAttrib.Exists = false;
        //        FieldAttrib.SeqNo = 0;
        //        FieldAttrib.SourceFileId = "";
        //        FieldAttrib.SourceFieldNm = "";
        //        FieldAttrib.SourceFieldValue = "";
        //        FieldAttrib.SourceFieldPositionStart = 0;
        //        FieldAttrib.SourceFieldPositionEnd = 0;
        //        FieldAttrib.TargetFieldNm = "";
        //        FieldAttrib.TargetFieldType = "";
        //        FieldAttrib.TargetFieldValue = "";
        //        //FieldAttrib.RoutineValidation = false;
        //        //FieldAttrib.RoutineNm = "";
        //        return (FieldAttrib);
        //    }
        //    FieldAttrib.Exists = true;
        //    return (FieldAttrib);
        //}
        #endregion

        #region // Extract Field Value from Line 
        ///// <summary>
        ///// Extract Value from source Line and assign in appropriate element of structure: 'Extracted'
        ///// </summary>
        ///// <param name="expression"></param>
        ///// <param name="LayoutTbl"></param>
        ///// <param name="LineNumber"></param>
        ///// <param name="Line"></param>
        ///// <param name="TargetFieldType"></param>
        ///// <param name="fullFileName"></param>
        ///// <returns>Extracted</returns>
        //private static Extracted ExtractFieldValue(string expression, DataTable LayoutTbl, int LineNumber, string Line, FieldType TargetFieldType, string fullFileName)
        //{
        //    string XtractedValue = "";
        //    int i;
        //    decimal dec;
        //    DateTime dt;
        //    int FieldStart;
        //    int FieldLength;

        //    DataRow[] LayoutR = null; // To hold the layout of the line
        //    RRDMMatchingFileFieldsFromBankToRRDM.SourceFileLayoutStruct FieldAttrib; // Struct to hold field attributes, retrieved from 'LayoutR'
        //    Extracted Xtract = new Extracted();
        //    Xtract.Success = false;
        //    Xtract.ValueString = "";
        //    Xtract.ValueInt = 0;
        //    Xtract.ValueDecimal = 0;
        //    Xtract.ValueDateTime = DateTime.MinValue;

        //    try
        //    {
        //        // Select row in table as per 'expression'
        //        LayoutR = LayoutTbl.Select(expression);

        //        if (LayoutR.Length == 1) // There should only be one row; many rows make it ambiguous...
        //        {
        //            FieldAttrib = GetFieldLayoutAttributes(LayoutR); // Retrieve attributes and place in 'FieldAttrib'
        //            if (!FieldAttrib.Exists)
        //            {
        //                RFMFunctions.LogExpressionFailure(fullFileName, LineNumber, expression);
        //                return (Xtract);
        //            }
        //        }
        //        else
        //        {
        //            RFMFunctions.LogExpressionFailure(fullFileName, LineNumber, expression);
        //            return (Xtract);
        //        }

        //        Xtract.ValueType = FieldAttrib.TargetFieldType;
        //        string SrcFldVal = FieldAttrib.SourceFieldValue;

        //        switch (SrcFldVal)
        //        {
        //            #region Rtn_Fixed (916,2)
        //            case "Rtn_Fixed":
        //                {
        //                    XtractedValue = FieldAttrib.TargetFieldValue;
        //                    break;
        //                }
        //            #endregion
        //            #region Rtn_ICBS-Numeric (916,3)
        //            case "Rtn_ICBS-Numeric":
        //                {
        //                    string srcVal;
        //                    decimal srcDec;

        //                    FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
        //                    FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
        //                    srcVal = Line.Substring(FieldStart, FieldLength);

        //                    if (Decimal.TryParse(srcVal, out srcDec))
        //                    {
        //                        int srcInt = (int)srcDec;
        //                        XtractedValue = srcInt.ToString();
        //                    }
        //                    else
        //                    {
        //                        return (Xtract);
        //                    }
        //                    break;
        //                }
        //            #endregion
        //            #region Rtn_ICBS-DateTime (916,4)
        //            case "Rtn_ICBS-DateTime":
        //                {
        //                    // contruct the XtractedValue string
        //                    // source format:        [20,170,516      92,103]
        //                    // XtractedValue format: [16-05-2017 09:21:03]

        //                    string srcDate, srcTime;
        //                    decimal decDate, decTime;
        //                    int intDate;
        //                    int intTime;
        //                    string strDate, strTime;

        //                    // ToDo: Validation, see below
        //                    FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
        //                    FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
        //                    srcDate = Line.Substring(FieldStart, 10);
        //                    srcTime = Line.Substring(FieldStart + 15, 7);

        //                    if (Decimal.TryParse(srcDate, out decDate))
        //                    {
        //                        intDate = (int)decDate;
        //                        strDate = string.Format("{0:0000-00-00}", intDate);
        //                    }
        //                    else
        //                    {
        //                        return (Xtract);
        //                    }
        //                    if (Decimal.TryParse(srcTime, out decTime))
        //                    {
        //                        intTime = (int)decTime;
        //                        strTime = string.Format("{0:00:00:00}", intTime);
        //                    }
        //                    else
        //                    {
        //                        return (Xtract);
        //                    }
        //                    XtractedValue = string.Format("{0} {1}", strDate, strTime);
        //                    break;
        //                }
        //            #endregion
        //            #region Rtn_IST-Account (916,5)
        //            case "Rtn_IST-Account":
        //                {
        //                    string srcVal;
        //                    FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
        //                    FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
        //                    srcVal = Line.Substring(FieldStart, FieldLength);
        //                    try
        //                    {
        //                        int cutFrom = srcVal.IndexOf("|");
        //                        if (cutFrom != -1)
        //                            XtractedValue = srcVal.Substring(0, cutFrom);
        //                        else
        //                            XtractedValue = srcVal;
        //                    }
        //                    catch (Exception ex) //ArgumentOutOfRangeException
        //                    {
        //                        Xtract.Success = false;
        //                        return (Xtract);
        //                    }
        //                    break;
        //                }
        //            #endregion
        //            #region Rtn_IST-ResponceCode (916,6)
        //            case "Rtn_IST-ResponseCode":
        //                {
        //                    string srcVal;
        //                    FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
        //                    FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
        //                    srcVal = Line.Substring(FieldStart, FieldLength);
        //                    try
        //                    {
        //                        int cutFrom = srcVal.IndexOf(" - ");
        //                        if (cutFrom != -1)
        //                            XtractedValue = srcVal.Substring(0, cutFrom);
        //                        else
        //                            XtractedValue = "0";
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        XtractedValue = "0";
        //                    }
        //                    break;
        //                }
        //            #endregion
        //            #region Rtn_IST-FXAmtEquiv (916,7)
        //            case "Rtn_IST-FXAmtEquiv":
        //                {
        //                    // Set to TransAmt if AmtEquiv = 0
        //                    string srcVal;
        //                    string srcAmt;
        //                    FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
        //                    FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
        //                    srcVal = Line.Substring(FieldStart, FieldLength);
        //                    srcAmt = srcVal.Trim();
        //                    if (Decimal.TryParse(srcAmt, out dec))
        //                    {
        //                        if (dec != 0)
        //                        {
        //                            // Not zero, use this 
        //                            XtractedValue = srcAmt;
        //                            break;
        //                        }
        //                        else
        //                        {
        //                            // ToDo: error handling
        //                            // Zero, use the TransAmt 
        //                            DataRow[] LayoutTransAmt = null; // To hold the layout of the line
        //                            RRDMMatchingFileFieldsFromBankToRRDM.SourceFileLayoutStruct TransAmtAttrib; // Struct to hold field attributes, retrieved from 'LayoutR'
        //                            LayoutTransAmt = LayoutTbl.Select("TargetFieldNm = 'TransAmt'");
        //                            TransAmtAttrib = GetFieldLayoutAttributes(LayoutTransAmt); // Retrieve attributes and place in 'FieldAttrib'
        //                            if (!TransAmtAttrib.Exists)
        //                            {
        //                                RFMFunctions.LogExpressionFailure(fullFileName, LineNumber, "TargetFieldNm = 'TransAmt'");
        //                                Xtract.Success = false;
        //                                return (Xtract);
        //                            }
        //                            FieldStart = TransAmtAttrib.SourceFieldPositionStart - 1;
        //                            FieldLength = TransAmtAttrib.SourceFieldPositionEnd - FieldStart;
        //                            XtractedValue = Line.Substring(FieldStart, FieldLength);
        //                            break;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Xtract.Success = false;
        //                        RFMFunctions.LogConversionFailure(fullFileName, LineNumber, FieldAttrib.TargetFieldNm, "IST-AmtEquiv", srcAmt);
        //                    }
        //                    break;
        //                }
        //            #endregion
        //            #region default (916,1)
        //            default:
        //                {
        //                    //Validate Start / Length
        //                    if (FieldAttrib.SourceFieldPositionStart < 1 || (FieldAttrib.SourceFieldPositionEnd < FieldAttrib.SourceFieldPositionStart))
        //                    {
        //                        string msg = string.Format("Error found in file : '{0}', Line: {1}! " +
        //                                 "\nError: Invalid attributes when extracting: '{2}'!\nStart Position: {3}\nEnd Position: {4}",
        //                                 fullFileName, LineNumber, expression, FieldAttrib.SourceFieldPositionStart, FieldAttrib.SourceFieldPositionEnd);
        //                        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
        //                        return (Xtract);
        //                    }
        //                    FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
        //                    FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
        //                    XtractedValue = Line.Substring(FieldStart, FieldLength);
        //                    break;
        //                }
        //                #endregion
        //        }

        //        // Trim white space
        //        string XtractedTrimmed = XtractedValue.Trim();

        //        // XtractedTrimmed in a string representation of the corresponding type 
        //        switch (TargetFieldType)
        //        {
        //            #region Character
        //            case FieldType.Character:
        //                {
        //                    Xtract.ValueString = XtractedTrimmed;
        //                    Xtract.Success = true;
        //                    break;
        //                }
        //            #endregion
        //            #region Numeric
        //            case FieldType.Numeric:
        //                {
        //                    if (Int32.TryParse(XtractedTrimmed, out i))
        //                    {
        //                        Xtract.ValueInt = i;
        //                        Xtract.Success = true;
        //                    }
        //                    else
        //                    {
        //                        Xtract.Success = false;
        //                        RFMFunctions.LogConversionFailure(fullFileName, LineNumber, FieldAttrib.TargetFieldNm, "Int32", XtractedTrimmed);
        //                    }
        //                    break;
        //                }
        //            #endregion
        //            #region Decimal
        //            case FieldType.Decimal:
        //                {
        //                    if (Decimal.TryParse(XtractedTrimmed, out dec))
        //                    {
        //                        Xtract.ValueDecimal = dec;
        //                        Xtract.Success = true;
        //                    }
        //                    else
        //                    {
        //                        Xtract.Success = false;
        //                        RFMFunctions.LogConversionFailure(fullFileName, LineNumber, FieldAttrib.TargetFieldNm, "Decimal", XtractedTrimmed);
        //                    }
        //                    break;
        //                }
        //            #endregion
        //            #region DateTime
        //            case FieldType.DateTime:
        //                {
        //                    if (DateTime.TryParse(XtractedTrimmed, out dt))
        //                    {
        //                        Xtract.ValueDateTime = dt;
        //                        Xtract.Success = true;
        //                    }
        //                    else
        //                    {
        //                        Xtract.Success = false;
        //                        RFMFunctions.LogConversionFailure(fullFileName, LineNumber, FieldAttrib.TargetFieldNm, "DateTime", XtractedTrimmed);
        //                    }
        //                    break;
        //                }
        //            #endregion
        //            #region default
        //            default:
        //                {
        //                    Xtract.Success = false;
        //                    RFMFunctions.LogConversionFailure(fullFileName, LineNumber, FieldAttrib.TargetFieldNm, "UNKNOWN", XtractedTrimmed);
        //                    break;
        //                }
        //                #endregion
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //ToDo: Hnadle Exception ?
        //        Xtract.Success = false;
        //        string msg1 = ex.Message;
        //        return (Xtract);
        //    }
        //    return (Xtract);
        //}
        #endregion

        #region // Get Matching CategoryId from BIN
        //public static string GetMatchingCategoryFromBIN(string BIN)
        //{
        //    string expression;
        //    string CategoryId = "";
        //    DataRow[] Row;

        //    expression = "BIN = " + BIN;
        //    try
        //    {
        //        Row = glbBINsToCategories.Select(expression);
        //        if (Row.Length != 1) // only a single row must heve been returned.. otherwise there are duplicate rows which should not have happened
        //        {
        //            // Error will handled upstream...
        //            return ("");
        //        }
        //        CategoryId = Row[0]["CategoryId"].ToString();
        //        return (CategoryId);
        //    }
        //    catch
        //    {
        //        // Error will handled upstream...
        //        return ("");
        //    }
        //}
        #endregion

    }
}
