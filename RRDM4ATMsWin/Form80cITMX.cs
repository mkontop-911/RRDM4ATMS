using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

// Alecos

namespace RRDM4ATMsWin
{
    public partial class Form80cITMX : Form
    {
        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();

        RRDMMatchingCategoriesSessions Mcs = new RRDMMatchingCategoriesSessions();

        RRDMGasParameters Gp = new RRDMGasParameters(); 

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        int WReconcCycleNo; 

        int WRowIndex; 

        string WCategoryId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WFunction;

        public Form80cITMX(string InSignedId, int SignRecordNo, string InOperator, string InFunction)
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

            Gp.ParamId = "707"; // fILTER 
            comboBoxFilter.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxFilter.DisplayMember = "DisplayValue";


        }
// On Load
        private void Form80c_Load(object sender, EventArgs e)
        {
            //Fill THE TABLE WITH ALL RECONCILIATION CATEGORIES THAT NEED RECONCILIATION 
           
            Rcs.ReadReconcCategoriesDistinctFillTable(WOperator, 1, "ITMX", "" ); 

            dataGridView1.DataSource = Rcs.TableReconcSessionsDistinct.DefaultView;

            //   Rc.ReadReconcCategories(WOperator,WOrigin, WRMCategory);         

            dataGridView1.Columns[0].Width = 140; // Category Name
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[1].Width = 100; // id 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 80; // id 
            dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView1.Columns[2].DefaultCellStyle.ForeColor = Color.Red;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 80; // id 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        // ON ROW ENTER CATEGORIES
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WCategoryId = rowSelected.Cells[1].Value.ToString();

            Rcs.ReadReconcCategoriesSessionsSpecificCat(WOperator,WSignedId, WCategoryId, 0);
            dataGridView2.DataSource = Rcs.TableReconcSessionsPerCategory.DefaultView;

            //Rms.ReadReconcCategoriesMatchingSessionsSpecificCat(WOperator, WCategoryId);

            //dataGridView2.DataSource = Rms.TableMatchingSessionsPerCategory.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                labelRight.Hide();
                panel3.Hide();
                MessageBox.Show("No testing data for this category");
            }
            else
            {
                labelRight.Show();
                panel3.Show();
            }

            dataGridView2.Columns[0].Width = 60; // 
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            //dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);

            dataGridView2.Columns[1].Width = 70; // 
            dataGridView2.Columns[1].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[2].Width = 100; // 
            dataGridView2.Columns[2].DefaultCellStyle.ForeColor = Color.LightSlateGray;

            dataGridView2.Columns[3].Width = 100; // 
            dataGridView2.Columns[3].DefaultCellStyle.ForeColor = Color.LightSlateGray;

        }

        // On Row Enter for Matching and reconciliations Cycles 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WReconcCycleNo = (int)rowSelected.Cells[0].Value;

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WReconcCycleNo);

            //Rms.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WRMCycleNo);

            label3.Text = "DETAILS FOR CYCLE : " + WReconcCycleNo.ToString();

            //---------------------------------------------------------------------
            // Reconciliation   
            //---------------------------------------------------------------------
            Us.ReadUsersRecord(Rcs.OwnerUserID); 
            textBoxOwner.Text = Us.UserName; 

            labelDateStart.Text = "Date Start : " + Rcs.StartDateTm.ToString();
            label12.Text = "Date End  : " + Rcs.EndDateTm.ToString();
            label21.Text = "Records Matched : " + Rcs.NumberOfMatchedRecs.ToString("#,##0");

            TimeSpan Remain1 = Rcs.EndDateTm - Rcs.StartDateTm;
            label13.Text = "Time Duration in Minutes : " + Remain1.Minutes.ToString("#,##0.00");

            label15.Text = "Exceptions : " + Rcs.NumberOfUnMatchedRecs.ToString();

            if (Remain1.Minutes > 25)
            {
                pictureBox2.BackgroundImage = appResImg.YELLOW_Repl;
            }

            if (Remain1.Minutes > 30)
            {
                pictureBox2.BackgroundImage = appResImg.RED_LIGHT_Repl;

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
                pictureBox2.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            }
            //---------------------------------------------------------------------
            // RECONCILIATION 
            //---------------------------------------------------------------------
            if (Rcs.StartReconcDtTm == NullPastDate)
            {
                label20.Text = "Reconciliation didnt start";
                label19.Text = "Reconciliation didnt start";
                panel4.Hide();

            }
            else
            {
                if (Rcs.EndReconcDtTm == NullPastDate)
                {
                    panel4.Hide();
                    label20.Text = "Date Start : " + Rcs.StartReconcDtTm.ToString();
                    label19.Text = "Reconciliation is in progess";
                }
                else
                {
                    panel4.Show();

                    label20.Text = "Date Start : " + Rcs.StartReconcDtTm.ToString();
                    label19.Text = "Date End  : " + Rcs.EndReconcDtTm.ToString();

                    TimeSpan Remain2 = Rcs.EndReconcDtTm - Rcs.StartReconcDtTm;
                    label18.Text = "Duration in Minutes : " + Remain2.Minutes.ToString("#,##0.00");
                    Mp.ReadMatchingTxnsMasterPoolForTotalsForUnMatched(WOperator, WCategoryId, WReconcCycleNo);
                    //Rm.ReadMatchedORUnMatchedFileForTotals(WOperator, WCategoryId, WRMCycleNo);

                    label16.Text = "Remaining Exceptions : " + Mp.TotalUnMatched.ToString();

                   

                    if (Mp.TotalUnMatched > 0)
                    {
                        pictureBox3.BackgroundImage = appResImg.RED_LIGHT_Repl;

                        Color Red = Color.Red;

                        label18.ForeColor = Red;
                    }
                    else
                    {
                        Color Black = Color.Black;

                        label18.ForeColor = Black;
                    }

                    if (Mp.TotalUnMatched == 0)
                    {
                        pictureBox3.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
                    }

                    if (Rcs.RemainReconcExceptions > 0)
                    {
                        pictureBox3.BackgroundImage = appResImg.RED_LIGHT_Repl;

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

        }

        // DEFINE OWNER 
        private void buttonChangeOwner_Click(object sender, EventArgs e)
        {
            Form503_CategOwners NForm503_CategOwners;
            WRowIndex = dataGridView1.SelectedRows[0].Index;
            int Mode = 3;
            string W_Application = ""; 
            NForm503_CategOwners = new Form503_CategOwners(WSignedId, WSignRecordNo, WOperator 
                                                                 ,WCategoryId, W_Application, Mode, "");
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
            Form80bΙΤΜΧ NForm80bITMX;

            NForm80bITMX = new Form80bΙΤΜΧ(WSignedId, WSignRecordNo, WOperator, WCategoryId, WReconcCycleNo, 0,"View");
            NForm80bITMX.ShowDialog();
        }
  
        // Finish 
       
        private void buttonBack_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }

    
    }
}
