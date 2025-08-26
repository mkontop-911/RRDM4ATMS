using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRDMRFMClasses;

using RRDM4ATMs;
using System.Globalization;

namespace RRDMRFMClasses
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
        RawImportError,
        NotExpected
    }
    #endregion

    public enum FieldType
    {
        Character,
        Numeric,
        Decimal,
        DateTime,
        Date,
        Time
    }

    //public struct Extracted
    //{
    //    public bool Success;
    //    public string ValueRAW;
    //    public string ValueType;
    //    public string ValueString;
    //    public int ValueInt;
    //    public decimal ValueDecimal;
    //    public DateTime ValueDateTime;
    //    public DateTime ValueDate;
    //    public DateTime ValueTime;
    //};

    public class RFMFunctions
    {
        #region eXractDateFromString()
        public static DateTime eXractDateFromString(string In)
        {
            int scanlen = In.Length - 8;
            if (scanlen < 0) // throw an exception to be caught by the caller
            {
                try
                {
                    scanlen = scanlen / 0;
                }
                catch
                {
                    throw;
                }
            }
            for (int i = 0; i < scanlen; i++)
            {
                DateTime dt;
                string subs = In.Substring(i, 8);
                string subsF1 = subs.Substring(0, 4) + "-" + subs.Substring(4, 2) + "-" + subs.Substring(6, 2);
                string subsF2 = subs.Substring(0, 2) + "-" + subs.Substring(2, 2) + "-" + subs.Substring(4, 4);

                // Test for YYYYMMDD
                if (DateTime.TryParse(subsF1, out dt))
                {
                    return (dt);
                }

                // Test for DDMMYYYY
                if (DateTime.TryParse(subsF2, out dt))
                {
                    return (dt);
                }
            }

            // Could not convert, notify caller with exception
            try
            {
                scanlen = scanlen / 0;
            }
            catch
            {
                throw;
            }

            return (DateTime.MinValue); // Unreachable but placed here to keep the compiler happy
        }
        #endregion

        #region RecordEventMsg
        public static void RecordEventMsg(string msg, EventLogEntryType type)
        {
            EventLogging.WriteEventLog("RRDMFileMonitor", type, msg);
            if (Environment.UserInteractive)
                Console.WriteLine(msg);
        }
        public static void RecordEventMsg(string msg, EventLogEntryType type, bool SkipWinEvent)
        {
            if (!SkipWinEvent)
                EventLogging.WriteEventLog("RRDMFileMonitor", type, msg);
            if (Environment.UserInteractive)
                Console.WriteLine(msg);
        }

        #endregion

        #region Archive File
        public static bool ArchiveFile(string srcFullPath, string archFullPath)
        {
            string msg = "";
            bool successArchive = true;

            try
            {
                File.Copy(srcFullPath, archFullPath, true);
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
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Information);
            }
            else
            {
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
            }
            return (successArchive);
        }
        #endregion

        #region // RenameFileToFailed
        //public static string RenameFileToFailed(string fileFullName)
        //{
        //    //string msg;
        //    string failedFileName = "";

        //    try
        //    {
        //        failedFileName = Path.ChangeExtension(fileFullName, Path.GetExtension(fileFullName) + ".Failed"); // Network drives?
        //        File.Move(fileFullName, failedFileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Rename failed! Same filename and extension must already exist. Delete this one...
        //        File.Delete(fileFullName);
        //        //msg = string.Format("Exception while renaming file [{0}] to [{1}]! The file is deleted.\r\nException Message: {2}", fileFullName, failedFileName, ex.Message);
        //        //RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
        //    }
        //    return (failedFileName);
        //}
        #endregion

        #region MoveFileToExceptionsDir
        public static string MoveFileToExceptionsDir(string fileFullName)
        {
            string msg;
            string failedFileName = "";

            string excDir = RFMStartPoint.rsf.ExceptionsDirectory;
            failedFileName = Path.Combine(excDir, Path.GetFileName(fileFullName)); // Network drives?

            try
            {
                if (File.Exists(failedFileName))
                {
                    File.Delete(failedFileName);
                }
                File.Move(fileFullName, failedFileName);
            }
            catch (Exception ex)
            {
                // Move to Exceptions failed! Must have been an IO error... Delete the original
                File.Delete(fileFullName);
                msg = string.Format("Exception while moving file [{0}] to [{1}]! The file is deleted.\r\nException Message: {2}", fileFullName, failedFileName, ex.Message);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
            }
            return (failedFileName);
        }
        #endregion

        #region DeleteInsertedRecords(string FUID)
        public static bool DeleteInsertedRecords(string FUID, string tblName)
        {
            string msg;
            RRDMMatchingTxns_InGeneralTables Tbl = new RRDMMatchingTxns_InGeneralTables();
            Tbl.DeleteRecordsByOriginFile(FUID, tblName);
            if (Tbl.ErrorFound)
            {
                msg = string.Format("An error occured while deleting invalidated records from table [{0}] for rows where [OriginFileName=[{1}]." +
                                    "\nManual intervention is required!\nThe error message from SQL reads: \n{2}",
                                     tblName, FUID, Tbl.ErrorOutput);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return false;
            }
            return true;
        }
        #endregion

        #region LogExpressionFailure
        public static void LogExpressionFailure(string fullFileName, int LineNumber, string expression)
        {
            DateTime dt = DateTime.Now;
            string fileName = Path.GetFileName(fullFileName);
            string sdt = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //string sdt = string.Format("{0}-{1}-{2} {3}:{4}:{5},{6}",
            //                            dt.Year.ToString("0000"), dt.Month.ToString("00"), dt.Day.ToString("00"),
            //                            dt.Hour.ToString("00"), dt.Minute.ToString("00"), dt.Second.ToString("00"), dt.Millisecond.ToString("000"));
            string FailedFileNameLog = fullFileName + ".Failed.Log";
            string msg = string.Format("{0} {1}, Line:{2}: Error reading field attributes from datatable! Failing expression: [{3}]\r\n",
                                        sdt, fileName, LineNumber.ToString("000000"), expression);
            File.AppendAllText(FailedFileNameLog, msg);
            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error, true); //Display if UserInteractive but skip EventLog
        }
        #endregion

        #region LogConversionFailure
        public static void LogConversionFailure(string fullFileName, int LineNumber, string TargetFieldName, string TargetFieldType, string SourceValue)
        {
            string fileName = Path.GetFileName(fullFileName);
            DateTime dt = DateTime.Now;
            string sdt = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //string sdt = string.Format("{0}-{1}-{2} {3}:{4}:{5},{6}",
            //                            dt.Year.ToString("0000"), dt.Month.ToString("00"), dt.Day.ToString("00"),
            //                            dt.Hour.ToString("00"), dt.Minute.ToString("00"), dt.Second.ToString("00"), dt.Millisecond.ToString("000"));
            string FailedFileNameLog = fullFileName + ".Failed.Log";
            string msg = string.Format("{0} {1}, Line:{2}: Invalid source value! Could not convert extracted value: [{3}] to [{4}]. The target field is: [{5}]\r\n",
                                        sdt, fileName, LineNumber.ToString("000000"), SourceValue, TargetFieldType, TargetFieldName);
            File.AppendAllText(FailedFileNameLog, msg);
            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error, true); // true = Display if UserInteractive but skip EventLog
        }
        #endregion

        #region LogConversionException
        public static void LogConversionException(string fullFileName, int LineNumber, string Expression, string exMsg)
        {
            string fileName = Path.GetFileName(fullFileName);
            DateTime dt = DateTime.Now;
            string sdt = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string FailedFileNameLog = fullFileName + ".Failed.Log";
            string msg = string.Format("{0} {1}, Line:{2} Exception raised! Expression is=[{3}]. The exception message is: {4}\r\n",
                                        sdt, fileName, LineNumber.ToString("000000"), Expression, exMsg);
            File.AppendAllText(FailedFileNameLog, msg);
            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error, true); // true = Display if UserInteractive but skip EventLog
        }
        #endregion

        #region Get Field Layout Attributes
        public static RRDMMappingFileFieldsFromBankToRRDM.SourceFileLayoutStruct GetFieldLayoutAttributes(DataRow[] LayoutRow)
        {
            RRDMMappingFileFieldsFromBankToRRDM.SourceFileLayoutStruct FieldAttrib = new RRDMMappingFileFieldsFromBankToRRDM.SourceFileLayoutStruct();
            try
            {
                FieldAttrib.SeqNo = (int)LayoutRow[0]["SeqNo"];
                FieldAttrib.SourceFileId = LayoutRow[0]["SourceFileId"].ToString();
                FieldAttrib.SourceFieldNm = LayoutRow[0]["SourceFieldNm"].ToString();
                FieldAttrib.SourceFieldValue = LayoutRow[0]["SourceFieldValue"].ToString();
                FieldAttrib.SourceFieldPositionStart = (int)LayoutRow[0]["SourceFieldPositionStart"];
                FieldAttrib.SourceFieldPositionEnd = (int)LayoutRow[0]["SourceFieldPositionEnd"];
                FieldAttrib.TargetFieldNm = LayoutRow[0]["TargetFieldNm"].ToString();
                FieldAttrib.TargetFieldType = LayoutRow[0]["TargetFieldType"].ToString();
                FieldAttrib.TargetFieldValue = LayoutRow[0]["TargetFieldValue"].ToString();
                //FieldAttrib.RoutineValidation = (bool)LayoutRow[0]["RoutineValidation"];
                //FieldAttrib.RoutineNm = LayoutRow[0]["RoutineNm"].ToString();
            }
            catch (Exception ex)
            {
                FieldAttrib.Exists = false;
                FieldAttrib.SeqNo = 0;
                FieldAttrib.SourceFileId = "";
                FieldAttrib.SourceFieldNm = "";
                FieldAttrib.SourceFieldValue = "";
                FieldAttrib.SourceFieldPositionStart = 0;
                FieldAttrib.SourceFieldPositionEnd = 0;
                FieldAttrib.TargetFieldNm = "";
                FieldAttrib.TargetFieldType = "";
                FieldAttrib.TargetFieldValue = "";
                //FieldAttrib.RoutineValidation = false;
                //FieldAttrib.RoutineNm = "";
                return (FieldAttrib);
            }
            FieldAttrib.Exists = true;
            return (FieldAttrib);
        }
        #endregion

        #region Extract Field Value from Line 
        /// <summary>
        /// Extract Value from source Line and assign in appropriate element of structure: 'Extracted'
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="LayoutTbl"></param> //The entire layout table, not just the Row, because there may be dependencies with other fields (other rows)
        /// <param name="LineNumber"></param>
        /// <param name="Line"></param>
        /// <param name="TargetFieldType"></param>
        /// <param name="fullFileName"></param>
        /// <returns>struct:Exctracted</returns>
        public static Extracted ExtractFieldValue(string expression, DataTable LayoutTbl, int LineNumber, string Line, FieldType TargetFieldType, string fullFileName)
        {
            string XtractedValue = "";
            string XtractedRAW = "";
            int i;
            decimal dec;
            DateTime dt;
            int FieldStart;
            int FieldLength;

            DataRow[] LayoutR = null;
            RRDMMappingFileFieldsFromBankToRRDM.SourceFileLayoutStruct FieldAttrib; // Struct to hold field attributes, derived from 'LayoutR'
            Extracted Xtract = new Extracted();
            Xtract.ValueRAW = "";
            Xtract.Success = false;
            Xtract.ValueString = "";
            Xtract.ValueInt = 0;
            Xtract.ValueDecimal = 0;
            Xtract.ValueDateTime = DateTime.MinValue; /// ???

            try
            {
                // Select row in table as per 'expression'
                LayoutR = LayoutTbl.Select(expression);

                if (LayoutR.Length == 1) // There should only be one row; many rows make it ambiguous...
                {
                    FieldAttrib = GetFieldLayoutAttributes(LayoutR); // Retrieve attributes and place in 'FieldAttrib'
                    if (!FieldAttrib.Exists)
                    {
                        RFMFunctions.LogExpressionFailure(fullFileName, LineNumber, expression);
                        return (Xtract);
                    }
                }
                else
                {
                    RFMFunctions.LogExpressionFailure(fullFileName, LineNumber, expression);
                    return (Xtract);
                }

                Xtract.ValueType = FieldAttrib.TargetFieldType;
                string SrcFldVal = FieldAttrib.SourceFieldValue;

                switch (SrcFldVal)
                {
                    // When it falls through we have XtractedRAW and XtractedValue populated...
                    #region Rtn_Fixed (916,2)
                    case "Rtn_Fixed":
                        {
                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
                            if (FieldAttrib.SourceFieldNm != "NotPresent")
                            {
                                XtractedRAW = Line.Substring(FieldStart, FieldLength);
                            }

                            XtractedValue = FieldAttrib.TargetFieldValue;

                            break;
                        }
                    #endregion
                    #region Rtn_ICBS-Numeric (916,3)
                    case "Rtn_ICBS-Numeric":
                        {
                            string srcVal = "";
                            decimal srcDec;

                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;

                            if (FieldAttrib.SourceFieldNm != "NotPresent")
                            {
                                srcVal = Line.Substring(FieldStart, FieldLength);
                                XtractedRAW = srcVal;
                            }

                            if (Decimal.TryParse(srcVal, out srcDec))
                            {
                                int srcInt = (int)srcDec;
                                XtractedValue = srcInt.ToString();
                            }
                            else
                            {
                                return (Xtract);
                            }
                            break;
                        }
                    #endregion
                    #region Rtn_ICBS-DateTime (916,4)
                    case "Rtn_ICBS-DateTime":
                        {
                            // contruct the XtractedValue string
                            // source format:        [20,170,516      92,103]
                            // XtractedValue format: [16-05-2017 09:21:03]

                            string srcDate = "19,000,101";
                            string srcTime = "00,000";
                            decimal decDate, decTime;
                            int intDate;
                            int intTime;
                            string strDate, strTime;

                            // ToDo: Validation, see below
                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                XtractedRAW = Line.Substring(FieldStart, FieldLength);
                                srcDate = Line.Substring(FieldStart, 10);
                                srcTime = Line.Substring(FieldStart + 15, 7);
                            }

                            if (Decimal.TryParse(srcDate, out decDate))
                            {
                                intDate = (int)decDate;
                                strDate = string.Format("{0:0000-00-00}", intDate);
                            }
                            else
                            {
                                return (Xtract);
                            }
                            if (Decimal.TryParse(srcTime, out decTime))
                            {
                                intTime = (int)decTime;
                                strTime = string.Format("{0:00:00:00}", intTime);
                            }
                            else
                            {
                                return (Xtract);
                            }
                            XtractedValue = string.Format("{0} {1}", strDate, strTime);
                            break;
                        }
                    #endregion
                    #region Rtn_IST-Account (916,5)
                    case "Rtn_IST-Account":
                        {
                            string srcVal = "";
                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;

                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                srcVal = Line.Substring(FieldStart, FieldLength);
                                XtractedRAW = srcVal;
                            }

                            try
                            {
                                int cutFrom = srcVal.IndexOf("|");
                                if (cutFrom != -1)
                                    XtractedValue = srcVal.Substring(0, cutFrom);
                                else
                                    XtractedValue = srcVal;
                            }
                            catch (Exception ex) //ArgumentOutOfRangeException
                            {
                                Xtract.Success = false;
                                return (Xtract);
                            }
                            break;
                        }
                    #endregion
                    #region Rtn_IST-ResponseCode (916,6)
                    case "Rtn_IST-ResponseCode":
                        {
                            string srcVal = "";
                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                srcVal = Line.Substring(FieldStart, FieldLength);
                                XtractedRAW = srcVal;
                            }
                            try
                            {
                                int cutFrom = srcVal.IndexOf(" - ");
                                if (cutFrom != -1)
                                    XtractedValue = srcVal.Substring(0, cutFrom);
                                else
                                    XtractedValue = "0";
                            }
                            catch (Exception ex)
                            {
                                XtractedValue = "0";
                                XtractedRAW = "";
                            }
                            break;
                        }
                    #endregion
                    #region Rtn_IST-FXAmtEquiv (916,7)
                    case "Rtn_IST-FXAmtEquiv":
                        {
                            // Set to TransAmt if AmtEquiv = 0
                            string srcVal = "0";
                            string srcAmt;
                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                srcVal = Line.Substring(FieldStart, FieldLength);
                                XtractedRAW = srcVal;
                            }

                            srcAmt = srcVal.Trim();
                            if (Decimal.TryParse(srcAmt, out dec))
                            {
                                if (dec != 0)
                                {
                                    // Not zero, use this 
                                    XtractedValue = srcAmt;
                                    break;
                                }
                                else
                                {
                                    // ToDo: error handling
                                    // Zero, use the TransAmt 
                                    DataRow[] LayoutTransAmt = null; // To hold the layout of the line
                                    RRDMMappingFileFieldsFromBankToRRDM.SourceFileLayoutStruct TransAmtAttrib; // Struct to hold field attributes, retrieved from 'LayoutR'
                                    LayoutTransAmt = LayoutTbl.Select("TargetFieldNm = 'TransAmt'");
                                    TransAmtAttrib = GetFieldLayoutAttributes(LayoutTransAmt); // Retrieve attributes and place in 'FieldAttrib'
                                    if (!TransAmtAttrib.Exists)
                                    {
                                        RFMFunctions.LogExpressionFailure(fullFileName, LineNumber, "TargetFieldNm = 'TransAmt'");
                                        Xtract.Success = false;
                                        return (Xtract);
                                    }
                                    FieldStart = TransAmtAttrib.SourceFieldPositionStart - 1;
                                    FieldLength = TransAmtAttrib.SourceFieldPositionEnd - FieldStart;
                                    XtractedRAW = XtractedValue = Line.Substring(FieldStart, FieldLength);
                                    break;
                                }
                            }
                            else
                            {
                                Xtract.Success = false;
                                RFMFunctions.LogConversionFailure(fullFileName, LineNumber, FieldAttrib.TargetFieldNm, "IST-AmtEquiv", srcAmt);
                            }
                            break;
                        }
                    #endregion
                    #region Rtn_NBG-TransType (916,8)
                    case "Rtn_NBG-TransType":
                        {
                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                XtractedRAW = Line.Substring(FieldStart, FieldLength);
                            }
                            switch (XtractedRAW)
                            {
                                case "0100":
                                case "0200":
                                case "0300":
                                case "0500":
                                case "0700":
                                case "3000":
                                case "4000":
                                    {
                                        XtractedValue = "11";
                                        break;
                                    }
                                case "0110":
                                case "0210":
                                case "0310":
                                case "0510":
                                case "0710":
                                case "3010":
                                case "4010":
                                    {
                                        XtractedValue = "22";
                                        break;
                                    }
                                case "0120":
                                case "0220":
                                case "0320":
                                case "0520":
                                case "0720":
                                case "3020":
                                case "4020":
                                    {
                                        // XtractedValue = "23"; The 110 is reversal and the 120 is suspect reversal.
                                        XtractedValue = "22";
                                        break;
                                    }
                                default:
                                    {
                                        XtractedValue = "99"; //irrelevant, the record will be dropped (not included in UNIV table)
                                        break;
                                    }
                            }
                            break;
                        }
                    #endregion
                    #region Rtn_NBG-CurrencyLookup (916,9)
                    case "Rtn_NBG-CurrencyLookup":
                        {
                            string srcVal = "";
                            string translatedlVal;

                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                srcVal = Line.Substring(FieldStart, FieldLength);
                                XtractedRAW = srcVal;
                            }

                            translatedlVal = CurrencyList.GetValue(srcVal);
                            if (translatedlVal != null)
                                XtractedValue = translatedlVal;
                            else
                                XtractedValue = "";
                            break;
                        }
                    #endregion
                    #region Rtn_NBG-TransDateTime (916,10)
                    case "Rtn_NBG-TransDateTime":
                        {
                            // contruct the XtractedValue string
                            // source format:        [170911] + [W_TRAN_TIME: '20:28:16']
                            // XtractedValue format: [11-09-2017 20:28:16]

                            string srcDate = "000101";
                            string srcTime = "00:00:00";
                            //decimal decDate, decTime;
                            //int intDate;
                            //int intTime;
                            //string strDate, strTime;

                            // ToDo: Validation, see below
                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                XtractedRAW = Line.Substring(FieldStart, FieldLength);
                                srcDate = Line.Substring(FieldStart, 6);

                                // We need to extract W_TRAN_TIME and combine the two
                                DataRow[] LayoutTranTime = null; // To hold the layout of the line
                                RRDMMappingFileFieldsFromBankToRRDM.SourceFileLayoutStruct TranTimeAttrib; // Struct to hold field attributes, retrieved from 'LayoutR'
                                LayoutTranTime = LayoutTbl.Select("TargetFieldNm = 'W_TRAN_TIME'");
                                TranTimeAttrib = GetFieldLayoutAttributes(LayoutTranTime); // Retrieve attributes and place in 'FieldAttrib'
                                if (!TranTimeAttrib.Exists)
                                {
                                    RFMFunctions.LogExpressionFailure(fullFileName, LineNumber, "TargetFieldNm = 'W_TRAN_TIME'");
                                    Xtract.Success = false;
                                    return (Xtract);
                                }
                                int fldStart = TranTimeAttrib.SourceFieldPositionStart - 1;
                                int fldLength = TranTimeAttrib.SourceFieldPositionEnd - fldStart;
                                srcTime = XtractedValue = Line.Substring(fldStart, fldLength);
                            }
                            string y = "20" + srcDate.Substring(0, 2);
                            string m = srcDate.Substring(2, 2);
                            string d = srcDate.Substring(4, 2);
                            XtractedValue = d + "-" + m + "-" + y + " " + srcTime;
                            break;
                        }
                    #endregion
                    #region Rtn_NBG-Date (916,11)
                    case "Rtn_NBG-Date":
                        {
                            // contruct the XtractedValue string
                            // source format:        [250917]
                            // XtractedValue format: [25-09-2017]

                            string srcDate = "010100";

                            // ToDo: Validation, see below
                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;

                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                srcDate = Line.Substring(FieldStart, FieldLength);
                                XtractedRAW = srcDate;
                            }

                            string y = "20" + srcDate.Substring(0, 2);
                            string m = srcDate.Substring(2, 2);
                            string d = srcDate.Substring(4, 2);
                            XtractedValue = d + "-" + m + "-" + y;
                            break;
                        }
                    #endregion
                    #region Rtn_NBG-Time (916,12)
                    case "Rtn_NBG-Time":
                        {
                            // contruct the XtractedValue string
                            // source format:        [23:51:25]
                            // XtractedValue format: [01-01-0001 23:51:25]

                            string srcTime = "00:00:00";

                            // ToDo: Validation, see below
                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                XtractedRAW = Line.Substring(FieldStart, FieldLength);
                                srcTime = "01-01-0001 " + Line.Substring(FieldStart, FieldLength);
                            }

                            XtractedValue = srcTime;
                            break;
                        }
                    #endregion
                    #region Rtn_NBG-DateInv (916,13)
                    case "Rtn_NBG-DateInv":
                        {
                            // contruct the XtractedValue string
                            // source format:        [170925]
                            // XtractedValue format: [25-09-2017]

                            string srcDate = "000101";

                            // ToDo: Validation, see below
                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;

                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                srcDate = Line.Substring(FieldStart, FieldLength);
                                XtractedRAW = srcDate;
                            }
                            string y = "20" + srcDate.Substring(4, 2);
                            string m = srcDate.Substring(2, 2);
                            string d = srcDate.Substring(0, 2);

                            XtractedValue = d + "-" + m + "-" + y;
                            break;
                        }
                    #endregion
                    #region Rtn_NBG-DecPIC10 (916,14)
                    case "Rtn_NBG-DecPIC10":
                        {
                            string srcVal = "0000000000";
                            decimal srcDec;

                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;


                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                srcVal = Line.Substring(FieldStart, FieldLength);
                                XtractedRAW = srcVal;
                            }
                            if (Decimal.TryParse(srcVal, out srcDec))
                            {
                                decimal xDec = srcDec / 100;

                                XtractedValue = xDec.ToString();
                            }
                            else
                            {
                                return (Xtract);
                            }
                            break;
                        }
                    #endregion
                    #region Rtn_AmtComma (916,15)
                    case "Rtn_AmtComma":
                        {
                            string srcVal = "0000000000";
                            decimal srcDec;

                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;


                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                srcVal = Line.Substring(FieldStart, FieldLength);
                                XtractedRAW = srcVal;
                            }
                            if (Decimal.TryParse(srcVal, out srcDec))
                            {
                                decimal xDec = srcDec / 100;

                                XtractedValue = xDec.ToString();
                            }
                            else
                            {
                                return (Xtract);
                            }
                            break;
                        }
                    #endregion
                    #region Rtn_Default (916,1) / default
                    case "Rtn_Default":
                    default:
                        {
                            ////Validate Start / Length
                            //if (FieldAttrib.SourceFieldPositionStart < 1 || (FieldAttrib.SourceFieldPositionEnd < FieldAttrib.SourceFieldPositionStart))
                            //{
                            //    string msg = string.Format("Error found in file : '{0}', Line: {1}! " +
                            //             "\nError: Invalid attributes when extracting: '{2}'!\nStart Position: {3}\nEnd Position: {4}",
                            //             fullFileName, LineNumber, expression, FieldAttrib.SourceFieldPositionStart, FieldAttrib.SourceFieldPositionEnd);
                            //    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                            //    return (Xtract);
                            //}
                            FieldStart = FieldAttrib.SourceFieldPositionStart - 1;
                            FieldLength = FieldAttrib.SourceFieldPositionEnd - FieldStart;
                            if (FieldAttrib.TargetFieldNm != "NotPresent")
                            {
                                XtractedValue = "";
                                XtractedRAW = XtractedValue;

                                try
                                {
                                    XtractedValue = Line.Substring(FieldStart, FieldLength);
                                    XtractedRAW = XtractedValue;
                                }
                                catch (ArgumentOutOfRangeException ex)
                                {
                                    if (FieldAttrib.IsUniversal)
                                        throw;
                                }
                            }
                            break;
                        }
                        #endregion
                }
                // Trim white space
                string XtractedTrimmed = XtractedValue.Trim();
                Xtract.ValueRAW = XtractedRAW.Trim();

                // XtractedTrimmed in a string representation of the corresponding type 
                switch (TargetFieldType)
                {
                    #region Numeric
                    case FieldType.Numeric:
                        {
                            if (Int32.TryParse(XtractedTrimmed, out i))
                            {
                                Xtract.ValueInt = i;
                                Xtract.Success = true;
                            }
                            else
                            {
                                Xtract.Success = false;
                                RFMFunctions.LogConversionFailure(fullFileName, LineNumber, FieldAttrib.TargetFieldNm, "Int32", XtractedTrimmed);
                            }
                            break;
                        }
                    #endregion
                    #region Decimal
                    case FieldType.Decimal:
                        {
                            if (Decimal.TryParse(XtractedTrimmed, out dec))
                            {
                                Xtract.ValueDecimal = dec;
                                Xtract.Success = true;
                            }
                            else
                            {
                                Xtract.Success = false;
                                RFMFunctions.LogConversionFailure(fullFileName, LineNumber, FieldAttrib.TargetFieldNm, "Decimal", XtractedTrimmed);
                            }
                            break;
                        }
                    #endregion
                    #region DateTime
                    case FieldType.DateTime:
                        {
                            // [dd-MM-yyyy HH:mm:ss] or [yyyy-MM-dd HH:mm:ss]

                            // if (DateTime.TryParse(XtractedTrimmed, out dt))
                            if (DateTime.TryParseExact(XtractedTrimmed, "dd-MM-yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dt))
                            {
                                Xtract.ValueDateTime = dt;
                                Xtract.Success = true;
                            }
                            else if (DateTime.TryParseExact(XtractedTrimmed, "yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dt))
                            {
                                Xtract.ValueDateTime = dt;
                                Xtract.Success = true;
                            }
                            else
                            {
                                Xtract.Success = false;
                                RFMFunctions.LogConversionFailure(fullFileName, LineNumber, FieldAttrib.TargetFieldNm, "DateTime", XtractedTrimmed);
                            }
                            break;
                        }
                    #endregion
                    #region Date
                    case FieldType.Date:
                        {
                            // [dd-MM-yyyy]
                            // if (DateTime.TryParse(XtractedTrimmed, out dt))
                            if (DateTime.TryParseExact(XtractedTrimmed, "dd-MM-yyyy", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dt))
                            {
                                Xtract.ValueDate = dt;
                                Xtract.Success = true;
                            }
                            else
                            {
                                Xtract.Success = false;
                                RFMFunctions.LogConversionFailure(fullFileName, LineNumber, FieldAttrib.TargetFieldNm, "Date", XtractedTrimmed);
                            }
                            break;
                        }
                    #endregion
                    #region Time
                    case FieldType.Time:
                        {
                            // [HH:mm:ss]
                            // if (DateTime.TryParse(XtractedTrimmed, out dt))
                            if (DateTime.TryParseExact(XtractedTrimmed, "HH:mm:ss", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dt))
                            {
                                Xtract.ValueTime = dt.ToUniversalTime();
                                Xtract.Success = true;
                            }
                            else
                            {
                                Xtract.Success = false;
                                RFMFunctions.LogConversionFailure(fullFileName, LineNumber, FieldAttrib.TargetFieldNm, "Time", XtractedTrimmed);
                            }
                            break;
                        }
                    #endregion
                    #region Character / Default
                    case FieldType.Character:
                    default:
                        {
                            Xtract.ValueString = XtractedTrimmed;
                            Xtract.Success = true;
                            break;
                        }
                        #endregion
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Exception ex1 = ex.InnerException;
                while (ex1 != null)
                {
                    msg += " - " + ex1.Message;
                }
                //ToDo: Handle Exception ?
                Xtract.Success = false;
                LogConversionException(fullFileName, LineNumber, expression, msg);
                return (Xtract);
            }
            return (Xtract);
        }
        #endregion

        #region Get Matching CategoryId from BIN
        public static string GetMatchingCategoryFromBIN(string BIN, DataTable BINsToCategories)
        {
            string expression;
            string CategoryId = "";
            DataRow[] Row;

            expression = "BIN = " + BIN;
            try
            {
                Row = BINsToCategories.Select(expression);
                if (Row.Length != 1) // only a single row must heve been returned.. otherwise there are duplicate rows which should not have happened
                {
                    // Error will handled upstream...
                    return ("");
                }
                CategoryId = Row[0]["CategoryId"].ToString();
                return (CategoryId);
            }
            catch
            {
                // Error will handled upstream...
                return ("");
            }
        }
        #endregion

        #region Insert new row in UNIV InMem DataTable
        public static bool InsertRowInMem(DataTable InMemdataTbl, RRDMMatchingTxns_InGeneralTables InitialTbl)
        {
            bool rc = false;
            try
            {
                DataRow InMemRow = InMemdataTbl.NewRow();
                InMemRow["SeqNo"] = InitialTbl.SeqNo;
                InMemRow["OriginFileName"] = InitialTbl.OriginFileName;
                InMemRow["OriginalRecordId"] = InitialTbl.OriginalRecordId;
                InMemRow["MatchingCateg"] = InitialTbl.MatchingCateg;
                InMemRow["Origin"] = InitialTbl.Origin;
                InMemRow["TransTypeAtOrigin"] = InitialTbl.TransTypeAtOrigin;
                InMemRow["TerminalId"] = InitialTbl.TerminalId;
                InMemRow["TransType"] = InitialTbl.TransType;
                InMemRow["TransDescr"] = InitialTbl.TransDescr;
                InMemRow["CardNumber"] = InitialTbl.CardNumber;
                InMemRow["AccNo"] = InitialTbl.AccNo;
                InMemRow["TransCurr"] = InitialTbl.TransCurr;
                InMemRow["TransAmt"] = InitialTbl.TransAmt;
                InMemRow["AmtFileBToFileC"] = InitialTbl.AmtFileBToFileC;
                InMemRow["TransDate"] = InitialTbl.TransDate;
                InMemRow["TraceNo"] = InitialTbl.TraceNo;
                InMemRow["RRNumber"] = InitialTbl.RRNumber;
                InMemRow["ResponseCode"] = InitialTbl.ResponseCode;
            //    InMemRow["T24RefNumber"] = InitialTbl.T24RefNumber;
                InMemRow["Processed"] = InitialTbl.Processed;
                InMemRow["ProcessedAtRMCycle"] = InitialTbl.ProcessedAtRMCycle;
                InMemRow["Operator"] = InitialTbl.Operator;
                InMemdataTbl.Rows.Add(InMemRow);
                rc = true;
            }
            catch (Exception ex)
            {
                RFMFunctions.RecordEventMsg(ex.Message, EventLogEntryType.Error);
                rc = false;
            }
            return (rc);
        }
        #endregion

        #region // Create UNIV InMem DataTable
        //public static DataTable CreateUNIV_MemTable()
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

        #region // ProcessLine
        ///// <summary>
        ///// Process a sigle Line from the source File...
        ///// </summary>
        ///// <param name="Origin"></param>
        ///// <param name="FileUID"></param>
        ///// <param name="LineNumber"></param>
        ///// <param name="Line"></param>
        ///// <param name="Line"></param>
        ///// <param name="LayoutTbl"></param>
        ///// <returns>true/false</returns>
        //public static bool ProcessLine(string Operator, string Origin, string FileUID, int LineNumber, string Line, DataTable LayoutTbl, 
        //                               DataTable _BINsToCategories, string _CategoryID, DataTable _InMemTable_UNIV, string fullFileName)
        //{
        //    BINsToCategories = _BINsToCategories;
        //    CategoryID = _CategoryID;
        //    InMemTable_UNIV = _InMemTable_UNIV;


        //    // string msg;
        //    string SelectExpression;
        //    Extracted Xtr;
        //    bool ExtractionErrorsFound = false;

        //    ReconcInitialTable InitialTbl = new ReconcInitialTable();
        //    InitialTbl.OriginFileName = FileUID;        // Generated, points into ReconcFileMonitorLog
        //    InitialTbl.OriginalRecordId = LineNumber;   // To identify the line where the record came from
        //    InitialTbl.Origin = Origin;                 // SystemOfOrigin
        //    InitialTbl.Operator = Operator;             // From pgm parameter
        //    InitialTbl.ProcessedAtRMCycle = 0;          // Will be set by matching routines
        //    InitialTbl.RRNumber = 0;                    // Leave empty
        //    InitialTbl.T24RefNumber = "";               // Leave empty

        //    InitialTbl.MatchingCateg = "";      // Retrieve from lookup table 

        //    InitialTbl.TerminalId = "";         // Retrieve from Line
        //    InitialTbl.TransType = 0;           // Retrieve from Line
        //    InitialTbl.TransDescr = "";         // Retrieve from Line
        //    InitialTbl.TransTypeAtOrigin = "";  // Retrieve from Line
        //    InitialTbl.CardNumber = "";         // Retrieve from Line
        //    InitialTbl.AccNo = "";              // Retrieve from Line
        //    InitialTbl.TransCurr = "";          // Retrieve from Line
        //    InitialTbl.TransAmt = 0;            // Retrieve from Line
        //    InitialTbl.AmtFileBToFileC = 0;     // Retrieve from Line
        //    InitialTbl.TransDate = DateTime.MinValue;   // Retrieve from Line
        //    InitialTbl.TraceNo = 0;             // Retrieve from Line
        //    InitialTbl.ResponseCode = "";        // Retrieve from Line

        //    InitialTbl.Processed = false;

        //    #region Extract: TransDate (DATETIME)
        //    // DATETIME
        //    SelectExpression = "TargetFieldNm = 'TransDate'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.DateTime, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.TransDate = Xtr.ValueDateTime;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    #region Extract: TerminalID (CHAR)
        //    // CHAR
        //    SelectExpression = "TargetFieldNm = 'TerminalId'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.TerminalId = Xtr.ValueString;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    #region Extract: TransType (INT)
        //    // INT
        //    SelectExpression = "TargetFieldNm = 'TransType'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Numeric, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.TransType = Xtr.ValueInt;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    #region Extract: TransDescr
        //    // CHAR
        //    SelectExpression = "TargetFieldNm = 'TransDescr'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.TransDescr = Xtr.ValueString;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    #region Extract: TransTypeAtOrigin
        //    // CHAR
        //    SelectExpression = "TargetFieldNm = 'TransTypeAtOrigin'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.TransTypeAtOrigin = Xtr.ValueString;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    #region Extract: CardNumber
        //    // CHAR
        //    SelectExpression = "TargetFieldNm = 'CardNumber'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.CardNumber = Xtr.ValueString;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    #region Get Matching Category from Card Number
        //    if (InitialTbl.CardNumber.Length >= 6 && BINsToCategories != null) // there is a value to search the table for; the table exists
        //    {
        //        string BIN = InitialTbl.CardNumber.Substring(0, 6);
        //        string CatId = GetMatchingCategoryFromBIN(BIN);
        //        if (CatId != "")
        //        {
        //            InitialTbl.MatchingCateg = CatId;
        //        }
        //        else
        //        {
        //            // Covers IST: Get instructions on how to handle from 'MatchingCateg' SourceFieldvalue
        //            string srcFldValue;
        //            string expr = "TargetFieldNm = 'MatchingCateg'";
        //            DataRow[] lR = null; // To hold the layout of the line
        //            lR = LayoutTbl.Select(expr);
        //            RRDMMatchingFileFieldsFromBankToRRDM.SourceFileLayoutStruct FldAtr = RFMFunctions.GetFieldLayoutAttributes(lR);

        //            if (FldAtr.Exists)
        //            {
        //                srcFldValue = FldAtr.SourceFieldValue;
        //                switch (srcFldValue)
        //                {
        //                    case "Rtn_IsMissing": // We have instructions! Get the value from 'TargetFieldValue' 
        //                        {
        //                            InitialTbl.MatchingCateg = FldAtr.TargetFieldValue;
        //                            break;
        //                        }
        //                    default: // We do not have instructions
        //                        {
        //                            RFMFunctions.LogConversionFailure(fullFileName, LineNumber, "MatchingCategory", "TableLookup", InitialTbl.CardNumber);
        //                            ExtractionErrorsFound = true;
        //                            break;
        //                        }
        //                }
        //            }
        //            else
        //            {
        //                //   We do not have instructions
        //                RFMFunctions.LogConversionFailure(fullFileName, LineNumber, "MatchingCategory", "TableLookup", InitialTbl.CardNumber);
        //                ExtractionErrorsFound = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        InitialTbl.MatchingCateg = CategoryID;
        //    }
        //    #endregion

        //    #region Extract: AccNo
        //    // CHAR
        //    SelectExpression = "TargetFieldNm = 'AccNo'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.AccNo = Xtr.ValueString;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    #region Extract: TransCurr
        //    // CHAR
        //    SelectExpression = "TargetFieldNm = 'TransCurr'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.TransCurr = Xtr.ValueString;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    #region Extract: TransAmount (DEC)
        //    // DEC
        //    // SelectExpression = "TargetFieldNm = 'TransAmt'"; //ToDo: corrrect field name in db
        //    SelectExpression = "TargetFieldNm = 'TransAmt'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Decimal, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.TransAmt = Xtr.ValueDecimal;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    #region Extract: AmtFileBToFileC (DEC)
        //    // DEC
        //    SelectExpression = "TargetFieldNm = 'AmtFileBToFileC'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Decimal, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.AmtFileBToFileC = Xtr.ValueDecimal;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    #region Extract: TraceNo (INT)
        //    // INT
        //    SelectExpression = "TargetFieldNm = 'TraceNo'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Numeric, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.TraceNo = Xtr.ValueInt;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    #region // Extract: RRNumber (INT) - Deprecated !!!
        //    //// INT
        //    //SelectExpression = "TargetFieldNm = 'RRNumber'";
        //    //Xtr = ExtractSubString(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Numeric, fullFileName);
        //    //if (Xtr.Success)
        //    //    InitialTbl.RRNumber = Xtr.ValueInt;
        //    //else
        //    //    ExtractionErrorsFound = true;
        //    #endregion

        //    #region Extract: ResponseCode (INT)
        //    // INT
        //    SelectExpression = "TargetFieldNm = 'ResponseCode'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.ResponseCode = Xtr.ValueString;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    #region // Extract: T24RefNumber (CHAR) 
        //    // CHAR
        //    SelectExpression = "TargetFieldNm = 'T24RefNumber'";
        //    Xtr = ExtractFieldValue(SelectExpression, LayoutTbl, LineNumber, Line, FieldType.Character, fullFileName);
        //    if (Xtr.Success)
        //        InitialTbl.T24RefNumber = Xtr.ValueString;
        //    else
        //        ExtractionErrorsFound = true;
        //    #endregion

        //    if (!ExtractionErrorsFound)
        //    #region INSERT into the table
        //    {
        //        // InitialTbl.InsertSingle(rsf.InitialTableName);
        //        //if (InitialTbl.ErrorFound)
        //        //{
        //        //    EventLogging.WriteEventLog("RRDMFileMonitor", EventLogEntryType.Error, InitialTbl.ErrorOutput);
        //        //    Console.WriteLine(InitialTbl.ErrorOutput);
        //        //    return (false);
        //        //}

        //        bool success = InsertRowInMem(InMemTable_UNIV, InitialTbl);
        //        if (!success)
        //        {
        //            Console.WriteLine("Could not insert new row in the In-Memory Data Table");
        //            return (false);
        //        }

        //    }
        //    else
        //    {
        //        // Inserted records (lines) will be deleted upstream...
        //        return false;
        //    }
        //    #endregion

        //    return (true);
        //}
        #endregion

    }
}
