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
 
namespace RRDM4ATMsWin
{
    class RRDME_JournalTxtClass
    {

        public string BankId;    
        public string AtmNo;

        //public int TraceNumber; 
        public DateTime TransDate;

        public string Operator; 

        public int FuId;

        public int RuId;

  //      public string Operator; 

  //      public int TraceNumber;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;
        //
        // READ Merge File based on catd no  
        //
        public void ReadJournalTextByTrace(string InOperator, string InAtmNo, int InTraceNumber)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            Int32 LastDigit = InTraceNumber % 10;

            if (LastDigit == 0)
            {
                // OK
            }
            else
            {
                InTraceNumber = (InTraceNumber - LastDigit) + 1; 
            }

            string SqlString = "SELECT *"
               + " FROM [ATMS_Journals].[dbo].[tblHstEjText]"
               + " WHERE Operator=@Operator AND AtmNo=@AtmNo AND TraceNumber = @TraceNumber ";

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
                        cmd.Parameters.AddWithValue("@TraceNumber", InTraceNumber);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                          
                            BankId = (string)rdr["BankId"];
                            AtmNo = (string)rdr["AtmNo"];
                            TransDate = (DateTime)rdr["TransDate"];

                            FuId = (int)rdr["FuId"];
                            RuId = (int)rdr["RuId"];
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
                    ErrorOutput = "An error occured in ComboClass............. " + ex.Message;

                }
        }
/*
        // UPDATE tblHstEjText from EJProcess 
        // 
        public void UpdateEjTextFromEjProcess(string InAtmNo, int InTraceNumber)
        {
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS_Journals].[dbo].[tblHstEjText] SET "
                            + " ReplCycle = @ReplCycle, CardNo = @CardNo, AccNo = @AccNo, TranType = @TranType, "
                            + " Descr = @Descr, CurNm = @CurNm, TranAmnt = @TranAmnt, ErrId = @ErrId, ErrDesc = @ErrDesc , Operator = @Operator "
                            + " WHERE AtmNo = @AtmNo AND TraceNumber = @TraceNumber", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@TraceNumber", InTraceNumber);

                        cmd.Parameters.AddWithValue("@ReplCycle", ReplCycle);

                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@AccNo", AccNo);
                        cmd.Parameters.AddWithValue("@TranType", TranType);
                        cmd.Parameters.AddWithValue("@Descr", Descr);

                        cmd.Parameters.AddWithValue("@CurNm", CurNm);
                        cmd.Parameters.AddWithValue("@TranAmnt", TranAmnt);

                        cmd.Parameters.AddWithValue("@ErrId", ErrId);

                        cmd.Parameters.AddWithValue("@ErrDesc", ErrDesc);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

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
                    string exception = ex.ToString();
                    // MessageBox.Show(exception);
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }
*/
    }
}
