using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form15 : Form
    {

        Bitmap SCREENinitial;

      //  public DataTable HolidaysTable = new DataTable();

      //  RRDMReplDatesCalc Rc = new RRDMReplDatesCalc();

        RRDMHolidays Ch = new RRDMHolidays();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        //multilingual
        CultureInfo culture;

        RRDMUsersRecords Xa = new RRDMUsersRecords(); // Make class availble 

        int BaseYear;
        int NextYear;

        bool FirstTime; 

        bool ComesFromRowEnter;

        bool ComesFromPreLoad;

        int WRowIndex; 

        int WSeqNo; 

        string WHolidaysVersion; 

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
 
        int WAction;

        public Form15(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
       
            WAction = InAction;  

            InitializeComponent();
         
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            label4.Text = WOperator;

            FirstTime = true;

            ComesFromPreLoad = true; 

            // Holidays Versions 

            Gp.ParamId = "215";
            comboBox4.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox4.DisplayMember = "DisplayValue";

            if (WOperator == "CRBAGRAA")
            {
             
                Gp.ParamId = "216";
                comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
                comboBox1.DisplayMember = "DisplayValue";

                comboBox1.Text = "2016";

            }
            else
            {
                Gp.ParamId = "216";
                comboBox1.DataSource = Gp.GetParamOccurancesNm(WOperator);
                comboBox1.DisplayMember = "DisplayValue";

                comboBox1.Text = DateTime.Now.Year.ToString();
            }

            label11.Text = "HOLIDAYS FOR BASE YEAR " + comboBox1.Text;

            comboBox3.Items.Add("Define");
            comboBox3.Items.Add("Yes");
            comboBox3.Items.Add("No");

        //    ComesFromPreLoad = false;

        }
        // FORM LOAD 

        private void Form15_Load(object sender, EventArgs e)
        {
            ShowHolidays();
            ComesFromPreLoad = false;
            //
        }

        // ON ROW ENTER FILL Fields 
       
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            
            WSeqNo = (int)rowSelected.Cells[0].Value;

            ComesFromRowEnter = true; 

            Ch.ReadHolidayBySeqNo(WOperator, WSeqNo);

            dateTimePicker1.Value = Ch.HoliDay;

            textBox4.Text = Ch.Descr;

            dateTimePicker2.Value = Ch.LastYearHoliday;
      
            if (Ch.HoliDay.DayOfYear == Ch.LastYearHoliday.DayOfYear-1)
            {
                comboBox3.Text = "Yes";
            }
            else
            {
                comboBox3.Text = "No";
            }
        }

        // ADD NEW
        int SeqNoOfInsert; 
        private void button2_Click(object sender, EventArgs e)
        {
            Ch.ReadSpecificDate(WOperator, dateTimePicker1.Value, comboBox4.Text);
            if (Ch.RecordFound == true)
            {
                MessageBox.Show("Holiday Already Exist");
                return;
            }
            if (Ch.RecordFound == true & Ch.IsWeekend == true)
            {
                MessageBox.Show("This is weekend. Cannot be added.");
                return;
            }

            if (comboBox3.Text == "Define")
            {
                MessageBox.Show("Please Change Define to Yes or No");
                return; 
            }

            Ch.BankId = WOperator;
            Ch.Year = BaseYear;
            Ch.HolidaysVersion = comboBox4.Text;
          
            Ch.HoliDay = dateTimePicker1.Value.Date ;

            Ch.IsHoliday = true;
            Ch.Descr = textBox4.Text ;

            Ch.LastYearHoliday = dateTimePicker2.Value.Date ;

            if (Ch.HoliDay == Ch.LastYearHoliday.AddYears(1))
            {
                Ch.DiffDayEveryYear = false;   
            }
            else
            {
                Ch.DiffDayEveryYear = true;
            }

            Ch.Operator = WOperator;

            SeqNoOfInsert = Ch.InsertHoliday(WOperator);

            int WRowIndex2= Ch.ReadHolidaysAndFindInsertLocation(WOperator, BaseYear, NextYear, WHolidaysVersion, SeqNoOfInsert);

            WRowIndex2 = WRowIndex2 - 1; 
          //  WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form15_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex2].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowIndex2));

            textBoxMsgBoard.Text = "New Holiday Inserted";

            //comboBox3.Text = "Define";

        }

        // UPDATE ROW
        private void button5_Click(object sender, EventArgs e)
        {

            //Ch.ReadSpecificDate(WOperator, dateTimePicker1.Value, comboBox4.Text);
            //if (Ch.RecordFound == false)
            //{
            //    MessageBox.Show("Nothing to update. This Holiday doesnt exist.");
            //    return;
            //}

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            Ch.ReadSpecificDate(WOperator, dateTimePicker1.Value, comboBox4.Text);
            if (Ch.RecordFound == true & Ch.IsWeekend == true)
            {
                MessageBox.Show("This is weekend. Cannot be Updated.");
                return;
            }

            Ch.BankId = WOperator;
            Ch.Year = BaseYear;
            Ch.HolidaysVersion = comboBox4.Text;

            Ch.HoliDay = dateTimePicker1.Value.Date;
         
            Ch.IsHoliday = true;
            Ch.Descr = textBox4.Text;

            Ch.LastYearHoliday = dateTimePicker2.Value.Date;

            if (Ch.HoliDay == Ch.LastYearHoliday.AddYears(1))
            {
                Ch.DiffDayEveryYear = false;
            }
            else
            {
                Ch.DiffDayEveryYear = true;
            }

            Ch.Operator = WOperator;

            // Update 
            Ch.UpdateHoliday(WSeqNo);

            //         dataGridView1.Sort(dataGridView1.Columns["Date"], ListSortDirection.Ascending);
            Form15_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, WRowIndex));

            textBoxMsgBoard.Text = "Entry Updated";

        }


        // DELETE ROW
        private void button4_Click(object sender, EventArgs e)
        {
            // Do not delete this method
            int WRowIndexGrid1 = dataGridView1.SelectedRows[0].Index;

            if (MessageBox.Show("Warning: Do you want to delete this entry ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
            {
                Ch.DeleteHolidayEntry(WSeqNo); 
                Form15_Load(this, new EventArgs());
            }
            else
            {
                return;
            }
            int temp2 = WRowIndexGrid1 - 1; 
            if (temp2 > 0)
            {
                dataGridView1.Rows[temp2].Selected = true;
                dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, temp2));
            }
        }

        // On Change Holidays Version 
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {   
            // ShowHolidays();
            if (FirstTime == true || ComesFromPreLoad == true)
            {
                return;
            }
            else
            {
                Form15_Load(this, new EventArgs());
            }
        }
        // on change base year 
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FirstTime == true || ComesFromPreLoad == true)
            {
                FirstTime = false;
                return;
            }
            else
            {
                label11.Text = "HOLIDAYS FOR BASE YEAR " + comboBox1.Text;
                Form15_Load(this, new EventArgs());
               
            }
           
            // ShowHolidays();
        }

        private void ShowHolidays()
        {
            WHolidaysVersion = comboBox4.Text;

            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Xa.Culture == "English")
            {
                culture = CultureInfo.CreateSpecificCulture("el-GR");
            }
            if (Usi.Culture == "Français")
            {
                culture = CultureInfo.CreateSpecificCulture("fr-FR");
            }
       
            // Show Holidays
            // Base Year 
            if (int.TryParse(comboBox1.Text, out BaseYear))
            {
            }
            else
            {
                //BaseYear = 2014 ;

                if (WOperator == "CRBAGRAA")
                {
                    BaseYear = 2014;
                }
                else
                {
                    BaseYear = 2018;
                }
            }
        
            if (BaseYear == 0) BaseYear = DateTime.Now.Year;

            label11.Show();
            label12.Show();
            panel3.Show();
            panel4.Show();
            buttonFinish.Show();

            Ch.ReadHolidaysAndFillTable(WOperator, BaseYear, WHolidaysVersion);

            if (Ch.NeedCorrection > 0)
            {
                //MessageBox.Show("Please make corrections on "+ Ch.NeedCorrection.ToString()+ "Entries"); 
                textBoxMsgBoard.Text = "Please make corrections on.." + Ch.NeedCorrection.ToString() + "..Entries"; 
            }
            else
            {
                textBoxMsgBoard.Text = "Act as needed" ;
            }

            ShowGrid(); 
     
        }

        // FINISH : UPDATE DATA BASES FROM GRID
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

       
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
// Define YES Or No 
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.Text == "Yes" )
            {
                DateTime Temp = dateTimePicker1.Value; 
                dateTimePicker2.Value = Temp.AddYears(-1); 
            }
        }

        private void ShowGrid()
        {

            dataGridView1.DataSource = Ch.HolidaysTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
               if ( ComesFromPreLoad == true)
                {

                }
               else
                {
                    MessageBox.Show("No Holiday Entries for this Year." + BaseYear.ToString());
                }
                
              //  this.Dispose();
                return;
            }
           
            textBoxTotalDays.Text = dataGridView1.Rows.Count.ToString(); 

            dataGridView1.Columns[0].Width = 60; // Seq No
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 50; // "NeedCorr"
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 90; //  "Operator"
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView1.Columns[3].Width = 50; // "Year"
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 100; // "Date"
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 100; // "Day"
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 200; // "Descr"
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[7].Width = 100; // "LastYear"
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Position line 
          
        }
// If change 
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            //if (ComesFromRowEnter == true)
            //{
            //    ComesFromRowEnter = false; 
            //    return; 
            //}
            //else
            //{
            //    comboBox3.Text = "Define";
            //}
            
        }
// Copy from base year to next 
        private void buttonCopy_Click(object sender, EventArgs e)
        {
            // 
            NextYear = 0;
            BaseYear = 0; 
            if (int.TryParse(comboBox1.Text, out BaseYear))
            {
            }       
            if (int.TryParse(textBoxNextYear.Text, out NextYear))
            {
            }
            if ((NextYear-BaseYear) != 1)
            {
                MessageBox.Show("Next Year or Base Year not correct. Difference must be one.");
                return; 
            }
            // 
            // Validation of Base year for presence of entriesand how many to copy
            //
            Ch.ReadHolidaysAndFillTable(WOperator, BaseYear, comboBox4.Text);

            if (Ch.TotalRows == 0)
            {
                MessageBox.Show("There are no entries in Base Year to copy");
                return;
            }
            else
            {
                MessageBox.Show("Number of Entries to be copied : " + Ch.TotalRows.ToString());
            }

            // 
            // Validation of Next year ... Check if entries already exists
            //
            Ch.ReadHolidaysAndFillTable(WOperator, NextYear, comboBox4.Text);

            if (Ch.TotalRows == 0)
            {
               // No entries in destination year
            }
            else
            {
                MessageBox.Show("There are Entries in destination year" + Environment.NewLine
                                + "If you wish to create new enries in destination year" + Environment.NewLine
                                  + "Then delete old entries(one by one) and repeat operation"
                                  );
                return; 

            }


            Ch.CopyHolidaysFromYearToYear(WOperator, BaseYear, NextYear);

            MessageBox.Show("Copy of base year to new year completed. "); 

        }
    }
}

