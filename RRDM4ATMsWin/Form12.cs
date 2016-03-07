using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs; 
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form12 : Form
    {
      
        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;


        Bitmap SCREENinitial;
 

        DataTable NextReplTable10 = new DataTable();

        DataTable TrendTable = new DataTable();

        DateTime WDtTm;

        DateTime NullFutureDate = new DateTime(2050, 11, 21);
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        RRDMReplDatesCalc Rc = new RRDMReplDatesCalc();

        RRDMHolidays Ch = new RRDMHolidays();

        RRDMMatchedDates Cm = new RRDMMatchedDates();

        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        //multilingual
        CultureInfo culture;

        RRDMUsersAndSignedRecord Xa = new RRDMUsersAndSignedRecord(); // Make class availble 

        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

        string WHolidaysVersion; 

        DateTime NextDate ;
        string NMonth;
        string NDay ;
        string NType ;

        DateTime SameAs ;
        string SMonth ;
        string SDay ;
        string SType ;

        string WMatchDatesCateg; 

        int WNextRow;

        string WSignedId;
        int WSignRecordNo;
        int WSecLevel;
        string WOperator;
    
        int WAction;

        public Form12(string InSignedId, int SignRecordNo, int InSecLevel, string InOperator, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
            WAction = InAction;  // 1 = Show ATM Next Replenishement dates and define matching , 

            InitializeComponent();
            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            label7.Text = WOperator;

            //TEST
            dateTimePicker1.Value = new DateTime(2014, 02, 28);
            dateTimePicker3.Value = new DateTime(2014, 03, 20);

            // MATCHED DATES TYPES 

            Gp.ParamId = "209";
            comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            //TEST
            comboBox1.Text = "Touristic";

            // MATCHED DATES TYPES 

            Gp.ParamId = "215";
            comboBox2.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox2.DisplayMember = "DisplayValue";

            label11.Hide();
            label18.Hide();
            label19.Hide();
            label12.Hide();
            panel3.Hide();
            panel4.Hide();
            ButtonFinish.Hide(); 
        }

        // FORM LOAD 
        private void Form12_Load(object sender, EventArgs e)
        {
            textBoxMsgBoard.Text = "Create next Repl Entries for the next period. Change matching date if needed.";

            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENinitial = memoryImage;
        }
       

        // Show Analysis

        private void button3_Click_1(object sender, EventArgs e)
        {
            WHolidaysVersion = comboBox2.Text; 

            Xa.ReadSignedActivityByKey(WSignRecordNo);

            if (Xa.Culture == "English")
            {
                culture = CultureInfo.CreateSpecificCulture("el-GR");
            }
            if (Xa.Culture == "Français")
            {
                culture = CultureInfo.CreateSpecificCulture("fr-FR");
            }

            //      if (WMode == 1) label14.Text = LocRM.GetString("Form67label14a", culture);
            //      if (WMode == 2) label14.Text = LocRM.GetString("Form67label14b", culture);

           
            WMatchDatesCateg = comboBox1.Text; 

            Cm.ReadMatchedLastDate(WOperator, WMatchDatesCateg);

            if (Cm.RecordFound == true) // Warning 
            {
                MessageBox.Show(" Warning: Last date matched:" + Cm.NextDate); 
            }

            label11.Show();
            label18.Show();
            label19.Show();
            label12.Show();
            panel3.Show();
            panel4.Show();
            ButtonFinish.Show();

     
            DateTime StartDt;
            DateTime FinishDt;
            DateTime StartMinus30;

            StartDt = dateTimePicker1.Value.Date;
            FinishDt = dateTimePicker3.Value.Date;
            StartMinus30 = StartDt.AddDays(-30);

            TimeSpan Remain = FinishDt - StartDt;
            label18.Text = (Remain.Days + 1).ToString();


            NextReplTable10 = new DataTable();
            NextReplTable10.Clear();


            if (WAction == 1) // find next dates
            {
                
             // Get next dates with the corresponding past ones 

                //
                // a) If Holiday = last year holiday 
                // b) If Weekend = last month same day
                // c) If Normal = Previous working day 
                int Request = 2; // Give Matching dates for period 
                Rc.GiveMeDataTableOfMatchedDatesOnly(WOperator, StartDt, FinishDt, WMatchDatesCateg, WHolidaysVersion,  Request);  
          
                labelStep1.Text = "Create Matched dates for ATM Group : " + WMatchDatesCateg.ToString();

                NextReplTable10.Columns.Add("Index", typeof(int));

                NextReplTable10.Columns.Add("NextDate", typeof(DateTime));
                NextReplTable10.Columns.Add("NMonth", typeof(string));
                NextReplTable10.Columns.Add("NDay", typeof(string));
                NextReplTable10.Columns.Add("NType", typeof(string));

                NextReplTable10.Columns.Add("SameAs", typeof(DateTime));
                NextReplTable10.Columns.Add("SMonth", typeof(string));
                NextReplTable10.Columns.Add("SDay", typeof(string));
                NextReplTable10.Columns.Add("SType", typeof(string));
                NextReplTable10.Columns.Add("CreatedDt", typeof(DateTime));

                DateTime TempDtTm = StartDt;
                // Fill IN GRIDDAYS BASED ON PUBLIC TABLE dtRDays of RC Class 
                //   DataRow RowGrid = GridDays.NewRow();
                bool WorkingDay;
                bool Weekend;
                bool Holiday;

                int I = 0;

                while (I < (Rc.dtRDays.Rows.Count))
                {
                   
                    DataRow RowTable = NextReplTable10.NewRow();

                    RowTable["Index"] = I + 1;

                    TempDtTm = (DateTime)Rc.dtRDays.Rows[I]["Date"];


                    RowTable["NextDate"] = TempDtTm.Date;
                    RowTable["NMonth"] = TempDtTm.ToString("MMM");
                    RowTable["NDay"] = TempDtTm.Date.DayOfWeek;
                   
                    WorkingDay = (bool)Rc.dtRDays.Rows[I]["Normal"];
                    Weekend = (bool)Rc.dtRDays.Rows[I]["Weekend"];
                    Holiday = (bool)Rc.dtRDays.Rows[I]["Special"];

                    if (WorkingDay == true) RowTable["NType"] = "WorkingDay";
                    if (Weekend == true) RowTable["NType"] = "Weekend";
                    if (Holiday == true) RowTable["NType"] = "Holiday";

                    // See if is already matched 
                    Cm.ReadNextDate(WOperator, WMatchDatesCateg, TempDtTm);
                    if (Cm.RecordFound == true)
                    {
                        RowTable["SameAs"] = Cm.SameAs;
                        RowTable["SMonth"] = Cm.SMonth;
                        RowTable["SDay"] = Cm.SDay;
                        RowTable["SType"] = Cm.SType;
                        RowTable["CreatedDt"] = Cm.DateInsert;
                    }
                    else
                    {
                        TempDtTm = (DateTime)Rc.dtRDays.Rows[I]["SameAsDate"];
                        RowTable["SameAs"] = TempDtTm.Date;
                        RowTable["SMonth"] = TempDtTm.ToString("MMM");
                        RowTable["SDay"] = TempDtTm.Date.DayOfWeek;

                        WorkingDay = (bool)Rc.dtRDays.Rows[I]["SameNormal"];
                        Weekend = (bool)Rc.dtRDays.Rows[I]["SameWeekend"];
                        Holiday = (bool)Rc.dtRDays.Rows[I]["SameSpecial"];

                        if (WorkingDay == true) RowTable["SType"] = "WorkingDay";
                        if (Weekend == true) RowTable["SType"] = "Weekend";
                        if (Holiday == true) RowTable["SType"] = "Holiday";
                    }

                   

                    // THIS FOR DEBUGGING 
                   // string X1 = Rc.dtRDays.Rows[I]["Date"].ToString();
                 //   string X2 = Rc.dtRDays.Rows[I]["RecDispensed"].ToString();

                    //      TotalRepl = TotalRepl + (decimal)Rc.dtRDays.Rows[I]["RecDispensed"];

                    NextReplTable10.Rows.Add(RowTable);

                    I++;
                }


                // ASSIGN AND SHOW GRID

                dataGridView1.DataSource = NextReplTable10.DefaultView;

                dataGridView1.Columns[0].Name = "Index";

                dataGridView1.Columns[1].Name = "NextDate";
                dataGridView1.Columns[2].Name = "NMonth";
                dataGridView1.Columns[3].Name = "NDay";
                dataGridView1.Columns[4].Name = "NType";

                dataGridView1.Columns[5].Name = "SameAs";
                dataGridView1.Columns[6].Name = "SMonth";
                dataGridView1.Columns[7].Name = "SDay";
                dataGridView1.Columns[8].Name = "SType";
                dataGridView1.Columns[9].Name = "CreatedDt";

                // SIZE
                dataGridView1.Columns["Index"].Width = 40; //
                dataGridView1.Columns["NextDate"].Width = 70;
                dataGridView1.Columns["NMonth"].Width = 70;
                dataGridView1.Columns["NDay"].Width = 60;
                dataGridView1.Columns["NType"].Width = 70;
                dataGridView1.Columns["SameAs"].Width = 70;
                dataGridView1.Columns["SMonth"].Width = 70;
                dataGridView1.Columns["SDay"].Width = 60;
                dataGridView1.Columns["SType"].Width = 70;
                dataGridView1.Columns["CreatedDt"].Width = 70;
           
            }
        }
      

        // A ROW IS SELECTED ASSIGN VALUES 
        // ON ROW ENTER DO
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
        
                      WNextRow = (int)rowSelected.Cells[0].Value;
                      WDtTm = (DateTime)rowSelected.Cells[1].Value;
                       textBox4.Text = WDtTm.ToShortDateString();
                       WDtTm = (DateTime)rowSelected.Cells[5].Value;
                       textBox3.Text = WDtTm.ToShortDateString();

                       dateTimePicker2.Value = WDtTm; 
            
        }

        // UPON DATE CHANGE 
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            WDtTm = dateTimePicker2.Value;
            NextReplTable10.Rows[WNextRow - 1]["SameAs"] = WDtTm.Date;

            NextReplTable10.Rows[WNextRow - 1]["SMonth"] = WDtTm.ToString("MMM");
            NextReplTable10.Rows[WNextRow - 1]["SDay"] = WDtTm.DayOfWeek;

            Ch.ReadSpecificDate(WOperator, WDtTm, WHolidaysVersion); // Check the TYPE through Holidays Class

            if (Ch.IsNormal == true & Ch.IsHoliday == false) NextReplTable10.Rows[WNextRow - 1]["SType"] = "WorkingDay";

            if (Ch.IsWeekend == true & Ch.IsHoliday == false) NextReplTable10.Rows[WNextRow - 1]["SType"] = "Weekend";

            if (Ch.IsHoliday == true) NextReplTable10.Rows[WNextRow - 1]["SType"] = "Holiday";

            dataGridView1.DataSource = NextReplTable10.DefaultView;

        }

        // Upon Button Finish Update Data Bases 
        private void ButtonFinish_Click(object sender, EventArgs e)
        {
           
            for (int rows = 0; rows < (dataGridView1.Rows.Count - 1); rows++)
            {

                NextDate = (DateTime)dataGridView1.Rows[rows].Cells["NextDate"].Value;
                NMonth = (string)dataGridView1.Rows[rows].Cells["NMonth"].Value;
                NDay = (string)dataGridView1.Rows[rows].Cells["NDay"].Value;
                NType = (string)dataGridView1.Rows[rows].Cells["NType"].Value;

                SameAs = (DateTime)dataGridView1.Rows[rows].Cells["SameAs"].Value;
                SMonth = (string)dataGridView1.Rows[rows].Cells["SMonth"].Value;
                SDay = (string)dataGridView1.Rows[rows].Cells["SDay"].Value;
                SType = (string)dataGridView1.Rows[rows].Cells["SType"].Value;

                // ASSIGN VALUES

                Cm.BankId = WOperator;
                Cm.MatchDatesCateg = WMatchDatesCateg; 
                Cm.NextDate = NextDate;
            //    Cm.Prive = WWPrive;

                Cm.ReadNextDate(WOperator, WMatchDatesCateg, NextDate);
                if (Cm.RecordFound == true)
                {
                    Cm.NMonth = NMonth;
                    Cm.NDay = NDay;
                    Cm.NType = NType;

                    Cm.SameAs = SameAs;
                    Cm.SMonth = SMonth;
                    Cm.SDay = SDay;
                    Cm.SType = SType;

                    Cm.DateInsert = DateTime.Today; 

                    Cm.UpdateNextDate(WOperator, WMatchDatesCateg, NextDate);
                }
                else
                {

                Cm.NMonth = NMonth; 
                Cm.NDay = NDay;
                Cm.NType = NType; 

                Cm.SameAs = SameAs; 
                Cm.SMonth = SMonth; 
                Cm.SDay = SDay;
                Cm.SType = SType; 

                Cm.DateInsert = DateTime.Today;

                Cm.Operator = WOperator;

                Cm.InsertNextDate(WOperator, WMatchDatesCateg, NextDate);
                }

            }

            MessageBox.Show("Matched Entries Inserted or Updated");

            textBoxMsgBoard.Text = "All Entries Updated! ";
        }

        // Bank Name 
        
      

        //AUDIT TRAIL : GET IMAGE AND INSERT IT IN AUDIT TRAIL 
        private void GetMainBodyImageAndStoreIt(string InCategory, string InSubCategory, string InTypeOfChange, int InUser)
        {
            Bitmap SCREENa;
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENa = memoryImage;

            AuditTrailClass At = new AuditTrailClass();
            //        At.InsertRecord(InCategory, InSubCategory, InTypeOfChange, InUser, SCREENa);
        }

       

        private void chart1_Click(object sender, EventArgs e)
        {

        }

       
       
    }
}

