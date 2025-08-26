using System;
using System.Data;

namespace RRDMRFMClasses
{
    public class ProcessLine_Core_Banking_T24
    {

        static DataTable BINsToCategories;
        static string CategoryID;

        #region ProcessLine
        /// <summary>
        /// Process a single line (T2401)
        /// </summary>
        public static bool ProcessLineT2401(string Operator, string Origin, string FileUID, int LineNumber, string Line,
                                            DataTable LayoutTbl, DataTable _BINsToCategories, 
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
                drUNIV["Operator"] = drRAW["Operator"] = Operator;                                          // From pgm parameter

                // UNIV, pretermined values
                drUNIV["ProcessedAtRMCycle"] = 0;                                       // Will be set by matching routines
                drUNIV["RRNumber"] = 0;                                                 // Leave empty
                drUNIV["FullTraceNo"] = "";                                             // Leave empty
                drUNIV["Twin"] = "";                                             // Leave empty
                drUNIV["Mask"] = 0;                                                     // Leave empty
                drUNIV["ItHasException"] = false;                                       // Leave empty
                drUNIV["UniqueRecordId"] = 0;                                           // Leave empty

                // UNIV, derived values, initialize here
                drUNIV["MatchingCateg"] = "";               // Retrieve from lookup table 

                drUNIV["TerminalId"] = "";                  // Retrieve from Line
                drUNIV["TransType"] = 0;                    // Retrieve from Line
                drUNIV["TransDescr"] = "";                  // Retrieve from Line
                drUNIV["TransTypeAtOrigin"] = "";           // Retrieve from Line
                drUNIV["CardNumber"] = "";                  // Retrieve from Line
                drUNIV["AccNo"] = "";                       // Retrieve from Line
                drUNIV["TransCurr"] = "";                   // Retrieve from Line - Use dict to translate form numeric
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
                            // No need to convert to specific type as it is 'character'
                            drRAW[sourceFieldNm] = Xtr.ValueRAW.Trim();

                            if (sourceFieldNm == "TRANSDATETIME")
                            {
                                DateTime dt;
                                string sdt = drRAW["TRANSDATETIME"].ToString().Substring(0, 10);
                                if (DateTime.TryParse(sdt, out dt))
                                    drRAW["TransDate"] = dt;
                                else
                                    drRAW["TransDate"] = DateTime.MinValue;
                            }
                            if (sourceFieldNm == "TRACENUM")
                            {
                                string v = drRAW["TRACENUM"].ToString();
                                int trNum = 0;
                                if (int.TryParse(v, out trNum))
                                    drRAW["TransTraceNumber"] = trNum;
                                else
                                    drRAW["TransTraceNumber"] = 0;
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
                    drRAW["TerminalIdentification"] = drRAW["TERMINALID"].ToString();
                    _InMemTable_RAW.Rows.Add(drRAW);

                    // Use 'CARDNUM' from RAW to get Matching Category
                    string BIN = drRAW["CARDNUM"].ToString().Substring(0, 6);
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
                    string xprsn = "ATMNo='" + drUNIV["TerminalId"] + "'";
                    DataRow[] rowsFound = AtmsTbl.Select(xprsn);
                    if (rowsFound.Length > 0)
                    {
                        // Add the row in the memory DataTable
                        _InMemTable_UNIV.Rows.Add(drUNIV);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Exception in ProcessLineT2401! {0}", ex.Message);
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
    }
}
