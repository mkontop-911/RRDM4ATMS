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
    public class RRDMErrorsORExceptionsCharacteristics
    {
        // Declarations 

        // Variables for reading errors 
        //public int ErrNo;
        public int ErrId;
        public int ErrType;
        // Values
        // 1 : Withdrawl EJournal Errors
        // 2 : Mainframe Withdrawl Errors
        // 3 : Deposit Errors Journal 
        // 4 : Deposit Mainframe Errors
        // 5 : Created by user Errors = eg moving to suspense 
        // 6 : Empty 
        // 7 : Created System Errors 
        // 
        public string ErrDesc;
        public string AtmNo;
        public int SesNo;
        public int TraceNo;
        public int TransNo;
        public int TransType;
        public string TransDescr;
        public DateTime DateTime;
        public bool NeedAction; public bool OpenErr;

        public string CurDes; public decimal ErrAmount; public int ActionId;
        public bool DrCust; public bool CrCust; public bool DrAtmCash; public bool CrAtmCash; public bool DrAtmSusp; public bool CrAtmSusp;
        public bool UnderAction;
        public bool DisputeAct;
        public bool ManualAct;
        public bool MainOnly;
        public bool FullCard;
        public bool ForeignCard;

        public DateTime DateInserted;
        public string BankId;

        public string BranchId;

        public bool TurboReconc;

        public string CardNo;
        public string ByWhom;
        public DateTime ActionDtTm;
        public int ActionSes;
        public string CustAccNo;
        public string AccountNo1;
        public string AccountNo2;
        public string AccountNo3;

        public bool DrAccount3;
        public bool CrAccount3;
        public string UserComment;
        public bool Printed;
        public DateTime DatePrinted;
        public string CircularDesc;
        public string CitId;
        public string Operator;

        public int NumOfErrors;
        public int NumOfErrorsLess200;
        public int ErrUnderAction;
        public int ErrUnderManualAction;

       
        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMReconcMasksVsMetaExceptions Rme = new RRDMReconcMasksVsMetaExceptions(); 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // Define the data table 
        public DataTable ExceptionsCharacteristicsTable = new DataTable();
        public int TotalSelected;

        string SqlString;

        // READ Error ID Record 
        public void ReadErrorsIDRecordsInTable(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ExceptionsCharacteristicsTable = new DataTable();
            ExceptionsCharacteristicsTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ExceptionsCharacteristicsTable.Columns.Add("Exception", typeof(int));
            ExceptionsCharacteristicsTable.Columns.Add("BankId", typeof(string));
            ExceptionsCharacteristicsTable.Columns.Add("Description", typeof(string));
            ExceptionsCharacteristicsTable.Columns.Add("NeedAction", typeof(bool));
            ExceptionsCharacteristicsTable.Columns.Add("Express", typeof(bool));
          
            SqlString = "SELECT *"
            + " FROM [dbo].[ErrorsIdCharacteristics] "
            + " WHERE Operator=@Operator";
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

                            // Read error Details

                            ErrId = (int)rdr["ErrId"]; // For Characteristics
                            ErrDesc = rdr["ErrDesc"].ToString(); // For Characteristics
                            ErrType = (int)rdr["ErrType"]; // For Characteristics
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            DateInserted = (DateTime)rdr["DateInserted"]; // For Characteristics

                            BankId = rdr["BankId"].ToString(); // For Characteristics

                            BranchId = rdr["BranchId"].ToString(); // For Characteristics
                            TurboReconc = (bool)rdr["TurboReconc"]; // For Characteristics

                            TraceNo = (int)rdr["TraceNo"];
                            CardNo = rdr["CardNo"].ToString();
                            TransNo = (int)rdr["TransNo"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = rdr["TransDescr"].ToString();

                            DateTime = (DateTime)rdr["DateTime"];
                            NeedAction = (bool)rdr["NeedAction"]; // For Characteristics
                            OpenErr = (bool)rdr["OpenErr"]; 
                            FullCard = (bool)rdr["FullCard"];
                            UnderAction = (bool)rdr["UnderAction"];
                            DisputeAct = (bool)rdr["DisputeAct"];
                            ManualAct = (bool)rdr["ManualAct"];
                            ByWhom = (string)rdr["ByWhom"];

                            ActionDtTm = (DateTime)rdr["ActionDtTm"];
                            ActionSes = (int)rdr["ActionSes"];

                            CurDes = rdr["CurDes"].ToString();
                            ErrAmount = (decimal)rdr["ErrAmount"];
                            ActionId = (int)rdr["ActionId"]; // For Characteristics ??

                            DrCust = (bool)rdr["DrCust"]; // For Characteristics
                            CrCust = (bool)rdr["CrCust"]; // For Characteristics
                            CustAccNo = rdr["CustAccNo"].ToString();

                            DrAtmCash = (bool)rdr["DrAtmCash"]; // For Characteristics
                            CrAtmCash = (bool)rdr["CrAtmCash"]; // For Characteristics
                            AccountNo1 = rdr["AccountNo1"].ToString();

                            DrAtmSusp = (bool)rdr["DrAtmSusp"]; // For Characteristics
                            CrAtmSusp = (bool)rdr["CrAtmSusp"]; // For Characteristics
                            AccountNo2 = rdr["AccountNo2"].ToString();

                            DrAccount3 = (bool)rdr["DrAccount3"];
                            CrAccount3 = (bool)rdr["CrAccount3"];
                            AccountNo3 = rdr["AccountNo3"].ToString();

                            ForeignCard = (bool)rdr["ForeignCard"];
                            MainOnly = (bool)rdr["MainOnly"]; // For Characteristics

                            UserComment = rdr["UserComment"].ToString();

                            Printed = (bool)rdr["Printed"];
                            DatePrinted = (DateTime)rdr["DatePrinted"];

                            CircularDesc = rdr["CircularDesc"].ToString(); // For Characteristics 
                            //
                            // Insert Record In Table 
                            //
                            DataRow RowSelected = ExceptionsCharacteristicsTable.NewRow();

                            RowSelected["Exception"] = ErrId;
                            RowSelected["BankId"] = BankId;
                            RowSelected["Description"] = ErrDesc;
                            RowSelected["NeedAction"] = NeedAction;
                            RowSelected["Express"] = TurboReconc;
                        
                            // ADD ROW
                            ExceptionsCharacteristicsTable.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in RRDMErrorsORExceptionsCharacteristics Class............. " + ex.Message;
                }
        }
        //
// READ Error ID Record 
        //
        public void ReadErrorsIDRecordsInTableDistict(string InOperator) 
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ExceptionsCharacteristicsTable = new DataTable();
            ExceptionsCharacteristicsTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ExceptionsCharacteristicsTable.Columns.Add("Exception", typeof(int));
            ExceptionsCharacteristicsTable.Columns.Add("Description", typeof(string));

            SqlString = "SELECT DISTINCT [ErrId],[ErrDesc] "
            + " FROM [dbo].[ErrorsIdCharacteristics] "
            + " WHERE Operator=@Operator";
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

                            // Read error Details

                            ErrId = (int)rdr["ErrId"]; // For Characteristics
                            ErrDesc = rdr["ErrDesc"].ToString(); // For Characteristics
                                                 //
                            // Insert Record In Table 
                            //
                            DataRow RowSelected = ExceptionsCharacteristicsTable.NewRow();

                            RowSelected["Exception"] = ErrId;
                          
                            RowSelected["Description"] = ErrDesc;
                           

                            // ADD ROW
                            ExceptionsCharacteristicsTable.Rows.Add(RowSelected);

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
                    ErrorOutput = "An error occured in RRDMErrorsORExceptionsCharacteristics Class............. " + ex.Message;
                }
        }
//
// READ Error ID Records And Create Mask Vs Meta Exceptions records 
//
        public void CreateMaskVsExceptionsRecords(string InOperator, string InCategory)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            SqlString = "SELECT *"
            + " FROM [dbo].[ErrorsIdCharacteristics] "
            + " WHERE Operator=@Operator";
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

                            // Read error Details

                            ErrId = (int)rdr["ErrId"]; // For Characteristics
                            ErrDesc = rdr["ErrDesc"].ToString(); // For Characteristics
                            ErrType = (int)rdr["ErrType"]; // For Characteristics
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            DateInserted = (DateTime)rdr["DateInserted"]; // For Characteristics

                            BankId = rdr["BankId"].ToString(); // For Characteristics

                            BranchId = rdr["BranchId"].ToString(); // For Characteristics
                            TurboReconc = (bool)rdr["TurboReconc"]; // For Characteristics

                            TraceNo = (int)rdr["TraceNo"];
                            CardNo = rdr["CardNo"].ToString();
                            TransNo = (int)rdr["TransNo"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = rdr["TransDescr"].ToString();

                            DateTime = (DateTime)rdr["DateTime"];
                            NeedAction = (bool)rdr["NeedAction"]; // For Characteristics
                            OpenErr = (bool)rdr["OpenErr"];
                            FullCard = (bool)rdr["FullCard"];
                            UnderAction = (bool)rdr["UnderAction"];
                            DisputeAct = (bool)rdr["DisputeAct"];
                            ManualAct = (bool)rdr["ManualAct"];
                            ByWhom = (string)rdr["ByWhom"];

                            ActionDtTm = (DateTime)rdr["ActionDtTm"];
                            ActionSes = (int)rdr["ActionSes"];

                            CurDes = rdr["CurDes"].ToString();
                            ErrAmount = (decimal)rdr["ErrAmount"];
                            ActionId = (int)rdr["ActionId"]; // For Characteristics ??

                            DrCust = (bool)rdr["DrCust"]; // For Characteristics
                            CrCust = (bool)rdr["CrCust"]; // For Characteristics
                            CustAccNo = rdr["CustAccNo"].ToString();

                            DrAtmCash = (bool)rdr["DrAtmCash"]; // For Characteristics
                            CrAtmCash = (bool)rdr["CrAtmCash"]; // For Characteristics
                            AccountNo1 = rdr["AccountNo1"].ToString();

                            DrAtmSusp = (bool)rdr["DrAtmSusp"]; // For Characteristics
                            CrAtmSusp = (bool)rdr["CrAtmSusp"]; // For Characteristics
                            AccountNo2 = rdr["AccountNo2"].ToString();

                            DrAccount3 = (bool)rdr["DrAccount3"];
                            CrAccount3 = (bool)rdr["CrAccount3"];
                            AccountNo3 = rdr["AccountNo3"].ToString();

                            ForeignCard = (bool)rdr["ForeignCard"];
                            MainOnly = (bool)rdr["MainOnly"]; // For Characteristics

                            UserComment = rdr["UserComment"].ToString();

                            Printed = (bool)rdr["Printed"];
                            DatePrinted = (DateTime)rdr["DatePrinted"];

                            CircularDesc = rdr["CircularDesc"].ToString(); // For Characteristics 
                            //
                            // Insert Record In SQL Table 
                            //

                            //cmd.Parameters.AddWithValue("@CategoryId", CategoryId);

                            //cmd.Parameters.AddWithValue("@MaskId", MaskId);
                            //cmd.Parameters.AddWithValue("@MaskName", MaskName);

                            //cmd.Parameters.AddWithValue("@MetaExceptionNo", MetaExceptionNo);

                            //cmd.Parameters.AddWithValue("@Operator", Operator);

                            Rme.CategoryId = InCategory;
                            Rme.MaskId = "";
                            Rme.MaskName = ErrDesc;
                            Rme.MetaExceptionNo = ErrId;
                            Rme.Operator = InOperator; 

                            Rme.InsertReconcCategoryMaskRecord(); 

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
                    ErrorOutput = "An error occured in RRDMErrorsORExceptionsCharacteristics Class............. " + ex.Message;
                }
        }

        // READ Error ID Record 
        public void ReadErrorsIDRecord(int InErrId, string InBankId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
            + " FROM [dbo].[ErrorsIdCharacteristics] "
            + " WHERE Errid = @ErrId AND BankId=@BankId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrId", InErrId);
                        cmd.Parameters.AddWithValue("@BankId", InBankId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read error Details

                            ErrId = (int)rdr["ErrId"];
                            ErrDesc = rdr["ErrDesc"].ToString();
                            ErrType = (int)rdr["ErrType"];
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            DateInserted = (DateTime)rdr["DateInserted"];

                            BankId = rdr["BankId"].ToString();

                            BranchId = rdr["BranchId"].ToString();
                            TurboReconc = (bool)rdr["TurboReconc"];

                            TraceNo = (int)rdr["TraceNo"];
                            CardNo = rdr["CardNo"].ToString();
                            TransNo = (int)rdr["TransNo"];
                            TransType = (int)rdr["TransType"];
                            TransDescr = rdr["TransDescr"].ToString();

                            DateTime = (DateTime)rdr["DateTime"];
                            NeedAction = (bool)rdr["NeedAction"];
                            OpenErr = (bool)rdr["OpenErr"];
                            FullCard = (bool)rdr["FullCard"];
                            UnderAction = (bool)rdr["UnderAction"];
                            DisputeAct = (bool)rdr["DisputeAct"];
                            ManualAct = (bool)rdr["ManualAct"];
                            ByWhom = (string)rdr["ByWhom"];

                            ActionDtTm = (DateTime)rdr["ActionDtTm"];
                            ActionSes = (int)rdr["ActionSes"];

                            CurDes = rdr["CurDes"].ToString();
                            ErrAmount = (decimal)rdr["ErrAmount"];
                            ActionId = (int)rdr["ActionId"];

                            DrCust = (bool)rdr["DrCust"];
                            CrCust = (bool)rdr["CrCust"];
                            CustAccNo = rdr["CustAccNo"].ToString();

                            DrAtmCash = (bool)rdr["DrAtmCash"];
                            CrAtmCash = (bool)rdr["CrAtmCash"];
                            AccountNo1 = rdr["AccountNo1"].ToString();

                            DrAtmSusp = (bool)rdr["DrAtmSusp"];
                            CrAtmSusp = (bool)rdr["CrAtmSusp"];
                            AccountNo2 = rdr["AccountNo2"].ToString();

                            DrAccount3 = (bool)rdr["DrAccount3"];
                            CrAccount3 = (bool)rdr["CrAccount3"];
                            AccountNo3 = rdr["AccountNo3"].ToString();

                            ForeignCard = (bool)rdr["ForeignCard"];
                            MainOnly = (bool)rdr["MainOnly"];

                            UserComment = rdr["UserComment"].ToString();

                            Printed = (bool)rdr["Printed"];
                            DatePrinted = (DateTime)rdr["DatePrinted"];

                            CircularDesc = rdr["CircularDesc"].ToString();

                            //        CitId = (int)rdr["CitId"];
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
                    ErrorOutput = "An error occured in RRDMErrorsORExceptionsCharacteristics Class............. " + ex.Message;
                }
        }

        // Insert Error 
        public void InsertErrorCharacteristics()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[ErrorsIdCharacteristics] "
                   + "([ErrId],[ErrDesc],[ErrType],[DateInserted],"
                   + "[BankId],[TurboReconc],"
                   + "[NeedAction],"
                   + "[DrCust],[CrCust],[DrAtmCash],[CrAtmCash],[DrAtmSusp],[CrAtmSusp],"
                   + "[DrAccount3],[CrAccount3],"
                   + "[MainOnly],"
                   + "[CircularDesc],[Operator])"
                  + " VALUES "
                   + "(@ErrId,@ErrDesc,@ErrType,@DateInserted,"
                   + "@BankId,@TurboReconc,"
                   + "@NeedAction,"
                   + "@DrCust,@CrCust,@DrAtmCash,@CrAtmCash,@DrAtmSusp,@CrAtmSusp,"
                   + "@DrAccount3,@CrAccount3,"
                   + "@MainOnly,"
                   + "@CircularDesc,@Operator)"; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@ErrId", ErrId);
                        cmd.Parameters.AddWithValue("@ErrDesc", ErrDesc);
                        cmd.Parameters.AddWithValue("@ErrType", ErrType);

                        cmd.Parameters.AddWithValue("@DateInserted", DateInserted);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                       
                        cmd.Parameters.AddWithValue("@TurboReconc", TurboReconc);

                        cmd.Parameters.AddWithValue("@NeedAction", NeedAction);
                        
                        cmd.Parameters.AddWithValue("@DrCust", DrCust);
                        cmd.Parameters.AddWithValue("@CrCust", CrCust);

                        cmd.Parameters.AddWithValue("@DrAtmCash", DrAtmCash);
                        cmd.Parameters.AddWithValue("@CrAtmCash", CrAtmCash);
                       
                        cmd.Parameters.AddWithValue("@DrAtmSusp", DrAtmSusp);
                        cmd.Parameters.AddWithValue("@CrAtmSusp", CrAtmSusp);

                        cmd.Parameters.AddWithValue("@DrAccount3", DrAccount3);
                        cmd.Parameters.AddWithValue("@CrAccount3", CrAccount3);

                        cmd.Parameters.AddWithValue("@MainOnly", MainOnly);

                        cmd.Parameters.AddWithValue("@CircularDesc", CircularDesc);

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
                    ErrorOutput = "An error occured in RRDMErrorsORExceptionsCharacteristicsClass............. " + ex.Message;
                }
        }

        // 
        // UPDATE all old errors < 100 with main only
        // WHEN ERROR BECOMES OLD IT CANNOT INFLUENCE ATM BALANCES 
        // 
        public void UpdateErrorCharacteristics(string InOperator, int InErrId, string InBankId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[ErrorsIdCharacteristics] SET  "

                           + " ErrDesc=@ErrDesc,ErrType=@ErrType,DateInserted=@DateInserted,"
                           + " TurboReconc=@TurboReconc,"
                           + " NeedAction=@NeedAction,"
                           + " DrCust=@DrCust,CrCust=@CrCust,DrAtmCash=@DrAtmCash,CrAtmCash=@CrAtmCash,DrAtmSusp=@DrAtmSusp,CrAtmSusp=@CrAtmSusp,"
                           + " DrAccount3=@DrAccount3,CrAccount3=@CrAccount3,"
                           + " MainOnly=MainOnly ,"
                           + " CircularDesc=@CircularDesc "
                           + " WHERE Operator = @Operator AND ErrId = @ErrId AND BankId = @BankId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ErrId", InErrId);
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
                       
                        cmd.Parameters.AddWithValue("@ErrDesc", ErrDesc);
                        cmd.Parameters.AddWithValue("@ErrType", ErrType);

                        cmd.Parameters.AddWithValue("@DateInserted", DateInserted);
                       

                        cmd.Parameters.AddWithValue("@TurboReconc", TurboReconc);

                        cmd.Parameters.AddWithValue("@NeedAction", NeedAction);

                        cmd.Parameters.AddWithValue("@DrCust", DrCust);
                        cmd.Parameters.AddWithValue("@CrCust", CrCust);

                        cmd.Parameters.AddWithValue("@DrAtmCash", DrAtmCash);
                        cmd.Parameters.AddWithValue("@CrAtmCash", CrAtmCash);

                        cmd.Parameters.AddWithValue("@DrAtmSusp", DrAtmSusp);
                        cmd.Parameters.AddWithValue("@CrAtmSusp", CrAtmSusp);

                        cmd.Parameters.AddWithValue("@DrAccount3", DrAccount3);
                        cmd.Parameters.AddWithValue("@CrAccount3", CrAccount3);

                        cmd.Parameters.AddWithValue("@MainOnly", MainOnly);

                        cmd.Parameters.AddWithValue("@CircularDesc", CircularDesc);

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
                    ErrorOutput = "An error occured in RRDMErrorsORExceptionsCharacteristics Class............. " + ex.Message;
                }
        }

        //
        // DELETE Category
        //
        public void DeleteErrorId(int InErrId, string InBankId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[ErrorsIdCharacteristics] "
                            + " WHERE ErrId=@ErrId AND BankId=@BankId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrId", InErrId);
                        cmd.Parameters.AddWithValue("@BankId", InBankId);
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
                    ErrorOutput = "An error occured in RRDMErrorsORExceptionsCharacteristics Class............. " + ex.Message;
                }
        }
    }
}
