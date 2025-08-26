using System;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Threading;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
//using System.Data.OleDb;
using RRDM4ATMs;
using RRDMAgent_Classes;
using System.Globalization;


namespace RRDM4ATMsWin
{
    public partial class Form200_SET_Vs_CAP : Form
    {
        // Variables

       // Form54 NForm54;
        Form55 NForm55;

        public bool Prive;

        //int WAction;

        int WJobCycleNo;

        string WCategoryId;

       // DateTime WCut_Off_Date;

        string MsgFilter;


        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();


        DateTime NullPastDate = new DateTime(1900, 01, 01);

        DateTime NullFutureDate = new DateTime(2050, 11, 21);


        string WSignedId;
       
        string WOperator;

        string WMatchingCateg;
        int WMatchingAtRMCycle; 

        // Methods 
        // READ ATMs Main
        // 
        public Form200_SET_Vs_CAP(string InSignedId, string InOperator
                                                                     , string InMatchingCateg, int InMatchingAtRMCycle)
        {
            WSignedId = InSignedId;
          
            WOperator = InOperator;

            WMatchingCateg = InMatchingCateg;

            WMatchingAtRMCycle = InMatchingAtRMCycle;

            InitializeComponent();

            // Set Working Date 

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            labelStep1.Text = "Settlement Date Vs Cap Dates for.."+ WMatchingCateg; 

            //*****************************************
            //
            //*****************************************

            textBoxMsgBoard.Text = "Job Cycles ";

            // ....

         

        }
     
        // Load
        private void Form200JobCycles_Load(object sender, EventArgs e)
        {

            Mpa.ReadMatchingTxnsMasterPool_FOR_SET_VS_CAP_DATE_Fill_Table(WMatchingCateg, WMatchingAtRMCycle); 

            ShowGrid1();

        }

        DateTime WSET_DATE;
        DateTime WCAP_DATE; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSET_DATE = (DateTime)rowSelected.Cells[0].Value;
            WCAP_DATE = (DateTime)rowSelected.Cells[1].Value;
            string WTransCurr= (string)rowSelected.Cells[2].Value;

            Mpa.ReadMatchingTxnsMasterPool_FOR_SET_VS_CAP_DATE_Fill_Table_TXNS(WMatchingCateg, WMatchingAtRMCycle
                                                                         , WSET_DATE, WCAP_DATE, WTransCurr); 

            ShowGrid2();

        }

        // ROW ENTER 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];
        }

        // Show Grid1
        private void ShowGrid1()
        {
            if (Mpa.DataTable_SET_VS_CAP.Rows.Count ==0)
            {
                return; 
            }

            dataGridView1.DataSource = Mpa.DataTable_SET_VS_CAP.DefaultView;

            dataGridView1.Columns[0].Width = 90; 
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 90;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 60; //TXNs
            dataGridView1.Columns[2].DefaultCellStyle.Format = "#,###";
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dataGridView1.Columns[3].Width = 120; // AMT 
            dataGridView1.Columns[3].DefaultCellStyle.Format = "#,##0.00";
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

        }

        // Show Grid2
        private void ShowGrid2()
        {
            // GL_Number, OriginFile , MatchingCateg As MatchingCategories

            
            dataGridView2.DataSource = Mpa.DataTable_SET_VS_CAP_TXNS.DefaultView;

            if (Mpa.DataTable_SET_VS_CAP_TXNS.Rows.Count == 0)
            {
                MessageBox.Show("Nothing to show.");
                panel4.Hide();
               // labelHeaderRight.Hide();
            }
            else
            {
                panel4.Show();
              //  labelHeaderRight.Show();
            }


            dataGridView2.Columns[0].Width = 120; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView3.Columns[0].Visible = false;

            //dataGridView2.Columns[1].Width = 230; // W_Identity
            //dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView2.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            //dataGridView2.Columns[2].Width = 60; //CB_TXNs
            //dataGridView2.Columns[2].DefaultCellStyle.Format = "#,###";
            //dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView2.Columns[3].Width = 120; // CB_TXNs_AMT 
            //dataGridView2.Columns[3].DefaultCellStyle.Format = "#,##0.00";
            //dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            //dataGridView2.Columns[4].Width = 60; // 2
            //dataGridView2.Columns[4].DefaultCellStyle.Format = "#,###";
            //dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView2.Columns[5].Width = 120; //  
            //dataGridView2.Columns[5].DefaultCellStyle.Format = "#,##0.00";
            //dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            //dataGridView2.Columns[6].Width = 60; // 3
            //dataGridView2.Columns[6].DefaultCellStyle.Format = "#,###";
            //dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            //dataGridView2.Columns[7].Width = 120; //  
            //dataGridView2.Columns[7].DefaultCellStyle.Format = "#,##0.00";
            //dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView2.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
        }

        
        // Message from Controller 
        private void buttonMsgs_Click_1(object sender, EventArgs e)
        {
            return; 
            
        }
        // Todays Controller 
        private void buttonCommController_Click_1(object sender, EventArgs e)
        {
            
        }
        //Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


       

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

      
        // Auditors Report 
        int WRowIndexLeft;
        

        private void NForm80b3_FormClosed(object sender, FormClosedEventArgs e)
        {
           
            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            Form200JobCycles_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndexLeft].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndexLeft));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
       
     

    }
}
