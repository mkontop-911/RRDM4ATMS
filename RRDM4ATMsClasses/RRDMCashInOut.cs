using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
namespace RRDM4ATMs
{
    public class RRDMCashInOut
    {
        // This Class creates transactions for cash out. 
        // ONE For Atm Statement and 
        // one for CIT Provider 
        // 
        RRDMNotesBalances Na = new RRDMNotesBalances();
        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass();
        RRDMAtmsClass Ac = new RRDMAtmsClass();
        RRDMTracesReadUpdate Ta = new RRDMTracesReadUpdate();
        RRDMPostedTrans Cs = new RRDMPostedTrans();
        RRDMAccountsClass Acc = new RRDMAccountsClass();
        RRDMUsersAndSignedRecord Us = new RRDMUsersAndSignedRecord();

        

        string WCitId; 

        int TraceNoCashOut;

        string WSignedId;
        int WSignRecordNo;
        string WBankId;
    //    bool WPrive;
        string WAtmNo;
        int WSesNo;
        int WStartTrxn;
        int WEndTrxn;

      //  public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        public void InsertTranForCashOut(string InSignedId, int InSignRecordNo, string InBankId, 
            string InAtmNo, int InSesNo, int InStartTrxn, int InEndTrxn)
        {
            
            ErrorFound = false;
            ErrorOutput = ""; 

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
            WBankId = InBankId;
     
            WAtmNo = InAtmNo;
            WSesNo = InSesNo;
            WStartTrxn = InStartTrxn;
            WEndTrxn = InEndTrxn; 

            Ac.ReadAtm(WAtmNo);

            WCitId = Ac.CitId;

          //  string WUserBankId;
            // ================USER BANK =============================
          //  Us.ReadUsersRecord(WSignedId); // Read USER record for the signed user
         //   WUserBankId = Us.UserBankId;
            // ========================================================

            Na.ReadSessionsNotesAndValues(InAtmNo, InSesNo, 2);

            Ta.ReadSessionsStatusTraces(InAtmNo, InSesNo); 
              
            if (Na.BalSets >= 1)
            {
                TraceNoCashOut = Ta.LastTraceNo + 1;
                Ta.LastTraceNo = TraceNoCashOut;
                Ta.UpdateSessionsStatusTraces(InAtmNo, InSesNo); 
                InsertTranInPool(InAtmNo, InSesNo, TraceNoCashOut,
                    Na.Balances1.CurrNm, Na.Balances1.MachineBal);

                //if (WCitId !="1000")
                //{
                //    int TransType = 21;
                //    String TransDesc = "ATM(" + InAtmNo.ToString() + ") TO CIT";  
                //    InsertTranInCit(InAtmNo, InSesNo, TraceNoCashOut,
                //                        Na.Balances1.CurrNm, Na.Balances1.MachineBal, TransType, TransDesc);
                //}
            }

            if (Na.BalSets >= 2)
            {
                TraceNoCashOut = Ta.LastTraceNo + 1; // Last didgit becomes
                Ta.LastTraceNo = TraceNoCashOut;
                Ta.UpdateSessionsStatusTraces(InAtmNo, InSesNo); 

                InsertTranInPool(InAtmNo, InSesNo, TraceNoCashOut,
                     Na.Balances2.CurrNm, Na.Balances2.MachineBal);
                //if (WCitId != "1000")
                //{
                //    int TransType = 21;
                //    String TransDesc = "ATM(" + InAtmNo.ToString() + ") TO CIT";  
                //    InsertTranInCit(InAtmNo, InSesNo, TraceNoCashOut,
                //                        Na.Balances1.CurrNm, Na.Balances1.MachineBal, TransType, TransDesc);
                //}
            }
            if (Na.BalSets >= 3)
            {
                TraceNoCashOut = Ta.LastTraceNo + 1; // Last didgit becomes
                Ta.LastTraceNo = TraceNoCashOut;
                Ta.UpdateSessionsStatusTraces(InAtmNo, InSesNo); 

                InsertTranInPool(InAtmNo, InSesNo, TraceNoCashOut, 
                     Na.Balances3.CurrNm, Na.Balances3.MachineBal);
            }

            if (Na.BalSets == 4)
            {
                TraceNoCashOut = Ta.LastTraceNo + 1; // Last didgit becomes
                Ta.LastTraceNo = TraceNoCashOut;
                Ta.UpdateSessionsStatusTraces(InAtmNo, InSesNo); 

                InsertTranInPool(InAtmNo, InSesNo, TraceNoCashOut, 
                      Na.Balances4.CurrNm, Na.Balances4.MachineBal);
            }
        }

        //// FOR CIT : CIT PUTS MONEY IN ATM
        //public void InsertTranForCashInCit(string InSignedid, int InSignRecordNo, string InBankId,
        //                                   string InAtmNo, int InSesNo)
        //{

        //    WSignedId = InSignedid;
        //    WSignRecordNo = InSignRecordNo;
        //    WBankId = InBankId;
    
        //    WAtmNo = InAtmNo;
        //    WSesNo = InSesNo;

        //    Ac.ReadAtm(WAtmNo);

        //    WCitId = Ac.CitId;

        //    Na.ReadSessionsNotesAndValues(InAtmNo, InSesNo, 2);

        //    Ta.ReadSessionsStatusTraces(InAtmNo, InSesNo);

        //    if (Na.BalSets >= 1)
        //    {
        //        TraceNoCashOut = Ta.LastTraceNo + 1;

        //        if (WCitId != "1000")
        //        {
        //            int TransType = 11;
        //            String TransDesc = "CIT TO ATM : " + InAtmNo.ToString();
        //            InsertTranInCit(InAtmNo, InSesNo, TraceNoCashOut, 
        //                                Na.Balances1.CurrNm, Na.Balances1.OpenBal, TransType, TransDesc);
        //        }
        //    }

        //    if (Na.BalSets >= 2)
        //    {
        //        TraceNoCashOut = Ta.LastTraceNo + 2; // Last didgit becomes 

        //        if (WCitId != "1000")
        //        {
        //            int TransType = 11;
        //            String TransDesc = "CIT TO ATM : " + InAtmNo.ToString();
        //            InsertTranInCit(InAtmNo, InSesNo, TraceNoCashOut,
        //                                Na.Balances1.CurrNm, Na.Balances1.OpenBal, TransType, TransDesc);
        //        }
        //    }
        //    if (Na.BalSets >= 3)
        //    {
              
        //    }

        //    if (Na.BalSets == 4)
        //    {
               
        //    }
        //}
        //
        // Insert Build and Insert Transaction 
        //

        private void InsertTranInPool(string InAtmNo, int InSesNo, int InTraceNo,
                                      string InCurrNm, decimal InTranAmount)
        {         
            //
            // BUILD THE TRANSACTION 
            //

            ErrorFound = false;
            ErrorOutput = ""; 

            Ac.ReadAtm(InAtmNo); 

            Tc.CurrDesc = InCurrNm;
            Tc.TranAmount = InTranAmount;

            Tc.AtmTraceNo = InTraceNo;
            Tc.EJournalTraceNo = InTraceNo;

            Tc.OriginName = "OurATMs-Repl : " + InAtmNo; 

            Tc.AtmNo = InAtmNo;
            Tc.SesNo = InSesNo;
            Tc.BankId = WBankId;
       
            Tc.BranchId = Ac.Branch;

            Tc.AtmDtTime = DateTime.Now; // THIS IS A TEMPORARY SOLUTION 
            //Tc.HostDtTime = DateTime.Now;

            Tc.SystemTarget = 9;
            Tc.TransType = 12; // Debit 
            Tc.TransDesc = "Machine Cash out from ATM";
            Tc.CardNo = "";
            Tc.CardOrigin = 1;

            Acc.ReadAndFindAccount("1000", Ac.Operator, InAtmNo, InCurrNm, "ATM Cash");
            if (Acc.RecordFound == false)
            {
                ErrorFound = true;
                ErrorOutput = "Account not found for ATMNo: " + InAtmNo;
                return; 
             //   MessageBox.Show("Account not found for ATMNo: " + InAtmNo);
            }
            Tc.AccNo = Acc.AccNo;  // Cash account No

            Tc.AuthCode = 0;
            Tc.RefNumb = 0;
            Tc.RemNo = 0;

            Tc.TransMsg = "";
            Tc.AtmMsg = "";
            Tc.ErrNo = 0;
            Tc.StartTrxn = WStartTrxn;
            Tc.EndTrxn = WEndTrxn;
            Tc.SuccTran = true;

            Tc.Operator = Ac.Operator; 

            // ADD THE TRANSACTION 
            Tc.InsertTransInPool(InAtmNo);
        }

        //
        // Insert Build and Insert Transaction in CIT Statement 
        //

 //private void InsertTranInCit(string InAtmNo, int InSesNo, int InTraceNo,
 //                                     string InCurrNm, decimal InTranAmount,
 //                                     int InTransType,string InTransDesc)
 //       {
 //           //
 //           // BUILD THE TRANSACTION 
 //           //          

 //           Ac.ReadAtm(InAtmNo);

 //           Acc.ReadAndFindAccountForCitId(WCitId, Ac.Operator, InCurrNm, "CIT Cash");

 //           // Create transaction for this ATM In CIT Cash Statement 
 //           //TEST
 //           Cs.TranOrigin = 7;
 //           Cs.CitId = WCitId;
 //           Cs.AccNo = Acc.AccNo;
 //           Cs.AtmNo = InAtmNo;
 //           Cs.ReplCycle = 0;
 //           Cs.BankId = WBankId;
      
 //           Cs.TranDtTime = DateTime.Today;
 //           Cs.TransType = InTransType;
 //           Cs.TransDesc = InTransDesc;
 //           //TEST
     
 //           Cs.CurrDesc = InCurrNm;
 //           Cs.TranAmount = InTranAmount;
 //           Cs.ValueDate = DateTime.Today;
 //           Cs.OpenRecord = true;

 //           Cs.Operator = Ac.Operator;

 //           Cs.InsertTranInCit(WCitId, Cs.AccNo); 
      
 //       }
    }
}
