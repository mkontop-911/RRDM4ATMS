using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form78b : Form
    {
        //RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();

        // Define the data table 
        public DataTable WTransToBePostedSelected = new DataTable();

        public int WPostedNo;
        public int UniqueIsChosen;

        public int WMaskRecordId; 

        public int WSelectedRow = 0;

        ////bool WithDate;
        //string Gridfilter; 
    
        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WHeader;
        string WFromForm; 

        public Form78b(string InSignedId, int InSignRecordNo, string InOperator, DataTable InTransToBePostedSelected, string InHeader, string InFromForm)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WHeader = InHeader;
            WFromForm = InFromForm; 

            WTransToBePostedSelected = new DataTable();
            WTransToBePostedSelected.Clear();
            
            WTransToBePostedSelected = InTransToBePostedSelected;

            if (WTransToBePostedSelected.Rows.Count == 0)
            {
                MessageBox.Show(" Nothing to show! ");
                return; 
            }

                InitializeComponent();

            labelWhatGrid.Text = WHeader; 

            if (WOperator == "ITMX")
            {
                button1.Hide(); 
            }
        }

        private void Form78b_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = WTransToBePostedSelected.DefaultView;
            // SHOW GRID
            if (WOperator == "ITMX" & WFromForm == "Form80bITMX")
            {
                ShowGridLeft();          
            }
            else
            {
                if (WFromForm == "Form502b")
                {
                    dataGridView1.Columns[0].Width = 50; // 
                    dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                    dataGridView1.Columns[1].Width = 200; // 
                    dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                }
                else
                {
                    if (WFromForm == "Form50b_AUDI")
                    {
                        ShowGrid_AUDI(); 
                    }
                    else
                    {
                        if (WFromForm == "Form200bITMX")
                        {
                            dataGridView1.Columns[0].Width = 190; // 
                            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                            dataGridView1.Columns[1].Width = 100; // 
                            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                            dataGridView1.Columns[2].Width = 130; // 
                            dataGridView1.Columns[3].Width = 130; // 
                            dataGridView1.Columns[4].Width = 130; // 
                        }
                        else
                        {
                            DataGridViewCellStyle style = new DataGridViewCellStyle();
                            style.Format = "N2";


                            dataGridView1.Columns[0].Width = 40; // MaskRecordId
                            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                            dataGridView1.Columns[1].Width = 40; // Status
                            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                            dataGridView1.Columns[2].Width = 40; //  Done
                            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                            dataGridView1.Columns[3].Width = 70; // Terminal
                            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                            dataGridView1.Columns[4].Width = 60; // Terminal Type, ATM, POS etc 
                            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                            dataGridView1.Columns[5].Width = 100; // Descr
                            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                            dataGridView1.Columns[6].Width = 60; // Exception
                            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                            dataGridView1.Columns[7].Width = 60; // MatchMask
                            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                            dataGridView1.Columns[8].Width = 90; // Account
                            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

                            dataGridView1.Columns[9].Width = 50; // Ccy
                            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                            dataGridView1.Columns[10].Width = 80; // Amount
                            dataGridView1.Columns[10].DefaultCellStyle = style;
                            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            dataGridView1.Columns[10].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                            dataGridView1.Columns[10].DefaultCellStyle.ForeColor = Color.Red;
                            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Purple;

                            dataGridView1.Columns[11].Width = 120; // Date
                            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                            dataGridView1.Columns[12].Width = 70; // ActionType
                            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                    }
                  
                      
                }
            }
           
        }

        // Show Grid Left 
        public void ShowGridLeft()
        {

            //dataGridView1.DataSource = Mp.MatchingMasterDataTableLeft.DefaultView;

            dataGridView1.Columns[0].Width = 40; // RecordId
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            dataGridView1.Columns[1].Width = 100; // ReconcCategory
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[2].Width = 40; //  RecCycle


            dataGridView1.Columns[3].Width = 50; // Ccy

            dataGridView1.Columns[4].Width = 70; // Amount
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Ascending);

            dataGridView1.Columns[5].Width = 80; // ExecutionTxnDtTm

            dataGridView1.Columns[6].Width = 40; // DebitMASK
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Width = 40; //CreditMASK
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // DATA TABLE ROWS DEFINITION 

            //        // This is InMode == 1
            //        MatchingMasterDataTableLeft.Columns.Add("RecordId", typeof(int));
            //        MatchingMasterDataTableLeft.Columns.Add("ReconcCategory", typeof(string));
            //        MatchingMasterDataTableLeft.Columns.Add("RecCycle", typeof(string));

            //MatchingMasterDataTableLeft.Columns.Add("Ccy", typeof(string));
            //MatchingMasterDataTableLeft.Columns.Add("Amount", typeof(string));
            //MatchingMasterDataTableLeft.Columns.Add("ExecutionTxnDtTm", typeof(DateTime));
            //MatchingMasterDataTableLeft.Columns.Add("DebitMASK", typeof(string));
            //MatchingMasterDataTableLeft.Columns.Add("CreditMASK", typeof(string));

        }

        // Show Grid AUDI
        public void ShowGrid_AUDI()
        {

            DataGridViewCellStyle style = new DataGridViewCellStyle();

            style.Format = "N2";

            dataGridView1.Columns[0].Width = 120; // Day
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 120; // Date
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 140; // Amount
            dataGridView1.Columns[2].DefaultCellStyle = style;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView1.Columns[3].Width = 120; // type
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 140; // Amount
            dataGridView1.Columns[4].DefaultCellStyle = style;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
        }

        // On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            if (WOperator == "ITMX" & WFromForm =="Form200bITMX")
            {
                labelSelected.Hide(); 
                return; 
            }

            if (WOperator == "ITMX" )
            {
                WMaskRecordId = (int)rowSelected.Cells[0].Value;

                labelSelected.Text = "Selected Item : " + WMaskRecordId.ToString();
            }
            else
            {
                if (WFromForm == "Form50b_AUDI")
                {
                    labelSelected.Text = "AUDI ATM " ;
                }
                else
                {
                    WPostedNo = (int)rowSelected.Cells[0].Value;

                    labelSelected.Text = "Selected Item : " + WPostedNo.ToString();
                }
                   
            }
            

            //WSelectedRow = e.RowIndex;

           
        }
// FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
// Select this unique 
        private void button1_Click(object sender, EventArgs e)
        {
            UniqueIsChosen = 1; 
            this.Dispose(); 
        }

    }
}
