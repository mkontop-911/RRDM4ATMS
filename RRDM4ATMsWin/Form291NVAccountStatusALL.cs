using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form291NVAccountStatusALL : Form
    {
        RRDMMatchingCategories Mc = new RRDMMatchingCategories();
        RRDMNVStatement_Lines_InternalAndExternal Se = new RRDMNVStatement_Lines_InternalAndExternal();

        //int RunningJob;

        string Ccy;
        string InternalAcc;
        string ExternalAcc;

        string W4DigitMainCateg;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        string WCategoryId;
        int WReconcCycleNo;
        int WMode;
        decimal Total; 

        public Form291NVAccountStatusALL(string InSignedId, int SignRecordNo,
                      string InOperator, int InReconcCycleNo, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WReconcCycleNo = InReconcCycleNo;
            WMode = InMode;
            // InMode = 1 Then only for this category ID
            // InMode = 5 Then for all categories this Cycle 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            UserId.Text = InSignedId;

            //RunningJob = 203;

            if (WMode == 1)
            {
                W4DigitMainCateg = WCategoryId.Substring(0, 4);
                //Find Internal and External Account 
                Mc.ReadMatchingCategorybyActiveCategId(WOperator, WCategoryId);
                if (W4DigitMainCateg == "EWB8")
                {
                    Ccy = Mc.VostroCurr;
                    InternalAcc = Mc.InternalAcc;
                    ExternalAcc = Mc.VostroAcc;
                }
            }

        }

        // LOAD SCREEN 
        private void Form291_Load(object sender, EventArgs e)
        {
            ShowGrid();

        }

        DataTable TableAccountStatusALL = new DataTable();
        int McSeqNo;
        int WString;
        DateTime WDate = new DateTime(2015, 02, 15);
        // Show Panel 
        private void ShowGrid()
        {
            // Read Totals and fill panel 
            if (WMode == 5)
            {
                string Origin = "Nostro - Vostro";

                Mc.ReadMatchingCategoriesAndFillTable(WOperator, Origin);

                TableAccountStatusALL = new DataTable();
                TableAccountStatusALL.Clear();

                // DATA TABLE ROWS DEFINITION 
                TableAccountStatusALL.Columns.Add("CategId", typeof(string));
                TableAccountStatusALL.Columns.Add("Internal", typeof(string));
                TableAccountStatusALL.Columns.Add("External", typeof(string));
                TableAccountStatusALL.Columns.Add("InternalBalance", typeof(decimal));
                TableAccountStatusALL.Columns.Add("InternalUnMatchedCR", typeof(decimal));
                TableAccountStatusALL.Columns.Add("InternalUnMatchedDR", typeof(decimal));
                TableAccountStatusALL.Columns.Add("ExternalUnMatchedCR", typeof(decimal));
                TableAccountStatusALL.Columns.Add("ExternalUnMatchedDR", typeof(decimal));
                TableAccountStatusALL.Columns.Add("Total", typeof(decimal));
                TableAccountStatusALL.Columns.Add("ExternalBalance", typeof(decimal));
                TableAccountStatusALL.Columns.Add("Difference", typeof(decimal));
                //Se.InternalAccBalance;

                int I = 0;

                while (I <= (Mc.TableMatchingCateg.Rows.Count - 1))
                {

                    // For each enry in table Update records. 

                    // READ 
                    McSeqNo = (int)Mc.TableMatchingCateg.Rows[I]["SeqNo"];

                    Mc.ReadMatchingCategorybySeqNoActive(WOperator, McSeqNo);

                    WString = 1;
                    Se.ReadNVStatements_LinesForTotals(WOperator, WSignedId, WString,
                                              Mc.VostroAcc, Mc.InternalAcc, WDate);

                    DataRow RowSelected = TableAccountStatusALL.NewRow();

                    RowSelected["CategId"] = Mc.CategoryId;
                    RowSelected["Internal"] = Mc.InternalAcc;
                    RowSelected["External"] = Mc.VostroAcc;
                    RowSelected["InternalBalance"] = Se.InternalAccBalance;

                    RowSelected["InternalUnMatchedCR"] = Se.UnMatchedInternalCR;
                    RowSelected["InternalUnMatchedDR"] = -Se.UnMatchedInternalDR;

                    RowSelected["ExternalUnMatchedCR"] = Se.UnMatchedExternalCR;
                    RowSelected["ExternalUnMatchedDR"] = (-Se.UnMatchedExternalDR);

                    Total = Se.InternalAccBalance + Se.UnMatchedInternalCR - Se.UnMatchedInternalDR
                             + Se.UnMatchedExternalCR - Se.UnMatchedExternalDR;

                    RowSelected["Total"] = Total;

                    RowSelected["ExternalBalance"] = Se.ExternalAccBalance;


                    RowSelected["Difference"] = Total + Se.ExternalAccBalance;

                    // ADD ROW
                    TableAccountStatusALL.Rows.Add(RowSelected);


                    I++; // Read Next entry of the table 
                }

                dataGridView1.DataSource = TableAccountStatusALL.DefaultView;

                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Format = "N2";

                dataGridView1.Columns[0].Width = 60; // CategId
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                dataGridView1.Columns[1].Width = 70; // Internal
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[1].Visible = true;

                dataGridView1.Columns[2].Width = 70; // External
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[2].Visible = true;

                dataGridView1.Columns[3].Width = 120;  // InternalBalance
                dataGridView1.Columns[3].DefaultCellStyle = style;
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[3].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[4].Width = 145;  // InternalUnMatchedCR
                dataGridView1.Columns[4].DefaultCellStyle = style;
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[4].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[5].Width = 145; //  InternalUnMatchedDR
                dataGridView1.Columns[5].DefaultCellStyle = style;
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[5].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[5].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[5].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[6].Width = 145; // ExternalUnMatchedCR
                dataGridView1.Columns[6].DefaultCellStyle = style;
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[6].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[6].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[6].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[7].Width = 145; // ExternalUnMatchedCR
                dataGridView1.Columns[7].DefaultCellStyle = style;
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                //dataGridView1.Columns[7].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[7].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[7].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[8].Width = 120; // Total
                dataGridView1.Columns[8].DefaultCellStyle = style;
                dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[8].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[8].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[8].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[9].Width = 120; // ExternalBalance
                dataGridView1.Columns[9].DefaultCellStyle = style;
                dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[9].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[9].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[9].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

                dataGridView1.Columns[10].Width = 95; // Difference
                dataGridView1.Columns[10].DefaultCellStyle = style;
                dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dataGridView1.Columns[10].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
                dataGridView1.Columns[10].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[10].HeaderCell.Style.Font = new Font("Tahoma", 09, FontStyle.Bold);

            }


            //WString = 1;

            //labelInternalAcno.Text = "INTERNAL ACCOUNT :" + InternalAcc;
            //labelExternalAcno.Text = "EXTERNAL ACCOUNT :" + ExternalAcc;

            //Se.ReadNVStatements_LinesForTotals(WOperator, WSignedId, WString,
            //                                    ExternalAcc, InternalAcc, WDate);

            //textBoxInternalAccBalance.Text = Se.InternalAccBalance.ToString("#,##0.00");
            //textBoxInternalAccTxns.Text = Se.InternalAccTxns.ToString();

            //textBoxUnMatchedInternalCR.Text = (-Se.UnMatchedInternalCR).ToString("#,##0.00");
            //textBoxUnMatchedInternalCRTxns.Text = Se.UnMatchedInternalCRTxns.ToString();

            //textBoxUnMatchedInternalDR.Text = Se.UnMatchedInternalDR.ToString("#,##0.00");
            //textBoxUnMatchedInternalDRTxns.Text = Se.UnMatchedInternalDRTxns.ToString();

            //textBoxUnMatchedExternalCREntries.Text = Se.UnMatchedExternalCR.ToString("#,##0.00");
            //textBoxUnMatchedExternalCRTxns.Text = Se.UnMatchedExternalCRTxns.ToString();

            //textBoxUnMatchedExternalDREntries.Text = (-Se.UnMatchedExternalDR).ToString("#,##0.00");
            //textBoxUnMatchedExternalDRTxns.Text = Se.UnMatchedExternalDRTxns.ToString();

            //decimal Total = Se.InternalAccBalance + Se.UnMatchedInternalCR - Se.UnMatchedInternalDR
            //                  + Se.UnMatchedExternalCR - Se.UnMatchedExternalDR;

            //textBoxTotal.Text = Total.ToString("#,##0.00");

            //textBoxExternalAccBalance.Text = Se.ExternalAccBalance.ToString("#,##0.00");
            //textBoxExternalAccTxns.Text = Se.ExternalAccTxns.ToString();

            //decimal Difference = Total + Se.ExternalAccBalance;

            //textBoxOpenAdjustmentsTxns.Text = Se.MatchedAdjustmentsOpenTxns.ToString();
            //textBoxOpenAdjustmentsAmtNegative.Text = Se.MatchedAdjustmentsOpenNegative.ToString("#,##0.00");
            //textBoxtextBoxOpenAdjustmentsAmtPositive.Text = Se.MatchedAdjustmentsOpenPositive.ToString("#,##0.00");

            //textBoxDifference.Text = Difference.ToString("#,##0.00");

            //if (Difference != 0)
            //{
            //    pictureBoxBalStatus.BackgroundImage = appResImg.RED_LIGHT_Repl;

            //    Color Red = Color.Red;
            //}

            //if (Difference == 0)
            //{
            //    pictureBoxBalStatus.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            //}
            //if (Se.ToBeConfirmedTxns > 0)
            //{
            //    labelToBeConfirmed.Show();
            //    textBoxToBeConfirmedEntries.Show();
            //    textBoxToBeConfirmedEntries.Text = Se.ToBeConfirmedTxns.ToString();
            //}
            //else
            //{
            //    labelToBeConfirmed.Hide();
            //    textBoxToBeConfirmedEntries.Hide();
            //}

            //textBoxOpenAdjustmentsTxns.Text = Se.MatchedAdjustmentsOpenTxns.ToString("#,##0.00");

            //if (Se.MatchedAdjustmentsOpenTxns > 0)
            //{
            //    textBoxOpenAdjustmentsAmtNegative.Text = Se.MatchedAdjustmentsOpenNegative.ToString("#,##0.00");
            //    textBoxtextBoxOpenAdjustmentsAmtPositive.Text = Se.MatchedAdjustmentsOpenPositive.ToString("#,##0.00");
            //}
            //else
            //{
            //    labelDr.Hide();
            //    labelCr.Hide();
            //    textBoxOpenAdjustmentsAmtNegative.Hide();
            //    textBoxtextBoxOpenAdjustmentsAmtPositive.Hide();
            //}
        }

        // ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WCategoryId = (string)rowSelected.Cells[0].Value;
        }

        // Expand Account Status 
        private void buttonExpandAccStatus_Click(object sender, EventArgs e)
        {
            Form291NVAccountStatus NForm291NVAccountStatus;

            int WRunningCycle = 503;
            int Mode = 1;
            NForm291NVAccountStatus = new Form291NVAccountStatus(WSignedId, WSignRecordNo, WOperator, WCategoryId, WRunningCycle, Mode);
            NForm291NVAccountStatus.Show();
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void buttonRefreshBal_Click(object sender, EventArgs e)
        {
            Form291_Load(this, new EventArgs()); // Load Grid    
        }


    }
}
