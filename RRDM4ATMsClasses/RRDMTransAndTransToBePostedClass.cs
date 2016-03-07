using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;


namespace RRDM4ATMs
{
    public class RRDMTransAndTransToBePostedClass
    {
        // Fields for both InPool and Trans to be posted 

        public int PostedNo;

        public string OriginId; // *
                                // "01" OurATMS-Matching
                                // "02" BancNet Matching                               
                                // "03" OurATMS-Reconc
                                // "04" OurATMS-Repl
                                // "05" Settlement
                                // "07" Disputes 
                                // "08" Instructions to CIT
                                
        public string OriginName;

        public string RMCateg; //*
        public int RMCategCycle; //*

        public int MaskRecordId; 

        public int ErrNo;
        public string AtmNo;
        public int SesNo;

        public int DisputeNo;
        public int DispTranNo;

        public string BankId;
                    
        public int AtmTraceNo;
    
        public string BranchId;
        public DateTime AtmDtTime;

        
        //public DateTime HostDtTime;

        public string CardNo;
        public int CardOrigin;

        public int SystemTarget;
        public int TransType; // 11 And 21 is related with customer withdrwals and reversals cassettes 
                              // 12 And 22 are related with Cash In and Cash out during Replenishment 
                              // 23 Cash customer deposits 24 Cheques customer deposits 
        public string TransDesc;
        public string AccNo;
        public bool GlEntry;

        public int TransType2; 
        public string TransDesc2;
        public string AccNo2;
        public bool GlEntry2;

        public string CurrDesc;
        public decimal TranAmount;
        public int AuthCode;
        public int RefNumb;
        public int RemNo;

        public string TransMsg;
        public string AtmMsg;

   //     public string OriginatorUser;
        public string AuthUser; // THIS IS FOR AUTHORise user to access these transactions 
        public string ActionBy;

        public int ActionCd2 ;

        public DateTime ActionDate ;

        public DateTime OpenDate;

        public bool OpenRecord;

        public int StartTrxn;
        public int EndTrxn;

        public decimal DepCount;

        public int CommissionCode;
        public decimal CommissionAmount; 

        public bool SuccTran;

        // DEFINE FIELDS FOR HOST TRANSACTIONS 

        public int TranNoH;
        public int TranOriginH;
        public int AtmTraceNoH;
        public int HostTraceNoH;
                       
        public DateTime HostDtTimeH;

        public string TransDescH;
        public string CardNoH ;

        public string AccNoH;

        public int AuthCodeH;
        public int RefNumbH;
        public int RemNoH;

        public string TransMsgH;
        public int ErrNoH;

        public bool SuccTranH;

        public bool HostMatched; 
        public DateTime MatchedDtTm;
        public int Reconciled;

        public string Operator; 

        string ParamId ;
        string OccuranceId ; 
        string RelatedParmId ;
        string RelatedOccuranceId;

        public DateTime GridFilterDate; // THIS WILL NOT CONTAIN MINUTES       

        // Fields for InPool Trans
        public int TranNo;
        public int EJournalTraceNo; 

        public int TotActions ;
        public int TotTransactions;
        public int TotActionsTaken ;
        public int TotActionsNotTaken ;

        // Define the data table 
        public DataTable TransToBePostedSelected = new DataTable();

        public DataTable TableDisputedTrans = new DataTable(); 

        public int TotalSelected; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;
    
        RRDMAtmsDailyTransHistory Ah = new RRDMAtmsDailyTransHistory();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMHolidays Ho = new RRDMHolidays();
        RRDMGasParameters Gp = new RRDMGasParameters();
  
        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();
        RRDMReplStatsClass Rs = new RRDMReplStatsClass();
        RRDMAtmsCostClass Ap = new RRDMAtmsCostClass();

        // READ ALL TRANS TO BE POSTED  BASED ON SELECTION CRITERIA  

        public void ReadAllTransToBePosted( string InSelectionCriteria, DateTime InToday, bool InWithDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TransToBePostedSelected = new DataTable();
            TransToBePostedSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TransToBePostedSelected.Columns.Add("PostedNo", typeof(int));
            TransToBePostedSelected.Columns.Add("Origin", typeof(string));
            TransToBePostedSelected.Columns.Add("OutStanding", typeof(string));

            TransToBePostedSelected.Columns.Add("CurrDesc", typeof(string));
            TransToBePostedSelected.Columns.Add("TranAmount", typeof(string));

            TransToBePostedSelected.Columns.Add("Type", typeof(string));
            TransToBePostedSelected.Columns.Add("TransDesc", typeof(string));
            TransToBePostedSelected.Columns.Add("AccNo", typeof(string));
            TransToBePostedSelected.Columns.Add("GlEntry", typeof(bool));

            TransToBePostedSelected.Columns.Add("Type2", typeof(string));
            TransToBePostedSelected.Columns.Add("TransDesc2", typeof(string));
            TransToBePostedSelected.Columns.Add("AccNo2", typeof(string));
            TransToBePostedSelected.Columns.Add("GlEntry2", typeof(bool));

            TransToBePostedSelected.Columns.Add("AtmNo", typeof(string));
            TransToBePostedSelected.Columns.Add("ReplCycle", typeof(int));

           

            string SqlString = "SELECT *"
                       + " FROM [dbo].[TransToBePosted] "
                       + " WHERE " + InSelectionCriteria; 
    
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if ( InWithDate == true)
                        {
                            cmd.Parameters.AddWithValue("@Today", InToday);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //************************************************
                            //************************************************

                            TotalSelected = TotalSelected + 1;

                            OpenRecord = (bool)rdr["OpenRecord"];

                            DataRow RowSelected = TransToBePostedSelected.NewRow();

                            RowSelected["PostedNo"] = (int)rdr["PostedNo"];

                            RowSelected["Origin"] = (string)rdr["OriginName"];
                            if (OpenRecord == true)
                            {
                                RowSelected["OutStanding"] = "YES";
                            }
                            else
                            {
                                RowSelected["OutStanding"] = "NO";
                            }
                            

                            RowSelected["CurrDesc"] = (string)rdr["CurrDesc"];

                            //decimal Temp = (decimal)rdr["TranAmount"] ;
                            //string Temp2 = Temp.ToString("#,##0.00");
                            RowSelected["TranAmount"] = ((decimal)rdr["TranAmount"]).ToString("#,##0.00");
                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit

                            if (TransType == 11 || TransType == 12)
                            {
                                RowSelected["Type"] = "DR";
                            }
                            if (TransType == 21 || TransType == 22)
                            {
                                RowSelected["Type"] = "CR";
                            }

                            RowSelected["TransDesc"] = (string)rdr["TransDesc"];
                            RowSelected["AccNo"] = (string)rdr["AccNo"];
                            RowSelected["GlEntry"] = (bool)rdr["GlEntry"];

                            TransType2 = (int)rdr["TransType2"]; // 11 for debit 21 for credit

                            if (TransType2 == 11 || TransType2 == 12)
                            {
                                RowSelected["Type2"] = "DR";
                            }
                            if (TransType2 == 21 || TransType2 == 22)
                            {
                                RowSelected["Type2"] = "CR";
                            }

                            RowSelected["TransDesc2"] = (string)rdr["TransDesc2"];
                            RowSelected["AccNo2"] = (string)rdr["AccNo2"];
                            RowSelected["GlEntry2"] = (bool)rdr["GlEntry2"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];

                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["ReplCycle"] = SesNo;
                        
                            // ADD ROW
                            TransToBePostedSelected.Rows.Add(RowSelected);

                            //************************************************
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    //  log4net.Config.XmlConfigurator.Configure();
                    //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "ReadTransToBePosted");
                }
        }
        // READ ALL TRANS TO BE POSTED  BASED ON FROM TO DATE   

        public void ReadAllTransToBePostedRange(string InSelectionCriteria, DateTime InDtFrom, DateTime InDtTo )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TransToBePostedSelected = new DataTable();
            TransToBePostedSelected.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            TransToBePostedSelected.Columns.Add("PostedNo", typeof(int));
            TransToBePostedSelected.Columns.Add("Origin", typeof(string));
            TransToBePostedSelected.Columns.Add("OpenRecord", typeof(bool));

            TransToBePostedSelected.Columns.Add("CurrDesc", typeof(string));
            TransToBePostedSelected.Columns.Add("TranAmount", typeof(string));

            TransToBePostedSelected.Columns.Add("Type", typeof(string));
            TransToBePostedSelected.Columns.Add("TransDesc", typeof(string));
            TransToBePostedSelected.Columns.Add("AccNo", typeof(string));
            TransToBePostedSelected.Columns.Add("GlEntry", typeof(bool));

            TransToBePostedSelected.Columns.Add("Type2", typeof(string));
            TransToBePostedSelected.Columns.Add("TransDesc2", typeof(string));
            TransToBePostedSelected.Columns.Add("AccNo2", typeof(string));
            TransToBePostedSelected.Columns.Add("GlEntry2", typeof(bool));

            TransToBePostedSelected.Columns.Add("AtmNo", typeof(string));
            TransToBePostedSelected.Columns.Add("ReplCycle", typeof(int));



            string SqlString = "SELECT *"
                       + " FROM [dbo].[TransToBePosted] "
                       + " WHERE " + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@WDtFrom", InDtFrom);
                        cmd.Parameters.AddWithValue("@WDtTo", InDtTo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            //************************************************
                            //************************************************

                            TotalSelected = TotalSelected + 1;

                            DataRow RowSelected = TransToBePostedSelected.NewRow();

                            RowSelected["PostedNo"] = (int)rdr["PostedNo"];

                            RowSelected["Origin"] = (string)rdr["OriginName"];

                            RowSelected["OpenRecord"] = (bool)rdr["OpenRecord"];

                            RowSelected["CurrDesc"] = (string)rdr["CurrDesc"];

                            decimal Temp = (decimal)rdr["TranAmount"];
                            string Temp2 = Temp.ToString("#,##0.00");
                            RowSelected["TranAmount"] = Temp2;
                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit

                            if (TransType == 11 || TransType == 12)
                            {
                                RowSelected["Type"] = "DR";
                            }
                            if (TransType == 21 || TransType == 22)
                            {
                                RowSelected["Type"] = "CR";
                            }

                            RowSelected["TransDesc"] = (string)rdr["TransDesc"];
                            RowSelected["AccNo"] = (string)rdr["AccNo"];
                            RowSelected["GlEntry"] = (bool)rdr["GlEntry"];

                            TransType2 = (int)rdr["TransType2"]; // 11 for debit 21 for credit

                            if (TransType2 == 11 || TransType2 == 12)
                            {
                                RowSelected["Type2"] = "DR";
                            }
                            if (TransType2 == 21 || TransType2 == 22)
                            {
                                RowSelected["Type2"] = "CR";
                            }

                            RowSelected["TransDesc2"] = (string)rdr["TransDesc2"];
                            RowSelected["AccNo2"] = (string)rdr["AccNo2"];
                            RowSelected["GlEntry2"] = (bool)rdr["GlEntry2"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];

                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["ReplCycle"] = SesNo;

                            // ADD ROW
                            TransToBePostedSelected.Rows.Add(RowSelected);

                            //************************************************
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    //  log4net.Config.XmlConfigurator.Configure();
                    //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "ReadTransToBePosted");
                }
        }

        // READ TRANS TO BE POSTED  BASED ON ERROR NUMBER AND CARD NUMBER 

        public void ReadTransToBePosted(int InErrNo, string InCardNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[TransToBePosted] "
          + " WHERE ErrNo = @ErrNo AND CardNo =@CardNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);
                        cmd.Parameters.AddWithValue("@CardNo", InCardNo);
                    

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            PostedNo = (int)rdr["PostedNo"];  
                       
                            OriginId = (string)rdr["OriginId"];
                            OriginName = (string)rdr["OriginName"];
                            RMCateg = (string)rdr["RMCateg"];
                            RMCategCycle = (int)rdr["RMCategCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            ErrNo = (int)rdr["ErrNo"];
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];

                            DisputeNo = (int)rdr["DisputeNo"];
                            DispTranNo = (int)rdr["DispTranNo"];

                            BankId = (string)rdr["BankId"];
                  
                            AtmTraceNo = (int)rdr["AtmTraceNo"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];

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

                            AuthUser = (string)rdr["AuthUser"];
                            ActionBy = (string)rdr["ActionBy"];

                            ActionCd2 = (int)rdr["ActionCd2"];

                            ActionDate = (DateTime)rdr["ActionDate"];

                            OpenDate = (DateTime)rdr["OpenDate"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            HostMatched = (bool)rdr["HostMatched"];
                            MatchedDtTm = (DateTime)rdr["MatchedDtTm"];
                            Reconciled = (int)rdr["Reconciled"];

                            GridFilterDate = (DateTime)rdr["GridFilterDate"];
                            Operator = (string)rdr["Operator"];
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                  //  log4net.Config.XmlConfigurator.Configure();
                  //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "ReadTransToBePosted");
                }
        }

        // READ TRANS TO BE POSTED  BASED ON ATMNo , TraceNo,  

        public void ReadTransToBePostedTraceSequence(string InAtmNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[TransToBePosted] "
          + " WHERE AtmNo = @AtmNo AND AtmTraceNo = @AtmTraceNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", InTraceNo);
                    

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            PostedNo = (int)rdr["PostedNo"];

                            OriginId = (string)rdr["OriginId"];
                            OriginName = (string)rdr["OriginName"];
                            RMCateg = (string)rdr["RMCateg"];
                            RMCategCycle = (int)rdr["RMCategCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            ErrNo = (int)rdr["ErrNo"];
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];

                            DisputeNo = (int)rdr["DisputeNo"];
                            DispTranNo = (int)rdr["DispTranNo"];

                            BankId = (string)rdr["BankId"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];

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

               //             OriginatorUser = (string)rdr["OriginatorUser"];
                            AuthUser = (string)rdr["AuthUser"];
                            ActionBy = (string)rdr["ActionBy"];

                            ActionCd2 = (int)rdr["ActionCd2"];

                            ActionDate = (DateTime)rdr["ActionDate"];

                            OpenDate = (DateTime)rdr["OpenDate"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            HostMatched = (bool)rdr["HostMatched"];
                            MatchedDtTm = (DateTime)rdr["MatchedDtTm"];
                            Reconciled = (int)rdr["Reconciled"];

                            GridFilterDate = (DateTime)rdr["GridFilterDate"];

                            Operator = (string)rdr["Operator"];
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                   
                 //   log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                   //                                               "ReadTransToBePostedTraceSequence");
                }
        }

        // READ TRANS TO BE POSTED  BASED ON IDentity  

        public void ReadTransToBePostedSpecific(int InPostedNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[TransToBePosted] "
          + " WHERE PostedNo = @PostedNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@PostedNo", InPostedNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            PostedNo = (int)rdr["PostedNo"];

                            OriginId = (string)rdr["OriginId"];
                            OriginName = (string)rdr["OriginName"];
                            RMCateg = (string)rdr["RMCateg"];
                            RMCategCycle = (int)rdr["RMCategCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            ErrNo = (int)rdr["ErrNo"];
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];

                            DisputeNo = (int)rdr["DisputeNo"];
                            DispTranNo = (int)rdr["DispTranNo"];

                            BankId = (string)rdr["BankId"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];

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

           //                 OriginatorUser = (string)rdr["OriginatorUser"];
                            AuthUser = (string)rdr["AuthUser"];
                            ActionBy = (string)rdr["ActionBy"];

                            ActionCd2 = (int)rdr["ActionCd2"];

                            ActionDate = (DateTime)rdr["ActionDate"];

                            OpenDate = (DateTime)rdr["OpenDate"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            HostMatched = (bool)rdr["HostMatched"];
                            MatchedDtTm = (DateTime)rdr["MatchedDtTm"];
                            Reconciled = (int)rdr["Reconciled"];

                            GridFilterDate = (DateTime)rdr["GridFilterDate"];

                            Operator = (string)rdr["Operator"];
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                  //  log4net.Config.XmlConfigurator.Configure();
                  //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                   //                                               "UpdateTransToBePostedAction1");
                }
        }

        // READ TRANS TO BE POSTED  BASED ON ATM Trace Number 

        public void ReadTransToBePostedSpecificTraceNo(string InAtmNo, int InAtmTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[TransToBePosted] "
          + " WHERE AtmNo = @AtmNo AND AtmTraceNo = @AtmTraceNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", InAtmTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            PostedNo = (int)rdr["PostedNo"];

                            OriginId = (string)rdr["OriginId"];
                            OriginName = (string)rdr["OriginName"];
                            RMCateg = (string)rdr["RMCateg"];
                            RMCategCycle = (int)rdr["RMCategCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];

                            ErrNo = (int)rdr["ErrNo"];

                           
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];

                            DisputeNo = (int)rdr["DisputeNo"];
                            DispTranNo = (int)rdr["DispTranNo"];

                            BankId = (string)rdr["BankId"];
                   
                            AtmTraceNo = (int)rdr["AtmTraceNo"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];

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

                   //         OriginatorUser = (string)rdr["OriginatorUser"];
                            AuthUser = (string)rdr["AuthUser"];
                            ActionBy = (string)rdr["ActionBy"];

                            ActionCd2 = (int)rdr["ActionCd2"];

                            ActionDate = (DateTime)rdr["ActionDate"];

                            OpenDate = (DateTime)rdr["OpenDate"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            HostMatched = (bool)rdr["HostMatched"];
                            MatchedDtTm = (DateTime)rdr["MatchedDtTm"];
                            Reconciled = (int)rdr["Reconciled"];

                            GridFilterDate = (DateTime)rdr["GridFilterDate"];

                            Operator = (string)rdr["Operator"];
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                //    log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "UpdateTransToBePostedAction1");
                }
        }
//
        // READ TRANS TO BE POSTED  BASED ON THE UNIQUE NUMBER  
//
        public void ReadTransToBePostedSpecificByMaskRecordId(int InMaskRecordId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [dbo].[TransToBePosted] "
          + " WHERE MaskRecordId = @MaskRecordId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaskRecordId", InMaskRecordId);
                       
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            PostedNo = (int)rdr["PostedNo"];

                            OriginId = (string)rdr["OriginId"];
                            OriginName = (string)rdr["OriginName"];
                            RMCateg = (string)rdr["RMCateg"];
                            RMCategCycle = (int)rdr["RMCategCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];

                            ErrNo = (int)rdr["ErrNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];

                            DisputeNo = (int)rdr["DisputeNo"];
                            DispTranNo = (int)rdr["DispTranNo"];

                            BankId = (string)rdr["BankId"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];

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

                            //         OriginatorUser = (string)rdr["OriginatorUser"];
                            AuthUser = (string)rdr["AuthUser"];
                            ActionBy = (string)rdr["ActionBy"];

                            ActionCd2 = (int)rdr["ActionCd2"];

                            ActionDate = (DateTime)rdr["ActionDate"];

                            OpenDate = (DateTime)rdr["OpenDate"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            HostMatched = (bool)rdr["HostMatched"];
                            MatchedDtTm = (DateTime)rdr["MatchedDtTm"];
                            Reconciled = (int)rdr["Reconciled"];

                            GridFilterDate = (DateTime)rdr["GridFilterDate"];

                            Operator = (string)rdr["Operator"];
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TransAndPosted Class by MaskRecordId............. " + ex.Message;
                    //    log4net.Config.XmlConfigurator.Configure();
                    //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "UpdateTransToBePostedAction1");
                }
        }


        // READ OPEN TRANS TO BE POSTED  
        // Read all Sequentially
        // For each record create two transactions in PostedTrans table 
        // Denote trans with action taken 

        public void ReadTransToBePostedAllAndCreatePostedTrans(string InSignedId, string InOperator, int InActionCd2)
        {

            TotTransactions = 0; 

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMPostedTrans Pt = new RRDMPostedTrans(); 

            string SqlString = "SELECT *"
          + " FROM [dbo].[TransToBePosted] "
          + " WHERE Operator = @Operator AND OpenRecord = 1 ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            PostedNo = (int)rdr["PostedNo"];

                            OriginId = (string)rdr["OriginId"];
                            OriginName = (string)rdr["OriginName"];
                            RMCateg = (string)rdr["RMCateg"];
                            RMCategCycle = (int)rdr["RMCategCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            ErrNo = (int)rdr["ErrNo"];
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];

                            DisputeNo = (int)rdr["DisputeNo"];
                            DispTranNo = (int)rdr["DispTranNo"];

                            BankId = (string)rdr["BankId"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];

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

                            //         OriginatorUser = (string)rdr["OriginatorUser"];
                            AuthUser = (string)rdr["AuthUser"];
                            ActionBy = (string)rdr["ActionBy"];

                            ActionCd2 = (int)rdr["ActionCd2"];

                            ActionDate = (DateTime)rdr["ActionDate"];

                            OpenDate = (DateTime)rdr["OpenDate"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            HostMatched = (bool)rdr["HostMatched"];
                            MatchedDtTm = (DateTime)rdr["MatchedDtTm"];
                            Reconciled = (int)rdr["Reconciled"];

                            GridFilterDate = (DateTime)rdr["GridFilterDate"];

                            Operator = (string)rdr["Operator"];

                            // Insert a pair of transactions in Posted Trans

                            // Posted First transaction 

                            // Close any open with this PostedNo

                            Pt.UpdateAsClosedTheAlreadyInTable(PostedNo); 
                            
                            //
                            Pt.TranToBePostedKey = PostedNo;
                            Pt.Origin = OriginName;
                            Pt.UserId = AuthUser;
                            Pt.AccNo = AccNo;
                            Pt.AtmNo = AtmNo;
                            Pt.ReplCycle = SesNo;
                            Pt.BankId = BankId;

                            Pt.TranDtTime = DateTime.Now;
                            Pt.TransType = TransType;
                            Pt.TransDesc = TransDesc;
                            //TEST
                            Pt.CurrDesc = CurrDesc;
                            Pt.TranAmount = TranAmount;
                            Pt.ValueDate = DateTime.Now;
                            Pt.OpenRecord = true;

                            Pt.Operator = Operator;

                            Pt.InsertTran(PostedNo, Pt.Origin);

                            // Posted Second transaction 
                            //

                            Pt.TranToBePostedKey = PostedNo;
                            Pt.Origin = OriginName;
                            Pt.UserId = AuthUser;
                            Pt.AccNo = AccNo2;
                            Pt.AtmNo = AtmNo;
                            Pt.ReplCycle = SesNo;
                            Pt.BankId = BankId;

                            Pt.TranDtTime = DateTime.Now;
                            Pt.TransType = TransType2;
                            Pt.TransDesc = TransDesc2;
                            //TEST
                            Pt.CurrDesc = CurrDesc;
                            Pt.TranAmount = TranAmount;
                            Pt.ValueDate = DateTime.Now;
                            Pt.OpenRecord = true;

                            Pt.Operator = Operator;

                            Pt.InsertTran(PostedNo, Pt.Origin);

                            TotTransactions = TotTransactions + 2 ; 

                            // UPDATE Transtobe posted and close 

                            ActionDate = DateTime.Now;

                            GridFilterDate = DateTime.Today.Date; // DATE For Grid filter 

                            // From Trans To be posted Form 

                            if (InActionCd2 == 4)
                            {
                                // Close Transaction to be posted 

                                ActionCd2 = 4; 

                                ActionDate = DateTime.Now;

                                GridFilterDate = DateTime.Today.Date; // DATE For Grid filter 

                                OpenRecord = false; // Close Record 

                                UpdateTransToBePostedAction1(PostedNo, InSignedId, InActionCd2);
                            }
                            else
                            {
                                // Do not update 
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    //    log4net.Config.XmlConfigurator.Configure();
                    //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "UpdateTransToBePostedAction1");
                }
        }
        //
        // READ SPECIFIC TRANSACTION FROM IN POOL FOR Disputes  
        //

        public void ReadInPoolTransSpecificForDisputesTable(int InTranNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableDisputedTrans = new DataTable();
            TableDisputedTrans.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
       
            TableDisputedTrans.Columns.Add("Chosen", typeof(bool));
            TableDisputedTrans.Columns.Add("DisputedAmnt", typeof(decimal));
            TableDisputedTrans.Columns.Add("Card", typeof(string));
            TableDisputedTrans.Columns.Add("Account", typeof(string));
            TableDisputedTrans.Columns.Add("Curr", typeof(string));
            TableDisputedTrans.Columns.Add("Amount", typeof(string));
            TableDisputedTrans.Columns.Add("TransDate", typeof(DateTime));
            TableDisputedTrans.Columns.Add("TransDescr", typeof(string));
            TableDisputedTrans.Columns.Add("MaskRecordId", typeof(int));
            TableDisputedTrans.Columns.Add("RMCategory", typeof(string));
            TableDisputedTrans.Columns.Add("MatchingDt", typeof(string));


            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolTrans] "
          + " WHERE TranNo = @TranNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TranNo", InTranNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalSelected = TotalSelected + 1 ; 

                            TranNo = (int)rdr["TranNo"];
                            OriginName = (string)rdr["OriginName"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            EJournalTraceNo = (int)rdr["EJournalTraceNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            BankId = (string)rdr["BankId"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];
                            MaskRecordId = (int)rdr["MaskRecordId"];

                            SystemTarget = (int)rdr["SystemTarget"];

                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];
                            CardNo = (string)rdr["CardNo"];

                            CardOrigin = (int)rdr["CardOrigin"];
                            AccNo = (string)rdr["AccNo"];

                            CurrDesc = (string)rdr["CurrDesc"];

                            TranAmount = (decimal)rdr["TranAmount"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNumb = (int)rdr["RefNumb"];
                            RemNo = (int)rdr["RemNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            AtmMsg = (string)rdr["AtmMsg"];

                            ErrNo = (int)rdr["ErrNo"];

                            StartTrxn = (int)rdr["StartTrxn"];
                            EndTrxn = (int)rdr["EndTrxn"];

                            DepCount = (decimal)rdr["DepCount"];

                            CommissionCode = (int)rdr["CommissionCode"];
                            CommissionAmount = (decimal)rdr["CommissionAmount"];

                            SuccTran = (bool)rdr["SuccTran"];

                            Operator = (string)rdr["Operator"];

                            // Fill Table 

                            DataRow RowSelected = TableDisputedTrans.NewRow();
                           
                            RowSelected["Chosen"] = false;
                            RowSelected["DisputedAmnt"] = TranAmount;

                            RowSelected["Card"] = CardNo;
                            RowSelected["Account"] = AccNo;
                            RowSelected["Curr"] = CurrDesc;
                            RowSelected["Amount"] = TranAmount.ToString("#,##0.00");
                            RowSelected["TransDate"] = AtmDtTime;
                            RowSelected["TransDescr"] = TransDesc;
                            RowSelected["MaskRecordId"] = MaskRecordId;
                            RowSelected["RMCategory"] = RMCateg;
                            RowSelected["MatchingDt"] = AtmDtTime.ToString();

                            // ADD ROW
                            TableDisputedTrans.Rows.Add(RowSelected);

                        
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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    //  log4net.Config.XmlConfigurator.Configure();
                    //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "ReadTranSpecific");
                }
        }

        //
        // READ SPECIFIC TRANSACTION FROM IN POOL based on Number 
        //
        public void ReadInPoolTransSpecific(int InTranNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 



            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolTrans] "
          + " WHERE TranNo = @TranNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TranNo", InTranNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            TranNo = (int)rdr["TranNo"];
                            OriginName = (string)rdr["OriginName"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            EJournalTraceNo = (int)rdr["EJournalTraceNo"];
          
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            BankId = (string)rdr["BankId"];
             
                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];
                            MaskRecordId = (int)rdr["MaskRecordId"];

                            SystemTarget = (int)rdr["SystemTarget"];

                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];
                            CardNo = (string)rdr["CardNo"];

                            CardOrigin = (int)rdr["CardOrigin"];
                            AccNo = (string)rdr["AccNo"];

                            CurrDesc = (string)rdr["CurrDesc"];

                            TranAmount = (decimal)rdr["TranAmount"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNumb = (int)rdr["RefNumb"];
                            RemNo = (int)rdr["RemNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            AtmMsg = (string)rdr["AtmMsg"];

                            ErrNo = (int)rdr["ErrNo"];

                            StartTrxn = (int)rdr["StartTrxn"];
                            EndTrxn = (int)rdr["EndTrxn"];

                            DepCount = (decimal)rdr["DepCount"];

                            CommissionCode = (int)rdr["CommissionCode"];
                            CommissionAmount = (decimal)rdr["CommissionAmount"];

                            SuccTran = (bool)rdr["SuccTran"];

                            Operator = (string)rdr["Operator"];

                            // For Konto creation 

                            RRDMReconcMatchedUnMatchedVisaAuthorClass Rma = new RRDMReconcMatchedUnMatchedVisaAuthorClass(); 

                            RRDMReconcCategories Rc = new RRDMReconcCategories();

                            Rma.Operator = (string)rdr["BankID"];
                            Rma.CardNumber = (string)rdr["cardnum"];

                            //451174******6838    
                            if (Rma.CardNumber.Substring(0, 6) == "451174")
                            {
                                Rma.RMCateg = "EWB102";

                                Rc.ReadReconcCategorybyCategId(Rma.Operator, Rma.RMCateg);
                                Rma.Origin = Rc.Origin;
                                Rma.TransTypeAtOrigin = Rc.TransTypeAtOrigin;
                                Rma.Product = Rc.Product;
                                Rma.CostCentre = Rc.CostCentre;
                            }
                            else
                            {
                                Rma.RMCateg = "EWB103";

                                Rc.ReadReconcCategorybyCategId(Rma.Operator, Rma.RMCateg);
                                Rma.Origin = Rc.Origin;
                                Rma.TransTypeAtOrigin = Rc.TransTypeAtOrigin;
                                Rma.Product = Rc.Product;
                                Rma.CostCentre = Rc.CostCentre;
                            }

                            Rma.OriginFileName = "[ATMS_Journals].[dbo].[tblHstAtmTxns]";
                            Rma.OriginalRecordId = (int)rdr["TraceNumber"];

                            Rma.RMCycle = 2; // We think what to insert here 

                            Rma.TerminalId = (string)rdr["atmno"];
                            Rma.TransType = (int)rdr["TransactionType"]; // We are interested only the ones with 11 and 21  
                            Rma.TransDescr = (string)rdr["trandesc"];


                            Rma.AccNumber = (string)rdr["acct1"];
                            Rma.TransCurr = (string)rdr["currency"];
                            Rma.TransAmount = (decimal)rdr["camount"];


                            // TRANSACTION DATE Tm 
                            DateTime TRanDate2 = (DateTime)rdr["TRanDate"];

                            TimeSpan Time2 = (TimeSpan)rdr["trantime"];

                            TRanDate2 = TRanDate2.Add(Time2);

                            Rma.TransDate = TRanDate2;

                            Rma.AtmTraceNo = (int)rdr["TraceNumber"];

                            Rma.RRNumber = 0;

                            Rma.ResponseCode = 0;

                            Rma.T24RefNumber = " ";

                            Rma.OpenRecord = true;
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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                  //  log4net.Config.XmlConfigurator.Configure();
                  //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "ReadTranSpecific");
                }
        }
        //
        // READ SPECIFIC TRANSACTION FROM IN POOL for KONTO
        //
        public void ReadInPoolTransSpecificForKONTO()
        {
            RRDMReconcMatchedUnMatchedVisaAuthorClass Rma = new RRDMReconcMatchedUnMatchedVisaAuthorClass();

            RRDMReconcCategories Rc = new RRDMReconcCategories();


            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                           + " FROM [ATMS].[dbo].[InPoolTrans] "
                           + " WHERE TransType = 11 AND (AtmNo = 'AB102' OR AtmNo = 'AB104') ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@TranNo", InTranNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];
                            OriginName = (string)rdr["OriginName"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            EJournalTraceNo = (int)rdr["EJournalTraceNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            BankId = (string)rdr["BankId"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];
                            MaskRecordId = (int)rdr["MaskRecordId"];

                            SystemTarget = (int)rdr["SystemTarget"];

                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];
                            CardNo = (string)rdr["CardNo"];

                            CardOrigin = (int)rdr["CardOrigin"];
                            AccNo = (string)rdr["AccNo"];

                            CurrDesc = (string)rdr["CurrDesc"];

                            TranAmount = (decimal)rdr["TranAmount"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNumb = (int)rdr["RefNumb"];
                            RemNo = (int)rdr["RemNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            AtmMsg = (string)rdr["AtmMsg"];

                            ErrNo = (int)rdr["ErrNo"];

                            StartTrxn = (int)rdr["StartTrxn"];
                            EndTrxn = (int)rdr["EndTrxn"];

                            DepCount = (decimal)rdr["DepCount"];

                            CommissionCode = (int)rdr["CommissionCode"];
                            CommissionAmount = (decimal)rdr["CommissionAmount"];

                            SuccTran = (bool)rdr["SuccTran"];

                            Operator = (string)rdr["Operator"];



                            Rma.Operator = (string)rdr["Operator"];
                            Rma.CardNumber = (string)rdr["CardNo"];

                            //451174******6838    
                            if (Rma.CardNumber.Substring(0, 6) == "451174")
                            {
                                Rma.RMCateg = "EWB102";

                                Rc.ReadReconcCategorybyCategId(Rma.Operator, Rma.RMCateg);
                                Rma.Origin = Rc.Origin;
                                Rma.TransTypeAtOrigin = Rc.TransTypeAtOrigin;
                                Rma.Product = Rc.Product;
                                Rma.CostCentre = Rc.CostCentre;
                            }
                            else
                            {
                                Rma.RMCateg = "EWB103";

                                Rc.ReadReconcCategorybyCategId(Rma.Operator, Rma.RMCateg);
                                Rma.Origin = Rc.Origin;
                                Rma.TransTypeAtOrigin = Rc.TransTypeAtOrigin;
                                Rma.Product = Rc.Product;
                                Rma.CostCentre = Rc.CostCentre;

                            }


                            Rma.OriginFileName = "[ATMS].[dbo].[InPoolTrans]";
                            Rma.OriginalRecordId = (int)rdr["TranNo"];

                            Rma.RMCycle = (int)rdr["SesNo"]; // We think what to insert here 

                            Rma.TerminalId = (string)rdr["AtmNo"];
                            Rma.TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            Rma.TransDescr = (string)rdr["TransDesc"];


                            Rma.AccNumber = (string)rdr["AccNo"];

                            Rma.TransCurr = (string)rdr["CurrDesc"];
                            Rma.TransAmount = (decimal)rdr["TranAmount"];

                            Rma.TransDate = (DateTime)rdr["AtmDtTime"];

                            Rma.AtmTraceNo = (int)rdr["AtmTraceNo"];

                            Rma.RRNumber = 0;

                            Rma.ResponseCode = 0;

                            Rma.T24RefNumber = " ";

                            Rma.OpenRecord = true;

                            Rma.InsertMatchedORUnMatchedFileRecord("KontoPool"); 
                 
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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    //  log4net.Config.XmlConfigurator.Configure();
                    //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "ReadTranSpecific");
                }
        }

        //
        // READ SPECIFIC TRANSACTION Based on MaskRecordId
        //
        public void ReadInPoolAtmByMaskReordId(int InMaskRecordId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolTrans] "
          + " WHERE  MaskRecordId = @MaskRecordId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaskRecordId", InMaskRecordId);
                       
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];
                            OriginName = (string)rdr["OriginName"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            EJournalTraceNo = (int)rdr["EJournalTraceNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            BankId = (string)rdr["BankId"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];
                            MaskRecordId = (int)rdr["MaskRecordId"];

                            SystemTarget = (int)rdr["SystemTarget"];

                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];
                            CardNo = (string)rdr["CardNo"];

                            CardOrigin = (int)rdr["CardOrigin"];
                            AccNo = (string)rdr["AccNo"];

                            CurrDesc = (string)rdr["CurrDesc"];

                            TranAmount = (decimal)rdr["TranAmount"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNumb = (int)rdr["RefNumb"];
                            RemNo = (int)rdr["RemNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            AtmMsg = (string)rdr["AtmMsg"];

                            ErrNo = (int)rdr["ErrNo"];

                            StartTrxn = (int)rdr["StartTrxn"];
                            EndTrxn = (int)rdr["EndTrxn"];

                            DepCount = (decimal)rdr["DepCount"];

                            CommissionCode = (int)rdr["CommissionCode"];
                            CommissionAmount = (decimal)rdr["CommissionAmount"];

                            SuccTran = (bool)rdr["SuccTran"];

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    //     log4net.Config.XmlConfigurator.Configure();
                    //    RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                                "ReadInPoolAtmTrace");
                }
        }

        //
        // READ SPECIFIC TRANSACTION FROM IN POOL ATM Based on Trace No 
        //
        public void ReadInPoolAtmTrace(string InAtmNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolTrans] "
          + " WHERE AtmNo = @AtmNo AND AtmTraceNo = @AtmTraceNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", InTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];
                            OriginName = (string)rdr["OriginName"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            EJournalTraceNo = (int)rdr["EJournalTraceNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            BankId = (string)rdr["BankId"];
                   
                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];
                            MaskRecordId = (int)rdr["MaskRecordId"];

                            SystemTarget = (int)rdr["SystemTarget"];

                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];
                            CardNo = (string)rdr["CardNo"];

                            CardOrigin = (int)rdr["CardOrigin"];
                            AccNo = (string)rdr["AccNo"];

                            CurrDesc = (string)rdr["CurrDesc"];

                            TranAmount = (decimal)rdr["TranAmount"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNumb = (int)rdr["RefNumb"];
                            RemNo = (int)rdr["RemNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            AtmMsg = (string)rdr["AtmMsg"];

                            ErrNo = (int)rdr["ErrNo"];

                            StartTrxn = (int)rdr["StartTrxn"];
                            EndTrxn = (int)rdr["EndTrxn"];

                            DepCount = (decimal)rdr["DepCount"];

                            CommissionCode = (int)rdr["CommissionCode"];
                            CommissionAmount = (decimal)rdr["CommissionAmount"];

                            SuccTran = (bool)rdr["SuccTran"];

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
               //     log4net.Config.XmlConfigurator.Configure();
                //    RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                  //                                                "ReadInPoolAtmTrace");
                }
        }

        //
        // READ SPECIFIC TRANSACTION FROM IN POOL Host Based on Trace No 
        //
        public void ReadInPoolHostTrace(string InAtmNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolHost] "
          + " WHERE AtmNoH = @AtmNoH AND AtmTraceNoH = @AtmTraceNoH";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmTraceNoH", InTraceNo);
                        cmd.Parameters.AddWithValue("@AtmNoH", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TranNoH = (int)rdr["TranNoH"];
                            TranOriginH = (int)rdr["TranOriginH"];
                            AtmTraceNoH = (int)rdr["AtmTraceNoH"];
                            HostTraceNoH = (int)rdr["HostTraceNoH"];

                            HostDtTimeH = (DateTime)rdr["HostDtTimeH"];

                            SystemTarget = (int)rdr["SystemTarget"];

                            TransDescH = (string)rdr["TransDescH"];
                            CardNoH = (string)rdr["CardNoH"];

                            AccNoH = (string)rdr["AccNoH"];

                            AuthCodeH = (int)rdr["AuthCodeH"];
                            RefNumbH = (int)rdr["RefNumbH"];
                            RemNoH = (int)rdr["RemNoH"];

                            TransMsgH = (string)rdr["TransMsgH"];

                            ErrNoH = (int)rdr["ErrNoH"];

                            SuccTranH = (bool)rdr["SuccTranH"];

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    //   log4net.Config.XmlConfigurator.Configure();
                    //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                               "UpdateTransToBePostedAction1");
                }
        }
        //
        // UPDATE Transaction for Deposit 
        // 
        public void UpdateTransforDep(int InTranNo)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[InPoolTrans] SET "
                            + " AtmMsg = @AtmMsg,DepCount = @DepCount"
                            + " WHERE TranNo = @TranNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@TranNo", TranNo);
                        cmd.Parameters.AddWithValue("@AtmMsg", AtmMsg);
                        cmd.Parameters.AddWithValue("@DepCount", DepCount);

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                 //   log4net.Config.XmlConfigurator.Configure();
                  //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                  //                                                "UpdateTransforDep");
                }
        }
        //
        // READ TRANSACTIONs For creating the Dispensed History Records  
        //
        public void ReadUpdateTransForDispensedHistory(string InOperator, string InAtmNo, DateTime FromDate, DateTime ToDate) 
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            int DrTrans = 0; 
            decimal DispensedAmt = 0;
            int CrTrans = 0;
            decimal DepAmt = 0; 
            bool First = true;

            int[] NumberOfTrans = new int[999];
            decimal[] AmountPerType = new decimal[999];

            DateTime PreviousDt = new DateTime(1900, 01, 01);

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolTrans] "
          + " WHERE AtmNo = @AtmNo AND AtmDtTime>= @FromDate AND  AtmDtTime<= @ToDate";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];

                            if (First == true)
                            {
                                PreviousDt = AtmDtTime;
                                First = false;
                            }

                            TransType = (int)rdr["TransType"];

                            //       CurrCode = (int)rdr["CurrCode"];
                            CurrDesc = (string)rdr["CurrDesc"];
                            TranAmount = (decimal)rdr["TranAmount"];

                            CommissionCode = (int)rdr["CommissionCode"];

                            CommissionAmount = (decimal)rdr["CommissionAmount"];

                            if (CommissionCode > 0)
                            {
                                // Create totals for commission records
                                NumberOfTrans[CommissionCode] = NumberOfTrans[CommissionCode] + 1;
                                AmountPerType[CommissionCode] = AmountPerType[CommissionCode] + CommissionAmount;
                            }

                            if (TransType == 11)
                            {
                                RecordFound = true;
                                DrTrans = DrTrans + 1;
                                DispensedAmt = DispensedAmt + TranAmount;
                                // Create totals for turnover 
                                NumberOfTrans[TransType] = NumberOfTrans[TransType] + 1;
                                AmountPerType[TransType] = AmountPerType[TransType] + TranAmount;
                            }

                            if (TransType == 23 || TransType == 24 || TransType == 25)
                            {
                                CrTrans = CrTrans + 1;
                                DepAmt = DepAmt + TranAmount;
                                NumberOfTrans[TransType] = NumberOfTrans[TransType] + 1;
                                AmountPerType[TransType] = AmountPerType[TransType] + TranAmount;
                            }

                            if (AtmDtTime.Date != PreviousDt.Date & DispensedAmt > 0)
                            {
                                // Insert Dispensed History transaction 

                                Am.ReadAtmsMainSpecific(InAtmNo);// READ MAIN 

                                Ah.AtmNo = InAtmNo;
                                Ah.BankId = Am.BankId;
                                Ah.DtTm = PreviousDt.Date;
                                Ah.Year = PreviousDt.Year;
                             //   Ah.Prive = Am.Prive;
                                Ah.DrTransactions = DrTrans;
                                Ah.DispensedAmt = DispensedAmt;
                                Ah.PreEstimated = 0;
                                Ah.CrTransactions = CrTrans;
                                Ah.DepAmount = DepAmt;


                                // Update Array values for other cost parameters 

                                // Annual Maintenance divided daily 
                                Ap.ReadTableATMsCostSpecific(InAtmNo);

                                decimal YearMaintenance = Ap.AnnualMaint;
                                Ho.GetDaysInAYear(DateTime.Now.Year);
                                Ah.C301DailyMaintAmount = YearMaintenance / Ho.daysInYear;

                                NumberOfTrans[301] = 1;
                                AmountPerType[301] = Ah.C301DailyMaintAmount;

                                // Annual Cost of Overhead Divided Daily 
                                
                                ParamId = "370";
                                OccuranceId = "325"; // find annual 
                                RelatedParmId = "" ;
                                RelatedOccuranceId = "" ;

                                Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);
                          //      Ds.ReadParametersSpecificNo(325); // find annual 
                                Ah.C307OverheadCost = Gp.Amount / Ho.daysInYear;

                                NumberOfTrans[307] = 1;
                                AmountPerType[307] = Ah.C307OverheadCost;

                                // Cost of investement Divided Daily 
                                Gp.ReadParametersSpecificId(InOperator, "500", "1", "", "");
                                int LastingYears = (int)Gp.Amount;

                                Ah.C309CostOfInvest = Ap.PurchaseCost / (LastingYears * 365);

                                NumberOfTrans[309] = 1;
                                AmountPerType[309] = Ah.C309CostOfInvest;


                                // Replenishment cost 

                                Rs.ReadReplStatClassSpecificDt(Ah.AtmNo, Ah.DtTm); // Get Total Repl Time and other fields 

                                if (Rs.RecordFound == true)
                                {
                                    NumberOfTrans[303] = 1;

                              //      Ds.ReadParametersSpecificNo(324); // find employee cost per minute 


                                    ParamId = "370";
                                    OccuranceId = "324"; 
                                    RelatedParmId = "";
                                    RelatedOccuranceId = "";

                                    // find employee cost per minute 

                                    Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);

                                    AmountPerType[303] = Rs.TotalReplMinutes * Gp.Amount;

                                    Ah.C303ReplTimeCost = Rs.TotalReplMinutes * Gp.Amount;

                                    // Cost of Money 
                                    // Find Repl No and Money in 
                                    // Find days for money in 
                                    // Find cost of money based on days

                                    Ta.ReadSessionsStatusTraces(Ah.AtmNo, Rs.ReplCycleNo);

                                    TimeSpan Diff = Ta.SesDtTimeEnd - Ta.SesDtTimeStart;
                                    string NumberDays1 = Diff.TotalHours.ToString();
                                    int NumberHours = Convert.ToInt32(Diff.TotalHours);

                                    // Ds.ReadParametersSpecificNo(322); // find cost of money

                                    ParamId = "370";
                                    OccuranceId = "322";
                                    RelatedParmId = "";
                                    RelatedOccuranceId = "";

                                    // find cost of money

                                    Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);

                                    NumberOfTrans[308] = NumberHours / 24;

                                    AmountPerType[308] = Rs.InMoneyLast * NumberHours * Gp.Amount / (Ho.daysInYear * 24);

                                    Ah.C308CostOfMoney = Rs.InMoneyLast * NumberHours * Gp.Amount / (Ho.daysInYear * 24);
                                }



                                /* Create Stats records  */
                                for (int i = 0; i < 999; i++)
                                {
                                    if (NumberOfTrans[i] > 0 || AmountPerType[i] > 0)
                                    {
                                        if (i == 401)
                                        {
                                            Ah.R401CommTran = NumberOfTrans[i];
                                            Ah.R401CommAmount = AmountPerType[i];
                                        }

                                        if (i == 402)
                                        {
                                            Ah.R402CommTran = NumberOfTrans[i];
                                            Ah.R402CommAmount = AmountPerType[i];
                                        }

                                        if (i == 403)
                                        {
                                            Ah.R403CommTran = NumberOfTrans[i];
                                            Ah.R403CommAmount = AmountPerType[i];
                                        }

                                        if (i == 404)
                                        {
                                            Ah.R404CommTran = NumberOfTrans[i];
                                            Ah.R404CommAmount = AmountPerType[i];
                                        }

                                        if (i == 405)
                                        {
                                            Ah.R405CommTran = NumberOfTrans[i];
                                            Ah.R405CommAmount = AmountPerType[i];
                                        }

                                        Ah.AtmNo = InAtmNo;
                                        Ah.BankId = Am.BankId;
                                   //     Ah.Prive = Am.Prive;
                                        Ah.DtTm = PreviousDt.Date;
                                        Ah.RecordType = i;

                                        // Read and assign description 
                                  //      Ds.ReadParametersSpecificNo(i);
                                     
                                        ParamId = "370";
                                        OccuranceId = i.ToString(); 
                                        RelatedParmId = "";
                                        RelatedOccuranceId = "";

                                        Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);
                                        
                                        Ah.Description = Gp.OccuranceNm;

                                        Ah.NumberOfTrans = NumberOfTrans[i];
                                        Ah.Amount = AmountPerType[i];
                                        Ah.DateCreated = DateTime.Now;

                                        Am.ReadAtmsMainSpecific(Ah.AtmNo);
                                        Ah.Operator = Am.Operator; 

                                        Ah.InsertTransHistoryByType(AtmNo, PreviousDt);
                                        // Initilise 
                                        NumberOfTrans[i] = 0;
                                        AmountPerType[i] = 0;
                                    }
                                }

                                Ah.Operator = Am.Operator; 
                                // Create Record 
                                Ah.InsertTransHistory(AtmNo, PreviousDt, DispensedAmt);

                                // Initialised amounts 
                                DrTrans = 0;
                                DispensedAmt = 0;
                                CrTrans = 0;
                                DepAmt = 0;


                            }

                            PreviousDt = AtmDtTime;

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    if (DispensedAmt > 0) // INSERT LAST DATE OF RDR LOOP 
                    {

                        // Insert Dispensed History transaction 

                        Am.ReadAtmsMainSpecific(InAtmNo);// READ MAIN 

                        Ah.AtmNo = InAtmNo;
                        Ah.BankId = Am.BankId;
                        Ah.DtTm = PreviousDt.Date;
                        Ah.Year = PreviousDt.Year;
                 
                        Ah.DrTransactions = DrTrans;
                        Ah.DispensedAmt = DispensedAmt;
                        Ah.PreEstimated = 0;
                        Ah.CrTransactions = CrTrans;
                        Ah.DepAmount = DepAmt;


                        // Update Array values for other cost parameters 

                        // Annual Maintenance divided daily 
                        Ap.ReadTableATMsCostSpecific(InAtmNo);

                        decimal YearMaintenance = Ap.AnnualMaint;
                        Ho.GetDaysInAYear(DateTime.Now.Year);
                        Ah.C301DailyMaintAmount = YearMaintenance / Ho.daysInYear;

                        NumberOfTrans[301] = 1;
                        AmountPerType[301] = Ah.C301DailyMaintAmount;

                        // Annual Cost of Overhead Divided Daily 
                      //  Ds.ReadParametersSpecificNo(325); 
                        
                        // find annual 
                        ParamId = "370";
                        OccuranceId = "325";
                        RelatedParmId = "";
                        RelatedOccuranceId = "";

                        Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);

                        Ah.C307OverheadCost = Gp.Amount / Ho.daysInYear;

                        NumberOfTrans[307] = 1;
                        AmountPerType[307] = Ah.C307OverheadCost;

                        // Cost of investement Divided Daily 
                        Gp.ReadParametersSpecificId(InOperator, "500", "1", "", "");
                        int LastingYears = (int)Gp.Amount;

                        Ah.C309CostOfInvest = Ap.PurchaseCost / (LastingYears * 365);

                        NumberOfTrans[309] = 1;
                        AmountPerType[309] = Ah.C309CostOfInvest;

                        // Replenishment cost 

                        Rs.ReadReplStatClassSpecificDt(Ah.AtmNo, Ah.DtTm); // Get Total Repl Time and other fields 

                        if (Rs.RecordFound == true)
                        {
                            NumberOfTrans[303] = 1;

                            // Ds.ReadParametersSpecificNo(324); // find employee cost per minute

                            // find employee cost per minute
                            ParamId = "370";
                            OccuranceId = "324";
                            RelatedParmId = "";
                            RelatedOccuranceId = "";

                            Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);

                            AmountPerType[303] = Rs.TotalReplMinutes * Gp.Amount;

                            Ah.C303ReplTimeCost = Rs.TotalReplMinutes * Gp.Amount;

                            // Cost of Money 
                            // Find Repl No and Money in 
                            // Find days for money in 
                            // Find cost of money based on days

                            Ta.ReadSessionsStatusTraces(Ah.AtmNo, Rs.ReplCycleNo);

                            TimeSpan Diff = Ta.SesDtTimeEnd - Ta.SesDtTimeStart;
                            string NumberDays1 = Diff.TotalHours.ToString();
                            int NumberHours = Convert.ToInt32(Diff.TotalHours);


                          //  Ds.ReadParametersSpecificNo(322); // find cost of money

                            // find cost of money
                            ParamId = "370";
                            OccuranceId = "322";
                            RelatedParmId = "";
                            RelatedOccuranceId = "";

                            Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);


                            NumberOfTrans[308] = NumberHours / 24;

                            AmountPerType[308] = Rs.InMoneyLast * NumberHours * Gp.Amount / (Ho.daysInYear * 24);

                            Ah.C308CostOfMoney = Rs.InMoneyLast * NumberHours * Gp.Amount / (Ho.daysInYear * 24);

                        }

                        /* Create Stats records  */
                        for (int i = 0; i < 999; i++)
                        {
                            if (NumberOfTrans[i] > 0 || AmountPerType[i] > 0)
                            {
                                if (i == 401)
                                {
                                    Ah.R401CommTran = NumberOfTrans[i];
                                    Ah.R401CommAmount = AmountPerType[i];
                                }

                                if (i == 402)
                                {
                                    Ah.R402CommTran = NumberOfTrans[i];
                                    Ah.R402CommAmount = AmountPerType[i];
                                }

                                if (i == 403)
                                {
                                    Ah.R403CommTran = NumberOfTrans[i];
                                    Ah.R403CommAmount = AmountPerType[i];
                                }

                                if (i == 404)
                                {
                                    Ah.R404CommTran = NumberOfTrans[i];
                                    Ah.R404CommAmount = AmountPerType[i];
                                }

                                if (i == 405)
                                {
                                    Ah.R405CommTran = NumberOfTrans[i];
                                    Ah.R405CommAmount = AmountPerType[i];
                                }

                                Ah.AtmNo = InAtmNo;
                                Ah.BankId = Am.BankId;
                           //     Ah.Prive = Am.Prive;
                                Ah.DtTm = PreviousDt.Date;
                                Ah.RecordType = i;

                                // Read and assign description 

                             //   Ds.ReadParametersSpecificNo(i);

                                ParamId = "370";
                                OccuranceId = i.ToString(); 
                                RelatedParmId = "";
                                RelatedOccuranceId = "";

                                Gp.ReadParametersSpecificId(InOperator, ParamId, OccuranceId, RelatedParmId, RelatedOccuranceId);
                                Ah.Description = Gp.OccuranceNm;

                                Ah.NumberOfTrans = NumberOfTrans[i];
                                Ah.Amount = AmountPerType[i];
                                Ah.DateCreated = DateTime.Now;

                                Ah.InsertTransHistoryByType(AtmNo, PreviousDt);
                                // Initilise 
                                NumberOfTrans[i] = 0;
                                AmountPerType[i] = 0;
                            }
                        }
                        // Create Record 
                        Ah.InsertTransHistory(InAtmNo, PreviousDt, DispensedAmt);

                        // Initialised amounts 
                        DrTrans = 0;
                        DispensedAmt = 0;
                        CrTrans = 0;
                        DepAmt = 0;

                    }

                }
                catch (Exception ex)
                {

                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    // log4net.Config.XmlConfigurator.Configure();
                    //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                             "ReadTransForDispensedHistory");

                }
        }

        //
        // READ  TRANS FOR A CARD 
        //
        public void ReadTransForCard(string InCardNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolTrans] "
          + " WHERE CardNo = @CardNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CardNo", InCardNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            TranNo = (int)rdr["TranNo"];
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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                 //   log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                  //                                                "ReadTransForCard");
                }
        }

        //
        // READ TRANS No by Using Trace No 
        //
        public void ReadTranForTrace(string InAtmNo,int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[InPoolTrans] "
          + " WHERE AtmNo = @AtmNo AND AtmTraceNo = @AtmTraceNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", InTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            TranNo = (int)rdr["TranNo"];

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                 //   log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                      //                                            "ReadTranForTrace");
                }
        }

        // UPDATE TRANSACTION ERROR NUMBER 
        // 
        public void UpdateTransErrNo(int InTranNo, int InErrNo)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[InPoolTrans] SET "
                            + " ErrNo = @ErrNo"
                            + " WHERE TranNo = @TranNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@TranNo", InTranNo);
                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                  //  log4net.Config.XmlConfigurator.Configure();
                  //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                     //                                             "UpdateTransErrNo");
                }
        }

        // Insert TRANS FROM EJOURNAL TO IN POOLTO BE UPDATED 
        //
        public void InsertTransInPool(string InAtmNo)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[InPoolTrans]"
                + " ([OriginName] ,[AtmTraceNo] ,[EJournalTraceNo] ,"
                + "[AtmNo] ,[SesNo] ,[BankId] ,[BranchId] ,"
                + "[AtmDtTime], "
                + "[SystemTarget] ,[TransType] ,[TransDesc] ,[CardNo] ,[CardOrigin],"
                + "[AccNo] ,[CurrDesc] ,[TranAmount] ,[AuthCode] ,[RefNumb],[RemNo],"
                + "[TransMsg] ,[AtmMsg] ,[ErrNo] ,[StartTrxn] ,[EndTrxn] ,[DepCount] ,"
                + "[CommissionCode],[CommissionAmount],[SuccTran],[Operator] )"
                + " VALUES "
                 + " (@OriginName ,@AtmTraceNo ,@EJournalTraceNo ,"
                + "@AtmNo ,@SesNo ,@BankId ,@BranchId ,"
                + "@AtmDtTime, "
                + "@SystemTarget ,@TransType ,@TransDesc ,@CardNo ,@CardOrigin,"
                + "@AccNo ,@CurrDesc ,@TranAmount ,@AuthCode ,@RefNumb,@RemNo ,"
                + "@TransMsg,@AtmMsg,@ErrNo ,@StartTrxn ,@EndTrxn,@DepCount ,"
                + "@CommissionCode,@CommissionAmount,@SuccTran,@Operator)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@OriginName", OriginName);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);
                        cmd.Parameters.AddWithValue("@EJournalTraceNo", EJournalTraceNo);
                     
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", SesNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                    
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                       
                        cmd.Parameters.AddWithValue("@AtmDtTime", AtmDtTime);
                        
                        cmd.Parameters.AddWithValue("@SystemTarget", SystemTarget);
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDesc", TransDesc);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@CardOrigin", CardOrigin);
                        
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                  
                        cmd.Parameters.AddWithValue("@CurrDesc", CurrDesc);
                        cmd.Parameters.AddWithValue("@TranAmount", TranAmount);
                        cmd.Parameters.AddWithValue("@AuthCode", AuthCode);
                        cmd.Parameters.AddWithValue("@RefNumb", RefNumb);
                        cmd.Parameters.AddWithValue("@RemNo", RemNo);

                        cmd.Parameters.AddWithValue("@TransMsg", TransMsg);
                        cmd.Parameters.AddWithValue("@AtmMsg", AtmMsg);
                        cmd.Parameters.AddWithValue("@ErrNo", ErrNo);
                        cmd.Parameters.AddWithValue("@StartTrxn", StartTrxn);
                        cmd.Parameters.AddWithValue("@EndTrxn", EndTrxn);

                        cmd.Parameters.AddWithValue("@DepCount", DepCount);

                        cmd.Parameters.AddWithValue("@CommissionCode", CommissionCode);
                        cmd.Parameters.AddWithValue("@CommissionAmount", CommissionAmount);
                      
                        cmd.Parameters.AddWithValue("@SuccTran", SuccTran);

                        Am.ReadAtmsMainSpecific(InAtmNo); 

                        cmd.Parameters.AddWithValue("@Operator", Am.Operator);

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
               //     log4net.Config.XmlConfigurator.Configure();
                //    RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "InsertTransInPool");
                }
        }

        // Insert NEW TRANS TO BE READY FOR UPDATING  
        //
        public void InsertTransToBePosted(string InAtmNo, int InErrNo, string InOriginName)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 
       
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[TransToBePosted]"
                + " ([OriginId] ,[OriginName] ,[RMCateg] ,[RMCategCycle] ," 
                + "[MaskRecordId] ,[ErrNo] ,[AtmNo] ,[SesNo] , [DisputeNo] , [DispTranNo] , [BankId] ,"
                + "[AtmTraceNo] ,[BranchId] ,[AtmDtTime],"
                + "[SystemTarget] ,[CardNo] ,[CardOrigin],"
                + "[TransType] ,[TransDesc],[AccNo],[GlEntry],"
                + "[TransType2] ,[TransDesc2],[AccNo2],[GlEntry2],"
                + "[CurrDesc] ,[TranAmount] ,[AuthCode] ,[RefNumb],"
                + "[RemNo] ,[TransMsg] ,[AtmMsg] , [AuthUser] ,[OpenDate] ,[OpenRecord],[Operator] )"
                + " VALUES "
                 + " (@OriginId ,@OriginName ,@RMCateg ,@RMCategCycle ,"
                 +"@MaskRecordId ,@ErrNo ,@AtmNo ,@SesNo , @DisputeNo, @DispTranNo ,@BankId ,"
                + "@AtmTraceNo ,@BranchId ,@AtmDtTime,"
                + "@SystemTarget ,@CardNo ,@CardOrigin,"
                + " @TransType ,@TransDesc , @AccNo , @GlEntry, "
                 + " @TransType2 ,@TransDesc2 , @AccNo2 , @GlEntry2 , "
                + "@CurrDesc ,@TranAmount ,@AuthCode ,@RefNumb,"
                + "@RemNo ,@TransMsg,@AtmMsg,@AuthUser,@OpenDate,@OpenRecord,@Operator )"; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginId", OriginId);
                        cmd.Parameters.AddWithValue("@OriginName", InOriginName);
                        cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
                        cmd.Parameters.AddWithValue("@RMCategCycle", RMCategCycle);

                        cmd.Parameters.AddWithValue("@MaskRecordId", MaskRecordId); 
                        cmd.Parameters.AddWithValue("@ErrNo", ErrNo);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", SesNo);

                        cmd.Parameters.AddWithValue("@DisputeNo", DisputeNo);
                        cmd.Parameters.AddWithValue("@DispTranNo", DispTranNo);

                        cmd.Parameters.AddWithValue("@BankId", BankId);
                       
                        cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@AtmDtTime", AtmDtTime);

                        cmd.Parameters.AddWithValue("@SystemTarget", SystemTarget);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@CardOrigin", CardOrigin);

                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDesc", TransDesc);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@GlEntry", GlEntry);

                        cmd.Parameters.AddWithValue("@TransType2", TransType2);
                        cmd.Parameters.AddWithValue("@TransDesc2", TransDesc2);
                        cmd.Parameters.AddWithValue("@AccNo2", AccNo2);
                        cmd.Parameters.AddWithValue("@GlEntry2", GlEntry2);
             
                        cmd.Parameters.AddWithValue("@CurrDesc", CurrDesc);
                        cmd.Parameters.AddWithValue("@TranAmount", TranAmount);

                        cmd.Parameters.AddWithValue("@AuthCode",AuthCode);
                        cmd.Parameters.AddWithValue("@RefNumb", RefNumb);

                        cmd.Parameters.AddWithValue("@RemNo", RemNo);
                        cmd.Parameters.AddWithValue("@TransMsg", TransMsg);
                        cmd.Parameters.AddWithValue("@AtmMsg", AtmMsg);
                        cmd.Parameters.AddWithValue("@AuthUser", AuthUser);
                        cmd.Parameters.AddWithValue("@OpenDate", OpenDate);
                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        
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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                 //   log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                  //                                                "InsertTransToBePosted");
                }
        }

        // DELETE OLD TRANS TO BE POSTED 
        // 
        public void DeleteOldTransToBePosted(string InAtmNo, int InErrNo)
        {
            // DELETE ACTION If Already taken   
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[TransToBePosted] "
                            + " WHERE ErrNo = @ErrNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    // MessageBox.Show(exception);
                 
                }

        }

        // CLOSE TRANS TO BE POSTED
        // 
        public void UpdateTransToBePostedClose(int InPostedNo)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[TransToBePosted] SET "
                            + " OpenRecord = @OpenRecord "
                            + " WHERE PostedNo = @PostedNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@PostedNo", InPostedNo);
                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                 //   log4net.Config.XmlConfigurator.Configure();
                  //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                  //                                                "UpdateTransToBePostedClose");

                }
        }

        // Initialize Trans to be posted Auth User for this User
        // Make all Auth User = 0 where Signed User = '500' say
        // UPDATE AUTHORISED USER ON TRANSCTION TO BE POSTED 
        // 
        public void ClearTransToBePostedAuthUser(string InAuthUser)
        {
            
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[TransToBePosted] SET "
                            + " AuthUser = '' "
                            + " WHERE AuthUser = @AuthUser ", conn))

                    {
                        cmd.Parameters.AddWithValue("@AuthUser", InAuthUser);

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    //    log4net.Config.XmlConfigurator.Configure();
                    //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "UpdateTransToBePostedAuthUser");
                }
        }



        // UPDATE AUTHORISED USER ON TRANSCTION TO BE POSTED 
        // 
        public void UpdateTransToBePostedAuthUser(string InAtmNo, string InAuthUser)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[TransToBePosted] SET "
                            + " AuthUser = @AuthUser "
                            + " WHERE AtmNo = @AtmNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AuthUser", InAuthUser);

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                //    log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "UpdateTransToBePostedAuthUser");
                }
        }

        // UPDATE ACTION ON TRANSCTION TO BE POSTED based on Tran number 
        // USED FOR MATCHED DATES TOO
        // Close it too 
        public void UpdateTransToBePostedAction1(int InTranNumber, string InUser, int InActionCode )
        {
        
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[TransToBePosted] SET "
                            + " ActionBy = @ActionBy,  ActionCd2 = @ActionCd2 , ActionDate = @ActionDate, "
                            + " GridFilterDate = @GridFilterDate,OpenRecord = @OpenRecord   "
                             + " WHERE PostedNo = @PostedNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@PostedNo", InTranNumber);
                        cmd.Parameters.AddWithValue("@ActionBy", InUser);
                        cmd.Parameters.AddWithValue("@ActionCd2", InActionCode);
                        cmd.Parameters.AddWithValue("@ActionDate", ActionDate);
                        //cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        //cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@GridFilterDate", GridFilterDate);
                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                 //   log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                   //                                               "UpdateTransToBePostedAction1");
                }
        }
        

      //
        // USED FOR MATCHED DATES 
        //
        public void UpdateTransToBePostedMatched(string InAtmNo, int InPostedNo, bool InHostMatched, 
                                                  DateTime InMatchedDtTm, int InReconciled)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[TransToBePosted] SET "
                            + " HostMatched = @HostMatched,  MatchedDtTm = @MatchedDtTm , Reconciled = @Reconciled "
                             + " WHERE AtmNo = @AtmNo AND PostedNo = @PostedNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@PostedNo", InPostedNo);
                        cmd.Parameters.AddWithValue("@HostMatched", InHostMatched);
                        cmd.Parameters.AddWithValue("@MatchedDtTm", InMatchedDtTm);
                        cmd.Parameters.AddWithValue("@Reconciled", InReconciled);


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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    //   log4net.Config.XmlConfigurator.Configure();
                    //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                               "UpdateTransToBePostedAction1");
                }
        }

        // Read to find actions finalised and not Finalised TOTAls Based on AtmNo And SesNo
        // 

        public void ReadTransToBePostedTotals(string InAtmNo, int InSesNo)
        {
            TotActions = 0;
            TotActionsTaken = 0;
            TotActionsNotTaken = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[TransToBePosted] "
          + " WHERE AtmNo = @AtmNo AND SesNo =@SesNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            PostedNo = (int)rdr["PostedNo"];

                            OriginId = (string)rdr["OriginId"];
                            OriginName = (string)rdr["OriginName"];
                            RMCateg = (string)rdr["RMCateg"];
                            RMCategCycle = (int)rdr["RMCategCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            ErrNo = (int)rdr["ErrNo"];
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];

                            DisputeNo = (int)rdr["DisputeNo"];
                            DispTranNo = (int)rdr["DispTranNo"];

                            BankId = (string)rdr["BankId"];
                     
                            AtmTraceNo = (int)rdr["AtmTraceNo"];

                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];

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

                     //       OriginatorUser = (string)rdr["OriginatorUser"];
                            AuthUser = (string)rdr["AuthUser"];
                            ActionBy = (string)rdr["ActionBy"];

                            ActionCd2 = (int)rdr["ActionCd2"];

                            ActionDate = (DateTime)rdr["ActionDate"];

                            OpenDate = (DateTime)rdr["OpenDate"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            HostMatched = (bool)rdr["HostMatched"];
                            MatchedDtTm = (DateTime)rdr["MatchedDtTm"];
                            Reconciled = (int)rdr["Reconciled"];

                            GridFilterDate = (DateTime)rdr["GridFilterDate"];

                            Operator = (string)rdr["Operator"];

                           
                            // All Actions 
                            
                                TotActions = TotActions + 1; 
                            

                            if (ActionCd2 == 1 ) // Action  taken
                            {
                                TotActionsTaken = TotActionsTaken + 1; 
                            }
                            if (ActionCd2 != 1 ) // Action Not Taken 
                            {
                                TotActionsNotTaken = TotActionsNotTaken + 1;
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                 //   log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "ReadTransToBePosted");
                }
        }

// Copy transactions for testing purpose
        // read and Copy from one ATM to another 

        //
        // READ all transactions based on Criteria  
        //
        public void CopyInPoolTrans(string InBankId, string InAtmNo, int InSesNo ,string TargetBank, string TargetAtm, 
                                  int TargetSesNo,  bool TargetPrive)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *" 
          + " FROM [ATMS].[dbo].[InPoolTrans] "
          + " WHERE BankId = @BankId AND AtmNo = @AtmNo AND SesNo = @SesNo ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo); 

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            TranNo = (int)rdr["TranNo"];
                            OriginName = (string)rdr["OriginName"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            EJournalTraceNo = (int)rdr["EJournalTraceNo"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            BankId = (string)rdr["BankId"];
                 
                            BranchId = (string)rdr["BranchId"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];
                            MaskRecordId = (int)rdr["MaskRecordId"];

                            SystemTarget = (int)rdr["SystemTarget"];

                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];
                            CardNo = (string)rdr["CardNo"];

                            CardOrigin = (int)rdr["CardOrigin"];
                            AccNo = (string)rdr["AccNo"];

                            //            CurrCode = (int)rdr["CurrCode"];
                            CurrDesc = (string)rdr["CurrDesc"];

                            TranAmount = (decimal)rdr["TranAmount"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNumb = (int)rdr["RefNumb"];
                            RemNo = (int)rdr["RemNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            AtmMsg = (string)rdr["AtmMsg"];

                            ErrNo = (int)rdr["ErrNo"];

                            StartTrxn = (int)rdr["StartTrxn"];
                            EndTrxn = (int)rdr["EndTrxn"];

                            DepCount = (decimal)rdr["DepCount"];

                            CommissionCode = (int)rdr["CommissionCode"];
                            CommissionAmount = (decimal)rdr["CommissionAmount"];

                            SuccTran = (bool)rdr["SuccTran"];

                            Operator = (string)rdr["Operator"];

                            // Insert / Copy transaction 

                            BankId = TargetBank;
                       //     Prive = TargetPrive; 
                            AtmNo = TargetAtm;
                            SesNo = TargetSesNo;
                            CurrDesc = "GBP"; 

                            InsertTransInPool(AtmNo); 

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
                    ErrorOutput = "An error occured in TransAndPosted Class............. " + ex.Message;
                    //  log4net.Config.XmlConfigurator.Configure();
                    //  RRDM4ATMsWin.Log.ProcessException(ex, "TransClass.cs",
                    //                                              "ReadTranSpecific");
                }
        }


    }
}
