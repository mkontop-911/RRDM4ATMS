using System;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using RRDM4ATMs;

namespace RRDMRFM_Journal_Classes
{
    public class ProcessJournalFile
    {
        private int ThreadIndex;

        public int stpReturnCode;
        public int stpReturnFUID;
        public string stpErrorText;
        public string stpErrorReference;

        public ProcessJournalFile(int indx)
        {
            ThreadIndex = indx;
        }

        #region Process Local Journal File
        // Returns FUID if successfull, -1 if not
        public int ProcessLocalJournalFile(string stpName, string eJType, string BankID, string AtmNo, string jlnFullFileName)
        {
            // int lineCount = 0;
            int retValue = -1;
            RRDMJTMIdentificationDetailsClass IdentClass = new RRDMJTMIdentificationDetailsClass();

            int stg = RfmjActionStage.Const_Step_3_InProgress;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Stage = stg;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Step_3_Descr = RfmjActionStage.getStageFromNumber(stg);
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Step_3_Start = DateTime.Now;

            #region Get the corresponding IdentificationDetails record
            IdentClass.ReadJTMIdentificationDetailsByAtmNo(AtmNo);
            if (!IdentClass.RecordFound)
            {
                string msg = string.Format("Error while reading the record for ATM:'{0}' (ReadJTMIdentificationDetailsByAtmNo)!\r\nThe error message reads: {1}",
                                     AtmNo,
                                     IdentClass.ErrorOutput);
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
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

            retValue = this.InvokeStpForRAWImport(stpName, eJType, BankID, AtmNo, jlnFullFileName);

            #endregion

            #region Update record in JTMIdentificationDetails table
            // The record has already been read in IdentClass; update only relevant fields

            IdentClass.SourceFileName = Path.GetFileName(jlnFullFileName);
            IdentClass.SourceFilePath = Path.GetDirectoryName(jlnFullFileName);
            IdentClass.DestnFilePath = jlnFullFileName;

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
                EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return (-1);
            }
            #endregion

            #region Update the Thread Status/Step
            stg = RfmjActionStage.Const_Step_3_Finished;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Stage = stg;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Step_3_Descr = RfmjActionStage.getStageFromNumber(stg);
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Step_3_End = DateTime.Now;
            #endregion

            return (retValue);
        }
        #endregion

        #region Invoke Stored Proc For Journals 
        // Returns FUID if successfull
        // Returns -1 if not successful (together eith ErrorText and ErrorReference)
        // Input file "jlnFullPathName" is the original journal file transformed so as to include a sequence number in front of each line
        public int InvokeStpForRAWImport(string SPName, string JournalType, string BankID, string ATMNo, string jlnFullPathName)
        {
            int rc = -1; // We use 'rc' to return success info to the caller ('-1' is failure)
            int ReturnCode = 1;
            int ReturnFUID = 0;
            string ErrorText = "init";
            string ErrorReference = "init";
            string SQLRelativeFile;

            this.stpErrorText = "";
            this.stpErrorReference = "";
            this.stpReturnCode = 0;
            this.stpReturnFUID = 0;


            SQLRelativeFile = Path.Combine(RfmjServer.RfmjOp.RfmjSQLRelativeFilePoolPath, Path.GetFileName(jlnFullPathName));

            string msg1 = string.Format("Invoking Stored Procedure '{0}' for the file '{1}'", SPName, jlnFullPathName);
            EventLogging.RecordEventMsg(msg1, EventLogEntryType.Information);

            string connectionString = ConfigurationManager.ConnectionStrings["ReconcRawImportConnectionString"].ConnectionString;

            RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();
            // ********INSERT BY PANICOS**************
            DateTime BeforeCallDtTime = DateTime.Now;
            // *************************BY PANICOS********************************

            int stg = RfmjActionStage.Const_Step_4_InProgress;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Stage = stg;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Step_4_Descr = RfmjActionStage.getStageFromNumber(stg);
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Step_4_Start = DateTime.Now;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    int stpRetCode = -1;
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    /*
	                @JournalType as NVARCHAR(20) ,
                    @BankID AS NVARCHAR(08) ,
                    @AtmNo AS NVARCHAR(20) ,
                    @FullPath AS NVARCHAR(254),
	                @FILEID AS INT OUTPUT, 
	                @ReturnCode AS INT OUTPUT,
	                @ErrorText as NVARCHAR(1024) OUTPUT,  
	                @ErrorReference as NVARCHAR(40)  OUTPUT                     * 
                     */

                    // the first are input parameters
                    cmd.Parameters.Add(new SqlParameter("@JournalType", JournalType));
                    cmd.Parameters.Add(new SqlParameter("@BankID", BankID));
                    cmd.Parameters.Add(new SqlParameter("@AtmNo", ATMNo));
                    cmd.Parameters.Add(new SqlParameter("@FullPath", SQLRelativeFile));

                    // the following are output parameters
                    SqlParameter retFUID = new SqlParameter("@FILEID", ReturnFUID);
                    retFUID.Direction = ParameterDirection.Output;
                    retFUID.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retFUID);

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    retErrorText.Size = 2000;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 100;
                    cmd.Parameters.Add(retErrorReference);

                    // Set ExecuteNonQuery() timeout
                    cmd.CommandTimeout = 450;  // seconds

                    // execute the command
                    cmd.ExecuteNonQuery(); // exceptions will be caught in CATCH

                    conn.Close();

                    stpRetCode = (int)cmd.Parameters["@ReturnCode"].Value;
                    this.stpReturnCode = stpRetCode;

                    string errTxt;
                    // EventLogging.RecordEventMsg("Line:278", EventLogEntryType.Warning);
                    if (cmd.Parameters["@ErrorText"].Value != DBNull.Value)
                    {
                        errTxt = (string)cmd.Parameters["@ErrorText"].Value;
                        if (string.IsNullOrWhiteSpace(errTxt))
                        {
                            errTxt = "STP returned blank";
                        }
                    }
                    else
                    {
                        errTxt = "STP returned null";
                    }
                    this.stpErrorText = errTxt;

                    if (stpRetCode == 0)
                    {
                        int fileID = 0;

                        // Type fileIDType = cmd.Parameters["@FILEID"].Value.GetType();

                        this.stpReturnFUID = fileID = (int)cmd.Parameters["@FILEID"].Value;

                        rc = (int)fileID;
                        if (rc == -1) // STP finished but returned -1 for FUID
                        {
                            string errRef;
                            // EventLogging.RecordEventMsg("Line:267", EventLogEntryType.Warning);
                            if (cmd.Parameters["@ErrorReference"].Value != DBNull.Value)
                            {
                                errRef = (string)cmd.Parameters["@ErrorReference"].Value;
                            }
                            else
                            {
                                errRef = "null";
                            }
                            this.stpErrorReference = errRef;

                            string msg = string.Format("Error encountered in Stored Procedure '{0}' for the file '{1}'\r\nError code: {2} \r\nErrorReference: {3} \r\nErrorText: {4}",
                                                       SPName, jlnFullPathName, stpRetCode, errRef, errTxt);
                            EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                        }

                    }
                    else // STP finished but returned error in 'ReturnCode' parameter
                    {
                        string errRef;
                        // string errTxt;

                        // EventLogging.RecordEventMsg("Line:300", EventLogEntryType.Warning);
                        if (cmd.Parameters["@ErrorReference"].Value != DBNull.Value)
                        {
                            errRef = (string)cmd.Parameters["@ErrorReference"].Value;
                            if (string.IsNullOrWhiteSpace(errTxt))
                            {
                                errTxt = "STP returned blank";
                            }
                        }
                        else
                        {
                            errRef = "STP returned null";
                        }
                        this.stpErrorReference = errRef;

                        //// EventLogging.RecordEventMsg("Line:311", EventLogEntryType.Warning);
                        //if (cmd.Parameters["@ErrorText"].Value != DBNull.Value)
                        //{
                        //    errTxt = (string)cmd.Parameters["@ErrorText"].Value;
                        //}
                        //else
                        //{
                        //    errTxt = "null";
                        //}
                        //this.stpErrorText = errTxt;

                        string msg = string.Format("Error encountered in Stored Procedure '{0}' for the file '{1}'\r\nError code: {2} \r\nErrorReference: {3} \r\nErrorText: {4}",
                                                   SPName, jlnFullPathName, stpRetCode, errRef, errTxt);
                        EventLogging.RecordEventMsg(msg, EventLogEntryType.Error);

                        rc = -1;
                    }
                }
                catch (Exception ex)
                {
                    // EventLogging.RecordEventMsg("Line:331", EventLogEntryType.Warning);

                    string msg = ex.Message;
                    Exception ex1 = ex.InnerException;
                    while (ex1 != null)
                    {
                        msg += "\r\n" + ex1.Message;
                        ex1 = ex1.InnerException;
                    }
                    rc = -1;
                    string msgi = string.Format("Exception encountered while calling Stored Procedure '{0}' for the file '{1}'\r\nThe error message is: {2}",
                                               SPName, jlnFullPathName, msg);
                    EventLogging.RecordEventMsg(msgi, EventLogEntryType.Error);
                    int msgLen = msgi.Length;
                    if (msgLen > 1975) msgLen = 1975;
                    this.stpErrorText = string.Format("Exception! --> {0}", msgi.Substring(0, msgLen));
                }


            }

            stg = RfmjActionStage.Const_Step_4_Finished;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Stage = stg;
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Step_4_Descr = RfmjActionStage.getStageFromNumber(stg);
            RfmjThreadRegistry.ThreadArray[ThreadIndex].workSheet.Step_4_End = DateTime.Now;

            // ****************INSERT BY PANICOS**************
            string Message = "ATM : " + ATMNo + " Pambos Call- Start-END..." + jlnFullPathName;

            Pt.InsertPerformanceTrace(BankID, BankID, 2, "LoadTrans-Pambos", ATMNo, BeforeCallDtTime, DateTime.Now, Message);
            // *************************END INSERT BY PANICOS********************************
            return (rc);
        }
        #endregion
    }
}
