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
    public partial class Form84 : Form
    {
        Form67 NForm67; 

        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMErrorsClassWithActions Ec = new RRDMErrorsClassWithActions();
        RRDMHostTransClass Ch = new RRDMHostTransClass();
        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool ErrorFound;
        bool Actiontaken;
        bool Decisiontaken;
      //  bool HostUpdated;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WAtmNo; 
        int WTranNo;
        int WTraceNo; 

        public Form84(string InSignedId, int InSignRecordNo, string InOperator, string InAtmNo, int InTraceNo, int InTranNo)
        {
            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;
            WAtmNo =  InAtmNo;
            WTranNo = InTraceNo;
            WTraceNo = InTraceNo; 
        
            WTranNo = InTranNo;
 
            InitializeComponent();

            label22.Text = "Date Time Traces for trans : " + WTranNo.ToString(); 
        }
        // LOAD 
        private void Form84_Load(object sender, EventArgs e)
        {
            // ATM 
            Tc.ReadInPoolTransSpecific(WTranNo);

            textBox1.Text = Tc.AtmDtTime.ToString(); 

            // HOST
            Ch.ReadHostTransTraceNo(Tc.BankId, Tc.AtmNo, Tc.AtmTraceNo);

            if (Ch.RecordFound == false)
            {
                textBox2.Text = "No Host Transaction found"; 
            }
            else textBox2.Text = Ch.HostDtTime.ToString(); 

            // ERROR 

            Ec.ReadErrorsTableSpecificByTransNo(WTranNo);

            if (Ec.RecordFound == false)
            {
                textBox3.Text = "No error for this Transaction";
                ErrorFound = false;
            }
            else
            {
               textBox3.Text = Ec.DateTime.ToString();
               ErrorFound = true;
            }
            // See if actions are taken on error  
            if (Ec.RecordFound == true & Ec.UnderAction == true)
            {
                // Read Trans to be posted 

                Tc.ReadTransToBePostedSpecificTraceNo(Tc.AtmNo, Tc.AtmTraceNo);

                if (Tc.RecordFound == false & ErrorFound == true)
                {
                    textBox4.Text = "No Actions were taken on error";
                }

                if (Tc.RecordFound == true)
                {
                    textBox4.Text = Tc.OpenDate.ToString();
                    Actiontaken = true;
                }

                // Decision 

                if (Tc.RecordFound == false)
                {
                    textBox5.Text = "N/A";
                }

                if (Tc.RecordFound == true & Actiontaken == true & Tc.ActionCd2 == 0)
                {
                    textBox5.Text = "No decision taken yet for this ation";
                }

                if (Tc.RecordFound == true & Tc.ActionCd2 > 0)
                {
                    textBox5.Text = Tc.ActionDate.ToString();
                    Decisiontaken = true;
                }

                // Host posting 

                if (Tc.RecordFound == true & Decisiontaken == false)
                {
                    textBox6.Text = "N/A";
                }

                if (Tc.HostMatched == false & Decisiontaken == true)
                {
                    textBox6.Text = "Host not updated with the decision yet.";
                }

                if (Tc.RecordFound == true & Tc.HostMatched == true)
                {
                    textBox6.Text = Tc.MatchedDtTm.ToString();
                   // HostUpdated = true;
                }

            }
            else     
            {
                textBox4.Text = "N/A";

                if (Ec.RecordFound == true & Ec.NeedAction == true & Ec.UnderAction == false ) 
                {
                    textBox4.Text = "No reconciliation action on Error yet";

                }

                
                textBox5.Text = "N/A";
                textBox6.Text = "N/A"; 
            }

        }
        // Journal Part 
        private void button3_Click(object sender, EventArgs e)
        {
            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            int Mode = 1; // Specific

            Tc.ReadInPoolTransSpecific(WTranNo);

            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, 0, WAtmNo, Tc.AtmTraceNo, Tc.AtmTraceNo, Mode);
            NForm67.Show();
        }
        // Full Journal 
        private void button4_Click(object sender, EventArgs e)
        {
            String JournalId = "[ATMS_Journals].[dbo].[tblHstEjText]";

            int Mode = 2; // FULL

            Tc.ReadInPoolTransSpecific(WTranNo);

            Ta.ReadSessionsStatusTraces(Tc.AtmNo, Tc.SesNo);

            RRDME_JournalTxtClass Jt = new RRDME_JournalTxtClass();

            Jt.ReadJournalTextByTrace(Ta.BankId, WAtmNo, Ta.FirstTraceNo);

            int FileInJournal = Jt.FuId;

            // WE SHOULD FIND OUT THE START AND OF THIS REPL. CYCLE 
            NForm67 = new Form67(WSignedId, WSignRecordNo, WOperator, JournalId, FileInJournal, WAtmNo, Ta.FirstTraceNo, Ta.LastTraceNo, Mode);
            NForm67.Show();
        }
        // Video Clip 
        private void button5_Click(object sender, EventArgs e)
        {
            // Based on Transaction No show video clip 
            //TEST
            VideoWindow videoForm = new VideoWindow();
            videoForm.ShowDialog();
        }
    }
}
