using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMNVDisputesTrans : Logger
    {
        public RRDMNVDisputesTrans() : base() { }

        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal(); 
        public int SeqNo;
        public int DispNo;
        public string InternalAccNo;
        public string VostroBank;
        public string ExternalAccNo;

        public bool IsExternalFile;
        public bool IsInternalFile;
        public int SeqNoInFile;
        public decimal DisputedAmt;
        public decimal DecidedAmount;
        
        public string ActionComment;
        public DateTime ActionDtTm;
        public bool ClosedDispute;

        string SelectionCriteria; 

        public string Operator;

        // Define the data table 
        public DataTable TableDisputeTxnEntries = new DataTable();

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
        // READ NVDisputesTrans
        // FILL UP A TABLE
        //
        public void ReadNVDisputesTransFillTable(int InDispNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableDisputeTxnEntries = new DataTable();
            TableDisputeTxnEntries.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TableDisputeTxnEntries.Columns.Add("SeqNo", typeof(int));
           
            TableDisputeTxnEntries.Columns.Add("Origin", typeof(string));
            TableDisputeTxnEntries.Columns.Add("AccNo", typeof(string));
            TableDisputeTxnEntries.Columns.Add("Code", typeof(string));

            TableDisputeTxnEntries.Columns.Add("ValueDate", typeof(DateTime));
            TableDisputeTxnEntries.Columns.Add("EntryDate", typeof(DateTime));

            TableDisputeTxnEntries.Columns.Add("DR/CR", typeof(string));
            TableDisputeTxnEntries.Columns.Add("Amt", typeof(decimal));

            TableDisputeTxnEntries.Columns.Add("OurRef", typeof(string));
            TableDisputeTxnEntries.Columns.Add("TheirRef", typeof(string));
            TableDisputeTxnEntries.Columns.Add("OtherDetails", typeof(string));

        
            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[NVDisputesTransTable] "
                    + " WHERE DispNo = @DispNo "
                    + " ORDER BY SeqNo ASC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@DispNo", InDispNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            DispNo = (int)rdr["DispNo"];
                            InternalAccNo = (string)rdr["InternalAccNo"];
                            VostroBank = (string)rdr["VostroBank"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];

                            IsExternalFile = (bool)rdr["IsExternalFile"];
                            IsInternalFile = (bool)rdr["IsInternalFile"];
                            SeqNoInFile = (int)rdr["SeqNoInFile"];
                            DisputedAmt = (decimal)rdr["DisputedAmt"];
                            DecidedAmount = (decimal)rdr["DecidedAmount"];
                            
                            ActionComment = (string)rdr["ActionComment"];
                            ActionDtTm = (DateTime)rdr["ActionDtTm"];
                            ClosedDispute = (bool)rdr["ClosedDispute"];

                            Operator = (string)rdr["Operator"];

                            if (IsExternalFile == true)
                            {
                                SelectionCriteria = " WHERE StmtAccountID ='" + ExternalAccNo + "' AND SeqNo =" + SeqNoInFile;
                                Se.ReadNVStatement_Lines_ExternalBySelectionCriteria(SelectionCriteria);
                            }

                            if (IsInternalFile == true)
                            {
                                SelectionCriteria = " WHERE StmtAccountID ='" + InternalAccNo + "' AND SeqNo =" + SeqNoInFile;
                                Se.ReadNVStatement_Lines_InternalBySelectionCriteria(SelectionCriteria);
                            }

                            DataRow RowSelected = TableDisputeTxnEntries.NewRow();

                            RowSelected["SeqNo"] = Se.SeqNo;

                            RowSelected["Origin"] = Se.Origin;

                            RowSelected["AccNo"] = Se.StmtAccountID;
                            
                            RowSelected["Code"] = Se.StmtLineTrxCode;
                            RowSelected["ValueDate"] = Se.StmtLineValueDate;
                            RowSelected["EntryDate"] = Se.StmtLineEntryDate;

                            if (Se.StmtLineIsDebit == true) RowSelected["DR/CR"] = "DR";
                            else RowSelected["DR/CR"] = "CR";

                            RowSelected["Amt"] = Se.StmtLineAmt;

                            RowSelected["OurRef"] = "  " + Se.StmtLineRefForAccountOwner;

                            RowSelected["TheirRef"] = Se.StmtLineRefForServicingBank;
                            RowSelected["OtherDetails"] = Se.StmtLineSuplementaryDetails;

                            // ADD ROW
                            TableDisputeTxnEntries.Rows.Add(RowSelected);

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
        // READ NVDisputesTrans SeqNo
        // 
        //
        public void ReadNVNVDisputesTransBySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVDisputesTransTable] "
                      + " WHERE Operator = @Operator AND SeqNo = @SeqNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            DispNo = (int)rdr["DispNo"];
                            InternalAccNo = (string)rdr["InternalAccNo"];
                            VostroBank = (string)rdr["VostroBank"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];

                            IsExternalFile = (bool)rdr["IsExternalFile"];
                            IsInternalFile = (bool)rdr["IsInternalFile"];
                            SeqNoInFile = (int)rdr["SeqNoInFile"];
                            DisputedAmt = (decimal)rdr["DisputedAmt"];
                            DecidedAmount = (decimal)rdr["DecidedAmount"];

                            ActionComment = (string)rdr["ActionComment"];
                            ActionDtTm = (DateTime)rdr["ActionDtTm"];
                            ClosedDispute = (bool)rdr["ClosedDispute"];

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
        // READ NVDisputesTrans Selection Criteria 
        // 
        //
        public void ReadNVDisputesTransBySelection(string InOperator, string InSelection)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVDisputesTransTable] "
                      + InSelection ;

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
                            DispNo = (int)rdr["DispNo"];
                            InternalAccNo = (string)rdr["InternalAccNo"];
                            VostroBank = (string)rdr["VostroBank"];
                            ExternalAccNo = (string)rdr["ExternalAccNo"];

                            IsExternalFile = (bool)rdr["IsExternalFile"];
                            IsInternalFile = (bool)rdr["IsInternalFile"];
                            SeqNoInFile = (int)rdr["SeqNoInFile"];
                            DisputedAmt = (decimal)rdr["DisputedAmt"];
                            DecidedAmount = (decimal)rdr["DecidedAmount"];

                            ActionComment = (string)rdr["ActionComment"];
                            ActionDtTm = (DateTime)rdr["ActionDtTm"];
                            ClosedDispute = (bool)rdr["ClosedDispute"];

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
        // Insert NVDisputesTrans
        //
        public int InsertNVDisputesTran()
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[NVDisputesTransTable] "
               + " ([DispNo] "
           + " ,[InternalAccNo] "
           + " ,[VostroBank] "
           + " ,[ExternalAccNo] "
           + " ,[IsExternalFile] "
           + " ,[IsInternalFile] "
           + " ,[SeqNoInFile] "
           + " ,[DisputedAmt] "
           + " ,[Operator]) "
   + " VALUES "
          + " (@DispNo "
          + "  ,@InternalAccNo "
          + "  ,@VostroBank "
          + "  ,@ExternalAccNo "
          + "  ,@IsExternalFile "
          + "  ,@IsInternalFile "
          + "  ,@SeqNoInFile "
          + "  ,@DisputedAmt "
          + "  ,@Operator)  "
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                     
                        cmd.Parameters.AddWithValue("@DispNo", DispNo);
                        cmd.Parameters.AddWithValue("@InternalAccNo", InternalAccNo);
                        cmd.Parameters.AddWithValue("@VostroBank", VostroBank);
                        cmd.Parameters.AddWithValue("@ExternalAccNo", ExternalAccNo);

                        cmd.Parameters.AddWithValue("@IsExternalFile", IsExternalFile);
                        cmd.Parameters.AddWithValue("@IsInternalFile", IsInternalFile);
                        cmd.Parameters.AddWithValue("@SeqNoInFile", SeqNoInFile);
                        cmd.Parameters.AddWithValue("@DisputedAmt", DisputedAmt);
                     
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated
                        SeqNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    ErrorFound = true;

                    CatchDetails(ex);
                }

            return SeqNo;
        }
        // 
        // UPDATE NVDisputesTrans
        // 
        public void UpdateNVDisputesTransRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[NVDisputesTransTable] SET "
                            + " DecidedAmount = @DecidedAmount,"
                             + " ActionComment = @ActionComment,"
                              + " ActionDtTm = @ActionDtTm,"
                            + " ClosedDispute = @ClosedDispute "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                       
                        cmd.Parameters.AddWithValue("@DecidedAmount", DecidedAmount);

                        cmd.Parameters.AddWithValue("@ActionComment", ActionComment);
                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);
                        cmd.Parameters.AddWithValue("@ClosedDispute", ClosedDispute);

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
        // DELETE NVDisputesTrans
        //
        public void DeleteNVDisputesTranEntry(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[NVDisputesTransTable] "
                            + " WHERE SeqNo = @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

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


    }
}
