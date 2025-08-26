using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form14a : Form
    {
        // Error record fields

        RRDMErrorsClassWithActions Pa = new RRDMErrorsClassWithActions();

        RRDMGetUniqueNumber Gu = new RRDMGetUniqueNumber();

        public int MetaNumber;

        //  AtmsMainClass Am = new AtmsMainClass();

        RRDMAtmsClass Ac = new RRDMAtmsClass();
        DateTime NullPastDate = new DateTime(1900, 01, 01);
        string WSignedId; 
        decimal WAmountInDiff;
     
        string WCurDes;
        string WRMCategory;
        int WRMCycle;
        //int WMaskRecordId; 
        string WAtmNo;
        int WSesNo;
        string WOperator; 

        public Form14a(string InSignedId, string InRMCategory, int InRMCycle, int InMaskRecordId,string InAtmNo, int InSesNo, string InOperator, decimal InAmountInDiff, string InCurrNm)
        {
            WSignedId = InSignedId;
            WRMCategory = InRMCategory;
            WRMCycle = InRMCycle;
            //WMaskRecordId = InMaskRecordId = 0; 
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;
            WOperator = InOperator; 
            WAmountInDiff = InAmountInDiff; 
      
            WCurDes = InCurrNm;

            if (WAmountInDiff == 0)
            {
                MessageBox.Show("Amount is Zero. Please review what you want to do" );
            } 
                       
            InitializeComponent();
            // Call Procedures 
            MetaNumber = 0; 
            if (WAmountInDiff < 0) // ECHO PERISSEVMA AT HOST 
            {
                checkBox2.Checked = true; // CR Cash 
                checkBox3.Checked = true; // DR Suspense 
                WAmountInDiff = -WAmountInDiff; // Turn it to possitive 
                textBox4.Text = "DR Cash- Move to Suspense"; 
            }
            else
            {
                checkBox1.Checked = true; // DR Cash
                checkBox4.Checked = true; // CR Suspense 
                textBox4.Text = "CR Cash - move to Suspense";
            }

            radioButton1.Checked = true; 
            textBox1.Text = "598"; 
            textBox11.Text = WAmountInDiff.ToString("#,##0.00");
            //   textBox14.Text = WCurrCd.ToString();
            textBox15.Text = WCurDes;
            textBoxMsgBoard.Text = "Review Error to be created. Press Update to create it. Then press finish"; 

        }
        // Update Error Table . One by one is updated  
        private void button1_Click_1(object sender, EventArgs e)
        {
        //    if (checkBox1.Checked == true) DrAtmCash = true;
        //    if (checkBox2.Checked == true) CrAtmCash = true;
        //    if (checkBox3.Checked == true) DrAtmSusp = true;
        //    if (checkBox4.Checked == true) CrAtmSusp = true;
        //    if (radioButton1.Checked == true) MainOnly = true;
        //    if (radioButton2.Checked == true) MainOnly = false;

            

            Pa.ErrId = 598;
            Pa.Operator = WOperator; 
            Pa.ReadErrorsIDRecord(Pa.ErrId, WOperator); // READ TO GET THE CHARACTERISTICS

            Ac.ReadAtm (WAtmNo) ;
            if (Ac.RecordFound == true)
            {
                Pa.BankId = Ac.BankId;
                Pa.BranchId = Ac.Branch;
                Pa.CitId = Ac.CitId;
            }
            else
            {
                // Read Category 
                Pa.BankId = WOperator;
                Pa.BranchId = "EWB HeadQuarters";
                Pa.CitId = "1000";
            }

            // INITIALISED WHAT IS NEEDED 

            Pa.CategoryId = WRMCategory;
            Pa.RMCycle = WRMCycle;
            Pa.UniqueRecordId = Gu.GetNextValue();


            Pa.ErrDesc = textBox4.Text;
            Pa.AtmNo = WAtmNo;
            Pa.SesNo = WSesNo;
            Pa.DateInserted = DateTime.Now;
            Pa.DateTime = DateTime.Now;
            
            Pa.ByWhom = WSignedId;  

          //  Pa.CurrCd = WCurrCd;
            Pa.CurDes = WCurDes;
            Pa.ErrAmount = WAmountInDiff;

            if (checkBox1.Checked == true) Pa.DrAtmCash = true; 
            else Pa.DrAtmCash = false ;
            if (checkBox2.Checked == true) Pa.CrAtmCash = true;
            else Pa.CrAtmCash = false;
            if (checkBox3.Checked == true) Pa.DrAtmSusp = true;
            else Pa.DrAtmSusp = false;
            if (checkBox4.Checked == true) Pa.CrAtmSusp = true;
            else Pa.CrAtmSusp = false;
            if (radioButton1.Checked == true) Pa.MainOnly = true;
          
            if (radioButton2.Checked == true) Pa.MainOnly = false;

            Pa.DatePrinted = NullPastDate;

            Pa.UserComment = "Move difference to suspense for ATM :" + WAtmNo; 

            Pa.OpenErr = true;

            Pa.Operator = WOperator;

            MetaNumber = Pa.InsertError(); // INSERT ERROR

            MessageBox.Show("Exception with id = 598 is created." + Environment.NewLine
                            +"This moves difference to suspense account.");  

            textBoxMsgBoard.Text = "Exception is created. Close form and take action on Error";

            this.Close();

        }
// Finish 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }
       
    }
}
