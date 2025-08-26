using System;
using System.Windows.Forms;
using RRDM4ATMs;
namespace RRDM4ATMsWin
{
    public partial class Form14bITMX : Form
    {
        // Meta exception creation 

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMMatchingTxns_MasterPoolITMX Mp = new RRDMMatchingTxns_MasterPoolITMX(); 

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        public int MetaNumber; 

        string WSignedId;
        string WOperator;
        string WRMCategoryId;
        int WRMCycleId;
        int WMaskRecordId; 
        string WAtmNo;
        int WSesNo;// ATM or RM 
        int WTransNo; 
        decimal WAmount;
        string WCurDes;
        int WMetaExceptionId;

        public Form14bITMX(string InSignedId, string InOperator, string InRmCategoryId, int InRMCycleId, int InMaskRecordId, string InAtmNo, int InSesNo, int InTransNo, decimal InAmount, string InCurrNm, int InMetaExceptionId)
        {
            WSignedId = InSignedId;
            WOperator = InOperator;
            WRMCategoryId = InRmCategoryId;
            WRMCycleId = InRMCycleId;
            WMaskRecordId = InMaskRecordId; 
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;
            WTransNo = InTransNo; 
            WAmount = InAmount;
            WCurDes = InCurrNm;
            WMetaExceptionId = InMetaExceptionId; 

            InitializeComponent();
            // Call Procedures 

            Er.ReadErrorsIDRecord(WMetaExceptionId, WOperator);

            textBox15.Text = WCurDes;
            textBox11.Text = WAmount.ToString("#,##0.00");
            textBox4.Text = Er.ErrDesc;
            textBox1.Text = WMetaExceptionId.ToString();

            if (WMetaExceptionId == 175) // Missing At Host 
            {
                radioButtonDRCustomer.Checked = true; 
                radioButton11.Checked = true; 
            }

            if (WMetaExceptionId == 185) // Double At Host 
            {
                radioButton3.Checked = true;
                radioButton10.Checked = true; 
            }

            if (WMetaExceptionId == 165) // Wrong Updating At Host
            {
                radioButton3.Checked = true;
                radioButton10.Checked = true;
            }

            if (WMetaExceptionId == 145) // Manual 
            {
                panel3.Enabled = true;
                panel4.Enabled = true;
            }
            else
            {
                panel3.Enabled = false;
                panel4.Enabled = false;
            }

            textBoxMsgBoard.Text = "Review MetaException to be created. Create it."; 
        
        }
// Create  Error record
        private void button1_Click(object sender, EventArgs e)
        {

            // Find needed information

            Mp.ReadMatchingTxnsMasterPoolSpecificRecordByMaskRecordId(WOperator, Mp.MaskRecordId); 
       
            Ac.ReadAtm(WAtmNo);
            if (Ac.RecordFound == true)
            {
                Er.BankId = Ac.BankId;
                Er.BranchId = Ac.Branch;
                Er.CitId = Ac.CitId;
            }
            else
            {
                // Read Category 
                Er.BankId = WOperator;
                Er.BranchId = "EWB HeadQuarters";
                Er.CitId = "1000";
            }

            // INITIALISED WHAT IS NEEDED 

            Er.CategoryId = WRMCategoryId ; 
            Er.RMCycle = WRMCycleId ;
            Er.UniqueRecordId = WMaskRecordId;
       
            Er.AtmNo = WAtmNo;
            Er.SesNo = WSesNo;
            Er.DateInserted = DateTime.Now;
            Er.DateTime = DateTime.Now;

            Er.ByWhom = WSignedId;

            //  Pa.CurrCd = WCurrCd;
            Er.CurDes = WCurDes;
            Er.ErrAmount = WAmount;

            Er.TraceNo = 0;
            Er.CardNo = "";
            Er.CustAccNo = Mp.AccountRequestor;
            Er.TransType = Mp.DebitTxnCode;
            Er.TransDescr = Mp.Particulars; 

            Er.DatePrinted = NullPastDate;

            Er.OpenErr = true;

            Er.UnderAction = true; 

            Er.Operator = WOperator;

            MetaNumber = Er.InsertError(); // INSERT ERROR 

            MessageBox.Show("Meta Excepttion with no = " + Er.ErrNo + " is created. ");

            //textBoxMsgBoard.Text = "Meta Exception is created. Close form.";

            this.Close();
        }
// FINISH 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
