using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;
//using System.Windows.Forms;

namespace RRDM4ATMs
{
    public class RRDMGasParameters
    {
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

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ parameters  through RefKey  

public void ReadParametersByRefKey(string InUserId, int InRefKey)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[GasParameters] "
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
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                 //   log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                //                                                  "ReadParametersSpecificNo");
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
               + " FROM [dbo].[GasParameters] "
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

                            // Read Bank Details
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
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                  // log4net.Config.XmlConfigurator.Configure();
                 //  RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                 //                                                 "ReadParametersSpecificNo");
                }
        }

        // Read Parameter Id and Occureance Id  

        public void ReadParametersSpecificParmAndOccurance(string InUserBank, string InParamId, string InOccuranceId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[GasParameters] "
           + " WHERE BankId = @BankId AND ParamId = @ParamId AND OccuranceId = @OccuranceId ";
            
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

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Bank Details
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
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                    //    log4net.Config.XmlConfigurator.Configure();
                  //  RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                  //                                                "ReadParametersSpecificNo");
                }
        }

        // GIVE PARAMETERS NO AND GET NAME 

        public void GetParameterName(string InUserBank, string InParamId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[GasParameters] "
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                //    log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                  //                                                "GetParameterName");
                }
        }
        // Get specific through Occurance Name 
        //
        public void ReadParametersSpecificNm(string InParamId, string InOccuranceNm)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[GasParameters] "
          + " WHERE ParamId = @ParamId AND OccuranceNm = @OccuranceNm";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ParamId", InParamId);
                        cmd.Parameters.AddWithValue("@OccuranceNm", InOccuranceNm);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read Bank Details
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
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                //    log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                    //                                              "ReadParametersSpecificNm");
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

            string SqlString = "SELECT * FROM [dbo].[GasParameters] WHERE BankId = @BankId AND ParamId = @ParamId AND OpenRecord = 1 ";
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                //    log4net.Config.XmlConfigurator.Configure();
                //    RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                    //                                              "ArrayList GetParamOccurancesNm()");
                }

            return OccurancesListNm;
        }

        // GET Array List Occurance No 
        //
        public ArrayList GetParamOccurancesId(string InUserBankId)
        {
            ArrayList OccurancesListId = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM [dbo].[GasParameters] WHERE BankId = @BankId AND ParamId = @ParamId AND OpenRecord = 1 ORDER BY OccuranceId";
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                 //   log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                   //                                               "ArrayList GetParamOccurancesNo()");
                }

            return OccurancesListId;
        }

        // GET Array List Models per Supplier 
        //
        public ArrayList GetParamOccurancesRelatedNm(string InUserBankId, string InRelatedParmId, string InRelatedOccuranceId)
        {
            ArrayList OccurancesListNm = new ArrayList();

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM [dbo].[GasParameters]"
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                 //   log4net.Config.XmlConfigurator.Configure();
                  //  RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                    //                                              "ArrayList GetParamOccurancesRelatedNm");
                }

            return OccurancesListNm;
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                 //   log4net.Config.XmlConfigurator.Configure();
                 //   RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                   //                                               "ArrayList GetParametersList()");
                }

            return ParamList;
        }

        // GET DISTICT PARAMETERS TO SHOW IN COMBO 
        public ArrayList GetParametersList2(string InUserBankId)
        {
            ArrayList ParamList = new ArrayList();
            ParamList.Add("100 Start");

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                    //   log4net.Config.XmlConfigurator.Configure();
                    //   RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                    //                                               "ArrayList GetParametersList()");
                }

            return ParamList;
        }

        // 
        // UPDATE Gas Param 
        //
        public void UpdateGasParamByKey(string InUserBankID, int InRefKey)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [dbo].[GasParameters] SET "
                            + " [BankId] = @BankId,"
                            + " [ParamId] = @ParamId,[ParamNm] = @ParamNm, "
                            + " [OccuranceId] = @OccuranceId, [OccuranceNm] = @OccuranceNm, " 
                            + " [Amount] = @Amount, "
                            + " [RelatedParmId] = @RelatedParmId,[RelatedOccuranceId] = @RelatedOccuranceId, "
                            + " [DateUpdated] = @DateUpdated, [OpenRecord] = @OpenRecord, [AccessLevel] = @AccessLevel  "
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

                    //    Operator = (string)rdr["Operator"];
                        

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                //    log4net.Config.XmlConfigurator.Configure();
                  //  RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                     //                                             "UpdateGasParam");
                }

        }

        //  Gp.UpdateGasParam(Gp.ParamId, Gp.OccuranceId);
        // UPDATE Gas Param 
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
                        new SqlCommand("UPDATE [dbo].[GasParameters] SET "
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                    //    log4net.Config.XmlConfigurator.Configure();
                    //  RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                    //                                             "UpdateGasParam");
                }

        }
        // 
        // Insert Gas Param 
        //
        public void InsertGasParam(string InUserBankID, string InParamId, string InOccurranceId)
        {
         
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [dbo].[GasParameters] ([BankId],[ParamId], [ParamNm],"
                + " [OccuranceId], [OccuranceNm], [Amount], [RelatedParmId], [RelatedOccuranceId], [DateUpdated], [Operator], [AccessLevel] )" 
                + " VALUES (@BankId, @ParamId, @ParamNm,"
                + " @OccuranceId, @OccuranceNm, @Amount, @RelatedParmId, @RelatedOccuranceId, @DateUpdated , @Operator, @AccessLevel )";

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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                 //   log4net.Config.XmlConfigurator.Configure();
                   // RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                   //                                               "InsertGasParam");
                }

        }
        /*
        // DELETE PARAMETER    
        //
        public void DeleteParameterEntryByRefKey(string InUserBankID, int InRefKey)
        {

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[GasParameters] "
                            + " WHERE BankId = @BankId AND [RefKey] = @RefKey", conn))

                    {
                        cmd.Parameters.AddWithValue("@BankId", InUserBankID);
                        cmd.Parameters.AddWithValue("@RefKey", InRefKey);

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
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                //    log4net.Config.XmlConfigurator.Configure();
                //    RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                  //                                                "DeleteParameterEntry");
                }
        }
*/
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
          + " FROM [dbo].[GasParameters] "
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

                            // Read Bank Details
                            RefKey = (int)rdr["RefKey"];
                            BankId = (string)rdr["Bankid"];
                            ParamId = (string)rdr["ParamId"];
                            ParamNm = (string)rdr["ParamNm"];
                            OccuranceId = (string)rdr["OccuranceId"];
                            OccuranceNm = (string)rdr["OccuranceNm"];
                            Amount = (decimal)rdr["Amount"];
                            RelatedParmId = (string)rdr["RelatedParmId"];
                            RelatedOccuranceId = (string)rdr["RelatedOccuranceId"];
                            OpenRecord = (bool)rdr["OpenRecord"];
                            AccessLevel = (int)rdr["AccessLevel"];
                         
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
                    ErrorFound = true;
                    ErrorOutput = "An error occured in Gas Parameters Class............. " + ex.Message;
                    //    log4net.Config.XmlConfigurator.Configure();
                //    RRDM4ATMsWin.Log.ProcessException(ex, "GasParamters.cs",
                 //                                                 "ReadParametersSpecificNo");
                }
        }
    }
}
