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

namespace RRDM4ATMsWin
{
    class RRDMErrorsClassWithActions
    {
        // Declarations 

        // Variables for reading errors 
        public int ErrNo;
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
    //    public int CurrCd; 
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

        int InTraceNumber;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 


        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass();
        RRDMAccountsClass Acc = new RRDMAccountsClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();

        string SqlString;
      

        // READ Error ID Record 
        public void ReadErrorsIDRecord(int InErrId, string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

          SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsIdCharacteristics] "
          + " WHERE Errid = @ErrId AND Operator=@Operator";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrId", InErrId);
                        cmd.Parameters.AddWithValue("@Operator", InOperator);

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
                    ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
                }
        }
        // Insert Error 
       public void InsertError()
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [ErrorsTable] ([ErrId],[ErrDesc],[ErrType],[AtmNo],[SesNo],[DateInserted],"
                   + "[BankId],[BranchId],[TurboReconc],[TraceNo],[CardNo],[TransNo],[TransType],[TransDescr],"
                   + "[DateTime],[NeedAction],[OpenErr],[FullCard],[UnderAction],[ManualAct],"
                   + "[ByWhom],[ActionDtTm],[ActionSes],[CurDes],[ErrAmount],[ActionId],"
                   + "[DrCust],[CrCust],[CustAccNo],[DrAtmCash],[CrAtmCash],[AccountNo1],[DrAtmSusp],[CrAtmSusp],[AccountNo2],"
                   + "[DrAccount3],[CrAccount3],[AccountNo3],[ForeignCard],[MainOnly],[UserComment],"
                   + "[Printed],[DatePrinted],[CircularDesc],[CitId], [Operator])"
                + " VALUES (@ErrId,@ErrDesc,@ErrType,@AtmNo,@SesNo,@DateInserted,"
                    + "@BankId,@BranchId,@TurboReconc,@TraceNo,@CardNo,@TransNo,@TransType,@TransDescr,"
                   + "@DateTime,@NeedAction,@OpenErr,@FullCard,@UnderAction,@ManualAct,"
                   + "@ByWhom,@ActionDtTm,@ActionSes,@CurDes,@ErrAmount,@ActionId,"
                   + "@DrCust,@CrCust,@CustAccNo,@DrAtmCash,@CrAtmCash,@AccountNo1,@DrAtmSusp,@CrAtmSusp,@AccountNo2,"
                   + "@DrAccount3,@CrAccount3,@AccountNo3,@ForeignCard, @MainOnly,@UserComment,"
                   + "@Printed,@DatePrinted,@CircularDesc,@CitId, @Operator)";

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

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", SesNo);

                        cmd.Parameters.AddWithValue("@DateInserted", DateInserted);

                        cmd.Parameters.AddWithValue("@BankId", BankId);
                    
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@TurboReconc", TurboReconc);

                        cmd.Parameters.AddWithValue("@TraceNo", TraceNo);
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@TransNo", TransNo);
                        cmd.Parameters.AddWithValue("@TransType", TransType);
                        cmd.Parameters.AddWithValue("@TransDescr", TransDescr);

                        cmd.Parameters.AddWithValue("@DateTime", DateTime);
                        cmd.Parameters.AddWithValue("@NeedAction", NeedAction);
                        cmd.Parameters.AddWithValue("@OpenErr", OpenErr);
                        cmd.Parameters.AddWithValue("@FullCard", FullCard);

                        cmd.Parameters.AddWithValue("@UnderAction", UnderAction); // 
                        cmd.Parameters.AddWithValue("@ManualAct", ManualAct);  //

                        cmd.Parameters.AddWithValue("@ByWhom", ByWhom);  //
                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);
                        cmd.Parameters.AddWithValue("@ActionSes", ActionSes); 
                       
                  
                        cmd.Parameters.AddWithValue("@CurDes", CurDes);
                        cmd.Parameters.AddWithValue("@ErrAmount", ErrAmount);
                        cmd.Parameters.AddWithValue("@ActionId", ActionId);
                        cmd.Parameters.AddWithValue("@DrCust", DrCust);
                        cmd.Parameters.AddWithValue("@CrCust", CrCust);
                        cmd.Parameters.AddWithValue("@CustAccNo", CustAccNo);

                        cmd.Parameters.AddWithValue("@DrAtmCash", DrAtmCash);
                        cmd.Parameters.AddWithValue("@CrAtmCash", CrAtmCash);
                        cmd.Parameters.AddWithValue("@AccountNo1", AccountNo1);

                        cmd.Parameters.AddWithValue("@DrAtmSusp", DrAtmSusp);
                        cmd.Parameters.AddWithValue("@CrAtmSusp", CrAtmSusp);
                        cmd.Parameters.AddWithValue("@AccountNo2", AccountNo2);

                        cmd.Parameters.AddWithValue("@DrAccount3", DrAccount3);
                        cmd.Parameters.AddWithValue("@CrAccount3", CrAccount3);
                        cmd.Parameters.AddWithValue("@AccountNo3", AccountNo3);


                        cmd.Parameters.AddWithValue("@ForeignCard", ForeignCard);
                        cmd.Parameters.AddWithValue("@MainOnly", MainOnly);

                        cmd.Parameters.AddWithValue("@UserComment", UserComment);
                        cmd.Parameters.AddWithValue("@Printed", Printed);
                        cmd.Parameters.AddWithValue("@DatePrinted", DatePrinted);

                        cmd.Parameters.AddWithValue("@CircularDesc", CircularDesc);

                        cmd.Parameters.AddWithValue("@CitId", CitId);
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
                    ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
                }
        }
        //
       // READ Last Number Inserted
        //
       public void ReadLastErrorNo(string InOperator, string InAtmNo, int InSesNo)
       {
           RecordFound = false;
           ErrorFound = false;
           ErrorOutput = ""; 

           SqlString = "SELECT *"
         + " FROM [dbo].[ErrorsTable] "
         + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND SesNo = @SesNo ";
           using (SqlConnection conn =
                         new SqlConnection(connectionString))
               try
               {
                   conn.Open();
                   using (SqlCommand cmd =
                       new SqlCommand(SqlString, conn))
                   {
                       cmd.Parameters.AddWithValue("@Operator", InOperator);
                       cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                       cmd.Parameters.AddWithValue("@SesNo", InSesNo);


                       // Read table 

                       SqlDataReader rdr = cmd.ExecuteReader();

                       while (rdr.Read())
                       {
                           RecordFound = true;
                           // Read error Details

                           ErrNo = (int)rdr["ErrNo"];                 
                           ErrDesc = rdr["ErrDesc"].ToString();                         
                           AtmNo = (string)rdr["AtmNo"];
                           SesNo = (int)rdr["SesNo"];
                          
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
                   ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
               }
       }

        // READ Error specific 
        public void ReadErrorsTableSpecific(int InErrNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

               SqlString = "SELECT *"
                + " FROM [dbo].[ErrorsTable] "
                + " WHERE ErrNo = @ErrNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                           RecordFound = true;

                            // Read error Details


                            ErrNo = (int)rdr["ErrNo"];
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

                            ForeignCard = (bool)rdr["ForeignCard"];
                            MainOnly = (bool)rdr["MainOnly"];

                            UserComment = rdr["UserComment"].ToString();

                            Printed = (bool)rdr["Printed"];
                            DatePrinted = (DateTime)rdr["DatePrinted"];

                            CircularDesc = rdr["CircularDesc"].ToString();

                            CitId = (string)rdr["CitId"];

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
                    ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
                }
        }

        // READ Errors by card number and trace number 
        //
        public void ReadErrorsByCardNoAndTrace(string InOperator, string InCardNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsTable] "
          + " WHERE Operator = @Operator AND CardNo = @CardNo AND TraceNo =@TraceNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@CardNo", InCardNo);
                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read error Details

                            ErrNo = (int)rdr["ErrNo"];
                            ErrId = (int)rdr["ErrId"];
                            ErrDesc = rdr["ErrDesc"].ToString();
                            ErrType = (int)rdr["ErrType"];

                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            DateInserted = (DateTime)rdr["DateInserted"];

                            BankId = rdr["BankId"].ToString();
                   //         Prive = (bool)rdr["Prive"];
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

                            ForeignCard = (bool)rdr["ForeignCard"];
                            MainOnly = (bool)rdr["MainOnly"];

                            UserComment = rdr["UserComment"].ToString();

                            Printed = (bool)rdr["Printed"];
                            DatePrinted = (DateTime)rdr["DatePrinted"];

                            CircularDesc = rdr["CircularDesc"].ToString();

                            CitId = (string)rdr["CitId"];

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
                    ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
                }
        }

        // READ Errors by card number ONLY  - with Full card or Bin only  
        //
        public void ReadErrorsByCardNo(string InOperator, int InSelectMode, string InEnteredId, DateTime InDateStart, DateTime InDateEnd, string InCardNoBin)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            if (InSelectMode == 8) // Crad 
            {
                SqlString = "SELECT *"
            + " FROM [dbo].[ErrorsTable] "
            + " WHERE Operator=@Operator and DateTime >=@InDateStart AND DateTime <= @InDateEnd AND (CardNo = @CardNo OR  CardNo = @CardNoBin)  "; 
              
            }

            if (InSelectMode == 9) // Account number  
            {
                SqlString = "SELECT *"
            + " FROM [dbo].[ErrorsTable] "
            + " WHERE Operator=@Operator and DateTime >=@InDateStart AND DateTime <= @InDateEnd  and CustAccNo =@AccNo  ";
             
            }

            if (InSelectMode == 10) // Trace Number which is numeric 
            {

                InTraceNumber = Convert.ToInt32(InEnteredId);

                SqlString = "SELECT *"
           + " FROM [dbo].[ErrorsTable] "
           + " WHERE Operator=@Operator and TraceNo=@TraceNo  "; 
            

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
                        cmd.Parameters.AddWithValue("@InDateStart", InDateStart);
                        cmd.Parameters.AddWithValue("@InDateEnd", InDateEnd);
                        if (InSelectMode == 8) // Card 
                        {
                            cmd.Parameters.AddWithValue("@CardNo", InEnteredId);
                            cmd.Parameters.AddWithValue("@CardNoBin", InCardNoBin);
                        }

                        if (InSelectMode == 9) // Account
                        {
                            cmd.Parameters.AddWithValue("@AccNo", InEnteredId);
                        }
                        if (InSelectMode == 10) // Trace Number  
                        {
                            cmd.Parameters.AddWithValue("@TraceNo", InTraceNumber);
                        }
                       

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read error Details

                            ErrNo = (int)rdr["ErrNo"];
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

                            //          CurrCd = (int)rdr["CurrCd"];
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

                            ForeignCard = (bool)rdr["ForeignCard"];
                            MainOnly = (bool)rdr["MainOnly"];

                            UserComment = rdr["UserComment"].ToString();

                            Printed = (bool)rdr["Printed"];
                            DatePrinted = (DateTime)rdr["DatePrinted"];

                            CircularDesc = rdr["CircularDesc"].ToString();

                            CitId = (string)rdr["CitId"];

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
                    ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
                }
        }

        // READ Error specific Trace No 
        public void ReadErrorsTableSpecificTraceNo(string InOperator, string InAtmNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsTable] "
          + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND TraceNo = @TraceNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read error Details


                            ErrNo = (int)rdr["ErrNo"];
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

                            //          CurrCd = (int)rdr["CurrCd"];
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

                            ForeignCard = (bool)rdr["ForeignCard"];
                            MainOnly = (bool)rdr["MainOnly"];

                            UserComment = rdr["UserComment"].ToString();

                            Printed = (bool)rdr["Printed"];
                            DatePrinted = (DateTime)rdr["DatePrinted"];

                            CircularDesc = rdr["CircularDesc"].ToString();

                            CitId = (string)rdr["CitId"];

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
                    ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
                }
        }
        //
        // READ Error specific by TranNo
        //
        public void ReadErrorsTableSpecificByTransNo(int InTransNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[ErrorsTable] "
          + " WHERE TransNo = @TransNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TransNo", InTransNo);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                             RecordFound = true;

                            // Read error Details


                            ErrNo = (int)rdr["ErrNo"];
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

                            ForeignCard = (bool)rdr["ForeignCard"];
                            MainOnly = (bool)rdr["MainOnly"];

                            UserComment = rdr["UserComment"].ToString();

                            Printed = (bool)rdr["Printed"];
                            DatePrinted = (DateTime)rdr["DatePrinted"];

                            CircularDesc = rdr["CircularDesc"].ToString();

                            CitId = (string)rdr["CitId"];

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
                    ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
                }
        }

        // UPDATE ERROR TABLE   
public void UpdateErrorsTableSpecific(int InErrNo)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE ErrorsTable SET OpenErr=@OpenErr,"
                            + "CardNo=@CardNo, CustAccNo=@CustAccNo,"
                            + "FullCard=@FullCard, UnderAction=@UnderAction, DisputeAct=@DisputeAct,"
                            + " ManualAct = @ManualAct, UserComment = @UserComment,"
                            + " ByWhom = @ByWhom, ActionDtTm = @ActionDtTm, ActionSes = @ActionSes,"
                                       + " Printed = @Printed, DatePrinted = @DatePrinted,CitId = @CitId" 
                            + " WHERE ErrNo=@ErrNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@ErrNo", InErrNo);
                        
                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@CustAccNo", CustAccNo);

                        cmd.Parameters.AddWithValue("@FullCard", FullCard);
                        cmd.Parameters.AddWithValue("@UnderAction", UnderAction);
                        cmd.Parameters.AddWithValue("@DisputeAct", DisputeAct);
                       
                        cmd.Parameters.AddWithValue("@ManualAct", ManualAct);
                        cmd.Parameters.AddWithValue("@UserComment", UserComment);

                        cmd.Parameters.AddWithValue("@ByWhom", ByWhom);
                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);
                        cmd.Parameters.AddWithValue("@ActionSes", ActionSes);
 
                        cmd.Parameters.AddWithValue("@Printed", Printed);
                        cmd.Parameters.AddWithValue("@DatePrinted", DatePrinted);
                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@OpenErr", OpenErr);

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
                    ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
                }
        }

// 
// UPDATE all old errors < 100 with main only
        // WHEN ERROR BECOMES OLD IT CANNOT INFLUENCE ATM BALANCES 
        // 
public void UpdateOldErrorsWithMainOnly (string InOperator, string InAtmNo, int InSesNo)
{
   
    ErrorFound = false;
    ErrorOutput = ""; 

    using (SqlConnection conn =
        new SqlConnection(connectionString))
        try
        {
            conn.Open();
            using (SqlCommand cmd =
                new SqlCommand("UPDATE ErrorsTable SET  "
                    + "MainOnly = @MainOnly "
                     + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND UnderAction = 0 AND SesNo < @SesNo ", conn))
            {
                cmd.Parameters.AddWithValue("@Operator", InOperator);
                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                cmd.Parameters.AddWithValue("@SesNo", InSesNo);
              
                cmd.Parameters.AddWithValue("@MainOnly", true);
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
            ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
        }
}
        //
// READ ALL ACTIVE for ATM AND COUNT THE NUMBER and The ones action has been taken   
//
public void ReadAllErrorsTableForCounters(string InOperator, string InAtmNo)
{
    RecordFound = false;
    ErrorFound = false;
    ErrorOutput = ""; 

    NumOfErrors = 0;
    ErrUnderAction = 0;
    ErrUnderManualAction = 0;

    string SqlString = "SELECT *"
  + " FROM [dbo].[ErrorsTable] "
  + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND OpenErr=1";
    using (SqlConnection conn =
                  new SqlConnection(connectionString))
        try
        {
            conn.Open();
            using (SqlCommand cmd =
                new SqlCommand(SqlString, conn))
            {
                cmd.Parameters.AddWithValue("@Operator", InOperator);
                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                // Read table 

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    RecordFound = true;

                    ErrNo = (int)rdr["ErrNo"];
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

                    ForeignCard = (bool)rdr["ForeignCard"];
                    MainOnly = (bool)rdr["MainOnly"];

                    UserComment = rdr["UserComment"].ToString();

                    Printed = (bool)rdr["Printed"];
                    DatePrinted = (DateTime)rdr["DatePrinted"];

                    CircularDesc = rdr["CircularDesc"].ToString();

                    CitId = (string)rdr["CitId"];

                    Operator = (string)rdr["Operator"];

                    NumOfErrors = NumOfErrors + 1 ;
                    if ( UnderAction == true) ErrUnderAction = ErrUnderAction + 1;
                    if ( ManualAct == true) ErrUnderManualAction = ErrUnderManualAction + 1;

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
            ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
        }
}

// READ ALL ACTIVE for ATM+Repl Cycle number  AND COUNT THE NUMBER and The ones action has been taken   
// EXCLUDE THE DEPOSITS 

public void ReadAllErrorsTableForCounterReplCycle(string InOperator, string InAtmNo, int InSesNo)
{
    RecordFound = false;
    ErrorFound = false;
    ErrorOutput = ""; 

    NumOfErrors = 0;
    ErrUnderAction = 0;
    ErrUnderManualAction = 0;

    NumOfErrorsLess200 = 0; 

    string SqlString = "SELECT *"
  + " FROM [dbo].[ErrorsTable] "
  + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND SesNo=@SesNo ";
    using (SqlConnection conn =
                  new SqlConnection(connectionString))
        try
        {
            conn.Open();
            using (SqlCommand cmd =
                new SqlCommand(SqlString, conn))
            {
                cmd.Parameters.AddWithValue("@Operator", InOperator);
                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                // Read table 

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    RecordFound = true;

                    ErrNo = (int)rdr["ErrNo"];
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

                    //       CurrCd = (int)rdr["CurrCd"];
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

                    ForeignCard = (bool)rdr["ForeignCard"];
                    MainOnly = (bool)rdr["MainOnly"];

                    UserComment = rdr["UserComment"].ToString();

                    Printed = (bool)rdr["Printed"];
                    DatePrinted = (DateTime)rdr["DatePrinted"];

                    CircularDesc = rdr["CircularDesc"].ToString();

                    CitId = (string)rdr["CitId"];

                    Operator = (string)rdr["Operator"];

                    NumOfErrors = NumOfErrors + 1;

                    if (ErrType == 1 || ErrType == 2) NumOfErrorsLess200 = NumOfErrorsLess200 + 1;
                    if (UnderAction == true) ErrUnderAction = ErrUnderAction + 1;
                    if (ManualAct == true) ErrUnderManualAction = ErrUnderManualAction + 1;

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
            ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
        }
}


// READ Errors TO IN THE TURBO PROCESS WITH PURPOSE TO UPDATE THEM WITH UNDER ACTION = True  
//
public void ReadAllErrorsTableForTurboAction(string InOperator,  string InAtmNo)
{
    RecordFound = false;
    ErrorFound = false;
    ErrorOutput = ""; 

    string SqlString = "SELECT *"
  + " FROM [dbo].[ErrorsTable] "
  + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND OpenErr=1 AND NeedAction=1";
    using (SqlConnection conn =
                  new SqlConnection(connectionString))
        try
        {
            conn.Open();
            using (SqlCommand cmd =
                new SqlCommand(SqlString, conn))
            {
                cmd.Parameters.AddWithValue("@Operator", InOperator);
                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                // Read table 

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    RecordFound = true;

                    // Read error Details
                    ErrNo = (int)rdr["ErrNo"];
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

                    //       CurrCd = (int)rdr["CurrCd"];
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

                    ForeignCard = (bool)rdr["ForeignCard"];
                    MainOnly = (bool)rdr["MainOnly"];

                    UserComment = rdr["UserComment"].ToString();

                    Printed = (bool)rdr["Printed"];
                    DatePrinted = (DateTime)rdr["DatePrinted"];

                    CircularDesc = rdr["CircularDesc"].ToString();

                    CitId = (string)rdr["CitId"];

                    // UPDATE ERROR TABLE AS A RESULT OF TURBO

                    UpdateErrorsTableActionTaken(ErrNo);
                    
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
            ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
        }
}

// AT CLOSING OF RECONCILIATION PROCESS READ Errors TO CREATE THE TRANSACTIONS  
        // ALSO CLOSE ERRORS FOR WHICH MANUAL ACTION WILL BE TAKEN 
//
public void ReadAllErrorsTableForPostingTrans(string InOperator, string InAtmNo, string InUserId, int InActionSes)
{
 //   RRDMPostedTrans Cs = new RRDMPostedTrans();

    int TargetSystem ;

    DateTime DateTimeH;
  
    int AuthCodeH;
    int RefNumbH;
    int RemNoH;

    RecordFound = false;
    ErrorFound = false;
    ErrorOutput = ""; 

    string SqlString = "SELECT *"
  + " FROM [dbo].[ErrorsTable] "
  + " WHERE Operator = @Operator AND AtmNo = @AtmNo AND OpenErr=1"
           + "   AND ((NeedAction=1 AND UnderAction = 1) OR (NeedAction=1 AND ManualAct = 1)) ";
    using (SqlConnection conn =
                  new SqlConnection(connectionString))
        try
        {
            conn.Open();
            using (SqlCommand cmd =
                new SqlCommand(SqlString, conn))
            {
                cmd.Parameters.AddWithValue("@Operator", InOperator);
                cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                // Read table 

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    RecordFound = true;

                    // Read error Details

                            ErrNo = (int)rdr["ErrNo"];
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

                            ForeignCard = (bool)rdr["ForeignCard"];
                            MainOnly = (bool)rdr["MainOnly"];

                            UserComment = rdr["UserComment"].ToString();

                            Printed = (bool)rdr["Printed"];
                            DatePrinted = (DateTime)rdr["DatePrinted"];

                            CircularDesc = rdr["CircularDesc"].ToString();

                            CitId = (string)rdr["CitId"];

                            if (ManualAct == true)
                            {
                                ReadErrorsTableSpecific(ErrNo);
                                ByWhom = InUserId;
                                ActionDtTm = DateTime.Now;
                                ActionSes = InActionSes; 
                                OpenErr = false;
                                UpdateErrorsTableSpecific(ErrNo); 
                            }

                            if (DisputeAct == true)
                            {
                                
                            }
                    // CLEAR OLD TRANSACTION TO BE POSTED 
                            if (OpenErr == true)
                            {
                                Tp.DeleteOldTransToBePosted(AtmNo, ErrNo);
                            }
                            

                            if (UnderAction == true)
                            {

                                // FIND OTHER NEEDED INFORMATION
                                // Such as Traget system, If foreign find authorization code etc. 


                                Tp.ReadInPoolAtmTrace(AtmNo, TraceNo);

                                TargetSystem = Tp.SystemTarget;

                                DateTime = Tp.AtmDtTime;

                                DateTimeH = DateTime;

                                AuthCodeH = 0;
                                RefNumbH = 0;
                                RemNoH = 0;

                                RRDMHostTransClass Ht = new RRDMHostTransClass();

                                Ht.ReadHostTransTraceNo(InOperator, AtmNo, TraceNo); // READ HOST FILE TO GET HOST DETAILS 

                                if (Ht.RecordFound == true) // If Found Update fields 
                                {
                                    DateTimeH = Ht.HostDtTime;
                                    //     CardNo = Ht.CardNo; // FULL CARD NUMBER 
                                    //     CustAccNo = Ht.AccNo; // Full account Number
                                    if (TargetSystem == 1) // JCC 
                                    {
                                        AuthCodeH = Ht.AuthCode;
                                        RefNumbH = Ht.RefNo;
                                        RemNoH = Ht.RemNo;
                                    }
                                }

                                // Insert Transactions (TWO Trans) In TransTo BePosted 

                                if (DrCust == true & CrAtmCash == true & DisputeAct == false)
                                {
                                    // DElete old for this error 
                                 //   Tp.DeleteOldTransToBePosted(WAtmNo, ErrNo);
                                    // DR CUSTOMER 

                                    Tp.TranOrigin = 3; // ORIGIN RECONCILIATION
                                    Tp.ErrNo = ErrNo;
                                    Tp.AtmNo = InAtmNo;
                                    Tp.SesNo = SesNo;
                                    Tp.BankId = BankId;
                           
                                    Tp.AtmTraceNo = TraceNo;

                                    Tp.BranchId = BranchId;
                                    Tp.AtmDtTime = DateTime;
                                    Tp.HostDtTime = DateTimeH;
                                    Tp.SystemTarget = TargetSystem;

                                    Tp.CardNo = CardNo;
                                    Tp.CardOrigin = 5; // Find OUT ... 

                                    // First Entry 
                                    Tp.TransType = 11; // MAKE REVERSE
                                    Tp.TransDesc = " Atm Rever For:" + DateTime.ToString();
                                    
                                    // If Jcc then use JCC GL SAME for AMex
                                    if (TargetSystem == 1 || TargetSystem == 3) // JCC=1 OR AMEX=3 
                                    {
                                        if (TargetSystem == 1) Acc.ReadAndFindAccount("1000", InOperator, InAtmNo, CurDes, "ATM Suspense");
                                        if (TargetSystem == 3) Acc.ReadAndFindAccount("1000", InOperator, InAtmNo, CurDes, "ATM American Express");

                                        if (Acc.RecordFound == false)
                                        {
                                            ErrorFound = false;
                                            ErrorOutput = "Account not found for ATMNo: " + InAtmNo; 
                                          //  MessageBox.Show("Account not found for ATMNo: " + InAtmNo);
                                            return; 
                                        }
                                        Tp.AccNo = Acc.AccNo;  // Cash account No
                                        Tp.GlEntry = true; 
                                    }
                                    else
                                    {
                                        Tp.AccNo = CustAccNo;
                                        if (Tp.AccNo == "") Tp.AccNo = "Not Available";
                                        Tp.GlEntry = false;
                                    }

                                   
                                    // Second Entry
                                    
                                    if (MainOnly == false)
                                    {
                                        Tp.TransType2 = 21; // MAKE REVERSE 
                                    }
                                    else
                                    {
                                        Tp.TransType2 = 22; // MAINFRAME CASH ONLY  
                                    }
                                    Tp.TransDesc2 = " CASH Rever-Trace: " + TraceNo;
                                    Acc.ReadAndFindAccount("1000", InOperator, InAtmNo, CurDes, "ATM Cash");
                                    if (Acc.RecordFound == false)
                                    {
                                        ErrorFound = false;
                                        ErrorOutput = "Account not found for ATMNo: " + InAtmNo;
                                        //  MessageBox.Show("Account not found for ATMNo: " + InAtmNo);
                                        return; 
                                        
                                       // MessageBox.Show("Account not found for ATMNo: " + InAtmNo);
                                    }
                                    Tp.AccNo2 = Acc.AccNo;  // Cash account No Cash account No
                                    Tp.GlEntry2 = true;
                                    // End Second Entry 

                                    Tp.CurrDesc = CurDes;
                                    Tp.TranAmount = ErrAmount;
                                    Tp.AuthCode = AuthCodeH;
                                    Tp.RefNumb = RefNumbH;
                                    Tp.RemNo = RemNoH;
                                    Tp.TransMsg = UserComment;
                                    Tp.AtmMsg = "";
                                    Tp.OpenDate = DateTime.Now;
                                    Tp.OpenRecord = true;
                                    Tp.Operator = Ac.Operator; 

                                    Tp.InsertTransToBePosted(AtmNo, Tp.ErrNo);

                                  
                                    

                                }

                                if (CrCust == true & DrAtmCash == true & DisputeAct == false)
                                {
                                    // DElete old for this error 
                                 //   Tp.DeleteOldTransToBePosted(WAtmNo, ErrNo);
                                    // CREATE A TRANSACTION TO CR CUSTOMER 

                                    Tp.TranOrigin = 3; // ORIGIN RECONCILIATION
                                    Tp.ErrNo = ErrNo;
                                    Tp.AtmNo = InAtmNo;
                                    Tp.SesNo = SesNo;
                                    Tp.BankId = BankId;
                         
                                    Tp.AtmTraceNo = TraceNo;

                                    Tp.BranchId = BranchId;
                                    Tp.AtmDtTime = DateTime;
                                    Tp.HostDtTime = DateTimeH;
                                    Tp.SystemTarget = TargetSystem;

                                    Tp.CardNo = CardNo;
                                    Tp.CardOrigin = 5; // Find OUT

                                    // First Entry 
                                    Tp.TransType = 21; // MAKE REVERSE 
                                    Tp.TransDesc = " Atm Rever For: " + DateTime.ToShortDateString();

                                    if (TargetSystem == 1 || TargetSystem == 3) // JCC=1 OR AMEX=3 
                                    {
                                        if (TargetSystem == 1) Acc.ReadAndFindAccount("1000", InOperator, InAtmNo, CurDes, "ATM Suspense");
                                        if (TargetSystem == 3) Acc.ReadAndFindAccount("1000", InOperator, InAtmNo, CurDes, "ATM American Express");

                                        if (Acc.RecordFound == false)
                                        {
                                            ErrorFound = false;
                                            ErrorOutput = "Account not found for ATMNo: " + InAtmNo;
                                            return; 
                                          //  MessageBox.Show("Account not found for ATMNo: " + InAtmNo);
                                        }
                                        Tp.AccNo = Acc.AccNo;  // Cash account No
                                        Tp.GlEntry = true;
                                    }
                                    else
                                    {
                                        Tp.AccNo = CustAccNo;
                                        if (Tp.AccNo == "") Tp.AccNo = "Not Available";
                                        Tp.GlEntry = false;
                                    }
                                   
                                    // Second Transaction 
                                    // CREATE A TRANSACTION TO DR CASH
                                   

                                    if (MainOnly == false)
                                    {
                                        Tp.TransType2 = 11; // MAKE REVERSE 
                                    }
                                    else
                                    {
                                        Tp.TransType2 = 12; // MAINFRAME CASH ONLY  
                                    }

                                    Tp.TransDesc2 = " CASH Rever-Trace: " + TraceNo;

                                    Acc.ReadAndFindAccount("1000", InOperator, InAtmNo, CurDes, "ATM Cash");
                                    if (Acc.RecordFound == false)
                                    {
                                        ErrorFound = false;
                                        ErrorOutput = "Account not found for ATMNo: " + InAtmNo;
                                        return; 
                                       // MessageBox.Show("Account not found for ATMNo: " + InAtmNo);
                                    }
                                    Tp.AccNo2 = Acc.AccNo;  // Cash account No
                                    Tp.GlEntry2 = true;
                                    // End of second 

                                    Tp.CurrDesc = CurDes;
                                    Tp.TranAmount = ErrAmount;
                                    Tp.AuthCode = AuthCodeH;
                                    Tp.RefNumb = RefNumbH;
                                    Tp.RemNo = RemNoH;
                                    Tp.TransMsg = UserComment;
                                    Tp.AtmMsg = "";
                                    Tp.OpenDate = DateTime.Now;
                                    Tp.OpenRecord = true;
                                    Tp.Operator = InOperator;

                                    Tp.InsertTransToBePosted(AtmNo, Tp.ErrNo);

                                    

                                }

                                if ((DrAtmCash == true & CrAtmSusp == true) || (CrCust == true & DrAtmCash == true & DisputeAct == true))
                                {
                                    // DElete old for this error 
                                //    Tp.DeleteOldTransToBePosted(WAtmNo, ErrNo);
                                    // CREATE A TRANSACTION TO DR AtmCash
                                    // CREATE A TRANSACTION TO CR Suspense


                                    Tp.TranOrigin = 3; // ORIGIN RECONCILIATION
                                    Tp.ErrNo = ErrNo;
                                    Tp.AtmNo = InAtmNo;
                                    Tp.SesNo = SesNo;
                                    Tp.BankId = BankId;
                              
                                    Tp.AtmTraceNo = TraceNo;

                                    Tp.BranchId = BranchId;
                                    Tp.AtmDtTime = DateTime;
                                    Tp.HostDtTime = DateTimeH;
                                    Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER 

                                    Tp.CardNo = "N/A";
                                    Tp.CardOrigin = 5; // Find OUT

                                    // First Entry 

                                    if (MainOnly == false)
                                    {
                                        Tp.TransType = 11; // MAKE REVERSE 
                                    }
                                    else
                                    {
                                        Tp.TransType = 12; // MAINFRAME CASH ONLY  
                                    }

                                    Tp.TransDesc = " Transfer to Suspense";

                                    Acc.ReadAndFindAccount("1000", InOperator, InAtmNo, CurDes, "ATM Cash");
                                    if (Acc.RecordFound == false)
                                    {
                                        ErrorFound = false;
                                        ErrorOutput = "Account not found for ATMNo: " + InAtmNo;
                                        return; 
                                       // MessageBox.Show("Account not found for ATMNo: " + InAtmNo);
                                    }
                                    Tp.AccNo = Acc.AccNo;  // Cash account No
                                    Tp.GlEntry = true;

                                    // Second Entry 
                                    // CREATE A TRANSACTION TO CR SUSPENSE 

                                    Tp.TransType2 = 21; // MAKE REVERSE 

                                    if (MainOnly == false)
                                    {
                                        Tp.TransType2 = 21; // MAKE REVERSE 
                                    }
                                    else
                                    {
                                        Tp.TransType2 = 22; // MAINFRAME CASH ONLY  
                                    }
                                    Tp.TransDesc2 = " Transfer from ATM Cash";

                                    Acc.ReadAndFindAccount("1000", InOperator, InAtmNo, CurDes, "ATM Suspense");
                                    if (Acc.RecordFound == false)
                                    {
                                        ErrorFound = false;
                                        ErrorOutput = "Account not found for ATMNo: " + InAtmNo;
                                        return; 
                                       // MessageBox.Show("Account not found for ATMNo: " + InAtmNo);
                                    }
                                    Tp.AccNo2 = Acc.AccNo;  // Suspense account No
                                    Tp.GlEntry2 = true;

                                    //         Tp.CurrCode = CurrCd;
                                    Tp.CurrDesc = CurDes;
                                    Tp.TranAmount = ErrAmount;
                                    Tp.AuthCode = AuthCodeH;
                                    Tp.RefNumb = RefNumbH;
                                    Tp.RemNo = RemNoH;
                                    Tp.TransMsg = UserComment;
                                    Tp.AtmMsg = "";
                                    Tp.OpenDate = DateTime.Now;
                                    Tp.OpenRecord = true;
                                    Tp.Operator = InOperator;

                                    Tp.InsertTransToBePosted(AtmNo, Tp.ErrNo);

                                    

                                }

                                if ((CrAtmCash == true & DrAtmSusp == true) || (DrCust == true & CrAtmCash == true & DisputeAct == false))
                                {
                                    // DElete old for this error 
                                 //   Tp.DeleteOldTransToBePosted(WAtmNo, ErrNo);
                                    // CREATE A TRANSACTION TO CR CASH

                                
                                    Tp.TranOrigin = 3; // ORIGIN RECONCILIATION
                                    Tp.ErrNo = ErrNo;
                                    Tp.AtmNo = InAtmNo;
                                    Tp.SesNo = SesNo;
                                    Tp.BankId = BankId;
                        
                                    Tp.AtmTraceNo = TraceNo;

                                    Tp.BranchId = BranchId;
                                    Tp.AtmDtTime = DateTime;
                                    Tp.HostDtTime = DateTimeH;
                                    Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER
                                    Tp.CardNo = "";
                                    Tp.CardOrigin = 5; // Find OUT

                                    // First Entry 

                                    if (MainOnly == false)
                                    {
                                        Tp.TransType = 21; // MAKE REVERSE 
                                    }
                                    else
                                    {
                                        Tp.TransType = 22; // MAINFRAME CASH ONLY  
                                    }

                                    Tp.TransDesc = " Transfer to Suspense";

                                    Acc.ReadAndFindAccount("1000", InOperator, InAtmNo, CurDes, "ATM Cash");
                                    if (Acc.RecordFound == false)
                                    {
                                        ErrorFound = false;
                                        ErrorOutput = "Account not found for ATMNo: " + InAtmNo;
                                        return; 
                                      //  MessageBox.Show("Account not found for ATMNo: " + InAtmNo);
                                    }
                                    Tp.AccNo = Acc.AccNo;  // Cash account No
                                    Tp.GlEntry = true;

                                    // Second Entry 
                                   

                                    if (MainOnly == false)
                                    {
                                        Tp.TransType2 = 11; // MAKE REVERSE 
                                    }
                                    else
                                    {
                                        Tp.TransType2 = 12; // MAINFRAME CASH ONLY  
                                    }

                                    Tp.TransDesc2 = " Transfer from ATM Cash";

                                    Acc.ReadAndFindAccount("1000", InOperator, InAtmNo, CurDes, "ATM Suspense");
                                    if (Acc.RecordFound == false)
                                    {
                                        ErrorFound = false;
                                        ErrorOutput = "Account not found for ATMNo: " + InAtmNo;
                                        return; 
                                       // MessageBox.Show("Account not found for ATMNo: " + InAtmNo);
                                    }
                                    Tp.AccNo2 = Acc.AccNo;  // Suspense account No
                                    Tp.GlEntry2 = true;

                                    Tp.CurrDesc = CurDes;
                                    Tp.TranAmount = ErrAmount;
                                    Tp.AuthCode = AuthCodeH;
                                    Tp.RefNumb = RefNumbH;
                                    Tp.RemNo = RemNoH;
                                    Tp.TransMsg = UserComment;
                                    Tp.AtmMsg = "";
                                    Tp.OpenDate = DateTime.Now;
                                    Tp.OpenRecord = true;
                                    Tp.Operator = InOperator;

                                    Tp.InsertTransToBePosted(AtmNo, Tp.ErrNo);


                                }
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
            ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
        }
}
// 
// This is called from Disputes 
//
public void CreateTransTobepostedfromDisputes(string InUserId, int InDispTranNo, int InTranType, decimal InAmount)
{
    //
// Create a Credit or a Debit to customer AS A RESULT OF a dispute 
    //
    //
          
           ErrorFound = false;
           ErrorOutput = ""; 
                        // Insert Transactions (TWO Trans) In TransTo BePosted 

                       RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
                       RRDMDisputeTrasactionClass Dt = new RRDMDisputeTrasactionClass();

                       Dt.ReadDisputeTran(InDispTranNo); 

                       Tp.ReadTranSpecific(Dt.DbTranNo);

                      //
                      // If InTranType = 21 means CRCust if InTranType = 11 then means DRCust
                      //

                       if (InTranType == 21)
                       {
                           // CREATE A TRANSACTION TO CR CUSTOMER 
                           // CREATE A TRANSACTION TO DR CASH
                           Tp.TranOrigin = 8; // From Dispute 

                           Tp.CardNo = Dt.CardNo;

                           Tp.TransType = 21; // MAKE REVERSE

                           Tp.TransDesc = "Result of Dispute:" + Dt.DisputeNumber.ToString();
                           Tp.GlEntry = false; 

                           // Second Entry 
                           Tp.TransType2 = 11; // MAKE REVERSE 

                           Tp.TransDesc2 = "Result of Dispute:" + Dt.DisputeNumber.ToString();

                           Acc.ReadAndFindAccount("1000", Tp.Operator, Tp.AtmNo, Tp.CurrDesc, "Disputes Acc");
                           if (Acc.RecordFound == false)
                           {
                               ErrorFound = false;
                               ErrorOutput = "Account not found for ATMNo: " + Tp.AtmNo; 
                               return; 
                           }
                           Tp.AccNo2 = Acc.AccNo;  // Cash account No Cash account No
                           Tp.GlEntry2 = true;

                           Tp.TranAmount = InAmount;

                           Tp.OpenDate = DateTime.Now;

                           Tp.TransMsg = Dt.ActionComment;

                           Tp.DisputeNo = Dt.DisputeNumber;
                           Tp.DispTranNo = Dt.DispTranNo;
                           Tp.Operator = Acc.Operator;

                           Tp.OpenRecord = true;

                           Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo);


                       }
                        if (InTranType == 11)
                        {
                            // CREATE A TRANSACTION TO DR CUSTOMER 
                            // CREATE A TRANSACTION TO CR CASH

                             Tp.TranOrigin = 8; // From Dispute 

                             Tp.CardNo = Dt.CardNo;
 
                             Tp.TransType = 11; // MAKE REVERSE

                             Tp.TransDesc = "Result of Dispute:" + Dt.DisputeNumber.ToString();

                             Tp.GlEntry = false; 

                            // SECOND ENTRY 
                             Tp.TransType2 = 21; // MAKE REVERSE 
                             Tp.TransDesc2 = "Result of Dispute:" + Dt.DisputeNumber.ToString();

                             Acc.ReadAndFindAccount("1000", Tp.Operator, Tp.AtmNo, Tp.CurrDesc, "Disputes Acc");
                             if (Acc.RecordFound == false)
                             {
                                 ErrorFound = false;
                                 ErrorOutput = "Account not found for ATMNo: " + Tp.AtmNo; 
                                 
                                 return; 
                             }
                             Tp.AccNo2 = Acc.AccNo;  // Cash account No Cash account No
                             Tp.GlEntry2 = true;
                           
                             Tp.TranAmount = InAmount;

                             Tp.OpenDate = DateTime.Now;

                             Tp.TransMsg = Dt.ActionComment;

                             Tp.DisputeNo = Dt.DisputeNumber;
                             Tp.DispTranNo = Dt.DispTranNo;
                             Tp.Operator = Acc.Operator;

                             Tp.OpenRecord = true;

                            Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo);
                            
                        }                    

}

// 
// This is called from Form51 - Replenishment  
//
public void CreateTransTobepostedfromReplenishment(string InOperator, string InAtmNo, int InSesNo, string InUserId, 
                                                   decimal OutCassetteAmnt, decimal InCassetteAmnt)
{
    //
    // At the end of Replenishment we create two sets of transactions
    // One for the money taken out and other for the money in 
    //

    ErrorFound = false;
    ErrorOutput = "";
    string WUser = InUserId; 

    Ac.ReadAtm(InAtmNo);

    // OVERRIDE USER
    if (Ac.CitId != "1000")
    {
        WUser = Ac.CitId ; 
    }

    // Insert Transactions (TWO Trans) In TransTo BePosted 

    // Money Out of Cassettes
    // 
        Tp.TranOrigin = 2; // ORIGIN Replenishment
        Tp.ErrNo = 0 ;
        Tp.AtmNo = InAtmNo;
        Tp.SesNo = InSesNo;
        Tp.BankId = Ac.BankId;

        Tp.AtmTraceNo = 0 ;

        Tp.BranchId = Ac.Branch;
        Tp.AtmDtTime = DateTime.Now;
        Tp.HostDtTime = DateTime.Now;
        Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER 

        Tp.CardNo = "N/A";
        Tp.CardOrigin = 5; // Find OUT

        // First Entry 
        Tp.TransType = 11; 
        Tp.TransDesc = "Money Remaining In Cassettes.Return them to Till";

        Acc.ReadAndFindAccount(WUser, InOperator, "", Ac.DepCurNm, "User or CIT Cash");
        if (Acc.RecordFound == false)
        {
            ErrorFound = false;
            ErrorOutput = "Account not found for User : " + WUser ;
            return;
        }

        Tp.AccNo = Acc.AccNo;  // USER Till Account 

        Tp.GlEntry = true;

        // Second Entry 
        // CREATE A TRANSACTION TO CR ATM CASH 

        Tp.TransType2 = 21; // MAKE Second Entry 
        Tp.TransDesc2 = "ATM Cash credited. Money remaining in cassettes.";
        // When we put 1000 we want to get the accounts for ATM 

        Acc.ReadAndFindAccount("1000", InOperator, InAtmNo, Ac.DepCurNm, "ATM Cash");
        if (Acc.RecordFound == false)
        {
            ErrorFound = false;
            ErrorOutput = "Account not found for ATMNo: " + InAtmNo;
            return;
        }
        Tp.AccNo2 = Acc.AccNo;  // ATM Cash Account 
        Tp.GlEntry2 = true;

        Tp.CurrDesc = Ac.DepCurNm;
        Tp.TranAmount = OutCassetteAmnt ;
        Tp.AuthCode = 0 ;
        Tp.RefNumb = 0 ;
        Tp.RemNo = 0 ;
        Tp.TransMsg = "N/A";
        Tp.AtmMsg = "";
        Tp.OpenDate = DateTime.Now;
        Tp.OpenRecord = true;
        Tp.Operator = InOperator;

        Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo);

    // MAKE SECOND PAIR OF TRANSACTIONS

        // Money In Cassettes
        // 

        Tp.TranOrigin = 2; // ORIGIN Replenishment
        Tp.ErrNo = 0;
        Tp.AtmNo = InAtmNo;
        Tp.SesNo = InSesNo;
        Tp.BankId = Ac.BankId;

        Tp.AtmTraceNo = 0;

        Tp.BranchId = Ac.Branch;
        Tp.AtmDtTime = DateTime.Now;
        Tp.HostDtTime = DateTime.Now;
        Tp.SystemTarget = 4; // Find Correct FOR GENERAL LEDGER 

        Tp.CardNo = "N/A";
        Tp.CardOrigin = 5; // Find OUT

        // First Entry 
        Tp.TransType = 11;
        Tp.TransDesc = "Money put in ATM Cassettes - DR ATM cash";

        Tp.AccNo = Acc.AccNo;  // ATM CAsh 
        Tp.GlEntry = true;

        // Second Entry 
        // CREATE A TRANSACTION TO CR ATM CASH 

        Tp.TransType2 = 21; // MAKE Second Entry 
        Tp.TransDesc2 = "User Till Credited. Money moved to ATM cassettes.";

        Acc.ReadAndFindAccount(WUser, InOperator, "", Ac.DepCurNm, "User or CIT Cash");
        if (Acc.RecordFound == false)
        {
            ErrorFound = false;
            ErrorOutput = "Account not found for User : " + WUser;
            return;
        }

        Tp.AccNo2 = Acc.AccNo;  // // USER Till Account 
        Tp.GlEntry2 = true;

        Tp.CurrDesc = Ac.DepCurNm;
        Tp.TranAmount = InCassetteAmnt;
        Tp.AuthCode = 0;
        Tp.RefNumb = 0;
        Tp.RemNo = 0;
        Tp.TransMsg = "N/A";
        Tp.AtmMsg = "";
        Tp.OpenDate = DateTime.Now;
        Tp.OpenRecord = true;
        Tp.Operator = InOperator;

        Tp.InsertTransToBePosted(Tp.AtmNo, Tp.ErrNo);

}

// UPDATE All ERROR TABLE FOR A SPECIFIC ATM - DURING TURBO PROCESS 
  
public void UpdateErrorsTableActionTaken(int InErrNo)
{
   
    ErrorFound = false;
    ErrorOutput = ""; 

    using (SqlConnection conn =
        new SqlConnection(connectionString))
        try
        {
            conn.Open();
            using (SqlCommand cmd =
                new SqlCommand("UPDATE ErrorsTable SET UnderAction=@UnderAction, UserComment = @UserComment, ActionDtTm = @ActionDtTm, "
                               + " Printed = @Printed" +
                    " WHERE ErrNo = @ErrNo AND OpenErr=1 AND NeedAction=1", conn))
            {
                cmd.Parameters.AddWithValue("@ErrNo", InErrNo);
                cmd.Parameters.AddWithValue("@UnderAction", 1);
                UserComment = " Turbo Action "; 
                cmd.Parameters.AddWithValue("@UserComment", UserComment);
                cmd.Parameters.AddWithValue("@ActionDtTm", DateTime.Now);
                cmd.Parameters.AddWithValue("@Printed", 0 );
                
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
            ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
        }
}
// ===============================REad and copy errors Ids===================
// ===============================From ModelBak to new Bank==================
// For each read record create ==============================================
// ==========================================================================
public void CopyErrorIds(string InBankA, string InBankB)
{
    RecordFound = false;
    ErrorFound = false;
    ErrorOutput = ""; 

    string SqlString = "SELECT *"
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
                cmd.Parameters.AddWithValue("@Operator", InBankA);

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

                    Operator = (string)rdr["Operator"];

                    // =========== Insert Record Id for new bank =======
                    // =================================================
                    // INSERT GAS PARAMETER WITH DIFFERENT BANK 
                    Operator = InBankB;
        
                    InsertErrorIdRecord();
                    // =================================================      
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
            ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
        }
}
// ==============================================================================
// ===============================INSERT ERROR ID==================================
// ================================================================================
public void InsertErrorIdRecord()
{
    
    ErrorFound = false;
    ErrorOutput = ""; 

    string cmdinsert = "INSERT INTO [dbo].[ErrorsIdCharacteristics] ([ErrId],[ErrDesc],[ErrType],[AtmNo],[SesNo],[DateInserted],"
           + "[BankId],[BranchId],[TurboReconc],[TraceNo],[CardNo],[TransNo],[TransType],[TransDescr],"
           + "[DateTime],[NeedAction],[OpenErr],[FullCard],[UnderAction],[ManualAct],"
           + "[ByWhom],[ActionDtTm],[ActionSes],[CurDes],[ErrAmount],[ActionId],"
           + "[DrCust],[CrCust],[CustAccNo],[DrAtmCash],[CrAtmCash],[AccountNo1],[DrAtmSusp],[CrAtmSusp],[AccountNo2],"
           + "[DrAccount3],[CrAccount3],[AccountNo3],[ForeignCard],[MainOnly],[UserComment],[Printed],[DatePrinted],[CircularDesc])"
        + " VALUES (@ErrId,@ErrDesc,@ErrType,@AtmNo,@SesNo,@DateInserted,"
            + "@BankId,@BranchId,@TurboReconc,@TraceNo,@CardNo,@TransNo,@TransType,@TransDescr,"
           + "@DateTime,@NeedAction,@OpenErr,@FullCard,@UnderAction,@ManualAct,"
           + "@ByWhom,@ActionDtTm,@ActionSes,@CurDes,@ErrAmount,@ActionId,"
           + "@DrCust,@CrCust,@CustAccNo,@DrAtmCash,@CrAtmCash,@AccountNo1,@DrAtmSusp,@CrAtmSusp,@AccountNo2,"
           + "@DrAccount3,@CrAccount3,@AccountNo3,@ForeignCard, @MainOnly,@UserComment,@Printed,@DatePrinted,@CircularDesc)";

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

                cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                cmd.Parameters.AddWithValue("@SesNo", SesNo);

                cmd.Parameters.AddWithValue("@DateInserted", DateInserted);

                cmd.Parameters.AddWithValue("@BankId", BankId);
            
                cmd.Parameters.AddWithValue("@BranchId", BranchId);
                cmd.Parameters.AddWithValue("@TurboReconc", TurboReconc);

                cmd.Parameters.AddWithValue("@TraceNo", TraceNo);
                cmd.Parameters.AddWithValue("@CardNo", CardNo);
                cmd.Parameters.AddWithValue("@TransNo", TransNo);
                cmd.Parameters.AddWithValue("@TransType", TransType);
                cmd.Parameters.AddWithValue("@TransDescr", TransDescr);

                cmd.Parameters.AddWithValue("@DateTime", DateTime);
                cmd.Parameters.AddWithValue("@NeedAction", NeedAction);
                cmd.Parameters.AddWithValue("@OpenErr", OpenErr);
                cmd.Parameters.AddWithValue("@FullCard", FullCard);

                cmd.Parameters.AddWithValue("@UnderAction", UnderAction); // 
                cmd.Parameters.AddWithValue("@ManualAct", ManualAct);  //

                cmd.Parameters.AddWithValue("@ByWhom", ByWhom);  //
                cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);
                cmd.Parameters.AddWithValue("@ActionSes", ActionSes);

                cmd.Parameters.AddWithValue("@CurDes", CurDes);
                cmd.Parameters.AddWithValue("@ErrAmount", ErrAmount);
                cmd.Parameters.AddWithValue("@ActionId", ActionId);
                cmd.Parameters.AddWithValue("@DrCust", DrCust);
                cmd.Parameters.AddWithValue("@CrCust", CrCust);
                cmd.Parameters.AddWithValue("@CustAccNo", CustAccNo);

                cmd.Parameters.AddWithValue("@DrAtmCash", DrAtmCash);
                cmd.Parameters.AddWithValue("@CrAtmCash", CrAtmCash);
                cmd.Parameters.AddWithValue("@AccountNo1", AccountNo1);

                cmd.Parameters.AddWithValue("@DrAtmSusp", DrAtmSusp);
                cmd.Parameters.AddWithValue("@CrAtmSusp", CrAtmSusp);
                cmd.Parameters.AddWithValue("@AccountNo2", AccountNo2);

                cmd.Parameters.AddWithValue("@DrAccount3", DrAccount3);
                cmd.Parameters.AddWithValue("@CrAccount3", CrAccount3);
                cmd.Parameters.AddWithValue("@AccountNo3", AccountNo3);


                cmd.Parameters.AddWithValue("@ForeignCard", ForeignCard);
                cmd.Parameters.AddWithValue("@MainOnly", MainOnly);

                cmd.Parameters.AddWithValue("@UserComment", UserComment);
                cmd.Parameters.AddWithValue("@Printed", Printed);
                cmd.Parameters.AddWithValue("@DatePrinted", DatePrinted);

                cmd.Parameters.AddWithValue("@CircularDesc", CircularDesc);

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
            ErrorOutput = "An error occured in ErrorsClassWithActions Class............. " + ex.Message;
        }
}

    }
}
