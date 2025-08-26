using System;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;


namespace RRDM4ATMs
{
    public class RRDMDepositsClass : Logger
    {
        public RRDMDepositsClass() : base() { }

        string WAtmNo;
        int WSesNo;

        // DEPOSITS DECLARATIONN 
      
        // DEPOSITS MACHINE
        //
         public struct DepositsMachine 
        {
          //  public int CurrCd;
            public string CurrNm;
            public int Trans;
            public int Notes;
            public decimal Amount;
            public int NotesRej;
            public decimal AmountRej;
            public int Envelops;
            public decimal EnvAmount; 
        };

        public DepositsMachine DepositsMachine1; // Declare Deposits1 of type Deposits 

        // Deposits Count

         public struct DepositsCount  
        {
            public int Trans;
            public int Notes;
            public decimal Amount;
            public int NotesRej;
            public decimal AmountRej;
            public int Envelops;
            public decimal EnvAmount; 
        };

        public DepositsCount DepositsCount1;

        public struct DepositsDiff
        {
            public int Trans;
            public int Notes;
            public decimal Amount;
            public int NotesRej;
            public decimal AmountRej;
            public int Envelops;
            public decimal EnvAmount;
        };

        public DepositsDiff DepositsDiff1;

        public bool DiffInDeposits; 

        // CHEQUES MACHINE

         public struct ChequesMachine  
        {
            public int Trans;
            public int Number;
            public decimal Amount;
        };

        public ChequesMachine ChequesMachine1;

        // CHEQUES COUNT

         public struct ChequesCount  
        {
            public int Trans;
            public int Number;
            public decimal Amount;
        };

        public ChequesCount ChequesCount1;

        public struct ChequesDiff
        {
            public int Trans;
            public int Number;
            public decimal Amount;
        };

        public ChequesDiff ChequesDiff1;

        public bool DiffInCheques; 


        public bool UpdSesNotes;
            

        // END of Declarations 

        string SqlString;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        public DataTable DataTableNotes = new DataTable();
        public DataTable DataTableTotals = new DataTable();

        // READ Session Notes AND CALCULATE BALANCES 
        //
        public void ReadDepositsSessionsNotesAndValuesDeposits(string InAtmNo, int InSesNo)
        {
            // initialise variables
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            SqlString = "SELECT *"
                                + " FROM [dbo].[SessionsNotesAndValues] "
                                + " WHERE SesNo = @SesNo AND AtmNo=@AtmNo";
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
                            RecordFound = true; 
                            // READ DEPOSITS
                        //    DepositsMachine1.CurrCd = (int)rdr["DepCurCd"];
                            DepositsMachine1.CurrNm = (string)rdr["DepCurNm"];

                            DepositsMachine1.Trans = (int)rdr["DepTransMach"];
                            DepositsMachine1.Notes = (int)rdr["DepNotesMach"];
                            DepositsMachine1.Amount = (decimal)rdr["DepAmountMach"];
                            DepositsMachine1.NotesRej = (int)rdr["DepNotesRejMach"];
                            DepositsMachine1.AmountRej = (decimal)rdr["DepAmountRejMach"];

                            DepositsMachine1.Envelops = (int)rdr["EnvelopsMach"];
                            DepositsMachine1.EnvAmount = (decimal)rdr["EnvAmountMach"];

                            DepositsCount1.Trans = (int)rdr["DepTransCount"];
                            DepositsCount1.Notes = (int)rdr["DepNotesCount"];
                            DepositsCount1.Amount = (decimal)rdr["DepAmountCount"];
                            DepositsCount1.NotesRej = (int)rdr["DepNotesRejCount"];
                            DepositsCount1.AmountRej = (decimal)rdr["DepAmountRejCount"];

                            DepositsCount1.Envelops = (int)rdr["EnvelopsCount"];
                            DepositsCount1.EnvAmount = (decimal)rdr["EnvAmountCount"];

                            // CALCULATE DEPOSITS DIFFERENCES

                            DepositsDiff1.Trans = DepositsCount1.Trans - DepositsMachine1.Trans;
                            DepositsDiff1.Notes = DepositsCount1.Notes - DepositsMachine1.Notes;
                            DepositsDiff1.Amount = DepositsCount1.Amount - DepositsMachine1.Amount;
                            DepositsDiff1.NotesRej = DepositsCount1.NotesRej - DepositsMachine1.NotesRej;
                            DepositsDiff1.AmountRej = DepositsCount1.AmountRej - DepositsMachine1.AmountRej;

                            DepositsDiff1.Envelops = DepositsCount1.Envelops - DepositsMachine1.Envelops;
                            DepositsDiff1.EnvAmount = DepositsCount1.EnvAmount - DepositsMachine1.EnvAmount; 

                            // cHECK if DIFFRENCE iN DEPOSITS

                            if (DepositsDiff1.Trans != 0 || DepositsDiff1.Notes != 0  
                                || DepositsDiff1.Amount != 0 || DepositsDiff1.NotesRej != 0
                                || DepositsDiff1.AmountRej != 0 || DepositsDiff1.Envelops != 0 || DepositsDiff1.EnvAmount != 0)
                            {
                                DiffInDeposits = true; 
                            }
                            else DiffInDeposits = false;

                            // READ CHEQUES 

                            ChequesMachine1.Trans = (int)rdr["ChequesTransMach"];
                            ChequesMachine1.Number = (int)rdr["ChequesNoMach"];
                            ChequesMachine1.Amount = (decimal)rdr["ChequesAmountMach"];

                            ChequesCount1.Trans = (int)rdr["ChequesTransCount"];
                            ChequesCount1.Number = (int)rdr["ChequesNoCount"];
                            ChequesCount1.Amount = (decimal)rdr["ChequesAmountCount"];

                            // DIFFERENCES FOR CHEQUES 

                            ChequesDiff1.Trans = ChequesCount1.Trans - ChequesMachine1.Trans;
                            ChequesDiff1.Number = ChequesCount1.Number - ChequesMachine1.Number;
                            ChequesDiff1.Amount = ChequesCount1.Amount - ChequesMachine1.Amount;

                            if (ChequesDiff1.Trans != 0 || ChequesDiff1.Number != 0|| ChequesDiff1.Amount != 0 )
                            {
                                DiffInCheques = true;
                            }
                            else DiffInCheques = false;

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
        // FIND NOTES By Currency and Facevalue AND FILL TABLE 
        //
        
        public void ReadDepositsAndGet_Table_With_Notes(string InAtmNo, int InReplCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableNotes = new DataTable();
            DataTableNotes.Clear();
         
           SqlString =
         " SELECT [Currency] "
           + "    ,[FaceValue] "
           + "     ,Sum([notes]) As Notes"
           + " FROM [ATM_MT_Journals_AUDI].[dbo].[Deposit_Txns_Analysis] "
           + "  where [AtmNo] = @AtmNo AND ReplCycle = @ReplCycle  "
           + "  group by [Currency],[FaceValue] "
           + "  Order By Currency, FaceValue ";

            using (SqlConnection conn =
           new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Currency", InCurrency);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableNotes);

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
        // Totals By Currency
        //
        public void ReadDepositsAndGet_Table_With_TotalsByCurrency(string InAtmNo, int InReplCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableTotals = new DataTable();
            DataTableTotals.Clear();

                SqlString =
             " SELECT [Currency] "
           + "     ,Sum([notes]) As TotalNotes "
           + " FROM [ATM_MT_Journals_AUDI].[dbo].[Deposit_Txns_Analysis] "
           + "  where [AtmNo] = @AtmNo AND ReplCycle = @ReplCycle  "
           + "  group by [Currency] "
           + "  Order By Currency  ";
          

            using (SqlConnection conn =
           new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                      
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@Currency", InCurrency);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableTotals);

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

        // UPDATE SESSION NOTES Machine Totals
        public int DepNotesMach; 
        public void UpdateDepositsNaWithMachineTotals(string InAtmNo, int InSesNo)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            UpdSesNotes = false;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE SessionsNotesAndValues  SET "
                           + " [DepNotesMach] = @DepNotesMach, "
                           + " [DepAmountMach] = @DepAmountMach "
                           //+ " [DepAmountRejMach] = @DepAmountRejMach,"
                           //+ " [EnvelopsMach] = @EnvelopsMach,"
                           //+ " [EnvAmountMach] = @EnvAmountMach,"
                           //+ " [ChequesTransMach] = @ChequesTransMach,"
                           //+ " [ChequesNoMach] = @ChequesNoMach,[ChequesAmountMach] = @ChequesAmountMach"
                            + " WHERE AtmNo= @AtmNo AND SesNo=@SesNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        //cmd.Parameters.AddWithValue("@DepTransMach", DepositsMachine1.Trans);
                        cmd.Parameters.AddWithValue("@DepNotesMach", DepNotesMach); // Value not available yet 
                        cmd.Parameters.AddWithValue("@DepAmountMach", DepositsMachine1.Amount); 
                      

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            UpdSesNotes = true;
                            //  textBoxMsg.Text = " ATMs Table UPDATED ";
                        }
                        else UpdSesNotes = false;//textBoxMsg.Text = " Nothing WAS UPDATED ";

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
        // UPDATE SESSION NOTES WITH NOTES 
        public void UpdateDepositsSessionsNotesAndValuesWithCount(string InAtmNo, int InSesNo)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            UpdSesNotes = false;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE SessionsNotesAndValues  SET "
                           + " [DepTransCount] = @DepTransCount,[DepNotesCount] = @DepNotesCount, "
                           + " [DepAmountCount] = @DepAmountCount,[DepNotesRejCount] = @DepNotesRejCount, "
                           + " [DepAmountRejCount] = @DepAmountRejCount,"
                           + " [EnvelopsCount] = @EnvelopsCount,"
                           + " [EnvAmountCount] = @EnvAmountCount,"
                           + " [ChequesTransCount] = @ChequesTransCount,"
                           + " [ChequesNoCount] = @ChequesNoCount,[ChequesAmountCount] = @ChequesAmountCount"
                            + " WHERE AtmNo= @AtmNo AND SesNo=@SesNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@SesNo", InSesNo);

                        cmd.Parameters.AddWithValue("@DepTransCount", DepositsCount1.Trans);
                        cmd.Parameters.AddWithValue("@DepNotesCount", DepositsCount1.Notes);
                        cmd.Parameters.AddWithValue("@DepAmountCount", DepositsCount1.Amount);
                        cmd.Parameters.AddWithValue("@DepNotesRejCount", DepositsCount1.NotesRej);
                        cmd.Parameters.AddWithValue("@DepAmountRejCount", DepositsCount1.AmountRej);

                        cmd.Parameters.AddWithValue("@EnvelopsCount", DepositsCount1.Envelops);
                        cmd.Parameters.AddWithValue("@EnvAmountCount", DepositsCount1.EnvAmount);

                        cmd.Parameters.AddWithValue("@ChequesTransCount", ChequesCount1.Trans);
                        cmd.Parameters.AddWithValue("@ChequesNoCount", ChequesCount1.Number);
                        cmd.Parameters.AddWithValue("@ChequesAmountCount", ChequesCount1.Amount);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            UpdSesNotes = true;
                            //  textBoxMsg.Text = " ATMs Table UPDATED ";
                        }
                        else UpdSesNotes = false;//textBoxMsg.Text = " Nothing WAS UPDATED ";

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
        // UPDATE  [ATM_MT_Journals_AUDI].[dbo].[Deposit_Txns_Analysis]
        //
        public void Update_Deposit_Txns_Analysis_ReplCycleNo(string InAtmNo, int InSesNo, int InTraceNo, DateTime InTransDateTime)
        {

            ErrorFound = false;
            ErrorOutput = "";

            UpdSesNotes = false;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATM_MT_Journals_AUDI].[dbo].[Deposit_Txns_Analysis]  SET "
                           + " [ReplCycle] = @ReplCycle "
                            + "  WHERE ReplCycle = 0   AND AtmNo = @AtmNo AND  TraceNo = @TraceNo AND TransDateTime=@TransDateTime ", conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InSesNo);
                        cmd.Parameters.AddWithValue("@TraceNo", InTraceNo);
                        cmd.Parameters.AddWithValue("@TransDateTime", InTransDateTime);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            UpdSesNotes = true;
                            //  textBoxMsg.Text = " ATMs Table UPDATED ";
                        }
                        else UpdSesNotes = false;//textBoxMsg.Text = " Nothing WAS UPDATED ";

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
        // FIND TOTALS FOR DEPOSITS OLD DATA BASE 
        //
        public void ReadDepositsTotals(string InAtmNo, int InSesNo)
        {
            decimal Amount ;

            string TransDesc ;

            decimal Counted ;

     //       decimal Differ ;

            // "DEPOSIT_BNA") // BNA TransType = 23

                DepositsMachine1.Trans = 0;
                DepositsMachine1.Amount = 0;

                // "DEPOSIT") // CHEQUES TransType = 24
          
                ChequesMachine1.Trans = 0 ;
                ChequesMachine1.Amount = 0 ;

                // "DEP CHEQUES") // ENVELOPS TransType = 25 
          
                DepositsMachine1.Envelops = 0 ;
                DepositsMachine1.EnvAmount = 0 ;

                RecordFound = false;
                ErrorFound = false;
                ErrorOutput = ""; 

            SqlString = "Select * FROM [dbo].[InPoolTrans]"
            + " WHERE AtmNo = @AtmNo AND SesNo = @SesNo AND (TransType = 23 OR TransType = 24 OR TransType = 25)";

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
                            RecordFound = true; 

                            Amount = (decimal)rdr["TranAmount"];

                            TransDesc = (string)rdr["TransDesc"];

                            Counted = (decimal)rdr["DepCount"];

                            if (TransDesc == "DEPOSIT_BNA") // BNA 
                            {
                                DepositsMachine1.Trans = DepositsMachine1.Trans + 1;
                                DepositsMachine1.Amount = DepositsMachine1.Amount + Amount;
                            }
                            if (TransDesc == "DEPOSIT") // CHEQUES 
                            {
                                ChequesMachine1.Trans = ChequesMachine1.Trans + 1;
                                // ChequesMachine1.Number = ChequesMachine1.Number + ???
                                ChequesMachine1.Amount = ChequesMachine1.Amount + Amount;
                            }

                            if (TransDesc == "DEP CHEQUES") // ENVELOPS 
                            {
                                DepositsMachine1.Envelops = DepositsMachine1.Envelops + 1;
                                DepositsMachine1.EnvAmount = DepositsMachine1.EnvAmount + Amount;
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

        //
        // FIND TOTALS FOR DEPOSITS NEW DATA BASE 
        //
        public void ReadDepositsTotals_NEW(string InTerminalId, int InReplCycleNo)
        {
            decimal TransAmount;
            int TransType ;
            string TransCurr ;
          
            // "DEPOSIT_BNA") // BNA TransType = 23

            DepositsMachine1.Trans = 0;
            DepositsMachine1.Amount = 0;

            // "DEPOSIT") // CHEQUES TransType = 24

            ChequesMachine1.Trans = 0;
            ChequesMachine1.Amount = 0;

            // "DEP CHEQUES") // ENVELOPS TransType = 25 

            DepositsMachine1.Envelops = 0;
            DepositsMachine1.EnvAmount = 0;

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                    + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                    + " WHERE ReplCycleNo = @ReplCycleNo AND TerminalId=@TerminalId AND (TransType = 23 OR TransType = 24 OR TransType = 25) ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@TerminalId", InTerminalId);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", InReplCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TransType = (int)rdr["TransType"];
                            TransCurr = rdr["TransCurr"].ToString();
                            TransAmount = (decimal)rdr["TransAmount"];

                            if (TransType == 23) // BNA 
                            {
                                DepositsMachine1.Trans = DepositsMachine1.Trans + 1;
                                DepositsMachine1.Amount = DepositsMachine1.Amount + TransAmount;
                            }
                            if (TransType == 24) // CHEQUES 
                            {
                                ChequesMachine1.Trans = ChequesMachine1.Trans + 1;
                                ChequesMachine1.Amount = ChequesMachine1.Amount + TransAmount;
                            }

                            if (TransType == 25) // ENVELOPS 
                            {
                                DepositsMachine1.Envelops = DepositsMachine1.Envelops + 1;
                                DepositsMachine1.EnvAmount = DepositsMachine1.EnvAmount + TransAmount;
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

       
    }
}
