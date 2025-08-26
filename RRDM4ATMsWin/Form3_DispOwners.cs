using System;
using System.Windows.Forms;
using RRDM4ATMs;


namespace RRDM4ATMsWin
{
    public partial class Form3_DispOwners : Form
    {
        Form3_DispOwnersHistory NForm3_DispOwnersHistory;

        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords(); 
        RRDMDisputesTableClass Di = new RRDMDisputesTableClass();
        RRDMDisputesOwnersHistory Dh = new RRDMDisputesOwnersHistory();

        RRDMUsers_Applications_Roles Usr = new RRDMUsers_Applications_Roles();

        string WApplication; 
        //   string WUserId;
        string WDispOwner; 
        //string WUserName;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        int WDispId;

        public Form3_DispOwners(string InSignedId, int SignRecordNo, string InOperator, int InDispId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
            WDispId = InDispId;
            
            InitializeComponent();

            Usi.ReadSignedActivityByKey(WSignRecordNo);
            WApplication = Usi.SignInApplication; 
        }
        // On Load Form 
        private void Form3DispOwners_Load(object sender, EventArgs e)
        {
            // Find Dispute details 


            Di.ReadDispute(WDispId);

            labelDispId.Text = "Dispute Id: " + WDispId.ToString();

            if (Di.HasOwner == true)
            {
                Us.ReadUsersRecord(Di.OwnerId);

                labelCurrentOwnerId.Text = "Current Owner Id:   " + Us.UserId;
                labelCuurentOwnerNm.Text = "Current Owner Name: " + Us.UserName;
            }
            else
            {
                labelCurrentOwnerId.Text = "Dispute Has no Specific Owner yet. " ;
                labelCuurentOwnerNm.Hide(); 
            }

            comboBox1.Items.Add("Select Reason"); // 
            comboBox1.Items.Add("Specialist In the area"); // 
            comboBox1.Items.Add("Work Distribution"); // 
            comboBox1.Items.Add("Old Owner not available"); // 
          
            comboBox1.Text = "Select Reason";
            textBoxMessage.Text = "Select Dispute Owner to Assign";

            
            int Mode = 3; // Dispute

            string SelectionCriteria = " WHERE Application ='" + WApplication + "' AND DisputeOfficer = 1 ";
            Usr.ReadUsersVsApplicationsVsRolesAndFillTable(SelectionCriteria, Mode, "");
          
            dataGridView1.DataSource = Usr.UsersVsApplicationsVsRolesDataTable.DefaultView;
          
            dataGridView1.Columns[0].Width = 70; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[0].Visible = false; 

            dataGridView1.Columns[1].Width = 80; // UserId
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

        }
        // ON ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WDispOwner = (string)rowSelected.Cells[1].Value;

            Us.ReadUsersRecord(WDispOwner);

            labelOfficerId.Text =   "Officer Id   : " + Us.UserId ;
            labelOfficerName.Text = "Officer Name : " + Us.UserName ; 
        }
// Assign New Owner
        private void buttonAssign_Click(object sender, EventArgs e)
        {
            // Validation

            if (Di.OwnerId == WDispOwner)
            {
                MessageBox.Show("You are Assigning the same officer! Not allowed ");
                return;
            }

            if (comboBox1.Text == "Select Reason")
            {
                MessageBox.Show("Please enter reason of assignment");
                return;
            }

            Dh.ReadDisputeOwnersRecordSpecificDispId(WOperator, WDispId);
            if (Dh.RecordFound == true)
            {
                // Update Record
                Dh.EndDate = DateTime.Now; 
                Dh.CloseDate = DateTime.Now;

                Dh.UpdateDisputeOwnersHistory(WOperator, Dh.SeqNumber); 
            }
            else
            {
            }

            Dh.DispId = WDispId ;
            Dh.StartDate = DateTime.Now;
            Dh.CreatorId = WSignedId;
            Dh.Reason = comboBox1.Text ;
            Dh.HasOwner = true;
            Dh.OwnerId = WDispOwner; 

            Dh.InsertDisputeOwnersHistory(WOperator, WDispId);

            // Update Dispute with Owner
            Di.HasOwner = true; 
            Di.OwnerId = WDispOwner; 
            Di.UpdateDisputeRecord(WDispId); 

            MessageBox.Show("Officer Assigned");

            Form3DispOwners_Load(this, new EventArgs());

            textBoxMessage.Text = "Officer Assigned"; 

        }


// DeAssign
        private void buttonDeAssign_Click(object sender, EventArgs e)
        {
            // Validation

            if (comboBox1.Text == "Select Reason")
            {
                MessageBox.Show("Please enter reason of de assignment");
                return;
            }

            Dh.ReadDisputeOwnersRecordSpecificDispId(WOperator, WDispId);
            if (Dh.RecordFound == true)
            {
                // Update Record
                Dh.EndDate = DateTime.Now;
                Dh.CloseDate = DateTime.Now;

                Dh.UpdateDisputeOwnersHistory(WOperator, Dh.SeqNumber);
            }
            else
            {

            }

            Dh.DispId = WDispId;
            Dh.StartDate = DateTime.Now;
            Dh.CreatorId = WSignedId;
            Dh.Reason = comboBox1.Text;
            Dh.HasOwner = false;
            Dh.OwnerId = "";

            Dh.InsertDisputeOwnersHistory(WOperator, WDispId);

            // Update Dispute with Owner = ""
            Di.HasOwner = false;
            Di.OwnerId = "";
            Di.UpdateDisputeRecord(WDispId);

            MessageBox.Show("Officer DeAssigned");

            Form3DispOwners_Load(this, new EventArgs());

            textBoxMessage.Text = "Officer DeAssigned"; 

        }

// History of oWNERS 
        private void buttonHistory_Click(object sender, EventArgs e)
        {
            NForm3_DispOwnersHistory = new Form3_DispOwnersHistory(WSignedId, WSignRecordNo, WOperator, WDispId);
            //NForm3DispOwners.FormClosed += NForm3DispOwners_FormClosed;
            NForm3_DispOwnersHistory.ShowDialog(); 
        }

// Finish 
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

    }
}
