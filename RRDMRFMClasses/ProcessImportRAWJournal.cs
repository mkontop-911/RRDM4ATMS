using System;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using RRDM4ATMs;

namespace RRDMRFMClasses
{
    public class ProcessImportRAWJournal
    {
        #region Process Local Journal File
        // Returns FUID if successfull, -1 if not
        public static int ProcessLocalJournalFile(string stpName, string eJType, string BankID, string AtmNo, string fullFileName)
        {
            // int lineCount = 0;
            int retValue = -1;
            RRDMJTMIdentificationDetailsClass IdentClass = new RRDMJTMIdentificationDetailsClass();

            #region Get the corresponding IdentificationDetails record
            IdentClass.ReadJTMIdentificationDetailsByAtmNo(AtmNo);
            if (!IdentClass.RecordFound)
            {
                string msg = string.Format("Error while reading the record for ATM:'{0}' (ReadJTMIdentificationDetailsByAtmNo)!\r\nThe error message reads: {1}",
                                     AtmNo,
                                     IdentClass.ErrorOutput);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return (-1);
            }
            #endregion

            #region // Check if in sequence
            //DateTime lastEJ = IdentClass.LoadingCompleted.Date;
            //try
            //{
            //    DateTime dfDT;
            //    dfDT = RFMFunctions.eXractDateFromString(Path.GetFileName(fullFileName));
            //    // If Journal date < Jd.LoadingCompleted.Date then there is a problem and you do not process this journal. 
            //    if (dfDT < lastEJ)
            //    {
            //        string msg = string.Format("eJournal of ATM:'{0}' out of sequence!", AtmNo);
            //        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

            //        #region Update record in JTMIdentificationDetails table
            //        // The record has already been read in IdentClass; update only relevant fields
            //        IdentClass.SourceFileName = Path.GetFileName(fullFileName);
            //        IdentClass.SourceFilePath = Path.GetDirectoryName(fullFileName);

            //        IdentClass.ResultCode = -1;
            //        IdentClass.ResultMessage = "OutOfSequence";

            //        IdentClass.UpdateRecordInJTMIdentificationDetailsByID(IdentClass.SeqNo);
            //        if (IdentClass.ErrorFound)
            //        {
            //            msg = string.Format("Error in updating JTMIdentificationDetails for ATM:{0}. The error message reads: \n{1} ",
            //                                AtmNo,
            //                                IdentClass.ErrorOutput);
            //            RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
            //            return (-1);
            //        }
            //        #endregion

            //        return (-1);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    string msg = string.Format("Invalid eJournal file name for of ATM:'{0}'!", AtmNo);
            //    RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);

            //    #region Update record in JTMIdentificationDetails table
            //    // The record has already been read in IdentClass; update only relevant fields
            //    IdentClass.SourceFileName = Path.GetFileName(fullFileName);
            //    IdentClass.SourceFilePath = Path.GetDirectoryName(fullFileName);

            //    IdentClass.ResultCode = -1;
            //    IdentClass.ResultMessage ="InvalidFileName";

            //    IdentClass.UpdateRecordInJTMIdentificationDetailsByID(IdentClass.SeqNo);
            //    if (IdentClass.ErrorFound)
            //    {
            //        msg = string.Format("Error in updating JTMIdentificationDetails for ATM:{0}. The error message reads: \n{1} ",
            //                            AtmNo,
            //                            IdentClass.ErrorOutput);
            //        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
            //        return (-1);
            //    }
            //    #endregion

            //    return (-1);

            //}
            #endregion


            #region Invoke Stored Procedure

            retValue = InvokeStpForRAWImport(stpName, eJType, BankID, AtmNo, fullFileName);
            #endregion

            #region Update record in JTMIdentificationDetails table
            // The record has already been read in IdentClass; update only relevant fields

            IdentClass.SourceFileName = Path.GetFileName(fullFileName);
            IdentClass.SourceFilePath = Path.GetDirectoryName(fullFileName);
            IdentClass.DestnFilePath = fullFileName;

            DateTime dt = DateTime.Now;
            IdentClass.DateLastUpdated = dt;
            IdentClass.FileUploadRequestDt = dt;
            IdentClass.FileParseEnd = dt;
            IdentClass.LoadingCompleted = dt;

            IdentClass.ResultCode = 1;
            IdentClass.ResultMessage = "Ready";
            IdentClass.Operator = BankID;

            IdentClass.UpdateRecordInJTMIdentificationDetailsByID(IdentClass.SeqNo);
            if (IdentClass.ErrorFound)
            {
                string msg = string.Format("Error in updating JTMIdentificationDetails for ATM:{0}. The error message reads: \n{1} ",
                                    AtmNo,
                                    IdentClass.ErrorOutput);
                RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                return (-1);
            }
            #endregion

            return (retValue);
        }
        #endregion

        #region Invoke Stored Proc For Journals 
        // Returns FUID if successfull, -1 if not
        public static int InvokeStpForRAWImport(string SPName, string JournalType, string BankID, string ATMNo, string fullPathName)
        {
            int rc = -1;
            int ReturnCode = 1;
            int ReturnFUID = 0;
            string jlnFullPathName;

            // Insert by Panicos 
            // Convert Journal with sequence infront of line. 
            //
            //---------------------------------
            RRDMJournalReadTxns_Text_Class Jrt = new RRDMJournalReadTxns_Text_Class();
            jlnFullPathName = Jrt.ConvertJournal(fullPathName); // Converted File 
            // Note
            int JournalLines = Jrt.LineCounter;
            //--------------------------------
            //

            string connectionString = ConfigurationManager.ConnectionStrings["ReconcRawImportConnectionString"].ConnectionString;

            // string connectionString = ConfigurationManager.ConnectionStrings["ReconcRaw_MT_ConnectionString"].ConnectionString;

            RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();
            // ********INSERT BY PANICOS**************
            DateTime BeforeCallDtTime = DateTime.Now;
            // *************************BY PANICOS********************************
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    int ret;
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@JournalType", JournalType));
                    cmd.Parameters.Add(new SqlParameter("@BankID", BankID));
                    cmd.Parameters.Add(new SqlParameter("@AtmNo", ATMNo));
                    cmd.Parameters.Add(new SqlParameter("@FullPath", jlnFullPathName));

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retFUID = new SqlParameter("@FILEID", ReturnFUID);
                    retFUID.Direction = ParameterDirection.Output;
                    retFUID.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retFUID);

                    cmd.CommandTimeout = 150; 
                    // execute the command
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH
                    ret = (int)cmd.Parameters["@ReturnCode"].Value;

                    conn.Close();

                    if (ret == 0)
                    {
                        rc = (int)cmd.Parameters["@FILEID"].Value;
                                      
                    }
                    else
                    {
                        rc = -1;
                        string msg = string.Format("Error encountered in Stored Peocedure '{0}' for the file '{1}'\nThe error code is: ",
                                                   SPName, fullPathName, ret);
                        RFMFunctions.RecordEventMsg(msg, EventLogEntryType.Error);
                        
                    }
                    //  DELETE CREATED JOURNAL 
                    RRDMGasParameters Gp = new RRDMGasParameters();
                    string ParId = "930";
                    string OccurId = "1";
                    Gp.ReadParametersSpecificId(BankID, ParId, OccurId, "", "");
                    if (Gp.OccuranceNm == "YES")
                        File.Delete(jlnFullPathName);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    Exception ex1 = ex.InnerException;
                    while (ex1 != null)
                    {
                        msg += "\r\n" + ex1.Message;
                        ex1 = ex1.InnerException;
                    }
                    rc = -1;
                    string msg1 = string.Format("Exception encountered while calling Stored Procedure '{0}' for the file '{1}'\r\nThe error message is: {2}",
                                               SPName, fullPathName, msg);
                    RFMFunctions.RecordEventMsg(msg1, EventLogEntryType.Error);
                }
            }
            // ****************INSERT BY PANICOS**************
            string Message = "ATM : " + ATMNo + " Pambos Call- Start-END..." + fullPathName;

            Pt.InsertPerformanceTrace(BankID, BankID, 2, "LoadTrans", "NBG101", BeforeCallDtTime, DateTime.Now, Message);
            // *************************END INSERT BY PANICOS********************************
            return (rc);
        }
        #endregion
    }
}
