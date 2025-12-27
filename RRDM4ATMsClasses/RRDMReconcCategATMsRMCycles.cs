using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;

namespace RRDM4ATMs
{
    public class RRDMReconcCategATMsRMCycles
    {

        public int SeqNo;
        public string CategoryId;
        public int RMCycle;

        public string AtmNo; 

        public DateTime CreatedDtTm;
        public string Currency;
        public decimal OpeningBalance;
        public decimal TotalJournalAmt;

        public decimal TotalMatchedAmt;
        public decimal TotalTransAmtAdj;
        public bool OpenRecord;
        public string Operator;

       

        // Define the data table 
        public DataTable TableMatchingSessionsPerCategory = new DataTable();

        public int TotalSelected;
        public int TotalRemainExceptions;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;


        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        //
        // Methods 
        // READ RM ATM Cycles 
        // FILL UP A TABLE
        //
        public void ReadReconcCategoriesATMsRMCycleSpecific(string InOperator, string InCategoryId, int InRMCycle, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReconcCategoriesMatchingWATMsRMCycles] "
                    + " WHERE Operator = @Operator AND CategoryId = @CategoryId AND RMCycle = @RMCycle AND AtmNo = @AtmNo"
                    + " ORDER BY SeqNo DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            CategoryId = (string)rdr["CategoryId"];
                            RMCycle = (int)rdr["RMCycle"];
                            AtmNo = (string)rdr["AtmNo"];

                            CreatedDtTm = (DateTime)rdr["CreatedDtTm"];

                            Currency = (string)rdr["Currency"];
                           
                            OpeningBalance = (decimal)rdr["OpeningBalance"];
                            TotalJournalAmt = (decimal)rdr["TotalJournalAmt"];
                            TotalMatchedAmt = (decimal)rdr["TotalMatchedAmt"];
                            TotalTransAmtAdj = (decimal)rdr["TotalTransAmtAdj"];
                            OpenRecord = (bool)rdr["OpenRecord"];
                            Operator = (string)rdr["Operator"];

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ReadReconcCategoriesATMsRMCycleSpecific(string InOperator, string InCategoryId, int InRMCycle, string InAtmNo)......... " + ex.Message;

                }
        }

        ////
        //// Methods 
        //// READ ReconcCategoriesMatchingSessions Specific 
        //// 
        ////
        //public void ReadReconcCategoriesMatchingSessionsByRmCycle(string InOperator, int InSeqNo)
        //{
        //    RecordFound = false;
        //    ErrorFound = false;
        //    ErrorOutput = "";


        //    SqlString = "SELECT *"
        //            + " FROM [ATMS].[dbo].[ReconcCategoriesMatchingSessions] "
        //            + " WHERE Operator = @Operator AND SeqNo = @SeqNo ";

        //    using (SqlConnection conn =
        //                  new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand(SqlString, conn))
        //            {

        //                cmd.Parameters.AddWithValue("@Operator", InOperator);
        //                cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

        //                // Read table 

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                while (rdr.Read())
        //                {

        //                    RecordFound = true;

        //                    SeqNo = (int)rdr["SeqNo"];

        //                    CategoryId = (string)rdr["CategoryId"];
        //                    RMCycle = (int)rdr["RMCycle"];
        //                    AtmNo = (string)rdr["AtmNo"];

        //                    CreatedDtTm = (DateTime)rdr["CreatedDtTm"];

        //                    Currency = (string)rdr["Currency"];

        //                    OpeningBalance = (decimal)rdr["OpeningBalance"];
        //                    TotalJournalAmt = (decimal)rdr["TotalJournalAmt"];
        //                    TotalMatchedAmt = (decimal)rdr["TotalMatchedAmt"];
        //                    TotalTransAmtAdj = (decimal)rdr["TotalTransAmtAdj"];
        //                    OpenRecord = (bool)rdr["OpenRecord"];
        //                    Operator = (string)rdr["Operator"];

        //                }

        //                // Close Reader
        //                rdr.Close();
        //            }

        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            ErrorFound = true;
        //            ErrorOutput = "An error occured in ReconcCategoriesMatchingSessions......... " + ex.Message;

        //        }
        //}
        //// 
        //// UPDATE RM Category Cycle Start date time 
        //// 
        //public void UpdateCategRMCycleWithReconStartDate(string InCategoryId, int InMatchSession)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    using (SqlConnection conn =
        //        new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesMatchingSessions] SET "
        //                    + " StartReconcDtTm = @StartReconcDtTm "
        //                    + " WHERE SeqNo = @SeqNo", conn))
        //            {
        //                cmd.Parameters.AddWithValue("@SeqNo", InMatchSession);
        //                cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
        //                //cmd.Parameters.AddWithValue("@StartReconcDtTm", StartReconcDtTm);

        //                int rows = cmd.ExecuteNonQuery();
        //                //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
        //                //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            ErrorFound = true;
        //            ErrorOutput = "An error occured in UpdateCategMatchSession Class............. " + ex.Message;
        //        }
        //}
        ////
        //// UPDATE Category Session at Reconciliation closing 
        //// 
        //public void UpdateCategRMCycleWithAuthorClosing(string InCategoryId, int InRMCycle)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = "";

        //    using (SqlConnection conn =
        //        new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =
        //                new SqlCommand("UPDATE [ATMS].[dbo].[ReconcCategoriesMatchingSessions] SET "
        //                    + " EndReconcDtTm = @EndReconcDtTm , RemainReconcExceptions = @RemainReconcExceptions "
        //                    + " WHERE SeqNo = @SeqNo", conn))
        //            {
        //                cmd.Parameters.AddWithValue("@SeqNo", InRMCycle);
        //                cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
        //                cmd.Parameters.AddWithValue("@EndReconcDtTm", DateTime.Now);
        //                //cmd.Parameters.AddWithValue("@RemainReconcExceptions", RemainReconcExceptions);

        //                int rows = cmd.ExecuteNonQuery();
        //                //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
        //                //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            ErrorFound = true;
        //            ErrorOutput = "An error occured in UpdateCategMatchSession Class............. " + ex.Message;
        //        }
        //}
        // ERRORS 

        decimal ErrAmount;

        public int NumberOfErrors;
        public int NumberOfErrJournal;
        public int ErrJournalThisCycle;
        public int NumberOfErrDep;
        public int NumberOfErrHost;
        public int ErrHostToday;
        public int ErrOutstanding; // Action was not taken on them 
        public int ErrorsAdjastingBalances;

        public bool ErrorsFound;
        public int WBanksClosedBalNubOfErr;
        public int WBanksClosedBalErrOutstanding;
        //public string WBanksClosedBalCurrNm; 
        public decimal BanksClosedBalAdjWithErrors;

        public int WSesNo;
        public int WFunction;

        // READ Errors TO CALCULATE REFRESED BALANCES 
        //
        public void ReadAllErrorsTableFromCategSessionForATMAddErrors(string InCategoryId, int InActionSes, string InAtmNo, decimal InBanksClosedBal, int InFunction)
        {
            WSesNo = InActionSes;
            WFunction = InFunction;
            BanksClosedBalAdjWithErrors = InBanksClosedBal;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ErrorsFound = false; // Related to errors 

            NumberOfErrors = 0;
            NumberOfErrJournal = 0;
            ErrJournalThisCycle = 0;
            NumberOfErrDep = 0;
            NumberOfErrHost = 0;
            ErrHostToday = 0;
            ErrOutstanding = 0;

            ErrorsAdjastingBalances = 0;

            WBanksClosedBalNubOfErr = 0;
            WBanksClosedBalErrOutstanding = 0;

            int ErrId; int ErrType;
            string ErrDesc; int TraceNo; int SesNo;
            int TransNo; int TransType; string TransDescr;
            DateTime DateTime; bool NeedAction;

            string CurDes;
            bool DrCust; bool CrCust; bool UnderAction;
            bool ManualAct; bool DrAtmCash; bool CrAtmCash;
            bool DrAtmSusp; bool CrAtmSusp; bool MainOnly;

            SqlString = "SELECT *"
                 + " FROM [dbo].[ErrorsTable] "
                 + " WHERE CategoryId = @CategoryId AND SesNo<=@ActionSes AND AtmNo=@AtmNo AND (OpenErr=1 OR (OpenErr=0 AND ActionSes = @ActionSes))  ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", InCategoryId);
                        cmd.Parameters.AddWithValue("@ActionSes", InActionSes);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ErrorsFound = true;

                            // Read error Details

                            ErrId = (int)rdr["ErrId"];
                            ErrType = (int)rdr["ErrType"];
                            ErrDesc = rdr["ErrDesc"].ToString();
                            AtmNo = rdr["AtmNo"].ToString();
                            SesNo = (int)rdr["SesNo"];
                            TraceNo = (int)rdr["TraceNo"];
                            TransNo = (int)rdr["TransNo"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = rdr["TransDescr"].ToString();
                            DateTime = (DateTime)rdr["DateTime"];
                            NeedAction = (bool)rdr["NeedAction"];

                            CurDes = rdr["CurDes"].ToString();
                            ErrAmount = (decimal)rdr["ErrAmount"];
                            DrCust = (bool)rdr["DrCust"];
                            CrCust = (bool)rdr["CrCust"];
                            UnderAction = (bool)rdr["UnderAction"];
                            ManualAct = (bool)rdr["ManualAct"];
                            DrAtmCash = (bool)rdr["DrAtmCash"];
                            CrAtmCash = (bool)rdr["CrAtmCash"];
                            DrAtmSusp = (bool)rdr["DrAtmSusp"];
                            CrAtmSusp = (bool)rdr["CrAtmSusp"];
                            MainOnly = (bool)rdr["MainOnly"];

                            NumberOfErrors = NumberOfErrors + 1;

                            if (UnderAction == true & ErrId != 165)
                            {
                                BanksClosedBalAdjWithErrors = BanksClosedBalAdjWithErrors + ErrAmount; 
                            }

                           
                          
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in NotesBalances Class............. " + ex.Message;
                }
        }   


    }
}


