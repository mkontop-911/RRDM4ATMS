using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMITMXLoadingFilesRegister : Logger
    {
        public RRDMITMXLoadingFilesRegister() : base() { }
        public int SeqNo;
        public int ITMXJobCycle;
        public string BankId;
        public string FileID;
        public DateTime ExpectedDate;
        public int ExpectedFirstKey;
        public int ExpectedLastKey;

        public DateTime ReceivedDate;
        public int ReceivedFirstKey;
        public int ReceivedLastKey;
        public int LoadedKeyId; 

        public string ReceivedCode;
        //00 Received as expected
        //01 Received with minor difference
        //02 Received with major difference - rejected 
        //99 Not received yet
        //04 Arrived in different Cycle
        //05 
        public string Description;
        public string Operator;

        public string Status;

        // Define the data table 
        public DataTable TableFilesLoadingRegister = new DataTable();

        public DataTable TableFilesLoadingRegisterHistory = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public int TotalReceived;
        public int TotalNotReceived;
        public int TotalRejected;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        //
        // Methods 
        // READ Files loaded and not loaded 
        // FILL UP A TABLE
        //
        public void ReadLoadingFilesRegisterFillTable(string InOperator, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableFilesLoadingRegister = new DataTable();
            TableFilesLoadingRegister.Clear();

            TotalReceived = 0;
            TotalNotReceived = 0;
            TotalRejected = 0;

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableFilesLoadingRegister.Columns.Add("ITMXJobCycle", typeof(int));
            TableFilesLoadingRegister.Columns.Add("BankId", typeof(string));
            TableFilesLoadingRegister.Columns.Add("FileID", typeof(string));
            TableFilesLoadingRegister.Columns.Add("ExpectedDate", typeof(DateTime));
            TableFilesLoadingRegister.Columns.Add("ReceivedDate", typeof(string));
            TableFilesLoadingRegister.Columns.Add("Status", typeof(string));
            

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ITMXLoadingFilesRegister] "
                    + InSelectionCriteria;

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
                            ITMXJobCycle = (int)rdr["ITMXJobCycle"];
                            BankId = (string)rdr["BankId"];
                            FileID = (string)rdr["FileID"];

                            ExpectedDate = (DateTime)rdr["ExpectedDate"];
                            ExpectedFirstKey = (int)rdr["ExpectedFirstKey"];
                            ExpectedLastKey = (int)rdr["ExpectedLastKey"];

                            ReceivedDate = (DateTime)rdr["ReceivedDate"];
                            ReceivedFirstKey = (int)rdr["ReceivedFirstKey"];
                            ReceivedLastKey = (int)rdr["ReceivedLastKey"];

                            LoadedKeyId = (int)rdr["LoadedKeyId"];

                            ReceivedCode = (string)rdr["ReceivedCode"];

                            Description = (string)rdr["Description"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableFilesLoadingRegister.NewRow();

                            RowSelected["ITMXJobCycle"] = ITMXJobCycle;
                            RowSelected["BankId"] = BankId;
                            RowSelected["FileID"] = FileID;
                            RowSelected["ExpectedDate"] = ExpectedDate;

                            if (ReceivedCode == "00")
                            {
                                TotalReceived = TotalReceived + 1;
                                Status = "File Received";
                                RowSelected["ReceivedDate"] = ReceivedDate.ToString();
                            }
                            else
                            {
                                if (ReceivedCode == "99")
                                {
                                    TotalNotReceived = TotalNotReceived + 1;
                                    Status = "File Not Received";
                                    RowSelected["ReceivedDate"] = "Not Applicable";
                                }
                                else
                                {
                                    TotalRejected = TotalRejected + 1;
                                    Status = "File Sent Back";
                                    RowSelected["ReceivedDate"] = "Not Applicable";
                                }
                            }

                            RowSelected["Status"] = Status;

                            // ADD ROW
                            TableFilesLoadingRegister.Rows.Add(RowSelected);

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
        // READ and Fill History table 
        // FILL UP A TABLE
        //
        public void ReadLoadingFilesRegisterHistoryFillTable(string InOperator, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableFilesLoadingRegisterHistory = new DataTable();
            TableFilesLoadingRegisterHistory.Clear();

            TotalReceived = 0;
            TotalNotReceived = 0;
            TotalRejected = 0;

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableFilesLoadingRegisterHistory.Columns.Add("FileID", typeof(string));
            TableFilesLoadingRegisterHistory.Columns.Add("ITMXJobCycle", typeof(int));
            TableFilesLoadingRegisterHistory.Columns.Add("ExpectedDate", typeof(DateTime));
            TableFilesLoadingRegisterHistory.Columns.Add("ReceivedDate", typeof(string));
            TableFilesLoadingRegisterHistory.Columns.Add("Status", typeof(string));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ITMXLoadingFilesRegister] "
                    + InSelectionCriteria
                    + " ORDER By ITMXJobCycle DESC "; 

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
                            ITMXJobCycle = (int)rdr["ITMXJobCycle"];
                            BankId = (string)rdr["BankId"];
                            FileID = (string)rdr["FileID"];

                            ExpectedDate = (DateTime)rdr["ExpectedDate"];
                            ExpectedFirstKey = (int)rdr["ExpectedFirstKey"];
                            ExpectedLastKey = (int)rdr["ExpectedLastKey"];

                            ReceivedDate = (DateTime)rdr["ReceivedDate"];
                            ReceivedFirstKey = (int)rdr["ReceivedFirstKey"];
                            ReceivedLastKey = (int)rdr["ReceivedLastKey"];

                            LoadedKeyId = (int)rdr["LoadedKeyId"];

                            ReceivedCode = (string)rdr["ReceivedCode"];

                            Description = (string)rdr["Description"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableFilesLoadingRegisterHistory.NewRow();

                            RowSelected["ITMXJobCycle"] = ITMXJobCycle;
                            RowSelected["FileID"] = FileID;
                            RowSelected["ExpectedDate"] = ExpectedDate;

                            if (ReceivedCode == "00")
                            {
                                TotalReceived = TotalReceived + 1;
                                Status = "File Received";
                                RowSelected["ReceivedDate"] = ReceivedDate.ToString();
                            }
                            else
                            {
                                if (ReceivedCode == "99")
                                {
                                    TotalNotReceived = TotalNotReceived + 1;
                                    Status = "File Not Received";
                                    RowSelected["ReceivedDate"] = "Not Applicable";
                                }
                                else
                                {
                                    TotalRejected = TotalRejected + 1;
                                    Status = "File Sent Back";
                                    RowSelected["ReceivedDate"] = "Not Applicable";
                                }
                            }

                            RowSelected["Status"] = Status;

                            // ADD ROW
                            TableFilesLoadingRegisterHistory.Rows.Add(RowSelected);

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
