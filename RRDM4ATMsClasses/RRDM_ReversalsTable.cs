using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDM_ReversalsTable : Logger
    {
        public RRDM_ReversalsTable() : base() { }

        public int SeqNo;
        public string FileId;
        public string MatchingCateg;

        public string TerminalId_4;
        public string TerminalId_2;
        public string CardNumber_4;
        public string CardNumber_2;

        public string AccNo_4;
        public string AccNo_2;

        public string TransDescr_4;
        public string TransDescr_2;

        public string TransCurr_4;
        public string TransCurr_2;

        public decimal TransAmt_4;
        public decimal TransAmt_2;
        public int TraceNo_4;
        public int TraceNo_2;

        public string RRNumber_4;
        public string RRNumber_2;
        public string FullTraceno_4;
        public string FullTraceno_2;

        public int SeqNo_4;
        public int SeqNo_2;
        public string ResponseCode_4;
        public string ResponseCode_2;
        
        public DateTime TransDate_4;
        public DateTime TransDate_2;
        public string TXNSRC_4;
        public string TXNSRC_2;

        public string TXNDEST_4;
        public string TXNDEST_2;
        public int RMCycleNo;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        //************************************************
        //

        public DataTable ReversalsDataTable = new DataTable();

        public int TotalSelected;

        readonly string ATMSconnectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;
        readonly string recconConnString = ConfigurationManager.ConnectionStrings["ReconConnectionString"].ConnectionString;

        // Read Fields In Table 
        private void ReadFieldsInTable(SqlDataReader rdr)
        {
 
            SeqNo = (int)rdr["SeqNo"];

            FileId = (string)rdr["FileId"];
            MatchingCateg = (string)rdr["MatchingCateg"];

            TerminalId_4 = (string)rdr["TerminalId_4"];
            TerminalId_2 = (string)rdr["TerminalId_2"];
            CardNumber_4 = (string)rdr["CardNumber_4"];
            CardNumber_2 = (string)rdr["CardNumber_2"];

            AccNo_4 = (string)rdr["AccNo_4"];
            AccNo_2 = (string)rdr["AccNo_2"];
            TransDescr_4 = (string)rdr["TransDescr_4"];
            TransDescr_2 = (string)rdr["TransDescr_2"];
            TransCurr_4 = (string)rdr["TransCurr_4"];
            TransCurr_2 = (string)rdr["TransCurr_2"];

            TransAmt_4 = (decimal)rdr["TransAmt_4"];
            TransAmt_2 = (decimal)rdr["TransAmt_2"];
            TraceNo_4 = (int)rdr["TraceNo_4"];
            TraceNo_2 = (int)rdr["TraceNo_2"];

            RRNumber_4 = (string)rdr["RRNumber_4"];
            RRNumber_2 = (string)rdr["RRNumber_2"];
            FullTraceno_4 = (string)rdr["FullTraceno_4"];
            FullTraceno_2 = (string)rdr["FullTraceno_2"];

            SeqNo_4 = (int)rdr["SeqNo_4"];
            SeqNo_2 = (int)rdr["SeqNo_2"];
            ResponseCode_4 = (string)rdr["ResponseCode_4"];
            ResponseCode_2 = (string)rdr["ResponseCode_2"];

            TransDate_4 = (DateTime)rdr["TransDate_4"];
            TransDate_2 = (DateTime)rdr["TransDate_2"];
            TXNSRC_4 = (string)rdr["TXNSRC_4"];
            TXNSRC_2 = (string)rdr["TXNSRC_2"];

            TXNDEST_4 = (string)rdr["TXNDEST_4"];
            TXNDEST_2 = (string)rdr["TXNDEST_2"];
            RMCycleNo = (int)rdr["RMCycleNo"];
            
        }

        //
        // READ Reversals and created table as per request.
        //
        public void ReadReversalsAndFillTable(string InUserId, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            // CLEAR TABLE
            ClearDefineTable();

            string SqlString = "SELECT *"
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[REVERSALs_PAIRs] "
                 + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(recconConnString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {
                        //  cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                            AddLineToTable(InUserId);

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();


                    InsertReport(InUserId, ReversalsDataTable); 
                   // InsertRecordsOfReport(InUserId);

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        // Clear Define Table
        private void ClearDefineTable()
        {
            ReversalsDataTable = new DataTable();
            ReversalsDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ReversalsDataTable.Columns.Add("SeqNo", typeof(int));
            ReversalsDataTable.Columns.Add("RevSeqNo1", typeof(int));
            ReversalsDataTable.Columns.Add("Terminal", typeof(string));
            ReversalsDataTable.Columns.Add("Descr", typeof(string));
            ReversalsDataTable.Columns.Add("Card", typeof(string));
            ReversalsDataTable.Columns.Add("Account", typeof(string));
            ReversalsDataTable.Columns.Add("Ccy", typeof(string));
            ReversalsDataTable.Columns.Add("Amount", typeof(decimal));
            ReversalsDataTable.Columns.Add("Date", typeof(DateTime));
            ReversalsDataTable.Columns.Add("TraceNo", typeof(int));
            ReversalsDataTable.Columns.Add("RRNumber", typeof(string));
            ReversalsDataTable.Columns.Add("RespCode", typeof(string));
            ReversalsDataTable.Columns.Add("FileId", typeof(string));
            ReversalsDataTable.Columns.Add("Category", typeof(string));
            ReversalsDataTable.Columns.Add("RevSeqNo2", typeof(int));
            ReversalsDataTable.Columns.Add("RevResp", typeof(string));
            ReversalsDataTable.Columns.Add("UserId", typeof(string));
        }

     
        // Insert 
        public void InsertReport(string InUserId, DataTable InTable)
        {
            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            //Clear Table 
            Tr.DeleteReport55_3(InUserId);

            if (InTable.Rows.Count > 0)
            {
                // RECORDS READ AND PROCESSED 
                
                using (SqlConnection conn =
                               new SqlConnection(recconConnString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport55_3]";

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

        // ADD LINE TO TABLE
        //RRDMAccountsClass Acc = new RRDMAccountsClass();
        private void AddLineToTable(string InUserId)
        {
            //
            // Fill In Table
            //

            DataRow RowSelected = ReversalsDataTable.NewRow();

            RowSelected["SeqNo"] = SeqNo;
            RowSelected["RevSeqNo1"] = SeqNo_2;
            RowSelected["Terminal"] = TerminalId_2;
            RowSelected["Descr"] = TransDescr_2;
            RowSelected["Card"] = CardNumber_2;
            RowSelected["Account"] = AccNo_2;
           
            RowSelected["Ccy"] = TransCurr_2;
            RowSelected["Amount"] = TransAmt_2;
            RowSelected["Date"] = TransDate_2;
            RowSelected["TraceNo"] = TraceNo_2;
            
            RowSelected["RRNumber"] = RRNumber_2;
            RowSelected["RespCode"] = ResponseCode_2;
            RowSelected["FileId"] = FileId;
            RowSelected["Category"] = MatchingCateg;
         
            RowSelected["RevSeqNo2"] = SeqNo_4;
            RowSelected["RevResp"] = ResponseCode_4;

            RowSelected["UserId"] = InUserId;

            // ADD ROW
            ReversalsDataTable.Rows.Add(RowSelected);
        }


        public int Count_11;
        public int Count_23;
        //
        // READ SPECIFIC TRANSACTION based on Number 
        //
        //  string WorkingTableName;
        public void ReadReversalsBy_Selection_criteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                  + " FROM [RRDM_Reconciliation_ITMX].[dbo].[REVERSALs_PAIRs]" 
                  + InSelectionCriteria
                  + " ";

            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //    cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

                           
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
        // Get both tables 
        public void ReadTransSpecificFromBothTables_By_SelectionCriteria(string InSelectionCriteria, string InTableName)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            Count_11 = 0;
            Count_23 = 0;

            SqlString =
           " WITH MergedTbl AS "
           + " ( "
           + " SELECT *  "
           + " FROM [RRDM_Reconciliation_ITMX].[dbo]." + InTableName
           + InSelectionCriteria // Includes Where
           + " UNION ALL  "
           + " SELECT * "
           + " FROM[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + InTableName
           + InSelectionCriteria // Includes Where
           + " ) "
           + " SELECT * FROM MergedTbl "
           + " ORDER BY SeqNo DESC "
            + "  ";


            using (SqlConnection conn =
                          new SqlConnection(ATMSconnectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //    cmd.Parameters.AddWithValue("@TransAmt", InTransAmt);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadFieldsInTable(rdr);

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
