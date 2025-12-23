using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using RRDM4ATMs;

// Alecos
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form65 : Form
    {

        int AtmsStatsGroup;
        int AtmsReplGroup;
        int AtmsReconcGroup;

        decimal InMinCash;
        decimal InMaxCash;

        int ReplAlertDays;

        decimal InsurOne;
        decimal InsurTwo;
        decimal InsurThree;
        decimal InsurFour;

        int NumberOfCass;

        double Latitude;
        double Longitude;

        bool ActiveAtm;

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

        string SaveCit;

        int SaveAtmsReplGroup;

        int SaveAtmsReconcGroup;

        DateTime FutureDate = new DateTime(2050, 11, 21);


        RRDMBanks Ba = new RRDMBanks();

        RRDMGroups Ga = new RRDMGroups();

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

        RRDMAtmsCostClass Ap = new RRDMAtmsCostClass();

        RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMAccountsClass Acc = new RRDMAccountsClass();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMComboClass Cc = new RRDMComboClass();

        RRDMTempAtmsLocation Tl = new RRDMTempAtmsLocation();

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

            WAtmNo = InAtmNo;
            WActType = InActType; // 1= View, 2=Update, 3=New Like , 4=New, 5= Inactivate 

            InitializeComponent();

            // ================USER BANK =============================
            Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
            WUserBankId = Us.Operator;
            // ========================================================
            if (WActType == 1) labelMenu.Text = "VIEW ATM " + WAtmNo;
            if (WActType == 2) labelMenu.Text = "UPDATE ATM " + WAtmNo;
            if (WActType == 3) labelMenu.Text = "ADD NEW-LIKE ATM " + WAtmNo;
            if (WActType == 4) labelMenu.Text = "ADD NEW ATM";
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
            Gp.RelatedParmId = "204"; // Model
            Gp.RelatedOccuranceId = "1";
            comboBoxModel.DataSource = Gp.GetArrayParamOccurancesRelatedNm(WUserBankId, Gp.RelatedParmId, Gp.RelatedOccuranceId);
            comboBoxModel.DisplayMember = "DisplayValue";

            // Terminal Type
            Gp.ParamId = "230"; // Terminal Type
            comboBoxTerminalType.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBoxTerminalType.DisplayMember = "DisplayValue";

            // ATM EJournal Layout Type 
            Gp.ParamId = "213"; // Ejournal 
            comboBoxEjournalTypeId.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBoxEjournalTypeId.DisplayMember = "DisplayValue";

            // ATM PHYSICAL CHARACTERISTICS 
            Gp.ParamId = "301"; // Atm Maintenace Type  
            comboBox1.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox1.DisplayMember = "DisplayValue";

            // FIRST CASSETTE

            Gp.ParamId = "201"; // Currencies first Cassette 
            comboBox2.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox2.DisplayMember = "DisplayValue";

            // Face Value first cassette 
            Gp.ParamId = "206";
            comboBox3.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox3.DisplayMember = "DisplayValue";

            // Capacity %  first cassette 
            Gp.ParamId = "207";
            comboBox4.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox4.DisplayMember = "DisplayValue";

            // SECOND CASSETTE 
            Gp.ParamId = "201"; // Currencies Second Cassettes 
            comboBox7.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox7.DisplayMember = "DisplayValue";

            // Face Value  
            Gp.ParamId = "206";
            comboBox6.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox6.DisplayMember = "DisplayValue";

            // Capacity %  
            Gp.ParamId = "207";
            comboBox5.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox5.DisplayMember = "DisplayValue";

            // THIRD CASSETTE 

            Gp.ParamId = "201"; // Currencies third Cassette 
            comboBox11.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox11.DisplayMember = "DisplayValue";

            // Face Value  
            Gp.ParamId = "206";
            comboBox10.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox10.DisplayMember = "DisplayValue";

            // Capacity %  
            Gp.ParamId = "207";
            comboBox9.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox9.DisplayMember = "DisplayValue";

            // FORTH CASSETTE 

            Gp.ParamId = "201"; // Currencies forth Cassettes 
            comboBox_41.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox5.DisplayMember = "DisplayValue";


            // Face Value  
            Gp.ParamId = "206";
            comboBox_42.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox_42.DisplayMember = "DisplayValue";

            // Capacity %  
            Gp.ParamId = "207";
            comboBox_43.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox_43.DisplayMember = "DisplayValue";

            // FIFTH CASSETTE 

            Gp.ParamId = "201"; // Currencies forth Cassettes 
            comboBox_51.DataSource = Gp.GetParamOccurancesNm(WUserBankId);
            comboBox5.DisplayMember = "DisplayValue";


            // Face Value  
            Gp.ParamId = "206";
            comboBox_52.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox_52.DisplayMember = "DisplayValue";

            // Capacity %  
            Gp.ParamId = "207";
            comboBox_53.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox_53.DisplayMember = "DisplayValue";

            // REPLENISHED TYPE   
            Gp.ParamId = "208";
            comboBox17.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
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
            comboBox20.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox20.DisplayMember = "DisplayValue";
            // 
            // CIT PROVIDERS 
            //  
            comboBox22.DataSource = Cc.GetCitIds(WUserBankId);
            comboBox22.DisplayMember = "DisplayValue";

            // Loading Schedule Groups
            RRDMJTMEventSchedules Js = new RRDMJTMEventSchedules();

            string WEventType = "EJournalLoading";

            comboBoxLoadingScheduleID.DataSource = Js.GetScheduleIdsByType(WUserBankId, WEventType);
            comboBoxLoadingScheduleID.DisplayMember = "DisplayValue";

            // NUMBER OF CASSETTES 
            // 
            Gp.ParamId = "302";
            comboBox12.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
            comboBox12.DisplayMember = "DisplayValue";

            // ALERT DAYS 
            // 
            Gp.ParamId = "303";
            comboBox8.DataSource = Gp.GetArrayParamOccurancesIds(WUserBankId);
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
                textBoxLikeAtm.Hide();
                label35.Hide();
                textBoxAtmNo.Text = WAtmNo;

                buttonRefresh.Hide();

                SELECTATMSAndFillControls(WAtmNo);   // Execute Method Select Atm 

            }

            // Show Data for updating 
            if (WActType == 2)
            {
                buttonAdd.Hide();
                button1.Show();
                textBoxLikeAtm.Hide();
                label35.Hide();
                textBoxAtmNo.Text = WAtmNo;

                SELECTATMSAndFillControls(WAtmNo);   // Execute Method Select Atm 

            }

            // Call Needed methods to show Like 
            if (WActType == 3)
            {
                button1.Hide();
                buttonAdd.Show();
                textBoxAtmNo.Text = "";

                SELECTATMSAndFillControls(WAtmNo);   // Execute Method Select Atm 


                textBoxLikeAtm.Text = WAtmNo; // THE LIKE ATM 

                textBox2.Text = "Fill Data";

                textBoxBR_ID.Text = "Fill Data";
                textBoxBR_NM.Text = "Fill Data";
                textBox5.Text = "Fill Data";

                textBox7.Text = "Fill Data"; // Postal Code 

                // Journal Characteristics  
                textBox1.Text = "Fill Data";
                textBox10.Text = "Fill Data";
                textBox20.Text = "Fill Data";
                textBox27.Text = "Fill Data";

                textBoxLatitude.Text = "";
                textBoxLongitude.Text = "";

            }

            // Show fields for NEW  
            if (WActType == 4)
            {
                button1.Hide();
                buttonAdd.Show();
                textBoxLikeAtm.Hide();
                label35.Hide();

            }

            buttonMenu1_Click(this, new EventArgs());
        }

        private void buttonMenu1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel2.Visible = false;
            panel3.Visible = false;
            panel5a.Visible = false;
            panel9.Visible = false;

            buttonMenu1.BackColor = Color.White;
            buttonMenu1.ForeColor = Color.FromArgb(35, 119, 192);
            buttonMenu2.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu2.ForeColor = Color.White;
            buttonMenu3.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu3.ForeColor = Color.White;
            buttonMenu4.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu4.ForeColor = Color.White;
            buttonMenu5.BackColor = Color.FromArgb(35, 119, 192);
            buttonMenu5.ForeColor = Color.White;

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
            // create a connection object
            //using (var scope = new System.Transactions.TransactionScope())
            try
            {

                ErrInResult = false;

                if (WActType == 2 || WActType == 3 || WActType == 4)  // Insert New 
                {
                    if (WActType == 2)
                    {
                        WAtmNo = (textBoxAtmNo.Text).Trim();

                        Ac.ReadAtm(WAtmNo);

                        SaveCit = Ac.CitId;

                        SaveAtmsReconcGroup = Ac.AtmsReconcGroup;

                        SaveAtmsReplGroup = Ac.AtmsReplGroup;
                    }
                    if (WActType == 3 || WActType == 4)
                    {
                        if (string.IsNullOrEmpty(textBoxAtmNo.Text))
                        {
                            MessageBox.Show("Insert ATM number ");
                            return;
                        }
                        else
                        {
                            WAtmNo = textBoxAtmNo.Text.Trim();
                        }

                        Ac.ReadAtm(WAtmNo);
                        if (Ac.RecordFound == true)
                        {
                            MessageBox.Show("THIS ATMNO ALREADY EXIST");
                            return;
                        }
                    }

                    // GOOGLE MAPS VALIDATION
                    int WMode = 1;
                    Tl.ReadTempAtmLocationSpecific(WAtmNo, WMode);
                    if (Tl.RecordFound == true & Tl.LocationFound)
                    {
                        if (Tl.AddressChanged == true)
                        {
                            textBox5.Text = Tl.NewStreet;
                            textBoxMunicipalityOrVillage.Text = Tl.NewTown;
                            comboBox27.Text = Tl.NewDistrict;
                            textBox7.Text = Tl.NewPostalCode;
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

                    // Other VALIDATION OF Input 
                    ErrInResult = false;

                    ValidationOfAtmInput(WAtmNo); // Validation of ATM Input 

                    ValidationOfPhysicalInput(WAtmNo); // Validation of Physical Input 

                    if (ErrInResult == true)
                    {
                        return;
                    }

                    Ac.AtmNo = WAtmNo;
                    Ac.AtmName = textBox2.Text;

                    Ac.BankId = comboBox24.Text;

                    Ac.Branch = textBoxBR_ID.Text;
                    Ac.BranchName = textBoxBR_NM.Text;

                    Ac.Street = textBox5.Text;
                    Ac.Town = textBoxMunicipalityOrVillage.Text;
                    Ac.District = comboBox27.Text;

                    Ac.PostalCode = textBox7.Text;

                    Ac.Country = comboBox28.Text;

                    Ac.CitId = comboBox22.Text;

                    if (textBoxLatitude.Text == "" & textBoxLongitude.Text == "")
                    {
                        Ac.Latitude = Convert.ToDouble("14.53");
                        Ac.Longitude = Convert.ToDouble("14.53");
                        textBoxLatitude.Text = Tl.Latitude.ToString();
                        textBoxLongitude.Text = Tl.Longitude.ToString();
                    }
                    else
                    {
                        Ac.Latitude = Convert.ToDouble(textBoxLatitude.Text);
                        Ac.Longitude = Convert.ToDouble(textBoxLongitude.Text);
                    }
                    // The Atms Stats Group and all other are vales coming from the validation method
                    Ac.AtmsStatsGroup = AtmsStatsGroup;
                    Ac.AtmsReplGroup = AtmsReplGroup;
                    Ac.AtmsReconcGroup = AtmsReconcGroup;

                    RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms();
                    RRDMReconcCategories Rc = new RRDMReconcCategories();
                    RRDMAuthorisationProcess Au = new RRDMAuthorisationProcess();
                    string WUserOfGroup = "";
                    string WReplUser;
                    string WReconUser;

                    Uaa.ReadUsersAccessAtmTableSpecificForAtmNoForRepl(WAtmNo);
                    if (Uaa.RecordFound == true)
                    {
                        WReplUser = Uaa.UserId;
                    }
                    else
                    {
                        WReplUser = "";
                    }

                    Uaa.ReadUsersAccessAtmTableSpecificForAtmNoForRecon(WAtmNo);

                    if (Uaa.RecordFound == true)
                    {
                        WReconUser = Uaa.UserId;
                    }
                    else
                    {
                        WReconUser = "";
                    }

                    // Examine change of ReplGroup
                    // 0 to 104 

                    if ((SaveAtmsReplGroup != AtmsReplGroup) & (SaveAtmsReconcGroup == AtmsReconcGroup))
                    {
                       

                        // Find out the owner of the new group
                        // Find the owner of the group 

                        // FIND not settled Authorisation records for previous owner and delete them 

                        Rc.ReadReconcCategorybyGroupId(SaveAtmsReconcGroup);
                        if (Rc.HasOwner == true)
                        {
                            // Delete all authorisation records for this ATM with Stage < 5 

                            Au.DeleteAuthorisationRecord_ChangeOwner(WAtmNo, Rc.OwnerUserID);

                        }

                        // ACT ONLY IF CIT = 1000
                        //if (Ac.CitId == "1000")
                        //{
                           
                            if (SaveAtmsReplGroup==0)
                            {
                                if (WReplUser!="")
                                {
                                    Uaa.DeleteUsersAtmTableEntryForATM(WAtmNo, WReplUser);
                                }         
                            }
                            
                            if (AtmsReplGroup==0)
                            {
                                // New 
                                // Do nothing 

                            }
                            if (AtmsReplGroup >= 0 || SaveAtmsReplGroup>0)
                            {
                                Uaa.DeleteUsersAtmTableEntryForATM(WAtmNo, WReconUser);

                                // this the new one 
                                Rc.ReadReconcCategorybyGroupId(AtmsReconcGroup);

                                if (Rc.RecordFound == true & Rc.HasOwner == true)
                                {
                                    WUserOfGroup = Rc.OwnerUserID;
                                    Uaa.UserId = WUserOfGroup;
                                    Uaa.AtmNo = WAtmNo;

                                    if (AtmsReplGroup > 0)
                                    {
                                        Uaa.Replenishment = true;
                                    }
                                    else
                                    {
                                        Uaa.Replenishment = false;
                                    }

                                    Uaa.Reconciliation = true;

                                    Uaa.GroupOfAtms = Ac.AtmsReconcGroup;

                                    Uaa.IsCit = false;

                                    Uaa.UseOfGroup = true;

                                    Uaa.UserId = Ac.CitId;
                                    Uaa.BankId = Ac.BankId;

                                    Uaa.DateOfInsert = DateTime.Now;

                                    Uaa.Operator = WOperator;

                                    Uaa.InsertUsersAtmTable(WUserOfGroup, Uaa.AtmNo, Uaa.GroupOfAtms);
                                }
                          

                            }

                        //}

                    }

                    // Change of Reconciliation Group 
                    if (SaveAtmsReconcGroup != AtmsReconcGroup)
                    {


                        // Delete User ATM entry
                        Uaa.DeleteUsersAtmTableEntryForATM(WAtmNo, WReconUser);
                        
                        // Find old Owner
                        Rc.ReadReconcCategorybyGroupId(SaveAtmsReconcGroup);
                        if (Rc.HasOwner == true)
                        {
                            // Delete all authorisation records for this ATM with Stage < 5 
                            //RRDMAuthorisationProcess Au = new RRDMAuthorisationProcess();
                            Au.DeleteAuthorisationRecord_ChangeOwner(WAtmNo, Rc.OwnerUserID);

                        }

                        // this the new one (owner)
                        Rc.ReadReconcCategorybyGroupId(AtmsReconcGroup);

                        if (Rc.HasOwner == true)
                        {
                            WUserOfGroup = Rc.OwnerUserID;
                            Uaa.UserId = WUserOfGroup;
                            Uaa.AtmNo = WAtmNo;

                            Uaa.Reconciliation = true;

                            if (AtmsReplGroup == AtmsReconcGroup)
                            {
                                Uaa.Replenishment = true;
                            }
                            else
                            {
                                Uaa.Replenishment = false;
                            }


                            Uaa.GroupOfAtms = Ac.AtmsReconcGroup;
                            if (Ac.CitId == "1000")
                            {
                                Uaa.IsCit = false;
                            }
                            else
                            {
                                Uaa.IsCit = true;
                            }


                            Uaa.UseOfGroup = true;

                            Uaa.UserId = Ac.CitId;
                            Uaa.BankId = Ac.BankId;

                            Uaa.DateOfInsert = DateTime.Now;

                            Uaa.Operator = WOperator;

                            Uaa.InsertUsersAtmTable(WUserOfGroup, Uaa.AtmNo, Uaa.GroupOfAtms);

                            // Update outstanding with the new group 
                            RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
                            Mpa.UpdateMpaRecordsWithNewGroupNumber(Uaa.AtmNo, Uaa.GroupOfAtms);
                        }
                        //}
                        //else
                        //{

                        //}

                    }


                    Ac.Loby = checkBox1.Checked;
                    Ac.Wall = checkBox2.Checked;
                    Ac.Drive = checkBox3.Checked;

                    Ac.OffSite = checkBox6.Checked;

                    Ac.TypeOfRepl = comboBox17.Text;
                    Ac.CashInType = comboBoxCashInType.Text;

                    Ac.HolidaysVersion = comboBox25.Text;

                    Ac.MatchDatesCateg = comboBox21.Text;

                    Ac.OverEst = Convert.ToInt32(comboBox20.Text);

                    Ac.MinCash = InMinCash;
                    Ac.MaxCash = InMaxCash;
                    Ac.ReplAlertDays = ReplAlertDays;

                    Ac.InsurOne = InsurOne;
                    Ac.InsurTwo = InsurTwo;
                    Ac.InsurThree = InsurThree;
                    Ac.InsurFour = InsurFour;

                    Ac.Supplier = comboBoxSupplier.Text;
                    Ac.Model = comboBoxModel.Text;

                    Ac.TerminalType = comboBoxTerminalType.Text;

                    Ac.EjournalTypeId = comboBoxEjournalTypeId.Text;


                    Ac.NoCassettes = Convert.ToInt32(comboBox12.Text);
                    Ac.DepoReader = checkBox4.Checked;
                    Ac.DepoRecycling = checkBoxDepoRecycling.Checked;
                    Ac.ChequeReader = checkBox5.Checked;

                    Ac.EnvelopDepos = checkBox8.Checked;

                    if (radioButton1.Checked == true)
                    {
                        Ac.ActiveAtm = true;
                    }
                    else
                    {
                        Ac.ActiveAtm = false;
                    }

                    Ac.DepCurNm = comboBox19.Text;

                    Ac.CasNo_11 = 1; // Cassette 1 

                    Ac.CurName_11 = comboBox2.Text;
                    Ac.FaceValue_11 = Convert.ToInt32(comboBox3.Text);
                    Ac.CasCapacity_11 = Convert.ToInt32(comboBox4.Text);

                    Ac.CasNo_12 = 2; // Cassette 2

                    Ac.CurName_12 = comboBox7.Text;
                    Ac.FaceValue_12 = Convert.ToInt32(comboBox6.Text);
                    Ac.CasCapacity_12 = Convert.ToInt32(comboBox5.Text);

                    Ac.CasNo_13 = 3; // Cassette 3

                    Ac.CurName_13 = comboBox11.Text;
                    Ac.FaceValue_13 = Convert.ToInt32(comboBox10.Text);
                    Ac.CasCapacity_13 = Convert.ToInt32(comboBox9.Text);

                    Ac.CasNo_14 = 4; // Cassette 4

                    Ac.CurName_14 = comboBox_41.Text;
                    Ac.FaceValue_14 = Convert.ToInt32(comboBox_42.Text);
                    Ac.CasCapacity_14 = Convert.ToInt32(comboBox_43.Text);

                    Ac.CasNo_15 = 5; // Cassette 5

                    Ac.CurName_15 = comboBox_51.Text;
                    Ac.FaceValue_15 = Convert.ToInt32(comboBox_52.Text);
                    Ac.CasCapacity_15 = Convert.ToInt32(comboBox_53.Text);

                    Ac.Operator = WOperator;

                    if (WActType == 2)
                    {
                        // Update
                        Ac.UpdateATM(WAtmNo);

                        Acc.ReadAccountsAndFillTableForAtmNo(WOperator, WAtmNo);

                        if (Acc.RecordFound == true)
                        {
                            Acc.BranchId = Ac.Branch;
                            Acc.UpdateAccount(Acc.SeqNumber, WOperator);
                        }
                        
                    }

                    if (WActType == 3 || WActType == 4)
                    {
                        // INSERT INSERT INSERT INSERT INSERT
                        Ac.InsertATM(WAtmNo); // Insert ATM record
                    }

                    // Prepare ATMs Main Record

                    if (WActType == 2)
                    {
                        Am.ReadAtmsMainSpecific(WAtmNo);
                    }

                    if (WActType == 3)
                    {
                        Am.ReadAtmsMainSpecific(textBoxLikeAtm.Text);
                        // Initialise for new atm 
                        Am.NextReplDt = FutureDate;
                        Am.EstReplDt = FutureDate;

                    }
                    if (WActType == 4)
                    {
                        // Initialise for new atm 
                        Am.NextReplDt = FutureDate;
                        Am.EstReplDt = FutureDate;

                        Am.LastUpdated = DateTime.Now;
                    }

                    Am.AtmNo = Ac.AtmNo;
                    Am.AtmName = Ac.AtmName;
                    Am.BankId = Ac.BankId;

                    Am.RespBranch = Ac.Branch;
                    Am.BranchName = Ac.BranchName;

                    Am.CitId = Ac.CitId;
                    Am.AtmsReplGroup = Ac.AtmsReplGroup;
                    Am.AtmsReconcGroup = Ac.AtmsReconcGroup;

                    Am.GL_CurrNm1 = Ac.DepCurNm;

                    Am.Operator = Ac.Operator;

                    if (WActType == 2)
                    {
                        Am.UpdateAtmsMain(WAtmNo); // UPDATE MAIN WITH NEW VALUES                                                      //
                    }

                    if (WActType == 3 || WActType == 4)
                    {
                        // INSERT INSERT INSERT INSERT INSERT
                        Am.InsertInAtmsMain(WAtmNo); // Insert AtmMain record 
                    }

                    if (WActType == 2)
                    {
                        Ap.ReadTableATMsCostSpecific(WAtmNo);
                    }
                    //  COST 
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

                    if (WActType == 2)
                    {
                        // Update
                        Ap.UpdateTableATMsCost(WAtmNo, Ac.BankId);

                        //RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms();
                        //RRDMReconcCategories Rc = new RRDMReconcCategories();

                        WUserOfGroup = "";

                        //if (Ac.CitId != SaveCit)
                        //{
                            if (Ac.CitId == "1000")
                            {
                                Uaa.UpdateUsersAtmTableForAtmAndCit(Ac.AtmNo, false);
                            }
                            else
                            {
                                Uaa.UpdateUsersAtmTableForAtmAndCit(Ac.AtmNo, true);
                            }

                         

                    }

                    if (WActType == 3 || WActType == 4)
                    {
                        // ===============Insert 3================
                        Ap.Operator = WOperator;
                        Ap.InsertTableATMsCost(WAtmNo, Ac.BankId);


                        // Insert CIT to ATM Relation 

                        if (Ac.CitId != "1000")
                        {
                            // RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms();

                            // Insert new record

                            Uaa.AtmNo = WAtmNo;

                            Uaa.Replenishment = true;
                            Uaa.Reconciliation = false;

                            Uaa.GroupOfAtms = Ac.AtmsReplGroup;

                            Uaa.IsCit = true;

                            if (Ac.AtmsReplGroup == 0) Uaa.UseOfGroup = false;
                            else Uaa.UseOfGroup = true;

                            Uaa.UserId = Ac.CitId;
                            Uaa.BankId = Ac.BankId;


                            Uaa.DateOfInsert = DateTime.Now;

                            Uaa.Operator = WOperator;


                            Uaa.InsertUsersAtmTable(Uaa.UserId, Uaa.AtmNo, Uaa.GroupOfAtms);
                        }
                    }

                    //  Journal Information 
                    //
                    if (WActType == 2)
                    {
                        // ATM JOURNAL DETAILS
                        Jd.ReadJTMIdentificationDetailsByAtmNo(WAtmNo);

                        Jd.DateLastUpdated = DateTime.Now;
                    }
                    Jd.AtmNo = WAtmNo;
                    Jd.UserId = WSignedId;

                    Jd.LoadingScheduleID = comboBoxLoadingScheduleID.Text;

                    //LOADING 
                    // Last loaded Date not available then insert current date 
                    //

                    DateTime WDate;
                    RRDMJTMEventSchedules Js = new RRDMJTMEventSchedules();

                    string WSelectionCriteria = " WHERE ScheduleID ='" + Jd.LoadingScheduleID + "'";

                    Js.ReadJTMEventSchedulesToGetRecord(WSelectionCriteria);

                    if (Js.EffectiveDateTmFrom > DateTime.Now)
                    {
                        WDate = Js.EffectiveDateTmFrom.AddDays(-1);
                    }
                    else
                    {
                        WDate = DateTime.Now;
                    }
                    DateTime NextLoading = Js.ReadCalculatedNextEventDateTm(WOperator, Jd.LoadingScheduleID,
                                                                               WDate, WDate);
                    textBoxNextLoading.Text = NextLoading.ToString();

                    if (Js.RecordFound == true)
                    {
                        Jd.NextLoadingDtTm = NextLoading;
                    }
                    else
                    {
                        Jd.NextLoadingDtTm = DateTime.Now;
                    }


                    Jd.LoadingScheduleID = comboBoxLoadingScheduleID.Text;
                    Jd.ATMIPAddress = textBox1.Text;
                    Jd.ATMMachineName = textBox10.Text;

                    Jd.ATMWindowsAuth = checkBoxWindowsAuth.Checked;

                    Jd.ATMAccessID = textBoxPhysicalName.Text;
                    Jd.ATMAccessPassword = textBox12.Text;

                    Jd.TypeOfJournal = comboBox26.Text;
                    Jd.SourceFileName = textBox19.Text;

                    Jd.SourceFilePath = textBox20.Text;

                    Jd.DestnFilePath = textBox27.Text;

                    Jd.Operator = WOperator;

                    if (WActType == 2)
                    {

                        Jd.UpdateRecordInJTMIdentificationDetailsByAtmNo(WAtmNo);

                    }

                    if (WActType == 2)
                    {
                        // UPDATE CASSETTES for outstanding Na.

                        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances();

                        Na.UpdateSessionsNotesAndValuesWithCassettes_1(WAtmNo
                             , Ac.FaceValue_11, Ac.FaceValue_12, Ac.FaceValue_13, Ac.FaceValue_14, Ac.FaceValue_15
                            );

                    }
                    // INSERT INSERT INSERT INSERT INSERT
                    if (WActType == 3 || WActType == 4)
                    {
                        // ===============Insert 4================
                        Jd.InsertNewRecordInJTMIdentificationDetails();
                    }

                    if (WActType == 3)
                    {
                        // ==============Copy ACCOUNTS FROM LIKE==========
                        Ac.ReadAtm(textBoxLikeAtm.Text);

                        Acc.CopyAccountsAtmToAtm(Ac.BankId, textBoxLikeAtm.Text, comboBox24.Text, WAtmNo);
                        if (Acc.RecordFound == false)
                        {
                            MessageBox.Show("There were no accounts to copy. After ATM creation  please go and create accounts manually for the added ATM .");
                            MessageBox.Show("ATM added without accounts");
                        }
                        else
                        {

                        }

                        MessageBox.Show("ATM " + WAtmNo + " added");
                        buttonFinishOrCancel.Text = "Finish";
                        buttonAdd.Visible = false;
                    }

                    if (WActType == 2)
                    {
                        MessageBox.Show("ATM : " + WAtmNo + " Updated ");

                        buttonFinishOrCancel.Text = "Finish";
                    }

                    if (WActType == 4)
                    {
                        MessageBox.Show("ATM : " + WAtmNo + " added. Go now and create Accounts!");

                        buttonFinishOrCancel.Text = "Finish";
                        buttonAdd.Visible = false;
                    }


                }
                //COMPLETE SCOPE

                // scope.Complete();
            }
            catch (Exception ex)
            {
                RRDMLog4Net Log = new RRDMLog4Net();

                StringBuilder WParameters = new StringBuilder();

                WParameters.Append("User : ");
                WParameters.Append("NotAssignYet");
                WParameters.Append(Environment.NewLine);

                WParameters.Append("ATMNo : ");
                WParameters.Append("NotDefinedYet");
                WParameters.Append(Environment.NewLine);

                string Logger = "RRDM4Atms";
                string Parameters = WParameters.ToString();

                Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

                System.Windows.Forms.MessageBox.Show("There is a system error with ID = " + Log.ErrorNo.ToString()
                    + " . Application will be aborted! Call controller to take care. ");
                Environment.Exit(0);
            }
            //finally
            //{
            //    scope.Dispose();
            //}

        }

        private void SELECTATMSAndFillControls(string InAtmNo)
        {
            Ac.ReadAtm(InAtmNo);
            if (Ac.RecordFound == true)
            {
                textBox2.Text = Ac.AtmName;
                comboBox24.Text = Ac.BankId;
                if (WActType == 3) // Like case 
                {
                    comboBox24.Text = WUserBankId;
                }

                textBoxBR_ID.Text = Ac.Branch;
                textBoxBR_NM.Text = Ac.BranchName;

                textBox5.Text = Ac.Street;
                textBoxMunicipalityOrVillage.Text = Ac.Town;
                comboBox27.Text = Ac.District;
                textBox7.Text = Ac.PostalCode;
                comboBox28.Text = Ac.Country;

                Latitude = Ac.Latitude;
                textBoxLatitude.Text = Latitude.ToString();

                Longitude = Ac.Longitude;
                textBoxLongitude.Text = Longitude.ToString();

                comboBox16.Text = Ac.AtmsStatsGroup.ToString();
                comboBox18.Text = Ac.AtmsReplGroup.ToString();
                comboBox23.Text = Ac.AtmsReconcGroup.ToString();

                comboBox22.Text = Ac.CitId;

                checkBox1.Checked = Ac.Loby;
                checkBox2.Checked = Ac.Wall;
                checkBox3.Checked = Ac.Drive;
                checkBox6.Checked = Ac.OffSite;

                comboBox17.Text = Ac.TypeOfRepl;

                comboBoxCashInType.Text = Ac.CashInType;

                comboBox25.Text = Ac.HolidaysVersion;

                comboBox21.Text = Ac.MatchDatesCateg;

                comboBox20.Text = Ac.OverEst.ToString();

                textBox9.Text = Ac.MinCash.ToString("#,##0.00");

                textBox8.Text = Ac.MaxCash.ToString("#,##0.00");

                comboBox8.Text = Ac.ReplAlertDays.ToString();

                textBox17.Text = Ac.InsurOne.ToString("#,##0.00");

                textBox14.Text = Ac.InsurTwo.ToString("#,##0.00");

                textBox16.Text = Ac.InsurThree.ToString("#,##0.00");

                textBox18.Text = Ac.InsurFour.ToString("#,##0.00");

                comboBoxSupplier.Text = Ac.Supplier;
                comboBoxModel.Text = Ac.Model;

                comboBoxTerminalType.Text = Ac.TerminalType;

                comboBoxEjournalTypeId.Text = Ac.EjournalTypeId;

                comboBox12.Text = Ac.NoCassettes.ToString();
                checkBox4.Checked = Ac.DepoReader;
                checkBoxDepoRecycling.Checked = Ac.DepoRecycling;
                checkBox5.Checked = Ac.ChequeReader;
                checkBox8.Checked = Ac.EnvelopDepos;

                ActiveAtm = Ac.ActiveAtm;

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

                comboBox19.Text = Ac.DepCurNm;

                comboBox2.Text = Ac.CurName_11;
                comboBox3.Text = Ac.FaceValue_11.ToString();
                comboBox4.Text = Ac.CasCapacity_11.ToString();

                comboBox7.Text = Ac.CurName_12;
                comboBox6.Text = Ac.FaceValue_12.ToString();
                comboBox5.Text = Ac.CasCapacity_12.ToString();

                comboBox11.Text = Ac.CurName_13;
                comboBox10.Text = Ac.FaceValue_13.ToString();
                comboBox9.Text = Ac.CasCapacity_13.ToString();

                comboBox_41.Text = Ac.CurName_14.ToString();
                comboBox_42.Text = Ac.FaceValue_14.ToString();
                comboBox_43.Text = Ac.CasCapacity_14.ToString();

                comboBox_51.Text = Ac.CurName_15.ToString();
                comboBox_52.Text = Ac.FaceValue_15.ToString();
                comboBox_53.Text = Ac.CasCapacity_15.ToString();
            }

            Ap.ReadTableATMsCostSpecific(WAtmNo);
            if (Ap.RecordFound == true)
            {
                dateTimePicker1.Value = Ap.ManifactureDt;
                dateTimePicker2.Value = Ap.PurchaseDt;
                dateTimePicker3.Value = Ap.DueServiceDt;
                dateTimePicker4.Value = Ap.LastServiceDt;

                textBox25.Text = Ap.PurchaseCost.ToString("#,##0.00");
                comboBox1.Text = Ap.MaintenanceCd.ToString();
                textBox24.Text = Ap.AnnualMaint.ToString("#,##0.00");
                textBox23.Text = Ap.CitOnCall.ToString("#,##0.00");
                textBox26.Text = CitAnnual.ToString("#,##0.00");
            }
            
            

            // JOURNAL DETAILS 
            Jd.ReadJTMIdentificationDetailsByAtmNo(WAtmNo);

            textBox1.Text = Jd.ATMIPAddress;

            textBox1.Text = Jd.ATMIPAddress;
            textBox10.Text = Jd.ATMMachineName;

            checkBoxWindowsAuth.Checked = Jd.ATMWindowsAuth;

            textBoxPhysicalName.Text = Jd.ATMAccessID;

            textBox12.Text = Jd.ATMAccessPassword;
            textBox15.Text = Jd.ATMAccessPassword;

            comboBox26.Text = Jd.TypeOfJournal;
            textBox19.Text = Jd.SourceFileName;

            textBox20.Text = Jd.SourceFilePath;

            textBox27.Text = Jd.DestnFilePath;

            comboBoxLoadingScheduleID.Text = Jd.LoadingScheduleID;

            textBoxNextLoading.Text = Jd.NextLoadingDtTm.ToString();

            // SWD
            textBoxCat.Text = Jd.SWDCategory;
            textBoxVersion.Text = Jd.SWVersion;
            textBoxVDate.Text = Jd.SWDate.ToString();

            if (Jd.TypeOfSWD == 1) radioButtonPreProd.Checked = true;
            else radioButtonPreProd.Checked = false;
            if (Jd.TypeOfSWD == 2) radioButtonPilot.Checked = true;
            else radioButtonPilot.Checked = false;
            if (Jd.TypeOfSWD == 3) radioButtonNormalProd.Checked = true;
            else radioButtonNormalProd.Checked = false;

        }

        private void ValidationOfAtmInput(string WAtmNo)
        {
            // VALIDATION OF INPUT NUMERIC FIELDS 

            // AtmsStatsGroup 
            if (int.TryParse(comboBox16.Text, out AtmsStatsGroup))
            {
            }
            else
            {
                MessageBox.Show(comboBox16.Text, "Please enter a valid number for AtmsStatsGroup!");
                ErrInResult = true;
                return;
            }
            //
            //AtmsReplGroup
            //
            if (int.TryParse(comboBox18.Text, out AtmsReplGroup))
            {
            }
            else
            {
                MessageBox.Show(comboBox18.Text, "Please enter a valid number for AtmsReplGroup!");
                ErrInResult = true;
                return;
            }

            // AtmsReconcGroup

            if (int.TryParse(comboBox23.Text, out AtmsReconcGroup))
            {
            }
            else
            {
                MessageBox.Show(comboBox23.Text, "Please enter a valid number for AtmsReconcGroup!");
                ErrInResult = true;
                return;
            }

            string TempCitId = comboBox22.Text;

            RRDMGasParameters Gp = new RRDMGasParameters();
            // Check if ATM is set to Hybrid Branch Replenishment / Reconciliation 
            Gp.OccuranceNm = "NO";
            string ParamId;
            string OccurId;
            ParamId = "823";
            string OccuranceId = "01"; // Short
            Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);
            if (Gp.RecordFound == true)
            {
                if (Gp.OccuranceNm == "YES")
                {
                    // HYBRID IS ACCEPTED
                    //Ac.ReadAtm(WAtmNo);
                    //if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
                    //{
                    //    MessageBox.Show("Not allowed Operation"
                    //        + "With Current functionality ATM must belong to a CIT."
                    //        );
                    //    return;
                    //}
                }
                else
                {
                    // Hybrid Repl and Reconciliation not accepted
                    // Ac.ReadAtm(WAtmNo);
                    if (TempCitId == "1000" & AtmsReplGroup == AtmsReconcGroup & WOperator != "ALPHA_CY")
                    {  if (WOperator == "BCAIEGCX") // BANK DE CAIRE
                        {
                            MessageBox.Show("Not allowed Operation" + Environment.NewLine
                            + "With Current functionality ATM must belong to a CIT." + Environment.NewLine
                              + "Set ATMs Repl Group to zero"
                            );
                            ErrInResult = true;
                            return;
                        }
                        
                    }
                }
            }
            else
            {
                // Hybrid Repl and Reconciliation not accepted

                if ((TempCitId == "1000" & AtmsReplGroup == AtmsReconcGroup))
                {
                    if (AtmsReplGroup == AtmsReconcGroup)
                    {
                        MessageBox.Show("Not allowed Operation" + Environment.NewLine
                      + "With Current functionality ATM must belong to a CIT " + Environment.NewLine
                      + "Set ATMs Repl Group to zero"
                      );

                        ErrInResult = true;
                        return;
                    }
                }
            }

            
                if (AtmsReplGroup != AtmsReconcGroup & TempCitId != "1000")
                {
                    MessageBox.Show("Please enter valid numbers " + Environment.NewLine
                                                 + "For AtmsReconcGroup And Replenishment group" + Environment.NewLine
                                                 + "Both Must Be of the same number"
                                                    );
                    ErrInResult = true;
                    return;
                }
            
            


            if (TempCitId == "1000" & Gp.OccuranceNm == "YES")
            {
                if (AtmsReplGroup == AtmsReconcGroup || AtmsReplGroup == 0 )
                {
                   // OK 
                }
                else
                {
                    MessageBox.Show("Please enter valid numbers " + Environment.NewLine
                                                + "For AtmsReconcGroup And Replenishment group" + Environment.NewLine
                                                + "Both Must Be of the same number or Replenishment group to be zero " + Environment.NewLine
                                                + ""
                                                   );
                    ErrInResult = true;
                    return;
                }
            }

            //RRDMGasParameters Gp = new RRDMGasParameters();
            //// Check if ATM is set to Hybrid Branch Replenishment / Reconciliation 
            //string ParamId;
            //string OccurId;
            //ParamId = "823";
            //string OccuranceId = "01"; // Short
            //Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);
            //if (Gp.RecordFound == true)
            //{
            //    if (Gp.OccuranceNm == "YES")
            //    {
            //        // HYBRID IS ACCEPTED
            //        //Ac.ReadAtm(WAtmNo);
            //        //if (Ac.CitId == "1000" & Ac.AtmsReplGroup == 0)
            //        //{
            //        //    MessageBox.Show("Not allowed Operation"
            //        //        + "With Current functionality ATM must belong to a CIT."
            //        //        );
            //        //    return;
            //        //}
            //    }
            //    else
            //    {
            //        // Hybrid Repl and Reconciliation not accepted
            //       // Ac.ReadAtm(WAtmNo);
            //        if (TempCitId == "1000" & AtmsReplGroup == AtmsReconcGroup)
            //        {
            //            MessageBox.Show("Not allowed Operation"
            //                + "With Current functionality ATM must belong to a CIT."
            //                );
            //            ErrInResult = true;
            //            return;
            //        }
            //    }
            //}
            //else
            //{
            //    // Hybrid Repl and Reconciliation not accepted
             
            //    if ((TempCitId == "1000" & AtmsReplGroup == AtmsReconcGroup))
            //    {
            //        if (AtmsReplGroup == AtmsReconcGroup)
            //        {
            //            MessageBox.Show("Not allowed Operation"
            //          + "With Current functionality ATM must belong to a CIT "
            //          + "Set ATMs Repl Group to zero"
            //          );
                        
            //              ErrInResult = true;
            //        return;
            //        }
                  
            //    }
            //}

            //
            // min cash 
            //
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
            if (NumberOfCass > 0)
            {
                // Check that total percentages = 100 
                int CasPerc1 = int.Parse(comboBox4.Text);
                int CasPerc2 = int.Parse(comboBox5.Text);
                int CasPerc3 = int.Parse(comboBox9.Text);
                int CasPerc4 = int.Parse(comboBox_43.Text);
                int CasPerc5 = int.Parse(comboBox_53.Text);

                int Total = CasPerc1 + CasPerc2 + CasPerc3 + CasPerc4 + CasPerc5;

                if (Total != 100)
                {
                    MessageBox.Show("Total cassette percentages must be equal to 100");
                    ErrInResult = true;
                    return;
                }
            }

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
            if (textBox12.Text != textBox15.Text)
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

            if (NumberOfCass > 5)
            {
                MessageBox.Show(comboBox12.Text, "Please enter a valid number no more than 4!");
                return;
            }

            ShowCassettes(NumberOfCass);
        }

        private void ShowCassettes(int InNoCass)
        {
            if (InNoCass == 5)
            {
                label19.Show();
                panel5.Show();
                label21.Show();
                panel6.Show();
                label25.Show();
                panel7.Show();
                label30.Show();
                panel8.Show();
                label_5.Show();
                panel_5.Show();
            }
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
                label_5.Hide();
                panel_5.Hide();

                // Cassette 5 Initialization

                comboBox_51.Text = "";
                comboBox_52.Text = "0";
                comboBox_53.Text = "0";
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
                label_5.Hide();
                panel_5.Hide();

                // Cassette 4 Initialization

                comboBox_41.Text = "";
                comboBox_42.Text = "0";
                comboBox_43.Text = "0";

                // Cassette 5 Initialization

                comboBox_51.Text = "";
                comboBox_52.Text = "0";
                comboBox_53.Text = "0";

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
                label_5.Hide();
                panel_5.Hide();

                // Cassette 3 Initialization

                comboBox11.Text = "";
                comboBox10.Text = "0";
                comboBox9.Text = "0";

                // Cassette 4 Initialization

                comboBox_41.Text = "";
                comboBox_42.Text = "0";
                comboBox_43.Text = "0";

                // Cassette 5 Initialization

                comboBox_51.Text = "";
                comboBox_52.Text = "0";
                comboBox_53.Text = "0";
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
                label_5.Hide();
                panel_5.Hide();

                // Cassette 2 Initialization

                comboBox7.Text = "";
                comboBox6.Text = "0";
                comboBox5.Text = "0";

                // Cassette 3 Initialization

                comboBox11.Text = "";
                comboBox10.Text = "0";
                comboBox9.Text = "0";

                // Cassette 4 Initialization

                comboBox_41.Text = "";
                comboBox_42.Text = "0";
                comboBox_43.Text = "0";

                // Cassette 5 Initialization

                comboBox_51.Text = "";
                comboBox_52.Text = "0";
                comboBox_53.Text = "0";
            }

            if (InNoCass == 0)
            {
                label19.Hide();
                panel5.Hide();
                label21.Hide();
                panel6.Hide();
                label25.Hide();
                panel7.Hide();
                label30.Hide();
                panel8.Hide();
                label_5.Hide();
                panel_5.Hide();

                // Cassette 1 Initialization

                comboBox2.Text = "";
                comboBox3.Text = "0";
                comboBox4.Text = "0";

                // Cassette 2 Initialization

                comboBox7.Text = "";
                comboBox6.Text = "0";
                comboBox5.Text = "0";

                // Cassette 3 Initialization

                comboBox11.Text = "";
                comboBox10.Text = "0";
                comboBox9.Text = "0";

                // Cassette 4 Initialization

                comboBox_41.Text = "";
                comboBox_42.Text = "0";
                comboBox_43.Text = "0";

                // Cassette 5 Initialization

                comboBox_51.Text = "";
                comboBox_52.Text = "0";
                comboBox_53.Text = "0";
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
                    int WGroup = 0;
                    int TempMode = 2;
                    Tl.DeleteTempAtmLocationRecord(WAtmNo, TempMode, WGroup);

                    Tl.UserId = WSignedId;
                    Tl.AtmNo = WAtmNo;

                    Tl.BankId = WOperator;
                    Tl.Mode = 2;

                    Tl.GroupNo = WGroup;
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

                    Tl.SeqNo = Tl.InsertTempAtmLocationRecord();


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
                    int WGroup = 0;
                    int TempMode = 1;
                    Tl.DeleteTempAtmLocationRecord(WAtmNo, TempMode, WGroup);

                    Tl.UserId = WSignedId;
                    Tl.AtmNo = WAtmNo;

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

                    Tl.SeqNo = Tl.InsertTempAtmLocationRecord();

                    //Tl.FindTempAtmLocationLastNo(WAtmNo, Tl.Mode);

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
            int WMode = 1;
            Tl.ReadTempAtmLocationSpecific(WAtmNo, WMode);
            if (Tl.RecordFound == true & Tl.LocationFound)
            {
                if (Tl.AddressChanged == true)
                {
                    textBox5.Text = Tl.NewStreet;
                    textBoxMunicipalityOrVillage.Text = Tl.NewTown;
                    comboBox27.Text = Tl.NewDistrict;
                    textBox7.Text = Tl.NewPostalCode;
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
        // Reconciliation Group 
        private void comboBox23_SelectedIndexChanged(object sender, EventArgs e)
        {
            RRDMGroups Gr = new RRDMGroups();

            if (int.TryParse(comboBox23.Text, out AtmsReconcGroup))
            {
            }
            else
            {

            }

            Gr.ReadGroup(AtmsReconcGroup);

            textBox13.Text = Gr.Description;
        }

        //combo Type Of Replenishement 
        private void comboBox17_SelectedIndexChanged(object sender, EventArgs e)
        {
            // REPLENISHED TYPE   
            Gp.ParamId = "208";

            Gp.ReadParametersSpecificParmAndOccurance(WUserBankId, Gp.ParamId, comboBox17.Text, "", "");
            textBox22.Text = Gp.OccuranceNm;
        }

        // Display Name of CIT provider 
        private void comboBox22_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string WCitId = comboBox22.Text;

            Us.ReadUsersRecord(WCitId);
            textBox6.Text = Us.UserName;

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
            Gp.ReadParametersSpecificNm(WOperator, Gp.ParamId, comboBoxSupplier.Text);

            // Model 
            Gp.RelatedParmId = "204"; // Supplier
            Gp.RelatedOccuranceId = Gp.OccuranceId;
            comboBoxModel.DataSource = Gp.GetArrayParamOccurancesRelatedNm(WUserBankId, Gp.RelatedParmId, Gp.RelatedOccuranceId);
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
