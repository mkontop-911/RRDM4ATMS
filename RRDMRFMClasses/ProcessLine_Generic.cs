using RRDM4ATMs;
using System;
using System.Data;

namespace RRDMRFMClasses
{
    public class ProcessLine_Generic
    {

        static DataTable BINsToCategories;
        static string CategoryID;
        static DataTable InMemTable_UNIV;

        #region ProcessLine
        /// <summary>
        /// Process a sigle Line from the source File...
        /// </summary>
        /// <param name="Origin"></param>
        /// <param name="FileUID"></param>
        /// <param name="LineNumber"></param>
        /// <param name="Line"></param>
        /// <param name="Line"></param>
        /// <param name="LayoutTbl"></param>
        /// <returns>true/false</returns>
        public static bool ProcessLineGeneric(string Operator, string Origin, string FileUID, int LineNumber, string Line, DataTable LayoutTbl,
                                       DataTable _BINsToCategories, string _CategoryID, DataTable _InMemTable_UNIV, string fullFileName)
        {
            BINsToCategories = _BINsToCategories;
            CategoryID = _CategoryID;
            InMemTable_UNIV = _InMemTable_UNIV;


            // string msg;
            string SelectExpression;
            Extracted Xtr;
            bool ExtractionErrorsFound = false;

            RRDMMatchingTxns_InGeneralTables InitialTbl = new RRDMMatchingTxns_InGeneralTables();
            InitialTbl.OriginFileName = FileUID;        // Generated, points into ReconcFileMonitorLog
            InitialTbl.OriginalRecordId = LineNumber;   // To identify the line where the record came from
            InitialTbl.Origin = Origin;                 // SystemOfOrigin
            InitialTbl.Operator = Operator;             // From pgm parameter
            InitialTbl.ProcessedAtRMCycle = 0;          // Will be set by matching routines
            InitialTbl.RRNumber = "0";                    // Leave empty
        //    InitialTbl.T24RefNumber = "";               // Leave empty

            InitialTbl.MatchingCateg = "";      // Retrieve from lookup table 

            InitialTbl.TerminalId = "";         // Retrieve from Line
            InitialTbl.TransType = 0;           // Retrieve from Line
            InitialTbl.TransDescr = "";         // Retrieve from Line
            InitialTbl.TransTypeAtOrigin = "";  // Retrieve from Line
            InitialTbl.CardNumber = "";         // Retrieve from Line
            InitialTbl.AccNo = "";              // Retrieve from Line
            InitialTbl.TransCurr = "";          // Retrieve from Line
            InitialTbl.TransAmt = 0;            // Retrieve from Line
            InitialTbl.AmtFileBToFileC = 0;     // Retrieve from Line
            InitialTbl.TransDate = DateTime.MinValue;   // Retrieve from Line
            InitialTbl.TraceNo = 0;             // Retrieve from Line
            InitialTbl.ResponseCode = "";        // Retrieve from Line

            InitialTbl.Processed = false;

            #region Extract: TransDate (DATETIME)
            // DATETIME
            SelectExpression = "TargetFieldNm = 'TransDate'";
            Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.DateTime, fullFileName);
            if (Xtr.Success)
                InitialTbl.TransDate = Xtr.ValueDateTime;
            else
                ExtractionErrorsFound = true;
            #endregion

            #region Extract: TerminalID (CHAR)
            // CHAR
            SelectExpression = "TargetFieldNm = 'TerminalId'";
            Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
            if (Xtr.Success)
                InitialTbl.TerminalId = Xtr.ValueString;
            else
                ExtractionErrorsFound = true;
            #endregion

            #region Extract: TransType (INT)
            // INT
            SelectExpression = "TargetFieldNm = 'TransType'";
            Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Numeric, fullFileName);
            if (Xtr.Success)
                InitialTbl.TransType = Xtr.ValueInt;
            else
                ExtractionErrorsFound = true;
            #endregion

            #region Extract: TransDescr
            // CHAR
            SelectExpression = "TargetFieldNm = 'TransDescr'";
            Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
            if (Xtr.Success)
                InitialTbl.TransDescr = Xtr.ValueString;
            else
                ExtractionErrorsFound = true;
            #endregion

            #region Extract: TransTypeAtOrigin
            // CHAR
            SelectExpression = "TargetFieldNm = 'TransTypeAtOrigin'";
            Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
            if (Xtr.Success)
                InitialTbl.TransTypeAtOrigin = Xtr.ValueString;
            else
                ExtractionErrorsFound = true;
            #endregion

            #region Extract: CardNumber
            // CHAR
            SelectExpression = "TargetFieldNm = 'CardNumber'";
            Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
            if (Xtr.Success)
                InitialTbl.CardNumber = Xtr.ValueString;
            else
                ExtractionErrorsFound = true;
            #endregion

            #region Get Matching Category from Card Number
            if (InitialTbl.CardNumber.Length >= 6 && BINsToCategories != null) // there is a value to search the table for; the table exists
            {
                string BIN = InitialTbl.CardNumber.Substring(0, 6);
                string CatId = RFMFunctions.GetMatchingCategoryFromBIN(BIN, BINsToCategories);
                if (CatId != "")
                {
                    InitialTbl.MatchingCateg = CatId;
                }
                else
                {
                    // Covers IST: Get instructions on how to handle from 'MatchingCateg' SourceFieldvalue
                    string srcFldValue;
                    string expr = "TargetFieldNm = 'MatchingCateg'";
                    DataRow[] lR = null; // To hold the layout of the line
                    lR = LayoutTbl.Select(expr);
                    RRDMMappingFileFieldsFromBankToRRDM.SourceFileLayoutStruct FldAtr = RFMFunctions.GetFieldLayoutAttributes(lR);

                    if (FldAtr.Exists)
                    {
                        srcFldValue = FldAtr.SourceFieldValue;
                        switch (srcFldValue)
                        {
                            case "Rtn_IsMissing": // We have instructions! Get the value from 'TargetFieldValue' 
                                {
                                    InitialTbl.MatchingCateg = FldAtr.TargetFieldValue;
                                    break;
                                }
                            default: // We do not have instructions
                                {
                                    RFMFunctions.LogConversionFailure(fullFileName, LineNumber, "MatchingCategory", "TableLookup", InitialTbl.CardNumber);
                                    ExtractionErrorsFound = true;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        //   We do not have instructions
                        RFMFunctions.LogConversionFailure(fullFileName, LineNumber, "MatchingCategory", "TableLookup", InitialTbl.CardNumber);
                        ExtractionErrorsFound = true;
                    }
                }
            }
            else
            {
                InitialTbl.MatchingCateg = CategoryID;
            }
            #endregion

            #region Extract: AccNo
            // CHAR
            SelectExpression = "TargetFieldNm = 'AccNo'";
            Xtr =RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
            if (Xtr.Success)
                InitialTbl.AccNo = Xtr.ValueString;
            else
                ExtractionErrorsFound = true;
            #endregion

            #region Extract: TransCurr
            // CHAR
            SelectExpression = "TargetFieldNm = 'TransCurr'";
            Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
            if (Xtr.Success)
                InitialTbl.TransCurr = Xtr.ValueString;
            else
                ExtractionErrorsFound = true;
            #endregion

            #region Extract: TransAmount (DEC)
            // DEC
            // SelectExpression = "TargetFieldNm = 'TransAmt'"; //ToDo: corrrect field name in db
            SelectExpression = "TargetFieldNm = 'TransAmt'";
            Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Decimal, fullFileName);
            if (Xtr.Success)
                InitialTbl.TransAmt = Xtr.ValueDecimal;
            else
                ExtractionErrorsFound = true;
            #endregion

            #region Extract: AmtFileBToFileC (DEC)
            // DEC
            SelectExpression = "TargetFieldNm = 'AmtFileBToFileC'";
            Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Decimal, fullFileName);
            if (Xtr.Success)
                InitialTbl.AmtFileBToFileC = Xtr.ValueDecimal;
            else
                ExtractionErrorsFound = true;
            #endregion

            #region Extract: TraceNo (INT)
            // INT
            SelectExpression = "TargetFieldNm = 'TraceNo'";
            Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Numeric, fullFileName);
            if (Xtr.Success)
                InitialTbl.TraceNo = Xtr.ValueInt;
            else
                ExtractionErrorsFound = true;
            #endregion

            #region // Extract: RRNumber (INT) - Deprecated !!!
            //// INT
            //SelectExpression = "TargetFieldNm = 'RRNumber'";
            //Xtr = ExtractSubString(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Numeric, fullFileName);
            //if (Xtr.Success)
            //    InitialTbl.RRNumber = Xtr.ValueInt;
            //else
            //    ExtractionErrorsFound = true;
            #endregion

            #region Extract: ResponseCode (INT)
            // INT
            SelectExpression = "TargetFieldNm = 'ResponseCode'";
            Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
            if (Xtr.Success)
                InitialTbl.ResponseCode = Xtr.ValueString;
            else
                ExtractionErrorsFound = true;
            #endregion

            #region // Extract: T24RefNumber (CHAR) 
            // CHAR
          //  SelectExpression = "TargetFieldNm = 'T24RefNumber'";
          //  Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
          //  if (Xtr.Success)
          ////      InitialTbl.T24RefNumber = Xtr.ValueString;
          //  else
          //      ExtractionErrorsFound = true;
            #endregion

            if (!ExtractionErrorsFound)
            #region INSERT into the table
            {
                // InitialTbl.InsertSingle(rsf.InitialTableName);
                //if (InitialTbl.ErrorFound)
                //{
                //    EventLogging.WriteEventLog("RRDMFileMonitor", EventLogEntryType.Error, InitialTbl.ErrorOutput);
                //    Console.WriteLine(InitialTbl.ErrorOutput);
                //    return (false);
                //}

                bool success = RFMFunctions.InsertRowInMem(InMemTable_UNIV, InitialTbl);
                if (!success)
                {
                    Console.WriteLine("Could not insert new row in the In-Memory Data Table");
                    return (false);
                }

            }
            else
            {
                // Inserted records (lines) will be deleted upstream...
                return false;
            }
            #endregion

            return (true);
        }
        #endregion


    }
}
