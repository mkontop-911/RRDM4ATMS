using System;
using System.Text;
//using System.Windows.Forms;
using System.Configuration;

namespace RRDM4ATMs
{
    public class RRDMUpdateAuthUserForSpecialGrids : Logger
    {
        public RRDMUpdateAuthUserForSpecialGrids() : base() { }
        // UPDATE AUTHORISED USER FOR TRANSACTIONS TO BE POSTED GRID 

        //  public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput; 

        string connectionString = AppConfig.GetConnectionString("ATMSConnectionString");

        RRDMAllowedAtmsAndUpdateFromJournal Aj = new RRDMAllowedAtmsAndUpdateFromJournal();
        RRDMUpdateGrids Ug = new RRDMUpdateGrids();
        RRDMTransAndTransToBePostedClass Tc = new RRDMTransAndTransToBePostedClass(); 

        string WSignedId;
        int WSignRecordNo;
   //     int WSecLevel;
        string WOperator;
   
        int WMode;

        public void UpdateAuthUserForTransToBePostedMethod(string InSignedId, int InSignRecordNo, 
                                          string InOperator, int InMode)
        {
            ErrorFound = false;
            ErrorOutput = ""; 

            WSignedId = InSignedId;
            WSignRecordNo = InSignRecordNo;
  
            WOperator = InOperator;

            WMode = InMode; // Mode 1 = no updatind of lastest Journals is needed
                            // Updating of Latest Journals is needed

                try
                {
                    // Define in ATMS Main all ATMs this user can access
                    // Also a table is created 
                   // LEAVE THIS HERE. DO NOT REMOVE 
                    string WFunction = "Reconc";
                    Aj.CreateTableOfAccess(WSignedId, WSignRecordNo, WOperator, WFunction);

                    if (WMode == 2)
                    {
                        // From eJournal update traces and transactions based on table  
                        Aj.UpdateLatestEjStatus(WSignedId, WSignRecordNo, WOperator);

                    }
                    
                    // THE BELOW IS SPECIFIC FOR TRANS TO BE POSTED

                    // Clear AuthUser in TranstoBe posted table
                    // this make the table initialised from this user

                    Tc.ClearTransToBePostedAuthUser(WSignedId);

                    // UPDATE TRANSACTIONS TO BE POSTED WITH AUTHORISED USER 
                    // Included to this there is Tc.UpdateTransToBePostedAuthUser(AtmNo, InUser) where each line is updated with Auth User 
                    Ug.ReadAtmsAndUpdateTransToBePostedAuthUser(Aj.dtAtmsMain, WSignedId);
                    
                }
                catch (Exception ex)
                {

                CatchDetails(ex);
            }
        }


    }
}

