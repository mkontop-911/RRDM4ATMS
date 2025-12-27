using System;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;

//using System.Windows.Forms;
//using System.Drawing;

namespace RRDM4ATMs
{
    public class RRDMArchiveCycles : Logger
    {
        public RRDMArchiveCycles() : base() { }

        public int SeqNo;

        public string Description;

        public DateTime FromDateTm;
        public DateTime ToDateTm;

        public int Status; // 1 : Just Created Cycle
                           // 2 : Instance SQL Server Created and Data Bases are restored
                           // 3 : Extra Records are deleted from Archive
                           // 4 : Extra Records are deleted from production => Cycle is completed 

        public DateTime DateCreated;

        public int RecordsInMaster; // How many records in Master file falls withing the Archive range 

        public DateTime DateRecordsDeleted;

        public int DeletedMasterRecords;

        public string Executable;

        public string SQLInstance;

        public string UserId; 

        public string Operator;

        // Define the data table 
        public DataTable TableArchiveCycles = new DataTable();

        public int TotalSelected;

        string SqlString; // Do not delete 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        // Read Table Fields
        private void ReadTableFields(SqlDataReader rdr)
        {
            try
            {
                SeqNo = (int)rdr["SeqNo"];

                Description = (string)rdr["Description"];

                FromDateTm = (DateTime)rdr["FromDateTm"];
                ToDateTm = (DateTime)rdr["ToDateTm"];

                Status = (int)rdr["Status"];

                DateCreated = (DateTime)rdr["DateCreated"];
                RecordsInMaster = (int)rdr["RecordsInMaster"];

                DateRecordsDeleted = (DateTime)rdr["DateRecordsDeleted"];
                DeletedMasterRecords = (int)rdr["DeletedMasterRecords"];

                Executable = (string)rdr["Executable"];
                SQLInstance = (string)rdr["SQLInstance"];
                
                UserId = (string)rdr["UserId"];

                Operator = (string)rdr["Operator"];
            }
            catch (Exception ex)
            { 
                CatchDetails(ex);
            }
          
        }

        //
        // Methods 
        // READ ITMXArchiveCycles
        // FILL UP A TABLE
        //
        public void ReadArchiveCyclesFillTable(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TableArchiveCycles = new DataTable();
            TableArchiveCycles.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 

            TableArchiveCycles.Columns.Add("Cycle", typeof(int));

            TableArchiveCycles.Columns.Add("Description", typeof(string));

            TableArchiveCycles.Columns.Add("Status", typeof(string));

            TableArchiveCycles.Columns.Add("FromDateTm", typeof(string));
            TableArchiveCycles.Columns.Add("ToDateTm", typeof(string));

            TableArchiveCycles.Columns.Add("Executable", typeof(string));
            
            TableArchiveCycles.Columns.Add("SQLInstance", typeof(string));

            TableArchiveCycles.Columns.Add("Date Created", typeof(string));
            TableArchiveCycles.Columns.Add("Records InMaster", typeof(int));

            TableArchiveCycles.Columns.Add("DateRecords Deleted", typeof(string));
            TableArchiveCycles.Columns.Add("Deleted Records", typeof(int));

            TableArchiveCycles.Columns.Add("UserId", typeof(string));
         
            SqlString = "SELECT *"
                    + " FROM [ATMS].[dbo].[ArchiveCycles] "
                    + " WHERE Operator = @Operator "
                    + " ORDER BY SeqNo DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                     
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                           
                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

                            DataRow RowSelected = TableArchiveCycles.NewRow();

                            RowSelected["Cycle"] = SeqNo;

                            RowSelected["Description"] = Description;

                            if (Status == 1 )
                            {
                                RowSelected["Status"] = "NotDoneYet-Cycle Just Oppened";
                            }
                            if (Status == 2)
                            {
                                RowSelected["Status"] = "Data Bases Are Created";
                            }
                            if (Status == 3)
                            {
                                RowSelected["Status"] = "Archive Extra Records Deleted";
                            }
                            if (Status == 4)
                            {
                                RowSelected["Status"] = "Active to be used Cycle"; // Deleted from production 
                            }

                            RowSelected["FromDateTm"] = FromDateTm.ToShortDateString();

                            RowSelected["ToDateTm"] = ToDateTm.ToShortDateString();

                            RowSelected["Executable"] = Executable;
                            RowSelected["SQLInstance"] = SQLInstance;
                           
                            RowSelected["Date Created"] = DateCreated.ToString();
                            RowSelected["Records InMaster"] = RecordsInMaster.ToString();

                            if (DateRecordsDeleted == NullPastDate.Date)
                            {
                                RowSelected["DateRecords Deleted"] = "Not Done" ;
                            }
                            else
                            {
                                RowSelected["DateRecords Deleted"] = DateRecordsDeleted.ToString();
                            }
                                
                            RowSelected["Deleted Records"] = DeletedMasterRecords.ToString();

                            RowSelected["UserId"] = UserId;

                            // ADD ROW
                            TableArchiveCycles.Rows.Add(RowSelected);

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
        // READ ArchiveCycles to find the latest one 
        // 
        //
        public void ReadLastReconcArchiveCycle(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ArchiveCycles] "
                      + " WHERE Operator = @Operator"
                      + " ORDER BY SeqNo DESC";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

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

    
        //
        // Methods 
        // READ ArchiveCycles
        // 
        //
        public void ReadArchiveCyclesByOperator(string InOperator)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ArchiveCycles] "
                      + " WHERE Operator = @Operator "
                      + " ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            TotalSelected = TotalSelected + 1;

                            ReadTableFields(rdr);

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
        // Methods 
        // READ  
        // 
        //
        public void ReadArchiveCyclesById(string InOperator, int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                      + " FROM [ATMS].[dbo].[ArchiveCycles] "
                      + " WHERE Operator = @Operator AND SeqNo = @SeqNo ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
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
        // Methods 
        // READ  Production Master POOL
        // LESS OR EQUAL
        //
        public int ReadNumberOfRecordsInPoolLessThan(string InOperator, DateTime InDate, int inMode, int InArchiveCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (inMode == 1)
            {
                connectionString = AppConfig.GetConnectionString("ReconConnectionString");
            }
            if (inMode == 2)
            {
                connectionString = AppConfig.Configuration.GetSection("ConnectionStrings")["ReconConnectionString_"+InArchiveCycle.ToString()];
            }

            int NumberOfRecords = 0;

            SqlString = "select COUNT(SeqNo)  As NumberOfRecords "
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                      + " WHERE Operator = @Operator AND TransDate <= @TransDate ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@TransDate", InDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            NumberOfRecords = (int)rdr["NumberOfRecords"];

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
            return NumberOfRecords; 
        }

        public int ReadNumberOfRecordsInPoolGreaterThan(string InOperator, DateTime InDate, int inMode, int InArchiveCycle)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            if (inMode == 1)
            {
                connectionString = AppConfig.GetConnectionString("ReconConnectionString");
            }
            if (inMode == 2)
            {
                connectionString = AppConfig.Configuration.GetSection("ConnectionStrings")["ReconConnectionString_" + InArchiveCycle.ToString()];
            }

            int NumberOfRecords = 0;

            SqlString = "select COUNT(SeqNo)  As NumberOfRecords "
                      + " FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                      + " WHERE Operator = @Operator AND TransDate <= @TransDate ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@TransDate", InDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            NumberOfRecords = (int)rdr["NumberOfRecords"];

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
            return NumberOfRecords;
        }

        // Insert NewArchiveCycle
        public int InsertNewArchiveCycle()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[ArchiveCycles] "
                + " ( "
                + " [Description],"
                   + " [FromDateTm],"
                + " [ToDateTm],"
                     + " [Status],"
                + " [DateCreated],"
                 + " [RecordsInMaster],"
                + " [UserId],"
                + " [Operator] )"
                + " VALUES"
                + " ( "
                + " @Description,"
                  + " @FromDateTm,"
                + " @ToDateTm,"
                 + " @Status,"
                + " @DateCreated,"
                 + " @RecordsInMaster,"
                + " @UserId,"
                + " @Operator ) ;"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";
            //+ " SELECT MsgID  = CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@Description", Description);
                        
                        cmd.Parameters.AddWithValue("@FromDateTm", FromDateTm);
                        cmd.Parameters.AddWithValue("@ToDateTm", ToDateTm);

                        cmd.Parameters.AddWithValue("@Status", Status);

                        cmd.Parameters.AddWithValue("@DateCreated", DateCreated);
                        cmd.Parameters.AddWithValue("@RecordsInMaster", RecordsInMaster);

                        cmd.Parameters.AddWithValue("@UserId", UserId);
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

        // UPDATE  ArchiveCycle
        // 
        public void UpdateArchiveCycle_1(int InSeqNo)
        {
            //int rows; 
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ArchiveCycles] SET "
                            + " Executable = @Executable, "
                            + " SQLInstance = @SQLInstance, "
                            + " Status = @Status, "
                            + " UserId = @UserId  "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                    
                        cmd.Parameters.AddWithValue("@Executable", Executable);
                        cmd.Parameters.AddWithValue("@SQLInstance", SQLInstance);

                        cmd.Parameters.AddWithValue("@Status", Status);

                        cmd.Parameters.AddWithValue("@UserId", UserId);
                     
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

        // UPDATE  ArchiveCycle
        // 
        public void UpdateArchiveCycle_2(int InSeqNo)
        {
            //int rows; 
            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[ArchiveCycles] SET "
                            + " DateRecordsDeleted = @DateRecordsDeleted, "
                            + " DeletedMasterRecords = @DeletedMasterRecords, "
                            + " Status = @Status, "
                            + " UserId = @UserId  "
                            + " WHERE SeqNo = @SeqNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@DateRecordsDeleted", DateRecordsDeleted);

                        cmd.Parameters.AddWithValue("@DeletedMasterRecords", DeletedMasterRecords);

                        cmd.Parameters.AddWithValue("@Status", Status);

                        cmd.Parameters.AddWithValue("@UserId", UserId);

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

        //
        // DELETE Archive Cycle 
        //
        public void DeleteArchiveCycle(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[ArchiveCycles] "
                            + " WHERE SeqNo =  @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        //rows number of record got updated

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

        //
        // DELETE Archive Cycle Extra Records With Date Greater Than 
        //
        
        public int DeleteArchiveCycleExtraRecords(int InArchiveCycle, DateTime InDate)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int DeletedRecords = 0; 

            connectionString = AppConfig.Configuration.GetSection("ConnectionStrings")["ReconConnectionString_" + InArchiveCycle.ToString()];

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                            + " WHERE TransDate > @TransDate ", conn))
                    {
                      
                        cmd.Parameters.AddWithValue("@TransDate", InDate);

                        //rows number of record got updated

                        DeletedRecords = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }

                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return DeletedRecords; 
        }

        //
        // DELETE Archive Cycle Extra Records With Date Greater Than 
        //

        public int DeleteProductionExtraRecords(string InOperator, DateTime InDate)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int DeletedRecords = 0;

            connectionString = AppConfig.GetConnectionString("ReconConnectionString");

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [RRDM_Reconciliation_ITMX].[dbo].[tblMatchingTxnsMasterPoolATMs] "
                            + " WHERE Operator = @Operator AND TransDate <= @TransDate ", conn))
                    {

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@TransDate", InDate);

                        //rows number of record got updated

                        DeletedRecords = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }

                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }
            return DeletedRecords;
        }

    }
}


