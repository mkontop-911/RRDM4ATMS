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

namespace RRDM4ATMsWin
{
    class RRDMHolidays
    {
        // DECLARE HOLIDAY FIELDS
 
        public string BankId;
        public int Year;
        public string HolidaysVersion; 
        public DateTime SpecialDay; 
   
        public bool IsHoliday;
        public string SpecialDescr;
        public bool DiffDayEveryYear;
        public DateTime LastYearSpecial; 
        public int SpecialId; 

        public bool IsNormal;
        public bool IsWeekend;
        public bool IsSpecialday;

        public DateTime NextWorkingDt;
        public bool NextWorkingDtFound; 

        public int daysInYear;

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        // READ Holiday 

        public void ReadSpecificDate( string InOperator, DateTime InDate, string InHolidaysVersion)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            IsNormal = false; IsWeekend = false; IsSpecialday = false; 
            int InYear = InDate.Year;

            int SeqDay = (int)InDate.DayOfWeek;

            if (SeqDay < 6 & SeqDay > 0) IsNormal = true;
            else IsNormal = false;

            if (SeqDay == 6 || SeqDay == 0) IsWeekend = true;
            else IsWeekend = false;

            IsSpecialday = false;
            IsHoliday = false;

            string SqlString = "SELECT *"
          + " FROM [dbo].[HolidaysAndSpecialDays] "
           + " WHERE Operator = @Operator AND SpecialDay = @SpecialDay AND HolidaysVersion = @HolidaysVersion";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SpecialDay", InDate);
                        cmd.Parameters.AddWithValue("@HolidaysVersion", InHolidaysVersion);
                      
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true; 

                            IsSpecialday = true;

                            if (IsSpecialday == true & IsNormal == true) IsNormal = false;

                            BankId = (string)rdr["BankId"];
                            Year = (int)rdr["Year"];
                            HolidaysVersion = (string)rdr["HolidaysVersion"];
                            SpecialDay = (DateTime)rdr["SpecialDay"];
                
                            IsHoliday = (bool)rdr["IsHoliday"];
                            SpecialDescr = (string)rdr["SpecialDescr"];
                            DiffDayEveryYear = (bool)rdr["DiffDayEveryYear"];
                            LastYearSpecial = (DateTime)rdr["LastYearSpecial"];
                            SpecialId = (int)rdr["SpecialId"];

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
                    ErrorOutput = "An error occured in Holidays Class............. " + ex.Message;
                }
        }

        // Insert New Holiday or Special  
        //
        public void InsertHoliday(string InOperator, DateTime InDate, string InHolidaysVersion )
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [dbo].[HolidaysAndSpecialDays]"
                + " ([BankId], [Year], [HolidaysVersion], [SpecialDay], "
                + " [IsHoliday], [SpecialDescr], [DiffDayEveryYear], [LastYearSpecial], [SpecialId], [Operator] )"
                + " VALUES (@BankId, @Year, @HolidaysVersion, @SpecialDay,"
                + " @IsHoliday, @SpecialDescr, @DiffDayEveryYear, @LastYearSpecial, @SpecialId , @Operator)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@Year", Year);
                        cmd.Parameters.AddWithValue("@HolidaysVersion", InHolidaysVersion);
                        cmd.Parameters.AddWithValue("@SpecialDay", InDate);
                
                        cmd.Parameters.AddWithValue("@IsHoliday", IsHoliday);

                        cmd.Parameters.AddWithValue("@SpecialDescr", SpecialDescr);
                        cmd.Parameters.AddWithValue("@DiffDayEveryYear", DiffDayEveryYear);
                        cmd.Parameters.AddWithValue("@LastYearSpecial", LastYearSpecial);

                        cmd.Parameters.AddWithValue("@SpecialId", SpecialId);

                        cmd.Parameters.AddWithValue("@Operator", InOperator);
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
                    ErrorOutput = "An error occured in Holidays Class............. " + ex.Message;
                }
        }


        // UPDATE Holiday 
        // 
        public void UpdateHoliday(string InOperator, DateTime InDate, string InHolidaysVersion )
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE [dbo].[HolidaysAndSpecialDays] SET "
                            + "BankId = @BankId,"
                             + "Year = @Year,"
                             + "HolidaysVersion = @HolidaysVersion, SpecialDay = @SpecialDay,"
                             + "IsHoliday = @IsHoliday, SpecialDescr = @SpecialDescr, DiffDayEveryYear = @DiffDayEveryYear,"
                             + "LastYearSpecial = @LastYearSpecial, SpecialId = @SpecialId"
                             + " WHERE Operator = @Operator AND SpecialDay = @SpecialDay AND HolidaysVersion = @HolidaysVersion", conn))
                    {
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@Year", Year);
                        cmd.Parameters.AddWithValue("@HolidaysVersion", InHolidaysVersion);
                        cmd.Parameters.AddWithValue("@SpecialDay", InDate);
                  
                        cmd.Parameters.AddWithValue("@IsHoliday", IsHoliday);

                        cmd.Parameters.AddWithValue("@SpecialDescr", SpecialDescr);
                        cmd.Parameters.AddWithValue("@DiffDayEveryYear", DiffDayEveryYear);
                        cmd.Parameters.AddWithValue("@LastYearSpecial", LastYearSpecial);

                        cmd.Parameters.AddWithValue("@SpecialId", SpecialId);
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
                    ErrorOutput = "An error occured in Holidays Class............. " + ex.Message;
                }
        }

       
// DELETE Holidays for year   
        //
        public void DeleteYearHoliday(string InOperator, int InYear, string InHolidaysVersion)
        {
         
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[HolidaysAndSpecialDays] "
                            + " WHERE Operator = @Operator AND Year = @Year AND  HolidaysVersion = @HolidaysVersion", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@Year", InYear);
                        cmd.Parameters.AddWithValue("@HolidaysVersion", InHolidaysVersion);

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
                    ErrorOutput = "An error occured in Holidays Class............. " + ex.Message;
                }
        }
        // DELETE Holiday   
        //
        public void DeleteHolidayEntry(string InOperator, DateTime InDate, string InHolidaysVersion)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[HolidaysAndSpecialDays] "
                            + " WHERE Operator = @Operator AND SpecialDay = @SpecialDay AND HolidaysVersion = @HolidaysVersion  ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@SpecialDay", InDate);
                        cmd.Parameters.AddWithValue("@HolidaysVersion", InHolidaysVersion);

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
                    ErrorOutput = "An error occured in Holidays Class............. " + ex.Message;
                }
        }

        // Copy Holidays 

        public void CopyHolidays(string InOperatorA, String InOperatorB, int InYear)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[HolidaysAndSpecialDays] "
           + " WHERE Operator = @Operator AND Year = @Year";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperatorA);
                        cmd.Parameters.AddWithValue("@Year", InYear);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;
                            
                            BankId = (string)rdr["BankId"];
                            Year = (int)rdr["Year"];
                            HolidaysVersion = (string)rdr["HolidaysVersion"];
                            SpecialDay = (DateTime)rdr["SpecialDay"];
                    
                            IsHoliday = (bool)rdr["IsHoliday"];
                            SpecialDescr = (string)rdr["SpecialDescr"];
                            DiffDayEveryYear = (bool)rdr["DiffDayEveryYear"];
                            LastYearSpecial = (DateTime)rdr["LastYearSpecial"];
                            SpecialId = (int)rdr["SpecialId"];

                            Operator = (string)rdr["Operator"];

                            // Insert Holiday 

                            InsertHoliday(InOperatorB, SpecialDay, HolidaysVersion);  

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
                    ErrorOutput = "An error occured in Holidays Class............. " + ex.Message;
                }
        }

        //
        // Get days of YEAR 
        // 
        public void GetNextWorkingDt(string InOperator, DateTime InStartDate, string InHolidaysVersion)
        {
            DateTime WDate = InStartDate; 
           // Read Next till Normal date 
            NextWorkingDtFound = false; 
            while ( NextWorkingDtFound == false)
            {
                WDate = WDate.AddDays(1);
                ReadSpecificDate(InOperator, WDate, InHolidaysVersion);

                if (IsNormal == true & IsSpecialday == false)
                {
                    NextWorkingDtFound = true;
                    NextWorkingDt = WDate; 
                }

            }

        }
        //
        // Get days of YEAR 
        // 
        public void GetDaysInAYear(int year)
        {

            daysInYear = 0;
            for (int i = 1; i <= 12; i++)
            {
                daysInYear += DateTime.DaysInMonth(year, i);
            }
     
        }


    }
}
