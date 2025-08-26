using System;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;
using System.Data;
//using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMNVStatement_APool_External : Logger
    {
        public RRDMNVStatement_APool_External() : base() { }

        public int SeqNo;
     
        public int JobCycle;
        public string StmtTrxReferenceNumber;
        public string StmtAccountID;
        public int StmtNumber;
       
        public int StmtSequenceNumber;
        public bool StmtOpeningBalanceIsFirst;
        public bool StmtOpeningBalanceIsDebit;
        public DateTime StmtOpeningBalanceDate;

        public string StmtOpeningBalanceCurrency;
        public decimal StmtOpeningBalanceAmt;
        public bool StmtClosingBalanceIsFinal;
        public bool StmtClosingBalanceIsDebit;
    
        public DateTime StmtClosingBalanceDate;
        public string StmtClosingBalanceCurrency;
        public decimal StmtClosingBalanceAmt;

        public string Operator;


        public string InternalAccNo;
        public string ExternalBankID;
        public string ExternalAccNo;
        public string Ccy;
        public DateTime DateTimeCreated;

        public decimal RangeAmtFrom;
        public decimal RangeAmtTo;
        public int LimitDays; 

        // Define the data table 
        public DataTable TableStatementsExternal = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;
        //
        // Methods 
        // READ Statement_APool_External
        // 
        //
        public void ReadNVStatement_APool_ExternalById(string InOperator,
                                       string InStmtTrxReferenceNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVStatement_APool_External] "
                      + " WHERE Operator = @Operator AND StmtTrxReferenceNumber = @StmtTrxReferenceNumber ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@StmtTrxReferenceNumber", InStmtTrxReferenceNumber);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            JobCycle = (int)rdr["JobCycle"];
                            StmtTrxReferenceNumber = (string)rdr["StmtTrxReferenceNumber"];
                            StmtAccountID = (string)rdr["StmtAccountID"];
                            StmtNumber = (int)rdr["StmtNumber"];

                            StmtSequenceNumber = (int)rdr["StmtSequenceNumber"];
                            StmtOpeningBalanceIsFirst = (bool)rdr["StmtOpeningBalanceIsFirst"];
                            StmtOpeningBalanceIsDebit = (bool)rdr["StmtOpeningBalanceIsDebit"];
                            StmtOpeningBalanceDate = (DateTime)rdr["StmtOpeningBalanceDate"];

                            StmtOpeningBalanceCurrency = (string)rdr["StmtOpeningBalanceCurrency"];
                            StmtOpeningBalanceAmt = (decimal)rdr["StmtOpeningBalanceAmt"];
                            StmtClosingBalanceIsFinal = (bool)rdr["StmtClosingBalanceIsFinal"];
                            StmtClosingBalanceIsDebit = (bool)rdr["StmtClosingBalanceIsDebit"];
                            
                            StmtClosingBalanceDate = (DateTime)rdr["StmtClosingBalanceDate"];
                            StmtClosingBalanceCurrency = (string)rdr["StmtClosingBalanceCurrency"];
                            StmtClosingBalanceAmt = (decimal)rdr["StmtClosingBalanceAmt"];

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

                    CatchDetails(ex);

                }
        }

        //
        // Methods 
        // READ NVStatement_APool_External
        // FILL UP A TABLE
        //
        public void ReadNVStatement_APool_ExternalFillTable(string InOperator, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableStatementsExternal = new DataTable();
            TableStatementsExternal.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableStatementsExternal.Columns.Add("SeqNo", typeof(int));
            TableStatementsExternal.Columns.Add("BankID", typeof(string));
            TableStatementsExternal.Columns.Add("ExternalAccNo", typeof(string));
            TableStatementsExternal.Columns.Add("Ccy", typeof(string));
            TableStatementsExternal.Columns.Add("OpeningBalanceDate", typeof(DateTime));
            TableStatementsExternal.Columns.Add("OpeningBalanceAmt", typeof(decimal));
            TableStatementsExternal.Columns.Add("ClosingBalanceAmt", typeof(decimal));
            TableStatementsExternal.Columns.Add("ClosingBalanceDate", typeof(DateTime));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[NVStatement_APool_External] "
                    + InSelectionCriteria
                    ;
               
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);
                      

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            JobCycle = (int)rdr["JobCycle"];
                            StmtTrxReferenceNumber = (string)rdr["StmtTrxReferenceNumber"];
                            StmtAccountID = (string)rdr["StmtAccountID"];
                            StmtNumber = (int)rdr["StmtNumber"];

                            StmtSequenceNumber = (int)rdr["StmtSequenceNumber"];
                            StmtOpeningBalanceIsFirst = (bool)rdr["StmtOpeningBalanceIsFirst"];
                            StmtOpeningBalanceIsDebit = (bool)rdr["StmtOpeningBalanceIsDebit"];
                            StmtOpeningBalanceDate = (DateTime)rdr["StmtOpeningBalanceDate"];

                            StmtOpeningBalanceCurrency = (string)rdr["StmtOpeningBalanceCurrency"];
                            StmtOpeningBalanceAmt = (decimal)rdr["StmtOpeningBalanceAmt"];
                            StmtClosingBalanceIsFinal = (bool)rdr["StmtClosingBalanceIsFinal"];
                            StmtClosingBalanceIsDebit = (bool)rdr["StmtClosingBalanceIsDebit"];

                            StmtClosingBalanceDate = (DateTime)rdr["StmtClosingBalanceDate"];
                            StmtClosingBalanceCurrency = (string)rdr["StmtClosingBalanceCurrency"];
                            StmtClosingBalanceAmt = (decimal)rdr["StmtClosingBalanceAmt"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableStatementsExternal.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["BankID"] = ExternalBankID;
                            RowSelected["ExternalAccNo"] = ExternalAccNo;
                            RowSelected["Ccy"] = Ccy;

                            RowSelected["OpeningBalanceDate"] = StmtOpeningBalanceDate;
                            RowSelected["OpeningBalanceAmt"] = StmtOpeningBalanceAmt;
                            RowSelected["ClosingBalanceAmt"] = StmtClosingBalanceAmt;
                            RowSelected["ClosingBalanceDate"] = StmtClosingBalanceDate;
                            // ADD ROW
                            TableStatementsExternal.Rows.Add(RowSelected);

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
