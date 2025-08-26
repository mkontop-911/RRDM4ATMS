using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMReplOrders_Pre_ATMs : Logger
    {
        public RRDMReplOrders_Pre_ATMs() : base() { }

        public string AtmNo;
        public string AtmName;
        public int ReplCycleNo;

        public string NeedStatus;
        public int OrderCycle;
        public decimal CurrentCassetes;

        public int DaysToLast;
        public decimal Deposits;
        public int CaptureCards;

        public int Errors;
        public DateTime NextRepl;
        public decimal ToLoad;

        public string UserId;

        // Define the data table 
        public DataTable TableReplOrders_Pre_ATMs = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // Read Table Fields
        private void ReadTableFields(SqlDataReader rdr)
        {

            AtmNo = (string)rdr["AtmNo"];
            AtmName = (string)rdr["AtmName"];
            ReplCycleNo = (int)rdr["ReplCycleNo"];

            NeedStatus = (string)rdr["NeedStatus"];
            OrderCycle = (int)rdr["OrderCycle"];
            CurrentCassetes = (decimal)rdr["CurrentCassetes"];

            DaysToLast = (int)rdr["DaysToLast"];
            Deposits = (decimal)rdr["Deposits"];
            CaptureCards = (int)rdr["CaptureCards"];

            Errors = (int)rdr["Errors"];
            NextRepl = (DateTime)rdr["NextRepl"];
            ToLoad = (decimal)rdr["ToLoad"];

            UserId = (string)rdr["UserId"];

        }

        //
        // Methods 
        // READ ReplOrders_Pre_ATMs
        // FILL UP A TABLE
        //
        public void ReadReplOrders_Pre_ATMsFillTable(int InOrderCycle)
        {

            TableReplOrders_Pre_ATMs = new DataTable();
            TableReplOrders_Pre_ATMs.Clear();

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ReplOrders_Pre_ATMs] "
                    + " WHERE OrderCycle = @OrderCycle  "
                    + " ORDER BY AtmNo ASC ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@OrderCycle", InOrderCycle);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableReplOrders_Pre_ATMs);

                        // Close conn
                        conn.Close();

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }

        //
        // Insert report datatable into the data Base Table
        // 
        public void Insert_In_ReplOrders_Pre_ATMs(string InOperator, string InSignedId, DataTable InTable)
        {

            if (InTable.Rows.Count > 0)
            {
                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[ReplOrders_Pre_ATMs]";

                            foreach (var column in InTable.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(InTable);
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }
            }

        }

        //
        // DELETE records from ReplOrders_Pre_ATMs when needed
        //
        public void DeleteReplOrders_Pre_ATMs(int InOrderCycle)
        {

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ReplOrders_Pre_ATMs] "
                             + " Where OrderCycle =@OrderCycle ", conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderCycle", InOrderCycle);

                        //rows number of record got updated

                        cmd.ExecuteNonQuery();

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
        // READ by Selection criteria 
        // 
        public void ReadReplOrders_Pre_ATMsBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReplOrders_Pre_ATMs] "
                      + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);
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
        // READ by ATM No
        // 
        public void ReadReplOrders_Pre_ATMsByAtmNo(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ReplOrders_Pre_ATMs] "
                      + " WHERE AtmNo = @AtmNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);
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
