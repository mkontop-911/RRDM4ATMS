using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMsWin
{
    class RRDMDisputesTableClass
    {
        //Declare dispute fields
       
        public int DispId;
        public string BankId;
 
        public string RespBranch;
        public DateTime LastUpdate;
        public int DispForm;
        public int DispType;
        public DateTime OpenDtTm;
        public DateTime TargetDate; // 21/11/2150 
        public DateTime CloseDate; // 21/11/2150
        public string CardNo;
        public string AccNo;
        public string CustName;
        public string CustPhone;
        public string CustEmail;
        public string OtherDispTypeDescr; 
        public string DispComments;
        public string VisitType;
        public string UserId;
        public bool Active;
        public string Operator; 
      
        public int TotalOpenDisp; 
        public int TotalForCard ;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        public void InsertDisputeRecord()
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [DisputesTable]"
                             + " ([BankId],[RespBranch],"
                             + " [DateTime],[DispForm],[DispType],[OpenDate],[TargetDate],[CloseDate], "
                             + " [CardNo],[AccNo],[CustName],[CustPhone],[CustEmail],"
                             + " [OtherDispTypeDescr],[DispComments],[VisitType],"
                             + "  [UserId],[Active],[Operator]) "
                             + " VALUES( "
                              + " @BankId,@RespBranch,"
                             + " @DateTime,@DispForm,@DispType,@OpenDate,@TargetDate,@CloseDate,@CardNo,"
                             + " @AccNo,@CustName,@CustPhone,@CustEmail,@OtherDispTypeDescr,@DispComments,@VisitType,"
                             + " @UserId, @Active,@Operator )";
                        

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", BankId);
           
                        cmd.Parameters.AddWithValue("@RespBranch", RespBranch);
                        cmd.Parameters.AddWithValue("@DateTime", LastUpdate);
                        cmd.Parameters.AddWithValue("@DispForm", DispForm);
                        cmd.Parameters.AddWithValue("@DispType", DispType);
                        cmd.Parameters.AddWithValue("@OpenDate", OpenDtTm);
                        cmd.Parameters.AddWithValue("@TargetDate", TargetDate);
                        cmd.Parameters.AddWithValue("@CloseDate", CloseDate);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@CustName", CustName);
                        cmd.Parameters.AddWithValue("@CustPhone", CustPhone);
                        cmd.Parameters.AddWithValue("@CustEmail", CustEmail);
                        cmd.Parameters.AddWithValue("@OtherDispTypeDescr", OtherDispTypeDescr);
                        cmd.Parameters.AddWithValue("@DispComments", DispComments);
                        cmd.Parameters.AddWithValue("@VisitType", VisitType);
                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        cmd.Parameters.AddWithValue("@Active", Active);
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
                    ErrorOutput = "An error occured in DisputesTable Class............. " + ex.Message;
                }
            
            //RecordFound = true;

            }

    

        public void ReadDisputeLastNo(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM DisputesTable"
                   + " WHERE DispId = (SELECT MAX(DispId) FROM DisputesTable) AND UserId = @UserId";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@UserId", InUserId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Dispute Details
                            DispId = (int)rdr["DispId"];
                            LastUpdate = (DateTime)rdr["DateTime"];
                            DispForm = (int)rdr["DispForm"];
                            DispType = (int)rdr["DispType"];
                            OpenDtTm = (DateTime)rdr["OpenDate"];
                            TargetDate = (DateTime)rdr["TargetDate"];
                            CloseDate = (DateTime)rdr["CloseDate"];
                            CardNo = (string)rdr["CardNo"];
                            AccNo = (string)rdr["AccNo"];
                            CustName = (string)rdr["CustName"];
                            CustPhone = (string)rdr["CustPhone"];
                            CustEmail = (string)rdr["CustEmail"];
                            OtherDispTypeDescr = (string)rdr["OtherDispTypeDescr"];
                            DispComments = (string)rdr["DispComments"];
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in DisputesTable Class............. " + ex.Message;
                }
        }
      // UPDATE   
        public void UpdateDisputeRecord(int DispId)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE DisputesTable SET "
                                     + " BankId = @BankId, RespBranch = @RespBranch, "
                                     + " DateTime = @DateTime, DispForm = @DispForm, DispType = @DispType, "
                                     + " OpenDate = @OpenDate, TargetDate = @TargetDate, CloseDate = @CloseDate, "
                                     + " CardNo = @CardNo, AccNo = @AccNo, CustName = @CustName, "
                                     + " CustPhone = @CustPhone, CustEmail = @CustEmail,"
                                     + " DispComments = @DispComments, VisitType = @VisitType, "
                                     + " UserId = @UserId, Active = @Active " 
                                     + " WHERE DispId = @DispId", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                
                        cmd.Parameters.AddWithValue("@RespBranch", RespBranch);

                        cmd.Parameters.AddWithValue("@DispId", DispId);
                        cmd.Parameters.AddWithValue("@DateTime", LastUpdate);
                        cmd.Parameters.AddWithValue("@DispForm", DispForm);
                        cmd.Parameters.AddWithValue("@DispType", DispType);
                        cmd.Parameters.AddWithValue("@OpenDate", OpenDtTm);
                        cmd.Parameters.AddWithValue("@TargetDate", TargetDate);
                        cmd.Parameters.AddWithValue("@CloseDate", CloseDate);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@CustName", CustName);
                        cmd.Parameters.AddWithValue("@CustPhone", CustPhone);
                        cmd.Parameters.AddWithValue("@CustEmail", CustEmail);
                        cmd.Parameters.AddWithValue("@OtherDispTypeDescr", OtherDispTypeDescr);
                        cmd.Parameters.AddWithValue("@DispComments", DispComments);
                        cmd.Parameters.AddWithValue("@VisitType", VisitType);

                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        cmd.Parameters.AddWithValue("@Active", Active);
                        
                        
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in DisputesTable Class............. " + ex.Message;
                }
        }
        // READ DISPUTE 
        public void ReadDispute(int InDispId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
              + " FROM [dbo].[DisputesTable] "
              + " WHERE DispId = @DispId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@DispId", InDispId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Dispute Details
                            DispId = (int)rdr["DispId"];

                            BankId = (string)rdr["BankId"];
           
                            RespBranch = (string)rdr["RespBranch"];

                            LastUpdate = (DateTime)rdr["DateTime"];
                            DispForm = (int)rdr["DispForm"];
                            DispType = (int)rdr["DispType"];
                            OpenDtTm = (DateTime)rdr["OpenDate"];
                            TargetDate = (DateTime)rdr["TargetDate"];
                            CloseDate = (DateTime)rdr["CloseDate"];
                            CardNo = (string)rdr["CardNo"];
                            AccNo = (string)rdr["AccNo"];
                            CustName = (string)rdr["CustName"];
                            CustPhone = (string)rdr["CustPhone"];
                            CustEmail = (string)rdr["CustEmail"];
                            OtherDispTypeDescr = (string)rdr["OtherDispTypeDescr"];
                            DispComments = (string)rdr["DispComments"];
                            VisitType = (string)rdr["VisitType"];

                            UserId = (string)rdr["UserId"];
                            Active = (bool)rdr["Active"];
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in DisputesTable Class............. " + ex.Message;
                }
        }
        // READ DISPUTE BY CARD NUMBER 
        public void ReadDisputeTotals(string InCardNo, string InBankId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            TotalOpenDisp = 0;
            TotalForCard = 0;
           
            string SqlString = "SELECT *"
          + " FROM [dbo].[DisputesTable] "
          + " WHERE BankId = @BankId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Dispute Details
                            DispId = (int)rdr["DispId"];

                            BankId = (string)rdr["BankId"];
               
                            RespBranch = (string)rdr["RespBranch"];

                            LastUpdate = (DateTime)rdr["DateTime"];
                            DispForm = (int)rdr["DispForm"];
                            DispType = (int)rdr["DispType"];
                            OpenDtTm = (DateTime)rdr["OpenDate"];
                            TargetDate = (DateTime)rdr["TargetDate"];
                            CloseDate = (DateTime)rdr["CloseDate"];
                            CardNo = (string)rdr["CardNo"];
                            AccNo = (string)rdr["AccNo"];
                            CustName = (string)rdr["CustName"];
                            CustPhone = (string)rdr["CustPhone"];
                            CustEmail = (string)rdr["CustEmail"];
                            OtherDispTypeDescr = (string)rdr["OtherDispTypeDescr"];
                            DispComments = (string)rdr["DispComments"];
                            VisitType = (string)rdr["VisitType"];

                            UserId = (string)rdr["UserId"];
                            Active = (bool)rdr["Active"];
                            Operator = (string)rdr["Operator"];

                            if (Active == true) TotalOpenDisp = TotalOpenDisp + 1;
                            if (CardNo == InCardNo) TotalForCard = TotalForCard + 1; 
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
                    ErrorOutput = "An error occured in DisputesTable Class............. " + ex.Message;
                }
        }
        //
        // DELETE Dispute 
        //
        public void DeleteDisputeRecord(int InDispId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            // DELETE DISPUTE 
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[DisputesTable] "
                            + " WHERE DispId =  @DispId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@DispId", InDispId);

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Authorization Class............. " + ex.Message;
                }

            // DELETE DISPUTE TRANSACTIONS 
            using (SqlConnection conn =
                           new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[DisputesTransTable] "
                            + " WHERE DisputeNumber =  @DisputeNumber ", conn))
                    {
                        cmd.Parameters.AddWithValue("@DisputeNumber", InDispId);

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Authorization Class............. " + ex.Message;
                }
        }
}
}
