using System;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMMatchedDates : Logger
    {
        public RRDMMatchedDates() : base() { }

        // DECLARE MATCHED DATES FIELDS
        public int SeqNo; 
        public string BankId;
  //      public int TypeOfRepl; 
        public string MatchDatesCateg; 
        public DateTime NextDate;

        public string NMonth;
        public string NDay;
        public string NType;

        public DateTime SameAs;
        public string SMonth;
        public string SDay;
        public string SType;

        public DateTime DateInsert;

        public int Previous_Month_OR_Year; 
                    // If 1 = previous Month
                    // if 2 = previous Year

        public string Operator; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 


        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        //
        // READ RDR Fields
        //
        private void ReadRDRFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];

            BankId = (string)rdr["BankId"];
            MatchDatesCateg = (string)rdr["MatchDatesCateg"];

            NextDate = (DateTime)rdr["NextDate"];
            NMonth = (string)rdr["NMonth"];
            NDay = (string)rdr["NDay"];
            NType = (string)rdr["NType"];

            SameAs = (DateTime)rdr["SameAs"];
            SMonth = (string)rdr["SMonth"];
            SDay = (string)rdr["SDay"];
            SType = (string)rdr["SType"];

            DateInsert = (DateTime)rdr["DateInsert"];

            Previous_Month_OR_Year = (int)rdr["Previous_Month_OR_Year"];

            Operator = (string)rdr["Operator"];
        }
        //
        // READ Next Date to find the matched one of previous month
        //
        public void ReadNextDatePrevious_MONTH(string InOperator, string InMatchDatesCateg, DateTime InDate)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[MatchedDatesTable] "
          + " WHERE Operator = @Operator "
          + " AND MatchDatesCateg = @MatchDatesCateg "
          + " AND NextDate = @NextDate "
          + " AND Previous_Month_OR_Year = 1 "; // 1 for previous month

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@MatchDatesCateg", InMatchDatesCateg);
                        cmd.Parameters.AddWithValue("@NextDate", InDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadRDRFields(rdr);
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
        // READ Next Date to find the matched one of previous YEAR
        //
        public void ReadNextDatePrevious_YEAR(string InOperator, string InMatchDatesCateg, DateTime InDate)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [dbo].[MatchedDatesTable] "
          + " WHERE Operator = @Operator "
          + " AND MatchDatesCateg = @MatchDatesCateg "
          + " AND NextDate = @NextDate "
          + " AND Previous_Month_OR_Year = 2 "; // 2 for previous YEAR

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@MatchDatesCateg", InMatchDatesCateg);
                        cmd.Parameters.AddWithValue("@NextDate", InDate);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadRDRFields(rdr);

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


        // READ MATCHED last record 
        // USE MatchDatesCateg as an identification key 
        //
        public void ReadMatchedLastDate(string InOperator, string InMatchDatesCateg, int InPrevious_Month_OR_Year)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM [dbo].[MatchedDatesTable] "
                   + " WHERE   NextDate = (SELECT MAX(NextDate)  "
                   + " FROM MatchedDatesTable) AND BankId = @BankId "
                   + " AND MatchDatesCateg = @MatchDatesCateg "
                   + " AND Previous_Month_OR_Year =@Previous_Month_OR_Year ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InOperator);
                        cmd.Parameters.AddWithValue("@MatchDatesCateg", InMatchDatesCateg);
                        cmd.Parameters.AddWithValue("@Previous_Month_OR_Year", InPrevious_Month_OR_Year);
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadRDRFields(rdr);
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

        // Insert NextDate  
        //
        public void InsertNextDate(string InOperator, string InMatchDatesCateg, DateTime InDate)
        {
            
            ErrorFound = false;
            ErrorOutput = "";
            
           string cmdinsert = "INSERT INTO [dbo].[MatchedDatesTable]"
               +" ([BankId], [MatchDatesCateg], [NextDate], [NMonth], [NDay], [NType],"
               + " [SameAs], [SMonth], [SDay], [SType], [DateInsert],[Previous_Month_OR_Year],[Operator] )"
               + " VALUES (@BankId, @MatchDatesCateg, @NextDate, @NMonth, @NDay, @NType,"
               + " @SameAs, @SMonth, @SDay, @SType, @DateInsert,@Previous_Month_OR_Year,@Operator )";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        
                        cmd.Parameters.AddWithValue("@BankId", InOperator);
                        cmd.Parameters.AddWithValue("@MatchDatesCateg", InMatchDatesCateg);
                        cmd.Parameters.AddWithValue("@NextDate", InDate);
                    
                        cmd.Parameters.AddWithValue("@NMonth", NMonth);
                        cmd.Parameters.AddWithValue("@NDay", NDay);
                        cmd.Parameters.AddWithValue("@NType", NType);
                        
                        cmd.Parameters.AddWithValue("@SameAs", SameAs);
                        cmd.Parameters.AddWithValue("@SMonth", SMonth);
                        cmd.Parameters.AddWithValue("@SDay", SDay);
                        cmd.Parameters.AddWithValue("@SType", SType);

                        cmd.Parameters.AddWithValue("@DateInsert", DateInsert);

                        cmd.Parameters.AddWithValue("@Previous_Month_OR_Year", Previous_Month_OR_Year);

                        cmd.Parameters.AddWithValue("@Operator", Operator);
                     
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

        // UPDATE 
        // 
        public void UpdateNextDate(string InOperator, string InMatchDatesCateg, DateTime InDate)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE [dbo].[MatchedDatesTable] SET "
                            + "[BankId] = @BankId, [MatchDatesCateg] = @MatchDatesCateg, [NextDate] = @NextDate, "
                             + "[NMonth] = @NMonth, [NDay] = @NDay, [NType] = @NType,"
                             + "[SameAs] = @SameAs, [SMonth] = @SMonth, [SDay] = @SDay, [SType] = @SType,"
                             + "[DateInsert] = @DateInsert"
                             + "  WHERE BankId = @BankId AND MatchDatesCateg = @MatchDatesCateg AND NextDate = @NextDate", conn))

                    {
                        cmd.Parameters.AddWithValue("@BankId", InOperator);
                        cmd.Parameters.AddWithValue("@MatchDatesCateg", InMatchDatesCateg);
                        cmd.Parameters.AddWithValue("@NextDate", InDate);
                      
                        cmd.Parameters.AddWithValue("@NMonth", NMonth);
                        cmd.Parameters.AddWithValue("@NDay", NDay);
                        cmd.Parameters.AddWithValue("@NType", NType);

                        cmd.Parameters.AddWithValue("@SameAs", SameAs);
                        cmd.Parameters.AddWithValue("@SMonth", SMonth);
                        cmd.Parameters.AddWithValue("@SDay", SDay);
                        cmd.Parameters.AddWithValue("@SType", SType);

                        cmd.Parameters.AddWithValue("@DateInsert", DateInsert);


                      
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

        // DELETE Holiday   
        //
        public void DeleteNextDateEntry(string InOperator, string InMatchDatesCateg, DateTime InDate)
        {
          
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[MatchedDatesTable] "
                            + "  WHERE BankId = @BankId AND MatchDatesCateg = @MatchDatesCateg AND NextDate = @NextDate ", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InOperator);
                        cmd.Parameters.AddWithValue("@MatchDatesCateg", InMatchDatesCateg);
                        cmd.Parameters.AddWithValue("@SpecialDay", InDate);

                       
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
        // READ Number of MATCHED dates from today by Replenishement Group 
        // 
        //
        public void ReadNumberOfMatchedDate(string InOperator, string InMatchDatesCateg)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT * FROM [dbo].[MatchedDatesTable] "
                   + " WHERE   NextDate = (SELECT MAX(NextDate)  FROM MatchedDatesTable) AND BankId = @BankId AND MatchDatesCateg = @MatchDatesCateg ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", InOperator);
                        cmd.Parameters.AddWithValue("@MatchDatesCateg", InMatchDatesCateg);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadRDRFields(rdr);

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


