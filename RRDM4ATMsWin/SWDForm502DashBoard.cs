using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class SWDForm502DashBoard : Form
    {
        RRDMSWDCategories Sc = new RRDMSWDCategories();
        RRDMSWDPackagesDistributionSessions Ds = new RRDMSWDPackagesDistributionSessions();
      
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();
        RRDMJTMQueue Jq = new RRDMJTMQueue();
        RRDMUsersRecords Us = new RRDMUsersRecords(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime WDateTime;
        int WSeqNoLeft;

        string WSelectionCriteria; 

        string WSWDCategory; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
       
        public SWDForm502DashBoard(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
          
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

            labelUserId.Text = InSignedId;

        }
// Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // 
            string Filter1 = " WHERE Operator = '" + WOperator + "' ";

            Sc.ReadSWDCategoriesAndFillTable(Filter1);
        
            dataGridView1.DataSource = Sc.TableSWDCateg.DefaultView;

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Width = 60; // Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 80; // Id
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 150; // Name
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            // REPLENISHED - Chart1

            int[] yvalues1 = { 170, 25 };
            //int[] yvalues1 = { Cc.TotRepl, Cc.TotNotRepl };
            string[] xvalues1 = { "Yes", "No" };

            // Set series members names for the X and Y values 
            chart1.Series[0].Points.DataBindXY(xvalues1, yvalues1);

        
        }
        // Ro
        private void dataGrid1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSeqNoLeft = (int)rowSelected.Cells[0].Value;
            WSWDCategory = rowSelected.Cells[1].Value.ToString();

            Sc.ReadSWDCategoriesbySeqNo(WOperator, WSeqNoLeft);

            label17.Text = "SELECTED CATEGORY : " + Sc.SWDCategoryId;

            textBox2.Text = Sc.EffectivePackageId;

            Ds.ReadSWDPackDistrSesbyPackageId(WOperator, Sc.EffectivePackageId);
            textBox3.Text = Ds.StartDateTm.ToString();

            Jd.ReadJTMIdentificationDetailsForSWTotals(Sc.SWDCategoryId, Sc.EffectivePackageId);
            textBoxUpdated.Text = Jd.TotalSameSWVersion.ToString();
            textBoxNotUpdated.Text = Jd.TotalNoSameSWVersion.ToString();
            textBoxTotalATMs.Text = (Jd.TotalSameSWVersion + Jd.TotalNoSameSWVersion).ToString();
            if ((Jd.TotalSameSWVersion + Jd.TotalNoSameSWVersion)>0)
            {
                textBoxPerc.Text = (Jd.TotalSameSWVersion * 100 / (Jd.TotalSameSWVersion + Jd.TotalNoSameSWVersion)).ToString();
            }
            
            // RECONCILED - Chart2
            int[] yvalues2 = { Jd.TotalSameSWVersion, Jd.TotalNoSameSWVersion };
            //int[] yvalues2 = { Cc.TotReconc, Cc.TotNotReconc1, Cc.TotNotReconc2 };
            string[] xvalues2 = { "Yes", "No" };

            // Set series members names for the X and Y values 
            chart2.Series[0].Points.DataBindXY(xvalues2, yvalues2);

            WSelectionCriteria = " WHERE SWDCategory ='" + Sc.SWDCategoryId + "'"; 
            int WMode = 1;
            WDateTime = NullPastDate;
           
            Jd.ReadJTMIdentificationDetailsToFillPartialTable(WSelectionCriteria, WMode, WDateTime);

            ShowGrid2();          
          
        }

        // Row Enter 
        string WAtmNo;
     
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WAtmNo = (string)rowSelected.Cells[0].Value;

           
            Jq.ReadJTMQueueByATMAndFillTable(WAtmNo);

            if (Jq.QueueJournalTable.Rows.Count > 0)
            {

                dataGridView3.DataSource = Jq.QueueJournalTable.DefaultView;

                dataGridView3.Columns[0].Width = 60; // 
                dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView3.Columns[1].Width = 60; // 
                dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView3.Columns[2].Width = 120; // 
                dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[3].Width = 80; // RequestorID
                dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[4].Width = 80; // 
                dataGridView3.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[5].Width = 70; // Type Of Journal 
                dataGridView3.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            
                dataGridView3.Columns[6].Width = 60; // Stage
                dataGridView3.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView3.Columns[7].Width = 60; // 
                dataGridView3.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView3.Columns[8].Width = 120; // Result Message
                dataGridView3.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView3.Columns[8].Visible = false;

                dataGridView3.Columns[9].Width = 120; // date 1
                dataGridView3.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[10].Width = 120; // date 2
                dataGridView3.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[11].Width = 120; // date 3
                dataGridView3.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[12].Width = 120; // date 4
                dataGridView3.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[13].Width = 120; // date 5
                dataGridView3.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView3.Columns[14].Width = 120; // date 6
                dataGridView3.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            }
            else
            {
                dataGridView3.DataSource = null;
                dataGridView3.Refresh();
            }
        }

        // Queue
        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            int MsgId = (int)rowSelected.Cells[0].Value;
            Jq.ReadJTMQueueByMsgID(MsgId); 
            if (Jq.RecordFound == true)
            {
                textBoxResultMsg.Text = Jq.ResultMessage; 
            }
            else
            {
                textBoxResultMsg.Text = "";
            }
        }

        private void ShowGrid2()
        {
            dataGridView2.DataSource = Jd.ATMsJournalDetailsTable.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                //MessageBox.Show("No ATMs Available!");
                return;
            }

            dataGridView2.Columns[0].Width = 80; // ATMNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[1].Width = 250; // LoadingScheduleID
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[1].Visible = false; 

            dataGridView2.Columns[2].Width = 80; // QueueRecId
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[3].Width = 120; // FileUploadRequestDt
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[3].Visible = false;

            dataGridView2.Columns[4].Width = 120; // FileParseEnd
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[4].Visible = false;

            dataGridView2.Columns[5].Width = 120; // LoadingCompleted
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[5].Visible = false;

            dataGridView2.Columns[6].Width = 70; // Result Code 
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView2.Columns[6].Visible = false;

            dataGridView2.Columns[7].Width = 120; // ResultMessage
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[8].Width = 120; // NextLoadingDtTm
            dataGridView2.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[8].Visible = false;

            dataGridView2.Columns[9].Width = 90; // SWDCategory
            dataGridView2.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[9].Visible = false;

            dataGridView2.Columns[10].Width = 90; // SWVersion
            dataGridView2.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[11].Width = 120; // SWDate
            dataGridView2.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[12].Width = 60; // TypeOfSWD
            dataGridView2.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[13].Width = 90; // TypeName
            dataGridView2.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
         
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

// Link to ATMs 
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form108 NForm108; 
                NForm108 = new Form108(WSignedId, WSignRecordNo, WOperator);
                NForm108.ShowDialog(); ;
           
        }
// Show the same
        private void buttonShowUpdated_Click(object sender, EventArgs e)
        {
            WSelectionCriteria = " WHERE SWDCategory ='"+ Sc.SWDCategoryId + "'" + " AND SWVersion ='" + Sc.EffectivePackageId +"'";
            int WMode = 1;
            WDateTime = NullPastDate;

            Jd.ReadJTMIdentificationDetailsToFillPartialTable(WSelectionCriteria, WMode, WDateTime);

            ShowGrid2();
        }

        private void buttonShowNotUpdated_Click(object sender, EventArgs e)
        {
            WSelectionCriteria = " WHERE SWDCategory ='" + Sc.SWDCategoryId + "'" + " AND SWVersion <>'" + Sc.EffectivePackageId + "'";
            int WMode = 1;
            WDateTime = NullPastDate;

            Jd.ReadJTMIdentificationDetailsToFillPartialTable(WSelectionCriteria, WMode, WDateTime);

            ShowGrid2();         
        }
// ALL
        private void button1_Click(object sender, EventArgs e)
        {
            WSelectionCriteria = " WHERE SWDCategory ='" + Sc.SWDCategoryId + "'";
            int WMode = 1;
            WDateTime = NullPastDate;

            Jd.ReadJTMIdentificationDetailsToFillPartialTable(WSelectionCriteria, WMode, WDateTime);

            ShowGrid2();
        }
// Package Info
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ProcessNo = 54;  // 
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            SWD_Form271 NSWD_Form271;

            NSWD_Form271 = new SWD_Form271(WSignedId, WSignRecordNo, WOperator, "SWDCat", 0);
            NSWD_Form271.ShowDialog();
        }
    }
}
