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
    class RRDMHostBatchesClass
    {

        public int BatchNo;
        
        public string AtmNo; 
        public string BankId;
        public string BranchId;
  //      public bool Prive;
        public int PreBatchNo;
        public int NextBatchNo;
      
        public DateTime DtTmStart;
        public DateTime DtTmEnd;
   
        public int HFirstTraceNo;
        public int HLastTraceNo;

        public string HCurrNm1;
        public decimal HBal1;
        public decimal HBal1Adjusted;

        public string HCurrNm2;
        public decimal HBal2;
        public decimal HBal2Adjusted;

        public string HCurrNm3;
        public decimal HBal3;
        public decimal HBal3Adjusted;

        public string HCurrNm4;
        public decimal HBal4;
        public decimal HBal4Adjusted;
    
        public bool AdjBalUpdated;

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;
        //
        // READ Host File Based on batch no  
        //
        public void ReadHostBatchesSpecific(int InBatchNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[HostBatchesTable]"
               + " WHERE BatchNo=@BatchNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BatchNo", InBatchNo);
                       

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            BatchNo = (int)rdr["BatchNo"];
                            AtmNo = (string)rdr["AtmNo"];
                            BankId = (string)rdr["BankId"];
                     
                            BranchId = (string)rdr["BranchId"];

                            PreBatchNo = (int)rdr["PreBatchNo"];
                            NextBatchNo = (int)rdr["NextBatchNo"];

                            DtTmStart = (DateTime)rdr["DtTmStart"];
                            DtTmEnd = (DateTime)rdr["DtTmEnd"];

                            HFirstTraceNo = (int)rdr["HFirstTraceNo"];
                            HLastTraceNo = (int)rdr["HLastTraceNo"];

                            HCurrNm1 = (string)rdr["HCurrNm1"];
                            HBal1 = (decimal)rdr["HBal1"];
                            HBal1Adjusted = (decimal)rdr["HBal1Adjusted"];

                            HCurrNm2 = (string)rdr["HCurrNm2"];
                            HBal2 = (decimal)rdr["HBal2"];
                            HBal2Adjusted = (decimal)rdr["HBal2Adjusted"];

                            HCurrNm3 = (string)rdr["HCurrNm3"];
                            HBal3 = (decimal)rdr["HBal3"];
                            HBal3Adjusted = (decimal)rdr["HBal3Adjusted"];

                            HCurrNm4 = (string)rdr["HCurrNm4"];
                            HBal4 = (decimal)rdr["HBal4"];
                            HBal4Adjusted = (decimal)rdr["HBal4Adjusted"];

                            AdjBalUpdated = (bool)rdr["AdjBalUpdated"];

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
                    ErrorOutput = "An error occured in HostBatchesClass............. " + ex.Message;

                }
        }

        //
        // READ Host File Based on ATM 
        //
        public void ReadHostLastBatch(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[HostBatchesTable]"
               + " WHERE AtmNo=@AtmNo ";

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
                            BatchNo = (int)rdr["BatchNo"];
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
                    ErrorOutput = "An error occured in HostBatchesClass............. " + ex.Message;

                }
        }

        // Insert Host Batch 
        //
        public void InsertHostBatch()
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[HostBatchesTable]"
            + "([AtmNo], [BankId],  [BranchId], [PreBatchNo], [NextBatchNo]," 
                +" [DtTmStart], [DtTmEnd], [HFirstTraceNo], [HLastTraceNo]," 
                + " [HCurrNm1], [HBal1], [HBal1Adjusted]," 
                + " [HCurrNm2], [HBal2], [HBal2Adjusted]," 
                + " [HCurrNm3], [HBal3], [HBal3Adjusted]," + " [HCurrNm4], [HBal4], [HBal4Adjusted]," 
                +" [AdjBalUpdated])" 
                + " VALUES (@AtmNo, @BankId,  @BranchId, @PreBatchNo, @NextBatchNo," 
                + " @DtTmStart, @DtTmEnd, @HFirstTraceNo, @HLastTraceNo," 
                + " @HCurrNm1, @HBal1, @HBal1Adjusted," 
                + " @HCurrNm2, @HBal2, @HBal2Adjusted," 
                + " @HCurrNm3, @HBal3, @HBal3Adjusted," 
                + " @HCurrNm4, @HBal4, @HBal4Adjusted," 
                + " @AdjBalUpdated)";

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
                    //    cmd.Parameters.AddWithValue("@Prive", Prive);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);

                        cmd.Parameters.AddWithValue("@PreBatchNo", PreBatchNo);
                        cmd.Parameters.AddWithValue("@NextBatchNo", NextBatchNo);

                        cmd.Parameters.AddWithValue("@DtTmStart", DtTmStart);
                        cmd.Parameters.AddWithValue("@DtTmEnd", DtTmEnd);

                        cmd.Parameters.AddWithValue("@HCurrNm1", HCurrNm1);
                        cmd.Parameters.AddWithValue("@HBal1", HBal1);
                        cmd.Parameters.AddWithValue("@HBal1Adjusted", HBal1Adjusted);


                        cmd.Parameters.AddWithValue("@HCurrNm2", HCurrNm2);
                        cmd.Parameters.AddWithValue("@HBal2", HBal2);
                        cmd.Parameters.AddWithValue("@HBal2Adjusted", HBal2Adjusted);


                        cmd.Parameters.AddWithValue("@HCurrNm3", HCurrNm3);
                        cmd.Parameters.AddWithValue("@HBal3", HBal3);
                        cmd.Parameters.AddWithValue("@HBal3Adjusted", HBal3Adjusted);

                        cmd.Parameters.AddWithValue("@HCurrNm4", HCurrNm4);
                        cmd.Parameters.AddWithValue("@HBal4", HBal4);
                        cmd.Parameters.AddWithValue("@HBal4Adjusted", HBal4Adjusted);

                        cmd.Parameters.AddWithValue("@AdjBalUpdated", AdjBalUpdated);

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
                    ErrorOutput = "An error occured in HostBatchesClass............. " + ex.Message;

                }
        }

        // UPDATE HOST Batches 
        // 
        public void UpdateHostBatches(int InSeqNumber, string InBankId)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[HostBatchesTable] SET "
                          
                    + " [AtmNo] = @AtmNo, [BankId] = @BankId, " 
                        + " [BranchId] = @BranchId, [PreBatchNo] = @PreBatchNo, [NextBatchNo] = @NextBatchNo," 
                        + " [DtTmStart] = @DtTmStart, [DtTmEnd] = @DtTmEnd," 
                        + " [HFirstTraceNo] = @HFirstTraceNo, [HLastTraceNo] = @HLastTraceNo," 
                        + " [HCurrNm1] = @HCurrNm1, [HBal1] =@HBal1, [HBal1Adjusted] = @HBal1Adjusted,"
                        + " [HCurrNm2] = @HCurrNm2, [HBal2] =@HBal2, [HBal2Adjusted] = @HBal2Adjusted,"
                        + " [HCurrNm3] = @HCurrNm3, [HBal3] =@HBal3, [HBal3Adjusted] = @HBal3Adjusted,"
                        + " [HCurrNm4] = @HCurrNm4, [HBal4] =@HBal4, [HBal4Adjusted] = @HBal4Adjusted,"
                        + " [AdjBalUpdated] = @AdjBalUpdated"
                         + " WHERE BatchNo = @BatchNo", conn))
                      
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                   //     cmd.Parameters.AddWithValue("@Prive", Prive);
                        cmd.Parameters.AddWithValue("@BranchId", BranchId);

                        cmd.Parameters.AddWithValue("@PreBatchNo", PreBatchNo);
                        cmd.Parameters.AddWithValue("@NextBatchNo", NextBatchNo);

                        cmd.Parameters.AddWithValue("@DtTmStart", DtTmStart);
                        cmd.Parameters.AddWithValue("@DtTmEnd", DtTmEnd);

                        cmd.Parameters.AddWithValue("@HCurrNm1", HCurrNm1);
                        cmd.Parameters.AddWithValue("@HBal1", HBal1);
                        cmd.Parameters.AddWithValue("@HBal1Adjusted", HBal1Adjusted);


                        cmd.Parameters.AddWithValue("@HCurrNm2", HCurrNm2);
                        cmd.Parameters.AddWithValue("@HBal2", HBal2);
                        cmd.Parameters.AddWithValue("@HBal2Adjusted", HBal2Adjusted);


                        cmd.Parameters.AddWithValue("@HCurrNm3", HCurrNm3);
                        cmd.Parameters.AddWithValue("@HBal3", HBal3);
                        cmd.Parameters.AddWithValue("@HBal3Adjusted", HBal3Adjusted);

                        cmd.Parameters.AddWithValue("@HCurrNm4", HCurrNm4);
                        cmd.Parameters.AddWithValue("@HBal4", HBal4);
                        cmd.Parameters.AddWithValue("@HBal4Adjusted", HBal4Adjusted);

                        cmd.Parameters.AddWithValue("@AdjBalUpdated", AdjBalUpdated);

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
                    ErrorOutput = "An error occured in HostBatchesClass............. " + ex.Message;

                }
        }
    }
}
