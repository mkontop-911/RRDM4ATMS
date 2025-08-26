using System;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class SWD_UCForm271a : UserControl
    {
        //RRDMSWDCategories Sc = new RRDMSWDCategories();
        RRDMSWDPackages Sp = new RRDMSWDPackages();
        RRDMSWDPackageFiles Pf = new RRDMSWDPackageFiles();
        RRDMSWDPackagesDistributionSessions Ds = new RRDMSWDPackagesDistributionSessions();

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();

        bool ViewWorkFlow;

        int WSeqNoGrid2;
        int WSeqNoGrid3;
        int WSeqNoGrid4;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        public void SWD_UCForm271aPar(string InSignedId, int SignRecordNo, string InOperator)
        {

            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
           
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }
            else
            {
                if (Usi.ProcessNo == 7) ViewWorkFlow = false;

            }

        }

        // SHOW SCREEN 

        public void SetScreen()
        {
            CheckIfNotes3();

            string Filter2 = " WHERE Operator ='" + WOperator + "'"
                            + " Order by SeqNo DESC";

            Sp.ReadSWDPackagesAndFillTable(Filter2);

            dataGridView2.DataSource = Sp.TableSWDPackages.DefaultView;

            if (dataGridView2.Rows.Count == 0)
            {
                // Create records
                return;
            }
            else
            {

            }

            dataGridView2.Columns[0].Width = 60; // SeqNo
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView2.Columns[1].Width = 120; // PackageId
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[2].Width = 120; // PackageName
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView2.Columns[3].Width = 120; // CreatedDtTm
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;


        }
        // Row ENTER For grid 2
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView2.Rows[e.RowIndex];

            WSeqNoGrid2 = (int)rowSelected.Cells[0].Value;

            Sp.ReadSWDPackagesbySeqNo(WOperator, WSeqNoGrid2);

            textBox11.Text = Sp.PackageId;

            textBoxPackId.Text = Sp.PackageId;
            label11.Text = "FILES OF PACKAGE : " + Sp.PackageId ; 
            textBox6.Text = Sp.BankPackId;
            textBox7.Text = Sp.PackageName;
            textBox8.Text = Sp.PackageDescription;

            checkBox10.Checked = Sp.ForUpdating;
            checkBox9.Checked = Sp.ForFullInstallation;

            checkBox8.Checked = Sp.PutOutOfService;
            checkBox7.Checked = Sp.NeedReboot;
            //
            CheckIfNotes2();
            //
            textBox5.Text = Sp.PriorityOneToFive.ToString();

            labelFilesWithin.Text = "Files for Package : " + Sp.PackageId;

            // Fill For Sessions Panel 
            textBox4.Text = Sp.PackageId;
            textBox3.Text = "15";

            // Load Grid 3 : The files 

            string Filter3 = " WHERE SWDCategoryId ='" + Sp.SWDCategoryId + "'"
                              + " AND PackId ='" + Sp.PackageId + "'";

            Pf.ReadSWDPackageFilesFillTable(Filter3);

            dataGridView3.DataSource = Pf.TableSWDPackageFiles.DefaultView;

            if (dataGridView3.Rows.Count == 0)
            {
                // Create records
                return;
            }
            else
            {

            }

            dataGridView3.Columns[0].Width = 60; // SeqNo
            dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView3.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);

            dataGridView3.Columns[1].Width = 120; // SWDCategory
            dataGridView3.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView3.Columns[1].Visible = false;

            dataGridView3.Columns[2].Width = 100; // PackId
            dataGridView3.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView3.Columns[2].Visible = false;

            dataGridView3.Columns[3].Width = 190; // Directory
            dataGridView3.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView3.Columns[3].Visible = false;

            dataGridView3.Columns[4].Width = 250; // FileIdOrigin
            dataGridView3.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView3.Columns[5].Width = 250; // FileIdDestination
            dataGridView3.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Load grid 4 : The Sessions

            string Filter4 = " WHERE SWDCategoryId ='" + Sp.SWDCategoryId + "'"
                          + " AND PackageId ='" + Sp.PackageId + "' ORDER BY PackDistrSesNo DESC";

            Ds.ReadSWDPackDistrSesAndFillTable(Filter4);

            dataGridView4.DataSource = Ds.TablePackDistrSes.DefaultView;

            if (dataGridView4.Rows.Count == 0)
            {
                // Create records
                return;
            }
            else
            {

            }
        
            dataGridView4.Columns[0].Width = 60; // PackDistrSesNo
            dataGridView4.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.Columns[0].DefaultCellStyle.Font = new Font("Tahoma", 09, FontStyle.Bold);
            dataGridView4.Columns[0].HeaderText = "SesNo";

            dataGridView4.Columns[1].Visible = false; // SWDCategoryId

            dataGridView4.Columns[2].Visible = false; // PackageId

            dataGridView4.Columns[3].Width = 60; // TypeOfDistribution
            dataGridView4.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.Columns[3].HeaderText = "Type";

            dataGridView4.Columns[4].Width = 90; // StartDateTm
            dataGridView4.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView4.Columns[4].Visible = true;

            dataGridView4.Columns[5].Width = 80; // "ATMsForSWD"
            dataGridView4.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.Columns[5].Visible = true;
            dataGridView4.Columns[5].HeaderText = "ToBeDone";

            dataGridView4.Columns[6].Width = 80; // ATMsDone
            dataGridView4.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.Columns[6].Visible = true;
            dataGridView4.Columns[6].HeaderText = "Done";

            dataGridView4.Columns[7].Width = 80; // ATMsNotDone
            dataGridView4.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.Columns[7].Visible = true;
            dataGridView4.Columns[7].HeaderText = "NotDone";
        }
        // Row ENTER For grid 3
        private void dataGridView3_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView3.Rows[e.RowIndex];

            WSeqNoGrid3 = (int)rowSelected.Cells[0].Value; //PackDistrSesNo

            Pf.ReadPackageFilesbySeqNo(WOperator, WSeqNoGrid3);
        }
        // Row ENTER For grid 4
        private void dataGridView4_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView4.Rows[e.RowIndex];

            WSeqNoGrid4 = (int)rowSelected.Cells[0].Value;

            Ds.ReadSWDPackDistrSesbyPackDistrSesNo(WOperator, WSeqNoGrid4);

            // initialise 
            radioButtonPreProduction.Checked = false;
            radioButtonPilot.Checked = false;
            radioButtonFullProduction.Checked = false;
            radioButtonSingle.Checked = false;

            if (Ds.TypeOfDistribution == 1) radioButtonPreProduction.Checked = true;
            if (Ds.TypeOfDistribution == 2) radioButtonPilot.Checked = true;
            if (Ds.TypeOfDistribution == 3) radioButtonFullProduction.Checked = true;
            if (Ds.TypeOfDistribution == 4) radioButtonSingle.Checked = true;

            textBoxAtmNo.Text = Ds.SingleAtmNo;

            dateTimePicker1.Value = Ds.StartDateTm;
            dateTimePicker4.Value = Ds.EndDateTm;

            checkBoxSuccessPrepro.Checked = Ds.SuccessPreProduction;
            checkBoxSuccessPilot.Checked = Ds.SuccessPilot;

            //if (Ds.Approver == "" & ViewWorkFlow == false)
            if (Ds.Approver == "")
            {
                if (Usi.ProcessNo == 7)
                {
                    //buttonPrepare.Hide();
                    buttonUpdate.Show();
                }
                else
                {
                    // 54,55,56 or authorised 

                    //buttonPrepare.Hide();
                    buttonUpdate.Hide();
                }

            
            }
            else
            {
                // This line is authorised 
              
                Usi.ReadSignedActivityByKey(WSignRecordNo);

                if (Usi.ProcessNo == 7 || Usi.ProcessNo == 54)
                {
                    Usi.WFieldNumeric1 = Ds.PackDistrSesNo;
                    Usi.ProcessNo = 54; // View Only 
                    ViewWorkFlow = true;
                }
                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
                // Already Approved 
                labelCompleted.Show();
                //buttonPrepare.Hide();
                buttonUpdate.Hide();

            }

            CheckIfNotes3();
        }
        //Make New Distribution Session
        private void checkBoxMakeNewPackage_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxMakeNewPackage.Checked == true)
            {
             
                Ds.SWDCategoryId = Sp.SWDCategoryId;
                Ds.PackageId = Sp.PackageId;
                Ds.TypeOfDistribution = 0;

                Ds.SingleAtmNo = "";

                //Ds.SingleAtmNo = textBoxAtmNo.Text;

                Ds.ATMsForSWD = 0;
                Ds.ATMsDone = 0;
                Ds.ATMsNotDone = 0;

                Ds.StartDateTm = DateTime.Now; 
                Ds.EndDateTm = DateTime.Now;

                Ds.SuccessPreProduction = false;
                Ds.SuccessPilot = false;

                Ds.Maker = WSignedId;
                Ds.Approver = "";
                Ds.ProcessStage = -1;
                Ds.Operator = WOperator;

                Ds.PackDistrSesNo = Ds.InsertSWDPackDistrSes();

                checkBoxMakeNewPackage.Checked = false;

                int WRow2 = dataGridView2.SelectedRows[0].Index;

                int WRow3 = dataGridView3.SelectedRows[0].Index;

                SetScreen();

                dataGridView2.Rows[WRow2].Selected = true;
                dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));

                dataGridView3.Rows[WRow3].Selected = true;
                dataGridView3_RowEnter(this, new DataGridViewCellEventArgs(1, WRow3));

             
                Usi.ReadSignedActivityByKey(WSignRecordNo);
                Usi.ReconcDifferenceStatus = 0; // No Difference
                Usi.ProcessNo = 7;
                Usi.WFieldNumeric1 = Ds.PackDistrSesNo;

                Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);


                labelCompleted.Hide();

                MessageBox.Show("New Distribution Session with id :" + Ds.PackDistrSesNo.ToString() + Environment.NewLine
                                + " has been created. " + Environment.NewLine
                                + " Update the needed fields and continue to Authorisation");

                ViewWorkFlow = false;
                //buttonPrepare.Show();
                buttonUpdate.Show();

                CheckIfNotes3();
            }
        }
        // Insert
        private void buttonPrepare_Click(object sender, EventArgs e)
        {
            if (radioButtonPreProduction.Checked == false
                & radioButtonPilot.Checked == false
                & radioButtonFullProduction.Checked == false
                & radioButtonSingle.Checked == false
                )
            {
                MessageBox.Show("Please select type of distribution");
                return;
            }
            if (radioButtonSingle.Checked == true)
            {
                if (textBoxAtmNo.Text == "")
                {
                    MessageBox.Show("Please enter Atm No");
                    return;
                }
            }

            if (radioButtonPilot.Checked == true
                & (checkBoxSuccessPrepro.Checked == false || labelNumberNotes3.Text == "0"))
            {
                MessageBox.Show("Please check pre-production success " + Environment.NewLine
                                  + " and include certificate in Notes.");
                return;
            }

            if (radioButtonFullProduction.Checked == true
               & (checkBoxSuccessPilot.Checked == false || labelNumberNotes3.Text == "0"))
            {
                MessageBox.Show("Please check pilot success " + Environment.NewLine
                                  + " and include certificate in Notes.");
                return;
            }

            Ds.StartDateTm = dateTimePicker1.Value;
            Ds.EndDateTm = dateTimePicker4.Value;

            if (Ds.EndDateTm < Ds.StartDateTm)
            {
                MessageBox.Show("End Date should be greatest or equal to Start Date");
                return;
            }

            Ds.SWDCategoryId = Sp.SWDCategoryId;
            Ds.PackageId = textBox4.Text;

            if (radioButtonPreProduction.Checked == true) Ds.TypeOfDistribution = 1;
            if (radioButtonPilot.Checked == true) Ds.TypeOfDistribution = 2;
            if (radioButtonFullProduction.Checked == true) Ds.TypeOfDistribution = 3;
            if (radioButtonSingle.Checked == true) Ds.TypeOfDistribution = 4;

            Ds.SingleAtmNo = textBoxAtmNo.Text;

            Ds.ATMsForSWD = 10;
            Ds.ATMsDone = 0;
            Ds.ATMsNotDone = 0;

            Ds.SuccessPreProduction = checkBoxSuccessPrepro.Checked;
            Ds.SuccessPilot = checkBoxSuccessPilot.Checked;

            Ds.Maker = WSignedId;
            Ds.Approver = "";
            Ds.ProcessStage = -1;
            Ds.Operator = WOperator;

            Ds.PackDistrSesNo = Ds.InsertSWDPackDistrSes();

            int WRow2 = dataGridView2.SelectedRows[0].Index;

            int WRow3 = dataGridView3.SelectedRows[0].Index;

            SetScreen();

            dataGridView2.Rows[WRow2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));

            dataGridView3.Rows[WRow3].Selected = true;
            dataGridView3_RowEnter(this, new DataGridViewCellEventArgs(1, WRow3));

          
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ReconcDifferenceStatus = 1; // No Difference
            Usi.ProcessNo = 7;
            Usi.WFieldNumeric1 = Ds.PackDistrSesNo;

            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);


        }
        // Update
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (Ds.PackDistrSesNo == 0)
            {
                MessageBox.Show("Please create a new session");
                return; 
            }
            Ds.ReadSWDPackDistrSesbyPackDistrSesNo(WOperator, Ds.PackDistrSesNo);

            Ds.SWDCategoryId = Sp.SWDCategoryId;

            if (radioButtonPreProduction.Checked == false
              & radioButtonPilot.Checked == false
              & radioButtonFullProduction.Checked == false
              & radioButtonSingle.Checked == false
              )
            {
                MessageBox.Show("Please select type of distribution");
                return;
            }
            if (radioButtonSingle.Checked == true)
            {
                if (textBoxAtmNo.Text == "")
                {
                    MessageBox.Show("Please enter Atm No");
                    return;
                }
            }

            if (radioButtonPilot.Checked == true
                & (checkBoxSuccessPrepro.Checked == false || labelNumberNotes3.Text == "0"))
            {
                MessageBox.Show("Please check pre-production success " + Environment.NewLine
                                  + " and include certificate in Notes.");
                return;
            }

            if (radioButtonFullProduction.Checked == true
               & (checkBoxSuccessPilot.Checked == false || labelNumberNotes3.Text == "0"))
            {
                MessageBox.Show("Please check pilot success " + Environment.NewLine
                                  + " and include certificate in Notes.");
                return;
            }

            Ds.StartDateTm = dateTimePicker1.Value;
            Ds.EndDateTm = dateTimePicker4.Value;

            if (Ds.EndDateTm < Ds.StartDateTm)
            {
                MessageBox.Show("End Date should be greatest or equal to Start Date");
                return;
            }

            Ds.PackageId = textBox4.Text;

            if (radioButtonPreProduction.Checked == true) Ds.TypeOfDistribution = 1;
            if (radioButtonPilot.Checked == true) Ds.TypeOfDistribution = 2;
            if (radioButtonFullProduction.Checked == true) Ds.TypeOfDistribution = 3;
            if (radioButtonSingle.Checked == true) Ds.TypeOfDistribution = 4;

            Ds.SingleAtmNo = textBoxAtmNo.Text;

            Ds.StartDateTm = dateTimePicker1.Value;
            Ds.EndDateTm = dateTimePicker4.Value;

            Ds.ATMsForSWD = 10;
            Ds.ATMsDone = 0;
            Ds.ATMsNotDone = 0;

            Ds.SuccessPreProduction = checkBoxSuccessPrepro.Checked;
            Ds.SuccessPilot = checkBoxSuccessPilot.Checked;

            Ds.Maker = WSignedId;
            Ds.Approver = "";
            Ds.ProcessStage = -1;

            Ds.UpdateSWDPackDistrSes(WOperator, Ds.PackDistrSesNo);

            MessageBox.Show("Updating done! You can move to authorisation.");

          
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ReconcDifferenceStatus = 1; // No problem
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            int WRow2 = dataGridView2.SelectedRows[0].Index;
            int WRow3 = dataGridView3.SelectedRows[0].Index;
            int WRow4 = dataGridView4.SelectedRows[0].Index;

            SetScreen();

            dataGridView2.Rows[WRow2].Selected = true;
            dataGridView2_RowEnter(this, new DataGridViewCellEventArgs(1, WRow2));

            dataGridView3.Rows[WRow3].Selected = true;
            dataGridView3_RowEnter(this, new DataGridViewCellEventArgs(1, WRow3));

            dataGridView4.Rows[WRow4].Selected = true;
            dataGridView4_RowEnter(this, new DataGridViewCellEventArgs(1, WRow4));

            Usi.ReadSignedActivityByKey(WSignRecordNo);
            Usi.ReconcDifferenceStatus = 1; // No Difference
            Usi.ProcessNo = 7;
            Usi.WFieldNumeric1 = Ds.PackDistrSesNo;
            Usi.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);
        }

        // Check If Notes 
        Form197 NForm197;
        RRDMCaseNotes Cn = new RRDMCaseNotes();
        string WNotesMode;
        string NotesOrder;
        string WNotesSearchP4;
        string WNotesParameter3;
        string WNotesParameter4;
        string NotesSearchP4;
        //
        // NOTES 2
        //
        private void CheckIfNotes2()
        {
            // NOTES 
            NotesOrder = "Descending";
            WNotesParameter4 = "Notes for Pac:" + Sp.PackageId;
            WNotesSearchP4 = "";
            Cn.ReadAllNotes(WNotesParameter4, WSignedId, NotesOrder, WNotesSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
        }

        // Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {
            WNotesMode = "Read"; //At this stage only read
            if (labelNumberNotes2.Text == "0" & WNotesMode == "Read")
            {
                MessageBox.Show("No Notes to read");
                return;
            }

            WNotesParameter3 = "";
            WNotesParameter4 = "Notes for Pac:" + Sp.PackageId;
            NotesSearchP4 = "";

            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WNotesParameter3, WNotesParameter4, WNotesMode, NotesSearchP4);
            NForm197.ShowDialog();

            // NOTES for final comment 
            NotesOrder = "Descending";
            WNotesParameter4 = "Notes for Pac:" + Sp.PackageId;
            WNotesSearchP4 = "";
            Cn.ReadAllNotes(WNotesParameter4, WSignedId, NotesOrder, WNotesSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";

        }
        //
        // NOTES 3
        //
        // Check If Notes 3 
        private void CheckIfNotes3()
        {
            // NOTES for final comment 
            NotesOrder = "Descending";
            WNotesParameter4 = "Notes for DistrVersion:" + Ds.PackDistrSesNo.ToString();
            WNotesSearchP4 = "";
            Cn.ReadAllNotes(WNotesParameter4, WSignedId, NotesOrder, WNotesSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";
        }
        // Notes 3
        private void buttonNotes3_Click(object sender, EventArgs e)
        {
            if (ViewWorkFlow == true) WNotesMode = "Read";
            else WNotesMode = "Update";

            if (labelNumberNotes3.Text == "0" & WNotesMode == "Read")
            {
                MessageBox.Show("No Notes to read");
                return;
            }
            WNotesParameter3 = "";
            WNotesParameter4 = "Notes for DistrVersion:" + Ds.PackDistrSesNo.ToString();
            NotesSearchP4 = "";

            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WNotesParameter3, WNotesParameter4, WNotesMode, NotesSearchP4);
            NForm197.ShowDialog();

            // NOTES for final comment 
            NotesOrder = "Descending";
            WNotesParameter4 = "Notes for DistrVersion:" + Ds.PackDistrSesNo.ToString();
            WNotesSearchP4 = "";
            Cn.ReadAllNotes(WNotesParameter4, WSignedId, NotesOrder, WNotesSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes3.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes3.Text = "0";

        }
        // Single ATM
        private void radioButtonSingle_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonSingle.Checked == true) textBoxAtmNo.Show();
            else textBoxAtmNo.Hide();
        }
// List Of ATMS 
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SWDForm502DashBoardAtms NSWDForm502DashBoardAtms;

            int Type = 0;
            if (radioButtonPreProduction.Checked == true) Type = 1;
            if (radioButtonPilot.Checked == true) Type = 2;
            if (radioButtonFullProduction.Checked == true) Type = 3;
            if (radioButtonSingle.Checked == true) Type = 4;

            if (Type ==4 || Type == 0)
            {
                MessageBox.Show("Please make the right choice");
                return; 
            }

            NSWDForm502DashBoardAtms = new SWDForm502DashBoardAtms(WSignedId, WSignRecordNo, WOperator, Sp.SWDCategoryId, Type);
            NSWDForm502DashBoardAtms.ShowDialog();
        }
// ATMs on MAp
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (HasInternet())
            {
                try
                {
                    // Read the URL from app.config
                    string MainURL = ConfigurationManager.AppSettings["RRDMMapsMainURL"];
                    // Invoke default browser
                    ProcessStartInfo sInfo = new ProcessStartInfo(MainURL);
                    Process.Start(sInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Maps cannot be shown. There might be a problem with Google Maps" + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("There is no Internet Connection");
            }
        }
        private bool HasInternet()
        {
            try
            {
                using (var client = new System.Net.WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }
    }
}
