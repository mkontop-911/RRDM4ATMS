using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm271a : UserControl
    {
        
        public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass(); 

        RRDMReconcCategoriesMatchingSessions Rms = new RRDMReconcCategoriesMatchingSessions();
        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool ViewWorkFlow ;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCategoryId;
        int WRMCycle; 

        public void UCForm271aPar(string InSignedId, int InSignRecordNo, string InOperator, string InCategory, int InRMCycle)
        {

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCategoryId = InCategory;

            WRMCycle = InRMCycle;
            InitializeComponent();

            Rms.ReadReconcCategoriesMatchingSessionsByRmCycle(WOperator, WRMCycle);

            label4.Text = "RM CATEGORY : " + WCategoryId +"--"+Rms.CategoryName +" ---- RM CYCLE : " + WRMCycle.ToString();

            if (WCategoryId == "EWB311")
            {
                label12.Text = "GL Balance"; 
            }

            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            Us.ReadSignedActivityByKey(WSignRecordNo);

            if (Us.ProcessNo == 54 || Us.ProcessNo == 55 || Us.ProcessNo == 56)
            {
                ViewWorkFlow = true;
            }
        }

         // SHOW SCREEN 

        public void SetScreen()
        {
            //
            // LEFT 
            //
            Color Red = Color.Red;
            Color Black = Color.Black ;


            string SearchingStringLeft = "Operator ='" + WOperator + "' AND RMCateg ='" + WCategoryId + "'"
                         + " AND OpenRecord = 1 ";
            Rm.ReadMatchedORUnMatchedFileTableLeft2(WOperator, "UnMatched", SearchingStringLeft);

            label5.Text = "Start Date Time...: " + Rms.StartDateTm.ToString();
            label6.Text = "GL Currency       : " + Rms.GlCurrency;
            label7.Text = "GL Account        : " + Rms.GlAccountNo;
            label13.Text = "Matched Trans Amt : " + Rms.MatchedTransAmt.ToString("#,##0.00");
            label14.Text = "UNMatched Amt.... : " + Rm.TotalAmount.ToString("#,##0.00");

            if (Rms.FileId11 != "")
            {
                label17.Show();
                label17.Text = "File A : " ;
                labelFileA.Show();
                labelFileA.Text = Rms.FileId11;
            }
            if (Rms.FileId21 != "")
            {
                label20.Show();
                label20.Text = "File B : " ;
                labelFileB.Show();
                labelFileB.Text = Rms.FileId21;
            }
            if (Rms.FileId31 != "")
            {
                label21.Show();
                label21.Text = "File C : " ;
                labelFileC.Show();
                labelFileC.Text = Rms.FileId31;
            }
            if (Rms.FileId41 != "")
            {
                label22.Show();
                label22.Text = "File D : " ;
                labelFileD.Show();
                labelFileD.Text = Rms.FileId41;
            }
            if (Rms.FileId51 != "")
            {
                label23.Show();
                label23.Text = "File E : " ;
                labelFileE.Show();
                labelFileE.Text = Rms.FileId51;
            }

            //
            // Right 
            //
            decimal TempTotal = Rms.GlYesterdaysBalance + Rms.MatchedTransAmt;
            textBox1.Text = TempTotal.ToString("#,##0.00");
            textBox2.Text = Rms.GlTodaysBalance.ToString("#,##0.00");
            textBox3.Text = (Rms.GlTodaysBalance - TempTotal).ToString("#,##0.00");

            if ((Rms.GlTodaysBalance - TempTotal) != 0)
            {
                label15.ForeColor = Red;
            }
            else
            {
                label15.ForeColor = Black;
            }

            textBox7.Text = Rms.NumberOfUnMatchedRecs.ToString();

            if (ViewWorkFlow == true)
            {
                Er.ReadAllErrorsTableClosedForCounters(WOperator, WCategoryId, "");
            }
            else
            {
                Er.ReadAllErrorsTableForCounters(WOperator, WCategoryId, "");
            }    

            textBox5.Text = Er.NumOfErrors.ToString();

            textBox4.Text = Er.ErrUnderAction.ToString();

            if (Er.ErrUnderAction > 0)
            {
                label24.Show();
                label25.Show();
                textBox6.Show();
                textBox8.Show();
                if (WCategoryId == "EWB102")
                {
                    textBox6.Text = Er.TotalUnderActionAmt.ToString("#,##0.00");
                    textBox8.Text = (Rms.GlTodaysBalance - (TempTotal + Er.TotalUnderActionAmt)).ToString("#,##0.00");
                }
                if (WCategoryId == "EWB311")
                {
                    label24.Text = "Meta Exception Total Amt"; 
                    textBox6.Text = Er.TotalUnderActionAmt.ToString("#,##0.00");

                    label25.Hide();
                    textBox8.Hide();
                }
                
            }
            else
            {
                label24.Hide();
                label25.Hide();
                textBox6.Hide();
                textBox8.Hide();
            }

            //textBox6.Text = (Er.NumOfErrors - Er.ErrUnderAction).ToString();

            if ((Er.NumOfErrors - Er.ErrUnderAction) > 0)
            {
                label8.ForeColor = Red;
            }
            else
            {
                // RRDM Blue 
            }
        }
    }
}
