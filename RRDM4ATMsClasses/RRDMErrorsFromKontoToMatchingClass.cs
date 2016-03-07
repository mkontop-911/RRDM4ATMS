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
    public class RRDMErrorsFromKontoToMatchingClass
    {

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
   
        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();


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
                                Tc.ReadInPoolTransSpecific(Tc.TranNo); // Find details of transaction from in Pool 

                                Am.ReadAtmsMainSpecific(InAtmNo);

                                Ac.ReadAtm(InAtmNo);

                                Ec.ReadErrorsIDRecord(ErrId, BankId);// FIND DETAILS OF TYPE OF ERROR

                                Ec.CategoryId = "N/A"; 
                                Ec.RMCycle = 0;
                                Ec.MaskRecordId = 0; 

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
                                Ec.TransNo = Tc.TranNo;
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
                                //MessageBox.Show("Error in Matched Errors table." 
                                //    + " Registered error trace number: " + TraceNo.ToString ()
                                //    +" doesnot exist in ATMs Pool of transactions");
                                ErrorFound = true;
                                ErrorOutput = "An error occured in ErrorsMatchingClass............. ";
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
                    ErrorOutput = "An error occured in ErrorsMatchingClass............. " + ex.Message;
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


                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {

                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in ErrorsMatchingClass............. " + ex.Message;

                }
        }
       
    }
}
