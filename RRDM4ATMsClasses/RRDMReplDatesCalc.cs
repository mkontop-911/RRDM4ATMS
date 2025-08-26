using System;
using System.Data;
using System.Windows.Forms;
// using System.Windows.Forms;


namespace RRDM4ATMs
{
    public class RRDMReplDatesCalc : Logger
    {
        public RRDMReplDatesCalc() : base() { }

        string WAtmNo;

        decimal WCurrentBal;
        string WMatchDatesCateg;
        string WHolidaysVersion;

        decimal Total;

        int SeqDay;
        int SeqDayToday;
        int SeqYearToday;

        public int TotWkend;
        public int TotHol;

        DateTime WDateStart;
        DateTime WDateEnd;

        int WRequest;

        DateTime y;

        DateTime WDt;

        DateTime SameAsDate;

        DateTime MatchedDate;

        //  DateTime MatchedDatePrevious_YEAR;

        int j;

        bool IsWorkingDay;
        bool IsWeekend;

        bool IsHoliday;

        //  bool DtFound;

        public string HolDesc;

        bool DiffDayEveryYear;

        //     int SpecialId;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime LongFutureDate = new DateTime(2050, 11, 21);

        public int NoOfDays;
        public DateTime EstReplDt;

        public int WorkingDaysTotal;
        public decimal TotalRepl = 0;

        // Define the data table 
        public DataTable dtRDays = new DataTable();
        // DATATable for Grid 
        public DataTable TableNextDaysTillRepl = new DataTable();

        // Remember

        /*     DateTime dateValue = new DateTime(2008, 6, 11);
                Console.WriteLine(dateValue.ToString("dddd"));    // Displays Wednesday */

        int SeqYearFirst;
        int SeqYearLast;
        string WOperator;

        //  public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        RRDMMatchedDates Md = new RRDMMatchedDates();
        RRDMHolidays Ch = new RRDMHolidays();

        RRDMAtmsDailyTransHistory Ah = new RRDMAtmsDailyTransHistory();
        RRDMAtmsClass Ac = new RRDMAtmsClass();

        // FIND NEXT REPLENISHMENT DATE
        public void FindNextRepl(string InOperator, string InAtmNo, int InOrdersCycle, decimal InAvailMoney, DateTime InWDt)
        {
            RRDMFixedDaysReplAtmClass Fr = new RRDMFixedDaysReplAtmClass();
            WOperator = InOperator;
            Ac.ReadAtm(InAtmNo); // GET type of Replenishment 

            //  Rc.GiveMeDataTableReplInfo(WOperator, WDTm, WOperator, WPrive, InAtmNo, NullPastDate, InAvailMoney, Ac.MatchDatesCateg);
            int Request = 3; // Find money will last at 

            GiveMeDataTableReplInfo(WOperator, InWDt,
                   NullPastDate, InAtmNo, InAvailMoney, Ac.MatchDatesCateg, Request);

            // Rc.NoOfDays Gives the number of days money will last for
            //
            if (NoOfDays == 0)
            {
                EstReplDt = InWDt;
            }

            if (NoOfDays > 0)
            {

                TableNextDaysTillRepl = new DataTable();
                TableNextDaysTillRepl.Clear();

                // DATA TABLE ROWS DEFINITION 
                TableNextDaysTillRepl.Columns.Add("Day", typeof(string)); // Date of the week 
                TableNextDaysTillRepl.Columns.Add("Date", typeof(DateTime));
                TableNextDaysTillRepl.Columns.Add("Amount", typeof(decimal));
                TableNextDaysTillRepl.Columns.Add("Type", typeof(string));
                TableNextDaysTillRepl.Columns.Add("Balance", typeof(decimal));

                DateTime TempDtTm = InWDt;
                //   DataRow RowGrid = GridDays.NewRow();

                WorkingDaysTotal = 0;

                TotalRepl = 0;

                int I = 0;

                while (I < (dtRDays.Rows.Count))
                {
                    // IN WHILE LOOP WE LEAVE OUT THE LAST DAY. THIS IS THE REPLENISHEMENT DAY 
                    DataRow RowGrid = TableNextDaysTillRepl.NewRow();

                    TempDtTm = (DateTime)dtRDays.Rows[I]["Date"];
                    // Read and find Final...  this aplies if over or discounted
                    Fr.ReadFixedDaysReplAtm(WOperator, InAtmNo, TempDtTm); // Correct if fixed 

                    if (Fr.RecordFound == true)
                    {
                        dtRDays.Rows[I]["RecDispensed"] = Fr.Final;
                    }

                    RowGrid["Day"] = TempDtTm.Date.DayOfWeek.ToString();
                    RowGrid["Date"] = TempDtTm.Date;
                    RowGrid["Amount"] = dtRDays.Rows[I]["RecDispensed"];

                    //decimal PreEstimated = (decimal)dtRDays.Rows[I]["RecDispensed"];

                    IsWorkingDay = (bool)dtRDays.Rows[I]["Normal"];
                    IsWeekend = (bool)dtRDays.Rows[I]["Weekend"];
                    IsHoliday = (bool)dtRDays.Rows[I]["Special"];

                    if (IsWorkingDay == true) WorkingDaysTotal = WorkingDaysTotal + 1;

                    if (IsWorkingDay == true) RowGrid["Type"] = "WorkingDay";
                    if (IsWeekend == true) RowGrid["Type"] = "Weekend";
                    if (IsHoliday == true) RowGrid["Type"] = "Holiday";

                    TotalRepl = TotalRepl + (decimal)dtRDays.Rows[I]["RecDispensed"];

                    RowGrid["Balance"] = InAvailMoney - TotalRepl;

                    if (I == dtRDays.Rows.Count - 1) // Last ROW 
                    {
                        EstReplDt = (DateTime)dtRDays.Rows[I]["Date"];
                        if (IsWorkingDay == true) RowGrid["Type"] = "Repl-WorkDay";
                        if (IsWeekend == true) RowGrid["Type"] = "Repl-Weekend";
                        if (IsHoliday == true) RowGrid["Type"] = "Repl-Holiday";
                    }

                    TableNextDaysTillRepl.Rows.Add(RowGrid);  // ADD THE ROW TO THE TABLE 

                    I++;
                }
            }
        }
        //
        // CALLED BY UCFormd to calculate the dates and amount for replenish
        // Call from Form50 to calculate amounts 
        //
        // Given current balance find how many days will last
        // WHEN ATM WILL RUN OUT OF MONEY
        // 
        public void GiveMeDataTableReplInfo(string InOperator, DateTime InDateStart,
            DateTime InDateEnd, string InAtmNo, decimal InCurrentBal, string InMatchDatesCateg, int InRequest)
        {

            bool FoundIn_YEAR;
            bool FoundIn_MONTH;
            bool FoundIn_Yesterday;

            WOperator = InOperator;

            WAtmNo = InAtmNo;

            WDateStart = InDateStart;
            WDateEnd = InDateEnd;
            WCurrentBal = InCurrentBal;
            WMatchDatesCateg = InMatchDatesCateg;
            WRequest = InRequest;

            if (InRequest == 4) WRequest = 3;

            ErrorFound = false;
            ErrorOutput = "";

            DateTime NullPastDate = new DateTime(1950, 11, 21);

            // WRequest = 1; // Find next working for replenishemnt     
            // WRequest = 2; // Next Replenishemnt DATE is given => Find Money and days 
            // WRequest = 3; // Find Out when ATM will run out of Money 

            TotWkend = 0;
            TotHol = 0;

            Total = 0; // Initialise total 

            dtRDays = new DataTable();
            dtRDays.Clear();
            // DATA TABLE ROWS DEFINITION 
            dtRDays.Columns.Add("Date", typeof(DateTime));
            dtRDays.Columns.Add("Normal", typeof(bool));
            dtRDays.Columns.Add("Weekend", typeof(bool));
            dtRDays.Columns.Add("Special", typeof(bool));
            dtRDays.Columns.Add("RecDispensed", typeof(decimal));
            dtRDays.Columns.Add("Total", typeof(decimal));

            dtRDays.Columns.Add("SameAsDate", typeof(DateTime));
            dtRDays.Columns.Add("SameNormal", typeof(bool));
            dtRDays.Columns.Add("SameWeekend", typeof(bool));
            dtRDays.Columns.Add("SameSpecial", typeof(bool));

            Ac.ReadAtm(WAtmNo); // to find Repl Diary Category
                                // Different versions may apply in each country 

            WHolidaysVersion = Ac.HolidaysVersion;

            int MM = WDateStart.Month;
            int DD = WDateStart.Day;

            SeqDayToday = (int)WDateStart.DayOfWeek;
            SeqYearToday = (int)WDateStart.DayOfYear;
            //bool FirstTime = false; 

            // Give me growth from LastYear To this Year
            decimal WGrowth;
            WGrowth = GiveMeGrowth(WOperator, WAtmNo, WDateStart);

            // FILL IN STRUCTURE WITH VALUES TILL NEXT REPLENISHMENT 

            y = WDateStart;
            j = 0;

            for (j = 0; j < 60; j++)
            {
                y = WDateStart.AddDays(j);

                FoundIn_YEAR = false;
                FoundIn_MONTH = false;
                FoundIn_Yesterday = false;

                //   CheckAndAssignTypeOfDays(WUserBankId, Ac.MatchDatesCateg, y, 2); // Find Matched dt and Special dt function = 2
                // Get date type
                GiveMeTypeOfDay(WOperator, y); // Find out type of day (eg Weekend, or Holiday or both)

                // Get Matched date Previous year
                //
                Md.ReadNextDatePrevious_YEAR(WOperator, WMatchDatesCateg, y);

                if (Md.RecordFound == true)
                {
                    MatchedDate = Md.SameAs;

                    Ah.ReadTransHistory(WAtmNo, WOperator, MatchedDate); // GETS MATCHED date dispensed

                    if (Ah.RecordFound == true)
                    {
                        // Fould In Year with value
                        FoundIn_YEAR = true;
                    }
                    else
                    {
                        // Not Found in year
                        FoundIn_YEAR = false;
                    }
                }
                //
                //
                if (FoundIn_YEAR == false)
                {
                    // Check if found in Month
                    //
                    // Get Matched date from previous month
                    //
                    Md.ReadNextDatePrevious_MONTH(WOperator, WMatchDatesCateg, y);

                    if (Md.RecordFound == true)
                    {
                        MatchedDate = Md.SameAs;

                        Ah.ReadTransHistory(WAtmNo, WOperator, MatchedDate); // GETS MATCHED date dispensed
                        if (Ah.RecordFound == true)
                        {
                            // Fould In Year with value
                            FoundIn_MONTH = true;
                        }
                        else
                        {
                            // Not Found in year
                            FoundIn_MONTH = false;
                        }
                    }
                }
                //
                if (FoundIn_YEAR == false & FoundIn_MONTH == false)
                {
                    // Yesterday day 
                    MatchedDate = WDateStart.AddDays(-1);

                    Ah.ReadTransHistory(WAtmNo, WOperator, MatchedDate); // GETS MATCHED date dispensed
                    if (Ah.RecordFound == true)
                    {
                        // Fould In Year with value
                        FoundIn_Yesterday = true;
                    }
                    else
                    {
                        // Not Found in year
                        FoundIn_Yesterday = false;
                        // This is a new ATM without History 
                        Ah.DispensedAmt = 0;
                        j = 60; // STOP LOOP
                        break;
                    }
                }

                if (FoundIn_YEAR == true
                    || FoundIn_MONTH == true
                    || FoundIn_Yesterday == true)
                {
                    // Continue
                    if (FoundIn_YEAR == true)
                    {
                        // Make the necessary adjustments based on growth.
                        // You adjust Ah.DispensedAmt
                        Ah.DispensedAmt = Ah.DispensedAmt * WGrowth;
                    }
                }
                else
                {
                    break;
                }

                //// Update counters 

                if (IsWeekend == true) TotWkend = TotWkend + 1;

                if (IsHoliday == true) TotHol = TotHol + 1;

                DataRow RowRepl = dtRDays.NewRow();

                GiveMeTypeOfDay(WOperator, y); // Find out type of day 

                RowRepl["Date"] = y;
                RowRepl["Normal"] = IsWorkingDay; // IfTrue is Normal
                RowRepl["Weekend"] = IsWeekend; // IfTrue is Weekend
                RowRepl["Special"] = IsHoliday;
                RowRepl["RecDispensed"] = Ah.DispensedAmt;

                Total = Total + Ah.DispensedAmt;

                RowRepl["Total"] = Total;

                RowRepl["SameAsDate"] = MatchedDate.Date;

                //  CheckAndAssignTypeOfDays(WUserBankId, Ac.MatchDatesCateg, MatchedDate.Date, 1); // FIND TYPE FOR MATCHEd DATE

                GiveMeTypeOfDay(WOperator, MatchedDate.Date); // Find out type of day 

                RowRepl["SameNormal"] = IsWorkingDay; // IfTrue is Normal
                RowRepl["SameWeekend"] = IsWeekend; // IfTrue is Weekend
                RowRepl["SameSpecial"] = IsHoliday;

                // Update PreEstimated
                UpdateInsertHistoryRecord(InAtmNo, y, Ah.DispensedAmt);

                dtRDays.Rows.Add(RowRepl);

                if (j == 0) // First Day = Today 
                {
                    // Assign Values to Structure Record 

                    SeqYearFirst = (int)y.DayOfYear;
                    SeqYearLast = (int)y.DayOfYear;
                }

                SeqYearLast = (int)y.DayOfYear;

                GiveMeTypeOfDay(WOperator, y); // Find out type of day 

                if (WRequest == 1 & (IsWorkingDay == true & IsHoliday == false) & j > 0) j = 60; // STOP LOOP

                if (WRequest == 2 & y == WDateEnd) j = 60; // STOP LOOp

                if (WRequest == 3)
                {
                    if (WCurrentBal < Total)
                    {
                        j = 60; // STOP LOOp
                                // OR we can use the instruction break
                    }
                }
            }

            if (SeqYearLast >= SeqYearFirst)
            {
                NoOfDays = SeqYearLast - SeqYearFirst; // Number of DAYS
            }
            else
            {
                int YYYY = WDateStart.Year;
                DateTime YearlastDay = new DateTime(YYYY, 12, 31);
                int SeqYearLastYear = (int)YearlastDay.DayOfYear;
                NoOfDays = SeqYearLastYear - SeqYearFirst + SeqYearLast; // Number of DAYS
            }

        }

        // Needed for Form 12 => Create matched dates 
        // 
        public void GiveMeDataTableOfMatchedDatesOnly_Previous_MONTH(string InOperator, DateTime InDateStart,
                                                     DateTime InDateEnd, string InMatchDatesCateg, string InHolidaysVersion, int InRequest)
        {
            // Populate table with corresponding dates
            //
            // a) If Holiday = 
            // b) If Weekend = last month same day
            // c) If Normal = Previous month's same day 

            WOperator = InOperator;

            WDateStart = InDateStart;
            WDateEnd = InDateEnd;

            WMatchDatesCateg = InMatchDatesCateg;
            WHolidaysVersion = InHolidaysVersion;
            WRequest = InRequest;

            DateTime NullPastDate = new DateTime(1900, 01, 01);

            dtRDays = new DataTable();
            dtRDays.Clear();
            // DATA TABLE ROWS DEFINITION 
            dtRDays.Columns.Add("Date", typeof(DateTime));
            dtRDays.Columns.Add("Normal", typeof(bool));
            dtRDays.Columns.Add("Weekend", typeof(bool));
            dtRDays.Columns.Add("Special", typeof(bool));

            dtRDays.Columns.Add("SameAsDate", typeof(DateTime));
            dtRDays.Columns.Add("SameNormal", typeof(bool));
            dtRDays.Columns.Add("SameWeekend", typeof(bool));
            dtRDays.Columns.Add("SameSpecial", typeof(bool));

            //int MM = WDateStart.Month;
            //int DD = WDateStart.Day;

            SeqDayToday = (int)WDateStart.DayOfWeek;
            SeqYearToday = (int)WDateStart.DayOfYear;

            // FILL IN STRUCTURE WITH VALUES TILL NEXT REPLENISHMENT 

            y = WDateStart;

            for (j = 0; j < 70; j++)
            {
                y = WDateStart.AddDays(j);

                GiveMeTypeOfDay(WOperator, y); // Find out type of day 

                if (IsHoliday == true) // If Special then set equal to last year  
                {
                    Ch.ReadSpecificDate(WOperator, y, WHolidaysVersion);

                    SameAsDate = Ch.LastYearHoliday; // Assign SAME As last years 

                }
                if ((IsHoliday == false & IsWeekend == true) // Weekened 
                    || (IsHoliday == false & IsWorkingDay == true) // Normal Day
                    )
                {
                    WDt = y;
                    WDt = WDt.AddMonths(-1);
                    SameAsDate = WDt; // Assign SAME AS DAte 
                }

                //if (IsHoliday == false & IsNormal == true) // Normal Date then find previous working date 
                //    {
                //            // FIND THE PREVIOUS WORKING DAY 

                //        WDt = y;
                //        DtFound = false;

                //        while (DtFound == false)
                //            {
                //             WDt = WDt.AddDays(-1); // Find the previous Normal 
                //             SeqDay = (int)WDt.DayOfWeek;
                //             if (SeqDay < 6 & SeqDay > 0 )
                //                {
                //                    //   CheckAndAssignTypeOfDays(WUserBankId, WMatchDatesCateg, WDt, 1);
                //                    GiveMeTypeOfDay(WOperator, WDt);

                //                    if (IsHoliday == false) DtFound = true;

                //                }
                //            }

                //         SameAsDate = WDt; // Assign SAME AS found DAte 
                //    }

                DataRow RowRepl = dtRDays.NewRow();

                GiveMeTypeOfDay(WOperator, y); // Refresh types for y 

                RowRepl["Date"] = y;
                RowRepl["Normal"] = IsWorkingDay; // IfTrue is Normal
                RowRepl["Weekend"] = IsWeekend; // IfTrue is Weekend
                RowRepl["Special"] = IsHoliday;

                RowRepl["SameAsDate"] = SameAsDate.Date;

                GiveMeTypeOfDay(WOperator, SameAsDate.Date); // Find out type of day 

                RowRepl["SameNormal"] = IsWorkingDay; // IfTrue is Normal
                RowRepl["SameWeekend"] = IsWeekend; // IfTrue is Weekend
                RowRepl["SameSpecial"] = IsHoliday;

                dtRDays.Rows.Add(RowRepl);

                if (j == 0) // First Day = Today 
                {
                    // Assign Values to Structure Record 

                    SeqYearFirst = (int)y.DayOfYear;
                    SeqYearLast = (int)y.DayOfYear;
                }

                SeqYearLast = (int)y.DayOfYear;

                GiveMeTypeOfDay(WOperator, y); // Refresh y 

                //          if (WRequest == 1 & IsNormal == true & j > 0) j = 40; // STOP LOOP

                if (WRequest == 2 & y == WDateEnd) j = 70; // STOP LOOP

            }

            if (SeqYearLast >= SeqYearFirst)
            {
                NoOfDays = SeqYearLast - SeqYearFirst; // Number of DAYS
            }
            else
            {
                int YYYY = WDateStart.Year;
                DateTime YearlastDay = new DateTime(YYYY, 12, 31);
                int SeqYearLastYear = (int)YearlastDay.DayOfYear;
                NoOfDays = SeqYearLastYear - SeqYearFirst + SeqYearLast; // Number of DAYS
            }

            //    return ; 

        }

        // Update Insert History record with the estimated to be dispensed
        private void UpdateInsertHistoryRecord(string InAtmNo, DateTime InDate, decimal InEstimated)
        {
           

            RRDMAtmsDailyTransHistory Ah = new RRDMAtmsDailyTransHistory();
            // here we must correct 
            Ah.ReadTransHistory(InAtmNo, WOperator, InDate.Date);
           // Ah.ReadTransHistory_Dispensed_Deposited(InAtmNo, InDate.Date,0);

            if (Ah.RecordFound == true)
            {
                Ah.PreEstimated = InEstimated;

                Ah.UpdatePreEstimatedDispensed(Ah.SeqNo);
            }
            else
            {
                MessageBox.Show("Examine what is this 988087");
                return; 
                Ah.AtmNo = InAtmNo;
                Ah.BankId = WOperator;
                Ah.Dt = InDate.Date;
                Ah.LoadedAtRMCycle = InDate.Year;

                Ah.PreEstimated = InEstimated;

                Ah.Operator = WOperator;

                Ah.InsertTransHistory_With_PreEstimated(InAtmNo, InDate.Date);
            }

        }

        // Needed for Form 12_Previous_YEAR => Create matched dates 
        // 
        public void GiveMeDataTableOfMatchedDatesOnly_Previous_YEAR(string InOperator, DateTime InDateStart,
                                                     DateTime InDateEnd, string InMatchDatesCateg, string InHolidaysVersion, int InRequest)
        {
            // Populate table with corresponding dates
            //
            // a) If Holiday = 
            // b) If Weekend = last month same day
            // c) If Normal = Previous month's same day 

            WOperator = InOperator;

            WDateStart = InDateStart;
            WDateEnd = InDateEnd;

            WMatchDatesCateg = InMatchDatesCateg;
            WHolidaysVersion = InHolidaysVersion;
            WRequest = InRequest;

            DateTime NullPastDate = new DateTime(1900, 01, 01);

            dtRDays = new DataTable();
            dtRDays.Clear();
            // DATA TABLE ROWS DEFINITION 
            dtRDays.Columns.Add("Date", typeof(DateTime));
            dtRDays.Columns.Add("Normal", typeof(bool));
            dtRDays.Columns.Add("Weekend", typeof(bool));
            dtRDays.Columns.Add("Special", typeof(bool));

            dtRDays.Columns.Add("SameAsDate", typeof(DateTime));
            dtRDays.Columns.Add("SameNormal", typeof(bool));
            dtRDays.Columns.Add("SameWeekend", typeof(bool));
            dtRDays.Columns.Add("SameSpecial", typeof(bool));

            //int MM = WDateStart.Month;
            //int DD = WDateStart.Day;

            SeqDayToday = (int)WDateStart.DayOfWeek;
            SeqYearToday = (int)WDateStart.DayOfYear;

            // FILL IN STRUCTURE WITH VALUES TILL NEXT REPLENISHMENT 

            y = WDateStart;

            for (j = 0; j < 70; j++)
            {
                y = WDateStart.AddDays(j);

                GiveMeTypeOfDay(WOperator, y); // Find out type of day 

                if (IsHoliday == true) // If Special then set equal to last year  
                {
                    Ch.ReadSpecificDate(WOperator, y, WHolidaysVersion);

                    SameAsDate = Ch.LastYearHoliday; // Assign SAME As last years 

                }
                if ((IsHoliday == false & IsWeekend == true) // Weekened 
                    || (IsHoliday == false & IsWorkingDay == true) // Normal Day
                    )
                {
                    WDt = y;
                    WDt = WDt.AddYears(-1);
                    SameAsDate = WDt; // Assign SAME AS DAte 
                }

                //if (IsHoliday == false & IsNormal == true) // Normal Date then find previous working date 
                //    {
                //            // FIND THE PREVIOUS WORKING DAY 

                //        WDt = y;
                //        DtFound = false;

                //        while (DtFound == false)
                //            {
                //             WDt = WDt.AddDays(-1); // Find the previous Normal 
                //             SeqDay = (int)WDt.DayOfWeek;
                //             if (SeqDay < 6 & SeqDay > 0 )
                //                {
                //                    //   CheckAndAssignTypeOfDays(WUserBankId, WMatchDatesCateg, WDt, 1);
                //                    GiveMeTypeOfDay(WOperator, WDt);

                //                    if (IsHoliday == false) DtFound = true;

                //                }
                //            }

                //         SameAsDate = WDt; // Assign SAME AS found DAte 
                //    }

                DataRow RowRepl = dtRDays.NewRow();

                GiveMeTypeOfDay(WOperator, y); // Refresh types for y 

                RowRepl["Date"] = y;
                RowRepl["Normal"] = IsWorkingDay; // IfTrue is Normal
                RowRepl["Weekend"] = IsWeekend; // IfTrue is Weekend
                RowRepl["Special"] = IsHoliday;

                RowRepl["SameAsDate"] = SameAsDate.Date;

                GiveMeTypeOfDay(WOperator, SameAsDate.Date); // Find out type of day 

                RowRepl["SameNormal"] = IsWorkingDay; // IfTrue is Normal
                RowRepl["SameWeekend"] = IsWeekend; // IfTrue is Weekend
                RowRepl["SameSpecial"] = IsHoliday;

                dtRDays.Rows.Add(RowRepl);

                if (j == 0) // First Day = Today 
                {
                    // Assign Values to Structure Record 

                    SeqYearFirst = (int)y.DayOfYear;
                    SeqYearLast = (int)y.DayOfYear;
                }

                SeqYearLast = (int)y.DayOfYear;

                GiveMeTypeOfDay(WOperator, y); // Refresh y 

                //          if (WRequest == 1 & IsNormal == true & j > 0) j = 40; // STOP LOOP

                if (WRequest == 2 & y == WDateEnd) j = 70; // STOP LOOp

                //     if (WRequest == 3)
                //     {
                //       if (WCurrentBal < Total)
                //    {
                //       j = 40; // STOP LOOp
                //     }
                //  }

            }

            if (SeqYearLast >= SeqYearFirst)
            {
                NoOfDays = SeqYearLast - SeqYearFirst; // Number of DAYS
            }
            else
            {
                int YYYY = WDateStart.Year;
                DateTime YearlastDay = new DateTime(YYYY, 12, 31);
                int SeqYearLastYear = (int)YearlastDay.DayOfYear;
                NoOfDays = SeqYearLastYear - SeqYearFirst + SeqYearLast; // Number of DAYS
            }

            //    return ; 

        }

        // Check if day is a Specialiday  => Find Matched date 
        //
        public void GiveMeTypeOfDay(string InOperator, DateTime InDateTime)
        {
            // InFunction 1 Check for special date, 2 : Find Matched date 
            int InYear = InDateTime.Year;

            SeqDay = (int)InDateTime.DayOfWeek;

            RRDMGasParameters Gp = new RRDMGasParameters();

            string ParId = "233";
            string OccurId = SeqDay.ToString(); // Check table for this 
            Gp.ReadParametersSpecificId(InOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            {
                // Eg for Greece SeqDay = 6 = Saturday and SeqDay = 0 = Sunday
                // Eg for Egypt SeqDay = 5 = Friday And 6 = Saturday and SeqDay = 0 = Sunday 
                IsWeekend = true;
                IsWorkingDay = false;
            }
            else
            {
                // For Egypt Sunday cones here 
                IsWeekend = false;
                IsWorkingDay = true;
                // IsNormal = true;
            }

            //if (SeqDay < 6 & SeqDay > 0) IsWorkingDay = true;
            //else IsWorkingDay = false;

            //if (SeqDay == 6 || SeqDay == 0) IsWeekend = true;
            //else IsWeekend = false;

            IsHoliday = false;

            Ch.ReadSpecificDate(InOperator, InDateTime, WHolidaysVersion);

            if (Ch.RecordFound == true) // If found then this Holiday  = Not working date 
            {
                // if (Ch.IsHoliday == true || Ch.IsHoliday == false) 
                //  {
                IsHoliday = true;
                HolDesc = Ch.Descr;
                DiffDayEveryYear = Ch.DiffDayEveryYear;
                MatchedDate = Ch.LastYearHoliday;
                // SpecialId = Ch.SpecialId;
                //   }
            }

        }
        //
        // Find Growth From this year to the next year
        //
        public decimal GiveMeGrowth(string InOperator, string InAtmNo, DateTime InWDate)
        {

            decimal Growth = 1;

            bool Valid = true;
            //
            // FIND total amount for the last 7 days
            //
            DateTime DateFrom = InWDate.AddDays(-7);
            DateTime DateTo = InWDate.AddDays(-1);
            decimal AmountB = Ah.ReadTotalDispForDaysRange(InAtmNo, DateFrom, DateTo);
            if (Ah.TotalRecords != 7)
            {
                Valid = false;
            }
            //
            // FIND CORRESPONDING Seven days from last year
            //
            DateFrom = DateFrom.AddYears(-1);
            DateTo = DateTo.AddYears(-1);
            decimal AmountA = Ah.ReadTotalDispForDaysRange(InAtmNo, DateFrom, DateTo);
            if (Ah.TotalRecords != 7)
            {
                Valid = false;
            }

            if (AmountA == 0 || AmountB == 0 || Valid == false)
            {
                Growth = 1;
            }
            else
            {

                Growth = 1 + (AmountB - AmountA) / 100;

                // If B>A then growth bigger than 1 else less than 1 
            }

            return Growth;
        }

    }
}


