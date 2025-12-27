using System;
using System.Text;

using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMHostMatchingClassGeneralLedger : Logger
    {
        public RRDMHostMatchingClassGeneralLedger() : base() { }

        public int PostedNo;
        public int TranOrigin;

        public int ErrNo;
        public string AtmNo;
        public int SesNo;
        public string BankId;
        public string BranchId;
      
        public int AtmTraceNo;

        public int SystemTarget;

        public DateTime AtmDtTime;
        public int TransType;
        public string TransDesc;
        public string CurrDesc;
        public decimal TranAmount;

        public string CardNo;
        public string AccNo;

        public DateTime HostDtTime;

        public int AuthCode;
        public int RefNumb;
        
        public int RemNo;

        public string ActionBy;

        public int ActionCd2;

        public bool HostMatched;
        public DateTime MatchedDtTm;
        public int Reconciled;

        public string Operator; 

        public bool GlEntry; 

        public string WOperator; 

        public int TotalNotMatched; 

        public DateTime ActionDate;

        public DateTime OpenDate;

        public int CardOrigin;
        public int TransType2;
        public string TransDesc2;
        public string AccNo2;
        public bool GlEntry2;
        public string TransMsg;
        public string AtmMsg;
        public bool OpenRecord;

     //   bool IsHoliday = true;

        DateTime DateToWorking;
        int WPostedNo;

    //    string WUserBankId; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        RRDMHostGeneralLegerTblsClass Ht = new RRDMHostGeneralLegerTblsClass();
        //
        // READ Merge File based on catd no  
        //

        // READ TRANS TO BE POSTED  BASED ON IDentity  

        public void MakeMatching(string InOperator, bool InHostMatched)
        {
            WOperator = InOperator;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
                     + " FROM [dbo].[TransToBePosted] "
                     + " WHERE Operator = @Operator AND HostMatched = @HostMatched AND ActionCd2 = 1";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@HostMatched", InHostMatched); // Not matched 

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            PostedNo = (int)rdr["PostedNo"];
                            TranOrigin = (int)rdr["TranOrigin"];
                            ErrNo = (int)rdr["ErrNo"];
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            BankId = (string)rdr["BankId"];
                   
                            AtmTraceNo = (int)rdr["AtmTraceNo"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];

                            HostDtTime = (DateTime)rdr["HostDtTime"];

                            SystemTarget = (int)rdr["SystemTarget"];
                             
                            CardNo = (string)rdr["CardNo"];
                            CardOrigin = (int)rdr["CardOrigin"];

                            // First Transaction
                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];
                            AccNo = (string)rdr["AccNo"];
                            GlEntry = (bool)rdr["GlEntry"];
                            // Second Transaction
                            TransType2 = (int)rdr["TransType2"]; // 11 for debit 21 for credit
                            TransDesc2 = (string)rdr["TransDesc2"];
                            AccNo2 = (string)rdr["AccNo2"];
                            GlEntry2 = (bool)rdr["GlEntry2"];
                            // End of second 

                            CurrDesc = (string)rdr["CurrDesc"];
                            TranAmount = (decimal)rdr["TranAmount"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNumb = (int)rdr["RefNumb"];
                            RemNo = (int)rdr["RemNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            AtmMsg = (string)rdr["AtmMsg"];

                            ActionBy = (string)rdr["ActionBy"];
                            ActionCd2 = (int)rdr["ActionCd2"];

                            ActionDate = (DateTime)rdr["ActionDate"];

                            OpenDate = (DateTime)rdr["OpenDate"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            HostMatched = (bool)rdr["HostMatched"];
                            MatchedDtTm = (DateTime)rdr["MatchedDtTm"];
                            Reconciled = (int)rdr["Reconciled"];
                            Operator = (string)rdr["Operator"];

                    // MAKE MATCHING 
                            WPostedNo = PostedNo; 

                            Ht.ReadHostGeneralLedgerSpecificRefNo(AtmNo, WPostedNo);
                            if (Ht.RecordFound == true)
                            {
                                // Update
                                // Matched, date of matched and Reconcile record
                                Tc.UpdateTransToBePostedMatched(AtmNo, WPostedNo, true, DateTime.Now, 1);

                                // Update host trans with the date of matching 

                                Ht.TranMatched = true;
                                Ht.MatchedDtTm = DateTime.Today;
                                Ht.UpdateHostGeneralLedgerSpecific(AtmNo, WPostedNo); 
                            }
                            else
                            {
                                int WorkDays = 3;

                                RRDMAtmsClass Ac = new RRDMAtmsClass();

                                Ac.ReadAtm(AtmNo); 

                                AddWorkdays(ActionDate.Date, WorkDays, Ac.HolidaysVersion); 

                                // ACTION date was taken later than three working days create exception messages  
                                if ( DateTime.Today > DateToWorking )
                                {
                                    WorkDays = 3;
                                    //MessageBox.Show("Not matched for more than allowed working days"); 
                                }
                            }

                        }

                        // Close ReaderOpenRecord
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

        // READ TRANS TO BE POSTED  BASED ON IDentity  

        public void GetMatchingTotals(string InOperator)
        {
            TotalNotMatched = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
                     + " FROM [dbo].[TransToBePosted] "
                     + " WHERE Operator = @Operator AND HostMatched = 0";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                     //   cmd.Parameters.AddWithValue("@HostMatched", InHostMatched);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalNotMatched = TotalNotMatched + 1; 

                        }

                        // Close ReaderOpenRecord
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
        public void AddWorkdays(DateTime originalDate, int workDays, string InHolidaysVersion)
        {
            RRDMHolidays Ch = new RRDMHolidays(); 
           
            DateTime tmpDate = originalDate;

            while (workDays > 0)
            {
                tmpDate = tmpDate.AddDays(1);
                Ch.ReadSpecificDate(WOperator, tmpDate.Date,InHolidaysVersion ); 
                if (Ch.IsNormal == true)
                {
                    workDays = workDays - 1 ; 
                }   
                else
                {
                }     
            }

            DateToWorking = tmpDate; 
        }
        // DELETE Trans to be posted 
        //
        public void DeleteOutStandingTransToBePosted(int InPostedNo)
        {
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[TransToBePosted] "
                            + " WHERE PostedNo = @PostedNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankSwiftId", InPostedNo);

                       
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


