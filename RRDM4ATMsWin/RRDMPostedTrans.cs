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


namespace RRDM4ATMsWin
{
    class RRDMPostedTrans
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
        public int TranOrigin;
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

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        //
        // READ ALL TRANSACTIONS FROM IN POSTED By Acc no   
        //
        public void ReadPostedTransByAccno(int InAccNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
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

                            TranOrigin = (int)rdr["TranOrigin"];
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

            string SqlString = "SELECT *"
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
                            
                            TranOrigin = (int)rdr["TranOrigin"];
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

        // Insert TRANS To Cit Statement based on UserId 
        //
        public void InsertTranInCit(string UserId, string AccNo)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [dbo].[PostedTrans]"
                    + "([TranToBePostedKey],[TranOrigin], [UserId], [AccNo], [AtmNo], [ReplCycle], [BankId], "
                    + " [TranDtTime], [TransType], [TransDesc],  [CurrDesc], [TranAmount], [ValueDate], [OpenRecord],[Operator] )"
                    + " VALUES (@TranToBePostedKey, @TranOrigin, @UserId, @AccNo, @AtmNo, @ReplCycle, @BankId, "
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
                        cmd.Parameters.AddWithValue("@TranOrigin", TranOrigin);
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

        // Insert TRANS in Posted Table 
        //
        public void InsertTran(int InTransToBePostedKey)
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[PostedTrans]"
                       + "([TranToBePostedKey],[TranOrigin], [UserId], [AccNo], [AtmNo], [ReplCycle], [BankId], "
                       + " [TranDtTime], [TransType], [TransDesc],  [CurrDesc], [TranAmount], [ValueDate], [OpenRecord],[Operator] )"
                       + " VALUES (@TranToBePostedKey, @TranOrigin, @UserId, @AccNo, @AtmNo, @ReplCycle, @BankId, "
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
                        cmd.Parameters.AddWithValue("@TranOrigin", TranOrigin);
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


    }
}

