using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;
namespace RRDM4ATMsWin
{
    public partial class Form14b_Input_Key : Form
    {
     
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string PeriodDesc; 
        string WPeriodKey ;

        private bool isCorrectInputKey;

        string WSignedId;
        string WOperator;

        DateTime WCutoffDate;

        int WMode;

        public bool IsCorrectInputKey { get => isCorrectInputKey; set => isCorrectInputKey = value; }

        public Form14b_Input_Key(string InSignedId, string InOperator,
                               DateTime InCutoffDate, int InMode)         
        {
            WSignedId = InSignedId;
            WOperator = InOperator;

            WCutoffDate = InCutoffDate;
            WMode = InMode; // if 1 = Check Input Key
                            
            InitializeComponent();

            
        }
        // 
        private void Form14b_All_Actions_Load(object sender, EventArgs e)
        {
            // Find the value of The WPeriodKey
            // Period 
            WPeriodKey = "1283838";
            labelCutOff.Text = "CUT_OFF_DATE_:" + WCutoffDate.ToString();

            DateTime July20 = new DateTime(2023, 07, 20);
            DateTime August31 = new DateTime(2023, 08, 31);

            if (WCutoffDate>= July20 & WCutoffDate<= August31)
            {
                labelPeriod.Text = "PERIOD FOR THE KEY IS FROM.."+ July20.ToShortDateString()+"..TO.."+ August31.ToShortDateString();
                WPeriodKey = "3582"; 
            }
            DateTime Sept1 = new DateTime(2023, 09, 01);
            DateTime Oct31 = new DateTime(2023, 10, 31);

            if (WCutoffDate >= Sept1 & WCutoffDate <= Oct31)
            {
                labelPeriod.Text = "PERIOD FOR THE KEY IS FROM.." + Sept1.ToShortDateString() + "..TO.." + Oct31.ToShortDateString();
                WPeriodKey = "3290";
            }
            DateTime Nov1 = new DateTime(2023, 11, 01);
            DateTime Dec31 = new DateTime(2023, 12, 31);

            if (WCutoffDate >= Nov1 & WCutoffDate <= Dec31)
            {
                labelPeriod.Text = "PERIOD FOR THE KEY IS FROM.." + Nov1.ToShortDateString() + "..TO.." + Dec31.ToShortDateString();
                WPeriodKey = "1482";
            }

            // YEAR 2024
            DateTime Jan_24_1 = new DateTime(2024, 01, 01);
            DateTime March_24_31 = new DateTime(2024, 03, 31);

            if (WCutoffDate >= Jan_24_1 & WCutoffDate <= March_24_31)
            {
                labelPeriod.Text = "PERIOD FOR THE KEY IS FROM.." + Jan_24_1.ToShortDateString() + "..TO.." + March_24_31.ToShortDateString();
                WPeriodKey = "2133";
            }

            DateTime Apr_24_1 = new DateTime(2024, 04, 01);
            DateTime July_24_31 = new DateTime(2024, 07, 31);

            if (WCutoffDate >= Apr_24_1 & WCutoffDate <= July_24_31)
            {
                labelPeriod.Text = "PERIOD FOR THE KEY IS FROM.." + Apr_24_1.ToShortDateString() + "..TO.." + July_24_31.ToShortDateString();
                WPeriodKey = "5548";
            }

            DateTime Aug_24_1 = new DateTime(2024, 08, 01);
            DateTime Dec_24_31 = new DateTime(2024, 12, 31);

            if (WCutoffDate >= Aug_24_1 & WCutoffDate <= Dec_24_31)
            {
                labelPeriod.Text = "PERIOD FOR THE KEY IS FROM.." + Aug_24_1.ToShortDateString() + "..TO.." + Dec_24_31.ToShortDateString();
                WPeriodKey = "5363";
            }

            // YEAR 2025
         
            DateTime Jan_25_1 = new DateTime(2025, 01, 01);
            DateTime March_25_31 = new DateTime(2025, 03, 31);

            if (WCutoffDate >= Jan_25_1 & WCutoffDate <= March_25_31)
            {
                labelPeriod.Text = "PERIOD FOR THE KEY IS FROM.." + Jan_25_1.ToShortDateString() + "..TO.." + March_25_31.ToShortDateString();
                WPeriodKey = "2533";
            }

            DateTime Apr_25_1 = new DateTime(2025, 04, 01);
            DateTime July_25_31 = new DateTime(2025, 07, 31);

            if (WCutoffDate >= Apr_25_1 & WCutoffDate <= July_25_31)
            {
                labelPeriod.Text = "PERIOD FOR THE KEY IS FROM.." + Apr_25_1.ToShortDateString() + "..TO.." + July_25_31.ToShortDateString();
                WPeriodKey = "4726";
            }

            DateTime Aug_25_1 = new DateTime(2025, 08, 01);
            DateTime Dec_25_31 = new DateTime(2025, 12, 31);

            if (WCutoffDate >= Aug_25_1 & WCutoffDate <= Dec_25_31)
            {
                labelPeriod.Text = "PERIOD FOR THE KEY IS FROM.." + Aug_25_1.ToShortDateString() + "..TO.." + Dec_25_31.ToShortDateString();
                WPeriodKey = "6237";
            }

        }
        // Confirm Vs Key 
        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            //string WCcy = TCcy.Trim();
           
            if (textBoxInputKey.Text.Trim() == WPeriodKey)
            {
                MessageBox.Show("Input Key Correct!"+Environment.NewLine
                    + "Continue Work"
                    );
                isCorrectInputKey = true; 
            }
            else
            {
                MessageBox.Show("Input Key NOT Correct!");
                isCorrectInputKey = false;
            }
        }
        // FINISH 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
