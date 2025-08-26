using System;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data.SqlClient;
using System.Configuration;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form8_Traces_Oper : Form
    {
        //Form16 NForm16;

        string connectionString = ConfigurationManager.ConnectionStrings
          ["ATMSConnectionString"].ConnectionString;

        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        bool User_U;
        bool AutoOperation;
        bool CutOff;
        bool All;

        string WUserId;
        int WRMCycle;

        string WSelectionCriteria; 

        //    string WOperator;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
        int WSelection; 

        public Form8_Traces_Oper(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator, int InSelection)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
            WSelection = InSelection; // 1 For Performance 
                            // 4 For Deletion Records 
            InitializeComponent();

            // Set Working Date 
            //RRDMGasParameters Gp = new RRDMGasParameters();
            //string ParId = "267";
            //string OccurId = "1";
            //Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            //string TestingDate = Gp.OccuranceNm;
            //if (TestingDate == "YES")
            //    labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            panelUsers.Hide();

            if (WSelection == 1)
            {
                labelStep1.Text = "Operational Actions are shown";

                // LOAD CUT OFF DATES
                comboBox1.DataSource = Rjc.GetCut_Off_Dates_List(WOperator);
                comboBox1.DisplayMember = "DisplayValue";

                // Traces for Critical Process 
                RRDMUsersRecords Us = new RRDMUsersRecords();

                comboBoxUsers.DataSource = Us.Get_Users_List(WOperator);
                comboBoxUsers.DisplayMember = "DisplayValue";

                //comboBox2.Text = "KontoLoadJournal"; 

                textBoxMsgBoard.Text = " Make your selection and press button Show";

                checkBoxCutOff.Checked = false;
                checkBoxUserUpdate.Checked = false;
                checkBoxAutoOperation.Checked = false;
                checkBoxAll.Checked = false;

                panelCutOff.Hide();
                panelUsers.Hide();

                labelHeader.Hide();
                panel2.Hide();

                dateTimePicker1.Value = DateTime.Now.Date;
                dateTimePicker2.Value = DateTime.Now.Date;
            }
// Show the Deletion ones 
            if (WSelection == 4)
            {
                panel3.Hide();
                labelFrom.Hide();
                labelTo.Hide();
                buttonShowRange.Hide();
                dateTimePicker1.Hide();
                dateTimePicker2.Hide();
                buttonPrint.Hide(); 

                labelStep1.Text = "Deletion Records";

                WSelectionCriteria = " WHERE Mode in (17) ";
                                             
                Pt.ReadPerformanceTraceAndFillTableForOperatingActions(WOperator, WSelectionCriteria);
                
                ShowGrid();
            }
            
        }
        // On Load form 
        private void Form8_Load(object sender, EventArgs e)
        {

        }
        // SHOW 
        private void button2_Click(object sender, EventArgs e)
        {
            if (WSelection == 1)
            {
                if (CutOff == false
               & User_U == false
               & AutoOperation == false
               & All == false
               )
                {
                    MessageBox.Show("Please make as selection");
                    return;
                }
                // ALL For USER AND CYCLE
                if (
                    CutOff == true
                    & User_U == true
                    & AutoOperation == true
                    )
                {
                    WSelectionCriteria = " WHERE UserId ='" + WUserId + "'"
                                          + " AND RMCycleNo=" + WRMCycle
                                          + " AND Mode in (5,6)"
                                          ;
                    Pt.ReadPerformanceTraceAndFillTableForOperatingActions(WOperator, WSelectionCriteria);
                }
                // Auto Operation
                if (CutOff == false
                      & User_U == false
                      & AutoOperation == true
                      & All == false
                      )
                {
                    WSelectionCriteria = " WHERE "
                                          + "  Mode in (5,6,7)"
                                          ; ;
                    Pt.ReadPerformanceTraceAndFillTableForOperatingActions(WOperator, WSelectionCriteria);
                }
                // ALL is True alone
                if (CutOff == false
                      & User_U == false
                      & AutoOperation == false
                      & All == true
                      )
                {
                    WSelectionCriteria = " WHERE "
                                          + "  Mode in (5,6)"
                                          ; ;
                    Pt.ReadPerformanceTraceAndFillTableForOperatingActions(WOperator, WSelectionCriteria);
                }

                // ALL For USER 
                if (
                    CutOff == false
                    & User_U == true
                    //  & User_NU == true
                    )
                {
                    WSelectionCriteria = " WHERE UserId ='" + WUserId + "'"
                                          + " AND Mode in (5,6)"
                                          ;
                    Pt.ReadPerformanceTraceAndFillTableForOperatingActions(WOperator, WSelectionCriteria);
                }

                ShowGrid();
            }
            // 
            // 
           

        }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            int WRecordNo = (int)rowSelected.Cells[0].Value;
            //Pt.ReadPerformanceTraceRecNo(WRecordNo);
           

            // textBoxMessageDetails.Text = Pt.Details;
        }

        private void ShowGrid()
        {
            dataGridView1.DataSource = Pt.TableTraces.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No Records to Show");
                return;
            }
            else
            {
                //dataGridView1.Show(); 
            }
            labelHeader.Show();
            panel2.Show();

            //ModelAtm = comboBox1.Text;

            labelHeader.Text = "SELECTED RESULTS";

            dataGridView1.Columns[0].Width = 80; // RecordNo;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 140; // DateTime;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 80; // Cut_Off_Date
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 200; // ProcessNm
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[2].Visible = false;

            dataGridView1.Columns[4].Width = 400; // Details
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[3].Visible = false;

            dataGridView1.Columns[5].Width = 90; //  Duration_Min
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[4].Visible = false;

            dataGridView1.Columns[6].Width = 60; // Mode;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 120; // UserId ;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[7].HeaderText = "User Id";

            dataGridView1.Columns[7].Width = 80; // RMCycleNo
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        // Print 
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ReportATMS73_2 will be build");
            return; 
            string P1 = "OPERATIONAL TRACES " ;

            string P2 = "";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;
            Form56R73 ReportATMS73 = new Form56R73(P1, P2, P3, P4, P5);
            ReportATMS73.Show();
        }
        // Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelHeader.Hide();
            panel2.Hide();
            string sub = comboBox1.Text.Substring(0, 3);
            string date = comboBox1.Text;
            DateTime Cut_Off_Date = DateTime.Parse(date);
        
            Rjc.ReadReconcJobCyclesByCutOffDate(Cut_Off_Date);

            textBoxRMCycleNumber.Text = Rjc.JobCycle.ToString();

            WRMCycle = Rjc.JobCycle; 

        }
// Get User Name
        private void comboBoxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            WUserId = comboBoxUsers.Text;
            Us.ReadUsersRecord(WUserId);
            textBoxUserName.Text = Us.UserName; 
        }

        //bool User_U;
        //bool User_NU;
        //bool CutOff;
        //bool All;

        // Cut Off
        private void checkBoxCutOff_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCutOff.Checked ==true)
            {
                CutOff = true;
                panelCutOff.Show();
            }
            else
            {
                CutOff = false;
                panelCutOff.Hide();
            }
        }
// User Updated
        private void checkBoxUserUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUserUpdate.Checked == true)
            {
               // panelUsers.Show();
                User_U = true;  
            }
            else
            {
                panelUsers.Hide();
                User_U = false;
            }
        }
// User Not Updated Actions 
        private void checkBoxUserNotUpdated_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUserUpdate.Checked == true)
            {
                //panelUsers.Show();
                AutoOperation = true; 
            }
            else
            {
                panelUsers.Hide();
                AutoOperation = false;
            }
        }
        // ALL 
        private void checkBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAll.Checked == true)
            {  
                All = true;
            }
            else
            { 
                All = false;
            }
        }
// Excel 
        private void buttonExcel_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            string ExcelDATE = DateTime.Now.Year.ToString()
                 + DateTime.Now.Month.ToString()
                 + DateTime.Now.Day.ToString()
                    + "_"
                      + DateTime.Now.Hour.ToString()
                      + "_"
                       + DateTime.Now.Minute.ToString()
                       + "_"
                       + DateTime.Now.Second.ToString()
                       ;

            MessageBox.Show("Excel will be created in Directory RRDM/Working " + Environment.NewLine
                + "Create it if it doesnt exist"
                );
            string Id = "..Operational_Traces..";

            string ExcelPath = "C:\\RRDM\\Working\\" + Id +"_"+ ExcelDATE + ".xlsx";
            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Pt.TableTraces, WorkingDir, ExcelPath);
        }
// The records for Range of Dates 
        private void buttonShowRange_Click(object sender, EventArgs e)
        {
            if (dateTimePicker2.Value>= dateTimePicker1.Value)
            {
                MessageBox.Show("Records will be in Ascending");
                // Show 
     
                Pt.ReadPerformanceTraceAndFillTableForOperatingActions_2(WOperator, dateTimePicker1.Value, dateTimePicker2.Value); 

                ShowGrid();
            }
            else
            {
                MessageBox.Show("Second date must be equal or greater than first");
                return; 
            }
                          
            }

        private void checkBoxAutoOperation_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoOperation.Checked == true)
            {
                //panelUsers.Show();
                AutoOperation = true;
            }
            else
            {
                //panelUsers.Hide();
                AutoOperation = false;
            }
        }
    }
    }

