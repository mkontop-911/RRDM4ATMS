using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80cATMs : Form
    {
        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        int WReconcCycleNo;

        //string WSelectionCriteria; 

        int WSeqNo; 

        int WRowIndex; 

        string WCategoryId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WFunction;

        public Form80cATMs(string InSignedId, int SignRecordNo, string InOperator, string InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction; // ALLOCATE

            InitializeComponent();

            // Set Working Date 
           
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            label1UserId.Text = WSignedId;

            if (WFunction == "Allocate")
            {
                textBoxMsgBoard.Text = "Assess Status and Allocate Work";
            }

            RRDMReconcJobCycles Djc = new RRDMReconcJobCycles();
            string WJobCategory = "ATMs";
            Djc.ReadLastReconcJobCycleATMsAndNostro(WOperator, WJobCategory);
            if (Djc.RecordFound == true)
            {                
                    WReconcCycleNo = Djc.JobCycle;
            }

            //TEST
            if (WOperator == "CRBAGRAA")
            {
                WReconcCycleNo = 205;
            }
            //if (WOperator == "ETHNCY2N")
            //{
            //    WReconcCycleNo = 4314;
            //}

            labelStep1.Text = "Work Alocation For Cycle No :" + WReconcCycleNo.ToString(); 

        }
// On Load
        private void Form80c_Load(object sender, EventArgs e)
        {
          
            int Mode = 1; // No Extra conditions 
            Rcs.ReadReconcCategoriesSessionsFillTable(WOperator, WReconcCycleNo, Mode);

            dataGridView1.DataSource = Rcs.TableReconcSessionsPerCategory.DefaultView;


            dataGridView1.Columns[0].Width = 40; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 70; // RunningJobNo
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].Visible = true; 

            dataGridView1.Columns[2].Width = 120; // CategoryId
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 70; // MatchedRecs
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 70; // UnMatchedRecs
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 100; // StartReconc
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 100; // EndReconc
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 90; // OwnerId
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[8].Width = 150; // OwnerName
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;       
        }

        // ON ROW ENTER CATEGORIES
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSeqNo = (int)rowSelected.Cells[0].Value;

            Rcs.ReadReconcCategoriesSessionsBySeqNo(WSeqNo);

            WCategoryId = Rcs.CategoryId; 

            if (Rcs.OwnerUserID != "")
            {
                Us.ReadUsersRecord(Rcs.OwnerUserID);

                textBoxOwner.Text = Rcs.OwnerUserID + " " + Us.UserName;
                buttonChangeOwner.Text = "Change Owner";
            }
            else
            {
                textBoxOwner.Text = "Category has no Owner.";
                buttonChangeOwner.Text = "Assign Owner";
            }

        }


        // DEFINE OWNER 
        private void buttonChangeOwner_Click(object sender, EventArgs e)
        {
            Form503_CategOwners NForm503_CategOwners;
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            int Mode = 2;
            string W_Application = ""; 
            NForm503_CategOwners = new Form503_CategOwners(WSignedId, WSignRecordNo, WOperator
                , WCategoryId, W_Application, Mode, "");
            NForm503_CategOwners.FormClosed += NForm503_CategOwners_FormClosed;
            NForm503_CategOwners.ShowDialog(); 


        }

        void NForm503_CategOwners_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form80c_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

        }
  
        // Finish 
       
        private void buttonBack_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }
  
    }
}
