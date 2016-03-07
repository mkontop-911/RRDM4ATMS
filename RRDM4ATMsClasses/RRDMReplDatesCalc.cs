using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;


namespace RRDM4ATMs
{
    public class RRDMReplDatesCalc
    {
     //   public DateTime NextWorkDate;
     //   public int DaysTillWork; 
     //   public int DayOfWeek;

    //    string WBankId;
     //   bool WPrive;
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
 
   //     DateTime YPreviousYear;
        DateTime WDt;

        DateTime SameAsDate;

        DateTime MatchedDate; 

     //   int i; 
        int j;

    //    bool Stop = false;

        bool IsNormal;
        bool IsWeekend; 
 
        bool IsHoliday;
 
        bool DtFound;

        public string HolDesc;

        bool DiffDayEveryYear;
    
        int SpecialId; 

        public int NoOfDays; 

        // Define the data table 
        public DataTable dtRDays = new DataTable();

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
       
        //
        // CALLED BY UCFormd to calculate the dates and amount for replenish
        // Call from Form50 to calculate amounts 
        //
        public void GiveMeDataTableReplInfo(string InOperator, DateTime InDateStart,
            DateTime InDateEnd, string InAtmNo, decimal InCurrentBal, string InMatchDatesCateg, int InRequest)
        {
            WOperator = InOperator; 
       
            WAtmNo = InAtmNo;

            WDateStart = InDateStart;
            WDateEnd = InDateEnd;
            WCurrentBal = InCurrentBal;
            WMatchDatesCateg = InMatchDatesCateg;
            WRequest = InRequest ;
            if (InRequest == 4) WRequest = 3;

            ErrorFound = false;
            ErrorOutput = ""; 

            DateTime NullPastDate = new DateTime(1950,11, 21);
          
               // WRequest = 1; // Find next working for replenishemnt     
              // WRequest = 2; // Next Replenishemnt DATE is given => Find Money and days 
              // WRequest = 3; // Find Out when ATM will run out of Money 
        
            TotWkend = 0 ;
            TotHol = 0 ; 

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

            WHolidaysVersion = Ac.HolidaysVersion; 

            int MM = WDateStart.Month;
            int DD = WDateStart.Day;

            SeqDayToday = (int)WDateStart.DayOfWeek;
            SeqYearToday = (int)WDateStart.DayOfYear;

            // FILL IN STRUCTURE WITH VALUES TILL NEXT REPLENISHMENT 

            y = WDateStart; 

            for (j = 0; j < 40; j++)
            {
                y = WDateStart.AddDays(j);

             //   CheckAndAssignTypeOfDays(WUserBankId, Ac.MatchDatesCateg, y, 2); // Find Matched dt and Special dt function = 2
                // Get date type
                GiveMeTypeOfDay(WOperator, y); // Find out type of day 

                // Get Matched date 
                Md.ReadNextDate(WOperator, WMatchDatesCateg, y);

                if (Md.RecordFound == true)
                {
                    MatchedDate = Md.SameAs;
                }
                else
                {
                    ErrorFound = true;
                    ErrorOutput = "Matched date not found for: " + y.ToString() + " Matched is assigned as the Previous to today."; 
                 //   MessageBox.Show("Matched date not found for: " + y.ToString() + " Matched is assigned as the Previous to today.");
                    MatchedDate = DateTime.Now.AddDays(-1);
                }

                // Update counters 

                if (IsWeekend == true) TotWkend = TotWkend + 1;

                if (IsHoliday == true) TotHol = TotHol + 1;  
          
                
                Ah.ReadTransHistory(WAtmNo, WOperator, MatchedDate); // GETS MATCHED date dispensed
                if (Ah.RecordFound == false)
                {
                  // THIS IS A NEW ATM WITHOUT PAST HISTORY 
                    Ah.ReadTransHistoryYesterday (WAtmNo, WOperator ); // FIND THE LATEST TURNOVER FOR THIS ATM
                    if (Ah.RecordFound == false)
                    {
                        ErrorFound = true;
                        ErrorOutput = "MSG345: No Dispensed Amounts found for this ATMNo:" + WAtmNo; 
                    //    MessageBox.Show("MSG345: No Dispensed Amounts found for this ATMNo:" + WAtmNo);  
                    }

                }
                
                DataRow RowRepl = dtRDays.NewRow();

                GiveMeTypeOfDay(WOperator, y); // Find out type of day 

                RowRepl["Date"] = y;
                RowRepl["Normal"] = IsNormal; // IfTrue is Normal
                RowRepl["Weekend"] = IsWeekend; // IfTrue is Weekend
                RowRepl["Special"] = IsHoliday;
                RowRepl["RecDispensed"] = Ah.DispensedAmt;
                Total = Total + Ah.DispensedAmt;
               
                RowRepl["Total"] = Total;

                RowRepl["SameAsDate"] = MatchedDate.Date;

              //  CheckAndAssignTypeOfDays(WUserBankId, Ac.MatchDatesCateg, MatchedDate.Date, 1); // FIND TYPE FOR MATCHEd DATE

                GiveMeTypeOfDay(WOperator, MatchedDate.Date); // Find out type of day 

                RowRepl["SameNormal"] = IsNormal; // IfTrue is Normal
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

                 GiveMeTypeOfDay(WOperator, y); // Find out type of day 

                 if (WRequest == 1 & (IsNormal == true & IsHoliday==false)  & j>0) j = 40; // STOP LOOP

                 if (WRequest == 2 & y == WDateEnd) j = 40; // STOP LOOp

                 if (WRequest == 3)
                 {
                     if (WCurrentBal < Total )
                     {
                         j = 40; // STOP LOOp
                     }
                 }
            }
      

            if (SeqYearLast >= SeqYearFirst)
            {
               NoOfDays = SeqYearLast - SeqYearFirst ; // Number of DAYS
            }
            else
            {
                int YYYY = WDateStart.Year;
                DateTime YearlastDay = new DateTime(YYYY, 12, 31);
                int SeqYearLastYear = (int)YearlastDay.DayOfYear;
                NoOfDays = SeqYearLastYear - SeqYearFirst  + SeqYearLast ; // Number of DAYS
            }
                       
        }

        // Needed for Form 12 => Create matched dates 
        // 
        public void GiveMeDataTableOfMatchedDatesOnly(string InOperator, DateTime InDateStart,
                                                     DateTime InDateEnd, string InMatchDatesCateg, string InHolidaysVersion, int InRequest)
        {
            // Populate table with corresponding dates
            //
            // a) If Holiday = last year holiday 
            // b) If Weekend = last month same day
            // c) If Normal = Previous working day 

            WOperator = InOperator;
         //   WBankId = InBankId;
         //   WPrive = InPrive;
          
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

            int MM = WDateStart.Month;
            int DD = WDateStart.Day;

            SeqDayToday = (int)WDateStart.DayOfWeek;
            SeqYearToday = (int)WDateStart.DayOfYear;

            // FILL IN STRUCTURE WITH VALUES TILL NEXT REPLENISHMENT 

            y = WDateStart;

            for (j = 0; j < 40; j++)
            {
                y = WDateStart.AddDays(j);

                GiveMeTypeOfDay(WOperator, y); // Find out type of day 

                if (IsHoliday == true) // If Special then set equal to last year  
                    {
                        Ch.ReadSpecificDate(WOperator, y, WHolidaysVersion);

                        SameAsDate = Ch.LastYearSpecial; // Assign SAME As last years 

                    }
                if (IsHoliday == false & IsWeekend == true) // WEEKEND is true then get equal date than previous month  
                    {
                        WDt = y;
                        WDt = WDt.AddMonths(-1);
                        SameAsDate = WDt; // Assign SAME AS DAte 
                    }

                if (IsHoliday == false & IsNormal == true) // Normal Date then find previous working date 
                    {
                            // FIND THE PREVIOUS WORKING DAY 

                        WDt = y;
                        DtFound = false;

                        while (DtFound == false)
                            {
                             WDt = WDt.AddDays(-1); // Find the previous Normal 
                             SeqDay = (int)WDt.DayOfWeek;
                             if (SeqDay < 6 & SeqDay > 0 )
                                {
                                    //   CheckAndAssignTypeOfDays(WUserBankId, WMatchDatesCateg, WDt, 1);
                                    GiveMeTypeOfDay(WOperator, WDt);

                                    if (IsHoliday == false) DtFound = true;

                                }
                            }

                         SameAsDate = WDt; // Assign SAME AS found DAte 
                    }
               
                    DataRow RowRepl = dtRDays.NewRow();

                    GiveMeTypeOfDay(WOperator, y); // Refresh types for y 

                    RowRepl["Date"] = y;
                    RowRepl["Normal"] = IsNormal; // IfTrue is Normal
                    RowRepl["Weekend"] = IsWeekend; // IfTrue is Weekend
                    RowRepl["Special"] = IsHoliday;

                    RowRepl["SameAsDate"] = SameAsDate.Date;

                    GiveMeTypeOfDay(WOperator, SameAsDate.Date); // Find out type of day 

                    RowRepl["SameNormal"] = IsNormal; // IfTrue is Normal
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

                    if (WRequest == 2 & y == WDateEnd) j = 40; // STOP LOOp

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

            if (SeqDay < 6 & SeqDay > 0) IsNormal = true;
            else IsNormal = false;

            if (SeqDay == 6 || SeqDay == 0) IsWeekend = true;
            else IsWeekend = false;

            IsHoliday = false;

            Ch.ReadSpecificDate(InOperator, InDateTime, WHolidaysVersion);

            if (Ch.RecordFound == true) // If found then this Holiday or special day = Not working date 
            {
               // if (Ch.IsHoliday == true || Ch.IsHoliday == false) 
              //  {
                    IsHoliday = true;
                    HolDesc = Ch.SpecialDescr;
                    DiffDayEveryYear = Ch.DiffDayEveryYear;
                    MatchedDate = Ch.LastYearSpecial;
                    SpecialId = Ch.SpecialId;
             //   }
            }
            
        }

        }
    }


