using System;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace RRDM4ATMs
{
    public class RRDMControllerMsgClass : Logger
    {
        public RRDMControllerMsgClass() : base() { }

        public int MesNo; 
        public bool ToAllAtms; 
        public string AtmNo; 
        public string FromUser; 
        public string ToUser; 
        public string BankId; 
  
        public string BranchId; 
        public DateTime DtTm; 
        public string Type; 
        public string Message;

        public DateTime ExpDate; 
        public bool SeriousMsg;
        public bool OpenMsg;
        public bool ReadMsg;
        public string Operator;

        public int SerMsgCount;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data table 
        public DataTable TableControllerMsgs = new DataTable();

        public int TotalSelected;

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;
        //
        // READ Controller Messages by Seq No 
        //
        public void ReadControlerMSGsSeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [dbo].[ControlerMSGs] "
          + " WHERE MesNo = @MesNo " ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@MesNo", InSeqNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                           
                            MesNo = (int)rdr["MesNo"];
                            ToAllAtms = (bool)rdr["ToAllAtms"];
                            AtmNo = (string)rdr["AtmNo"];
                            FromUser = (string)rdr["FromUser"];
                            ToUser = (string)rdr["ToUser"];
                            BankId = (string)rdr["BankId"];

                            BranchId = (string)rdr["BranchId"];
                            DtTm = (DateTime)rdr["DtTm"];
                            Type = (string)rdr["Type"];
                            Message = (string)rdr["Message"];

                            ExpDate = (DateTime)rdr["ExpDate"];
                            SeriousMsg = (bool)rdr["SeriousMsg"];
                            OpenMsg = (bool)rdr["OpenMsg"];
                            ReadMsg = (bool)rdr["ReadMsg"];
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
        // READ Controller Messages and fill the table 
        //
        public void ReadControlerMSGsToFillTable(string InuserId)
        {
           
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableControllerMsgs = new DataTable();
            TableControllerMsgs.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableControllerMsgs.Columns.Add("MesNo", typeof(int));
            TableControllerMsgs.Columns.Add("FromUser", typeof(string));
            TableControllerMsgs.Columns.Add("ToUser", typeof(string));
            TableControllerMsgs.Columns.Add("DtTm", typeof(DateTime));
            TableControllerMsgs.Columns.Add("Type", typeof(string));
            TableControllerMsgs.Columns.Add("Message", typeof(string));
            TableControllerMsgs.Columns.Add("OpenMsg", typeof(bool));
            TableControllerMsgs.Columns.Add("ReadMsg", typeof(bool));

            string SqlString = "SELECT *"
          + " FROM [dbo].[ControlerMSGs] "
          + " WHERE FromUser = @FromUser " ; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@FromUser", InuserId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            MesNo = (int)rdr["MesNo"];

                            ToAllAtms = (bool)rdr["ToAllAtms"];
                            AtmNo = (string)rdr["AtmNo"];
                            FromUser = (string)rdr["FromUser"];
                            ToUser = (string)rdr["ToUser"];
                            BankId = (string)rdr["BankId"];

                            BranchId = (string)rdr["BranchId"];
                            DtTm = (DateTime)rdr["DtTm"];
                            Type = (string)rdr["Type"];
                            Message = (string)rdr["Message"];

                            ExpDate = (DateTime)rdr["ExpDate"];
                            SeriousMsg = (bool)rdr["SeriousMsg"];
                            OpenMsg = (bool)rdr["OpenMsg"];
                            ReadMsg = (bool)rdr["ReadMsg"];
                            Operator = (string)rdr["Operator"];

                            DataRow RowSelected = TableControllerMsgs.NewRow();

                            RowSelected["MesNo"] = MesNo;
                            RowSelected["FromUser"] = FromUser;
                            RowSelected["ToUser"] = ToUser;
                            RowSelected["DtTm"] = DtTm;
                            RowSelected["Type"] = Type;
                            RowSelected["Message"] = Message;
                            RowSelected["OpenMsg"] = OpenMsg;
                            RowSelected["ReadMsg"] = ReadMsg;

                            // ADD ROW
                            TableControllerMsgs.Rows.Add(RowSelected);

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
        // READ Controller Messages to see if there is a serious one 
        //
        public void ReadControlerMSGsSerious(string InMsgFilter)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SerMsgCount = 0; 
       
            string SqlString = "SELECT *"
          + " FROM [dbo].[ControlerMSGs] "
          + " WHERE " + InMsgFilter;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
               
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SeriousMsg = (bool)rdr["SeriousMsg"];

                            if (SeriousMsg == true)
                            {
                                SerMsgCount = SerMsgCount + 1;
                            }

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
//// Number of messages 
//        public int CountMSGs(string User)
//        {
//            RecordFound = false;
//            ErrorFound = false;
//            ErrorOutput = ""; 

//            int count = 0;
//            string SqlString = "SELECT COUNT(*)"
//          + " FROM [dbo].[ControlerMSGs] "
//          + " WHERE ToUser='" + User+"'";

//            using (SqlConnection conn =
//                          new SqlConnection(connectionString))
//                try
//                {
//                    conn.Open();
//                    using (SqlCommand cmd =
//                        new SqlCommand(SqlString, conn))
//                    {

//                        // Read table 

//                        count = (int)cmd.ExecuteScalar();
//                    }

//                    // Close conn
//                    conn.Close();
//                }
//                catch (Exception ex)
//                {
//                    conn.Close();

//                    CatchDetails(ex);

//                }

//            return count;
//        }
// Unread messages 
        public int CountUnreadMSGs(string User)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int count = 0;
            string SqlString = "SELECT COUNT(*)"
          + " FROM [dbo].[ControlerMSGs] "
          + " WHERE ToUser= @ToUser AND ReadMsg=0";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ToUser", User);
                        // Read table 

                        count = (int)cmd.ExecuteScalar();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }

            return count;
        }

        // Insert NEW MSQ 
        //
        public void InsertMSg()
        {

            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [ControlerMSGs]"
                + " ([ToAllAtms],[AtmNo],[FromUser],[ToUser],[BankId],"
                 + " [BranchId],[DtTm],[Type],[Message],[ExpDate],[SeriousMsg],[OpenMsg],[Operator])"
                + " VALUES"
                + " (@ToAllAtms,@AtmNo,@FromUser,@ToUser,@BankId,"
                + " @BranchId,@DtTm,@Type,@Message,@ExpDate,@SeriousMsg,@OpenMsg, @Operator)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@ToAllAtms", ToAllAtms);
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@FromUser", FromUser);
                        cmd.Parameters.AddWithValue("@ToUser", ToUser);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
              
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@DtTm", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Type", Type);

                        cmd.Parameters.AddWithValue("@Message", Message);
                        cmd.Parameters.AddWithValue("@ExpDate", DateTime.Now);

                        cmd.Parameters.AddWithValue("@SeriousMsg", SeriousMsg);
                        cmd.Parameters.AddWithValue("@OpenMsg", 1 );

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

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
// Update message 
        public void UpdateMsgAsRead(int MsgId)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "UPDATE [ControlerMSGs]"
                + "SET [ReadMsg]=1 "
                + " WHERE "
                + " [MesNo]=@MsgNo";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@MsgNo", MsgId);

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

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
