using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using RRDM4ATMs;

// Alecos
using System.Diagnostics;
using RRDM4ATMsClasses;

namespace RRDM4ATMsWin
{
    public partial class Form503_Maker_Checker : Form
    {

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMGasParameters Gp = new RRDMGasParameters();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool WActiveDirectory; 

        Bitmap SCREENinitial;

        int AuditTrailUniqueID = 0;

        bool InternalChange;

        string WUserId; 

        int WSeqNo;

        int WRowIndex;

        string WPrefix; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WOrigin;
        int WMode;

        public Form503_Maker_Checker(string InSignedId, int SignRecordNo, string InOperator,
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

            //**********************************************************
            //********** ACTIVE DIRECTORY ******************************
            //************* START **************************************
            //**********************************************************

            string ParId = "264";
            string OccurId = "1";
            //TEST
            //RRDMGasParameters Gp = new RRDMGasParameters();
            //WOperator = "ETHNCY2N"; 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

            //Gp.OccuranceNm = "YES";
            // AdDomainName

            if (Gp.OccuranceNm == "YES") // Active directory needed
            {
                // Do not create password. 

                WActiveDirectory = true;

               // buttonCheckAD.Show();
            }
            else
            {
                WActiveDirectory = false;
               // buttonCheckAD.Hide();
            }

         
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;
            labelUserId.Text = WSignedId;

        }
        // Load 
        private void Form503_Load(object sender, EventArgs e)
        {
            // SHOW ALL OF THIS comboBoxFilter
            labelStep1.Text = "Users for Bank/Operator";
            string filter = "Operator = '" + WOperator + "' AND  UserType = 'Employee' ";
            string S_Application = ""; 

            Us.ReadUsersAndFillDataTable(WSignedId, WOperator, filter, S_Application); // Read User table 

            ShowGridUsers();

            
            buttonUpdate.Show();
          

        }
        // On Row Enter
        
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENinitial = memoryImage;

            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
           
            WUserId = (string)rowSelected.Cells[0].Value;

            Us.ReadUsersRecord(WUserId); 

            textBoxUserId.Text = Us.UserId ;
            textBoxUserName.Text = Us.UserName ;

            // Read the tail
            Us.ReadUsersRecord_Tail_Info(WUserId); 

            if (Us.User_Is_Maker == true)
            {
                radioButtonMaker.Checked = true;
               
                radioButtonChecker.Checked = false;
               // radioButtonNonOfThetwo.Checked = false;

                label3.Show();
                textBoxDateOfAssignment.Show();
                textBoxDateOfAssignment.Text = Us.DtTimeOfAssigment.ToString();
            }
            if (Us.User_Is_Checker == true)
            {
                radioButtonChecker.Checked = true;

                radioButtonMaker.Checked = false;
               // radioButtonNonOfThetwo.Checked = false;

                label3.Show();
                textBoxDateOfAssignment.Show();
                textBoxDateOfAssignment.Text = Us.DtTimeOfAssigment.ToString(); 
            }
            if (Us.User_Is_Maker == false & Us.User_Is_Checker == false)
            {
               // radioButtonNonOfThetwo.Checked = true;

                label3.Hide();
                textBoxDateOfAssignment.Hide(); 
            }

            // textBoxCitId.Text = Bb.CitId; 

            textBoxUserId.ReadOnly = true; 

            buttonUpdate.Show();
        }

        //******************
        // SHOW GRID dataGridView1
        //******************
        private void ShowGridUsers()
        {

            dataGridView1.DataSource = Us.UsersInDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                return;
            }

            dataGridView1.Columns[0].Width = 120; // User Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[1].Width = 170; // User Name
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 100; //  email 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);

            dataGridView1.Columns[3].Width = 100; // Mobile
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Width = 80; // date Open
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 50; // User Type
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 50; // Cit Id 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }


        // UPDATE user 
        string WMessage; 
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (radioButtonMaker.Checked == true)
            {
                Us.User_Is_Maker = true;
                Us.DtTimeOfAssigment = DateTime.Now;

                Us.User_Is_Checker = false;

                WMessage = WUserId + "..Became Maker manage Users"; 
            }
            if (radioButtonChecker.Checked == true)
            {
                Us.User_Is_Checker = true;
                Us.DtTimeOfAssigment = DateTime.Now;

                Us.User_Is_Maker = false;

                WMessage = WUserId + "..Became Checker manage Users";
            }

            //if (radioButtonNonOfThetwo.Checked == true)
            //{
            //    Us.User_Is_Maker = false;
            //    Us.User_Is_Checker = false;
            //    Us.DtTimeOfAssigment = NullPastDate;

            //    WMessage = WUserId + "..Became no Maker or Checker manage Users";
            //}
            
            Us.UpdateTail(WUserId);

            //AUDIT TRAIL 
            //AUDIT TRAIL 
            string AuditCategory = "Admin_Work";
            string AuditSubCategory = "Create_Maker_Checker";
            string AuditAction = "Update";
            string Message = WMessage;
            GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);

            MessageBox.Show("Updating Done!"+Environment.NewLine
                + Message
                );

            WRowIndex = dataGridView1.SelectedRows[0].Index;

            int scrollPosition = dataGridView1.FirstDisplayedScrollingRowIndex;

            textBoxMsgBoard.Text = "User updated.";

            Form503_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            dataGridView1.FirstDisplayedScrollingRowIndex = scrollPosition;
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

        //AUDIT TRAIL : GET IMAGE AND INSERT IT IN AUDIT TRAIL 
        private void GetMainBodyImageAndStoreIt(string InCategory, string InSubCategory,
            string InTypeOfChange, string InUser, string Message)
        {

            Bitmap SCREENb;
            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENb = memoryImage;

            RRDM_AuditTrailClass_NEW At = new RRDM_AuditTrailClass_NEW();
            //AuditTrailUniqueID = 6; 
            if (AuditTrailUniqueID.Equals(0))
            {
                AuditTrailUniqueID = At.InsertRecord(InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
                AuditTrailUniqueID = 0 ; 
            }
            else
            {
               At.UpdateRecord(AuditTrailUniqueID, InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
            }

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

// Add a user
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxUserId.Text =="")
            {
                MessageBox.Show("Please Enter User Id");
                return; 
            }
            if (textBoxUserName.Text == "")
            {
                MessageBox.Show("Please Enter User Name");
                return;
            }

            // Read User - If 1 it doesnt exist if 2 exist 
            Us.ReadIfExist(WOperator, Us.UserId);

            if (Us.RecordFound == true)
            {
                MessageBox.Show("User already Exist");
                return;
            }
            // 
            if (WActiveDirectory == true)
            {
                // THIS IS TRUE WHEN IN PRODUCTION 
                // 
                RRDMActiveDirectoryHelper Ah = new RRDMActiveDirectoryHelper();
                bool IsInActive = Ah.isUserInAD(Us.UserId);

                if (IsInActive == false)
                {
                    MessageBox.Show("User is not in Active Directory" + Environment.NewLine
                         + "Open the user in Active and then try again"

                         )
                        ;
                    return;
                }
            }

            // FILL IN THE DATA FOR INSERT 


        }

            
    }
}
