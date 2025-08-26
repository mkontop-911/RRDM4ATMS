using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Text;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form291NVAccountStatus : Form
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
      
        public Form291NVAccountStatus(string InSignedId, int SignRecordNo, 
                      string InOperator, string InReconcCategoryId, int InReconcCycleNo, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WCategoryId = InReconcCategoryId;
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
                if (Mc.RecordFound)
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
            ShowPanel(); 
           
        }

        DataTable TableAccountStatus = new DataTable();
        int McSeqNo;
        int WString ;
        DateTime WDate = new DateTime(2015, 02, 15);
        // Show Panel 
        private void ShowPanel()
        {
            // Read Totals and fill panel 
            if (WMode == 5)
            {
                string Origin = "Nostro - Vostro";
                Mc.ReadMatchingCategoriesAndFillTable(WOperator, Origin);


                TableAccountStatus = new DataTable();
                TableAccountStatus.Clear();

                // DATA TABLE ROWS DEFINITION 
                TableAccountStatus.Columns.Add("SeqNo", typeof(int));
                TableAccountStatus.Columns.Add("Identity", typeof(string));

                int I = 0;

                while (I <= (Mc.TableMatchingCateg.Rows.Count - 1))
                {

                    // For each enry in table Update records. 

                    // READ 
                    McSeqNo = (int)Mc.TableMatchingCateg.Rows[I]["SeqNo"];

                    Mc.ReadMatchingCategorybySeqNoActive(WOperator, McSeqNo);

                    WString = 1; 
                    Se.ReadNVStatements_LinesForTotals(WOperator, WSignedId, WString,
                                               ExternalAcc, InternalAcc, WDate);
                    //VostroBank = (string)rdr["VostroBank"];
                    //VostroCurr = (string)rdr["VostroCurr"];
                    //VostroAcc = (string)rdr["VostroAcc"];
                    //InternalAcc = (string)rdr["InternalAcc"];

                    I++; // Read Next entry of the table 
                }
                }
            
            WString = 1;
           
            labelInternalAcno.Text = "INTERNAL ACCOUNT :" + InternalAcc;
            labelExternalAcno.Text = "EXTERNAL ACCOUNT :" + ExternalAcc;

            Se.ReadNVStatements_LinesForTotals(WOperator, WSignedId, WString,
                                                ExternalAcc, InternalAcc, WDate);

            textBoxInternalAccBalance.Text = Se.InternalAccBalance.ToString("#,##0.00");
            textBoxInternalAccTxns.Text = Se.InternalAccTxns.ToString();

            textBoxUnMatchedInternalCR.Text = Se.UnMatchedInternalCR.ToString("#,##0.00");
            textBoxUnMatchedInternalCRTxns.Text = Se.UnMatchedInternalCRTxns.ToString();

            textBoxUnMatchedInternalDR.Text = (-Se.UnMatchedInternalDR).ToString("#,##0.00");
            textBoxUnMatchedInternalDRTxns.Text = Se.UnMatchedInternalDRTxns.ToString();

            textBoxUnMatchedExternalCREntries.Text = Se.UnMatchedExternalCR.ToString("#,##0.00");
            textBoxUnMatchedExternalCRTxns.Text = Se.UnMatchedExternalCRTxns.ToString();

            textBoxUnMatchedExternalDREntries.Text = (-Se.UnMatchedExternalDR).ToString("#,##0.00");
            textBoxUnMatchedExternalDRTxns.Text = Se.UnMatchedExternalDRTxns.ToString();

            decimal Total = Se.InternalAccBalance + Se.UnMatchedInternalCR - Se.UnMatchedInternalDR
                              + Se.UnMatchedExternalCR -Se.UnMatchedExternalDR;

            textBoxTotal.Text = Total.ToString("#,##0.00");

            textBoxExternalAccBalance.Text = Se.ExternalAccBalance.ToString("#,##0.00");
            textBoxExternalAccTxns.Text = Se.ExternalAccTxns.ToString();

            decimal Difference = Total + Se.ExternalAccBalance;

            textBoxOpenAdjustmentsTxns.Text = Se.MatchedAdjustmentsOpenTxns.ToString();
            textBoxOpenAdjustmentsAmtNegative.Text = Se.MatchedAdjustmentsOpenNegative.ToString("#,##0.00");
            textBoxtextBoxOpenAdjustmentsAmtPositive.Text = Se.MatchedAdjustmentsOpenPositive.ToString("#,##0.00");

            textBoxDifference.Text = Difference.ToString("#,##0.00");

            int Exceptions = Se.UnMatchedExternalCRTxns + Se.UnMatchedExternalDRTxns; 

            if (Exceptions != 0)
            {
                pictureBoxBalStatus.BackgroundImage = appResImg.RED_LIGHT_Repl;

                Color Red = Color.Red;
            }

            if (Exceptions == 0)
            {
                pictureBoxBalStatus.BackgroundImage = appResImg.GREEN_LIGHT_Repl;
            }

            if (Se.ToBeConfirmedTxns > 0 )
            {
                labelToBeConfirmed.Show();
                textBoxToBeConfirmedEntries.Show(); 
                textBoxToBeConfirmedEntries.Text = Se.ToBeConfirmedTxns.ToString();
            }
            else
            {
                labelToBeConfirmed.Hide();
                textBoxToBeConfirmedEntries.Hide();
            }

            textBoxOpenAdjustmentsTxns.Text = Se.MatchedAdjustmentsOpenTxns.ToString("#,##0.00");

            if (Se.MatchedAdjustmentsOpenTxns >0)
            {
                textBoxOpenAdjustmentsAmtNegative.Text = Se.MatchedAdjustmentsOpenNegative.ToString("#,##0.00");
                textBoxtextBoxOpenAdjustmentsAmtPositive.Text = Se.MatchedAdjustmentsOpenPositive.ToString("#,##0.00");
            }
            else
            {
                labelDr.Hide();
                labelCr.Hide(); 
                textBoxOpenAdjustmentsAmtNegative.Hide() ;
                textBoxtextBoxOpenAdjustmentsAmtPositive.Hide() ; 
            }
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
