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
using System.Collections;
using System.IO;
namespace RRDM4ATMs
{
    public class RRDMReconcMatchedUnMatchedVisaAuthorClass
    {
        public int SeqNo;

        public string OriginFileName; //* 

        public int OriginalRecordId; 

        public string RMCateg;
        public int RMCycle;

        //MaskRecordId
        //UniqueRecordId
        public int MaskRecordId;
        public int UniqueRecordId; 

        public string Origin; //
        public string TransTypeAtOrigin;//
        public string Product;//
        public string CostCentre;//

        public string TerminalId;
        public int TransType;

        public string TransDescr;
        public string CardNumber;
        public string AccNumber;

        public string TransCurr;
        public decimal TransAmount;

        public DateTime TransDate;

        public int AtmTraceNo;
        public int RRNumber;
        public int ResponseCode;
        public string T24RefNumber;

        public bool Matched;
        public string MatchMask;
        public DateTime SystemMatchingDtTm;

        public string MatchedType; // USED FOR MATCHED TRANSACTIONS 

        public string UnMatchedType; // USED FOR UN-MATCHED TRANSACTIONS 

        public int MetaExceptionId;

        public int MetaExceptionNo; 

        public bool ActionByUser;

        public string UserId;
        public string Authoriser;

        public DateTime AuthoriserDtTm;

        public string ActionType; // 1 ... Meta Exception Default 
                                  // 2 ... Meta Exception Manual 
                                  // 3 ... Match it with other 
                                  // 4 ... Postpone 
                                  // 5 ... Close it 

        public bool RemainsForMatching; // USED FOR UNMATCHED VISA AUTHORIZATIONS 
        public bool WaitingForUpdating; // USED FOR MATCHED AUTHORISATIONS 

        public bool OpenRecord; // Currently only for UnMatched 

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public int TotalUnMatched; 
        public decimal TotalAmountUnMatched ; 

        RRDMReconcCategoriesMatchingSessions Rms = new RRDMReconcCategoriesMatchingSessions();

        RRDMReconcMasksVsMetaExceptions Rme = new RRDMReconcMasksVsMetaExceptions();

        // Define the data table 
        //public DataTable RMRecords = new DataTable();

        //public DataTable RMRecordsRRNa = new DataTable();

        //public DataTable RMRecordsRRNb = new DataTable();

        //public DataTable RMRecordsRRNc = new DataTable();

        //public DataTable RMUnmathed = new DataTable();

        public DataTable RMDataTableLeft = new DataTable();

        public DataTable RMDataTableRight = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public int TotalSelected;
        public decimal TotalAmount; 

        string SqlString; // Do not delete 

        string WTempFile;

        string SqlStringFilePart; 

        string MatchedFileId = "[ATMS].[dbo].[WtblRMCategoriesMatchedTrans]";
        string UnMatchedFileId = "[ATMS].[dbo].[WtblRMCategoriesUnMatchedTrans]";
        string VisaAuthorisationsFileId = "[ATMS].[dbo].[WtblVisaAuthorisationsPool]"; 

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        //
        // Methods 
        // READ Specific by SeqNo
        // FILL UP A TABLE
        //
        public void ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(string InWhatFile, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool MaskFile = false;

            if (InWhatFile == "Matched" || InWhatFile == "UnMatched" || InWhatFile == "VisaAuthorPool")
            {
                // 

                if (InWhatFile == "Matched")
                {
                    SqlString = "SELECT *"
                      + " FROM " + MatchedFileId
                      + " WHERE SeqNo = @SeqNo";

                }
                if (InWhatFile == "UnMatched")
                {
                    SqlString = "SELECT *"
                     + " FROM " + UnMatchedFileId
                     + " WHERE SeqNo = @SeqNo";
                }

                if (InWhatFile == "VisaAuthorPool")
                {
                    SqlString = "SELECT *"
                     + " FROM " + VisaAuthorisationsFileId
                     + " WHERE SeqNo = @SeqNo";
                }

            }
            else  // other type of file from Json
            {
                MaskFile = true;
                SqlString = "SELECT *"
               + " FROM " + InWhatFile
               + " WHERE UniqueRecordId = @UniqueRecordId";
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (MaskFile == true)
                        {
                            cmd.Parameters.AddWithValue("@UniqueRecordId", InSeqNo);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        }


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            OriginFileName = (string)rdr["OriginFileName"];

                            OriginalRecordId = (int)rdr["OriginalRecordId"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (string)rdr["T24RefNumber"];

                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];

                            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

                            MatchedType = (string)rdr["MatchedType"];

                            UnMatchedType = (string)rdr["UnMatchedType"];

                            MetaExceptionId = (int)rdr["MetaExceptionId"];

                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            ActionType = (string)rdr["ActionType"];

                            RemainsForMatching = (bool)rdr["RemainsForMatching"];
                            WaitingForUpdating = (bool)rdr["WaitingForUpdating"];

                            OpenRecord = (bool)rdr["OpenRecord"];

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
                    ErrorOutput = "An error occured in ReadMatchedORUnMatchedFileSpecificRecordBySeqNo......... " + ex.Message;

                }
        }
        //
        // Methods 
        // READ Specific by Mask Id 
        // 
        //
        public void ReadMatchedORUnMatchedFileSpecificRecordByMaskId(string InWhatFile, int InMaskRecordId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool MaskFile = false; 

            if (InWhatFile == "Matched" || InWhatFile == "UnMatched" || InWhatFile == "VisaAuthorPool")
            {
                // 

                if (InWhatFile == "Matched")
                {
                    SqlString = "SELECT *"
                      + " FROM " + MatchedFileId
                      + " WHERE MaskRecordId = @MaskRecordId";

                }
                if (InWhatFile == "UnMatched")
                {
                    SqlString = "SELECT *"
                     + " FROM " + UnMatchedFileId
                     + " WHERE MaskRecordId = @MaskRecordId";
                }

                if (InWhatFile == "VisaAuthorPool")
                {
                    SqlString = "SELECT *"
                     + " FROM " + VisaAuthorisationsFileId
                     + " WHERE MaskRecordId = @MaskRecordId";
                }

            }
            else
            {
                MaskFile = true; 
                SqlString = "SELECT *"
               + " FROM " + InWhatFile
               + " WHERE MaskRecordId = @MaskRecordId";
            }
   

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (MaskFile == true)
                        {
                            cmd.Parameters.AddWithValue("@MaskRecordId", InMaskRecordId);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@MaskRecordId", InMaskRecordId);
                        }
                        

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            OriginFileName = (string)rdr["OriginFileName"];

                            OriginalRecordId = (int)rdr["OriginalRecordId"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (string)rdr["T24RefNumber"];

                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];

                            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

                            MatchedType = (string)rdr["MatchedType"];

                            UnMatchedType = (string)rdr["UnMatchedType"];

                            MetaExceptionId = (int)rdr["MetaExceptionId"];

                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            ActionType = (string)rdr["ActionType"];

                            RemainsForMatching = (bool)rdr["RemainsForMatching"];
                            WaitingForUpdating = (bool)rdr["WaitingForUpdating"];

                            OpenRecord = (bool)rdr["OpenRecord"];

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
                    ErrorOutput = "An error occured in ReadMatchedORUnMatchedFileSpecificRecordBySeqNo......... " + ex.Message;

                }
        }
        //
        // Methods 
        // READ TOTAL UNMATCHED FOR RM CATEGORY AND LESS < RM Cycle 
        // FILL UP A TABLE
        //
        public void ReadMatchedORUnMatchedFileForTotals(string InOperator, string InRMCateg, int InRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalUnMatched = 0;

            TotalAmountUnMatched = 0;


            SqlString = "SELECT *"
             + " FROM " + UnMatchedFileId
              + " WHERE Operator = 'CRBAGRAA' AND RMCateg = @RMCateg AND RMCycle <= @RMCycle AND ActionType = '0'"; 
        

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalUnMatched = TotalUnMatched + 1 ;

                            TotalAmountUnMatched = TotalAmountUnMatched + 1 ;

                            SeqNo = (int)rdr["SeqNo"];

                            OriginFileName = (string)rdr["OriginFileName"];

                            OriginalRecordId = (int)rdr["OriginalRecordId"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (string)rdr["T24RefNumber"];

                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];

                            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

                            MatchedType = (string)rdr["MatchedType"];

                            UnMatchedType = (string)rdr["UnMatchedType"];

                            MetaExceptionId = (int)rdr["MetaExceptionId"];
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            ActionType = (string)rdr["ActionType"];

                            RemainsForMatching = (bool)rdr["RemainsForMatching"];
                            WaitingForUpdating = (bool)rdr["WaitingForUpdating"];

                            OpenRecord = (bool)rdr["OpenRecord"];

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
                    ErrorOutput = "An error occured in ReadMatchedORUnMatchedFileTableLeft ......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ MATCHED TRANS AND Fill TABLE LEFT
        // FILL UP A TABLE
        //
        public void ReadMatchedORUnMatchedFileTableLeft(string InOperator, string InFilter, string InWhatFile, string InSortValue)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMDataTableLeft = new DataTable();
            RMDataTableLeft.Clear();
            TotalSelected = 0;


            // DATA TABLE ROWS DEFINITION 

            RMDataTableLeft.Columns.Add("SeqNo", typeof(int));
            RMDataTableLeft.Columns.Add("File", typeof(string));
            RMDataTableLeft.Columns.Add("Mask", typeof(string));        
            RMDataTableLeft.Columns.Add("Card", typeof(string));
            RMDataTableLeft.Columns.Add("Account", typeof(string));
            RMDataTableLeft.Columns.Add("Curr", typeof(string));
            RMDataTableLeft.Columns.Add("Amount", typeof(string));
            RMDataTableLeft.Columns.Add("TransDate", typeof(DateTime));
            RMDataTableLeft.Columns.Add("TransDescr", typeof(string));
            RMDataTableLeft.Columns.Add("RMCategory", typeof(string));
            RMDataTableLeft.Columns.Add("MatchingDt", typeof(string));

            if (InWhatFile == "Matched")
            {
                SqlString = "SELECT *"
                  + " FROM " + MatchedFileId
                  + " WHERE " + InFilter 
                  +" ORDER BY " + InSortValue;


            }
            if (InWhatFile == "UnMatched")
            {
                SqlString = "SELECT *"
                 + " FROM " + UnMatchedFileId
                  + " WHERE " + InFilter
                  +" ORDER BY " + InSortValue;
            }

            if (InWhatFile == "VisaAuthorPool")
            {
                SqlString = "SELECT *"
                 + " FROM " + VisaAuthorisationsFileId
                 + " WHERE " + InFilter
                 + " ORDER BY " + InSortValue;
            }

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

                            TotalSelected = TotalSelected + 1 ; 

                            SeqNo = (int)rdr["SeqNo"];

                            OriginFileName = (string)rdr["OriginFileName"];

                            OriginalRecordId = (int)rdr["OriginalRecordId"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (string)rdr["T24RefNumber"];

                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];

                            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

                            MatchedType = (string)rdr["MatchedType"];

                            UnMatchedType = (string)rdr["UnMatchedType"];

                            MetaExceptionId = (int)rdr["MetaExceptionId"];
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            ActionType = (string)rdr["ActionType"];

                            RemainsForMatching = (bool)rdr["RemainsForMatching"];
                            WaitingForUpdating = (bool)rdr["WaitingForUpdating"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

                            if (Matched == true) WTempFile = "M";
                            else
                            {
                                WTempFile = "U";
                                TotalAmountUnMatched = TotalAmountUnMatched + TransAmount; 
                            } 



                            // Fill Table 

                            DataRow RowSelected = RMDataTableLeft.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["File"] = WTempFile;
                            RowSelected["Mask"] = MatchMask;
                            RowSelected["Card"] = CardNumber;
                            RowSelected["Account"] = AccNumber;
                            RowSelected["Curr"] = TransCurr;
                            RowSelected["Amount"] = TransAmount.ToString("#,##0.00");;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["TransDescr"] = TransDescr;
                            RowSelected["RMCategory"] = RMCateg;
                            RowSelected["MatchingDt"] = SystemMatchingDtTm.ToString();

                            // ADD ROW
                            RMDataTableLeft.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in ReadMatchedORUnMatchedFileTableLeft ......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ MATCHED TRANS AND Fill TABLE LEFT Based on Period 
        // FILL UP A TABLE
        //
        public void ReadMatchedORUnMatchedFileTableLeftByPeriod(string InOperator, string InFilter, string InWhatFile,
                                                                               DateTime InFromDt, DateTime InToDt, string InSortValue)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMDataTableLeft = new DataTable();
            RMDataTableLeft.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            RMDataTableLeft.Columns.Add("SeqNo", typeof(int));
            RMDataTableLeft.Columns.Add("File", typeof(string));
            RMDataTableLeft.Columns.Add("Mask", typeof(string));
            RMDataTableLeft.Columns.Add("Card", typeof(string));
            RMDataTableLeft.Columns.Add("Account", typeof(string));
            RMDataTableLeft.Columns.Add("Curr", typeof(string));
            RMDataTableLeft.Columns.Add("Amount", typeof(string));
            RMDataTableLeft.Columns.Add("TransDate", typeof(DateTime));
            RMDataTableLeft.Columns.Add("TransDescr", typeof(string));
            RMDataTableLeft.Columns.Add("RMCategory", typeof(string));
            RMDataTableLeft.Columns.Add("MatchingDt", typeof(string));

            if (InWhatFile == "Matched")
            {
                SqlString = "SELECT *"
                  + " FROM " + MatchedFileId
                  + " WHERE " + InFilter + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                  + " ORDER BY " + InSortValue;


            }
            if (InWhatFile == "UnMatched")
            {
                SqlString = "SELECT *"
                 + " FROM " + UnMatchedFileId
                  + " WHERE " + InFilter + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                  + " ORDER BY " + InSortValue;

            }

            if (InWhatFile == "VisaAuthorPool")
            {
                SqlString = "SELECT *"
                 + " FROM " + VisaAuthorisationsFileId
                 + " WHERE " + InFilter + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                 + " ORDER BY " + InSortValue;
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                        cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            SeqNo = (int)rdr["SeqNo"];

                            OriginFileName = (string)rdr["OriginFileName"];

                            OriginalRecordId = (int)rdr["OriginalRecordId"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (string)rdr["T24RefNumber"];

                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];

                            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

                            MatchedType = (string)rdr["MatchedType"];

                            UnMatchedType = (string)rdr["UnMatchedType"];

                            MetaExceptionId = (int)rdr["MetaExceptionId"];
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            ActionType = (string)rdr["ActionType"];

                            RemainsForMatching = (bool)rdr["RemainsForMatching"];
                            WaitingForUpdating = (bool)rdr["WaitingForUpdating"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

                            if (Matched == true) WTempFile = "M";
                            else WTempFile = "U";

                            // Fill Table 

                            DataRow RowSelected = RMDataTableLeft.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["File"] = WTempFile;
                            RowSelected["Mask"] = MatchMask;
                            RowSelected["Card"] = CardNumber;
                            RowSelected["Account"] = AccNumber;
                            RowSelected["Curr"] = TransCurr;
                            RowSelected["Amount"] = TransAmount.ToString("#,##0.00");;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["TransDescr"] = TransDescr;
                            RowSelected["RMCategory"] = RMCateg;
                            RowSelected["MatchingDt"] = SystemMatchingDtTm.ToString();

                            // ADD ROW
                            RMDataTableLeft.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in ReadMatchedORUnMatchedFileTableLeft ......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ MATCHED and UNMATCHED TRANS IN Both Files 
        // FILL UP the  TABLE
        //
        public void ReadBothMatchedUnMatchedFileTable(string InOperator, string InFilter, 
                                                                                DateTime InFromDt, DateTime InToDt, string InSortValue, int InFrom)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InFrom = 1 normal 
            // InFrom = 2 Disputes 
           

            RMDataTableLeft = new DataTable();
            RMDataTableLeft.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            if (InFrom == 1)
            {
                RMDataTableLeft.Columns.Add("SeqNo", typeof(int));
                RMDataTableLeft.Columns.Add("File", typeof(string));
                RMDataTableLeft.Columns.Add("Mask", typeof(string));
            }
            if (InFrom == 2)
            {
                RMDataTableLeft.Columns.Add("Chosen", typeof(bool));
                RMDataTableLeft.Columns.Add("DisputedAmnt", typeof(decimal));
            }

            RMDataTableLeft.Columns.Add("Card", typeof(string));
            RMDataTableLeft.Columns.Add("Account", typeof(string));
            RMDataTableLeft.Columns.Add("Curr", typeof(string));
            RMDataTableLeft.Columns.Add("Amount", typeof(string));
            RMDataTableLeft.Columns.Add("TransDate", typeof(DateTime));
            RMDataTableLeft.Columns.Add("TransDescr", typeof(string));
            RMDataTableLeft.Columns.Add("MaskRecordId", typeof(int));
            RMDataTableLeft.Columns.Add("RMCategory", typeof(string));
            RMDataTableLeft.Columns.Add("MatchingDt", typeof(string));

            if (InFromDt != NullPastDate)
            {
                SqlString = "SELECT *"
                  + " FROM " + MatchedFileId
                  + " WHERE " + InFilter + " AND (TransDate >= @FromDt AND TransDate <= @ToDt) "
                   + " ORDER BY " + InSortValue;
            }
            
            if (InFromDt == NullPastDate)
            {
                SqlString = "SELECT *"
                  + " FROM " + MatchedFileId
                  + " WHERE " + InFilter 
                   + " ORDER BY " + InSortValue;
            }

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        if (InFromDt != NullPastDate)
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

                            SeqNo = (int)rdr["SeqNo"];

                            OriginFileName = (string)rdr["OriginFileName"];

                            OriginalRecordId = (int)rdr["OriginalRecordId"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (string)rdr["T24RefNumber"];

                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];

                            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

                            MatchedType = (string)rdr["MatchedType"];

                            UnMatchedType = (string)rdr["UnMatchedType"];

                            MetaExceptionId = (int)rdr["MetaExceptionId"];
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            ActionType = (string)rdr["ActionType"];

                            RemainsForMatching = (bool)rdr["RemainsForMatching"];
                            WaitingForUpdating = (bool)rdr["WaitingForUpdating"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

                            if (Matched == true) WTempFile = "M";
                            else WTempFile = "U";

                            // Fill Table 

                            DataRow RowSelected = RMDataTableLeft.NewRow();
                            if (InFrom == 1)
                            {
                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["File"] = WTempFile;
                                RowSelected["Mask"] = MatchMask;
                            }
                            if (InFrom == 2)
                            {
                                RowSelected["Chosen"] = false;
                                RowSelected["DisputedAmnt"] = TransAmount; 
                            }
                         
                            RowSelected["Card"] = CardNumber;
                            RowSelected["Account"] = AccNumber;
                            RowSelected["Curr"] = TransCurr;
                            RowSelected["Amount"] = TransAmount.ToString("#,##0.00");
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["TransDescr"] = TransDescr;
                            RowSelected["MaskRecordId"] = MaskRecordId;                  
                            RowSelected["RMCategory"] = RMCateg;
                            RowSelected["MatchingDt"] = SystemMatchingDtTm.ToString();

                            // ADD ROW
                            RMDataTableLeft.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in ReadMatchedORUnMatchedFileTableLeft ......... " + ex.Message;

                }
            // 
            // Second Table UNMATCHED
            //

            if (InFromDt != NullPastDate)
            {
                SqlString = "SELECT *"
                  + " FROM " + UnMatchedFileId
                  + " WHERE " + InFilter + " AND (TransDate > @FromDt AND TransDate < @ToDt) "
                   + " ORDER BY " + InSortValue;
            }


            if (InFromDt == NullPastDate)
            {
                SqlString = "SELECT *"
                  + " FROM " + UnMatchedFileId
                  + " WHERE " + InFilter
                   + " ORDER BY " + InSortValue;
            }

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        if (InFromDt != NullPastDate)
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

                            SeqNo = (int)rdr["SeqNo"];

                            OriginFileName = (string)rdr["OriginFileName"];

                            OriginalRecordId = (int)rdr["OriginalRecordId"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (string)rdr["T24RefNumber"];

                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];

                            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

                            MatchedType = (string)rdr["MatchedType"];

                            UnMatchedType = (string)rdr["UnMatchedType"];

                            MetaExceptionId = (int)rdr["MetaExceptionId"];
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            ActionType = (string)rdr["ActionType"];

                            RemainsForMatching = (bool)rdr["RemainsForMatching"];
                            WaitingForUpdating = (bool)rdr["WaitingForUpdating"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

                            if (Matched == true) WTempFile = "M";
                            else WTempFile = "U";

                            // Fill Table 

                            DataRow RowSelected = RMDataTableLeft.NewRow();
                            if (InFrom == 1)
                            {
                                RowSelected["SeqNo"] = SeqNo;
                                RowSelected["File"] = WTempFile;
                                RowSelected["Mask"] = MatchMask;
                            }
                            if (InFrom == 2)
                            {
                                RowSelected["Chosen"] = false;
                                RowSelected["DisputedAmnt"] = TransAmount;
                            }
                            RowSelected["Card"] = CardNumber;
                            RowSelected["Account"] = AccNumber;
                            RowSelected["Curr"] = TransCurr;
                            RowSelected["Amount"] = TransAmount.ToString("#,##0.00");;
                            RowSelected["TransDate"] = TransDate;
                            RowSelected["TransDescr"] = TransDescr;
                            RowSelected["MaskRecordId"] = MaskRecordId;
                            RowSelected["RMCategory"] = RMCateg;
                            RowSelected["MatchingDt"] = SystemMatchingDtTm.ToString();

                            // ADD ROW
                            RMDataTableLeft.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in ReadMatchedORUnMatchedFileTableLeft ......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ MATCHED TRANS AND Fill TABLE LEFT FOR ATMS 
        // FILL UP A TABLE
        //
        public void ReadMatchedORUnMatchedFileTableLeft2(string InOperator, string InWhatFile, string InSearchingString)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMDataTableLeft = new DataTable();
            RMDataTableLeft.Clear();

            TotalSelected = 0;
            TotalAmount = 0; 
            // DATA TABLE ROWS DEFINITION 
            
            RMDataTableLeft.Columns.Add("SeqNo", typeof(int));
            RMDataTableLeft.Columns.Add("Done", typeof(string));
            RMDataTableLeft.Columns.Add("ATMNo", typeof(string));
            RMDataTableLeft.Columns.Add("Descr", typeof(string));
            RMDataTableLeft.Columns.Add("Card", typeof(string));
            RMDataTableLeft.Columns.Add("Account", typeof(string));
            RMDataTableLeft.Columns.Add("Curr", typeof(string));
            RMDataTableLeft.Columns.Add("Amount", typeof(string));
            RMDataTableLeft.Columns.Add("Date", typeof(DateTime));
            RMDataTableLeft.Columns.Add("RMCategory", typeof(string));
            RMDataTableLeft.Columns.Add("RRNumber", typeof(int));


            if (InWhatFile == "Matched")
            {
                SqlString = "SELECT *"
                  + " FROM " + MatchedFileId
                  + " WHERE " + InSearchingString ;


            }
            if (InWhatFile == "UnMatched")
            {
                SqlString = "SELECT *"
                 + " FROM " + UnMatchedFileId
                 + " WHERE " + InSearchingString;
            }

            if (InWhatFile == "VisaAuthorPool")
            {
                SqlString = "SELECT *"
                 + " FROM " + VisaAuthorisationsFileId
                + " WHERE " + InSearchingString;
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            OriginFileName = (string)rdr["OriginFileName"];

                            OriginalRecordId = (int)rdr["OriginalRecordId"];

                            RMCateg = (string)rdr["RMCateg"];

                            RMCycle = (int)rdr["RMCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (string)rdr["T24RefNumber"];

                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];

                            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

                            MatchedType = (string)rdr["MatchedType"];

                            UnMatchedType = (string)rdr["UnMatchedType"];

                            MetaExceptionId = (int)rdr["MetaExceptionId"];
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            ActionType = (string)rdr["ActionType"];

                            RemainsForMatching = (bool)rdr["RemainsForMatching"];
                            WaitingForUpdating = (bool)rdr["WaitingForUpdating"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

                            // Fill Table 

                            DataRow RowSelected = RMDataTableLeft.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            if (ActionType != "0")
                            {
                                RowSelected["Done"] = "YES";
                            }
                            else
                            {
                                RowSelected["Done"] = "NO";
                            }
        
                            RowSelected["ATMNo"] = TerminalId;
                            RowSelected["Descr"] = TransDescr;
                            RowSelected["Card"] = CardNumber;
                            RowSelected["Account"] = AccNumber;
                            RowSelected["Curr"] = TransCurr;
                            RowSelected["Amount"] = TransAmount.ToString("#,##0.00");;
                            RowSelected["Date"] = TransDate;
                            RowSelected["RMCategory"] = RMCateg;
                            RowSelected["RRNumber"] = RRNumber;

                            // ADD ROW
                            RMDataTableLeft.Rows.Add(RowSelected);

                            // UPDATE META EXCEPTION NUMBER 

                            Rme.ReadReconcMaskRecord(InOperator, RMCateg, MatchMask);

                            if (Rme.RecordFound)
                            {
                                UnMatchedType = Rme.MaskName;
                                MetaExceptionId = Rme.MetaExceptionNo;
                            }
                            else
                            {
                                UnMatchedType = "Not Specified";
                                MetaExceptionId = 0 ;
                            }

                            UpdateMatchedORUnMatchedRecordFooter(InOperator, InWhatFile, SeqNo);

                            TotalSelected = TotalSelected + 1; 
                            TotalAmount = TotalAmount + TransAmount; 

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
                    ErrorOutput = "An error occured in ReadMatchedORUnMatchedFileTableLeft ......... " + ex.Message;

                }
        }

        //
        // Methods 
        // READ TRANSACTIONS For the RIGHT Side of Form271b 
        // FILL UP A TABLE
        //
        public void ReadMatchedORUnMatchedFileTablerRight2(string InOperator, string InWhatFile, string InSearchingString)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMDataTableRight = new DataTable();
            RMDataTableRight.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            RMDataTableRight.Columns.Add("SeqNo", typeof(int));
            RMDataTableRight.Columns.Add("Done", typeof(string));
            RMDataTableRight.Columns.Add("ATMNo", typeof(string));
            RMDataTableRight.Columns.Add("Descr", typeof(string));
            RMDataTableRight.Columns.Add("Card", typeof(string));
            RMDataTableRight.Columns.Add("Account", typeof(string));
            RMDataTableRight.Columns.Add("Curr", typeof(string));
            RMDataTableRight.Columns.Add("Amount", typeof(string));
            RMDataTableRight.Columns.Add("Date", typeof(DateTime));
            RMDataTableRight.Columns.Add("RMCategory", typeof(string));
            RMDataTableRight.Columns.Add("RRNumber", typeof(int));

            if (InWhatFile == "Matched")
            {
                SqlString = "SELECT *"
                  + " FROM " + MatchedFileId
                  + " WHERE " + InSearchingString;
            }
            if (InWhatFile == "UnMatched")
            {
                SqlString = "SELECT *"
                 + " FROM " + UnMatchedFileId
                 + " WHERE " + InSearchingString;
            }

            if (InWhatFile == "VisaAuthorPool")
            {
                SqlString = "SELECT *"
                 + " FROM " + VisaAuthorisationsFileId
                + " WHERE " + InSearchingString;
            }


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

                            SeqNo = (int)rdr["SeqNo"];

                            OriginFileName = (string)rdr["OriginFileName"];

                            OriginalRecordId = (int)rdr["OriginalRecordId"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (string)rdr["T24RefNumber"];

                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];

                            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

                            MatchedType = (string)rdr["MatchedType"];

                            UnMatchedType = (string)rdr["UnMatchedType"];

                            MetaExceptionId = (int)rdr["MetaExceptionId"];
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            ActionType = (string)rdr["ActionType"];

                            RemainsForMatching = (bool)rdr["RemainsForMatching"];
                            WaitingForUpdating = (bool)rdr["WaitingForUpdating"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

                            // Fill Table 

                            DataRow RowSelected = RMDataTableRight.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            if (ActionType != "0")
                            {
                                RowSelected["Done"] = "YES";
                            }
                            else
                            {
                                RowSelected["Done"] = "NO";
                            }

                            RowSelected["ATMNo"] = TerminalId;

                            RowSelected["Descr"] = TransDescr;

                            RowSelected["Card"] = CardNumber;
                            RowSelected["Account"] = AccNumber;

                            RowSelected["Curr"] = TransCurr;
                            RowSelected["Amount"] = TransAmount.ToString("#,##0.00");;
                            RowSelected["Date"] = TransDate;
                            RowSelected["RMCategory"] = RMCateg;

                            RowSelected["RRNumber"] = RRNumber;

                            // ADD ROW
                            RMDataTableRight.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in ReadMatchedORUnMatchedFileTableLeft ......... " + ex.Message;

                }
        }
        //
        // Methods 
        // READ MATCHED TRANS AND Fill TABLE RIGHT
        // FILL UP A TABLE
        //
        public void ReadMatchedORUnMatchedFileTableRight(string InOperator, string InWhatFile, string InSearchingString)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RMDataTableRight = new DataTable();
            RMDataTableRight.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            RMDataTableRight.Columns.Add("SeqNo", typeof(int));
            RMDataTableRight.Columns.Add("Done", typeof(bool));
            RMDataTableRight.Columns.Add("Descr", typeof(string));
            RMDataTableRight.Columns.Add("Card", typeof(string));
            RMDataTableRight.Columns.Add("Account", typeof(string));
            RMDataTableRight.Columns.Add("Curr", typeof(string));
            RMDataTableRight.Columns.Add("Amount", typeof(string));
            RMDataTableRight.Columns.Add("Date", typeof(DateTime));
            RMDataTableRight.Columns.Add("RMCategory", typeof(string));
            RMDataTableRight.Columns.Add("RRNumber", typeof(int));


            if (InWhatFile == "Matched")
            {
                SqlString = "SELECT *"
                  + " FROM " + MatchedFileId
                  + " WHERE " + InSearchingString;


            }
            if (InWhatFile == "UnMatched")
            {
                SqlString = "SELECT *"
                 + " FROM " + UnMatchedFileId
                 + " WHERE " + InSearchingString;
            }


            if (InWhatFile == "VisaAuthorPool")
            {
                SqlString = "SELECT *"
                 + " FROM " + VisaAuthorisationsFileId
                + " WHERE " + InSearchingString;
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@RMCateg", InRMCateg);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            OriginFileName = (string)rdr["OriginFileName"];

                            OriginalRecordId = (int)rdr["OriginalRecordId"];

                            RMCateg = (string)rdr["RMCateg"];
                            RMCycle = (int)rdr["RMCycle"];

                            MaskRecordId = (int)rdr["MaskRecordId"];
                            UniqueRecordId = (int)rdr["UniqueRecordId"];

                            Origin = (string)rdr["Origin"];
                            TransTypeAtOrigin = (string)rdr["TransTypeAtOrigin"];
                            Product = (string)rdr["Product"];
                            CostCentre = (string)rdr["CostCentre"];

                            TerminalId = (string)rdr["TerminalId"];

                            TransType = (int)rdr["TransType"];

                            TransDescr = (string)rdr["TransDescr"];

                            CardNumber = (string)rdr["CardNumber"];

                            AccNumber = (string)rdr["AccNumber"];

                            TransCurr = (string)rdr["TransCurr"];
                            TransAmount = (decimal)rdr["TransAmount"];

                            TransDate = (DateTime)rdr["TransDate"];

                            AtmTraceNo = (int)rdr["AtmTraceNo"];
                            RRNumber = (int)rdr["RRNumber"];
                            ResponseCode = (int)rdr["ResponseCode"];
                            T24RefNumber = (string)rdr["T24RefNumber"];

                            Matched = (bool)rdr["Matched"];
                            MatchMask = (string)rdr["MatchMask"];

                            SystemMatchingDtTm = (DateTime)rdr["SystemMatchingDtTm"];

                            MatchedType = (string)rdr["MatchedType"];

                            UnMatchedType = (string)rdr["UnMatchedType"];

                            MetaExceptionId = (int)rdr["MetaExceptionId"];
                            MetaExceptionNo = (int)rdr["MetaExceptionNo"];

                            ActionByUser = (bool)rdr["ActionByUser"];
                            UserId = (string)rdr["UserId"];
                            Authoriser = (string)rdr["Authoriser"];

                            AuthoriserDtTm = (DateTime)rdr["AuthoriserDtTm"];

                            ActionType = (string)rdr["ActionType"];

                            RemainsForMatching = (bool)rdr["RemainsForMatching"];
                            WaitingForUpdating = (bool)rdr["WaitingForUpdating"];

                            OpenRecord = (bool)rdr["OpenRecord"];

                            Operator = (string)rdr["Operator"];

                            // Fill Table 

                            DataRow RowSelected = RMDataTableRight.NewRow();

                            RowSelected["SeqNo"] = SeqNo;

                            RowSelected["Done"] = false;

                            RowSelected["Descr"] = TransDescr;

                            RowSelected["Card"] = CardNumber;
                            RowSelected["Account"] = AccNumber;

                            RowSelected["Curr"] = TransCurr;
                            RowSelected["Amount"] = TransAmount.ToString("#,##0.00");;
                            RowSelected["Date"] = TransDate;
                            RowSelected["RMCategory"] = RMCateg;
                            RowSelected["RRNumber"] = RRNumber;

                            // ADD ROW
                            RMDataTableRight.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in ReadMatchedORUnMatchedFileTableLeft ......... " + ex.Message;

                }
        }
        // 
        // Insert Matched File recort 
        //
        public void InsertMatchedORUnMatchedFileRecord(string InWhatFile)
        {
            if (InWhatFile == "Matched")
            {
                SqlStringFilePart =  MatchedFileId;

            }
            if (InWhatFile == "UnMatched")
            {
                SqlStringFilePart = UnMatchedFileId;
            }


            if (InWhatFile == "VisaAuthorPool")
            {
                SqlStringFilePart = VisaAuthorisationsFileId;
            }
            
            if (InWhatFile == "KontoPool")
            {
                SqlStringFilePart = "[RRDM_Reconciliation].[dbo].[tblReconcInPoolKONTO]";
            }


            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert =
                 "INSERT INTO " + SqlStringFilePart
                     + " ([OriginFileName], [OriginalRecordId],"
                     + " [RMCateg], [RMCycle], [MaskRecordId], [UniqueRecordId],"
                     + " [Origin], [TransTypeAtOrigin], [Product], [CostCentre], "
                     +" [TerminalId], [TransType], [TransDescr],"
                     +" [CardNumber], [AccNumber], [TransCurr], [TransAmount],"
                     +" [TransDate], [AtmTraceNo], [RRNumber], [ResponseCode], [T24RefNumber],"
                     +" [OpenRecord], [Operator]) "
                     + " VALUES (@OriginFileName, @OriginalRecordId,"
                     + " @RMCateg, @RMCycle,@MaskRecordId,@UniqueRecordId,"
                     +" @Origin,@TransTypeAtOrigin, @Product, @CostCentre,"
                     +" @TerminalId, @TransType, @TransDescr,"
                     +" @CardNumber, @AccNumber, @TransCurr, @TransAmount,"
                     +" @TransDate, @AtmTraceNo, @RRNumber, @ResponseCode, @T24RefNumber,"
                     +" @OpenRecord, @Operator)" ;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                    
                        cmd.Parameters.AddWithValue("@OriginFileName", OriginFileName);
                       
                        cmd.Parameters.AddWithValue("@OriginalRecordId", OriginalRecordId);
                     
                        cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
                        cmd.Parameters.AddWithValue("@RMCycle", RMCycle);

                        cmd.Parameters.AddWithValue("@MaskRecordId", MaskRecordId);

                        cmd.Parameters.AddWithValue("@UniqueRecordId", UniqueRecordId);
                        

                        cmd.Parameters.AddWithValue("@Origin", Origin);
                        cmd.Parameters.AddWithValue("@TransTypeAtOrigin", TransTypeAtOrigin);

                        cmd.Parameters.AddWithValue("@Product", Product);
                        cmd.Parameters.AddWithValue("@CostCentre", CostCentre);

                        cmd.Parameters.AddWithValue("@TerminalId", TerminalId);
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDescr", TransDescr);

                        cmd.Parameters.AddWithValue("@CardNumber", CardNumber);
                        cmd.Parameters.AddWithValue("@AccNumber", AccNumber);
                        cmd.Parameters.AddWithValue("@TransCurr", TransCurr);
                        cmd.Parameters.AddWithValue("@TransAmount", TransAmount);

                        cmd.Parameters.AddWithValue("@TransDate", TransDate);
                        cmd.Parameters.AddWithValue("@AtmTraceNo", AtmTraceNo);
                        cmd.Parameters.AddWithValue("@RRNumber", RRNumber);
                        cmd.Parameters.AddWithValue("@ResponseCode", ResponseCode);

                        cmd.Parameters.AddWithValue("@T24RefNumber", T24RefNumber);

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
                    ErrorOutput = "An error occured in MatchedORUnMatched ............. " + ex.Message;
                }
        }

        // UPDATE MATCHED RECORDS Footer 
        // 
        public void UpdateMatchedORUnMatchedRecordFooter(string InOperator, string InWhatFile, int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InWhatFile == "Matched")
            {
                SqlStringFilePart = MatchedFileId;

            }

            if (InWhatFile == "UnMatched")
            {
                SqlStringFilePart = UnMatchedFileId;
            }

            if (InWhatFile == "VisaAuthorPool")
            {
                SqlStringFilePart = VisaAuthorisationsFileId;
            }


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand (
                            "UPDATE " + SqlStringFilePart 
                            + " SET " 
                            + " [Matched] = @Matched, [MatchMask] = @MatchMask,"
                            + " [SystemMatchingDtTm] = @SystemMatchingDtTm, [MatchedType] = @MatchedType, [UnMatchedType] = @UnMatchedType,"
                            + " [MetaExceptionId] = @MetaExceptionId, [MetaExceptionNo] = @MetaExceptionNo, "
                            + " [ActionByUser] = @ActionByUser, [UserId] = @UserId,"
                            + " [Authoriser] = @Authoriser, [AuthoriserDtTm] = @AuthoriserDtTm, [ActionType] = @ActionType,"
                            + " [RemainsForMatching] = @RemainsForMatching, [WaitingForUpdating] = @WaitingForUpdating, "
                            + " [OpenRecord] = @OpenRecord " 
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@Matched", Matched);
                        cmd.Parameters.AddWithValue("@MatchMask", MatchMask);
                        cmd.Parameters.AddWithValue("@SystemMatchingDtTm", SystemMatchingDtTm);

                        cmd.Parameters.AddWithValue("@MatchedType", MatchedType);
                        cmd.Parameters.AddWithValue("@UnMatchedType", UnMatchedType);

                        cmd.Parameters.AddWithValue("@MetaExceptionId", MetaExceptionId);
                        cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);

                        cmd.Parameters.AddWithValue("@ActionByUser", ActionByUser);

                        cmd.Parameters.AddWithValue("@UserId", UserId);
                        cmd.Parameters.AddWithValue("@Authoriser", Authoriser);
                        cmd.Parameters.AddWithValue("@AuthoriserDtTm", AuthoriserDtTm);

                        cmd.Parameters.AddWithValue("@ActionType", ActionType);

                        cmd.Parameters.AddWithValue("@RemainsForMatching", RemainsForMatching);
                        cmd.Parameters.AddWithValue("@WaitingForUpdating", WaitingForUpdating);

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
                    ErrorOutput = "An error occured in UpdateMatchedORUnMatchedRecordFooter Class............. " + ex.Message;
                }
        }

        //
        // DELETE Record In Matched File
        //
        public void DeleteRecordInMatchedORUnMatchedFile(string InWhatFile, int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            if (InWhatFile == "Matched")
            {
                SqlStringFilePart = MatchedFileId;

            }
            if (InWhatFile == "UnMatched")
            {
                SqlStringFilePart = UnMatchedFileId;
            }

            if (InWhatFile == "VisaAuthorPool")
            {
                SqlStringFilePart = VisaAuthorisationsFileId;
            }


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM " + SqlStringFilePart
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in DeleteRecordInMatchedORUnMatchedFile Class............. " + ex.Message;
                }

        }

 
    }
}
