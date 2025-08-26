using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using RRDM4ATMs;
// using GASClassLib; 

namespace RRDM4ATMsWin
{
    public partial class Form63 : Form
    {
    
        Bitmap SCREENinitial;
        string AuditTrailUniqueID = "";
        Form2_EMailContent NForm2_EMailContent; 

        DateTime WDTm = new DateTime(2014, 02, 28);

        string Tablefilter; 

        // DATATable for Grid 
        public DataTable GridDays = new DataTable();
        DataTable dtAtmsMain = new DataTable();
    //    SqlDataAdapter daAtmsMain;

        RRDMSessionsNotesBalances Na = new RRDMSessionsNotesBalances(); // Class Notes 

        RRDMAtmsMainClass Am = new RRDMAtmsMainClass(); // ATM MAIN CLASS TO UPDATE NEXT REPLENISHMENT DATE

        RRDMReplDatesCalc Rc = new RRDMReplDatesCalc(); // Locate next Replenishment 

        RRDMDepositsClass Dc = new RRDMDepositsClass();

        RRDMUsersAccessToAtms Uaa = new RRDMUsersAccessToAtms(); 

        RRDMUsersRecords Ua = new RRDMUsersRecords();

        RRDMReplOrdersClass Ra = new RRDMReplOrdersClass(); 

        RRDMUpdateGrids Ug = new RRDMUpdateGrids();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMComboClass Cc = new RRDMComboClass(); 

        RRDMEmailClass2 Em = new RRDMEmailClass2();

        RRDMAtmsClass Ac = new RRDMAtmsClass(); 

        DateTime LongFutureDate = new DateTime(2050, 11, 21);

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WCitId; 

        int WReplActNo; 
        string WAtmNo;
  

        string WSignedId;
        int WSignRecordNo;
        string WOperator;

        public Form63(string InSignedId, int SignRecordNo, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WOperator = InOperator;

            InitializeComponent();

            //buttonNext.Hide();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;

            comboBox1.DataSource = Cc.GetCitIds(WOperator);
            comboBox1.DisplayMember = "DisplayValue";

            //TEST
            comboBox1.Text = "2000"; 

            textBoxMsgBoard.Text = "Take the appropriate action for the ATMs in Need";


            // Read USER and ATM Table 
            //WAction = 1; // Update Main record AuthUser field with User Applies to single or group of ATMs 
            //RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            //Usi.ReadSignedActivityByKey(WSignRecordNo); // REad Access level 

            //if (Usi.SecLevel == "03")
            //{
            //    Ug.ReadUsersAccessAtmAndUpdateReplActions(WSignedId, 2); // Zero , Initialiase Authorize User
            //    Ug.ReadUsersAccessAtmAndUpdateReplActions(WSignedId, 1); // Assign Authorize User
            //}        

/*
             if (Ua.SecLevel == 3)
            {
                SQLString = "Select [AtmNo]  FROM [dbo].[AtmsMain] WHERE Operator = '" 
                    + WOperator + "' AND AuthUser ='" + WSignedId +"'";
            }
          

            // THIS ACCESS LEVEL = 4 is the Controller - Is allowed to see all 

             if (Ua.SecLevel == 4)
             {
                 SQLString = "Select [AtmNo]  FROM [dbo].[AtmsMain] WHERE Operator = '" + WOperator + "'"; 
             }
  */         

             // 10 : Normal for replenishment 
             // 11 : Late Replenishment => Today.Now > Next Replenishment date
             // 12 : Replenish Now not enough money for Today
             // 13 : Inform G4S to Replenished in "two" days.
             // 14 : ATM will run out of money during Weekened or Holiday
             // 15 : Estimated next replenishement date < than next planned date 
             // 16 : ATM appears to have many captured cards and has many Errors

           
            }

        private void Form63_Load_1(object sender, EventArgs e)
        {
            // Security level 3 
            //
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);
            if (Usi.SecLevel == "03")
            {
                Tablefilter = "Operator ='" + WOperator + "' AND AuthUser = '" + WSignedId + "' AND AuthorisedRecord =0  " ;
            }
          
            //TEST
            Usi.SecLevel = "04"; 

            // Security level 4
            //
            if (Usi.SecLevel == "04")
            {
                Tablefilter = "Operator ='" + WOperator + "'" + " AND AuthorisedRecord =0  ";
            }

            DateTime WDtFrom = NullPastDate;
            DateTime WDtTo = NullPastDate;

            // Read Table

            Ra.ReadReplActionsAndFillTable(WOperator, Tablefilter, WDtFrom, WDtTo, 2);

            ShowGrid();
            if (dataGridView1.Rows.Count != 0)
            {
                System.Drawing.Bitmap memoryImage;
                memoryImage = new System.Drawing.Bitmap(tableLayoutPanelMain.Width, tableLayoutPanelMain.Height);
                tableLayoutPanelMain.DrawToBitmap(memoryImage, tableLayoutPanelMain.ClientRectangle);
                SCREENinitial = memoryImage;
            }
          
        }

// Show Grid 
        public void ShowGrid()
        {
            dataGridView1.DataSource = Ra.TableReplOrders.DefaultView;

            dataGridView1.Columns[0].Width = 40; // ActNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // ActId
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 40; // AtmNo

            dataGridView1.Columns[3].Width = 50; // CycleNo

            dataGridView1.Columns[4].Width = 70; // AmountWas

            dataGridView1.Columns[5].Width = 70; // LastReplDt
            dataGridView1.Columns[5].Width = 70; // NewEstReplDt
            dataGridView1.Columns[5].Width = 70; // PassReplCycleDate

            dataGridView1.Columns[5].Width = 70; // EstAmount
            dataGridView1.Columns[5].Width = 70; // InMoneyReal

            if (dataGridView1.Rows.Count == 0)
            {
                //MessageBox.Show("No transactions to be posted");
                Form2 MessageForm = new Form2("No Actions within this dates range! ");
                MessageForm.ShowDialog();

                this.Dispose();
                return;
            }
        }

        // ON ROW ENTER ASSIGN ATMNo
        //
        private void dataGridView1_RowEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

            WReplActNo = (int)rowSelected.Cells[0].Value;

            Ra.ReadReplActionsSpecific(WReplActNo);

            WAtmNo = Ra.AtmNo;

            Ac.ReadAtm(WAtmNo);
            Am.ReadAtmsMainSpecific(WAtmNo);

            textBox1.Text = WAtmNo;

            textBox3.Text = WReplActNo.ToString(); 

            // CIT operator 
            if (Ac.CitId != "1000")
            {
                Us.ReadUsersRecord(Ac.CitId);
                textBoxReplOwner.Text = Us.UserName;
            }
            else
            {
                textBoxReplOwner.Text = "Repl will be done by Bank personel.";
            }

            Ra.ReadReplActionsForAtm(WAtmNo, WReplActNo);

            if (Ra.RecordFound == true)
            {
                if (Ra.ActiveRecord == true)
                {
                    textBoxStatus.Text = "Active"; 
                }
                else textBoxStatus.Text = "Inactive"; 

                if (Am.NeedType == 10)
                {
                    textBoxNeedType.Text = " THIS ATM is not in NEED ";
                }
                if (Am.NeedType == 11)
                {
                    textBoxNeedType.Text = " Replenishment Has been delayed ";
                }
                if (Am.NeedType == 12)
                {
                    textBoxNeedType.Text = " Replenish now because of low balance ";
                }
                if (Am.NeedType == 13 & Ac.CitId != "1000" & Ra.RecordFound == true)
                {
                    textBoxNeedType.Text = " Inform CIT " + Ac.CitId + " to replenish ";

                    textBoxRecomAction.Text = "A Replenishment Action with Id : " + Ra.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                        + "Next Replenishment : " + Ra.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                        + "Amount for Replenihsment : " + Ra.NewAmount.ToString("#,##0.00") + Environment.NewLine
                        + "An email and report will be sent to  " + Environment.NewLine
                        + "CIT provider and responsible departments with instructions. ";

                }
                if (Am.NeedType == 13 & Ac.CitId == "1000" & Ra.RecordFound == true)
                {
                    textBoxNeedType.Text = "Bank ATM Operator was alerted to replenish ";

                    textBoxRecomAction.Text = " A alert Replenishment Action with Id : " + Ra.ReplOrderNo.ToString() + " is suggested " + Environment.NewLine
                                   + " Next Replenishment : " + Ra.NewEstReplDt.Date.ToShortDateString() + Environment.NewLine
                                   + " Amount for Replenihsment : " + Ra.NewAmount.ToString("#,##0.00") + Environment.NewLine
                                   + " An Alerting SMS and email was sent to ATM owner at this email:  " + Environment.NewLine
                                   + " " + Ac.AtmReplUserEmail;

                }
             
                if (Am.NeedType == 14)
                {
                    textBoxNeedType.Text = " Current Balance will run low during not working day ";
                } 
                if (Am.NeedType == 15)
                {
                    textBoxNeedType.Text = " Estimated next is less than planed replenishment ";
                }
                if (Am.NeedType == 16)
                {
                    textBoxNeedType.Text = " Replenishment Has been delayed And Running Out of Money";
                }

                // Get the USER RESPONSIBLE FOR THIS ATM
                if (Ra.AtmsReplGroup > 0) Uaa.FindUserForRepl("", Ra.AtmsReplGroup);

             //       UsersAccessToAtms Uaa = new UsersAccessToAtms(); 
                //    UsersAndSignedRecord Ua = new UsersAndSignedRecord();

                else Uaa.FindUserForRepl(WAtmNo, 0);

                // FIND USER FOR THIS 

                if (Uaa.RecordFound == true)
                {
                    Ua.ReadUsersRecord(Uaa.UserId); // Get Info for User 

                    textBox8.Text = Ua.UserId;
                    textBox9.Text = Ua.UserName;
                    textBox10.Text = Ua.email;
                    textBox11.Text = Ua.MobileNo;

                    Ra.OwnerUser = Uaa.UserId;
                }
                else
                {

                    textBox8.Text = "UnKnown";
                    textBox9.Text = "UnKnown";
                    textBox10.Text = "UnKnown";
                    textBox11.Text = "UnKnown";

                    Ra.OwnerUser = "UnKnown";
                }

                Ra.UpdateReplActionsForAtm(WAtmNo, WReplActNo);
            }
                      

        }

        // SHOW GRID BASED ON SELECTION CRITERIA 
        private void button2_Click_1(object sender, EventArgs e)
        {
            // THIS FUNCTION IS FOR SECURITY LEVEL 4
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo); // REad Access level 
            //TEST 
      /*      if (Ua.SecLevel != 4)
            {
                MessageBox.Show("This function is not allowed. It is only for Users having Sec Level 4");
                return;
            }
       */

            if (radioButton1.Checked == false & radioButton2.Checked == false)
            {
                MessageBox.Show("Please make your choice");
                return;
            }

            if (radioButton1.Checked == true) // All ATMS in NEED 
            {
                
                    Tablefilter = "Operator ='" + WOperator + "' AND CitId ='" + WCitId + "' AND ActiveRecord = 1" ;
             
            }

            if (radioButton2.Checked == true) // All including Not Active   
            {
              
                    Tablefilter = "Operator ='" + WOperator + "' AND CitId ='" + WCitId +"'" ;
              
            }
/*
            if (radioButton3.Checked == true) // NEED OF MONEY - NEED OF REPLENISHMENT   
            {
                if (WPrive == true)
                {
                    Tablefilter = "BankId ='" + WBankId + "'"
                        + " AND (NeedType = 12 OR NeedType = 13 OR NeedType = 14) AND CitId = " + WCitId;
                }
                else // NOT PRIVE
                {
                    Tablefilter = " Prive = 0 AND (NeedType = 12 OR NeedType = 13 OR NeedType = 14) AND CitId = " + WCitId;
                }
            }
 */

            DateTime WDtFrom = NullPastDate;
            DateTime WDtTo = NullPastDate;
// Read Table 
            Ra.ReadReplActionsAndFillTable(WOperator, Tablefilter, WDtFrom, WDtTo, 2);

            ShowGrid(); 
        }

        // ACTIONS ON LIST 
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButtonEmail.Checked == false & radioButtonInactivate.Checked == false & radioButtonActivate.Checked == false)
            {
                MessageBox.Show("Please make your choice");
                return;
            }
            // EMAIL ACTION
            if (radioButtonEmail.Checked == true)
            {         
                int Function = 1 ; // Prepare string .. Do not update
                //TEST
                // Read all actions creater than 25 
                // WE HAVE CHANGED METHOD IN Ra.Read....
                int WOrdersCycle = 0;  
                Ra.ReadReplActionsForCITAndUpdate(WCitId, WOrdersCycle, WOperator, WSignedId);

                if (Ra.RecordFound == true)
                {
                    // Report
                    //Form56R15 InstructionsToCit = new Form56R15(WCitId, WOperator, Ra.PublicCitString);
                    //InstructionsToCit.Show(); 

                    // Email 
                 
                    NForm2_EMailContent = new Form2_EMailContent(WSignedId,  WSignRecordNo, WOperator, WCitId, WOrdersCycle, Ra.PublicCitString);
                    //NForm2_EMailContent.FormClosed += NForm2_EMailContent_FormClosed;
                    NForm2_EMailContent.ShowDialog();

                }
                else
                {
                    MessageBox.Show("There no Actions to be taken for this CIT. Refresh testing data if you are in testing mode");
                    return;
                }   
            }
// If Inactivate then 
            if (radioButtonInactivate.Checked == true)
            {
                // Set action to inactive

                if (textBoxComment.Text == "")
                {
                    MessageBox.Show("Please enter comment.");
                    return; 
                }

                Ra.ReadReplActionsForAtm(WAtmNo, WReplActNo);

                Ra.ActiveRecord = false; 

                Ra.InactivateComment = textBoxComment.Text;

                Ra.UpdateReplActionsForAtm(WAtmNo, WReplActNo);

                Form63_Load_1(this, new EventArgs());
                
            }
// If Activate then             
            if (radioButtonActivate.Checked == true)
            {
                // Set action to Active

                if (textBoxComment.Text == "")
                {
                    MessageBox.Show("Please enter comment.");
                    return; 
                }

                Ra.ReadReplActionsForAtm(WAtmNo, WReplActNo);

                Ra.ActiveRecord = true ; 

                Ra.InactivateComment = "" ;

                Ra.UpdateReplActionsForAtm(WAtmNo, WReplActNo);

                Form63_Load_1(this, new EventArgs());
                
            }
            //AUDIT TRAIL 
            
            ////AUDIT TRAIL 
            //string AuditCategory = "Operations";
            //string AuditSubCategory = "Repl Actions";
            //string AuditAction = "Send Email";

            //string Message = textBoxMsgBoard.Text;
            //GetMainBodyImageAndStoreIt(AuditCategory, AuditSubCategory, AuditAction, WSignedId, Message);
        }

        //void NForm2_EMailContent_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    Form63_Load_1(this, new EventArgs());
        //}
     
      
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
        // COMBO BOX Change - New CIT IS INSERTED 
       

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            WCitId = comboBox1.Text;
         
                Us.ReadUsersRecord(WCitId);
                textBox7.Text = Us.UserName;
                return;
      
        }
// Finish 
      

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }

        private void radioButtonInactivate_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonInactivate.Checked == true)
            {
                labelComment.Show();
                textBoxComment.Show(); 
            }
            else
            {
                labelComment.Hide();
                textBoxComment.Hide(); 
            }
        }
    }
}
