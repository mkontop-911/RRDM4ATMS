using System;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;
using System.Data;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMNVMT995AndMT996Pool : Logger
    {
        public RRDMNVMT995AndMT996Pool() : base() { }

        public int SeqNo;
        public int DispNo;
        public string Sender;
        public string MessageType;
        public string Receiver;
        public string TrxReferenceNumber;
        public string RelatedReference;

        public string QueryLine1;
        public string QueryLine2;
        public string Narrative;
        public string OriginalMessageMT;

        public string OriginalMessageDate;
        //

        public string Operator;

        // Define the data table 
        public DataTable TableNVMT995Pool = new DataTable();

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
        // READ NVMT995AND996
        // FILL UP A TABLE
        //
        public void ReadNVMT995PoolFillTable(string InOperator, string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableNVMT995Pool = new DataTable();
            TableNVMT995Pool.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableNVMT995Pool.Columns.Add("SeqNo", typeof(int));
            TableNVMT995Pool.Columns.Add("MT_ID", typeof(string));
            TableNVMT995Pool.Columns.Add("Receiver", typeof(string));
            TableNVMT995Pool.Columns.Add("TrxReference", typeof(string));
            TableNVMT995Pool.Columns.Add("RelatedReference", typeof(string));
            TableNVMT995Pool.Columns.Add("QueryLine1", typeof(string));
            TableNVMT995Pool.Columns.Add("Narrative", typeof(string));
            TableNVMT995Pool.Columns.Add("OriginalMessageMT", typeof(string));

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[NVMT995AndMT996Pool] "
                    + InSelectionCriteria
                    + " Order By SeqNo "
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
                        //cmd.Parameters.AddWithValue("@MessageType", InMessageType);
                        //cmd.Parameters.AddWithValue("@Receiver", InReceiver);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];
                            DispNo = (int)rdr["DispNo"];
                            Sender = (string)rdr["Sender"];
                            MessageType = (string)rdr["MessageType"];
                            Receiver = (string)rdr["Receiver"];

                            TrxReferenceNumber = (string)rdr["TrxReferenceNumber"];
                            RelatedReference = (string)rdr["RelatedReference"];

                            QueryLine1 = (string)rdr["QueryLine1"];
                            QueryLine2 = (string)rdr["QueryLine2"];
                            Narrative = (string)rdr["Narrative"];
                            OriginalMessageMT = (string)rdr["OriginalMessageMT"];
                            OriginalMessageDate = (string)rdr["OriginalMessageDate"];

                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableNVMT995Pool.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["MT_ID"] = MessageType;
                            RowSelected["Receiver"] = Receiver;
                            RowSelected["TrxReference"] = TrxReferenceNumber;
                            RowSelected["RelatedReference"] = RelatedReference;
                            RowSelected["QueryLine1"] = QueryLine1;
                            RowSelected["Narrative"] = Narrative;
                            RowSelected["OriginalMessageMT"] = OriginalMessageMT;

                            // ADD ROW
                            TableNVMT995Pool.Rows.Add(RowSelected);

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
        // READ NVMT995Pool by SeqNo
        // 
        //
        public void ReadNVMT995PoolBySeqNo(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[NVMT995AndMT996Pool] "
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
                            Sender = (string)rdr["Sender"];
                            MessageType = (string)rdr["MessageType"];
                            Receiver = (string)rdr["Receiver"];

                            TrxReferenceNumber = (string)rdr["TrxReferenceNumber"];
                            RelatedReference = (string)rdr["RelatedReference"];

                            QueryLine1 = (string)rdr["QueryLine1"];
                            QueryLine2 = (string)rdr["QueryLine2"];
                            Narrative = (string)rdr["Narrative"];
                            OriginalMessageMT = (string)rdr["OriginalMessageMT"];
                            OriginalMessageDate = (string)rdr["OriginalMessageDate"];

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
        // Insert NVMT995Pool
        //
        public int InsertRecordInNVMT995Pool()
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[NVMT995AndMT996Pool] "
                  + " ([DispNo],"
                + " [Sender],"
                + " [MessageType],"
                + " [Receiver],"
                + " [TrxReferenceNumber],"
                + " [RelatedReference],"
                + " [QueryLine1],"
                + " [QueryLine2],"
                + " [Narrative],"
                + " [OriginalMessageMT],"
                + " [OriginalMessageDate],"
                + " [Operator] )"
                + " VALUES"
                + " (@DispNo,"
                + " @Sender,"
                + " @MessageType,"
                + " @Receiver,"
                + " @TrxReferenceNumber,"
                + " @RelatedReference,"
                + " @QueryLine1,"
                + " @QueryLine2,"
                + " @Narrative,"
                + " @OriginalMessageMT,"
                + " @OriginalMessageDate,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@DispNo", DispNo);
                        cmd.Parameters.AddWithValue("@Sender", Sender);
                        cmd.Parameters.AddWithValue("@MessageType", MessageType);
                        cmd.Parameters.AddWithValue("@Receiver", Receiver);
                        cmd.Parameters.AddWithValue("@TrxReferenceNumber", TrxReferenceNumber);
                        cmd.Parameters.AddWithValue("@RelatedReference", RelatedReference);
                        cmd.Parameters.AddWithValue("@QueryLine1", QueryLine1);
                        cmd.Parameters.AddWithValue("@QueryLine2", QueryLine2);
                        cmd.Parameters.AddWithValue("@Narrative", Narrative);
                        cmd.Parameters.AddWithValue("@OriginalMessageMT", OriginalMessageMT);
                        cmd.Parameters.AddWithValue("@OriginalMessageDate", OriginalMessageDate);
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

                    CatchDetails(ex);
                }

            return SeqNo;
        }

        // 
        // UPDATE NVMT995Pool Record 
        // 
        public void UpdateNVMT995PoolRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[NVMT995AndMT996Pool] SET "
                            + " QueryLine1 = @QueryLine1,"
                            + " QueryLine2 = @QueryLine2,"
                            + " Narrative = @Narrative ,"
                            + " OriginalMessageMT = @OriginalMessageMT "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@QueryLine1", QueryLine1);
                        cmd.Parameters.AddWithValue("@QueryLine2", QueryLine2);
                        cmd.Parameters.AddWithValue("@Narrative", Narrative);
                        cmd.Parameters.AddWithValue("@OriginalMessageMT", OriginalMessageMT);

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
        // DELETE NVMT995Pool
        //
        public void DeleteNVMT995PoolEntry(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[NVMT995AndMT996Pool] "
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
