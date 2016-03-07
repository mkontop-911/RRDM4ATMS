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

namespace RRDM4ATMsWin
{
    public partial class Form46 : Form
    {
        Bitmap SCREENinitial;
        string AuditTrailUniqueID = "";

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
   //     bool WPrive;
        int WChosenGroup;
        int WFunctionNo;

      //  int WUserName; 
        
    //    bool InsertUpdateMade; 

        RRDMUsersAndSignedRecord Xa = new RRDMUsersAndSignedRecord(); // Make class availble 

        RRDMGroups Ga = new RRDMGroups();

        public Form46(string InSignedId, int SignRecordNo, string InOperator, int InChosenGroup, int InFunctionNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
          //  WPrive = InPrive;
            WChosenGroup = InChosenGroup;
            WFunctionNo = InFunctionNo; 

            // FunctionNo = 1 is updating , 2 Is Add, 3 is delete 

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            button6.Hide();
            button4.Hide();
            button5.Hide();

            if (WFunctionNo == 1) button6.Show();
            if (WFunctionNo == 2) button4.Show();
            if (WFunctionNo == 3) button5.Show(); 

            if (WFunctionNo == 1 || WFunctionNo == 3) // UPDATING OR DELETE 
            {
                Ga.ReadGroup(WChosenGroup);

                textBox1.Text = Ga.GroupNo.ToString ();
                textBox3.Text = Ga.BankId;

             //   checkBox1.Checked = Ga.Prive;
                checkBox3.Checked = Ga.MoreThanOneBank;
                checkBox5.Checked = Ga.Stats;
                checkBox4.Checked = Ga.Replenishment;
                checkBox6.Checked = Ga.Reconciliation;
                textBox2.Text = Ga.Description;
                checkBox2.Checked = Ga.Inactive; 

            if (WFunctionNo == 1 ) textBoxMsgBoard.Text = " UPDATE FIELDS AND PRESS THE UPDATE BUTTON ";
            if (WFunctionNo == 3) textBoxMsgBoard.Text = " ENSURE WHAT YOU WANT AND PRESS DELETE BUTTON "; 
            }

            if (WFunctionNo == 2) // INSERT NEW
            {

        
                textBox3.Text= WOperator;
              //  checkBox1.Checked = true

                textBox1.Hide();
                label1.Hide();

             //   textBoxMsg.Text = " INPUT DATA FOR THE NEW Group ";
            }

        }

        private void Form46_Load(object sender, EventArgs e)
        {

            System.Drawing.Bitmap memoryImage;
            memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
            tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
            SCREENinitial = memoryImage;
        }
        //
        // UPDATE Group
        // Function = 1 

        private void button6_Click(object sender, EventArgs e)
        {
            if (WFunctionNo == 2 || WFunctionNo == 3)
            {
                MessageBox.Show(" YOU HAVE CHOSEN THE WRONG BUTTON ");
                return;
            }


            Ga.BankId = WOperator;
          

            Ga.MoreThanOneBank = checkBox3.Checked;
            Ga.Stats = checkBox5.Checked;
            Ga.Replenishment = checkBox4.Checked;
            Ga.Reconciliation = checkBox6.Checked;
            Ga.Description = textBox2.Text;
            Ga.DtTmCreated = DateTime.Now;
            Ga.Inactive = checkBox2.Checked;

            Ga.UpdateGroup(Ga.GroupNo);

            MessageBox.Show(" The Group WAS UPDATED");

            textBoxMsgBoard.Text = " The Group WAS UPDATED "; 

            //AUDIT TRAIL 
            string AuditCategory = "Maintenance";
            string AuditSubCategory = "Groups";
            string AuditAction = "Update";
            string Message = textBoxMsgBoard.Text;
            GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);

        }

        //
        // OPEN NEW GROUP
        // ADD

        private void button4_Click(object sender, EventArgs e)
        {
            if (WFunctionNo == 1 || WFunctionNo == 3)
            {
                MessageBox.Show(" YOU HAVE CHOSEN THE WRONG BUTTON ");
                return;
            }


            Ga.BankId = WOperator;
            
                Ga.BankId = textBox3.Text;
            

            Ga.MoreThanOneBank = checkBox3.Checked;
            Ga.Stats = checkBox5.Checked;
            Ga.Replenishment = checkBox4.Checked;
            Ga.Reconciliation = checkBox6.Checked;
            Ga.Description = textBox2.Text;
            Ga.DtTmCreated = DateTime.Now;
            Ga.Inactive = checkBox2.Checked;

            Ga.Operator = WOperator;

            Ga.InsertGroup();

            Ga.ReadGroupLastNo();

            textBox1.Text = Ga.GroupNo.ToString();
            textBox1.Show();
            label1.Show();

            MessageBox.Show(" THE NEW Group HAS BEEN CREATED");

            textBoxMsgBoard.Text = " THE NEW Group HAS BEEN CREATED ";

            //AUDIT TRAIL 
            //AUDIT TRAIL 
            string AuditCategory = "Maintenance";
            string AuditSubCategory = "Groups";
            string AuditAction = "New Group";
            string Message = textBoxMsgBoard.Text;
            GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);
        }

        // DELETE GROUP 

        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Do you want to delete this Group?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                             == DialogResult.Yes)
            {
                if (WFunctionNo == 1 || WFunctionNo == 2)
                {
                    MessageBox.Show(" YOU HAVE CHOSEN THE WRONG BUTTON ");
                    return;
                }

                Ga.DeleteGroupsEntry(Ga.GroupNo);

                Ga.Description = " DELETED RECORD";

                textBoxMsgBoard.Text = " THE Group HAS BEEN Deleted ";

                //AUDIT TRAIL 
                string AuditCategory = "Maintenance";
                string AuditSubCategory = "Groups";
                string AuditAction = "Delete Group";
                string Message = textBoxMsgBoard.Text;
                GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);
            }
            else
            {
            }
            

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

            AuditTrailClass At = new AuditTrailClass();

            if (AuditTrailUniqueID.Equals(""))
            {
                AuditTrailUniqueID = At.InsertRecord(InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
            }
            else
            {
                At.UpdateRecord(AuditTrailUniqueID, InCategory, InSubCategory, InTypeOfChange, InUser, SCREENb, SCREENinitial, Message);
            }

        }

        // 
        // Cancel 
        //
        private void Cancel_Click(object sender, EventArgs e)
        {
                this.Close(); 
        }

    }
}
