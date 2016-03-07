using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
namespace RRDM4ATMsWin
{
    class RRDMHostTblsClass
    {
        // DECLARE General Ledger Transactions FIELDS
        //
        public int TranNo;
        public string BankId;
        public bool Prive;
       
        public string AtmNo;
        public string AccNo;

        public string Curr;
        public decimal TranAmnt; 

        public DateTime HostDtTm;
        public int RefNo;

        public bool TranMatched; 
        public DateTime MatchedDtTm;
        public string Comment;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ Specific Trans in General Ledger Transactions by TranNo

        public void ReadHostGeneralLedgerSpecificTranNo(int InTranNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[tblHHostGeneralLedger] "
          + " WHERE TranNo = @TranNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TranNo", InTranNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read trans Details
                            TranNo = (int)rdr["TranNo"];
                            BankId = (string)rdr["BankId"];
                            Prive = (bool)rdr["Prive"];
                            AtmNo = (string)rdr["AtmNo"];
                            AccNo = (string)rdr["AccNo"];
                            Curr = (string)rdr["Curr"];
                            TranAmnt = (decimal)rdr["TranAmnt"];
                            HostDtTm = (DateTime)rdr["HostDtTm"];
                            RefNo = (int)rdr["RefNo"];

                            TranMatched = (bool)rdr["TranMatched"];
                            MatchedDtTm = (DateTime)rdr["MatchedDtTm"];

                            Comment = (string)rdr["Comment"];

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
                    ErrorOutput = "An error occured in HostTblsClass............. " + ex.Message;

                }
        }

        // READ Specific Trans in General Ledger Transactions by Specific Ref no

        public void ReadHostGeneralLedgerSpecificRefNo(string InAtmNo, int InRefNo)
        {
            RecordFound = false;

            string SqlString = "SELECT *"
                  + " FROM [dbo].[tblHHostGeneralLedger] "
                  + " WHERE AtmNo = @AtmNo AND RefNo = @RefNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@RefNo", InRefNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read trans Details
                            TranNo = (int)rdr["TranNo"];
                            BankId = (string)rdr["BankId"];
                         
                            AtmNo = (string)rdr["AtmNo"];
                            AccNo = (string)rdr["AccNo"];
                            Curr = (string)rdr["Curr"];
                            TranAmnt = (decimal)rdr["TranAmnt"];
                            HostDtTm = (DateTime)rdr["HostDtTm"];
                            RefNo = (int)rdr["RefNo"];

                            TranMatched = (bool)rdr["TranMatched"];
                            MatchedDtTm = (DateTime)rdr["MatchedDtTm"];

                            Comment = (string)rdr["Comment"];

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
                    ErrorOutput = "An error occured in HostTblsClass............. " + ex.Message;
                }
        }

        // UPDATE HOST transaction with matched fields by Ref No which is the trace No
        // 
        public void UpdateHostGeneralLedgerSpecific(string InAtmNo, int InRefNo)
        {

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE tblHHostGeneralLedger SET "
                             + "TranMatched = @TranMatched, MatchedDtTm = @MatchedDtTm, Comment = @Comment "
                             + " WHERE AtmNo = @AtmNo AND RefNo = @RefNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@RefNo", InRefNo);
                        cmd.Parameters.AddWithValue("@TranMatched", TranMatched);
                        cmd.Parameters.AddWithValue("@MatchedDtTm", MatchedDtTm);

                        cmd.Parameters.AddWithValue("@Comment", Comment);

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
                    ErrorOutput = "An error occured in HostTblsClass............. " + ex.Message;
                }
        }
    }
}
