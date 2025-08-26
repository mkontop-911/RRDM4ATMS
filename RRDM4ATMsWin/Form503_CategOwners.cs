using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form503_CategOwners : Form
    {
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
        RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles(); 
        //RRDMMatchingCategories Mc = new RRDMMatchingCategories();
        RRDMReconcCategories Rc = new RRDMReconcCategories(); 
        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        RRDMNVReconcCategoriesSessions NVRcs = new RRDMNVReconcCategoriesSessions();
        RRDMGasParameters Gp = new RRDMGasParameters();

        bool Current_HasOwner; 
        string WApplication;

        string WCurrentOwnerId; 

        string WCurrentSessionOwner;

        bool Is_Maker; 

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCategoryId;

        string W_Application;
        int WMode;
        string WNewOwner; 

        public Form503_CategOwners(string InSignedId, int InSignRecordNo, string InOperator,
                           string InCategoryId, string In_Application, int InMode, string InOwner)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WCategoryId = InCategoryId;

            W_Application = In_Application;

            WMode = InMode;

            WNewOwner = InOwner; 
            // InMode = 1 from Matching Category ( During its definition in Form503)
            // InMode = 2 from Work Alocation Category ATM
            // InMode = 3 from Work Alocation ITMX & WReconcCycleNo > 0 
            // InMode = 4 from Work Alocation  NOSTRO & WReconcCycleNo > 0 

            InitializeComponent();

            if (WNewOwner != "")
            {
                // Means came from Maker 
                Is_Maker = true; 
            }

            Usi.ReadSignedActivityByKey(WSignRecordNo);
            WApplication = Usi.SignInApplication;
            // FIND Current Owner 
            Rc.ReadReconcCategorybyCategId(WOperator, WCategoryId);
            Current_HasOwner = Rc.HasOwner; 
            WCurrentSessionOwner = Rc.OwnerUserID; 

            // Reason of assignment 
            Gp.ParamId = "453";
            comboBoxReason.DataSource = Gp.GetParamOccurancesNm(WOperator);
            comboBoxReason.DisplayMember = "DisplayValue";

            //comboBoxReason.Items.Add("Select Reason"); // 
            //comboBoxReason.Items.Add("Specialist In the area"); // 
            //comboBoxReason.Items.Add("Work Distribution"); // 
            //comboBoxReason.Items.Add("Old Owner not available"); // 

            comboBoxReason.Text = "Select Reason";
            textBoxMessage.Text = "Select Reconc Officer to Assign";

        }
        // On Load Form 

        private void Form503_CategOwners_Load(object sender, EventArgs e)
        {

            if (WMode == 1)
            {
                Rc.ReadReconcCategorybyCategId(WOperator, WCategoryId);
                // Find Categories details 
                //Mc.ReadMatchingCategorybyCategId(WOperator, WCategoryId);

                labelCategId.Text = "Category Id: " + WCategoryId;
                WCurrentOwnerId = Rc.OwnerUserID;

                if (Rc.HasOwner == true)
                {
                    Us.ReadUsersRecord(Rc.OwnerUserID);
                    
                    labelCurrentOwnerId.Text = "Current Owner Id:   " + Us.UserId;
                    labelCuurentOwnerNm.Text = "Current Owner Name: " + Us.UserName;
                }
                else
                {
                    labelCurrentOwnerId.Text = "Category Has no Specific Owner yet. ";
                    labelCuurentOwnerNm.Hide();
                }
            }

            if (WMode == 2 || WMode == 3)
            {
                // Find Categories details 

                //  Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WReconcCycleNo);
                int WReconcCycleNo = 0; 

                Rcs.ReadReconcCategoriesSessionsSpecific(WOperator, WCategoryId, WReconcCycleNo);

                labelCategId.Text = "Category Nm: " + Rcs.CategoryName;

                WCurrentOwnerId = Rcs.OwnerUserID; 

                if (Rcs.OwnerUserID != "")
                {
                    Us.ReadUsersRecord(Rcs.OwnerUserID);

                    labelCurrentOwnerId.Text = "Current Owner Id:   " + Us.UserId;
                   
                    labelCuurentOwnerNm.Text = "Current Owner Name: " + Us.UserName;
                }
                else
                {
                    labelCurrentOwnerId.Text = "Category Has no Specific Owner yet. ";
                    labelCuurentOwnerNm.Hide();
                }
            }

            if (WMode == 4)
            {
                int WReconcCycleNo = 0; 
                NVRcs.ReadNVReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WReconcCycleNo);

                labelCategId.Text = "Category Nm: " + NVRcs.CategoryName;

                if (NVRcs.OwnerId != "")
                {
                    Us.ReadUsersRecord(NVRcs.OwnerId);

                    labelCurrentOwnerId.Text = "Current Owner Id:   " + Us.UserId;
                    labelCuurentOwnerNm.Text = "Current Owner Name: " + Us.UserName;
                }
                else
                {
                    labelCurrentOwnerId.Text = "Category Has no Specific Owner yet. ";
                    labelCuurentOwnerNm.Hide();
                }
            }

            int Mode = 4; // Reconciliation officers

            string SelectionCriteria = " WHERE Application ='" + WApplication + "' AND ReconcOfficer = 1 ";
            Usr.ReadUsersVsApplicationsVsRolesAndFillTable(SelectionCriteria, Mode, "");

            dataGridView1.DataSource = Usr.UsersVsApplicationsVsRolesDataTable.DefaultView;

            dataGridView1.Columns[0].Width = 70; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = 120; // UserId
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[2].Width = 90; // Application
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 60; // SecLevel
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[4].Width = 60; // RoleName
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[5].Width = 70; // Authoriser
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[6].Width = 70; // DisputeOfficer
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

          //  Us.ReadReconcOfficers(WOperator);

           
        }

        // ON ROW ENTER 
        string WSelectedOwner; 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WSelectedOwner = (string)rowSelected.Cells[1].Value;

            Us.ReadUsersRecord(WSelectedOwner);

            labelOfficerId.Text = "Officer Id   : " + Us.UserId;
            labelOfficerName.Text = "Officer Name : " + Us.UserName;
        }
        // Assign New Owner
        private void buttonAssign_Click(object sender, EventArgs e)
        {
            // Validation

            if (Is_Maker == true)
            {
                if (WSelectedOwner != WNewOwner)
                {
                    MessageBox.Show("You are Allowed to Select only the:.."+ WNewOwner);
                    return;
                }
            }

            if (WMode == 1 & WCurrentSessionOwner == WSelectedOwner)
            {
                
                MessageBox.Show("You are Assigning the same officer! Not allowed ");
                return;
            }

            if ((WMode == 2 || WMode == 3) & Rcs.OwnerUserID == WSelectedOwner)
            {
                MessageBox.Show("You are Assigning the same officer! Not allowed ");
                return;
            }

            if (WMode == 4 & NVRcs.OwnerId == WSelectedOwner)
            {
                MessageBox.Show("You are Assigning the same officer! Not allowed ");
                return;
            }

            if (comboBoxReason.Text == "Select Reason")
            {
                MessageBox.Show("Please enter reason of assignment");
                return;
            }
            // Update Category with Owner
            if (WMode == 1)
            {
                // Delete Outstanding Authorisations
                RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
                int Temp = Ap.DeleteAuthorisationRecord_ForSpecificUserAndStageLessThan5(WCurrentOwnerId, WCategoryId);
                MessageBox.Show("The Previous Owner Had Openned Authorisation Records"+ Environment.NewLine
                    + "These were now deleted" + Environment.NewLine
                    + "Number deleted..:" + Temp.ToString() + Environment.NewLine
                    + "The new owner will take care for them" + Environment.NewLine
                    );
                Rc.HasOwner = true;
                Rc.OwnerUserID = WSelectedOwner;
                Rc.UpdateReconcCategory(WOperator, Rc.CategoryId);
                Rcs.UpdateReconcCategorySessionWithReconWithOwner(WCategoryId, WSelectedOwner);
            }


            if (WMode == 2 || WMode == 3)
            {
            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            int Temp = Ap.DeleteAuthorisationRecord_ForSpecificUserAndStageLessThan5(WCurrentOwnerId, WCategoryId);
                MessageBox.Show("The Previous Owner Had Openned Authorisation Records" + Environment.NewLine
                    + "These were now deleted" + Environment.NewLine
                    + "Number deleted..:"+ Temp.ToString() + Environment.NewLine
                    + "The new owner will take care for them" + Environment.NewLine
                    );
                Rc.HasOwner = true;
            Rc.OwnerUserID = WSelectedOwner;
            Rc.UpdateReconcCategory(WOperator, Rc.CategoryId);
            Rcs.UpdateReconcCategorySessionWithReconWithOwner(WCategoryId, WSelectedOwner);
            
            }

            if (WMode == 4)
            {
                int WReconcCycleNo = 0;

                // NVRcs.OwnerId = WSelectedOwner;
                NVRcs.UpdateReconcCategorySessionWithReconWithOwner(WCategoryId, WReconcCycleNo, WSelectedOwner);
            }

            MessageBox.Show("Officer Assigned "+ Environment.NewLine
                             + "An Email Has been send to alert new owner" 
                             );

            int WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form503_CategOwners_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            textBoxMessage.Text = "Officer Assigned";
        }

        // DeAssign
        private void buttonDeAssign_Click(object sender, EventArgs e)
        {
            // Validation

            if (comboBoxReason.Text == "Select Reason")
            {
                MessageBox.Show("Please enter reason of de assignment");
                return;
            }
            // Update  with Owner = ""
            if (WMode == 1)
            {
                Rc.HasOwner = Current_HasOwner;
                Rc.OwnerUserID = WCurrentSessionOwner;
                Rc.UpdateReconcCategory(WOperator, Rc.CategoryId);
            }


            if (WMode == 2 || WMode == 3)

            {
                Rc.HasOwner = Current_HasOwner;
                Rc.OwnerUserID = WCurrentSessionOwner;
                Rc.UpdateReconcCategory(WOperator, Rc.CategoryId);
                
                Rcs.UpdateReconcCategorySessionWithReconWithOwner(WCategoryId,  WCurrentSessionOwner);
            }

            if (WMode == 4)
            {
                NVRcs.OwnerId = "";
                int WReconcCycleNo = 0;

                NVRcs.UpdateReconcCategorySessionWithReconWithOwner(WCategoryId, WReconcCycleNo, NVRcs.OwnerId);
            }


            MessageBox.Show("Officer DeAssigned");

            int WRowIndex = dataGridView1.SelectedRows[0].Index;

            Form503_CategOwners_Load(this, new EventArgs());

            dataGridView1.Rows[WRowIndex].Selected = true;
            dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRowIndex));

            textBoxMessage.Text = "Officer DeAssigned";
        }

        // Finish 
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


    }
}
