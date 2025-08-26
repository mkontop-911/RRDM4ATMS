using System;
using System.Windows.Forms;

//multilingual
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class Form111 : Form
    {
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMEncryptPasswordOrField En = new RRDMEncryptPasswordOrField();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
        RRDMGasParameters Gp = new RRDMGasParameters(); 

        string WAuthoriser;
     
        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        int WAuthorSeqNumber; 
        
        public Form111(string InSignedId, int SignRecordNo, string InOperator, string InAuthoriser, string InAuthName, int InAuthorSeqNumber)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            WAuthoriser = InAuthoriser;

            WAuthorSeqNumber = InAuthorSeqNumber; 

            InitializeComponent();

            textBox1.Text = WAuthoriser;
            textBox2.Text = InAuthName;

            txtBoxPassword.PasswordChar = '*';
           
        }
        // Cancel 
        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
        // Update Authorization 
        //private void buttonBack_Click(object sender, EventArgs e)
        //{
        //    // Update Dispute transaction with authorizer. 
        //    //this.Dispose(); 
        //}

        private void buttonAuthor_Click(object sender, EventArgs e)
        {
          
            // =============================================
            Us.ReadUsersRecord(WAuthoriser); // Read USER record for the Authoriser 

            // ===========================================
            bool PasswordMatched = En.CheckPassword(txtBoxPassword.Text, Us.PassWord);

            if (PasswordMatched == false)
            {
                MessageBox.Show(" Wrong UserID and/or Password ");
                return;
            }
            //DecryptedPassword = En.DecryptField(Us.PassWord);

            //if (txtBoxPassword.Text != DecryptedPassword)
            //{
            //    MessageBox.Show(" Wrong Password ");
            //    return;
            //}

            Gp.ReadParametersSpecificId(Us.Operator, "451", "2", "", "");
            int Temp = ((int)Gp.Amount);

            if (DateTime.Now > Us.DtToBeChanged)
            {
                // Your Password has expired
                MessageBox.Show("Your Password has expired");
                return;
            }
            UpdateAuthorRecord("YES"); 
        }

        // Update Authorization Record 
        private void UpdateAuthorRecord(string InDecision)
        {
            Ap.ReadAuthorizationSpecific(WAuthorSeqNumber);
            Ap.AuthDecision = InDecision;
            if (textBox3.TextLength > 0)
            {
                Ap.AuthComment = textBox3.Text;
            }
            Ap.DateAuthorised = DateTime.Now;
            Ap.Stage = 4;

            Ap.UpdateAuthorisationRecord(WAuthorSeqNumber);
            if (InDecision == "YES")
            {
                MessageBox.Show("Authorization ACCEPTED!");
                this.Dispose();
            }
            //if (InDecision == "NO")
            //{
            //    MessageBox.Show("Authorization REJECTED!");
            //    this.Dispose();
            //}

        }

        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    TimerCount = TimerCount + 1;
        //    Console.Beep(2000, 500);
        //    if (TimerCount == 15)
        //        this.Dispose();

        //}

     
    }
}
