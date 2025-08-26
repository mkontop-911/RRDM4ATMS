using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMMatchingDiscrepancies : Logger
    {
        public RRDMMatchingDiscrepancies() : base() { }

        public int SeqNo;
        public string UserId;
        public int OriginSeqNo;
        public DateTime TransDate; 
        public string FileId;
        public string WCase;
        public int Type;
       
        public int DublInPos;
        public int InPos;
        public int NotInPos;
        public string TerminalId;
        public int TraceNo;
       
        public string RRNumber;
        public string CardNumber;
        public string AccNo;
        public decimal TransAmt;
        public string MatchingCateg;
       
        public int RMCycle;
        public string Matched_Characters;
        public DateTime FullDtTm;
        public string ResponseCode;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string SqlString; 

        // Define the data table 
        public DataTable TableMatchingDiscrepancies = new DataTable();

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;    
     
        int rows;

        //
        // Methods 
        // READ And 
        // FILL UP A TABLE
        //
        public void ReadMatchingDiscrepanciesFillTable(string InTerminalId, DateTime InTransDate, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMatchingDiscrepancies = new DataTable();
            TableMatchingDiscrepancies.Clear();

            // DATA TABLE ROWS DEFINITION 

            SqlString = "SELECT *"
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MatchingDiscrepancies_BDC] "
                    + " WHERE TerminalId = @TerminalId AND TransDate = @TransDate AND TraceNo = @TraceNo";

            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TransDate", InTransDate);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TraceNo", InTraceNo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableMatchingDiscrepancies);

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

        // Methods 
        // READ And 
        // FILL UP A TABLE
        //
        public void ReadMatchingDiscrepanciesFillTableByATM_RMCycle(string InMatchingCateg, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMatchingDiscrepancies = new DataTable();
            TableMatchingDiscrepancies.Clear();

            // DATA TABLE ROWS DEFINITION 

            SqlString = "SELECT *"
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[MatchingDiscrepancies_BDC] "
                    + " WHERE  MatchingCateg = @MatchingCateg AND RMCycle = @RMCycle  ";

            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@RMCycle", InRMCycle);
             

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableMatchingDiscrepancies);

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


        public void Read_AND_SHOW_IST_OUTSTANDING_RECORDS()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableMatchingDiscrepancies = new DataTable();
            TableMatchingDiscrepancies.Clear();

            // 
  //          select MatchingCateg, Net_TransDate, Count(*) as NumberOfRecords
  //FROM[RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns]
  //Where Processed = 0
  //group by MatchingCateg, Net_TransDate

            SqlString = " select MatchingCateg, Net_TransDate, Count(*) as NumberOfRecords "
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Switch_IST_Txns] "
                    + " WHERE Processed = 0 "
                    + " GROUP by MatchingCateg, Net_TransDate "
                    + " ORDER BY MatchingCateg, Net_TransDate "
                    ;

            using (SqlConnection conn =
              new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@RMCycle", InRMCycle);


                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableMatchingDiscrepancies);

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
