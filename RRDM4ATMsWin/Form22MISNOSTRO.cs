using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form22MISNOSTRO : Form
    {
           // Variables
        // ATM MAIN FIELDS 
        string AtmNo;
        int CurrentSesNo;
       

     //   int AtmsReplGroup;
    //    int AtmsReconcGroup;
        string CitId;

        string AuthUser;
        string AuthUserPrevious; 

        bool UnderRepl;
        DateTime LastReplDt;
      //  int TypeOfRepl;

        // NULL Values 
        DateTime NextReplDt;

    

     //   int TotNotRepl;
        int TotRepl;
        int result1;
    //    int result2; 

        int I; 

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        DataTable MISRepl1 = new DataTable();

        RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms(); 
        RRDMUsersRecords Ua = new RRDMUsersRecords();

        RRDMReplStatsClass Rs = new RRDMReplStatsClass();

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

        RRDMSessionsTracesReadUpdate Ta = new RRDMSessionsTracesReadUpdate();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMEmailClass2 Em = new RRDMEmailClass2();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        //multilingual
        CultureInfo culture;

        RRDMUsersRecords Xa = new RRDMUsersRecords(); // Make class availble 

        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);
   //
        string WSignedId;
        int WSignRecordNo;
     //   int WSecLevel;
     
     //   bool WPrive;
        string WUserId;
        DateTime WDtTm;

        string WOperator;
       
        string WBankId;
        string WAtmNo; 
        int WAction; 

        // Methods 
        // READ ATMs Main
        // 
       public Form22MISNOSTRO(string InSignedNo, int SignRecordNo, string InOperator,  string InUserId,
           DateTime InDtTm, string InBankId, string InAtmNo, int InAction)
        {
            WSignedId = InSignedNo;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
         //   WPrive = InPrive;
            WUserId = InUserId;
            WDtTm = InDtTm;
          
            WBankId = InBankId;
            WAtmNo = InAtmNo; 
            WAction = InAction; // 1= Show all NOT replenished, 
                                // 2= Show all replenished by USER, 
                                // 3= Show the ones under Replenishment 
                                // 11= Show all Non Reconciled Once , 
                                // 12= Not RECONCILED More than once 
                                // 21= Show Replenish STATS records for a DAy. 
                               // 22 = Show for a user per day 
                               // 23 = Show Bank Volume for Particular day 
                               // 24 = SHOW ATMS OF A PARTICULAR CitId 
                               // 25 = SHOW Diputes OF A PARTICULAR date 
                               // 26 = Show ATM Profitabilty per period 

            InitializeComponent();

            if (WAction == 1) label14.Text = "NOT REPLENISH YET";
            if (WAction == 2) label14.Text = "REPLENISHED ATMS";
            if (WAction == 3) label14.Text = "CURRENTLY UNDER REPLENISHMENT";
            if (WAction == 11) label14.Text = "NOT RECONCILED FOR TODAY";
            if (WAction == 12) label14.Text = "NOT RECONCILED FOR MORE THAN ONE DAY";
            if (WAction == 21) label14.Text = "LISTING OF KEY PERFORMANCE BY External Bank ";
            if (WAction == 22) label14.Text = "LISTING OF KEY PERFORMANCE BY DATE FOR USER : " + WUserId.ToString(); 
            if (WAction == 23) label14.Text = "LISTING OF BANK VOLUMES AND INDICATORS FOR : " + WBankId;
            if (WAction == 25) label14.Text = "LISTING OF ATMs DISPUTES FOR Date : " + WDtTm.ToShortDateString();
            if (WAction == 26) label14.Text = "PROFITABILITY PER PERIOD FOR ATM : " + WAtmNo;
        }
       //
       // With LOAD LOAD DATA GRID
       //

       private void Form22MIS_Load(object sender, EventArgs e)
       {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

           if (Usi.Culture == "English")
           {
               culture = CultureInfo.CreateSpecificCulture("el-GR");
           }
           if (Usi.Culture == "Français")
           {
               culture = CultureInfo.CreateSpecificCulture("fr-FR");
           }

           if (WAction == 1) label14.Text = LocRM.GetString("Form22MISlabel14a", culture);
           if (WAction == 2) label14.Text = LocRM.GetString("Form22MISlabel14b", culture);

           DataTable WorkingTable = new DataTable();
           WorkingTable = new DataTable();
           WorkingTable.Clear();

           if (WAction == 1 || WAction == 2 || WAction == 3)
           {
               // DATA TABLE ROWS DEFINITION 
               //

               WorkingTable.Columns.Add("AtmNo", typeof(string));
               WorkingTable.Columns.Add("Resp_Branch", typeof(string));
               WorkingTable.Columns.Add("PlannedDate", typeof(DateTime));
               if (WAction == 2)
               {
                   WorkingTable.Columns.Add("ReplMinutes", typeof(int));
                   WorkingTable.Columns.Add("Errors", typeof(int));
                   WorkingTable.Columns.Add("Diff", typeof(decimal));
               }
               WorkingTable.Columns.Add("UserId", typeof(string));
               WorkingTable.Columns.Add("UserName", typeof(string));
               WorkingTable.Columns.Add("email", typeof(string));
               WorkingTable.Columns.Add("Mobile", typeof(string));

               bool Consider = false;

               string SqlString = "SELECT *"
                   + " FROM [dbo].[AtmsMain] "
                   + " WHERE Operator=@Operator "
                   + " ORDER BY AuthUser ";

               using (SqlConnection conn =
                             new SqlConnection(connectionString))
                   try
                   {
                       conn.Open();
                       using (SqlCommand cmd =
                           new SqlCommand(SqlString, conn))
                       {
                           cmd.Parameters.AddWithValue("@Operator", WOperator);

                           // Read table 

                           SqlDataReader rdr = cmd.ExecuteReader();

                           //TESTING
                           DateTime WReplDate = DateTime.Today;
                           WReplDate = new DateTime(2014, 04, 18); // IF NOT TESTING THIS IS TODAY

                           while (rdr.Read())
                           {

                               // Check wether is under consideration 
                               CitId = (string)rdr["CitId"];

                               //if (WCitId == CitId)
                               //{
                               //    Consider = true; // We have A CIT ATM
                               //}
                               //else Consider = false;


                               if (Consider == true) // DO FOR ALL RELATED 
                               {
                                   I = I + 1;

                                   AtmNo = (string)rdr["AtmNo"];

                                   CurrentSesNo = (int)rdr["CurrentSesNo"];

                                   LastReplDt = (DateTime)rdr["LastReplDt"];

                                   NextReplDt = (DateTime)rdr["NextReplDt"];

                                   AuthUser = (string)rdr["AuthUser"];

                                   // If Next Repl date is today and last Repl date is less than today then = delayed Repl 
                                   // Next repl > today and and last repl = today => Repl doone today
                                   // Next repl > today and last repl < today then not in question = do nothing 

                                   if (NextReplDt != NullPastDate)
                                   {
                                       result1 = DateTime.Compare(LastReplDt.Date, WReplDate.Date);
                                       if (result1 == 0) // Equal dates 
                                       {
                                           //  Repl Done today 

                                           TotRepl = TotRepl + 1;


                                           if (WAction == 2)
                                           {

                                               DataRow RowJ = WorkingTable.NewRow();

                                               RowJ["AtmNo"] = AtmNo;
                                               RowJ["Resp_Branch"] = (string)rdr["RespBranch"];
                                               RowJ["PlannedDate"] = (DateTime)rdr["NextReplDt"];

                                               Rs.ReadReplStatClassSpecific(AtmNo, CurrentSesNo);

                                               RowJ["ReplMinutes"] = Rs.ReplMinutes;
                                               RowJ["Errors"] = Rs.ErrorsAtm + Rs.ErrorsHost;
                                               RowJ["Diff"] = Rs.DiffMinus + Rs.DiffPlus;

                                               RowJ["UserId"] = Rs.UserId;
                                               RowJ["UserName"] = Rs.UserName;

                                               // FIND USER Details 

                                               Ua.ReadUsersRecord(Rs.UserId); // Get Info for User 

                                               RowJ["email"] = Ua.email;
                                               RowJ["Mobile"] = Ua.MobileNo;

                                               WorkingTable.Rows.Add(RowJ);
                                           }
                                       }

                                       result1 = DateTime.Compare(NextReplDt.Date, WReplDate.Date);

                                       if (result1 == 0 || result1 < 0) // Equal dates or less
                                       {
                                           // Not done Repl
                                           if (WAction == 1)
                                           {
                                               DataRow RowJ = WorkingTable.NewRow();

                                               RowJ["AtmNo"] = AtmNo;
                                               RowJ["Resp_Branch"] = (string)rdr["RespBranch"];
                                               RowJ["PlannedDate"] = (DateTime)rdr["NextReplDt"];

                                               // FIND USER FOR THIS ATM

                                               Ac.ReadAtm(AtmNo);

                                               if (Ac.AtmsReplGroup > 0) Uaa.FindUserForRepl("", Ac.AtmsReplGroup);

                                               else Uaa.FindUserForRepl(AtmNo, 0);
                                               //TEST
                                               if (Uaa.RecordFound == false)
                                               {
                                                   RowJ["UserId"] = 1121;
                                                   RowJ["UserName"] = "Alexandros Georgiou";
                                                   RowJ["email"] = "a.georgiou@cytanet.com.cy";
                                                   RowJ["Mobile"] = "99039438048";
                                               }
                                               else
                                               {
                                                   Ua.ReadUsersRecord(Uaa.UserId); // Get Info for User 

                                                   RowJ["UserId"] = Ua.UserId;
                                                   RowJ["UserName"] = Ua.UserName;
                                                   RowJ["email"] = Ua.email;
                                                   RowJ["Mobile"] = Ua.MobileNo;
                                               }


                                               

                                               WorkingTable.Rows.Add(RowJ);
                                           }
                                       }
                                       if (result1 > 0) // Bigger than today
                                       {
                                           // Future Repl
                                       }
                                   }

                                   // Under Replenishment 

                                   if (WAction == 3)
                                   {


                                       Ta.ReadSessionsStatusTraces(AtmNo, CurrentSesNo);

                                       if (Ta.ProcessMode == -1) // ATM is still in Process
                                       {
                                           // Find previous session Trace 

                                           Ta.ReadSessionsStatusTraces(AtmNo, Ta.PreSes);

                                       }

                                       if (Ta.RecordFound == true)
                                       {
                                           // Check the currently under Replenishment
                                           if (Ta.ProcessMode == 0)
                                           {
                                               UnderRepl = true;
                                           }
                                           else UnderRepl = false;
                                       }
                                       if (UnderRepl == true)
                                       {
                                           // CHECK IF TWO ATMS ARE Replenished at the same time 
                                           if (AuthUser == AuthUserPrevious)
                                           {
                                               // Send email to controller 
                                               string Recipient = "panicos.michael@cablenet.com.cy";

                                               string Subject = "Two ATMs Replenished at the same time.";

                                               string EmailBody = "User: " + AuthUser + " is Replenishing the two ATMs at the same time";

                                               Em.SendEmail(WOperator, Recipient, Subject, EmailBody);
                                               //      if (Em.MessageSent == true) MessageBox.Show("Email to:" + Recipient + " Has been sent");

                                           }

                                           AuthUserPrevious = AuthUser;

                                           // FILL IN TABLE FOR UNDER REPL ATMS 

                                           DataRow RowJ = WorkingTable.NewRow();

                                           RowJ["AtmNo"] = AtmNo;
                                           RowJ["Resp_Branch"] = (string)rdr["RespBranch"];
                                           RowJ["PlannedDate"] = (DateTime)rdr["NextReplDt"];

                                           // FIND USER FOR THIS ATM

                                           Ac.ReadAtm(AtmNo);

                                           if (Ac.AtmsReplGroup > 0) Uaa.FindUserForRepl("", Ac.AtmsReplGroup);

                                           else Uaa.FindUserForRepl(AtmNo, 0);

                                           //TEST
                                           if (Uaa.RecordFound == false)
                                           {
                                               RowJ["UserId"] = 1121;
                                               RowJ["UserName"] = "Alexandros Georgiou";
                                               RowJ["email"] = "a.georgiou@cytanet.com.cy";
                                               RowJ["Mobile"] = "99039438048";
                                           }
                                           else
                                           {
                                               Ua.ReadUsersRecord(Uaa.UserId); // Get Info for User 

                                               RowJ["UserId"] = Ua.UserId;
                                               RowJ["UserName"] = Ua.UserName;
                                               RowJ["email"] = Ua.email;
                                               RowJ["Mobile"] = Ua.MobileNo;
                                           }

                                           WorkingTable.Rows.Add(RowJ);
                                       }

                                   }

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

                       string exception = ex.ToString();
                       MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                   }

           }

           if (WAction == 11 || WAction == 12)
           {
               // DATA TABLE ROWS DEFINITION 
               //

               WorkingTable.Columns.Add("AtmNo", typeof(string));
               WorkingTable.Columns.Add("Resp_Branch", typeof(string));
               WorkingTable.Columns.Add("Errors", typeof(int));
               WorkingTable.Columns.Add("ErrorsATM", typeof(int));
               WorkingTable.Columns.Add("ErrorsHost", typeof(int));
               WorkingTable.Columns.Add("DiffAmount", typeof(decimal));
               WorkingTable.Columns.Add("CyclesInDiff", typeof(int));
               WorkingTable.Columns.Add("UserId", typeof(int));
               WorkingTable.Columns.Add("UserName", typeof(string));
               WorkingTable.Columns.Add("email", typeof(string));
               WorkingTable.Columns.Add("Mobile", typeof(string));

               bool Consider = false;

               string SqlString = "SELECT *"
                   + " FROM [dbo].[AtmsMain] "
                   + " WHERE Operator=@Operator AND ReconcDiff = 1";

               using (SqlConnection conn =
                             new SqlConnection(connectionString))
                   try
                   {
                       conn.Open();
                       using (SqlCommand cmd =
                           new SqlCommand(SqlString, conn))
                       {
                           cmd.Parameters.AddWithValue("@Operator", WOperator);

                           // Read table 

                           SqlDataReader rdr = cmd.ExecuteReader();

                           while (rdr.Read())
                           {

                               AtmNo = (string)rdr["AtmNo"];
                               // Check wether is under consideration 
                               CitId = (string)rdr["CitId"];

                               //if (WCitId == CitId)
                               //{
                               //    Consider = true; // We have A CIT ATM
                               //}
                               //else Consider = false;


                               if (Consider == true) // DO FOR ALL RELATED 
                               {
                                   I = I + 1;

                                   CurrentSesNo = (int)rdr["CurrentSesNo"];

                                   LastReplDt = (DateTime)rdr["LastReplDt"];

                                   NextReplDt = (DateTime)rdr["NextReplDt"];

                                   //        AtmsReconcGroup = (int)rdr["AtmsReconcGroup"];

                                   Ta.ReadSessionsStatusTraces(AtmNo, CurrentSesNo);

                                   if (WAction == 11 & Ta.SessionsInDiff == 1)
                                   {

                                       DataRow RowJ = WorkingTable.NewRow();

                                       RowJ["AtmNo"] = AtmNo;
                                       RowJ["Resp_Branch"] = (string)rdr["RespBranch"];

                                       Na.ReadAllErrorsTable(AtmNo, CurrentSesNo);

                                       RowJ["Errors"] = Na.NumberOfErrors;
                                       RowJ["ErrorsATM"] = Na.NumberOfErrJournal;
                                       RowJ["ErrorsHost"] = Na.NumberOfErrHost;
                                       // Assign Ta
                                       RowJ["DiffAmount"] = Ta.Diff1.DiffCurr1;
                                       RowJ["CyclesInDiff"] = Ta.SessionsInDiff;

                                       // FIND USER FOR THIS ATM

                                       Ac.ReadAtm(AtmNo);

                                       if (Ac.AtmsReplGroup > 0) Uaa.FindUserForRepl("", Ac.AtmsReplGroup);

                                       else Uaa.FindUserForRepl(AtmNo, 0);

                                       //TEST
                                       if (Uaa.RecordFound == false)
                                       {
                                           RowJ["UserId"] = 1121;
                                           RowJ["UserName"] = "Alexandros Georgiou";
                                           RowJ["email"] = "a.georgiou@cytanet.com.cy";
                                           RowJ["Mobile"] = "99039438048";
                                       }
                                       else
                                       {
                                           Ua.ReadUsersRecord(Uaa.UserId); // Get Info for User 

                                           RowJ["UserId"] = Ua.UserId;
                                           RowJ["UserName"] = Ua.UserName;
                                           RowJ["email"] = Ua.email;
                                           RowJ["Mobile"] = Ua.MobileNo;
                                       }

                                       WorkingTable.Rows.Add(RowJ);
                                   }

                                   if (WAction == 12 & Ta.SessionsInDiff > 1)
                                   {

                                       DataRow RowJ = WorkingTable.NewRow();

                                       RowJ["AtmNo"] = AtmNo;
                                       RowJ["Resp_Branch"] = (string)rdr["RespBranch"];

                                       Na.ReadAllErrorsTable(AtmNo, CurrentSesNo);

                                       RowJ["Errors"] = Na.NumberOfErrors;
                                       RowJ["ErrorsATM"] = Na.NumberOfErrJournal;
                                       RowJ["ErrorsHost"] = Na.NumberOfErrHost;
                                       // Assign Ta
                                       RowJ["DiffAmount"] = Ta.Diff1.DiffCurr1;
                                       RowJ["CyclesInDiff"] = Ta.SessionsInDiff;
                                       // FIND USER FOR THIS ATM

                                       Ac.ReadAtm(AtmNo);

                                       if (Ac.AtmsReplGroup > 0) Uaa.FindUserForRepl("", Ac.AtmsReplGroup);

                                       else Uaa.FindUserForRepl(AtmNo, 0);
                                       //TEST
                                       if (Uaa.RecordFound == false)
                                       {
                                           RowJ["UserId"] = 1121;
                                           RowJ["UserName"] = "Alexandros Georgiou";
                                           RowJ["email"] = "a.georgiou@cytanet.com.cy";
                                           RowJ["Mobile"] = "99039438048";
                                       }
                                       else
                                       {
                                           Ua.ReadUsersRecord(Uaa.UserId); // Get Info for User 

                                           RowJ["UserId"] = Ua.UserId;
                                           RowJ["UserName"] = Ua.UserName;
                                           RowJ["email"] = Ua.email;
                                           RowJ["Mobile"] = Ua.MobileNo;
                                       }

                                       WorkingTable.Rows.Add(RowJ);
                                   }
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

                       string exception = ex.ToString();
                       MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                   }
           }

           if (WAction == 21) // List Indicators for Bank 
           {

               MISRepl1 = new DataTable();
               MISRepl1.Clear();

                string SqlString2 =
               " SELECT "
               + " ExternalBank AS BANK, "
               + " Count(CategoryId) As No_Categories,"
               //+ " Count(CASE WHEN [StatementLoaded]=1 THEN 1 END) As LoadedStmts, "
               //+ " Count(CASE WHEN [StatementLoaded]=0 THEN 1 END) As NotLoadedStmts, "
               + " SUM(TotalNumberProcessed) As TotalNumberProcessed, "
               + " SUM(MatchedDefault) As MatchedDefault,"
               + " SUM(AutoButToBeConfirmed) As AutoButToBeConfirmed, "
               + " PercentEfficiency = (SUM(MatchedDefault) + SUM(AutoButToBeConfirmed))*100/SUM(TotalNumberProcessed) , "
               + " SUM(OutstandingAlerts) As OutstandingAlerts,"
               + " SUM(OutstandingDisputes) As OutstandingDisputes,"
               + " SUM(ManualToBeConfirmed) As ManualToBeConfirmed,"
               + " SUM(MatchedFromAutoToBeConfirmed) As MatchedFromAutoToBeConfirmed ,"
               + " SUM(MatchedFromManualToBeConfirmed) As MatchedFromManualToBeConfirmed,"
               + " UnMatchedNo = SUM(TotalNumberProcessed - "
               + " (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) , "
               + " count(CASE WHEN(TotalNumberProcessed - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) > 0 THEN 1 END) As CatUnMatched, "
               + " SUM(UnMatchedAmt) As UnMatchedAmt"
               + " FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
               + " WHERE Operator = @Operator AND CAST(StartDailyProcess AS Date) = @WorkingDtA  "
               + " GROUP BY ExternalBank "
               + " ORDER BY ExternalBank DESC "
               ;
               using (SqlConnection conn =
                           new SqlConnection(connectionString))
                   try
                   {
                       conn.Open();

                       //Create an Sql Adapter that holds the connection and the command
                       SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);


                       sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@WorkingDtA", WDtTm);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(MISRepl1);

                       //Fill the dataGrid that will be displayed with the dataset
                       dataGridView1.DataSource = MISRepl1.DefaultView;

                       // Close conn
                       conn.Close();

                   }

                   catch (Exception ex)
                   {

                       string exception = ex.ToString();

                   }


               dataGridView1.Columns[0].Width = 85; // 
               dataGridView1.Columns[1].Width = 65; // 
               dataGridView1.Columns[2].Width = 65; // 
               dataGridView1.Columns[3].Width = 65; //
               dataGridView1.Columns[4].Width = 65; // 
               dataGridView1.Columns[5].Width = 65; // 
               dataGridView1.Columns[6].Width = 65; // 
               dataGridView1.Columns[7].Width = 65; // 
               dataGridView1.Columns[8].Width = 65; // 
               dataGridView1.Columns[9].Width = 100; // 


               //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
               //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
               //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


               dataGridView1.Show();

           }

           if (WAction == 22) // List a user within day 
           {
               //   labelStep1.Text = "User Performance Analysis"; 


               MISRepl1 = new DataTable();
               MISRepl1.Clear();

               string SqlString2 =
                " SELECT "
              + " CAST(StartDailyProcess AS Date), "
              + " Count(CategoryId) As No_Categories,"
              //+ " Count(CASE WHEN [StatementLoaded]=1 THEN 1 END) As LoadedStmts, "
              //+ " Count(CASE WHEN [StatementLoaded]=0 THEN 1 END) As NotLoadedStmts, "
              + " SUM(TotalNumberProcessed) As TotalNumberProcessed, "
              + " SUM(MatchedDefault) As MatchedDefault,"
              + " SUM(AutoButToBeConfirmed) As AutoButToBeConfirmed, "
              + " PercentEfficiency = (SUM(MatchedDefault) + SUM(AutoButToBeConfirmed))*100/SUM(TotalNumberProcessed) , "
              + " SUM(OutstandingAlerts) As OutstandingAlerts,"
              + " SUM(OutstandingDisputes) As OutstandingDisputes,"
              + " SUM(ManualToBeConfirmed) As ManualToBeConfirmed,"
              + " SUM(MatchedFromAutoToBeConfirmed) As MatchedFromAutoToBeConfirmed ,"
              + " SUM(MatchedFromManualToBeConfirmed) As MatchedFromManualToBeConfirmed,"
              + " UnMatchedNo = SUM(TotalNumberProcessed - "
              + " (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) , "
              + " count(CASE WHEN(TotalNumberProcessed - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) > 0 THEN 1 END) As CatUnMatched, "
              + " SUM(UnMatchedAmt) As UnMatchedAmt"
              + " FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
              + " WHERE Operator = @Operator AND OwnerId = @OwnerId  "
              + " GROUP BY CAST(StartDailyProcess AS Date) "
              + " ORDER BY CAST(StartDailyProcess AS Date) DESC "
              ;

                using (SqlConnection conn =
                           new SqlConnection(connectionString))
                   try
                   {
                       conn.Open();

                       //Create an Sql Adapter that holds the connection and the command
                       SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@OwnerId", WUserId);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                       //Create a datatable that will be filled with the data retrieved from the command
                       //    DataSet MISds = new DataSet();
                       sqlAdapt.Fill(MISRepl1);

                       //Fill the dataGrid that will be displayed with the dataset
                       dataGridView1.DataSource = MISRepl1.DefaultView;

                       // Close conn
                       conn.Close();

                   }

                   catch (Exception ex)
                   {

                       string exception = ex.ToString();
                       MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                   }

               //  MessageBox.Show(" Number of ATMS = " + I);

               dataGridView1.Columns[0].Width = 85; // 
               //dataGridView1.Columns[1].Width = 65; // 
               //dataGridView1.Columns[2].Width = 65; // 
               //dataGridView1.Columns[3].Width = 65; //
               //dataGridView1.Columns[4].Width = 65; // 
               //dataGridView1.Columns[5].Width = 65; // 
               //dataGridView1.Columns[6].Width = 65; // 
               //dataGridView1.Columns[7].Width = 65; // 
               //dataGridView1.Columns[8].Width = 65; // 
               //dataGridView1.Columns[9].Width = 65; // 
               //dataGridView1.Columns[10].Width = 65; // 
               //dataGridView1.Columns[11].Width = 100; // 


               //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
               //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
               //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


               dataGridView1.Show();

           }

           if (WAction == 23) // List of ATMs Business for a particular date 
           {
               //   labelStep1.Text = "User Performance Analysis"; 

               MISRepl1 = new DataTable();
               MISRepl1.Clear();

                string SqlString2 =
           " SELECT "
            + " CAST(StartDailyProcess AS Date), "
            + " Count(CategoryId) As No_Categories,"
            //+ " Count(CASE WHEN [StatementLoaded]=1 THEN 1 END) As LoadedStmts, "
            //+ " Count(CASE WHEN [StatementLoaded]=0 THEN 1 END) As NotLoadedStmts, "
            + " SUM(TotalNumberProcessed) As TotalNumberProcessed, "
            + " SUM(MatchedDefault) As MatchedDefault,"
            + " SUM(AutoButToBeConfirmed) As AutoButToBeConfirmed, "
            + " PercentEfficiency = (SUM(MatchedDefault) + SUM(AutoButToBeConfirmed))*100/SUM(TotalNumberProcessed) , "
            + " SUM(OutstandingAlerts) As OutstandingAlerts,"
            + " SUM(OutstandingDisputes) As OutstandingDisputes,"
            + " SUM(ManualToBeConfirmed) As ManualToBeConfirmed,"
            + " SUM(MatchedFromAutoToBeConfirmed) As MatchedFromAutoToBeConfirmed ,"
            + " SUM(MatchedFromManualToBeConfirmed) As MatchedFromManualToBeConfirmed,"
            + " UnMatchedNo = SUM(TotalNumberProcessed - "
            + " (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) , "
            + " count(CASE WHEN(TotalNumberProcessed - (MatchedDefault + MatchedFromAutoToBeConfirmed + MatchedFromManualToBeConfirmed)) > 0 THEN 1 END) As CatUnMatched, "
            + " SUM(UnMatchedAmt) As UnMatchedAmt"
            + " FROM [ATMS].[dbo].[NVReconcCategoriesSessions]"
            + " WHERE Operator = @Operator AND ExternalBank = @ExternalBank  "
            + " GROUP BY CAST(StartDailyProcess AS Date) "
            + " ORDER BY CAST(StartDailyProcess AS Date) DESC "
            ;


                using (SqlConnection conn =
                           new SqlConnection(connectionString))
                   try
                   {
                       conn.Open();

                       //Create an Sql Adapter that holds the connection and the command
                       SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@ExternalBank", WBankId);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(MISRepl1);

                       //Fill the dataGrid that will be displayed with the dataset
                       dataGridView1.DataSource = MISRepl1.DefaultView;

                       // Close conn

                       conn.Close();

                   }

                   catch (Exception ex)
                   {

                       string exception = ex.ToString();
                       MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                   }

               dataGridView1.Columns[0].Width = 85; // 
               dataGridView1.Columns[1].Width = 75; // 
               dataGridView1.Columns[2].Width = 75; // 
               dataGridView1.Columns[3].Width = 75; //
               dataGridView1.Columns[4].Width = 75; // 


               //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
               //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
               //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


               dataGridView1.Show();

           }

          

           if (WAction == 25) // List of ATMs Business for a particular date 
           {
               //   labelStep1.Text = "User Performance Analysis"; 

               MISRepl1 = new DataTable();
               MISRepl1.Clear();

               string SqlString2 =
                   " SELECT [DisputeNumber],[DispDtTm],[TranDate],[TransDesc]"
                            + ",[AtmNo],[CardNo],[CurrencyNm],[TranAmount]"
                            + ",[DisputedAmt],[DecidedAmount],[ClosedDispute]"
                            + " FROM [ATMS].[dbo].[DisputesTransTable]"
                            + " WHERE CAST(DispDtTm AS DATE) = @DispDtTm AND Operator = @Operator"
                            + " ORDER BY DisputeNumber DESC ";


               using (SqlConnection conn =
                           new SqlConnection(connectionString))
                   try
                   {
                       conn.Open();

                       //Create an Sql Adapter that holds the connection and the command
                       SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                       sqlAdapt.SelectCommand.Parameters.AddWithValue("@DispDtTm", WDtTm);

                       sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                       //Create a datatable that will be filled with the data retrieved from the command
                       //    DataSet MISds = new DataSet();
                       sqlAdapt.Fill(MISRepl1);

                       //Fill the dataGrid that will be displayed with the dataset
                       dataGridView1.DataSource = MISRepl1.DefaultView;

                       // Close conn
                       conn.Close();

                   }

                   catch (Exception ex)
                   {

                       string exception = ex.ToString();
                       MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                   }

               //    MessageBox.Show(" Size of datagrid:" + dataGridView1.Rows.Count.ToString()); 

               dataGridView1.Columns[0].Width = 75; // 
               dataGridView1.Columns[1].Width = 75; // 
               dataGridView1.Columns[2].Width = 75; // 
               dataGridView1.Columns[3].Width = 75; //
               dataGridView1.Columns[4].Width = 75; // 
               dataGridView1.Columns[5].Width = 75; // 
               dataGridView1.Columns[6].Width = 75; // 
               dataGridView1.Columns[7].Width = 75; // 
               dataGridView1.Columns[8].Width = 75; //
               dataGridView1.Columns[9].Width = 75; // 
               dataGridView1.Columns[10].Width = 75; // 

               //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
               //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
               //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


               dataGridView1.Show();

           }

           if (WAction == 26) // FOR AN ATM LIST PROFITABILITY  
           {
               //   labelStep1.Text = "User Performance Analysis"; 

               MISRepl1 = new DataTable();
               MISRepl1.Clear();

               string SqlString2 =
          " SELECT CAST(DtTm AS Date) As Date "
              + " , Sum ([C301DailyMaintAmount] + [C303ReplTimeCost] +[C307OverheadCost] + [C308CostOfMoney]+ [C309CostOfInvest]) As COST"
              + " , Sum ( [R401CommTran] + [R402CommTran] + [R403CommTran] + [R404CommTran] + [R405CommTran]) As REVENEW"
              + " , Sum ( ( [R401CommTran] + [R402CommTran] + [R403CommTran] + [R404CommTran] + [R405CommTran])"
                    + "- ([C301DailyMaintAmount] + [C303ReplTimeCost] +[C307OverheadCost] + [C308CostOfMoney]+ [C309CostOfInvest])) AS PROFIT "
              + " , Sum([C301DailyMaintAmount]) As MaintCost, Sum([C303ReplTimeCost]) As ReplCost"
              + ", Sum([C307OverheadCost]) As Overheads, Sum([C308CostOfMoney]) As CostOfMoney "
              + ", Sum([C309CostOfInvest]) As InvestCost"
              + ", Sum([R401CommTran]) As Comm1Tran , Sum([R401CommAmount]) As Comm1Amount"
              + ", Sum([R402CommTran]) As Comm2Tran , Sum([R402CommAmount]) As Comm2Amount"
              + ", Sum([R403CommTran]) As Comm3Tran , Sum([R403CommAmount]) As Comm3Amount"
              + ", Sum([R404CommTran]) As Comm4Tran , Sum([R404CommAmount]) As Comm4Amount"
              + " , Sum([R405CommTran]) As Comm5Tran , Sum([R405CommAmount]) As Comm5Amount"
        + " FROM [ATMS].[dbo].[AtmDispAmtsByDay] "
          + " WHERE Operator = @Operator AND AtmNo =@AtmNo"
          + " GROUP BY AtmNo, CAST(DtTm AS Date)  "
          + " ORDER BY CAST(DtTm AS Date) DESC ";


               using (SqlConnection conn =
                           new SqlConnection(connectionString))
                   try
                   {
                       conn.Open();

                       //Create an Sql Adapter that holds the connection and the command
                       SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                       sqlAdapt.SelectCommand.Parameters.AddWithValue("@AtmNo", WAtmNo);

                       sqlAdapt.SelectCommand.Parameters.AddWithValue("@Operator", WOperator);

                       //Create a datatable that will be filled with the data retrieved from the command
                       //    DataSet MISds = new DataSet();
                       sqlAdapt.Fill(MISRepl1);

                       //Fill the dataGrid that will be displayed with the dataset
                       dataGridView1.DataSource = MISRepl1.DefaultView;

                       // Close conn
                       conn.Close();

                   }

                   catch (Exception ex)
                   {

                       string exception = ex.ToString();
                       MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                   }

               //    MessageBox.Show(" Size of datagrid:" + dataGridView1.Rows.Count.ToString()); 

               dataGridView1.Columns[0].Width = 75; // 
               dataGridView1.Columns[1].Width = 75; // 
               dataGridView1.Columns[2].Width = 75; // 
               dataGridView1.Columns[3].Width = 75; //
               dataGridView1.Columns[4].Width = 75; // 
               dataGridView1.Columns[5].Width = 75; // 
               dataGridView1.Columns[6].Width = 75; // 
               dataGridView1.Columns[7].Width = 75; // 
               dataGridView1.Columns[8].Width = 75; //
               dataGridView1.Columns[9].Width = 75; // 
               dataGridView1.Columns[10].Width = 75; // 

               //     dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
               //     dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
               //     dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);


               dataGridView1.Show();

           }

           // 1= Show all NOT replenished, 
           // 2= Show all replenished by USER, 
           // 3= Under Replenishment 
           // 11= Show all Non Reconciled Once ,
           // 12= Not RECONCILED More than once 


           // SHOW DATA GRID ..  LIST for 1, 2, 11 etc 
           if (WAction == 1 || WAction == 2 || WAction == 3 || WAction == 11 || WAction == 12)
           {
               dataGridView1.DataSource = WorkingTable.DefaultView;

               //      dataGridView1.Columns[0].Width = 60;
               //       dataGridView1.Columns[1].Width = 350;
               //       dataGridView1.Columns[2].Width = 60;

               //       dataGridView1.Columns[0].HeaderText = LocRM.GetString("Form67Grd1Cl0", culture);
               //       dataGridView1.Columns[1].HeaderText = LocRM.GetString("Form67Grd1Cl01", culture);
               //      dataGridView1.Columns[2].HeaderText = LocRM.GetString("Form67Grd1Cl02", culture);

               if (WAction == 2)
               {

                   dataGridView1.Columns[0].Width = 65; // 
                   dataGridView1.Columns[1].Width = 65; // 
                   dataGridView1.Columns[2].Width = 65; // 
                   dataGridView1.Columns[3].Width = 65; //
                   dataGridView1.Columns[4].Width = 65; // 
                   dataGridView1.Columns[5].Width = 65; //
                   dataGridView1.Columns[6].Width = 65; // 
                   dataGridView1.Columns[7].Width = 100; // NAME 
                   dataGridView1.Columns[8].Width = 65; // 
                   dataGridView1.Columns[9].Width = 65; // 

                   dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
               }

               dataGridView1.Show();
           }
       }

        }     
           
        }
    

    


