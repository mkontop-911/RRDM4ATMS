using System;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Security.Principal;
//using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMGasParameters : Logger
    {
        public RRDMGasParameters() : base() { }

        public int RefKey;
        public string BankId;

        public string ParamId;
        public string ParamNm;

        public string OccuranceId;
        public string OccuranceNm;
        public decimal Amount;
        public string RelatedParmId;
        public string RelatedOccuranceId;
        public DateTime DateUpdated;
        public bool OpenRecord; 

        public string Operator;
        public int AccessLevel;

        public string UserId;
        public string MachineId;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        // Define the data table 
        public DataTable DataTableOccurancesIds = new DataTable();
        public DataTable DataTableAllParameters = new DataTable();
        public int TotalSelected;

        readonly string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ Parameters Fields
        private void ReadParametersFields(SqlDataReader rdr)
        {
            // Read prameters details 
            RefKey = (int)rdr["RefKey"];
            BankId = (string)rdr["Bankid"];
            ParamId = (string)rdr["ParamId"];
            ParamNm = (string)rdr["ParamNm"];
            OccuranceId = (string)rdr["OccuranceId"];
            OccuranceNm = (string)rdr["OccuranceNm"];
            Amount = (decimal)rdr["Amount"];
            RelatedParmId = (string)rdr["RelatedParmId"];
            RelatedOccuranceId = (string)rdr["RelatedOccuranceId"];
            DateUpdated = (DateTime)rdr["DateUpdated"];
            OpenRecord = (bool)rdr["OpenRecord"];
            Operator = (string)rdr["Operator"];
            AccessLevel = (int)rdr["AccessLevel"];

            UserId = (string)rdr["UserId"];
            MachineId = (string)rdr["MachineId"];

    }
        //
        // READ ALL PARAMETERS AND FILL UP A TABLE
        //
        public void ReadParametersAndFillDataTable(string InOperator, string InSignedId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableAllParameters = new DataTable();
            DataTableAllParameters.Clear();

            TotalSelected = 0;

            string SqlString = "SELECT *"
                  + " FROM [ATMS].[dbo].[GasParameters] "
                       + " WHERE Operator = @Operator  "
                        + " ORDER BY ParamId,  OccuranceId ";

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", InOperator);
                        
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllParameters);

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

        public string ReadParametersAndFillDataTable_101()
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableAllParameters = new DataTable();
            DataTableAllParameters.Clear();

            TotalSelected = 0;
          

            string SqlString = "SELECT *"
                  + " FROM [ATMS].[dbo].[GasParameters] "
                       + " WHERE ParamId = '101'  "
                    //    + " ORDER BY ParamId,  OccuranceId "
                        ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@BankId", InUserId);
                        //cmd.Parameters.AddWithValue("@RefKey", InRefKey);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadParametersFields(rdr);
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
            return Operator; 
        }
            
        


        //
        // READ HISTORY
        //
        public void ReadParametersAndFillDataTable_History(string InOperator, string InSignedId, string InString , int InMode)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // InMode 12 = History for parameter ID 

            DataTableAllParameters = new DataTable();
            DataTableAllParameters.Clear();

            TotalSelected = 0;

            string SqlString="";

            if (InMode == 12)
            {
             SqlString = "SELECT * "
                      + " FROM [ATMS].[dbo].[GasParameters] "
                      + " For system_time all "
                      + " WHERE ParamId = @Parameter  "
                      + " ORDER  by OccuranceId, EndTime  DESC ";
            }
            
            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Parameter", InString);

                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(DataTableAllParameters);

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


        // READ parameters  through RefKey  

        public void ReadParametersByRefKey(string InUserId, int InRefKey)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[GasParameters] "
           + " WHERE BankId = @BankId AND RefKey = @RefKey ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserId);
                        cmd.Parameters.AddWithValue("@RefKey", InRefKey);
                        
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadParametersFields(rdr);
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

        // READ parameters  through Occurance Id

        public void ReadParameterByOccuranceId(string InParamId, string InOccuranceId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[GasParameters] "
           + " WHERE ParamId = @ParamId AND OccuranceId = @OccuranceId ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ParamId", InParamId);
                        cmd.Parameters.AddWithValue("@OccuranceId", InOccuranceId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadParametersFields(rdr);
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


        // READ a line (Occurance) from parameters  through Occurance No 

        public void ReadParametersSpecificId(string InUserBank, string InParamId, string InOccuranceId, 
                                             string InRelatedParmId, string InRelatedOccuranceId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[GasParameters] "
               + " WHERE BankId = @BankId AND ParamId = @ParamId AND OccuranceId = @OccuranceId "
               + " AND RelatedParmId = @RelatedParmId AND RelatedOccuranceId =@RelatedOccuranceId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBank);
                        cmd.Parameters.AddWithValue("@ParamId", InParamId);
                        cmd.Parameters.AddWithValue("@OccuranceId", InOccuranceId);
                        cmd.Parameters.AddWithValue("@RelatedParmId", InRelatedParmId);
                        cmd.Parameters.AddWithValue("@RelatedOccuranceId", InRelatedOccuranceId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadParametersFields(rdr);

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
                    CatchDetails(ex);

                    //ErrorOutput = "We had error in System with reference=..."+ CatchDetails(ex)+Environment.NewLine
                    //    + "Please inform the RRDM Controller"
                    //    ;

                }
        }

        public void ReadParametersSpecificIdNoOperator(string InParamId, string InOccuranceId,
                                            string InRelatedParmId, string InRelatedOccuranceId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
               + " FROM [ATMS].[dbo].[GasParameters] "
               + " WHERE ParamId = @ParamId AND OccuranceId = @OccuranceId "
               + " AND RelatedParmId = @RelatedParmId AND RelatedOccuranceId =@RelatedOccuranceId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                       // cmd.Parameters.AddWithValue("@BankId", InUserBank);
                        cmd.Parameters.AddWithValue("@ParamId", InParamId);
                        cmd.Parameters.AddWithValue("@OccuranceId", InOccuranceId);
                        cmd.Parameters.AddWithValue("@RelatedParmId", InRelatedParmId);
                        cmd.Parameters.AddWithValue("@RelatedOccuranceId", InRelatedOccuranceId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadParametersFields(rdr);

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
                    CatchDetails(ex);

                    //ErrorOutput = "We had error in System with reference=..."+ CatchDetails(ex)+Environment.NewLine
                    //    + "Please inform the RRDM Controller"
                    //    ;

                }
        }

        // Read Parameter Id and Occureance Id  

        public void ReadParametersSpecificParmAndOccurance(string InUserBank, string InParamId, string InOccuranceId,
                 string InRelatedParmId, string InRelatedOccuranceId)
        {
            
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
           + " FROM [ATMS].[dbo].[GasParameters] "
           + " WHERE BankId = @BankId "
           + " AND ParamId = @ParamId AND OccuranceId = @OccuranceId AND RelatedParmId = @RelatedParmId  AND RelatedOccuranceId = @RelatedOccuranceId ";
            
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBank);
                        cmd.Parameters.AddWithValue("@ParamId", InParamId);
                        cmd.Parameters.AddWithValue("@OccuranceId", InOccuranceId);
                        cmd.Parameters.AddWithValue("@RelatedParmId", InRelatedParmId);
                        cmd.Parameters.AddWithValue("@RelatedOccuranceId", InRelatedOccuranceId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadParametersFields(rdr);

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

        // Read Para Occureance Id  from Related Parameter

        public void ReadParametersSpecificOccuranceFromRelated(string InUserBank, string InParamId, 
                 string InRelatedParmId, string InRelatedOccuranceId)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
           + " FROM [ATMS].[dbo].[GasParameters] "
           + " WHERE BankId = @BankId "
           + " AND ParamId = @ParamId AND RelatedParmId = @RelatedParmId  AND RelatedOccuranceId = @RelatedOccuranceId ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBank);
                        cmd.Parameters.AddWithValue("@ParamId", InParamId);
                        cmd.Parameters.AddWithValue("@RelatedParmId", InRelatedParmId);
                        cmd.Parameters.AddWithValue("@RelatedOccuranceId", InRelatedOccuranceId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadParametersFields(rdr);

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

        // GIVE PARAMETERS NO AND GET NAME 

        public void GetParameterName(string InUserBank, string InParamId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[GasParameters] "
           + " WHERE BankId = @BankId AND ParamId = @ParamId ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBank);
                        cmd.Parameters.AddWithValue("@ParamId", InParamId);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                           
                            ParamNm = (string)rdr["ParamNm"];

                            AccessLevel = (int)rdr["AccessLevel"];

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
        // Get specific through Occurance Name 
        //
        public void ReadParametersSpecificNm(string InUserBank, string InParamId, string InOccuranceNm)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[GasParameters] "
          + " WHERE BankId = @BankId AND ParamId = @ParamId AND OccuranceNm = @OccuranceNm";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBank);
                        cmd.Parameters.AddWithValue("@ParamId", InParamId);
                        cmd.Parameters.AddWithValue("@OccuranceNm", InOccuranceNm);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadParametersFields(rdr);

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
        public ArrayList GetParamOccurancesNm(string InUserBankId)
        {
            ArrayList OccurancesListNm = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM [ATMS].[dbo].[GasParameters] "
                +" WHERE BankId = @BankId AND ParamId = @ParamId AND OpenRecord = 1"
                +" ORDER BY OccuranceId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBankId);
                        cmd.Parameters.AddWithValue("@ParamId", ParamId);
                        
                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;
                            OccuranceNm = (string)rdr["OccuranceNm"];
                            OccurancesListNm.Add(OccuranceNm);
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

            return OccurancesListNm;
        }

        //
        // For a specific parameter get Occurances and fill table 
        //
        public void ReadAllOccurancesNmsForSpecificParDataTable(string InUserBankId, string InParamId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DataTableOccurancesIds = new DataTable();
            DataTableOccurancesIds.Clear();

            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            DataTableOccurancesIds.Columns.Add("OccuranceId", typeof(string));
            DataTableOccurancesIds.Columns.Add("OccuranceNm", typeof(string));

            string SqlString = "SELECT * FROM [ATMS].[dbo].[GasParameters] "
                             + " WHERE BankId = @BankId AND ParamId = @ParamId AND OpenRecord = 1 ORDER BY OccuranceId";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBankId);
                        cmd.Parameters.AddWithValue("@ParamId", InParamId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;
                            
                            OccuranceId = (string)rdr["OccuranceId"];
                            OccuranceNm = (string)rdr["OccuranceNm"];

                            //Fill Table 
                            DataRow RowSelected = DataTableOccurancesIds.NewRow();

                            RowSelected["OccuranceId"] = OccuranceId;
                            RowSelected["OccuranceNm"] = OccuranceNm;

                            // ADD ROW
                            DataTableOccurancesIds.Rows.Add(RowSelected);
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

        // GET Array List Occurance No 
        //
        //GetParamOccurancesIds
        public ArrayList GetArrayParamOccurancesIds(string InUserBankId)
        {
            ArrayList OccurancesListId = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM [ATMS].[dbo].[GasParameters] WHERE BankId = @BankId AND ParamId = @ParamId AND OpenRecord = 1 ORDER BY OccuranceId";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBankId);
                        cmd.Parameters.AddWithValue("@ParamId", ParamId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            OccuranceId = (string)rdr["OccuranceId"];
                            OccurancesListId.Add(OccuranceId);
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

            return OccurancesListId;
        }
        //
        // GET Unique Occurance Name of the Related 
        //
        public string GetParamOccurancesRelatedNm(string InUserBankId, string InRelatedParmId, string InRelatedOccuranceId)
        {
           
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT * FROM [ATMS].[dbo].[GasParameters]"
                + " WHERE [BankId] = @BankId AND [ParamId] = @ParamId AND [OccuranceId] = @OccuranceId AND OpenRecord = 1";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBankId);
                        cmd.Parameters.AddWithValue("@ParamId", InRelatedParmId);
                        cmd.Parameters.AddWithValue("@OccuranceId", InRelatedOccuranceId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;

                            OccuranceNm = (string)rdr["OccuranceNm"];
                           
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

            return OccuranceNm;
        }
        // GET Array List Models per Supplier 
        //
        public ArrayList GetArrayParamOccurancesRelatedNm(string InUserBankId, string InRelatedParmId, string InRelatedOccuranceId)
        {
            ArrayList OccurancesListNm = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM [ATMS].[dbo].[GasParameters]"
                + " WHERE [BankId] = @BankId AND [RelatedParmId] = @RelatedParmId AND [RelatedOccuranceId] = @RelatedOccuranceId AND OpenRecord = 1";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBankId);
                        cmd.Parameters.AddWithValue("@RelatedParmId", InRelatedParmId);
                        cmd.Parameters.AddWithValue("@RelatedOccuranceId", InRelatedOccuranceId);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;

                            OccuranceNm = (string)rdr["OccuranceNm"];
                            OccurancesListNm.Add(OccuranceNm);
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

            return OccurancesListNm;
        }
        // GET Occ No And Name for a specific parameter 
        public ArrayList GetArrayParametersOccNoAndName(string InUserBankId, string InParamId)
        {
            ArrayList OccuranceList = new ArrayList
            {
                "00 Make Selection"
            };

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string OccNoNm;

            string SqlString = "SELECT * FROM [ATMS].[dbo].[GasParameters]" 
                             + " WHERE BankId = @BankId AND ParamId = @ParamId AND OpenRecord = 1";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBankId);
                        cmd.Parameters.AddWithValue("@ParamId", InParamId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            OccuranceId = (string)rdr["OccuranceId"];
                            OccuranceNm = (string)rdr["OccuranceNm"];
                           
                            OccNoNm = OccuranceId + " " + OccuranceNm;

                            OccuranceList.Add(OccNoNm);

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

            return OccuranceList;
        }

        // GET DISTICT PARAMETERS TO SHOW IN COMBO 
        public ArrayList GetParametersList(string InUserBankId)
        {
            ArrayList ParamList = new ArrayList();
            ParamList.Add(0);

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT distinct ParamId , ParamNm FROM [ATMS].[dbo].[GasParameters] WHERE BankId = @BankId AND OpenRecord = 1";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBankId);
                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ParamId = (string)rdr["ParamId"];
                            ParamNm = (string)rdr["ParamNm"];

                            ParamList.Add(ParamId);
                          
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

            return ParamList;
        }

        // GET DISTICT PAR ID + PAR NAME TO SHOW IN COMBO 
        public ArrayList GetParametersList2(string InUserBankId)
        {
            ArrayList ParamList = new ArrayList
            {
                "100 Start"
            };

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string ParNoNm;

            string SqlString = "SELECT distinct ParamId , ParamNm FROM [ATMS].[dbo].[GasParameters] WHERE BankId = @BankId AND OpenRecord = 1";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBankId);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();


                        while (rdr.Read())
                        {
                            RecordFound = true;
                            ParamId = (string)rdr["ParamId"];
                            ParamNm = (string)rdr["ParamNm"];

                            ParNoNm = ParamId  +" " +ParamNm;

                            ParamList.Add(ParNoNm);

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

            return ParamList;
        }

        // 
        // UPDATE Gas Param by Ref Key
        //
        public void UpdateGasParamByKey(string InUserBankID, int InRefKey)
        {
            //UserId = (string)rdr["UserId"];
            //MachineId = (string)rdr["MachineId"];
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[GasParameters] SET "
                            + " [BankId] = @BankId,"
                            + " [ParamId] = @ParamId,[ParamNm] = @ParamNm, "
                            + " [OccuranceId] = @OccuranceId, [OccuranceNm] = @OccuranceNm, " 
                            + " [Amount] = @Amount, "
                            + " [RelatedParmId] = @RelatedParmId,[RelatedOccuranceId] = @RelatedOccuranceId, "
                            + " [DateUpdated] = @DateUpdated, [OpenRecord] = @OpenRecord, [AccessLevel] = @AccessLevel,  "
                            + " [UserId] = @UserId, [MachineId] = @MachineId "
                            + " WHERE BankId = @BankId AND RefKey = @RefKey", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBankID);
                        cmd.Parameters.AddWithValue("@RefKey", InRefKey);
                        cmd.Parameters.AddWithValue("@ParamId", ParamId);
                        cmd.Parameters.AddWithValue("@OccuranceId", OccuranceId);

                        cmd.Parameters.AddWithValue("@ParamNm", ParamNm);
                        cmd.Parameters.AddWithValue("@OccuranceNm", OccuranceNm);

                        cmd.Parameters.AddWithValue("@Amount", Amount);

                        cmd.Parameters.AddWithValue("@RelatedParmId", RelatedParmId);

                        cmd.Parameters.AddWithValue("@RelatedOccuranceId", RelatedOccuranceId);

                        cmd.Parameters.AddWithValue("@DateUpdated", DateTime.Now);

                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);

                        cmd.Parameters.AddWithValue("@AccessLevel", AccessLevel);

                        cmd.Parameters.AddWithValue("@UserId", WindowsIdentity.GetCurrent().Name);
                        cmd.Parameters.AddWithValue("@MachineId", Environment.MachineName);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                         //   outcome = " ATMs Table UPDATED ";
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
        // 
        // UPDATE Gas Param by ParamId
        //
        public void UpdateGasParamByParamId(string InUserBankID, string InParamId)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[GasParameters] SET "
                            + " [ParamNm] = @ParamNm "
                            + " WHERE ParamId = @ParamId", conn))
                    {
                     
                        cmd.Parameters.AddWithValue("@ParamId", ParamId);                
                        cmd.Parameters.AddWithValue("@ParamNm", ParamNm);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            //   outcome = " ATMs Table UPDATED ";
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
        // 
        // UPDATE Gas Param by ParamId and Occur
        //
        public void UpdateGasParamByParamIdAndOccur(string InUserBankID, string InParamId, 
                                                                      string InOccuranceId, string InOccuranceNm)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[GasParameters] SET "
                            + " [OccuranceNm] = @OccuranceNm "
                            + " WHERE ParamId = @ParamId AND OccuranceId = @OccuranceId ", conn))
                    {

                        cmd.Parameters.AddWithValue("@ParamId", InParamId);
                        cmd.Parameters.AddWithValue("@OccuranceId", InOccuranceId);
                        cmd.Parameters.AddWithValue("@OccuranceNm", InOccuranceNm);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            //   outcome = " ATMs Table UPDATED ";
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
        //
        //  Gp.UpdateGasParam(Gp.ParamId, Gp.OccuranceId);
        //  UPDATE Gas Param 
        //
        public void UpdateGasParam(string InUserBankID, string InParamId, string InOccuranceId)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[GasParameters] SET "
                            + " [BankId] = @BankId, "
                            + " [ParamId] = @ParamId,[ParamNm] = @ParamNm, "
                            + " [OccuranceId] = @OccuranceId, [OccuranceNm] = @OccuranceNm, "
                            + " [Amount] = @Amount, "
                            + " [RelatedParmId] = @RelatedParmId,[RelatedOccuranceId] = @RelatedOccuranceId, "
                            + " [DateUpdated] = @DateUpdated, [OpenRecord] = @OpenRecord, [AccessLevel] = @AccessLevel  "
                            + " WHERE BankId = @BankId AND ParamId = @ParamId AND OccuranceId = @OccuranceId", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBankID);
               
                        cmd.Parameters.AddWithValue("@ParamId", InParamId);
                        cmd.Parameters.AddWithValue("@ParamNm", ParamNm);
                        cmd.Parameters.AddWithValue("@OccuranceId", InOccuranceId);                     
                        cmd.Parameters.AddWithValue("@OccuranceNm", OccuranceNm);

                        cmd.Parameters.AddWithValue("@Amount", Amount);

                        cmd.Parameters.AddWithValue("@RelatedParmId", RelatedParmId);

                        cmd.Parameters.AddWithValue("@RelatedOccuranceId", RelatedOccuranceId);

                        cmd.Parameters.AddWithValue("@DateUpdated", DateTime.Now);

                        cmd.Parameters.AddWithValue("@OpenRecord", OpenRecord);

                        cmd.Parameters.AddWithValue("@AccessLevel", AccessLevel);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            //   outcome = " ATMs Table UPDATED ";
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
        // 
        // Insert Gas Param 
        //
        public void InsertGasParam(string InUserBankID, string InParamId, string InOccurranceId)
        {
         
            ErrorFound = false;
            ErrorOutput = "";
           

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[GasParameters] ([BankId],[ParamId], [ParamNm],"
                + " [OccuranceId], [OccuranceNm], [Amount], [RelatedParmId], [RelatedOccuranceId], [DateUpdated], [Operator],"
                + " [AccessLevel], [UserId], [MachineId] )"
                + " VALUES (@BankId, @ParamId, @ParamNm,"
                + " @OccuranceId, @OccuranceNm, @Amount, @RelatedParmId, @RelatedOccuranceId, @DateUpdated , @Operator, "
                + " @AccessLevel, @UserId, @MachineId )";

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@ParamId", InParamId);
                        cmd.Parameters.AddWithValue("@OccuranceId", InOccurranceId);

                        cmd.Parameters.AddWithValue("@BankId", InUserBankID);
              

                        cmd.Parameters.AddWithValue("@ParamNm", ParamNm);
                        cmd.Parameters.AddWithValue("@OccuranceNm", OccuranceNm);

                        cmd.Parameters.AddWithValue("@Amount", Amount);

                        cmd.Parameters.AddWithValue("@RelatedParmId", RelatedParmId);

                        cmd.Parameters.AddWithValue("@RelatedOccuranceId", RelatedOccuranceId);

                        cmd.Parameters.AddWithValue("@DateUpdated", DateTime.Now);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        cmd.Parameters.AddWithValue("@AccessLevel", AccessLevel);

                        cmd.Parameters.AddWithValue("@UserId", WindowsIdentity.GetCurrent().Name);
                        cmd.Parameters.AddWithValue("@MachineId", Environment.MachineName);

                        // Execute and check success 
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            //   outcome = " ATMs Table UPDATED ";
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
        //
        // DELETE PARAMETER    
        //
        public void DeleteParameterEntryByRefKey(int InRefKey)
        {

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[GasParameters] "
                            + " WHERE [RefKey] = @RefKey ", conn))

                    {
                        cmd.Parameters.AddWithValue("@RefKey", InRefKey);

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
        // CREATE PARAMETERS IN TEXT
        //
        public string CopyParameters_To_TEXT_Delimiter_File(string InOperator, string InSignedId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            TotalSelected = 0;

            string DateNow = DateTime.Now.ToString();

            //string CreatedFile = "C:\\RRDM\\Working\\TAB_File_Parmeters_"+ DateNow + ".txt";
            string CreatedFile = "C:\\RRDM\\W_New_Parameters_Loading\\ALL_Parameters.txt";

            //RRDMAccountsClass Acc = new RRDMAccountsClass();
            //RRDMActions_Occurances Aoc = new RRDMActions_Occurances();

            string SqlString =
             
              " SELECT * "
            // + " SELECT UniqueRecordId  "
            //+ " , TerminalId  "
            //+ " , TransDescr  "
            //+ " , CardNumber  "
            //+ " , AccNumber  "
            //+ " , TransCurr  "
            //+ " , TransAmount  "
            //+ " , TransDate  "
            //+ " , TraceNoWithNoEndZero  "
            //   + " , RRNumber  "
            //+ " , MatchMask  "
            //+ " , RMCateg  "
            //+ " , TXNSRC  "
            //+ " , TXNDEST  "
            + " FROM [ATMS].[dbo].GasParameters "
                + " WHERE Operator = @Operator " 
            + "  ";

            StreamWriter outputFile = new StreamWriter(@CreatedFile);

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {

                       
                           cmd.Parameters.AddWithValue("@Operator", InOperator);
                       

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;
                          
                            ReadParametersFields(rdr);

                            string strline = "";

                            TotalSelected = TotalSelected + 1;

                          
                            if (TotalSelected == 1)
                            {
                                // Make headings
                                strline = "RefKey" + "\t"
                                                   + "BankId" + "\t"
                                                   + "ParamId" + "\t"
                                                   + "ParamNm" + "\t"
                                                   + "OccuranceId" + "\t"
                                                   + "OccuranceNm" + "\t"
                                                   + "Amount" + "\t"
                                                   + "RelatedParmId" + "\t"
                                                   + "RelatedOccuranceId" + "\t"
                                                   + "DateUpdated" + "\t"
                                                   + "OpenRecord" + "\t"
                                                   + "Operator" + "\t"
                                                    + "AccessLevel";
                                outputFile.WriteLine(strline);

                            }

                            strline = RefKey + "\t"
                                                   + BankId + "\t"
                                                   + ParamId + "\t"
                                                   + ParamNm + "\t"
                                                   + OccuranceId + "\t"
                                                   + OccuranceNm + "\t"
                                                   + Amount + "\t"
                                                   + RelatedParmId + "\t"
                                                   + RelatedOccuranceId + "\t"
                                                   + DateUpdated + "\t"
                                                   + OpenRecord + "\t"
                                                   + Operator + "\t"
                                                   + AccessLevel;
                            outputFile.WriteLine(strline);

                        }

                        outputFile.Close();
                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    RecordFound = false;
                    conn.Close();

                    CatchDetails(ex);

                }
            return CreatedFile;
        }




        //
        // Move parameters from UAT to production    
        //
        public void MoveParametersFromUAT_To_Production(string InOperator, string InFullPath)
        {
            int Counter = 0; 
            string WOperator = InOperator;

            string WFullPath_01 = InFullPath ;

            // Truncate BULK Table 

            string SQLCmd = "TRUNCATE TABLE [ATMS].[dbo].W_BULK_Parameters";

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

            // Bulk insert the txt file to this temporary table

            SQLCmd = " BULK INSERT [ATMS].[dbo].W_BULK_Parameters" 
                          //  + InTableB
                          + " FROM '" + WFullPath_01.Trim() + "'"
                          + " WITH (FIRSTROW=2,TABLOCK,CODEPAGE ='1253'   " // MUST be examined (may be change db character set to UTF8)
                          + " ,ROWs_PER_BATCH=15000 "
                          + ",FIELDTERMINATOR = '\t'"
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

                        Counter = cmd.ExecuteNonQuery();
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

            // Message With records inserted
            MessageBox.Show("Number of records in input file are:"+ Counter.ToString());

            // CHECK IF COMING FROM UAT
            //RecordFound = false;
            //ErrorFound = false;
            //ErrorOutput = "";

            Counter = 0; 

           

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // CHECK FOR ACTIVE DIRECTORY

            string SqlString = " SELECT * "
                           + " FROM [ATMS].[dbo].[W_BULK_Parameters] "
                           + "  WHERE ParamId = '264' AND OccuranceId = '1' AND OccuranceNm = 'NO' ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                           //ReadParametersFields(rdr);
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

                    return; 
                }

            if (RecordFound == true)
            {
                // It is OK 
                // ACTIVE DIRECTORY
                // 
                ErrorFound = false;
                ErrorOutput = "";

                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd =
                            new SqlCommand(" "
                                + " UPDATE [ATMS].[dbo].[W_BULK_Parameters] "
                                + " SET[OccuranceNm] = 'YES' "
                                + " WHERE ParamId = '264' AND OccuranceId = 1 AND OccuranceNm = 'NO' ", conn))
                        {
                            //cmd.Parameters.AddWithValue("@BankId", InUserBankID);

                            // Execute and check success 
                            int rows = cmd.ExecuteNonQuery();
                            if (rows > 0)
                            {
                                //   outcome = " ATMs Table UPDATED ";
                            }

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
            else
            {
                
                MessageBox.Show("The parameters say that Active Directory is..="+OccuranceNm
                    + "It should be NO as a prove that comes from UAT."
                    );

                return; 
            }

            // Truncate Parameters 

            SQLCmd = "TRUNCATE TABLE [ATMS].[dbo].GasParameters";

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

            // INSERT Parameters from Bulk
            //
            Counter = 0; 

            SQLCmd =
                //"SET dateformat dmy "
                " INSERT INTO [ATMS].[dbo].GasParameters"
                + "( "
                 + " [BankId] "
                 + " ,[ParamId] "
                  + " ,[ParamNm] "
                 + " ,[OccuranceId], [OccuranceNm], [Amount], [RelatedParmId] "
                 + " ,[RelatedOccuranceId], [DateUpdated], [Operator], [AccessLevel]"
                 +" ) "
                 
                 + " SELECT "
                + " [BankId] "
                 + " ,[ParamId] "
                  + " ,[ParamNm] "
                 + " ,[OccuranceId], [OccuranceNm], [Amount], [RelatedParmId] "
                 + " ,[RelatedOccuranceId]"
                 + " ,CAST(DateUpdated aS datetime) "
                 +", [Operator], [AccessLevel]"
                 + " FROM [ATMS].[dbo].W_BULK_Parameters"
                 + " ORDER BY ParamId, OccuranceId  "
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
                    CatchDetails(ex);
                }

            MessageBox.Show("Number of records inerted In Parameters are:" + Counter.ToString());

        }

        // 
        // READ parameters  and Create new for other Bank 
        // This is used by the Grand Master User 
        //

        public void CopyParameters(string InBankA, string InBankB )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [ATMS].[dbo].[GasParameters] "
           + " WHERE BankId = @BankId ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InBankA);


                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadParametersFields(rdr);
                          
                            // INSERT GAS PARAMETER WITH DIFFERENT BANK 
                            BankId = InBankB;
                            Operator = BankId; 
                        
                            InsertGasParam(BankId, ParamId, OccuranceId);

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


    }
}
