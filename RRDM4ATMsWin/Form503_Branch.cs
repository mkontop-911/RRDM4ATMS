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
    public partial class Form503_Branch : Form
    {
      
        RRDMBank_Branches Bb = new RRDMBank_Branches();
        RRDMGasParameters Gp = new RRDMGasParameters();
   
        RRDMUsersRecords Us = new RRDMUsersRecords();
    
        RRDMTempAtmsLocation Tl = new RRDMTempAtmsLocation();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

     
        bool InternalChange;

        int WSeqNo;

        int WRowIndex;

        string WPrefix; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WOrigin;
        int WMode;

        public Form503_Branch(string InSignedId, int SignRecordNo, string InOperator,
                                                        string InOrigin, int InMode)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WOrigin = InOrigin;
            WMode = InMode;
            // InMode = 1 means is Branches
          
            if (WOperator == "ETHNCY2N")
            {
                WPrefix = WOperator.Substring(0, 3);
                WPrefix = "NBG";
            }
            
            InitializeComponent();

            // Set Working Date 
            RRDMGasParameters Gp = new RRDMGasParameters();

            //RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();
            //Mpa.InsertTransMasterPoolATMs_2_Insert_Manually(WOperator);

            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = WSignedId;

            // Districts
            Gp.ParamId = "226";
            comboBoxDistrict.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxDistrict.DisplayMember = "DisplayValue";

            comboBoxFilter.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxFilter.DisplayMember = "DisplayValue";
            
            // Countries
            Gp.ParamId = "227";
            comboBoxCountry.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxCountry.DisplayMember = "DisplayValue";

        }
        // Load 
        private void Form503_Load(object sender, EventArgs e)
        {
            // SHOW ALL OF THIS comboBoxFilter
            
            string SelectionCriteria = " WHERE Operator='" + WOperator +"'"; 
            Bb.ReadBranchesAtmAndFillTable(WSignedId,SelectionCriteria);

            dataGridView1.DataSource = Bb.BranchesDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                buttonAdd.Hide();
                buttonUpdate.Hide();
                buttonDelete.Hide();
                return;
            }

            dataGridView1.Columns[0].Width = 60; // Seq No
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].Visible = false;

            //dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);

            dataGridView1.Columns[1].Width = 90; // BranchId
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 300; // BranchName
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 20; // CIT that it is not needed. 
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].Visible = false; 

            dataGridView1.Columns[4].Width = 150; // Street
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 150; // Town
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 150; // District
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            buttonAdd.Hide();
            buttonUpdate.Show();
            buttonDelete.Show();

        }
        // On Row Enter

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            if (checkBoxMakeNewBranch.Checked == true)
            {
                InternalChange = true;
                checkBoxMakeNewBranch.Checked = false;
            }
            else
            {
                InternalChange = false;
            }


            WSeqNo = (int)rowSelected.Cells[0].Value;

            Bb.ReadBranchBySeqNo(WSeqNo);

            textBoxBranchId.Text = Bb.BranchId ;
            textBoxBranchName.Text = Bb.BranchName ;
           // textBoxCitId.Text = Bb.CitId; 
            textBoxStreet.Text = Bb.Street ;
            textBoxTownOrVillage.Text = Bb.Town ;
            comboBoxDistrict.Text = Bb.District ;
            textBoxPostalCode.Text = Bb.PostalCode ;
            comboBoxCountry.Text = comboBoxCountry.Text;
            textBoxLatitude.Text = Bb.Latitude.ToString();
            textBoxLongitude.Text = Bb.Longitude.ToString();

            textBoxBranchId.ReadOnly = true; 

            buttonUpdate.Show();
        }
        // ADD
        private void buttonAdd_Click(object sender, EventArgs e)
        {
           

        }

        // UPDATE Branch
        private void buttonUpdate_Click(object sender, EventArgs e)
        {

           
        }

        // DELETE Branch
        private void buttonDelete_Click(object sender, EventArgs e)
        {
           
        }

        // Change Filter 
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WMode == 5)
            {
                return;
            }
            else Form503_Load(this, new EventArgs());
        }
     
   
        private void button52_Click(object sender, EventArgs e)
        {
            FormHelp helpForm = new FormHelp("Branch Definition");
            helpForm.ShowDialog();
        }
     
        // Finish 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void checkBoxMakeNewBranch_CheckedChanged(object sender, EventArgs e)
        {
            
        }
        // Search Google
        int WActType;
        string WBranchId; 
        private void buttonSearchGoogle_Click(object sender, EventArgs e)
        {
            
        }
// Refresh postition
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
           
        }
// Print
        private void buttonPrint_Click(object sender, EventArgs e)
        {

            string P1 = "Branches Details ";

            string P2 = "Second Par";
            string P3 = "Third Par";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R76 Report76 = new Form56R76(P1, P2, P3, P4, P5);
            Report76.Show();
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }
// Add
        private void buttonAdd_2_Click(object sender, EventArgs e)
        {
            // Check if Branch Already Exists
            Bb.ReadBranchByBranchId(textBoxBranchId.Text.Trim());
            if (Bb.RecordFound == true)
            {
                MessageBox.Show("Already Exist Branch Id ! ");
                return;
            }
            Bb.BankId = WOperator;
            Bb.BranchId = textBoxBranchId.Text.Trim();
            Bb.BranchName = textBoxBranchName.Text;
            Bb.CitId = "1000";
            Bb.Street = textBoxStreet.Text;
            Bb.Town = textBoxTownOrVillage.Text;
            Bb.District = comboBoxDistrict.Text;
            Bb.PostalCode = textBoxPostalCode.Text;
            Bb.Country = comboBoxCountry.Text;
            Bb.Latitude = Convert.ToDouble("14.53");
            Bb.Longitude = Convert.ToDouble("14.53");
            Bb.UpdatedDate = DateTime.Now;
            Bb.Operator = WOperator;

            // Insert NEXT LOADING 

            WSeqNo = Bb.InsertBranch(textBoxBranchId.Text.Trim());

            ////checkBoxMakeNewVersion.Checked = false;
            //WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form503_Load(this, new EventArgs());

            //dataGridView1.Rows[Mc.PositionInGrid].Selected = true;
            //dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, Mc.PositionInGrid));
        }
        // Update 2
        private void buttonUpdate_2_Click(object sender, EventArgs e)
        {
            Bb.ReadBranchBySeqNo(WSeqNo);

            Bb.BranchId = textBoxBranchId.Text.Trim();
            Bb.BranchName = textBoxBranchName.Text;
            Bb.CitId = "Not_Needed";
            Bb.Street = textBoxStreet.Text;
            Bb.Town = textBoxTownOrVillage.Text;
            Bb.District = comboBoxDistrict.Text;
            Bb.PostalCode = textBoxPostalCode.Text;
            Bb.Country = comboBoxCountry.Text;
            Bb.Latitude = Convert.ToDouble("14.53");
            Bb.Longitude = Convert.ToDouble("14.53");

            Bb.UpdatedDate = DateTime.Now;


            Bb.UpdateBranch(WSeqNo);

            MessageBox.Show("Updating Done!");

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            textBoxMsgBoard.Text = "Branch updated.";

            Form503_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
        }
// DELETE 2 
        private void buttonDelete_2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete(Close) this branch?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                         == DialogResult.Yes)
            {
                Bb.DeleteBranchBySeqNo(WSeqNo);
                Bb.DeleteBranchBySeqNo(WSeqNo);

                MessageBox.Show("Deleted! Branch :" + Bb.BranchId);

                textBoxMsgBoard.Text = "Branch Deleted.";

                int WRowIndex1 = dataGridView1.SelectedRows[0].Index;

                Form503_Load(this, new EventArgs());



                if (WRowIndex1 > 0)
                {
                    WRowIndex1 = WRowIndex1 - 1;
                    dataGridView1.Rows[WRowIndex1].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                }
            }
            else
            {
                return;
            }
        }
// Make new 
        private void checkBox_Make_Branch_2_CheckedChanged(object sender, EventArgs e)
        {
            if (InternalChange == true)
            {
                return;
            }

            if (checkBoxMakeNewBranch.Checked == true)
            {
                buttonAdd.Show();
                textBoxBranchId.ReadOnly = false;
                buttonUpdate.Hide();
                buttonDelete.Hide();

                // Enable

                textBoxBranchId.Text = "";
                // textBoxCitId.Text = "" ; 
                textBoxBranchName.Text = "Fill Data";
                textBoxStreet.Text = "Fill Data";
                textBoxTownOrVillage.Text = "Fill Data";
                //   comboBoxDistrict.Text = "";
                textBoxPostalCode.Text = "Fill Data";
                //  comboBoxCountry.Text = "";
                textBoxLatitude.Text = "Fill Data";
                textBoxLongitude.Text = "Fill Data";

            }
            else
            {
                buttonAdd.Hide();
                buttonUpdate.Show();
                buttonDelete.Show();

                //dataGridView2.Enabled = true;
                int WRowIndex1 = -1;

                if (dataGridView1.Rows.Count > 0)
                {
                    WRowIndex1 = dataGridView1.SelectedRows[0].Index;
                }

                Form503_Load(this, new EventArgs());

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.Rows[WRowIndex1].Selected = true;
                    dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex1));
                }

            }
        }
// Search position 
        private void buttonSearchPos_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Internet not available. Press Add and the Branch will be added! ");
            return;
            string SeqNoURL = ConfigurationManager.AppSettings["RRDMMapsGeoQueryURL"];

            try
            {
                if (WActType == 1) // View 
                {
                    //*********************
                    // Use the URl to show
                    //*********************

                    RRDMAtmsClass Ac = new RRDMAtmsClass();
                    Ac.ReadAtm(WBranchId);
                    int WGroup = 0;
                    int TempMode = 2;
                    Tl.DeleteTempAtmLocationRecord(WBranchId, TempMode, WGroup);

                    Tl.UserId = WSignedId;
                    Tl.AtmNo = WBranchId;

                    Tl.BankId = WOperator;
                    Tl.Mode = 2;

                    Tl.GroupNo = WGroup;
                    Tl.GroupDesc = "Show ATM " + WBranchId;

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
                        if (String.IsNullOrEmpty(textBoxBranchId.Text))
                        {
                            MessageBox.Show("Insert Branch Id please");
                            return;
                        }
                        else
                        {
                            WBranchId = textBoxBranchId.Text.Trim();
                        }
                    }
                    int WGroup = 0;
                    int TempMode = 1;
                    Tl.DeleteTempAtmLocationRecord(WBranchId, TempMode, WGroup);

                    Tl.UserId = WSignedId;
                    Tl.AtmNo = WBranchId;

                    Tl.BankId = WOperator;
                    Tl.Mode = 1;

                    Tl.GroupNo = 0;
                    Tl.GroupDesc = "Find Cordinates for ATM " + WBranchId;

                    Tl.DtTmCreated = DateTime.Now;

                    Tl.Street = textBoxStreet.Text;
                    Tl.Town = textBoxTownOrVillage.Text;
                    Tl.District = comboBoxDistrict.Text;
                    Tl.PostalCode = textBoxPostalCode.Text;
                    Tl.Country = comboBoxCountry.Text;

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
// Refressh 
        private void buttonRefreshPos_Click(object sender, EventArgs e)
        {
            int WMode = 1;
            Tl.ReadTempAtmLocationSpecific(WBranchId, WMode);
            if (Tl.RecordFound == true & Tl.LocationFound)
            {
                if (Tl.AddressChanged == true)
                {
                    //
                    textBoxStreet.Text = Tl.NewStreet;
                    textBoxTownOrVillage.Text = Tl.NewTown;
                    comboBoxDistrict.Text = Tl.NewDistrict;
                    textBoxPostalCode.Text = Tl.NewPostalCode;
                    comboBoxCountry.Text = Tl.NewCountry;
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
// ROW ENTER 
        private void dataGridView2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            if (checkBoxMakeNewBranch.Checked == true)
            {
                InternalChange = true;
                checkBoxMakeNewBranch.Checked = false;
            }
            else
            {
                InternalChange = false;
            }


            WSeqNo = (int)rowSelected.Cells[0].Value;

            Bb.ReadBranchBySeqNo(WSeqNo);

            textBoxBranchId.Text = Bb.BranchId;
            textBoxBranchName.Text = Bb.BranchName;
            // textBoxCitId.Text = Bb.CitId; 
            textBoxStreet.Text = Bb.Street;
            textBoxTownOrVillage.Text = Bb.Town;
            comboBoxDistrict.Text = Bb.District;
            textBoxPostalCode.Text = Bb.PostalCode;
            comboBoxCountry.Text = comboBoxCountry.Text;
            textBoxLatitude.Text = Bb.Latitude.ToString();
            textBoxLongitude.Text = Bb.Longitude.ToString();

            textBoxBranchId.ReadOnly = true;

            buttonUpdate.Show();
        }
    }
}
