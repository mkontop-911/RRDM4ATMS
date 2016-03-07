using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMPostedTrans
    {
        // POSTED TRANSACTIONS 
        // THESE COME FROM TRANACTIONS TO BE POSTED 
        // Which are created from 
        // 1) Money send to CIT
        // 2) Money in and out from ATM 
        // 3) Correction of Errors during reconciliation
        // 4) Transaction to be posted during Disputes 
        //
        public int TranNo;
        public int TranToBePostedKey; // 
        public string Origin;
        public string UserId; // 
        public string AccNo;
        public string AtmNo;
        public int ReplCycle;
        public string BankId;
    
        public DateTime TranDtTime;
        public int TransType;
        public string TransDesc;

        public string CurrDesc;
        public decimal TranAmount;
        public DateTime ValueDate; 
        public bool OpenRecord;
        public string Operator;

        public decimal TotalDebit12; 
        public decimal TotalCredit22; 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        string WhatFile = "[ATMS].[dbo].[PostedTrans]";

        decimal TotNewAmount;

        // Define the data table 
        public DataTable TablePostedTrans = new DataTable();

        public int TotalSelected;

        public decimal LineBal = 0;
        public decimal OldLineBal = 0;
        public decimal TotalCr = 0;
        public decimal TotalDr = 0;

        string SqlString;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 
        //
        // READ ALL TRANSACTIONS AND FILL THE TABLE   
        //
        public void ReadPostedTransAndFillTheTable(string InOperator, string InFilter,
                                                   DateTime InFromDt, DateTime InToDt, int InFunction)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            LineBal = 0;
            OldLineBal = 0;
            TotalCr = 0;
            TotalDr = 0;

            //int TransType;
            //decimal TranAmount;
            bool FirstTime = true;

            TablePostedTrans = new DataTable();
            TablePostedTrans.Clear();

            // DATA TABLE ROWS DEFINITION 
            //[dbo].[PostedTrans]              
           
            TablePostedTrans.Columns.Add("TranDtTime", typeof(DateTime));
            TablePostedTrans.Columns.Add("TransType", typeof(int));
            TablePostedTrans.Columns.Add("TransDesc", typeof(string));

            TablePostedTrans.Columns.Add("CurrDesc", typeof(string));
            TablePostedTrans.Columns.Add("Debits(-)", typeof(string));
            TablePostedTrans.Columns.Add("Credits(+)", typeof(string));
            TablePostedTrans.Columns.Add("Balance", typeof(string));

            TablePostedTrans.Columns.Add("AtmNo", typeof(string));

            TablePostedTrans.Columns.Add("Origin", typeof(string));
            TablePostedTrans.Columns.Add("UserId", typeof(string));

            TablePostedTrans.Columns.Add("TranNo", typeof(int));
            TablePostedTrans.Columns.Add("ReplCycle", typeof(int));
            TablePostedTrans.Columns.Add("BalDecimal", typeof(decimal));
 
                SqlString = "SELECT *"
                  + " FROM " + WhatFile
                  + " WHERE " + InFilter
                  + " ORDER BY TranNo ASC ";
         

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //if (InFromDt != NullPastDate)
                        //{
                        //    cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                        //    cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        //}

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            TranNo = (int)rdr["TranNo"];
                            TransType = (int)rdr["TransType"];
                            TranAmount = (decimal)rdr["TranAmount"];

                            OldLineBal = LineBal;

                            if (TransType == 21 & InFunction == 1)
                            {
                                LineBal = LineBal + TranAmount;

                            }

                            if (TransType == 11 & InFunction == 1)
                            {
                                LineBal = LineBal - TranAmount;

                            }

                            TranDtTime = (DateTime)rdr["TranDtTime"];

                            // After Opening Balance is calculated we continue 
                            // with the transactions of In Days. 

                            if (TranDtTime.Date >= InFromDt.Date & TranDtTime.Date <= InToDt.Date)
                            {
                                RecordFound = true;

                                if (FirstTime == true & InFunction == 1)
                                {

                                    FirstTime = false;
                                    DataRow RowWa = TablePostedTrans.NewRow();
                                    //RowWa["TranNo"] = TranNo;
                                    RowWa["TransDesc"] = "Balance B/F";

                                    RowWa["Balance"] = OldLineBal.ToString("#,##0.00");

                                    TablePostedTrans.Rows.Add(RowWa);

                                }

                                if (TransType == 21)
                                {
                                    TotalCr = TotalCr + TranAmount;
                                }

                                if (TransType == 11)
                                {
                                    TotalDr = TotalDr + TranAmount;
                                }

                                // Add ROW IN Table 
                                DataRow Row= TablePostedTrans.NewRow();

                                Row["TranDtTime"] = TranDtTime.Date;
                                //RowW["TranDtTime"] = (DateTime)rdr["TranDtTime"];
                                Row["TransType"] = TransType;
                                string TempDescr = (string)rdr["TransDesc"];
                                if (TransType == 12 || TransType == 22)
                                {
                                    TempDescr = TempDescr + " (Affects Mainframe Bal only)";
                                }
                                Row["TransDesc"] = TempDescr;
                                Row["CurrDesc"] = (string)rdr["CurrDesc"];

                                if (TransType == 11 || TransType == 12)
                                {
                                    Row["Debits(-)"] = TranAmount.ToString("#,##0.00");
                                    Row["Credits(+)"] = "";
                                }

                                if (TransType == 21 || TransType == 22)
                                {
                                    Row["Debits(-)"] = "";
                                    Row["Credits(+)"] = TranAmount.ToString("#,##0.00");
                                }
                                Row["Balance"] = LineBal.ToString("#,##0.00");

                                Row["AtmNo"] = (string)rdr["AtmNo"];

                                Row["Origin"] = (string)rdr["Origin"];
                                Row["UserId"] = (string)rdr["UserId"];

                                Row["TranNo"] = TranNo;

                                Row["ReplCycle"] = (int)rdr["ReplCycle"];
                                Row["BalDecimal"] = LineBal;

                                TablePostedTrans.Rows.Add(Row);
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
                    ErrorOutput = "An error occured in PostedTrans Class............. " + ex.Message;

                }
        }
        //
        // READ ALL TRANSACTIONS FROM IN POSTED By Acc no   
        //
        public void ReadPostedTransByAccno(int InAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
               + " FROM [dbo].[PostedTrans]"
               + " WHERE AccNo = @AccNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccNo", InAccNo);
                       
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];

                            TranToBePostedKey = (int)rdr["TranToBePostedKey"];

                            Origin = (string)rdr["Origin"];
                            UserId = (string)rdr["UserId"];

                            AccNo = (string)rdr["AccNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            ReplCycle = (int)rdr["ReplCycle"];
                            BankId = (string)rdr["BankId"];


                            TranDtTime = (DateTime)rdr["TranDtTime"];
                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];

                            CurrDesc = (string)rdr["CurrDesc"];

                            TranAmount = (decimal)rdr["TranAmount"];

                            ValueDate = (DateTime)rdr["ValueDate"];

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
                    ErrorOutput = "An error occured in PostedTrans Class............. " + ex.Message;

                }
        }

        //
        // READ SPECIFIC TRANSACTION FROM IN POOL By USER  
        //
        public void ReadTranSpecific(int InTranNo, string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT *"
               + " FROM [dbo].[PostedTrans]"
               + " WHERE TranNo = @TranNo AND UserId=@UserId";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TranNo", InTranNo);
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            
                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];

                            TranToBePostedKey = (int)rdr["TranToBePostedKey"];

                            Origin = (string)rdr["Origin"];

                            UserId = (string)rdr["UserId"];

                            AccNo = (string)rdr["AccNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            ReplCycle = (int)rdr["ReplCycle"];
                            BankId = (string)rdr["BankId"];
                 
                            TranDtTime = (DateTime)rdr["TranDtTime"]; 
                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];
                          
                            CurrDesc = (string)rdr["CurrDesc"];

                            TranAmount = (decimal)rdr["TranAmount"];

                            ValueDate = (DateTime)rdr["ValueDate"];

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
                    ErrorOutput = "An error occured in PostedTrans Class............. " + ex.Message;

                }
        }
        //
        // READ TRANSACTIONs AND Find Totals per In Account and ATM 
        //
        public void ReadTransForAccountTotals(string InAtmNo, string InAccNo, DateTime InTranDtTime)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalDebit12 = 0 ;
            TotalCredit22 = 0 ;

            SqlString = "SELECT *"
               + " FROM [dbo].[PostedTrans]"
               + " WHERE AtmNo = @AtmNo AND AccNo=@AccNo AND TranDtTime > @TranDtTime AND OpenRecord = 1";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AccNo", InAccNo);
                        cmd.Parameters.AddWithValue("@TranDtTime", InTranDtTime);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];

                            TranToBePostedKey = (int)rdr["TranToBePostedKey"];

                            Origin = (string)rdr["Origin"];

                            UserId = (string)rdr["UserId"];

                            AccNo = (string)rdr["AccNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            ReplCycle = (int)rdr["ReplCycle"];
                            BankId = (string)rdr["BankId"];

                            TranDtTime = (DateTime)rdr["TranDtTime"];
                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];

                            CurrDesc = (string)rdr["CurrDesc"];

                            TranAmount = (decimal)rdr["TranAmount"];

                            ValueDate = (DateTime)rdr["ValueDate"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

                            if (TransType == 12)
                            {
                                TotalDebit12 = TotalDebit12 + TranAmount; 
                            }
                            if (TransType == 22)
                            {
                                TotalCredit22 = TotalCredit22 + TranAmount;
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
                    ErrorOutput = "An error occured in PostedTrans Class............. " + ex.Message;

                }
        }  

        // Insert TRANS in Posted Table 
        //
        public void InsertTran(int InTransToBePostedKey,string InOrigin)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[PostedTrans] "
                       + "([TranToBePostedKey],[Origin], [UserId], [AccNo], [AtmNo], [ReplCycle], [BankId], "
                       + " [TranDtTime], [TransType], [TransDesc],  [CurrDesc], [TranAmount], [ValueDate], [OpenRecord],[Operator] )"
                       + " VALUES (@TranToBePostedKey, @Origin, @UserId, @AccNo, @AtmNo, @ReplCycle, @BankId, "
                       + " @TranDtTime, @TransType, @TransDesc, @CurrDesc, @TranAmount, @ValueDate, @OpenRecord, @Operator)";


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@TranToBePostedKey", TranToBePostedKey);
                        cmd.Parameters.AddWithValue("@Origin", InOrigin);
                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", ReplCycle);
                        cmd.Parameters.AddWithValue("@BankId", BankId);


                        cmd.Parameters.AddWithValue("@TranDtTime", TranDtTime);

                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDesc", TransDesc);


                        cmd.Parameters.AddWithValue("@CurrDesc", CurrDesc);
                        cmd.Parameters.AddWithValue("@TranAmount", TranAmount);

                        cmd.Parameters.AddWithValue("@ValueDate", ValueDate);

                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);

                        cmd.Parameters.AddWithValue("@Operator", Operator);
                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in PostedTrans Class............. " + ex.Message;
                }
        }


        // UPDATE 
        // 
        public void UpdateTranCitAsClose(int InTranNo, bool InOpenRecord)
           {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[PostedTrans] SET "
                            + " OpenRecord = @OpenRecord "
                            + " WHERE TranNo = @TranNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@TranNo", InTranNo);
                        cmd.Parameters.AddWithValue("@OpenRecord", InOpenRecord);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in PostedTrans Class............. " + ex.Message;
                }
        }

        // Closed all transactions with in Posted No
        // 
        public void UpdateAsClosedTheAlreadyInTable(int InTranToBePostedKey)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[PostedTrans] SET "
                            + " OpenRecord = 0 "
                            + " WHERE TranToBePostedKey = @TranToBePostedKey", conn))
                    {
                        cmd.Parameters.AddWithValue("@TranToBePostedKey", InTranToBePostedKey);
                       
                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in PostedTrans Class............. " + ex.Message;
                }
        }
        //// Insert TRANS To Cit Statement based on UserId 
        ////
        //public void InsertTranInCit(string InUserId, string InAccNo, string InOrigin)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = ""; 

        //    string cmdinsert = "INSERT INTO [dbo].[PostedTrans]"
        //            + "([TranToBePostedKey],[Origin], [UserId], [AccNo], [AtmNo], [ReplCycle], [BankId], "
        //            + " [TranDtTime], [TransType], [TransDesc],  [CurrDesc], [TranAmount], [ValueDate], [OpenRecord],[Operator] )"
        //            + " VALUES (@TranToBePostedKey, @Origin, @UserId, @AccNo, @AtmNo, @ReplCycle, @BankId, "
        //            + " @TranDtTime, @TransType, @TransDesc, @CurrDesc, @TranAmount, @ValueDate, @OpenRecord, @Operator)";


        //    using (SqlConnection conn =
        //        new SqlConnection(connectionString))
        //        try
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd =

        //               new SqlCommand(cmdinsert, conn))
        //            {
        //                cmd.Parameters.AddWithValue("@TranToBePostedKey", TranToBePostedKey);
        //                cmd.Parameters.AddWithValue("@Origin", Origin);
        //                cmd.Parameters.AddWithValue("@UserId", InUserId);
        //                cmd.Parameters.AddWithValue("@AccNo", InAccNo);

        //                cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
        //                cmd.Parameters.AddWithValue("@ReplCycle", ReplCycle);
        //                cmd.Parameters.AddWithValue("@BankId", BankId);


        //                cmd.Parameters.AddWithValue("@TranDtTime", TranDtTime);

        //                cmd.Parameters.AddWithValue("@TransType", TransType);
        //                cmd.Parameters.AddWithValue("@TransDesc", TransDesc);


        //                cmd.Parameters.AddWithValue("@CurrDesc", CurrDesc);
        //                cmd.Parameters.AddWithValue("@TranAmount", TranAmount);

        //                cmd.Parameters.AddWithValue("@ValueDate", ValueDate);

        //                cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);

        //                cmd.Parameters.AddWithValue("@Operator", Operator);
        //                //rows number of record got updated

        //                int rows = cmd.ExecuteNonQuery();
        //                //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
        //                //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

        //            }
        //            // Close conn
        //            conn.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            conn.Close();
        //            ErrorFound = true;
        //            ErrorOutput = "An error occured in PostedTrans Class............. " + ex.Message;
        //        }
        //}

    }
}

