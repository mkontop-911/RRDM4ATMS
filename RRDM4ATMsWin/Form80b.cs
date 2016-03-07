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

// Alecos
using System.Configuration;
using System.Diagnostics;

namespace RRDM4ATMsWin
{
    public partial class Form80b : Form
    {
        RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass(); 
        RRDMReconcCategories Rc = new RRDMReconcCategories();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions(); 
       
        RRDMReconcCategoriesMatchingSessions Rms = new RRDMReconcCategoriesMatchingSessions();

        RRDMTransAndTransToBePostedClass Tp = new RRDMTransAndTransToBePostedClass(); 

        RRDMCaseNotes Cn = new RRDMCaseNotes();

        // NOTES START
        string Order;
        string WParameter4;
        string WSearchP4;
        string WMode; 
        // NOTES END 

        int WUniqueNo;
        int WRRN;

        DateTime FromDt;
        DateTime ToDt; 

        int WMaskRecordId;
        string WInputField; 
        string WSortValue;

        bool ATMTrans; 

        string WFileIdForBoth; 

        string WFilter;

        bool FirstCycle; 

        string WMask; 
        string WSubString;
        
        //string WFileName;
        //bool Matched;

        string WhatFile;

        //string WUnMatchedFile;
        //string WMatchedFile; 
     
        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        
        string WCategoryId;
        int WRMCycleNo;
        int WInMaskRecordId; 
        string WFunction; 

        public Form80b(string InSignedId, int InSignRecordNo, string InOperator, string InCategoryId, int InRMCycleNo,
                                                  int InMaskRecordId,string InFunction)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCategoryId = InCategoryId;
            WRMCycleNo = InRMCycleNo;

            WInMaskRecordId = InMaskRecordId; 

            WFunction = InFunction; // "Reconc", "View", "Investigation"

            InitializeComponent();

            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
            labelUserId.Text = InSignedId;

            if (WFunction == "Investigation")
            {
                labelStep1.Text = "Dispute Investigation Process" ;
            }
            else
            {
                labelStep1.Text = "Matched and Unmatched Files of RM Category : " + WCategoryId + "/ RM Cycle : " + WRMCycleNo;
            }
            

            if (WFunction == "View")
            {
                label17.Hide();
                panel24.Hide();
                textBoxMsgBoard.Text = "View Only"; 
            }
            if (WFunction == "Interactive A") // FOR ACTION ON UNMATCHED 
            {
                label17.Show();
                panel24.Show(); 
            }

            FirstCycle = true; 
            // SELECT

            comboBoxFilter.Items.Add("Matched");

            comboBoxFilter.Items.Add("UnMatched");

            //comboBoxFilter.Items.Add("UnMatchedAllCycles");

            comboBoxFilter.Items.Add("Both");

            comboBoxFilter.Text = "Matched";

            // SORT 

            comboBoxSort.Items.Add("CardNo");

            comboBoxSort.Items.Add("AccountNo");

            comboBoxSort.Items.Add("SeqNo");

            comboBoxSort.Text = "SeqNo";

            //// Unique 

            //comboBoxUnique.Items.Add("SearchInCurrentRMCycle");

            //comboBoxUnique.Items.Add("SearchInAllCyclesThisCateg");

            //comboBoxUnique.Items.Add("SearchInAll-RMCategories");

            //comboBoxUnique.Items.Add("NoUnique");

            //comboBoxUnique.Text = "NoUnique";

            dateTimePicker1.Value = new DateTime(2015, 08, 28);

            radioButton1.Checked = true;

            panel5.Hide();

            if (WFunction == "Investigation")
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
                comboBoxFilter.Text = "Both";
                comboBoxSort.Text = "SeqNo";
                checkBoxUnique.Checked = true;
                radioButtonCard.Checked = true;
                panel9.Hide();
                checkBoxUnique.Hide();

                buttonRegisterDispute.Show(); 
                
                textBoxInputField.Text = "4375071234567892";
               
                textBoxMsgBoard.Text = "Select dates, and other and press Show. ";  
            }
            else
            {
                buttonRegisterDispute.Hide();
            }           
        }
// Load 
        private void Form80b_Load(object sender, EventArgs e)
        {
            buttonMovedToUnMatched.Show();
            buttonMoveToMatched.Show(); 

            if (comboBoxFilter.Text == "UnMatched")
            {
                WhatFile = "UnMatched";
                buttonMovedToUnMatched.Hide(); 
            }

            if (comboBoxFilter.Text == "Matched")
            {
                WhatFile = "Matched";
                buttonMoveToMatched.Hide(); 
            }

            if (comboBoxFilter.Text == "Both")
            {
                WhatFile = "Both";
                buttonMoveToMatched.Hide();
            }

            if (checkBoxUnique.Checked == false) // Not Unique 
            //if (comboBoxUnique.Text == "NoUnique")
            {
                if (comboBoxSort.Text == "SeqNo") WSortValue = "SeqNo";
                if (comboBoxSort.Text == "CardNo") WSortValue = "CardNumber";
                if (comboBoxSort.Text == "AccountNo") WSortValue = "AccNumber";

                if (radioButton1.Checked == true) // Only this category and this Cycle
                {
                    WFilter = "Operator = @Operator AND RMCateg ='" + WCategoryId + "' AND RMCycle = " + WRMCycleNo + " AND OpenRecord = 1 ";

                    if (WFunction == "View" || WFunction == "Investigation")
                    {
                        WFilter = "Operator = @Operator AND RMCateg ='" + WCategoryId + "' AND RMCycle = " + WRMCycleNo; // Open and closed 
                      
                    }
                }
                if (radioButton2.Checked == true) // All transactions
                {
                    WFilter = "Operator = @Operator AND OpenRecord = 1 "; // Only open

                    if (WFunction == "View" || WFunction == "Investigation")
                    {
                        WFilter = "Operator = @Operator"; // Open and closed 
                    }
                }

            }
            else
            {
                // Unique
                if (radioButtonCard.Checked == false & radioButtonAccount.Checked == false & radioButtonUniqueNo.Checked == false)
                {
                    MessageBox.Show("Please select and Continue ");
                    return; 
                }

                if (textBoxInputField.Text == "")
                {
                    if (WFunction != "Investigation")
                                 MessageBox.Show("Please enter value!");
                    return;
                } 

                WInputField = textBoxInputField.Text;

                if (radioButtonCard.Checked == true) // Card 
                {
                    if (radioButton1.Checked == true) // Only this category and this Cycle
                    {
                        WFilter = "Operator = @Operator AND RMCateg ='" + WCategoryId + "' AND RMCycle = " + WRMCycleNo
                                        + " AND CardNumber ='" + WInputField + "' AND OpenRecord = 1 ";

                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter = "Operator = @Operator AND RMCateg ='" + WCategoryId + "' AND RMCycle = " + WRMCycleNo
                                         + " AND CardNumber ='" + WInputField + "'";  // Open and closed 

                        }
                    }
                    if (radioButton2.Checked == true) // All transactions OR this MAskRecordId
                    {
                        WFilter = "Operator = @Operator " + " AND CardNumber ='" + WInputField + "' AND OpenRecord = 1 ";  // Only open

                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter = "Operator = @Operator " + " AND CardNumber ='" + WInputField + "'";  // Open and closed 
                            if (WInMaskRecordId > 0)
                                WFilter = "Operator = @Operator AND MaskRecordId = " + WInMaskRecordId; // Only this transaction  
                        }
                    }

                }

                if (radioButtonAccount.Checked == true) // Account 
                {
                    if (radioButton1.Checked == true) // Only this category and this Cycle
                    {
                        WFilter = "Operator = @Operator AND RMCateg ='" + WCategoryId + "' AND RMCycle = " + WRMCycleNo
                                        + " AND AccNumber ='" + WInputField + "' AND OpenRecord = 1 ";

                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter = "Operator = @Operator AND RMCateg ='" + WCategoryId + "' AND RMCycle = " + WRMCycleNo
                                         + " AND AccNumber ='" + WInputField + "'";  // Open and closed 
                        }
                    }
                    if (radioButton2.Checked == true) // All transactions
                    {
                        WFilter = "Operator = @Operator " + " AND AccNumber ='" + WInputField + "' AND OpenRecord = 1 ";  // Only open

                        if (WFunction == "View" || WFunction == "Investigation")
                        {
                            WFilter = "Operator = @Operator " + " AND AccNumber ='" + WInputField + "'";  // Open and closed 

                        }
                    }

                }
               
            }

            if (radioButton1.Checked == true) // Only this Categ and Cycle both
            {
                if (WhatFile == "Both") // Both Files 
                {
                    FromDt = NullPastDate;
                    ToDt = NullPastDate;
                    int From = 1; 
                    Rm.ReadBothMatchedUnMatchedFileTable(WOperator, WFilter, FromDt, ToDt, WSortValue, From);
                    dataGridView1.DataSource = Rm.RMDataTableLeft.DefaultView;

                }
                else // Matched or UNMATCHED
                {
                    Rm.ReadMatchedORUnMatchedFileTableLeft(WOperator, WFilter, WhatFile, WSortValue);
                    dataGridView1.DataSource = Rm.RMDataTableLeft.DefaultView;
                }
               


                //dataGridView1.; 
            }

            if (radioButton2.Checked == true) // All transactions
            {
                WSortValue = "SeqNo" ; 
                FromDt = dateTimePicker1.Value.AddDays(-1);
                ToDt = dateTimePicker2.Value.AddDays(1);

                if (WhatFile == "Both") // Both Files
                {
                    int From = 1;
                    if (WInMaskRecordId >0)
                    {
                        Rm.ReadBothMatchedUnMatchedFileTable(WOperator, WFilter, NullPastDate, NullPastDate, WSortValue, From); // Unique MaskRecordId
                        
                    }
                    else
                    {
                        Rm.ReadBothMatchedUnMatchedFileTable(WOperator, WFilter, FromDt, ToDt, WSortValue, From);   
                    }

                    dataGridView1.DataSource = Rm.RMDataTableLeft.DefaultView;
                }
                else  // Only MATCHED OR UNMATCHED 
                {
                    Rm.ReadMatchedORUnMatchedFileTableLeftByPeriod(WOperator, WFilter, WhatFile, FromDt, ToDt, WSortValue);
                    dataGridView1.DataSource = Rm.RMDataTableLeft.DefaultView;
                }
               
                //dataGridView1.; 
            }

            //dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

            dataGridView1.Columns[0].Width = 40; // SeqNo
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[1].Width = 40; // 
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Width = 40; // 

            dataGridView1.Columns[3].Width = 110; // 

            dataGridView1.Columns[4].Width = 70; // 

            dataGridView1.Columns[5].Width = 40; // 
            dataGridView1.Columns[5].Width = 50; // 

            ////RMDataTableLeft.Columns.Add("SeqNo", typeof(int));
            ////RMDataTableLeft.Columns.Add("File", typeof(string));
            ////RMDataTableLeft.Columns.Add("Mask", typeof(string));
            ////RMDataTableLeft.Columns.Add("CardNumber", typeof(string));
            ////RMDataTableLeft.Columns.Add("AccNumber", typeof(string));
            ////RMDataTableLeft.Columns.Add("TransCurr", typeof(string));
            ////RMDataTableLeft.Columns.Add("TransAmount", typeof(decimal));
            ////RMDataTableLeft.Columns.Add("TransDate", typeof(DateTime));
            ////RMDataTableLeft.Columns.Add("TransDescr", typeof(string));

            if (dataGridView1.Rows.Count == 0 )
            {
                MessageBox.Show("No transactions for this selection");

                panel7.Hide();
                panel21.Hide();
                label11.Hide();
                textBoxMask.Hide(); 
                return;
            }
            else
            {
                panel7.Show();
                panel21.Show();
                label11.Show();
                textBoxMask.Show();
            }
            //TEST
            if (textBoxInputField.Text == "4375071234567892" & WInMaskRecordId == 0 & dataGridView1.Rows.Count >2 )
            {
                int WRow1 = 3;
                dataGridView1.Rows[WRow1].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, WRow1));
            }
           

            FirstCycle = false; 
              
            }    
           
// On ROW ENTER 
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];
            WUniqueNo = (int)rowSelected.Cells[0].Value;

            if (WhatFile == "Both")
            {
                WFileIdForBoth = (string)rowSelected.Cells[1].Value;
                if (WFileIdForBoth == "M")
                {
                    Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo("Matched", WUniqueNo);
                    WCategoryId = Rm.RMCateg; 
                }
                if (WFileIdForBoth == "U")
                {
                    Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo("UnMatched", WUniqueNo);
                    WCategoryId = Rm.RMCateg; 
                }

            }
            else
            {
                Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFile, WUniqueNo);
            }

            // NOTES START  
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Rm.MaskRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
            // NOTES END

            WMaskRecordId = Rm.MaskRecordId;

            textBox10.Text = Rm.RMCateg;
            textBox11.Text = Rm.TerminalId;  

            textBoxRmCycle.Text = Rm.RMCycle.ToString(); 
            textBoxCardNo.Text = Rm.CardNumber;
            textBoxAccNo.Text = Rm.AccNumber;
            textBoxCurr.Text = Rm.TransCurr;
            textBoxAmnt.Text = Rm.TransAmount.ToString("#,##0.00");
            textBoxDtTm.Text = Rm.TransDate.ToString(); 
            textBoxTraceNo.Text = Rm.AtmTraceNo.ToString();
            textBoxUniqueID.Text = Rm.MaskRecordId.ToString();

            if (Rm.RMCateg.Substring(0, 4) == "EWB1")
            {
                Tp.ReadInPoolTransSpecific(Rm.OriginalRecordId);
                if (Tp.RecordFound == true)
                {
                    RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();
                    Ta.ReadSessionsStatusTraces(Tp.AtmNo, Tp.SesNo);
                    if (Ta.RecordFound & Ta.ProcessMode > 0)
                    {
                        buttonReplPlay.Show();
                    }
                    else
                    {
                        buttonReplPlay.Hide();
                    }
                }
                
                button3.Show();
                button4.Show();
                button5.Show();
                label26.Show();
                label25.Show();
                label19.Show();

                Tp.ReadInPoolTransSpecific(Rm.OriginalRecordId); // Read Transactions details 
                ATMTrans = true; 
            }
            else
            {
                buttonReplPlay.Hide();
                button3.Hide();
                button4.Hide();
                button5.Hide();
                label26.Hide();
                label25.Hide();
                label19.Hide();
                ATMTrans = false;

            }
            Rms.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, Rm.RMCycle); 
            if (Rms.StartReconcDtTm == NullPastDate)
            {
                buttonReconcPlay.Hide(); 

            }
            else 
            {
                buttonReconcPlay.Show(); 
            }

            
            if (Rm.UserId != "")
            {
                label21.Show();
                label22.Show();
                textBox7.Text = Rm.UserId;
                textBox8.Text = Rm.Authoriser; 
            }
            else 
            {
                label21.Hide();
                label22.Hide();
                textBox7.Hide();
                textBox8.Hide();
            }
           
            textBoxMask.Text = Rm.MatchMask;
// Check for exceptions 
            if (Rm.MetaExceptionNo > 0 || (ATMTrans == true & Tp.ErrNo >0))
            {
                label3.Show();
                panel3.Show();
                if (Rm.MetaExceptionNo > 0)
                {
                    Ec.ReadErrorsTableSpecific(Rm.MetaExceptionNo);

                    textBoxExceptionNo.Text = Rm.MetaExceptionNo.ToString();

                    textBoxExceptionDesc.Text = Ec.ErrDesc;

                }
                if (ATMTrans == true & Tp.ErrNo > 0)
                {
                    Ec.ReadErrorsTableSpecific(Tp.ErrNo);

                    textBoxExceptionNo.Text = Tp.ErrNo.ToString();

                    textBoxExceptionDesc.Text = Ec.ErrDesc;
                }
               
            }
            else
            {
                label3.Hide();
                panel3.Hide();
            }

            Tp.ReadTransToBePostedSpecificByMaskRecordId(Rm.MaskRecordId);
            if (Tp.RecordFound == true)
            {
                label27.Show();
                panel8.Show();

                textBoxCreated.Text = Tp.OpenDate.ToString();
                if (Tp.ActionDate != NullPastDate) textBoxPosted.Text = Tp.ActionDate.ToString();
                else textBoxPosted.Text = "Not Posted yet." ; 
            }
            else
            {
                label27.Hide();
                panel8.Hide();
            }

            if (Rm.Matched == false) 
            {
                textBox6.Text = Rm.UnMatchedType; 
            }
            else textBox6.Text = "Matched" ; 

            if (WhatFile == "UnMatched" & Rm.UnMatchedType == "DUPLICATE")
            {
                if (Rm.MatchMask =="") WMask = "000";
                else WMask = Rm.MatchMask;
            }
            else
            {            
                 WMask = Rm.MatchMask;
            }             

            Rms.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, Rm.RMCycle);

            // First Line
            if (Rms.FileId11 != "")
            {
                labelFileA.Show();
                textBox1.Show(); 

                labelFileA.Text = "File A : " + Rms.FileId11;
                labelFileA.Show();
                WSubString = WMask.Substring(0, 1);
                if (WSubString == "0")
                {
                    textBox1.BackColor = Color.Red;
                    textBox1.ForeColor = Color.White;
                    textBox1.Text = "0"; 
                }
                if (WSubString == "1")
                {
                    textBox1.BackColor = Color.Lime;
                    textBox1.ForeColor = Color.White;
                    textBox1.Text = "1"; 
                }
                if (WSubString == ">")
                {
                    textBox3.BackColor = Color.Lime;
                    textBox3.ForeColor = Color.White;
                    textBox3.Text = ">";
                }
            }
            else
            {
                labelFileA.Hide();
                textBox1.Hide(); 
            }

            // Second Line 
            if (Rms.FileId21 != "")
            {
                labelFileB.Show();
                textBox2.Show();

                labelFileB.Text = "File B : "+ Rms.FileId21;
                labelFileB.Show();
                WSubString = WMask.Substring(1, 1);
                if (WSubString == "0")
                {
                    textBox2.BackColor = Color.Red;
                    textBox2.ForeColor = Color.White;
                    textBox2.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox2.BackColor = Color.Lime;
                    textBox2.ForeColor = Color.White;
                    textBox2.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox3.BackColor = Color.Lime;
                    textBox3.ForeColor = Color.White;
                    textBox3.Text = ">";
                }
            }
            else
            {
                labelFileB.Hide();
                textBox2.Hide();
            }

            // Third Line 
            //
            if (Rms.FileId31 != "")
            {
                labelFileC.Show();
                textBox3.Show();

                labelFileC.Text = "File C : " + Rms.FileId31;
                labelFileC.Show();
                WSubString = WMask.Substring(2, 1);
                if (WSubString == "0")
                {
                    textBox3.BackColor = Color.Red;
                    textBox3.ForeColor = Color.White;
                    textBox3.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox3.BackColor = Color.Lime;
                    textBox3.ForeColor = Color.White;
                    textBox3.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox3.BackColor = Color.Lime;
                    textBox3.ForeColor = Color.White;
                    textBox3.Text = ">";
                }
            }
            else
            {
                labelFileC.Hide();
                textBox3.Hide();
            }

            // Forth Line 
            if (Rms.FileId41 != "")
            {
                labelFileD.Show();
                textBox4.Show();

                labelFileD.Text = "File D : " + Rms.FileId41;
                labelFileD.Show();
                WSubString = WMask.Substring(3, 1);
                if (WSubString == "0")
                {
                    textBox4.BackColor = Color.Red;
                    textBox4.ForeColor = Color.White;
                    textBox4.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox4.BackColor = Color.Lime;
                    textBox4.ForeColor = Color.White;
                    textBox4.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox3.BackColor = Color.Lime;
                    textBox3.ForeColor = Color.White;
                    textBox3.Text = ">";
                }
            }
            else
            {
                labelFileD.Hide();
                textBox4.Hide();
            }

            // Fifth Line 
            if (Rms.FileId51 != "")
            {
                labelFileE.Show();
                textBox5.Show();

                labelFileE.Text = "File E : " + Rms.FileId51;
                labelFileE.Show();
                WSubString = WMask.Substring(4, 1);
                if (WSubString == "0")
                {
                    textBox5.BackColor = Color.Red;
                    textBox5.ForeColor = Color.White;
                    textBox5.Text = "0";
                }
                if (WSubString == "1")
                {
                    textBox5.BackColor = Color.Lime;
                    textBox5.ForeColor = Color.White;
                    textBox5.Text = "1";
                }
                if (WSubString == ">")
                {
                    textBox3.BackColor = Color.Lime;
                    textBox3.ForeColor = Color.White;
                    textBox3.Text = ">";
                }
            }
            else
            {
                labelFileE.Hide();
                textBox5.Hide();
            }

            WRRN = Rm.RRNumber;

            // Check if dispute already registered for this transaction 

            RRDMDisputeTrasactionClass Dt = new RRDMDisputeTrasactionClass();
            // Check if already exist
            Dt.ReadDisputeTranForInPool(WMaskRecordId);
            if (Dt.RecordFound == true)
            {
                textBox9.Text = Dt.DisputeNumber.ToString();
                //WFoundDisp = Dt.DisputeNumber;
                if (WFunction == "Investigation" & WInMaskRecordId == 0) 
                                    MessageBox.Show("Dispute with no : " + Dt.DisputeNumber.ToString() + " already registered for this transaction.");
                labelDispute.Show();
                textBox9.Show();
                buttonRegisterDispute.Hide();
            }
            else
            {
                labelDispute.Hide();
                textBox9.Hide();
                if (WFunction == "Investigation") 
                            buttonRegisterDispute.Show();
            }

            textBoxInputField.Text = Rm.CardNumber;
        }


// Move to Unmatched 
        private void buttonMovedToUnMatched_Click(object sender, EventArgs e)
        {
            // Testing 
            // Read Matched
            // Update Matched 
            // Insert in Matched 
            // Delete in Mathed 

            if (textBoxNewMask.TextLength == 0)
            {
                MessageBox.Show("Please Enter New Mask");
                return;
            }

            if (textBoxNewMask.Text == "11" 
                         || textBoxNewMask.Text == "111" || textBoxNewMask.Text == "1111" || textBoxNewMask.Text == "11111")
            {
                MessageBox.Show("Please the correct New Mask");
                return; 
            }

            if (textBoxNewMask.TextLength != Rms.TotalFiles)
            {
                MessageBox.Show("Please Enter the correct length for New Mask");
                return; 
            }
            
            WhatFile = "Matched";
            Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFile, WUniqueNo);

            Rm.ActionByUser = true;

            Rm.UserId = WSignedId; 

            Rm.MatchedType = "User Had changed it to Unmatched" ; 

            Rm.MetaExceptionId = 12212 ; 

            Rm.OpenRecord = false; 

            Rm.UpdateMatchedORUnMatchedRecordFooter(WOperator, WhatFile, WUniqueNo);

            WhatFile = "UnMatched";

            Rm.MatchMask = textBoxNewMask.Text;

            Rm.UnMatchedType = "User Had move it from Matched to Unmatched";

            Rm.Matched = false; 

            Rm.OpenRecord = true ; 

            Rm.InsertMatchedORUnMatchedFileRecord(WhatFile);

            Form80b_Load(this, new EventArgs());

            textBoxNewMask.Text = ""; 

            MessageBox.Show("Item has moved to UnMatched");

        }

// Move to Matched 
        private void buttonMoveToMatched_Click(object sender, EventArgs e)
        {
            // Insert in Matched

            if (textBoxNewMask.TextLength == 0 )
            {
                MessageBox.Show("Please Enter New Mask");
                return;
            }

            if (textBoxNewMask.Text == "11" || textBoxNewMask.Text == "111" || textBoxNewMask.Text == "1111"
                 || textBoxNewMask.Text == "11111" || textBoxNewMask.Text == "111111")
            {  
            }
            else
            {
                MessageBox.Show("Please Enter the correct New Mask");
                return;
            }

            if (textBoxNewMask.TextLength != Rms.TotalFiles)
            {
                MessageBox.Show("Please Enter the correct length for New Mask");
                return;
            }

            WhatFile = "UnMatched";
            Rm.ReadMatchedORUnMatchedFileSpecificRecordBySeqNo(WhatFile, WUniqueNo);

            Rm.ActionByUser = true;

            Rm.UserId = WSignedId;

            Rm.MatchedType = "User Had changed it to Matched";

            Rm.MetaExceptionId = 12213;

            Rm.OpenRecord = false;

            Rm.UpdateMatchedORUnMatchedRecordFooter(WOperator, WhatFile, WUniqueNo);

            WhatFile = "Matched";

            Rm.MatchMask = textBoxNewMask.Text;

            Rm.UnMatchedType = "User Had moved it UnMatched to Matched";

            Rm.Matched = true; 

            Rm.OpenRecord = true;

            Rm.InsertMatchedORUnMatchedFileRecord(WhatFile);

            Form80b_Load(this, new EventArgs());

            textBoxNewMask.Text = "";

            MessageBox.Show("Item has moved to Matched");
        }


// Show Exception 
        private void buttonShowException_Click(object sender, EventArgs e)
        {
            Form24 NForm24; 
            bool Replenishment = true;
            int ErrNo = Rm.MetaExceptionNo;
            string SearchFilter = "ErrNo =" + ErrNo;
            NForm24 = new Form24(WSignedId, WSignRecordNo, WOperator, WCategoryId,Rm.RMCycle, "", Replenishment, SearchFilter);
            NForm24.ShowDialog();
        }
// Notes 
        private void buttonNotes2_Click(object sender, EventArgs e)
        {          
            Form197 NForm197;
            string WParameter3 = "";
            string WParameter4 = "UniqueRecordId: " + Rm.MaskRecordId;
            string SearchP4 = "";
            //if (ViewWorkFlow == true) WMode = "Read";
            //else WMode = "Update";
            WMode = "Update";
            NForm197 = new Form197(WSignedId, WSignRecordNo, WOperator, WParameter3, WParameter4, WMode, SearchP4);
            NForm197.ShowDialog();

            // NOTES 
            Order = "Descending";
            WParameter4 = "UniqueRecordId: " + Rm.MaskRecordId;
            WSearchP4 = "";
            Cn.ReadAllNotes(WParameter4, WSignedId, Order, WSearchP4);
            if (Cn.RecordFound == true)
            {
                labelNumberNotes2.Text = Cn.TotalNotes.ToString();
            }
            else labelNumberNotes2.Text = "0";
            //SetScreen();
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // Print 

            string WD11 ;
            string WD12 ;
            string WD13 ;

            if (WhatFile == "Matched")
            {
            
                WD11 = WOperator ; 
                WD12 = WCategoryId ;
                WD13 = WRMCycleNo.ToString();

            Form56R_EWB001_Matched ReportMatched = new Form56R_EWB001_Matched(WD11, WD12, WD13);
            ReportMatched.Show();

            }

            if (WhatFile == "UnMatched" || WhatFile == "Both")
            {

                WD11 = WOperator;
                WD12 = WCategoryId;
                WD13 = WRMCycleNo.ToString();

                Form56R_EWB002_UnMatched ReportUnMatched = new Form56R_EWB002_UnMatched(WD11, WD12, WD13);
                ReportUnMatched.Show();

            }

        }

// SHOW Selection 
        private void buttonShowSelection_Click(object sender, EventArgs e)
        {
            buttonMovedToUnMatched.Show();
            buttonMoveToMatched.Show();
           
            Form80b_Load(this, new EventArgs());

        }
        // Finish 
        private void buttonNext_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }
// ON COMBO CHANGE LOAD 
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FirstCycle == false)
            {
                Form80b_Load(this, new EventArgs());
            }

        } 

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }
// Transcaction Trail 
        private void buttonTransTrail_Click(object sender, EventArgs e)
        {
            Form78b NForm78b; 
            RRDMReconcMaskRecordsLocation Rj= new RRDMReconcMaskRecordsLocation();
            //TEST
            Rj.ReadtblReconcMaskSpecific(WMaskRecordId, WOperator, Rm.RMCateg, Rm.RMCycle);

            if (Rj.RecordFound == true)
            {
                string WHeader = "TRAIL FOR TRANSACTION WITH ID : " + WMaskRecordId.ToString();

                NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rj.JsonSelectedTable, WHeader,"Form80b");
                //NForm78b.FormClosed += NForm78b_FormClosed;
                NForm78b.ShowDialog();

            }
            else
            {
                MessageBox.Show("This selection shows all incidents of transaction" + Environment.NewLine
                                 + "In different matching files. " + Environment.NewLine
                                 + "For this transaction there is non testing data to show." 
                                 );
                return;
            }
        }

        void NForm78b_FormClosed(object sender, FormClosedEventArgs e)
        {
            throw new NotImplementedException();
        }


        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

 // Unique search 
        private void checkBoxUnique_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUnique.Checked  == true )
            {
                panel5.Show();
            }
            else
            {
                panel5.Hide();

                radioButtonCard.Checked = false;
                radioButtonAccount.Checked = false;
                radioButtonUniqueNo.Checked = false;

                textBoxInputField.Text = "" ; 
            }
        }
// Radio Button one only cat/cycle
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                panel6.Hide(); 
            }
            else
            {
                panel6.Show(); 
            }
        }
// Radio two all Trans 
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButton2.Checked == true)
            {
                panel6.Show();
            }
            else
            {
                panel6.Hide();
            }

        }
// EXPAND GRID
        private void buttonExpandGridRight_Click(object sender, EventArgs e)
        {
            Form78b NForm78b; 
            string WHeader = "LIST OF TRANSACTIONS";
            NForm78b = new Form78b(WSignedId, WSignRecordNo, WOperator, Rm.RMDataTableLeft, WHeader,"Form80b");
            //NForm78b.FormClosed += NForm78b_FormClosed;
            NForm78b.ShowDialog(); 
        }
// Show Trans to Be posted 
        private void buttonTranPosted_Click(object sender, EventArgs e)
        { 
            Form78 NForm78;

            Tp.ReadInPoolTransSpecific(Rm.OriginalRecordId);
            if (Tp.RecordFound & Tp.ErrNo > 0)
            {
                NForm78 = new Form78(WSignedId, WSignRecordNo, WOperator,
                                                    "", 0, Tp.ErrNo, 1);
                NForm78.ShowDialog(); 
            }
            else
            {
                if (Rm.MetaExceptionNo > 0)
                {
                    NForm78 = new Form78(WSignedId, WSignRecordNo, WOperator,
                                                   "", 0, Rm.MetaExceptionNo, 1);
                    NForm78.ShowDialog(); 
                }
                else
                {
                    MessageBox.Show("No Transactions/actions were taken for this.");
                    return; 
                }            
              
            }       
        }
// Replenishment Play 
        private void buttonReplPlay_Click(object sender, EventArgs e)
        {
            Form51 NForm51; 
            Us.ReadSignedActivityByKey(WSignRecordNo);
            Us.ProcessNo = 54; // View only for replenishment already done  
            Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

            Tp.ReadInPoolTransSpecific(Rm.OriginalRecordId);
            if (Tp.RecordFound == true)
            {
                RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();
                Ta.ReadSessionsStatusTraces(Tp.AtmNo, Tp.SesNo);
                if (Ta.RecordFound & Ta.ProcessMode > 0)
                {
                    NForm51 = new Form51(WSignedId, WSignRecordNo, WOperator, Tp.AtmNo, Tp.SesNo);
                    NForm51.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No Replenishement done for this ATM and Replen Cycle");
                    return; 
                }
             
            }
            else
            {
                MessageBox.Show("No Replenishement for this type of transaction");
                return; 
            }       
            
        }
// Reconciliation Play 
        private void buttonReconcPlay_Click(object sender, EventArgs e)
        {
            if (Rm.RMCateg == "EWB110")
            {
                Form71 NForm71;
                Us.ReadSignedActivityByKey(WSignRecordNo);
                Us.ProcessNo = 54; // View only for reconciliation already done  
                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                Tp.ReadInPoolTransSpecific(Rm.OriginalRecordId);
                if (Tp.RecordFound == true)
                {
                    RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();
                    Ta.ReadSessionsStatusTraces(Tp.AtmNo, Tp.SesNo);
                    if (Ta.RecordFound & Ta.ProcessMode > 1)
                    {
                        NForm71 = new Form71(WSignedId, WSignRecordNo, WOperator, Tp.AtmNo, Tp.SesNo);
                        NForm71.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No Reconciliation done for this ATM and Replen Cycle");
                        return;
                    }
                }
            }
            else
            {
                // Update Us Process number
                Us.ReadSignedActivityByKey(WSignRecordNo);
                Us.ProcessNo = 54; // Reconciliation view 
                Us.UpdateSignedInTableStepLevelAndOther(WSignRecordNo);

                Form271 NForm271;

                NForm271 = new Form271(WSignedId, WSignRecordNo, WOperator, Rm.RMCateg, Rm.RMCycle);

                NForm271.ShowDialog();

                //MessageBox.Show("No Reconciliation for this type of transaction");
                return;
            }

          
        }
// Text From Journal 
        private void button3_Click(object sender, EventArgs e)
        {
            Form67 NForm67; 
            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            int Mode = 1; // Specific
  
            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, 0, Tp.AtmNo, Tp.AtmTraceNo, Tp.AtmTraceNo, Mode);
            NForm67.ShowDialog();
        }
// Full Journal
        private void button4_Click(object sender, EventArgs e)
        {
            Form67 NForm67; 
            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            Tp.ReadInPoolTransSpecific(Rm.OriginalRecordId); 

            RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate(); 
            int Mode = 2; // FULL

            //TEST

            Ta.ReadSessionsStatusTraces(Tp.AtmNo, Tp.SesNo);

            RRDME_JournalTxtClass Jt = new RRDME_JournalTxtClass();

            Jt.ReadJournalTextByTrace(Tp.BankId, Tp.AtmNo, Ta.FirstTraceNo);

            int FileInJournal = Jt.FuId;

            // WE SHOULD FIND OUT THE START AND OF THIS REPL. CYCLE 
            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, FileInJournal, Tp.AtmNo, Ta.FirstTraceNo, Ta.LastTraceNo, Mode);
            NForm67.Show();
        }
// SHOW Video Clip
        private void button5_Click(object sender, EventArgs e)
        {
            // Based on Transaction No show video clip 
            //TEST
            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog();
        }
// Register Dispute 
        private void buttonRegisterDispute_Click(object sender, EventArgs e)
        {
            Form5 NForm5; 
            int From = 2; // Coming from Pre-Investigattion ATMs 
            NForm5 = new Form5(WSignedId, WSignRecordNo, WOperator, Rm.CardNumber, WMaskRecordId, 0, 0, "" ,From, "ATM");
            NForm5.ShowDialog();
            this.Dispose();
        }

    }
}
