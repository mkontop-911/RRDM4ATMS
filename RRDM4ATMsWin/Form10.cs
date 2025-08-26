using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form10 : Form
    {
      
        //Form35 NForm35; 
        Bitmap SCREENinitial;
   
        DataTable NextReplTable10 = new DataTable();

        DataTable TrendTable = new DataTable();

        DateTime WDtTm;

        DateTime NullFutureDate = new DateTime(2050, 11, 21);

        RRDMReplDatesCalc Rc = new RRDMReplDatesCalc();

        RRDMFixedDaysReplAtmClass Fr = new RRDMFixedDaysReplAtmClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMAtmsClass Ac = new RRDMAtmsClass(); 

        //multilingual
        CultureInfo culture;

        RRDMUsersRecords Xa = new RRDMUsersRecords(); // Make class availble 

        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        decimal OldBal;
        decimal NewBal;

        string WMatchDatesCateg;

        int Index;

        DateTime NextDate;
        string Day;
        string Type;

        DateTime SameAs;
        string SDay;
        string SType;

        decimal Suggested;
        string Correction;
        decimal Final;

        string WBankId;

        string WAtmNo; 

        string WCurrNm;
        int WNextRow;
 
        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
   
        int WAction;

        public Form10(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
    
            WAction = InAction;  // 1 = Show 

            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            
            //TEST
            dateTimePicker1.Value = new DateTime(2014, 02, 28);
            dateTimePicker3.Value = new DateTime(2014, 03, 20);
            //TEST
            if (WSignedId == "1005")
            {
               
                textBox1.Text = "AB102";
             //   textBox8.Text = "CRBAGRAA";
              //  textBox6.Text = "Alpha BANK"; 
            }
            if (WSignedId == "03ServeUk")
            {
                textBox1.Text = "ServeUk102";
              //  textBox8.Text = "ServeUk";
              //  textBox6.Text = "Serve UK for ATMs"; 
            }
            

            comboBox2.Items.Add("-25 %");
            comboBox2.Items.Add("-20 %");
            comboBox2.Items.Add("-15 %");
            comboBox2.Items.Add("-10 %");
            comboBox2.Items.Add("0 %");
            comboBox2.Items.Add("+10 %");
            comboBox2.Items.Add("+15 %");
            comboBox2.Items.Add("+20 %");
            comboBox2.Items.Add("+25 %");

            label11.Hide();
            label18.Hide();
            label19.Hide();
            label12.Hide();
            label13.Hide();
            panel3.Hide();
            panel4.Hide();
            chart1.Hide(); 

        }
        // FORM LOAD 
        private void Form10_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet23.FixedDaysReplAtm' table. You can move, or remove it, as needed.
         //   this.FixedDaysReplAtmTableAdapter.Fill(this.aTMSDataSet23.FixedDaysReplAtm);
            textBoxMsgBoard.Text = " Make your choice and create the Repl Entries for the next period";

            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENinitial = memoryImage;

        }

        // Show Analysis
        private void button3_Click(object sender, EventArgs e)
        {

          
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please enter a valid ATM No");
                return; 
            }
            if (textBox1.Text != "AB102" & textBox1.Text != "AB104")
            {
                MessageBox.Show("Please enter ATM No = AB102 or AB104 that have testing data");
                return;
            }

            Ac.ReadAtm(textBox1.Text);
            if (Ac.RecordFound == false)
            {
                MessageBox.Show("Not Found ATM");
                return; 
            }

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

            label11.Show();
            label18.Show();
            label19.Show();
            label12.Show();
            label13.Show();
            panel3.Show();
            panel4.Show();
            chart1.Show();

            textBoxMsgBoard.Text = "Select a row and input needed information";

          //  DateTime y;
            DateTime StartDt;
            DateTime FinishDt;
            DateTime StartMinus30;

            StartDt = dateTimePicker1.Value.Date;
            FinishDt = dateTimePicker3.Value.Date;
            StartMinus30 = StartDt.AddDays(-30);

            TimeSpan Remain = FinishDt - StartDt;
            label18.Text = (Remain.Days+1).ToString();

            WAtmNo = textBox1.Text;

            Ac.ReadAtm(WAtmNo); 

            WBankId = Ac.BankId;

            WMatchDatesCateg = Ac.MatchDatesCateg;

            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator);
            WCurrNm = Ba.BasicCurName;

            NextReplTable10 = new DataTable();
            NextReplTable10.Clear();

            if (WAction == 1) 
            {        

                int Request = 2; // Give Matching dates for period 
                Rc.GiveMeDataTableReplInfo(WOperator, StartDt,
                                       FinishDt, WAtmNo, 0, WMatchDatesCateg, Request);
                
                labelStep1.Text = "Create Next days Repl Amounts for ATM : " + WAtmNo ;

                NextReplTable10.Columns.Add("Index", typeof(int));
                NextReplTable10.Columns.Add("NextDate", typeof(DateTime));
                NextReplTable10.Columns.Add("Day", typeof(string));
                NextReplTable10.Columns.Add("Type", typeof(string));
                NextReplTable10.Columns.Add("SameAs", typeof(DateTime));
                NextReplTable10.Columns.Add("SDay", typeof(string));
                NextReplTable10.Columns.Add("SType", typeof(string));
                NextReplTable10.Columns.Add("Suggested", typeof(decimal));
                NextReplTable10.Columns.Add("Correction%", typeof(string));
                NextReplTable10.Columns.Add("Final", typeof(decimal));

                DateTime TempDtTm = StartDt;
                // Fill IN GRIDDAYS BASED ON PUBLIC TABLE dtRDays of RC Class 
                //   DataRow RowGrid = GridDays.NewRow();
                bool WorkingDay ;
                bool Weekend ;
                bool Holiday ;

                int I = 0;

                I = 0;

                while (I < (Rc.dtRDays.Rows.Count))
                {
                    // IN WHILE LOOP WE LEAVE OUT THE LAST DAY. THIS IS THE REPLENISHEMENT DAY 
                    DataRow RowTable = NextReplTable10.NewRow();

                    RowTable["Index"] = I + 1;

                    NextDate = (DateTime)Rc.dtRDays.Rows[I]["Date"];

                    RowTable["NextDate"] = NextDate.Date;
                    RowTable["Day"] = NextDate.Date.DayOfWeek;

                    WorkingDay = (bool)Rc.dtRDays.Rows[I]["Normal"];
                    Weekend = (bool)Rc.dtRDays.Rows[I]["Weekend"];
                    Holiday = (bool)Rc.dtRDays.Rows[I]["Special"];

                    if (WorkingDay == true) RowTable["Type"] = "WorkingDay";
                    if (Weekend == true) RowTable["Type"] = "Weekend";
                    if (Holiday == true) RowTable["Type"] = "Holiday";

                    TempDtTm = (DateTime)Rc.dtRDays.Rows[I]["SameAsDate"];

                    RowTable["SameAs"] = TempDtTm.Date;
                    RowTable["SDay"] = TempDtTm.Date.DayOfWeek;

                    WorkingDay = (bool)Rc.dtRDays.Rows[I]["SameNormal"];
                    Weekend = (bool)Rc.dtRDays.Rows[I]["SameWeekend"];
                    Holiday = (bool)Rc.dtRDays.Rows[I]["SameSpecial"];

                    if (WorkingDay == true) RowTable["SType"] = "WorkingDay";
                    if (Weekend == true) RowTable["SType"] = "Weekend";
                    if (Holiday == true) RowTable["SType"] = "Holiday";

                    RowTable["Suggested"] = Rc.dtRDays.Rows[I]["RecDispensed"];

                    Fr.ReadFixedDaysReplAtm(WOperator, WAtmNo, NextDate);
                    if (Fr.RecordFound == true)
                    {
                        RowTable["Correction%"] = Fr.Correction;
                        RowTable["Final"] = Fr.Final; 
                    }
                    else
                    {
                        RowTable["Correction%"] = "0 %";
                        RowTable["Final"] = Rc.dtRDays.Rows[I]["RecDispensed"];
                    }


              //      TotalRepl = TotalRepl + (decimal)Rc.dtRDays.Rows[I]["RecDispensed"];

                    NextReplTable10.Rows.Add(RowTable);

                    I++;
                }

          
                // ASSIGN AND SHOW GRID

                dataGridView1.DataSource = NextReplTable10.DefaultView;
               
                dataGridView1.Columns[0].Name = "Index";
                dataGridView1.Columns[1].Name = "NextDate";
                dataGridView1.Columns[2].Name = "Day";
                dataGridView1.Columns[3].Name = "Type";
                dataGridView1.Columns[4].Name = "SameAs";
                dataGridView1.Columns[5].Name = "SDay";
                dataGridView1.Columns[6].Name = "SType";
                dataGridView1.Columns[7].Name = "Suggested";
                dataGridView1.Columns[8].Name = "Correction%";
                dataGridView1.Columns[9].Name = "Final";

                // SIZE
                dataGridView1.Columns["Index"].Width = 40; //
                dataGridView1.Columns["NextDate"].Width = 70;
                dataGridView1.Columns["Day"].Width = 60;
                dataGridView1.Columns["Type"].Width = 60;
                dataGridView1.Columns["SameAs"].Width = 70;
                dataGridView1.Columns["SDay"].Width = 60;
                dataGridView1.Columns["SType"].Width = 60;
                dataGridView1.Columns["Suggested"].Width = 75;
                dataGridView1.Columns["Correction%"].Width = 60;
                dataGridView1.Columns["Final"].Width = 75;


                string SqlString2 =
                     "SELECT DtTm AS Date,"
                     + " SUM(DispensedAmt) AS Disp_Amt"
             + " FROM [ATMS].[dbo].[AtmDispAmtsByDay] "
             + " WHERE BankId = @BankId AND DtTm BETWEEN @DtTm1 AND @DtTm2"
             + " GROUP BY DtTm "
             + " ORDER BY DtTm ASC ";

                using (SqlConnection conn =
                            new SqlConnection(connectionString))
                    try
                    {
                        conn.Open();

                        //Create an Sql Adapter that holds the connection and the command
                        SqlDataAdapter sqlAdapt = new SqlDataAdapter(SqlString2, conn);

                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@BankId", WBankId);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DtTm1", StartMinus30);
                        sqlAdapt.SelectCommand.Parameters.AddWithValue("@DtTm2", StartDt);

                        //Create a datatable that will be filled with the data retrieved from the command
                        //    DataSet MISds = new DataSet();
                        sqlAdapt.Fill(TrendTable);

                        // Close conn
                        conn.Close();

                    }

                    catch (Exception ex)
                    {
                        string exception = ex.ToString();
                        MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                    }
           
                // Set chart data source  
                chart1.DataSource = TrendTable.DefaultView;
                chart1.Series[0].Name = "Disp_Amt";
                // Set series members names for the X and Y values  
                chart1.Series[0].XValueMember = "Date";
                chart1.Series[0].YValueMembers = "Disp_Amt";

                // Data bind to the selected data source  
                chart1.DataBind();
 
            }

        }

        // A ROW IS SELECTED ASSIGN VALUES 
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            if (WAction == 1)
            {
                WNextRow = (int)rowSelected.Cells[0].Value;
                WDtTm = (DateTime)rowSelected.Cells[1].Value;
                textBox4.Text = WDtTm.ToShortDateString();
                WDtTm = (DateTime)rowSelected.Cells[4].Value;
                textBox3.Text = WDtTm.ToShortDateString();
                // old Balance 
                OldBal  = (decimal)rowSelected.Cells[7].Value;
                textBox2.Text = OldBal.ToString("#,##0.00");
                // Increase
                string temp2 = (string)rowSelected.Cells[8].Value;
                comboBox2.Text = temp2.ToString();
                // New balance
                NewBal = (decimal)rowSelected.Cells[9].Value;
                textBox5.Text = NewBal.ToString("#,##0.00"); 
            }

        }

   
        private void tableLayoutPanelMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanelHeader_Paint(object sender, PaintEventArgs e)
        {

        }
      //  // UPDATE INPUT 
      //  private void button4_Click(object sender, EventArgs e)
      //  {
      //      decimal NewRecommend;

      //      if (decimal.TryParse(textBox5.Text, out NewRecommend))
      //      {
      //          // Take the correct action 
      //      }
      //      else
      //      {
      //          MessageBox.Show(textBox5.Text, "Please enter a valid number!");
      //          return;
      //      }



      //     NextReplTable10.Rows[WNextRow - 1]["Final"] = NewRecommend; 

      //     dataGridView1.DataSource = NextReplTable10.DefaultView;
             


      //      //AUDIT TRAIL 
      ///*      string AuditCategory = "Maintenance";
      //      string AuditSubCategory = "Cash-In";
      //      string AuditAction = "Update";
      //      int User = 123;
      //      GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, User); */

      //  }

    //    //AUDIT TRAIL : GET IMAGE AND INSERT IT IN AUDIT TRAIL 
    //    private void GetMainBodyImageAndStoreIt(string InCategory, string InSubCategory, string InTypeOfChange, int InUser)
    //    {
    //        Bitmap SCREENa;
    //        System.Drawing.Bitmap memoryImage;
    //        memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
    //        tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
    //        SCREENa = memoryImage;

    //        AuditTrailClass At = new AuditTrailClass();
    ////        At.InsertRecord(InCategory, InSubCategory, InTypeOfChange, InUser, SCREENa);
    //    }
       
        // ON VALUE CHANGED 
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
         
            if (comboBox2.Text == "-25 %") NewBal = OldBal * 75 / 100;
            if (comboBox2.Text == "-20 %") NewBal = OldBal * 80 / 100;
            if (comboBox2.Text == "-15 %") NewBal = OldBal * 85 / 100;
            if (comboBox2.Text == "-10 %") NewBal = OldBal * 90 / 100;
            if (comboBox2.Text == "0 %") NewBal = OldBal * 100 / 100;
            if (comboBox2.Text == "+10 %") NewBal = OldBal * 110 / 100;
            if (comboBox2.Text == "+15 %") NewBal = OldBal * 115 / 100;
            if (comboBox2.Text == "+20 %") NewBal = OldBal * 120 / 100;
            if (comboBox2.Text == "+25 %") NewBal = OldBal * 125 / 100;

            textBox5.Text = NewBal.ToString("#,##0.00"); 

            NextReplTable10.Rows[WNextRow - 1]["Correction%"] = comboBox2.Text;

            NextReplTable10.Rows[WNextRow - 1]["Final"] = NewBal;

            dataGridView1.DataSource = NextReplTable10.DefaultView;

        }
        //// Chart 1 
        //private void chart1_Click(object sender, EventArgs e)
        //{
        //    int WF = 0;
        //    string Heading;
       
        //        label2.Text = "LAST 30 DAYS TREND";
        //        Heading = label2.Text;
        //        WF = 31; // Means show large for Form10 - Special matched dates  
        //        NForm35 = new Form35(WSignedId, WSignRecordNo, WOperator, TrendTable, Heading, WF);
        //        NForm35.Show();
           
        //}

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            int K = 0;

            while (K <= (dataGridView1.Rows.Count - 1))
            {
                Index = (int)dataGridView1.Rows[K].Cells["Index"].Value;

                NextDate = (DateTime)dataGridView1.Rows[K].Cells["NextDate"].Value;
                Day = (string)dataGridView1.Rows[K].Cells["Day"].Value;
                Type = (string)dataGridView1.Rows[K].Cells["Type"].Value;

                SameAs = (DateTime)dataGridView1.Rows[K].Cells["SameAs"].Value;
                SDay = (string)dataGridView1.Rows[K].Cells["SDay"].Value;
                SType = (string)dataGridView1.Rows[K].Cells["SType"].Value;

                Suggested = (decimal)dataGridView1.Rows[K].Cells["Suggested"].Value;
                Correction = (string)dataGridView1.Rows[K].Cells["Correction%"].Value;
                Final = (decimal)dataGridView1.Rows[K].Cells["Final"].Value;

                // Create Record
                // Fr.BankId = WBankId; 
                // ASSIGN VALUES

                Fr.BankId = WOperator;
                Fr.AtmNo = WAtmNo;
                Fr.NextDate = NextDate;
                //      Fr.Prive = WWPrive;

                Fr.ReadFixedDaysReplAtm(WOperator, WAtmNo, NextDate);
                if (Fr.RecordFound == true)
                {
                    if (Correction == "0 %")
                    {
                        Fr.DeleteFixedDaysReplAtm(WOperator, WAtmNo, NextDate);
                    }

                    Fr.Day = Day;
                    Fr.Type = Type;

                    Fr.SameAs = SameAs;

                    Fr.SDay = SDay;
                    Fr.SType = SType;

                    Fr.Suggested = Suggested;
                    Fr.Correction = Correction;
                    Fr.Final = Final;

                    Fr.DateInsert = DateTime.Today;

                    Fr.UpdateFixedDaysReplAtm(WOperator, WAtmNo, NextDate); //Update
                }
                else
                {
                    if (Correction != "0 %")
                    {
                        Fr.Day = Day;
                        Fr.Type = Type;
                        Fr.BankId = WOperator;
                        Fr.SameAs = SameAs;

                        Fr.SDay = SDay;
                        Fr.SType = SType;

                        Fr.Suggested = Suggested;
                        Fr.Correction = Correction;
                        Fr.Final = Final;

                        Fr.DateInsert = DateTime.Today;

                        Fr.Operator = WOperator;

                        Fr.InsertFixedDaysReplAtm(WOperator, WAtmNo, NextDate); // Insert
                    }
                }


                K++; // Read Next entry of the table 
            }

            MessageBox.Show("New Final Amounts Entries Inserted or Updated");

            textBoxMsgBoard.Text = "All Entries Updated! ";
        }
// Finish 
        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
}
