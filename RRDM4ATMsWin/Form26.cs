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
    public partial class Form26 : Form
    {
     //   FormMainScreen NFormMainScreen;
        Form67 NForm67; 
        RRDMCaptureCardsClass Cc = new RRDMCaptureCardsClass();
      

        DateTime NullFutureDate = new DateTime(2050, 11, 21);
 
        int tempRowIndex;

        int WCaptNo;
        int WTraceNo; 
        string Gridfilter;
   
        string InputCardNo;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
  
        string WAtmNo;
        int WSesNo;

        public Form26(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InSesNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
    
            WAtmNo = InAtmNo;
            WSesNo = InSesNo; 

            InitializeComponent(); 
            // Call Procedures
            
            labelATMno.Text = WAtmNo;
            labelSessionNo.Text = WSesNo.ToString();
            labelToday.Text = DateTime.Now.ToShortDateString();
            pictureBox1.BackgroundImage = Properties.Resources.logo2;
          
        }

        // ON LOAD OF FORM DO 
        private void Form26_Load(object sender, EventArgs e)
        {
         
            Gridfilter = "Operator ='" + WOperator + "' AND AtmNo = '" + WAtmNo + "'";
            capturedCardsBindingSource.Filter = Gridfilter ;

            dataGridView1.Sort(dataGridView1.Columns[0],ListSortDirection.Descending);

            this.capturedCardsTableAdapter.Fill(this.aTMSDataSet33.CapturedCards);

            label14.Text = " ALL ACTIVE CAPTURE CARDS FOR THIS ATM / SESSION ";

            textBoxMsgBoard.Text = " YOU CAN LOCATE THE CAPTURED CARD. PRINT THE NECESSARY DOC. AFTER SIGNING" +
                " THIS IS SCANNED INTO THE SYSTEM";

            if (dataGridView1.Rows.Count == 0)
            {
                button2.Enabled = false;
            }
            else button2.Enabled = true;
        }

        //
  // Choose from Grid 
        //
 private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow rowSelected = dataGridView1.Rows[e.RowIndex];

         //   Poss = e.RowIndex; 
        
            WCaptNo = (int)rowSelected.Cells[0].Value;

            ShowRow( WCaptNo); // Show Row   
        }
        //
        // TAKE ACTION
        //
 private void button2_Click_1(object sender, EventArgs e)
        {
            
            tempRowIndex = dataGridView1.SelectedRows[0].Index;

            if (radioButton4.Checked == false & radioButton5.Checked == false & radioButton7.Checked == false)
            {
                MessageBox.Show("Make your choice please.");
                return;
            }

            // IF REQUEST FOR UNDO ACTION 
            if (radioButton7.Checked == true)
            {
            //    OldPoss = Poss; 

                Cc.ReadCaptureCard(WCaptNo);

                Cc.ActionCode = 0 ;
              
                Cc.ActionComments = "";
                Cc.CustomerNm = "";
                Cc.ActionDtTm = NullFutureDate;
                Cc.OpenRec = true;

                Cc.UpdateCapturedCardSpecific(WCaptNo);

               ShowRow(WCaptNo); // Show Row 

                capturedCardsBindingSource.Filter = Gridfilter;
                this.capturedCardsTableAdapter.Fill(this.aTMSDataSet33.CapturedCards);

         //       dataGridView1.Rows[OldPoss].Selected = true;
                dataGridView1.Rows[tempRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, tempRowIndex));
                

                textBoxMsgBoard.Text = " Action is cancelled ";

                return; 
            }
         
            // DELIVERED To CUSTOMER 


            if ( radioButton4.Checked == true )
            {
                // It will be delivered to customer 

                if (String.IsNullOrEmpty(textBox3usercommentold.Text) || String.IsNullOrEmpty(textBox2Name.Text) 
                    || String.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("Comment , Full Card and Name must have Values");
                    return; 
                }         

                Cc.ReadCaptureCard(WCaptNo);

                Cc.CardNo = textBox2.Text; // Full Card No 
                Cc.ActionCode = 1 ;
               
                Cc.ActionComments = textBox3usercommentold.Text;
                Cc.CustomerNm = textBox2Name.Text;
                Cc.ActionDtTm = DateTime.Now;
           

                Cc.UpdateCapturedCardSpecific(WCaptNo);

                ShowRow(WCaptNo); // Show Row 

                PrintFormFromAction();

                capturedCardsBindingSource.Filter = Gridfilter;
                this.capturedCardsTableAdapter.Fill(this.aTMSDataSet33.CapturedCards);

                dataGridView1.Rows[tempRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, tempRowIndex));

                MessageBox.Show("Action Taken and captured card form printed");

                textBoxMsgBoard.Text = " Action Taken ";

                return; 
                
            }

            // DELIVERED TO CARDS Department 

            if (radioButton5.Checked == true )
            {
                if (String.IsNullOrEmpty(textBox3usercommentold.Text) || String.IsNullOrEmpty(textBox2Name.Text))
                {
                    MessageBox.Show("Both User Comment And Name must have Values");
                    return;
                }

                Cc.ReadCaptureCard(WCaptNo);

                Cc.CardNo = textBox2.Text; // Full Card No 
                Cc.ActionCode = 2;
                
                Cc.ActionComments = textBox3usercommentold.Text;
                Cc.CustomerNm = textBox2Name.Text;
                Cc.ActionDtTm = DateTime.Now;
          

                Cc.UpdateCapturedCardSpecific(WCaptNo);

                ShowRow(WCaptNo); // Show Row 

                PrintFormFromAction();
                    // SHOW Updated Grid

                capturedCardsBindingSource.Filter = Gridfilter;
                this.capturedCardsTableAdapter.Fill(this.aTMSDataSet33.CapturedCards);

                dataGridView1.Rows[tempRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, tempRowIndex));

                MessageBox.Show("Action Taken and captured card form printed");

                textBoxMsgBoard.Text = " Action Taken ";

                return; 
               
            }

            // Handled By Branch  

            if (radioButton6.Checked == true)
            {
                if (String.IsNullOrEmpty(textBox3usercommentold.Text) )
                {
                    MessageBox.Show("User Comment must have Value");
                    return;
                }

                Cc.ReadCaptureCard(WCaptNo);

                Cc.CardNo = textBox2.Text; // Full Card No 
                Cc.ActionCode = 3;
               
                Cc.ActionComments = textBox3usercommentold.Text;
                Cc.CustomerNm = "N/A";
                Cc.ActionDtTm = DateTime.Now;
            

                Cc.UpdateCapturedCardSpecific(WCaptNo);

                ShowRow(WCaptNo); // Show Row 

                PrintFormFromAction();

                    // SHOW Updated Grid

                capturedCardsBindingSource.Filter = Gridfilter;
                this.capturedCardsTableAdapter.Fill(this.aTMSDataSet33.CapturedCards);

                dataGridView1.Rows[tempRowIndex].Selected = true;
                dataGridView1_RowEnter(this, new DataGridViewCellEventArgs(1, tempRowIndex));

                MessageBox.Show("Action Taken and captured card form printed");

                textBoxMsgBoard.Text = " Branch will Act ";

                return; 

            }           
 
        }
       
        // SHOW NEW SELECTION 
        private void button5Show_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
               // TextBox1HasValue = false;
                MessageBox.Show("Input Card No Please");
                return; 
            }
            else
            {
             //   TextBox1HasValue = true;
                InputCardNo = textBox1.Text; 
            }

            Gridfilter = "AtmNo = '" + WAtmNo + "'" + " AND " + "CardNo = '" + InputCardNo + "'";

            label14.Text = "CHOSEN CARD ";

            textBoxMsgBoard.Text = " THIS IS YOUR CHOSEN CARD. TAKE ACTION IF YOU WISH. " +
                    " IF EMPTY THEN CARD NOT FOUND";
            
            capturedCardsBindingSource.Filter = Gridfilter;

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
        //    dataGridView1.Sort(dataGridView1.Columns[6], ListSortDirection.Ascending);
            
            this.capturedCardsTableAdapter.Fill(this.aTMSDataSet33.CapturedCards);

            if (dataGridView1.Rows.Count == 0)
            {
                button2.Enabled = false;
            }
            else button2.Enabled = true;
        }


        // Show as Selected 
        private void button6_Click(object sender, EventArgs e)
        {

            if (radioButton1.Checked == false & radioButton2.Checked == false & radioButton3.Checked == false)
            {
                MessageBox.Show("Make your choice please.");
                return; 
            }

            if (radioButton1.Checked == true)
            {
                Gridfilter = "AtmNo = '" + WAtmNo + "'" + " AND " + "ActionDtTm = ' "+ NullFutureDate+"'";

                 textBoxMsgBoard.Text = " All Outstanding Captured for this ATM";
            }

            if (radioButton2.Checked == true)
            {
                Gridfilter = "AtmNo = '" + WAtmNo + "'" ;
                textBoxMsgBoard.Text = " All Captured for this ATM Including closed";
            }

            if (radioButton3.Checked == true)
            {
                Gridfilter = "AtmNo = '" + WAtmNo + "'" + " AND " + "SesNo = " + WSesNo ;
                textBoxMsgBoard.Text = " All Captured for this ATM Repl Cycle";
            }

            capturedCardsBindingSource.Filter = Gridfilter;

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Descending);
            //    dataGridView1.Sort(dataGridView1.Columns[6], ListSortDirection.Ascending);

            this.capturedCardsTableAdapter.Fill(this.aTMSDataSet33.CapturedCards);

            if (dataGridView1.Rows.Count == 0)
            {
                button2.Enabled = false ;
            }
            else button2.Enabled = true;
        }

        private void ShowRow(int InCaptNo)
        {
            Cc.ReadCaptureCard(InCaptNo);

            label7.Text = Cc.CardNo;
            label11.Text = Cc.ReasonDesc;
            if (Cc.ActionDtTm == NullFutureDate)
            {
                label17.Hide();
                label18.Hide();
            }
            else
            {
                label17.Show();
                label18.Show();
                label18.Text = Cc.ActionDtTm.ToString();
            }

            WTraceNo = Cc.TraceNo;

            if (Cc.ActionCode == 1) radioButton4.Checked = true;
            if (Cc.ActionCode == 2) radioButton5.Checked = true;
            if (Cc.ActionCode == 3) radioButton6.Checked = true;

            if (Cc.ActionCode == 0)
            {
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
                radioButton7.Checked = false;
            }

            textBox3usercommentold.Text = Cc.ActionComments;
            textBox2Name.Text = Cc.CustomerNm;
            textBox2.Text = Cc.CardNo;

            if (Cc.ActionDtTm == NullFutureDate)
            {
                textBoxMsgBoard.Text = " Action not taken yet for this card.";
            }
            else
            {
                textBoxMsgBoard.Text = " Action taken for this card.";
            }
        }

        //
 // Based on Transaction No show video clip
        //
private void button3_Click(object sender, EventArgs e)
        {
        VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog(); 
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
        
        //
        // Locate Card In EJournal 
        // 
        private void button5_Click(object sender, EventArgs e)
        {

            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            int Mode = 1; // Specific

            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, 0, WAtmNo, WTraceNo, WTraceNo, Mode);
            NForm67.Show();
        }
        //
        // Print Captured Card Form
        //
        private void PrintFormFromAction()
        {
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            Ac.ReadAtm(WAtmNo);
            RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();
            Us.ReadUsersRecord(WSignedId);
            RRDMBanks Ba = new RRDMBanks();
            Ba.ReadBank(WOperator); 
            
            String P1 = Cc.CardNo;
            String P2 = Cc.CustomerNm;
            String P3 = Cc.CaptDtTm.ToString();
            String P4 = Cc.ReasonDesc;
            String P5 = WOperator;
            String P6 = Ba.BankName; 
            String P7 = Ac.BranchName;
            String P8 = WAtmNo ; 
            String P9 = Cc.ActionComments;
            String P10 = Us.UserName;
            String P11 = "";
            String P12 = "";
            String P13 = "";
            if (Cc.ActionCode == 3)
            {
                P11 = "X"; 
                P12 = ""; 
                P13 = ""; 
            }
             if (Cc.ActionCode == 2)
            {
                P11 = ""; 
                P12 = "X"; 
                P13 = ""; 
            }
             if (Cc.ActionCode == 1)
            {
                P11 = ""; 
                P12 = ""; 
                P13 = "X"; 
            }

             Form56R2 ReportCaptured = new Form56R2(P1,P2,P3,P4,P5,P6,P7,
                                   P8,P9,P10,P11,P12,P13);
            ReportCaptured.Show();

        }

        private void PrintFromAction55()
        {
        
        }
        // FINISH 
        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }     
        
    }
}
