using System;
using System.Drawing;
using System.Windows.Forms;
using RRDM4ATMs;

namespace RRDM4ATMsWin
{
    public partial class UCForm281a : UserControl
    {

        //public event EventHandler ChangeBoardMessage;
        public string guidanceMsg;

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();
        //RRDMMatchingCategoriesSessions Rms = new RRDMMatchingCategoriesSessions();

        //RRDMReconcMatchedUnMatchedVisaAuthorClass Rm = new RRDMReconcMatchedUnMatchedVisaAuthorClass(); 

        RRDMErrorsClassWithActions Er = new RRDMErrorsClassWithActions();

        RRDMUsersRecords Us = new RRDMUsersRecords();

        RRDMBanks Ba = new RRDMBanks();

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        bool ViewWorkFlow;

        String Ccy;

        string WSignedId;
        int WSignRecordNo;
        string WOperator;
        string WCategoryId;
        int WReconcCycleNo;

        public void UCForm281aPar(string InSignedId, int InSignRecordNo, string InOperator, string InCategory, int InReconcCycleId)
        {

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WOperator = InOperator;

            WCategoryId = InCategory;

            WReconcCycleNo = InReconcCycleId;
            InitializeComponent();

            Rcs.ReadReconcCategorySessionByCatAndRunningJobNo(WOperator, WCategoryId, WReconcCycleNo);

            label4.Text = "RECONCILIATION CATEGORY : " + Rcs.CategoryName + " ---- CYCLE : " + WReconcCycleNo.ToString();

            if (WCategoryId == "EWB311")
            {
                label12.Text = "GL Balance";
            }

            Ba.ReadBank(WOperator);
            Ccy = Ba.BasicCurName;

            label36.Text = "Balances in : " + Ccy;
            // ................................
            // Handle View ONLY 
            // ''''''''''''''''''''''''''''''''
            RRDMUserSignedInRecords Usi = new RRDMUserSignedInRecords();
            Usi.ReadSignedActivityByKey(WSignRecordNo);

            if (Usi.ProcessNo == 54 || Usi.ProcessNo == 55 || Usi.ProcessNo == 56)
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
            Color Black = Color.Black;


            //string SearchingStringLeft = "Operator ='" + WOperator + "' AND RMCateg ='" + WCategoryId + "'"
            //             + " AND OpenRecord = 1 ";
            //Rm.ReadMatchedORUnMatchedFileTableLeft2(WOperator, "UnMatched", SearchingStringLeft);



            //label5.Text = "Start Date Time...: " + Rms.StartDateTm.ToString();
            //label6.Text = "GL Currency       : " + Rms.GlCurrency;
            //label7.Text = "GL Account        : " + Rms.GlAccountNo;
            //label13.Text = "Matched Trans Amt : " + Rms.MatchedTransAmt.ToString("#,##0.00");
            //label14.Text = "UNMatched Amt.... : " + Rm.TotalAmount.ToString("#,##0.00");

            //***************************************************************
            //FILL IN DEBIT LEG 
            //***************************************************************

            //Rms.ReadMatchingCategoriesSessionsByRunningJobNo(WOperator, Rcs.MatchingSessionDr);

            //if (Rms.FileId11 != "")
            if (Rcs.MatchingCat01 != "")
            {
                label17.Show();
                label17.Text = "File A : ";
                labelFileA.Show();
                labelFileA.Text = Rcs.MatchingCat01;
            }
            if (Rcs.MatchingCat02 != "")
            {
                label20.Show();
                label20.Text = "File B : ";
                labelFileB.Show();
                labelFileB.Text = Rcs.MatchingCat02;
            }
            if (Rcs.MatchingCat03 != "")
            {
                label21.Show();
                label21.Text = "File C : ";
                labelFileC.Show();
                labelFileC.Text = Rcs.MatchingCat03;
            }

            //StartDateTm = (DateTime)rdr["StartDateTm"];
            //EndDateTm = (DateTime)rdr["EndDateTm"];

            //label5.Text = "Matching started : " + Rms.StartDateTm.ToString();
            //label13.Text = "Matching Ended   : " + Rms.EndDateTm.ToString();

            //***************************************************************
            //FILL IN CREDIT LEG 
            //***************************************************************

            //Rms.ReadMatchingCategoriesSessionsByRunningJobNo(WOperator, Rcs.MatchingSessionCr);

            if (Rcs.MatchingCat04 != "")
            {
                label34.Show();
                label34.Text = "File A : ";
                label28.Show();
                label28.Text = Rcs.MatchingCat04;
            }
            if (Rcs.MatchingCat05 != "")
            {
                label32.Show();
                label32.Text = "File B : ";
                label27.Show();
                label27.Text = Rcs.MatchingCat05;
            }
            if (Rcs.MatchingCat06 != "")
            {
                label31.Show();
                label31.Text = "File C : ";
                label26.Show();
                label26.Text = Rcs.MatchingCat06;
            }

            //label7.Text = "Matching started : " + Rms.StartDateTm.ToString();
            //label14.Text = "Matching Ended   : " + Rms.EndDateTm.ToString();

            //
            // Right PANEL 
            //

            textBox1.Text = Rcs.MatchedTransAmt.ToString("#,##0.00");
            textBox2.Text = Rcs.NotMatchedTransAmt.ToString("#,##0.00");

            textBox3.Text = Rcs.NumberOfMatchedRecs.ToString();
            textBox7.Text = Rcs.NumberOfUnMatchedRecs.ToString();

            RRDMMatchingReconcExceptionsInfoITMX Mre = new RRDMMatchingReconcExceptionsInfoITMX();
            Mre.ReadMatchingReconcExceptionsInfoForTotals(WOperator, WCategoryId, WReconcCycleNo);

            textBox5.Text = Mre.TotalExceptionsHandleByUser.ToString();

            if (ViewWorkFlow == true)
            {
                Er.ReadAllErrorsTableClosedForCounters(WOperator, WCategoryId, "");
            }
            else
            {
                Er.ReadAllErrorsTableForCounters(WOperator, WCategoryId, "", 0, "");
            }

            //textBox5.Text = Er.NumOfErrors.ToString();

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
                    //textBox8.Text = (Rms.GlTodaysBalance - (TempTotal + Er.TotalUnderActionAmt)).ToString("#,##0.00");
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
