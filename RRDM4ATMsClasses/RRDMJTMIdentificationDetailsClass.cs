using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Transactions;

namespace RRDM4ATMs
{
    public class RRDMJTMIdentificationDetailsClass : Logger
    {
        public RRDMJTMIdentificationDetailsClass() : base() { }

        public int SeqNo;
        public string AtmNo;
        public DateTime DateLastUpdated;
        public string UserId;
     
        public string LoadingScheduleID;
        
        public string ATMIPAddress;

        public string ATMMachineName;
        public bool ATMWindowsAuth;

        public string ATMAccessID;
        public string ATMAccessPassword;

        public string TypeOfJournal;
        public string SourceFileName;
        public string SourceFilePath;
        
        public string DestnFilePath;

        public int QueueRecId; 

        public DateTime FileUploadRequestDt;
   
        public DateTime FileParseEnd;

        public DateTime LoadingCompleted;    

        public int ResultCode;
                    // -1 Waiting for processing 
                    //  0 E-Journal Loaded by Alecos in SQL tables 
                    //  1 E-Journal updated to RRDM  
        public string ResultMessage;

        public DateTime NextLoadingDtTm;

        public string SWDCategory;
        public string SWVersion;
        public DateTime SWDate;
        public int TypeOfSWD;

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string SqlString; 

        // Define the data table 
        public DataTable ATMsJournalDetailsTable ;

        public int TotalSelected; 

        string connectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        //
        // Read Reader fields 
        //
        private void ReaderFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
            AtmNo = (string)rdr["AtmNo"];

            DateLastUpdated = (DateTime)rdr["DateLastUpdated"];
            UserId = (string)rdr["UserId"];

            LoadingScheduleID = (string)rdr["LoadingScheduleID"];

            ATMIPAddress = (string)rdr["ATMIPAddress"];

            ATMMachineName = (string)rdr["ATMMachineName"];

            ATMWindowsAuth = (bool)rdr["ATMWindowsAuth"];

            ATMAccessID = (string)rdr["ATMAccessID"];
            ATMAccessPassword = (string)rdr["ATMAccessPassword"];

            TypeOfJournal = (string)rdr["TypeOfJournal"];
            SourceFileName = (string)rdr["SourceFileName"];
            SourceFilePath = (string)rdr["SourceFilePath"];

            DestnFilePath = (string)rdr["DestnFilePath"];

            QueueRecId = (int)rdr["QueueRecId"];

            FileUploadRequestDt = (DateTime)rdr["FileUploadRequestDt"];

            FileParseEnd = (DateTime)rdr["FileParseEnd"];

            LoadingCompleted = (DateTime)rdr["LoadingCompleted"];

            ResultCode = (int)rdr["ResultCode"];
            ResultMessage = (string)rdr["ResultMessage"];

            NextLoadingDtTm = (DateTime)rdr["NextLoadingDtTm"];

            SWDCategory = (string)rdr["SWDCategory"];
            SWVersion = (string)rdr["SWVersion"];
            SWDate = (DateTime)rdr["SWDate"];
            TypeOfSWD = (int)rdr["TypeOfSWD"]; // 1: Preproduction, 2:Pilot, 3:Production, 4: Single Atm
      
            Operator = (string)rdr["Operator"];
        
        }

        // READ JTMIdentificationDetails to fill FULL table 

        public void ReadJTMIdentificationDetailsToFillFullTable(string InMode, string InAtmNo)
        {

            ATMsJournalDetailsTable = new DataTable();
            ATMsJournalDetailsTable.Clear();
            TotalSelected = 0;

            // InMode    : SingleAtm
            //           : AllReadyForLoading

            if (InMode == "SingleAtm")
            {
                SqlString = "SELECT *"
                 + " FROM [dbo].[JTMIdentificationDetails] "
                 + " WHERE AtmNo = @AtmNo";
            }

            if (InMode == "AllReadyForLoading")
            {
                SqlString = "SELECT *"
                 + " FROM [dbo].[JTMIdentificationDetails] "
                 + " WHERE NextLoadingDtTm <= @CurrentDtTm And ResultCode = 1";
            }

            if (InMode == "GetAllLoaded")
            {
                SqlString = "SELECT *"
                 + " FROM [dbo].[JTMIdentificationDetails] "
                 + " WHERE ResultCode = 0";
            }

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {
                      
                        if (InMode == "SingleAtm")
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        }
                        if (InMode == "AllReadyForLoading")
                        {
                            sqlAdapt.SelectCommand.Parameters.AddWithValue("@CurrentDtTm", DateTime.Now);
                        }

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(ATMsJournalDetailsTable);

                        // Close conn
                        conn.Close();

                    
                        RecordFound = false;
                        ErrorFound = false;

                        if (ATMsJournalDetailsTable.Rows.Count > 0)
                        {
                            RecordFound = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorFound = true ;

                    conn.Close();

                    CatchDetails(ex);                

                }
        }

        //
        // READ JTMIdentificationDetails to fill partial table 
        //

        public void ReadJTMIdentificationDetailsToFillPartialTable(string InSelectionCriteria, int inMode, DateTime InDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // inMode = 1 without date 
            // inMode = 2 with date 

            ATMsJournalDetailsTable = new DataTable();
            ATMsJournalDetailsTable.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            ATMsJournalDetailsTable.Columns.Add("ATMNo", typeof(string));
            ATMsJournalDetailsTable.Columns.Add("LoadingScheduleID", typeof(string));

            ATMsJournalDetailsTable.Columns.Add("QueueRecId", typeof(string));  
            ATMsJournalDetailsTable.Columns.Add("FileUploadRequestDt", typeof(DateTime));
            ATMsJournalDetailsTable.Columns.Add("FileParseEnd", typeof(DateTime));
            ATMsJournalDetailsTable.Columns.Add("LoadingCompleted", typeof(DateTime));
                  
            ATMsJournalDetailsTable.Columns.Add("ResultCode", typeof(int));
            ATMsJournalDetailsTable.Columns.Add("ResultMessage", typeof(string));     
            ATMsJournalDetailsTable.Columns.Add("NextLoadingDtTm", typeof(DateTime));
            ATMsJournalDetailsTable.Columns.Add("SWDCategory", typeof(string));
            ATMsJournalDetailsTable.Columns.Add("SWVersion", typeof(string));
            ATMsJournalDetailsTable.Columns.Add("SWDate", typeof(DateTime));
            ATMsJournalDetailsTable.Columns.Add("TypeOfSWD", typeof(int));
            ATMsJournalDetailsTable.Columns.Add("TypeName", typeof(string));

            SqlString = "SELECT *"
                             + " FROM [dbo].[JTMIdentificationDetails] "
                             + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (inMode == 2)
                        {
                            cmd.Parameters.AddWithValue("@Date", InDate);
                        }

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read ATMs Journal fields 

                            ReaderFields(rdr);
                            //
                            // Fill In Table
                            //
                            DataRow RowSelected = ATMsJournalDetailsTable.NewRow();
                         
                            RowSelected["ATMNo"] = AtmNo;
                            RowSelected["LoadingScheduleID"] = LoadingScheduleID;

                            RowSelected["QueueRecId"] = QueueRecId;
                            RowSelected["FileUploadRequestDt"] = FileUploadRequestDt;
                            RowSelected["FileParseEnd"] = FileParseEnd;
                            RowSelected["LoadingCompleted"] = LoadingCompleted;
                           
                            RowSelected["ResultCode"] = ResultCode;
                            RowSelected["ResultMessage"] = ResultMessage;
                            RowSelected["NextLoadingDtTm"] = NextLoadingDtTm;
                            RowSelected["SWDCategory"] = SWDCategory;
                            RowSelected["SWVersion"] = SWVersion;
                            RowSelected["SWDate"] = SWDate;
                            RowSelected["TypeOfSWD"] = TypeOfSWD;
                            if (TypeOfSWD == 1) RowSelected["TypeName"] = "Pre-Production";
                            if (TypeOfSWD == 2) RowSelected["TypeName"] = "Pilot";
                            if (TypeOfSWD == 3) RowSelected["TypeName"] = "Production";
                            // ADD ROW
                            ATMsJournalDetailsTable.Rows.Add(RowSelected);

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

        // READ JTMIdentificationDetails by AtmNo

        public void ReadJTMIdentificationDetailsByAtmNo(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                  + " FROM ATMS.[dbo].[JTMIdentificationDetails] "
                  + " WHERE AtmNo = @AtmNo";
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
                            ReaderFields(rdr);
                        }
                        ErrorFound = false;
                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                    RecordFound = false;
                    conn.Close();
                    // Alecos
                    // CatchDetails(ex);

                }
        }

        // READ JTMIdentificationDetails by SelectionCriteria

        public void ReadJTMIdentificationDetailsBySelectionCriteria(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                  + " FROM [dbo].[JTMIdentificationDetails] "

                  + InSelectionCriteria; 
               //   + " WHERE AtmNo = @AtmNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       // cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ReaderFields(rdr);
                        }
                        ErrorFound = false;
                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                    RecordFound = false;
                    conn.Close();
                    // Alecos
                    // CatchDetails(ex);

                }
        }

        // READ JTMIdentificationDetails by SWD Category and Version
        public int TotalSameSWVersion;
        public int TotalNoSameSWVersion; 
        public void ReadJTMIdentificationDetailsForSWTotals(string InSWDCategory, string InSWVersion)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSameSWVersion = 0;
            TotalNoSameSWVersion = 0;

            SqlString = "SELECT *"
                  + " FROM [dbo].[JTMIdentificationDetails] "
                  + " WHERE SWDCategory = @SWDCategory ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@SWDCategory", InSWDCategory);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReaderFields(rdr);

                            if(SWVersion == InSWVersion)
                            {
                                TotalSameSWVersion = TotalSameSWVersion + 1;                                
                            }
                            else
                            {
                                TotalNoSameSWVersion = TotalNoSameSWVersion + 1;
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

        // Insert NEW Record in JTMIdentificationDetails
        //
        int rows; 
        public void InsertNewRecordInJTMIdentificationDetails()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[JTMIdentificationDetails]"
                + " ([AtmNo], [UserId], "
                + " [LoadingScheduleID],"
                + "[ATMIPAddress],"
                + "[ATMMachineName],[ATMWindowsAuth],[ATMAccessID],[ATMAccessPassword], "
                + "[TypeOfJournal],[SourceFileName],[SourceFilePath], [DestnFilePath], "
                + "[NextLoadingDtTm], "
                + "[Operator] )"
                + " VALUES"
                + " ( @AtmNo, @UserId,"
                + "@LoadingScheduleID,"
                + "@ATMIPAddress,"
                + "@ATMMachineName,@ATMWindowsAuth,@ATMAccessID,@ATMAccessPassword, "
                + "@TypeOfJournal,@SourceFileName,@SourceFilePath, @DestnFilePath,"
                + "@NextLoadingDtTm ,"
                + " @Operator )";
            //+ " SELECT CAST(SCOPE_IDENTITY() AS int)";
           
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);

                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        cmd.Parameters.AddWithValue("@LoadingScheduleID", LoadingScheduleID);

                        cmd.Parameters.AddWithValue("@ATMIPAddress", ATMIPAddress);

                        cmd.Parameters.AddWithValue("@ATMMachineName", ATMMachineName);
                        cmd.Parameters.AddWithValue("@ATMWindowsAuth", ATMWindowsAuth);

                        cmd.Parameters.AddWithValue("@ATMAccessID", ATMAccessID);
                        cmd.Parameters.AddWithValue("@ATMAccessPassword", ATMAccessPassword);

                        cmd.Parameters.AddWithValue("@TypeOfJournal", TypeOfJournal);
                        cmd.Parameters.AddWithValue("@SourceFileName", SourceFileName);
                        cmd.Parameters.AddWithValue("@SourceFilePath", SourceFilePath);

                        cmd.Parameters.AddWithValue("@DestnFilePath", DestnFilePath);

                        cmd.Parameters.AddWithValue("@NextLoadingDtTm", NextLoadingDtTm);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        rows = cmd.ExecuteNonQuery();
                        if (rows < 1)
                        { 
                            ErrorFound = true;
                            ErrorOutput = string.Format("Record not Inserted! AtmNo = [{0}]", AtmNo);
                        }
                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                    conn.Close();
                    CatchDetails(ex);    
                }
        }


        // UPDATE Update Record In JTMIdentificationDetails by ATm no 
        // 
        public void UpdateRecordInJTMIdentificationDetailsByAtmNo(string InAtmNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE dbo.JTMIdentificationDetails SET "
                             + " DateLastUpdated = @DateLastUpdated, UserId = @UserId, "
                             + " LoadingScheduleID = @LoadingScheduleID,"
                             + " AtmNo = @AtmNo, ATMIPAddress = @ATMIPAddress,"
                             + " ATMMachineName = @ATMMachineName, ATMWindowsAuth = @ATMWindowsAuth, "
                             + " ATMAccessID = @ATMAccessID, ATMAccessPassword = @ATMAccessPassword, "
                             + " TypeOfJournal = @TypeOfJournal, SourceFileName = @SourceFileName, SourceFilePath = @SourceFilePath,"
                             + " DestnFilePath = @DestnFilePath, "
                             + " ResultCode = @ResultCode, "
                             + " ResultMessage = @ResultMessage, "
                             + " QueueRecId = @QueueRecId, "                         
                             + " FileUploadRequestDt = @FileUploadRequestDt, "
                             + " FileParseEnd = @FileParseEnd, LoadingCompleted = @LoadingCompleted,"
                             + " NextLoadingDtTm = @NextLoadingDtTm, "
                             + " SWDCategory = @SWDCategory, "
                             + " SWVersion = @SWVersion, "
                             + " SWDate = @SWDate, "
                             + " TypeOfSWD = @TypeOfSWD, "
                             + " Operator = @Operator  "
                             + " WHERE AtmNo = @AtmNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@DateLastUpdated", DateLastUpdated);

                        cmd.Parameters.AddWithValue("@UserId", UserId);
              
                        cmd.Parameters.AddWithValue("@LoadingScheduleID", LoadingScheduleID);
                     
                        cmd.Parameters.AddWithValue("@ATMIPAddress", ATMIPAddress);

                        cmd.Parameters.AddWithValue("@ATMMachineName", ATMMachineName);

                        cmd.Parameters.AddWithValue("@ATMWindowsAuth", ATMWindowsAuth);

                        cmd.Parameters.AddWithValue("@ATMAccessID", ATMAccessID);
                        cmd.Parameters.AddWithValue("@ATMAccessPassword", ATMAccessPassword);

                        cmd.Parameters.AddWithValue("@TypeOfJournal", TypeOfJournal);
                        cmd.Parameters.AddWithValue("@SourceFileName", SourceFileName);
                        cmd.Parameters.AddWithValue("@SourceFilePath", SourceFilePath);

                        cmd.Parameters.AddWithValue("@DestnFilePath", DestnFilePath);

                        cmd.Parameters.AddWithValue("@QueueRecId", QueueRecId);

                        cmd.Parameters.AddWithValue("@ResultCode", ResultCode);
                        cmd.Parameters.AddWithValue("@ResultMessage", ResultMessage);

                        cmd.Parameters.AddWithValue("@FileUploadRequestDt", FileUploadRequestDt);
                    
                        cmd.Parameters.AddWithValue("@FileParseEnd", FileParseEnd);
                        cmd.Parameters.AddWithValue("@LoadingCompleted", LoadingCompleted);

                        cmd.Parameters.AddWithValue("@NextLoadingDtTm", NextLoadingDtTm);

                        cmd.Parameters.AddWithValue("@SWDCategory", SWDCategory);
                        cmd.Parameters.AddWithValue("@SWVersion", SWVersion);
                        cmd.Parameters.AddWithValue("@SWDate", SWDate);
                        cmd.Parameters.AddWithValue("@TypeOfSWD", TypeOfSWD);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                     
                        cmd.ExecuteNonQuery();
                     

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                  
                    using (var scope2 = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew))
                        try
                        {
                            ErrorFound = true;

                            CatchDetails(ex);

                            scope2.Complete();
                   
                        }
                        catch (Exception )
                        {
                            
                        }
                        finally
                        {
                            scope2.Dispose();
                        }
                }
        }


        // UPDATE Update Record In JTMIdentificationDetails by seq no
        // 
        public void UpdateRecordInJTMIdentificationDetailsByID(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE dbo.JTMIdentificationDetails SET "
                            + " DateLastUpdated = @DateLastUpdated, UserId = @UserId, "
                             + "LoadingScheduleID = @LoadingScheduleID,"
                             + "AtmNo = @AtmNo, ATMIPAddress = @ATMIPAddress,"
                             + "ATMMachineName = @ATMMachineName, ATMWindowsAuth = @ATMWindowsAuth, "
                             + "ATMAccessID = @ATMAccessID, ATMAccessPassword = @ATMAccessPassword, "
                             + "TypeOfJournal = @TypeOfJournal, SourceFileName = @SourceFileName, SourceFilePath = @SourceFilePath,"
                             + " DestnFilePath = @DestnFilePath, "
                             + " ResultCode = @ResultCode, "
                             + "ResultMessage = @ResultMessage, "
                             + "FileUploadRequestDt = @FileUploadRequestDt, "
                             + "QueueRecId = @QueueRecId, "  
                             + "FileParseEnd = @FileParseEnd, "
                             + "LoadingCompleted = @LoadingCompleted, "
                             + " SWDCategory = @SWDCategory, "
                             + " SWVersion = @SWVersion, "
                             + "Operator = @Operator  "
                             + "WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);

                        cmd.Parameters.AddWithValue("@DateLastUpdated", DateLastUpdated);

                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        cmd.Parameters.AddWithValue("@LoadingScheduleID", LoadingScheduleID);

                        cmd.Parameters.AddWithValue("@ATMIPAddress", ATMIPAddress);

                        cmd.Parameters.AddWithValue("@ATMMachineName", ATMMachineName);

                        cmd.Parameters.AddWithValue("@ATMWindowsAuth", ATMWindowsAuth);

                        cmd.Parameters.AddWithValue("@ATMAccessID", ATMAccessID);
                        cmd.Parameters.AddWithValue("@ATMAccessPassword", ATMAccessPassword);

                        cmd.Parameters.AddWithValue("@TypeOfJournal", TypeOfJournal);
                        cmd.Parameters.AddWithValue("@SourceFileName", SourceFileName);
                        cmd.Parameters.AddWithValue("@SourceFilePath", SourceFilePath);

                        cmd.Parameters.AddWithValue("@DestnFilePath", DestnFilePath);

                        cmd.Parameters.AddWithValue("@ResultCode", ResultCode);
                        cmd.Parameters.AddWithValue("@ResultMessage", ResultMessage);

                        cmd.Parameters.AddWithValue("@FileUploadRequestDt", FileUploadRequestDt);

                        cmd.Parameters.AddWithValue("@QueueRecId", QueueRecId);

                        cmd.Parameters.AddWithValue("@FileParseEnd", FileParseEnd);

                        cmd.Parameters.AddWithValue("@LoadingCompleted", LoadingCompleted);

                        cmd.Parameters.AddWithValue("@SWVersion", SWVersion);

                        cmd.Parameters.AddWithValue("@SWDCategory", SWDCategory);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                     
                        cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    ErrorFound = true;
                    ErrorOutput = ex.Message;
                    conn.Close();
                    CatchDetails(ex);
                }
        }

        //
        // DELETE Record In JTMIdentificationDetails by ATM No
        //
        public void DeleteRecordInJTMIdentificationDetailsByAtmNo(string InAtmNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM dbo.JTMIdentificationDetails "
                            + " WHERE AtmNo = @AtmNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                      
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
