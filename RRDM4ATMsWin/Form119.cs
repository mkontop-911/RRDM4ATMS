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
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Configuration;

//multilingual
using System.Resources;
using System.Globalization;

namespace RRDM4ATMsWin
{
    public partial class Form119 : Form
    {
       // Form13 NForm13;

   //     string WUserBankId;
   //     string WAccessToBankTypes;
    //    int WSecLevel;

        string WSignedId;
        int WSignRecordNo;
    //    string WBankId;
    //    bool WPrive;
        string WChosenUserId;
        string WUserName;

        string WAtmNo;
        int WGroup;

        RRDMUsersAccessToAtms Ba = new RRDMUsersAccessToAtms();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord(); // Make class availble 
        RRDMGroups Ga = new RRDMGroups();

        public Form119(string InSignedId, int SignRecordNo, string InChosenUserId, string InUserName)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
       //     WBankId = InBankId;
       //     WPrive = InPrive;
            WChosenUserId = InChosenUserId;
            WUserName = InUserName;
            InitializeComponent();
            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;

            // ===============================
         //   Us.ReadUsersRecord(InSignedId); // Read USER record for the signed user
         //   WAccessToBankTypes = Us.AccessToBankTypes;
      //      WUserBankId = Us.UserBankId;
         //   WSecLevel = Us.SecLevel;
            // ===============================
        }

        private void Form119_Load(object sender, EventArgs e)
        {
            
            string filter = "UserId='" + WChosenUserId + "'";
            usersAtmTableBindingSource.Filter = filter;
            this.usersAtmTableTableAdapter.Fill(this.aTMSDataSet23.UsersAtmTable);

            textBoxMsgBoard.Text = " Make Your Choice and Press the Button "; 
            // TODO: This line of code loads data into the 'aTMSDataSet23.UsersAtmTable' table. You can move, or remove it, as needed.
            this.usersAtmTableTableAdapter.Fill(this.aTMSDataSet23.UsersAtmTable);

        }

        // ADD ATM  

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox11.Text))
            {
                MessageBox.Show("Insert ATM Number Please");
                return;
            }

            Ba.ReadUsersAccessAtmTable(WChosenUserId);

            if (Ba.NoOfAtmsRepl > 0 || Ba.NoOfAtmsReconc > 0)
            {
                if (String.IsNullOrEmpty(textBox11.Text))
                {
                    MessageBox.Show("Insert ATM Number Please - This User Deals with Individual ATMs");
                    return;
                }
            }

            if (Ba.NoOfGroupsRepl > 0 || Ba.NoOfGroupsReconc > 0)
            {
                if (String.IsNullOrEmpty(textBox6.Text))
                {
                    MessageBox.Show("Insert Group Number Please - This User Deals with Group of ATMs");
                    return;
                }
            }


            // ATM  HANDLING 
            Ba.AtmNo = textBox11.Text;

            // CHECK IF ATM EXIST 

            RRDMAtmsClass Aa = new RRDMAtmsClass();

            Aa.ReadAtm(Ba.AtmNo);

            if (Aa.RecordFound == false)
            {
                MessageBox.Show("ATM Number not valid");
                return;
            }

            if (checkBox4.Checked == false & checkBox5.Checked == false)
            {
                MessageBox.Show("Make At Least one Choice");
                return;
            }
            else
            {
                Ba.Replenishment = checkBox4.Checked;
                Ba.Reconciliation = checkBox5.Checked;
            }

            Ba.GroupOfAtms = 0;

            Ba.UserId = WChosenUserId;

        //    La.ReadUsersRecord(Ba.UserId);

            Ba.BankId = Aa.BankId;

        //    Ba.Prive = Aa.Prive;

            Ba.DateOfInsert = DateTime.Now;

            Ba.Operator = Aa.Operator; 

            Ba.ReadUsersAccessAtmTableSpecific(Ba.UserId, Ba.AtmNo, Ba.GroupOfAtms);
            if (Ba.RecordFound == true)
            {
                MessageBox.Show("RECORD ALREADY EXIST");
                return;
            }

            Ba.InsertUsersAtmTable(Ba.UserId, Ba.AtmNo, Ba.GroupOfAtms);

            string filter = "UserId='" + Ba.UserId + "'";
            usersAtmTableBindingSource.Filter = filter;
            this.usersAtmTableTableAdapter.Fill(this.aTMSDataSet23.UsersAtmTable);

            textBoxMsgBoard.Text = " USER ACCESS TO ATM RELATION WAS ADDED ";

        }


        // ADD GROUP 

        private void button2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox6.Text))
            {
                MessageBox.Show("Insert Group Number Please");
                return;
            }

            Ba.ReadUsersAccessAtmTable(WChosenUserId);

            if (Ba.NoOfAtmsRepl > 0 || Ba.NoOfAtmsReconc > 0)
            {
                if (String.IsNullOrEmpty(textBox11.Text))
                {
                    MessageBox.Show("Insert ATM Number Please - This User Deals with Individual ATMs");
                    return;
                }
            }

            if (Ba.NoOfGroupsRepl > 0 || Ba.NoOfGroupsReconc > 0)
            {
                if (String.IsNullOrEmpty(textBox6.Text))
                {
                    MessageBox.Show("Insert Group Number Please - This User Deals with Group of ATMs");
                    return;
                }
            }


            // GROUP HANDLING 
            Ba.GroupOfAtms = int.Parse(textBox6.Text);

            Ba.UseOfGroup = true;

            Ga.ReadGroup(Ba.GroupOfAtms);

            if (Ga.RecordFound == false)
            {
                MessageBox.Show(" Group Number not valid");
                return;
            }


            if (checkBox2.Checked == true & Ga.Replenishment == false)
            {
                MessageBox.Show(" Group is not defined for replenish");
                return;
            }

            if (checkBox1.Checked == true & Ga.Reconciliation == false)
            {
                MessageBox.Show(" Group is not defined for reconciliation");
                return;
            }

            if (checkBox2.Checked == true) Ba.Replenishment = true;
            if (checkBox1.Checked == true) Ba.Reconciliation = true;

            Ba.AtmNo = "";

            Ba.UserId = WChosenUserId;

  //          La.ReadUsersRecord(Ba.UserId);

            Ba.BankId = Ga.BankId;

         //   Ba.Prive = Ga.Prive;

            Ba.DateOfInsert = DateTime.Now;

            Ba.Operator = Ga.Operator; 

            Ba.ReadUsersAccessAtmTableSpecific(Ba.UserId, Ba.AtmNo, Ba.GroupOfAtms);
            if (Ba.RecordFound == true)
            {
                MessageBox.Show("RECORD ALREADY EXIST");
                return;
            }

            Ba.InsertUsersAtmTable(Ba.UserId, Ba.AtmNo, Ba.GroupOfAtms);

            string filter = "UserId='" + Ba.UserId + "'";
            usersAtmTableBindingSource.Filter = filter;
            this.usersAtmTableTableAdapter.Fill(this.aTMSDataSet23.UsersAtmTable);

            textBoxMsgBoard.Text = " GROUP RELATION WAS INSERTED ";

        }

        //
        //
        // CHOOSE ROW TO BE DELETED 
        //
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            //textBox1.Text = rowSelected.Cells[0].Value.ToString();
            WAtmNo = rowSelected.Cells[1].Value.ToString(); ;
            WGroup = (int)rowSelected.Cells[2].Value;
        }


        // DELETE RELATION 

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (WAtmNo == "" & WGroup == 0)
            {
                MessageBox.Show("CHOOSE A ROW (RELATION ) TO BE DELETED PLEASE");
                return;
            }


            Ba.DeleteUsersAtmTableEntry(WChosenUserId, WAtmNo, WGroup);

            string filter = "UserId='" + WChosenUserId + "'";
            usersAtmTableBindingSource.Filter = filter;
            this.usersAtmTableTableAdapter.Fill(this.aTMSDataSet23.UsersAtmTable);

            textBoxMsgBoard.Text = " USER ACCESS TO ATM OR GROUP RELATION WAS DELETED ";

        }
        // Finish 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }
        
       
      
    }
}
