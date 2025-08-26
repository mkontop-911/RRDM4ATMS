using System;
using System.Data;

namespace RRDMRFMClasses
{
    public class ProcessLine_Switch_Base24
    {

        static DataTable BINsToCategories;
        static string CategoryID;

        #region ProcessLine
        /// <summary>
        /// Process a single line (FTOD_TLF)
        /// </summary>
        /// <param name="Operator"></param>
        /// <param name="Origin"></param>
        /// <param name="FileUID"></param>
        /// <param name="LineNumber"></param>
        /// <param name="Line"></param>
        /// <param name="LayoutTbl"></param> 
        /// <param name="_BINsToCategories"></param>
        /// <param name="_CategoryID"></param>
        /// <param name="_InMemTable_RAW"></param>
        /// <param name="_InMemTable_UNIV"></param>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public static bool ProcessLineFTOD_TLF(string Operator, string Origin, string FileUID, int LineNumber, 
                                               string Line, DataTable LayoutTbl,  DataTable _BINsToCategories, 
                                               string _CategoryID, DataTable _InMemTable_RAW, DataTable _InMemTable_UNIV, 
                                               string fullFileName, DataTable AtmsTbl)
        {
            BINsToCategories = _BINsToCategories;
            CategoryID = _CategoryID;


            string SelectExpression = "Method Invocation";
            Extracted Xtr;
            bool ExtractionErrorsFound = false;

            try
            {
                DataRow drRAW = _InMemTable_RAW.NewRow();
                DataRow drUNIV = _InMemTable_UNIV.NewRow();


                // Common for UNIV and RAW
                drUNIV["SeqNo"] = drRAW["SeqNo"] = 0;                                   // identity....
                drUNIV["Origin"] = drRAW["Origin"] = Origin;                            // SystemOfOrigin
                drUNIV["OriginFileName"] = drRAW["OriginFileName"] = FileUID;           // Generated, points into ReconcFileMonitorLog
                drUNIV["OriginalRecordId"] = drRAW["OriginalRecordId"] = LineNumber;    // To identify the line where the record came from
                drUNIV["Operator"] = drRAW["Operator"] = Operator;                      // From pgm parameter

                // UNIV, pretermined values
                drUNIV["ProcessedAtRMCycle"] = 0;                                       // Will be set by matching routines
                drUNIV["RRNumber"] = 0;                                                 // Leave empty
                drUNIV["Mask"] = 0;                                                     // Leave empty
                drUNIV["ItHasException"] = false;                                       // Leave empty
                drUNIV["UniqueRecordId"] = 0;                                           // Leave empty
                drUNIV["FullTraceNo"] = "";                                             // Leave empty
                drUNIV["Twin"] = "";                                             // Leave empty

                // UNIV, derived values, initialize here
                drUNIV["MatchingCateg"] = "";               // Retrieve from lookup table 

                drUNIV["TerminalId"] = "";                  // Retrieve from Line
                drUNIV["TransType"] = 0;                    // Retrieve from Line
                drUNIV["TransDescr"] = "";                  // Retrieve from Line
                drUNIV["TransTypeAtOrigin"] = "";           // Retrieve from Line
                drUNIV["CardNumber"] = "";                  // Retrieve from Line
                drUNIV["AccNo"] = "";                       // Retrieve from Line
                drUNIV["TransCurr"] = "";                   // Retrieve from Line - Translate form numeric
                drUNIV["TransAmt"] = 0;                     // Retrieve from Line
                drUNIV["AmtFileBToFileC"] = 0;              // Retrieve from Line
                drUNIV["TransDate"] = DateTime.MinValue;    // Retrieve from Line
                drUNIV["TraceNo"] = 0;                      // Retrieve from Line
                drUNIV["ResponseCode"] = "";                // Retrieve from Line
                drUNIV["Processed"] = false;

                foreach (DataRow dr in LayoutTbl.Rows)
                {
                    string targetFieldNm = dr["TargetFieldNm"].ToString();
                    string targetFieldType = dr["TargetFieldType"].ToString();
                    string sourceFieldNm = dr["SourceFieldNm"].ToString();
                    bool isUniv = (bool)dr["IsUniversal"];

                    FieldType targetFldType_enum; // enumerated value
                    switch (targetFieldType)
                    {
                        case "DateTime": { targetFldType_enum = FieldType.DateTime; break; }
                        case "Numeric": { targetFldType_enum = FieldType.Numeric; break; }
                        case "Decimal": { targetFldType_enum = FieldType.Decimal; break; }
                        case "Date": { targetFldType_enum = FieldType.Date; break; }
                        case "Time": { targetFldType_enum = FieldType.Time; break; }
                        case "Character":
                        default: { targetFldType_enum = FieldType.Character; break; }
                    }

                    SelectExpression = "TargetFieldNm = '" + targetFieldNm + "'";

                    Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, targetFldType_enum, fullFileName);
                    if (Xtr.Success)
                    {
                        #region RAW - add extracted RAW value in corresponing column of this row 
                        if (sourceFieldNm != "NotPresent") 
                        {
                            //switch (targetFieldType)
                            //{
                            //    case "DateTime": { drRAW[sourceFieldNm] = Xtr.ValueDateTime; break; }
                            //    case "Numeric": { drRAW[sourceFieldNm] = Xtr.ValueInt; break; }
                            //    case "Decimal": { drRAW[sourceFieldNm] = Xtr.ValueDecimal; break; }
                            //    case "Date": { drRAW[targetFieldNm] = Xtr.ValueDate; break; }
                            //    case "Time": { drRAW[targetFieldNm] = Xtr.ValueTime; break; }
                            //    case "Character":
                            //    default: { drRAW[sourceFieldNm] = Xtr.ValueString; break; }
                            //}

                            /* Fytefta ...*/
                            if (sourceFieldNm == "W_TRAN_DATE")
                            {
                                drRAW["W_TRAN_DATE"] = Xtr.ValueRAW;
                                drRAW["TransDate"] = Xtr.ValueDateTime;
                            }
                            else if (sourceFieldNm == "W_SEQ_NUM")
                            {
                                int tr = 0;
                                string xR = Xtr.ValueRAW;
                                drRAW["W_SEQ_NUM"] = xR;
                                string str = xR;
                                if (int.TryParse(str, out tr))
                                    drRAW["TransTraceNumber"] = tr;
                                else
                                    drRAW["TransTraceNumber"] = 0;
                            }
                            else
                            {
                                drRAW[sourceFieldNm] = Xtr.ValueRAW.Trim();
                            }
                        }
                        #endregion

                        #region UNIV - add extracted and transformed value in corresponing column of this row 
                        if (isUniv) 
                        {
                            switch (targetFieldType)
                            {
                                case "DateTime": { drUNIV[targetFieldNm] = Xtr.ValueDateTime; break; }
                                case "Numeric": { drUNIV[targetFieldNm] = Xtr.ValueInt; break; }
                                case "Decimal": { drUNIV[targetFieldNm] = Xtr.ValueDecimal; break; }
                                case "Date": { drUNIV[targetFieldNm] = Xtr.ValueDate; break; }
                                case "Time": { drUNIV[targetFieldNm] = Xtr.ValueTime; break; }
                                case "Character":
                                default: { drUNIV[targetFieldNm] = Xtr.ValueString; break; }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        ExtractionErrorsFound = true;
                    }
                } // end foreach loop

                if (!ExtractionErrorsFound)
                {
                    // RAW rows are allways added to the mem table
                    drRAW["TerminalIdentification"] = drRAW["W_TERM_ID"].ToString();
                    _InMemTable_RAW.Rows.Add(drRAW); 

                    // UNIV records are filtered...
                    // Filter based on W_REC and W_T_CDE values in RAW
                    // Fytefta
                    string wRec = drRAW["W_REC"].ToString();
                    string wtcde = drRAW["W_T_CDE"].ToString();
                    if ((wRec == "0100" || wRec == "0110" || wRec == "0120" || 
                         wRec == "0300" || wRec == "0310" || wRec == "0320" || 
                         wRec == "0500" || wRec == "0510" || wRec == "0520" ||
                         wRec == "0700" || wRec == "0710" || wRec == "0720" ||
                         wRec == "3000" || wRec == "3010" || wRec == "3020" || 
                         wRec == "4000" || wRec == "4010" || wRec == "4020" 
                        ) && (wtcde == "10")
                       )
                    {
                        // drUNIV["MatchingCateg"] = ???
                        string BIN = drRAW["W_PAN"].ToString().Substring(0, 6);
                        string CatId = RFMFunctions.GetMatchingCategoryFromBIN(BIN, BINsToCategories); // look in memory datatable
                        if (CatId != "")
                        {
                            drUNIV["MatchingCateg"] = CatId;
                        }
                        else
                        {
                            drUNIV["MatchingCateg"] = CategoryID;
                        }

                        // Check if the ATM present in the ATMs table
                        string xprsn = "ATMNo='" + drUNIV["TerminalId"]+"'";
                        DataRow[] rowsFound = AtmsTbl.Select(xprsn);
                        if (rowsFound.Length > 0)
                        {
                            // Add the row in the memory DataTable
                            _InMemTable_UNIV.Rows.Add(drUNIV);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Exception in ProcessLineFTOD_TLF! {0}", ex.Message);
                Exception ex1 = ex.InnerException;
                while (ex1 != null)
                {
                    msg += "\r\n" + ex1.Message;
                }

                RFMFunctions.LogConversionException(fullFileName, LineNumber, SelectExpression, msg);
                ExtractionErrorsFound = true;
            }
            return (ExtractionErrorsFound);
        }

        #endregion

        #region // Deprecated
        //#region Extract: TransDate (DATETIME)
        //// DATETIME
        //SelectExpression = "TargetFieldNm = 'TransDate'";
        //Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.DateTime, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.TransDate = Xtr.ValueDateTime;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //#region Extract: TerminalID (CHAR)
        //// CHAR
        //SelectExpression = "TargetFieldNm = 'TerminalId'";
        //Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.TerminalId = Xtr.ValueString;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //#region Extract: TransType (INT)
        //// INT
        //SelectExpression = "TargetFieldNm = 'TransType'";
        //Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Numeric, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.TransType = Xtr.ValueInt;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //#region Extract: TransDescr
        //// CHAR
        //SelectExpression = "TargetFieldNm = 'TransDescr'";
        //Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.TransDescr = Xtr.ValueString;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //#region Extract: TransTypeAtOrigin
        //// CHAR
        //SelectExpression = "TargetFieldNm = 'TransTypeAtOrigin'";
        //Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.TransTypeAtOrigin = Xtr.ValueString;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //#region Extract: CardNumber
        //// CHAR
        //SelectExpression = "TargetFieldNm = 'CardNumber'";
        //Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.CardNumber = Xtr.ValueString;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //#region Get Matching Category from Card Number
        //if (InitialTbl.CardNumber.Length >= 6 && BINsToCategories != null) // there is a value to search the table for; the table exists
        //{
        //    string BIN = InitialTbl.CardNumber.Substring(0, 6);
        //    string CatId = RFMFunctions.GetMatchingCategoryFromBIN(BIN, BINsToCategories);
        //    if (CatId != "")
        //    {
        //        InitialTbl.MatchingCateg = CatId;
        //    }
        //    else
        //    {
        //        // Covers IST: Get instructions on how to handle from 'MatchingCateg' SourceFieldvalue
        //        string srcFldValue;
        //        string expr = "TargetFieldNm = 'MatchingCateg'";
        //        DataRow[] lR = null; // To hold the layout of the line
        //        lR = LayoutTbl.Select(expr);
        //        RRDMMatchingFileFieldsFromBankToRRDM.SourceFileLayoutStruct FldAtr = RFMFunctions.GetFieldLayoutAttributes(lR);

        //        if (FldAtr.Exists)
        //        {
        //            srcFldValue = FldAtr.SourceFieldValue;
        //            switch (srcFldValue)
        //            {
        //                case "Rtn_IsMissing": // We have instructions! Get the value from 'TargetFieldValue' 
        //                    {
        //                        InitialTbl.MatchingCateg = FldAtr.TargetFieldValue;
        //                        break;
        //                    }
        //                default: // We do not have instructions
        //                    {
        //                        RFMFunctions.LogConversionFailure(fullFileName, LineNumber, "MatchingCategory", "TableLookup", InitialTbl.CardNumber);
        //                        ExtractionErrorsFound = true;
        //                        break;
        //                    }
        //            }
        //        }
        //        else
        //        {
        //            //   We do not have instructions
        //            RFMFunctions.LogConversionFailure(fullFileName, LineNumber, "MatchingCategory", "TableLookup", InitialTbl.CardNumber);
        //            ExtractionErrorsFound = true;
        //        }
        //    }
        //}
        //else
        //{
        //    InitialTbl.MatchingCateg = CategoryID;
        //}
        //#endregion

        //#region Extract: AccNo
        //// CHAR
        //SelectExpression = "TargetFieldNm = 'AccNo'";
        //Xtr =RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.AccNo = Xtr.ValueString;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //#region Extract: TransCurr
        //// CHAR
        //SelectExpression = "TargetFieldNm = 'TransCurr'";
        //Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.TransCurr = Xtr.ValueString;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //#region Extract: TransAmount (DEC)
        //// DEC
        //// SelectExpression = "TargetFieldNm = 'TransAmt'"; //ToDo: corrrect field name in db
        //SelectExpression = "TargetFieldNm = 'TransAmt'";
        //Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Decimal, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.TransAmt = Xtr.ValueDecimal;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //#region Extract: AmtFileBToFileC (DEC)
        //// DEC
        //SelectExpression = "TargetFieldNm = 'AmtFileBToFileC'";
        //Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Decimal, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.AmtFileBToFileC = Xtr.ValueDecimal;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //#region Extract: TraceNo (INT)
        //// INT
        //SelectExpression = "TargetFieldNm = 'TraceNo'";
        //Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Numeric, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.TraceNo = Xtr.ValueInt;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //#region // Extract: RRNumber (INT) - Deprecated !!!
        ////// INT
        ////SelectExpression = "TargetFieldNm = 'RRNumber'";
        ////Xtr = ExtractSubString(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Numeric, fullFileName);
        ////if (Xtr.Success)
        ////    InitialTbl.RRNumber = Xtr.ValueInt;
        ////else
        ////    ExtractionErrorsFound = true;
        //#endregion

        //#region Extract: ResponseCode (INT)
        //// INT
        //SelectExpression = "TargetFieldNm = 'ResponseCode'";
        //Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.ResponseCode = Xtr.ValueString;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //#region // Extract: T24RefNumber (CHAR) 
        //// CHAR
        //SelectExpression = "TargetFieldNm = 'T24RefNumber'";
        //Xtr = RFMFunctions.ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //if (Xtr.Success)
        //    InitialTbl.T24RefNumber = Xtr.ValueString;
        //else
        //    ExtractionErrorsFound = true;
        //#endregion

        //if (!ExtractionErrorsFound)
        //#region INSERT into the table
        //{
        //    // InitialTbl.InsertSingle(rsf.InitialTableName);
        //    //if (InitialTbl.ErrorFound)
        //    //{
        //    //    EventLogging.WriteEventLog("RRDMFileMonitor", EventLogEntryType.Error, InitialTbl.ErrorOutput);
        //    //    Console.WriteLine(InitialTbl.ErrorOutput);
        //    //    return (false);
        //    //}

        //    bool success = RFMFunctions.InsertRowInMem(InMemTable_UNIV, InitialTbl);
        //    if (!success)
        //    {
        //        Console.WriteLine("Could not insert new row in the In-Memory Data Table");
        //        return (false);
        //    }

        //}
        //else
        //{
        //    // Inserted records (lines) will be deleted upstream...
        //    return false;
        //}
        //#endregion

        // DataRow drUNIV = _InMemTable_UNIV.Rows.Add();
        #endregion
    }
}
