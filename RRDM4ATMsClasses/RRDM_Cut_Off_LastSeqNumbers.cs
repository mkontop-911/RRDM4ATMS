using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDM_Cut_Off_LastSeqNumbers : Logger
    {
        public RRDM_Cut_Off_LastSeqNumbers() : base() { }

        // For each matching category we create one record / ATM if ATMs
        // eg 
        // for ATM102
        // last trace in T24, which is category X1, is ....5253.. say
        // last trace in visa system, whiich is category X2,  is ... 5240 ... say   

        public int SeqNo;

        public string AtmNo; // 

        public DateTime Cut_Off_Date; // 

        public int RMCycle;
        public int ReplCycleNo;

        public string MatchingCateg; //    

        public int LastRecordSeqNo;
        public int LastTrace;
        public DateTime LastTraceDate;

        public bool IsSeqNoInMasterFound;
        public int SeqNoInMaster;

        public DateTime DateRecordCreated;

        public string TablePhysicalName;

        public string Operator;

        string SqlString;

        // Define the data table 
        public DataTable DataTableCutOffEntries = new DataTable();


        public int TotalSelected;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        //
        // Reader fields 
        //
        private void ReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            AtmNo = (string)rdr["AtmNo"];

            Cut_Off_Date = (DateTime)rdr["Cut_Off_Date"];

            RMCycle = (int)rdr["RMCycle"];
            ReplCycleNo = (int)rdr["ReplCycleNo"];

            MatchingCateg = (string)rdr["MatchingCateg"];

            LastRecordSeqNo = (int)rdr["LastRecordSeqNo"];
            LastTrace = (int)rdr["LastTrace"];

            LastTraceDate = (DateTime)rdr["LastTraceDate"];

            IsSeqNoInMasterFound = (bool)rdr["IsSeqNoInMasterFound"];

            SeqNoInMaster = (int)rdr["SeqNoInMaster"];

            DateRecordCreated = (DateTime)rdr["DateRecordCreated"];

            TablePhysicalName = (string)rdr["TablePhysicalName"];

            Operator = (string)rdr["Operator"];

        }

        //  String WTableId;
        //
        // READ CIT_G4S_Repl_Entries to fill table 
        //
        int SaveMode;
        public void ReadCIT_G4S_Repl_EntriesToFillDataTable(string InOperator, string InSignedId, string InSelectionCriteria, int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SaveMode = InMode;

            DataTableCutOffEntries = new DataTable();
            DataTableCutOffEntries.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            SqlString = "SELECT *"
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Cut_Off_LastSeqNumbers]"
               + InSelectionCriteria;

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        //sqlAdapt.SelectCommand.Parameters.AddWithValue("@UserId", WSignedId);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableCutOffEntries);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            //InsertWReportAtmRepl(InSignedId); 
        }

        //
        // READ CIT_G4S_Repl_Entries to fill table by ATM and Cut Off Date 
        //

        public void ReadCIT_G4S_Repl_Entries_For_Table_ByAtmNo_Cut_Off_Date(string InAtmNo, DateTime InCut_Off_Date)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";


            DataTableCutOffEntries = new DataTable();
            DataTableCutOffEntries.Clear();

            TotalSelected = 0;

            //// DATA TABLE ROWS DEFINITION 

            SqlString = "SELECT *"
               + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Cut_Off_LastSeqNumbers]"
               + " WHERE AtmNo = @AtmNo AND Cut_Off_Date = @Cut_Off_Date ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableCutOffEntries);

                    }
                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    conn.Close();
                    CatchDetails(ex);
                }

            //InsertWReportAtmRepl(InSignedId); 
        }
        //
        // READ CIT_G4S_Repl_Entries by Selection Criteria 
        //
        public void ReadCIT_G4S_Repl_EntriesBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Cut_Off_LastSeqNumbers]"
                 + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@FieldName", InFieldName);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

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
        // READ Entries by ATM No, ReplCycleNo
        //

        public void ReadCIT_G4S_Repl_EntriesByATMandDate(string InAtmNo, int InReplCycleNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                 + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Cut_Off_LastSeqNumbers]"
                 + " WHERE AtmNo = @AtmNo AND ReplCycleNo = @ReplCycleNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", InReplCycleNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            ReaderFields(rdr);

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
        // READ CIT_G4S_Repl_Entries by SequenceNumber 
        //
        public void ReadCIT_G4S_Repl_EntriesSeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                  + " FROM [RRDM_Reconciliation_ITMX].[dbo].[Cut_Off_LastSeqNumbers]"
                 + " WHERE SeqNo = @SeqNo";

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

                            ReaderFields(rdr);

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

        // Records with last traces 
        public bool NoTxns;
        public void DefineLastTraces(string InOperator, string InMatchingCategoryId, string InTableId, string InAtmNo, int WRMCycle)
        {
            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            //RRDMMatchingTxns_InGeneralTables Mgt = new RRDMMatchingTxns_InGeneralTables();
            RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();
            int LastTraceNo = 0;
            // Find details of the last record in Destination File

            LastTraceNo = Mgt.ReadAndFindMaxSeqNo(InTableId, InMatchingCategoryId, InAtmNo, 2);

            Cut_Off_Date = Mgt.TransDate.Date;

            LastRecordSeqNo = Mgt.SeqNo; // this is irrelevant Last record might not have the last SeqNo 

            LastTraceDate = Mgt.TransDate;


            if (LastTraceNo == 0)
            {
                NoTxns = true;
                return;
            }
            else
            {
                NoTxns = false;
            }

            AtmNo = InAtmNo;

            RMCycle = WRMCycle;

            MatchingCateg = InMatchingCategoryId;


            LastTrace = LastTraceNo;


            TablePhysicalName = InTableId;

            Operator = InOperator;

            // Find the corresponding record in Master table 

            string SelectionCriteria = " WHERE  MatchingCateg ='" + InMatchingCategoryId + "'"
                          + " AND  TraceNoWithNoEndZero <=" + LastTraceNo
                          + " AND  TerminalId ='" + InAtmNo
                          + "' AND IsMatchingDone = 0 "
                          + " ORDER By TraceNoWithNoEndZero Desc ";

            Mpa.ReadMatchingTxnsMasterPoolBySelectionCriteria(SelectionCriteria, 1);

            if (Mpa.RecordFound == true)
            {
                // OK 
                IsSeqNoInMasterFound = true;
                SeqNoInMaster = Mpa.SeqNo;
                ReplCycleNo = Mpa.ReplCycleNo;
            }
            else
            {
                IsSeqNoInMasterFound = false;
                SeqNoInMaster = 0;
                ReplCycleNo = 0;
                //System.Windows.Forms.MessageBox.Show("89e9e Record Not Found. Maybe Journal Not Loaded Yet.");
            }


            int Temp = InsertCut_Off_EntriesRecord();
        }

        //
        // Insert Cut Off Entries 
        //
        public int InsertCut_Off_EntriesRecord()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [RRDM_Reconciliation_ITMX].[dbo].[Cut_Off_LastSeqNumbers]"
                    + " ("
                    + " [AtmNo]"
                    + ",[Cut_Off_Date]"
                    + ",[RMCycle] "
                    + ",[ReplCycleNo] "
                    + ",[MatchingCateg] " // For each matching category we create one record 

                    + ",[LastRecordSeqNo]"
                    + ",[LastTrace]"
                    + ",[LastTraceDate]"

                    + ",[IsSeqNoInMasterFound] "
                    + ",[SeqNoInMaster]"
                    + ",[DateRecordCreated]"
                    + ",[TablePhysicalName]"

                    + ",[Operator]"
                    + ")"
                    + " VALUES "
                    + " ("
                    + " @AtmNo"
                    + " ,@Cut_Off_Date"
                    + ",@RMCycle "
                    + ",@ReplCycleNo "
                    + ",@MatchingCateg "

                    + ",@LastRecordSeqNo "
                    + ",@LastTrace "
                    + ",@LastTraceDate "

                    + ",@IsSeqNoInMasterFound "
                    + ",@SeqNoInMaster "
                    + ",@DateRecordCreated "
                    + ",@TablePhysicalName "

                    + ",@Operator "
                    + ")  "
                    + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);

                        cmd.Parameters.AddWithValue("@Cut_Off_Date", Cut_Off_Date);

                        cmd.Parameters.AddWithValue("@RMCycle", RMCycle);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);
                        cmd.Parameters.AddWithValue("@MatchingCateg", MatchingCateg);


                        cmd.Parameters.AddWithValue("@LastRecordSeqNo", LastRecordSeqNo);
                        cmd.Parameters.AddWithValue("@LastTrace", LastTrace);
                        cmd.Parameters.AddWithValue("@LastTraceDate", LastTraceDate);

                        cmd.Parameters.AddWithValue("@IsSeqNoInMasterFound", IsSeqNoInMasterFound);
                        cmd.Parameters.AddWithValue("@SeqNoInMaster", SeqNoInMaster);
                        cmd.Parameters.AddWithValue("@DateRecordCreated", DateTime.Now);
                        cmd.Parameters.AddWithValue("@TablePhysicalName", TablePhysicalName);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

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

        //
        // UPDATE Entries 
        // 
        public void UpdateCut_Off_EntriesRecordForMpa(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Cut_Off_LastSeqNumbers]"
                              + " SET "
                              + " IsSeqNoInMasterFound = @IsSeqNoInMasterFound "
                              + " ,SeqNoInMaster = @SeqNoInMaster "
                              + " ,ReplCycleNo = @ReplCycleNo "
                              + ""
                              + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@IsSeqNoInMasterFound", IsSeqNoInMasterFound);

                        cmd.Parameters.AddWithValue("@SeqNoInMaster", SeqNoInMaster);

                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

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
        // UPDATE Entries when ReplNo = Zero 
        // 
        public void UpdateCut_Off_EntriesWhenReplIsZero(string InAtmNo, int InReplCycleNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            int rows;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [RRDM_Reconciliation_ITMX].[dbo].[Cut_Off_LastSeqNumbers]"
                              + " SET "
                               + " ReplCycleNo = @ReplCycleNo "
                              + " WHERE AtmNo = @AtmNo AND ReplCycleNo = 0 ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", InReplCycleNo);

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
        // DELETE SeqNo
        //
        public void DeleteCut_Off_EntriesRecord(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[Cut_Off_LastSeqNumbers]"
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);


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
}


