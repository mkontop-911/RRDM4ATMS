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
using System.Configuration;
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form503_CategOwners : Form
    {
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
        RRDMReconcCategories Rc = new RRDMReconcCategories(); 

        string WCategOwner;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCategoryId;

        public Form503_CategOwners(string InSignedId, int InSignRecordNo, string InOperator, string InCategoryId)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WCategoryId = InCategoryId;

            InitializeComponent();

        }
        // On Load Form 

        private void Form503_CategOwners_Load(object sender, EventArgs e)
        {
            // Find Categories details 
            Rc.ReadReconcCategorybyCategId(WOperator, WCategoryId);

            labelCategId.Text = "Category Id: " + WCategoryId ;

            if (Rc.HasOwner == true)
            {
                Us.ReadUsersRecord(Rc.OwnerId);

                labelCurrentOwnerId.Text = "Current Owner Id:   " + Us.UserId;
                labelCuurentOwnerNm.Text = "Current Owner Name: " + Us.UserName;
            }
            else
            {
                labelCurrentOwnerId.Text = "Category Has no Specific Owner yet. ";
                labelCuurentOwnerNm.Hide();
            }

            comboBox1.Items.Add("Select Reason"); // 
            comboBox1.Items.Add("Specialist In the area"); // 
            comboBox1.Items.Add("Work Distribution"); // 
            comboBox1.Items.Add("Old Owner not available"); // 

            comboBox1.Text = "Select Reason";
            textBoxMessage.Text = "Select Reconc Officer to Assign";

            Us.ReadReconcOfficers(WOperator);

            dataGridView1.DataSource = Us.ReconcUsersSelected.DefaultView;

        }
       
        // ON ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WCategOwner = (string)rowSelected.Cells[0].Value;

            Us.ReadUsersRecord(WCategOwner);

            labelOfficerId.Text = "Officer Id   : " + Us.UserId;
            labelOfficerName.Text = "Officer Name : " + Us.UserName;
        }
        // Assign New Owner
        private void buttonAssign_Click(object sender, EventArgs e)
        {
            // Validation

            if (Rc.OwnerId == WCategOwner)
            {
                MessageBox.Show("You are Assigning the same officer! Not allowed ");
                return;
            }

            if (comboBox1.Text == "Select Reason")
            {
                MessageBox.Show("Please enter reason of assignment");
                return;
            }

            
            // Update Dispute with Owner
            Rc.HasOwner = true;
            Rc.OwnerId = WCategOwner;
            Rc.UpdateCategory(WOperator, Rc.CategoryId); 
        
            MessageBox.Show("Officer Assigned");

            Form503_CategOwners_Load(this, new EventArgs());

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

            // Update Dispute with Owner = ""
            Rc.HasOwner = false;
            Rc.OwnerId = "";
            Rc.UpdateCategory(WOperator, Rc.CategoryId); 

            MessageBox.Show("Officer DeAssigned");

            Form503_CategOwners_Load(this, new EventArgs());

            textBoxMessage.Text = "Officer DeAssigned"; 
        }

        // Finish 
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

       
    }
}
