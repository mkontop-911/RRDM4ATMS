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
    public class RRDMHostTransClass
    {
        public int TranNo;
        public string BankId;
    
        public string AtmNo;
    
        public int Trace4Digit;
        public DateTime HostDtTime;
        public string CardNo;
        public string AccNo;

        public int AuthCode;
        public int RefNo;
        public int RemNo;
        public int ErrNo;

        public string TransMsg;
        public bool SuccTran;

        public int TraceNumber; 
        public int TargetSystem;

        public string Operator; 
     //   public int Reconciled; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass(); 
        //
        // READ Merge File based on catd no  
        //
        public void ReadHostTransCard(string InOperator, string InCardNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[tblHHostTrans]"
               + " WHERE Operator=@Operator AND CardNo=@CardNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
               //         cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@CardNo", InCardNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];
                            BankId = (string)rdr["BankId"];
                         //   Prive = (bool)rdr["Prive"];
                            AtmNo = (string)rdr["AtmNo"];

                            Trace4Digit = (int)rdr["TraceNo"];
                            HostDtTime = (DateTime)rdr["HostDtTime"];
                            CardNo = (string)rdr["CardNo"];
                            AccNo = (string)rdr["AccNo"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNo = (int)rdr["RefNo"];
                            RemNo = (int)rdr["RemNo"];
                            ErrNo = (int)rdr["ErrNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            SuccTran = (bool)rdr["SuccTran"];

                            TraceNumber = (int)rdr["TraceNumber"];
                            TargetSystem = (int)rdr["TargetSystem"];
                  //          Reconciled = (int)rdr["Reconciled"];

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
                    ErrorOutput = "An error occured in HostTrans Class............. " + ex.Message;
                }
        }

        //
        // READ HostTrans based on tran no  
        //
        public void ReadHostTransTranNo(string InOperator, int InTranNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[tblHHostTrans]"
               + " WHERE Operator=@Operator AND TranNo=@TranNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                 //       cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TranNo", InTranNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];
                            BankId = (string)rdr["BankId"];
                    
                            AtmNo = (string)rdr["AtmNo"];

                            Trace4Digit = (int)rdr["TraceNo"];
                            HostDtTime = (DateTime)rdr["HostDtTime"];
                            CardNo = (string)rdr["CardNo"];
                            AccNo = (string)rdr["AccNo"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNo = (int)rdr["RefNo"];
                            RemNo = (int)rdr["RemNo"];
                            ErrNo = (int)rdr["ErrNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            SuccTran = (bool)rdr["SuccTran"];

                            TraceNumber = (int)rdr["TraceNumber"];
                            TargetSystem = (int)rdr["TargetSystem"];
                     //       Reconciled = (int)rdr["Reconciled"];


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
                    ErrorOutput = "An error occured in HostTrans Class............. " + ex.Message;
                }
        }

        //
        // READ HostTrans based on trace no  
        //
        public void ReadHostTransTraceNo(string InOperator, string InAtmNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[tblHHostTrans]"
               + " WHERE Operator=@Operator AND AtmNo =@AtmNo AND TraceNumber=@TraceNumber ";

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
                        cmd.Parameters.AddWithValue("@TraceNumber", InTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];
                            BankId = (string)rdr["BankId"];
                       
                            AtmNo = (string)rdr["AtmNo"];

                            Trace4Digit = (int)rdr["TraceNo"];
                            HostDtTime = (DateTime)rdr["HostDtTime"];
                            CardNo = (string)rdr["CardNo"];
                            AccNo = (string)rdr["AccNo"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNo = (int)rdr["RefNo"];
                            RemNo = (int)rdr["RemNo"];
                            ErrNo = (int)rdr["ErrNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            SuccTran = (bool)rdr["SuccTran"];

                            TraceNumber = (int)rdr["TraceNumber"]; // THIS THE BIG 
                            TargetSystem = (int)rdr["TargetSystem"];
                    //        Reconciled = (int)rdr["Reconciled"];

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
                    ErrorOutput = "An error occured in HostTrans Class............. " + ex.Message;
                }
        }
      // Insert transactions 
        //
        public void InsertTrans(string InOperator, string InAtmNo)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [dbo].[tblHHostTrans]"
                          + " ([BankId], [AtmNo], [TraceNo], [HostDtTime],"
                          + " [CardNo], [AccNo], [AuthCode], [RefNo], [RemNo], [ErrNo],"
                          + " [TransMsg], [SuccTran], [TraceNumber], [TargetSystem], [Operator])"
                          + " VALUES (@BankId,  @AtmNo, @TraceNo, @HostDtTime,"
                          + " @CardNo, @AccNo, @AuthCode, @RefNo, @RemNo, @ErrNo,"
                          + " @TransMsg, @SuccTran, @TraceNumber, @TargetSystem, @Operator) ";


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        //  cmd.Parameters.AddWithValue("@Prive", Prive);
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@TraceNo", Trace4Digit);
                        cmd.Parameters.AddWithValue("@HostDtTime", HostDtTime);

                        cmd.Parameters.AddWithValue("@CardNo", CardNo);

                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@AuthCode", AuthCode);
                        cmd.Parameters.AddWithValue("@RefNo", RefNo);
                        cmd.Parameters.AddWithValue("@RemNo", RemNo);

                        cmd.Parameters.AddWithValue("@ErrNo", ErrNo);

                        cmd.Parameters.AddWithValue("@TransMsg", TransMsg);
                        cmd.Parameters.AddWithValue("@SuccTran", SuccTran);
                        cmd.Parameters.AddWithValue("@TraceNumber", TraceNumber);
                        cmd.Parameters.AddWithValue("@TargetSystem", TargetSystem);
                        cmd.Parameters.AddWithValue("@Operator", Operator);
                        //       cmd.Parameters.AddWithValue("@Reconciled", Reconciled);

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
                    ErrorOutput = "An error occured in HostTrans Class............. " + ex.Message;
                }
        }
        // Copy transactions for testing purpose
        // read and Copy from one ATM to another 

        //
        // READ all transactions based on Criteria  
        //
        public void CopyHostTrans(string InOperator, string InAtmNo, string TargetBank, string TargetAtm,
                                  bool TargetPrive)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[tblHHostTrans]"
               + " WHERE Operator=@Operator AND AtmNo =@AtmNo ";

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

                            TranNo = (int)rdr["TranNo"];
                            BankId = (string)rdr["BankId"];
                      
                            AtmNo = (string)rdr["AtmNo"];

                            Trace4Digit = (int)rdr["TraceNo"];
                            HostDtTime = (DateTime)rdr["HostDtTime"];
                            CardNo = (string)rdr["CardNo"];
                            AccNo = (string)rdr["AccNo"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNo = (int)rdr["RefNo"];
                            RemNo = (int)rdr["RemNo"];
                            ErrNo = (int)rdr["ErrNo"];

                            TransMsg = (string)rdr["TransMsg"];
                            SuccTran = (bool)rdr["SuccTran"];

                            TraceNumber = (int)rdr["TraceNumber"]; // THIS THE BIG 
                            TargetSystem = (int)rdr["TargetSystem"];
                       //     Reconciled = (int)rdr["Reconciled"]; // Small Integer 

                            // Insert / Copy transaction 

                            Operator = TargetBank;
                       //     Prive = TargetPrive;
                            AtmNo = TargetAtm;

                            InsertTrans(Operator, AtmNo);

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
                    ErrorOutput = "An error occured in HostTrans Class............. " + ex.Message;
                }
        }
        //
        // READ all transactions based on Criteria  
        //
        public void CopyFromPoolToTrans()
        {
           
            Tc.ReadInPoolTransSpecific(94);

            TranNo = Tc.TranNo;
            BankId = Tc.BankId;
    
            AtmNo = Tc.AtmNo;

            Trace4Digit = Tc.AtmTraceNo;
            HostDtTime = Tc.AtmDtTime;
            CardNo = Tc.CardNo;
            AccNo = Tc.AccNo;

            AuthCode = Tc.AuthCode;
            RefNo = Tc.RefNumb;
            RemNo = Tc.RemNo;
            ErrNo = Tc.ErrNo;

            TransMsg = Tc.TransMsg;
            SuccTran = Tc.SuccTran;

            TraceNumber = Tc.AtmTraceNo; // THIS THE BIG 
            TargetSystem = Tc.SystemTarget;

        //    Operator = WOperator; 

            InsertTrans(BankId, AtmNo);


        }       
   

    }
}
