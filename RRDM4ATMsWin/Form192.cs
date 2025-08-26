using System;
using System.Windows.Forms;
using RRDM4ATMs;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form192 : Form
    {
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMUsersRecords Us = new RRDMUsersRecords();

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator;
   
        public Form192(string InSignedId, int InSignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();

            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                 labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            label17.Text = WOperator;

            textBoxMsgBoard.Text = "Change and update Quality Parameters"; 
        }
        // FROM LOAD 
        private void Form192_Load(object sender, EventArgs e)
        {
            // TYPE A
            // Matched Days 

            // RED 
            Gp.ParamId = "901"; // Number Values    
            comboBox2.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox2.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "601", "1", "", "");
            comboBox2.Text = ((int)Gp.Amount).ToString();

            // GREEN
            Gp.ParamId = "901"; // Number Values    
            comboBox3.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox3.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "602", "1",  "", "");
            comboBox3.Text = ((int)Gp.Amount).ToString();

            // EXCEPTION FIXED DAYS 

            // RED 
            Gp.ParamId = "901"; // Number Values    
            comboBox5.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox5.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "601", "2",  "", "");
            comboBox5.Text = ((int)Gp.Amount).ToString();

            // GREEN
            Gp.ParamId = "901"; // Number Values    
            comboBox4.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox4.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "602", "2",  "", "");
            comboBox4.Text = ((int)Gp.Amount).ToString();

            // UPDATE HOLIDAYS

            // RED 
            Gp.ParamId = "901"; // Number Values    
            comboBox7.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox7.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "601", "3",  "", "");
            comboBox7.Text = ((int)Gp.Amount).ToString();

            // GREEN
            Gp.ParamId = "901"; // Number Values    
            comboBox6.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox6.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "602", "3",  "", "");
            comboBox6.Text = ((int)Gp.Amount).ToString();

            // DISPUTE RESOLUTION DAYS 

            // RED 
            Gp.ParamId = "901"; // Number Values    
            comboBox17.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox17.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "601", "4",  "", "");
            comboBox17.Text = ((int)Gp.Amount).ToString();

            // GREEN
            Gp.ParamId = "901"; // Number Values    
            comboBox16.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox16.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "602", "4",  "", "");
            comboBox16.Text = ((int)Gp.Amount).ToString();

            /* -----------TYPE B */

            // UNRECONCILED Number

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox13.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox13.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "603", "1",  "", "");
            comboBox13.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox12.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox12.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "604", "1",  "", "");
            comboBox12.Text = ((int)Gp.Amount).ToString();

            // UNRECONCILED DAYS

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox20.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox20.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "603", "2",  "", "");
            comboBox20.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox19.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox19.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "604", "2",  "", "");
            comboBox19.Text = ((int)Gp.Amount).ToString();

            // DISPUTES NUMBER 

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox11.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox11.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "603", "3",  "", "");
            comboBox11.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox10.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox10.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "604", "3",  "", "");
            comboBox10.Text = ((int)Gp.Amount).ToString();

            // ATMS ERRORS NUMBER  

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox9.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox9.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "603", "4",  "", "");
            comboBox9.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox8.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox8.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "604", "4",  "", "");
            comboBox8.Text = ((int)Gp.Amount).ToString();

            // HOST ERRORS NUMBER  

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox15.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox15.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "603", "5",  "", "");
            comboBox15.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox14.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox14.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "604", "5",  "", "");
            comboBox14.Text = ((int)Gp.Amount).ToString();

            // REPL DURATION MINUTES  

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox18.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox18.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "603", "6",  "", "");
            comboBox18.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox1.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "604", "6",  "", "");
            comboBox1.Text = ((int)Gp.Amount).ToString();

            // REPL Input Money difference from recommended   

            // Green 

            Gp.ParamId = "901"; // Number Values    
            comboBox23.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox23.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "603", "7",  "", "");
            comboBox23.Text = ((int)Gp.Amount).ToString();

            // Red 

            Gp.ParamId = "901"; // Number Values    
            comboBox22.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox22.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "604", "7",  "", "");
            comboBox22.Text = ((int)Gp.Amount).ToString();

            // TYPE C 
            // 
            //Dispute Target date 

            Gp.ParamId = "901"; // Number Values    
            comboBox21.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox21.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "605", "1",  "", "");
            comboBox21.Text = ((int)Gp.Amount).ToString();

            //Based days for unreconcilied 

            Gp.ParamId = "901"; // Number Values    
            comboBox24.DataSource = Gp.GetArrayParamOccurancesIds(WOperator);
            comboBox24.DisplayMember = "DisplayValue";

            Gp.ReadParametersSpecificId(WOperator, "605", "2",  "", "");
            comboBox24.Text = ((int)Gp.Amount).ToString();
        }
        // USE NEXT BUTTON TO UPDATE 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            ValidateInput();

            // UPDATE TYPE A first column
            Gp.ReadParametersSpecificId(WOperator, "601", "1",  "", "");
            Gp.Amount = (decimal.Parse(comboBox2.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "601", "2",  "", "");
            Gp.Amount = (decimal.Parse(comboBox5.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "601", "3",  "", "");
            Gp.Amount = (decimal.Parse(comboBox7.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "601", "4",  "", "");
            Gp.Amount = (decimal.Parse(comboBox17.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            // UPDATE TYPE A second column
            Gp.ReadParametersSpecificId(WOperator, "602", "1",  "", "");
            Gp.Amount = (decimal.Parse(comboBox3.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "602", "2",  "", "");
            Gp.Amount = (decimal.Parse(comboBox4.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "602", "3",  "", "");
            Gp.Amount = (decimal.Parse(comboBox6.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "602", "4",  "", "");
            Gp.Amount = (decimal.Parse(comboBox16.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            // UPDATE TYPE B first column
            Gp.ReadParametersSpecificId(WOperator, "603", "1",  "", "");
            Gp.Amount = (decimal.Parse(comboBox13.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "603", "2",  "", "");
            Gp.Amount = (decimal.Parse(comboBox20.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "603", "3",  "", "");
            Gp.Amount = (decimal.Parse(comboBox11.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "603", "4",  "", "");
            Gp.Amount = (decimal.Parse(comboBox9.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "603", "5",  "", "");
            Gp.Amount = (decimal.Parse(comboBox15.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "603", "6",  "", "");
            Gp.Amount = (decimal.Parse(comboBox18.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "603", "7",  "", "");
            Gp.Amount = (decimal.Parse(comboBox23.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            // UPDATE TYPE B second column
            Gp.ReadParametersSpecificId(WOperator, "604", "1",  "", "");
            Gp.Amount = (decimal.Parse(comboBox12.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "604", "2",  "", "");
            Gp.Amount = (decimal.Parse(comboBox19.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "604", "3",  "", "");
            Gp.Amount = (decimal.Parse(comboBox10.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "604", "4",  "", "");
            Gp.Amount = (decimal.Parse(comboBox8.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "604", "5",  "", "");
            Gp.Amount = (decimal.Parse(comboBox14.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "604", "6",  "", "");
            Gp.Amount = (decimal.Parse(comboBox1.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            Gp.ReadParametersSpecificId(WOperator, "604", "7",  "", "");
            Gp.Amount = (decimal.Parse(comboBox22.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            // OTHER 
            // Dispute target 
            Gp.ReadParametersSpecificId(WOperator, "605", "1",  "", "");
            Gp.Amount = (decimal.Parse(comboBox21.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);
            // Days in unreconcilied 
            Gp.ReadParametersSpecificId(WOperator, "605", "2",  "", "");
            Gp.Amount = (decimal.Parse(comboBox24.Text));
            Gp.UpdateGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            MessageBox.Show("All data has been updated");
        }

        // Validate Input
        //
        private void ValidateInput()
        {
            // TYPE A 
            if ((int.Parse(comboBox3.Text)) < (int.Parse(comboBox2.Text)))
            {
                MessageBox.Show("Please correct Matched Days");
                return;
            }

            if ((int.Parse(comboBox4.Text)) < (int.Parse(comboBox5.Text)))
            {
                MessageBox.Show("Please correct Exception Fixed Days");
                return;
            }

            if ((int.Parse(comboBox6.Text)) < (int.Parse(comboBox7.Text)))
            {
                MessageBox.Show("Please correct Holidays Days");
                return;
            }

            if ((int.Parse(comboBox16.Text)) < (int.Parse(comboBox17.Text)))
            {
                MessageBox.Show("Please correct Dispute Days");
                return;
            }

            // TYPE B

            if ((int.Parse(comboBox12.Text)) < (int.Parse(comboBox13.Text)))
            {
                MessageBox.Show("Please correct Unreconciled Number");
                return;
            }

            if ((int.Parse(comboBox19.Text)) < (int.Parse(comboBox20.Text)))
            {
                MessageBox.Show("Please correct Unreconciled Days");
                return;
            }

            if ((int.Parse(comboBox10.Text)) < (int.Parse(comboBox11.Text)))
            {
                MessageBox.Show("Please correct Dispute Number ");
                return;
            }

            if ((int.Parse(comboBox8.Text)) < (int.Parse(comboBox9.Text)))
            {
                MessageBox.Show("Please correct ATM Errors ");
                return;
            }

            if ((int.Parse(comboBox14.Text)) < (int.Parse(comboBox15.Text)))
            {
                MessageBox.Show("Please correct Host Errors ");
                return;
            }

            if ((int.Parse(comboBox1.Text)) < (int.Parse(comboBox18.Text)))
            {
                MessageBox.Show("Please correct Repl Duration ");
                return;
            }

            if ((int.Parse(comboBox22.Text)) < (int.Parse(comboBox23.Text)))
            {
                MessageBox.Show("Please correct Money In Percentage");
                return;
            }
        }
        // Finish 
        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
}
