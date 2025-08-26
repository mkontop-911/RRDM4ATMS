using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMDisputesTableClass : Logger
    {
        public RRDMDisputesTableClass() : base() { }

        //Declare dispute fields

        public int DispId;
        public string BankId;
        public string DisputeCreatorId; 
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
        public bool IsCardLostStolen;  
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

        readonly string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        //
        // Read Dipute Fields 
        //
        private void ReadDispFields(SqlDataReader rdr)
        {
          
            DispId = (int)rdr["DispId"];

            BankId = (string)rdr["BankId"];

            DisputeCreatorId = (string)rdr["DisputeCreatorId"];

            RespBranch = (string)rdr["RespBranch"];
            LastUpdateDtTm = (DateTime)rdr["LastUpdateDtTm"];
             
            DispFrom = (int)rdr["DispFrom"];
            // 1 = call is coming from Main Form, 
            // 2 = Call is coming from Pre-Investigation (Unique)
            // 3 = Call is coming from deposits in difference,  
            // 4 = call is coming for updating details of dispute,  
            // 5 = call is coming from Reconciliation Process for ATMs CAsh - Record found in pool 
            //
            // 7 = call is coming from Reconciliation Process matching reconciliation - record found through mask,
            // 111 from Dispute pre-investigation 
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
            IsCardLostStolen = (bool)rdr["IsCardLostStolen"];
            OpenByUserId = (string)rdr["OpenByUserId"];
            HasOwner = (bool)rdr["HasOwner"];
           
            OwnerId = (string)rdr["OwnerId"];
            Active = (bool)rdr["Active"];
            Operator = (string)rdr["Operator"];
        }
        //
        // Methods 
        // READ Disputes in Table   
        // FILL UP A TABLE
        //
        public void ReadDisputesInTable(string InOperator, string InSignedId, string InCardNo,  
            DateTime InOpenDate, bool InWithDate, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            //SelectionCriteria = "Operator = @Operator AND Active = 1 AND OwnerId = @OwnerId ";
            //SelectionCriteria = "Operator ='" + WOperator + "'" + " AND Active = 1 AND OwnerId ='" + WSignedId + "'";
            //SelectionCriteria = "Operator ='" + WOperator + "'" + " AND Active = 1 AND OwnerId ='" + WSignedId + "'";
            //SelectionCriteria = "Operator ='" + WOperator + "'";
            //SelectionCriteria = "Operator ='" + WOperator + "' AND CardNo='" + WCardNo + "'";

            DisputesSelected = new DataTable();
            DisputesSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DisputesSelected.Columns.Add("DispId", typeof(int));
            DisputesSelected.Columns.Add("CustName", typeof(string));
            DisputesSelected.Columns.Add("DispType", typeof(int));
            DisputesSelected.Columns.Add("Settled", typeof(string));
            DisputesSelected.Columns.Add("Origin", typeof(string));
            DisputesSelected.Columns.Add("Creator", typeof(string));
            DisputesSelected.Columns.Add("DispOfficer", typeof(string));
            DisputesSelected.Columns.Add("RespBranch", typeof(string));
            DisputesSelected.Columns.Add("OpenDate", typeof(DateTime));
            
           
            if (InMode == 11)
            {
                SqlString = "SELECT * "
                                    + " FROM [dbo].[DisputesTable] "
                                     + " WHERE Operator = @Operator  AND OwnerId = @OwnerId "
                                     + " ORDER BY DispId DESC "
                                     ;
            }
            if (InMode == 12)
            {
                SqlString = "SELECT *"
                                    + " FROM [dbo].[DisputesTable] "
                                     + " WHERE Operator = @Operator ";
            }
            if (InMode == 13)
            {
                SqlString = "SELECT *"
                                    + " FROM [dbo].[DisputesTable] "
                                     + " WHERE Operator = @Operator AND CardNo = @CardNo";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InMode == 11)
                        {
                            cmd.Parameters.AddWithValue("@Operator", InOperator);
                            cmd.Parameters.AddWithValue("@OwnerId", InSignedId);
                        }
                        if (InMode == 12)
                        {
                            cmd.Parameters.AddWithValue("@Operator", InOperator);
                          
                        }
                        if (InMode == 13)
                        {
                            cmd.Parameters.AddWithValue("@Operator", InOperator);
                            cmd.Parameters.AddWithValue("@CardNo", InCardNo);

                        }
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

                            ReadDispFields(rdr);

                            DataRow RowSelected = DisputesSelected.NewRow();

                            RowSelected["DispId"] = DispId;
                            RowSelected["CustName"] = CustName;

                            RowSelected["DispType"] = DispType;

                            string TempVisitType = VisitType;

                            if (TempVisitType == "Through JCC request") // Means Is JCC 
                            {
                                RowSelected["Origin"] = "Other Bank Cust";
                            }
                            else
                            {
                                RowSelected["Origin"] = "Our Customer";
                            }

                            RowSelected["Creator"] = DisputeCreatorId;
                           
                            RowSelected["DispOfficer"] = OwnerId;
                            RowSelected["RespBranch"] = RespBranch;

                            IsCardLostStolen = IsCardLostStolen;

                            RowSelected["OpenDate"] = OpenDate;

                            if (Active == true)
                            {
                                RowSelected["Settled"] = "NO";
                            }
                            else
                            {
                                RowSelected["Settled"] = "YES";
                            }
                           
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
        
        // READ DISPUTE 
        public void ReadDispute(int InDispId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
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

                            ReadDispFields(rdr);

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
          + " FROM [dbo].[DisputesTable] "
          + " WHERE BankId = @BankId ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                      //  cmd.Parameters.AddWithValue("@CardNo", InCardNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadDispFields(rdr);

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

                            DisputeOwnerTotal = DisputeOwnerTotal + 1;

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
        // Methods 
        // READ Disputes in Table   
        // FILL UP A TABLE
        //
        public void ReadDisputesInTableByRangeAndSelectionCriteria(string InOperator, string InSignedId, string InSelectionCriteria,
            DateTime InFromDt, DateTime InToDt)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            bool WithDate = true;
            if (InFromDt == NullPastDate )
            {
                WithDate = false; 
            }
            //SelectionCriteria = "Operator = @Operator AND Active = 1 AND OwnerId = @OwnerId ";
            //SelectionCriteria = "Operator ='" + WOperator + "'" + " AND Active = 1 AND OwnerId ='" + WSignedId + "'";
            //SelectionCriteria = "Operator ='" + WOperator + "'" + " AND Active = 1 AND OwnerId ='" + WSignedId + "'";
            //SelectionCriteria = "Operator ='" + WOperator + "'";
            //SelectionCriteria = "Operator ='" + WOperator + "' AND CardNo='" + WCardNo + "'";


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
            DisputesSelected.Columns.Add("Settled", typeof(string));
            DisputesSelected.Columns.Add("RespBranch", typeof(string));

          if (WithDate == true)
            {
                SqlString = "SELECT * "
                                   + " FROM [dbo].[DisputesTable] "
                                   + InSelectionCriteria
                                    + " AND  ( CAST(OpenDate AS Date)>=@FromDt AND CAST(OpenDate AS Date)<=@ToDt) ";
            }
            if (WithDate == false)
            {
                SqlString = "SELECT * "
                                   + " FROM [dbo].[DisputesTable] "
                                   + InSelectionCriteria
                                    + " ";
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (WithDate == true)
                        {
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }
                       
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadDispFields(rdr);

                            DataRow RowSelected = DisputesSelected.NewRow();

                            RowSelected["DispId"] = DispId;
                            RowSelected["CustName"] = CustName;

                            RowSelected["DispType"] = DispType;

                            string TempVisitType = VisitType;

                            if (TempVisitType == "Through JCC request") // Means Is JCC 
                            {
                                RowSelected["Origin"] = "Other Bank Cust";
                            }
                            else
                            {
                                RowSelected["Origin"] = "Our Customer";
                            }

                            IsCardLostStolen = IsCardLostStolen;

                            RowSelected["OpenDate"] = OpenDate;
                            RowSelected["OwnerId"] = OwnerId;

                            if (Active == true)
                            {
                                RowSelected["Settled"] = "NO";
                            }
                            else
                            {
                                RowSelected["Settled"] = "YES";
                            }
                            
                            RowSelected["RespBranch"] = RespBranch;

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
        // Dispute Last Number
        //

        public void ReadDisputeLastNo(string InOpenByUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * FROM DisputesTable"
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

                            ReadDispFields(rdr);

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
        // SUM for Dispute owner 
        //

        public int FindSumByOwner(string InOwner)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Sum = 0; 

            SqlString = "SELECT Count(*) As CountDisp FROM DisputesTable "
                   + " WHERE Active = 1 and HasOwner = 1 AND OwnerId = @OwnerId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@OwnerId", InOwner);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            Sum = (int)rdr["CountDisp"];

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
            return Sum; 
        }

        // Create a psaudo Dispute 
        public int Create_Pseudo_Dispute(string InOperator, string InSignedId , int InUniqueRecordId, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            string SelectionCriteria = " WHERE UniqueRecordId = " + InUniqueRecordId;
            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);
            //  DispId = (int)rdr["DispId"]; // Set Up automatically

            BankId = InOperator;

            DisputeCreatorId = InSignedId;

            RespBranch = "015";
            LastUpdateDtTm = DateTime.Now;

            DispFrom = 111;  // 111 = from Dispute Pre - investigation SOLO 
            // 1 = call is coming from Main Form, 
            // 111 = from Dispute Pre - investigation SOLO 
            // 2 = Call is coming from Pre-Investigation (Unique)
            // 3 = Call is coming from deposits in difference,  
            // 4 = call is coming for updating details of dispute,  
            // 5 = call is coming from Reconciliation Process for ATMs CAsh - Record found in pool 
            //
            // 7 = call is coming from Reconciliation Process matching reconciliation - record found through mask,

            DispType = 2 ; // ATM Gave Less Money 
            OpenDate = DateTime.Now;
            TargetDate = DateTime.Now;

            CloseDate = NullPastDate;
            CardNo = Mpa.CardNumber;
            AccNo = Mpa.AccNumber;

            CustName = "Not Specified " ;

            CustPhone = "Not Specified";
            CustEmail = "Not Specified";
            OtherDispTypeDescr = "This Is A Pseudo Dispute ";
            DispComments = "Create after examination by officer" ;

            VisitType = "EMail By Branch";
            IsCardLostStolen = false;
            OpenByUserId = InSignedId;
            HasOwner = true;

            OwnerId = InSignedId;
            Active = true;
            Operator = InOperator;

            int DisputeNumber = InsertDisputeRecord(); 

            return DisputeNumber; 


        }



        // 
        // Insert Dispute
        //

        public int InsertDisputeRecord()
        {
           
            ErrorFound = false;
            ErrorOutput = "";
           
            //DisputeCreatorId
            string cmdinsert = "INSERT INTO [DisputesTable]"
                             + " ([BankId],[DisputeCreatorId],[RespBranch],"
                             + " [LastUpdateDtTm],[DispFrom],[DispType],[OpenDate],[TargetDate],[CloseDate], "
                             + " [CardNo],[AccNo],[CustName],[CustPhone],[CustEmail],"
                             + " [OtherDispTypeDescr],[DispComments],[VisitType],"
                             + " [IsCardLostStolen],"
                             + " [OpenByUserId],"
                             + " [HasOwner],[OwnerId],"
                             + " [Active],[Operator]) "
                             + " VALUES ( "
                              + " @BankId,@DisputeCreatorId,@RespBranch,"
                             + " @LastUpdateDtTm,@DispFrom,@DispType,@OpenDate,@TargetDate,@CloseDate,@CardNo,"
                             + " @AccNo,@CustName,@CustPhone,@CustEmail,@OtherDispTypeDescr,@DispComments,@VisitType,"
                             + " @IsCardLostStolen , "
                             + " @OpenByUserId,"
                             + " @HasOwner,@OwnerId, "
                             + " @Active,@Operator ) "
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
                        cmd.Parameters.AddWithValue("@DisputeCreatorId", DisputeCreatorId);
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
                        cmd.Parameters.AddWithValue("@IsCardLostStolen", IsCardLostStolen);
                        cmd.Parameters.AddWithValue("@OpenByUserId", OpenByUserId);
                        cmd.Parameters.AddWithValue("@HasOwner", HasOwner);
                        cmd.Parameters.AddWithValue("@OwnerId", OwnerId);
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
            //DisputeCreatorId
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
                                     + " IsCardLostStolen = @IsCardLostStolen,  "
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

                        cmd.Parameters.AddWithValue("@IsCardLostStolen", IsCardLostStolen);

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
