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
    public class RRDMJTMQueueCreationClass
    {
        public int MsgID;

        public DateTime MsgDateTime;
        public string RequestorID;
        public string RequestorMachine;
        public string Command;

        public int Priority;
        public string BatchID;
        public string ATMNo;
        public string ATMMachineName;

        public string ATMAccessID;
        public string ATMAccessPassword;

        public int Stage;
        public int ResultCode;
        public string ResultMessage;

        public DateTime FileUploadStart;
        public DateTime FileUploadEnd;
        public DateTime FileParseStart;
        public DateTime FileParseEnd;

        public int TotalRecords;

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        string connectionString = ConfigurationManager.ConnectionStrings
           ["ATMSConnectionString"].ConnectionString;

        RRDMAtmsClass Ac = new RRDMAtmsClass();

        RRDMJTMIdentificationDetailsClass Jd = new RRDMJTMIdentificationDetailsClass();

        RRDMJTMQueue Jq = new RRDMJTMQueue(); 

        //
        // Method called from Application or Choreographer for Inserting Records in Queue for E-Journal reading
        //

        public void InsertRecordsInJTMQueue(string InRequestor, string InCommand, string InMode , int InPriority ,string InBatch, string InAtmNo ) 
        {
            // InCommand : GET
            //           : DELETE
            // InMode    : SingleAtm
            //           : Group 
            //           : AllAtms

            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            try
            {

                Jd.ReadJTMIdentificationDetailsToFillTable(InMode, InBatch, InAtmNo);

                if (Jd.RecordFound == true)
                {
                    RecordFound = true;

                    TotalRecords = Jd.TotalSelected; 
                    // Read all records of created Table and insert in Queque 

                    int I = 0;

                    while (I <= (Jd.ATMsJournalDetailsSelected.Rows.Count - 1))
                    {

                        Jq.MsgDateTime = DateTime.Now;
                        Jq.RequestorID = InRequestor;
                        Jq.RequestorMachine = "Panicos Machine";

                        Jq.Command = InCommand;

                       
                        Jq.Priority = InPriority;

                        
                        Jq.BatchID = InBatch;

                        Jq.AtmNo = (string)Jd.ATMsJournalDetailsSelected.Rows[I]["AtmNo"];
                        Jq.BankID = (string)Jd.ATMsJournalDetailsSelected.Rows[I]["BankID"];
                        Jq.ATMIPAddress = (string)Jd.ATMsJournalDetailsSelected.Rows[I]["ATMIPAddress"];

                        Jq.ATMMachineName = (string)Jd.ATMsJournalDetailsSelected.Rows[I]["ATMMachineName"];
                        Jq.ATMWindowsAuth = (bool)Jd.ATMsJournalDetailsSelected.Rows[I]["ATMWindowsAuth"];
                        Jq.ATMAccessID = (string)Jd.ATMsJournalDetailsSelected.Rows[I]["ATMAccessID"];

                        Jq.ATMAccessPassword = (string)Jd.ATMsJournalDetailsSelected.Rows[I]["ATMAccessPassword"];
                        Jq.TypeOfJournal = (string)Jd.ATMsJournalDetailsSelected.Rows[I]["TypeOfJournal"];
                        Jq.SourceFileName = (string)Jd.ATMsJournalDetailsSelected.Rows[I]["SourceFileName"];

                        Jq.SourceFilePath = (string)Jd.ATMsJournalDetailsSelected.Rows[I]["SourceFilePath"];
                        Jq.DestnFilePath = (string)Jd.ATMsJournalDetailsSelected.Rows[I]["DestnFilePath"];
                        Jq.Operator = (string)Jd.ATMsJournalDetailsSelected.Rows[I]["Operator"];

                        Jq.InsertNewRecordInJTMQueue();

                        Jd.ReadJTMIdentificationDetailsByAtmNo(Jq.AtmNo);

                        Jd.FileUploadRequestDt = Jq.MsgDateTime;
                        Jd.ResultCode = 0 ;
                        Jd.ResultMessage = ""; 

                        Jd.UpdateRecordInJTMIdentificationDetailsByAtmNo(Jq.AtmNo); 

                        I++;
                    }
                }
                else
                {
                    RecordFound = false;
                }

            }
                catch (Exception ex)
                {

                    ErrorFound = true;
                    ErrorOutput = "An error occured In InsertRecordsInJTMQueue(string InRequestor, string InCommand, string InMode , string InBatch, string InAtmNo ) ......... " + ex.Message;

                }
        }

    }
}
