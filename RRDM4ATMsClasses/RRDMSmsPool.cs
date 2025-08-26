using System;
//using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMSmsPool : Logger
    {
        public RRDMSmsPool() : base() { }

        public int SmsNo;
        public int MaskRecordId;
        public string Origin;
        public string DestinationMobile;
        public string SmsText;
        public DateTime DateTimeCreated;
        public DateTime DateTimeToBeSent;
        public DateTime DateTimeSent;

        public bool OutStanding;
        public string Operator;

        //// Define the data table 
        //public DataTable DataTableMasks = new DataTable();
        //public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;
        //
        // READ sms by Sms No 
        //
        public void ReadSmsPoolBySmsNo(string InOperator, int InSmsNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[SMSPool]"
               + " WHERE Operator = @Operator AND SmsNo = @SmsNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SmsNo", InSmsNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SmsNo = (int)rdr["SmsNo"];

                            InSmsNo = (int)rdr["MaskRecordId"];

                            Origin = (string)rdr["Origin"];

                            DestinationMobile = (string)rdr["DestinationMobile"];
                            SmsText = (string)rdr["SmsText"];

                            DateTimeCreated = (DateTime)rdr["DateTimeCreated"];
                            DateTimeToBeSent = (DateTime)rdr["DateTimeToBeSent"];
                            DateTimeSent = (DateTime)rdr["DateTimeSent"];

                            OutStanding = (bool)rdr["OutStanding"];

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
        // READ sms by maskrecordid
        //
        public void ReadSmsPoolByMaskRecordId(string InOperator, int InMaskRecordId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[SMSPool]"
               + " WHERE Operator = @Operator AND MaskRecordId = @MaskRecordId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);                  
                        cmd.Parameters.AddWithValue("@MaskRecordId", InMaskRecordId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SmsNo = (int)rdr["SmsNo"];

                            InMaskRecordId = (int)rdr["MaskRecordId"];

                            Origin = (string)rdr["Origin"];

                            DestinationMobile = (string)rdr["DestinationMobile"];
                            SmsText = (string)rdr["SmsText"];

                            DateTimeCreated = (DateTime)rdr["DateTimeCreated"];
                            DateTimeToBeSent = (DateTime)rdr["DateTimeToBeSent"];
                            DateTimeSent = (DateTime)rdr["DateTimeSent"];

                            OutStanding = (bool)rdr["OutStanding"];

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

        // Insert SMS Record 
        //
        public int InsertSMSPoolRecord()
        {
            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[SMSPool] "
                    + "([MaskRecordId], [Origin], [DestinationMobile], "
                    + " [SmsText], "
                    + " [DateTimeCreated], "
                    + " [DateTimeToBeSent], "
                    + " [Operator] )"
                    + " VALUES (@MaskRecordId, @Origin, @DestinationMobile,"
                    + " @SmsText, "
                    + " @DateTimeCreated, "
                    + " @DateTimeToBeSent, "
                    + " @Operator ) ;"
                    + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@MaskRecordId", MaskRecordId);

                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@DestinationMobile", DestinationMobile);

                        cmd.Parameters.AddWithValue("@SmsText", SmsText);

                        cmd.Parameters.AddWithValue("@DateTimeCreated", DateTimeCreated);
                        cmd.Parameters.AddWithValue("@DateTimeToBeSent", DateTimeToBeSent);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        SmsNo = (int)cmd.ExecuteScalar();

                        //  SET[MaskRecordId] = <MaskRecordId, int,>
                        //,[Origin] = <Origin, nvarchar(100),>
                        //,[DestinationMobile] = <DestinationMobile, nvarchar(30),>
                        //,[SmsText] = <SmsText, nvarchar(1000),>
                        //,[DateTimeCreated] = <DateTimeCreated, datetime,>
                        //,[DateTimeToBeSent] = <DateTimeToBeSent, datetime,>
                        //,[DateTimeSent] = <DateTimeSent, datetime,>
                        //,[OutStanding] = <OutStanding, bit,>
                        //,[Operator] = <Operator, nvarchar(8),>

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return SmsNo;
        }

        // UPDATE SMS Record
        // 
        public void UpdateSmsRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[SMSPool] SET "
                              + " DateTimeToBeSent = @DateTimeToBeSent, "
                              + " OutStanding = @OutStanding  "
                              + " WHERE SmsNo = @SmsNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SmsNo", SmsNo);
                        cmd.Parameters.AddWithValue("@DateTimeSent", DateTimeSent);
                        cmd.Parameters.AddWithValue("@OutStanding", OutStanding);

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

                    CatchDetails(ex);
                }
        }

        //
        // DELETE Category
        //
        public void DeleteMaskRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[SMSPool] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

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

                    CatchDetails(ex);
                }

        }
    }
}
