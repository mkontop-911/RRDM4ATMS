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
    public partial class Form15 : Form
    {

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        Bitmap SCREENinitial;

        public DataTable HolidaysTable = new DataTable();

        RRDMReplDatesCalc Rc = new RRDMReplDatesCalc();

        RRDMHolidays Ch = new RRDMHolidays();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        //multilingual
        CultureInfo culture;

        RRDMUsersAndSignedRecord Xa = new RRDMUsersAndSignedRecord(); // Make class availble 

        ResourceManager LocRM = new ResourceManager("RRDM4ATMsWin.appRes", typeof(Form40).Assembly);

        int BaseYear;
        int NextYear;

        bool SameYears; 

        bool DeleteRow; 

        int WRow;

        int TotalRows;

        string WHolidaysVersion;

        string NeedCorr;
        string BankId;
        int Year;
        DateTime Date;
        string Descr;
        DateTime LastYear;
        bool Holiday;
        bool SpecialDt; 
        string Same;

        string SQLString;

        string WSignedId;
        int WSignRecordNo;
        int WSecLevel;
        string WOperator;
 
        int WAction;

        public Form15(string InSignedId, int SignRecordNo, int InSecLevel, string InOperator, int InAction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
       
            WAction = InAction;  

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            label4.Text = WOperator;

            // Holidays Versions 

            Gp.ParamId = "215";
            comboBox4.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBox4.DisplayMember = "DisplayValue";

            comboBox1.Text = "2014"; 

            comboBox1.Items.Add("2012");
            comboBox1.Items.Add("2013");
            comboBox1.Items.Add("2014");
            comboBox1.Items.Add("2015");
            comboBox1.Items.Add("2016");

            comboBox2.Text = "0"; 

            comboBox2.Items.Add("0");
            comboBox2.Items.Add("2015");
            comboBox2.Items.Add("2016");
            comboBox2.Items.Add("2017");

            comboBox3.Items.Add("Yes");
            comboBox3.Items.Add("No");

       //     textBox2.Text = WOperator;
            
            ShowHolidays(); 
  
        }
        // FORM LOAD 

        private void Form15_Load(object sender, EventArgs e)
        {
            

            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENinitial = memoryImage;
        }

        // On Change Holidays Version 
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowHolidays();
        }
        // on change base year 
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowHolidays();
        }

        // on change target year 
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowHolidays();
        }

        private void ShowHolidays()
        {
            WHolidaysVersion = comboBox4.Text; 

            Xa.ReadSignedActivityByKey(WSignRecordNo);

            if (Xa.Culture == "English")
            {
                culture = CultureInfo.CreateSpecificCulture("el-GR");
            }
            if (Xa.Culture == "Français")
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
                comboBox1.Text = DateTime.Now.Year.ToString();
            }
            // Next Year 
            if (int.TryParse(comboBox2.Text, out NextYear))
            {
            }
            else
            {
                comboBox2.Text = "0";
            }

            if (BaseYear == 0) BaseYear = DateTime.Now.Year;
          //  BaseYear = int.Parse(comboBox1.Text);
         //   NextYear = int.Parse(comboBox2.Text);

            if (NextYear > 0)
            {
                label11.Text = "HOLIDAYS FOR TARGET YEAR - " + NextYear.ToString();
                textBoxMsgBoard.Text = "Review the entries and make corrections one by one. When all changes are made press Finish";
            }
            else // Next Year = 0 
            {
                NextYear = BaseYear;
                label11.Text = "BASE YEAR IS SHOWN AND IT IS: " + comboBox1.Text;
                textBoxMsgBoard.Text = " Make changes on Base Year and press Finish or copy base year to create New Year";
            }


            if (BaseYear == NextYear)
            {
              //  MessageBox.Show("Years are the same! System will show the days for this year");
                SameYears = true;
            }
            else SameYears = false;

            label11.Show();
            label12.Show();
            panel3.Show();
            panel4.Show();
            buttonFinish.Show();  

            HolidaysTable = new DataTable();
            HolidaysTable.Clear();
            bool RecordFound = false;

            // DATA TABLE ROWS DEFINITION 
            HolidaysTable.Columns.Add("SerialNo", typeof(int));
            HolidaysTable.Columns.Add("NeedCorr", typeof(string));
            HolidaysTable.Columns.Add("BankId", typeof(string));
            HolidaysTable.Columns.Add("Year", typeof(int));
            HolidaysTable.Columns.Add("Date", typeof(DateTime));
            HolidaysTable.Columns.Add("Descr", typeof(string));
            HolidaysTable.Columns.Add("LastYear", typeof(DateTime));
            HolidaysTable.Columns.Add("Holiday", typeof(bool));
            HolidaysTable.Columns.Add("SpecialDt", typeof(bool));
            HolidaysTable.Columns.Add("Same", typeof(string));

            DateTime WTempDt;
            DateTime NotCorrTempDate = new DateTime(BaseYear, 12, 31);

            TotalRows = 0;

            SQLString = "Select * FROM [dbo].[HolidaysAndSpecialDays] "
            + " WHERE BankId = @BankId AND Year = @Year AND HolidaysVersion = @HolidaysVersion ";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(SQLString, conn))
                    {
                        //       cmd.Parameters.AddWithValue("@CardNo", WCardNo);
                        cmd.Parameters.AddWithValue("@BankId", WOperator);
                        cmd.Parameters.AddWithValue("@Year", BaseYear);
                        cmd.Parameters.AddWithValue("@HolidaysVersion", WHolidaysVersion);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            RecordFound = true;

                            TotalRows = TotalRows + 1;

                            DataRow RowGrid = HolidaysTable.NewRow();

                            RowGrid["SerialNo"] = TotalRows;

                            RowGrid["BankId"] = (string)rdr["BankId"];
                            RowGrid["Year"] = NextYear;

                            bool TempDiff = (bool)rdr["DiffDayEveryYear"];
                            if (TempDiff == true) RowGrid["Same"] = "No";
                            if (TempDiff == false) RowGrid["Same"] = "Yes";

                            if (SameYears == true)
                            {
                                RowGrid["Date"] = (DateTime)rdr["SpecialDay"];
                                RowGrid["LastYear"] = (DateTime)rdr["LastYearSpecial"];
                                RowGrid["NeedCorr"] = "";
                            }

                            if (TempDiff == false & SameYears == false)
                            {
                                WTempDt = (DateTime)rdr["SpecialDay"];
                                RowGrid["LastYear"] = WTempDt.AddYears(NextYear - BaseYear - 1); // Put the Date of Last Year 
                                RowGrid["Date"] = WTempDt.AddYears(NextYear - BaseYear);
                                RowGrid["NeedCorr"] = "No";
                            }

                            if (TempDiff == true & SameYears == false)
                            {
                                WTempDt = (DateTime)rdr["SpecialDay"];
                                RowGrid["LastYear"] = WTempDt.AddYears(NextYear - BaseYear - 1); // Put the Date of Last Year 

                                RowGrid["Date"] = NotCorrTempDate; // USER MUST CORRECT THIS 
                                RowGrid["NeedCorr"] = "Yes";
                            }

                            Holiday = (bool)rdr["IsHoliday"];
                            RowGrid["Holiday"] = Holiday;

                            if (Holiday == true)
                            {
                                RowGrid["SpecialDt"] = false;
                            }
                            else
                            {
                                RowGrid["SpecialDt"] = true;
                            }


                            RowGrid["Descr"] = (string)rdr["SpecialDescr"];

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

                    string exception = ex.ToString();
                    MessageBox.Show(ex.ToString());

                }

            if (RecordFound == false)
            {
                MessageBox.Show(" No Holidays Found for the specified Year!");
                dataGridView1.DataSource = HolidaysTable.DefaultView;
                return;
            }
            else
            {
            }
            //STAVROS
            DataColumn[] keys = new DataColumn[1];
            keys[0] = HolidaysTable.Columns["SerialNo"];

            HolidaysTable.PrimaryKey = keys;

            dataGridView1.DataSource = HolidaysTable.DefaultView;

            dataGridView1.Rows[0].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, 0));

            //  dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Name = "SerialNo";
            dataGridView1.Columns[1].Name = "NeedCorr";
            dataGridView1.Columns[2].Name = "BankId";
            dataGridView1.Columns[3].Name = "Year";
            dataGridView1.Columns[4].Name = "Date";
            dataGridView1.Columns[5].Name = "Descr";
            dataGridView1.Columns[6].Name = "LastYear";
            dataGridView1.Columns[7].Name = "Holiday";
            dataGridView1.Columns[8].Name = "SpecialDt";
            dataGridView1.Columns[9].Name = "Same";

            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Sort(dataGridView1.Columns["Date"], ListSortDirection.Ascending);

            // SIZE

            dataGridView1.Columns["NeedCorr"].Width = 50; //
            dataGridView1.Columns["BankId"].Width = 75;
            dataGridView1.Columns["Year"].Width = 50;
            dataGridView1.Columns["Date"].Width = 75;
            dataGridView1.Columns["Descr"].Width = 100;
            dataGridView1.Columns["LastYear"].Width = 75;
            dataGridView1.Columns["Holiday"].Width = 55;
            dataGridView1.Columns["SpecialDt"].Width = 55;
            dataGridView1.Columns["Same"].Width = 60;

            textBox1.Text = TotalRows.ToString();

            dataGridView1.Rows[0].Selected = true;
            dataGridView1_RowEnter_1(this, new DataGridViewCellEventArgs(1, 0));

        }
      
        // ON ROW ENTER FILL Fields 
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        { 
            if (DeleteRow == false)
            {
                DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
                WRow = e.RowIndex;
              
                NeedCorr = (string)rowSelected.Cells["NeedCorr"].Value;
                Date = (DateTime)rowSelected.Cells["Date"].Value;
                Descr = (string)rowSelected.Cells["Descr"].Value;
                LastYear = (DateTime)rowSelected.Cells["LastYear"].Value;
                Holiday = (bool)rowSelected.Cells["Holiday"].Value;
                SpecialDt = (bool)rowSelected.Cells["SpecialDt"].Value;
                Same = (string)rowSelected.Cells["Same"].Value;
                dateTimePicker1.Value = Date.Date;
                textBox4.Text = Descr;
                dateTimePicker2.Value = LastYear.Date;
                if (Holiday == true) radioButton1.Checked = true;
                if (SpecialDt == true) radioButton2.Checked = true; 
                comboBox3.Text = Same; 
            }    
                    
        }

        // UPDATE ROW
        private void button5_Click(object sender, EventArgs e)
        {
            DateTime NotCorrTempDate = new DateTime(BaseYear, 12, 31);

            DataGridViewRow rowSelected = dataGridView1.Rows[WRow];

            NeedCorr = (string)rowSelected.Cells["NeedCorr"].Value;
            if (dateTimePicker1.Value == NotCorrTempDate)
            {
                MessageBox.Show("Correct Date Please");
                return;
            }
            else rowSelected.Cells["NeedCorr"].Value = "No";

            rowSelected.Cells["Date"].Value = dateTimePicker1.Value;
            rowSelected.Cells["Descr"].Value = textBox4.Text;
            rowSelected.Cells["LastYear"].Value = dateTimePicker2.Value;
            if (radioButton1.Checked == true)
            {
                rowSelected.Cells["Holiday"].Value = true;
            }
            else rowSelected.Cells["Holiday"].Value = false;
            if (radioButton2.Checked == true)
            {
                rowSelected.Cells["SpecialDt"].Value = true;
            }
            else rowSelected.Cells["SpecialDt"].Value = false;
            
            rowSelected.Cells["Same"].Value = comboBox3.Text;

            dataGridView1.Refresh();

            dataGridView1.Sort(dataGridView1.Columns["Date"], ListSortDirection.Ascending);

            textBoxMsgBoard.Text = "Entry Updated";

        }
       

        // INSER NEW
        private void button2_Click(object sender, EventArgs e)
        {
            // Show New Grid 

            DataRow RowGrid = HolidaysTable.NewRow();
            TotalRows = TotalRows + 1;
            RowGrid["SerialNo"] = TotalRows;
            RowGrid["BankId"] = WOperator;
            RowGrid["Year"] = NextYear;
            RowGrid["Same"] = comboBox3.Text;

            RowGrid["Date"] = dateTimePicker1.Value;
            RowGrid["LastYear"] = dateTimePicker2.Value;
            RowGrid["NeedCorr"] = "No";
            RowGrid["Descr"] = textBox4.Text;

            if (radioButton1.Checked == true)
            {
                RowGrid["Holiday"] = true;
            }
            else RowGrid["Holiday"] = false;
            if (radioButton2.Checked == true)
            {
                RowGrid["SpecialDt"] = true;
            }
            else RowGrid["SpecialDt"] = false;

            HolidaysTable.Rows.Add(RowGrid);

            dataGridView1.Refresh();

            dataGridView1.Sort(dataGridView1.Columns["Date"], ListSortDirection.Ascending);

            textBoxMsgBoard.Text = "New Inserted";
   
        }

        // DELETE ROW
        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this day?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                             == DialogResult.Yes)
            {
                int SaveRow = WRow;
                DeleteRow = true;

                DataRow dr = HolidaysTable.Rows.Find(dataGridView1.Rows[SaveRow].Cells["SerialNo"].Value);

                dr.Delete();
                //     HolidaysTable.Rows[SaveRow].Delete();

             //   MessageBox.Show(" Entry with Date : " + Date.ToString() + " and Description: '" + Descr + "' Is deleted.  ");

                dataGridView1.DataSource = HolidaysTable.DefaultView;

                dataGridView1.Sort(dataGridView1.Columns["Date"], ListSortDirection.Ascending);

                textBoxMsgBoard.Text = " Entry with Date : " + Date.ToString() + " and Description: '" + Descr + "' Is deleted.  ";

                DeleteRow = false;
            }
            else
            {
            }          

        }

        // FINISH : UPDATE DATA BASES FROM GRID
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            // Read ALL Grid entries and check if all corrections are made.
            // If all corrections are made read one by one and update (Insert ) in Table .... If already exist then update 
            int rows;
            for (rows = 0; rows < (dataGridView1.Rows.Count ); rows++)
            {

                NeedCorr = (string)dataGridView1.Rows[rows].Cells["NeedCorr"].Value;

                if (NeedCorr == "Yes")
                {
                    MessageBox.Show("Correct all entries please");
                    return;
                }
                else
                {

                }

            }

            Ch.DeleteYearHoliday(WOperator, NextYear, WHolidaysVersion); // Delete All previous entries

            for (rows = 0; rows < (dataGridView1.Rows.Count ); rows++)
            {

                NeedCorr = (string)dataGridView1.Rows[rows].Cells["NeedCorr"].Value;
                BankId = (string)dataGridView1.Rows[rows].Cells["BankId"].Value;
                Year = (int)dataGridView1.Rows[rows].Cells["Year"].Value;
                Date = (DateTime)dataGridView1.Rows[rows].Cells["Date"].Value;
                Descr = (string)dataGridView1.Rows[rows].Cells["Descr"].Value;
                LastYear = (DateTime)dataGridView1.Rows[rows].Cells["LastYear"].Value;
                Holiday = (bool)dataGridView1.Rows[rows].Cells["Holiday"].Value;
                SpecialDt = (bool)dataGridView1.Rows[rows].Cells["SpecialDt"].Value;
                Same = (string)dataGridView1.Rows[rows].Cells["Same"].Value;

                // ASSIGN VALUES

                Ch.BankId = BankId;
                Ch.Year = NextYear;

                
                    //TEST
                    Ch.HolidaysVersion = WHolidaysVersion;
                    Ch.SpecialDay = Date;
             
                    Ch.IsHoliday = Holiday;
                    Ch.SpecialDescr = Descr;
                    if (Same == "No")
                    {
                        Ch.DiffDayEveryYear = true;
                    }
                    else Ch.DiffDayEveryYear = false;
                    Ch.LastYearSpecial = LastYear;
                    //TEST
                    Ch.SpecialId = 999;

                    Ch.Operator = WOperator; 

                    Ch.InsertHoliday(WOperator, Date, WHolidaysVersion);
            }

            MessageBox.Show("Holiday Entries Inserted or Updated");

            textBoxMsgBoard.Text = "All Entries Updated! ";
        }

        private void toolTipMessages_Popup(object sender, PopupEventArgs e)
        {

        }

        private void toolTipController_Popup(object sender, PopupEventArgs e)
        {

        }

        private void tableLayoutPanelMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanelHeader_Paint(object sender, PaintEventArgs e)
        {

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
        
      
    }
}

