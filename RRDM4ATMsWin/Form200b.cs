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
    public partial class Form200b : Form
    {
        RRDMReconcCategories Rc = new RRDMReconcCategories();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMReconcCategoriesVsSourcesFiles Rmf = new RRDMReconcCategoriesVsSourcesFiles(); 

        RRDMReconcCategoriesMatchingSessions Rms = new RRDMReconcCategoriesMatchingSessions();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        int WSeqNo; 

        //int WRowIndex;

        //int WMRCycleNo;

        string WCategoryId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WFunction;

        public Form200b(string InSignedId, int SignRecordNo, string InOperator, string InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WFunction = InFunction; // MATCHINGSTATUS

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
            label1UserId.Text = WSignedId;


            if (WFunction == "MATCHINGSTATUS")
            {
                textBoxMsgBoard.Text = "View Matching Status. Categories on the left, Files on the right. ";
            }

        }
// LOAD FORM 
        private void Form200b_Load(object sender, EventArgs e)
        {
            Rc.ReadReconcCategoriesForMatchingStatus(WOperator);

            dataGridView1.DataSource = Rc.ReconcCateg.DefaultView;

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Width = 60; // Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 120; // Name
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 60; // Number of exceptions 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].Width = 60; // Has Owner
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

                //textBoxOwner.Text = Rc.OwnerId + " " + Us.UserName;
                //buttonChangeOwner.Text = "Change Owner";
            }
            else
            {
                //textBoxOwner.Text = "Category has no Owner.";
                //buttonChangeOwner.Text = "Assign Owner";
            }

            Rmf.ReadReconcCategoryVsSourcesANDFillTable(WCategoryId);

            dataGridView2.DataSource = Rmf.RMCategoryFiles.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
           
                //MessageBox.Show("No testing data for this category");
            }
            else
            {
              
            }

            dataGridView2.Columns[0].Width = 70; // 
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

            WSeqNo = (int)rowSelected.Cells[0].Value;

            Rmf.ReadReconcCategoriesVsSourcebySeqNo(WSeqNo); 

          

            //label3.Text = "DETAILS FOR CYCLE : " + WMRCycleNo.ToString();

            ////---------------------------------------------------------------------
            //// MATCHING  
            ////---------------------------------------------------------------------

            //labelDateStart.Text = "Date Start : " + Rms.StartDateTm.ToString();
            //label12.Text = "Date End : " + Rms.EndDateTm.ToString();
            //label21.Text = "Records processed : " + Rms.NumberOfProcessRec.ToString("#,##0");


            //TimeSpan Remain1 = Rms.EndDateTm - Rms.StartDateTm;
            //label13.Text = "Time Duration in Minutes : " + Remain1.Minutes.ToString("#,##0.00");

            //label15.Text = "Exceptions : " + Rms.NumberOfExceptions.ToString();

            //if (Remain1.Minutes > 25)
            //{
            //    pictureBox2.BackgroundImage = Properties.Resources.YELLOW_Repl;
            //}

            //if (Remain1.Minutes > 30)
            //{
            //    pictureBox2.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;

            //    Color Red = Color.Red;

            //    label13.ForeColor = Red;
            //}
            //else
            //{
            //    Color Black = Color.Black;

            //    label13.ForeColor = Black;
            //}

            //if (Remain1.Minutes < 25)
            //{
            //    pictureBox2.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl;
            //}
           
        }


        // See the Files 

        private void button6_Click_1(object sender, EventArgs e)
        {
            //Form80b NForm80b;

            MessageBox.Show("FUNCTIONALITY UNDER DEVELOPMENT");

            //NForm80b = new Form80b(WSignedId, WSignRecordNo, WOperator, WCategoryId, WMRCycleNo, "View");
            //NForm80b.ShowDialog();
        }

        // Finish 

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}
