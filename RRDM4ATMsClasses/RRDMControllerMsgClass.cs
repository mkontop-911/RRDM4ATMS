using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration; 

namespace RRDM4ATMs
{
    public class RRDMControllerMsgClass
    {
      
        public bool ToAllAtms; 
        public string AtmNo; 
        public string FromUser; 
        public string ToUser; 
        public string BankId; 
  
        public string BranchId; 
   //     public DateTime DtTm; 
        public string Type; 
        public string Message;
     //   public DateTime ExpDate; 

        public bool SeriousMsg;
     //   public bool OpenMsg; 

        public int SerMsgCount;

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;
        //
        // READ Controller Messages to see if there is a serious one 
        //
        public void ReadControlerMSGs(string InMsgFilter)
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ControllerMsgClass ............. " + ex.Message;
                }
        }
// Number of messages 
        public int CountMSGs(string User)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            int count = 0;
            string SqlString = "SELECT COUNT(*)"
          + " FROM [dbo].[ControlerMSGs] "
          + " WHERE ToUser='" + User+"'";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        // Read table 

                        count = (int)cmd.ExecuteScalar();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ControllerMsgClass............. " + ex.Message;

                }

            return count;
        }
// Unread messages 
        public int CountUnreadMSGs(string User)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            int count = 0;
            string SqlString = "SELECT COUNT(*)"
          + " FROM [dbo].[ControlerMSGs] "
          + " WHERE ToUser= '" + User + "' AND ReadMsg=0";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        // Read table 

                        count = (int)cmd.ExecuteScalar();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ControllerMsgClass............. " + ex.Message;

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ControllerMsgClass............. " + ex.Message;

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ControllerMsgClass............. " + ex.Message;

                }
        }
    }
}
