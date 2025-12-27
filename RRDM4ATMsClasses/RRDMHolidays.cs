using System;
using System.Text;
using System.Data;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMHolidays : Logger
    {
        public RRDMHolidays() : base() { }

        // DECLARE HOLIDAY FIELDS
        public int SeqNo; 
        public string BankId;
        public int Year;
        public string HolidaysVersion; 
        public DateTime HoliDay; 
   
        public string Descr;
        public bool DiffDayEveryYear;
        public DateTime LastYearHoliday; 
      
        public string Operator;

        public bool IsNormal;
        public bool IsWeekend;
        public bool IsHoliday;
     
        public DateTime NextWorkingDt;
        public bool NextWorkingDtFound; 

        public int daysInYear;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        public DataTable HolidaysTable = new DataTable();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");
        // Holiday Fields 
        private void ReadHolidaysFields(SqlDataReader rdr)
        {
            SeqNo = (int)rdr["SeqNo"];
            BankId = (string)rdr["BankId"];
            Year = (int)rdr["Year"];
            HolidaysVersion = (string)rdr["HolidaysVersion"];
            HoliDay = (DateTime)rdr["HoliDay"];

            Descr = (string)rdr["Descr"];
            DiffDayEveryYear = (bool)rdr["DiffDayEveryYear"];
            LastYearHoliday = (DateTime)rdr["LastYearHoliday"];
        
            Operator = (string)rdr["Operator"];
        }

        // Read Table 
        //  Holiday 
        public int TotalRows;
        public int NeedCorrection;
        public void ReadHolidaysAndFillTable(string InOperator, int InBaseYear,string InHolidaysVersion)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            NeedCorrection = 0;

            TotalRows = 0;
         
            HolidaysTable = new DataTable();
            HolidaysTable.Clear();

            // DATA TABLE ROWS DEFINITION 
            HolidaysTable.Columns.Add("SeqNo", typeof(int));
            HolidaysTable.Columns.Add("NeedCorr", typeof(string));
            HolidaysTable.Columns.Add("Operator", typeof(string));
            HolidaysTable.Columns.Add("Year", typeof(int));
            HolidaysTable.Columns.Add("Date", typeof(DateTime));
            HolidaysTable.Columns.Add("Day", typeof(string));
            HolidaysTable.Columns.Add("Descr", typeof(string));
            HolidaysTable.Columns.Add("LastYear", typeof(DateTime));

            DateTime WTempDt;
            DateTime NotCorrTempDate = new DateTime(InBaseYear, 12, 31);

            string SQLString = "Select * FROM [dbo].[HolidaysTable] "
            + " WHERE Operator = @Operator AND Year = @Year AND HolidaysVersion = @HolidaysVersion "
            + " Order By Holiday, SeqNo " ;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@Year", InBaseYear);
                        cmd.Parameters.AddWithValue("@HolidaysVersion", InHolidaysVersion);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalRows = TotalRows + 1;

                            ReadHolidaysFields(rdr);

                            DataRow RowGrid = HolidaysTable.NewRow();

                            RowGrid["SeqNo"] = SeqNo;
                           
                            RowGrid["Operator"] = Operator ;
                            RowGrid["Year"] = Year;

                            RowGrid["Date"] = HoliDay.Date;
                            RowGrid["Day"] = HoliDay.DayOfWeek;

                            RowGrid["Descr"] = Descr;
                            RowGrid["LastYear"] = LastYearHoliday.Date;

                            if (DiffDayEveryYear & HoliDay == NotCorrTempDate)
                            {
                                RowGrid["NeedCorr"] = "Yes";
                                NeedCorrection = NeedCorrection + 1; 
                            }
                            else
                            {
                                RowGrid["NeedCorr"] = "";
                            }

                            HolidaysTable.Rows.Add(RowGrid);
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

        // Read Table 
        //  Holiday 

        public int ReadHolidaysAndFindInsertLocation(string InOperator, int InBaseYear, int InNextYear, 
                                            string InHolidaysVersion, int InSeqNoOfInsert)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            int InsertLocation = 0; 

            string SQLString = "Select * FROM [dbo].[HolidaysTable] "
            + " WHERE Operator = @Operator AND Year = @Year AND HolidaysVersion = @HolidaysVersion "
            + " Order By Holiday ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@Year", InBaseYear);
                        cmd.Parameters.AddWithValue("@HolidaysVersion", InHolidaysVersion);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadHolidaysFields(rdr);

                            InsertLocation = InsertLocation + 1; 

                            if (InSeqNoOfInsert == SeqNo)
                            {                    
                                break;
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
            return InsertLocation; 
        }

        // READ Holiday by SeqNo

        public void ReadHolidayBySeqNo(string InOperator, int InSeqNo )
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            string SqlString = "SELECT *"
          + " FROM [dbo].[HolidaysTable] "
           + " WHERE SeqNo = @SeqNo ";

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

                            ReadHolidaysFields(rdr);

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

        // READ Holiday 

        public void ReadSpecificDate( string InOperator, DateTime InDate, string InHolidaysVersion)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            RRDMGasParameters Gp = new RRDMGasParameters();

            IsNormal = false;
            IsWeekend = false; 

            int InYear = InDate.Year;

            int SeqDay = (int)InDate.DayOfWeek;

            string ParId = "233";
            string OccurId = SeqDay.ToString(); // Check table for this 
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            {
                // Eg for Greece SeqDay = 6 = Saturday and SeqDay = 0 = Sunday
                // Eg for Egypt SeqDay = 5 = Friday And 6 = Saturday and SeqDay = 0 = Sunday 
                IsWeekend = true;
                IsNormal = false;
            }
            else
            {
                // For Egypt Sunday cones here 
                IsWeekend = false;
                IsNormal = true; 
            }

            //if (SeqDay < 6 & SeqDay > 0) IsNormal = true;
            //else IsNormal = false;

            //if (SeqDay == 6 || SeqDay == 0) IsWeekend = true;
            //else IsWeekend = false;

            IsHoliday = false;

            string SqlString = "SELECT *"
          + " FROM [dbo].[HolidaysTable] "
           + " WHERE Operator = @Operator AND HoliDay = @HoliDay AND HolidaysVersion = @HolidaysVersion";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@HoliDay", InDate);
                        cmd.Parameters.AddWithValue("@HolidaysVersion", InHolidaysVersion);
                      
                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ReadHolidaysFields(rdr);

                            IsHoliday = true;

                            if (IsHoliday == true & IsNormal == true) IsNormal = false;
                
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


        // Insert New Holiday or Special  
        //
        public int InsertHoliday(string InOperator)
        {
           
            ErrorFound = false;
            ErrorOutput = ""; 

            string cmdinsert = "INSERT INTO [dbo].[HolidaysTable]"
                + " ([BankId], [Year], [HolidaysVersion], [HoliDay], "
                + " [Descr], [DiffDayEveryYear], [LastYearHoliday], [Operator] )"
                + " VALUES (@BankId, @Year, @HolidaysVersion, @HoliDay,"
                + "  @Descr, @DiffDayEveryYear, @LastYearHoliday , @Operator )"
                +" SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@BankId", InOperator);
                        cmd.Parameters.AddWithValue("@Year", Year);
                        cmd.Parameters.AddWithValue("@HolidaysVersion", HolidaysVersion);
                        cmd.Parameters.AddWithValue("@HoliDay", HoliDay.Date);
                
                        cmd.Parameters.AddWithValue("@Descr", Descr);
                        cmd.Parameters.AddWithValue("@DiffDayEveryYear", DiffDayEveryYear);
                        cmd.Parameters.AddWithValue("@LastYearHoliday", LastYearHoliday.Date);

                        cmd.Parameters.AddWithValue("@Operator", InOperator);

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


        // UPDATE Holiday 
        // 
        public void UpdateHoliday(int InSeqNo )
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(" UPDATE [dbo].[HolidaysTable] SET "
                            + "BankId = @BankId,"
                             + "Year = @Year,"
                             + "HolidaysVersion = @HolidaysVersion, HoliDay = @HoliDay,"
                             + " Descr = @Descr, DiffDayEveryYear = @DiffDayEveryYear,"
                             + "LastYearHoliday = @LastYearHoliday "
                             + " WHERE SeqNo = @SeqNo ", conn))
                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);
                        cmd.Parameters.AddWithValue("@BankId", BankId);
                        cmd.Parameters.AddWithValue("@Year", Year);
                        cmd.Parameters.AddWithValue("@HolidaysVersion", HolidaysVersion);
                        cmd.Parameters.AddWithValue("@HoliDay", HoliDay.Date);
                  
                        cmd.Parameters.AddWithValue("@Descr", Descr);
                        cmd.Parameters.AddWithValue("@DiffDayEveryYear", DiffDayEveryYear);
                        cmd.Parameters.AddWithValue("@LastYearHoliday", LastYearHoliday.Date);

                      //  cmd.Parameters.AddWithValue("@Operator", Operator);

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

       
// DELETE Holidays for WHOLE year   
        //
        public void DeleteYearHoliday(string InOperator, string InYear)
        {
         
            ErrorFound = false;
            ErrorOutput = "";
         
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[HolidaysTable] "
                            + " WHERE Operator=@Operator AND Year = @Year", conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@Year", InYear);
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
        public void DeleteHolidayEntry(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [dbo].[HolidaysTable] "
                            + " WHERE SeqNo = @SeqNo", conn))
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

        // Copy Holidays from Year to year

        public void CopyHolidaysFromYearToYear(string InOperator, int InBaseYear, int InNextYear)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            bool Valid = false; 

            DateTime NotCorrTempDate = new DateTime(InNextYear, 12, 31);

            string SqlString = "SELECT *"
          + " FROM [dbo].[HolidaysTable] "
           + " WHERE Operator = @Operator AND Year = @Year "
           + " Order By Holiday ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@Operator", InOperator);
                        cmd.Parameters.AddWithValue("@Year", InBaseYear);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            Valid = false; 

                            ReadHolidaysFields(rdr);

                            Year = InNextYear;

                            if (DiffDayEveryYear == false)
                            {
                                HoliDay = HoliDay.AddYears(1);
                                // Check if Weekend 

                                ReadSpecificDate(InOperator, HoliDay, HolidaysVersion);

                                if (IsWeekend == true)
                                {
                                    Valid = false; 
                                }
                                else
                                {
                                    Valid = true; 
                                }
                                LastYearHoliday = LastYearHoliday.AddYears(1);
                            }
                            else
                            {
                                Valid = true;
                                LastYearHoliday = HoliDay;
                                HoliDay = NotCorrTempDate;
                            }

                            if (Valid == true)
                            {
                                 InsertHoliday(InOperator);
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

        //private bool IsWeekend(DateTime InDate)
        //{
        //    bool IsWeekend;
        //    int SeqDay = (int)InDate.DayOfWeek;

        //    if (SeqDay == 6 || SeqDay == 0) IsWeekend = true;
        //    else IsWeekend = false;

        //    return IsWeekend;
        //}

        // Copy Holidays from Bank to Bank

        public void CopyHolidays(string InOperatorA, String InOperatorB, int InYear)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = ""; 

            string SqlString = "SELECT *"
          + " FROM [dbo].[HolidaysTable] "
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

                            ReadHolidaysFields(rdr);

                            // Insert Holiday 

                            //    InsertHoliday(InOperatorB, HoliDay, HolidaysVersion);  
                            InsertHoliday(InOperatorB);

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
        // Get days of YEAR 
        // 
        public int DaysTillNextWorking;
        public DateTime GetNextWorkingDt(string InOperator, DateTime InStartDate, string InHolidaysVersion)
        {
            DaysTillNextWorking = 0;
            DateTime WDate = InStartDate; 
           // Read Next till Normal date 
            NextWorkingDtFound = false; 
            while ( NextWorkingDtFound == false)
            {
                WDate = WDate.AddDays(1);
                ReadSpecificDate(InOperator, WDate, InHolidaysVersion);

                if (IsNormal == true & IsHoliday == false)
                {
                    NextWorkingDtFound = true;
                    NextWorkingDt = WDate;
                    DaysTillNextWorking = DaysTillNextWorking + 1; 
                }
                else
                {
                    DaysTillNextWorking = DaysTillNextWorking + 1;
                }

            }

            return NextWorkingDt; 

        }
        //
        // Get days of YEAR 
        // 
        public int DaysTillSecondWorking;
        public DateTime GetNextSecondWorkingDt_NBG(string InOperator, DateTime InStartDate, string InHolidaysVersion)
        {
            DateTime NBG_CutOffDate = NullPastDate; 
            DaysTillSecondWorking = 0;
            DateTime WDate = InStartDate;
            // Read Next till Normal date 
            
            while (DaysTillSecondWorking < 2)
            {
                WDate = WDate.AddDays(1);
                ReadSpecificDate(InOperator, WDate, InHolidaysVersion);

                if (IsNormal == true & IsHoliday == false)
                {
                    NBG_CutOffDate = WDate;
                    DaysTillSecondWorking = DaysTillSecondWorking + 1;
                }
            }
            NBG_CutOffDate = NBG_CutOffDate.AddDays(-1);
            return NBG_CutOffDate;
        }
        
      // Get next working date 
        public DateTime GetNextSecondWorkingDt(string InOperator, DateTime InStartDate, string InHolidaysVersion)
        {
            DateTime NextWorkingDate = NullPastDate;
            
            DateTime WDate = InStartDate;
            // Read Next till Normal date 
            bool NextWorkingFound = false; 

            while (NextWorkingFound == false)
            {
                WDate = WDate.AddDays(1);
                ReadSpecificDate(InOperator, WDate, InHolidaysVersion);

                if (IsNormal == true & IsHoliday == false)
                {
                    NextWorkingDate = WDate;
                    NextWorkingFound = true; 
                }
            }
           
            return NextWorkingDate;
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


