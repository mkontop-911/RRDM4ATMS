using System;
using System.Data;
using System.Text;
////using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMBank_Branches : Logger
    {
        public RRDMBank_Branches() : base() { }

        public int SeqNo;

        public string BankId;
        public string BranchId; // Branch Identity 
        
        public string BranchName;

        public string CitId; // it is filed if the Branch is the CIT Branch = eg Branch 001 has the CIT = 2000

        public string Street;
        public string Town;
        public string District;
        public string PostalCode;
        public string Country;

        public double Latitude;
        public double Longitude;

        public DateTime UpdatedDate; 

        public string Operator;

        /// <summary>
        /// 
        /// </summary>
        /// 

        // Define the data table 
        public DataTable BranchesDataTable = new DataTable();

        public int TotalSelected;

        /// <summary>
        /// 
        /// </summary>

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;
        readonly string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;
        //
        // Read Fields 
        // 
        private void BranchesReaderFields(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];
         
            BankId = (string)rdr["BankId"];

            BranchId = (string)rdr["BranchId"];
            BranchName = (string)rdr["BranchName"];

            CitId = (string)rdr["CitId"];

            Street = (string)rdr["Street"];
            Town = (string)rdr["Town"];
            District = (string)rdr["District"];

            PostalCode = (string)rdr["PostalCode"];
            Country = (string)rdr["Country"];

            Latitude = (double)rdr["Latitude"];
            Longitude = (double)rdr["Longitude"];

            UpdatedDate = (DateTime)rdr["UpdatedDate"];     

            Operator = (string)rdr["Operator"];

        }

        //
        // READ Branches  and created table as per request.
        //
        public void ReadBranchesAtmAndFillTable(string InUserId, string InSelectionCriteria)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ClearDefineTable();

            string SqlString = "SELECT *"
               + " FROM Bank_Branches"
                 + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                            new SqlCommand(SqlString, conn))
                    {
                      //  cmd.Parameters.AddWithValue("@Operator", InOperator);
                        //cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            BranchesReaderFields(rdr);

                            AddLineToTable();

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                    InsertRecordsOfReport(InUserId);

                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails_2(ex);

                }
        }
      
        // Clear Define Table
        private void ClearDefineTable()
        {
            BranchesDataTable = new DataTable();
            BranchesDataTable.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            BranchesDataTable.Columns.Add("SeqNo", typeof(int));
            BranchesDataTable.Columns.Add("Branch", typeof(string));
            BranchesDataTable.Columns.Add("BranchName", typeof(string));
            BranchesDataTable.Columns.Add("CitId", typeof(string));
            BranchesDataTable.Columns.Add("Street", typeof(string));
            BranchesDataTable.Columns.Add("Town", typeof(string));
            BranchesDataTable.Columns.Add("District", typeof(string));
            BranchesDataTable.Columns.Add("PostalCode", typeof(string));
            BranchesDataTable.Columns.Add("Country", typeof(string));

            BranchesDataTable.Columns.Add("Latitude", typeof(double));
            BranchesDataTable.Columns.Add("Longitude", typeof(double));

            BranchesDataTable.Columns.Add("E_Finance", typeof(bool));

            BranchesDataTable.Columns.Add("InternalAcno", typeof(string));
            BranchesDataTable.Columns.Add("ExternalAcno", typeof(string));
        }

        // ADD LINE TO TABLE
        RRDMAccountsClass Acc = new RRDMAccountsClass();
        private void AddLineToTable()
        {
            //
            // Fill In Table
            //

            DataRow RowSelected = BranchesDataTable.NewRow();

            RowSelected["SeqNo"] = SeqNo;
           
            RowSelected["Branch"] = BranchId;

            RowSelected["BranchName"] = BranchName;

            RowSelected["CitId"] = CitId;

            RowSelected["Street"] = Street;

            RowSelected["Town"] = Town;
            RowSelected["District"] = District;
            RowSelected["PostalCode"] = PostalCode;

            RowSelected["Country"] = Country;

            RowSelected["Latitude"] = Latitude;
            RowSelected["Longitude"] = Longitude;

            Acc.ReadAndFindAccountSpecificForNostroVostro("GL_" + BranchId, Operator); 
            if (Acc.RecordFound == true)
            {
                RowSelected["E_Finance"] = true;

                RowSelected["InternalAcno"] = Acc.AccNoInternal; 
                RowSelected["ExternalAcno"] = Acc.AccNo;
            }
            else
            {
                RowSelected["E_Finance"] = false;

                RowSelected["InternalAcno"] = "N/A";
                RowSelected["ExternalAcno"] = "N/A";
            }
            
            // ADD ROW
            BranchesDataTable.Rows.Add(RowSelected);
        }

        // Insert Records Of Report 
        private void InsertRecordsOfReport(string InUserId)
        {
            ////ReadTable And Insert In Sql Table 
            RRDMTempTablesForReportsATMS Tr = new RRDMTempTablesForReportsATMS();

            //Clear Table 
            Tr.DeleteReport76(InUserId);

            int I = 0;

            while (I <= (BranchesDataTable.Rows.Count - 1))
            {

                RecordFound = true;

                Tr.Branch = (string)BranchesDataTable.Rows[I]["Branch"];
                Tr.BranchName = (string)BranchesDataTable.Rows[I]["BranchName"];
                Tr.Street = (string)BranchesDataTable.Rows[I]["Street"];

                Tr.Town = (string)BranchesDataTable.Rows[I]["Town"];
                Tr.District = (string)BranchesDataTable.Rows[I]["District"];
                Tr.PostalCode = (string)BranchesDataTable.Rows[I]["PostalCode"];

                Tr.Country = (string)BranchesDataTable.Rows[I]["Country"];

                Tr.Latitude = (double)BranchesDataTable.Rows[I]["Latitude"];
                Tr.Longitude = (double)BranchesDataTable.Rows[I]["Longitude"];

                Tr.E_Finance = (bool)BranchesDataTable.Rows[I]["E_Finance"];
                Tr.InternalAcno = (string)BranchesDataTable.Rows[I]["InternalAcno"];
                Tr.ExternalAcno = (string)BranchesDataTable.Rows[I]["ExternalAcno"];
           
                // Insert record for printing 
                //
                Tr.InsertReport76(InUserId);

                I++; // Read Next entry of the table 

            }
        }
     
        //
        // READ Bank Branch by SeqNo
        //
        public void ReadBranchBySeqNo(int InSeqNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM Bank_Branches"
               + " WHERE SeqNo=@SeqNo";
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

                            BranchesReaderFields(rdr);

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
        // READ Bank Branch by Selection Criteria
        //
        public void ReadBranchBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM Bank_Branches"
               +  InSelectionCriteria ;
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

                            BranchesReaderFields(rdr);

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
        // READ Bank Branch by BranchId
        //
        public void ReadBranchByBranchId(string InBranchId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM Bank_Branches"
               + " WHERE BranchId=@BranchId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BranchId", InBranchId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            BranchesReaderFields(rdr);

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
        // Insert ATM
        //
        //int rows;
        public int InsertBranch(string InBranchId)
        {
            string cmdinsert = "INSERT INTO [Bank_Branches] "
                + " ( [BankId],"
                + "[BranchId],[BranchName],[CitId], "
                + "[Street],[Town],[District],[PostalCode],[Country],"
                + "[Latitude],[Longitude],"
                 + "[UpdatedDate],"
                + "[Operator])"
                + " VALUES (@BankId,"
                + "@BranchId,@BranchName, @CitId,"
                 + "@Street,@Town,@District,@PostalCode,@Country,"
                + "@Latitude,@Longitude,"
                  + "@UpdatedDate,"
                + "@Operator)"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@BranchName", BranchName);
                        cmd.Parameters.AddWithValue("@CitId", CitId);

                        cmd.Parameters.AddWithValue("@Street", Street);
                        cmd.Parameters.AddWithValue("@Town", Town);
                        cmd.Parameters.AddWithValue("@District", District);

                        cmd.Parameters.AddWithValue("@PostalCode", PostalCode);

                        cmd.Parameters.AddWithValue("@Country", Country);

                        cmd.Parameters.AddWithValue("@Latitude", Latitude);
                        cmd.Parameters.AddWithValue("@Longitude", Longitude);

                        cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
                        
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        SeqNo = cmd.ExecuteNonQuery();
                        //if (rows > 0) exception = " A NEW ATM WAS CREADED - ITs ATM No iS GIVEN AUTOMATICALLY. GO TO TABLE TO SEE IT";
                        //else exception = " Nothing WAS UPDATED ";

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

        // UPDATE 

        public void UpdateBranch(int InSeqNo)
        {
            int rows; 

            string strUpdate = "UPDATE Bank_Branches SET"
                + " BankId=@BankId,"
                + "BranchId=@BranchId, BranchName=@BranchName, CitId=@CitId,"
                + "Street=@Street, Town=@Town, District=@District, PostalCode=@PostalCode,Country=@Country,"
                + "Latitude=@Latitude,Longitude=@Longitude,"
                + "UpdatedDate=@UpdatedDate "
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
                        
                        cmd.Parameters.AddWithValue("@BankId", BankId);

                        cmd.Parameters.AddWithValue("@BranchId", BranchId);
                        cmd.Parameters.AddWithValue("@CitId", CitId);
                        cmd.Parameters.AddWithValue("@BranchName", BranchName);

                        cmd.Parameters.AddWithValue("@Street", Street);
                        cmd.Parameters.AddWithValue("@Town", Town);
                        cmd.Parameters.AddWithValue("@District", District);

                        cmd.Parameters.AddWithValue("@PostalCode", PostalCode);

                        cmd.Parameters.AddWithValue("@Country", Country);

                        cmd.Parameters.AddWithValue("@Latitude", Latitude);
                        cmd.Parameters.AddWithValue("@Longitude", Longitude);

                        cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);

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
        // DELETE Branch by SeqNo
        // DELETE ALL RELATIONS 
        //
        public void DeleteBranchBySeqNo(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    // ATM FIELDS 
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM dbo.Bank_Branches "
                            + " WHERE SeqNo = @SeqNo ", conn))
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

        // Catch details 
        private static void CatchDetails_2(Exception ex)
        {
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            WParameters.Append("Origin of Error : ");
           // WParameters.Append(InOrigin);
            WParameters.Append(Environment.NewLine);

            WParameters.Append("ATMNo : ");
            WParameters.Append("NotDefinedYet");
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);
            if (Environment.UserInteractive)
            {
                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                    + " . Application will be aborted! Call controller to take care. ");
            }
            //  Environment.Exit(0);
        }
    }
}
