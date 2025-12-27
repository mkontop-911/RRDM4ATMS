using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Globalization;

namespace RRDM4ATMs
{
    public class RRDM_Copy_Txns_From_IST_ToMatched_And_Vice : Logger
    {
        public RRDM_Copy_Txns_From_IST_ToMatched_And_Vice() : base() { }

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public int Counter;
        int ReturnCode;
        public string ProgressText;
        public string ErrorReference;
        public int ret;

        string connectionStringITMX = AppConfig.GetConnectionString("ReconConnectionString");

        string SPName;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);
        // 
        public void MOVE_ITMX_TXNS_TO_MATCHED(string InTableId, int InReconcCycleNo)
        {
           

            // CHeck If File Exist in Destination
         

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            switch (InTableId)
            {
                case "tblMatchingTxnsMasterPoolATMs": // MASTER FILE 
                    {
                        MOVE_ITMX_TXNS_TO_MATCHED_tblMatchingTxnsMasterPoolATMs(InTableId, InReconcCycleNo);
                        break;
                    }
                default:
                    {
                        MOVE_ITMX_TXNS_TO_MATCHED_PER_TABLE_NAME(InTableId, InReconcCycleNo);
                        break;
                    }
            }
        }


        public void ReadTableToSeeIfExist(string InDataBase, string InTableId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

          
            string Dbo_TableId = "[dbo]."+ InTableId; 
            //// DATA TABLE ROWS DEFINITION 
            //use[RRDM_Reconciliation_MATCHED_TXNS];
            //SELECT name as FieldNm,column_id, system_type_id,max_length,precision,scale
            //FROM sys.columns WHERE[object_id] = OBJECT_ID('[dbo].tblMatchingTxnsMasterPoolATMs');
            string SqlString = " USE " + InDataBase
                + " SELECT name as FieldNm,column_id, system_type_id,max_length,precision,scale "
            + " FROM sys.columns "
            + " WHERE[object_id] = OBJECT_ID('"+ Dbo_TableId + "')"
            + " ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@ParamId", ParamId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;
                          //  TotalSelected = TotalSelected + 1;
                            string WFieldNm = (string)rdr["FieldNm"];
                            return; 
                            //FieldNames.Add(WFieldNm);
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }        
        }


        public void MOVE_ITMX_TXNS_TO_HST(string InTableId, int InReconcCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            switch (InTableId)
            {
                case "tblMatchingTxnsMasterPoolATMs": // MASTER FILE 
                    {
                        MOVE_ITMX_TXNS_TO_MATCHED_tblMatchingTxnsMasterPoolATMs_HST(InTableId, InReconcCycleNo);
                        break;
                    }
                default:
                    {
                        MOVE_ITMX_TXNS_TO_MATCHED_PER_TABLE_NAME_HST(InTableId, InReconcCycleNo);
                        break;
                    }
            }
        }
// 
// 
//
        public void DELETE_DELETE_TXNS_FROM_HST_MAIN(string InOperator,string InSignedId, int InReconcCycleNo, int InMode)
        {
            //RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
            RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
            RRDMGasParameters Gp = new RRDMGasParameters();
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

            // InMode = 2 is interactive else is not 

            bool DeleteRecords = false;
            int DeleteDays;
            string ParamId = "853";
            string OccuranceId = "6"; // DELETE
            string S_DeleteDateLimit = "";
            DateTime DeleteDateLimit = NullPastDate;
            DateTime StartDeletionForFile = DateTime.Now;
            DateTime SavedStartDt = DateTime.Now;

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true)
            {
                DeleteDays = (int)Gp.Amount; // 

                //AgingDays_HST = 0; 

                // Current CutOffdate
                string WSelection = " WHERE JobCycle =" + InReconcCycleNo;
                Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

                DeleteDateLimit = Rjc.Cut_Off_Date.AddDays(-DeleteDays);

                //WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-0);

                Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(DeleteDateLimit);

                if (Rjc.RecordFound == true)
                {
                    S_DeleteDateLimit = Rjc.Cut_Off_Date.ToString("yyyy-MM-dd");

                    //MessageBox.Show("DELETE RECORDS FROM HST Starts" + Environment.NewLine
                    //       + "For date equal or less than.." + S_DeleteDateLimit
                    //       );
                    DeleteRecords = true;
                }
            }
            else
            {
                    //MessageBox.Show("Parameter not available for delete records");
                return; 
            }
            if (InMode ==2)
            {
                if (MessageBox.Show("Do you want to delete from History records " + Environment.NewLine
                                + "Less than date " + S_DeleteDateLimit + "...????"
                                //+ Mt.W_MPComment
                                , "Verification Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
                {
                    // YES Proceed
                }
                else
                {
                    // Stop 
                    return;
                }
            }

            if (InMode == 2)
                MessageBox.Show("Delete of Records starts ");
            string WFileName = "";
            //MoveToHistory = true;
            if (DeleteRecords == true)
            {
                //******************************
                // 
                // DELETE FROM HST 
                // 
                //******************************
                int Mode = 17; // Updating Action 
                string ProcessName = "DELETE Records from HST Data Base";
                string Message = "DELETE RECORDS STARTS for Days before:.." + S_DeleteDateLimit;
                SavedStartDt = DateTime.Now;

                Pt.InsertPerformanceTrace_With_USER(InOperator, InOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, InSignedId, InReconcCycleNo);

                string TotalProgressText = "Delete Cycle: ..."
                                         + InReconcCycleNo.ToString() + Environment.NewLine;
                //RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
                //RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
                string WSelectionCriteria = " WHERE Operator = @Operator ";
                Mf.ReadReconcSourceFilesToFillDataTable_FULL(InOperator, WSelectionCriteria);

                int I = 0;
                int K = 0;

                while (I <= (Mf.SourceFilesDataTable.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    int SeqNo = (int)Mf.SourceFilesDataTable.Rows[I]["SeqNo"];
                    Mf.ReadReconcSourceFilesBySeqNo(SeqNo);

                    if (Mf.IsMoveToMatched == true || Mf.SourceFileId == "Atms_Journals_Txns") // the indication that this table is a moving table 
                    {
                        if (Mf.SourceFileId == "Atms_Journals_Txns")
                        {
                            WFileName = "tblMatchingTxnsMasterPoolATMs";
                        }
                        else
                        {
                            WFileName = Mf.SourceFileId;
                        }

                        // Check That File Exist in target data base 
                        string TargetDB = "[RRDM_Reconciliation_ITMX_HST]";
                        ReadTableToSeeIfExist(TargetDB, WFileName);

                        if (RecordFound == true)
                        {
                            // File Exist
                        }
                        else
                        {
                            // File do not exist
                            MessageBox.Show("File.." + WFileName + Environment.NewLine
                                + "DOES NOT EXIST In ITMX_HST Data Base."
                                + "REPORT TO THE HELP DESK."
                                );
                            I = I + 1;
                            continue;
                        }

                        // Check That File Exist in target data base 
                        TargetDB = "[RRDM_Reconciliation_MATCHED_TXNS_HST]";
                        ReadTableToSeeIfExist(TargetDB, WFileName);

                        if (RecordFound == true)
                        {
                            // File Exist
                        }
                        else
                        {
                            // File do not exist
                            MessageBox.Show("File.." + WFileName + Environment.NewLine
                                + "DOES NOT EXIST In MATCHED_TXNS_HST Data Base."
                                + "REPORT TO THE HELP DESK."
                                );
                            I = I + 1;
                            continue;
                        }

                        // START DELETION 
                        StartDeletionForFile = DateTime.Now;

                        DELETE_TXNS_FROM_HST(WFileName, InReconcCycleNo, DeleteDateLimit);

                    }

                    Mode = 17; // 
                    ProcessName = "DELETE Records from HST Data Base";
                    Message = "DELETED RECORDS for FILE.." + WFileName + "..Number=." + TotalDeleted.ToString();
                    DateTime FinishDeletion = DateTime.Now;
                    Pt.InsertPerformanceTrace_With_USER(InOperator, InOperator, Mode, ProcessName, "", StartDeletionForFile, FinishDeletion, Message, InSignedId, InReconcCycleNo);

                    I = I + 1;
                }

                //TotalProgressText = TotalProgressText + DateTime.Now + " DELETE FROM HST has finished" + Environment.NewLine;
                //TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;

                // *****************************

                Mode = 17; // Updating Action 
                ProcessName = "DELETE Records from HST Data Base";
                Message = "DELETE RECORDS HAS FINISHED.for Days before:.." + S_DeleteDateLimit;

                //textBoxMsgBoard.Text = "Current Status : Ready";

                Pt.InsertPerformanceTrace_With_USER(InOperator, InOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, InSignedId, InReconcCycleNo);
                //*********************************
                if (InMode == 2)
                MessageBox.Show("Delete of Records has finished");

                //Form8_Traces_Oper NForm8_Traces_Oper;
                //int MMode = 4;
                //NForm8_Traces_Oper = new Form8_Traces_Oper(WSignedId, WSignRecordNo, "7", WOperator, MMode);
                //NForm8_Traces_Oper.ShowDialog();
                // Form502_Load(this, new EventArgs());
            }
        }
        //
        public int TotalDeleted; 
        //
        public void DELETE_TXNS_FROM_HST(string InTableId, int InReconcCycleNo, DateTime InDeleteDateLimit)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            string PhysicalTableId;
            TotalDeleted = 0; 
            //
            switch (InTableId)
            {
                case "tblMatchingTxnsMasterPoolATMs": // MASTER FILE 
                    {
                        //DELETE_ITMX_AND_MATCHED_tblMatchingTxnsMasterPoolATMs_HST(InTableId, InReconcCycleNo);
                        PhysicalTableId = "[RRDM_Reconciliation_ITMX_HST].[dbo]."+ InTableId;
                        Delete_In_HST_By_LIMIT_DATE(PhysicalTableId, InDeleteDateLimit);
                        PhysicalTableId = "[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo]." + InTableId;
                        Delete_In_HST_By_LIMIT_DATE(PhysicalTableId, InDeleteDateLimit);
                        PhysicalTableId = "[ATM_MT_Journals_AUDI_HST].[dbo].[tblHstAtmTxns]";
                        Delete_In_HST_By_LIMIT_DATE_AUDI(PhysicalTableId, InDeleteDateLimit);
                        // DELETE from ITMX_HST
                        // DELETE from Master_TXNS_HST
                        // DELETE From AUDI HSt 
                        break;
                    }
                default:
                    {
                        //DELETE_ITMX_AND_MATCHED_HST_TXNS_PER_TABLE_NAME(InTableId, InReconcCycleNo);
                        PhysicalTableId = "[RRDM_Reconciliation_ITMX_HST].[dbo]." + InTableId;
                        Delete_In_HST_By_LIMIT_DATE(PhysicalTableId, InDeleteDateLimit);
                        PhysicalTableId = "[RRDM_Reconciliation_MATCHED_TXNS_HST].[dbo]." + InTableId;
                        Delete_In_HST_By_LIMIT_DATE(PhysicalTableId, InDeleteDateLimit);
                        // DELETE from ITMX_HST
                        // DELETE from Master_TXNS_HST
                        break;
                    }
            }
        }
        // DELETE RECORDS LESS THAN SPECIFIC DATE 
        // IN LOOP 
        public void Delete_In_HST_By_LIMIT_DATE(string InPhysicalTableId, DateTime InDeleteDateLimit)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int count = 100;

            while (count>0)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("DELETE TOP (25000) FROM " + InPhysicalTableId
                                + " WHERE Net_TransDate <=  @Net_TransDate ", conn))
                        {
                            cmd.Parameters.AddWithValue("@Net_TransDate", InDeleteDateLimit);
                            cmd.CommandTimeout = 2000; 

                            count = cmd.ExecuteNonQuery();

                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }
                TotalDeleted = TotalDeleted + count; 

            } // End of while 
        }
        //
        // IN LOOP AUDI
        //
        public void Delete_In_HST_By_LIMIT_DATE_AUDI(string InPhysicalTableId, DateTime InDeleteDateLimit)
        {
            // TRANDATE differs 
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int count = 100;

            while (count > 0)
            {
                using (SqlConnection conn =
                new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("DELETE TOP (25000) FROM " + InPhysicalTableId
                                + " WHERE TranDate <=  @TranDate ", conn))
                        {
                            cmd.Parameters.AddWithValue("@TranDate", InDeleteDateLimit);
                            cmd.CommandTimeout = 2000;

                            count = cmd.ExecuteNonQuery();

                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }

                TotalDeleted = TotalDeleted + count;
            } // End of while 
        }
        // UNDO
        // MOVE_MASTER_TXNS_TO_ITMX
        public void MOVE_MATCHED_TXNS_TO_ITMX(string InTableId, int InReconcCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            switch (InTableId)
            {
                case "tblMatchingTxnsMasterPoolATMs": // MASTER FILE 
                    {
                        MOVE_MATCHED_TXNS_TO_ITMX_tblMatchingTxnsMasterPoolATMs(InTableId, InReconcCycleNo);
                        break;
                    }

                default:
                    {
                        MOVE_MATCHED_TXNS_TO_ITMX_PER_TABLE_NAME(InTableId, InReconcCycleNo);
                        break;
                    }
            }

        }

        // UNDO
        // MOVE_MASTER_TXNS_TO_ITMX
        public void MOVE_MATCHED_TXNS_TO_ITMX_By_Category(string InTableId, int InReconcCycleNo, string InCategoryId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            switch (InTableId)
            {
                case "tblMatchingTxnsMasterPoolATMs": // MASTER FILE 
                    {
                        MOVE_MATCHED_TXNS_TO_ITMX_tblMatchingTxnsMasterPoolATMs_AND_Category(InTableId, InReconcCycleNo, InCategoryId);
                        break;
                    }

                default:
                    {
                        MOVE_MATCHED_TXNS_TO_ITMX_PER_TABLE_NAME_AND_CATEGORY(InTableId, InReconcCycleNo, InCategoryId);
                        break;
                    }
            }

        }
        //
        // MOVE MASTER FILE FROM ITMX TO MATCHED
        //
        public void MOVE_ITMX_TXNS_TO_MATCHED_tblMatchingTxnsMasterPoolATMs(string InTableId, int InReconcCycleNo)
        {
            // First 
            // Clear up unwanted BDC299 
            int AgingDaysShort;  // any transction less than these days is moving to the Matched Txns 
            int AgingDays_HST; // This is the dates from moving to History data Base
                               // eg Moving From MATCHED to MATCHED_HST
            int AgingCycle_HST; // THIS IS THE CYCLE FOR MOVING TO HISTORY

            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParamId;
            //string OccurId;

            int MinusMin = 0;
            ParamId = "853";
            string OccuranceId = "1"; // Short for table cleaning 
            //  OccurId = "1"; // 
            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            AgingDaysShort = (int)Gp.Amount; // 
            //AgingDaysShort = 3; 

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            string WSelection = " WHERE JobCycle =" + InReconcCycleNo;
            Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

            DateTime WAgingDateLimitShort = Rjc.Cut_Off_Date.AddDays(-AgingDaysShort);

            //string ReversedCut_Off_Date = WAgingDateLimitShort.ToString("yyyy-MM-dd");

            // Here mature what is needed based on aging dates 
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            string PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + InTableId;

            Bio.DELETE_BDC299ByLowerLimit(PhysicalName, WAgingDateLimitShort); // DELETE THE BDC299

            
            // PASS THIS DATE TO MASTER TO MOVE OLD RECORDS OR UNMATCHED TO MATCHED DATA BASE
            // NOTE THIS WAY WE WILL MOVE UNMATCHED AND REVERSALS 
            string ReversedCut_Off_Date = WAgingDateLimitShort.ToString("yyyy-MM-dd");
            //
            // Call Procedure to make the move
            //
            ReturnCode = -20;
            ProgressText = "";
            ErrorReference = "";
            ret = -1;

            SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_MOVE_ITMX_TXNS_TO_MATCHED_tblMatchingTxnsMasterPoolATMs]";

            using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            {
                try
                {
                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);
                    cmd.CommandType = CommandType.StoredProcedure;
                    // the first are input parameters
                   // cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));
                    SqlParameter WCycleNo = new SqlParameter("@RMCycleNo", InReconcCycleNo);
                    WCycleNo.Direction = ParameterDirection.Input;
                    WCycleNo.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(WCycleNo);

                    cmd.Parameters.Add(new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date)); // NOT USED IN STORE PROCEDURE 
                    // the following are output parameters
                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 1800;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
                    ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;

                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn2.Close();
                    string Error = ex.Message;
                    CatchDetails(ex);
                }
            }

            if (ret == 0)
            {

                // OK
                //MessageBox.Show("Moving of data Finished ... VALID CALL" + Environment.NewLine
                //      + ProgressText);
            }
            else
            {
                // NOT OK
                MessageBox.Show("NOT VALID CALL - stp_00_MOVE_ITMX_TXNS_TO_MATCHED_Per table.." + Environment.NewLine
                         + "AND TABLE ID: " + InTableId + Environment.NewLine);
            }

            //  cmd.Parameters.Add(new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date));


            // MOVE FROM MATCHED TO MATCHED_HST
            //WSelection = " WHERE JobCycle =" + InReconcCycleNo;
            //Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

            //ParamId = "853";
            //OccuranceId = "5"; // HST

            //Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            //if (Gp.RecordFound == true)
            //{
            //    AgingDays_HST = (int)Gp.Amount; // 

            //    //AgingDays_HST = 1;

            //    DateTime WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-AgingDays_HST);

            //    // Find Corresponding Matching Cycle No 


            //    Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(WAgingDateLimit_HST);

            //    if (Rjc.RecordFound == true)
            //    {
            //        string ReversedCut_Off_Date = Rjc.Cut_Off_Date.ToString("yyyy-MM-dd");

            //        // UPDATE PARAMETER 853 with HST date
            //        Gp.OccuranceNm = ReversedCut_Off_Date;
            //        Gp.UpdateGasParam(Gp.BankId, ParamId, OccuranceId);

            //        //// GET THE HST DATE
            //        //DateTime HST_DATE; 

            //        //if (DateTime.TryParseExact(Gp.OccuranceNm, "yyyy-MM-dd", CultureInfo.InvariantCulture
            //        //              , System.Globalization.DateTimeStyles.None, out HST_DATE))
            //        //{

            //        //}


            //        ret = -1;

            //        SPName = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[stp_00_MOVE_MATCHED_tblMatchingTxnsMasterPoolATMs_To_HST]";

            //        using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            //        {
            //            try
            //            {
            //                conn2.Open();

            //                SqlCommand cmd = new SqlCommand(SPName, conn2);
            //                cmd.CommandType = CommandType.StoredProcedure;
            //                // the first are input parameters
            //                cmd.Parameters.Add(new SqlParameter("@RMCycleNo", Rjc.JobCycle));

            //                cmd.Parameters.Add(new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date));
            //                // the following are output parameters
            //                SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
            //                retCode.Direction = ParameterDirection.Output;
            //                retCode.SqlDbType = SqlDbType.Int;
            //                cmd.Parameters.Add(retCode);

            //                SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
            //                retProgressText.Direction = ParameterDirection.Output;
            //                retProgressText.SqlDbType = SqlDbType.NVarChar;
            //                retProgressText.Size = 3000;
            //                cmd.Parameters.Add(retProgressText);

            //                SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
            //                retErrorReference.Direction = ParameterDirection.Output;
            //                retErrorReference.SqlDbType = SqlDbType.NVarChar;
            //                retErrorReference.Size = 3000;
            //                cmd.Parameters.Add(retErrorReference);

            //                // execute the command
            //                cmd.CommandTimeout = 1800;  // seconds
            //                cmd.ExecuteNonQuery(); // errors will be caught in CATCH

            //                ret = (int)cmd.Parameters["@ReturnCode"].Value;
            //                ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

            //                conn2.Close();

            //            }
            //            catch (Exception ex)
            //            {
            //                conn2.Close();
            //                CatchDetails(ex);
            //            }
            //        }

            //        if (ret == 0)
            //        {

            //            // OK
            //            //MessageBox.Show("Moving of data Finished ... VALID CALL" + Environment.NewLine
            //            //      + ProgressText);
            //        }
            //        else
            //        {
            //            // NOT OK
            //            MessageBox.Show("NOT VALID CALL - stp_00_MOVE_MATCHED_TXNS_OF_MASTER_TO_HST.." + Environment.NewLine
            //                    + "AND TABLE ID: " + InTableId + Environment.NewLine
            //               + ProgressText);

            //        }

            //        ret = -1;

            //        SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_MOVE_ITMX_tblMatchingTxnsMasterPoolATMs_To_HST]";

            //        using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            //        {
            //            try
            //            {
            //                conn2.Open();

            //                SqlCommand cmd = new SqlCommand(SPName, conn2);
            //                cmd.CommandType = CommandType.StoredProcedure;
            //                // the first are input parameters
            //                cmd.Parameters.Add(new SqlParameter("@RMCycleNo", Rjc.JobCycle));

            //                cmd.Parameters.Add(new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date));
            //                // the following are output parameters
            //                SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
            //                retCode.Direction = ParameterDirection.Output;
            //                retCode.SqlDbType = SqlDbType.Int;
            //                cmd.Parameters.Add(retCode);

            //                SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
            //                retProgressText.Direction = ParameterDirection.Output;
            //                retProgressText.SqlDbType = SqlDbType.NVarChar;
            //                retProgressText.Size = 3000;
            //                cmd.Parameters.Add(retProgressText);

            //                SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
            //                retErrorReference.Direction = ParameterDirection.Output;
            //                retErrorReference.SqlDbType = SqlDbType.NVarChar;
            //                retErrorReference.Size = 3000;
            //                cmd.Parameters.Add(retErrorReference);

            //                // execute the command
            //                cmd.CommandTimeout = 1800;  // seconds
            //                cmd.ExecuteNonQuery(); // errors will be caught in CATCH

            //                ret = (int)cmd.Parameters["@ReturnCode"].Value;
            //                ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

            //                conn2.Close();

            //            }
            //            catch (Exception ex)
            //            {
            //                conn2.Close();
            //                CatchDetails(ex);
            //            }
            //        }

            //        if (ret == 0)
            //        {

            //            // OK
            //            //MessageBox.Show("Moving of data Finished ... VALID CALL" + Environment.NewLine
            //            //      + ProgressText);
            //        }
            //        else
            //        {
            //            // NOT OK
            //            MessageBox.Show("NOT VALID CALL - stp_00_MOVE_MATCHED_TXNS_OF_MASTER_TO_HST.." + Environment.NewLine
            //                    + "AND TABLE ID: " + InTableId + Environment.NewLine
            //               + ProgressText);

            //        }
            //    }


           // }

        }

        public void MOVE_ITMX_TXNS_TO_MATCHED_tblMatchingTxnsMasterPoolATMs_HST(string InTableId, int InReconcCycleNo)
        {
            // First 
           
            int AgingDays_HST; // This is the dates from moving to History data Base
                               // eg Moving From MATCHED to MATCHED_HST
            int AgingCycle_HST; // THIS IS THE CYCLE FOR MOVING TO HISTORY

            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParamId;
            string OccuranceId;

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
           
            // MOVE FROM MATCHED TO MATCHED_HST
            //WSelection = " WHERE JobCycle =" + InReconcCycleNo;
            //Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

            ParamId = "853";
            OccuranceId = "5"; // HST

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true)
            {
                
                AgingDays_HST = (int)Gp.Amount; // 

                //AgingDays_HST = 0;

                // Current CutOffdate
                string WSelection = " WHERE JobCycle =" + InReconcCycleNo;
                Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

                DateTime WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-AgingDays_HST);

                // Find Corresponding Matching Cycle No 

                Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(WAgingDateLimit_HST);

                if (Rjc.RecordFound == true)
                {
                    string ReversedCut_Off_Date = Rjc.Cut_Off_Date.ToString("yyyy-MM-dd");

                    // UPDATE PARAMETER 853 with HST date
                    Gp.OccuranceNm = ReversedCut_Off_Date;
                    Gp.UpdateGasParam(Gp.BankId, ParamId, OccuranceId);

                    // FROM ITMX TO HST based on date 
                    ret = -1;
                    
                    SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_MOVE_ITMX_tblMatchingTxnsMasterPoolATMs_To_HST]";

                    using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
                    {
                        try
                        {
                            conn2.Open();

                            SqlCommand cmd = new SqlCommand(SPName, conn2);
                            cmd.CommandType = CommandType.StoredProcedure;
                            // the first are input parameters
                            cmd.Parameters.Add(new SqlParameter("@RMCycleNo", Rjc.JobCycle));

                            cmd.Parameters.Add(new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date));
                            // the following are output parameters
                            SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                            retCode.Direction = ParameterDirection.Output;
                            retCode.SqlDbType = SqlDbType.Int;
                            cmd.Parameters.Add(retCode);

                            SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                            retProgressText.Direction = ParameterDirection.Output;
                            retProgressText.SqlDbType = SqlDbType.NVarChar;
                            retProgressText.Size = 3000;
                            cmd.Parameters.Add(retProgressText);

                            SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                            retErrorReference.Direction = ParameterDirection.Output;
                            retErrorReference.SqlDbType = SqlDbType.NVarChar;
                            retErrorReference.Size = 3000;
                            cmd.Parameters.Add(retErrorReference);

                            // execute the command
                            cmd.CommandTimeout = 1800;  // seconds
                            cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                            ret = (int)cmd.Parameters["@ReturnCode"].Value;
                            ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                            conn2.Close();

                        }
                        catch (Exception ex)
                        {
                            conn2.Close();
                            CatchDetails(ex);
                        }
                    }

                    if (ret == 0)
                    {

                        // OK
                        //MessageBox.Show("Moving of data Finished ... VALID CALL" + Environment.NewLine
                        //      + ProgressText);
                    }
                    else
                    {
                        // NOT OK
                        MessageBox.Show("NOT VALID CALL - stp_00_MOVE_MATCHED_TXNS_OF_MASTER_TO_HST.." + Environment.NewLine
                                + "AND TABLE ID: " + InTableId + Environment.NewLine
                           + ProgressText);

                    }

                    // FROM MATCHED TO HST 
                    ret = -1;

                    SPName = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[stp_00_MOVE_MATCHED_tblMatchingTxnsMasterPoolATMs_To_HST]";

                    using (SqlConnection conn3 = new SqlConnection(connectionStringITMX))
                    {
                        try
                        {
                            conn3.Open();

                            SqlCommand cmd = new SqlCommand(SPName, conn3);
                            cmd.CommandType = CommandType.StoredProcedure;
                            // the first are input parameters
                            cmd.Parameters.Add(new SqlParameter("@RMCycleNo", Rjc.JobCycle));

                            cmd.Parameters.Add(new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date));
                            // the following are output parameters
                            SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                            retCode.Direction = ParameterDirection.Output;
                            retCode.SqlDbType = SqlDbType.Int;
                            cmd.Parameters.Add(retCode);

                            SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                            retProgressText.Direction = ParameterDirection.Output;
                            retProgressText.SqlDbType = SqlDbType.NVarChar;
                            retProgressText.Size = 3000;
                            cmd.Parameters.Add(retProgressText);

                            SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                            retErrorReference.Direction = ParameterDirection.Output;
                            retErrorReference.SqlDbType = SqlDbType.NVarChar;
                            retErrorReference.Size = 3000;
                            cmd.Parameters.Add(retErrorReference);

                            // execute the command
                            cmd.CommandTimeout = 1800;  // seconds
                            cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                            ret = (int)cmd.Parameters["@ReturnCode"].Value;
                            ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                            conn3.Close();

                        }
                        catch (Exception ex)
                        {
                            conn3.Close();
                            CatchDetails(ex);
                        }
                    }

                    if (ret == 0)
                    {

                        // OK
                        //MessageBox.Show("Moving of data Finished ... VALID CALL" + Environment.NewLine
                        //      + ProgressText);
                    }
                    else
                    {
                        // NOT OK
                        MessageBox.Show("NOT VALID CALL - stp_00_MOVE_MATCHED_TXNS_OF_MASTER_TO_HST.." + Environment.NewLine
                                + "AND TABLE ID: " + InTableId + Environment.NewLine
                           + ProgressText);

                    }

                   
                    // MOVE FROM AUDI TO HISTORY 

                    ret = -1;
                    
                    SPName = "[ATM_MT_Journals_AUDI].[dbo].[stp_00_MOVE_tblHstAtmTxns_To_HST]";

                    using (SqlConnection conn4 = new SqlConnection(connectionStringITMX))
                    {
                        try
                        {
                            conn4.Open();

                            SqlCommand cmd = new SqlCommand(SPName, conn4);
                            cmd.CommandType = CommandType.StoredProcedure;
                            // the first are input parameters
                            cmd.Parameters.Add(new SqlParameter("@RMCycleNo", Rjc.JobCycle));

                            cmd.Parameters.Add(new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date));
                            // the following are output parameters
                            SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                            retCode.Direction = ParameterDirection.Output;
                            retCode.SqlDbType = SqlDbType.Int;
                            cmd.Parameters.Add(retCode);

                            SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                            retProgressText.Direction = ParameterDirection.Output;
                            retProgressText.SqlDbType = SqlDbType.NVarChar;
                            retProgressText.Size = 3000;
                            cmd.Parameters.Add(retProgressText);

                            SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                            retErrorReference.Direction = ParameterDirection.Output;
                            retErrorReference.SqlDbType = SqlDbType.NVarChar;
                            retErrorReference.Size = 3000;
                            cmd.Parameters.Add(retErrorReference);

                            // execute the command
                            cmd.CommandTimeout = 1800;  // seconds
                            cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                            ret = (int)cmd.Parameters["@ReturnCode"].Value;
                            ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                            conn4.Close();

                        }
                        catch (Exception ex)
                        {
                            conn4.Close();
                            CatchDetails(ex);
                        }
                    }

                    if (ret == 0)
                    {

                        // OK
                        //MessageBox.Show("Moving of data Finished ... VALID CALL" + Environment.NewLine
                        //      + ProgressText);
                    }
                    else
                    {
                        // NOT OK
                        MessageBox.Show("NOT VALID CALL - stp_00_MOVE_tblHstAtmTxns_To_HST.." + Environment.NewLine
                                + "AND TABLE ID: " + InTableId + Environment.NewLine
                           + ProgressText);

                    }
                }

            }

        }

        //public void DELETE_ITMX_AND_MATCHED_tblMatchingTxnsMasterPoolATMs_HST(string InTableId, int InReconcCycleNo)
        //{

        //    //******************************
        //    // DELETE TXNS FROM ITMX HST AND ALSO MATCHED
        //    // Per FILE 
        //    //*****************************
        //    ReturnCode = -20;
        //    ProgressText = "";
        //    ErrorReference = "";

        //    int AgingDays_HST; // This is the dates from moving to History data Base
        //                       // eg Moving From MATCHED to MATCHED_HST
        //    int AgingCycle_HST; // THIS IS THE CYCLE FOR MOVING TO HISTORY

        //    int DeleteOlder_CutOff; // Delete Records From History eg if more than defined days

        //    DateTime WCurrentCut_Off_Date;

        //    bool DeletionExist = false;

        //    RRDMGasParameters Gp = new RRDMGasParameters();

        //    string ParamId;
        //    string OccuranceId;

        //    RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        //    // MOVE FROM MATCHED TO MATCHED_HST

        //    ParamId = "853";
        //    OccuranceId = "5"; // HST

        //    Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

        //    if (Gp.RecordFound == true)
        //    {
        //        AgingDays_HST = (int)Gp.Amount; // 

        //        // Current CutOffdate
        //        string WSelection = " WHERE JobCycle =" + InReconcCycleNo;
        //        Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

        //        WCurrentCut_Off_Date = Rjc.Cut_Off_Date;

        //        DateTime WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-AgingDays_HST);

        //        // Find Corresponding Matching Cycle No based on CUT Off 
        //        Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(WAgingDateLimit_HST);
        //        //Rjc.RecordFound = true;
        //        if (Rjc.RecordFound == true)
        //        {
        //            //
        //            // MAKE IT AS STRING TO PASS IT TO STORE PROCEDURE
        //            //
        //            string ReversedCut_Off_Date_Hst = Rjc.Cut_Off_Date.ToString("yyyy-MM-dd");

        //            string ReversedForDeletion = "1950-11-21";
        //            // Now find out if there is Deletion 

        //            ParamId = "853";
        //            OccuranceId = "6"; // Deletion ... the parameter contains the above to move to history
        //                               // Eg : If move to Hst = 80 then we delete anything above this 

        //            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

        //            if (Gp.RecordFound == true)
        //            {
        //                DeleteOlder_CutOff = (int)Gp.Amount; // 

        //                if (DeleteOlder_CutOff > (AgingDays_HST + 2))
        //                {
        //                    // OK this is how should be 
        //                    ReversedForDeletion = (WCurrentCut_Off_Date.AddDays(-(DeleteOlder_CutOff))).ToString("yyyy-MM-dd");
        //                    //ReversedForDeletion = (Rjc.Cut_Off_Date.AddDays(-(AgingDays_HST + DeleteAbove_Hst))).ToString("yyyy-MM-dd");

        //                    DeletionExist = true;
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Please examine Parameter 853 , Occurance 6 " + Environment.NewLine
        //                       + "Deletion number of days Not correct" +
        //                        "");
        //                    DeletionExist = false;
        //                }
        //            }
        //            else
        //            {
        //                DeletionExist = false;
        //            }
        //            //
        //            // If deletion exist 
        //            //
        //            if (DeletionExist == true)
        //            {
        //                // DELETE ITMX HST 
        //                ret = -1;

        //                SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_DELETE_FROM_ITMX_HST]";

        //                using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
        //                {
        //                    try
        //                    {
        //                        conn2.Open();

        //                        SqlCommand cmd = new SqlCommand(SPName, conn2);

        //                        cmd.CommandType = CommandType.StoredProcedure;

        //                        // the first are input parameters
        //                        cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));

        //                        cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

        //                        //cmd.Parameters.Add(new SqlParameter("@ReversedForDeletion", ReversedForDeletion));

        //                        cmd.Parameters.Add(new SqlParameter("@DeleteFromDate", ReversedForDeletion));

        //                        //@DeleteFromDate

        //                        // the following are output parameters

        //                        SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
        //                        retCode.Direction = ParameterDirection.Output;
        //                        retCode.SqlDbType = SqlDbType.Int;
        //                        cmd.Parameters.Add(retCode);

        //                        SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
        //                        retProgressText.Direction = ParameterDirection.Output;
        //                        retProgressText.SqlDbType = SqlDbType.NVarChar;
        //                        retProgressText.Size = 3000;
        //                        cmd.Parameters.Add(retProgressText);

        //                        SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
        //                        retErrorReference.Direction = ParameterDirection.Output;
        //                        retErrorReference.SqlDbType = SqlDbType.NVarChar;
        //                        retErrorReference.Size = 3000;
        //                        cmd.Parameters.Add(retErrorReference);

        //                        // execute the command
        //                        cmd.CommandTimeout = 1800;  // seconds
        //                        cmd.ExecuteNonQuery(); // errors will be caught in CATCH

        //                        ret = (int)cmd.Parameters["@ReturnCode"].Value;
        //                        ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
        //                        ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
        //                        conn2.Close();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        conn2.Close();
        //                        string Error = ex.Message;
        //                        CatchDetails(ex);
        //                    }
        //                }

        //                if (ret == 0)
        //                {

        //                    // OK
        //                    //MessageBox.Show("VALID CALL" + Environment.NewLine
        //                    // + ProgressText);
        //                }
        //                else
        //                {
        //                    // NOT OK
        //                    MessageBox.Show("NOT VALID CALL - stp_00_DELETE_FROM_ITMX_HST" + InTableId + Environment.NewLine
        //                             + ProgressText);
        //                }
        //                // DELETE FROM HST - Matched
        //                ret = -1;

        //                SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_DELETE_FROM_MATCHED_HST]";

        //                using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
        //                {
        //                    try
        //                    {
        //                        conn2.Open();

        //                        SqlCommand cmd = new SqlCommand(SPName, conn2);

        //                        cmd.CommandType = CommandType.StoredProcedure;

        //                        // the first are input parameters
        //                        cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));

        //                        cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

        //                        cmd.Parameters.Add(new SqlParameter("@DeleteFromDate", ReversedForDeletion));

        //                        // cmd.Parameters.Add(new SqlParameter("@DeleteFromDate", ReversedForDeletion));

        //                        //@DeleteFromDate

        //                        // the following are output parameters

        //                        SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
        //                        retCode.Direction = ParameterDirection.Output;
        //                        retCode.SqlDbType = SqlDbType.Int;
        //                        cmd.Parameters.Add(retCode);

        //                        SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
        //                        retProgressText.Direction = ParameterDirection.Output;
        //                        retProgressText.SqlDbType = SqlDbType.NVarChar;
        //                        retProgressText.Size = 3000;
        //                        cmd.Parameters.Add(retProgressText);

        //                        SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
        //                        retErrorReference.Direction = ParameterDirection.Output;
        //                        retErrorReference.SqlDbType = SqlDbType.NVarChar;
        //                        retErrorReference.Size = 3000;
        //                        cmd.Parameters.Add(retErrorReference);

        //                        // execute the command
        //                        cmd.CommandTimeout = 1800;  // seconds
        //                        cmd.ExecuteNonQuery(); // errors will be caught in CATCH

        //                        ret = (int)cmd.Parameters["@ReturnCode"].Value;
        //                        ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
        //                        ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
        //                        conn2.Close();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        conn2.Close();
        //                        string Error = ex.Message;
        //                        CatchDetails(ex);
        //                    }
        //                }

        //                if (ret == 0)
        //                {

        //                    // OK
        //                    //MessageBox.Show("VALID CALL" + Environment.NewLine
        //                    // + ProgressText);
        //                }
        //                else
        //                {
        //                    // NOT OK
        //                    MessageBox.Show("NOT VALID CALL - stp_00_DELETE_FROM_MATCHED_HST" + InTableId + Environment.NewLine
        //                             + ProgressText);
        //                }

        //                // MOVE FROM AUDI TO HISTORY 

        //                ret = -1;
        //                //
        //                // THIS STP IS IN ITMX
        //                //
        //                SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_DELETE_FROM_AUDI_HST]";

        //                using (SqlConnection conn3 = new SqlConnection(connectionStringITMX))
        //                {
        //                    try
        //                    {
        //                        conn3.Open();

        //                        SqlCommand cmd = new SqlCommand(SPName, conn3);
        //                        cmd.CommandType = CommandType.StoredProcedure;

        //                        // the first are input parameters
        //                        cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));

        //                        cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

        //                        cmd.Parameters.Add(new SqlParameter("@DeleteFromDate", ReversedForDeletion));

        //                        // cmd.Parameters.Add(new SqlParameter("@DeleteFromDate", ReversedForDeletion));

        //                        //@DeleteFromDate

        //                        // the following are output parameters

        //                        SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
        //                        retCode.Direction = ParameterDirection.Output;
        //                        retCode.SqlDbType = SqlDbType.Int;
        //                        cmd.Parameters.Add(retCode);

        //                        SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
        //                        retProgressText.Direction = ParameterDirection.Output;
        //                        retProgressText.SqlDbType = SqlDbType.NVarChar;
        //                        retProgressText.Size = 3000;
        //                        cmd.Parameters.Add(retProgressText);

        //                        SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
        //                        retErrorReference.Direction = ParameterDirection.Output;
        //                        retErrorReference.SqlDbType = SqlDbType.NVarChar;
        //                        retErrorReference.Size = 3000;
        //                        cmd.Parameters.Add(retErrorReference);

        //                        // execute the command
        //                        cmd.CommandTimeout = 1800;  // seconds
        //                        cmd.ExecuteNonQuery(); // errors will be caught in CATCH

        //                        ret = (int)cmd.Parameters["@ReturnCode"].Value;
        //                        ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
        //                        ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
        //                        conn3.Close();

        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        conn3.Close();
        //                        CatchDetails(ex);
        //                    }
        //                }

        //                if (ret == 0)
        //                {

        //                    // OK
        //                    //MessageBox.Show("Moving of data Finished ... VALID CALL" + Environment.NewLine
        //                    //      + ProgressText);
        //                }
        //                else
        //                {
        //                    // NOT OK
        //                    MessageBox.Show("NOT VALID CALL - stp_00_MOVE_tblHstAtmTxns_To_HST.." + Environment.NewLine
        //                            + "AND TABLE ID: " + InTableId + Environment.NewLine
        //                       + ProgressText);

        //                }

        //            }
        //            else
        //            {
        //                // Deletion doesnt exist. 
        //            }

        //        }
        //    }

            

        //}
        // MOVE MASTER FILE FROM MATCHED TO ITMX
        public void MOVE_MATCHED_TXNS_TO_ITMX_tblMatchingTxnsMasterPoolATMs(string InTableId, int InReconcCycleNo)
        {
            ReturnCode = -20;
            ProgressText = "";
            ErrorReference = "";
            ret = -1;

            SPName = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[stp_00_MOVE_MATCHED_TXNS_TO_ITMX_tblMatchingTxnsMasterPoolATMs]";

            using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            {
                try
                {
                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);
                    cmd.CommandType = CommandType.StoredProcedure;
                    // the first are input parameters
                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));
                    // the following are output parameters
                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 1800;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }

            if (ret == 0)
            {

                // OK
                //MessageBox.Show("Moving of data Finished ... VALID CALL" + Environment.NewLine
                //      + ProgressText);
            }
            else
            {
                // NOT OK
                MessageBox.Show("NOT VALID CALL - stp_00_MOVE_MATCHED_TXNS_TO_ITMX_Per table.." + Environment.NewLine
                        + "AND TABLE ID: " + InTableId + Environment.NewLine
                   + ProgressText);

            }

        }

        // MOVE MASTER FILE One Record FROM MATCHED TO ITMX
        public void MOVE_MATCHED_TXNS_TO_ITMX_tblMatchingTxnsMasterPoolATMs_Unique(int InUniqueRecordId)
        {
            ReturnCode = -20;
            ProgressText = "";
            ErrorReference = "";
            ret = -1;

            SPName = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[stp_00_MOVE_MATCHED_TXNS_TO_ITMX_tblMatchingTxnsMasterPoolATMs_Unique]";

            using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            {
                try
                {
                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);
                    cmd.CommandType = CommandType.StoredProcedure;
                    // the first are input parameters
                    cmd.Parameters.Add(new SqlParameter("@UniqueRecordId", InUniqueRecordId));
                    // the following are output parameters
                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 900;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }

            if (ret == 0)
            {

                // OK
                //MessageBox.Show("Moving of data Finished ... VALID CALL" + Environment.NewLine
                //      + ProgressText);
            }
            else
            {
                // NOT OK
                MessageBox.Show("Moving of data stp_00_MOVE_MATCHED_TO_ITMX_DB_01_POOL ... NOT VALID CALL - UNIQUE" + Environment.NewLine
                         + ProgressText);
            }

        }
        // MOVE TABLE FROM ITMX TO MATCHED 
        public void MOVE_ITMX_TXNS_TO_MATCHED_PER_TABLE_NAME(string InTableId, int InReconcCycleNo)
        {
            //******************************
            // stp_00_MOVE_ITMX_TXNS_TO_MATCHED
            //*****************************
            ReturnCode = -20;
            ProgressText = "";
            ErrorReference = "";

            int AgingDaysShort;  // any transction less than these days is moving to the Matched Txns 
            int AgingDaysShort_POS; // This is longer as POS might take 40 days to respond
            int AgingDays_HST; // This is the dates from moving to History data Base
                               // eg Moving From MATCHED to MATCHED_HST
            int AgingCycle_HST; // THIS IS THE CYCLE FOR MOVING TO HISTORY

            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParamId;
            //string OccurId;

            int MinusMin = 0;
            ParamId = "853";
            string OccuranceId = "1"; // Short
            //  OccurId = "1"; // 
            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            AgingDaysShort = (int)Gp.Amount; // 25 dates for testing 

            ParamId = "853";
            OccuranceId = "2"; // Long POS 

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            AgingDaysShort_POS = (int)Gp.Amount; // 202 dates for testing 
                                                 // AgingDays = 0; 
                                                 // Find Current Cut off date 

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            string WSelection = " WHERE JobCycle =" + InReconcCycleNo;
            Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

            DateTime WAgingDateLimitShort = Rjc.Cut_Off_Date.AddDays(-AgingDaysShort); // 25 
            DateTime WAgingDateLimit_POS = Rjc.Cut_Off_Date.AddDays(-AgingDaysShort_POS); // 40


            string ReversedCut_Off_Date = WAgingDateLimitShort.ToString("yyyy-MM-dd");

            // Here mature what is needed based on aging dates // UPDATE AS PROCESS In ORDER to be moved to Matched 
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            string PhysicalName = "[RRDM_Reconciliation_ITMX].[dbo]." + InTableId;
            Bio.UpdateAgingRecords(PhysicalName, InReconcCycleNo, WAgingDateLimitShort, WAgingDateLimit_POS);

            // MOVE FROM ITMX TO MATCHED 
            ret = -1;

            SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_MOVE_ITMX_TXNS_TO_MATCHED_PER_TABLE_NAME]";

            using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            {
                try
                {
                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters
                   // cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));
                    SqlParameter WTable = new SqlParameter("@TableName", InTableId);
                    WTable.Direction = ParameterDirection.Input;
                    WTable.SqlDbType = SqlDbType.NVarChar;
                   // WPath.Size = 400;
                    cmd.Parameters.Add(WTable);

                    //cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));
                    SqlParameter WCycleNo = new SqlParameter("@RMCycleNo", InReconcCycleNo);
                    WCycleNo.Direction = ParameterDirection.Input;
                    WCycleNo.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(WCycleNo);

                   // cmd.Parameters.Add(new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date));
                    SqlParameter WDate = new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date);
                    WDate.Direction = ParameterDirection.Input;
                    WDate.SqlDbType = SqlDbType.NVarChar;
                    // WPath.Size = 400;
                    cmd.Parameters.Add(WDate);

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 1800;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
                    ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    string Error = ex.Message;
                    CatchDetails(ex);
                }
            }

            if (ret == 0)
            {

                // OK
                //MessageBox.Show("VALID CALL" + Environment.NewLine
                // + ProgressText);
            }
            else
            {
                // NOT OK
                MessageBox.Show("NOT VALID CALL - stp_00_MOVE_ITMX_TXNS_TO_MATCHED_" + InTableId + Environment.NewLine
                         + ProgressText);
            }

            // MOVE FROM MATCHED TO MATCHED_HST

            //ParamId = "853";
            //OccuranceId = "5"; // HST

            //Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            //if (Gp.RecordFound == true)
            //{
            //    AgingDays_HST = (int)Gp.Amount; // 

            //    //AgingDays_HST = 1; 

            //    DateTime WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-AgingDays_HST);

            //    Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(WAgingDateLimit_HST);

            //    if (Rjc.RecordFound == true)
            //    {
            //        // Find Corresponding Matching Cycle No 

            //        ReversedCut_Off_Date = Rjc.Cut_Off_Date.ToString("yyyy-MM-dd");

            //        ret = -1;

            //        SPName = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[stp_00_MOVE_MATCHED_TXNS_PER_TABLE_NAME_TO_MATCHED_HST]";

            //        using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            //        {
            //            try
            //            {
            //                conn2.Open();

            //                SqlCommand cmd = new SqlCommand(SPName, conn2);

            //                cmd.CommandType = CommandType.StoredProcedure;

            //                // the first are input parameters
            //                cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));

            //                cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

            //                cmd.Parameters.Add(new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date));

            //                // the following are output parameters

            //                SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
            //                retCode.Direction = ParameterDirection.Output;
            //                retCode.SqlDbType = SqlDbType.Int;
            //                cmd.Parameters.Add(retCode);

            //                SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
            //                retProgressText.Direction = ParameterDirection.Output;
            //                retProgressText.SqlDbType = SqlDbType.NVarChar;
            //                retProgressText.Size = 3000;
            //                cmd.Parameters.Add(retProgressText);

            //                SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
            //                retErrorReference.Direction = ParameterDirection.Output;
            //                retErrorReference.SqlDbType = SqlDbType.NVarChar;
            //                retErrorReference.Size = 3000;
            //                cmd.Parameters.Add(retErrorReference);

            //                // execute the command
            //                cmd.CommandTimeout = 1800;  // seconds
            //                cmd.ExecuteNonQuery(); // errors will be caught in CATCH

            //                ret = (int)cmd.Parameters["@ReturnCode"].Value;
            //                ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
            //                ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
            //                conn2.Close();
            //            }
            //            catch (Exception ex)
            //            {
            //                conn2.Close();
            //                string Error = ex.Message;
            //                CatchDetails(ex);
            //            }
            //        }

            //        if (ret == 0)
            //        {

            //            // OK
            //            //MessageBox.Show("VALID CALL" + Environment.NewLine
            //            // + ProgressText);
            //        }
            //        else
            //        {
            //            // NOT OK
            //            MessageBox.Show("NOT VALID CALL - stp_00_MOVE_MATCHED_TXNS_TO_HST" + InTableId + Environment.NewLine
            //                     + ProgressText);
            //        }

            //        ret = -1;

            //        SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_MOVE_ITMX_PER_TABLE_NAME_TO_MATCHED_HST]";

            //        using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            //        {
            //            try
            //            {
            //                conn2.Open();

            //                SqlCommand cmd = new SqlCommand(SPName, conn2);

            //                cmd.CommandType = CommandType.StoredProcedure;

            //                // the first are input parameters
            //                cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));

            //                cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

            //                cmd.Parameters.Add(new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date));

            //                // the following are output parameters

            //                SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
            //                retCode.Direction = ParameterDirection.Output;
            //                retCode.SqlDbType = SqlDbType.Int;
            //                cmd.Parameters.Add(retCode);

            //                SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
            //                retProgressText.Direction = ParameterDirection.Output;
            //                retProgressText.SqlDbType = SqlDbType.NVarChar;
            //                retProgressText.Size = 3000;
            //                cmd.Parameters.Add(retProgressText);

            //                SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
            //                retErrorReference.Direction = ParameterDirection.Output;
            //                retErrorReference.SqlDbType = SqlDbType.NVarChar;
            //                retErrorReference.Size = 3000;
            //                cmd.Parameters.Add(retErrorReference);

            //                // execute the command
            //                cmd.CommandTimeout = 1800;  // seconds
            //                cmd.ExecuteNonQuery(); // errors will be caught in CATCH

            //                ret = (int)cmd.Parameters["@ReturnCode"].Value;
            //                ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
            //                ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
            //                conn2.Close();
            //            }
            //            catch (Exception ex)
            //            {
            //                conn2.Close();
            //                string Error = ex.Message;
            //                CatchDetails(ex);
            //            }
            //        }

            //        if (ret == 0)
            //        {

            //            // OK
            //            //MessageBox.Show("VALID CALL" + Environment.NewLine
            //            // + ProgressText);
            //        }
            //        else
            //        {
            //            // NOT OK
            //            MessageBox.Show("NOT VALID CALL - stp_00_MOVE_ITMX_TXNS_TO_HST" + InTableId + Environment.NewLine
            //                     + ProgressText);
            //        }
            //    }
            //}



        }
        // MOVE TABLE FROM ITMX TO HST 
        public void MOVE_ITMX_TXNS_TO_MATCHED_PER_TABLE_NAME_HST(string InTableId, int InReconcCycleNo)
        {
            //******************************
            // stp_00_MOVE_ITMX_TXNS_TO_HST
            // Per FILE MOVE ITMX AND MATCHED
            //*****************************
            ReturnCode = -20;
            ProgressText = "";
            ErrorReference = "";
            
            int AgingDays_HST; // This is the dates from moving to History data Base
                               // eg Moving From MATCHED to MATCHED_HST
            int AgingCycle_HST; // THIS IS THE CYCLE FOR MOVING TO HISTORY

            //int DeleteOlder_CutOff; // Delete Records From History eg if more than defined days

            DateTime WCurrentCut_Off_Date; 

            //bool DeletionExist = false; 

            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParamId;
            string OccuranceId ;

            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            
            // MOVE FROM MATCHED TO MATCHED_HST

            ParamId = "853";
            OccuranceId = "5"; // HST

            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

            if (Gp.RecordFound == true)
            {
                AgingDays_HST = (int)Gp.Amount; // 

                // Current CutOffdate
                string WSelection = " WHERE JobCycle =" + InReconcCycleNo;
                Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

                WCurrentCut_Off_Date = Rjc.Cut_Off_Date; 

                DateTime WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-AgingDays_HST);

                // Find Corresponding Matching Cycle No based on CUT Off 
                Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(WAgingDateLimit_HST);
                //Rjc.RecordFound = true;
                if (Rjc.RecordFound == true)
                {
                   //
                   // MAKE IT AS STRING TO PASS IT TO STORE PROCEDURE
                   //
                   string ReversedCut_Off_Date_Hst = Rjc.Cut_Off_Date.ToString("yyyy-MM-dd");

                 

 // MOVE FROM ITMX TO HST
                        ret = -1;

                        SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_MOVE_ITMX_PER_TABLE_NAME_TO_HST]";

                        using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
                        {
                            try
                            {
                                conn2.Open();

                                SqlCommand cmd = new SqlCommand(SPName, conn2);

                                cmd.CommandType = CommandType.StoredProcedure;

                                // the first are input parameters
                                cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));

                                cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                                cmd.Parameters.Add(new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date_Hst));

                                // cmd.Parameters.Add(new SqlParameter("@DeleteFromDate", ReversedForDeletion));

                                //@DeleteFromDate

                                // the following are output parameters

                                SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                                retCode.Direction = ParameterDirection.Output;
                                retCode.SqlDbType = SqlDbType.Int;
                                cmd.Parameters.Add(retCode);

                                SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                                retProgressText.Direction = ParameterDirection.Output;
                                retProgressText.SqlDbType = SqlDbType.NVarChar;
                                retProgressText.Size = 3000;
                                cmd.Parameters.Add(retProgressText);

                                SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                                retErrorReference.Direction = ParameterDirection.Output;
                                retErrorReference.SqlDbType = SqlDbType.NVarChar;
                                retErrorReference.Size = 3000;
                                cmd.Parameters.Add(retErrorReference);

                                // execute the command
                                cmd.CommandTimeout = 1800;  // seconds
                                cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                                ret = (int)cmd.Parameters["@ReturnCode"].Value;
                                ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
                                ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
                                conn2.Close();
                            }
                            catch (Exception ex)
                            {
                                conn2.Close();
                                string Error = ex.Message;
                                CatchDetails(ex);
                            }
                        }

                        if (ret == 0)
                        {

                            // OK
                            //MessageBox.Show("VALID CALL" + Environment.NewLine
                            // + ProgressText);
                        }
                        else
                        {
                            // NOT OK
                            MessageBox.Show("NOT VALID CALL - stp_00_MOVE_ITMX_TO_HST" + InTableId + Environment.NewLine
                                     + ProgressText);
                        }
// MOVE FROM MATCHED TO HST
                    ret = -1;

                    SPName = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[stp_00_MOVE_MATCHED_TXNS_PER_TABLE_NAME_TO_MATCHED_HST]";

                    using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
                    {
                        try
                        {
                            conn2.Open();

                            SqlCommand cmd = new SqlCommand(SPName, conn2);

                            cmd.CommandType = CommandType.StoredProcedure;

                            // the first are input parameters
                            cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));

                            cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                            cmd.Parameters.Add(new SqlParameter("@AgingLimitDate", ReversedCut_Off_Date_Hst));

                            // cmd.Parameters.Add(new SqlParameter("@DeleteFromDate", ReversedForDeletion));

                            //@DeleteFromDate

                            // the following are output parameters

                            SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                            retCode.Direction = ParameterDirection.Output;
                            retCode.SqlDbType = SqlDbType.Int;
                            cmd.Parameters.Add(retCode);

                            SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                            retProgressText.Direction = ParameterDirection.Output;
                            retProgressText.SqlDbType = SqlDbType.NVarChar;
                            retProgressText.Size = 3000;
                            cmd.Parameters.Add(retProgressText);

                            SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                            retErrorReference.Direction = ParameterDirection.Output;
                            retErrorReference.SqlDbType = SqlDbType.NVarChar;
                            retErrorReference.Size = 3000;
                            cmd.Parameters.Add(retErrorReference);

                            // execute the command
                            cmd.CommandTimeout = 1800;  // seconds
                            cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                            ret = (int)cmd.Parameters["@ReturnCode"].Value;
                            ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
                            ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
                            conn2.Close();
                        }
                        catch (Exception ex)
                        {
                            conn2.Close();
                            string Error = ex.Message;
                            CatchDetails(ex);
                        }
                    }

                    if (ret == 0)
                    {

                        // OK
                        //MessageBox.Show("VALID CALL" + Environment.NewLine
                        // + ProgressText);
                    }
                    else
                    {
                        // NOT OK
                        MessageBox.Show("NOT VALID CALL - stp_00_MOVE_MATCHED_TXNS_TO_HST" + InTableId + Environment.NewLine
                                 + ProgressText);
                    }

              

                    
                }
            }

        }

        // DELETE TXNS FROM ITMX HST AND ALSO MATCHED 
        //public void DELETE_ITMX_AND_MATCHED_HST_TXNS_PER_TABLE_NAME(string InTableId, int InReconcCycleNo)
        //{
        //    //******************************
        //    // DELETE TXNS FROM ITMX HST AND ALSO MATCHED
        //    // Per FILE 
        //    //*****************************
        //    ReturnCode = -20;
        //    ProgressText = "";
        //    ErrorReference = "";

        //    int AgingDays_HST; // This is the dates from moving to History data Base
        //                       // eg Moving From MATCHED to MATCHED_HST
        //    int AgingCycle_HST; // THIS IS THE CYCLE FOR MOVING TO HISTORY

        //    int DeleteOlder_CutOff; // Delete Records From History eg if more than defined days

        //    DateTime WCurrentCut_Off_Date;

        //    bool DeletionExist = false;

        //    RRDMGasParameters Gp = new RRDMGasParameters();

        //    string ParamId;
        //    string OccuranceId;

        //    RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        //    // MOVE FROM MATCHED TO MATCHED_HST

        //    ParamId = "853";
        //    OccuranceId = "5"; // HST

        //    Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

        //    if (Gp.RecordFound == true)
        //    {
        //        AgingDays_HST = (int)Gp.Amount; // 

        //        // Current CutOffdate
        //        string WSelection = " WHERE JobCycle =" + InReconcCycleNo;
        //        Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

        //        WCurrentCut_Off_Date = Rjc.Cut_Off_Date;

        //        DateTime WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-AgingDays_HST);

        //        // Find Corresponding Matching Cycle No based on CUT Off 
        //        Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(WAgingDateLimit_HST);
        //        //Rjc.RecordFound = true;
        //        if (Rjc.RecordFound == true)
        //        {
        //            //
        //            // MAKE IT AS STRING TO PASS IT TO STORE PROCEDURE
        //            //
        //            string ReversedCut_Off_Date_Hst = Rjc.Cut_Off_Date.ToString("yyyy-MM-dd");

        //            string ReversedForDeletion = "1950-11-21";
        //            // Now find out if there is Deletion 

        //            ParamId = "853";
        //            OccuranceId = "6"; // Deletion ... the parameter contains the above to move to history
        //                               // Eg : If move to Hst = 80 then we delete anything above this 

        //            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

        //            if (Gp.RecordFound == true)
        //            {
        //                DeleteOlder_CutOff = (int)Gp.Amount; // 

        //                if (DeleteOlder_CutOff > (AgingDays_HST + 5))
        //                {
        //                    // OK this is how should be 
        //                    ReversedForDeletion = (WCurrentCut_Off_Date.AddDays(-(DeleteOlder_CutOff))).ToString("yyyy-MM-dd");
        //                    //ReversedForDeletion = (Rjc.Cut_Off_Date.AddDays(-(AgingDays_HST + DeleteAbove_Hst))).ToString("yyyy-MM-dd");

        //                    DeletionExist = true;
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Please examine Parameter 853 , Occurance 6 " + Environment.NewLine
        //                       + "Deletion number of days Not correct" +
        //                        "");
        //                    DeletionExist = false;
        //                }
        //            }
        //            else
        //            {
        //                DeletionExist = false;
        //            }
        //            //
        //            // If deletion exist 
        //            //
        //            if (DeletionExist == true)
        //            {
        //                // DELETE ITMX HST 
        //                ret = -1;

        //                SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_DELETE_FROM_ITMX_HST]";

        //                using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
        //                {
        //                    try
        //                    {
        //                        conn2.Open();

        //                        SqlCommand cmd = new SqlCommand(SPName, conn2);

        //                        cmd.CommandType = CommandType.StoredProcedure;

        //                        // the first are input parameters
        //                        cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));

        //                        cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

        //                        cmd.Parameters.Add(new SqlParameter("@DeleteFromDate", ReversedForDeletion));

        //                        // cmd.Parameters.Add(new SqlParameter("@DeleteFromDate", ReversedForDeletion));

        //                        //@DeleteFromDate

        //                        // the following are output parameters

        //                        SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
        //                        retCode.Direction = ParameterDirection.Output;
        //                        retCode.SqlDbType = SqlDbType.Int;
        //                        cmd.Parameters.Add(retCode);

        //                        SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
        //                        retProgressText.Direction = ParameterDirection.Output;
        //                        retProgressText.SqlDbType = SqlDbType.NVarChar;
        //                        retProgressText.Size = 3000;
        //                        cmd.Parameters.Add(retProgressText);

        //                        SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
        //                        retErrorReference.Direction = ParameterDirection.Output;
        //                        retErrorReference.SqlDbType = SqlDbType.NVarChar;
        //                        retErrorReference.Size = 3000;
        //                        cmd.Parameters.Add(retErrorReference);

        //                        // execute the command
        //                        cmd.CommandTimeout = 1800;  // seconds
        //                        cmd.ExecuteNonQuery(); // errors will be caught in CATCH

        //                        ret = (int)cmd.Parameters["@ReturnCode"].Value;
        //                        ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
        //                        ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
        //                        conn2.Close();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        conn2.Close();
        //                        string Error = ex.Message;
        //                        CatchDetails(ex);
        //                    }
        //                }

        //                if (ret == 0)
        //                {

        //                    // OK
        //                    //MessageBox.Show("VALID CALL" + Environment.NewLine
        //                    // + ProgressText);
        //                }
        //                else
        //                {
        //                    // NOT OK
        //                    MessageBox.Show("NOT VALID CALL - stp_00_DELETE_FROM_ITMX_HST" + InTableId + Environment.NewLine
        //                             + ProgressText);
        //                }
        //                // DELETE FROM HST - Matched
        //                ret = -1;

        //                SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_DELETE_FROM_MATCHED_HST]";

        //                using (SqlConnection conn3 = new SqlConnection(connectionStringITMX))
        //                {
        //                    try
        //                    {
        //                        conn3.Open();

        //                        SqlCommand cmd = new SqlCommand(SPName, conn3);

        //                        cmd.CommandType = CommandType.StoredProcedure;

        //                        // the first are input parameters
        //                        cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));

        //                        cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

        //                        cmd.Parameters.Add(new SqlParameter("@DeleteFromDate", ReversedForDeletion));

        //                        // cmd.Parameters.Add(new SqlParameter("@DeleteFromDate", ReversedForDeletion));

        //                        //@DeleteFromDate

        //                        // the following are output parameters

        //                        SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
        //                        retCode.Direction = ParameterDirection.Output;
        //                        retCode.SqlDbType = SqlDbType.Int;
        //                        cmd.Parameters.Add(retCode);

        //                        SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
        //                        retProgressText.Direction = ParameterDirection.Output;
        //                        retProgressText.SqlDbType = SqlDbType.NVarChar;
        //                        retProgressText.Size = 3000;
        //                        cmd.Parameters.Add(retProgressText);

        //                        SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
        //                        retErrorReference.Direction = ParameterDirection.Output;
        //                        retErrorReference.SqlDbType = SqlDbType.NVarChar;
        //                        retErrorReference.Size = 3000;
        //                        cmd.Parameters.Add(retErrorReference);

        //                        // execute the command
        //                        cmd.CommandTimeout = 1800;  // seconds
        //                        cmd.ExecuteNonQuery(); // errors will be caught in CATCH

        //                        ret = (int)cmd.Parameters["@ReturnCode"].Value;
        //                        ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
        //                        ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
        //                        conn3.Close();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        conn3.Close();
        //                        string Error = ex.Message;
        //                        CatchDetails(ex);
        //                    }
        //                }

        //                if (ret == 0)
        //                {

        //                    // OK
        //                    //MessageBox.Show("VALID CALL" + Environment.NewLine
        //                    // + ProgressText);
        //                }
        //                else
        //                {
        //                    // NOT OK
        //                    MessageBox.Show("NOT VALID CALL - stp_00_DELETE_FROM_MATCHED_HST" + InTableId + Environment.NewLine
        //                             + ProgressText);
        //                }


        //            }


        //        }
        //    }

        //}
        // MOVE TABLE FROM MATCHED TO ITMX
        public void MOVE_MATCHED_TXNS_TO_ITMX_PER_TABLE_NAME(string InTableId, int InReconcCycleNo)
        {
            //******************************
            // stp_00_MOVE_MASTER_TXNS_TO_ITMX_05_EGYPT_123
            //*****************************
            ReturnCode = -20;
            ProgressText = "";
            ErrorReference = "";

            ret = -1;

            SPName = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[stp_00_MOVE_MATCHED_TXNS_TO_ITMX_PER_TABLE_NAME]";

            using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            {
                try
                {
                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters
                    cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 1800;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
                    ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn2.Close();
                    string Error = ex.Message;
                    CatchDetails(ex);
                }
            }

            if (ret == 0)
            {

                // OK
                //MessageBox.Show("VALID CALL" + Environment.NewLine
                // + ProgressText);
            }
            else
            {
                // NOT OK
                MessageBox.Show("NOT VALID CALL - stp_00_MOVE_MATCHED_TXNS_TO_ITMX" + InTableId + Environment.NewLine
                         + ProgressText);
            }

        }

        // MOVE TABLE FROM MATCHED TO ITMX for a table and category
        public void MOVE_MATCHED_TXNS_TO_ITMX_PER_TABLE_NAME_AND_CATEGORY(string InTableId, int InReconcCycleNo, string InCategoryId)
        {
            //******************************
            // stp_00_MOVE_MASTER_TXNS_TO_ITMX_05_EGYPT_123
            //*****************************
            ReturnCode = -20;
            ProgressText = "";
            ErrorReference = "";
            ret = -1;

            SPName = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[stp_00_MOVE_MATCHED_TXNS_TO_ITMX_PER_TABLE_NAME_AND_CATEGORY]";

            using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            {
                try
                {
                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters
                    cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@CategoryId", InCategoryId));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 900;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
                    ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
                    conn2.Close();


                }
                catch (Exception ex)
                {
                    conn2.Close();
                    string Error = ex.Message;
                    CatchDetails(ex);
                }
            }

            if (ret == 0)
            {

                // OK
                //MessageBox.Show("VALID CALL" + Environment.NewLine
                // + ProgressText);
            }
            else
            {
                // NOT OK
                MessageBox.Show("NOT VALID CALL - stp_00_MOVE_MATCHED_TXNS_TO_ITMX_Per CATEGORY.." + InCategoryId + Environment.NewLine
                         + "AND TABLE ID: " + InTableId + Environment.NewLine
                    + ProgressText);
            }

        }

        // MOVE TABLE FROM MATCHED TO ITMX for a table and category
        public void MOVE_MATCHED_TXNS_TO_ITMX_tblMatchingTxnsMasterPoolATMs_AND_Category(string InTableId, int InReconcCycleNo, string InCategoryId)
        {
            //******************************
            // stp_00_MOVE_MASTER_TXNS_TO_ITMX_05_EGYPT_123
            //*****************************
            ReturnCode = -20;
            ProgressText = "";
            ErrorReference = "";
            ret = -1;

            SPName = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[stp_00_MOVE_MATCHED_TXNS_TO_ITMX_tblMatchingTxnsMasterPoolATMs_AND_Category]";

            using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            {
                try
                {
                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@CategoryId", InCategoryId));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 900;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    ProgressText = (string)cmd.Parameters["@ProgressText"].Value;
                    ErrorReference = (string)cmd.Parameters["@ErrorReference"].Value;
                    conn2.Close();


                }
                catch (Exception ex)
                {
                    conn2.Close();
                    string Error = ex.Message;
                    CatchDetails(ex);
                }
            }

            if (ret == 0)
            {

                // OK
                //MessageBox.Show("VALID CALL" + Environment.NewLine
                // + ProgressText);
            }
            else
            {
                // NOT OK
                MessageBox.Show("NOT VALID CALL - stp_00_MOVE_MATCHED_TXNS_TO_ITMX_Per CATEGORY.." + InCategoryId + Environment.NewLine
                         + "AND TABLE ID: " + InTableId + Environment.NewLine
                    + ProgressText);
            }

        }

        //
        // Store procedure to Undo files from Matching
        //
        public void stp_00_UNDO_RMCYCLE_FILES_MATCHING(int InReconcCycleNo, string InCategoryId, string InFileId)
        {
            // CALL STORE PROCEDURE TO UNDO THE REST
            // THESE ARE FILES LIKE Reconciliation Sessions etc. 

            ReturnCode = -20;
            ProgressText = "";
            ErrorReference = "";
            ret = -1;

            string connectionStringITMX = AppConfig.GetConnectionString("ReconConnectionString");

            string SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_UNDO_RMCYCLE_FILES_MATCHING_CATEGORY_ONLY]";

            using (SqlConnection conn2 = new SqlConnection(connectionStringITMX))
            {
                try
                {

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));

                    cmd.Parameters.Add(new SqlParameter("@CategoryId", InCategoryId));

                    cmd.Parameters.Add(new SqlParameter("@SourceFileID", InFileId));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 900;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                    conn2.Close();

                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }
        }


    }
}


