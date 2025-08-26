using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using RRDM4ATMs;
//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form191 : Form
    {
        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        // NOTES 
        string Order;
        string WParameter4;
        string WSearchP4;

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
        string WSecLevel;
        string WOperator;
        //   bool WPrive;
        public Form191(string InSignedId, int InSignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;

            InitializeComponent();


            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();
            string ParId = "267";
            string OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            string TestingDate = Gp.OccuranceNm;
            if (TestingDate == "YES")
                labelToday.Text = new DateTime(2017, 03, 01).ToShortDateString();
            else labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            label18.Text = WOperator;

            // Banks available for the seed bank 
            comboBox2.DataSource = Cc.GetBanksIds(WOperator);
            comboBox2.DisplayMember = "DisplayValue";



            textBoxMsgBoard.Text = "For new change the parameter id to requested id and press Add. For maintenance use grid or comboBox. ";

            if (WSecLevel == "09")
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

            textBoxParId.Text = Gp.ParamId.ToString();
            comboBox2.Text = Gp.BankId;
            textBox4.Text = Gp.ParamNm;
            textBox5.Text = Gp.OccuranceId;
            textBox6.Text = Gp.OccuranceNm;
            textBox7.Text = Gp.Amount.ToString("#,##0.0000");
            textBox8.Text = Gp.RelatedParmId;
            textBox9.Text = Gp.RelatedOccuranceId;

            if (Gp.RelatedParmId != "")
            {
                textBox11.Text = Gp.GetParamOccurancesRelatedNm(WOperator, Gp.RelatedParmId, Gp.RelatedOccuranceId);

                // REFRESH INITIAL 
                Gp.ReadParametersByRefKey(WOperator, WRefKey);
                label22.Show();
                textBox11.Show();
            }
            else
            {
                label22.Hide();
                textBox11.Hide();
            }


            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Parameter Id:" + Gp.ParamId.ToString() + " Occurance Id: " + Gp.OccuranceId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes.Text = "0";

        }
        // UPDATE LINE  
        private void button5_Click(object sender, EventArgs e)
        {
            WRow = dataGridView1.SelectedRows[0].Index;

            string OldParamNm = Gp.ParamNm;

            // Validate Input and get numeric values 
            ValidateInput(); // Validate Input and get numeric values 
            if (ValidationPassed == false) return;

            Gp.BankId = comboBox2.Text;
            Gp.ParamId = textBoxParId.Text.Trim();
            Gp.ParamNm = textBox4.Text.Trim();
            Gp.OccuranceId = textBox5.Text.Trim();
            Gp.OccuranceNm = (textBox6.Text).Trim();
            Gp.Amount = EnteredAmount;
            Gp.RelatedParmId = textBox8.Text.Trim();
            Gp.RelatedOccuranceId = textBox9.Text.Trim();

            

            Gp.UpdateGasParamByKey(WOperator, WRefKey);

            if (OldParamNm.Trim() != Gp.ParamNm) // Name has been changed FOR ALL
            {
                Gp.UpdateGasParamByParamId(WOperator, Gp.ParamId);
            }

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


            if (textBoxNewPar.Text == "")
            {
                Gp.ParamId = textBoxParId.Text;

            }
            else
            {
                // This is new

                Gp.ParamId = textBoxNewPar.Text;
            }


            Gp.BankId = comboBox2.Text;


            Gp.GetParameterName(Gp.BankId, Gp.ParamId);

            if (Gp.RecordFound)
            {
                //if (WSecLevel != Gp.AccessLevel)
                //{
                //    MessageBox.Show("Your access level does not allow to add this parameter");
                //    return;
                //}
            }

            Gp.ParamNm = textBox4.Text.Trim();
            Gp.OccuranceId = textBox5.Text.Trim();

            Gp.OccuranceNm = (textBox6.Text).Trim();
            Gp.Amount = EnteredAmount;
            Gp.RelatedParmId = textBox8.Text.Trim();
            Gp.RelatedOccuranceId = textBox9.Text.Trim();
            Gp.Operator = WOperator;
            Gp.AccessLevel = 6;

            Gp.ReadParametersSpecificParmAndOccurance(WOperator, Gp.ParamId, Gp.OccuranceId, Gp.RelatedParmId, Gp.RelatedOccuranceId);
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
            if (textBoxNewPar.Text == "")
            {
                Gp.ParamId = textBoxParId.Text;

            }
            else
            {
                // This is new

                Gp.ParamId = textBoxNewPar.Text;
            }
            Gp.ParamNm = textBox4.Text.Trim();

            comboBox1.Text = Gp.ParamId + " " + Gp.ParamNm;

            ShowFilter(); // Our method 

            textBoxNewPar.Text = "";

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

                //if (WSecLevel != Gp.AccessLevel )
                //{
                //    MessageBox.Show("Your access level does not allow to delete this parameter");
                //    return;
                //}

                Gp.DeleteParameterEntryByRefKey(WRefKey);

                InternalChange = true;
                comboBox1.DataSource = Gp.GetParametersList2(WOperator); // Refresh to get new
                comboBox1.DisplayMember = "DisplayValue";

                //
                // Re -initialise
                //
                if (textBoxNewPar.Text == "")
                {
                    Gp.ParamId = textBoxParId.Text;

                }
                else
                {
                    // This is new

                    Gp.ParamId = textBoxNewPar.Text;
                }
                Gp.ParamNm = textBox4.Text;

                comboBox1.Text = Gp.ParamId + " " + Gp.ParamNm;

                ShowFilter(); // Our method 

                textBoxNewPar.Text = "";

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
                WComboIndex = 0;
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

            if (textBoxParId.Text.Trim() == "853" & textBox5.Text.Trim() == "5" & EnteredAmount < 70)
            {
                MessageBox.Show("The HST Entered Amount is less than 70 days. "+ Environment.NewLine
                    +"This is not allowed");
                ValidationPassed = false;
                return;
            }

            // Check existance of Ralated Parameter if inputed 
            if (textBox8.Text != "" || textBox9.Text != "")
            {
                textBox11.Text = Gp.GetParamOccurancesRelatedNm(WOperator, textBox8.Text, textBox9.Text);
                if (Gp.RecordFound == false)
                {
                    MessageBox.Show("Related Parameters + Occurance do not exist ");
                    ValidationPassed = false;
                    label22.Hide();
                    textBox11.Hide();
                    return;
                }
                else
                {
                    label22.Show();
                    textBox11.Show();
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
            // AND DO THIS PARAMETER TOO
            // CASH MANAGEMENT BEFORE REPLENISHEMENT
            ParId = "211";
            OccurId = "1";
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            if (checkBoxCash.Checked == true)
            {
                // Gp.OccuranceNm = "YES";
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
                MessageBox.Show("Parameters of BankA : " + BankA + " Not found.. check bank name");
            }
            else
            {
                MessageBox.Show("Parameters of BankA : " + BankA + " Have been copied to BankB : " + BankB);
            }

        }
        // NOTES button 
        private void buttonNotes_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = "Parameters";
            string WParameter4 = WParameter4 = "Parameter Id:" + Gp.ParamId.ToString() + " Occurance Id: " + Gp.OccuranceId;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            string WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES for final comment 
            Order = "Descending";
            WParameter4 = "Parameter Id:" + Gp.ParamId.ToString() + " Occurance Id: " + Gp.OccuranceId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes.Text = "0";

        }
        // FINISH 

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Excel
        private void buttonExcel_Click(object sender, EventArgs e)
        {
            // Folder, where a file is created.
            // Make sure to change this folder to your own folder 

            //string someText = "C# Corner is a community of software and data developers";
            //File.WriteAllText(@"C:\Temp\csc.txt", someText);
            //// Read a file  
            //string readText = File.ReadAllText(@"C:\Temp\csc.txt");
            //Console.WriteLine(readText);
            //string folder = @"C:\Temp\";
            //return; 
            //// Filename  
            //string fileName = "CSharpCornerAuthors.txt";
            //// Fullpath. You can direct hardcode it if you like.  
            //string fullPath = folder + fileName;
            //// An array of strings  
            //string[] authors = {"Mahesh Chand", "Allen O'Neill", "David McCarter",
            //             "Raj Kumar", "Dhananjay Kumar"};
            //// Write array of strings to a file using WriteAllLines.  
            //// If the file does not exists, it will create a new file.  
            //// This method automatically opens the file, writes to it, and closes file  
            //File.WriteAllLines(fullPath, authors);
            //// Read a file  
            //readText = File.ReadAllText(fullPath);
            //Console.WriteLine(readText);
            //return;
            RRDMGasParameters Gp = new RRDMGasParameters();
            Gp.ReadParametersAndFillDataTable(WOperator, WSignedId);

            RRDM_EXCEL_AND_Directories XL = new RRDM_EXCEL_AND_Directories();

            // string ExcelPath = "C:\\_KONTO\\CreateXL\\Files_" + DateTime.Now + ".xls";
            string ExcelDATE = DateTime.Now.Year.ToString()
                       + DateTime.Now.Month.ToString()
                       + DateTime.Now.Day.ToString()
                       + "_"
                       + DateTime.Now.Hour.ToString()
                        + DateTime.Now.Minute.ToString()
                        + DateTime.Now.Second.ToString()
                        ;

            string ExcelPath;
            string WorkingDir;

          

            ExcelPath = "C:\\RRDM\\W_New_Parameters_Loading\\ALL_Parameters.xls";
            WorkingDir = "C:\\RRDM\\W_New_Parameters_Loading\\";
            XL.ExportToExcel(Gp.DataTableAllParameters, WorkingDir, ExcelPath);

          

        }
        // Copy from Text
        private void buttonCopyFromText_Click(object sender, EventArgs e)
        {


            RRDMGasParameters Gp = new RRDMGasParameters();
            string FullPath = "C:\\RRDM\\W_New_Parameters_Loading\\ALL_Parameters.txt";

            // FIND FULL FILE PATH

            string WFullPath_01 = "";

            string[] specificFiles = Directory.GetFiles("C:\\RRDM\\W_New_Parameters_Loading");

            if (specificFiles == null || specificFiles.Length == 0)
            {
                MessageBox.Show("No such a text file in the directory...");

                return;
            }
            bool FileFound = false;
            foreach (string file in specificFiles)
            {
                //int FileLen = file.Length;
                long length = new System.IO.FileInfo(file).Length;
                if (length < 150)
                {
                    File.Delete(file);
                    continue;
                }
                if (file == FullPath)
                {
                    FileFound = true;
                }
            }
            if (FileFound)
            {
                // OK 
            }
            else
            {
                MessageBox.Show("Not the proper file in Directory");
                return;
            }
            Gp.MoveParametersFromUAT_To_Production(WOperator, FullPath);
        }
// Create Text File 
        private void buttonCreateTEXT_File_Click(object sender, EventArgs e)
        {

            string OutputFileNm = Gp.CopyParameters_To_TEXT_Delimiter_File(WOperator, WSignedId);                                                                                               
           
            if (Gp.RecordFound == true)
            {
                MessageBox.Show("Tab delimeter file created as.." + OutputFileNm);
                return;
            }
            else
            {
                MessageBox.Show("Tab delimeter was not created ");
                return;
            }
        }
// Parameter History
        private void linkLabelHst_Par_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form78d_FileRecords NForm78d_FileRecords;
            
            int WMode = 12; // ParamId
                           // 
                           // 
         
            string WTableId = "";
            string WString = WParamId; 

            NForm78d_FileRecords = new Form78d_FileRecords(WOperator, WSignedId, WTableId, WString, 0, "", WMode, false );
            NForm78d_FileRecords.Show();
        }
    }
}
