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

// Alecos
using System.Configuration;
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form80c : Form
    {
        RRDMReconcCategories Rc = new RRDMReconcCategories();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMReconcCategoriesMatchingSessions Rms = new RRDMReconcCategoriesMatchingSessions();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        int WRowIndex; 

        int WMRCycleNo;

        string WCategoryId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WFunction;

        public Form80c(string InSignedId, int SignRecordNo, string InOperator, string InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction; // ALLOCATE

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
            label1UserId.Text = WSignedId;


            if (WFunction == "Allocate")
            {
                textBoxMsgBoard.Text = "Assess Status and Allocate Work";
            }

            Gp.ParamId = "707"; // fILTER 
            comboBoxFilter.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxFilter.DisplayMember = "DisplayValue";


        }
// On Load
        private void Form80c_Load(object sender, EventArgs e)
        {
           
            Rc.ReadReconcCategoriesForAllocation(WOperator);

            dataGridView1.DataSource = Rc.ReconcCateg.DefaultView;

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Width = 60; // Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 120; // Name
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 60; // Number of exceptions 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].Width = 60; // Has Owner
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].Width = 60; // Owner
        }

        // ON ROW ENTER CATEGORIES
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WCategoryId = rowSelected.Cells[0].Value.ToString();

            Rc.ReadReconcCategorybyCategId(WOperator, WCategoryId);


            if (Rc.HasOwner == true)
            {
                Us.ReadUsersRecord(Rc.OwnerId);

                textBoxOwner.Text = Rc.OwnerId + " " + Us.UserName;
                buttonChangeOwner.Text = "Change Owner";
            }
            else
            {
                textBoxOwner.Text = "Category has no Owner.";
                buttonChangeOwner.Text = "Assign Owner";
            }

            Rms.ReadReconcCategoriesMatchingSessionsSpecificCat(WOperator, WCategoryId);

            dataGridView2.DataSource = Rms.TableMatchingSessionsPerCategory.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                label2.Hide();
                panel3.Hide();
                MessageBox.Show("No testing data for this category");
            }
            else
            {
                label2.Show();
                panel3.Show();
            }


            dataGridView2.Columns[0].Width = 60; // Cycle no
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);

            dataGridView2.Columns[1].Width = 70; // 
            dataGridView2.Columns[1].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[2].Width = 100; // start dt 
            dataGridView2.Columns[2].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[3].Width = 100; //  End dt 
            dataGridView2.Columns[3].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[4].Width = 70; //  Number of unmatched records 
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.Columns[5].Width = 70; //  Number of process files  
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        // On Row Enter for Matching and reconciliations Cycles 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WMRCycleNo = (int)rowSelected.Cells[0].Value;

            Rms.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WMRCycleNo);


            label3.Text = "DETAILS FOR CYCLE : " + WMRCycleNo.ToString();

            //---------------------------------------------------------------------
            // MATCHING  
            //---------------------------------------------------------------------

            labelDateStart.Text = "Date Start : " + Rms.StartDateTm.ToString();
            label12.Text = "Date End : " + Rms.EndDateTm.ToString();
            label21.Text = "Records processed : " + Rms.NumberOfMatchedRecs.ToString("#,##0");


            TimeSpan Remain1 = Rms.EndDateTm - Rms.StartDateTm;
            label13.Text = "Time Duration in Minutes : " + Remain1.Minutes.ToString("#,##0.00");

            label15.Text = "Exceptions : " + Rms.NumberOfUnMatchedRecs.ToString();

            if (Remain1.Minutes > 25)
            {
                pictureBox2.BackgroundImage = Properties.Resources.YELLOW_Repl;
            }

            if (Remain1.Minutes > 30)
            {
                pictureBox2.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;

                Color Red = Color.Red;

                label13.ForeColor = Red;
            }
            else
            {
                Color Black = Color.Black;

                label13.ForeColor = Black;
            }

            if (Remain1.Minutes < 25)
            {
                pictureBox2.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl;
            }
            //---------------------------------------------------------------------
            // RECONCILIATION 
            //---------------------------------------------------------------------
            if (Rms.StartReconcDtTm == NullPastDate)
            {
                label20.Text = "Reconciliation didnt start";
                label19.Text = "Reconciliation didnt start";
                panel4.Hide();

            }
            else
            {
                panel4.Show();

                label20.Text = "Date Start : " + Rms.StartReconcDtTm.ToString();
                label19.Text = "Date End : " + Rms.EndReconcDtTm.ToString();

                TimeSpan Remain2 = Rms.EndReconcDtTm - Rms.StartReconcDtTm;
                label18.Text = "Duration in Hours : " + Remain2.Hours.ToString("#,##0.00");

                label16.Text = "Remaining Exceptions : " + Rms.RemainReconcExceptions.ToString();

                if (Remain2.Hours > 2)
                {
                    pictureBox3.BackgroundImage = Properties.Resources.YELLOW_Repl;
                }

                if (Remain2.Hours > 2.5)
                {
                    pictureBox3.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;

                    Color Red = Color.Red;

                    label18.ForeColor = Red;
                }
                else
                {
                    Color Black = Color.Black;

                    label18.ForeColor = Black;
                }

                if (Remain2.Hours < 2)
                {
                    pictureBox3.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl;
                }

                if (Rms.RemainReconcExceptions > 0)
                {
                    pictureBox3.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;

                    Color Red = Color.Red;

                    label16.ForeColor = Red;
                }
                else
                {
                    Color Black = Color.Black;

                    label16.ForeColor = Black;
                }

            }
        }

        // DEFINE OWNER 
        private void buttonChangeOwner_Click(object sender, EventArgs e)
        {
            Form503_CategOwners NForm503_CategOwners;
            WRowIndex = dataGridView1.SelectedRows[0].Index;

            NForm503_CategOwners = new Form503_CategOwners(WSignedId, WSignRecordNo, WOperator, WCategoryId);
            NForm503_CategOwners.FormClosed += NForm503_CategOwners_FormClosed;
            NForm503_CategOwners.ShowDialog(); 
        }

        void NForm503_CategOwners_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form80c_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));
        }


        // See the Files 

        private void button6_Click_1(object sender, EventArgs e)
        {
            Form80b NForm80b;

            NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, WCategoryId, WMRCycleNo, 0,"View");
            NForm80b.ShowDialog();
        }
  
        // Finish 
       
        private void buttonBack_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }

    
    }
}
