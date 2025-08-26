using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;
using static System.Threading.Thread;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDM_THREADS : Logger
    {
        public RRDM_THREADS() : base() { }

        public int SeqNo;
        public int RMCycle; 
        public DateTime CreatedDateTime;
        public string MatchingCateg;

        public int GroupId;
        public string ReconcCategoryId;
        public DateTime FinishDateTm;

        public int StatusFromRRDM;
        public int Status;

        public DataTable ThreadsTable = new DataTable();

        public bool ErrorFound;
        public string ErrorOutput;
        public bool RecordFound;

        string SqlString; 


        string connectionString = ConfigurationManager.ConnectionStrings ["ATMSConnectionString"].ConnectionString;

        DateTime NullFutureDate = new DateTime(2050, 12, 31);

        static SemaphoreSlim _semaphore; 
        static CountdownEvent _countdown;

        private void ReadTableFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            RMCycle = (int)rdr["RMCycle"];
            CreatedDateTime = (DateTime)rdr["CreatedDateTime"];
            MatchingCateg = (string)rdr["MatchingCateg"];

            GroupId = (int)rdr["GroupId"];
            ReconcCategoryId = (string)rdr["ReconcCategoryId"];
            FinishDateTm = (DateTime)rdr["FinishDateTm"];

            StatusFromRRDM = (int)rdr["StatusFromRRDM"];
            Status = (int)rdr["Status"];
        }

        public void ReadThreadsAndFillTable(string InSelectionCriteria)
        {
            // Continue with groups of ATMs 
            ThreadsTable = new DataTable();
            ThreadsTable.Clear();

            SqlString = "SELECT * "
                    + " FROM [ATMS].[dbo].[ThreadsForMatching] "
                    + InSelectionCriteria;

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                       // sqlAdapt.SelectCommand.Parameters.AddWithValue("@MatchingCategoryId", WMatchingCateg);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(ThreadsTable);

                        // Close conn
                        conn.Close();

                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);

                }
        }

        public void PopulateThreadErrorTable(DataTable InThreadsTable)
        {
            try
            {
                int WSeqNo; 
                int  I = 0;

                // LOOP FOR GROUPS OF THIS MATCHING CATEGORY

                while (I <= (InThreadsTable.Rows.Count - 1))
                {

                    WSeqNo = (int)InThreadsTable.Rows[I]["SeqNo"];

                    string WSelectionCriteria = " WHERE SeqNo=" + WSeqNo;

                    ReadThreadBySelectionCriteria(WSelectionCriteria);

                    string WTableId = "ThreadsForMatching_Errors"; 

                    CreateThreadEntryInTable(MatchingCateg, GroupId, ReconcCategoryId, RMCycle, WTableId); 


                    I++; // Read Next entry of the table - next Group

                }
            }
            catch
            {

            }
           
        }

        //
        // Methods 
        // READ  
        // 
        //
        public void ReadThreadBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
                      + " FROM [ATMS].[dbo].[ThreadsForMatching] "
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

      

        public void CreateThreadEntryInTable(string inMatchingCateg, int inGroupId, string inReconcCategoryId, int InRMCycle, string InTableId)
        {


             ErrorFound = false;
             ErrorOutput = "";
            string cmdinsert = "INSERT INTO [ATMS].[dbo]." + InTableId +
        //   string cmdinsert = "INSERT INTO [ATMS].[dbo].[ThreadsForMatching] " +
           "(" +
             "[RMCycle]" +
             ",[CreatedDateTime]" +
           ",[MatchingCateg]" +
           ",[GroupId]" +
           ",[ReconcCategoryId]" +
           ",[FinishDateTm]" +
           ",[StatusFromRRDM]" +
           ",[Status] )" +
           "VALUES " +
           "( " +
            " @RMCycle " +
            " ,@CreatedDateTime " +
           ", @MatchingCateg " +
           ", @GroupId " +
           ", @ReconcCategoryId " +
           ", @FinishDateTm " +
            ", @StatusFromRRDM " +
           ", @Status )";
        
            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@RMCycle", InRMCycle);
                        cmd.Parameters.AddWithValue("@CreatedDateTime", DateTime.Now);

                        cmd.Parameters.AddWithValue("@MatchingCateg", inMatchingCateg);

                        cmd.Parameters.AddWithValue("@GroupId", inGroupId);

                        cmd.Parameters.AddWithValue("@ReconcCategoryId", inReconcCategoryId);

                        cmd.Parameters.AddWithValue("@FinishDateTm", NullFutureDate);

                        if (InTableId == "ThreadsForMatching_Errors")
                        {
                            cmd.Parameters.AddWithValue("@StatusFromRRDM", StatusFromRRDM); // This is the response of Thread from Cycle

                            cmd.Parameters.AddWithValue("@Status", Status); // This is the response from Tread creator. 
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@StatusFromRRDM", -1); // This is the response of Thread from Cycle

                            cmd.Parameters.AddWithValue("@Status", -1); // This is the response from Tread creator. 
                        }
                  

                       

                        cmd.ExecuteScalar();
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
        public void CreateThreads( string InOperator, string InSignedId, int InRmCycle)
        {
            // 
            int NumberOfThreads;
            RRDMGasParameters Gp = new RRDMGasParameters();
            string OccuranceId = "9"; // Multithreading 
            Gp.ReadParametersSpecificId(InOperator, "914", OccuranceId , "", ""); // 

            if (Gp.RecordFound == true)
            {
                NumberOfThreads = (int)Gp.Amount;
            }
            else
            {
                NumberOfThreads = 9; 
            }


            _semaphore= new SemaphoreSlim(NumberOfThreads);

            string sConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];

            SqlConnection oConnection;
            SqlCommand oCommand;
            DataTable oDtResults = new DataTable();

            oConnection = new SqlConnection();
            oConnection.ConnectionString = connectionString;
            oConnection.Open();

            oCommand = new SqlCommand();
            oCommand.CommandType = CommandType.Text;
            oCommand.Connection = oConnection;

           

            oCommand.CommandText = @"SELECT COUNT(*)  FROM [ATMS].dbo.[ThreadsForMatching]";
            int iNoOfThreads = (int)oCommand.ExecuteScalar();

            _countdown = new CountdownEvent(iNoOfThreads);


            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT " + 
                                    " [MatchingCateg] " +
                                    ",[GroupId]" +
                                    ",[ReconcCategoryId]" +
                                    " FROM [ATMS].[dbo].[ThreadsForMatching] ";
                   

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        //cmd.Parameters.AddWithValue("@Cut_Off_Date", InCut_Off_Date);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                           RecordFound = true;
                           var oTI = new TreadInstanceMatching((string)rdr["MatchingCateg"], (int)rdr["GroupId"], (string)rdr["ReconcCategoryId"], InOperator, InSignedId, InRmCycle);
                           var t = new Thread(() => oTI.MatchingThread());
                           t.Start();
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

            _countdown.Wait();
            _countdown.Dispose();
            //MeageBox.Show("ALL THREADS Completed"); 
        }

        class TreadInstanceMatching
        {
            private readonly string _MatchingCateg;
            private readonly int _GroupId;
            private readonly string _ReconcCategoryId;

            private readonly string _Operator;
            private readonly string _SignedId;
            private readonly int _RMCycle;


            public TreadInstanceMatching(string InMatchingCateg, int InGroupId, string InReconcCategoryId, string InOperator, string InSignedId, int InRMCycle )
            {
                _MatchingCateg = InMatchingCateg;
                _GroupId = InGroupId;
                _ReconcCategoryId = InReconcCategoryId;
                _Operator= InOperator;
                _SignedId = InSignedId;
                _RMCycle = InRMCycle;
        }
            public void MatchingThread()
            {
                _semaphore.Wait();
                RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4_MULTI Multi = new RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4_MULTI();
                Multi.Matching_FindGroupsForThisMatchingCategoryAndProcessPerGroup(_Operator, _SignedId, _MatchingCateg, _RMCycle, _GroupId, _ReconcCategoryId, false);
                _countdown.Signal();
                _semaphore.Release();

                //Update Table with relevant Ending time

                UpdateEndOfThreadMaster(_MatchingCateg, _GroupId, _ReconcCategoryId); 
            }

            public void UpdateEndOfThreadMaster(string InMatchingCateg, int InGroupId, string InReconcCategoryId)
            {
                //int rows; 
                bool ErrorFound = false;
                string ErrorOutput = "";
                string connectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;


                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand("UPDATE [ATMS].[dbo].[ThreadsForMatching] SET "
                                + " FinishDateTm = @FinishDateTm "
                                + " , Status = @Status "
                                + " WHERE [MatchingCateg] = @MatchingCateg"
                                + "   AND [GroupId] = @GroupId"
                                + "   AND [ReconcCategoryId] = @ReconcCategoryId ", conn))
                        {
                            cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                            cmd.Parameters.AddWithValue("@GroupId", InGroupId);
                            cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);

                            cmd.Parameters.AddWithValue("@FinishDateTm", DateTime.Now);

                            cmd.Parameters.AddWithValue("@Status", 1);

                            cmd.ExecuteNonQuery();

                        }
                        // Close conn
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        //CatchDetails(ex);
                    }
            }
        }

        public void UpdateEndOfThread_RRDMMatching(string InMatchingCateg, int InGroupId, string InReconcCategoryId)
        {
            //int rows; 
            bool ErrorFound = false;
            string ErrorOutput = "";
            string connectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;


            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ThreadsForMatching] SET "   
                            + " StatusFromRRDM = @StatusFromRRDM "
                            + " WHERE [MatchingCateg] = @MatchingCateg"
                            + "   AND [GroupId] = @GroupId"
                            + "   AND [ReconcCategoryId] = @ReconcCategoryId ", conn))
                    {
                        cmd.Parameters.AddWithValue("@MatchingCateg", InMatchingCateg);
                        cmd.Parameters.AddWithValue("@GroupId", InGroupId);
                        cmd.Parameters.AddWithValue("@ReconcCategoryId", InReconcCategoryId);
                        
                        cmd.Parameters.AddWithValue("@StatusFromRRDM", 1);

                        cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    //CatchDetails(ex);
                }
        }

        public void TruncateTempThreadsTable()
        {
            string SQLCmd = "TRUNCATE TABLE [ATMS].[dbo].[ThreadsForMatching] ";

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
                    return;
                }
        }
    }


}

