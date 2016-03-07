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
    public partial class Form54 : Form
    {
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        string WWSignedId;
        int WSignRecordNo;
        string WOperator;
     
        public Form54(string InWSignedId, int SignRecordNo, string InOperator)
        {
            WWSignedId = InWSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;
       
            InitializeComponent();
             
      //      InsertAtm.SetToolTip(TextBox2, "Check messages from controller.");
      //      InsertAtm.ShowAlways = true;

            string Role = "ATMs Controller(4)";
            Us.ReadRoleRecord(InOperator, Role);
            label2.Text = Us.UserId + " " + Us.UserName;
            textBox10.Text = Us.MobileNo;
            textBox9.Text = Us.email; 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // WE CREATE MESSAGE to CONTROLLER 
        private void button1_Click(object sender, EventArgs e)
        {
                Cm.ToAllAtms = false;
                if (textBox2.Text.Length > 0)
                {
                    Cm.AtmNo = textBox2.Text;
                }
                else
                {
                    Cm.AtmNo = "";
                }
                Cm.FromUser= WWSignedId;
                Cm.ToUser = Us.UserId;
                Cm.BankId = WOperator; 
     
                Cm.BranchId = "";
                Cm.Type = ""; 
                Cm.Message = textBox1.Text ; 
                Cm.SeriousMsg = true ;
                Cm.Operator = WOperator; 

            Cm.InsertMSg();

            MessageBox.Show(" MSG WAS ADDED");

            textBox2.Text = "";
            textBox1.Text = "";

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (WWSignedId != Us.UserId)
            {
                MessageBox.Show(" You are not allowed this function. You are not the controller!");
                return; 
            }
            Cm.ToAllAtms = false;

            Cm.FromUser = WWSignedId;

            if (textBox6.Text.Length > 0)
            {
                Cm.ToUser = textBox6.Text;
            }
            else
            {
                Cm.ToUser = "";
            }

            if (textBox3.Text.Length > 0)
            {
                Cm.AtmNo = textBox3.Text;
            }
            else
            {
                Cm.AtmNo = "";
            }
                   
            Cm.BankId = WOperator;

            Cm.BranchId = "";
            Cm.Type = "";
            Cm.Message = textBox4.Text;
            Cm.SeriousMsg = true;
            Cm.Operator = WOperator;

            Cm.InsertMSg();

            MessageBox.Show(" MSG WAS ADDED");

            textBox6.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";

         //   MessageBox.Show(" Not available functionality yet");
         //   return;
        }
    }
}
