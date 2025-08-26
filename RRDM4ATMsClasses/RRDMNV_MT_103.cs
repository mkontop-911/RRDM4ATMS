using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMNV_MT_103 : Logger
    {
        public RRDMNV_MT_103() : base() { }

        public int SeqNo;
        public string MessageType;
        public string Sender;
        public string Receiver;

        public string Reference;
        public DateTime ValueDate;
        public string Ccy;
        public decimal Amount;

        public string DR_Account;
        public string CR_Account;
        public string SenderInfo;
        public string ReceiverInfo;

        public string RemittanceInfo2;
        public string CommisionType;
        public string CcyComm;
        public decimal CommisionAmt;

        public bool Processed;
        public bool Correct;
        public string ErrorType;
        public string Maker;
        public string Authoriser;
        public bool Settled;
        public int UniqueRecordId; 

        // Define the data table 
        public DataTable MT_103_DataTable;
        public DataTable MT_103_OriginBanks;

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // MT_103 Fields
        private void ReadMT_103_Fields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
           
            MessageType = (string)rdr["MessageType"];
            Sender = (string)rdr["Sender"];
            Receiver = (string)rdr["Receiver"];
            
            Reference = (string)rdr["Reference"];
            ValueDate = (DateTime)rdr["ValueDate"];
            Ccy = (string)rdr["Ccy"];
            Amount = (decimal)rdr["Amount"];
            
            DR_Account = (string)rdr["DR_Account"];
            CR_Account = (string)rdr["CR_Account"];
            SenderInfo = (string)rdr["SenderInfo"];
            ReceiverInfo = (string)rdr["ReceiverInfo"];
            
            RemittanceInfo2 = (string)rdr["RemittanceInfo2"];
            CommisionType = (string)rdr["CommisionType"];
            CcyComm = (string)rdr["CcyComm"];
            CommisionAmt = (decimal)rdr["CommisionAmt"];

            Processed = (bool)rdr["Processed"];
            Correct = (bool)rdr["Correct"];
            ErrorType = (string)rdr["ErrorType"];
            Maker = (string)rdr["Maker"];
            Authoriser = (string)rdr["Authoriser"];
            Settled = (bool)rdr["Settled"];
            UniqueRecordId = (int)rdr["UniqueRecordId"];

        }

        
        //
        // READ MT_103 BASED ON SeqNo 
        //
        public void ReadActionBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MT_103_Table] "
                         + " WHERE SeqNo = @SeqNo  "
                          + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadMT_103_Fields(rdr);

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

        //
        // READ Action AND Filled table OriginBanks
        //

        public void ReadMT_103AndFillTableOriginBanks(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MT_103_OriginBanks = new DataTable();
            MT_103_OriginBanks.Clear();

            string SqlString = "SELECT DISTINCT Sender "
                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MT_103_Table] "
                         + InSelectionCriteria
                          + " ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InAtmNo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(MT_103_OriginBanks);

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

        //
        // READ Action AND Filled table
        //

        public void ReadMT_103AndFillTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            MT_103_DataTable = new DataTable();
            MT_103_DataTable.Clear();

            string SqlString = "SELECT * "
                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MT_103_Table] "
                         + InSelectionCriteria
                          + " ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InAtmNo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(MT_103_DataTable);

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

        //
        // Methods 
        // ReadNV_MT_103_BULK
        // 
        //
        public string ab;
        public void ReadNV_MT_103_BULK(string InThis_20_Reference)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ab = "";

            ab +=   "MT 103 Details" + "\r\n"; 

            SqlString = "SELECT *"
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[BULK_MT_103] "
                      + " WHERE This_20_Reference = @This_20_Reference";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@This_20_Reference", InThis_20_Reference);
                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;
                          
                            string Sender = (string)rdr["Sender"];
                            ab += "Sender" + "\r\n";
                            ab += Sender + "\r\n";
                            string Messagetype = (string)rdr["Messagetype"];
                            ab += "Messagetype" + "\r\n";
                            ab += Messagetype + "\r\n";
                            string Receiver = (string)rdr["Receiver"];
                            ab += "Receiver" + "\r\n";
                            ab += Receiver + "\r\n"; 
                            string Reference_Combine = (string)rdr["Reference_Combine"];
                            ab += "Reference_Combine" + "\r\n";
                            ab += Reference_Combine + "\r\n";

                            string This_20_Reference = (string)rdr["This_20_Reference"];
                            ab += "This_20_Reference" + "\r\n";
                            ab += This_20_Reference + "\r\n";
                            string This_23B = (string)rdr["This_23B"];
                            ab += "This_23B" + "\r\n";
                            ab += This_23B + "\r\n";
                            string This_32A_ValueDateAndAmount = (string)rdr["This_32A_ValueDateAndAmount"];
                            ab += "This_32A_ValueDateAndAmount" + "\r\n";
                            ab += This_32A_ValueDateAndAmount + "\r\n";
                            string This_33B = (string)rdr["This_33B"];
                            ab += "This_33B" + "\r\n";
                            ab += This_33B + "\r\n";

                            string This_50K_OrderringCustomer = (string)rdr["This_50K_OrderringCustomer"];
                            ab += "This_50K_OrderringCustomer" + "\r\n";
                            ab += This_50K_OrderringCustomer + "\r\n";
                            string This_53B_SenderAccount_etc = (string)rdr["This_53B_SenderAccount_etc"];
                            ab += "his_53B_SenderAccount_etc" + "\r\n";
                            ab += This_53B_SenderAccount_etc + "\r\n";
                            string This_59_BeneficiaryAccNo_etc = (string)rdr["This_59_BeneficiaryAccNo_etc"];
                            ab += "This_59_BeneficiaryAccNo_etc" + "\r\n";
                            ab += This_59_BeneficiaryAccNo_etc + "\r\n";
                            string This_70_RemittanceInformation = (string)rdr["This_70_RemittanceInformation"];
                            ab += "This_70_RemittanceInformation" + "\r\n";
                            ab += This_70_RemittanceInformation + "\r\n";

                            string This_71A_DetailsOfCharges = (string)rdr["This_71A_DetailsOfCharges"];
                            ab += "This_71A_DetailsOfCharges" + "\r\n";
                            ab += This_71A_DetailsOfCharges + "\r\n";
                            string This_71G_ReceiversCharge = (string)rdr["This_71G_ReceiversCharge"];
                            ab += "This_71G_ReceiversCharge" + "\r\n";
                            ab += This_71G_ReceiversCharge + "\r\n";

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
       

    }
}
