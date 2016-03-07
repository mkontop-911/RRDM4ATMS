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
using System.Data.SqlClient;
using System.Configuration;
//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form193 : Form
    {
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMCounterClass Cc = new RRDMCounterClass();
        RRDMDisputesTableClass Dtc = new RRDMDisputesTableClass();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

   //     string WUserOperator;

        int QualityRange1;
        int QualityRange2;

   //     int WRow;
    //    string Tablefilter;


        string WSignedId;
        int WSignRecordNo;
        int WSecLevel;
        string WOperator;
     
        public Form193(string InSignedId, int InSignRecordNo, int InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
      

            InitializeComponent();
            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            textBox1.Text = WOperator;

            textBoxMsgBoard.Text = "View Status. Red = Problem, Green = No problem, Yellow = Warning"; 
        }
        // Load data 
        private void Form193_Load(object sender, EventArgs e)
        {
            //TESTING
            DateTime WReplDate = DateTime.Today;
            WReplDate = new DateTime(2014, 04, 18); // IF NOT TESTING THIS IS TODAY
            // Read No Reconcile Days limit 
            Gp.ReadParametersSpecificId(WOperator,"605", "2",  "", "");
            int DaysLimit = (int)Gp.Amount;

            Cc.ReadAtmsMainTotals(WReplDate, WOperator, "1000", DaysLimit);

            Dtc.ReadDisputeTotals("1000", WOperator);

            textBox2.Text = (Cc.TotNotReconc1 + Cc.TotNotReconc2).ToString();
            textBox3.Text = Cc.TotNotReconc3.ToString();
            textBox4.Text = Dtc.TotalOpenDisp.ToString();

            textBox5.Text = Cc.TotErrorsAtm.ToString();
            textBox6.Text = Cc.TotErrorsHost.ToString();


            // UNRECONCILED Number  

            int Temp = Cc.TotNotReconc1 + Cc.TotNotReconc2; 

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox13.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox13.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"603", "1",  "", "");
            comboBox13.Text = ((int)Gp.Amount).ToString();
            QualityRange1 = (int)Gp.Amount;


            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox12.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox12.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"604", "1",  "", "");
            comboBox12.Text = ((int)Gp.Amount).ToString();
            QualityRange2 = (int)Gp.Amount;

            if (Temp < QualityRange1)
            {
                // Green
                pictureBox2.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl1;
            }

            if (Temp > QualityRange2)
            {
                // Red 
                pictureBox2.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;
            }
            if (Temp >= QualityRange1 & Temp <= QualityRange2)
            {
                // Yellow 
                pictureBox2.BackgroundImage = Properties.Resources.YELLOW_Repl;
            }

            // UNRECONCILED DAYS

            Temp = Cc.TotNotReconc3;

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox20.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox20.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"603", "2",  "", "");
            comboBox20.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox19.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox19.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"604", "2",  "", "");
            comboBox19.Text = ((int)Gp.Amount).ToString();

            if (Temp < QualityRange1)
            {
                // Green
                pictureBox3.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl1;
            }

            if (Temp > QualityRange2)
            {
                // Red 
                pictureBox3.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;
            }
            if (Temp >= QualityRange1 & Temp <= QualityRange2)
            {
                // Yellow 
                pictureBox3.BackgroundImage = Properties.Resources.YELLOW_Repl;
            }


            // DISPUTES NUMBER 

            Temp = Dtc.TotalOpenDisp;

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox11.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox11.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"603", "3",  "", "");
            comboBox11.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox10.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox10.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"604", "3",  "", "");
            comboBox10.Text = ((int)Gp.Amount).ToString();

            if (Temp < QualityRange1)
            {
                // Green
                pictureBox4.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl1;
            }

            if (Temp > QualityRange2)
            {
                // Red 
                pictureBox4.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;
            }
            if (Temp >= QualityRange1 & Temp <= QualityRange2)
            {
                // Yellow 
                pictureBox4.BackgroundImage = Properties.Resources.YELLOW_Repl;
            }

            // ATMS ERRORS NUMBER  

            Temp = Cc.TotErrorsAtm; 

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox9.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox9.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"603", "4",  "", "");
            comboBox9.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox8.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox8.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"604", "4",  "", "");
            comboBox8.Text = ((int)Gp.Amount).ToString();

            if (Temp < QualityRange1)
            {
                // Green
                pictureBox5.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl1;
            }

            if (Temp > QualityRange2)
            {
                // Red 
                pictureBox5.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;
            }
            if (Temp >= QualityRange1 & Temp <= QualityRange2)
            {
                // Yellow 
                pictureBox5.BackgroundImage = Properties.Resources.YELLOW_Repl;
            }

            // HOST ERRORS NUMBER  

            Temp = Cc.TotErrorsHost; 

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox15.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox15.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"603", "5",  "", "");
            comboBox15.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox14.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox14.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"604", "5",  "", "");
            comboBox14.Text = ((int)Gp.Amount).ToString();

            if (Temp < QualityRange1)
            {
                // Green
                pictureBox6.BackgroundImage = Properties.Resources.GREEN_LIGHT_Repl1;
            }

            if (Temp > QualityRange2)
            {
                // Red 
                pictureBox6.BackgroundImage = Properties.Resources.RED_LIGHT_Repl;
            }
            if (Temp >= QualityRange1 & Temp <= QualityRange2)
            {
                // Yellow 
                pictureBox6.BackgroundImage = Properties.Resources.YELLOW_Repl;
            }

            // REPL DURATION MINUTES  

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox18.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox18.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"603", "6",  "", "");
            comboBox18.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox1.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"604", "6",  "", "");
            comboBox1.Text = ((int)Gp.Amount).ToString();

            // REPL Input Money difference from recommended   

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox23.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox23.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"603", "7",  "", "");
            comboBox23.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox22.DataSource = Gp.GetParamOccurancesId(WOperator);
            comboBox22.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator,"604", "7",  "", "");
            comboBox22.Text = ((int)Gp.Amount).ToString();

        }

      

    }
}
