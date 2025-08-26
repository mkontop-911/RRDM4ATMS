using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

//24-04-2015 Alecos
using System.Diagnostics;

//multilingual
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class FormMainScreenCIT : Form
    {
        public EventHandler LoggingOut;

      //  FormMainScreen NFormMainScreen; 

        Form108 NForm108;
        Form8 NForm8;
        Form12 NForm12; 
        Form13 NForm13;
        Form15 NForm15;

        Form200 NForm200;
        Form21MIS NForm21MIS; 
      
        Form5 NForm5;
        Form3 NForm3;
      
        //Form116XXX NForm116; // Dispute pre investigation 

        Form18 NForm18; // CIT Providers  

        Form136 NForm136; // Datagrid showing all Country Banks for a Group 
      
        Form10 NForm10;

        Form9 NForm9;
    //    Form25 NForm25;
       
        Form143 NForm143;
        //Form45 NForm45;
        Form47 NForm47;
       // Form49 NForm49;
        Form50 NForm50;

        Form52 NForm52;
        Form53 NForm53; 
        Form152 NForm152;

        Form54 NForm54;
        Form55 NForm55;
    //    MessageBoxCustom NMessageBoxCustom;
        Form63 NForm63;
     
        Form78 NForm78;
        Form81 NForm81;
     
        Form85 NForm85;

        Form191 NForm191;
        Form192 NForm192;
        Form193 NForm193;

        Form19a NForm19;

        Form300 NForm300;

        Form112 NForm112;

        Form2Testing NForm2Testing;

   //     Form9AlexMaps NForm9AlexMaps;

  //      Form50_CurrentStatusAtms NForm50_CurrentStatusAtms;

        string MsgFilter;

        RRDMUsersAccessToAtms Ba = new RRDMUsersAccessToAtms();
        RRDMUsersRecords Us = new RRDMUsersRecords();
        RRDMGasParameters Gp = new RRDMGasParameters();
        RRDMControllerMsgClass Cm = new RRDMControllerMsgClass();
        RRDMCaseNotes Cn = new RRDMCaseNotes();
        RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess(); 

        // NOTES 
        string Order = "Descending";

        string WParameter4 = "Main Menu - Issues For System";
        string WSearchP4 = "";

        int WAction;

        string WSignedId;
        int WSignRecordNo;
        string WSecLevel;
        string WOperator; 
        
        public FormMainScreenCIT(string InSignedId, int SignRecordNo, string InSecLevel, string InOperator)
        {
            WSignedId = InSignedId;
            WSignRecordNo = SignRecordNo;
            WSecLevel = InSecLevel;
            WOperator = InOperator;
       
            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = appResImg.logo2;
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (WSignedId == "7777" ) 
            {
                foreach (Control c in this.panel2.Controls)
                {
                    if (c is Button)
                    {
                        string Temp = c.Name.Substring(6, 2);
                        //  MessageBox.Show(c.Name);
                        MessageBox.Show("Update this 804 parameter");
                        Gp.ReadParametersSpecificId(WOperator, "804", Temp, "803", WSecLevel.ToString());

                        if (Gp.RecordFound == true)
                        {
                            c.Enabled = true;

                            MessageBox.Show(c.Name);
                            MessageBox.Show(c.Text);
                        }
                        else
                        {
                          //  button18.Enabled = false;
                          //  button18.BackColor = Color.Silver;
                         //   button18.ForeColor = Color.DarkGray;
                            c.Enabled = false;
                            c.BackColor = Color.Silver;
                            c.ForeColor = Color.DarkGray;
                        }
                    }
                }
            }


            labelUSERno.Text = WSignedId;

            // NOTES 
              Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
              if (Cn.RecordFound == true)
              {
                  labelNumberNotes.Text = Cn.TotalNotes.ToString(); 
              }
              else labelNumberNotes.Text = "0";

            // ****************************************
            // Records for authorisation for this user
            // ****************************************
            Ap.ReadAuthorizationsUserTotal(WSignedId) ;
            if (Ap.RecordFound == true)
            {
                labelAuthNumber.Text = Ap.TotalNumberforUser.ToString();
            }
            else labelAuthNumber.Text = "0";         

            // ....
            // ....
            // Call Procedures to see if serious message

        //    WAtmNo = ""; 

            //MsgFilter =
            //        "(OpenMsg= 1 AND AtmNo= '" + WAtmNo + "' AND AtmNo NOT LIKE '')"
            //    + " OR (OpenMsg= 1 AND ToAllAtms = 1)" 
            //    + " OR (OpenMsg= 1 AND ToUser='" + WSignedId + "')" 
            //    + " OR (OpenMsg= 1 AND FromUser='" + WSignedId + "')" ;

             MsgFilter =
                    "(ReadMsg = 0 AND ToAllAtms = 1)"
                + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')" ;
             
        
            Cm.ReadControlerMSGsSerious(MsgFilter);
            
            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(button51, messagesStatus);
                toolTipMessages.ShowAlways = true;

            }
            else
            {
                
                toolTipMessages.SetToolTip(button51, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }

            toolTipController.SetToolTip(button52, "Communicate with today's controller.");

            checkBox1.Checked = false; 

            //****************************************
            //DESCRIPTIONS 
            //****************************************
            toolTipHeadings.ShowAlways = true;
 
            toolTipButtons.SetToolTip(label2, "INFRASTRUCTURE" + Environment.NewLine
                                         + "Definition of what is needed for system to operate."
                                         );
 
            toolTipButtons.SetToolTip(label3, "OPERATION" + Environment.NewLine
                                         + "Through the below buttons you operate the system."
                                         );

            toolTipButtons.SetToolTip(label4, "MONITORING" + Environment.NewLine
                                         + "Central controller monitors all ATMs by area."
                                         );

            //toolTipButtons.SetToolTip(label1, "DISPUTES" + Environment.NewLine
            //                            + "Register and manage disputes."
            //                            );
            toolTipButtons.SetToolTip(label10, "MIS" + Environment.NewLine
                                       + "Special Reports for management."
                                       );
            toolTipButtons.SetToolTip(label5, "QUALITY" + Environment.NewLine
                                       + "Audit Trail and Exception Reports for KPI."
                                       );
            toolTipButtons.SetToolTip(label11, "ELECTRONIC AUTHORISATIONS" + Environment.NewLine
                                       + "Management of Authorisations for " + Environment.NewLine
                                       + "Requestors and Authorisers ."
                                       ); 
        }

        private void FormMainScreen_Load(object sender, EventArgs e)
        {
            toolTipHeadings.ShowAlways = true;

            //Define Banks 
            toolTipButtons.SetToolTip(label2, "Infrastructure" + Environment.NewLine
                                         + "Definition of what is needed for system to operate."
                                         );

            if (checkBox1.Checked == true)
            {
                toolTipButtons.ShowAlways = true;
                //Define Banks 
                toolTipButtons.SetToolTip(button13, "Definition of Banks based on SWIFT number." + Environment.NewLine
                                             + "Also define a Group of Banks."
                                             );

                ////ATMs Maintenance  
                //toolTipButtons.SetToolTip(button53, "Display all ATMs." + Environment.NewLine
                //                             + "Add, Update, Delete and View details." + Environment.NewLine
                //                             + "Use the Add Like functionality for quick creation of a new ATM." + Environment.NewLine
                //                             + "Use of Google Maps to define the ATM location."
                //                             );

                //Holidays and special days definition.   
                toolTipButtons.SetToolTip(button28, "Define Holidays and Special Days." + Environment.NewLine
                                             + "For simplicity and efficiency next year's holidays are based on the previous year."
                                             );

                ////Users and CIT providers.   
                //toolTipButtons.SetToolTip(button54, "Define Users and Cash In Transit Providers (CIT)." + Environment.NewLine
                //                             + "Define access role levels." + Environment.NewLine
                //                             + "Set access rights at functionality and ATM level."
                //                             );

                //Accounts Management    
                toolTipButtons.SetToolTip(button18, "Define General Ledger accounts per ATM." + Environment.NewLine
                                             + "Based on these Posted Transactions are created."
                                             );

                //ATM Groups    
                toolTipButtons.SetToolTip(button14, "Allocate ATMs into groups." + Environment.NewLine
                                             + "These can be assigned to CIT providers."
                                             );

                ////Matched Days    
                //toolTipButtons.SetToolTip(button29, "For Cash In calculation previous turnover of similar(matched) days is used." + Environment.NewLine
                //                             + "ATM Controller once a month defines the matched days of next period by ATMs category."
                //                             );

                ////Parameters Setting    
                //toolTipButtons.SetToolTip(button21, "Define System Parameters." 
                //                             );

                ////Matched Days exceptions   
                //toolTipButtons.SetToolTip(button55, "Define Matched days for ATMs exceptions." + Environment.NewLine
                //                             + " % Decrease or increase of matched days turnover is updated."
                //                             );

                //Quality Parameters setting   
                toolTipButtons.SetToolTip(button36, "Set Quality key indicators." + Environment.NewLine
                                             + "Depature from these indicators is reported by the system."
                                             );

                ////ATMs migration process 
                //toolTipButtons.SetToolTip(button30, "From excel or file ATMs availble details move to temporary area." + Environment.NewLine
                //                             + "Information is viewed and upadted using ATMs maintenance functionality." + Environment.NewLine
                //                             + "Upon finalisation we have an active ATM."
                //                             );

                // System Performance analysis         
                toolTipButtons.SetToolTip(button74, "Review critical process performance." + Environment.NewLine
                                             + "An analysis is displayed and alerts are sent when beyond predefined limits."
                                             );


                //Training material  
                toolTipButtons.SetToolTip(button56, "View training material." + Environment.NewLine
                                             + "System and operational workflows are available." + Environment.NewLine
                                             + "Circulars, Videos and other documentation can be available."
                                             );

                //Replenishment
                toolTipButtons.SetToolTip(button57, "Replenishment is a workflow of 5 self explanatory steps." + Environment.NewLine
                                            + "At the end a Replenishment Report is printed for signing."
                                            );

                //Reconciliation
                toolTipButtons.SetToolTip(button58, "Reconciliation workflow is required when there are balances in differences due to errors." + Environment.NewLine
                                            + "Transactions to be posted are created.");


                //Mass Reconciliation
                toolTipButtons.SetToolTip(button11, "Mass Reconciliation is done centrally by controller." + Environment.NewLine
                                              + "Functionality for turbo reconciliation is provided and reports are printed.");


                //Captured cards
                toolTipButtons.SetToolTip(button10, "View captured cards info for management." + Environment.NewLine
                                             + "Updated (up to the minute) information is available." + Environment.NewLine
                                             + "A Captured Card form for signing is prepared. ");


                //MY Transactions 
                toolTipButtons.SetToolTip(button59, "View the up to the minute transactions." + Environment.NewLine
                                             + "Different searching criteria can be applied." + Environment.NewLine
                                             + "Transactions files, ejournals and video clips are available."
                                             );

                //Deposits reconciliation 
                toolTipButtons.SetToolTip(button34, "Deposits of all kinds are displayed." + Environment.NewLine
                                             + "Differences can be resolved or moved to Disputes system."
                                             );

                //My ATMS operation  
                toolTipButtons.SetToolTip(button15, "View the Atms current and historical operational status." + Environment.NewLine
                                             + "Though this you can search and view info for all Repl Cycles by period." + Environment.NewLine
                                             + "Google Maps show the location of the ATM."
                                             );

                //Trans to be posted  
                toolTipButtons.SetToolTip(button35, "The transactions are created from reconciliation workflow or from Dispute management actions." + Environment.NewLine
                                             + "Transactions Vouchers are printed."
                                             );

                //Create Actions for ATMs in need 
                toolTipButtons.SetToolTip(button60,  "Identify which ATMS are in need based on the last minute picture." + Environment.NewLine
                                             + "Take the required action." + Environment.NewLine
                                             + " 10 : Normal for replenishment " + Environment.NewLine
                                             + " 11 : Late Replenishment => Today.Now > Next Replenishment date" + Environment.NewLine
                                             + " 12 : Replenish Now not enough money for Today" + Environment.NewLine
                                             + " 13 : Inform G4S to Replenished in two days." + Environment.NewLine
                                             + " 14 : ATM will run out of money during Weekened or Holiday" + Environment.NewLine
                                             + " 15 : Estimated next replenishement date < than next planned date" + Environment.NewLine
                                             + " 16 : ATM appears to have many captured cards and has many Errors"
                                             );

                //Manage Actions 
                toolTipButtons.SetToolTip(button61, "Manage the newly created actions." + Environment.NewLine
                                             + "Send the necessary emails to ATM operators and CIT providers. "
                                             );

                //Calculate money in 
                toolTipButtons.SetToolTip(button62, "Calculate at any time how much money is needed for an ATM." + Environment.NewLine
                                             + "This function can be used just before openning the machine " + Environment.NewLine
                                             + " for replenishment to prepare the necessary amount of money beforehand " + Environment.NewLine
                                             + " and thus speed up the replenishment process." 
                                             );

                //All Needed information for CIT providers 
                toolTipButtons.SetToolTip(button33, "View information for Cash In Transit Providers." + Environment.NewLine
                                             + "Comprehensive up to the minute automatic statement cash statement is displayed."
                                             );

                //Errors Exception management 
                toolTipButtons.SetToolTip(button25, "View errors that need special attention." + Environment.NewLine
                                             + "This is an exception as errors are handled through the specialised workflows."
                                             );

                //E-Journal Drilling 
                toolTipButtons.SetToolTip(button91, "All ejournals are available for interogation." + Environment.NewLine
                                             + "Up to the last minute information is available." + Environment.NewLine
                                             + "E Journals are transformed into formated information of different categories." + Environment.NewLine
                                             + "Transactions of any kind, errors, captured cards, replenishment process."
                                             );

                //Today's operation Status 
                toolTipButtons.SetToolTip(button12, "View current Replenishment and Reconciliation status." + Environment.NewLine
                                             + "Drilling down facilities are available for more details."
                                             );

                //Today's reports  
                toolTipButtons.SetToolTip(button16, "View a list of all daily reports." + Environment.NewLine
                                             + "A number of other reports can be created on customer request."
                                             );

                // Shows the ATMs on MAP         
                toolTipButtons.SetToolTip(button73, "Each ATM is marked on Map." + Environment.NewLine
                                             + "Double click on it to see the exact addreess and type."
                                             );

                //MIS- ATMs replenishment and Reconciliation  operational performance 
                toolTipButtons.SetToolTip(button63, "MIS - Reporting." + Environment.NewLine
                                             + "Useful management information for replenishment and reconciliation per period." + Environment.NewLine
                                             + "Drilling down to individual ATM is available for a more detailed analysis ."
                                             );

                //CIT Provider's performance   
                toolTipButtons.SetToolTip(button26, "MIS - Per period reporting." + Environment.NewLine
                                             + "Cash In Transit providers performance is displayed." + Environment.NewLine
                                             + "Comparison of performance in figures and charts."
                                             );

                //Atms daily business  
                toolTipButtons.SetToolTip(button64, "MIS - Per period reporting." + Environment.NewLine
                                             + "Atms Daily business for withdrawls and deposits is displayed." + Environment.NewLine
                                             + "Drilling down to individual ATM is available for a more detailed analysis ."
                                             );

                //ATMs profitability   
                toolTipButtons.SetToolTip(button20, "View profitablity per ATM." + Environment.NewLine
                                             + "It is based on revenew drivers as well as a number of expense drivers."
                                             );

                //Personnel performance    
                toolTipButtons.SetToolTip(button65, "View performance per person." + Environment.NewLine
                                             + "Based on current up to the minute data departures from the indicators are displayed."
                                             );

                //Authorises Analysis    
                toolTipButtons.SetToolTip(button80, "View performance for Authorisers per person." + Environment.NewLine
                                             + "Quick , Slow, Unacceptable Authorisers." + Environment.NewLine
                                             + "Always matched Requestor Matched ?? Why? "
                                             );

                //Departure from quality indicators     
                toolTipButtons.SetToolTip(button32, "View the expected level of quality defined and agreed with management." + Environment.NewLine
                                             + "Performance drivers can be: time taken for replenishment, number of errors kept outstanding etc."
                                             );

                //System Audit Trail     
                toolTipButtons.SetToolTip(button19, "View activity of system maintenance." + Environment.NewLine
                                             + "Use search to view past changes on system."
                                             );

                //System health Check     
                toolTipButtons.SetToolTip(button31, "The Controller must perform some operation on system to keep it healthy." + Environment.NewLine
                                             + "The delayed or not done work is displayed."
                                             );

                ////Dispute Pre-Investigation      
                //toolTipButtons.SetToolTip(button67, "Before Dispute is registered a pre-investigation can be made to resolve it." + Environment.NewLine
                //                             + "Information about journal, replenishment , reconciliation, errors and video clips " + Environment.NewLine
                //                             + "related to the disputed transaction is readily available."
                //                             );

                ////Dispute Registration       
                //toolTipButtons.SetToolTip(button68, "Register new Disputes." + Environment.NewLine
                //                             + "A Dispute is related to one or many transactions." + Environment.NewLine
                //                             + "A Dispute form is printed to be signed by both the customer and the Bank."
                //                             );

                ////Dispute Management        
                //toolTipButtons.SetToolTip(button69, "The Dispute management is based on a workflow." + Environment.NewLine
                //                             + "Investigation is made and actions are taken." + Environment.NewLine
                //                             + "Transactions to be posted based on decision are generated."
                //                             );

                //Authorization Management        
                toolTipButtons.SetToolTip(button75, "The Pending Authorizations are manage here." + Environment.NewLine
                                             + "You can go and Authorise , or Update action after authorization is made." + Environment.NewLine
                                             + "Also if authorizer is not availble you can undo(Delete) authorization request."
                                             );

                //MIS - Historical Information on disputes        
                toolTipButtons.SetToolTip(button17, "View settled and outstanding disputes by period." + Environment.NewLine
                                             + "Drilling down facilities is available."
                                             );

                //MIS - Settled Disputes by officer         
                toolTipButtons.SetToolTip(button27, "The performance of each officer is measured against the Settled Disputes." + Environment.NewLine
                                             + "Drilling down facilities is available."
                                             );

                //MIS - Refresed testing data         
                toolTipButtons.SetToolTip(button22, "Data are refreshed and Demostration of system can start from the beggining." + Environment.NewLine
                                             + "AB102 is ready for replenishement, reconciliation and trans posting." + Environment.NewLine
                                             + "While AB104, with production rich journal, has already been reconciled." + Environment.NewLine
                                             + "It is ready to show capture cards, transactions, eJournal Drilling down." + Environment.NewLine
                                             + "Also the dispute total cycle reaching the posting of transactions can be demostrated."
                                             );

                //Departure from quality indicators     
                toolTipButtons.SetToolTip(button37, "Check if transactions were updated by Cashier." + Environment.NewLine
                                             + "System suggest and Cashier must act.."
                                             );
               
            }
            else
            {
            
            }  

        }

        // Log Out 
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           // Go to FORM40
            LoggingOut(sender, e);

        }

        // Form Closed

        private void FormMainScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoggingOut(sender, e);
        }
        // Disable / enable Ballon Info 
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox1.Checked)
            {
                toolTipButtons.Active = true;
                FormMainScreen_Load(this, new EventArgs());
            }
            else
            {
                toolTipButtons.Active = false;
           //     FormMainScreen_Load(this, new EventArgs());
            }
        }
        //
        // GO TO ATMS NEW AND MAINTENance 
        // 
        private void button53_Click(object sender, EventArgs e)
        { 
            //TEST
            if (WSecLevel == "06" || WSignedId == "1005" || WSignedId == "SERVE31" || WSignedId == "500")
            {
                NForm108 = new Form108(WSignedId, WSignRecordNo, WOperator);
                NForm108.ShowDialog(); ;
            }
            else
            {
                //   NMessageBoxCustom = new MessageBoxCustom(" YOU CANNOT ACCESS ATMS MAINTENANCE");
                //   NMessageBoxCustom.ShowDialog(); ; 
                MessageBox.Show(" YOU CANNOT ACCESS ATMS MAINTENANCE");
                return;
            }

        }  
        
       
        // Authorised Personnel 

        private void button54_Click(object sender, EventArgs e)
        {
            //TESTING
            /*
            if (WSecLevel < 6)
            {
                MessageBox.Show(" YOU CANNOT ACCESS PERSONEL MAINTENANCE");
                return;
            }
             */
            // Sixth which is zero is CitId
            NForm13 = new Form13(WSignedId, WSignRecordNo,WOperator ,"1000", 1);
            NForm13.ShowDialog(); ;
        }
             
        // Replenishement 
        // REPLENISHMENT ..... 
        // ..........
        // SINGLES
        // WFunction = 1 Normal branch ATM
       
        // GROUPS
        // 5 Normal Group belonging to Bank . 
        private void button57_Click(object sender, EventArgs e)
        {
            Ba.ReadUserAccessToAtms(WSignedId); // READ ALL TABLE ENTRIES TO SEE IF ONE TO ONE 

            if (Ba.NoOfAtmsRepl == 0 & Ba.NoOfGroupsRepl == 0)
            {
                MessageBox.Show(" YOU ARE NOT AUTHORISED TO DO REPLENISHMENT ");
                return;
            }

            if (Ba.NoOfGroupsRepl == 0) WAction = 1; // Replenishment for individual of more than one ATM,
            Us.ReadUsersRecord(WSignedId); // READ USER DATA
            if (Ba.NoOfGroupsRepl > 0) WAction = 5; // Replenishment for Group of ATM 

            NForm152 = new Form152(WSignedId, WSignRecordNo,WOperator, WAction);
            NForm152.ShowDialog();
        }   
       
       

        // Reconciliation 
        // RECONCILIATION ..... 
        // ........
        // 

        private void button58_Click(object sender, EventArgs e)
        {
            Ba.ReadUserAccessToAtms(WSignedId); // READ TO SEE GROUP OR ATM 

            if (Ba.NoOfAtmsReconc == 0 & Ba.NoOfGroupsReconc == 0)
            {
                MessageBox.Show(" YOU ARE NOT AUTHORIZE FOR RECONCILIATION ");
                return;
            }

            if (Ba.NoOfGroupsReconc > 0)
            {
                MessageBox.Show(" YOU HAVE A GROUP. Push the GROUP Button please ");
                return;
            }


            if (Ba.NoOfAtmsReconc > 0)
            {
                WAction = 2; // Reconciliation for individual ATMs

                NForm152 = new Form152(WSignedId, WSignRecordNo, WOperator, WAction);
                NForm152.ShowDialog(); ;
            }         
        }

      
        // Mass reconciliation - Reconciliation by Group 

        private void button11_Click(object sender, EventArgs e)
        {
            WAction = 11; // Reconciliation for Group of ATMs

            MessageBox.Show(" YOU ARE NOT AUTHORIZED FOR MASS RECONCILIATION ");
            return;

            /*
            NForm52 = new Form52(WSignedId, WSignRecordNo, WOperator, WPrive, WAction);
            NForm52.ShowDialog(); ;      
            
            Ba.ReadUsersAccessAtmTable(WSignedId);

            if ( Ba.NoOfGroupsReconc == 0 )
            {
                MessageBox.Show(" YOU ARE NOT AUTHORIZED FOR MASS RECONCILIATION ");
                return;
            }

            if (Ba.NoOfGroupsReconc > 0) // YOU HAVE A GROUP TO RECONCILE 
            {
                 WAction = 11; // Reconciliation for Group of ATMs

                NForm52 = new Form52(WSignedId, WSignRecordNo, WOperator, WPrive, WAction);
                NForm52.ShowDialog(); ;              
            }
             */
        }

        // Capture Cards Management 
        private void button10_Click(object sender, EventArgs e)
        {
            WAction = 3; // Cards Management 
            NForm152 = new Form152(WSignedId, WSignRecordNo, WOperator, WAction);
            NForm152.ShowDialog(); ;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

        }
       
        // CIRCULARS AND VIDEO FOR TRAININGS 
        private void button56_Click(object sender, EventArgs e)
        {
            NForm300 = new Form300();
            NForm300.ShowDialog(); ;
        }
       
       
        // Controller's STATS
        private void button12_Click(object sender, EventArgs e)
        {
            // YOU must have the right security level to allow this
            // This is available to Controller for one bank or for many banks  
              
               
                RRDMBanks Ba = new RRDMBanks();
                Ba.ReadBank(WOperator);
                Ba.ReadNoBanksInGroup(Ba.GroupName);
                if (Ba.BanksInGroup > 1)
                {
                    int Wf = 0; 
                    NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel,WOperator, Ba.GroupName,Wf);
                    NForm136.ShowDialog(); ;
                }

                if (Ba.BanksInGroup == 1)
                {   // 0 stands for CitNo... CitNo>0 when calling from Form18 (Cit View)

                    NForm200 = new Form200(WSignedId, WSignRecordNo, WSecLevel, WOperator, "1000");
                    NForm200.ShowDialog(); ;
                }
            
        }
          
        

        // Dispute Pre Investigation 
        private void button67_Click(object sender, EventArgs e)
        {
            //NForm116 = new Form116XXX(WSignedId, WSignRecordNo, WOperator);
            //NForm116.ShowDialog(); ;
        }   
      
        // DISPUTE REGISTRATION  
        private void button68_Click(object sender, EventArgs e)
        {
            int From = 1;
            NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator,"",  0, 0, 0 ,"",From,"");
            NForm5.ShowDialog(); ;
        }
         
        // MANAGE DISPUTES 
        //private void button69_Click(object sender, EventArgs e)
        //{
        //    NForm3 = new Form3(WSignedId, WSignRecordNo, WOperator,"",Nul);


        //    NForm3.ShowDialog();
        //}

        // AUthorize Dispute Decisions 
      
        private void button75_Click(object sender, EventArgs e)
        {
            string TempAtmNo = "";
            int TempSesNo = 0;
            int WDisputeNo = 0;
            int WDisputeTranNo = 0;
            NForm112 = new Form112(WSignedId, WSignRecordNo, WOperator, "Normal", TempAtmNo, TempSesNo, WDisputeNo, WDisputeTranNo, "", 0);
            NForm112.ShowDialog();
        }
                       
        // GO TO BANKS 
        private void button13_Click(object sender, EventArgs e)
        {
            /*
            if (WSecLevel < 9 )
            {
                 MessageBox.Show(" ONLY GLOBAL GAS MASTER USER IS ALLOWED TO GO TO BANKS ");
                 return; 
            }
             */
            NForm143 = new Form143(WSignedId, WSignRecordNo, WOperator);
            NForm143.ShowDialog(); ;
            
          //  this.Hide(); 
        }
        // MAINTENACE OF GROUPS 
        private void button14_Click(object sender, EventArgs e)
        {
            // READ DETAILS OF ACCESS RIGHTS AND GO TO FORM1 
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo); 
            //
            //NFormMainScreen = new FormMainScreen(WSignedId, WSignRecordNo, Usi.SecLevel, WOperator);  
            //NFormMainScreen.ShowDialog();
          
        }
        //
        // SHOW MY ATMs 
        //
        private void button15_Click(object sender, EventArgs e)
        {
            //if (WSecLevel > 4)
            //{
            //    MessageBox.Show(" THIS BUTTON IS ONLY FOR OPERATIONAL OFFICERS");
            //    return;
            //}

            WAction = 1; // Show INFO FOR ATMS 
            NForm47 = new Form47(WSignedId, WSignRecordNo, WOperator ,"",  WAction);
            NForm47.ShowDialog(); ;
       
        }

        //
        // ERRORS MANAGEMENT 
        // 

        private void button25_Click(object sender, EventArgs e)
        {

            //if (WSecLevel > 4)
            //{
            //    MessageBox.Show(" THIS BUTTON IS ONLY FOR OPERATIONAL OFFICERS");
            //    return;
            //}


            WAction = 2; // Show INFO FOR ATMS 
            NForm47 = new Form47(WSignedId, WSignRecordNo, WOperator ,"", WAction);
            NForm47.ShowDialog(); ;
            // this.Close();
        }

        void NForm49_FormClosed(object sender, FormClosedEventArgs e)
        {
         //   MainScreen_Load(this, new EventArgs());
        }

        // Refresh Testing data 
        //
        private void button22_Click(object sender, EventArgs e)
        {
            // 
            string connectionString = ConfigurationManager.ConnectionStrings
              ["ATMSConnectionString"].ConnectionString;

            string RCT = "ATMS.[dbo].[Stp_Refresh_Testing_Data]";

            using (SqlConnection conn =
               new SqlConnection(connectionString))
                try
                {
                    conn.Open();
                    using (SqlCommand cmd =
                       new SqlCommand(RCT, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Parameters

                        int rows = cmd.ExecuteNonQuery();
                        //    if (rows > 0) textBoxMsg.Text = " RECORD INSERTED IN SQL ";
                        //    else textBoxMsg.Text = " Nothing WAS UPDATED ";

                    }
                    // Close conn
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                }

            MessageBox.Show("Testing Data have been initialised"); 

        }
        //
        // THIS GOES TO ATMS IN NEED
        //
        private void button60_Click(object sender, EventArgs e)
        {
            //if (WSecLevel > 4)
            //{
            //    MessageBox.Show(" THIS BUTTON IS ONLY FOR OPERATIONAL OFFICERS - Sec Level 3 and 4");
            //    return;
            //}

            NForm50 = new Form50(WSignedId, WSignRecordNo, WOperator);
            NForm50.ShowDialog(); ;
        }

        // GO TO MANAGE ACTIONS 
        private void button61_Click(object sender, EventArgs e)
        {
            //TEST
            /*
            if (WSecLevel > 4)
            {
                MessageBox.Show(" THIS BUTTON IS ONLY FOR OPERATIONAL OFFICERS - Sec Level 3 and 4");
                return;
            }
             */

            NForm63 = new Form63(WSignedId, WSignRecordNo, WOperator);
            NForm63.ShowDialog(); ;
        }

       
       
        // Message from Controller 

        private void button51_Click(object sender, EventArgs e)
        {
            NForm55 = new Form55(MsgFilter, WSignedId);
            NForm55.ShowDialog();

            MsgFilter =
                   "(ReadMsg = 0 AND ToAllAtms = 1)"
               + " OR (ReadMsg = 0 AND ToUser='" + WSignedId + "')";

           
            Cm.ReadControlerMSGsSerious(MsgFilter);

            if (Cm.SerMsgCount > 0)
            {
                string messagesStatus = " You have " + Cm.SerMsgCount.ToString() + " personal messages from the controller.";

                toolTipMessages.SetToolTip(button51, messagesStatus);
                toolTipMessages.ShowAlways = true;

            }
            else
            {

                toolTipMessages.SetToolTip(button51, "Check messages from controller.");
                toolTipMessages.ShowAlways = true;
            }
        }
        /*
        private void buttonMsgs_Click(object sender, EventArgs e)
        {
            NForm55 = new Form55(MsgFilter, WSignedId);
            NForm55.ShowDialog();
        }
         */
        // Todays Controller 

        private void button52_Click(object sender, EventArgs e)
        {
            NForm54 = new Form54(WSignedId, WSignRecordNo, WOperator);
            NForm54.ShowDialog();
        }
        
        // My Transactions 
        private void button59_Click_1(object sender, EventArgs e)
        {
            WAction = 7; // Go to Print Transactions by session or period  
            NForm52 = new Form52(WSignedId, WSignRecordNo, WOperator, WAction);
            NForm52.ShowDialog(); ;
        }
       
       
        //
        // G4S Can calculate what money to put in before going for replenishment 
        //
        private void button62_Click(object sender, EventArgs e)
        {
            WAction = 8; // 
            NForm152 = new Form152(WSignedId, WSignRecordNo, WOperator, WAction);
            NForm152.ShowDialog(); ;
        }
     

        //PRESENT AND MANAGE DEPOSITS
        private void button34_Click(object sender, EventArgs e)
        {
            // Go to FORM52 
            // SHOW deposits
            // See them on Journal
            // Input amounts for exceptions

            // Move exceptions to soft disputes 

            // Print report for approval and filing 

            WAction = 4;
            NForm152 = new Form152(WSignedId, WSignRecordNo, WOperator, WAction);
            NForm152.ShowDialog(); ;
        }

        //
        // NEW FORM ... NEW FORM 
        // 
        // EXCEPTIONS Figuers for replenishemnt 
        private void button55_Click(object sender, EventArgs e)
        {
            WAction = 1;
            NForm10 = new Form10(WSignedId, WSignRecordNo, WSecLevel, WOperator,WAction);
            NForm10.ShowDialog(); ;
        }
       

        // MIS REPORTS 

        private void button63_Click(object sender, EventArgs e)
        {
            // YOU must have the right security level to allow this
            // This is available to Controller for one bank or for many banks  

         
                // 
                RRDMBanks Ba = new RRDMBanks();
                Ba.ReadBank(WOperator);
                Ba.ReadNoBanksInGroup(Ba.GroupName);
                if (Ba.BanksInGroup > 1)
                {
                    int WF = 1;
                    NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel,WOperator, Ba.GroupName, WF);
                    NForm136.ShowDialog(); ;
                }

                if (Ba.BanksInGroup == 1)
                {
                    WAction = 1;

                    NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
                    NForm21MIS.ShowDialog(); ;
                }
            
        }
      
        // MIS REPORTS - PERSONNEL PERFORMANCE 

        private void button65_Click(object sender, EventArgs e)
        {
            // YOU must have the right security level to allow this
            // This is available to Controller for one bank or for many banks  

        
                // 
                RRDMBanks Ba = new RRDMBanks();
                Ba.ReadBank(WOperator);
                Ba.ReadNoBanksInGroup(Ba.GroupName);
                if (Ba.BanksInGroup > 1)
                {
                    int WF = 2;
                    NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel, WOperator, Ba.GroupName, WF);
                    NForm136.ShowDialog(); ;
                }

                if (Ba.BanksInGroup == 1)
                {
                    WAction = 2;

                    NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator,  WAction);
                    NForm21MIS.ShowDialog(); ;
                }
            
        }
     

        // MIS REPORTS - ATMS DAILY BUSINESS 

      
        private void button64_Click(object sender, EventArgs e)
        {
                // 
                RRDMBanks Ba = new RRDMBanks();
                Ba.ReadBank(WOperator);
                Ba.ReadNoBanksInGroup(Ba.GroupName);
                if (Ba.BanksInGroup > 1)
                {
                    int WF = 3;
                    NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel,WOperator, Ba.GroupName, WF);
                    NForm136.ShowDialog(); ;
                }

                if (Ba.BanksInGroup == 1)
                {
                    WAction = 3;

                    NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
                    NForm21MIS.ShowDialog(); ;
                }
            
        }

        // MIS REPORTS - ATMS PROFITABILITY  

        private void button20_Click(object sender, EventArgs e)
        {    
                // 
                RRDMBanks Ba = new RRDMBanks();
                Ba.ReadBank(WOperator);
                Ba.ReadNoBanksInGroup(Ba.GroupName);
                if (Ba.BanksInGroup > 1)
                {
                    int WF = 6;
                    NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel, WOperator, Ba.GroupName, WF);
                    NForm136.ShowDialog(); ;
                }

                if (Ba.BanksInGroup == 1)
                {
                    WAction = 6;

                    NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
                    NForm21MIS.ShowDialog(); ;
                }
            
        }
        // CIT PROVIDERS 
        private void button26_Click(object sender, EventArgs e)
        {   
                // 
                RRDMBanks Ba = new RRDMBanks();
                Ba.ReadBank(WOperator);
                Ba.ReadNoBanksInGroup(Ba.GroupName);
                if (Ba.BanksInGroup > 1)
                {
                    int WF = 4;
                    NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel, WOperator, Ba.GroupName, WF);
                    NForm136.ShowDialog(); ;
                }

                if (Ba.BanksInGroup == 1)
                {
                    WAction = 4;

                    NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
                    NForm21MIS.ShowDialog(); ;
                }
            
        }

        // DISPUTES MIS 
        private void button17_Click(object sender, EventArgs e)
        {
            
                // 
                RRDMBanks Ba = new RRDMBanks();
                Ba.ReadBank(WOperator);
                Ba.ReadNoBanksInGroup(Ba.GroupName);
                if (Ba.BanksInGroup > 1)
                {
                    int WF = 5;
                    NForm136 = new Form136(WSignedId, WSignRecordNo, WSecLevel, WOperator, Ba.GroupName, WF);
                    NForm136.ShowDialog(); ;
                }

                if (Ba.BanksInGroup == 1)
                {
                    WAction = 5;

                    NForm21MIS = new Form21MIS(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
                    NForm21MIS.ShowDialog(); ;
                }
            
        }

     
        
        private void button23_Click(object sender, EventArgs e)
        {
            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog();
        }

        // General Audit trail 
        // This will be based on screen image captured 

        private void button19_Click(object sender, EventArgs e)
        {
            Form76 NForm76 = new Form76(WSignedId, WSignRecordNo, WOperator);
            NForm76.ShowDialog(); ;
            //MessageBox.Show("General Audit trail this will be based on screen image captured + record registration id");  
        }
        // WORD 
        private void button24_Click(object sender, EventArgs e)
        {
          //  Form66 newForm66 = new Form66();
          //  newForm66.ShowDialog(); ;
        }
 
        // DAILY REPORTS 
        private void button16_Click(object sender, EventArgs e)
        {
            int Wf = 0;
            NForm9 = new Form9(WSignedId, WSignRecordNo, WSecLevel, WOperator, Wf);
            NForm9.ShowDialog(); ;

        }
        // MATCHED DATES
        private void button29_Click(object sender, EventArgs e)
        {
            WAction = 1;
            NForm12 = new Form12(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm12.ShowDialog(); ;
        }
      
        // ATMs Migration 
        private void button30_Click(object sender, EventArgs e)
        {

            MessageBox.Show("Future development " + " Bank fills in an excel with Info. Excel is loaded to the system." 
                            + " ATM/s are activated after data are verified by User. "); 
        }

       
        // ATMs Controller's Health Check
        private void button31_Click(object sender, EventArgs e)
        {
            NForm81 = new Form81(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm81.ShowDialog(); ;
        }
        // DEPATURE FROM QUALITY INDICATORS
        private void button32_Click(object sender, EventArgs e)
        {
            NForm193 = new Form193(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm193.ShowDialog(); ;
        }
        // CIT Providers 
        private void button33_Click(object sender, EventArgs e)
        {
          /*  int ParSec = 100 + WSecLevel ; // 103 say 
            Gp.ReadParametersSpecificNm(ParSec, "button33");
            if (Gp.RecordFound == false)
            {
                MessageBox.Show("Not Authorised to access this button"); 
            }
           */
            WAction = 1;
            NForm18 = new Form18(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm18.ShowDialog(); ;
        }
       

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {          
            CheckForMessages(); // Check for messages from controller

            RRDMAuthorisationProcess Ap = new RRDMAuthorisationProcess();
            Ap.CheckOutstandingAuthorizationsStage1(WSignedId); // There is record for authorizer with Stage 1 
            if (Ap.RecordFound == true)
            {
                Console.Beep(2000, 500);
                Console.Beep(3000, 1000);
                Console.Beep(2000, 500);
                MessageBox.Show(new Form { TopMost = true }, "You have requests for authorization. From : " + Ap.Requestor
                    );
            }

            Ap.CheckOutstandingAuthorizationsStage3(WSignedId); // Threre is record for Requestor with stage 3 
            if (Ap.RecordFound == true)
            {
                Console.Beep(2000, 500);
                Console.Beep(3000, 1000);
                Console.Beep(2000, 500);
                MessageBox.Show(new Form { TopMost = true }, "Authorization has been made. by : " + Ap.Authoriser);
            }

            // ****************************************
            // Records for authorisation for this user
            // ****************************************
            Ap.ReadAuthorizationsUserTotal(WSignedId);
            if (Ap.RecordFound == true)
            {
                labelAuthNumber.Text = Ap.TotalNumberforUser.ToString();
            }
            else labelAuthNumber.Text = "0";     
        }

        int NumberMsgs = 0;

        private void CheckForMessages()
        {
            //SQL command to get number of unread messages
            RRDMControllerMsgClass cmc = new RRDMControllerMsgClass();
            int NewNumberMsgs = cmc.CountUnreadMSGs(WSignedId);

            //If different than existing then update the number and notify use
            int OldNumberOfMsgs = NumberMsgs;

            NumberMsgs = NewNumberMsgs;

            if (NumberMsgs == 0)
            {
                labelNumberMsgs.Text = "0";
                labelNumberMsgs.Visible = false;
            }
            else
            {
                labelNumberMsgs.Visible = true;
                labelNumberMsgs.Text = NumberMsgs.ToString();
            }

            if(OldNumberOfMsgs<NewNumberMsgs)
            {

                MessageBox.Show(new Form { TopMost = true }, "You have new messages.");
            }
        }

        private void labelNumberMsgs_Click(object sender, EventArgs e)
        {

        }  
        //
        // PARAMETERS MANAGEMENT 
        //
        private void button21_Click(object sender, EventArgs e)
        {
            NForm191 = new Form191(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm191.ShowDialog();
        }
        // MANAGE TRANSACTIONS TO BE POSTED
        // These are created after reconciliation 
        private void button35_Click(object sender, EventArgs e)
        {
            int Mode = 1;
            NForm78 = new Form78(WSignedId, WSignRecordNo,WOperator ,"", 0, 0, Mode);
            NForm78.ShowDialog(); ;
        }
        // DEFINE QUALITY INDICATORS 
        private void button36_Click(object sender, EventArgs e)
        {
            NForm192 = new Form192(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm192.ShowDialog(); ;
        }
        // ACCOUNTS MANAGEMENT 
        private void button18_Click_1(object sender, EventArgs e)
        {
            // Sixth which is zero is CitNo
            //NForm82 = new Form82(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            //NForm82.ShowDialog(); ;

            NForm85 = new Form85(WSignedId, WSignRecordNo, WSecLevel, WOperator);
            NForm85.ShowDialog(); ;
        }
        // EXIT 
        private void button72_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        // MIS FOR DISPUTES 
        private void button27_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + " Closed Disputes by Officer.");
        }
        // E_JOURNAL DRILLING 
       
        private void button91_Click(object sender, EventArgs e)
        {
            NForm53 = new Form53(WSignedId, WSignRecordNo, WOperator);
            NForm53.ShowDialog(); ;
        }
        //
        // System Performance
        //
        private void button74_Click(object sender, EventArgs e)
        {
            WAction = 1;

            NForm8 = new Form8(WSignedId, WOperator, "", WAction);
            NForm8.ShowDialog(); ;

        }

        // GO to Matching of outstanding postings 

        private void button37_Click(object sender, EventArgs e)
        {
            WAction = 1;

            NForm19 = new Form19a(WSignedId, WSignRecordNo, WSecLevel, WOperator, WAction);
            NForm19.ShowDialog(); ;
        }

        private void button73_Click(object sender, EventArgs e)
        {
            if (HasInternet())
            {
                try
                {
                    // Read the URL from app.config
                    string MainURL = ConfigurationManager.AppSettings["RRDMMapsMainURL"];
                    // Invoke default browser
                    ProcessStartInfo sInfo = new ProcessStartInfo(MainURL);
                    Process.Start(sInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Maps cannot be shown. There might be a problem with Google Maps" + ex.Message);
                }         
            }
            else
            {
                MessageBox.Show("There is no Internet Connection");
            }

        }

        

        // Copy Trans from AB102 to ServeUk102
        private void button1_Click(object sender, EventArgs e)
        {
            // STAVROS CALCULATION 
            NForm2Testing = new Form2Testing();
            NForm2Testing.ShowDialog(); ;

       //     TransAndPostedClass Tc = new TransAndPostedClass();
       //     HostTransClass Ht = new HostTransClass();
       //     AtmsDailyTransHistory Ah = new AtmsDailyTransHistory(); 
       //     GasParameters Gp = new GasParameters(); 
        //      string BankId = "ServeUk";
      //      string AtmNo = "ServeUk102";
         //     string BankId = "CRBAGRAA";

      //      Gp.CopyParameters("ServeUk","CRBAGRAA");
      //      string AtmNo = "AB102";
      //      int SesNo = 6694 ;
      //      string TargetBank = "ServeUk";
      //      string TargetAtm = "ServeUk102";
      //      int TargetSesNo = 6695 ;
     //       bool TargetPrive = false; 

            // Pool Transactions
  //          Tc.CopyInPoolTrans(BankId, AtmNo, SesNo, TargetBank, TargetAtm, TargetSesNo, TargetPrive);

            // Host Trans 
            //  Ht.CopyHostTrans(BankId, AtmNo, TargetBank, TargetAtm, TargetPrive);

            // Copy History 
  //          Ah.CopyTransHistory(BankId, AtmNo, TargetBank, TargetAtm, TargetPrive);

     //       Ht.CopyFromPoolToTrans();// Copies on in pool record to host file 

      //      MessageBox.Show("Trans copied from: " + AtmNo + " To :" + TargetAtm) ; 

        }
        // Future MIS
        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + " Transaction Mixed + Balance enquiries + Withdrawls + Deposits + Etc.");
        }
        // Future MIS
        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + " Service Level per Model. Is NCR faster than Diebold? ");
        }
        // Future MIS 
        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + " Destination of Transactions , what Own systems , other destinations not own? ");
        }
        // Future MIS 
        private void button80_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + "Authorisers = the Good , the Bad and the Neutral ");
        }
        // Future Module 
        private void button76_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + "Module that provides information to the Banks' corporate WareHouse. ");
        }
        // NOTES
        private void buttonNotes_Click(object sender, EventArgs e)
        {
            Form197 NForm197;
            string WParameter3 = ""; 
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, "", WParameter3, WParameter4,"Update", WSearchP4);
            NForm197.FormClosed += NForm197_FormClosed;
            NForm197.ShowDialog();
        }

        void NForm197_FormClosed(object sender, FormClosedEventArgs e)
        {
            // NOTES 
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes.Text = "0";
        }

         
// Money Notes quality 
        private void button81_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Future development " + Environment.NewLine
                + "Based on Reject Bin volumes quality of insert Notes is measured " + Environment.NewLine 
                + "Excess by the machine Reject is an evidence that quality of Notes is not that good!");
        }

// Software version details 
        private void button6_Click(object sender, EventArgs e)
        {
           
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();

        }
// Make RED 
        private void button2_Click_1(object sender, EventArgs e)
        {

            Color Red = Color.Red;
            button91.BackColor = Red;
            button13.BackColor = Red;
            //button53.BackColor = Red;
            //button54.BackColor = Red;
            button14.BackColor = Red;
            //button21.BackColor = Red;
            button12.BackColor = Red;
            button73.BackColor = Red;

            button15.BackColor = Red;
            button10.BackColor = Red;
            //button34.BackColor = Red;
            button59.BackColor = Red;
            button25.BackColor = Red;

            button26.BackColor = Red;
            button64.BackColor = Red;
            button65.BackColor = Red;

            button28.BackColor = Red;
            button18.BackColor = Red;
            //button29.BackColor = Red;
            //button55.BackColor = Red;

            button33.BackColor = Red;

            button60.BackColor = Red;
            button61.BackColor = Red;
        }
// Make Silver 
        private void button3_Click_1(object sender, EventArgs e)
        {
            Color Silver = Color.Silver;
            Color Black = Color.Black;
            button36.BackColor = Silver;
            button36.ForeColor = Black;
            button57.BackColor = Silver;
            button57.ForeColor = Black;
            button58.BackColor = Silver;
            button58.ForeColor = Black;
            button34.BackColor = Silver;
            button34.ForeColor = Black;
            //button67.BackColor = Silver;
            //button67.ForeColor = Black;
            //button68.BackColor = Silver;
            //button68.ForeColor = Black;
            //button69.BackColor = Silver;
            //button69.ForeColor = Black;
            button63.BackColor = Silver;
            button63.ForeColor = Black;
            button20.BackColor = Silver;
            button20.ForeColor = Black;
            button17.BackColor = Silver;
            button17.ForeColor = Black;
            button75.BackColor = Silver;
            button75.ForeColor = Black;
            button35.BackColor = Silver;
            button35.ForeColor = Black;
            button37.BackColor = Silver;
            button37.ForeColor = Black;
            button32.BackColor = Silver;
            button32.ForeColor = Black;
            button74.BackColor = Silver;
            button74.ForeColor = Black;
            button31.BackColor = Silver;
            button31.ForeColor = Black;
            button11.BackColor = Silver;
            button11.ForeColor = Black;
        }
// Start Reporting Services 
        private void button4_Click_1(object sender, EventArgs e)
        {
            string P1 = "ATMs Basic Information ";

            string P2 = "";
            string P3 = "";
            string P4 = WOperator;
            string P5 = WSignedId;

            Form56R11 AtmsBasic = new Form56R11(P1, P2, P3, P4, P5);
            AtmsBasic.Show();
        }
// Check Internet 
        private void button5_Click(object sender, EventArgs e)
        {
            if (HasInternet())
            {
                MessageBox.Show("There is Internet Connection");
            }
            else
            {
                MessageBox.Show("There is NO Internet Connection");
            }

            RRDMEmailClass2 Em = new RRDMEmailClass2();

            string Recipient = "panicos.michael@cablenet.com.cy";

            string Subject = "Replenish ATM";

            string WEmailContent = "Testing Internet";

            Em.SendEmail(WOperator, Recipient, Subject, WEmailContent);
            if (Em.MessageSent == true)
            {
                MessageBox.Show("NO Problem with Google Email.");
            }
            else
            {
                MessageBox.Show("There is problem with Google Email." + Environment.NewLine 
                    + " Email is used at differrent locations at the same time." + Environment.NewLine
                    + " Google has blocked acount for this reason");
            }
        }

        private bool HasInternet()
        {
            try
            {
                using (var client = new System.Net.WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }
// Assist - show Panel4 
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAssist.Checked == true)
            {
                panel4.Show();
            }
            else
            {
                panel4.Hide();
            }
        }
      
    }
}
