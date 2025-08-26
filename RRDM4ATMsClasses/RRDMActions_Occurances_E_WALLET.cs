using System;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Threading;

namespace RRDM4ATMs
{
    public class RRDMActions_Occurances_E_WALLET : Logger
    {
        public RRDMActions_Occurances_E_WALLET() : base() { }

        // 
        public int SeqNo;

        public string ActionId; // eg 11 for debit customer CR Atm cash
                                //    21 for Credit customer and DR Atm cash 
        public int Occurance;

        public string ActionNm;
        public bool Is_GL_Action;

        public string GL_Sign_1;
        public string ShortAccID_1;
        public string AccName_1;

        public string Branch_1;
        public string AccNo_1;
        public string StatementDesc_1;

        public string GL_Sign_2;
        public string ShortAccID_2;
        public string AccName_2;

        public string Branch_2;
        public string AccNo_2;

        public string StatementDesc_2;

        public string Ccy;
        public decimal DoubleEntryAmt;

        public string UniqueKeyOrigin; // InUniqueKeyOrigin =
                                       //= "Master_Pool"
                                       // Replenishment
                                       // Dispute_DispId OR UniversalRecordId
        public int UniqueKey;          // UniversalRecordId
                                       // SesNo
        public string Maker;
        public string Authoriser;
        public int AuthorizationKey;

        public string Maker_ReasonOfAction;
        public string Stage; // 01 is Temporary Mode
                             // 02 comfirmed by maker
                             // 03 Settled at finish of workflow and after authorisation 
                             // 04 Posted  
        public string RMCateg; // Like BDC251 or RECATMS-108  
        public int MatchingAtRMCycle; // Like the Cycle that matching was made
        public int RMCycle; //  current RM Cycle at the point of Authorisation 
        public string AtmNo;
        public int ReplCycle; // If >0 means that came from Replenishment cycle. 
        public DateTime ActionAtDateTime;
        public bool Settled;
        public string OriginWorkFlow;
        public string Operator;

        string SqlString;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;



        // Define the data table 
        //public DataTable AccountsForAtmDataTable;

        public DataTable TableActionOccurances_Big;
        public DataTable TableActionOccurances_Small;
        public DataTable TxnsTableFromAction;

        readonly string connectionString = ConfigurationManager.ConnectionStrings
                                  ["ATMSConnectionString"].ConnectionString;

        // Action Occurance Fields
        private void ReadActionOccuranceFields(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];

            ActionId = (string)rdr["ActionId"];

            Occurance = (int)rdr["Occurance"];

            ActionNm = (string)rdr["ActionNm"];
            Is_GL_Action = (bool)rdr["Is_GL_Action"];

            GL_Sign_1 = (string)rdr["GL_Sign_1"];
            ShortAccID_1 = (string)rdr["ShortAccID_1"];
            AccName_1 = (string)rdr["AccName_1"];

            Branch_1 = (string)rdr["Branch_1"];
            AccNo_1 = (string)rdr["AccNo_1"];
            StatementDesc_1 = (string)rdr["StatementDesc_1"];

            GL_Sign_2 = (string)rdr["GL_Sign_2"];
            ShortAccID_2 = (string)rdr["ShortAccID_2"];
            AccName_2 = (string)rdr["AccName_2"];

            Branch_2 = (string)rdr["Branch_2"];
            AccNo_2 = (string)rdr["AccNo_2"];
            StatementDesc_2 = (string)rdr["StatementDesc_2"];

            Ccy = (string)rdr["Ccy"];
            DoubleEntryAmt = (decimal)rdr["DoubleEntryAmt"];

            UniqueKeyOrigin = (string)rdr["UniqueKeyOrigin"];
            UniqueKey = (int)rdr["UniqueKey"];
            Maker = (string)rdr["Maker"];

            Authoriser = (string)rdr["Authoriser"];
            AuthorizationKey = (int)rdr["AuthorizationKey"];

            Maker_ReasonOfAction = (string)rdr["Maker_ReasonOfAction"];

            Stage = (string)rdr["Stage"];

            RMCateg = (string)rdr["RMCateg"];
            MatchingAtRMCycle = (int)rdr["MatchingAtRMCycle"];

            RMCycle = (int)rdr["RMCycle"];

            AtmNo = (string)rdr["AtmNo"];
            ReplCycle = (int)rdr["ReplCycle"];

            ActionAtDateTime = (DateTime)rdr["ActionAtDateTime"];

            Settled = (bool)rdr["Settled"];

            OriginWorkFlow = (string)rdr["OriginWorkFlow"];

            Operator = (string)rdr["Operator"];
        }

        //
        // READ Action Occurances by Unique Record Id
        //

        public void ReadActionsOccurancesByUniqueRecordId(int InUniqueKey)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = " SELECT * "
                      + " FROM [ATMS].[dbo].[Actions_Occurances]"
                      + " WHERE UniqueKeyOrigin = 'Master_Pool' AND UniqueKey = @UniqueKey  "
                       + "   ";

            using (SqlConnection conn =
                           new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                        //cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        //cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

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
        // READ Action Occurances by Unique Record Id
        //

        public void ReadActionsOccurancesByUniqueRecordIdFirstAction(int InUniqueKey, string InOriginWorkFlow)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = " SELECT TOP (1) *  "
                      + " FROM [ATMS].[dbo].[Actions_Occurances]"
                      + " WHERE UniqueKeyOrigin = 'Master_Pool' AND UniqueKey = @UniqueKey  "
                       + " AND  OriginWorkFlow = @OriginWorkFlow "
                       + " AND ActionId In ( '03', '04', '05', '06', '08', '10','11', '71', '81', '91', '92', '95', '96')  "; // these are valid actions 

            using (SqlConnection conn =
                           new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                        cmd.Parameters.AddWithValue("@OriginWorkFlow", InOriginWorkFlow);
                        //cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

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
        // Read last action by unique key 
        public void ReadActionsOccurancesByUniqueRecordIdActionIdLastAction(int InUniqueKey)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = " SELECT *  "
                      + " FROM [ATMS].[dbo].[Actions_Occurances]"
                      + " WHERE UniqueKeyOrigin = 'Master_Pool' AND UniqueKey = @UniqueKey  "
                       //+ " AND  ActionId = @ActionId   " 
                      + " ORDER by SeqNo "
                       ;

            using (SqlConnection conn =
                           new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                       // cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        //cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

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
        // READ Action Occurances AND Filled table BIG 
        //

        public void ReadActionsOccurancesAndFillTable_Big(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableActionOccurances_Big = new DataTable();
            TableActionOccurances_Big.Clear();
            //RowSelected["ActionId"] = ActionId;
            //RowSelected["Occurance"] = Occurance.ToString();

            SqlString = " SELECT * "
                      + " FROM [ATMS].[dbo].[Actions_Occurances]"
                      + InSelectionCriteria
                       + " Order By SeqNo ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        // sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@TerminalId", InAtmNo);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(TableActionOccurances_Big);

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


        // Read to see if found occurances for this ATM and Repl Cycle 
        public void ReadActionsOccurancesToSeeIf_Exists(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableActionOccurances_Big = new DataTable();
            TableActionOccurances_Big.Clear();
            //RowSelected["ActionId"] = ActionId;
            //RowSelected["Occurance"] = Occurance.ToString();

            SqlString = " SELECT * "
                      + " FROM [ATMS].[dbo].[Actions_Occurances]"
                      + InSelectionCriteria
                       + " Order By ActionId DESC ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       
                        //cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        //cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

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
        // READ Action Occurances AND Filled table Small
        //

        public void ReadActionsOccurancesAndFillTable_Small(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableActionOccurances_Small = new DataTable();
            TableActionOccurances_Small.Clear();

            // DATA TABLE ROWS DEFINITION 
            TableActionOccurances_Small.Columns.Add("SeqNo", typeof(int));
            TableActionOccurances_Small.Columns.Add("ActionId", typeof(string));
            TableActionOccurances_Small.Columns.Add("Occurance", typeof(string));
            TableActionOccurances_Small.Columns.Add("ActionNm", typeof(string));
            TableActionOccurances_Small.Columns.Add("ActionAtDateTime", typeof(string));
            TableActionOccurances_Small.Columns.Add("Amount", typeof(string));
            TableActionOccurances_Small.Columns.Add("Branch_1", typeof(string));
            TableActionOccurances_Small.Columns.Add("AccNo_1", typeof(string));
            TableActionOccurances_Small.Columns.Add("AccName_1", typeof(string));
            TableActionOccurances_Small.Columns.Add("DR/CR_1", typeof(string));
            TableActionOccurances_Small.Columns.Add("Description_1", typeof(string));
            TableActionOccurances_Small.Columns.Add("Branch_2", typeof(string));
            TableActionOccurances_Small.Columns.Add("AccNo_2", typeof(string));
            TableActionOccurances_Small.Columns.Add("AccName_2", typeof(string));
            TableActionOccurances_Small.Columns.Add("DR/CR_2", typeof(string));
            TableActionOccurances_Small.Columns.Add("Description_2", typeof(string));

            TableActionOccurances_Small.Columns.Add("Stage", typeof(string));

            TableActionOccurances_Small.Columns.Add("WorkFlow", typeof(string));

            SqlString = " SELECT * "
                      + " FROM [ATMS].[dbo].[Actions_Occurances]"
                      + InSelectionCriteria
                       + " Order By SeqNo  ";

            using (SqlConnection conn =
                           new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                        //cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        //cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

                            //FILL TABLE 
                            DataRow RowSelected = TableActionOccurances_Small.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["ActionId"] = ActionId;
                            RowSelected["Occurance"] = Occurance.ToString();
                            RowSelected["ActionNm"] = ActionNm;
                            RowSelected["ActionAtDateTime"] = ActionAtDateTime.ToString();
                            RowSelected["Amount"] = DoubleEntryAmt.ToString("#,##0.00");
                            RowSelected["Branch_1"] = Branch_1;
                            RowSelected["AccNo_1"] = AccNo_1;
                            RowSelected["AccName_1"] = AccName_1;
                            RowSelected["DR/CR_1"] = GL_Sign_1;
                            RowSelected["Description_1"] = StatementDesc_1;
                            RowSelected["Branch_2"] = Branch_2;
                            RowSelected["AccNo_2"] = AccNo_2;
                            RowSelected["AccName_2"] = AccName_2;
                            RowSelected["DR/CR_2"] = GL_Sign_2;
                            RowSelected["Description_2"] = StatementDesc_2;

                            RowSelected["Stage"] = Stage;

                            RowSelected["WorkFlow"] = OriginWorkFlow;

                            
                            // Description
                            // ADD ROW
                            TableActionOccurances_Small.Rows.Add(RowSelected);
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
        // READ Action Occurances AND Filled table Small
        //

        public void ReadActionsOccurancesAndFillTable_Small_Manual(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableActionOccurances_Small = new DataTable();
            TableActionOccurances_Small.Clear();

            // DATA TABLE ROWS DEFINITION 
            TableActionOccurances_Small.Columns.Add("SeqNo", typeof(int));
            TableActionOccurances_Small.Columns.Add("ActionId", typeof(string));
            TableActionOccurances_Small.Columns.Add("Occurance", typeof(string));
            TableActionOccurances_Small.Columns.Add("ActionNm", typeof(string));
            TableActionOccurances_Small.Columns.Add("Action Reason", typeof(string));
            TableActionOccurances_Small.Columns.Add("ActionDateTime", typeof(string));
            TableActionOccurances_Small.Columns.Add("Terminal", typeof(string));
            TableActionOccurances_Small.Columns.Add("CardNo", typeof(string));
            TableActionOccurances_Small.Columns.Add("AccNo", typeof(string));
            TableActionOccurances_Small.Columns.Add("Amount", typeof(string));
            TableActionOccurances_Small.Columns.Add("TraceNo", typeof(string));
            TableActionOccurances_Small.Columns.Add("RRNumber", typeof(string));
            TableActionOccurances_Small.Columns.Add("TransDate", typeof(string));
            TableActionOccurances_Small.Columns.Add("Stage", typeof(string));

            SqlString = " SELECT * "
                      + " FROM [ATMS].[dbo].[Actions_Occurances]"
                      + InSelectionCriteria
                       + " Order By SeqNo  ";

            using (SqlConnection conn =
                           new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                        //cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        //cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

                            //FILL TABLE 
                            DataRow RowSelected = TableActionOccurances_Small.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["ActionId"] = ActionId;
                            RowSelected["Occurance"] = Occurance.ToString();
                            RowSelected["ActionNm"] = ActionNm;
                            RowSelected["Action Reason"] = Maker_ReasonOfAction;
                            RowSelected["ActionDateTime"] = ActionAtDateTime.ToString();
                            RowSelected["Stage"] = Stage;

                            if (UniqueKeyOrigin == "Master_Pool")
                            {
                                // Read record 
                                Mpa.ReadInPoolTransSpecificUniqueRecordId(UniqueKey, 2);

                                RowSelected["Terminal"] = Mpa.TerminalId;
                                RowSelected["CardNo"] = Mpa.CardNumber;
                                RowSelected["AccNo"] = Mpa.AccNumber;
                                RowSelected["Amount"] = Mpa.TransAmount.ToString("#,##0.00");
                                RowSelected["TraceNo"] = Mpa.TraceNoWithNoEndZero.ToString();
                                RowSelected["RRNumber"] = Mpa.RRNumber;
                                RowSelected["TransDate"] = Mpa.TransDate;
                            }
                            else
                            {
                                MessageBox.Show("Handle this in class Actions_Occurances");
                                return;
                            }



                            // Description
                            // ADD ROW
                            TableActionOccurances_Small.Rows.Add(RowSelected);
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
        // READ Action Occurances AND Get Totals For 
        //      Excess and Shortage
        //
        public decimal CIT_Unload;
        public decimal CIT_Load;
        public decimal CIT_Deposits;

        public decimal Excess;
        public decimal CIT_Shortage;

        public decimal Shortage;

        public decimal Current_DisputeShortage;

        public decimal Current_ExcessBalance;
        public decimal Current_ShortageBalance;

        public decimal CIT_Returns;

        public decimal Current_CR_Customer;
        public decimal Current_DR_Customer;

        public decimal Action_CR_ATMCash;
        public decimal Action_DR_ATMCash;


        public void ReadActionsOccurancesBySelectionCriteriaToGetTotals(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            CIT_Unload = 0;
            CIT_Load = 0;
            CIT_Deposits = 0; 

            Excess = 0;
            CIT_Shortage = 0;

            Shortage = 0;

            Current_DisputeShortage = 0;

            Current_ExcessBalance = 0; // Short id = 40

            Current_ShortageBalance = 0; // Short id = 50

            CIT_Returns = 0;

            Current_CR_Customer = 0; // Short id = 20
            Current_DR_Customer = 0;// Short id = 20

            SqlString = " SELECT * "
                      + " FROM [ATMS].[dbo].[Actions_Occurances]"
                      + InSelectionCriteria
                       + " Order By SeqNo  ";

            using (SqlConnection conn =
                           new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);

                        // Read table 
                        cmd.CommandTimeout = 35;  // seconds
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

                            if (Is_GL_Action == true)
                            {
                                if (ActionId == "25")
                                {
                                    // Debit CIT // SM_UNLOAD 
                                    CIT_Unload = DoubleEntryAmt;
                                }
                                if (ActionId == "24")
                                {
                                    // CREDIT CIT // SM_LOAD 
                                    CIT_Load = DoubleEntryAmt;
                                }
                                // Deposits 
                                if (ActionId == "26")
                                {
                                    // CREDIT CIT Deposits 
                                    CIT_Deposits = DoubleEntryAmt;
                                }

                                if (ActionId == "30" || ActionId == "28")
                                {
                                    // Excess by CIT
                                    Excess = Excess + DoubleEntryAmt;
                                }
                                // CIT 
                                if (ActionId == "29" || ActionId == "27")
                                {
                                    // Shortage by CIT 
                                    CIT_Shortage = CIT_Shortage + DoubleEntryAmt;
                                }
                                // Non CIT 
                                if (ActionId == "39" || ActionId == "37")
                                {
                                    // Shortage Generic 
                                    Shortage = Shortage + DoubleEntryAmt;
                                }

                                // CIT Returns 
                                if (ActionId == "86")
                                {
                                    // Returns from CIT
                                    CIT_Returns = CIT_Returns + DoubleEntryAmt;
                                }

                                // CURRENT EXCESS
                                // First Entry Excess
                                if (GL_Sign_1 == "CR" & ShortAccID_1 == "40")
                                {
                                    Current_ExcessBalance = Current_ExcessBalance + DoubleEntryAmt;
                                }
                                if (GL_Sign_1 == "DR" & ShortAccID_1 == "40")
                                {
                                    Current_ExcessBalance = Current_ExcessBalance - DoubleEntryAmt;
                                }
                                // Second Entry Excess
                                if (GL_Sign_2 == "CR" & ShortAccID_2 == "40")
                                {
                                    Current_ExcessBalance = Current_ExcessBalance + DoubleEntryAmt;
                                }
                                if (GL_Sign_2 == "DR" & ShortAccID_2 == "40")
                                {
                                    Current_ExcessBalance = Current_ExcessBalance - DoubleEntryAmt;
                                }
                                // Dispute_Shortage
                                // First Entry Excess
                                if (GL_Sign_1 == "CR" & ShortAccID_1 == "49")
                                {
                                    Current_DisputeShortage = Current_DisputeShortage + DoubleEntryAmt;
                                }
                                if (GL_Sign_1 == "DR" & ShortAccID_1 == "49")
                                {
                                    Current_DisputeShortage = Current_DisputeShortage - DoubleEntryAmt;
                                }
                                // Second Entry Excess
                                if (GL_Sign_2 == "CR" & ShortAccID_2 == "49")
                                {
                                    Current_DisputeShortage = Current_DisputeShortage + DoubleEntryAmt;
                                }
                                if (GL_Sign_2 == "DR" & ShortAccID_2 == "49")
                                {
                                    Current_DisputeShortage = Current_DisputeShortage - DoubleEntryAmt;
                                }

                                // CURRENT Shortage CIT
                                // First Entry Shortage
                                if (GL_Sign_1 == "CR" & ShortAccID_1 == "50")
                                {
                                    Current_ShortageBalance = Current_ShortageBalance + DoubleEntryAmt;
                                }
                                if (GL_Sign_1 == "DR" & ShortAccID_1 == "50")
                                {
                                    Current_ShortageBalance = Current_ShortageBalance - DoubleEntryAmt;
                                }
                                // Second Entry Shortage
                                if (GL_Sign_2 == "CR" & ShortAccID_2 == "50")
                                {
                                    Current_ShortageBalance = Current_ShortageBalance + DoubleEntryAmt;
                                }
                                if (GL_Sign_2 == "DR" & ShortAccID_2 == "50")
                                {
                                    Current_ShortageBalance = Current_ShortageBalance - DoubleEntryAmt;
                                }

                                // CURRENT Shortage Generic 
                                // First Entry Shortage
                                if (GL_Sign_1 == "CR" & ShortAccID_1 == "51")
                                {
                                    Current_ShortageBalance = Current_ShortageBalance + DoubleEntryAmt;
                                }
                                if (GL_Sign_1 == "DR" & ShortAccID_1 == "51")
                                {
                                    Current_ShortageBalance = Current_ShortageBalance - DoubleEntryAmt;
                                }
                                // Second Entry Shortage
                                if (GL_Sign_2 == "CR" & ShortAccID_2 == "51")
                                {
                                    Current_ShortageBalance = Current_ShortageBalance + DoubleEntryAmt;
                                }
                                if (GL_Sign_2 == "DR" & ShortAccID_2 == "51")
                                {
                                    Current_ShortageBalance = Current_ShortageBalance - DoubleEntryAmt;
                                }

                                // CURRENT CUSTOMER 
                                // First Entry Shortage
                                if (GL_Sign_1 == "CR" & ShortAccID_1 == "20")
                                {
                                    Current_CR_Customer = Current_CR_Customer + DoubleEntryAmt;
                                }
                                if (GL_Sign_1 == "DR" & ShortAccID_1 == "20")
                                {
                                    Current_DR_Customer = Current_CR_Customer - DoubleEntryAmt;
                                }
                                // Second Entry Shortage
                                if (GL_Sign_2 == "CR" & ShortAccID_2 == "20")
                                {
                                    Current_CR_Customer = Current_CR_Customer + DoubleEntryAmt;
                                }
                                if (GL_Sign_2 == "DR" & ShortAccID_2 == "20")
                                {
                                    Current_DR_Customer = Current_CR_Customer - DoubleEntryAmt;
                                }

                            }
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    // FIND If we have Current Excess or Current Shortage 
                    if (Current_ExcessBalance == 0 || Current_ShortageBalance == 0)
                    {
                        // Do nothing 
                    }
                    else
                    {
                        // Both of them greater than zero => Combine them 
                        if (Current_ExcessBalance == -Current_ShortageBalance)
                        {
                            Current_ExcessBalance = 0;
                            Current_ShortageBalance = 0;
                        }
                        if (Current_ExcessBalance > -Current_ShortageBalance)
                        {
                            Current_ExcessBalance = Current_ExcessBalance - (-Current_ShortageBalance);
                            Current_ShortageBalance = 0;
                        }
                        if (-Current_ShortageBalance > Current_ExcessBalance)
                        {
                            Current_ShortageBalance = Current_ShortageBalance + Current_ExcessBalance;
                            Current_ExcessBalance = 0;
                        }
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

        }

        public void ReadActionsOccurancesBySelectionCriteriaToGetTotals_AUDI_Type(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // THIS IS RELATED WITH AUDI TYPE

            CIT_Unload = 0;

            CIT_Load = 0;

            Excess = 0;
            CIT_Shortage = 0;

            Shortage = 0;

            Current_DisputeShortage = 0;

            Current_ExcessBalance = 0; // Short id = 32

            Current_ShortageBalance = 0; // Short id = 33

            CIT_Returns = 0;

            Current_CR_Customer = 0; // Short id = 20
            Current_DR_Customer = 0;// Short id = 20

            Action_CR_ATMCash = 0; // Short id = 30
            Action_DR_ATMCash = 0; // Short id = 30

            SqlString = " SELECT * "
                      + " FROM [ATMS].[dbo].[Actions_Occurances]"
                      + InSelectionCriteria
                       + " Order By SeqNo  ";

            using (SqlConnection conn =
                           new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);

                        // Read table 
                        cmd.CommandTimeout = 35;  // seconds
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

                            if (Is_GL_Action == true)
                            {
                                if (ActionId == "18")
                                {
                                    // Debit CIT // SM_UNLOAD 
                                    CIT_Unload = DoubleEntryAmt;
                                }
                                if (ActionId == "14")
                                {
                                    // CREDIT CIT // SM_LOAD 
                                    CIT_Load = DoubleEntryAmt;
                                }
                                if (ActionId == "20")
                                {
                                    // Excess For ATM
                                    Excess = Excess + DoubleEntryAmt;
                                }
                                //// CIT 
                                //if (ActionId == "21" )
                                //{
                                //    // Shortage by CIT 
                                //    CIT_Shortage = CIT_Shortage + DoubleEntryAmt;
                                //}
                                // Non CIT 
                                if (ActionId == "21")
                                {
                                    // Shortage Generic 
                                    Shortage = Shortage + DoubleEntryAmt;
                                }

                                // CURRENT EXCESS
                                // First Entry Excess
                                if (GL_Sign_1 == "CR" & ShortAccID_1 == "32")
                                {
                                    Current_ExcessBalance = Current_ExcessBalance + DoubleEntryAmt;
                                }
                                if (GL_Sign_1 == "DR" & ShortAccID_1 == "32")
                                {
                                    Current_ExcessBalance = Current_ExcessBalance - DoubleEntryAmt;
                                }
                                // Second Entry Excess
                                if (GL_Sign_2 == "CR" & ShortAccID_2 == "32")
                                {
                                    Current_ExcessBalance = Current_ExcessBalance + DoubleEntryAmt;
                                }
                                if (GL_Sign_2 == "DR" & ShortAccID_2 == "32")
                                {
                                    Current_ExcessBalance = Current_ExcessBalance - DoubleEntryAmt;
                                }
                                // Dispute_Shortage
                                // First Entry Excess
                                //if (GL_Sign_1 == "CR" & ShortAccID_1 == "33")
                                //{
                                //    Current_DisputeShortage = Current_DisputeShortage + DoubleEntryAmt;
                                //}
                                //if (GL_Sign_1 == "DR" & ShortAccID_1 == "33")
                                //{
                                //    Current_DisputeShortage = Current_DisputeShortage - DoubleEntryAmt;
                                //}
                                //// Second Entry Excess
                                //if (GL_Sign_2 == "CR" & ShortAccID_2 == "33")
                                //{
                                //    Current_DisputeShortage = Current_DisputeShortage + DoubleEntryAmt;
                                //}
                                //if (GL_Sign_2 == "DR" & ShortAccID_2 == "33")
                                //{
                                //    Current_DisputeShortage = Current_DisputeShortage - DoubleEntryAmt;
                                //}

                                // CURRENT Shortage CIT
                                // First Entry Shortage
                                if (GL_Sign_1 == "CR" & ShortAccID_1 == "33")
                                {
                                    Current_ShortageBalance = Current_ShortageBalance + DoubleEntryAmt;
                                }
                                if (GL_Sign_1 == "DR" & ShortAccID_1 == "33")
                                {
                                    Current_ShortageBalance = Current_ShortageBalance - DoubleEntryAmt;
                                }
                                // Second Entry Shortage
                                if (GL_Sign_2 == "CR" & ShortAccID_2 == "33")
                                {
                                    Current_ShortageBalance = Current_ShortageBalance + DoubleEntryAmt;
                                }
                                if (GL_Sign_2 == "DR" & ShortAccID_2 == "33")
                                {
                                    Current_ShortageBalance = Current_ShortageBalance - DoubleEntryAmt;
                                }

                                // CURRENT Shortage Generic 
                                // First Entry Shortage
                                //if (GL_Sign_1 == "CR" & ShortAccID_1 == "51")
                                //{
                                //    Current_ShortageBalance = Current_ShortageBalance + DoubleEntryAmt;
                                //}
                                //if (GL_Sign_1 == "DR" & ShortAccID_1 == "51")
                                //{
                                //    Current_ShortageBalance = Current_ShortageBalance - DoubleEntryAmt;
                                //}
                                //// Second Entry Shortage
                                //if (GL_Sign_2 == "CR" & ShortAccID_2 == "51")
                                //{
                                //    Current_ShortageBalance = Current_ShortageBalance + DoubleEntryAmt;
                                //}
                                //if (GL_Sign_2 == "DR" & ShortAccID_2 == "51")
                                //{
                                //    Current_ShortageBalance = Current_ShortageBalance - DoubleEntryAmt;
                                //}

                                // CURRENT CUSTOMER 
                                // First Entry Customer 
                                if (GL_Sign_1 == "CR" & ShortAccID_1 == "20")
                                {
                                    Current_CR_Customer = Current_CR_Customer + DoubleEntryAmt;
                                }
                                if (GL_Sign_1 == "DR" & ShortAccID_1 == "20")
                                {
                                    Current_DR_Customer = Current_CR_Customer - DoubleEntryAmt;
                                }
                                // Second Entry Customer 
                                if (GL_Sign_2 == "CR" & ShortAccID_2 == "20")
                                {
                                    Current_CR_Customer = Current_CR_Customer + DoubleEntryAmt;
                                }
                                if (GL_Sign_2 == "DR" & ShortAccID_2 == "20")
                                {
                                    Current_DR_Customer = Current_CR_Customer - DoubleEntryAmt;
                                }

                                // ATM Cash 
                                // First Entry ATM Cash 
                                if (GL_Sign_1 == "CR" & ShortAccID_1 == "30")
                                {
                                    Action_CR_ATMCash = Action_CR_ATMCash + DoubleEntryAmt;
                                }
                                if (GL_Sign_1 == "DR" & ShortAccID_1 == "30")
                                {
                                    Action_DR_ATMCash = Action_DR_ATMCash - DoubleEntryAmt;
                                }
                                // OR Second Entry ATM Cash
                                if (GL_Sign_2 == "CR" & ShortAccID_2 == "30")
                                {
                                    Action_CR_ATMCash = Action_CR_ATMCash + DoubleEntryAmt;
                                }
                                if (GL_Sign_2 == "DR" & ShortAccID_2 == "30")
                                {
                                    Action_DR_ATMCash = Action_DR_ATMCash - DoubleEntryAmt;
                                }

                            }
                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    // FIND If we have Current Excess or Current Shortage 
                    if (Current_ExcessBalance == 0 || Current_ShortageBalance == 0)
                    {
                        // Do nothing 
                    }
                    else
                    {
                        // Both of them greater than zero => Combine them 
                        if (Current_ExcessBalance == -Current_ShortageBalance)
                        {
                            Current_ExcessBalance = 0;
                            Current_ShortageBalance = 0;
                        }
                        if (Current_ExcessBalance > -Current_ShortageBalance)
                        {
                            Current_ExcessBalance = Current_ExcessBalance - (-Current_ShortageBalance);
                            Current_ShortageBalance = 0;
                        }
                        if (-Current_ShortageBalance > Current_ExcessBalance)
                        {
                            Current_ShortageBalance = Current_ShortageBalance + Current_ExcessBalance;
                            Current_ExcessBalance = 0;
                        }
                    }

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

        }


        // WAIT FOR DISPUTE 
        public int WaitForDisputeNo = 0;
        public decimal WaitForDisputeAmt = 0;

        public int WaitAndOpenDisputeNo = 0;
        public decimal WaitAndOpenDisputeAmt = 0;

        public int WaitAndSettledDisputeNo = 0;
        public decimal WaitAndSettledDisputeAmt = 0;

        // NOT WAIT
        public int NoWaitDisputeNo = 0;
        public decimal NoWaitDisputeAmt = 0;

        public int NoWaitSettledDisputeNo = 0;
        public decimal NoWaitSettledDisputeAmt = 0;
        //
        // GET RICH PICTURE FOR A UNIT OF WORK ATM and Replenishment Cycle 
        //
        public void ReadActionsOccurancesTo_RichPicture_One_ATM(string InAtmNo, int InReplCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();

            decimal R_Current_ExcessBalance = 0; // Short id = 40
            decimal R_Current_ShortageBalance = 0; // Short id = 50

            // Fill up a MemoryTable and interrogate sequentially  
            string WSelectionCriteria = "WHERE AtmNo ='" + InAtmNo
                + "' AND ReplCycle =" + InReplCycleNo + "";

            ReadActionsOccurancesBySelectionCriteriaToGetTotals(WSelectionCriteria);

            WaitForDisputeNo = 0;
            WaitForDisputeAmt = 0;

            WaitAndOpenDisputeNo = 0;
            WaitAndOpenDisputeAmt = 0;

            WaitAndSettledDisputeNo = 0;
            WaitAndSettledDisputeAmt = 0;
            // Not Wait
            NoWaitDisputeNo = 0;
            NoWaitDisputeAmt = 0;

            NoWaitSettledDisputeNo = 0;
            NoWaitSettledDisputeAmt = 0;

            ReadActionsOccurancesAndFillTable_Small(WSelectionCriteria);

            try
            {

                int I = 0;
                int K = TableActionOccurances_Small.Rows.Count - 1;
                // bool VariablesEqual = false;

                //if (I == K) VariablesEqual = true;

                while (I <= K)
                {

                    RecordFound = true;

                    int WSeqNo = (int)TableActionOccurances_Small.Rows[I]["SeqNo"];
                    string WActionId = (string)TableActionOccurances_Small.Rows[I]["ActionId"];

                    ReadActionsOccuarnceBySeqNo(WSeqNo);


                    //if (Is_GL_Action == false)
                    //{
                    if (ActionId == "06") // Wait for Dispute
                    {

                        WaitForDisputeNo = WaitForDisputeNo + 1;
                        WaitForDisputeAmt = WaitForDisputeAmt + DoubleEntryAmt;
                        // Wait for Dispute
                        // Check if dispute was open for this Unit number
                        string ActionIdForDispute = "05"; // 05_Move case  to dispute 
                        int WMode = 1;
                        ReadActionsOccurancesByUniqueKey_By_Mode(UniqueKeyOrigin, UniqueKey
                                                                              , ActionIdForDispute, WMode);

                        if (RecordFound == true)
                        {
                            // corresponding dispute found
                            // Find out if action is taken 

                            Dt.ReadDisputeTranByUniqueRecordId(UniqueKey);
                            if (Dt.RecordFound == true)
                            {
                                WaitAndOpenDisputeNo = WaitAndOpenDisputeNo + 1;
                                WaitAndOpenDisputeAmt = WaitAndOpenDisputeAmt + Dt.DisputedAmt;

                                if (Dt.ClosedDispute == true)
                                {

                                    WaitAndSettledDisputeNo = WaitAndSettledDisputeNo + 1;
                                    WaitAndSettledDisputeAmt = WaitAndSettledDisputeAmt + Dt.DecidedAmount;
                                }
                                else
                                {
                                    // Not settled is the 
                                }

                            }
                        }
                        else
                        {
                            // No corresponding dispute found

                        }

                    }
                    //}
                    //
                    // No Wait Dispute 
                    //
                    if (ActionId == "05") // Open Dispute alone - No wait
                    {
                        // Check if Wait for dispute was present 
                        string ActionIdForDispute = "06"; // 06_Wait For Dispute 
                        int WMode = 1;
                        ReadActionsOccurancesByUniqueKey_By_Mode(UniqueKeyOrigin, UniqueKey
                                                                              , ActionIdForDispute, WMode);

                        if (RecordFound == true)
                        {
                            // Not to be considered 
                        }
                        else
                        {
                            Dt.ReadDisputeTranByUniqueRecordId(UniqueKey);
                            if (Dt.RecordFound == true)
                            {
                                NoWaitDisputeNo = NoWaitDisputeNo + 1;
                                NoWaitDisputeAmt = NoWaitDisputeAmt + Dt.DisputedAmt;

                                if (Dt.ClosedDispute == true)
                                {
                                    NoWaitSettledDisputeNo = NoWaitSettledDisputeNo + 1;
                                    NoWaitSettledDisputeAmt = NoWaitSettledDisputeAmt + Dt.DecidedAmount;
                                }
                            }
                        }

                    }

                    I++; // Read Next entry of the table 

                }

            }
            catch (Exception ex)
            {

                CatchDetails(ex);
                return;

            }

            // Fill the table for each AtmNo and Repl Cycle c

        }

        //
        // GET RICH PICTURE FOR A UNIT OF WORK ATM and Replenishment Cycle 
        //
        public DataTable TxnsTableAllCycles;
        public DataTable TxnsTableAllCycles_Details;
        public decimal TotalUnload;
        public decimal TotalDeposits;
        public decimal Totalload;
        public decimal TotalExcess;
        public decimal TotalShortages;
        public decimal TotalDisputeShortages;
        public string WOperator; 

        public bool IsSemaphore;
        public void ReadRichPicture_ALL_ATMs_By_Selection(string InSignedId,string InOperator,string InSelectionCriteria, DateTime InFromDt, 
            DateTime InToDt, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            IsSemaphore = false;

            WOperator = InOperator; 

            TotalUnload = 0;
            TotalDeposits = 0;
            Totalload = 0;
            TotalExcess = 0;
            TotalShortages = 0;
            TotalDisputeShortages = 0;

            // CASH MANAGEMENT
            RRDMGasParameters Gp = new RRDMGasParameters();
            RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms();
            //string WOperator = "BCAIEGCX";
            string ParId = "555";
            string OccurId = "01";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            
            if (Gp.OccuranceNm != "N/A")
            {
                if (Gp.OccuranceNm == "YES")
                {
                    MessageBox.Show("Semaphore_01 : Wait for a while and try again");
                    IsSemaphore = true;
                    return;
                }
                else
                {
                    Gp.OccuranceNm = "YES";
                    Gp.UpdateGasParamByKey(Gp.Operator, Gp.RefKey);
                }
            }

            //Semaphore semaphoreObject = new Semaphore(initialCount: 1, maximumCount: 1);

            RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate(); // Activate Class 
            RRDMAtmsClass Ac = new RRDMAtmsClass();

            //MessageBox.Show("Enter the process of Semaphore"); 

            TxnsTableAllCycles = new DataTable();
            TxnsTableAllCycles.Clear();

            TxnsTableAllCycles_Details = new DataTable();
            TxnsTableAllCycles_Details.Clear();

            if (InMode == 1)
            {
                SqlString = " SELECT * "
                     + " FROM [ATMS].[dbo].[SessionsStatusTraces] "
                     + " WHERE DiffRepl = 1 "
                     + " ORDER By AtmNo , SesNo  ";
            }

            if (InMode == 8)
            {
                SqlString = " SELECT * "
                     + " FROM [ATMS].[dbo].[SessionsStatusTraces] "
                     + " WHERE DiffRepl = 1 and Maker =@Maker "
                     + " ORDER By AtmNo , SesNo  ";
            }

            if (InMode == 2 || InMode == 3 || InMode == 4 || InMode == 7 || InMode == 9 )
            {
                SqlString = " SELECT * "
                     + " FROM [ATMS].[dbo].[SessionsStatusTraces] "
                     + InSelectionCriteria
                     + " ORDER By AtmNo , SesNo  ";
            }

            if (InMode == 5)
            {
                SqlString = " SELECT * "
                     + " FROM [ATMS].[dbo].[SessionsStatusTraces] "
                     + InSelectionCriteria
                     + " ORDER By SesDtTimeEnd DESC  ";
            }

            if (InMode == 6)
            {
                SqlString = " SELECT * "
                     + " FROM [ATMS].[dbo].[SessionsStatusTraces] "
                     + " WHERE ProcessMode IN (0, -5, -6) "
                     + "  AND SesDtTimeEnd >= @FromDt  AND SesDtTimeEnd < @ToDt "
                     + " ORDER By AtmNo , SesNo   ";
            }

            using (SqlConnection conn =
                      new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Maker", InSignedId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@FromDt", InFromDt);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ToDt", InToDt.AddDays(1));

                        //Create a datatable that will be filled with the data retrieved from the command
                        sqlAdapt.SelectCommand.CommandTimeout = 300;
                        sqlAdapt.Fill(TxnsTableAllCycles);

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    if (Gp.OccuranceNm != "N/A")
                    {
                        Gp.OccuranceNm = "NO";
                        Gp.UpdateGasParamByKey(Gp.Operator, Gp.RefKey);
                    }

                    CatchDetails(ex);
                }

            
            // DATA TABLE ROWS DEFINITION 
            TxnsTableAllCycles_Details.Columns.Add("ATM_No", typeof(string));
            TxnsTableAllCycles_Details.Columns.Add("Repl_No", typeof(int));
            TxnsTableAllCycles_Details.Columns.Add("ProcessMode", typeof(int));

            TxnsTableAllCycles_Details.Columns.Add("Cycle_Start_DATE", typeof(DateTime)); // Ses Start Date
            TxnsTableAllCycles_Details.Columns.Add("SM_DATE", typeof(DateTime)); // SesDtTimeEnd

            TxnsTableAllCycles_Details.Columns.Add("Cit_Id", typeof(string)); // CIT
            TxnsTableAllCycles_Details.Columns.Add("Group", typeof(string)); // Group
            TxnsTableAllCycles_Details.Columns.Add("Owner", typeof(string)); // Owner

            TxnsTableAllCycles_Details.Columns.Add("CIT_Unload", typeof(decimal));
            TxnsTableAllCycles_Details.Columns.Add("CIT_Deposits", typeof(decimal));
            TxnsTableAllCycles_Details.Columns.Add("CIT_Load", typeof(decimal));

            TxnsTableAllCycles_Details.Columns.Add("CIT_Excess", typeof(decimal));
            TxnsTableAllCycles_Details.Columns.Add("CIT_Shortage", typeof(decimal));

            TxnsTableAllCycles_Details.Columns.Add("Current_DisputeShortage", typeof(decimal));

            TxnsTableAllCycles_Details.Columns.Add("Current_ExcessBalance", typeof(decimal));
            TxnsTableAllCycles_Details.Columns.Add("Current_ShortageBalance", typeof(decimal));

            TxnsTableAllCycles_Details.Columns.Add("Current_CR_Customer", typeof(decimal));
            TxnsTableAllCycles_Details.Columns.Add("Current_DR_Customer", typeof(decimal));
            //OTHER
            TxnsTableAllCycles_Details.Columns.Add("WaitForDisputeNo", typeof(int));
            TxnsTableAllCycles_Details.Columns.Add("WaitForDisputeAmt", typeof(decimal));

            TxnsTableAllCycles_Details.Columns.Add("WaitAndOpenDisputeNo", typeof(int));
            TxnsTableAllCycles_Details.Columns.Add("WaitAndOpenDisputeAmt", typeof(decimal));

            TxnsTableAllCycles_Details.Columns.Add("WaitAndSettledDisputeNo", typeof(int));
            TxnsTableAllCycles_Details.Columns.Add("WaitAndSettledDisputeAmt", typeof(decimal));

            TxnsTableAllCycles_Details.Columns.Add("NoWaitDisputeNo", typeof(int));
            TxnsTableAllCycles_Details.Columns.Add("NoWaitDisputeAmt", typeof(decimal));

            TxnsTableAllCycles_Details.Columns.Add("NoWaitSettledDisputeNo", typeof(int));
            TxnsTableAllCycles_Details.Columns.Add("NoWaitSettledDisputeAmt", typeof(decimal));

            TxnsTableAllCycles_Details.Columns.Add("Rec_DATE", typeof(DateTime)); // ReplFinDtTm

            TxnsTableAllCycles_Details.Columns.Add("Maker", typeof(string)); // Maker
            TxnsTableAllCycles_Details.Columns.Add("Auth", typeof(string)); // Authoriser

            // if (Current_ShortageBalance < 0
            //     || WaitForDisputeNo < WaitAndSettledDisputeNo
            //     || NoWaitDisputeNo < NoWaitSettledDisputeAmt ) 
            // 

            try
            {

                int I = 0;
                int K = TxnsTableAllCycles.Rows.Count - 1;

                while (I <= K)
                {

                    RecordFound = true;

                    string WAtmNo = (string)TxnsTableAllCycles.Rows[I]["AtmNo"];
                    int Repl_No = (int)TxnsTableAllCycles.Rows[I]["SesNo"];


                    // READ THE NEEDED FIELDS
                    // Rich Picture
                    ReadActionsOccurancesTo_RichPicture_One_ATM(WAtmNo, Repl_No);

                    // FILL TABLE

                    //NEW ROW
                    DataRow RowSelected = TxnsTableAllCycles_Details.NewRow();

                    RowSelected["ATM_No"] = WAtmNo;
                    RowSelected["Repl_No"] = Repl_No;

                    Ta.ReadSessionsStatusTraces(WAtmNo, Repl_No);

                    if (Ta.RecordFound == true)
                    {
                        RowSelected["ProcessMode"] = Ta.ProcessMode;
                        RowSelected["Cycle_Start_DATE"] = Ta.SesDtTimeStart;
                        RowSelected["SM_DATE"] = Ta.SesDtTimeEnd;
                        RowSelected["Rec_DATE"] = Ta.Repl1.ReplFinDtTm;
                        RowSelected["Maker"] = Ta.Maker;
                        RowSelected["Auth"] = Ta.Authoriser;
                    }

                    Ac.ReadAtm(WAtmNo);
                    RowSelected["Cit_Id"] = Ac.CitId;
                    RowSelected["Group"] = Ac.AtmsReconcGroup;

                    Uaa.ReadUsersAccessAtmTableSpecificForAtmNo(WAtmNo);
                    if (Uaa.RecordFound == true)
                    {
                        RowSelected["Owner"] = Uaa.UserId;
                    }
                    else
                    {
                        RowSelected["Owner"] = "N/A";
                    }

                    RowSelected["CIT_Unload"] = CIT_Unload;
                    RowSelected["CIT_Load"] = CIT_Load;
                    RowSelected["CIT_Deposits"] = CIT_Deposits;

                    RowSelected["CIT_Excess"] = Excess;
                    RowSelected["CIT_Shortage"] = CIT_Shortage;

                    RowSelected["Current_DisputeShortage"] = Current_DisputeShortage;

                    RowSelected["Current_ExcessBalance"] = Current_ExcessBalance;
                    RowSelected["Current_ShortageBalance"] = Current_ShortageBalance;

                    TotalUnload = TotalUnload + CIT_Unload;
                    TotalDeposits = TotalDeposits + CIT_Deposits;
                    Totalload = Totalload + CIT_Load;
                    TotalExcess = TotalExcess + Current_ExcessBalance;
                    TotalShortages = TotalShortages + Current_ShortageBalance;
                    TotalDisputeShortages = TotalDisputeShortages + Current_DisputeShortage;

                    //             public decimal TotalUnload;
                    //public decimal Totalload;
                    //public decimal TotalExcess;
                    //public decimal TotalShortages;

                    RowSelected["Current_CR_Customer"] = Current_CR_Customer;
                    RowSelected["Current_DR_Customer"] = Current_DR_Customer;

                    RowSelected["WaitForDisputeNo"] = WaitForDisputeNo;
                    RowSelected["WaitForDisputeAmt"] = WaitForDisputeAmt;

                    RowSelected["WaitAndOpenDisputeNo"] = WaitAndOpenDisputeNo;
                    RowSelected["WaitAndOpenDisputeAmt"] = WaitAndOpenDisputeAmt;

                    RowSelected["WaitAndSettledDisputeNo"] = WaitAndSettledDisputeNo;
                    RowSelected["WaitAndSettledDisputeAmt"] = WaitAndSettledDisputeAmt;
                    //// Not Wait

                    RowSelected["NoWaitDisputeNo"] = NoWaitDisputeNo;
                    RowSelected["NoWaitDisputeAmt"] = NoWaitDisputeAmt;

                    RowSelected["NoWaitSettledDisputeNo"] = NoWaitSettledDisputeNo;
                    RowSelected["NoWaitSettledDisputeAmt"] = NoWaitSettledDisputeAmt;

                    // ADD ROW
                    TxnsTableAllCycles_Details.Rows.Add(RowSelected);


                    I++; // Read Next entry of the table 

                }

            }
            catch (Exception ex)
            {

                if (Gp.OccuranceNm != "N/A")
                {
                    Gp.OccuranceNm = "NO";
                    Gp.UpdateGasParamByKey(Gp.Operator, Gp.RefKey);
                }
                CatchDetails(ex);
                return;

            }


            Gp.OccuranceNm = "NO";
            Gp.UpdateGasParamByKey(Gp.Operator, Gp.RefKey);

            // semaphoreObject.Release();

            // Fill the table for each AtmNo and Repl Cycle c

        }

        //
        // READ Action Occurances AND Filled table Small + Info from Mpa
        //

        public void ReadActionsOccurancesAndFillTable_Small_Manual_AND_Other_Info(string InSignedId, int InRMCycle, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode = 25 Auditors Report 

            TableActionOccurances_Small = new DataTable();
            TableActionOccurances_Small.Clear();

            // DATA TABLE ROWS DEFINITION 
            TableActionOccurances_Small.Columns.Add("UniqueRecordId", typeof(int));
            TableActionOccurances_Small.Columns.Add("ActionId", typeof(string));
            TableActionOccurances_Small.Columns.Add("Occurance", typeof(string));
            TableActionOccurances_Small.Columns.Add("ActionNm", typeof(string));
            TableActionOccurances_Small.Columns.Add("ActionReason", typeof(string));
            TableActionOccurances_Small.Columns.Add("ActionDateTime", typeof(DateTime));
            TableActionOccurances_Small.Columns.Add("MatchMask", typeof(string)); //*
            TableActionOccurances_Small.Columns.Add("Trans_Descr", typeof(string)); // *
            TableActionOccurances_Small.Columns.Add("Terminal", typeof(string));
            TableActionOccurances_Small.Columns.Add("CardNo", typeof(string));
            TableActionOccurances_Small.Columns.Add("AccNo", typeof(string));
            TableActionOccurances_Small.Columns.Add("Amount", typeof(string));
            TableActionOccurances_Small.Columns.Add("TraceNo", typeof(string));
            TableActionOccurances_Small.Columns.Add("RRNumber", typeof(string));
            TableActionOccurances_Small.Columns.Add("TransDate", typeof(DateTime));
            TableActionOccurances_Small.Columns.Add("Maker", typeof(string)); //*
            TableActionOccurances_Small.Columns.Add("Author", typeof(string));//*
            TableActionOccurances_Small.Columns.Add("MatchingCateg", typeof(string));//*
            TableActionOccurances_Small.Columns.Add("MatchingCycle", typeof(string));//*

            TableActionOccurances_Small.Columns.Add("OriginWorkFlow", typeof(string));//*
            TableActionOccurances_Small.Columns.Add("RMCateg", typeof(string));//*
            TableActionOccurances_Small.Columns.Add("ReplCycle", typeof(string));//*

            TableActionOccurances_Small.Columns.Add("TXNSRC", typeof(string));//*
            TableActionOccurances_Small.Columns.Add("TXNDEST", typeof(string));//*
            TableActionOccurances_Small.Columns.Add("SeqNo", typeof(int));//*
            TableActionOccurances_Small.Columns.Add("RMCycle", typeof(string));//*
            TableActionOccurances_Small.Columns.Add("UserId", typeof(string));//*

            if (InMode == 25)
            {
                SqlString = " SELECT * "
                     + " FROM [ATMS].[dbo].[Actions_Occurances]"
                     + " WHERE UniqueKeyOrigin = 'Master_Pool' AND RMCycle = @RMCycle and Stage = '03' "
                      + " Order By AtmNo , ActionAtDateTime  ";
            }
            

            using (SqlConnection conn =
                           new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        //cmd.Parameters.AddWithValue("@ActionId", InActionId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

                            //FILL TABLE 
                            DataRow RowSelected = TableActionOccurances_Small.NewRow();


                            RowSelected["UniqueRecordId"] = UniqueKey;

                            RowSelected["ActionId"] = ActionId;
                            RowSelected["Occurance"] = Occurance.ToString();
                            RowSelected["ActionNm"] = ActionNm;
                            RowSelected["ActionReason"] = Maker_ReasonOfAction;
                            RowSelected["ActionDateTime"] = ActionAtDateTime;

                            RowSelected["Maker"] = Maker;
                            RowSelected["Author"] = Authoriser;

                            RowSelected["SeqNo"] = SeqNo;

                            RowSelected["RMCycle"] = RMCycle;

                            RowSelected["UserId"] = InSignedId;

                            RowSelected["OriginWorkFlow"] = OriginWorkFlow;
                            RowSelected["RMCateg"] = RMCateg;
                            RowSelected["ReplCycle"] = ReplCycle;

                            if (UniqueKeyOrigin == "Master_Pool")
                            {
                                // Read record 
                                Mpa.ReadInPoolTransSpecificUniqueRecordId(UniqueKey, 2);

                                RowSelected["Terminal"] = Mpa.TerminalId;
                                RowSelected["CardNo"] = Mpa.CardNumber;
                                RowSelected["AccNo"] = Mpa.AccNumber;
                                RowSelected["Amount"] = DoubleEntryAmt.ToString("#,##0.00");
                                RowSelected["TraceNo"] = Mpa.TraceNoWithNoEndZero.ToString();
                                RowSelected["RRNumber"] = Mpa.RRNumber;
                                RowSelected["TransDate"] = Mpa.TransDate;

                                RowSelected["MatchMask"] = Mpa.MatchMask;
                                RowSelected["Trans_Descr"] = Mpa.TransDescr;

                                RowSelected["MatchingCateg"] = Mpa.MatchingCateg;
                                RowSelected["MatchingCycle"] = Mpa.MatchingAtRMCycle;


                                RowSelected["TXNSRC"] = Mpa.TXNSRC;
                                RowSelected["TXNDEST"] = Mpa.TXNDEST;

                            }
                            else
                            {
                                MessageBox.Show("Handle this in class Actions_Occurances");
                                return;
                            }

                            // CREATE Unique record in 

                            // Description
                            // ADD ROW
                            TableActionOccurances_Small.Rows.Add(RowSelected);
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

            int WMode = 2;

           InsertReport(InSignedId, TableActionOccurances_Small);

        }

        // Insert 
        public void InsertReport(string InUserId, DataTable InTable)
        {
            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            //Clear Table 
            Tr.DeleteReport55_4(InUserId);

            if (InTable.Rows.Count > 0)
            {
                // RECORDS READ AND PROCESSED 

                using (SqlConnection conn =
                               new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        using (SqlBulkCopy s = new SqlBulkCopy(conn))
                        {
                            s.DestinationTableName = "[ATMS].[dbo].[WReport55_4]";

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

            // DELETE Dublicates from WReport55_4
            // This will be used in auditors report as a primary file 
            DeleteDuplicates_WReport55_4(InUserId);

        }

        //
        // DELETE Duplicates 
        //
        int Count;
        private void DeleteDuplicates_WReport55_4(string InUserId)
        {
            ErrorFound = false;
            ErrorOutput = "";
            Count = 0;

            string CMD_Details = " WITH cte AS( "
                      + " SELECT "
                      + "  SeqNo, "
                      //+ "  TerminalId, "
                      //+ "  TraceNoWithNoEndZero, "
                      //+ "   TransAmount, "
                      //+ "   TransDate, "
                      + "   UniqueRecordId, "
                      + "   ROW_NUMBER() OVER( "
                      + "       PARTITION BY "
                      //+ "       TerminalId, "
                      //+ "       TraceNoWithNoEndZero, "
                      //+ "       TransAmount, "
                      //+ "       TransDate, "
                      + "       UniqueRecordId "
                      + "   ORDER BY "
                      //+ "     TerminalId, "
                      //+ "     TraceNoWithNoEndZero, "
                      //+ "     TransAmount, "
                      //+ "     TransDate, "
                      + "    UniqueRecordId "
                      + "    ) row_num "
                      + " FROM [ATMS].[dbo].[WReport55_4] "
                      + " WHERE UserId = @UserId "
                      + " ) "
                      + " DELETE FROM cte "
                      + " WHERE row_num > 1 ";
            //string SqlCommand = "DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
            //                + " WHERE MatchingCateg = @MatchingCateg "; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {

                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(CMD_Details, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", InUserId);

                        //rows number of record got updated

                        Count = cmd.ExecuteNonQuery();
                        //  Count = cmd.ExecuteNonQueryAsync(); 
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
        // Create Actions 
        //
        string WCcy;
        decimal WDoubleEntryAmt;
        string WAtmNo;
        int WReplCycle;
        string W_Application;
        //string WOperator; 
        public void CreateActionsTxnsPerActionId(string InOperator, string InSignedId,
                             string InActionId, string InUniqueKeyOrigin, int InUniqueKey,
                              string InCcy, decimal InDoubleEntryAmt, string InAtmNo, int InReplCycle,
                              string InMaker_ReasonOfAction, string InOriginWorkFlow, string In_W_Application
                              )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            WOperator = InOperator; 
            WCcy = InCcy;
            WDoubleEntryAmt = InDoubleEntryAmt;
            WAtmNo = InAtmNo;
            WReplCycle = InReplCycle;
            W_Application = In_W_Application; 
            // Read Occurances 
            // (string InUniqueRecordIdOrigin, int InUniqueKey, string InActionId)
            ReadActionsOccurancesByUniqueKey(InUniqueKeyOrigin, InUniqueKey, InActionId);

            if (RecordFound == true)
            {
                // Delete all with Stage= "01" before you start
                DeleteActionsOccurancesUniqueKeyAndActionID(InUniqueKeyOrigin, InUniqueKey, InActionId);

            }
            // Create data table based on what txns created
            ClearTableTxnsTableFromAction();

            RRDMActions_GL Ag = new RRDMActions_GL();
            string SelectionCriteria = " WHERE ActionId ='" + InActionId + "'";
            Ag.ReadActionsAndFillTable(SelectionCriteria);

            int I = 0;
            // Occurances for this action 
            while (I <= (Ag.ActionsDataTable.Rows.Count - 1))
            {
                //    RecordFound = true;
                int WSeqNo = (int)Ag.ActionsDataTable.Rows[I]["SeqNo"];

                Ag.ReadActionBySeqNo(WSeqNo);

                bool WithInsert = false;

                CreateAndInsertInActionOccurances(InOperator, InSignedId,
                                           InUniqueKeyOrigin, InUniqueKey, InActionId, Ag.Occurance
                                           , InMaker_ReasonOfAction, InOriginWorkFlow);

                ReadActionsTxnsCreateTableByUniqueKey(InUniqueKeyOrigin, InUniqueKey, InActionId, Ag.Occurance, "Any", 1);

                I = I + 1;
            }

        }

        // Insert in occurances 
        public void CreateAndInsertInActionOccurances(string InOperator, string InSignedId,
                                 string InUniqueKeyOrigin, int InUniqueKey, string InActionId,
                                 int InOccurance, string InMaker_ReasonOfAction, string InOriginWorkFlow)
        {
            string WAccNo = "";
            string WAtmNo = "";
            string WCitId = "";
            string FirstAccName = "";
            string SecondAccName = "";
            string FirstAccNo = "";
            string SecondAccNo = "";
            string FirstBranch = "";
            string SecondBranch = "";
            string WOperator = "";
            string CategoryId = "";
            string WRMCateg = "";
            string UserBranch = "";
            string WAtmBranch = "";
            int WMatchingAtRMCycle = 0;
            bool IsT24_Version = false;
            string WT24Branch = "";

            int WSeqNo = InUniqueKey; 

            string connectionString = "";
            if (W_Application == "ETISALAT")
            {
                connectionString = ConfigurationManager.ConnectionStrings["ETISALATConnectionString"].ConnectionString;
            }
            if (W_Application == "QAHERA")
            {
                connectionString = ConfigurationManager.ConnectionStrings["QAHERAConnectionString"].ConnectionString;
            }

            RRDMUsersRecords Ua = new RRDMUsersRecords();
            Ua.ReadUsersRecord(InSignedId);

            UserBranch = Ua.Branch;

            RRDMAccountsClass Acc = new RRDMAccountsClass();
            // 
            // Find needed information from Master Pool 
            //
            //if (InUniqueKeyOrigin == "Master_Pool")
            //{
            //    RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

            //    string SelectionCriteria = " WHERE UniqueRecordId = " + InUniqueKey;
            //    Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

            //    WAtmNo = Mpa.TerminalId;

            //    // Find ATM Branch 
            //    Acc.ReadAccountsBasedOn_ShortAccID_EntityNo(InOperator, "30", WAtmNo);

            //    if (Acc.RecordFound == true)
            //    {
            //        WAtmBranch = Acc.BranchId;
            //    }
            //    else
            //    {
            //        WAtmBranch = "NotFound";
            //    }

            //    WOperator = Mpa.Operator;
            //    CategoryId = Mpa.MatchingCateg;

            //    WRMCateg = Mpa.RMCateg;
            //    WMatchingAtRMCycle = Mpa.MatchingAtRMCycle;

            //    WTerminalId = Mpa.TerminalId;
            //    WTraceNumber = Mpa.TraceNoWithNoEndZero;
            //    WRRNumber = Mpa.RRNumber;
            //    if (WRRNumber == "0" || WRRNumber == "") WRRNumber = WTraceNumber.ToString();

            //    WAccNo = Mpa.AccNumber;
            //    if (WAccNo == "") WAccNo = "AccNo Not Available";
            //    if (WAccNo.Length == 16 & WAccNo != "")
            //    {
            //        IsT24_Version = true;
            //        // WT24Branch 
            //        WT24Branch  = WAccNo.Substring(WAccNo.Length - 4);
            //    }
            //    else
            //    {
            //        IsT24_Version = false; 
            //    }
            //    WDate = Mpa.TransDate.Date;
            //    WTXNSRC = Mpa.TXNSRC;
            //    WTXNDEST = Mpa.TXNDEST;

            //    if (WDoubleEntryAmt == 0)
            //    {
            //        WCcy = Mpa.TransCurr;
            //        WDoubleEntryAmt = Mpa.TransAmount;
            //    }

            //    Ac.ReadAtm(WAtmNo);
            //    if (Ac.RecordFound == true)
            //    {
            //        if (Ac.CitId == "1000")
            //        {
            //            // Replenished By Bank
            //            WCitId = Ac.CitId;
            //        }
            //        else
            //        {
            //            // Replenished by name CIT 
            //            WCitId = Ac.CitId;
            //        }
            //    }

            //}

            // Find needed information for MOBILE 
            if (InUniqueKeyOrigin == "Master_Pool_MOBILE")
            {

                RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();
                string SelectionCriteria = " WHERE  SeqNo =" + WSeqNo;

                //[ETISALAT].[dbo].[ETISALAT_TPF_TXNS_MASTER]

                string MasterTableName = W_Application + ".[dbo].[ETISALAT_TPF_TXNS_MASTER]";

                Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, MasterTableName, 1, W_Application);

//                SELECT TOP(1000) [SeqNo]
//      ,[ActionId]
//      ,[Occurance]
//      ,[ActionNm]
//      ,[Is_GL_Action]
//      ,[GL_Sign_1]
//      ,[ShortAccID_1]
//      ,[AccName_1]
//      ,[Branch_1]
//      ,[AccNo_1]
//      ,[StatementDesc_1]
//      ,[GL_Sign_2]
//      ,[ShortAccID_2]
//      ,[AccName_2]
//      ,[Branch_2]
//      ,[AccNo_2]
//      ,[StatementDesc_2]
//      ,[Ccy]  -- YES..NEEDED
//      ,[DoubleEntryAmt] -- YES
//      ,[UniqueKeyOrigin]  -- YES 
//      ,[UniqueKey]  -- YES SeqNo not universal
//,[Maker] -- YES
//      ,[Authoriser] -- Blank
//      ,[AuthorizationKey]
//      ,[Maker_ReasonOfAction] -- From the REGISTERED SCREEN
//,[Stage] -- 01
//      ,[RMCateg] -- ETI375
//      ,[MatchingAtRMCycle] -- 278
//      ,[RMCycle] -- Current Cycle
//,[AtmNo] -- blank
//      ,[ReplCycle] -- zero
//      ,[ActionAtDateTime]
//      ,[Settled]
//      ,[OriginWorkFlow]
//      ,[Operator]
//      ,[LoadingExcelCycleNo]
//        FROM[ATMS].[dbo].[Actions_Occurances]

        //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        //string SelectionCriteria = " WHERE UniqueRecordId = " + InUniqueKey;
        //Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 2);

        WAtmNo = "";

                // Find ATM Branch 
                //Acc.ReadAccountsBasedOn_ShortAccID_EntityNo(InOperator, "30", WAtmNo);

                //if (Acc.RecordFound == true)
                //{
                //    WAtmBranch = Acc.BranchId;
                //}
                //else
                //{
                //    WAtmBranch = "NotFound";
                //}

                WAtmBranch = UserBranch; 

                WOperator = InOperator;
                CategoryId = Mmob.MatchingCateg;

                WRMCateg = Mmob.MatchingCateg;
                WMatchingAtRMCycle = Mmob.MatchingAtRMCycle;

                WTerminalId = "Nothing In Mobile";
                WTraceNumber = 0; // Nothing in Mobile
                WRRNumber = Mmob.RRNumber;
               
                WAccNo = "No Account in Mobile" ;
               
                WDate = Mmob.TransDate.Date;
                WTXNSRC = "No for Mobile";
                WTXNDEST = "No for Mobile";

                if (WDoubleEntryAmt == 0)
                {
                    WCcy = Mmob.TransCurr;
                    WDoubleEntryAmt = Mmob.TransAmount;
                }

                //Ac.ReadAtm(WAtmNo);
                //if (Ac.RecordFound == true)
                //{
                //    if (Ac.CitId == "1000")
                //    {
                //        // Replenished By Bank
                //        WCitId = Ac.CitId;
                //    }
                //    else
                //    {
                //        // Replenished by name CIT 
                //        WCitId = Ac.CitId;
                //    }
                //}

                // IF MOBILE 
               // WCitId = "1000"; 

            }

            //if (InUniqueKeyOrigin == "Replenishment")
            //{
            //    RRDMAtmsClass Ac = new RRDMAtmsClass();
            //    RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

            //    WAtmNo = Na.ReadNotesSesionsBySesNoToGetATMNo(InUniqueKey);
            //    Na.ReadSessionsNotesAndValues(WAtmNo, InUniqueKey, 2);
            //    WOperator = Na.Operator;

            //    Ac.ReadAtm(WAtmNo);
            //    if (Ac.RecordFound == true)
            //    {
            //        if (Ac.CitId == "1000")
            //        {
            //            // Replenished By Bank
            //            WCitId = Ac.CitId;
            //        }
            //        else
            //        {
            //            // Replenished by name CIT 
            //            WCitId = Ac.CitId;
            //        }
            //    }

            //    // Find ATM Branch 
            //    Acc.ReadAccountsBasedOn_ShortAccID_EntityNo(InOperator, "30", WAtmNo);

            //    if (Acc.RecordFound == true)
            //    {
            //        WAtmBranch = Acc.BranchId;
            //    }
            //    else
            //    {
            //        WAtmBranch = "NotFound";
            //    }

            //    //TransAmount = WDoubleEntryAmt; // take amount from action occurance 

            //    WTerminalId = WAtmNo;
            //    WTraceNumber = 0;
            //    WRRNumber = InUniqueKey.ToString();
            //    WAccNo = "";

            //    Ta.ReadSessionsStatusTraces(WAtmNo, InUniqueKey);
            //    WDate = Ta.SesDtTimeEnd.Date;
            //    WTXNSRC = "0";
            //    WTXNDEST = "0";

            //}

            // Find Current RM Cycle 
            // FIND CURRENT CUTOFF CYCLE
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            string WJobCategory = W_Application;

            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            // *****************************
            RRDMActions_GL Ag = new RRDMActions_GL();

            Ag.ReadActionByActionId(InOperator, InActionId, InOccurance);
            string WEntityNo = "";
            string WEntityNm = "";
            //
            // First Entry
            //
            if (Ag.Is_GL_Action == true)
            {
                //if (WCitId!= "1000")
                //{
                Acc.ReadAccountsBasedOn_ShortAccID(InOperator, Ag.ShortAccID_1);

                if (Acc.RecordFound == true)
                {
                    WEntityNm = Acc.EntityNm;
                    FirstAccName = Acc.AccName;
                }
                else
                {
                    FirstAccName = "Account not in data base";
                }
                //}
                //else
                //{
                //    // Case where the replenishment is done by branch User
                //    WEntityNm = "BranchId"; 
                //}


                // FOR FIRST ENTRY 
                switch (Acc.EntityNm)
                {
                    case "AtmNo":
                        {
                            WEntityNo = WAtmNo;
                            break;
                        }
                    case "CitId":
                        {
                            WEntityNo = WCitId;
                            break;
                        }

                    case "CategoryId":
                        {
                            WEntityNo = CategoryId;
                            break;
                        }
                    //case "CustAcc":
                    //    {
                    //        WEntityNo = WAtmNo;
                    //        break;
                    //    }

                    case "BranchId":
                        {
                            // User Branch
                            //Ua.ReadUsersRecord(InSignedId);
                            //WEntityNo = Ua.Branch;
                            break;
                        }
                    //case "UserId":
                    //    {
                    //        WEntityNo = WAtmNo;
                    //        break;
                    //    }

                    default:
                        {
                            WEntityNo = "";
                            break;
                        }
                }

                // FIRST : FIND ACCOUNT NUMBER  
                if (Ag.ShortAccID_1 == "20")
                {
                    if (IsT24_Version == true)
                    {
                        FirstBranch = WAccNo.Substring(WAccNo.Length - 4);
                    }
                    else
                    {
                        FirstBranch = WAccNo.Substring(0, 4); // For Flex is like this 
                                                              //For T24 we get the last four digits of the account number
                    }
                    
                    FirstAccNo = WAccNo;
                }
                else
                {
                    // If CIT then will be found 
                    Acc.ReadAccountsBasedOn_ShortAccID_EntityNo(InOperator, Ag.ShortAccID_1, WEntityNo);
                    if (Acc.RecordFound == true)
                    {
                        FirstAccNo = Acc.AccNo;
                        if (WCitId != "1000")
                        {
                            if (Acc.BranchId == "XXX" || Acc.BranchId == "XXXX")
                            {
                                FirstBranch = "001"; // ID // OR 0001 for T24? 
                                if (IsT24_Version == true)
                                {
                                    FirstBranch = "0001";
                                }
                            }
                            else
                            {
                                FirstBranch = Acc.BranchId; // ID // OK for Flex and T24 too
                            }

                        }
                        else
                        {
                            if (Acc.BranchId == "XXX" || Acc.BranchId == "XXXX")
                            {
                                // Get the branch from ATM Branch 
                                FirstBranch = WAtmBranch; // ID 
                            }
                            else
                            {
                                FirstBranch = Acc.BranchId; // ID 
                            }
                        }

                        if (Ag.ShortAccID_1 == "53")
                        {
                            FirstBranch = UserBranch;
                        }
                    }
                    else
                    {
                        if (WEntityNm == "Branch")
                        {

                        }
                        else
                        {
                            FirstAccNo = "Not Found";
                            FirstBranch = "Not Found"; // ID 
                        }

                    }

                }
                // Second Entry 
                Acc.ReadAccountsBasedOn_ShortAccID(InOperator, Ag.ShortAccID_2);
                if (Acc.RecordFound == true)
                {
                    SecondAccName = Acc.AccName;
                }
                else
                {
                    SecondAccName = "Account not in data base";
                }

                switch (Acc.EntityNm)
                {
                    case "AtmNo":
                        {
                            WEntityNo = WAtmNo; // Multi
                            break;
                        }
                    case "CitId":
                        {
                            WEntityNo = WCitId; // Multi
                            break;
                        }
                    //case "BranchId":
                    //    {
                    //        // User Branch
                    //        RRDMUsersRecords Ua = new RRDMUsersRecords();
                    //        Ua.ReadUsersRecord(InSignedId);
                    //        WEntityNo = Ua.Branch; // Multi
                    //        break;
                    //    }
                    //case "":
                    //    {
                    //        WEntityNo = "";
                    //        break;
                    //    }
                    case "CategoryId":
                        {
                            WEntityNo = CategoryId; // Multi
                            break;
                        }
                    //case "CustAcc":
                    //    {
                    //        WEntityNo = WAtmNo;
                    //        break;
                    //    }


                    //case "UserId":
                    //    {
                    //        WEntityNo = WAtmNo;
                    //        break;
                    //    }

                    default:
                        {
                            //MessageBox.Show("Not defined ");
                            WEntityNo = "";
                            break;
                        }
                }

                // SECOND: FIND ACCOUNT NUMBER AND BRANCH 
                if (Ag.ShortAccID_2 == "20")
                {
                    if (IsT24_Version == true)
                    {
                        SecondBranch = WAccNo.Substring(WAccNo.Length - 4);
                    }
                    else
                    {
                        SecondBranch = WAccNo.Substring(0, 4); // For Flex is like this 
                                                              //For T24 we get the last four digits of the account number
                    }
                    
                    SecondAccNo = WAccNo;
                }
                else
                {
                    Acc.ReadAccountsBasedOn_ShortAccID_EntityNo(InOperator, Ag.ShortAccID_2, WEntityNo);
                    if (Acc.RecordFound == true)
                    {
                        //SecondBranch = Acc.BranchId;

                        SecondAccNo = Acc.AccNo;

                        if (WCitId != "1000")
                        {
                            if (Acc.BranchId == "XXX" || Acc.BranchId == "XXXX")
                            {
                                SecondBranch = "001"; // ID 
                                if (IsT24_Version == true )
                                {
                                    SecondBranch = "0001"; // ID 
                                }
                            }
                            else
                            {
                                SecondBranch = Acc.BranchId; // ID 
                            }

                        }
                        else
                        {
                            if (Acc.BranchId == "XXX" || Acc.BranchId == "XXXX")
                            {
                                // Get the branch from ATM Branch 
                                SecondBranch = WAtmBranch; // ID 
                            }
                            else
                            {
                                SecondBranch = Acc.BranchId; // ID 
                            }
                        }
                        if (Ag.ShortAccID_2 == "53")
                        {
                            SecondBranch = UserBranch;
                        }
                    }
                    else
                    {
                        SecondBranch = "Not Found";
                        SecondAccNo = "Not Found";
                    }
                }
            }

            // PREPARE RECORD
            //RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            ActionId = Ag.ActionId;
            Occurance = Ag.Occurance;

            ActionNm = Ag.ActionNm;
            Is_GL_Action = Ag.Is_GL_Action;

            if (Ag.Is_GL_Action == true)
            {
                GL_Sign_1 = Ag.GL_Sign_1;
                ShortAccID_1 = Ag.ShortAccID_1;
                AccName_1 = FirstAccName;

                Branch_1 = FirstBranch;
                AccNo_1 = FirstAccNo;
                //StatementDesc_1 = Ag.StatementDesc_1;

                StatementDesc_1 = BuildEntryDescription(Ag.StatementDesc_1
                                                , WTerminalId
                                                , WTraceNumber
                                                , WRRNumber
                                                , WAccNo
                                                , WDate
                                                , WTXNSRC
                                                , WTXNDEST
                                                );

                GL_Sign_2 = Ag.GL_Sign_2;
                ShortAccID_2 = Ag.ShortAccID_2;
                AccName_2 = SecondAccName;

                Branch_2 = SecondBranch;
                AccNo_2 = SecondAccNo;

                //StatementDesc_2 = Ag.StatementDesc_2;

                StatementDesc_2 = BuildEntryDescription(Ag.StatementDesc_2
                                                , WTerminalId
                                                , WTraceNumber
                                                , WRRNumber
                                                , WAccNo
                                                , WDate
                                                , WTXNSRC
                                                , WTXNDEST
                                                );

                Ccy = WCcy;
                DoubleEntryAmt = WDoubleEntryAmt;
            }
            else
            {
                // NOT A GL ACTION 
                GL_Sign_1 = "N/A";
                ShortAccID_1 = "N/A";
                AccName_1 = "N/A";

                Branch_1 = "N/A";
                AccNo_1 = "N/A";
                //StatementDesc_1 = Ag.StatementDesc_1;

                StatementDesc_1 = "N/A";

                GL_Sign_2 = "N/A";
                ShortAccID_2 = "N/A";
                AccName_2 = "N/A";

                Branch_2 = "N/A";
                AccNo_2 = "N/A";

                StatementDesc_2 = "N/A";

                Ccy = WCcy;
                DoubleEntryAmt = WDoubleEntryAmt;
            }

            UniqueKeyOrigin = InUniqueKeyOrigin;
            UniqueKey = WSeqNo;

            Maker = InSignedId;

            Maker_ReasonOfAction = InMaker_ReasonOfAction;

            Stage = "01"; // Temporary mode
            RMCateg = WRMCateg;
            MatchingAtRMCycle = WMatchingAtRMCycle;
            RMCycle = WReconcCycleNo;

            AtmNo = WAtmNo;
            ReplCycle = WReplCycle;

            Settled = false;

            OriginWorkFlow = InOriginWorkFlow;

            Operator = InOperator;

            InsertActionOccurance();

        }
        //
        // Clear Table
        //
        public void ClearTableTxnsTableFromAction()
        {
            TxnsTableFromAction = new DataTable();
            TxnsTableFromAction.Clear();

            // DATA TABLE ROWS DEFINITION 
            TxnsTableFromAction.Columns.Add("SeqNo", typeof(int));
            TxnsTableFromAction.Columns.Add("ActionId", typeof(string));
            TxnsTableFromAction.Columns.Add("Occurance", typeof(string));
            TxnsTableFromAction.Columns.Add("ActionNm", typeof(string));
            TxnsTableFromAction.Columns.Add("Branch", typeof(string));
            TxnsTableFromAction.Columns.Add("AccNo", typeof(string));
            TxnsTableFromAction.Columns.Add("DR/CR", typeof(string));
            TxnsTableFromAction.Columns.Add("AccName", typeof(string));
            TxnsTableFromAction.Columns.Add("Amount", typeof(string));
            TxnsTableFromAction.Columns.Add("Description", typeof(string));
            TxnsTableFromAction.Columns.Add("Stage", typeof(string));
        }
        //
        // Create Data Table 
        //
        string WTerminalId;
        int WTraceNumber;
        string WRRNumber;
        string WAccNo;
        DateTime WDate;
        string WTXNSRC;
        string WTXNDEST;
        int TransType_1;
        int TransType_2;
        string Description_1;
        string Description_2;
        int DisputeNo;
        int DisputeTranNo;
        bool GL_1;
        bool GL_2;
        //string WAtmNo;
        //string WCitId = "";
        //decimal TransAmount = 0;
        int WSesNo;

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();
        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();
        RRDMAtmsClass Ac = new RRDMAtmsClass();

        public void ReadActionsTxnsCreateTableByUniqueKey(string InUniqueKeyOrigin,
                                                int InUniqueKey, string InActionId, int InOccurance,
                                                string InCallerProcess, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            //
            // if InMode = 2 then create txn in Transaction to be posted 
            //

            string WCitId = "";
            decimal TransAmount = 0;
            string SqlString = "";
            string WOperator = "";

            //if (InUniqueKeyOrigin == "Master_Pool")
            //{
            //    int WSeqNo = InUniqueKey; 
            //    RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();
            //    string SelectionCriteria = " WHERE  SeqNo =" + WSeqNo;

            //    //[ETISALAT].[dbo].[ETISALAT_TPF_TXNS_MASTER]

            //    string MasterTableName = W_Application + ".[dbo].[ETISALAT_TPF_TXNS_MASTER]";

            //    Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, MasterTableName, 2, W_Application);

            //    //string SelectionCriteria = " WHERE UniqueRecordId =" + InUniqueKey;
            //    //Mpa.ReadInPoolTransSpecificBySelectionCriteria(SelectionCriteria, 2);
            //    //TransAmount = WDoubleEntryAmt; // take amount from action occurance 

            //    //WTerminalId = Mpa.TerminalId;
            //    WTraceNumber = Mpa.TraceNoWithNoEndZero;
            //    WRRNumber = Mpa.RRNumber;
            //    WAccNo = Mpa.AccNumber;
            //    WDate = Mpa.TransDate.Date;
            //    WTXNSRC = Mpa.TXNSRC;
            //    WTXNDEST = Mpa.TXNDEST;
            //    WOperator = Mpa.Operator;
            //    // SET UP SQL STRING 
            //    SqlString = "SELECT * "
            //           + " FROM [ATMS].[dbo].[Actions_Occurances]"
            //           + " WHERE UniqueKey = @UniqueKey AND UniqueKeyOrigin = @UniqueKeyOrigin "
            //           + " AND ActionId = @ActionId AND Occurance = @Occurance "
            //            + " ORDER By Occurance ";
            //    if (InActionId == "All")
            //    {
            //        SqlString = "SELECT * "
            //           + " FROM [ATMS].[dbo].[Actions_Occurances]"
            //           + " WHERE UniqueKey = @UniqueKey AND UniqueKeyOrigin = @UniqueKeyOrigin "
            //            // + " AND ActionId = @ActionId AND Occurance = @Occurance "
            //            + " ORDER By Occurance ";
            //    }
            //}

            if (InUniqueKeyOrigin == "Master_Pool_MOBILE")
            {

                // READ FROM MOBILE
                int WSeqNo = InUniqueKey;
                RRDMMatchingTxns_InGeneralTables_MOBILE Mmob = new RRDMMatchingTxns_InGeneralTables_MOBILE();
                string SelectionCriteria = " WHERE  SeqNo =" + WSeqNo;

                //[ETISALAT].[dbo].[ETISALAT_TPF_TXNS_MASTER]

                string MasterTableName = W_Application + ".[dbo].[ETISALAT_TPF_TXNS_MASTER]";

                Mmob.ReadTransSpecificFromSpecificTable_MOBILE(SelectionCriteria, MasterTableName, 2, W_Application);

                //WTerminalId = Mmob.TerminalId;
                //WTraceNumber = 0;
                WRRNumber = Mmob.RRNumber;
                //WAccNo = "NOT AVAILABLE" ;
                WDate = Mmob.TransDate.Date;
                //WTXNSRC = Mmob.TXNSRC;
                //WTXNDEST = Mmob.TXNDEST;
                WOperator = Mmob.Operator;

                WAtmNo = "No ATM";

                // SET UP SQL STRING 
                SqlString = "SELECT * "
                       + " FROM [ATMS].[dbo].[Actions_Occurances]"
                       + " WHERE UniqueKey = @UniqueKey AND UniqueKeyOrigin = @UniqueKeyOrigin "
                       + " AND ActionId = @ActionId AND Occurance = @Occurance "
                        + " ORDER By Occurance ";
                if (InActionId == "All")
                {
                    SqlString = "SELECT * "
                       + " FROM [ATMS].[dbo].[Actions_Occurances]"
                       + " WHERE UniqueKey = @UniqueKey AND UniqueKeyOrigin = @UniqueKeyOrigin "
                        // + " AND ActionId = @ActionId AND Occurance = @Occurance "
                        + " ORDER By Occurance ";
                }
            }
            //*****************************
            //if (InUniqueKeyOrigin == "Replenishment")
            //{

            //    WAtmNo = Na.ReadNotesSesionsBySesNoToGetATMNo(InUniqueKey);
            //    Na.ReadSessionsNotesAndValues(WAtmNo, InUniqueKey, 2);
            //    WSesNo = Na.SesNo;
            //    Ac.ReadAtm(WAtmNo);
            //    if (Ac.RecordFound == true)
            //    {
            //        if (Ac.CitId == "1000")
            //        {
            //            // Replenished By Bank
            //        }
            //        else
            //        {
            //            // Replenished by name CIT 
            //            WCitId = Ac.CitId;
            //        }
            //    }

            //    //TransAmount = WDoubleEntryAmt; // take amount from action occurance 

            //    WTerminalId = WAtmNo;
            //    WTraceNumber = 0;
            //    WRRNumber = InUniqueKey.ToString();
            //    WAccNo = "";

            //    Ta.ReadSessionsStatusTraces(WAtmNo, InUniqueKey);
            //    WDate = Ta.SesDtTimeEnd.Date;
            //    WTXNSRC = "0";
            //    WTXNDEST = "0";
            //    WOperator = Ta.Operator;

            //    SqlString = "SELECT * "
            //           + " FROM [ATMS].[dbo].[Actions_Occurances]"
            //           + " WHERE UniqueKey = @UniqueKey  AND UniqueKeyOrigin = @UniqueKeyOrigin "
            //           + " AND ActionId = @ActionId AND Occurance = @Occurance"
            //            + " ORDER By Occurance ";
            //}


            // Find Current RM Cycle number 
            string WJobCategory = W_Application;
            RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
            WOperator = "BCAIEGCX";
            int WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);


            //TxnsTableFromAction = new DataTable();
            //TxnsTableFromAction.Clear();



            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                        cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);
                        cmd.Parameters.AddWithValue("@Occurance", InOccurance);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

                            if (Is_GL_Action == true)
                            {
                                //FILL TABLE 
                                DataRow RowSelected_1 = TxnsTableFromAction.NewRow();

                                RowSelected_1["SeqNo"] = SeqNo;
                                RowSelected_1["ActionId"] = ActionId;
                                RowSelected_1["Occurance"] = Occurance.ToString();
                                RowSelected_1["ActionNm"] = ActionNm;
                                RowSelected_1["Branch"] = Branch_1;
                                RowSelected_1["AccNo"] = AccNo_1;
                                RowSelected_1["AccName"] = AccName_1;
                                RowSelected_1["Amount"] = DoubleEntryAmt.ToString("#,##0.00");
                                RowSelected_1["DR/CR"] = GL_Sign_1;
                                RowSelected_1["Description"] = Description_1 =//StatementDesc_1;
                                     BuildEntryDescription(StatementDesc_1
                                                 , WTerminalId
                                                 , WTraceNumber
                                                 , WRRNumber
                                                 , WAccNo
                                                 , WDate
                                                 , WTXNSRC
                                                 , WTXNDEST
                                                 );
                                RowSelected_1["Stage"] = Stage;
                                // Description
                                // ADD ROW
                                TxnsTableFromAction.Rows.Add(RowSelected_1);

                                DataRow RowSelected_2 = TxnsTableFromAction.NewRow();

                                RowSelected_2["SeqNo"] = SeqNo;
                                RowSelected_2["ActionId"] = ActionId;
                                RowSelected_2["Occurance"] = Occurance.ToString();
                                RowSelected_2["Branch"] = Branch_2;
                                RowSelected_2["AccNo"] = AccNo_2;
                                RowSelected_2["AccName"] = AccName_2;
                                RowSelected_2["Amount"] = DoubleEntryAmt.ToString("#,##0.00");
                                RowSelected_2["DR/CR"] = GL_Sign_2;
                                RowSelected_2["Description"] = Description_2 = //StatementDesc_2;
                                    BuildEntryDescription(StatementDesc_2
                                                 , WTerminalId
                                                 , WTraceNumber
                                                 , WRRNumber
                                                 , WAccNo
                                                 , WDate
                                                 , WTXNSRC
                                                 , WTXNDEST
                                                 );
                                RowSelected_2["Stage"] = Stage;
                                // ADD ROW
                                TxnsTableFromAction.Rows.Add(RowSelected_2);

                            }


                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    if (InMode == 2)
                    {
                        // Find values for Insert in pool transaction 
                        if (GL_Sign_1 == "DR")
                            TransType_1 = 11;
                        else TransType_1 = 21;
                        if (GL_Sign_2 == "DR")
                            TransType_2 = 11;
                        else TransType_2 = 21;

                        RRDMDisputeTransactionsClass Dt = new RRDMDisputeTransactionsClass();
                        Dt.ReadDisputeTranByUniqueRecordId(Mpa.UniqueRecordId);
                        if (Dt.RecordFound == true)
                        {

                            DisputeNo = Dt.DisputeNumber;
                            DisputeTranNo = Dt.DispTranNo;
                        }
                        else
                        {
                            DisputeNo = 0;
                            DisputeTranNo = 0;
                        }
                        if (ShortAccID_1 == "20")
                        {
                            // Customer account number 
                            GL_1 = false;
                        }
                        else
                        {
                            GL_1 = true;
                        }
                        if (ShortAccID_2 == "20")
                        {
                            // Customer account number 
                            GL_2 = false;
                        }
                        else
                        {
                            GL_2 = true;
                        }
                        //
                        // CREATE RECORD FOR MASTER POOL 
                        //
                        if (InUniqueKeyOrigin == "Master_Pool")
                        {
                            RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass
                            {
                                OriginId = "00",
                                OriginName = InCallerProcess,

                                RMCateg = Mpa.RMCateg,
                                RMCategCycle = Mpa.MatchingAtRMCycle,
                                ActionSeqNo = SeqNo,
                                UniqueRecordId = Mpa.UniqueRecordId,
                                ErrNo = Mpa.MetaExceptionId,
                                AtmNo = Mpa.TerminalId,
                                SesNo = Mpa.ReplCycleNo,
                                RMCycleNo = WReconcCycleNo,
                                DisputeNo = DisputeNo,
                                DispTranNo = DisputeTranNo,
                                BankId = Mpa.Operator,
                                AtmTraceNo = Mpa.TraceNoWithNoEndZero,
                                RRNumber = Mpa.RRNumber,
                                TXNSRC = Mpa.TXNSRC,
                                TXNDEST = Mpa.TXNDEST,
                                BranchId = Branch_1,
                                AtmDtTime = Mpa.TransDate,
                                SystemTarget = 0,
                                CardNo = Mpa.Card_Encrypted,
                                CardOrigin = 0,
                                BranchId1 = Branch_1,
                                TransType = TransType_1,
                                TransDesc = Description_1,
                                AccNo = AccNo_1,
                                GlEntry = GL_1,

                                BranchId2 = Branch_2,
                                TransType2 = TransType_2,
                                TransDesc2 = Description_2,
                                AccNo2 = AccNo_2,
                                GlEntry2 = GL_2,
                                CurrDesc = Ccy,
                                TranAmount = DoubleEntryAmt,
                                //RefNumb = 0, // NOOOO
                                //RemNo = 0,  // NOOO
                                TransMsg = Mpa.Comments,
                                AtmMsg = "",

                                MakerUser = Mpa.UserId,
                                AuthUser = Authoriser,

                                OpenDate = DateTime.Now,
                                OpenRecord = true,
                                Operator = Mpa.Operator
                            };

                            Tp.InsertTransToBePosted_BDC(SeqNo);
                        }

                        //
                        // CREATE RECORD FOR == "Replenishment")
                        //
                        if (InUniqueKeyOrigin == "Replenishment")
                        {
                            RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass
                            {
                                OriginId = "00",
                                OriginName = InCallerProcess,

                                RMCateg = "",
                                RMCategCycle = RMCycle,
                                ActionSeqNo = SeqNo,
                                UniqueRecordId = WSesNo,
                                ErrNo = 0,
                                AtmNo = WAtmNo,
                                SesNo = WSesNo,
                                RMCycleNo = WReconcCycleNo,
                                DisputeNo = 0,
                                DispTranNo = 0,
                                BankId = Ta.Operator,

                                AtmTraceNo = WTraceNumber,
                                RRNumber = WRRNumber,
                                TXNSRC = WTXNSRC,
                                TXNDEST = WTXNDEST,
                                BranchId = Branch_1,
                                AtmDtTime = WDate,
                                SystemTarget = 0,
                                CardNo = "",
                                CardOrigin = 0,
                                BranchId1 = Branch_1,
                                TransType = TransType_1,
                                TransDesc = Description_1,
                                AccNo = AccNo_1,
                                GlEntry = GL_1,

                                BranchId2 = Branch_2,
                                TransType2 = TransType_2,
                                TransDesc2 = Description_2,
                                AccNo2 = AccNo_2,
                                GlEntry2 = GL_2,
                                CurrDesc = Na.Balances1.CurrNm,
                                TranAmount = DoubleEntryAmt,

                                TransMsg = "This is a replenishment txn",
                                AtmMsg = "",
                                MakerUser = Maker,
                                AuthUser = Authoriser,
                                OpenDate = DateTime.Now,
                                OpenRecord = true,
                                Operator = Ta.Operator
                            };

                            Tp.InsertTransToBePosted_BDC(SeqNo);
                        }

                    }

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
        }
        private string BuildEntryDescription(string InStatementDesc
                                             , string InTerminalId
                                             , int InTraceNumber
                                             , string InRRNumber
                                             , string InAccNo
                                             , DateTime InDate
                                             , string InTXNSRC
                                             , string InTXNDEST
                                             )
        {
            string WDescription = "N/A";

            string ShortDesc = InStatementDesc.Substring(0, 2);
            string ShortTermId = WTerminalId.Substring(3, 5);


            // FOR FIRST ENTRY 
            switch (ShortDesc)
            {
                case "01":
                    {
                        //01_Reversal,Trace 155688,date 03-10-2019_ATM_0567
                        WDescription = "01_Reversal,Trace " + InTraceNumber.ToString()
                                      + ",date " + InDate.ToShortDateString() + "_" + ShortTermId;
                        break;
                    }
                case "02":
                    {
                        //02_REPLENISHMENT 00000071 13-10-2019
                        WDescription = "02_REPLENISHMENT ATM " + InTerminalId
                                     + " ,date " + InDate.ToShortDateString();
                        break;
                    }
                case "03":
                    {
                        //03_REMAINING 00000071 13-10-2019
                        WDescription = "03_REMAINING ATM " + InTerminalId
                                    + " ,date " + InDate.ToShortDateString();
                        break;
                    }
                case "04":
                    {
                        // 04_EXCESS 000000900 10-10-2019
                        WDescription = "04_EXCESS ATM " + InTerminalId
                                    + " ,date " + InDate.ToShortDateString();
                        break;
                    }
                case "05":
                    {
                        // 05_SHORTAGE 000000900 10-10-2019
                        WDescription = "05_SHORTAGE ATM " + InTerminalId
                                   + " ,date " + InDate.ToShortDateString();
                        break;
                    }
                case "06":
                    {
                        // 06_Reversal_Cust_Acc_xxxxxxxxxxx 13 - 10 - 2019
                        WDescription = "06_Reversal_For_Cust_Acc_" + InAccNo
                                    + " ,date " + InDate.ToShortDateString();
                        break;
                    }
                case "07":
                    {
                        // 07_Reversal_Cust_Reference_xxxxxxxxxxx 13-10-2019 - Dstn 5
                        WDescription = "07_Reversal_Cust_Reference_" + InRRNumber
                                    + " ,date " + InDate.ToString() + " DSTN" + InTXNDEST;
                        break;
                    }
                case "08":
                    {
                        //08_Reversal Reference xxxxxxxx_ 13-10-2019 
                        WDescription = "08_Reversal Reference-" + InRRNumber
                                   + " ,date " + InDate.ToShortDateString();
                        break;
                    }
                case "09":
                    {
                        //09_Reversal Reference xxxxxxxx_ 13-10-2019 to Cat_ 
                        WDescription = "09_Reversal Reference-" + InRRNumber
                                   + " ,date " + InDate.ToShortDateString() + " To Cat_" + RMCateg;
                        break;
                    }
                case "10":
                    {
                        // 10_Reversal Reference xxxxxxxx_ 13-10-2019 From Cat_ 
                        WDescription = "10_Reversal Reference-" + InRRNumber
                                  + " ,date " + InDate.ToShortDateString() + " From_Cat_" + RMCateg;
                        break;
                    }
                case "11":
                    {
                        // 11_DEPOSITS Counted 00000071 13-10-2019 
                        WDescription = "11_DEPOSITS Counted-ATM-" + InTerminalId
                                  + " ,date " + InDate.ToShortDateString();
                        break;
                    }
                case "12":
                    {
                        //12_EXCESS 000000900 10-10- 2019(DEPOSITS)
                        WDescription = "12_EXCESS ATM " + InTerminalId
                                    + " ,date " + InDate.ToShortDateString() + "(DEPOSITS)";
                        break;
                    }
                case "13":
                    {
                        //13_SHORTAGE 000000900 10 - 10 - 2019(DEPOSITS)
                        WDescription = "13_SHORTAGE ATM " + InTerminalId
                                   + " ,date " + InDate.ToShortDateString() + "(DEPOSITS)";
                        break;
                    }
                case "14":
                    {
                        //01_Reversal,Trace 155688,date 03-10-2019_ATM_0567
                        WDescription = "14_Reversal Missing,Trace " + InTraceNumber.ToString()
                                      + ",date " + InDate.ToShortDateString() + "_" + ShortTermId;
                        break;
                    }
                case "15":
                    {
                        //01_Reversal,Trace 155688,date 03-10-2019_ATM_0567
                        WDescription = "15_Reversal bills,Trace " + InTraceNumber.ToString()
                                      + ",date " + InDate.ToShortDateString() + "_" + ShortTermId;
                        break;
                    }
                default:
                    {
                        //MessageBox.Show("Not defined ");
                        WDescription = "Not Found Definition";

                        break;
                    }
            }



            return WDescription;
        }
        //
        // Read By SeqNo
        //
        public void ReadActionsOccuarnceBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
                       + " FROM [ATMS].[dbo].[Actions_Occurances]"
                       + " WHERE SeqNo = @SeqNo "
                        + " ";

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

                            ReadActionOccuranceFields(rdr);

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
        // Read By Selection criteria
        //
        public void ReadCheckActionsOccuarnceBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
                       + " FROM [ATMS].[dbo].[Actions_Occurances] "
                       + InSelectionCriteria
                        + " ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       // cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

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
        // Read By UniqueKey
        //
        public void ReadActionsOccurancesByUniqueKey(string InUniqueKeyOrigin, int InUniqueKey, string InActionId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            string SqlString = "";
            //Replenishment
            if (InUniqueKeyOrigin == "Master_Pool")
            {
                SqlString = "SELECT * "
                                       + " FROM [ATMS].[dbo].[Actions_Occurances]"
                                       + " WHERE UniqueKeyOrigin = @UniqueKeyOrigin AND UniqueKey = @UniqueKey "
                                        + " ORDER By Occurance ";
            }
            if (InUniqueKeyOrigin == "Master_Pool_MOBILE")
            {
                SqlString = "SELECT * "
                                       + " FROM [ATMS].[dbo].[Actions_Occurances]"
                                       + " WHERE UniqueKeyOrigin = @UniqueKeyOrigin AND UniqueKey = @UniqueKey "
                                        + " ORDER By Occurance ";
            }
            if (InUniqueKeyOrigin == "Replenishment")
            {
                SqlString = "SELECT * "
                                       + " FROM [ATMS].[dbo].[Actions_Occurances]"
                                       + " WHERE  UniqueKeyOrigin = @UniqueKeyOrigin AND UniqueKey = @UniqueKey AND ActionId = @ActionId "
                                        + "  ";
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);
                        cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                        cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

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
        // Read By UniqueKey and Mode
        //
        public void ReadActionsOccurancesByUniqueKey_By_Mode(string InUniqueKeyOrigin, int InUniqueKey
                                                                              , string InActionId, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            string SqlString = "";

            string WSelection = "";

            // InMode = 1 find if a dispute was opened for this usnique Id 
            if (InMode == 1)
            {
                WSelection = " WHERE UniqueKeyOrigin ='" + InUniqueKeyOrigin + "' AND UniqueKey =" + InUniqueKey
               + " AND ActionId ='" + InActionId + "' ";
            }

            // If Open check if it is outstanding

            //Replenishment
            if (InUniqueKeyOrigin == "Master_Pool")
            {
                SqlString = "SELECT * "
                                       + " FROM [ATMS].[dbo].[Actions_Occurances]"
                                       + WSelection
                                        + " ORDER By Occurance ";
            }
            if (InUniqueKeyOrigin == "Replenishment")
            {
                SqlString = "SELECT * "
                                       + " FROM [ATMS].[dbo].[Actions_Occurances]"
                                       + " WHERE UniqueKey = @UniqueKey AND ActionId = @ActionId "
                                        + "  ";
            }


            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);
                        cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                        cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReadActionOccuranceFields(rdr);

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
        // READ Action Occurances 
        //
        public void ReadReadActionOccuranceByActionId(string InOperator,
                                                         string InActionId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
                         + " FROM [ATMS].[dbo].[Actions_Occurances]"
                         + " WHERE Operator = @Operator AND ActionId = @ActionId "
                          + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadActionOccuranceFields(rdr);

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
        // READ Action Occurances for Maker Or Authorisers
        //
        public void ReadReadActionOccuranceByMakerORAuthor(string InMaker)

        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
                         + " FROM [ATMS].[dbo].[Actions_Occurances]"
                         + " WHERE (Maker = @Maker OR Authoriser = @Authoriser )"
                          + "  ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Maker", InMaker);
                        cmd.Parameters.AddWithValue("@Authoriser", InMaker);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                            // Read Fields
                            ReadActionOccuranceFields(rdr);

                            break;

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

        // Insert ActionOccurance
        //
        public void InsertActionOccurance()
        {

            ErrorFound = false;
            ErrorOutput = "";
            // MatchingAtRMCycle

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[Actions_Occurances] "
          + "           ( "
          + "  [ActionId] "
          + "  ,[Occurance] "
          + "  ,[ActionNm] "
          + "  ,[Is_GL_Action] "
          + "  ,[GL_Sign_1] "
          + "  ,[ShortAccID_1] "
          + "  ,[AccName_1] "
          + "  ,[Branch_1] "
          + "  ,[AccNo_1] "
          + "  ,[StatementDesc_1] "
          + "  ,[GL_Sign_2] "
          + "  ,[ShortAccID_2] "
          + "  ,[AccName_2] "
          + "  ,[Branch_2] "
          + "  ,[AccNo_2] "
          + "  ,[StatementDesc_2] "
          + "  ,[Ccy] "
          + "  ,[DoubleEntryAmt] "
          + "  ,[UniqueKeyOrigin] "
          + "  ,[UniqueKey] "
          + "  ,[Maker] "
          + "  ,[Maker_ReasonOfAction] "
          + "  ,[Stage] "
           + "  ,[RMCateg] "
             + "  ,[MatchingAtRMCycle] "
          + "  ,[RMCycle] "
            + "  ,[AtmNo] "
              + "  ,[ReplCycle] "
          + "  ,[ActionAtDateTime] "
          + "  ,[Settled] "
           + "  ,[OriginWorkFlow] "
          + "  ,[Operator]) "
                    + " VALUES "
                + "    (  "
           + "  @ActionId "
           + " , @Occurance "
           + " , @ActionNm "
           + " , @Is_GL_Action "
           + " , @GL_Sign_1 "
           + " , @ShortAccID_1 "
           + " , @AccName_1 "
           + " , @Branch_1 "
           + " , @AccNo_1 "
           + " , @StatementDesc_1 "
           + " , @GL_Sign_2 "
           + " , @ShortAccID_2 "
           + " , @AccName_2 "
           + " , @Branch_2 "
           + " , @AccNo_2 "
           + " , @StatementDesc_2 "
           + " , @Ccy "
           + " , @DoubleEntryAmt "
           + " , @UniqueKeyOrigin "
           + " , @UniqueKey "
           + " , @Maker "
           + " , @Maker_ReasonOfAction "
           + " , @Stage "
           + " , @RMCateg "
           + " , @MatchingAtRMCycle "
            + " , @RMCycle "
            + " , @AtmNo "
             + " , @ReplCycle "
           + " , @ActionAtDateTime "
           + " , @Settled "
           + " , @OriginWorkFlow "
           + " , @Operator) ";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {


                        cmd.Parameters.AddWithValue("@ActionId", ActionId);
                        cmd.Parameters.AddWithValue("@Occurance", Occurance);
                        cmd.Parameters.AddWithValue("@ActionNm", ActionNm);

                        cmd.Parameters.AddWithValue("@Is_GL_Action", Is_GL_Action);
                        cmd.Parameters.AddWithValue("@GL_Sign_1", GL_Sign_1);
                        cmd.Parameters.AddWithValue("@ShortAccID_1", ShortAccID_1);
                        cmd.Parameters.AddWithValue("@AccName_1", AccName_1);
                        cmd.Parameters.AddWithValue("@Branch_1", Branch_1);
                        cmd.Parameters.AddWithValue("@AccNo_1", AccNo_1);
                        cmd.Parameters.AddWithValue("@StatementDesc_1", StatementDesc_1);

                        cmd.Parameters.AddWithValue("@GL_Sign_2", GL_Sign_2);
                        cmd.Parameters.AddWithValue("@ShortAccID_2", ShortAccID_2);
                        cmd.Parameters.AddWithValue("@AccName_2", AccName_2);
                        cmd.Parameters.AddWithValue("@Branch_2", Branch_2);
                        cmd.Parameters.AddWithValue("@AccNo_2", AccNo_2);
                        cmd.Parameters.AddWithValue("@StatementDesc_2", StatementDesc_2);

                        cmd.Parameters.AddWithValue("@Ccy", Ccy);
                        cmd.Parameters.AddWithValue("@DoubleEntryAmt", DoubleEntryAmt);

                        cmd.Parameters.AddWithValue("@UniqueKeyOrigin", UniqueKeyOrigin);
                        cmd.Parameters.AddWithValue("@UniqueKey", UniqueKey);
                        cmd.Parameters.AddWithValue("@Maker", Maker);
                        cmd.Parameters.AddWithValue("@Maker_ReasonOfAction", Maker_ReasonOfAction);
                        cmd.Parameters.AddWithValue("@Stage", Stage);

                        cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
                        cmd.Parameters.AddWithValue("@MatchingAtRMCycle", MatchingAtRMCycle);
                        cmd.Parameters.AddWithValue("@RMCycle", RMCycle);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", ReplCycle);
                        cmd.Parameters.AddWithValue("@ActionAtDateTime", DateTime.Now);

                        cmd.Parameters.AddWithValue("@Settled", Settled);

                        cmd.Parameters.AddWithValue("@OriginWorkFlow", OriginWorkFlow);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

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
        // Insert Force Matching
        //public void InsertActionOccuranceForceMatching(SqlConnection conn)
        //{

        //    ErrorFound = false;
        //    ErrorOutput = "";
        //    // MatchingAtRMCycle

        //    string cmdinsert = "INSERT INTO [ATMS].[dbo].[Actions_Occurances] "
        //  + "           ( "
        //  + "  [ActionId] "
        //  + "  ,[Occurance] "
        //  + "  ,[ActionNm] "
        //  + "  ,[Is_GL_Action] "
        //  + "  ,[GL_Sign_1] "
        //  + "  ,[ShortAccID_1] "
        //  + "  ,[AccName_1] "
        //  + "  ,[Branch_1] "
        //  + "  ,[AccNo_1] "
        //  + "  ,[StatementDesc_1] "
        //  + "  ,[GL_Sign_2] "
        //  + "  ,[ShortAccID_2] "
        //  + "  ,[AccName_2] "
        //  + "  ,[Branch_2] "
        //  + "  ,[AccNo_2] "
        //  + "  ,[StatementDesc_2] "
        //  + "  ,[Ccy] "
        //  + "  ,[DoubleEntryAmt] "
        //  + "  ,[UniqueKeyOrigin] "
        //  + "  ,[UniqueKey] "
        //  + "  ,[Maker] "
        //  + "  ,[Maker_ReasonOfAction] "
        //  + "  ,[Stage] "
        //   + "  ,[RMCateg] "
        //     + "  ,[MatchingAtRMCycle] "
        //  + "  ,[RMCycle] "
        //    + "  ,[AtmNo] "
        //      + "  ,[ReplCycle] "
        //  + "  ,[ActionAtDateTime] "
        //  + "  ,[Settled] "
        //   + "  ,[OriginWorkFlow] "
        //  + "  ,[Operator]) "
        //            + " VALUES "
        //        + "    (  "
        //   + "  @ActionId "
        //   + " , @Occurance "
        //   + " , @ActionNm "
        //   + " , @Is_GL_Action "
        //   + " , @GL_Sign_1 "
        //   + " , @ShortAccID_1 "
        //   + " , @AccName_1 "
        //   + " , @Branch_1 "
        //   + " , @AccNo_1 "
        //   + " , @StatementDesc_1 "
        //   + " , @GL_Sign_2 "
        //   + " , @ShortAccID_2 "
        //   + " , @AccName_2 "
        //   + " , @Branch_2 "
        //   + " , @AccNo_2 "
        //   + " , @StatementDesc_2 "
        //   + " , @Ccy "
        //   + " , @DoubleEntryAmt "
        //   + " , @UniqueKeyOrigin "
        //   + " , @UniqueKey "
        //   + " , @Maker "
        //   + " , @Maker_ReasonOfAction "
        //   + " , @Stage "
        //   + " , @RMCateg "
        //   + " , @MatchingAtRMCycle "
        //    + " , @RMCycle "
        //    + " , @AtmNo "
        //     + " , @ReplCycle "
        //   + " , @ActionAtDateTime "
        //   + " , @Settled "
        //   + " , @OriginWorkFlow "
        //   + " , @Operator) ";

        //    //using (SqlConnection conn =
        //    //    new SqlConnection(connectionString))
        //    try
        //    {
        //        //conn.Open();
        //        using (SqlCommand cmd =

        //           new SqlCommand(cmdinsert, conn))
        //        {


        //            cmd.Parameters.AddWithValue("@ActionId", ActionId);
        //            cmd.Parameters.AddWithValue("@Occurance", Occurance);
        //            cmd.Parameters.AddWithValue("@ActionNm", ActionNm);

        //            cmd.Parameters.AddWithValue("@Is_GL_Action", Is_GL_Action);
        //            cmd.Parameters.AddWithValue("@GL_Sign_1", GL_Sign_1);
        //            cmd.Parameters.AddWithValue("@ShortAccID_1", ShortAccID_1);
        //            cmd.Parameters.AddWithValue("@AccName_1", AccName_1);
        //            cmd.Parameters.AddWithValue("@Branch_1", Branch_1);
        //            cmd.Parameters.AddWithValue("@AccNo_1", AccNo_1);
        //            cmd.Parameters.AddWithValue("@StatementDesc_1", StatementDesc_1);

        //            cmd.Parameters.AddWithValue("@GL_Sign_2", GL_Sign_2);
        //            cmd.Parameters.AddWithValue("@ShortAccID_2", ShortAccID_2);
        //            cmd.Parameters.AddWithValue("@AccName_2", AccName_2);
        //            cmd.Parameters.AddWithValue("@Branch_2", Branch_2);
        //            cmd.Parameters.AddWithValue("@AccNo_2", AccNo_2);
        //            cmd.Parameters.AddWithValue("@StatementDesc_2", StatementDesc_2);

        //            cmd.Parameters.AddWithValue("@Ccy", Ccy);
        //            cmd.Parameters.AddWithValue("@DoubleEntryAmt", DoubleEntryAmt);

        //            cmd.Parameters.AddWithValue("@UniqueKeyOrigin", UniqueKeyOrigin);
        //            cmd.Parameters.AddWithValue("@UniqueKey", UniqueKey);
        //            cmd.Parameters.AddWithValue("@Maker", Maker);
        //            cmd.Parameters.AddWithValue("@Maker_ReasonOfAction", Maker_ReasonOfAction);
        //            cmd.Parameters.AddWithValue("@Stage", Stage);

        //            cmd.Parameters.AddWithValue("@RMCateg", RMCateg);
        //            cmd.Parameters.AddWithValue("@MatchingAtRMCycle", MatchingAtRMCycle);
        //            cmd.Parameters.AddWithValue("@RMCycle", RMCycle);

        //            cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
        //            cmd.Parameters.AddWithValue("@ReplCycle", ReplCycle);
        //            cmd.Parameters.AddWithValue("@ActionAtDateTime", DateTime.Now);

        //            cmd.Parameters.AddWithValue("@Settled", Settled);

        //            cmd.Parameters.AddWithValue("@OriginWorkFlow", OriginWorkFlow);
        //            cmd.Parameters.AddWithValue("@Operator", Operator);

        //            cmd.ExecuteNonQuery();

        //        }
        //        // Close conn
        //        // conn.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        // conn.Close();

        //        CatchDetails(ex);
        //    }
        //}

        // UPDATE Occurances stage 
        // 
        public void UpdateOccurancesStage(string InUniqueKeyOrigin, int InUniqueKey, string InStage,
                                                     DateTime InActionAtDateTime, int InRMCycle, string InMaker)
        {
            ErrorFound = false;
            ErrorOutput = "";
            int Count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[Actions_Occurances] SET "
                            + " Stage = @Stage, ActionAtDateTime = @ActionAtDateTime, RMCycle = @RMCycle "
                            + "  WHERE UniqueKeyOrigin = @UniqueKeyOrigin AND  UniqueKey =  @UniqueKey AND Stage <> '03' AND Maker = @Maker ", conn))
                    {

                        cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);
                        cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                        cmd.Parameters.AddWithValue("@Stage", InStage);
                        cmd.Parameters.AddWithValue("@ActionAtDateTime", InActionAtDateTime);
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@Maker", InMaker);

                        Count = cmd.ExecuteNonQuery();
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

        // UPDATE Occurances LoadingExcelCycleNo
        // 
        public void UpdateOccurancesLoadingExcelCycleNo(string InAtmNo, int InReplCycle, string InActionId, int InLoadingExcelCycleNo)
        {
            ErrorFound = false;
            ErrorOutput = "";
            int Count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[Actions_Occurances] SET "
                            + " LoadingExcelCycleNo = @LoadingExcelCycleNo  "
                            + "  WHERE AtmNo = @AtmNo and ReplCycle = @ReplCycle and OriginWorkFlow = 'Replenishment' and ActionId=@ActionId ", conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle);
                        cmd.Parameters.AddWithValue("@ActionId", InActionId);
                        cmd.Parameters.AddWithValue("@LoadingExcelCycleNo", InLoadingExcelCycleNo);

                        Count = cmd.ExecuteNonQuery();
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

        // DELETE (int InRMCycle, string InOriginWorkFlow)
        // 
        public void DeleteOccurancesByLoadingExcelCycleNo(int InLoadingCycle, string InOriginWorkFlow)
        {
            ErrorFound = false;
            ErrorOutput = "";
            int Count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" DELETE FROM [ATMS].[dbo].[Actions_Occurances] "
                          //  + "  WHERE RMCycle = @RMCycle AND LoadingExcelCycleNo = @LoadingExcelCycleNo  ", conn))
                          + "  WHERE LoadingExcelCycleNo = @LoadingExcelCycleNo  ", conn))
                    {

                        //cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@LoadingExcelCycleNo", InLoadingCycle); // Set it the same 

                        Count = cmd.ExecuteNonQuery();
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

        // DELETE (int InRMCycle, string InOriginWorkFlow)
        // 
        public void DeleteOccurancesByAtmNoAndSesNo(string InAtmNo, int InReplCycle )
        {
            ErrorFound = false;
            ErrorOutput = "";
            int Count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" DELETE FROM [ATMS].[dbo].[Actions_Occurances] "
                            + "  WHERE AtmNo = @AtmNo AND ReplCycle = @ReplCycle AND Stage In ('01','02') ", conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycle", InReplCycle); // 

                        Count = cmd.ExecuteNonQuery();
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
        // UPDATE Occurances stage 
        // 
        public void UpdateOccurancesForAuthoriser(string InUniqueKeyOrigin, int InUniqueKey, string InAuthoriser, int InAuthorizationKey, string InMaker)
        {
            ErrorFound = false;
            ErrorOutput = "";
            int Count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[Actions_Occurances] SET "
                            + " Authoriser = @Authoriser, AuthorizationKey = @AuthorizationKey, Settled = 1 ,  Stage = '03'  "
                             + "  WHERE UniqueKeyOrigin = @UniqueKeyOrigin AND  UniqueKey =  @UniqueKey AND Stage <> '03' AND Maker = @Maker ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);
                        cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                        cmd.Parameters.AddWithValue("@Authoriser", InAuthoriser);
                        cmd.Parameters.AddWithValue("@AuthorizationKey", InAuthorizationKey);
                        cmd.Parameters.AddWithValue("@Maker", InMaker);

                        Count = cmd.ExecuteNonQuery();
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




        // UPDATE Occurances stage 
        // 
        public void UpdateOccurancesForAuthoriser_2(int InSeqNo, string InAuthoriser, int InAuthorizationKey)
        {
            ErrorFound = false;
            ErrorOutput = "";
            int Count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[Actions_Occurances] SET "
                            + " Authoriser = @Authoriser, AuthorizationKey = @AuthorizationKey  "
                             + "  WHERE SeqNo = @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@Authoriser", InAuthoriser);
                        cmd.Parameters.AddWithValue("@AuthorizationKey", InAuthorizationKey);

                        Count = cmd.ExecuteNonQuery();
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
        // Read Totals 
        // 
        public int NumberOfPairs;

        public void ReadTotals(int InAuthorizationKey)
        {
            ErrorFound = false;
            ErrorOutput = "";

            NumberOfPairs = 0;

            string SqlString = "SELECT * "
                      + " FROM [ATMS].[dbo].[Actions_Occurances]"
                      + " WHERE AuthorizationKey = @AuthorizationKey "
                       + " ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AuthorizationKey", InAuthorizationKey);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            NumberOfPairs = NumberOfPairs + 1;

                            ReadActionOccuranceFields(rdr);

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
        // DELETE Occuarances for UniqueKey and ACTION ID AND STAGE 01 or 02
        // THIS MIGHT BE MANY 
        public void DeleteActionsOccurancesUniqueKeyAndActionID(string InUniqueKeyOrigin, int InUniqueKey, string InActionId)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int Count = 0;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[Actions_Occurances] "
                            + " WHERE UniqueKeyOrigin = @UniqueKeyOrigin AND UniqueKey =  @UniqueKey AND ActionId = @ActionId  " +
                            "AND (Stage = '01' OR Stage = '02')  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);
                        cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                        cmd.Parameters.AddWithValue("@ActionId", InActionId);

                        //rows number of record got updated
                        Count = cmd.ExecuteNonQuery();
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

        public void DeleteActionsOccurancesUniqueKeyAndActionID_Dispute_Postponed(string InUniqueKeyOrigin, int InUniqueKey, string InActionId)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int Count = 0;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[Actions_Occurances] "
                            + " WHERE UniqueKeyOrigin = @UniqueKeyOrigin AND UniqueKey =  @UniqueKey AND ActionId = @ActionId  " +
                            "AND (Stage = '01' OR Stage = '02' OR Stage = '03')  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);
                        cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                        cmd.Parameters.AddWithValue("@ActionId", InActionId);

                        //rows number of record got updated
                        Count = cmd.ExecuteNonQuery();
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
        // DELETE Occurances for UniqueKey and ACTION ID AND STAGE 03
        // THIS MIGHT BE more than one
        public void DeleteActionsOccurancesUniqueKeyAndActionID_From_CIT_Mgmnt(string InUniqueKeyOrigin, int InUniqueKey, string InActionId)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int Count = 0;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[Actions_Occurances] "
                            + " WHERE UniqueKeyOrigin = @UniqueKeyOrigin AND UniqueKey =  @UniqueKey AND ActionId = @ActionId  " +
                            "AND  Stage = '03'  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueKeyOrigin", InUniqueKeyOrigin);
                        cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);
                        cmd.Parameters.AddWithValue("@ActionId", InActionId);

                        //rows number of record got updated
                        Count = cmd.ExecuteNonQuery();
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
        // DELETE Occuarances for UniqueKey and not authorised - No Action ID 
        // THIS IS UNIQUE 
        //
        public void DeleteActionsOccurances_ForUniqueNotAuthor(string InUniqueKeyOrigin, int InUniqueKey)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int count = 0;
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[Actions_Occurances] "
                            + " WHERE UniqueKey =  @UniqueKey  AND (Stage = '01' OR Stage = '02')  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@UniqueKey", InUniqueKey);

                        //rows number of record got updated

                        count = cmd.ExecuteNonQuery();

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
