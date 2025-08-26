using System;
using System.Windows.Forms;
using RRDM4ATMs;

//multilingual

namespace RRDM4ATMsWin
{
    public partial class Form119 : Form
    {
        string WOperator;
        string WSignedId;
        int WSignRecordNo;
  
        string WChosenUserId;
        string WUserName;

        string WAtmNo;
        int WGroup;

        RRDMUsersAccessToAtms Ua = new RRDMUsersAccessToAtms();
        RRDMUsersRecords Us = new RRDMUsersRecords(); // Make class availble 
        RRDMGroups Ga = new RRDMGroups();

        public Form119(string InOperator, string InSignedId, int SignRecordNo, string InChosenUserId, string InUserName)
        {
            WOperator = InOperator;
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
    
            WChosenUserId = InChosenUserId;
            WUserName = InUserName;
            InitializeComponent();

            // Set Working Date 
           
            labelToday.Text = DateTime.Now.ToShortDateString();

            pictureBox1.BackgroundImage = appResImg.logo2;

            textBox8.Text = "ATMs ACCESS FOR USER.." + WChosenUserId; 
        }

        private void Form119_Load(object sender, EventArgs e)
        {
            
            string filter = " UserId='" + WChosenUserId + "'";

            Ua.ReadUserAccessToAtmsFillTable(filter);

            ShowGridUserToAtms(); 

            textBoxMsgBoard.Text = " Make Your Choice and Press the Button "; 
        }

        //
        //
        // CHOOSE ROW TO BE DELETED 
        //
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
      
            WAtmNo = rowSelected.Cells[1].Value.ToString();

            textBoxAtmNo.Text = WAtmNo;

            Ua.ReadUsersAccessAtmTableSpecific(WChosenUserId, WAtmNo, 0);

            if (Ua.Replenishment) checkBox4.Checked = true;
            else checkBox4.Checked = false;

            if (Ua.Reconciliation) checkBox5.Checked = true;
            else checkBox5.Checked = false;

        }


        // ADD ATM  

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxAtmNo.Text))
            {
                MessageBox.Show("Insert ATM Number Please");
                return;
            }
            // ATM  HANDLING 
            Ua.AtmNo = textBoxAtmNo.Text;
   

            // CHECK IF ATM EXIST 

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            Ac.ReadAtm(Ua.AtmNo);

            if (Ac.CitId != "1000" & checkBox4.Checked == true)
            {
                MessageBox.Show("This ATM is replenished" + Environment.NewLine
                                + "By CIT_Id =.." + Ac.CitId  +Environment.NewLine
                                + "Not allowed operation "
                                );
               return;
            }

            if (Ac.CitId == "1000" & Ac.AtmsReplGroup>0 & checkBox4.Checked == true)
            {
                MessageBox.Show("This ATM is replenished" + Environment.NewLine
                                + "By Branch =.." + Ac.Branch + Environment.NewLine
                                + "And Replenishment workflow is done by centre " + Environment.NewLine
                                + "Not allowed operation " + Environment.NewLine
                                + "Update ATMReplGroup to zero if you want to add " + Environment.NewLine
                                
                                );
                return;
            }

            if (Ac.RecordFound == false)
            {
                MessageBox.Show("ATM Number not valid");
                return;
            }

            Ua.ReadUsersAccessAtmTableSpecific(WChosenUserId, Ua.AtmNo, 0);
            if (Ua.RecordFound == true)
            {
                MessageBox.Show("RECORD ALREADY EXIST");
                return;
            }

            if (checkBox4.Checked == false & checkBox5.Checked == false)
            {
                MessageBox.Show("Tick please");
                return;
            }
            else
            {
                Ua.Replenishment = checkBox4.Checked;
                Ua.Reconciliation = false;
            }

            Ua.GroupOfAtms = 0;

            Ua.IsCit = false; 

            Ua.UserId = WChosenUserId;

            Ua.UseOfGroup = false; 

            Ua.BankId = Ac.BankId;

            Ua.DateOfInsert = DateTime.Now;

            Ua.Operator = Ac.Operator; 

            Ua.InsertUsersAtmTable(Ua.UserId, Ua.AtmNo, Ua.GroupOfAtms);

            MessageBox.Show("Atm has been added");

            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
            Am.ReadAtmsMainSpecific(Ua.AtmNo);
            Am.AuthUser = Ua.AtmNo;
            Am.UpdateAtmsMain(Ua.AtmNo);

            Form119_Load(this, new EventArgs());

            textBoxMsgBoard.Text = " USER ACCESS TO ATM RELATION WAS ADDED ";

        }

        // Update
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxAtmNo.Text))
            {
                MessageBox.Show("Insert ATM Number Please");
                return;
            }


            Ua.ReadUsersAccessAtmTableSpecific(WChosenUserId, textBoxAtmNo.Text, 0);
            if (Ua.RecordFound == false)
            {
                MessageBox.Show("RECORD DOES NOT EXIST");
                return;
            }

            // ATM  HANDLING 
            Ua.AtmNo = textBoxAtmNo.Text;

            // CHECK IF ATM EXIST 

            RRDMAtmsClass Ac = new RRDMAtmsClass();

            Ac.ReadAtm(Ua.AtmNo);

            if (Ac.RecordFound == false)
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
                Ua.Replenishment = checkBox4.Checked;
                Ua.Reconciliation = checkBox5.Checked;
            }

            Ua.GroupOfAtms = 0;

            Ua.IsCit = false;

            Ua.UserId = WChosenUserId;

            Ua.UseOfGroup = false;

            Ua.BankId = Ac.BankId;

            Ua.DateOfInsert = DateTime.Now;

            Ua.Operator = Ac.Operator;      

            Ua.UpdateUsersAtmTable(Ua.UserId, Ua.AtmNo, Ua.GroupOfAtms);
  
            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();
            Am.ReadAtmsMainSpecific(Ua.AtmNo);
            Am.AuthUser = Ua.AtmNo;
            Am.UpdateAtmsMain(Ua.AtmNo);

            Form119_Load(this, new EventArgs());

            textBoxMsgBoard.Text = " USER ACCESS TO ATM RELATION WAS ADDED ";
        }

        // DELETE RELATION 

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (WAtmNo == "" & WGroup == 0)
            {
                MessageBox.Show("CHOOSE A ROW (RELATION ) TO BE DELETED PLEASE");
                return;
            }


            Ua.DeleteUsersAtmTableEntry(WChosenUserId, WAtmNo, WGroup);

            RRDMAtmsMainClass Am = new RRDMAtmsMainClass();

            if (WAtmNo != "")
            {
                Am.ReadAtmsMainSpecific(WAtmNo);
                if (Am.AuthUser == WChosenUserId) Am.AuthUser = "";
                Am.UpdateAtmsMain(WAtmNo);
            }

            Form119_Load(this, new EventArgs());

            textBoxMsgBoard.Text = " USER ACCESS TO ATM OR GROUP RELATION WAS DELETED ";

        }

        //******************
        // SHOW GRID dataGridView3
        //******************
        private void ShowGridUserToAtms()
        {
            dataGridView1.DataSource = Ua.UsersToAtmsDataTable.DefaultView;

            if (dataGridView1.Rows.Count == 0)
            {
                return;
            }
            else
            {
                dataGridView1.Show();
            }
            dataGridView1.Columns[0].Width = 50; // User Id
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 70; // Atm no
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 200; //  ATM Name 
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dataGridView1.Columns[3].Width = 50; //  Group of ATMs
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].Visible = false;
           
            dataGridView1.Columns[4].Width = 80; // Replenishment
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[5].Width = 80; // Reconciliation 
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Width = 130; // Date of insert 
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        // Finish 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

    }
}
