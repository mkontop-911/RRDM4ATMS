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

namespace RRDM4ATMs
{
    public class RRDMDisputesTableClass
    {
        //Declare dispute fields
       
        public int DispId;
        public string BankId;
 
        public string RespBranch;
        public DateTime LastUpdateDtTm;
        public int DispFrom;
        public int DispType;
        public DateTime OpenDate;
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
        public string OpenByUserId;

        public bool HasOwner;
        public string OwnerId; 

        public bool Active;
        public string Operator; 
      
        public int TotalOpenDisp; 
        public int TotalForCard ;

        public int DisputeOwnerTotal; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data table 
        public DataTable DisputesSelected = new DataTable();
        public int TotalSelected;
        string SqlString; // Do not delete 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        //
        // Methods 
        // READ Disputes in Table   
        // FILL UP A TABLE
        //
        public void ReadDisputesInTable(string InSelectionCriteria, DateTime InOpenDate, bool InWithDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DisputesSelected = new DataTable();
            DisputesSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DisputesSelected.Columns.Add("DispId", typeof(int));
            DisputesSelected.Columns.Add("CustName", typeof(string));
            DisputesSelected.Columns.Add("DispType", typeof(int));
            DisputesSelected.Columns.Add("Origin", typeof(string));
            DisputesSelected.Columns.Add("OpenDate", typeof(DateTime));
            DisputesSelected.Columns.Add("OwnerId", typeof(string));
            DisputesSelected.Columns.Add("BankId", typeof(string));
            DisputesSelected.Columns.Add("RespBranch", typeof(string));

            SqlString = "SELECT *"
                     + " FROM [dbo].[DisputesTable] "
                      + " WHERE " + InSelectionCriteria; 

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InWithDate == true)
                        {
                            cmd.Parameters.AddWithValue("@OpenDate", InOpenDate);
                        }
                    
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            DataRow RowSelected = DisputesSelected.NewRow();

                            RowSelected["DispId"] = (int)rdr["DispId"];
                            RowSelected["CustName"] = (string)rdr["CustName"];

                            RowSelected["DispType"] = (int)rdr["DispType"];

                            string TempVisitType = (string)rdr["VisitType"];

                            if (TempVisitType == "Through JCC request") // Means Is JCC 
                            {
                                RowSelected["Origin"] = "Other Bank Cust";
                            }
                            else
                            {
                                RowSelected["Origin"] = "Our Customer";
                            }
                            
                            RowSelected["OpenDate"] = (DateTime)rdr["OpenDate"];
                            RowSelected["OwnerId"] = (string)rdr["OwnerId"];

                            RowSelected["BankId"] = (string)rdr["BankId"];
                            RowSelected["RespBranch"] = (string)rdr["RespBranch"];

                            // ADD ROW
                            DisputesSelected.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in Reading table Disputes ............. " + ex.Message;

                }
        }

        // 
        // Insert Dispute
        //

        public void InsertDisputeRecord()
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [DisputesTable]"
                             + " ([BankId],[RespBranch],"
                             + " [LastUpdateDtTm],[DispFrom],[DispType],[OpenDate],[TargetDate],[CloseDate], "
                             + " [CardNo],[AccNo],[CustName],[CustPhone],[CustEmail],"
                             + " [OtherDispTypeDescr],[DispComments],[VisitType],"
                             + "  [OpenByUserId],[Active],[Operator]) "
                             + " VALUES( "
                              + " @BankId,@RespBranch,"
                             + " @LastUpdateDtTm,@DispFrom,@DispType,@OpenDate,@TargetDate,@CloseDate,@CardNo,"
                             + " @AccNo,@CustName,@CustPhone,@CustEmail,@OtherDispTypeDescr,@DispComments,@VisitType,"
                             + " @OpenByUserId, @Active,@Operator )";
                        

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
                        cmd.Parameters.AddWithValue("@LastUpdateDtTm", LastUpdateDtTm);
                        cmd.Parameters.AddWithValue("@DispFrom", DispFrom);
                        cmd.Parameters.AddWithValue("@DispType", DispType);
                        cmd.Parameters.AddWithValue("@OpenDate", OpenDate);
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
                        cmd.Parameters.AddWithValue("@OpenByUserId", OpenByUserId);
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
        // 
        // Dispute Last Number
        //

        public void ReadDisputeLastNo(string InOpenByUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM DisputesTable"
                   + " WHERE DispId = (SELECT MAX(DispId) FROM DisputesTable) AND OpenByUserId = @OpenByUserId";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@OpenByUserId", InOpenByUserId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Dispute Details
                            DispId = (int)rdr["DispId"];
                            LastUpdateDtTm = (DateTime)rdr["LastUpdateDtTm"];
                            DispFrom = (int)rdr["DispFrom"];
                            DispType = (int)rdr["DispType"];
                            OpenDate = (DateTime)rdr["OpenDate"];
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
                                     + " LastUpdateDtTm = @LastUpdateDtTm, DispFrom = @DispFrom, DispType = @DispType, "
                                     + " OpenDate = @OpenDate, TargetDate = @TargetDate, CloseDate = @CloseDate, "
                                     + " CardNo = @CardNo, AccNo = @AccNo, CustName = @CustName, "
                                     + " CustPhone = @CustPhone, CustEmail = @CustEmail,"
                                     + " DispComments = @DispComments, VisitType = @VisitType, "
                                     + " OpenByUserId = @OpenByUserId, HasOwner = @HasOwner, OwnerId = @OwnerId, " 
                                     + " Active = @Active " 
                                     + " WHERE DispId = @DispId", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                
                        cmd.Parameters.AddWithValue("@RespBranch", RespBranch);

                        cmd.Parameters.AddWithValue("@DispId", DispId);
                        cmd.Parameters.AddWithValue("@LastUpdateDtTm", LastUpdateDtTm);
                        cmd.Parameters.AddWithValue("@DispFrom", DispFrom);
                        cmd.Parameters.AddWithValue("@DispType", DispType);
                        cmd.Parameters.AddWithValue("@OpenDate", OpenDate);
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

                        cmd.Parameters.AddWithValue("@OpenByUserId", OpenByUserId);

                        cmd.Parameters.AddWithValue("@HasOwner", HasOwner);
                        cmd.Parameters.AddWithValue("@OwnerId", OwnerId);

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

                            LastUpdateDtTm = (DateTime)rdr["LastUpdateDtTm"];
                            DispFrom = (int)rdr["DispFrom"];
                            DispType = (int)rdr["DispType"];
                            OpenDate = (DateTime)rdr["OpenDate"];
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

                            OpenByUserId = (string)rdr["OpenByUserId"];

                            HasOwner = (bool)rdr["HasOwner"];
                            OwnerId = (string)rdr["OwnerId"];
                            
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

                            LastUpdateDtTm = (DateTime)rdr["LastUpdateDtTm"];
                            DispFrom = (int)rdr["DispFrom"];
                            DispType = (int)rdr["DispType"];
                            OpenDate = (DateTime)rdr["OpenDate"];
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

                            OpenByUserId = (string)rdr["OpenByUserId"];

                            HasOwner = (bool)rdr["HasOwner"];
                            OwnerId = (string)rdr["OwnerId"];

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
        // Dispute Owner Total 
        //

        public void ReadDisputeOwnerTotal(string InOwnerId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DisputeOwnerTotal = 0;

            string SqlString = "SELECT DispId "
          + " FROM [dbo].[DisputesTable] "
          + " WHERE OwnerId = @OwnerId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@OwnerId", InOwnerId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            DisputeOwnerTotal = DisputeOwnerTotal + 1 ; 
                            
                            DispId = (int)rdr["DispId"];
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
                    ErrorOutput = "An error occured in Disputes Owner Class............. " + ex.Message;
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
