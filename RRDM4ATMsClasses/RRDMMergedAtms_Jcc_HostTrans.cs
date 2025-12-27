using System;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMMergedAtms_Jcc_HostTrans : Logger
    {
        public RRDMMergedAtms_Jcc_HostTrans() : base() { }

        public int TranNo;
        public string BankId;

        public string AtmNo;
        public int SesNo;
        public int AtmTraceNo;
        string SqlString;

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
        public int RefNo;
        public int RemNo;

        public string Operator; 

        public int Index;

        int InTraceNumber;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        //
        // READ Merge File based on catd no  
        //
        public void ReadMergeFileCard(string InOperator, int InSelectMode, string InEnteredId, DateTime InDateStart, DateTime InDateEnd, int InTranNo)
        {
            int I = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 
           
            if (InSelectMode == 8) // Card 
            {
                   SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MergeAtmsHostTransFile]"
               + " WHERE Operator=@Operator and AtmDtTime >=@TransDateStart AND AtmDtTime <= @TransDateEnd and CardNo=@CardNo  "
                  + " Order by TranNo ";
            }

            if (InSelectMode == 9) // Account number  
            {
                SqlString = "SELECT *"
            + " FROM [ATMS].[dbo].[MergeAtmsHostTransFile]"
            + " WHERE Operator=@Operator and AtmDtTime >=@TransDateStart AND AtmDtTime <= @TransDateEnd and AccNo=@AccNo  "
               + " Order by TranNo ";
            }

            if (InSelectMode == 10) // Trace Number which is numeric 
            {

                InTraceNumber = Convert.ToInt32(InEnteredId);

                SqlString = "SELECT *"
           + " FROM [ATMS].[dbo].[MergeAtmsHostTransFile]"
           + " WHERE Operator=@Operator and AtmTraceNo=@AtmTraceNo  "
              + " Order by TranNo ";

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
                            cmd.Parameters.AddWithValue("@TransDateStart", InDateStart);
                            cmd.Parameters.AddWithValue("@TransDateEnd", InDateEnd);
                            if (InSelectMode == 8) // Crad 
                            {
                                cmd.Parameters.AddWithValue("@CardNo", InEnteredId);
                            }

                            if (InSelectMode == 9) // Account
                            {
                                cmd.Parameters.AddWithValue("@AccNo", InEnteredId);
                            }
                            if (InSelectMode == 10) // Trace Number  
                            {
                                cmd.Parameters.AddWithValue("@AtmTraceNo", InTraceNumber);
                            }
                    
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];

                            if (TranNo == InTranNo)
                            {
                                Index = I; 
                            }
                           
                            BankId = (string)rdr["BankId"];
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];

                            SystemTarget = (int)rdr["SystemTarget"];
                                                                              
                            AtmDtTime = (DateTime)rdr["AtmDtTime"];
                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];

                            CurrDesc = (string)rdr["CurrDesc"];
                            TranAmount = (decimal)rdr["TranAmount"];
                            CardNo = (string)rdr["CardNo"];
                            AccNo = (string)rdr["AccNo"];
 
                            HostDtTime = (DateTime)rdr["HostDtTime"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNo = (int)rdr["RefNo"];
                            RemNo = (int)rdr["RemNo"];

                            Operator = (string)rdr["Operator"];

                            I = I + 1; 

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

        //
        // READ Merge File based on tran no  
        //
        public void ReadMergeFileTranNo(string InOperator, int InTranNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MergeAtmsHostTransFile]"
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
                      
                        cmd.Parameters.AddWithValue("@TranNo", InTranNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];
                            BankId = (string)rdr["BankId"];
                     //       Prive = (bool)rdr["Prive"];
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];

                            SystemTarget = (int)rdr["SystemTarget"];

                            AtmDtTime = (DateTime)rdr["AtmDtTime"];
                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];

                            CurrDesc = (string)rdr["CurrDesc"];
                            TranAmount = (decimal)rdr["TranAmount"];
                            CardNo = (string)rdr["CardNo"];
                            AccNo = (string)rdr["AccNo"];

                            HostDtTime = (DateTime)rdr["HostDtTime"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNo = (int)rdr["RefNo"];
                            RemNo = (int)rdr["RemNo"];

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

                    CatchDetails(ex);

                }
        }

        //
        // READ Merge File based on trace no  
        //
        public void ReadMergeFileTraceNo(string InOperator, string InAtmNo, int InTraceNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[MergeAtmsHostTransFile]"
               + " WHERE Operator=@Operator AND AtmNo =@AtmNo AND AtmTraceNo=@AtmTraceNo ";

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
                        cmd.Parameters.AddWithValue("@AtmTraceNo", InTraceNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TranNo = (int)rdr["TranNo"];
                            BankId = (string)rdr["BankId"];
                     //       Prive = (bool)rdr["Prive"];
                            AtmNo = (string)rdr["AtmNo"];
                            SesNo = (int)rdr["SesNo"];
                            AtmTraceNo = (int)rdr["AtmTraceNo"];

                            SystemTarget = (int)rdr["SystemTarget"];
                            
                            AtmDtTime = (DateTime)rdr["AtmDtTime"];
                            TransType = (int)rdr["TransType"]; // 11 for debit 21 for credit
                            TransDesc = (string)rdr["TransDesc"];

                            CurrDesc = (string)rdr["CurrDesc"];
                            TranAmount = (decimal)rdr["TranAmount"];
                            CardNo = (string)rdr["CardNo"];
                            AccNo = (string)rdr["AccNo"];

                            HostDtTime = (DateTime)rdr["HostDtTime"];

                            AuthCode = (int)rdr["AuthCode"];
                            RefNo = (int)rdr["RefNo"];
                            RemNo = (int)rdr["RemNo"];

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

                    CatchDetails(ex);

                }
        }
    }
}


