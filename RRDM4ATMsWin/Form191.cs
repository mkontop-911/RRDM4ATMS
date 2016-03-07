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
    public partial class Form191 : Form
    {
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMComboClass Cc = new RRDMComboClass();

        string WParamId;
        string WParamNm;
        decimal EnteredAmount;

        bool InternalChange; 

        int WComboIndex; 

        bool ValidationPassed; 

        int WRefKey;
        int WRow;
        string Tablefilter;

        string WSignedId;
        int WSignRecordNo;
        int WSecLevel;
        string WOperator;
     //   bool WPrive;
        public Form191(string InSignedId, int InSignRecordNo, int InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            label18.Text = WOperator;

            // Banks available for the seed bank 
            comboBox2.DataSource = Cc.GetBanksIds(WOperator);
            comboBox2.DisplayMember = "DisplayValue";

        

            textBoxMsgBoard.Text = "For new change the parameter id to requested id and press Add. For maintenance use grid or comboBox. "; 

            if (WSecLevel == 9)
            {
                panel5.Show();
                textBox10.Text = "ModelBak";
                label19.Show();
                comboBox3.Show(); 
            }
            else
            {
                panel5.Hide();
                label19.Hide();
                comboBox3.Hide(); 
            }
        }

        private void Form191_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'aTMSDataSet55.GasParameters' table. You can move, or remove it, as needed.
            
            // TODO: This line of code loads data into the 'aTMSDataSet15.GasParameters' table. You can move, or remove it, as needed.
           
            //Tablefilter = "Operator ='" + WOperator + "' AND OpenRecord = 1";
            //gasParametersBindingSource.Filter = Tablefilter;
          
            //this.gasParametersTableAdapter.Fill(this.aTMSDataSet55.GasParameters);

            //dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);
          

            // LOAD COMBO BOX
            comboBox1.DataSource = Gp.GetParametersList2(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

      //      int WComboIndex = comboBox1.SelectedIndex;

            comboBox1.SelectedIndex = WComboIndex; 

            // *******************************************************************
            // LOAD SPECIALS 
            // *******************************************************************

            string ParId;
            string OccurId;
            // CASH MANAGEMENT
            ParId = "202";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            // Cash In Management                                         
            if (Gp.OccuranceNm == "YES")
            {
                checkBoxCash.Checked = true;
            }
            else
            {
                checkBoxCash.Checked = false;
            }

            // Load Dispute ELECTRONIC AUTHORISATION Dispute 
            ParId = "260";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                                
            if (Gp.OccuranceNm == "YES")
            {
                checkBoxAuthorDisput.Checked = true;
            }
            else
            {
                checkBoxAuthorDisput.Checked = false;
            }

            // Load Replenishment ELECTRONIC AUTHORISATION Replenishment 
            ParId = "261";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                checkBoxAuthorRepl.Checked = true;
            }
            else
            {
                checkBoxAuthorRepl.Checked = false;
            }

            // Load Transaction ELECTRONIC AUTHORISATION Reconciliation 
            ParId = "262";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                checkBoxAuthorReconc.Checked = true;
            }
            else
            {
                checkBoxAuthorReconc.Checked = false;
            }

            // Load Google Licence. 
            ParId = "263";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                checkBoxGoogleLicence.Checked = true;
            }
            else
            {
                checkBoxGoogleLicence.Checked = false;
            }
            
            // Load Active Directory Mode 
            ParId = "264";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (Gp.OccuranceNm == "YES")
            {
                checkBoxActiveDirectory.Checked = true;

            }
            else
            {
                checkBoxActiveDirectory.Checked = false;
            }

           

        }
        // ON ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //  WRow = e.RowIndex;
            WRefKey = (int)rowSelected.Cells[0].Value;

            Gp.ReadParametersByRefKey(WOperator, WRefKey);

            WParamId = Gp.ParamId;
            WParamNm = Gp.ParamNm;

            textBox3.Text = Gp.ParamId.ToString();
            comboBox2.Text = Gp.BankId; 
            textBox4.Text = Gp.ParamNm;
            textBox5.Text = Gp.OccuranceId;
            textBox6.Text = Gp.OccuranceNm;
            textBox7.Text = Gp.Amount.ToString("#,##0.00");
            textBox8.Text = Gp.RelatedParmId;
            textBox9.Text = Gp.RelatedOccuranceId;

        }
        // UPDATE LINE  
        private void button5_Click(object sender, EventArgs e)
        {
            WRow = dataGridView1.SelectedRows[0].Index;

            if (WSecLevel != Gp.AccessLevel)
            {
                MessageBox.Show("Your access level does not allow to update this parameter");
                return;
            }

            // Validate Input and get numeric values 
            ValidateInput(); // Validate Input and get numeric values 
            if (ValidationPassed == false) return;

            Gp.BankId = comboBox2.Text;
            Gp.ParamId = textBox3.Text;
            Gp.ParamNm = textBox4.Text;
            Gp.OccuranceId = textBox5.Text;
            Gp.OccuranceNm = textBox6.Text;
            Gp.Amount = EnteredAmount;
            Gp.RelatedParmId = textBox8.Text;
            Gp.RelatedOccuranceId = textBox9.Text;

            Gp.UpdateGasParamByKey(WOperator, WRefKey);

            ShowFilter();

            dataGridView1.Rows[WRow].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow));
        }
        // ADD NEW 
        private void button2_Click(object sender, EventArgs e)
        {
          
            // Validate Input and get numeric values
 
            ValidateInput(); // Validate Input and get numeric values 
            if (ValidationPassed == false) return;


            if (textBox1.Text == "")
            {
                Gp.ParamId = textBox3.Text;
                
            }
            else
            {
                // This is new

                Gp.ParamId = textBox1.Text;
            }
            

            Gp.BankId = comboBox2.Text;


            Gp.GetParameterName(Gp.BankId, Gp.ParamId);

            if (Gp.RecordFound)
            {
                if (WSecLevel != Gp.AccessLevel)
                {
                    MessageBox.Show("Your access level does not allow to add this parameter");
                    return;
                }
            }

            Gp.ParamNm = textBox4.Text;
            Gp.OccuranceId = textBox5.Text;
          
            Gp.OccuranceNm = textBox6.Text;
            Gp.Amount = EnteredAmount;
            Gp.RelatedParmId = textBox8.Text;
            Gp.RelatedOccuranceId = textBox9.Text; 
            Gp.Operator = WOperator;
            Gp.AccessLevel = WSecLevel; 

            Gp.ReadParametersSpecificParmAndOccurance(WOperator, Gp.ParamId, Gp.OccuranceId); 
            if (Gp.RecordFound == true & Gp.OpenRecord == true)
            {
                MessageBox.Show("Parameter and Occurance Already in Data Base");
                return;
            }
            else Gp.InsertGasParam(WOperator, Gp.ParamId, Gp.OccuranceId);

            InternalChange = true; 
            comboBox1.DataSource = Gp.GetParametersList2(WOperator); // Refresh to get new
            comboBox1.DisplayMember = "DisplayValue";
            //
            // Re -initialise
            //
            if (textBox1.Text == "")
            {
                Gp.ParamId = textBox3.Text;

            }
            else
            {
                // This is new

                Gp.ParamId = textBox1.Text;
            }
            Gp.ParamNm = textBox4.Text;

            comboBox1.Text = Gp.ParamId + " " + Gp.ParamNm;

            ShowFilter(); // Our method 

            textBox1.Text = ""; 

            // Initialise internal change 
            InternalChange = false; 

        }

        // DELETE 
        private void button4_Click(object sender, EventArgs e)
        {
           
            if (MessageBox.Show("Warning: Do you want to delete this parameter occurance?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                             == DialogResult.Yes)
            {
                Gp.ReadParametersByRefKey(WOperator, WRefKey);

                if (WSecLevel != Gp.AccessLevel )
                {
                    MessageBox.Show("Your access level does not allow to delete this parameter");
                    return;
                }

                Gp.OpenRecord = false; 

                Gp.UpdateGasParamByKey(WOperator, WRefKey);

                InternalChange = true;
                comboBox1.DataSource = Gp.GetParametersList2(WOperator); // Refresh to get new
                comboBox1.DisplayMember = "DisplayValue";

                //
                // Re -initialise
                //
                if (textBox1.Text == "")
                {
                    Gp.ParamId = textBox3.Text;

                }
                else
                {
                    // This is new

                    Gp.ParamId = textBox1.Text;
                }
                Gp.ParamNm = textBox4.Text;

                comboBox1.Text = Gp.ParamId + " " + Gp.ParamNm;

                ShowFilter(); // Our method 

                textBox1.Text = "";

                // Initialise internal change 
                InternalChange = false; 
            }
            else
            {
            }
            
        }

        // Show for specific parameter 
        private void ShowFilter()
        {
            string Cob1 = comboBox1.Text;
            string TempParam = Cob1.Substring(0, 3);
            //   int TempParam = int.Parse(comboBox1.Text.Substring(0, 3));

            if (TempParam == "100")
            {
                Tablefilter = "Operator ='" + WOperator + "' AND OpenRecord = 1";
            }
            else Tablefilter = "Operator ='" + WOperator + "' AND ParamId ='" + TempParam + "' AND OpenRecord = 1";
         //   else Tablefilter = "Operator ='" + WOperator + "' AND ParamId ='" + WParamId + "'";

            gasParametersBindingSource.Filter = Tablefilter;

      

            this.gasParametersTableAdapter.Fill(this.aTMSDataSet55.GasParameters);

            if (TempParam == "100")
            {
                dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);
            }
            else
            {
                dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
                //dataGridView1.Sort(dataGridView1.Columns[5], ListSortDirection.Ascending);
            }

            if (dataGridView1.Rows.Count == 0)
            {
                // LOAD COMBO BOX
                //InternalChange = true; 
                //comboBox1.DataSource = Gp.GetParametersList2(WOperator);
                //comboBox1.DisplayMember = "DisplayValue";

                //comboBox1.SelectedIndex = 0 ;
                //InternalChange = false; 

                // Save combo Index 
                WComboIndex = 0 ;
                // Reload 
                //
                Form191_Load(this, new EventArgs());
            }

        }

        // Input Validation 
        private void ValidateInput()
        {
            ValidationPassed = true; 

            if (decimal.TryParse(textBox7.Text, out EnteredAmount))
            {
            }
            else
            {
                MessageBox.Show(textBox7.Text, "Please enter a valid Amount!");
                ValidationPassed = false; 
                return;
            }

            // Check existance of Ralated Parameter if inputed 
            if (textBox8.Text != "" || textBox9.Text != "")
            {
                Gp.ReadParametersSpecificParmAndOccurance(comboBox2.Text, textBox8.Text, textBox9.Text);
                if (Gp.RecordFound == false)
                {
                    MessageBox.Show("Related Parameters + Occurance do not exist ");
                    ValidationPassed = false; 
                    return;
                } 
            }
            
            
        }

        // UPDATE Specials 
        private void buttonUpdateSpecials_Click(object sender, EventArgs e)
        {
            // Save combo Index 
            WComboIndex = comboBox1.SelectedIndex;

            string ParId;
            string OccurId;

            // Read and Update 

            // CASH MANAGEMENT
            ParId = "202";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (checkBoxCash.Checked == true)
            {
                Gp.OccuranceNm = "YES";
            }
            else
            {
                Gp.OccuranceNm = "NO";
            }

            Gp.UpdateGasParamByKey(Gp.Operator, Gp.RefKey);

            // Dispute ELECTRONIC AUTHORISATION - Dispute 
            ParId = "260";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (checkBoxAuthorDisput.Checked == true)
            {
                Gp.OccuranceNm = "YES";
            }
            else
            {
                Gp.OccuranceNm = "NO";
            }

            Gp.UpdateGasParamByKey(Gp.Operator, Gp.RefKey);

            // Replenishment ELECTRONIC AUTHORISATION - Repl 
            ParId = "261";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (checkBoxAuthorRepl.Checked == true)
            {
                Gp.OccuranceNm = "YES";
            }
            else
            {
                Gp.OccuranceNm = "NO";
            }

            Gp.UpdateGasParamByKey(Gp.Operator, Gp.RefKey);

            // Transactions ELECTRONIC AUTHORISATION - Reconc
            ParId = "262";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (checkBoxAuthorReconc.Checked == true)
            {
                Gp.OccuranceNm = "YES";
            }
            else
            {
                Gp.OccuranceNm = "NO";
            }

            Gp.UpdateGasParamByKey(Gp.Operator, Gp.RefKey); 

            // Update Goodle Licence 
            ParId = "263";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (checkBoxGoogleLicence.Checked == true)
            {
                Gp.OccuranceNm = "YES";
            }
            else
            {
                Gp.OccuranceNm = "NO";
            }

            Gp.UpdateGasParamByKey(Gp.Operator, Gp.RefKey);

            // Update Active Directory Mode
            ParId = "264";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (checkBoxActiveDirectory.Checked == true)
            {
                Gp.OccuranceNm = "YES";
            }
            else
            {
                Gp.OccuranceNm = "NO";
              
            }

            Gp.UpdateGasParamByKey(Gp.Operator, Gp.RefKey);
            

            // Reload 
            //
            Form191_Load(this, new EventArgs());

            MessageBox.Show(" Specials parameters Updated! ");

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Do not delete the below needed for internal change 
            //string Cob1 = comboBox1.Text;
            //string Temp = Cob1.Substring(0, 3);
            //if (Temp != "")
            //{
            //    Gp.GetParameterName(WOperator, Temp);
            //}
            //else //  
            //{

            //}
            if (InternalChange == false)
            {
                ShowFilter();
            }
            

        }
        // ACTION ALLOW ONLY TO GRAND MASTER 
        private void button1_Click(object sender, EventArgs e)
        {
            string BankA = textBox10.Text;
            string BankB = textBox2.Text;
            bool Prive = checkBox1.Checked;
            Gp.CopyParameters(BankA, BankB);

            if (Gp.RecordFound == false)
            {
                MessageBox.Show("Parameters of BankA : " + BankA + " Not found.. check bank name" );  
            }
            else
            {
                MessageBox.Show("Parameters of BankA : " + BankA + " Have been copied to BankB : " + BankB) ;  
            }         

        }

        // FINISH 
      
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

     
    }
}
