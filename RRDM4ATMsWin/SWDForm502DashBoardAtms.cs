using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class SWDForm502DashBoardAtms : Form
    {
        RRDMSWDCategories Sc = new RRDMSWDCategories();
        RRDMSWDPackagesDistributionSessions Ds = new RRDMSWDPackagesDistributionSessions();
      
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();
        RRDMJTMQueue Jq = new RRDMJTMQueue();
        RRDMUsersRecords Us = new RRDMUsersRecords(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);
        DateTime WDateTime;
        //int WSeqNoLeft;

        string WSelectionCriteria; 
        
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WSWDCategory;
        int WTypeOfDistribution; 
       
        public SWDForm502DashBoardAtms(string InSignedId, int SignRecordNo, string InOperator, string InSWDCategory, int InType)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WSWDCategory = InSWDCategory;
            WTypeOfDistribution = InType;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            labelUserId.Text = InSignedId;

            if (WTypeOfDistribution == 1)
            {
                WSelectionCriteria = " WHERE SWDCategory ='" + WSWDCategory + "' AND TypeOfSWD = 1";              
            }
            if (WTypeOfDistribution == 2)
            {
                WSelectionCriteria = " WHERE SWDCategory ='" + WSWDCategory + "' AND TypeOfSWD = 2";
            }
            if (WTypeOfDistribution == 3)
            {
                WSelectionCriteria = " WHERE SWDCategory ='" + WSWDCategory + "' AND TypeOfSWD = 3";
            }          

        }
// Load 
        private void Form502_Load(object sender, EventArgs e)
        {
            // 
            //WSelectionCriteria = " WHERE SWDCategory ='" + WSWDCategory + "'";
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

    }
}
