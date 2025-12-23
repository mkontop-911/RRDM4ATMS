using System;
using System.Collections;
using System.Configuration;
using System.Data;
//
using System.Data.SqlClient;
using System.IO;
using System.Linq; 
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace RRDM4ATMs
{
    public class RRDM_BULK_IST_AndOthers_Records_ALL_2 : Logger
    {
        public RRDM_BULK_IST_AndOthers_Records_ALL_2() : base() { }
    
        //public const string TableName = "[RRDM_Reconciliation_ITMX].[dbo].[BDC_IST_BULK_Records_ALL_2]";

        public int SeqNo;
        public string LOCAL_DATE;
        public string LOCAL_TIME;
        public string TXN;
        public string ACQ_CURRENCY_CODE;

        public string AMOUNT;
        public string AMOUNT_EQUIV;
        public string SETTLEMENT_AMOUNT;
        public string CH_AMOUNT;
        public string TRACE;

        public string TERMID;
        public string PAN;
        public string MASK_PAN;
        public string ACTNUM;
        public string RESPCODE;

        public string TXNSRC;
        public string TXNDEST;
        public string REFNUM;
        public string CAP_DATE;
        public string TRANDATE;

        public string TRANTIME;
        public string AUTHNUM;
        public string MERCHANT_TYPE;
        public string ACCEPTORNAME;
        public string TERMLOC;

        public string LoadedAtRMCycle;

        public bool ErrorFound;
        public string ErrorOutput;

        public bool RecordFound;

        readonly DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        //************************************************
        //

        public DataTable Bulk_IST_Table = new DataTable();


        // Uses ReconConnection String
        string connectionString = ConfigurationManager.ConnectionStrings["ReconConnectionString"].ConnectionString;

        //string connectionString = ConfigurationManager.ConnectionStrings
        // ["ATMSConnectionString"].ConnectionString;

        //string ReconConnectionString = ConfigurationManager.ConnectionStrings
        //["RRDM_Reconciliation_ITMX"].ConnectionString;

        // Read Fields In Table 
        private void ReadFieldsInTable(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];

            LOCAL_DATE = (string)rdr["LOCAL_DATE"];
            LOCAL_TIME = (string)rdr["LOCAL_TIME"];
            TXN = (string)rdr["TXN"];
            ACQ_CURRENCY_CODE = (string)rdr["ACQ_CURRENCY_CODE"];

            AMOUNT = (string)rdr["AMOUNT"];
            AMOUNT_EQUIV = (string)rdr["AMOUNT_EQUIV"];
            SETTLEMENT_AMOUNT = (string)rdr["SETTLEMENT_AMOUNT"];
            CH_AMOUNT = (string)rdr["CH_AMOUNT"];
            TRACE = (string)rdr["TRACE"];

            TERMID = (string)rdr["TERMID"];
            PAN = (string)rdr["PAN"];
            MASK_PAN = (string)rdr["MASK_PAN"];
            ACTNUM = (string)rdr["ACTNUM"];
            RESPCODE = (string)rdr["RESPCODE"];

            TXNSRC = (string)rdr["TXNSRC"];
            TXNDEST = (string)rdr["TXNDEST"];
            REFNUM = (string)rdr["REFNUM"];
            CAP_DATE = (string)rdr["CAP_DATE"];
            TRANDATE = (string)rdr["TRANDATE"];

            TRANTIME = (string)rdr["TRANTIME"];
            AUTHNUM = (string)rdr["AUTHNUM"];
            MERCHANT_TYPE = (string)rdr["MERCHANT_TYPE"];
            ACCEPTORNAME = (string)rdr["ACCEPTORNAME"];
            TERMLOC = (string)rdr["TERMLOC"];

            LoadedAtRMCycle = (string)rdr["LoadedAtRMCycle"];
        }

        //
        // Methods 
        // READ SOURCE IST RECORD 
        // 
        //
        int TotalSelected = 0;
        public decimal Read_SOURCE_Table_And_CH_Amount(int InLoadedAtRMCycle, string InAUTHNUM, string InPAN)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            decimal CH_AMOUNT = 0;

            SqlString =
               " SELECT [SeqNo]  "
                 + ",[TXN] "
                + ",[ACQ_CURRENCY_CODE] "
                + ", CAST([AMOUNT] As decimal(18,2))  as AMOUNT "
                 + ", CAST([AMOUNT_EQUIV] As decimal(18,2))  as AMOUNT_EQUIV "
                  + ", CAST([SETTLEMENT_AMOUNT] As decimal(18,2))  as SETTLEMENT_AMOUNT "
                   + ", CAST([CH_AMOUNT] As decimal(18,2))  as CH_AMOUNT "
                + ",[PAN] "
                + ",[ACTNUM] "
                + ",[TXNSRC] "
                + ",[TXNDEST] "
                + ",[MERCHANT_TYPE] "
                + ",[ACCEPTORNAME] "
                + ",[TERMLOC] "
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].BULK_Switch_IST_Txns_ALL_2 "
               + " WHERE LoadedAtRMCycle = @LoadedAtRMCycle AND AUTHNUM = @AUTHNUM AND PAN = @PAN "
               + " ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);
                        cmd.Parameters.AddWithValue("@AUTHNUM", InAUTHNUM);
                        cmd.Parameters.AddWithValue("@PAN", InPAN);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            SeqNo = (int)rdr["SeqNo"];

                            string TXN = (string)rdr["TXN"];

                            string ACQ_CURRENCY_CODE = (string)rdr["ACQ_CURRENCY_CODE"];

                            decimal AMOUNT = (decimal)rdr["AMOUNT"];
                            decimal AMOUNT_EQUIV = (decimal)rdr["AMOUNT_EQUIV"];
                            decimal SETTLEMENT_AMOUNT = (decimal)rdr["SETTLEMENT_AMOUNT"];

                            CH_AMOUNT = (decimal)rdr["CH_AMOUNT"];

                            string PAN = (string)rdr["PAN"];
                            string ACTNUM = (string)rdr["ACTNUM"];
                            string TXNSRC = (string)rdr["TXNSRC"];
                            string TXNDEST = (string)rdr["TXNDEST"];
                            string MERCHANT_TYPE = (string)rdr["MERCHANT_TYPE"];
                            string ACCEPTORNAME = (string)rdr["ACCEPTORNAME"];
                            string TERMLOC = (string)rdr["TERMLOC"];


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
            return CH_AMOUNT;
        }
        // GET AMOUNTS FOR RATE 
        public decimal FxAmt;
        public decimal LocalAmt;
        //
        public void Read_SOURCE_Table_And_GET_FX_AND_LOCAL_Amts(int InLoadedAtRMCycle, string InFxCcy)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            SqlString =
               " SELECT TOP 1 [SeqNo]  "
                + ",CAST([AMOUNT] As decimal(18,2)) As FxAmt "
                + ",CAST([CH_AMOUNT] As decimal(18,2)) As LocalAmt "
               + " FROM [RRDM_Reconciliation_ITMX].BULK_Switch_IST_Txns_ALL "
               + " WHERE LoadedAtRMCycle =  @LoadedAtRMCycle AND RESPCODE = '0' AND ACQ_CURRENCY_CODE = @ACQ_CURRENCY_CODE "
               + " ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);
                        cmd.Parameters.AddWithValue("@ACQ_CURRENCY_CODE", InFxCcy);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            FxAmt = (decimal)rdr["FxAmt"];
                            LocalAmt = (decimal)rdr["LocalAmt"];

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

        // Read Field Names 
        //

        public ArrayList ReadTableToGetFieldNames_Array_List(string InTableId, bool InWithHeaders, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Mode = 1 or 2

            ArrayList FieldNames = new ArrayList();
            if (InWithHeaders == true)
            {
                FieldNames.Add("Please Fill");
                FieldNames.Add("Not_Present_but_has_value");
                FieldNames.Add("Not_Present_but_has_Formula");
            }
            string PhysicalFileName;
            if (InMode == 1)
            {
                PhysicalFileName = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId;
            }
            else
            {
                PhysicalFileName = InTableId;
            }

            //+ InTableId; 
            TotalSelected = 0;

          

            //// DATA TABLE ROWS DEFINITION 

            SqlString = " SELECT name as FieldNm,column_id, system_type_id,max_length,precision,scale "
            + " FROM sys.columns "
            + " WHERE[object_id] = OBJECT_ID('" + PhysicalFileName + "')"
            + " ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@BankId", InUserBankId);
                        //cmd.Parameters.AddWithValue("@ParamId", ParamId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;
                            TotalSelected = TotalSelected + 1;
                            string WFieldNm = (string)rdr["FieldNm"];
                            FieldNames.Add(WFieldNm);
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

            return FieldNames;
        }


        public ArrayList ReadTableToGetFieldNames_Array_List_MOBILE(string InDataBase, string InTableId, bool InWithHeaders, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Mode = 1 or 2

            ArrayList FieldNames = new ArrayList();
            if (InWithHeaders == true)
            {
                FieldNames.Add("Please Fill");
                FieldNames.Add("Not_Present_but_has_value");
                FieldNames.Add("Not_Present_but_has_Formula");
            }
            string PhysicalFileName;
            if (InMode == 1)
            {
                PhysicalFileName = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId;
            }
            else
            {
                PhysicalFileName = InTableId;
            }

            //+ InTableId; 
            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            SqlString = " USE "+InDataBase+ " SELECT name as FieldNm,column_id, system_type_id,max_length,precision,scale "
            + " FROM sys.columns "
            + " WHERE[object_id] = OBJECT_ID('" + PhysicalFileName + "')"
            + " ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@BankId", InUserBankId);
                        //cmd.Parameters.AddWithValue("@ParamId", ParamId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;
                            TotalSelected = TotalSelected + 1;
                            string WFieldNm = (string)rdr["FieldNm"];
                            FieldNames.Add(WFieldNm);
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

            return FieldNames;
        }


        // Read Formula Values 
        //

        public ArrayList ReadTableToGetFormulaValues_Array_List(string InTableId, string InFormula 
                                               , int InMode, bool IsDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string ResultField; 

            // Mode = 1 or 2

            ArrayList FormulaValues = new ArrayList();
           
            string PhysicalFileName;
            if (InMode == 1)
            {
                PhysicalFileName = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId;
            }
            else
            {
                PhysicalFileName = "[RRDM_Reconciliation_ITMX].[dbo]." + InTableId;
            }

            //+ InTableId; 
            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 
            SqlString = "SELECT DISTINCT " + InFormula + " As ResultField "
                              + " FROM " + PhysicalFileName
                              ;

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@BankId", InUserBankId);
                        //cmd.Parameters.AddWithValue("@ParamId", ParamId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            
                            TotalSelected = TotalSelected + 1;
                            if (IsDate == true)
                            {
                               DateTime D_ResultField = (DateTime)rdr["ResultField"];
                               ResultField = D_ResultField.ToString(); 
                            }
                            else
                            {
                                ResultField = (string)rdr["ResultField"];
                            }
                           
                            FormulaValues.Add(ResultField);
                            // LEAVE it here to catch the error
                            RecordFound = true;
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

                    ErrorOutput = ex.Message;
                }

            return FormulaValues;
        }

        //
        // Find if field exists 
        //
        public void ReadTableToGetFieldNames(string InTableId, string InFieldId)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string PhysicalFileName = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId;
            //+ InTableId; 
            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            SqlString = " SELECT name as FieldNm,column_id, system_type_id,max_length,precision,scale "
            + " FROM sys.columns "
            + " WHERE[object_id] = OBJECT_ID('" + PhysicalFileName + "') AND name=@name"
            + " ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", InFieldId);
                        //cmd.Parameters.AddWithValue("@ParamId", ParamId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            string WFieldNm = (string)rdr["FieldNm"];

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




        // GET Array List Occurance Nm 
        //
        public ArrayList GetFIELD_Values(string InTableId, string InFieldName)
        {
            ArrayList FieldValues = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string PhysicalFileName = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId;
            //+ InTableId;

            string SqlString = "SELECT TOP 50 Left(" + InFieldName + ",20) As FieldValue"
                                + " FROM " + PhysicalFileName;
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@BankId", InUserBankId);
                        //cmd.Parameters.AddWithValue("@ParamId", ParamId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;
                            string WFiledValue = (string)rdr["FieldValue"];
                            FieldValues.Add(WFiledValue);
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

            return FieldValues;
        }
        // Clean testing tables and records 
        //
        public void CleanTables(string InSourceTable, int InRMCycle)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string BulkTable_ALL = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InSourceTable + "_ALL";
            string Std_RRDM = "[RRDM_Reconciliation_ITMX].[dbo]." + InSourceTable;
            string Reversals = "[RRDM_Reconciliation_ITMX].[dbo].REVERSALs_PAIRs";

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            // Read fields of existing table
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            bool WithHeaders = false;
            // Clean BULK
            Bg.ReadTableToGetFieldNames_Array_List(BulkTable_ALL, WithHeaders, 2);

            if (Bg.RecordFound == true)
            {
                UNDO_Table_For_Cycle_Delete_Loaded_Only(BulkTable_ALL, InRMCycle);
            }

            // OTHER FILES

            // CLEAN RRDM STANDARD
            Bg.ReadTableToGetFieldNames_Array_List(Std_RRDM, WithHeaders, 2);

            if (Bg.RecordFound == true)
            {
                // Delete records for RRDM standard and created Reversals
                UNDO_Table_For_Cycle_Delete_Loaded_Only(Std_RRDM, InRMCycle);
            }

            // Clean REversals 
            Bg.ReadTableToGetFieldNames_Array_List(Reversals, WithHeaders, 2);

            if (Bg.RecordFound == true)
            {
                // Delete records for RRDM standard and created Reversals
                UNDO_Table_For_Cycle_Delete_Reversals(Reversals, InRMCycle);
            }

        }
        // Truncate Table 
        //
        public void TruncateTable(string InPhysicalFileName)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            //ArrayList FieldNamesC = new ArrayList();
            bool WithHeaders = false;

            // FIND OUT IF FILE EXIST

            Bg.ReadTableToGetFieldNames_Array_List(InPhysicalFileName, WithHeaders, 2);

            if (Bg.RecordFound == true)
            {
                // FILE EXISTS
                string SQLCmd = "TRUNCATE TABLE " + InPhysicalFileName;

                using (SqlConnection conn = new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
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
        //
        // UNDO Table FOR Cycle -  BULK 
        //
        public void UNDO_Table_For_Cycle_Delete_Loaded_Only(string InPhysicalFileName, int InRMCycleNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Counter = 0;

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            //ArrayList FieldNamesC = new ArrayList();
            bool WithHeaders = false;

            // FIND OUT IF FILE EXIST 
            Bg.ReadTableToGetFieldNames_Array_List(InPhysicalFileName, WithHeaders, 2);

            if (Bg.RecordFound == true)
            {
                // FILE EXISTS
                // DO THE JOB ---- ONLY DELETE ... NO UPDATE 
                //
                // DELETE ALL RECORDS LOADED FOR THIS CYCLE 
                //
                string SQLCmd = "DELETE FROM " + InPhysicalFileName
                                 + " WHERE LoadedAtRMCycle = @RMCycleNo ";

                using (SqlConnection conn = new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@RMCycleNo ", InRMCycleNo);

                            cmd.CommandTimeout = 300;  // seconds
                            Counter = cmd.ExecuteNonQuery();
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

        //
        // UNDO Table FOR Cycle -  BULK 
        //
        public void UNDO_Table_For_Cycle_Delete_Loaded_Only_MOBILE(string InDataBase, string InPhysicalFileName, int InRMCycleNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Counter = 0;

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            //ArrayList FieldNamesC = new ArrayList();
            bool WithHeaders = false;

            // FIND OUT IF FILE EXIST 
            Bg.ReadTableToGetFieldNames_Array_List_MOBILE(InDataBase, InPhysicalFileName, WithHeaders, 2);
            //Bg.ReadTableToGetFieldNames_Array_List(InPhysicalFileName, WithHeaders, 2);

            if (Bg.RecordFound == true)
            {
                // FILE EXISTS
                // DO THE JOB ---- ONLY DELETE ... NO UPDATE 
                //
                // DELETE ALL RECORDS LOADED FOR THIS CYCLE 
                //
                int Count = 0; 
                string SQLCmd = "DELETE FROM " + InPhysicalFileName
                                 + " WHERE LoadedAtRMCycle = @RMCycleNo ";

                using (SqlConnection conn = new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@RMCycleNo ", InRMCycleNo);

                            cmd.CommandTimeout = 300;  // seconds
                            Counter = cmd.ExecuteNonQuery();
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
        }

        //
        // UNDO Reversals 
        //
        public void UNDO_Table_For_Cycle_Delete_Reversals(string InPhysicalFileName, int InRMCycleNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Counter = 0;

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            //ArrayList FieldNamesC = new ArrayList();
            bool WithHeaders = false;

            // FIND OUT IF FILE EXIST 
            Bg.ReadTableToGetFieldNames_Array_List(InPhysicalFileName, WithHeaders, 2);

            if (Bg.RecordFound == true)
            {
                // FILE EXISTS
                // DO THE JOB ---- ONLY DELETE ... NO UPDATE 
                //
                // DELETE ALL RECORDS LOADED FOR THIS CYCLE 
                //
                string SQLCmd = "DELETE FROM " + InPhysicalFileName
                                 + " WHERE RMCycleNo = @RMCycleNo ";

                using (SqlConnection conn = new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@RMCycleNo ", InRMCycleNo);

                            cmd.CommandTimeout = 300;  // seconds
                            Counter = cmd.ExecuteNonQuery();
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
        //
        // UNDO Table FOR Cycle -  NON BULK 
        //
        public string ProgressText_3 ;
        public void UNDO_Table_For_Cycle(string InPhysicalFileName, int InRMCycleNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Counter = 0;

            ProgressText_3 = ""; 

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            //ArrayList FieldNamesC = new ArrayList();
            bool WithHeaders = false;

            // FIND OUT IF FILE EXIST
            Bg.ReadTableToGetFieldNames_Array_List(InPhysicalFileName, WithHeaders, 2);

            if (Bg.RecordFound == true)
            {
                // FILE EXISTS
                // DO THE JOB
                //
                // DELETE ALL RECORDS LOADED FOR THIS CYCLE 
                //if (InPhysicalFileName == "[QAHERA].[dbo].[QAHERA_TPF_TXNS]")
                //{
                //    InPhysicalFileName = InPhysicalFileName + "_MASTER";                
                //}
                    //
                string SQLCmd = "DELETE FROM " + InPhysicalFileName
                                 + " WHERE LoadedAtRMCycle = @RMCycleNo ";

                using (SqlConnection conn = new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycleNo);

                            cmd.CommandTimeout = 300;  // seconds
                            Counter = cmd.ExecuteNonQuery();
                            cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                        ProgressText_3 = ProgressText_3 + InPhysicalFileName
                               + "..Deleted Records.." + Counter.ToString() + Environment.NewLine;
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchDetails(ex);
                    }

                ErrorFound = false;
                ErrorOutput = "";



            }
        }

        public void UNDO_Table_For_Cycle_MOBILE(string InDB, string InPhysicalFileName, int InRMCycleNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Counter = 0;

            ProgressText_3 = "";

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            //ArrayList FieldNamesC = new ArrayList();
            bool WithHeaders = false;

            // FIND OUT IF FILE EXIST
            Bg.ReadTableToGetFieldNames_Array_List_MOBILE(InDB, InPhysicalFileName, WithHeaders, 2);

            if (Bg.RecordFound == true)
            {
                // FILE EXISTS
                // DO THE JOB
                //
                // DELETE ALL RECORDS LOADED FOR THIS CYCLE 
                int Count = 0;

                string SQLCmd = "DELETE FROM " + InPhysicalFileName
                                 + " WHERE LoadedAtRMCycle = @RMCycleNo ";

                using (SqlConnection conn = new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycleNo);

                            cmd.CommandTimeout = 300;  // seconds
                            Counter = cmd.ExecuteNonQuery();
                            Count = cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                        ProgressText_3 = ProgressText_3 + InPhysicalFileName
                               + "..Deleted Records.." + Counter.ToString() + Environment.NewLine;
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchDetails(ex);
                    }

                if (InPhysicalFileName == InDB + ".[dbo].QAHERA_TPF_TXNS" || InPhysicalFileName == InDB + ".[dbo].ETISALAT_TPF_TXNS")
                {
                    InPhysicalFileName = InPhysicalFileName + "_MASTER";

                    SQLCmd = "DELETE FROM " + InPhysicalFileName
                                 + " WHERE LoadedAtRMCycle = @RMCycleNo ";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd =
                                new SqlCommand(SQLCmd, conn))
                            {
                                cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycleNo);

                                cmd.CommandTimeout = 300;  // seconds
                                Counter = cmd.ExecuteNonQuery();
                                Count = cmd.ExecuteNonQuery();
                            }
                            // Close conn
                            conn.Close();
                            ProgressText_3 = ProgressText_3 + InPhysicalFileName
                                   + "..Deleted Records.." + Counter.ToString() + Environment.NewLine;
                        }
                        catch (Exception ex)
                        {
                            conn.Close();
                            CatchDetails(ex);
                        }

                }
                //

                ErrorFound = false;
                ErrorOutput = "";

                // If MOBILE Do for the old records matched at this Cycle 

                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(" UPDATE " + InPhysicalFileName
                       + " SET "
                       + "    [Comments] = '' "
                       + "   ,[IsMatchingDone] = 0 "
                       + "   ,[Matched] = 0 "
                       + "   ,[MatchMask] = '' "
                       + "  WHERE MatchingAtRMCycle = @RMCycleNo "
                       , conn))
                        {
                            cmd.Parameters.AddWithValue("@RMCycleNo", InRMCycleNo);

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
        // DELETE UNWANTED BDC299
        public void DELETE_BDC299ByLowerLimit(string InPhysicalFileName, DateTime InLowerLimitDate)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Counter = 0;

          //  ProgressText_3 = "";

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            //ArrayList FieldNamesC = new ArrayList();
            bool WithHeaders = false;

            // FIND OUT IF FILE EXIST
            Bg.ReadTableToGetFieldNames_Array_List(InPhysicalFileName, WithHeaders, 2);

            if (Bg.RecordFound == true)
            {
                // FILE EXISTS
                // DO THE JOB
                //
                // DELETE ALL RECORDS LOADED FOR THIS CYCLE 
                //
                string SQLCmd = "DELETE FROM " + InPhysicalFileName
                                 + " WHERE MatchingCateg = 'BDC299' AND Net_TransDate <= @Net_TransDate ";

                using (SqlConnection conn = new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@Net_TransDate", InLowerLimitDate);

                            cmd.CommandTimeout = 300;  // seconds
                            Counter = cmd.ExecuteNonQuery();
                            cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                        //ProgressText_3 = ProgressText_3 + InPhysicalFileName
                        //       + "..Deleted Records.." + Counter.ToString() + Environment.NewLine;
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchDetails(ex);
                    }

            }
        }
        //
        // UNDO Table FOR Cycle -  NON BULK 
        //

        public void UpdateAgingRecords(string InPhysicalFileName, int InRMCycleNo
                                                ,DateTime InAgingDateLimitShort, DateTime InAgingDateLimit_POS)
        {
            //
            // This method updates aging records in order to be moved to Matched or Historical
            //
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Counter = 0;

            //ProgressText_3 = "";

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            //ArrayList FieldNamesC = new ArrayList();
            bool WithHeaders = false;

            // FIND OUT IF FILE EXIST
            Bg.ReadTableToGetFieldNames_Array_List(InPhysicalFileName, WithHeaders, 2);

            if (Bg.RecordFound == true)
            {
                // FILE EXISTS
                // DO THE JOB
                //
                // DELETE ALL RECORDS LOADED FOR THIS CYCLE 
                //
                string SQLCmd = "  UPDATE " + InPhysicalFileName
                                + " SET "
                                + " [Processed] = 1 "
                                + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle "
                                + ",[Comment] = SUBSTRING('Aging_Record_' + Comment , 1, 80) "
                                + " WHERE Net_TransDate <= @AgingLimitDateShort AND Processed = 0 "
                                + " AND MatchingCateg NOT IN ('BDC231', 'BDC233', 'BDC272', 'BDC273', 'BDC208') "
                                // BDC208 has to do with Meeza that is mixed up 
                                
                                 + "  UPDATE " + InPhysicalFileName
                                 + " SET "
                                  + " [Processed] = 1 "
                                + " ,[ProcessedAtRMCycle] = @ProcessedAtRMCycle "
                                + ",[Comment] = SUBSTRING('Aging_Record_' + Comment , 1, 80) "
                                + " WHERE Net_TransDate <= @AgingDateLimit_POS AND Processed = 0 "
                                + " AND MatchingCateg IN ('BDC231', 'BDC233', 'BDC272', 'BDC273') "; 

                //string SQLCmd = "DELETE FROM " + InPhysicalFileName
                //                 + " WHERE LoadedAtRMCycle = @RMCycleNo ";

                using (SqlConnection conn = new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQLCmd, conn))
                        {
                            cmd.Parameters.AddWithValue("@ProcessedAtRMCycle", InRMCycleNo);
                            cmd.Parameters.AddWithValue("@AgingLimitDateShort ", InAgingDateLimitShort);
                            cmd.Parameters.AddWithValue("@AgingDateLimit_POS ", InAgingDateLimit_POS);

                            cmd.CommandTimeout = 300;  // seconds
                            Counter = cmd.ExecuteNonQuery();
                            cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                        //ProgressText_3 = ProgressText_3 + InPhysicalFileName
                        //       + "..Deleted Records.." + Counter.ToString() + Environment.NewLine;
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        CatchDetails(ex);
                    }
             

            }
        }
        //
        // Read table to see if records
        //
        public void ReadTableToSeeIfRecords(string InTableId)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            //// 

            string PhysicalFileName = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId;
            //+ InTableId;

            SqlString = "SELECT TOP 1 * "
                                + " FROM " + PhysicalFileName
                                + " "
                                ;

            using (SqlConnection conn =
                       new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        // cmd.Parameters.AddWithValue("@name", InFieldName);
                        //cmd.Parameters.AddWithValue("@ParamId", ParamId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true; // leave here to catch the error before 
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
                    ErrorOutput = ex.Message;
                    //CatchDetails(ex);
                }

        }

        //
        // Find Tables Delimiter Errors
        //
        bool Correct; 
        public bool FindIfDelimiterErrorsInInputTable(string InTableId,string InInputFilePath, string InConnectionString)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            bool Correct = false; 

            //InTableId = "[ETISALAT].[dbo].[BULK_ETISALAT_TPF_TXNS_ALL]";
            //InConnectionString = ConfigurationManager.ConnectionStrings["ETISALATConnectionString"].ConnectionString;
            //InInputFilePath = "C:\\VBoxShared\\ETISALAT_TPF_TXNS_20250110.001";

            //*******************************************
            string connectionString = InConnectionString;
            string tableName = InTableId; 

            int columnCount;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT TOP 1 * FROM {tableName}"; // Get one row to inspect column count

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    columnCount = reader.FieldCount; // Get number of columns
                                                     // Console.WriteLine($"Number of columns: {columnCount}");
                }
            }

            int DelimiterNo = columnCount - 3; // seqno, Cycle number and -1
            string filePath = InInputFilePath;  // Path to your file
            int expectedCommas = DelimiterNo; // Expected number of commas per line
            string ErrorsfilePath = "C:\\RRDM\\WORKING\\Delimeter_errors.002";

            try
            {
                int lineNumber = 1;
                int FoundExceptions = 0;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        int commaCount = line.Split(',').Length - 1; // Count commas
                        if (commaCount != expectedCommas)
                        {
                            //  Console.WriteLine($"Line {lineNumber}: Expected {expectedCommas}, Found {commaCount}");

                            string textToAdd = line + "Expected:" + expectedCommas + "Found:" + commaCount;

                            File.AppendAllText(ErrorsfilePath, textToAdd + Environment.NewLine);

                            FoundExceptions = FoundExceptions + 1;
                        }
                        lineNumber++;
                    }
                }
                // Console.WriteLine("Check completed!");
                if (FoundExceptions > 0)
                {
                   MessageBox.Show("Checked of commas completed. Exceptions found:" + FoundExceptions.ToString()+Environment.NewLine
                       + "You will find the Errors in..." + ErrorsfilePath
                       );
                    Correct = false;
                }
                else Correct = true; 

            }
            catch (Exception ex)
            {
                ErrorOutput = ex.Message;
                CatchDetails(ex);
            }

            return Correct; 
            ///
            

        }

        public bool FindIfDelimiterErrorsInInputTable_ATMS(string InTableId, string InInputFilePath, string InDelimeter, string InConnectionString)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int FoundExceptions = 0; 

            TotalSelected = 0;

            bool Correct = false;

            //*******************************************
            string connectionString = InConnectionString;
            string tableName = InTableId;

            int columnCount;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT TOP 1 * FROM {tableName}"; // Get one row to inspect column count

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    columnCount = reader.FieldCount; // Get number of columns
                                                     // Console.WriteLine($"Number of columns: {columnCount}");
                }
            }

            DateTime NowTime = DateTime.Now;

            int DelimiterNo = columnCount - 1; //  -1
            string ErrorsfilePath = "C:\\RRDM\\WORKING\\Delimeter_errors_"+ InTableId + "_.003";

            if (File.Exists(ErrorsfilePath))
            {
                File.Delete(ErrorsfilePath);
                //Console.WriteLine("File deleted successfully.");
            }

            string filePath = InInputFilePath;  // Path to your file
                                                // int expectedCommas = DelimiterNo; // Expected number of commas per line
            

            if (InDelimeter == "tap")
            {
                if (File.Exists(filePath))
                {
                    var lines = File.ReadLines(filePath); // Read lines one by one
                    int lineNumber = 1;

                    foreach (var line in lines)
                    {
                        int tabCount = 0;

                        // Count tabs manually
                        foreach (char c in line)
                        {
                            if (c == '\t') tabCount++;
                        }

                        if (tabCount != DelimiterNo)
                        {
                            //  Console.WriteLine($"Line {lineNumber}: Expected {expectedCommas}, Found {commaCount}");

                            string textToAdd = line + "Expected:" + DelimiterNo + "Found:" + tabCount;

                            File.AppendAllText(ErrorsfilePath, textToAdd + Environment.NewLine);

                            FoundExceptions = FoundExceptions + 1;
                        }

                        //Console.WriteLine($"Line {lineNumber}: {tabCount} tab(s)");
                        lineNumber++;
                    }

                    if (FoundExceptions > 0)
                    {
                        string text = "Checked of taps completed. Exceptions found:" + FoundExceptions.ToString() + Environment.NewLine
                            + "You will find the Errors in..." + ErrorsfilePath;
                        string caption = "Files_Loading";
                        int timeout = 5000;
                        AutoClosingMessageBox.Show(text, caption, timeout);

                        //MessageBox.Show("Checked of taps completed. Exceptions found:" + FoundExceptions.ToString() + Environment.NewLine
                        //    + "You will find the Errors in..." + ErrorsfilePath
                        //    );
                        Correct = false;
                    }
                    else Correct = true;
                }
               
            }

            if (InDelimeter == ",")
            {
                try
                {
                    int lineNumber = 1;
                    int CountDel = 0;
                    FoundExceptions = 0;
                    using (StreamReader reader = new StreamReader(filePath))
                    {

                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (InDelimeter == ",")
                            {
                                CountDel = line.Split(',').Length - 1;
                            }

                            if (CountDel != DelimiterNo)
                            {
                                //  Console.WriteLine($"Line {lineNumber}: Expected {expectedCommas}, Found {commaCount}");

                                string textToAdd = line + "Expected:" + DelimiterNo + "Found:" + CountDel;

                                File.AppendAllText(ErrorsfilePath, textToAdd + Environment.NewLine);

                                FoundExceptions = FoundExceptions + 1;
                            }
                            lineNumber++;
                        }
                    }
                    // Console.WriteLine("Check completed!");
                    if (FoundExceptions > 0)
                    {
                        string text = "Checked of taps completed. Exceptions found:" + FoundExceptions.ToString() + Environment.NewLine
                            + "You will find the Errors in..." + ErrorsfilePath;
                        string caption = "Files_Loading";
                        int timeout = 5000;
                        AutoClosingMessageBox.Show(text, caption, timeout);
                        //MessageBox.Show("Checked of taps completed. Exceptions found:" + FoundExceptions.ToString() + Environment.NewLine
                        //    + "You will find the Errors in..." + ErrorsfilePath
                        //    );
                        Correct = false;
                    }
                    else Correct = true;

                }
                catch (Exception ex)
                {
                    ErrorOutput = ex.Message;
                    CatchDetails(ex);
                }

            }


            return Correct;
            ///


        }

        public bool CheckIfWindowsOrUnix_ATMS(string InInputFilePath)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool Response = false;

            string filePath = InInputFilePath; // Change this to your actual file path

            if (File.Exists(filePath))
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string fileContent = System.Text.Encoding.UTF8.GetString(fileBytes);

                if (fileContent.Contains("\r\n"))
                {
                    //MessageBox.Show("The file uses Windows (CRLF) line endings.");
                    Response = true; 
                }
                else if (fileContent.Contains("\n"))
                {
                    //MessageBox.Show("The file uses Unix (LF) line endings.");
                }
                else
                {
                    MessageBox.Show("No standard line endings detected.");
                }
            }
            else
            {
                MessageBox.Show("File not found.");
            }

            return Response; 

            //string line; 

            //try
            //{
            //    using (StreamReader reader = new StreamReader(InInputFilePath))
            //    {
            //        for (int i = 0; i < 10; i++) // Read first 10 lines
            //        {
            //            line = reader.ReadLine();
            //            if (line == null) break; // End of file

            //            if (line.Contains("\r\n")) // Windows lines end with \r 
            //            {
            //                return true;
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error reading file: {ex.Message}");
            //}
            //return false;
        }



        //bool Correct = false;

        //    string filePath = InInputFilePath; // Replace with your actual file path

        //    string content = File.ReadAllText(filePath);

        //    if (content.Contains("\r\n"))
        //    {
        //        // WINDOWS
        //        Correct = true; 
        //    }
        //    if (content.Contains("\n"))
        //    {
        //        // UNIX FILE 
        //        Correct = false;
        //    }
        //    if (content.Contains("CRLF"))
        //    {
        //        // WINDOWS
        //        Correct = true;
        //    }


         //   return Correct;
            ///


        //}

        //
        // Find Tables Delimiter Errors
        //
        public void ReplaceValueInGivenTextFile(string InInputFilePath, string InOldString, string InNewString)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            //InInputFilePath = "C:\\VBoxShared\\ETISALAT_TPF_TXNS_20250110.001";
            //InOldString = "faisal,giza";
            //InNewString = "faisal giza"; // No comma

            //// 
            ///
            string inputFilePath = InInputFilePath;  // Large file path
            string tempFilePath = "C:\\RRDM\\WORKING\\Replace_Temporary"; // Temporary file for writing
            string oldString = InOldString; // String to replace
            string newString = InNewString; // Replacement string

            try
            {
                // Open the input and output file streams
                using (StreamReader reader = new StreamReader(inputFilePath))
                using (StreamWriter writer = new StreamWriter(tempFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null) // Read line by line
                    {
                        writer.WriteLine(line.Replace(oldString, newString)); // Replace & write
                    }
                }

                // Replace original file with the modified one
                File.Delete(inputFilePath);
                File.Move(tempFilePath, inputFilePath);

                MessageBox.Show("Replacement completed successfully!");

                //Console.WriteLine("Replacement completed successfully!");
            }
            catch (Exception ex)
            {
               
                  //  conn.Close();
                    ErrorOutput = ex.Message;
                    CatchDetails(ex);
                
            }

            

        }

        //
        // Get table of Values 
        //
        public DataTable FieldsValueDataTable = new DataTable();

        public void ReadTableToGetFieldValues(string InTableId, string InFieldName)
        {
            // " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version "
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            FieldsValueDataTable = new DataTable();
            FieldsValueDataTable.Clear();

            TotalSelected = 0;

            //int Length = InFieldName.Length;

            string PhysicalFileName = "[RRDM_Reconciliation_ITMX].[dbo]." + InTableId;
            //+ InTableId; 


            //// DATA TABLE ROWS DEFINITION 

            //SqlString = " SELECT name ,column_id, system_type_id,Cast(max_length As int) as FieldLength,precision,scale "
            //+ " FROM sys.columns "
            //+ " WHERE[object_id] = OBJECT_ID('" + PhysicalFileName + "')" + " AND name =@name"
            //+ " ";

            //using (SqlConnection conn =
            //             new SqlConnection(connectionString))
            //    try
            //    {
            //        conn.Open();
            //        using (SqlCommand cmd =
            //            new SqlCommand(SqlString, conn))
            //        {
            //            cmd.Parameters.AddWithValue("@name", InFieldName);
            //            //cmd.Parameters.AddWithValue("@ParamId", ParamId);

            //            // Read table 
            //            SqlDataReader rdr = cmd.ExecuteReader();


            //            while (rdr.Read())
            //            {
            //                RecordFound = true;

            //                Length = (int)rdr["FieldLength"];
            //            }

            //            // Close Reader
            //            rdr.Close();
            //        }

            //        // Close conn
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        conn.Close();

            //        CatchDetails(ex);
            //    }


            //// DATA TABLE ROWS DEFINITION 

           // PhysicalFileName = "[RRDM_Reconciliation_ITMX].[dbo]." + InTableId;
            //+ InTableId;
            // DISTINCT
            //SqlString = "SELECT TOP 100 Left(" + InFieldName + ","+ Length + ") As FieldValue "
            //                    + " FROM " + PhysicalFileName;
            if (InFieldName == "Please Fill")
            {
                return;
            }

            SqlString = "SELECT DISTINCT TOP 1000 " + InFieldName + " As FieldValue "
                                + " FROM " + PhysicalFileName;

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.Fill(FieldsValueDataTable);

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
        // Get table of Values 
        //
        public DataTable Fields_NAMES_DataTable = new DataTable();

        public void ReadTableToGetField_NAMES(string InTableId)
        {
            // " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version "
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string Field_Name; 

            Fields_NAMES_DataTable = new DataTable();
            Fields_NAMES_DataTable.Clear();

            Fields_NAMES_DataTable.Columns.Add("Field_Name", typeof(string));

            TotalSelected = 0;

            string PhysicalFileName = "[RRDM_Reconciliation_ITMX].[dbo]." + InTableId;

            //// DATA TABLE ROWS DEFINITION 

            SqlString = " SELECT name as Field_Name,column_id, system_type_id,max_length,precision,scale "
         + " FROM sys.columns "
         + " WHERE[object_id] = OBJECT_ID('" + PhysicalFileName + "')"
         + " ";

            using (SqlConnection conn =
                         new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@name", InFieldName);
                        //cmd.Parameters.AddWithValue("@ParamId", ParamId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;

                            Field_Name = (string)rdr["Field_Name"];

                          //  Fields_NAMES_DataTable.Columns.Add("Field_Name", typeof(string));

                            DataRow RowSelected = Fields_NAMES_DataTable.NewRow();

                            RowSelected["Field_Name"] = Field_Name;
                            
                            Fields_NAMES_DataTable.Rows.Add(RowSelected);
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
        public string ReformatedField;
        public void ReadTableToGetReformatForSpecific(string InTableId, string InFieldName,
                                                     string InFieldValue, string InReformat)
        {
            // " WHERE OutputFile_Id = @OutputFile_Id AND OutputFile_Version = @OutputFile_Version "
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;


            string PhysicalFileName = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId;

            if (InFieldName == "Not_Present_but_has_Formula")
            {
                SqlString = "SELECT TOP 1 " + InReformat + " As ReformatedField "
                               + " FROM " + PhysicalFileName
                               ;
            }
            else
            {
                SqlString = "SELECT TOP 1 " + InReformat + " As ReformatedField "
                                + " FROM " + PhysicalFileName
                                + " WHERE " + InFieldName + "='" + InFieldValue + "'"
                                ;
            }
            //// 
            using (SqlConnection conn =
                       new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", InFieldName);
                        //cmd.Parameters.AddWithValue("@ParamId", ParamId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {

                            ReformatedField = (string)rdr["ReformatedField"];

                            RecordFound = true; // leave here to catch the error before 
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
                    ErrorOutput = ex.Message;
                    //CatchDetails(ex);
                }

        }

        // ReFormat a field 
        public void Read_SOURCE_Table_AND_Reformat_Field(string InTableId,
                 string InFieldId, string InRTN_Name, int InLoadedAtRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            switch (InRTN_Name)
            {
                case "Rtn_Clear_Amt":
                    {
                        // Call a method 
                        Rtn_Clear_Amt(InTableId,
                                         InFieldId, InLoadedAtRMCycle);
                        break;
                    }
                case "Rtn_NONE":
                    {


                        break;
                    }
                default:
                    {
                        string stpErrorText = "NOT VALID FILE ID";
                        break;
                    }
            }


            ////SqlString =
            ////   " SELECT " + InRTN_Name + " As InputField"
            ////   + " FROM [RRDM_Reconciliation_ITMX].BULK_"+ InTableId
            ////   + " "
            ////   + " ";

            ////using (SqlConnection conn =
            ////              new SqlConnection(connectionString))
            ////    try
            ////    {
            ////        conn.Open();
            ////        using (SqlCommand cmd =
            ////            new SqlCommand(SqlString, conn))
            ////        {

            ////          //  cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);

            ////            // Read table 

            ////            SqlDataReader rdr = cmd.ExecuteReader();

            ////            while (rdr.Read())
            ////            {

            ////                RecordFound = true;

            ////                string WInputField = (string)rdr["InputField"];


            ////            }
            ////            // Close Reader
            ////            rdr.Close();
            ////        }
            ////        // Close conn
            ////        conn.Close();
            ////    }
            ////    catch (Exception ex)
            ////    {
            ////        conn.Close();

            ////        CatchDetails(ex);
            ////    }

        }

        // ReFormat a field 
        public void Rtn_Clear_Amt(string InTableId,
                 string InFieldId, int InLoadedAtRMCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int Counter;

            TotalSelected = 0;

            string SQLCmd = " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId
                    + " SET " + InFieldId + " = replace(" + InFieldId + ", '\"', '') "
                   + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId
                    + " SET " + InFieldId + " = replace(" + InFieldId + ", '-', '')"
                   + " Update [RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId
                    + " SET " + InFieldId + " = replace(" + InFieldId + ", ',', '')"
                  + " ";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.CommandTimeout = 350;
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    //stpErrorText = stpErrorText + "Cancel During Correcting AMOUNT";
                    //stpReturnCode = -1;

                    //stpReferenceCode = stpErrorText;
                    CatchDetails(ex);
                    return;
                }


            //SqlString =
            //   " SELECT " + InRTN_Name + " As InputField"
            //   + " FROM [RRDM_Reconciliation_ITMX].BULK_" + InTableId
            //   + " "
            //   + " ";

            //using (SqlConnection conn =
            //              new SqlConnection(connectionString))
            //    try
            //    {
            //        conn.Open();
            //        using (SqlCommand cmd =
            //            new SqlCommand(SqlString, conn))
            //        {

            //            //  cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InLoadedAtRMCycle);

            //            // Read table 

            //            SqlDataReader rdr = cmd.ExecuteReader();

            //            while (rdr.Read())
            //            {

            //                RecordFound = true;

            //                string WInputField = (string)rdr["InputField"];


            //            }
            //            // Close Reader
            //            rdr.Close();
            //        }
            //        // Close conn
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        conn.Close();

            //        CatchDetails(ex);
            //    }

        }


        //
        //private static void CatchDetails(Exception ex)
        //{
        //    RRDMLog4Net Log = new RRDMLog4Net();

        //    StringBuilder WParameters = new StringBuilder();

        //    WParameters.Append("User : ");
        //    WParameters.Append("NotAssignYet");
        //    WParameters.Append(Environment.NewLine);

        //    WParameters.Append("ATMNo : ");
        //    WParameters.Append("NotDefinedYet");
        //    WParameters.Append(Environment.NewLine);

        //    string Logger = "RRDM4Atms";
        //    string Parameters = WParameters.ToString();

        //    Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

        //    if (Environment.UserInteractive)
        //    {
        //        System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
        //            + " . Application will be aborted! Call controller to take care. ");
        //    }
        //}

    }
}
