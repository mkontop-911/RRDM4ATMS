using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80cNV : Form
    {
        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMNVReconcCategoriesSessions Rcs = new RRDMNVReconcCategoriesSessions();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        int WReconcCycleNo;

        int WSeqNo; 

        int WRowIndex; 

        string WCategoryId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WFunction;

        public Form80cNV(string InSignedId, int SignRecordNo, string InOperator, string InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction; // ALLOCATE

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            label1UserId.Text = WSignedId;

            if (WFunction == "Allocate")
            {
                textBoxMsgBoard.Text = "Assess Status and Allocate Work";
            }

            RRDMReconcJobCycles Djc = new RRDMReconcJobCycles();
            string WJobCategory = "NostroReconciliation";
            Djc.ReadLastReconcJobCycleATMsAndNostro(WOperator, WJobCategory);
            if (Djc.RecordFound == true)
            {                
                    WReconcCycleNo = Djc.JobCycle;
            }

            labelStep1.Text = "Work Allocation For Job Cycle No :" + WReconcCycleNo; 

        }
// On Load
        private void Form80c_Load(object sender, EventArgs e)
        {
            string SelectionCriteria = " WHERE RunningJobNo =" + WReconcCycleNo 
                                                  + " AND UnMatchedAmt > 0 "; 
                   
            Rcs.ReadNVReconcCategoriesSessionsSpecificRunningJobCycle(SelectionCriteria);

            ShowGridRcs(); 
        }

        // ON ROW ENTER CATEGORIES
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WSeqNo = (int)rowSelected.Cells[0].Value;

            Rcs.ReadNVReconcCategoriesSessionsBySeqNo(WSeqNo);

            WCategoryId = Rcs.CategoryId; 

            if (Rcs.OwnerId != "")
            {
                Us.ReadUsersRecord(Rcs.OwnerId);

                textBoxOwner.Text = Rcs.OwnerId + " " + Us.UserName;
                buttonChangeOwner.Text = "Change Owner";
            }
            else
            {
                textBoxOwner.Text = "Category has no Owner.";
                buttonChangeOwner.Text = "Assign Owner";
            }

        }
// Show Grid RCS
        private void ShowGridRcs()
        {
            dataGridView1.DataSource = Rcs.TableReconcSessionsPerCategory.DefaultView;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Format = "N2";

            dataGridView1.Columns[0].Width = 40; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 70; // CategoryId
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns[2].Width = 190; // Name
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 60; // ccy 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 90; // MatchedAuto
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 90; // ToBeConfirmed
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 90; // UnMatched
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[7].Width = 120; // LocalAmt
            dataGridView1.Columns[7].DefaultCellStyle = style;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[7].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[8].Width = 80; // Alerts
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[9].Width = 80; // Disputes
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[10].Width = 90; // OwnerId
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[11].Width = 150; // OwnerName
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[12].Width = 200; //StartManual
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[13].Width = 200; //EndManual
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // DEFINE OWNER 
        private void buttonChangeOwner_Click(object sender, EventArgs e)
        {
            Form503_CategOwners NForm503_CategOwners;
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            int Mode = 4;
            string W_Application = "";

            NForm503_CategOwners = new Form503_CategOwners(WSignedId, WSignRecordNo, WOperator
                                                                          , WCategoryId, W_Application, Mode,"");
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
