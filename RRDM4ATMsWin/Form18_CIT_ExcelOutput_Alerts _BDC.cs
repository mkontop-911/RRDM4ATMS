using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form18_CIT_ExcelOutput_Alerts_BDC : Form
    {
       
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDM_CIT_EXCEL_TO_BANK Ce = new RRDM_CIT_EXCEL_TO_BANK();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        //RRDMReplOrdersClass Ro = new RRDMReplOrdersClass(); 
        //RRDM_Cit_ExcelOutputCycles Coc = new RRDM_Cit_ExcelOutputCycles();

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        //DateTime WDateTime;
      
        string WSelectionCriteria; 
        
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCitId; 
       
        public Form18_CIT_ExcelOutput_Alerts_BDC(string InSignedId, int InSignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;
       
               // 
         
        }
// Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // 
            //WSelectionCriteria = " Where ActiveRecord = 1 AND PassReplCycle = 0 AND NewEstReplDt < @NewEstReplDt "; 
            string WSelectionCriteria = " WHERE [Valid_Entry] = 0  "; // Show the Invalid Ones 

            Ce.ReadRecordsFrom_CIT_Excel_Records_The_Invalid(WSelectionCriteria);

            if (Ce.DataTableAllFields.Rows.Count > 0)
            {

                ShowGrid1();

                //textBoxTotalBlue.Text = Total_DarkBlue.ToString();
                //textBoxTotalGreen.Text = Total_Greens.ToString();
                //textBoxTotalRed.Text = Total_Red.ToString();
                //textBoxWaitingAuth.Text = Total_Yellow.ToString();

            }

            //ShowGrid1();

        }
     

        // Row Enter 
       int WSeqNo ;
       string WAtmNo; 

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Ce.Read_CIT_Excel_TableBySeqNo(WSeqNo);

            WAtmNo = Ce.AtmNo;

            //ItHasForex = Ce.Check_IF_FOREX_IN_BULK(WBulkRecordId);

            // Check if this entry is valid for the basics 
            // ATMs Group
            // Owner 
            // ATM replenished by whom? 
            //  RRDMUsersRecords Ur = new RRDMUsersRecords(); 
            Ac.ReadAtm(WAtmNo);
            if (Ac.RecordFound == true)
            {
                //if (Ac.CitId == "1000" || Ac.AtmsReplGroup == 0 || Ac.AtmReplUserId == null || Ac.AtmReplUserId =="")
                //{
                //    MessageBox.Show("Please update the details of this ATM"+ Environment.NewLine
                //                    + "Current details are: " + Environment.NewLine
                //                    + "CitId:.."+ Ac.CitId + Environment.NewLine
                //                    + "Ac.AtmReplUserId:.." + Ac.AtmReplUserId 
                //        );
                //    return; 
                //}

                buttonUpdateATM.Show();
                
            }
            else
            {
                MessageBox.Show("ATM.." + WAtmNo + "..Not Found in Database" + Environment.NewLine
                                  + "No Journal was loaded yet  " + Environment.NewLine
                                  + "You can wait for the Journal" + Environment.NewLine
                                  + "Or Open the ATM through the RRDM process " + Environment.NewLine
                                  + "if you choose to go with the second option you must open the ATM accounts too. " + Environment.NewLine
                                  );
                
                buttonUpdateATM.Hide();

                return;

            }
            textBoxOrdersCycleNo.Text = WSeqNo.ToString(); 

        }

        // UPDATE ATM
        private void buttonUpdateATM_Click(object sender, EventArgs e)
        {
            // Go directly to Update
            Form65 NForm65; 
            NForm65 = new Form65(WSignedId, WSignRecordNo, WOperator, WAtmNo, 2);
            NForm65.FormClosed += NForm65_FormClosed;
            NForm65.ShowDialog(); 
        }
        int WRow;
        int scrollPosition;
        void NForm65_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (dataGridView1.Rows.Count != 0)
            {

                Ce.UpdateInfoForInvalidATM(WAtmNo);

                WRow = dataGridView1.SelectedRows[0].Index;
                scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;
                // Load Grid 
                Form502_Load(this, new EventArgs());

                dataGridView1.Rows[WRow].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));

                dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
            }
            else
            {
                // Load Grid 
                Form502_Load(this, new EventArgs());
            }
        }
        // CREATE ATM
        private void buttonCreateATM_Click(object sender, EventArgs e)
        {
            // Go Directly to Create 
            //
            
            //WAtmNo = "9999";

            //Form65 NForm65;

            //NForm65 = new Form65(WSignedId, WSignRecordNo, WOperator, WAtmNo, 4);
            //NForm65.FormClosed += NForm65_FormClosed;
            //NForm65.ShowDialog(); ;
        }

        private void ShowGrid1()
        {

            //Total_DarkBlue = 0;
            //Total_Greens = 0;
            //Total_Red = 0;
            //Total_Yellow = 0;

            dataGridView1.DataSource = null;
            dataGridView1.Refresh();

            dataGridView1.DataSource = Ce.DataTableAllFields.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No ATMs Available!");
                return;
            }
           

            //DataGridViewCellStyle style = new DataGridViewCellStyle();
            //style.Format = "N2";

            //dataGridView1.Columns[0].Width = 50; // SeqNo
            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[0].Visible = true;

            //dataGridView1.Columns[1].Width = 55; // STATUS
            ////dataGridView1.Columns[1].DefaultCellStyle = style;
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[2].Width = 60; // AtmNo
            //dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[2].Visible = true;

            //dataGridView1.Columns[3].Width = 100; // ReplCycleStartDate As CIT_Start
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[3].Visible = true;

            //dataGridView1.Columns[4].Width = 100; // ReplCycleEndDate As CIT_End
            //dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[4].Visible = true;

            //dataGridView1.Columns[5].Width = 70; // CIT_Total_Returned
            //dataGridView1.Columns[5].DefaultCellStyle = style;
            //dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[5].HeaderText = "CIT_Returned";

            //dataGridView1.Columns[6].Width = 70; // CIT_Total_Deposit_Local_Ccy
            //dataGridView1.Columns[6].DefaultCellStyle = style;
            //dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[6].HeaderText = "CIT_Deposits";

            //dataGridView1.Columns[7].Width = 70; // SWITCH_Returns
            //dataGridView1.Columns[7].DefaultCellStyle = style;
            //dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[7].HeaderText = "Switch_Returned";

            //dataGridView1.Columns[8].Width = 70; // SWITCH_Deposits
            //dataGridView1.Columns[8].DefaultCellStyle = style;
            //dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Columns[8].HeaderText = "Switch_Deposits";

            //dataGridView1.Columns[9].Width = 40; // Journal
            ////dataGridView1.Columns[6].DefaultCellStyle = style;
            //dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[10].Width = 40; // CIT_ID 
            ////dataGridView1.Columns[6].DefaultCellStyle = style;
            //dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            //    //WSeqNo = (int)rowSelected.Cells[0].Value;
            //    //string STATUS = (string)row.Cells[1].Value;

            //    //if (STATUS == "01" || STATUS == "00")
            //    //{
            //    //    row.DefaultCellStyle.BackColor = Color.Gainsboro;
            //    //    row.DefaultCellStyle.ForeColor = Color.Red;
            //    //    Total_Red = Total_Red + 1;
            //    //}
            //    //if (STATUS == "02")
            //    //{
            //    //    row.DefaultCellStyle.BackColor = Color.Gainsboro;
            //    //    row.DefaultCellStyle.ForeColor = Color.Yellow;
            //    //    Total_Yellow = Total_Yellow + 1;
            //    //}
            //    //if (STATUS == "03")
            //    //{
            //    //    row.DefaultCellStyle.BackColor = Color.Gainsboro;
            //    //    row.DefaultCellStyle.ForeColor = Color.Green;
            //    //    Total_Greens = Total_Greens + 1;
            //    //}
            //    //if (STATUS == "04")
            //    //{
            //    //    row.DefaultCellStyle.BackColor = Color.Gainsboro;
            //    //    row.DefaultCellStyle.ForeColor = Color.DarkBlue;
            //    //    Total_DarkBlue = Total_DarkBlue + 1;
            //    //}

            //}
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
//
// print Loading cycles
//
        


// Export to excel 
        private void buttonPrintCycle_Click(object sender, EventArgs e)
        {
            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelPath = "C:\\RRDM\\Working\\Controller_Alerts" + ".xls";

            string WorkingDir = "C:\\RRDM\\Working\\";
            XL.ExportToExcel(Ce.DataTableAllFields, WorkingDir, ExcelPath);
        }

    }
}
