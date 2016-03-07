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
    public class RRDMMatchedDates
    {
        // DECLARE MATCHED DATES FIELDS

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

        public string Operator; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 


        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ Next Date to find the matched one 

        public void ReadNextDate(string InOperator, string InMatchDatesCateg, DateTime InDate)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[MatchedDatesTable] "
           + " WHERE Operator = @Operator AND MatchDatesCateg = @MatchDatesCateg AND NextDate = @NextDate";

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
                            
                            BankId = (string)rdr["BankId"];
                            MatchDatesCateg = (string)rdr["MatchDatesCateg"];
                        //    Prive = (bool)rdr["Prive"];

                            NextDate = (DateTime)rdr["NextDate"];
                            NMonth = (string)rdr["NMonth"];
                            NDay = (string)rdr["NDay"];
                            NType = (string)rdr["NType"];

                            SameAs = (DateTime)rdr["SameAs"];
                            SMonth = (string)rdr["SMonth"];
                            SDay = (string)rdr["SDay"];
                            SType = (string)rdr["SType"];
                            
                            DateInsert = (DateTime)rdr["DateInsert"];

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
                    ErrorOutput = "An error occured in class Matched Dates ............. " + ex.Message;

                }
        }

        // READ MATCHED last record 
        // USE MatchDatesCateg as an identification key 
        //
        public void ReadMatchedLastDate(string InOperator, string InMatchDatesCateg)
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

                            BankId = (string)rdr["BankId"];
                            MatchDatesCateg = (string)rdr["MatchDatesCateg"];
                    //        Prive = (bool)rdr["Prive"];

                            NextDate = (DateTime)rdr["NextDate"];
                            NMonth = (string)rdr["NMonth"];
                            NDay = (string)rdr["NDay"];
                            NType = (string)rdr["NType"];

                            SameAs = (DateTime)rdr["SameAs"];
                            SMonth = (string)rdr["SMonth"];
                            SDay = (string)rdr["SDay"];
                            SType = (string)rdr["SType"];

                            DateInsert = (DateTime)rdr["DateInsert"];
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
                    ErrorOutput = "An error occured in class Matched Dates ............. " + ex.Message;

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
               + " [SameAs], [SMonth], [SDay], [SType], [DateInsert],[Operator] )" 
               + " VALUES (@BankId, @MatchDatesCateg, @NextDate, @NMonth, @NDay, @NType,"
               + " @SameAs, @SMonth, @SDay, @SType, @DateInsert,@Operator )";

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

                        cmd.Parameters.AddWithValue("@Operator", Operator);
                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    ErrorFound = true;
                    ErrorOutput = "An error occured in class Matched Dates ............. " + ex.Message;

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
                    ErrorOutput = "An error occured in class Matched Dates ............. " + ex.Message;

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
                    ErrorOutput = "An error occured in class Matched Dates ............. " + ex.Message;

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

                            BankId = (string)rdr["BankId"];
                            MatchDatesCateg = (string)rdr["MatchDatesCateg"];
                    //        Prive = (bool)rdr["Prive"];

                            NextDate = (DateTime)rdr["NextDate"];
                            NMonth = (string)rdr["NMonth"];
                            NDay = (string)rdr["NDay"];
                            NType = (string)rdr["NType"];

                            SameAs = (DateTime)rdr["SameAs"];
                            SMonth = (string)rdr["SMonth"];
                            SDay = (string)rdr["SDay"];
                            SType = (string)rdr["SType"];

                            DateInsert = (DateTime)rdr["DateInsert"];

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
                    ErrorOutput = "An error occured in class Matched Dates ............. " + ex.Message;

                }
        }
    }
}
