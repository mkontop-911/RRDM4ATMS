using System;
using System.Data;
using System.Text;
//using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Configuration;
//using System.Collections;

namespace RRDM4ATMs
{
    public class RRDMSessions_Form153_Corrections : Logger
    {
        public RRDMSessions_Form153_Corrections() : base() { }

//        SeqNo int Unchecked
//AtmNo nvarchar(20)    Checked
//Form153Change   nvarchar(100)   Checked
//SesDtTimeStart_Before1  datetime Checked
//ChangeByUser nvarchar(50)    Checked
//RMCycleOfChange int Checked
//ReplCycleNo int Checked
//SesDtTimeStart_Before datetime    Checked
//SesDtTimeEnd_Before datetime Checked
//ProcessMode_Before int Checked
//SesDtTimeStart_After datetime    Checked
//SesDtTimeEnd_After  datetime Checked
//ProcessMode_After int Checked
//LoadedAtRMCycle int Unchecked
//ReconcAtRMCycle int Checked
//Operator nvarchar(8) Checked

        public int SeqNo;

        public string AtmNo;
        public string Form153ChangeDesc;
        public DateTime DtTmAtChange;
        public string ChangeByUser;

        public int RMCycleOfChange;

        public int ReplCycleNo;

        public DateTime SesDtTimeStart_Before;
        public DateTime SesDtTimeEnd_Before;
        public int ProcessMode_Before;

        public DateTime SesDtTimeStart_After;
        public DateTime SesDtTimeEnd_After;
        public int ProcessMode_After;

        public int LoadedAtRMCycle;
        public int ReconcAtRMCycle;
        public string Operator;

        // Define the data table 
        public DataTable ChangesInForm153Table = new DataTable();
        public int TotalSelected;

        public bool InspectionAlert; 

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;
        readonly string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        public void ReadSessions_Form153_Corrections_Fill_Table(string InAtmNo)
        {

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            ChangesInForm153Table = new DataTable();
            ChangesInForm153Table.Clear();

            TotalSelected = 0;
         
            //***************************************************
            string SqlString = "SELECT * "
               + " FROM [ATMS].[dbo].[Sessions_Form153_Corrections] "
               + " WHERE AtmNo = @AtmNo"
               + "  ORDER by DtTmAtChange DESC "
               ;

            using (SqlConnection conn =
             new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    //Create an Sql Adapter that holds the connection and the command
                    using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString, conn))
                    {

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", InAtmNo);
                       
                        //Create a datatable that will be filled with the data retrieved from the command

                        sqlAdapt.Fill(ChangesInForm153Table);

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


        // Insert File Field Record 
        //
        public int InsertSessions_Form153_Corrections(string InAtmNo, int InSesNo)
        {

            ErrorFound = false;
            ErrorOutput = "";
            int CorSeqNo = 0; 
        //public string AtmNo;
        //public string Form153ChangeDesc;
        //public DateTime DtTmAtChange;
        //public string ChangeByUser;

        //public int RMCycleOfChange;

        //public int ReplCycleNo;

        //public DateTime SesDtTimeStart_Before;
        //public DateTime SesDtTimeEnd_Before;
        //public int ProcessMode_Before;

        //public DateTime SesDtTimeStart_After;
        //public DateTime SesDtTimeEnd_After;
        //public int ProcessMode_After;

        //public int LoadedAtRMCycle;
        //public int ReconcAtRMCycle;
        //public string Operator;

        string cmdinsert = "INSERT INTO [ATMS].[dbo].[Sessions_Form153_Corrections] "
                    + "([AtmNo], [Form153ChangeDesc],[DtTmAtChange], [ChangeByUser], "
                       + " [RMCycleOfChange],[ReplCycleNo] ,"
                         + " [SesDtTimeStart_Before],[SesDtTimeEnd_Before] ,[ProcessMode_Before] ,"
                    + " [LoadedAtRMCycle],[ReconcAtRMCycle] ,[Operator] )"
                    + " VALUES (@AtmNo, @Form153ChangeDesc, @DtTmAtChange, @ChangeByUser, "
                    + " @RMCycleOfChange, @ReplCycleNo, "
                     + " @SesDtTimeStart_Before, @SesDtTimeEnd_Before, @ProcessMode_Before, "
                     + " @LoadedAtRMCycle, @ReconcAtRMCycle, @Operator )"
                    + " SELECT CAST(SCOPE_IDENTITY() AS int)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =

                       new SqlCommand(cmdinsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", AtmNo);
                        cmd.Parameters.AddWithValue("@Form153ChangeDesc", Form153ChangeDesc);
                        cmd.Parameters.AddWithValue("@DtTmAtChange", DtTmAtChange);
                        cmd.Parameters.AddWithValue("@ChangeByUser", ChangeByUser);
                        cmd.Parameters.AddWithValue("@RMCycleOfChange", RMCycleOfChange);
                        cmd.Parameters.AddWithValue("@ReplCycleNo", ReplCycleNo);

                        cmd.Parameters.AddWithValue("@SesDtTimeStart_Before", SesDtTimeStart_Before);
                        cmd.Parameters.AddWithValue("@SesDtTimeEnd_Before", SesDtTimeEnd_Before);
                        cmd.Parameters.AddWithValue("@ProcessMode_Before", ProcessMode_Before);
                        
                        cmd.Parameters.AddWithValue("@LoadedAtRMCycle", LoadedAtRMCycle);
                        cmd.Parameters.AddWithValue("@ReconcAtRMCycle", ReconcAtRMCycle);
                        cmd.Parameters.AddWithValue("@Operator", Operator);

                        //CorSeqNo = cmd.ExecuteNonQuery();
                        CorSeqNo = (int)cmd.ExecuteScalar();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    CatchDetails(ex);
                }

            return CorSeqNo;
        }

        // UPDATE Physical Inspection record 
        // 
        public void UpdateSessions_Form153_Corrections(int InSeqNo)
        {
            ErrorFound = false;
            ErrorOutput = "";
            int rows = 0; 
            //public DateTime SesDtTimeStart_After;
            //public DateTime SesDtTimeEnd_After;
            //public int ProcessMode_After;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE [ATMS].[dbo].[Sessions_Form153_Corrections] SET "
                              + " SesDtTimeStart_After = @SesDtTimeStart_After, "
                              +"  SesDtTimeEnd_After = @SesDtTimeEnd_After , "
                                + " ProcessMode_After = @ProcessMode_After "
                              + " WHERE SeqNo = @SeqNo", conn))

                    {
                        cmd.Parameters.AddWithValue("@SeqNo", InSeqNo);

                   //     cmd.Parameters.AddWithValue("@Selection", Selection);
                        cmd.Parameters.AddWithValue("@SesDtTimeStart_After", SesDtTimeStart_After);
                        cmd.Parameters.AddWithValue("@SesDtTimeEnd_After", SesDtTimeEnd_After);
                        cmd.Parameters.AddWithValue("@ProcessMode_After", ProcessMode_After);

                        rows = cmd.ExecuteNonQuery();        

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



