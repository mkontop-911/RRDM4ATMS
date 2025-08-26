using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
using System.Data;

namespace RRDM4ATMsWin
{
    public partial class Form2Merchant : Form
    {
        // Define the data table 
        public DataTable WGridTable = new DataTable();

        string WHeader;
        string WCardNumber, WD11;
        string WAccNo, WD12;
        string WDateTime, WD13;
        string WAmount,WD14;
        string WCcy, WD15;
        string WAuthorisationCd, WD16; 
        string WMerchantId, WD17;
        string WMerchantName,WD18;
        string WTranDescription, WD19;
        string WOperator; 

        public Form2Merchant(string InOperator, string InHeader, string InCardNumber, string InAccNo, 
                                     string InDateTime, string InAmount, string InCcy, string InAuthorisationCd, 
                                                    string InMerchantId, string InMerchantName, string InTranDescription  )
        {
            WHeader = InHeader;
            WCardNumber = InCardNumber;
            WAccNo = InAccNo;
            WDateTime = InDateTime;
            WAmount = InAmount;
            WCcy = InCcy;
            WAuthorisationCd = InAuthorisationCd;
            WMerchantId = InMerchantId;
            WMerchantName = InMerchantName;
            WTranDescription = InTranDescription;
            WOperator = InOperator; 

            //WGridTable = InTable; 
            InitializeComponent();
        }
        // Load form 
        private void Form2MessageBox_Load(object sender, EventArgs e)
        {
          // MessageBox.Show(WMessage); 
            labelHeader.Text = WHeader;
         
            textBox1.Text = WD11 =WCardNumber;
            textBox2.Text = WD12 = WAccNo;
            textBox3.Text = WD13 = WDateTime;
            textBox4.Text = WD14 = WAmount;
            textBox5.Text = WD15 = WCcy;
            textBoxAuthCd.Text = WD16 = WAuthorisationCd; 
            textBox6.Text = WD17 = WMerchantId;
            textBox7.Text = WD18 = WMerchantName;
            textBox8.Text = WD19 = WTranDescription;
 
        }
      
// Print The details 
    
        private void buttonPrint_Click(object sender, EventArgs e)
        {

            Form56R5Merchant ReportMerchant = new Form56R5Merchant(WD11, WD12, WD13, WD14, WD15, WD16, WD17,
                       WD18, WD19,
                       WHeader,
                       WOperator);

            ReportMerchant.Show();
        }
// Finish 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
}
