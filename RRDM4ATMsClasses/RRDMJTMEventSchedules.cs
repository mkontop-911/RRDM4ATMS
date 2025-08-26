using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMJTMEventSchedules : Logger
    {
        public RRDMJTMEventSchedules() : base() { }

        public int SeqNo;
        public string ScheduleID;
        public string EventType;

        public DateTime DateLastUpdated;
        public string UserId;

        public DateTime EffectiveDateTmFrom;
        public DateTime EffectiveDateTmTo;

        public bool Recurrence;
        public bool RecurDaily;
        public bool RecurWeekly;
        public bool RecurMonthly;
        public bool RecurPerMinutes;

        public bool RecurEveryDays;
        public int NumberOfDays;
        public bool RecurEveryWeekDay;

        public int RecurPerMinutesValue;

        public string Operator;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string SqlString;

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime LongFutureDate = new DateTime(2050, 11, 21);

        // Define the data table 
        public DataTable EventSchedulesTable = new DataTable();

        public int TotalSelected;

        string connectionString = ConfigurationManager.ConnectionStrings["ATMSConnectionString"].ConnectionString;

        //
        // Read Reader fields 
        //
        private void ReaderFields(SqlDataReader rdr)
        {

            SeqNo = (int)rdr["SeqNo"];

            ScheduleID = (string)rdr["ScheduleID"];
            EventType = (string)rdr["EventType"];

            DateLastUpdated = (DateTime)rdr["DateLastUpdated"];
            UserId = (string)rdr["UserId"];

            EffectiveDateTmFrom = (DateTime)rdr["EffectiveDateTmFrom"];
            EffectiveDateTmTo = (DateTime)rdr["EffectiveDateTmTo"];

            Recurrence = (bool)rdr["Recurrence"];
            RecurDaily = (bool)rdr["RecurDaily"];
            RecurWeekly = (bool)rdr["RecurWeekly"];
            RecurMonthly = (bool)rdr["RecurMonthly"];
            RecurPerMinutes = (bool)rdr["RecurPerMinutes"];

            RecurEveryDays = (bool)rdr["RecurEveryDays"];
            NumberOfDays = (int)rdr["NumberOfDays"];
            RecurEveryWeekDay = (bool)rdr["RecurEveryWeekDay"];

            RecurPerMinutesValue = (int)rdr["RecurPerMinutesValue"];

            Operator = (string)rdr["Operator"];

        }
        //
        // READ JTMEventSchedules Record 
        //
        public void ReadJTMEventSchedulesToGetRecord(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                             + " FROM [ATMS].[dbo].[JTMEventSchedules] "
                             + InSelectionCriteria;

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

                            // Read ATMs Journal fields 
                            ReaderFields(rdr);

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
        // READ JTMEventSchedules to fill partial table 
        //

        public void ReadJTMEventSchedulesToFillPartialTable(string InSelectionCriteria)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            EventSchedulesTable = new DataTable();
            EventSchedulesTable.Clear();
            TotalSelected = 0;

            // DATA TABLE ROWS DEFINITION 
            EventSchedulesTable.Columns.Add("SeqNo", typeof(int));
            EventSchedulesTable.Columns.Add("ScheduleID", typeof(string));
            EventSchedulesTable.Columns.Add("DateLastUpdated", typeof(DateTime));
            EventSchedulesTable.Columns.Add("UserId", typeof(string));

            SqlString = "SELECT * "
                             + " FROM [ATMS].[dbo].[JTMEventSchedules] "
                             + InSelectionCriteria;

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        //cmd.Parameters.AddWithValue("@Operator", InOperator);

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
                            DataRow RowSelected = EventSchedulesTable.NewRow();

                            RowSelected["SeqNo"] = SeqNo;
                            RowSelected["ScheduleID"] = ScheduleID;
                            RowSelected["DateLastUpdated"] = DateLastUpdated;
                            RowSelected["UserId"] = UserId;

                            // ADD ROW
                            EventSchedulesTable.Rows.Add(RowSelected);

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
        // Get Event Group Ids (Schedule Id)
        //
        public ArrayList GetScheduleIdsByType(string InOperator, string InEventType)
        {
            ArrayList EventTypeList = new ArrayList();
            //  CitNosList.Add("Own");

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT * "
                               + " FROM [ATMS].[dbo].[JTMEventSchedules] "
                               + " WHERE EventType = @EventType "
                               + " ORDER BY  ScheduleID, EventType ";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@EventType", InEventType);

                        // Read table 
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            ScheduleID = (string)rdr["ScheduleID"];

                            EventTypeList.Add(ScheduleID);
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

            return EventTypeList;
        }

        //
        // Calculate next Event Date Time 
        //
        DateTime NextDtTmToLoad;
        public DateTime ReadCalculatedNextEventDateTm(string InOperator, string InScheduleID, DateTime InWdateTime, DateTime InLastLoadedDatTm)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            SqlString = "SELECT *"
                             + " FROM [ATMS].[dbo].[JTMEventSchedules] "
                             + " WHERE ScheduleID = @ScheduleID ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SqlString, conn))
                    {
                        cmd.Parameters.AddWithValue("@ScheduleID", InScheduleID);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            // Read ATMs Journal fields 
                            ReaderFields(rdr);

                            // CALCULATE NEXT DATE AND TIME 

                            // Equal occurs if called from Form65 = ATMs maintenance 
                            //
                            // Calculate date time 
                            if (RecurDaily == true & RecurEveryDays == true)
                            {
                                // Find Next Day 
                                int Hours = EffectiveDateTmFrom.Hour;
                                int Minutes = EffectiveDateTmFrom.Minute;

                                NextDtTmToLoad = InLastLoadedDatTm.Date.AddDays(NumberOfDays);
                                NextDtTmToLoad = NextDtTmToLoad.AddHours(Hours);
                                NextDtTmToLoad = NextDtTmToLoad.AddMinutes(Minutes);

                            }
                            if (RecurDaily == true & RecurEveryWeekDay == true)
                            {
                                // Find current Day and Time 
                                int Hours = EffectiveDateTmFrom.Hour;
                                int Minutes = EffectiveDateTmFrom.Minute;

                                NextDtTmToLoad = InLastLoadedDatTm.Date;
                                NextDtTmToLoad = NextDtTmToLoad.AddHours(Hours);
                                NextDtTmToLoad = NextDtTmToLoad.AddMinutes(Minutes);

                                // Find Next Working Date 
                                RRDMHolidays Hol = new RRDMHolidays();
                                NextDtTmToLoad = Hol.GetNextWorkingDt(InOperator, NextDtTmToLoad, "Hol Version01 - Standard");

                            }
                            if (RecurPerMinutes == true) // PER MINUTES 
                            {
                                // Find current Day and Time 
                                NextDtTmToLoad = InLastLoadedDatTm.AddMinutes(RecurPerMinutesValue);

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
            return NextDtTmToLoad;
        }



        // Insert NEW Record in JTMEventSchedules
        //
        public int InsertNewRecordInJTMEventSchedules()
        {

            ErrorFound = false;
            ErrorOutput = "";

            string cmdinsert = "INSERT INTO [ATMS].[dbo].[JTMEventSchedules]"
           + "([ScheduleID]"
           + ",[EventType]"
           + ",[DateLastUpdated]"
           + ",[UserId]"
           + ",[EffectiveDateTmFrom]"
           + ",[EffectiveDateTmTo]"
           + ",[Recurrence]"
           + ",[RecurDaily]"
           + ",[RecurWeekly]"
           + ",[RecurMonthly]"
           + ",[RecurPerMinutes]"
           + ",[RecurEveryDays]"
           + ",[NumberOfDays]"
           + ",[RecurEveryWeekDay]"
           + ",[RecurPerMinutesValue]"
           + ",[Operator])"
     + " VALUES "
            + "(@ScheduleID"
           + ",@EventType"
           + ",@DateLastUpdated"
           + ",@UserId"
           + ",@EffectiveDateTmFrom"
           + ",@EffectiveDateTmTo"
           + ",@Recurrence"
           + ",@RecurDaily"
           + ",@RecurWeekly"
           + ",@RecurMonthly"
           + ",@RecurPerMinutes"
           + ",@RecurEveryDays"
           + ",@NumberOfDays"
           + ",@RecurEveryWeekDay"
           + ",@RecurPerMinutesValue"
           + ",@Operator )"
                + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {

                        cmd.Parameters.AddWithValue("@ScheduleID", ScheduleID);
                        cmd.Parameters.AddWithValue("@EventType", EventType);

                        cmd.Parameters.AddWithValue("@DateLastUpdated", DateLastUpdated);

                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        cmd.Parameters.AddWithValue("@EffectiveDateTmFrom", EffectiveDateTmFrom);
                        cmd.Parameters.AddWithValue("@EffectiveDateTmTo", EffectiveDateTmTo);

                        cmd.Parameters.AddWithValue("@Recurrence", Recurrence);
                        cmd.Parameters.AddWithValue("@RecurDaily", RecurDaily);

                        cmd.Parameters.AddWithValue("@RecurWeekly", RecurWeekly);

                        cmd.Parameters.AddWithValue("@RecurMonthly", RecurMonthly);
                        cmd.Parameters.AddWithValue("@RecurPerMinutes", RecurPerMinutes);

                        cmd.Parameters.AddWithValue("@RecurEveryDays", RecurEveryDays);
                        cmd.Parameters.AddWithValue("@NumberOfDays", NumberOfDays);
                        cmd.Parameters.AddWithValue("@RecurEveryWeekDay", RecurEveryWeekDay);

                        cmd.Parameters.AddWithValue("@RecurPerMinutesValue", RecurPerMinutesValue);

                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //rows number of record got updated

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
        //
        // UPDATE Update Record In JTMEventSchedules by seq no
        // 
        public void UpdateRecordInJTMEventSchedules(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[JTMEventSchedules] SET "
                           + " ScheduleID = @ScheduleID"
                           + " ,EventType = @EventType"
                           + ", DateLastUpdated = @DateLastUpdated"
                           + ", UserId = @UserId"
                           + ", EffectiveDateTmFrom = @EffectiveDateTmFrom"
                           + ", EffectiveDateTmTo = @EffectiveDateTmTo"
                           + ", Recurrence = @Recurrence"
                           + ", RecurDaily = @RecurDaily"
                           + ", RecurWeekly = @RecurWeekly"
                           + ", RecurMonthly = @RecurMonthly"
                           + ", RecurPerMinutes = @RecurPerMinutes"
                           + ", RecurEveryDays = @RecurEveryDays"
                           + ", NumberOfDays = @NumberOfDays"
                           + ", RecurEveryWeekDay = @RecurEveryWeekDay"
                           + ", RecurPerMinutesValue = @RecurPerMinutesValue"
                           + ", Operator = @Operator "
                           + " WHERE SeqNo = @SeqNo", conn))
                    {

                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                        cmd.Parameters.AddWithValue("@ScheduleID", ScheduleID);
                        cmd.Parameters.AddWithValue("@EventType", EventType);

                        cmd.Parameters.AddWithValue("@DateLastUpdated", DateLastUpdated);

                        cmd.Parameters.AddWithValue("@UserId", UserId);

                        cmd.Parameters.AddWithValue("@EffectiveDateTmFrom", EffectiveDateTmFrom);
                        cmd.Parameters.AddWithValue("@EffectiveDateTmTo", EffectiveDateTmTo);

                        cmd.Parameters.AddWithValue("@Recurrence", Recurrence);
                        cmd.Parameters.AddWithValue("@RecurDaily", RecurDaily);

                        cmd.Parameters.AddWithValue("@RecurWeekly", RecurWeekly);

                        cmd.Parameters.AddWithValue("@RecurMonthly", RecurMonthly);
                        cmd.Parameters.AddWithValue("@RecurPerMinutes", RecurPerMinutes);

                        cmd.Parameters.AddWithValue("@RecurEveryDays", RecurEveryDays);
                        cmd.Parameters.AddWithValue("@NumberOfDays", NumberOfDays);
                        cmd.Parameters.AddWithValue("@RecurEveryWeekDay", RecurEveryWeekDay);

                        cmd.Parameters.AddWithValue("@RecurPerMinutesValue", RecurPerMinutesValue);

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

        //
        // DELETE Record In JTMJournalLoadingSchedules by SeqNo
        //
        public void DeleteRecordInJTMIdentificationDetailsByAtmNo(int InSeqNo)
        {

            ErrorFound = false;
            ErrorOutput = "";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("DELETE FROM [ATMS].[dbo].[JTMEventSchedules] "
                            + " WHERE SeqNo = @SeqNo ", conn))
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

       
    }
}
