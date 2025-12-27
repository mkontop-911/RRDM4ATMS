using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDM_LoadFiles_InGeneral_Auto : Logger
    {
        public RRDM_LoadFiles_InGeneral_Auto() : base() { }

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        // 
        public void CreateBulk_And_STD_RRDM_Tables(string InFullPath_01, string InTableId, string InDelimiter)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 
        // Read first line
        //string WorkingDIR = "C:\\RRDM\\FilePool\\NCR_FOREX\\NCR_FOREX_20191126.001";
            string BulkTable = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId;

            string Wd = "";
            if (InDelimiter == "Tap")
            {
                Wd = "\t";
            }
            if (InDelimiter == "Comma")
            {
                Wd = ",";
            }
            // FIND OUT IF PHYSICAL BULK TABLE EXIST 
            
            string SQLCmd;
            bool TableExist = false;

            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            //ArrayList FieldNamesC = new ArrayList();
            bool WithHeaders = false;

            Bg.ReadTableToGetFieldNames_Array_List(InTableId, WithHeaders,1);

            if (Bg.RecordFound== true)
            {
                TableExist = true;
            }
            else
            {
                TableExist = false;
            }
            
            // If table exist 
            // MAKE VALIDATION THAT TEXT FILE IS THE SAME FORMAT AS BEFORE
            //
            if (TableExist == true)
            {
                //
                // CHECK THAT FILE PROVIDED IS THE SAME AS THE ONE PROVIDED BEFORE
                //
               // RRDM_BULK_IST_AndOthers_Records_ALL_2 Bg = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
                ArrayList FieldNamesA = new ArrayList();
                WithHeaders = false;

                FieldNamesA = Bg.ReadTableToGetFieldNames_Array_List(InTableId, WithHeaders,1);

                int Length = FieldNamesA.Count;

                // Find other
                string lineA;
                using (StreamReader reader = new StreamReader(InFullPath_01))
                {
                    lineA = reader.ReadLine();
                }

                //string SQL_Create_BULK = "";
                int K1 = 0;
                // Split the cvs on a comma.
                //string Delimeter = '\t'; 
                if (Wd == "\t")
                {
                    string[] FileNameB = lineA.Split('\t');
                    foreach (string value in FileNameB)
                    {
                        //string Tvalue = value.Replace(" ", string.Empty);
                        //string Tvalue = value.Replace(" ", "_");
                        Bg.ReadTableToGetFieldNames(InTableId, value);
                        if (Bg.RecordFound == true)
                        {
                            K1 = K1 + 1;
                        }
                        else
                        {
                            MessageBox.Show("Not Found field ..with value.." + value);
                            ErrorFound = true;
                            return;
                        }

                    }

                    if (K1 != Length)
                    {
                        MessageBox.Show("Fields in file not equal to previous..");
                        ErrorFound = true;
                        return;
                    }
                }
                if (Wd == ",")
                {
                    string[] FileNameB = lineA.Split(',');
                    foreach (string value in FileNameB)
                    {
                        string Tvalue = value.Replace(" ", string.Empty);
                        Bg.ReadTableToGetFieldNames(InTableId, Tvalue);
                        if (Bg.RecordFound == true)
                        {
                            K1 = K1 + 1;
                        }
                        else
                        {
                            MessageBox.Show("Not Found field ..with value.." + value);
                            ErrorFound = true;
                            return;
                        }

                    }

                    if (K1 != Length)
                    {
                        MessageBox.Show("Fields in file not equal to previous..");
                        ErrorFound = true;
                        return;
                    }
                }
            }

            if (TableExist == false)
            {

                // BULK DOES NOT EXIST
                // CREATE FIRST TABLE BULK
                string line;
                using (StreamReader reader = new StreamReader(InFullPath_01))
                {
                    line = reader.ReadLine();
                }

                string SQL_Create_BULK = "";
                int K = 0;
                // Split the cvs on a comma.
                //string Delimeter = '\t'; 
                if (Wd == "\t")
                {
                    string[] parts = line.Split('\t');
                    foreach (string value in parts)
                    {
                        //string Tvalue = value.Replace(" ", string.Empty);
                        string Tvalue = value.Replace(" ", "_");
                        if (Tvalue == "") Tvalue = "Filler" + K.ToString();
                        K = K + 1;
                        if (K == 1)
                        {
                            SQL_Create_BULK = "CREATE TABLE " + BulkTable + Environment.NewLine
                                + " ( " + Environment.NewLine
                                + Tvalue + " nvarchar(1000) " + Environment.NewLine
                                ;

                        }
                        else
                        {
                            if (Tvalue != "")
                                SQL_Create_BULK = SQL_Create_BULK + "," + Tvalue + " nvarchar(1000) " + Environment.NewLine;
                        }
                    }
                }

                if (Wd == ",")
                {
                    string[] parts = line.Split(',');
                    foreach (string value in parts)
                    {
                        string Tvalue = value.Replace(" ", string.Empty);
                        if (Tvalue == "") Tvalue = "Filler" + K.ToString();
                        K = K + 1;
                        if (K == 1)
                        {
                            SQL_Create_BULK = "CREATE TABLE " + BulkTable + Environment.NewLine
                                + " ( " + Environment.NewLine
                                + Tvalue + " nvarchar(1000) " + Environment.NewLine
                                ;

                        }
                        else
                        {
                            if (Tvalue!="")
                            {
                                SQL_Create_BULK = SQL_Create_BULK + "," + Tvalue + " nvarchar(1000) " + Environment.NewLine;
                            }
                            
                            else
                            {
                                Tvalue = Tvalue; 
                            }
                        }
                    }
                }


                SQL_Create_BULK = SQL_Create_BULK + " ) ";

                // Create Table 

                using (SqlConnection conn = new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQL_Create_BULK, conn))
                        {
                            cmd.CommandTimeout = 350;  // seconds
                            cmd.ExecuteNonQuery();
                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        MessageBox.Show("Error during BULK Creation --Check Columns Names");

                        CatchDetails(ex);

                    }

                // Create second Table Bulk ALL
                // CREATE SECOND TABLE BULK ALL
                string WName = "BULK_" + InTableId + "_ALL";

                string BulkTable_ALL = "[RRDM_Reconciliation_ITMX].[dbo]." + WName;
                 
                using (StreamReader reader = new StreamReader(InFullPath_01))
                {
                    line = reader.ReadLine();
                }

                 SQL_Create_BULK = "";
                 K = 0;
                // Split the cvs on a comma.
                //string Delimeter = '\t'; 
                if (Wd == "\t")
                {
                    string[] parts2 = line.Split('\t');
                    foreach (string value in parts2)
                    {
                        string Tvalue = value.Replace(" ", string.Empty);
                        if (Tvalue == "") Tvalue = "Filler" + K.ToString();
                        K = K + 1;
                        if (K == 1)
                        {
                            SQL_Create_BULK = "CREATE TABLE " + BulkTable_ALL + Environment.NewLine
                                + " ( " + Environment.NewLine
                                + " [SeqNo] [int] IDENTITY(1,1) NOT NULL " + Environment.NewLine
                                + "," + Tvalue + " nvarchar(100) " + Environment.NewLine
                                ;

                        }
                        else
                        {
                            if (Tvalue!="")
                            SQL_Create_BULK = SQL_Create_BULK + "," + Tvalue + " nvarchar(1000) " + Environment.NewLine;
                        }
                    }
                }
                //
                // Comma delimiter 
                //
                if (Wd == ",")
                {
                    string[] parts2 = line.Split(',');
                    foreach (string value in parts2)
                    {
                        string Tvalue = value.Replace(" ", string.Empty);
                        if (Tvalue == "") Tvalue = "Filler" + K.ToString();
                        K = K + 1;
                        if (K == 1)
                        {
                            SQL_Create_BULK = "CREATE TABLE " + BulkTable_ALL + Environment.NewLine
                                + " ( " + Environment.NewLine
                                + " [SeqNo] [int] IDENTITY(1,1) NOT NULL " + Environment.NewLine
                                + "," + Tvalue + " nvarchar(100) " + Environment.NewLine
                                ;

                        }
                        else
                        {
                            if (Tvalue != "")
                                SQL_Create_BULK = SQL_Create_BULK + "," + Tvalue + " nvarchar(1000) " + Environment.NewLine;
                        }
                    }
                }



                SQL_Create_BULK = SQL_Create_BULK
                    + " , LoadedAtRMCycle int NOT NULL " + Environment.NewLine
                    + " ,CONSTRAINT [PK_"+ WName + "] PRIMARY KEY CLUSTERED " + Environment.NewLine
                + "(" + Environment.NewLine
                + "	[SeqNo] ASC " + Environment.NewLine
                 + " )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] " + Environment.NewLine
                + " ) ON [PRIMARY] " + Environment.NewLine
                    ;

                // Create Table 

                using (SqlConnection conn = new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SQL_Create_BULK, conn))
                        {
                            cmd.CommandTimeout = 350;  // seconds
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
            else
            {
                // TABLES ALREADY CREATED
               
            }
            
            //
            // Create RRDM STD TABLE If not Exists
            //

            string RRDM_STD_NewTable = "[RRDM_Reconciliation_ITMX].[dbo]." + InTableId;

            TableExist = false;

            Bg.ReadTableToGetFieldNames_Array_List(RRDM_STD_NewTable, WithHeaders,2);

            if (Bg.RecordFound == true)
            {
                TableExist = true;
            }
            else
            {
                TableExist = false;
            }
  
            if (TableExist == true)
            {
                // Do nothing
            }
            else
            {
                RRDMMatchingSourceFiles Msf = new RRDMMatchingSourceFiles();

                Msf.ReadReconcSourceFilesByFileId(InTableId); 

                if (Msf.TableStructureId == "Atms And Cards")
                {

                    // Call Store Procedure to create standard tables 
                    Create_RRDM_STD_Table_ITMX(InTableId);

                    Create_RRDM_STD_Table_MATCHED_TXNS(InTableId);
                }
                else
                {
                    // OTHER THAN ATMS AND CARDS
                    return; 
                    string RRDMSQLTable = "[RRDM_Reconciliation_ITMX].[dbo]." + InTableId;
                    // IN ITMX
                    CreateRRDM_Standard_Table_MT_103_AndOthers(Msf.TableStructureId, InTableId, RRDMSQLTable);
                    // IN MATCHED DATA BASE
                    string RRDMSQLTable_2 = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo]." + InTableId;
                    CreateRRDM_Standard_Table_MT_103_AndOthers(Msf.TableStructureId, InTableId, RRDMSQLTable_2);

                }

            }

        }
        public string SQL_Create_RRDMTable = "";
        public string SQL_Constrains = "";
        // 
        public void CreateRRDM_Standard_Table_MT_103_AndOthers(string InTableName , string InStructureNm , string InRRDMSQLTable)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Read first line
            //string WorkingDIR = "C:\\RRDM\\FilePool\\NCR_FOREX\\NCR_FOREX_20191126.001";
          
          
            // FIND OUT IF BULK EXIST 
            string SQLCmd;
            bool TableExist = false;
            
            string SQL_Create_RRDMTable = "";
            string SQL_Constrains = "";

            RRDMUniversalTableFieldsDefinition Ud = new RRDMUniversalTableFieldsDefinition();

            string WSelectionCriteria = " WHERE TableStructureId ='"+ InStructureNm + "' "; 
            Ud.ReadUniversalTableFieldsDefinitionToFillDataTable(WSelectionCriteria);

            string LineField = "";
            string SQL_ConstrainsLine = "";
            int I = 0;

            while (I <= (Ud.DataTableUniversalTableFields.Rows.Count - 1))
            {
                //    RecordFound = true;
                int SeqNo = (int)Ud.DataTableUniversalTableFields.Rows[I]["SeqNo"];
                string FieldName = (string)Ud.DataTableUniversalTableFields.Rows[I]["FieldName"];
                string FieldDBName = (string)Ud.DataTableUniversalTableFields.Rows[I]["FieldDBName"];
                string FieldType = (string)Ud.DataTableUniversalTableFields.Rows[I]["FieldType"];
                int FieldLength = (int)Ud.DataTableUniversalTableFields.Rows[I]["FieldLength"];

                if (FieldType == "Character")
                {
                    LineField = " nvarchar(" + FieldLength.ToString() + ")";

                    SQL_ConstrainsLine = " ALTER TABLE " + InRRDMSQLTable + " ADD CONSTRAINT[DF_"
                        + InTableName + "_" + FieldName + "]  DEFAULT('') FOR[" + FieldName + "]";

                }

                if (FieldType == "Date")
                {
                    LineField = " [date] ";

                    SQL_ConstrainsLine = " ALTER TABLE  " + InRRDMSQLTable + " ADD CONSTRAINT[DF_"
                       + InTableName + "_" + FieldName + "]  DEFAULT('1900-01-01') FOR[" + FieldName + "]";


                }

                if (FieldType == "Decimal")
                {
                    LineField = "  [decimal](18, 2) ";

                    SQL_ConstrainsLine = " ALTER TABLE  " + InRRDMSQLTable + " ADD CONSTRAINT[DF_"
                       + InTableName + "_" + FieldName + "]  DEFAULT((0)) FOR[" + FieldName + "]";

                }

                if (FieldType == "DateTime")
                {
                    LineField = "  datetime ";
                    SQL_ConstrainsLine = " ALTER TABLE " + InRRDMSQLTable + " ADD CONSTRAINT[DF_"
                      + InTableName + "_" + FieldName + "]  DEFAULT('1900-01-01') FOR[" + FieldName + "]";

                }

                if (FieldType == "Numeric")
                {
                    LineField = " Int ";
                    
                    SQL_ConstrainsLine = " ALTER TABLE  " + InRRDMSQLTable + " ADD CONSTRAINT[DF_"
                      + InTableName + "_" + FieldName + "]  DEFAULT((0)) FOR[" + FieldName + "]";

                }

                if (FieldType == "Boolean")
                {
                    LineField = " bit ";
                    SQL_ConstrainsLine = " ALTER TABLE  " + InRRDMSQLTable + " ADD CONSTRAINT[DF_"
                      + InTableName + "_" + FieldName + "]  DEFAULT((0)) FOR[" + FieldName + "]";

                }


                if (I==0)
                {
                    
                    SQL_Create_RRDMTable = "CREATE TABLE " + InRRDMSQLTable + Environment.NewLine
                        + " ( " + Environment.NewLine
                        + " [SeqNo] [int] IDENTITY(1,1) NOT NULL " + Environment.NewLine
                        + " ,[OriginFileName] nvarchar(100)  " + Environment.NewLine
                        + " ,[Origin] nvarchar(100) " + Environment.NewLine
                        + " ,[Operator] nvarchar(20) " + Environment.NewLine
                        + " ,[LoadedAtRMCycle] int " + Environment.NewLine
                        + "," + FieldDBName + LineField + Environment.NewLine
                        
                        ;

                }
                else
                {
                    SQL_Create_RRDMTable = SQL_Create_RRDMTable + "," + FieldDBName  +LineField + Environment.NewLine;
                }

                SQL_Constrains = SQL_Constrains + SQL_ConstrainsLine + Environment.NewLine; 

                I++; // Read Next entry of the table 
            }

            //SQL_Create_RRDMTable = SQL_Create_RRDMTable + " ) ";

            SQL_Create_RRDMTable = SQL_Create_RRDMTable
                  + " ,CONSTRAINT [PK_" + InTableName + "] PRIMARY KEY CLUSTERED " + Environment.NewLine
              + "(" + Environment.NewLine
              + "	[SeqNo] ASC " + Environment.NewLine
               + " )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] " + Environment.NewLine
              + " ) ON [PRIMARY] " + Environment.NewLine
              //+ " GO " + Environment.NewLine
             //+ " USE " [RRDM_Reconciliation_MATCHED_TXNS]" + Environment.NewLine
              + SQL_Constrains
                  ;

            // Create Table 

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQL_Create_RRDMTable, conn))
                    {
                        cmd.CommandTimeout = 350;  // seconds
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

        // LOAD FIRST BULK
        // 
        public int Counter; 
        public void LoadBulk_First_Table(string InFullPath_01, string InTableId, string InDelimiter)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            Counter = 0; 

            // Read first line
            // string WorkingDIR = "C:\\RRDM\\FilePool\\NCR_FOREX\\NCR_FOREX_20191126.001";
            string BulkTable = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId;
            string BulkTable_ALL = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId + "_ALL"; 

            string Wd = "";
            if (InDelimiter == "Tap")
            {
                Wd = "\t";
            }
            if (InDelimiter == "Comma")
            {
                Wd = ",";
            }

            // Read fields of existing table
            RRDM_BULK_IST_AndOthers_Records_ALL_2 Bio = new RRDM_BULK_IST_AndOthers_Records_ALL_2();
            
            // truncate table
            Bio.TruncateTable(BulkTable); 

            // BULK INSERT First table 

            // Bulk insert the txt file to this temporary table
           
            string SQLCmd = " BULK INSERT " + BulkTable
                          + " FROM '" + InFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'  " // MUST be examined (may be change db character set to UTF8)
                          + " ,ROWs_PER_BATCH=15000 "
                          + ",FIELDTERMINATOR = '" + Wd + "'"
                          + " ,ROWTERMINATOR = '\r\n' )"
                          ;

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        //cmd.Parameters.AddWithValue("@FullPath", InFullPath);
                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                    CatchDetails(ex);
                    return;
                }
        }
        //
        // MOVE DATA From first to SECOND BULK ALL
        // 
        public void MoveBULK_From_First_To_ALL(string InTableId, string InDelimiter, int InReconcCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            Counter = 0;
            // Read first line
            // string WorkingDIR = "C:\\RRDM\\FilePool\\NCR_FOREX\\NCR_FOREX_20191126.001";
            string BulkTable = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId;
            string BulkTable_ALL = "[RRDM_Reconciliation_ITMX].[dbo].BULK_" + InTableId + "_ALL";


            // MOVE DATA FROM FIRST TABLE TO THE SECOND ALL

            string SQLCmd =
                  "INSERT INTO " + BulkTable_ALL + Environment.NewLine
                + " Select * " + " ,@RMCycle " + " FROM " + BulkTable + Environment.NewLine
                + "   "
                ;

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {

                    conn.Open();
                    //conn.StatisticsEnabled = true;
                    using (SqlCommand cmd =
                        new SqlCommand(SQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 300;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                        //var stats = conn.RetrieveStatistics();
                        // commandExecutionTimeInMs = (long)stats["ExecutionTime"];


                    }
                    // Close conn
                    //conn.StatisticsEnabled = false;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                    CatchDetails(ex);
                    return;
                }
        }
        //
        //  CREATE_RRDM_STD_TABLE
        // DEFINITION OF FIELDS

        public void Create_RRDM_STD_Table_ITMX(string InTableId)
         {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //string TargetTable = "[RRDM_Reconciliation_ITMX].[dbo]." + InTableId;

            int ReturnCode = -1;
            
            string ErrorText = "";
            string ErrorReference = "";
            
            string SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_CREATE_RRDM_STD_TABLE_ITMX]";

            using (
                SqlConnection conn2 = new SqlConnection(connectionString))
            {
                try
                {
                    int ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));
                 
                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    //ErrorText = (string)cmd.Parameters["@ErrorText"].Value;

                    conn2.Close();

                    if (ret == 0)
                    {
                        // OK
                        RecordFound = true;
                    }
                    else
                    {
                        RecordFound = false;
                        // NOT OK
                    }
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }

        }

        public void Create_RRDM_STD_Table_MATCHED_TXNS(string InTableId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            //string TargetTable = "[RRDM_Reconciliation_ITMX].[dbo]." + InTableId;

            int ReturnCode = -1;

            string ErrorText = "";
            string ErrorReference = "";

            string SPName = "[RRDM_Reconciliation_MATCHED_TXNS].[dbo].[stp_00_CREATE_RRDM_STD_TABLE_MATCHED_TXNS]";

            using (
                SqlConnection conn2 = new SqlConnection(connectionString))
            {
                try
                {
                    int ret = -1;

                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);

                    cmd.CommandType = CommandType.StoredProcedure;

                    // the first are input parameters

                    cmd.Parameters.Add(new SqlParameter("@TableName", InTableId));

                    // the following are output parameters

                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retErrorText = new SqlParameter("@ErrorText", ErrorText);
                    retErrorText.Direction = ParameterDirection.Output;
                    retErrorText.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    //ErrorText = (string)cmd.Parameters["@ErrorText"].Value;

                    conn2.Close();

                    if (ret == 0)
                    {
                        // OK
                        RecordFound = true;
                    }
                    else
                    {
                        RecordFound = false;
                        // NOT OK
                    }
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    CatchDetails(ex);
                }
            }
        }

        //
        //  INSERT RECORS IN STANDARD TABLE 
        //
        public void Insert_Records_RRDM_STD_Table(string InFileSeqNo, string InOrigin, string InTerminalType
                                                      , string InOperator, int InReconcCycleNo, string InSQLCmd)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            Counter = 0; 
            // Instruction is in SQLCmd

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                   // conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(InSQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@OriginFileName", InFileSeqNo);
                        cmd.Parameters.AddWithValue("@Origin", InOrigin);
                        //cmd.Parameters.AddWithValue("@TerminalType", InTerminalType);
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                      //  var stats = conn.RetrieveStatistics();
                      //  commandExecutionTimeInMs = (long)stats["ExecutionTime"];
                    }
                    // Close conn
                   // conn.StatisticsEnabled = false;
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                    CatchDetails(ex);
                    return;
                }
        }
        //
        //  INSERT RECORS IN STANDARD TABLE 
        //
        public void UPDATE_FILES_With_EXTRA(string InOperator, int InReconcCycleNo, string InSQLCmd)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            int Count = 0;
            // Instruction is in SQLCmd

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.StatisticsEnabled = true;
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(InSQLCmd, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds
                        Count = cmd.ExecuteNonQuery();
                        //  var stats = conn.RetrieveStatistics();
                        //  commandExecutionTimeInMs = (long)stats["ExecutionTime"];
                    }
                    // Close conn
                    conn.StatisticsEnabled = false;
                    conn.Close();


                }
                catch (Exception ex)
                {
                    conn.StatisticsEnabled = false;
                    conn.Close();

                    CatchDetails(ex);
                    return;
                }
        }
        //
        //  CREATE_RRDM_STD_TABLE _ REVERSALS 
        //
        public void Create_RRDM_STD_Table_Reversals(string InTableId, int InReconcCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMMatchingTxns_InGeneralTables_BDC Mg = new RRDMMatchingTxns_InGeneralTables_BDC();

            string SqlString;
            int Counter; 

            // INSERT In REVERSALS for MEEZA_POS
            string FileId = InTableId;

            SqlString = "INSERT INTO "
                           + "[RRDM_Reconciliation_ITMX].[dbo].[REVERSALs_PAIRs]"
                           + "( "
                            + " [FileId] "
                             + " ,[RMCycleNo] "
                             + " ,[MatchingCateg] "
                            + " ,[TerminalId_4] "
                              + " ,[TerminalId_2] "
                             + " ,[CardNumber_4] "
                               + " ,[CardNumber_2] "

                                 + " ,[AccNo_4] "
                               + " ,[AccNo_2] "
                                 + " ,[TransDescr_4] "
                               + " ,[TransDescr_2] "
                                 + " ,[TransCurr_4] "
                               + " ,[TransCurr_2] "

                            + " ,[TransAmt_4] "
                             + " ,[TransAmt_2] "
                                + " ,[RRNumber_4] "
                              + " ,[RRNumber_2] "
                            + " ,[FullTraceno_4] "
                              + " ,[FullTraceno_2] "
                            + " ,[SeqNo_4] "
                            + " ,[SeqNo_2] "
                            + ",[ResponseCode_4] "
                             + ",[ResponseCode_2] "
                            + ",[TransDate_4] "
                            + ",[TransDate_2] "
                            + ",[TXNSRC_4] "
                             + ",[TXNSRC_2] "
                            + ",[TXNDEST_4] "
                              + ",[TXNDEST_2] "
                            + ") "
          + " SELECT "
          + "  @FileId "
          + " , @RMCycleNo "
          + " , A.MatchingCateg As MatchingCateg "
          + " , A.TerminalId As TerminalId_4, B.TerminalId As TerminalId_2 "
          + ",A.CardNumber AS CardNumber_4 ,B.CardNumber AS CardNumber_2 "
              + ",A.AccNo AS AccNo_4 ,B.AccNo AS AccNo_2 "
          + ",A.TransDescr AS TransDescr_4 ,B.TransDescr AS TransDescr_2 "
          + ",A.TransCurr AS TransCurr_4 ,B.TransCurr AS TransCurr_2 "
          + ",A.TransAmt As TransAmt_4 ,B.TransAmt As TransAmt_2 "
             + ",A.RRNumber AS RRNumber_4 ,B.RRNumber AS RRNumber_2 "
          + ",A.FullTraceno AS FullTraceno_4 ,B.FullTraceno AS FullTraceno_2 "
          + ", a.SeqNo As SeqNo_4, b.SeqNo As SeqNo_2"
          + ", a.ResponseCode As ResponseCode_4 , B.ResponseCode As ResponseCode_2 "
          + ", a.TransDate As TransDate_4,b.TransDate as TransDate_2 "
          + ", a.TXNSRC As TXNSRC_4,b.TXNSRC as TXNSRC_2 "
          + ", a.TXNDEST As TXNDEST_4,b.TXNDEST as TXNDEST_2 "
          + " FROM [RRDM_Reconciliation_ITMX].[dbo]."+InTableId+" A "
          + " LEFT JOIN [RRDM_Reconciliation_ITMX].[dbo]." + InTableId + " B "
          + " ON "
          // SIMPLE-We keep all these fields for matching purposes - some of them may not have value eg Card Number
          + " A.TerminalId = B.TerminalId " 
          + " AND A.CardNumber = B.CardNumber "
          + " AND A.TransAmt = B.TransAmt "
          + " AND A.RRNumber = B.RRNumber "
          + " AND A.Net_TransDate = B.Net_TransDate "
          + " WHERE (A.Processed = 0 AND B.Processed = 0) "
          + "  AND (A.TransType = 21 AND B.TransType = 11)  "
            + " AND A.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
       + " AND B.Comment <> 'Reversals'  "  // EXCLUDE ALREADY REVERSED
       ;

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@FileId", FileId);
                        cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);

                        cmd.CommandTimeout = 350;  // seconds
                        Counter = cmd.ExecuteNonQuery();
                    }
                    // Close conn
                    conn.Close();
                  
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                    CatchDetails(ex);
                    return;
                }

            // UPDATE REVERSALS for ORIGINAL RECORDs

            // UPDATE REVERSALS for 2 (ORIGINAL RECORD)

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // UPDATE REVERSALS for  (ORIGINAL RECORD)

            
            if (Counter>0)
            {
                // REVERSALS FOUND
                Counter = 0;

                string TableId = "[RRDM_Reconciliation_ITMX].[dbo]." + InTableId;
                int SeqNo_2;
                int SeqNo_4;

                SqlString = "SELECT * "
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[REVERSALs_PAIRs]"
                      + " WHERE RMCycleNo = @RMCycleNo and FileId = @FileId "
                      + "";

                // OPEN Connection to assist individual updatings
                SqlConnection conn2 = new SqlConnection(connectionString);
                conn2.Open();

                using (SqlConnection conn =
                              new SqlConnection(connectionString))
                    try

                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                        {

                            cmd.Parameters.AddWithValue("@FileId", FileId);
                            cmd.Parameters.AddWithValue("@RMCycleNo", InReconcCycleNo);
                            // Read table 

                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                RecordFound = true;

                                SeqNo_2 = (int)rdr["SeqNo_2"];
                                SeqNo_4 = (int)rdr["SeqNo_4"];


                                Mg.UpdateProcessedBySeqNoFromReversals(TableId, SeqNo_2, InReconcCycleNo, "Reversals", conn2);
                                Mg.UpdateProcessedBySeqNoFromReversals(TableId, SeqNo_4, InReconcCycleNo, "Reversals", conn2);

                                Counter = Counter + 2; // two instead of one 

                            }

                            // Close Reader
                            rdr.Close();
                        }

                        // Close conn
                        conn.Close();
                        conn2.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        conn2.Close();
                        ErrorFound = true;
                        ErrorOutput = ex.Message;
                        CatchDetails(ex);
                        return;
                    }
            }

        }

        // Insert Action_GL
        //
        public string NewATMDate ;
        public string ATMId;
        public int TransactionSequence;
       
        public decimal TotalNetValue;
        public string CurrencyCode;
      
        public decimal DepositAmt;
   
        public decimal LcyEquivalentTotal;
        public string NoteRate;
        public decimal CommisionTotal; 

        public string Canceled;
        public string Confirmed;


        public int Insert_FOREX_CHILD(bool InWithdrawl)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int SeqNo = 0; 
            // AccName_2 
            string cmdinsert = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[BULK_NCR_FOREX_CHILD] "
           + "   ([NewATMDate] "
         + "   ,[ATMId] "
         + "   ,[TransactionSequence] "
        
         + "   ,[TotalNetValue] "
         + "   ,[CurrencyCode] "
        
       
        + "  , DepositAmt "
        + "  , LcyEquivalentTotal "
        + "  , NoteRate "
        + "  , CommisionTotal "

         + "  ,[Canceled] "
         + "   ,[Confirmed]" 
        + ", [Withdrawl]" 
         + ") "
                    + " VALUES "
          + "   ("
          +    "@NewATMDate"
         + "    ,@ATMId "
         + "   ,@TransactionSequence "
      
         + "    ,@TotalNetValue "
         + "   ,@CurrencyCode "

         + "  , @DepositAmt "
         + "  , @LcyEquivalentTotal "
         + "  , @NoteRate "
         + "  , @CommisionTotal "
         + "   ,@Canceled"
         + "   ,@Confirmed" 
         + " , @Withdrawl"
         + ") "
         + " SELECT CAST(SCOPE_IDENTITY() AS int)";
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@NewATMDate", NewATMDate);
                        cmd.Parameters.AddWithValue("@ATMId", ATMId);
                        cmd.Parameters.AddWithValue("@TransactionSequence", TransactionSequence);

                        if (InWithdrawl == true)
                        {
                            cmd.Parameters.AddWithValue("@TotalNetValue", TotalNetValue);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@TotalNetValue", 0);
                        }

                        if (InWithdrawl == true)
                        {
                            cmd.Parameters.AddWithValue("@CurrencyCode", "EGP");

                            cmd.Parameters.AddWithValue("@DepositAmt", 0);

                            cmd.Parameters.AddWithValue("@LcyEquivalentTotal", LcyEquivalentTotal);
                            cmd.Parameters.AddWithValue("@NoteRate", 0);
                            cmd.Parameters.AddWithValue("@CommisionTotal", CommisionTotal);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CurrencyCode", CurrencyCode);

                            cmd.Parameters.AddWithValue("@DepositAmt", DepositAmt);

                            cmd.Parameters.AddWithValue("@LcyEquivalentTotal", 0);
                            cmd.Parameters.AddWithValue("@NoteRate", NoteRate);
                            cmd.Parameters.AddWithValue("@CommisionTotal", 0);
                        }
                        
                        cmd.Parameters.AddWithValue("@Canceled", Canceled);
                        cmd.Parameters.AddWithValue("@Confirmed", Confirmed);

                        cmd.Parameters.AddWithValue("@Withdrawl", InWithdrawl);

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

        //
        // READ BULK_NCR_FOREX_CHILD
        //
        
        public decimal Pre_DepositAmt;

        public void ReadFOREX_Record(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * "
                         + " FROM [RRDM_Reconciliation_ITMX].[dbo].[BULK_NCR_FOREX_CHILD] "
                         + " WHERE SeqNo = @SeqNo "
                          + "  ";

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

                            Pre_DepositAmt = (decimal)rdr["DepositAmt"];

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

        // UPDATE 

        public void UpdateFOREX_Deposit(int InSeqNo, decimal InNewDepositAmt)
        {
            int rows;

            string strUpdate = "UPDATE [RRDM_Reconciliation_ITMX].[dbo].[BULK_NCR_FOREX_CHILD] SET"
                + " DepositAmt = @DepositAmt "
                + " WHERE SeqNo=@SeqNo ";
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(strUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@DepositAmt", InNewDepositAmt);

                        rows = cmd.ExecuteNonQuery();

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
        // MOVE NCR_FOREX to IST 
        //
        public void COPY_NCR_FOREX_ToIST(string InTableId, int InReconcCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            //******************************
            // 
            // MOVE FOREX TXNS to IST 
            // stp_00_MOVE_TXNS_FROM_FOREX_TO_IST
            //******************************
            int ReturnCode = -20;
            string ProgressText = "";
            string ErrorReference = "";
            int ret = -1;

            string SPName = "[RRDM_Reconciliation_ITMX].[dbo].[stp_00_MOVE_TXNS_FROM_FOREX_TO_IST]";

            using (SqlConnection conn2 = new SqlConnection(connectionString))
            {
                try
                {
                    conn2.Open();

                    SqlCommand cmd = new SqlCommand(SPName, conn2);
                    cmd.CommandType = CommandType.StoredProcedure;
                    // the first are input parameters
                    cmd.Parameters.Add(new SqlParameter("@RMCycleNo", InReconcCycleNo));
                    // the following are output parameters
                    SqlParameter retCode = new SqlParameter("@ReturnCode", ReturnCode);
                    retCode.Direction = ParameterDirection.Output;
                    retCode.SqlDbType = SqlDbType.Int;
                    cmd.Parameters.Add(retCode);

                    SqlParameter retProgressText = new SqlParameter("@ProgressText", ProgressText);
                    retProgressText.Direction = ParameterDirection.Output;
                    retProgressText.SqlDbType = SqlDbType.NVarChar;
                    retProgressText.Size = 3000;
                    cmd.Parameters.Add(retProgressText);

                    SqlParameter retErrorReference = new SqlParameter("@ErrorReference", ErrorReference);
                    retErrorReference.Direction = ParameterDirection.Output;
                    retErrorReference.SqlDbType = SqlDbType.NVarChar;
                    retErrorReference.Size = 3000;
                    cmd.Parameters.Add(retErrorReference);

                    // execute the command
                    cmd.CommandTimeout = 500;  // seconds
                    cmd.ExecuteNonQuery(); // errors will be caught in CATCH

                    ret = (int)cmd.Parameters["@ReturnCode"].Value;
                    ProgressText = (string)cmd.Parameters["@ProgressText"].Value;

                    conn2.Close();
                }
                catch (Exception ex)
                {
                    conn2.Close();
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                    CatchDetails(ex);
                    return;
                }
            }

            if (ret == 0)
            {

                // OK
                // MessageBox.Show("Moving of data Finished ... VALID CALL" + Environment.NewLine
                //            + ProgressText);
            }
            else
            {
                // NOT OK
                MessageBox.Show("Error during copy of of FOREX to IST" + Environment.NewLine
                         + ProgressText);
            }

        }

      
    }
}


