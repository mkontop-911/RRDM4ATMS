using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMCaptureCardsClass
    {
        // Declare fields 
        //
        public int CaptNo; 
        public string AtmNo; 
        public string BankId; 
   
        public string BranchId; 
        public int SesNo;
        public int TraceNo; 
        public string CardNo;
        public DateTime CaptDtTm;
        public int CaptureCd; 
        public string ReasonDesc;
        public DateTime ActionDtTm; 
        public string CustomerNm;
        public string ActionComments;
        public int ActionCode; 
    
        public bool OpenRec;

        public string Operator; 
        
    //    public bool Inactive;

        public int CaptureCardsNo; 

        public bool RecordFound;

        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ Group 

        public void ReadCaptureCard(int InCaptNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[CapturedCards] "
          + " WHERE CaptNo = @CaptNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@CaptNo", InCaptNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Captured Details 


                            CaptNo = (int)rdr["CaptNo"];
                            AtmNo = (string)rdr["AtmNo"];
                            BankId = (string)rdr["BankId"];
                     //       Prive = (bool)rdr["Prive"];
                            BranchId = (string)rdr["BranchId"];
                            SesNo = (int)rdr["SesNo"];
                            TraceNo = (int)rdr["TraceNo"];
                            CardNo = (string)rdr["CardNo"];
                            CaptDtTm = (DateTime)rdr["CaptDtTm"];
                            CaptureCd = (int)rdr["CaptureCd"];
                            ReasonDesc = (string)rdr["ReasonDesc"];
                            ActionDtTm = (DateTime)rdr["ActionDtTm"];

                            CustomerNm = (string)rdr["CustomerNm"];
                            ActionComments = (string)rdr["ActionComments"];
                            ActionCode = (int)rdr["ActionCode"];
                        
                          //  ScannedSigned = (image)rdr["ScannedSigned"];

                            OpenRec = (bool)rdr["OpenRec"];
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
                    ErrorOutput = "An error occured in class Capture Cards ............. " + ex.Message;
                    
                }
        }
       
        // Insert NEW Captured Card 
        //
        public void InsertCapturedCard(string InAtmNo)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [CapturedCards]"
                + " ([AtmNo],[BankId],[BranchId],[SesNo],"
                + " [TraceNo],[CardNo],[CaptDtTm],[CaptureCd],[ReasonDesc],[ActionDtTm],"
                + " [CustomerNm],[ActionComments],[ActionCode],"
                 + "[OpenRec], [Operator])"
                + " VALUES("
                + " @AtmNo,@BankId,@BranchId,@SesNo,"
                + " @TraceNo,@CardNo,@CaptDtTm,@CaptureCd,@ReasonDesc,@ActionDtTm,"
                + " @CustomerNm,@ActionComments,@ActionCode,"
                 + "@OpenRec, @Operator"
                +  ")";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                    
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@SesNo", SesNo);

                        cmd.Parameters.AddWithValue("@TraceNo", TraceNo);

                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@CaptDtTm", CaptDtTm);
                        cmd.Parameters.AddWithValue("@CaptureCd", CaptureCd);
                        cmd.Parameters.AddWithValue("@ReasonDesc", ReasonDesc);

                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);

                        cmd.Parameters.AddWithValue("@CustomerNm", CustomerNm);
                        cmd.Parameters.AddWithValue("@ActionComments", ActionComments);
                        cmd.Parameters.AddWithValue("@ActionCode", ActionCode);
                     
                   //     cmd.Parameters.AddWithValue("@ScannedSigned", ); // image 

                        cmd.Parameters.AddWithValue("@OpenRec", 1);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                      //  if (rows > 0) exception = "" ;
                     //   else exception = "NO RECORD WAS ADDED";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in class Capture Cards ............. " + ex.Message;
                }
        }


        // UPDATE Captured Cards
        // 
        public void UpdateCapturedCardSpecific(int InCaptNo)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE CapturedCards SET "
                            + " CardNo = @CardNo,ActionDtTm = @ActionDtTm,CustomerNm = @CustomerNm,"
                             + "ActionComments = @ActionComments,"
                             + "ActionCode = @ActionCode,"
                             + "OpenRec = @OpenRec "
                            + " WHERE CaptNo = @CaptNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@CaptNo", InCaptNo);

                        cmd.Parameters.AddWithValue("@CardNo", CardNo);
                        cmd.Parameters.AddWithValue("@ActionDtTm", ActionDtTm);

                        cmd.Parameters.AddWithValue("@CustomerNm", CustomerNm);
                        cmd.Parameters.AddWithValue("@ActionComments", ActionComments);
                        cmd.Parameters.AddWithValue("@ActionCode", ActionCode);
                       
                 //       cmd.Parameters.AddWithValue("@ScannedSigned", IMAGE);

                        cmd.Parameters.AddWithValue("@OpenRec", OpenRec);


                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                    //    if (rows > 0) exception = "";
                   //     else exception = "Nothing WAS UPDATED";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in class Capture Cards ............. " + ex.Message;
                }
        }

        // NUMBER OF CAPTURED CARDS WITHIN GIVEN SESSION
        //
        public void ReadCapturedCardsNoWithinSession(string InAtmNo, int InSesNo)
        {
            // initialise variables

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[CapturedCards] "
          + " WHERE SesNo = @SesNo AND AtmNo=@AtmNo AND OpenRec =1";
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
                            CaptureCardsNo = CaptureCardsNo + 1;
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
                    ErrorOutput = "An error occured in class Capture Cards ............. " + ex.Message;

                }
        }
       
    }
}
