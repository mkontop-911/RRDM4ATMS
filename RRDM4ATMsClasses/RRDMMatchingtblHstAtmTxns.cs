using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
//using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMMatchingtblHstAtmTxns : Logger
    {
        public RRDMMatchingtblHstAtmTxns() : base() { }

        //public string AtmNo;
        //public int TraceNumber;

        //public string TranDesc;
        //public string TxtLine;
        //public DateTime TranDate;
        //public bool TranTime;

        public DataTable HstAtmTxnsDataTable = new DataTable();

        public string SqlString; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ Group 

        public void ReadtblHstAtmTxns(string InSelectionCriteria, DateTime InDateFrom, DateTime InDateTo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Define the data table 

            HstAtmTxnsDataTable = new DataTable();
            HstAtmTxnsDataTable.Clear();

            SqlString = "SELECT atmno As AtmNo, " 
           + " cast(trace as int) as TraceNo ,"
           + " ISNULL(trandesc, '') AS trandesc,"
           + " ISNULL(TxtLine, '') AS TxtLine,"
           + " CAmount,ISNULL(cardnum, '') AS cardnum,"
           + " ISNULL(TRanDate, '') AS TRanDate,"
           + " ISNULL(trantime, '') AS trantime"
               + " FROM [ATM_MT_Journals].[dbo].[tblHstAtmTxns] "
               + InSelectionCriteria
               + " AND  (TRanDate >= @TRanDateFrom AND TRanDate <= @TRanDateTo)  "
               + " ORDER by TraceNumber";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TRanDateFrom", InDateFrom);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@TRanDateTo", InDateTo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(HstAtmTxnsDataTable);

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

       
    }
}
