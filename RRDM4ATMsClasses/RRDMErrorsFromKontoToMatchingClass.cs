using System;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMErrorsFromKontoToMatchingClass : Logger
    {
        public RRDMErrorsFromKontoToMatchingClass() : base() { }

        // Declare fields 
        //

        public int SeqNo;
        public string BankId;
        public bool Prive;
        public int ErrId;  
        public string AtmNo;
        public int TraceNo;
        public DateTime DateInserted;
        public DateTime DateRead;
        public bool OpenRecord;


        // Values of ERROR Type
        // 1 : Withdrawl EJournal Errors
        // 2 : Mainframe Withdrawl Errors
        // 3 : Deposit Errors Journal 
        // 4 : Deposit Mainframe Errors
        // 5 : Created by user Errors = eg moving to suspense 
        // 6 : Empty 
        // 7 : Created System Errors 
        // 
        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01); 
   
        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();


        // READ Sequencially all open errors  

        public void ReadInsertMatchedErrors(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [ATMS_Journals].[dbo].[tblMatchingErrors] "
          + " WHERE AtmNo = @AtmNo AND OpenRecord = 1 ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];
                            BankId = (string)rdr["BankId"];
                            Prive = (bool)rdr["Prive"];
                            ErrId = (int)rdr["ErrId"]; 
                            AtmNo = (string)rdr["AtmNo"];
                            TraceNo = (int)rdr["TraceNo"];
                            DateInserted = (DateTime)rdr["DateInserted"];

                            // Read ATM Transaction Based on Trace Number 
                            Tc.ReadTranForTrace(InAtmNo, TraceNo); // Find Tran number FROM ATMS POOL 
                            if (Tc.RecordFound == true)
                            {
                                Mpa.ReadInPoolTransSpecificUniqueRecordId(Tc.UniqueRecordId,2); // Find details of transaction from in Pool 

                                Am.ReadAtmsMainSpecific(InAtmNo);

                                Ac.ReadAtm(InAtmNo);

                                Ec.ReadErrorsIDRecord(ErrId, BankId);// FIND DETAILS OF TYPE OF ERROR

                                Ec.CategoryId = Tc.RMCateg; 
                                Ec.RMCycle = Tc.RMCategCycle;
                                Ec.UniqueRecordId = Tc.UniqueRecordId; 

                                Ec.AtmNo = InAtmNo;
                                Ec.SesNo = Tc.SesNo;
 
                                Ec.DateInserted = DateTime.Now;
                                Ec.DateTime = Tc.AtmDtTime;
                                Ec.BranchId = Ac.Branch;
                                Ec.ByWhom = Am.AuthUser;

                             //   Ec.CurrCd = Tc.CurrCode;
                                Ec.CurDes = Tc.CurrDesc;
                                Ec.ErrAmount = Tc.TranAmount;

                                Ec.TraceNo = Tc.AtmTraceNo;
                                Ec.CardNo = Tc.CardNo;
                                Ec.CustAccNo = Tc.AccNo;
                               
                                Ec.TransType = Tc.TransType;
                                Ec.TransDescr = Tc.TransDesc;

                                Ec.DatePrinted = NullPastDate;

                                Ec.OpenErr = true;

                                Ec.CitId = Am.CitId;

                                Ec.Operator = Am.Operator; 
                                //
                                Ec.InsertError(); // INSERT ERROR 
                                //

                                // UPDATE Records as closed in Konto

                                DateRead = DateTime.Now;
                                OpenRecord = false;

                                UpdateMatchedErrorsToClose(SeqNo); 
                            }
                            else
                            {
                                System.Windows.Forms.MessageBox.Show("An error occured in ErrorsMatchingClass............. ");
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
                    CatchDetails(ex);
                }
        }

      


        // UPDATE ErrorMatched to close 
        // 
        public void UpdateMatchedErrorsToClose(int InSeqNo)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 
         
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS_Journals].[dbo].[tblMatchingErrors] SET "
                             + "DateRead = @DateRead,"
                             + "OpenRecord = @OpenRecord "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@DateRead", DateRead);
                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);


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


