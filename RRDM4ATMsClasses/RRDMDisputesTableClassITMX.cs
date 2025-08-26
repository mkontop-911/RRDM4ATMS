using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMDisputesTableClassITMX : Logger
    {
        public RRDMDisputesTableClassITMX() : base() { }

        //Declare dispute fields

        public int DispId;
        public string BankId;
        public string RespBranch;

        public DateTime LastUpdateDtTm;
        public int DispFrom; // 1 = from Form5, ... 7 = From Matching process 
        public string CreatedByEntity; // eg ITMX or Bank 
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
      
        public bool ReadByBank;
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
            DisputesSelected.Columns.Add("BankId", typeof(string));
            DisputesSelected.Columns.Add("CustName", typeof(string));
            DisputesSelected.Columns.Add("DispType", typeof(int));
            DisputesSelected.Columns.Add("Origin", typeof(string));
            DisputesSelected.Columns.Add("OpenDate", typeof(DateTime));
            DisputesSelected.Columns.Add("OwnerId", typeof(string));
            DisputesSelected.Columns.Add("RespBranch", typeof(string));

            SqlString = "SELECT *"
                     + " FROM [ATMS].[dbo].[DisputesTableITMX] "
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

                            RowSelected["BankId"] = (string)rdr["BankId"];

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

                    CatchDetails(ex); 

                }
        }

        // 
        // Insert Dispute
        //

        public int InsertDisputeRecord()
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[DisputesTableITMX]"
                             + " ([BankId],[RespBranch],"
                             + " [LastUpdateDtTm],[DispFrom],[CreatedByEntity],[DispType],[OpenDate],[TargetDate],[CloseDate], "
                             + " [CardNo],[AccNo],[CustName],[CustPhone],[CustEmail],"
                             + " [OtherDispTypeDescr],[DispComments],[VisitType],"
                             + "  [OpenByUserId],[Active],[Operator]) "
                             + " VALUES( "
                              + " @BankId,@RespBranch,"
                             + " @LastUpdateDtTm,@DispFrom,@CreatedByEntity,@DispType,@OpenDate,@TargetDate,@CloseDate,@CardNo,"
                             + " @AccNo,@CustName,@CustPhone,@CustEmail,@OtherDispTypeDescr,@DispComments,@VisitType,"
                             + " @OpenByUserId, @Active,@Operator )"
                            +" SELECT CAST(SCOPE_IDENTITY() AS int)";


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
                        cmd.Parameters.AddWithValue("@CreatedByEntity", CreatedByEntity);
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

                        DispId = (int)cmd.ExecuteScalar();

                    }

                        // Close conn
                        conn.Close();
                    
                }

                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return DispId; 

            }
       
      // UPDATE   
        public void UpdateDisputeRecord(int InDispId)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[DisputesTableITMX] SET "
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

                        cmd.Parameters.AddWithValue("@DispId", InDispId);
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

        // UPDATE   
        public void UpdateReadByBank(int InDispId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[DisputesTableITMX] SET "
                                     + " ReadByBank = @ReadByBank "
                                     + " WHERE DispId = @DispId", conn))
                    {
                       
                        cmd.Parameters.AddWithValue("@DispId", InDispId);
                        cmd.Parameters.AddWithValue("@ReadByBank", ReadByBank);
                      

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
        // READ DISPUTE 
        public void ReadDispute(int InDispId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT *"
              + " FROM [ATMS].[dbo].[DisputesTableITMX] "
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
                            CreatedByEntity = (string)rdr["CreatedByEntity"];
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
                            
                            ReadByBank = (bool)rdr["ReadByBank"];
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

                    CatchDetails(ex);
                }
        }

        // READ DISPUTE 
        public void ReadDisputesAndUpdateStageIfFound(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
              + " FROM [ATMS].[dbo].[DisputesTableITMX] "
              + " WHERE "+ InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@DispId", InDispId);

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
                            CreatedByEntity = (string)rdr["CreatedByEntity"];
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
                            
                            ReadByBank = (bool)rdr["ReadByBank"];
                            Active = (bool)rdr["Active"];
                            Operator = (string)rdr["Operator"];

                            //Update Read By Bank 
                            ReadByBank = true; 
                            UpdateReadByBank(DispId); 

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
        // READ DISPUTE BY CARD NUMBER 
        public void ReadDisputeTotals(string InCardNo, string InBankId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            TotalOpenDisp = 0;
            TotalForCard = 0;
           
            SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[DisputesTableITMX] "
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
                            CreatedByEntity = (string)rdr["CreatedByEntity"];
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
                            
                            ReadByBank = (bool)rdr["ReadByBank"];
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

                    CatchDetails(ex);
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

            SqlString = "SELECT DispId "
          + " FROM [ATMS].[dbo].[DisputesTableITMX] "
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

                    CatchDetails(ex);
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
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[DisputesTableITMX] "
                            + " WHERE DispId =  @DispId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@DispId", InDispId);

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
