using System;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form54 : Form
    {

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();

        bool IsController;
        string ControlerId;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        public Form54(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            string Application = WSignedId.Substring(0, 4);

            if (Application == "NOST")
            {
                //string Role = "ATMs Controller(4)";
                //Us.ReadRoleRecord(InOperator, Role);
                ControlerId = "NOSTROAuther";
                Us.ReadUsersRecord(ControlerId);
                label2.Text = Us.UserId + " " + Us.UserName;
                textBox10.Text = Us.MobileNo;
                textBox9.Text = Us.email;
            }
            else
            {
                if (Application == "ITMX")
                {
                    //string Role = "ATMs Controller(4)";
                    //Us.ReadRoleRecord(InOperator, Role);
                    ControlerId = "ITMXUser3";
                    Us.ReadUsersRecord(ControlerId);

                    label2.Text = Us.UserId + " " + Us.UserName;
                    textBox10.Text = Us.MobileNo;
                    textBox9.Text = Us.email;
                }
                else
                {
                    //string Role = "ATMs Controller(4)";
                    //Us.ReadRoleRecord(InOperator, Role);
                    ControlerId = "487116";
                    Us.ReadUsersRecord(ControlerId);

                    label2.Text = Us.UserId + " " + Us.UserName;
                    textBox10.Text = Us.MobileNo;
                    textBox9.Text = Us.email;
                }
            }

            if (WSignedId == Us.UserId)
            {
                IsController = true;
                // Get Users if you are a controller 
                comboBoxUsers.DataSource = Us.GetUsersList(WOperator, WSignedId);
                comboBoxUsers.DisplayMember = "DisplayValue";
                tabPage1.Enabled = false;
                tabPage2.Enabled = true;
            }
            else
            {
                IsController = false;
                tabPage1.Enabled = true;
                tabPage2.Enabled = false;
            }

        }

        // WE CREATE MESSAGE to CONTROLLER 
        private void button1_Click(object sender, EventArgs e)
        {
            if (IsController == true)
            {
                MessageBox.Show(" You are not allowed this function. You are the controller!");
                return;
            }
            Cm.ToAllAtms = false;
            if (textBox2.Text.Length > 0)
            {
                Cm.AtmNo = textBox2.Text;
            }
            else
            {
                Cm.AtmNo = "";
            }
            Cm.FromUser = WSignedId;
            Cm.ToUser = ControlerId;
            Cm.BankId = WOperator;

            Cm.BranchId = Us.Branch;
            Cm.Type = "Msg To Controller";
            Cm.Message = textBox1.Text;

            if (checkBox1.Checked == true)
            {
                Cm.SeriousMsg = true;
            }
            else
            {
                Cm.SeriousMsg = false;
            }

            Cm.Operator = WOperator;

            Cm.InsertMSg();

            MessageBox.Show(" MSG WAS ADDED");

            textBox2.Text = "";
            textBox1.Text = "";
        }
        // Message From controller 
        private void button3_Click(object sender, EventArgs e)
        {
            if (IsController == false)
            {
                MessageBox.Show(" You are not allowed this function. You are not the controller!");
                return;
            }

            Cm.ToAllAtms = false;

            Cm.FromUser = WSignedId;

            if (comboBoxUsers.Text.Length > 0)
            {
                Cm.ToUser = comboBoxUsers.Text;
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

            Cm.BranchId = Us.Branch;
            Cm.Type = "Controlers Msg";
            Cm.Message = textBox4.Text;
            if (checkBox2.Checked == true)
            {
                Cm.SeriousMsg = true;
            }
            else
            {
                Cm.SeriousMsg = false;
            }
            Cm.Operator = WOperator;

            Cm.InsertMSg();

            MessageBox.Show(" MSG WAS ADDED");

            //textBox6.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";

            //   MessageBox.Show(" Not available functionality yet");
            //   return;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        // Finish 
        private void button4_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
    }
}
