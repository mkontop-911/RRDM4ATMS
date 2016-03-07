using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using RRDM4ATMs;

// Alecos
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form65 : Form
    {
        string exception; // SHOWS WHAT WENT WRONG 

        // Fields For creating ATMMain 
     //   string InsertedAtmNo;
        string AtmName;
        string BankId;
         
        string RespBranch;

        int AtmsStatsGroup;
        int AtmsReplGroup;
        int AtmsReconcGroup;

        string TypeOfRepl;
        string CashInType; 
        string HolidaysVersion; 
        string MatchDatesCateg;

        string CitId; 

        int OverEst;

        decimal InMinCash;
        decimal InMaxCash;

        decimal MinCash;
        decimal MaxCash;
        int ReplAlertDays;

        decimal InsurOne;
        decimal InsurTwo;
        decimal InsurThree;
        decimal InsurFour;

        int NumberOfCass;

        string BranchName;
        double Latitude;
        double Longitude; 

        bool ActiveAtm;
        bool OffSite;

      //  string TempCitId;

        bool ErrInResult;

        // ATM Physical 
        DateTime ManifactureDt;
        DateTime PurchaseDt;
        DateTime DueServiceDt;
        DateTime LastServiceDt;

        decimal PurchaseCost;
  
        int MaintenanceCd;
        decimal AnnualMaint;

        decimal CitOnCall;
        decimal CitAnnual;

        DateTime FutureDate = new DateTime(2050, 11, 21);

        bool RecordFound;

        string connectionString = ConfigurationManager.ConnectionStrings
            ["ATMSConnectionString"].ConnectionString;


        RRDMBanks Ba = new RRDMBanks();

        RRDMGroups Ga = new RRDMGroups();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMAtmsCostClass Ap = new RRDMAtmsCostClass();

        RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass(); 

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMAccountsClass Acc = new RRDMAccountsClass(); 

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMTempAtmLocation Tl = new RRDMTempAtmLocation();

        string WUserBankId; 

        // Variables for Replenishemnt 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
    //    bool WPrive;
        string WAtmNo;
        int WActType;

        public Form65(string InSignedId, int SignRecordNo, string InOperator, string InAtmNo, int InActType)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
    //        WPrive = InPrive;
            WAtmNo = InAtmNo;
            WActType = InActType; // 1= View, 2=Update, 3=New Like , 4=New, 5= Inactivate 

            InitializeComponent();

            // ================USER BANK =============================
            Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            WUserBankId = Us.Operator;
            // ========================================================

            // Activate combos when updating takes place 

            // Banks available for the seed bank 
            comboBox24.DataSource = Cc.GetBanksIds(WUserBankId); 
            comboBox24.DisplayMember = "DisplayValue";
          
            // ATM categories groups
            comboBox16.DataSource = Cc.GetAtmCategories(WUserBankId); 
            comboBox16.DisplayMember = "DisplayValue";

            // ATM Replenishment Group
            comboBox18.DataSource = Cc.GetAtmsReplGroups(WUserBankId);
            comboBox18.DisplayMember = "DisplayValue";

            // ATM Reconc Group
            comboBox23.DataSource = Cc.GetAtmsReconcGroups(WUserBankId);
            comboBox23.DisplayMember = "DisplayValue";

            

            // Districts
            Gp.ParamId = "226";
            comboBox27.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox27.DisplayMember = "DisplayValue";

            // Countries
            Gp.ParamId = "227";
            comboBox28.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox28.DisplayMember = "DisplayValue";


            // Suppliers 
            Gp.ParamId = "204"; // Suppliers   
            comboBoxSupplier.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBoxSupplier.DisplayMember = "DisplayValue";

            // Model 
            Gp.RelatedParmId = "204"; // Supplier
            Gp.RelatedOccuranceId = "1";
            comboBoxModel.DataSource = Gp.GetParamOccurancesRelatedNm(WUserBankId, Gp.RelatedParmId, Gp.RelatedOccuranceId);
            comboBoxModel.DisplayMember = "DisplayValue";

            // ATM PHYSICAL CHARACTERISTICS 
            Gp.ParamId = "301"; // Atm Maintenace Type  
            comboBox1.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox1.DisplayMember = "DisplayValue";

            // FIRST CASSETTE

            Gp.ParamId = "201"; // Currencies first Cassette 
            comboBox2.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox2.DisplayMember = "DisplayValue";

            // Face Value first cassette 
            Gp.ParamId = "206";
            comboBox3.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox3.DisplayMember = "DisplayValue";

            // Capacity %  first cassette 
            Gp.ParamId = "207";
            comboBox4.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox4.DisplayMember = "DisplayValue";

            // SECOND CASSETTE 
            Gp.ParamId = "201"; // Currencies Second Cassettes 
            comboBox7.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox7.DisplayMember = "DisplayValue";

            // Face Value  
            Gp.ParamId = "206";
            comboBox6.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox6.DisplayMember = "DisplayValue";

            // Capacity %  
            Gp.ParamId = "207";
            comboBox5.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox5.DisplayMember = "DisplayValue";


            // THIRD CASSETTE 

            Gp.ParamId = "201"; // Currencies third Cassette 
            comboBox11.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox11.DisplayMember = "DisplayValue";


            // Face Value  
            Gp.ParamId = "206";
            comboBox10.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox10.DisplayMember = "DisplayValue";

            // Capacity %  
            Gp.ParamId = "207";
            comboBox9.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox9.DisplayMember = "DisplayValue";


            // FORTH CASSETTE 

            Gp.ParamId = "201"; // Currencies forth Cassettes 
            comboBox15.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox5.DisplayMember = "DisplayValue";


            // Face Value  
            Gp.ParamId = "206";
            comboBox14.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox14.DisplayMember = "DisplayValue";

            // Capacity %  
            Gp.ParamId = "207";
            comboBox13.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox13.DisplayMember = "DisplayValue";

            // REPLENISHED TYPE   
            Gp.ParamId = "208";
            comboBox17.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox17.DisplayMember = "DisplayValue";

            // Holidays Versions 

            Gp.ParamId = "215";
            comboBox25.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox25.DisplayMember = "DisplayValue";
 
            // MATCHED DATES TYPES 
         
            Gp.ParamId = "209";
            comboBox21.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox21.DisplayMember = "DisplayValue";

            // OVER % 
            Gp.ParamId = "210";
            comboBox20.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox20.DisplayMember = "DisplayValue";
   
            // CIT PROVIDERS 
            //  
            comboBox22.DataSource = Cc.GetCitIds(WUserBankId);
            comboBox22.DisplayMember = "DisplayValue";

            // NUMBER OF CASSETTES 
            // 
            Gp.ParamId = "302";
            comboBox12.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox12.DisplayMember = "DisplayValue";

            // ALERT DAYS 
            // 
            Gp.ParamId = "303";
            comboBox8.DataSource = Gp.GetParamOccurancesId(WUserBankId);
            comboBox8.DisplayMember = "DisplayValue";


            // DEPOSIT CURRENCY 
            Gp.ParamId = "201"; // Currencies forth Cassettes 
            comboBox19.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox9.DisplayMember = "DisplayValue";

            // REpl Cash In Type 
            Gp.ParamId = "203"; // Cash In Type 
            comboBoxCashInType.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBoxCashInType.DisplayMember = "DisplayValue";
   

            dateTimePicker1.CustomFormat = "yyyy";

            // 1= View, 2=Update, 3=New Like , 4=New, 5= Inactivate 

            // Call Needed methods to Show DATA ONLY 
            if (WActType == 1)
            {
                button1.Hide();
                buttonAdd.Hide();
                textBox6.Hide();
                label35.Hide();
                textBoxAtmNo.Text = WAtmNo;

                buttonRefresh.Hide(); 

                ErrInResult = false;

                SELECTATMS(WAtmNo);   // Execute Method Select Atm 

                if (ErrInResult == true)
                {
                    return;
                }
            }

            // Show Data for updating 
            if (WActType == 2)
            {
                buttonAdd.Hide();
                button1.Show();
                textBox6.Hide();
                label35.Hide();
                textBoxAtmNo.Text = WAtmNo;

                ErrInResult = false;

                SELECTATMS(WAtmNo);   // Execute Method Select Atm 

                if (ErrInResult == true)
                {
                    return;
                }

            }

            // Call Needed methods to show Like 
            if (WActType == 3)
            {
                button1.Hide();
                buttonAdd.Show();
                textBoxAtmNo.Text = "";

                ErrInResult = false;

                SELECTATMS(WAtmNo);   // Execute Method Select Atm 

                if (ErrInResult == true)
                {
                    return;
                }

                textBox6.Text = WAtmNo; // THE LIKE ATM 

                textBox2.Text = "Fill Data";
           
                textBox3.Text = "Fill Data";
                textBox4.Text = "Fill Data";
                textBox5.Text = "Fill Data";

                textBox7.Text = "Fill Data"; // Postal Code 

                textBoxLatitude.Text = "";
                textBoxLongitude.Text = "";

            }

            // Show fields for NEW  
            if (WActType == 4)
            {
                button1.Hide();
                buttonAdd.Show();
                textBox6.Hide();
                label35.Hide();
               
            }

            buttonMenu1_Click(this, new EventArgs());
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void buttonMenu1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel2.Visible = false;
            panel3.Visible = false;
            panel5a.Visible = false;
            panel9.Visible = false;

            buttonMenu1.BackColor = Color.White;
            buttonMenu1.ForeColor = Color.FromArgb(35,119,192);
            buttonMenu2.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu2.ForeColor = Color.White;
            buttonMenu3.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu3.ForeColor = Color.White;
            buttonMenu4.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu4.ForeColor = Color.White;
            buttonMenu5.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu5.ForeColor = Color.White;

        }

        private void comboBoxModel_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonMenu2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true;
            panel3.Visible = false;
            panel5a.Visible = false;
            panel9.Visible = false;

            buttonMenu2.BackColor = Color.White;
            buttonMenu2.ForeColor = Color.FromArgb(35, 119, 192);
            buttonMenu1.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu1.ForeColor = Color.White;
            buttonMenu3.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu3.ForeColor = Color.White;
            buttonMenu4.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu4.ForeColor = Color.White;
            buttonMenu5.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu5.ForeColor = Color.White;
        }

        private void buttonMenu3_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = true;
            panel5a.Visible = false;
            panel9.Visible = false;

            buttonMenu3.BackColor = Color.White;
            buttonMenu3.ForeColor = Color.FromArgb(35, 119, 192);
            buttonMenu1.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu1.ForeColor = Color.White;
            buttonMenu2.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu2.ForeColor = Color.White;
            buttonMenu4.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu4.ForeColor = Color.White;
            buttonMenu5.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu5.ForeColor = Color.White;
        }

        private void buttonMenu4_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel5a.Visible = true;
            panel9.Visible = false;

            buttonMenu4.BackColor = Color.White;
            buttonMenu4.ForeColor = Color.FromArgb(35, 119, 192);
            buttonMenu1.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu1.ForeColor = Color.White;
            buttonMenu2.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu2.ForeColor = Color.White;
            buttonMenu3.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu3.ForeColor = Color.White;
            buttonMenu5.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu5.ForeColor = Color.White;
        }

        // Add ATM OR UPDATE 
        // 1= View, 2=Update, 3=New Like , 4=New, 5= Inactivate 
        private void button2_Click(object sender, EventArgs e)
        {

            if (WActType == 3)
            {
                if (textBoxAtmNo.Text == "")
                {
                    MessageBox.Show("Please enter Atm No");
                    return; 
                }
                else
                {
                    WAtmNo = textBoxAtmNo.Text; 
                }
            }

            // GOOGLE MAPS VALIDATION
            Tl.ReadTempAtmLocationSpecific(WAtmNo);
            if (RecordFound == true & Tl.LocationFound)
            {
                if (Tl.AddressChanged == true)
                {
                    textBox5.Text = Tl.NewStreet;
                    textBoxMunicipalityOrVillage.Text = Tl.NewTown;
                    comboBox27.Text = Tl.NewDistrict;
                    comboBox28.Text = Tl.NewCountry;
                }

                textBoxLatitude.Text = Tl.Latitude.ToString();
                textBoxLongitude.Text = Tl.Longitude.ToString();
            }
            else
            {
                if (textBoxLatitude.Text == "" || textBoxLongitude.Text == "")
                {
                    if (MessageBox.Show("Google Map position not updated yet. Do you want to Add or Update without Google coordinates?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                             == DialogResult.Yes)
                    {
                        // Proceed 
                    }
                    else
                    {
                        MessageBox.Show(" Search Location in Google Maps");
                        return;
                    }
                }            
            }


            ErrInResult = false;

            ValidationOfAtmInput(WAtmNo); // Validation of ATM Input 

            ValidationOfPhysicalInput(WAtmNo); // Validation of Physical Input 

            if (ErrInResult == true)
            {
                return;
            }

            if (WActType == 2)
            {
                UpdateATM(WAtmNo); // Update ATM

                GetAtmNoAndfields(WAtmNo);  // Get NEW VALUES from updated ATM

                Am.ReadAtmsMainSpecific(WAtmNo);

                Am.AtmNo = WAtmNo;
                Am.AtmName = AtmName;
                Am.BankId = BankId;

                Am.RespBranch = RespBranch;
                Am.BranchName = BranchName;

                Am.NextReplDt = FutureDate;
                Am.EstReplDt = FutureDate;

                Am.LastUpdated = DateTime.Now;

                Am.CitId = CitId;

                Am.AtmsReconcGroup = AtmsReconcGroup; 

                Am.UpdateAtmsMain(WAtmNo); // UPDATE MAIN WITH NEW VALUES 
                //
                // READ FOR UPDATE
                // 
                Ap.ReadTableATMsCostSpecific(WAtmNo);

                Ap.ManifactureDt = ManifactureDt;
                Ap.PurchaseDt = PurchaseDt;
                Ap.DueServiceDt = DueServiceDt;
                Ap.LastServiceDt = LastServiceDt;
                Ap.PurchaseCost = PurchaseCost;
                Ap.MaintenanceCd = MaintenanceCd;
                Ap.AnnualMaint = AnnualMaint;
                Ap.CitOnCall = CitOnCall;
                Ap.CitAnnual = CitAnnual;

                Ap.UpdateTableATMsCost(WAtmNo, BankId);

                // ATM JOURNAL DETAILS
                Jd.ReadJTMIdentificationDetailsByAtmNo(WAtmNo);
                Jd.DateLastUpdated = DateTime.Now; 
                Jd.UserId = WSignedId;

                Jd.BatchID = textBox13.Text;
                Jd.ATMIPAddress = textBox1.Text;
                Jd.ATMMachineName = textBox10.Text;

                Jd.ATMWindowsAuth = checkBoxWindowsAuth.Checked;

                Jd.ATMAccessID = textBox11.Text;
                Jd.ATMAccessPassword = textBox12.Text;

                Jd.TypeOfJournal = comboBox26.Text;
                Jd.SourceFileName = textBox19.Text;

                Jd.SourceFilePath = textBox20.Text;

                Jd.DestnFilePath = textBox27.Text;


                Jd.UpdateRecordInJTMIdentificationDetailsByAtmNo(WAtmNo); 

                MessageBox.Show("ATM has been updated");

                buttonFinishOrCancel.Text = "Finish"; 

            }
            if (WActType == 3) // INSERT LIKE
            {
                if (String.IsNullOrEmpty(textBoxAtmNo.Text))
                {
                    MessageBox.Show("Insert ATM number");
                    return;
                }
                else
                {
                    WAtmNo = textBoxAtmNo.Text;
                }

                ErrInResult = false;

                SELECTATMS(WAtmNo);   // Execute Method Select Atm 

                if (ErrInResult == true)
                {
                    return;
                }

                if (RecordFound == true)
                {
                    MessageBox.Show("THIS ATM NO ALREADY EXIST");
                    return;
                }
                // =============INSERT 1=======================
                InsertATM(WAtmNo);

                GetAtmNoAndfields(WAtmNo);      // Get Some needed fields

                Am.AtmNo = WAtmNo;
                Am.AtmName = AtmName;
                Am.BankId = BankId;

      //          Am.Prive = WPrive;

                Am.RespBranch = RespBranch;
                Am.BranchName = BranchName;

                Am.NextReplDt = FutureDate;
                Am.EstReplDt = FutureDate;

                Am.LastUpdated = DateTime.Now;

                Am.CitId = CitId;

                Am.AtmsReconcGroup = AtmsReconcGroup;

                Am.Operator = WOperator;

                // =================INSERT 2=================
                Am.InsertInAtmsMain(WAtmNo); // Insert AtmMain record 

                // INSERT PHYSICAL
                //
                Ap.ManifactureDt = ManifactureDt;
                Ap.PurchaseDt = PurchaseDt;
                Ap.DueServiceDt = DueServiceDt;
                Ap.LastServiceDt = LastServiceDt;
                Ap.PurchaseCost = PurchaseCost;
                Ap.MaintenanceCd = MaintenanceCd;
                Ap.AnnualMaint = AnnualMaint;
                Ap.CitOnCall = CitOnCall;
                Ap.CitAnnual = CitAnnual; 

                // ===============Insert 3================
                Ap.InsertTableATMsCost(WAtmNo, BankId); 

                // INSER Journal Information 
                //
     
                Jd.AtmNo = WAtmNo;
                Jd.UserId = WSignedId ;
                Jd.BatchID = textBox13.Text; 
                Jd.ATMIPAddress = textBox1.Text;
                Jd.ATMMachineName = textBox10.Text;

                Jd.ATMWindowsAuth = checkBoxWindowsAuth.Checked;

                Jd.ATMAccessID = textBox11.Text;
                Jd.ATMAccessPassword = textBox12.Text;

                Jd.TypeOfJournal = comboBox26.Text;
                Jd.SourceFileName = textBox19.Text;

                Jd.SourceFilePath = textBox20.Text;

                Jd.DestnFilePath = textBox27.Text;

                Jd.Operator = WOperator; 

                // ===============Insert 4================
                Jd.InsertNewRecordInJTMIdentificationDetails(); 

                // ==============Copy ACCOUNTS FROM LIKE==========
                Ac.ReadAtm(textBox6.Text); 

                Acc.CopyAccountsAtmToAtm(Ac.BankId, textBox6.Text, comboBox24.Text, WAtmNo);
                if (Acc.RecordFound == false)
                {
                    MessageBox.Show("There were no accounts to copy. After ATM creation go and create accounts manually please for the added ATM .");
                    MessageBox.Show("ATM added without accounts");
                }
                else
                {
                    MessageBox.Show("ATM added");

                    buttonFinishOrCancel.Text = "Finish"; 

                    buttonAdd.Visible = false; 
                }   

            }
            if (WActType == 4)  // Insert New 
            {
                if (String.IsNullOrEmpty(textBoxAtmNo.Text))
                {
                    MessageBox.Show("Insert ATM number ");
                    return;
                }
                else
                {
                    WAtmNo = textBoxAtmNo.Text;
                }

                ErrInResult = false;

                SELECTATMS(WAtmNo);   // Execute Method Select Atm 

                if (ErrInResult == true)
                {
                    return;
                }

                if (RecordFound == true)
                {
                    MessageBox.Show("THIS ATM NO ALREADY EXIST");
                    return;
                }

                InsertATM(WAtmNo); // Insert ATM record

                GetAtmNoAndfields(WAtmNo);      // Get Some needed fields

                Am.AtmNo = WAtmNo;
                Am.AtmName = AtmName;
                Am.BankId = BankId;

                Am.RespBranch = RespBranch;
                Am.BranchName = BranchName;

                Am.NextReplDt = FutureDate;
                Am.EstReplDt = FutureDate;

                Am.LastUpdated = DateTime.Now;

                Am.CitId = CitId;

                Am.AtmsReconcGroup = AtmsReconcGroup;

                Am.Operator = WOperator;

                Am.InsertInAtmsMain(WAtmNo); // Insert AtmMain record 

                // INSERT PHYSICAL
                //
                Ap.ManifactureDt = ManifactureDt;
                Ap.PurchaseDt = PurchaseDt;
                Ap.DueServiceDt = DueServiceDt;
                Ap.LastServiceDt = LastServiceDt;
                Ap.PurchaseCost = PurchaseCost;
                Ap.MaintenanceCd = MaintenanceCd;
                Ap.AnnualMaint = AnnualMaint;
                Ap.CitOnCall = CitOnCall;
                Ap.CitAnnual = CitAnnual;

                Ap.InsertTableATMsCost(WAtmNo, BankId); 

                textBox6.Text = " NEW ATM ";

                MessageBox.Show("ATM added");

                buttonFinishOrCancel.Text = "Finish"; 
            } 
        }

        private void SELECTATMS(string InAtmNo)
        {
            RecordFound = false;
            string stringlSelect = "SELECT * "
                + " FROM TableATMsBasic"
                + " WHERE AtmNo=@AtmNo";
            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(stringlSelect, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            RecordFound = true;

                            textBox2.Text = rdr["AtmName"].ToString();
                            comboBox24.Text = rdr["BankId"].ToString();
                            if (WActType == 3) // Like case 
                            {
                                comboBox24.Text = WUserBankId; 
                            }
                          //  checkBox7.Checked = (bool)rdr["Prive"];
                            textBox3.Text = rdr["Branch"].ToString();
                            textBox4.Text = rdr["BranchName"].ToString();

                            textBox5.Text = rdr["Street"].ToString();
                            textBoxMunicipalityOrVillage.Text = rdr["Town"].ToString();
                            comboBox27.Text = rdr["District"].ToString();
                            textBox7.Text = rdr["PostalCode"].ToString();
                            comboBox28.Text = rdr["Country"].ToString();

                            Latitude = (double)rdr["Latitude"];
                            textBoxLatitude.Text = Latitude.ToString();

                            Longitude = (double)rdr["Longitude"];
                            textBoxLongitude.Text = Longitude.ToString();
                        
                            comboBox16.Text = rdr["AtmsStatsGroup"].ToString();
                            comboBox18.Text = rdr["AtmsReplGroup"].ToString();
                            comboBox23.Text = rdr["AtmsReconcGroup"].ToString();

                            comboBox22.Text = rdr["CitId"].ToString();

                            checkBox1.Checked = (bool)rdr["Loby"];
                            checkBox2.Checked = (bool)rdr["Wall"];
                            checkBox3.Checked = (bool)rdr["Drive"];
                            checkBox6.Checked = (bool)rdr["OffSite"];

                            comboBox17.Text = rdr["TypeOfRepl"].ToString();

                            comboBoxCashInType.Text = rdr["CashInType"].ToString();

                            comboBox25.Text = rdr["HolidaysVersion"].ToString();

                            comboBox21.Text = rdr["MatchDatesCateg"].ToString();
                            
                            comboBox20.Text = rdr["OverEst"].ToString();

                            decimal MinCash = (decimal)rdr["MinCash"];

                            textBox9.Text = MinCash.ToString("#,##0.00");

                            decimal MaxCash = (decimal)rdr["MaxCash"];
                            textBox8.Text = MaxCash.ToString("#,##0.00");

                            comboBox8.Text = rdr["ReplAlertDays"].ToString();

                            decimal Insur; 

                            Insur = (decimal)rdr["InsurOne"];
                            textBox17.Text = Insur.ToString("#,##0.00");

                            Insur = (decimal)rdr["InsurTwo"];
                            textBox14.Text = Insur.ToString("#,##0.00");

                            Insur = (decimal)rdr["InsurThree"];
                            textBox16.Text = Insur.ToString("#,##0.00");

                            Insur = (decimal)rdr["InsurFour"];
                            textBox18.Text = Insur.ToString("#,##0.00");

                            comboBoxSupplier.Text = rdr["Supplier"].ToString();
                            comboBoxModel.Text = rdr["Model"].ToString();
                            comboBox12.Text = rdr["NoCassettes"].ToString();
                            checkBox4.Checked = (bool)rdr["DepoReader"];
                            checkBox5.Checked = (bool)rdr["ChequeReader"];
                            checkBox8.Checked = (bool)rdr["EnvelopDepos"];

                            ActiveAtm = (bool)rdr["ActiveAtm"];

                            if (ActiveAtm == true)
                            {
                                radioButton1.Checked = true;
                            }
                            else radioButton2.Checked = true;

                            if (WActType == 3)
                            {
                                radioButton1.Checked = false;
                                radioButton2.Checked = true;
                            }

                     //       comboBox18.Text = rdr["DepCurCd"].ToString();
                            comboBox19.Text = rdr["DepCurNm"].ToString();


                            //   MessageBox.Show(comboBox1.Text);
                           
                            comboBox2.Text = rdr["CurName_11"].ToString();
                            comboBox3.Text = rdr["FaceValue_11"].ToString();
                            comboBox4.Text = rdr["CasCapacity_11"].ToString();
                         
                            comboBox7.Text = rdr["CurName_12"].ToString();
                            comboBox6.Text = rdr["FaceValue_12"].ToString();
                            comboBox5.Text = rdr["CasCapacity_12"].ToString();
                          
                            comboBox11.Text = rdr["CurName_13"].ToString();
                            comboBox10.Text = rdr["FaceValue_13"].ToString();
                            comboBox9.Text = rdr["CasCapacity_13"].ToString();
                          
                            comboBox15.Text = rdr["CurName_14"].ToString();
                            comboBox14.Text = rdr["FaceValue_14"].ToString();
                            comboBox13.Text = rdr["CasCapacity_14"].ToString();

                            if (WActType != 4)
                            {
                                try
                                {
                                //    LocationScreenshot = (byte[])rdr["LocationImage"];

                                }
                                catch { }
                            }

                        }

                        // Close Reader
                        rdr.Close();
                    }

                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    exception = ex.ToString();
                    // MessageBox.Show(exception);
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }

            Ap.ReadTableATMsCostSpecific(WAtmNo);

            dateTimePicker1.Value = Ap.ManifactureDt;
            dateTimePicker2.Value = Ap.PurchaseDt;
            dateTimePicker3.Value = Ap.DueServiceDt;
            dateTimePicker4.Value = Ap.LastServiceDt;

            textBox25.Text = Ap.PurchaseCost.ToString("#,##0.00");
            comboBox1.Text = Ap.MaintenanceCd.ToString();
            textBox24.Text = Ap.AnnualMaint.ToString("#,##0.00");
            textBox23.Text = Ap.CitOnCall.ToString("#,##0.00");
            textBox26.Text = CitAnnual.ToString("#,##0.00");

            Jd.ReadJTMIdentificationDetailsByAtmNo(WAtmNo);


            textBox13.Text = Jd.BatchID;
            textBox1.Text = Jd.ATMIPAddress;

            
            textBox1.Text = Jd.ATMIPAddress;
            textBox10.Text = Jd.ATMMachineName ;

            checkBoxWindowsAuth.Checked = Jd.ATMWindowsAuth ;

            textBox11.Text = Jd.ATMAccessID ;

            textBox12.Text = Jd.ATMAccessPassword ;
            textBox15.Text = Jd.ATMAccessPassword;

            comboBox26.Text = Jd.TypeOfJournal ;
            textBox19.Text = Jd.SourceFileName ;

            textBox20.Text = Jd.SourceFilePath ;

            textBox27.Text = Jd.DestnFilePath ;

        }
        private void GetAtmNoAndfields(string InAtmNo)
        {
            string stringlSelect = "SELECT * "
                + " FROM TableATMsBasic"
                + " WHERE AtmNo = @AtmNo";

            using (SqlConnection conn =
                          new SqlConnection(connectionString))
                try
                {
                    conn.Open();

                    using (SqlCommand cmd =
                        new SqlCommand(stringlSelect, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);

                        // Read table 

                        SqlDataReader rdr = cmd.ExecuteReader();

                        // Get Values to create ATM Main
                        while (rdr.Read())
                        {


                            AtmName = rdr["AtmName"].ToString();

                            BankId = (string)rdr["BankId"];

                            RespBranch = rdr["Branch"].ToString();
                            BranchName = rdr["BranchName"].ToString();
                          
                            Latitude = (double)rdr["Latitude"];
                            Longitude = (double)rdr["Longitude"];

                            AtmsStatsGroup = (int)rdr["AtmsStatsGroup"];
                            AtmsReplGroup = (int)rdr["AtmsReplGroup"];
                            AtmsReconcGroup = (int)rdr["AtmsReconcGroup"];

                            CitId = (string)rdr["CitId"];

                            OffSite = (bool)rdr["OffSite"];

                            TypeOfRepl = (string)rdr["TypeOfRepl"];
                            CashInType = (string)rdr["CashInType"];   

                            HolidaysVersion = (string)rdr["HolidaysVersion"];

                            MatchDatesCateg = (string)rdr["MatchDatesCateg"];

                            OverEst = (int)rdr["OverEst"];

                            MinCash = (decimal)rdr["MinCash"];

                            MaxCash = (decimal)rdr["MaxCash"];

                            ReplAlertDays = (int)rdr["ReplAlertDays"];

                            InsurOne = (decimal)rdr["InsurOne"];
                            InsurTwo = (decimal)rdr["InsurTwo"];
                            InsurThree = (decimal)rdr["InsurThree"];
                            InsurFour = (decimal)rdr["InsurFour"];
                        }
                        // Close Reader
                        rdr.Close();

                    }

                    // Close conn
                    conn.Close();

                }
                catch (Exception ex)
                {
                    exception = ex.ToString();
                    // MessageBox.Show(exception);
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }

        private void ValidationOfAtmInput(string WAtmNo)
        {
            // VALIDATION OF INPUT NUMERIC FIELDS 
         

            if (decimal.TryParse(textBox9.Text, out InMinCash))
            {
            }
            else
            {
                MessageBox.Show(textBox9.Text, "Please enter a valid number InMinCash!");
                ErrInResult = true;
                return;
            }

            if (decimal.TryParse(textBox8.Text, out InMaxCash))
            {
            }
            else
            {
                MessageBox.Show(textBox8.Text, "Please enter a valid number InMaxCash!");
                ErrInResult = true;
                return;
            }

            if (int.TryParse(comboBox8.Text, out ReplAlertDays))
            {
            }
            else
            {
                MessageBox.Show(comboBox8.Text, "Please enter a valid number ReplAlertDays!");
                ErrInResult = true;
                return;
            }

            if (decimal.TryParse(textBox17.Text, out InsurOne))
            {
            }
            else
            {
                MessageBox.Show(textBox17.Text, "Please enter a valid number InsurOne!");
                ErrInResult = true;
                return;
            }

            if (decimal.TryParse(textBox14.Text, out InsurTwo))
            {
            }
            else
            {
                MessageBox.Show(textBox14.Text, "Please enter a valid number InsurTwo!");
                ErrInResult = true;
                return;
            }

            if (decimal.TryParse(textBox16.Text, out InsurThree))
            {
            }
            else
            {
                MessageBox.Show(textBox16.Text, "Please enter a valid number InsurThree!");
                ErrInResult = true;
                return;
            }

            if (decimal.TryParse(textBox18.Text, out InsurFour))
            {
            }
            else
            {
                MessageBox.Show(textBox16.Text, "Please enter a valid number InsurFour!");
                ErrInResult = true;
                return;
            }

            if (int.TryParse(comboBox12.Text, out NumberOfCass))
            {
            }
            else
            {
                MessageBox.Show(comboBox12.Text, "Please enter a valid number NumberOfCass!");
                ErrInResult = true;
                return;
            }

            // Check that total percentages = 100 
            int CasPerc1 = int.Parse(comboBox4.Text);
            int CasPerc2 = int.Parse(comboBox5.Text);
            int CasPerc3 = int.Parse(comboBox9.Text);
            int CasPerc4 = int.Parse(comboBox13.Text);

            int Total = CasPerc1 + CasPerc2 + CasPerc3 + CasPerc4;

            if (Total != 100)
            {
                MessageBox.Show("Total cassette percentages must be equal to 100");
                ErrInResult = true;
                return;
            }

            //if (textBoxLatitude.Text == "" || textBoxLatitude.Text == "Fill Data")
            //{
            //    MessageBox.Show("Please Fill In Postion Data");
            //    return; 
            //}

        }

        private void ValidationOfPhysicalInput(string WAtmNo)
        {
            // VALIDATION OF INPUT NUMERIC FIELDS 

            ManifactureDt = dateTimePicker1.Value;
            PurchaseDt = dateTimePicker2.Value;
            DueServiceDt = dateTimePicker3.Value;
            LastServiceDt = dateTimePicker4.Value;

            if (decimal.TryParse(textBox25.Text, out PurchaseCost))
            {
            }
            else
            {
                MessageBox.Show(textBox25.Text, "Please enter a valid number Purchase Cost!");
                ErrInResult = true;
                return;
            }

            if (int.TryParse(comboBox1.Text, out MaintenanceCd))
            {
            }
            else
            {
                MessageBox.Show(comboBox1.Text, "Please enter a valid number Maintenance Code!");
                ErrInResult = true;
                return;
            }

            if (decimal.TryParse(textBox24.Text, out AnnualMaint))
            {
            }
            else
            {
                MessageBox.Show(textBox24.Text, "Please enter a valid number AnnualMaint!");
                ErrInResult = true;
                return;
            }

            if (decimal.TryParse(textBox23.Text, out CitOnCall))
            {
            }
            else
            {
                MessageBox.Show(textBox23.Text, "Please enter a valid number CitOnCall!");
                ErrInResult = true;
                return;
            }

            if (decimal.TryParse(textBox26.Text, out CitAnnual))
            {
            }
            else
            {
                MessageBox.Show(textBox26.Text, "Please enter a valid number CitAnnual!");
                ErrInResult = true;
                return;
            }
            // Check the two passwords are the same. 
            if (textBox12.Text !=  textBox15.Text)
            {
                MessageBox.Show("The two passwords inputed for Journal are not the same!");
                ErrInResult = true;
                return;
            }
            // 
            if (decimal.TryParse(textBox26.Text, out CitAnnual))
            {
            }
            else
            {
                MessageBox.Show(textBox26.Text, "Please enter a valid number CitAnnual!");
                ErrInResult = true;
                return;
            }
        }

        // UPDATE 

        private void UpdateATM(string InAtmNo)
        {
            string strUpdate = "UPDATE TableATMsBasic SET"
                + " AtmName=@AtmName, BankId=@BankId,"
                + "Branch=@Branch, BranchName=@BranchName,"
                + "Street=@Street, Town=@Town, District=@District, PostalCode=@PostalCode,Country=@Country," 
                + "Latitude=@Latitude,Longitude=@Longitude,"
                + " AtmsStatsGroup=@AtmsStatsGroup,AtmsReplGroup=@AtmsReplGroup, AtmsReconcGroup=@AtmsReconcGroup,"
                + " Loby=@Loby, Wall=@Wall, Drive=@Drive,OffSite=@OffSite,"
                + "TypeOfRepl=@TypeOfRepl, CashInType=@CashInType, HolidaysVersion=@HolidaysVersion,MatchDatesCateg=@MatchDatesCateg," 
                + "OverEst=@OverEst, MinCash=@MinCash, MaxCash=@MaxCash, ReplAlertDays=@ReplAlertDays,"
                 + "InsurOne=@InsurOne, InsurTwo =@InsurTwo, InsurThree=@InsurThree, InsurFour=@InsurFour,"
                + "Supplier=@Supplier, Model=@Model, CitId=@CitId, NoCassettes=@NoCassettes,"
                + " DepoReader=@DepoReader, ChequeReader=@ChequeReader,EnvelopDepos=@EnvelopDepos,"
               + " ActiveAtm=@ActiveAtm,"
               + " DepCurNm=@DepCurNm,"
                + " CurName_11=@CurName_11,FaceValue_11=@FaceValue_11,CasCapacity_11=@CasCapacity_11,"
                + " CurName_12=@CurName_12,FaceValue_12=@FaceValue_12,CasCapacity_12=@CasCapacity_12,"
                + " CurName_13=@CurName_13,FaceValue_13=@FaceValue_13,CasCapacity_13=@CasCapacity_13,"
                + " CurName_14=@CurName_14,FaceValue_14=@FaceValue_14,CasCapacity_14=@CasCapacity_14"
                + " WHERE AtmNo=@AtmNo";
            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand(strUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@AtmName", textBox2.Text);
                        
                        cmd.Parameters.AddWithValue("@BankId", comboBox24.Text);
                   //     cmd.Parameters.AddWithValue("WOperator", 0);
                        
                        cmd.Parameters.AddWithValue("@Branch", textBox3.Text);
                        cmd.Parameters.AddWithValue("@BranchName", textBox4.Text);

                        cmd.Parameters.AddWithValue("@Street", textBox5.Text);
                        cmd.Parameters.AddWithValue("@Town", textBoxMunicipalityOrVillage.Text);
                        cmd.Parameters.AddWithValue("@District", comboBox27.Text);
                        cmd.Parameters.AddWithValue("@PostalCode", textBox7.Text);
                       
                        cmd.Parameters.AddWithValue("@Country", comboBox28.Text);

                        cmd.Parameters.AddWithValue("@Latitude", Convert.ToDouble(textBoxLatitude.Text));
                        cmd.Parameters.AddWithValue("@Longitude",  Convert.ToDouble(textBoxLongitude.Text));

                  //      cmd.Parameters.AddWithValue("@LocationImage", LocationScreenshot);

                        cmd.Parameters.AddWithValue("@AtmsStatsGroup", AtmsStatsGroup);
                        cmd.Parameters.AddWithValue("@AtmsReplGroup", AtmsReplGroup);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);

                        cmd.Parameters.AddWithValue("@Loby", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@Wall", checkBox2.Checked);
                        cmd.Parameters.AddWithValue("@Drive", checkBox3.Checked);

                        cmd.Parameters.AddWithValue("@OffSite", checkBox6.Checked);

                        cmd.Parameters.AddWithValue("@TypeOfRepl", comboBox17.Text);

                        cmd.Parameters.AddWithValue("@CashInType", comboBoxCashInType.Text);
                        
                        cmd.Parameters.AddWithValue("@HolidaysVersion", comboBox25.Text);

                        cmd.Parameters.AddWithValue("@MatchDatesCateg", comboBox21.Text);

                        cmd.Parameters.AddWithValue("@OverEst", comboBox20.Text);

                        cmd.Parameters.AddWithValue("@MinCash", InMinCash);
                        cmd.Parameters.AddWithValue("@MaxCash", InMaxCash);
                        cmd.Parameters.AddWithValue("@ReplAlertDays", ReplAlertDays);

                        cmd.Parameters.AddWithValue("@InsurOne", InsurOne);
                        cmd.Parameters.AddWithValue("@InsurTwo", InsurTwo);
                        cmd.Parameters.AddWithValue("@InsurThree", InsurThree);
                        cmd.Parameters.AddWithValue("@InsurFour", InsurFour);

                        cmd.Parameters.AddWithValue("@Supplier", comboBoxSupplier.Text);
                        cmd.Parameters.AddWithValue("@Model", comboBoxModel.Text);

                        cmd.Parameters.AddWithValue("@CitId", comboBox22.Text);

                        cmd.Parameters.AddWithValue("@NoCassettes", comboBox12.Text);
                        cmd.Parameters.AddWithValue("@DepoReader", checkBox4.Checked);
                        cmd.Parameters.AddWithValue("@ChequeReader", checkBox5.Checked);
                        cmd.Parameters.AddWithValue("@EnvelopDepos", checkBox8.Checked);

                        if (radioButton1.Checked == true)
                        {
                            cmd.Parameters.AddWithValue("@ActiveAtm", 1);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ActiveAtm", 0);
                        }

                        // Deposits Currency
                        Gp.ParamId = "201" ; 
                        Gp.ReadParametersSpecificNm(Gp.ParamId, comboBox19.Text);
                        cmd.Parameters.AddWithValue("@DepCurNm", comboBox19.Text);

                        // Cassettes 

                        Gp.ParamId = "201"; // Currency for cassettes 

                        Gp.ReadParametersSpecificNm(Gp.ParamId, comboBox2.Text);

                    //    cmd.Parameters.AddWithValue("@CurCode_11", Gp.OccuranceId);
                        cmd.Parameters.AddWithValue("@CurName_11", comboBox2.Text);
                        cmd.Parameters.AddWithValue("@FaceValue_11", comboBox3.Text);
                        cmd.Parameters.AddWithValue("@CasCapacity_11", comboBox4.Text);

                        Gp.ReadParametersSpecificNm(Gp.ParamId, comboBox7.Text);

                      //  cmd.Parameters.AddWithValue("@CurCode_12", Gp.OccuranceId);
                        cmd.Parameters.AddWithValue("@CurName_12", comboBox7.Text);
                        cmd.Parameters.AddWithValue("@FaceValue_12", comboBox6.Text);
                        cmd.Parameters.AddWithValue("@CasCapacity_12", comboBox5.Text);

                        Gp.ReadParametersSpecificNm(Gp.ParamId, comboBox11.Text);

                     //   cmd.Parameters.AddWithValue("@CurCode_13", Gp.OccuranceId);
                        cmd.Parameters.AddWithValue("@CurName_13", comboBox11.Text);
                        cmd.Parameters.AddWithValue("@FaceValue_13", comboBox10.Text);
                        cmd.Parameters.AddWithValue("@CasCapacity_13", comboBox9.Text);

                        Gp.ReadParametersSpecificNm(Gp.ParamId, comboBox15.Text);

                     //   cmd.Parameters.AddWithValue("@CurCode_14", Gp.OccuranceId);
                        cmd.Parameters.AddWithValue("@CurName_14", comboBox15.Text);
                        cmd.Parameters.AddWithValue("@FaceValue_14", comboBox14.Text);
                        cmd.Parameters.AddWithValue("@CasCapacity_14", comboBox13.Text);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0) exception = " ATMs Table UPDATED ";
                        else exception = " Nothing WAS UPDATED ";

                        // Shows Updated Grid
                        /*  Form1 NForm1 = new Form1();
                          NForm1.Show(); */

                    }
                    // Close conn
                    conn.Close();

                }

                catch (Exception ex)
                {
                    exception = ex.ToString();
                    // MessageBox.Show(exception);
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));

                }
        }

        // Insert ATM
        private void InsertATM(string InAtmNo)
        {
            string cmdinsert = "INSERT INTO [TableATMsBasic] "
                + " ([AtmNo],[AtmName],[BankId],"
                + "[Branch],[BranchName],"
                + "[Street],[Town],[District],[PostalCode],[Country],"
                + "[Latitude],[Longitude],"
                + "[AtmsStatsGroup],[AtmsReplGroup],[AtmsReconcGroup],"
                + "[Loby],[Wall],[Drive],[OffSite],"
                + "[TypeOfRepl],  [CashInType], [HolidaysVersion],[MatchDatesCateg],[OverEst],[MinCash],[MaxCash],[ReplAlertDays],"
                + "[InsurOne],[InsurTwo],[InsurThree],[InsurFour],"
                + "[Supplier],[Model],[CitId],[NoCassettes],"
                + "[DepoReader],[ChequeReader],[EnvelopDepos],"
                + " [DepCurNm],"
                + "[CasNo_11],[CurName_11],[FaceValue_11] ,[CasCapacity_11],"
                + "[CasNo_12],[CurName_12],[FaceValue_12],[CasCapacity_12],"
                + "[CasNo_13],[CurName_13],[FaceValue_13] ,[CasCapacity_13],"
                + "[CasNo_14],[CurName_14],[FaceValue_14] ,[CasCapacity_14],"
                + "[ActiveAtm],"
                + "[Operator])"
                + " VALUES (@AtmNo,@NAtmName,@BankId,"
                + "@NBranch,@NBranchName,"
                 + "@Street,@Town,@District,@PostalCode,@Country,"
                + "@Latitude,@Longitude,"
                + "@AtmsStatsGroup,@AtmsReplGroup,@AtmsReconcGroup,"
                + "@NLoby,@NWall,@NDrive,@OffSite,"
                + "@TypeOfRepl, @CashInType, @HolidaysVersion,@MatchDatesCateg,@OverEst,@MinCash,@MaxCash,@ReplAlertDays,"
                 + "@InsurOne,@InsurTwo,@InsurThree,@InsurFour,"
                + "@NSupplier,@NModel,@CitId,@NNoCassettes,"
                + "@NDepoReader,@NChequeReader,@EnvelopDepos,"          
                 + " @DepCurNm,"
                + "@CasNo_11,@CurName_11,@FaceValue_11 ,@CasCapacity_11,"
                + "@CasNo_12,@CurName_12,@FaceValue_12,@CasCapacity_12,"
                + "@CasNo_13,@CurName_13,@FaceValue_13 ,@CasCapacity_13,"
                + "@CasNo_14,@CurName_14,@FaceValue_14 ,@CasCapacity_14, "
                + "@ActiveAtm,"
                + "@Operator)";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(cmdinsert, conn))
                    {
                        // MessageBox.Show (textBox1.Text);

                        cmd.Parameters.AddWithValue("@AtmNo", InAtmNo);
                        cmd.Parameters.AddWithValue("@NAtmName", textBox2.Text);

                        cmd.Parameters.AddWithValue("@BankId", comboBox24.Text);
                       
                        cmd.Parameters.AddWithValue("@NBranch", textBox3.Text);
                        cmd.Parameters.AddWithValue("@NBranchName", textBox4.Text);

                        cmd.Parameters.AddWithValue("@Street", textBox5.Text);
                        cmd.Parameters.AddWithValue("@Town", textBoxMunicipalityOrVillage.Text);
                        cmd.Parameters.AddWithValue("@District", comboBox27.Text);

                        cmd.Parameters.AddWithValue("@PostalCode", textBox7.Text);

                        cmd.Parameters.AddWithValue("@Country", comboBox28.Text);

                        if (textBoxLatitude.Text == "" & textBoxLongitude.Text == "")
                        {
                            cmd.Parameters.AddWithValue("@Latitude", Convert.ToDouble("14.53"));
                            cmd.Parameters.AddWithValue("@Longitude", Convert.ToDouble("14.53"));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Latitude", Convert.ToDouble(textBoxLatitude.Text));
                            cmd.Parameters.AddWithValue("@Longitude", Convert.ToDouble(textBoxLongitude.Text));
                        }
                       
               //         cmd.Parameters.AddWithValue("@LocationImage", LocationScreenshot);

                        cmd.Parameters.AddWithValue("@AtmsStatsGroup", AtmsStatsGroup);
                        cmd.Parameters.AddWithValue("@AtmsReplGroup", AtmsReplGroup);
                        cmd.Parameters.AddWithValue("@AtmsReconcGroup", AtmsReconcGroup);

                        cmd.Parameters.AddWithValue("@NLoby", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@NWall", checkBox2.Checked);
                        cmd.Parameters.AddWithValue("@NDrive", checkBox3.Checked);

                        cmd.Parameters.AddWithValue("@OffSite", checkBox6.Checked);

                        cmd.Parameters.AddWithValue("@TypeOfRepl", comboBox17.Text);
                        cmd.Parameters.AddWithValue("@CashInType", comboBoxCashInType.Text);

                        cmd.Parameters.AddWithValue("@HolidaysVersion", comboBox25.Text);

                        cmd.Parameters.AddWithValue("@MatchDatesCateg", comboBox21.Text);
                        
                        cmd.Parameters.AddWithValue("@OverEst", comboBox20.Text);

                        cmd.Parameters.AddWithValue("@MinCash", InMinCash);
                        cmd.Parameters.AddWithValue("@MaxCash", InMaxCash);
                        cmd.Parameters.AddWithValue("@ReplAlertDays", ReplAlertDays);

                        cmd.Parameters.AddWithValue("@InsurOne", InsurOne);
                        cmd.Parameters.AddWithValue("@InsurTwo", InsurTwo);
                        cmd.Parameters.AddWithValue("@InsurThree", InsurThree);
                        cmd.Parameters.AddWithValue("@InsurFour", InsurFour);


                        cmd.Parameters.AddWithValue("@NSupplier", comboBoxSupplier.Text);
                        cmd.Parameters.AddWithValue("@NModel", comboBoxModel.Text);
                        cmd.Parameters.AddWithValue("@CitId", comboBox22.Text);
                        cmd.Parameters.AddWithValue("@NNoCassettes", Convert.ToInt32(comboBox12.Text));
                        cmd.Parameters.AddWithValue("@NDepoReader", checkBox4.Checked);
                        cmd.Parameters.AddWithValue("@NChequeReader", checkBox5.Checked);

                        cmd.Parameters.AddWithValue("@EnvelopDepos", checkBox8.Checked);

                        if (radioButton1.Checked == true)
                        {
                            cmd.Parameters.AddWithValue("@ActiveAtm", 1);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ActiveAtm", 0);
                        }

                        // Deposits Currency
                  //      Gp.ParamId = 201;
                  //      Gp.ReadParametersSpecificNm(Gp.ParamId, comboBox19.Text);
                  //      cmd.Parameters.AddWithValue("@DepCurCd", Gp.OccuranceId);
                        cmd.Parameters.AddWithValue("@DepCurNm", comboBox19.Text);

                        // Cassettes 

                     //   Gp.ParamId = 201; // Currency for cassettes 

                    //    Gp.ReadParametersSpecificNm(Gp.ParamId, comboBox2.Text);

                        cmd.Parameters.AddWithValue("@CasNo_11", 1); // Cassette 1 
                     //   cmd.Parameters.AddWithValue("@CurCode_11", Gp.OccuranceId);
                        cmd.Parameters.AddWithValue("@CurName_11", comboBox2.Text);
                        cmd.Parameters.AddWithValue("@FaceValue_11", Convert.ToInt32(comboBox3.Text));
                        cmd.Parameters.AddWithValue("@CasCapacity_11", Convert.ToInt32(comboBox4.Text));

                     //   Gp.ReadParametersSpecificNm(Gp.ParamId, comboBox7.Text);

                        cmd.Parameters.AddWithValue("@CasNo_12", 2); // Cassette 2
                     //   cmd.Parameters.AddWithValue("@CurCode_12", Gp.OccuranceId);
                        cmd.Parameters.AddWithValue("@CurName_12", comboBox7.Text);
                        cmd.Parameters.AddWithValue("@FaceValue_12", Convert.ToInt32(comboBox6.Text));
                        cmd.Parameters.AddWithValue("@CasCapacity_12", Convert.ToInt32(comboBox5.Text));

                     //   Gp.ReadParametersSpecificNm(Gp.ParamId, comboBox11.Text);

                        cmd.Parameters.AddWithValue("@CasNo_13", 3); // Cassette 3
                     //   cmd.Parameters.AddWithValue("@CurCode_13", Gp.OccuranceId);
                        cmd.Parameters.AddWithValue("@CurName_13", comboBox11.Text);
                        cmd.Parameters.AddWithValue("@FaceValue_13", Convert.ToInt32(comboBox10.Text));
                        cmd.Parameters.AddWithValue("@CasCapacity_13", Convert.ToInt32(comboBox9.Text));

                    //    Gp.ReadParametersSpecificNm(Gp.ParamId, comboBox15.Text);

                        cmd.Parameters.AddWithValue("@CasNo_14", 4); // Cassette 4
                  //      cmd.Parameters.AddWithValue("@CurCode_14", Gp.OccuranceId);
                        cmd.Parameters.AddWithValue("@CurName_14", comboBox15.Text);
                        cmd.Parameters.AddWithValue("@FaceValue_14", Convert.ToInt32(comboBox14.Text));
                        cmd.Parameters.AddWithValue("@CasCapacity_14", Convert.ToInt32(comboBox13.Text));

                        cmd.Parameters.AddWithValue("@Operator", WOperator);

                        //rows number of record got updated

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0) exception = " A NEW ATM WAS CREADED - ITs ATM No iS GIVEN AUTOMATICALLY. GO TO TABLE TO SEE IT";
                        else exception = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    exception = ex.ToString();
                    // MessageBox.Show(exception);
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }
        }

        // CASSETTES CHANGE 
        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse(comboBox12.Text, out NumberOfCass))
            {
            }
            else
            {
                MessageBox.Show(comboBox12.Text, "Please enter a valid number!");
                return;
            }

            if (NumberOfCass > 4)
            {
                MessageBox.Show(comboBox12.Text, "Please enter a valid number no more than 4!");
                return;
            }

            ShowCassettes(NumberOfCass);
        }


        private void ShowCassettes(int InNoCass)
        {
            if (InNoCass == 4)
            {
                label19.Show();
                panel5.Show();
                label21.Show();
                panel6.Show();
                label25.Show();
                panel7.Show();
                label30.Show();
                panel8.Show();
            }
            if (InNoCass == 3)
            {

                label19.Show();
                panel5.Show();
                label21.Show();
                panel6.Show();
                label25.Show();
                panel7.Show();
                label30.Hide();
                panel8.Hide();

                // Cassette 4 Initialization
               
                comboBox15.Text = "";
                comboBox14.Text = "0";
                comboBox13.Text = "0";

            }
            if (InNoCass == 2)
            {
                label19.Show();
                panel5.Show();
                label21.Show();
                panel6.Show();
                label25.Hide();
                panel7.Hide();
                label30.Hide();
                panel8.Hide();

                // Cassette 3 Initialization
               
                comboBox11.Text = "";
                comboBox10.Text = "0";
                comboBox9.Text = "0";

                // Cassette 4 Initialization
               
                comboBox15.Text = "";
                comboBox14.Text = "0";
                comboBox13.Text = "0";
            }
            if (InNoCass == 1)
            {
                label19.Show();
                panel5.Show();
                label21.Hide();
                panel6.Hide();
                label25.Hide();
                panel7.Hide();
                label30.Hide();
                panel8.Hide();

                // Cassette 2 Initialization
                
                comboBox7.Text = "";
                comboBox6.Text = "0";
                comboBox5.Text = "0";

                // Cassette 3 Initialization
                
                comboBox11.Text = "";
                comboBox10.Text = "0";
                comboBox9.Text = "0";

                // Cassette 4 Initialization
                
                comboBox15.Text = "";
                comboBox14.Text = "0";
                comboBox13.Text = "0";
            }

        }

       // MapsForm newForm;

        private void buttonSearchGoogle_Click(object sender, EventArgs e)
        {
            string SeqNoURL = ConfigurationManager.AppSettings["RRDMMapsGeoQueryURL"];

            try
            {
                if (WActType == 1) // View 
                {
                    //*********************
                    // Use the URl to show
                    //*********************

                    RRDMAtmsClass Ac = new RRDMAtmsClass();
                    Ac.ReadAtm(WAtmNo);

                    int TempMode = 2;
                    Tl.DeleteTempAtmLocationRecord(WAtmNo, TempMode);

                    Tl.BankId = WOperator;
                    Tl.Mode = 2;

                    Tl.GroupNo = 1;
                    Tl.GroupDesc = "Show ATM " + WAtmNo;

                    Tl.DtTmCreated = DateTime.Now;

                    Tl.Street = Ac.Street;
                    Tl.Town = Ac.Town;
                    Tl.District = Ac.District;
                    Tl.PostalCode = Ac.PostalCode;
                    Tl.Country = Ac.Country;

                    Tl.Latitude = Ac.Latitude;
                    Tl.Longitude = Ac.Longitude;

                    Tl.ColorId = "2";
                    Tl.ColorDesc = "Normal Color";

                    Tl.InsertTempAtmLocationRecord(WAtmNo);

                    Tl.FindTempAtmLocationLastNo(WAtmNo, Tl.Mode);

                    // Format the URL with the query string (ATMSeqNo=#)

                    string QueryURL = SeqNoURL + "?ATMSeqNo=" + Tl.SeqNo.ToString();

                    // Invoke default browser
                    ProcessStartInfo sInfo = new ProcessStartInfo(QueryURL);
                    Process.Start(sInfo);

                }
                else // 2, 3, 4 Change and new => create record 
                {

                    if (WActType == 3 || WActType == 4) // 
                    {
                        if (String.IsNullOrEmpty(textBoxAtmNo.Text))
                        {
                            MessageBox.Show("Insert ATM number please");
                            return;
                        }
                        else
                        {
                            WAtmNo = textBoxAtmNo.Text;
                        }
                    }

                    int TempMode = 1;
                    Tl.DeleteTempAtmLocationRecord(WAtmNo, TempMode);

                    Tl.BankId = WOperator;
                    Tl.Mode = 1;

                    Tl.GroupNo = 0;
                    Tl.GroupDesc = "Find Cordinates for ATM " + WAtmNo;

                    Tl.DtTmCreated = DateTime.Now;

                    Tl.Street = textBox5.Text;
                    Tl.Town = textBoxMunicipalityOrVillage.Text;
                    Tl.District = comboBox27.Text;
                    Tl.PostalCode = textBox7.Text;
                    Tl.Country = comboBox28.Text;

                    if (textBoxLatitude.Text != "")
                    {
                        Tl.Latitude = Convert.ToDouble(textBoxLatitude.Text);
                    }
                    else Tl.Latitude = 0;

                    if (textBoxLongitude.Text != "")
                    {
                        Tl.Longitude = Convert.ToDouble(textBoxLongitude.Text);
                    }
                    else Tl.Longitude = 0;

                    Tl.ColorId = "2";
                    Tl.ColorDesc = "Normal Color";

                    Tl.InsertTempAtmLocationRecord(WAtmNo);

                    Tl.FindTempAtmLocationLastNo(WAtmNo, Tl.Mode);

                    // Format the URL with the query string (ATMSeqNo=#)

                    string QueryURL = SeqNoURL + "?ATMSeqNo=" + Tl.SeqNo.ToString();

                    // Invoke default browser
                    ProcessStartInfo sInfo = new ProcessStartInfo(QueryURL);
                    Process.Start(sInfo);


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("678: Error in processing" + ex.Message);  
            }       
            
        }

        // Refresh 
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            Tl.ReadTempAtmLocationSpecific(WAtmNo);
            if (RecordFound == true & Tl.LocationFound)
            {
                if (Tl.AddressChanged == true)
                {
                    textBox5.Text = Tl.NewStreet;
                    textBoxMunicipalityOrVillage.Text = Tl.NewTown;
                    comboBox27.Text = Tl.NewDistrict;
                    comboBox28.Text = Tl.NewCountry;
                }

                textBoxLatitude.Text = Tl.Latitude.ToString();
                textBoxLongitude.Text = Tl.Longitude.ToString();
            }
            else
            {
                MessageBox.Show("Google Map position not updated yet. "); 
                return; 
            }
        }     
       
        //combo Type Of Replenishement 
        private void comboBox17_SelectedIndexChanged(object sender, EventArgs e)
        {
            // REPLENISHED TYPE   
            Gp.ParamId = "208";

            Gp.ReadParametersSpecificParmAndOccurance(WUserBankId, Gp.ParamId, comboBox17.Text);
            textBox22.Text  = Gp.OccuranceNm; 
        }

        // Display Name of CIT provider 
        private void comboBox22_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string WCitId = comboBox22.Text;

            Us.ReadUsersRecord(WCitId);
            textBox21.Text = Us.UserName;

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void Form65_Load(object sender, EventArgs e)
        {

        }
        // When Selection changes for ATM Maintenance type 
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            string MaintType = comboBox1.Text;

            Gp.ParamId = "301"; // Atm Maintenace Type

            Gp.ReadParametersSpecificId(WUserBankId, Gp.ParamId, MaintType, "", "");

            textBox24.Text = Gp.Amount.ToString("#,##0.00");
             
        }
        // Change Supplier
        private void comboBoxSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            Gp.ParamId = "204";
            Gp.ReadParametersSpecificNm(Gp.ParamId, comboBoxSupplier.Text); 

            // Model 
            Gp.RelatedParmId = "204"; // Supplier
            Gp.RelatedOccuranceId = Gp.OccuranceId ;
            comboBoxModel.DataSource = Gp.GetParamOccurancesRelatedNm(WUserBankId, Gp.RelatedParmId, Gp.RelatedOccuranceId);
            comboBoxModel.DisplayMember = "DisplayValue";
        }
// Finish 
        private void buttonFinishOrCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void buttonMenu5_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel5a.Visible = false;
            panel9.Visible = true;

          
            buttonMenu1.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu1.ForeColor = Color.White;
            buttonMenu2.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu2.ForeColor = Color.White;
            buttonMenu3.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu3.ForeColor = Color.White;
            buttonMenu4.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu4.ForeColor = Color.White;

            buttonMenu5.BackColor = Color.White;
            buttonMenu5.ForeColor = Color.FromArgb(35, 119, 192);
        }

    }
}
