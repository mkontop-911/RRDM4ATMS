using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMJTMIdentificationDetailsClass
    {

        public int SeqNo;
        public string AtmNo;
        public DateTime DateLastUpdated;
        public string UserId;
     
        public string BatchID;
       
        public string ATMIPAddress;

        public string ATMMachineName;
        public bool ATMWindowsAuth;

        public string ATMAccessID;
        public string ATMAccessPassword;

        public string TypeOfJournal;
        public string SourceFileName;
        public string SourceFilePath;
        
        public string DestnFilePath;

        public DateTime FileUploadRequestDt;
   
        public DateTime FileParseEnd;

        public int ResultCode;
        public string ResultMessage;

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string SqlString; 

        // Define the data table 
        public DataTable ATMsJournalDetailsSelected = new DataTable();

        public int TotalSelected; 

        string connectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        // READ JTMIdentificationDetails to fill table 

        public void ReadJTMIdentificationDetailsToFillTable(string InMode, string InBatchID, string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode = "SingleAtm"
            // InMode = "AllAtms"
            // InMode = "Batch"

            ATMsJournalDetailsSelected = new DataTable();
            ATMsJournalDetailsSelected.Clear();

            TotalSelected = 0;


            // DATA TABLE ROWS DEFINITION 
            ATMsJournalDetailsSelected.Columns.Add("AtmNo", typeof(string));
            ATMsJournalDetailsSelected.Columns.Add("BankID", typeof(string));
            ATMsJournalDetailsSelected.Columns.Add("ATMIPAddress", typeof(string));

            ATMsJournalDetailsSelected.Columns.Add("ATMMachineName", typeof(string));
            ATMsJournalDetailsSelected.Columns.Add("ATMWindowsAuth", typeof(bool));
            ATMsJournalDetailsSelected.Columns.Add("ATMAccessID", typeof(string));

            ATMsJournalDetailsSelected.Columns.Add("ATMAccessPassword", typeof(string));
            ATMsJournalDetailsSelected.Columns.Add("TypeOfJournal", typeof(string));
            ATMsJournalDetailsSelected.Columns.Add("SourceFileName", typeof(string));

            ATMsJournalDetailsSelected.Columns.Add("SourceFilePath", typeof(string));
            ATMsJournalDetailsSelected.Columns.Add("DestnFilePath", typeof(string));
            ATMsJournalDetailsSelected.Columns.Add("Operator", typeof(string));

            if (InMode == "SingleAtm")
            {
                SqlString = "SELECT *"
                 + " FROM [dbo].[JTMIdentificationDetails] "
                 + " WHERE AtmNo = @AtmNo";
            }

            if (InMode == "AllAtms")
            {
                SqlString = "SELECT *"
                 + " FROM [dbo].[JTMIdentificationDetails] ";
            }

            if (InMode == "Batch")
            {
               SqlString = "SELECT *"
                 + " FROM [dbo].[JTMIdentificationDetails] "
                 + " WHERE BatchID = @BatchID";
            }

           

            using (SqlConnection conn = new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        if (InMode == "SingleAtm")
                        {
                            cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        }

                       
                        if (InMode == "Batch")
                        {
                            cmd.Parameters.AddWithValue("@BatchID", InBatchID);
                        }
                        

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalSelected = TotalSelected + 1; 

                            AtmNo = (string)rdr["AtmNo"];

                            DateLastUpdated = (DateTime)rdr["DateLastUpdated"];
                            UserId = (string)rdr["UserId"];

                            BatchID = (string)rdr["BatchID"];

                            ATMIPAddress = (string)rdr["ATMIPAddress"];

                            ATMMachineName = (string)rdr["ATMMachineName"];

                            ATMWindowsAuth = (bool)rdr["ATMWindowsAuth"];

                            ATMAccessID = (string)rdr["ATMAccessID"];
                            ATMAccessPassword = (string)rdr["ATMAccessPassword"];

                            TypeOfJournal = (string)rdr["TypeOfJournal"];
                            SourceFileName = (string)rdr["SourceFileName"];
                            SourceFilePath = (string)rdr["SourceFilePath"];

                            DestnFilePath = (string)rdr["DestnFilePath"];

                            FileUploadRequestDt = (DateTime)rdr["FileUploadRequestDt"];

                            FileParseEnd = (DateTime)rdr["FileParseEnd"];

                            ResultCode = (int)rdr["ResultCode"];
                            ResultMessage = (string)rdr["ResultMessage"];

                            Operator = (string)rdr["Operator"];

                            Ac.ReadAtm(AtmNo);

                            if (Ac.ActiveAtm == true)
                            {
                                DataRow RowSelected = ATMsJournalDetailsSelected.NewRow();

                                RowSelected["AtmNo"] = AtmNo;
                                RowSelected["BankID"] = Ac.BankId;
                                RowSelected["ATMIPAddress"] = ATMIPAddress;

                                RowSelected["ATMMachineName"] = ATMMachineName;
                                RowSelected["ATMWindowsAuth"] = ATMWindowsAuth;
                                RowSelected["ATMAccessID"] = ATMAccessID;

                                RowSelected["ATMAccessPassword"] = ATMAccessPassword;
                                RowSelected["TypeOfJournal"] = TypeOfJournal;
                                RowSelected["SourceFileName"] = SourceFileName;

                                RowSelected["SourceFilePath"] = SourceFilePath;
                                RowSelected["DestnFilePath"] = DestnFilePath;
                                RowSelected["Operator"] = Operator;

                                // ADD ROW
                                ATMsJournalDetailsSelected.Rows.Add(RowSelected);
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured ReadJTMIdentificationDetailsByAtmNo(string InAtmNo)........ " + ex.Message;

                }
        }

        // READ JTMIdentificationDetails by AtmNo

        public void ReadJTMIdentificationDetailsByAtmNo(string InAtmNo)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
                  + " FROM [dbo].[JTMIdentificationDetails] "
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

                            SeqNo = (int)rdr["SeqNo"];
                            AtmNo = (string)rdr["AtmNo"];
                           
                            DateLastUpdated = (DateTime)rdr["DateLastUpdated"];
                            UserId = (string)rdr["UserId"];

                            BatchID = (string)rdr["BatchID"];

                            ATMIPAddress = (string)rdr["ATMIPAddress"];

                            ATMMachineName = (string)rdr["ATMMachineName"];

                            ATMWindowsAuth = (bool)rdr["ATMWindowsAuth"];

                            ATMAccessID = (string)rdr["ATMAccessID"];
                            ATMAccessPassword = (string)rdr["ATMAccessPassword"];

                            TypeOfJournal = (string)rdr["TypeOfJournal"];
                            SourceFileName = (string)rdr["SourceFileName"];
                            SourceFilePath = (string)rdr["SourceFilePath"];
                          
                            DestnFilePath = (string)rdr["DestnFilePath"];

                            FileUploadRequestDt = (DateTime)rdr["FileUploadRequestDt"];
                           
                            FileParseEnd = (DateTime)rdr["FileParseEnd"];

                            ResultCode = (int)rdr["ResultCode"];
                            ResultMessage = (string)rdr["ResultMessage"];

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured ReadJTMIdentificationDetailsByAtmNo(string InAtmNo)........ " + ex.Message;

                }
        }


        // Insert NEW Record in JTMIdentificationDetails
        //
        public void InsertNewRecordInJTMIdentificationDetails()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [dbo].[JTMIdentificationDetails]"
                + " ([AtmNo], [UserId], "
                + " [BatchID],"
                + "[ATMIPAddress],"
                + "[ATMMachineName],[ATMWindowsAuth],[ATMAccessID],[ATMAccessPassword], "
                + "[TypeOfJournal],[SourceFileName],[SourceFilePath], [DestnFilePath], "
                + "[Operator] )"
                + " VALUES"
                + " ( @AtmNo, @UserId,"
                + "@BatchID,"
                + "@ATMIPAddress,"
                + "@ATMMachineName,@ATMWindowsAuth,@ATMAccessID,@ATMAccessPassword, "
                + "@TypeOfJournal,@SourceFileName,@SourceFilePath, @DestnFilePath,"
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

                        cmd.Parameters.AddWithValue("@BatchID", BatchID);

                        cmd.Parameters.AddWithValue("@ATMIPAddress", ATMIPAddress);

                        cmd.Parameters.AddWithValue("@ATMMachineName", ATMMachineName);
                        cmd.Parameters.AddWithValue("@ATMWindowsAuth", ATMWindowsAuth);

                        cmd.Parameters.AddWithValue("@ATMAccessID", ATMAccessID);
                        cmd.Parameters.AddWithValue("@ATMAccessPassword", ATMAccessPassword);

                        cmd.Parameters.AddWithValue("@TypeOfJournal", TypeOfJournal);
                        cmd.Parameters.AddWithValue("@SourceFileName", SourceFileName);
                        cmd.Parameters.AddWithValue("@SourceFilePath", SourceFilePath);

                        cmd.Parameters.AddWithValue("@DestnFilePath", DestnFilePath);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in InsertNewRecordInJTMIdentificationDetails().......... " + ex.Message;
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
                             + "BatchID = @BatchID,"
                             + "AtmNo = @AtmNo, ATMIPAddress = @ATMIPAddress,"
                             + "ATMMachineName = @ATMMachineName, ATMWindowsAuth = @ATMWindowsAuth, "
                             + "ATMAccessID = @ATMAccessID, ATMAccessPassword = @ATMAccessPassword, "
                             + "TypeOfJournal = @TypeOfJournal, SourceFileName = @SourceFileName, SourceFilePath = @SourceFilePath,"
                             + " DestnFilePath = @DestnFilePath, "
                             + " ResultCode = @ResultCode, "
                             + "ResultMessage = @ResultMessage, "
                             + "FileUploadRequestDt = @FileUploadRequestDt, "
                             + "  FileParseEnd = @FileParseEnd, Operator = @Operator  "
                             + " WHERE AtmNo = @AtmNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        cmd.Parameters.AddWithValue("@DateLastUpdated", DateLastUpdated);

                        cmd.Parameters.AddWithValue("@UserId", UserId);
              
                        cmd.Parameters.AddWithValue("@BatchID", BatchID);
                     
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
                    
                        cmd.Parameters.AddWithValue("@FileParseEnd", FileParseEnd);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured UpdateRecordInJTMIdentificationDetailsByAtmNo(string InAtmNo)......... " + ex.Message;

                }
        }


        // UPDATE Update Record In JTMIdentificationDetails by ATm no 
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
                             + "BatchID = @BatchID,"
                             + "AtmNo = @AtmNo, ATMIPAddress = @ATMIPAddress,"
                             + "ATMMachineName = @ATMMachineName, ATMWindowsAuth = @ATMWindowsAuth, "
                             + "ATMAccessID = @ATMAccessID, ATMAccessPassword = @ATMAccessPassword, "
                             + "TypeOfJournal = @TypeOfJournal, SourceFileName = @SourceFileName, SourceFilePath = @SourceFilePath,"
                             + " DestnFilePath = @DestnFilePath, "
                             + " ResultCode = @ResultCode, "
                             + "ResultMessage = @ResultMessage, "
                             + "FileUploadRequestDt = @FileUploadRequestDt, "
                             + "  FileParseEnd = @FileParseEnd, Operator = @Operator  "
                             + " WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);

                        cmd.Parameters.AddWithValue("@DateLastUpdated", DateLastUpdated);

                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        cmd.Parameters.AddWithValue("@BatchID", BatchID);

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

                        cmd.Parameters.AddWithValue("@FileParseEnd", FileParseEnd);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured UpdateRecordInJTMIdentificationDetailsByID()..\nThe error reads:\n" + ex.Message;

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

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //             if (rows > 0) textBoxMsg.Text = " ATMs Table UPDATED ";
                        //            else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in DeleteRecordInJTMIdentificationDetailsByAtmNo(string InAtmNo) ............. " + ex.Message;

                }
        }
    }
}
