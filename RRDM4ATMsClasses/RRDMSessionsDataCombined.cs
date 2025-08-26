using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMSessionsDataCombined : Logger
    {
        public RRDMSessionsDataCombined() : base() { }

        // HEADER 
        public int SeqNo;
        public string AtmNo;
        public int SesNo;
        public DateTime SesDtTimeStart;
        public DateTime SesDtTimeEnd;
        public DateTime DateOpened;
        public int ProcessMode; // 0, 1, 2 
        // FIRST PART DURING JOURNAL LOADING BASED ON IST TXNS
        public string Line_1; 
        public decimal OpenBal;
        public decimal WithDrawls; // BASED ON IST
        public decimal Deposits;
        public decimal Remaining;
        public decimal GL_BalanceFromCore;
        public decimal Excess;
        public decimal Shortage;
        public string Remark;
        public int CapturedCards;
        // SECOND PART DURING JOURNAL LOADING BASED ON E-Journal TXNS
        public string Line_2;
        public decimal OpenBal1;
        public decimal WithDrawls1;
        public decimal Deposits1;
        public decimal Remaining1;
        public decimal GL_BalanceFromCore1;
        public decimal Excess1;
        public decimal Shortage1;
        public string Remark1;
        public int CapturedCards1;
        // THIRD PART CREATED AFTER AFTER MANUAL COUNTING
        public string Line_3;
        public decimal OpenBal2;
        public decimal WithDrawls2;
        public decimal Deposits2;
        public decimal Remaining2;
        public decimal GL_BalanceFromCore2;
        public decimal Excess2;
        public decimal Shortage2;
        public string Remark2;
        public int CapturedCards2;

        // CREATED AT RM CYCLE 
        public int RMCycle; 
        //
        //************
        //
        
        // Define the data table 
        public DataTable TableSessionsDataCombined = new DataTable();

        public DataTable TableSessionsDataCombined_2 = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;

        // Read Table Fields
        private void ReadTableFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
            AtmNo = (string)rdr["AtmNo"];
            SesNo = (int)rdr["SesNo"];

            SesDtTimeStart = (DateTime)rdr["SesDtTimeStart"];
            SesDtTimeEnd = (DateTime)rdr["SesDtTimeEnd"];
            DateOpened = (DateTime)rdr["DateOpened"];

            ProcessMode = (int)rdr["ProcessMode"];

            Line_1 = (string)rdr["Line_1"];

            OpenBal = (decimal)rdr["OpenBal"];
            WithDrawls = (decimal)rdr["WithDrawls"];
            Deposits = (decimal)rdr["Deposits"];
            Remaining = (decimal)rdr["Remaining"];
            GL_BalanceFromCore = (decimal)rdr["GL_BalanceFromCore"];
            Excess = (decimal)rdr["Excess"];
            Shortage = (decimal)rdr["Shortage"];

            Remark = (string)rdr["Remark"];
            CapturedCards = (int)rdr["CapturedCards"];

            Line_2 = (string)rdr["Line_2"];

            OpenBal1 = (decimal)rdr["OpenBal1"];
            WithDrawls1 = (decimal)rdr["WithDrawls1"];
            Deposits1 = (decimal)rdr["Deposits1"];
            Remaining1 = (decimal)rdr["Remaining1"];
            GL_BalanceFromCore1 = (decimal)rdr["GL_BalanceFromCore1"];
            Excess1 = (decimal)rdr["Excess1"];
            Shortage1 = (decimal)rdr["Shortage1"];

            Remark1 = (string)rdr["Remark1"];
            CapturedCards1 = (int)rdr["CapturedCards1"];

            Line_3 = (string)rdr["Line_3"];

            OpenBal2 = (decimal)rdr["OpenBal2"];
            WithDrawls2 = (decimal)rdr["WithDrawls2"];
            Deposits2 = (decimal)rdr["Deposits2"];
            Remaining2 = (decimal)rdr["Remaining2"];
            GL_BalanceFromCore2 = (decimal)rdr["GL_BalanceFromCore2"];
            Excess2 = (decimal)rdr["Excess2"];
            Shortage2 = (decimal)rdr["Shortage2"];

            Remark2 = (string)rdr["Remark2"];
            CapturedCards2 = (int)rdr["CapturedCards2"];

            RMCycle = (int)rdr["RMCycle"];

        }

        //
        // Methods 
        // READ SessionsDataCombined
        // FILL UP A TABLE
        //
        public void ReadSessionsDataCombined(string InOperator, string InSignedId ,string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableSessionsDataCombined = new DataTable();
            TableSessionsDataCombined.Clear();

            TotalSelected = 0;

            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[SessionsDataCombined] "
                    + InSelectionCriteria ;

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                       // sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableSessionsDataCombined);

                        // Close conn
                        conn.Close();

                       // InsertReport(InOperator, InSignedId, TableSessionsDataCombined);

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }
        }

        public void ReadSessionsDataCombined_Fill_the_Two_linesTable(string InOperator, string InSignedId, string InSelectionCriteria      
                      , DateTime InFromDt, DateTime InToDt, string InAtmNo ,int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableSessionsDataCombined_2 = new DataTable();
            TableSessionsDataCombined_2.Clear();

            // DATA TABLE ROWS DEFINITION 
            TableSessionsDataCombined_2.Columns.Add("SeqNo", typeof(int));
            TableSessionsDataCombined_2.Columns.Add("Color", typeof(int));
            TableSessionsDataCombined_2.Columns.Add("AtmNo", typeof(string));
            TableSessionsDataCombined_2.Columns.Add("SesNo", typeof(string));

            TableSessionsDataCombined_2.Columns.Add("SesDtTimeStart", typeof(DateTime));
            TableSessionsDataCombined_2.Columns.Add("SesDtTimeEnd", typeof(DateTime));

            TableSessionsDataCombined_2.Columns.Add("Line_Type", typeof(string));

            TableSessionsDataCombined_2.Columns.Add("OpenBal", typeof(decimal));
            TableSessionsDataCombined_2.Columns.Add("WithDrawls", typeof(decimal));
            TableSessionsDataCombined_2.Columns.Add("Deposits", typeof(decimal));
            TableSessionsDataCombined_2.Columns.Add("Remaining", typeof(decimal));

            TableSessionsDataCombined_2.Columns.Add("GL_BalanceFromCore", typeof(decimal));
            TableSessionsDataCombined_2.Columns.Add("Excess", typeof(decimal));
            TableSessionsDataCombined_2.Columns.Add("Shortage", typeof(decimal));

            TableSessionsDataCombined_2.Columns.Add("Remark", typeof(string));
            TableSessionsDataCombined_2.Columns.Add("CapturedCards", typeof(int));

            TableSessionsDataCombined_2.Columns.Add("DateOpened", typeof(DateTime));
            TableSessionsDataCombined_2.Columns.Add("ProcessMode", typeof(int));

            TotalSelected = 0;

            int Color = 11;

            if (InMode == 1)
            {
                SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[SessionsDataCombined] "
                    + InSelectionCriteria
                    + " ORDER By RMCycle DESC, AtmNo ASC ";
            }

            if (InMode == 2)
            {
                SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[SessionsDataCombined] "
                    + " WHERE AtmNo = @AtmNo AND CAST(SesDtTimeEnd as Date) BETWEEN @FromDt AND @ToDt "
                    + " ORDER By RMCycle DESC, AtmNo ASC ";
            }

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {
                        if (InMode ==2)
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                            cmd.Parameters.AddWithValue("@FromDt", InFromDt);
                            cmd.Parameters.AddWithValue("@ToDt", InToDt);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadTableFields(rdr);

                            switch (Color)
                            {
                                case 11:
                                    {
                                        Color = 12; 
                                        break; 
                                    }
                                case 12:
                                    {
                                        Color = 11;
                                        break;
                                    }
                            }

                            DataRow RowSelected = TableSessionsDataCombined_2.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["Color"] = Color;
                            RowSelected["AtmNo"] = AtmNo;
                            RowSelected["SesNo"] = SesNo.ToString();

                            RowSelected["SesDtTimeStart"] = SesDtTimeStart;
                            RowSelected["SesDtTimeEnd"] = SesDtTimeEnd;
                            RowSelected["DateOpened"] = DateOpened;

                            RowSelected["Line_Type"] = Line_1;

                            RowSelected["OpenBal"] = OpenBal;
                            RowSelected["WithDrawls"] = WithDrawls;
                            RowSelected["Deposits"] = Deposits;
                            RowSelected["Remaining"] = Remaining;

                            RowSelected["GL_BalanceFromCore"] = GL_BalanceFromCore;
                            RowSelected["Excess"] = Excess;
                            RowSelected["Shortage"] = Shortage;

                            RowSelected["Remark"] = Remark;
                            RowSelected["CapturedCards"] = CapturedCards;
                            RowSelected["ProcessMode"] = ProcessMode;


                            // ADD ROW
                            TableSessionsDataCombined_2.Rows.Add(RowSelected);

                            DataRow RowSelected_2 = TableSessionsDataCombined_2.NewRow();

                            RowSelected_2["SeqNo"] = SeqNo;
                            RowSelected_2["Color"] = Color;

                            RowSelected_2["AtmNo"] = "";
                            RowSelected_2["SesNo"] = "Second";

                            RowSelected_2["SesDtTimeStart"] = SesDtTimeStart.ToShortDateString();
                            RowSelected_2["SesDtTimeEnd"] = SesDtTimeEnd.ToShortDateString();
                            RowSelected_2["DateOpened"] = DateOpened;

                            RowSelected_2["Line_Type"] = Line_2;

                            RowSelected_2["OpenBal"] = OpenBal1;
                            RowSelected_2["WithDrawls"] = WithDrawls1;
                            RowSelected_2["Deposits"] = Deposits1;
                            RowSelected_2["Remaining"] = Remaining1;

                            RowSelected_2["GL_BalanceFromCore"] = GL_BalanceFromCore1;
                            RowSelected_2["Excess"] = Excess1;
                            RowSelected_2["Shortage"] = Shortage1;

                            RowSelected_2["Remark"] = Remark1;
                            RowSelected_2["CapturedCards"] = CapturedCards1;

                            RowSelected_2["ProcessMode"] = ProcessMode;

                            // ADD ROW
                            TableSessionsDataCombined_2.Rows.Add(RowSelected_2);



                            if (ProcessMode > 0 || (Remark2 != "" & ProcessMode == 0) )
                            {
                                // Counted had been imputed 
                                // ADD NEXT ROW
                                DataRow RowSelected_3 = TableSessionsDataCombined_2.NewRow();

                                RowSelected_3["SeqNo"] = SeqNo;
                                RowSelected_3["Color"] = Color;
                                //if ((Excess1 != Excess)
                                //    ||(
                                //    )
                                RowSelected_3["AtmNo"] = "";
                                RowSelected_3["SesNo"] = "MANUAL";

                                RowSelected_3["SesDtTimeStart"] = SesDtTimeStart.ToShortDateString();
                                RowSelected_3["SesDtTimeEnd"] = SesDtTimeEnd.ToShortDateString();
                                RowSelected_3["DateOpened"] = DateOpened;
                                RowSelected_3["ProcessMode"] = ProcessMode;

                                RowSelected_3["Line_Type"] = Line_3;

                                RowSelected_3["OpenBal"] = OpenBal2;
                                RowSelected_3["WithDrawls"] = WithDrawls2;
                                RowSelected_3["Deposits"] = Deposits2;
                                RowSelected_3["Remaining"] = Remaining2;

                                RowSelected_3["GL_BalanceFromCore"] = GL_BalanceFromCore2;
                                RowSelected_3["Excess"] = Excess2;
                                RowSelected_3["Shortage"] = Shortage2;

                                RowSelected_3["Remark"] = Remark2;
                                RowSelected_3["CapturedCards"] = CapturedCards2;

                                RowSelected_3["ProcessMode"] = ProcessMode;

                                // ADD ROW
                                TableSessionsDataCombined_2.Rows.Add(RowSelected_3);

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

        // Insert 
        public void InsertReport(string InOperator, string InSignedId, DataTable InTable)
        {

            if (InTable.Rows.Count > 0)
            {
                //Clear REPORT Table 
                RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

                //Clear Table 
                Tr.DeleteReport79();

                // RECORDS READ AND PROCESSED 
                //TableMpa
                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport79]";

                            foreach (var column in InTable.Columns)
                                s.ColumnMappings.Add(column.ToString(), column.ToString());

                            s.WriteToServer(InTable);
                        }
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        CatchDetails(ex);
                    }
            }

        }
        //
        // Methods 
        // READ  
        // 
        //
        public void ReadSessionsDataCombinedBySelectionCriteria(string  InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[SessionsDataCombined] "
                      + InSelectionCriteria;  

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);
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
        // Methods 
        // READ  
        // 
        //
        public void ReadSessionsDataCombinedBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[SessionsDataCombined] "
                      + " WHERE SeqNo = @SeqNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);
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
        // UPDATE SessionsDataCombined
        // UPON counted input you do this Update
        //
        public void Update_SessionsDataCombined(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows = 0; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE SessionsDataCombined SET "
                            + "[ProcessMode]=@ProcessMode,"
                             + "[OpenBal2]=@OpenBal2,"
                            + "[WithDrawls2]=@WithDrawls2,"
                            + "[Deposits2] =@Deposits2,"
                            + "[Remaining2] = @Remaining2,"
                            + " [GL_BalanceFromCore2] = @GL_BalanceFromCore2, "
                            + " [Excess2] = @Excess2, "
                            + " [Shortage2] = @Shortage2,"
                            + " [Remark2] = @Remark2,"
                            + " [CapturedCards2] = @CapturedCards2 "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode); 

                        cmd.Parameters.AddWithValue("@OpenBal2", OpenBal2);

                        cmd.Parameters.AddWithValue("@WithDrawls2", WithDrawls2);

                        cmd.Parameters.AddWithValue("@Deposits2", Deposits2);

                        cmd.Parameters.AddWithValue("@Remaining2", Remaining2);

                        cmd.Parameters.AddWithValue("@GL_BalanceFromCore2", GL_BalanceFromCore2);

                        cmd.Parameters.AddWithValue("@Excess2", Excess2);

                        cmd.Parameters.AddWithValue("@Shortage2", Shortage2);

                        cmd.Parameters.AddWithValue("@Remark2", Remark2);
                        cmd.Parameters.AddWithValue("@CapturedCards2", CapturedCards2);

                        // Execute and check success 
                        rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            //    outcome = " ATMs Table UPDATED ";
                        }

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
        // Insert ExcelOutputCycle
        public int InsertSessionsDataCombined()
        {
           
            ErrorFound = false;
            ErrorOutput = "";
            
            string cmdinsert = "INSERT INTO [ATMS].[dbo].[SessionsDataCombined] "
                + " ( "
                + " [AtmNo],"
                + " [SesNo],"
                + " [SesDtTimeStart],"
                + " [SesDtTimeEnd],"
                + " [DateOpened],"
                + " [ProcessMode],"
                  + " [OpenBal],"
                + " [WithDrawls],"
                + " [Deposits],"
                + " [Remaining],"
                + " [GL_BalanceFromCore],"
                + " [Excess],"
                 + " [Shortage],"
                 + " [Remark],"
                 + " [CapturedCards],"
                  + " [OpenBal1],"
                + " [WithDrawls1],"
                + " [Deposits1],"
                + " [Remaining1],"
                + " [GL_BalanceFromCore1],"
                + " [Excess1],"
                 + " [Shortage1],"
                 + " [Remark1],"
                 + " [CapturedCards1],"
                + " [RMCycle] )"
                + " VALUES"
                + " ( "
                + " @AtmNo,"
                + " @SesNo,"
                + " @SesDtTimeStart,"
                + " @SesDtTimeEnd,"
                + " @DateOpened,"
                + " @ProcessMode,"
                + " @OpenBal,"
                + " @WithDrawls,"
                 + " @Deposits,"
                + " @Remaining,"
                + " @GL_BalanceFromCore,"
                + " @Excess,"
                + " @Shortage,"
                 + " @Remark,"
                + " @CapturedCards,"
                 + " @OpenBal1,"
                + " @WithDrawls1,"
                 + " @Deposits1,"
                + " @Remaining1,"
                + " @GL_BalanceFromCore1,"
                + " @Excess1,"
                + " @Shortage1,"
                 + " @Remark1,"
                + " @CapturedCards1,"
                + " @RMCycle ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int) ";
            //+ " SELECT MsgID  = CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
               
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);

                        cmd.Parameters.AddWithValue("@SesNo", SesNo);

                        cmd.Parameters.AddWithValue("@SesDtTimeStart", SesDtTimeStart);

                        cmd.Parameters.AddWithValue("@SesDtTimeEnd", SesDtTimeEnd);
                  
                        cmd.Parameters.AddWithValue("@DateOpened", DateOpened);

                        cmd.Parameters.AddWithValue("@ProcessMode", ProcessMode);
                        cmd.Parameters.AddWithValue("@OpenBal", OpenBal);
                        cmd.Parameters.AddWithValue("@WithDrawls", WithDrawls);

                        cmd.Parameters.AddWithValue("@Deposits", Deposits);

                        cmd.Parameters.AddWithValue("@Remaining", Remaining);

                        cmd.Parameters.AddWithValue("@GL_BalanceFromCore", GL_BalanceFromCore);

                        cmd.Parameters.AddWithValue("@Excess", Excess);

                        cmd.Parameters.AddWithValue("@Shortage", Shortage);

                        cmd.Parameters.AddWithValue("@Remark", Remark);
                        cmd.Parameters.AddWithValue("@CapturedCards", CapturedCards);

                        cmd.Parameters.AddWithValue("@OpenBal1", OpenBal1);
                        cmd.Parameters.AddWithValue("@WithDrawls1", WithDrawls1);

                        cmd.Parameters.AddWithValue("@Deposits1", Deposits1);

                        cmd.Parameters.AddWithValue("@Remaining1", Remaining1);

                        cmd.Parameters.AddWithValue("@GL_BalanceFromCore1", GL_BalanceFromCore1);

                        cmd.Parameters.AddWithValue("@Excess1", Excess1);

                        cmd.Parameters.AddWithValue("@Shortage1", Shortage1);

                        cmd.Parameters.AddWithValue("@Remark1", Remark1);
                        cmd.Parameters.AddWithValue("@CapturedCards1", CapturedCards1);
                        cmd.Parameters.AddWithValue("@RMCycle", RMCycle);

                        //rows number of record got updated
                        SeqNo = (int)cmd.ExecuteScalar();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex); 
                }

            return SeqNo;
        }

      
    }
}
