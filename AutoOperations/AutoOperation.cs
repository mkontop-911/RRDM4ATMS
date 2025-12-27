using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading;
using RRDM4ATMs;
using TWF_ProcessFiles;
using RRDMAgent_Classes;
using System.Globalization;
using Microsoft.Win32;
using System.Security.Principal;


namespace AutoOperations
{
    //public class AutoOperation
    //{
    //}
    public abstract class Logger
    {
        public bool HasErrors { get; set; }
        public int LogErrorNo { get; set; }
        // public List<string> Errors { get; set; }
        public string ErrorDetails { get; set; }
        public void CatchDetails(Exception ex)
        {
            HasErrors = true;

            //if (Errors == null)
            //{
            //    Errors = new List<string>();
            //}
            RRDMLog4Net Log = new RRDMLog4Net();

            StringBuilder WParameters = new StringBuilder();

            string WDatetime = DateTime.Now.ToString();

            WParameters.Append("User : ");
            WParameters.Append("NotAssignYet");
            WParameters.Append(Environment.NewLine);

            WParameters.Append("DtTm : ");
            WParameters.Append(WDatetime);
            WParameters.Append(Environment.NewLine);

            string Logger = "RRDM4Atms";
            string Parameters = WParameters.ToString();

            Log.CreateAndInsertRRDMLog4NetMessage(ex, Logger, Parameters);

            LogErrorNo = Log.ErrorNo;
            if (Environment.UserInteractive)
            {
                //  System.Windows.Forms.MessageBox.Show("There is an issue to be reported to the helpdesk " + Environment.NewLine
                //                                          + "Issue reference number: " + Log.ErrorNo.ToString());
            }
            ErrorDetails = ("There is an issue to be reported to the helpdesk " + Environment.NewLine
                                                         + "Issue reference number: " + Log.ErrorNo.ToString());
            //    Environment.Exit(0);}
        }
    }

    public class RRDM_Auto_Load_Match : Logger
    {
        private static IConfiguration _staticConfiguration;

        /// <summary>
        /// Configures the static IConfiguration instance for the class.
        /// Call this method from consuming applications to inject configuration.
        /// </summary>
        /// <param name="configuration">The IConfiguration instance to use</param>
        public static void ConfigureConfiguration(IConfiguration configuration)
        {
            _staticConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public RRDM_Auto_Load_Match() : base() 
        {
            // Initialize configuration if not already set
            if (_staticConfiguration == null)
            {
                // Fallback: Build configuration from appsettings.json
                _staticConfiguration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
            }

            // Initialize connection strings from configuration
            ATMSconnectionString = _staticConfiguration.GetConnectionString("ATMSConnectionString");
            recconConnString = _staticConfiguration.GetConnectionString("ReconConnectionString");
        }

        public bool RecordFound;
        public bool ErrorFound;
        public string ErrorOutput;

        DateTime NullPastDate = new DateTime(1900, 01, 01);

        string SqlString; // Do not delete 

        //************************************************
        //

        public DataTable ReversalsDataTable = new DataTable();

        public DataTable SavedSourceFilesDataTable = new DataTable();

        public int TotalSelected;

        string WSignedId = "Controller";
        string WOperator = "BCAIEGCX";
        string WJobCategory = "ATMs";

        int WReconcCycleNo;
        DateTime WCut_Off_Date;
        string ReversedCut_Off_Date;

        string TotalProgressText;

        string TotalProgressTextOpenForm;

        string ProcessName;
        string Message;
        int Mode;

        string PRX;

        int Counter;

        bool IsServiceAvailable;

        bool IsMatchingDone;

        DateTime SavedStartDt;

        bool J_UnderLoading;

        bool FirstMessage;

        int WServiceReqID;
        bool CommandSent;


        readonly string ATMSconnectionString;
        readonly string recconConnString;

        RRDMReconcJobCycles Rjc = new RRDMReconcJobCycles();
        RRDMMatchingSourceFiles Rs = new RRDMMatchingSourceFiles();

        RRDMMatchingCategories Mc = new RRDMMatchingCategories();

        RRDMReconcCategories Rc = new RRDMReconcCategories();

        RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();

        RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

        RRDM_LoadFiles_InGeneral_EMR_BDC Lf_BDC = new RRDM_LoadFiles_InGeneral_EMR_BDC();

        //RRDM_LoadFiles_InGeneral_EMR_ABE Lf_ABE = new RRDM_LoadFiles_InGeneral_EMR_ABE();

        RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4 Mt = new RRDMMatchingOfTxns_V02_MinMaxDt_BDC_4();
        //RRDMMatchingOfTxns_V02_MinMaxDt_BDC_5 Mt = new RRDMMatchingOfTxns_V02_MinMaxDt_BDC_5();

        RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();

        RRDMJournalReadTxns_Text_Class Jc = new RRDMJournalReadTxns_Text_Class();

        RRDMAgentQueue Aq = new RRDMAgentQueue();

        RRDMGasParameters Gp = new RRDMGasParameters();

        RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

        RRDMReconcCategoriesSessions Rcs = new RRDMReconcCategoriesSessions();


        //
        // Create New Cycle 
        //
        int WReconcCycleNoFirst;
        DateTime WFirstCut_Off_Date; 

        public void MasterAuto(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

           
            // Action 1 - Create Cycle
            // Do we have one every day? Ask DINA
            /*
            Mode = 7; // Updating Action 
            ProcessName = "Auto_Processed";
            Message = "Creating Cycle Starts and Check existance of files" ;
            SavedStartDt = DateTime.Now;

            WReconcCycleNo = CreateNewCycle(WSignedId);

            Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);

            WCut_Off_Date = Rjc.Cut_Off_Date;

            ReversedCut_Off_Date = WCut_Off_Date.ToString("yyyyMMdd");

            WReconcCycleNoFirst = Rjc.ReadFirstReconcJobCycle(WOperator, WJobCategory);

            if (WReconcCycleNo == 0)
            {
                if (Environment.UserInteractive)
                {
                    //MessageBox.Show("Cut Off Cycle is Zero. Start a new Cycle.");
                    //return;
                }
            }
            else
            {
                //
                // This is the first date of the system 
                // 
                WFirstCut_Off_Date = Rjc.Cut_Off_Date.Date;
            }

            Mode = 7; // Updating Action - Completion
            ProcessName = "Auto_Processed";
            Message = "Cycle created.. " + WReconcCycleNo.ToString();

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);


            //ReversedCut_Off_Date = "20240108";
            //
            // Check that IST and Flexcube exist in directories 
            //
            // CHECK FOR IST
            //
            bool IsIstPresentInDirectory = false;
            string[] specificFiles = { };
            string WSourceFileId = "Switch_IST_Txns";
            //Switch_IST_Txns_20240108
            //Flexcube_20230501

            string InSourceDirectory = "C:\\RRDM\\FilePool\\Switch_IST_Txns";
            specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_" + ReversedCut_Off_Date + ".???");

            if (specificFiles == null || specificFiles.Length == 0)
            {
                IsIstPresentInDirectory = false;
            }
            else
            {
                IsIstPresentInDirectory = true;
            }
            //
            // CHECK FOR FLEXCUBE
            //
            bool IsFlexcubePresentInDirectory = false;
            WSourceFileId = "Flexcube";
            //Switch_IST_Txns_20240108
            //Flexcube_20230501
            InSourceDirectory = "C:\\RRDM\\FilePool\\Flexcube";
            specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_" + ReversedCut_Off_Date + ".???");

            if (specificFiles == null || specificFiles.Length == 0)
            {
                IsFlexcubePresentInDirectory = false;
            }
            else
            {
                IsFlexcubePresentInDirectory = true;
            }

            if (IsIstPresentInDirectory == true & IsFlexcubePresentInDirectory == true)
            {
                // If both files are present 
                // Continue

                Mode = 7; // Updating Action - Completion
                ProcessName = "Auto_Processed";
                Message = "IST AND Flexcube are present " ;

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", DateTime.Now, DateTime.Now, Message, WSignedId, WReconcCycleNo);


            }
            else
            {
                Mode = 7; // Updating Action - Completion
                ProcessName = "Auto_Processed";
                Message = "Cannot proceed.. IST AND Flexcube not both present ";

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", DateTime.Now, DateTime.Now, Message, WSignedId, WReconcCycleNo);

                return;
            }
            */

            // Action 2 - Move Journals to the directory

            Mode = 7; // Start Action 
            ProcessName = "Auto_Processed"; 
            Message = "Moving Journals to directory starts." ;
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);

          
            UtilityForJournalsMove(WSignedId, WReconcCycleNo, WCut_Off_Date);

            //
            Mode = 7; // Updating Action - Completion
            ProcessName = "Auto_Processed";
            Message = "Moving Journals Finishes. ";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);



            // Action 3 - Load Journals
            // 
            Mode = 7; // Start Action 
            ProcessName = "Auto_Processed";
            Message = "Loading Journals.";
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);

            Method_LoadJournals(WSignedId, WReconcCycleNo, WCut_Off_Date);

           
            if (IsServiceAvailable == false)
            {
                Mode = 7; // 
                ProcessName = "Auto_Processed";
                Message = "Service Not Available. Proces stops";
                SavedStartDt = DateTime.Now;

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);


                return; 
            }

                //
            Mode = 7; // Updating Action - Completion
            ProcessName = "Auto_Processed";
            Message = "Loading of Journals Finishes. ";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);



            //specificFiles = Directory.GetFiles(InSourceDirectory, WSourceFileId + "_????????.???.???");
            //  CheckIfMinimumFilesExists(string InOperator, string InSourceFileId, string InSourceDirectory, DateTime InDateExpected, string InFileNameMask, int InMode)
            //
            // Action 4 - Load Files
            // Action 3 - Load Journals
            // 
            Mode = 7; // Start Action 
            ProcessName = "Auto_Processed";
            Message = "Loading Files Starts.";
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);

            LoadFilesProcess(WSignedId, WReconcCycleNo, WCut_Off_Date);

            if (IsFilesLoaded == "NO")
            {

                return;
            }
            else
            {
                // Continue 
            }

            //
            Mode = 7; // Updating Action - Completion
            ProcessName = "Auto_Processed";
            Message = "Loading of Files Finishes. ";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);

            //***********************
            //MATCHING AND THE REST
            //**********************
            Mode = 7; // Matching
            ProcessName = "Auto_Processed";
            Message = "Matching Starts. Cycle:.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);

            //

            // Action 5 - Do Matching 
            Method_DoMatching(WSignedId, WReconcCycleNo, WCut_Off_Date);

            Mode = 7; // Updating Action - Completion
            ProcessName = "Auto_Processed";
            Message = "Matching and Moving Records Finishes. ";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);

        }

        //
        // Create New Cycle 
        //


        public int CreateNewCycle(string InUserId)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Current Cycle Number 

            WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
            //
            // NEW CYCLE
            //
            int NewReconcCycleNo = Rjc.Create_A_New_ReconcJobCycle(WOperator, WSignedId, Rjc.Cut_Off_Date.AddDays(1));

            return NewReconcCycleNo;
        }
        //
        // USE UTILITY TO MOVE Journals to the Directory 
        //
        public void UtilityForJournalsMove(string InUserId, int InRRDMCycle, DateTime InCutOffDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            // Find NCR START DATE TIME 
            DateTime NCR_Start = DateTime.Now; 
            string ParId = "862"; // Date and Time 
            string OccurId = "01"; // 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound)
            {
                try
                {
                    NCR_Start = Convert.ToDateTime(Gp.OccuranceNm);
                }
                catch (Exception ex)
                {
                    if (Environment.UserInteractive)
                    {
                       // MessageBox.Show("822 parameter date is wrong for Meeza");
                    }

                    ErrorFound = true;
                    CatchDetails(ex);
                }
               }

            // Find NCR Source Directory 
            string  NCR_SourchDirectory = ""; 
             ParId = "861"; // Source Directory and Traget Directories  
            OccurId = "01"; // 01 For Source Dir NCR // 03 For Source Dir for Wincor 
                            // 11 for NCR Target Directory // 13 for Wincor Target Directory

            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound)
            {
                try
                {
                    NCR_SourchDirectory = Gp.OccuranceNm;
                }
                catch (Exception ex)
                {
                    if (Environment.UserInteractive)
                    {
                        // MessageBox.Show("822 parameter date is wrong for Meeza");
                    }

                    ErrorFound = true;
                    CatchDetails(ex);
                }
            }

            DateTime WDateTimeNow = DateTime.Now;

            var processJournalsNCR = new TWF_ProcessFiles.ProcessFiles
            {
                ReferenceDate = new DateTime(2024, 5, 1),
                MaxReturnDate = new DateTime(2000, 1, 1),
                SourceDirectory = @"\\192.168.10.10\C$\INPUT\NCR",
                TargetDirectory = @"C:\Temp\Files\OUTPUT",
                TrasferType = TWF_ProcessFiles.ProcessFiles.FileTransferType.NCR
            };
            processJournalsNCR.filesProcessing();
            Console.WriteLine(processJournalsNCR.NoOfFiles);
            Console.WriteLine(processJournalsNCR.MaxReturnDate);
            Console.ReadLine();

            if (processJournalsNCR.ErrorIndicator == false)
            {
                DateTime Kntoreturn = processJournalsNCR.MaxReturnDate; 
                // For each type of Journals KONTO utility will give the max date Time read
                // This will be kept in parameters to be the next starting point. 
                // MAX MUST BE GIVEN BY KONTO
                string WDt = Kntoreturn.ToString();
                string TempParId = "862"; // Date and Time 
                string TempOccurId = "01"; // 
                string TempOccuranceNm = WDt; 
                Gp.UpdateGasParamByParamIdAndOccur(WOperator, TempParId,
                                                                      TempOccurId
                                                                      , TempOccuranceNm); 
            }

        }
        //
        // Check loaded journals and Call Alecos Service
        //
        public void Method_LoadJournals(string InUserId, int InRRDMCycle, DateTime InCutOffDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";

            DateTime WCut_Off_Date = InCutOffDate;

            string SourceFileId = "Atms_Journals_Txns";

            Rs.ReadReconcSourceFilesByFileId(SourceFileId);

            string InSourceDirectory = Rs.SourceDirectory;

            string[] allJournals = Directory.GetFiles(InSourceDirectory, "*.*");

            if (allJournals.Length == 0)
            {
                // MessageBox.Show(" There are no files for loading");
                //textBox1.Text = "0";
                return;
            }
            else
            {
                // DELETE jln
                foreach (string file in allJournals)
                {
                    string myFilePath = @file;
                    string ext = Path.GetExtension(myFilePath);

                    if (ext == ".jln")
                    {
                        File.Delete(file);
                        // Count1 = Count1 + 1;
                    }
                }
                // MessageBox.Show("Deleted Journals .jln..=.." + Count1.ToString());
                // After this check you 
                // Check if Dublicate and delete 
                allJournals = Directory.GetFiles(InSourceDirectory, "*.*");

                if (allJournals.Length == 0)
                {
                    // Re Check here
                    // MessageBox.Show(" There are no files for loading");
                    //textBox1.Text = "0";
                    //textBoxMsgBoard.Text = "Current Status:Ready";
                    return;
                }
                int Count2 = 0;
                foreach (string file in allJournals)
                {
                    if (Rs.CheckIfFileIsDublicate(file) == true)
                    {
                        // Delete File 
                        File.Delete(file);
                        Count2 = Count2 + 1;
                    }

                }

                if (Count2 > 0)
                {
                    /*
                     // MessageBox.Show("Journals found that were loaded before." + Environment.NewLine
                             //  + "Number of journals :.. " + Count2.ToString()
                             //  + "...These journals were deleted from directory"
                                                                            );
                                                                            */
                }

                allJournals = Directory.GetFiles(InSourceDirectory, "*.*");

                if (allJournals.Length == 0)
                {
                    // Re Check here
                    //MessageBox.Show(" There are no files for loading");
                    //textBox1.Text = "0";
                    //textBoxMsgBoard.Text = "Current Status:Ready";
                    return;
                }

            }

            // 
            // ************************
            // CREATE NOT PRESENT ATMS
            // ************************
            RRDMAtmsClass Ac = new RRDMAtmsClass();
            string DateString;
            string result0;
            string Temp;
            string result1;
            string Vendor;
            string JournalName;
            string WAtmNo;
            string TempEjournalTypeId = "N/A";
            int WServiceReqID;
            bool CommandSent;


            foreach (string file in allJournals)
            {
                DateString = file.Substring(file.Length - 19);
                DateString = DateString.Substring(0, 8);

                bool Condition = false;

                DateTime FileDATEresult;

                if (DateTime.TryParseExact(DateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out FileDATEresult))
                {

                }

                if (Condition == true)
                {
                    if (FileDATEresult > WCut_Off_Date)
                    {
                        /*
                        MessageBox.Show("There are Journals with Date Creater than the Cycle date." + Environment.NewLine
                                         + "This is not allowed. Remove and restart." + Environment.NewLine
                                         + "Journal is: " + file
                                        );
                                        */
                        return;
                    }
                }

                result0 = file.Substring(file.Length - 11);
                Temp = result0.Substring(0, 4);
                if (Temp == "_EJ_")
                {
                    // Valid
                    result1 = file.Substring(file.Length - 7);
                    Vendor = result1.Substring(0, 3);
                    JournalName = file.Substring(file.Length - 28);
                    WAtmNo = JournalName.Substring(0, 8);
                    // 00000102_20191024_EJ_DBL.000
                    Ac.ReadAtm(WAtmNo);
                    if (Ac.RecordFound == true)
                    {
                        // DO NOTHING WE WILL UPDATE JOURNAL LATER
                        //// ATM Found
                        //if (Vendor == "NCR") TempEjournalTypeId = "NCR_01";
                        //if (Vendor == "DBL") TempEjournalTypeId = "DBLD_01";
                        //if (Vendor == "WCR") TempEjournalTypeId = "Wincor_01";

                        //if (TempEjournalTypeId != Ac.EjournalTypeId)
                        //Ac.UpdateEjournalTypeId(WAtmNo, TempEjournalTypeId);
                    }
                    else
                    {
                        // Insert ATM_No
                        Ac.CreateNewAtmBasedOnGeneral_Model(WOperator, WAtmNo, JournalName);
                    }
                }
                else
                {
                    // Not Valid 
                }

            }

            //************************************
            //************************************
            // TRUNCATE Temp Table 
            // Do it every morning 
            //  RRDMJournalReadTxns_Text_Class Jc = new RRDMJournalReadTxns_Text_Class();
            string WFile = "[ATM_MT_Journals_AUDI].[dbo].[tblHstEjText_Short]";
            Jc.TruncateTempTable(WFile);


            string ParamId;

            // Check Service status

            string ServiceId = "10"; // For Journals

            bool Available = ServiceAvailableStatus(ServiceId);

            if (Available == true)
            {
                // Start Service
                //
                IsServiceAvailable = true;
                // Insert Command 
                Aq.ReqDateTime = DateTime.Now;
                Aq.RequestorID = WSignedId;
                Aq.RequestorMachine = Environment.MachineName;
                Aq.Command = "SERVICE_START_AND_MONITOR";
                Aq.ServiceId = ServiceId;
                // 
                ParamId = "915";

                Gp.ReadParameterByOccuranceId(ParamId, Aq.ServiceId);
                if (Gp.RecordFound == true)
                {
                    Aq.ServiceName = Gp.OccuranceNm;
                }
                else
                {
                    Aq.ServiceName = "Not Specified";
                }

                Aq.Priority = 0; // Highest
                Aq.Operator = WOperator;
                Aq.OriginalReqID = 0;
                Aq.OriginalRequestorID = WSignedId;

                WServiceReqID = Aq.InsertNewRecordInAgentQueue();



            }
            else
            {
                // If for example is "SERVICE_START_AND_MONITOR" then Journals are in loading process
                if (Environment.UserInteractive)
                {
                    string msg = string.Format("Service was in status: {0}", serviceStatus + Environment.NewLine
                    + "Go to Task Manager Service section and start 'RRDM Agent' "
                    );
                   // MessageBox.Show(msg);
                }

                IsServiceAvailable = false; 


                return;
            }

            J_UnderLoading = true;

            Rjc.ReadReconcJobCyclesById(WOperator, WReconcCycleNo);

            Rjc.SpareBool_1 = true; // Journal Loading strts
            Rjc.SpareInt_1 = WServiceReqID; // Requested service

            Rjc.UpdateSpecialFields(WReconcCycleNo);


            //*******************************
            Mode = 5; // Updating Action 
            ProcessName = "LoadingOfJournals";
            Message = "Loading Of Journals starts -Cycle:.." + WReconcCycleNo.ToString() + " - Request Id " + WServiceReqID.ToString();
            DateTime J_SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", J_SavedStartDt, J_SavedStartDt, Message, WSignedId, WReconcCycleNo);

            LoadJournalsFinish(); 

            if (IsJournalsLoaded == true)
            {
                // Continue 
            }
            else
            {
                // Journals not loaded 
            }
        }

        // CHECK WHEN JOURNAL LOADING FINISH
        //
        bool IsJournalsLoaded; 
        public void LoadJournalsFinish()
        {
            bool JournalsUnderLoading = true;
            IsJournalsLoaded = false; 

            while (JournalsUnderLoading == true)
            {
                // Wait 
                Thread.Sleep(20000); // sleep 20 seconds 

                // Check 
                WReconcCycleNo = Rjc.ReadLastReconcJobCycleATMsAndNostroWithMinusOne(WOperator, WJobCategory);
                // WCut_Off_Date = Rjc.Cut_Off_Date;
                bool WJournalLoadingStarted = Rjc.SpareBool_1;
                int WQueueId = Rjc.SpareInt_1;

                RRDMAgentQueue Aq = new RRDMAgentQueue();

                if (WJournalLoadingStarted == true & WQueueId > 0)
                {

                    string WSelectionCriteria = " WHERE OriginalReqId =" + WQueueId
                                      + " AND OriginalRequestorID ='" + WSignedId + "'";
                    Aq.ReadAgentQueueBySelectionCriteria(WSelectionCriteria);

                    if (Aq.RecordFound == true & Aq.MessageSent == false)
                    {
                        //
                        // Update as messagesent = true
                        //
                        // Save date 
                        int SavedSeqNo = Aq.ReqID;
                        DateTime SavedDateFinish = Aq.CmdExecStarted; // From Record Just read


                        // Get Date from 
                        WSelectionCriteria = " Where ReqID =" + WQueueId; // This is the requested Id
                        Aq.ReadAgentQueueBySelectionCriteria(WSelectionCriteria);

                        DateTime SavedDateStart = Aq.CmdExecStarted;

                        if (SavedDateFinish >= SavedDateStart)
                        {
                            // Update Record
                            Aq.UpdateRecordInAgentQueueForMessageSent(SavedSeqNo);

                            //*******************************
                            RRDMReconcFileMonitorLog Flog = new RRDMReconcFileMonitorLog();
                            Flog.ReadLoadedFilesByCycleNumber_All(WReconcCycleNo);

                            RRDMPerformanceTraceClass Pt = new RRDMPerformanceTraceClass();
                            int Mode = 5; // Updating Action 
                            string ProcessName = "LoadingOfJournals";
                            string Message = "Loading Of.." + Flog.Journal_Total.ToString() + "..Journals Finishes - Request Id "
                                           + Aq.ReqID.ToString();

                            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedDateStart, SavedDateFinish, Message, WSignedId, WReconcCycleNo);
                            //*******************************

                            //MessageBox.Show(new Form { TopMost = true }, "Journals Loading Has Finished!");

                            Rjc.ReadReconcJobCyclesById(WOperator, WReconcCycleNo);

                            Rjc.SpareBool_1 = false; // Journal Loading starts
                            Rjc.SpareInt_1 = 0; // Requested service

                            Rjc.UpdateSpecialFields(WReconcCycleNo);

                            // Loading of Journals has finished

                            JournalsUnderLoading = false;

                            IsJournalsLoaded = true; 

                        }
                    }
                    {

                    }
                }

            }

        }
        // Check If Minimum Files are in Directories
        // 
        //
        string IsFilesLoaded; 
        public void LoadFilesProcess(string InUserId, int InRRDMCycle, DateTime InCutOffDate)
        {
            RecordFound = false;
            ErrorFound = false;
            ErrorOutput = "";
            IsFilesLoaded = ""; // YES or NO


            //text = "Loading from Journals DB to Master STARTS";
            //caption = "LOADING";
            //timeout = 5000;
            //AutoClosingMessageBox.Show(text, caption, timeout);

            //MessageBox.Show("Loading from Journals DB to Master STARTS");
            DateTime startTime = DateTime.Now;
            // READ AUDI AND INSERT IN Master file 
            RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2_Pambos2 PambosLoad = new RRDMJournalRead_HstAtmTxns_AndCreateTable_V2_With_SM_BDC_2_Pambos2();
            int WSignRecordNo = 10;
            PambosLoad.ReadJournal_Txns_And_Insert_In_Pool(WSignedId, WSignRecordNo, WOperator, WReconcCycleNo);

            DateTime endTime = DateTime.Now;

            TimeSpan span = endTime.Subtract(startTime);

            //text = "Loading from Journals DB to Master FINISHES" + Environment.NewLine
            //    + "Time Elapsed In Minutes.." + span.TotalMinutes;
            //caption = "LOADING";
            //timeout = 5000;
            //AutoClosingMessageBox.Show(text, caption, timeout);

            //MessageBox.Show("Loading from Journals DB to Master FINISHES" + Environment.NewLine
            //    + "Time Elapsed In Minutes.." + span.TotalMinutes
            //    );

            // CheckLoadingOfJournals();

            if (J_UnderLoading == true)
            {
                //MessageBox.Show("Journals Under Loading");
                //return;
            }
            // Before start we check the sign on users. 
            //bool IsAllowedToSignIn = false;
            //bool ThereAreUsersInSystem = CheckForSignInUsers(IsAllowedToSignIn);

            //if (ThereAreUsersInSystem == true)
            //{
            //    // Decide whether to move forward or not. 
            //}

            // Truncate Table From Audi - tmplog for Pambos
            string WFile = "[ATM_MT_Journals_AUDI].[dbo].[tmplog]";
            Jc.TruncateTempTable(WFile);

            // Truncate Table working table 1 
            WFile = "[RRDM_Reconciliation_ITMX].[dbo].[Working_General_Table]";

            Jc.TruncateTempTable(WFile);

            // Truncate Table 2 
            WFile = "[RRDM_Reconciliation_ITMX].[dbo].[Working_Master_Pool_Report]";

            Jc.TruncateTempTable(WFile);

            startTime = DateTime.Now;

            //text = "Supervisor mode Work starts";
            //caption = "LOADING";
            //timeout = 5000;
            //AutoClosingMessageBox.Show(text, caption, timeout);

            //// MessageBox.Show("Supervisor mode Work starts");
            //textBoxMsgBoard.Text = "Current Status : Supervisor Mode data loading process";

            // CHECK IF RECYCLING ATMS
            string ParId = "948";
            string OccurId = "1"; // 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            {
                RRDMRepl_SupervisorMode_Master_Recycle Smaster = new RRDMRepl_SupervisorMode_Master_Recycle();

                int Sm_Mode = 3;
                Smaster.CreateReplCyclesFrom_Pambos_Details_Of_Supervisor_Mode(WSignedId, WSignRecordNo, WOperator,
                                                                 Sm_Mode, "NO_Form153");

                endTime = DateTime.Now;

                span = endTime.Subtract(startTime);

                //text = "Supervisor mode Work FINISHES" + Environment.NewLine
                //    + "Time Elapsed In Minutes.." + span.TotalMinutes;
                //caption = "LOADING";
                //timeout = 5000;
                //AutoClosingMessageBox.Show(text, caption, timeout);

                //MessageBox.Show("Supervisor mode Work FINISHES" + Environment.NewLine
                //    + "Time Elapsed In Minutes.." + span.TotalMinutes
                //    );
            }
            else
            {
                //
                // Supervisor mode
                //
                RRDMRepl_SupervisorMode_Master Smaster = new RRDMRepl_SupervisorMode_Master();
                int Sm_Mode = 3;
                Smaster.CreateReplCyclesFrom_Pambos_Details_Of_Supervisor_Mode(WSignedId, WSignRecordNo, WOperator,
                                                                 Sm_Mode, "NO_Form153");

                endTime = DateTime.Now;

                span = endTime.Subtract(startTime);

                //MessageBox.Show("Supervisor mode Work FINISHES" + Environment.NewLine
                //    + "Time Elapsed In Minutes.." + span.TotalMinutes
                //    );
            }


            // CLEAR Duplicates due to NCR problems
            //*****************************************

            ParId = "103";
            OccurId = "1"; // 
            Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
            if (Gp.RecordFound & Gp.OccuranceNm == "YES")
            {
                // Means delete duplicates from loaded journals due to NCR Vision problem 

                // Dublicates in A : 


                RRDMMatchingTxns_MasterPoolATMs Mpa = new RRDMMatchingTxns_MasterPoolATMs();

                Mpa.DeleteDuplicates_NCR_Vision(WReconcCycleNo);

                if (Mpa.Count > 0)
                {
                    //text = "Deleted Duplicates due to NCR Vision problem." + Environment.NewLine
                    //         + "Number deleted..=" + Mpa.Count.ToString();
                    //caption = "NCR DUPLICATE";
                    //timeout = 5000;
                    //AutoClosingMessageBox.Show(text, caption, timeout);
                    //MessageBox.Show("Deleted Duplicates due to NCR Vision problem." + Environment.NewLine
                    //         + "Number deleted..=" + Mpa.Count.ToString()
                    //          );
                }
                else
                {
                    //MessageBox.Show("NO Duplicates FOUND due NCR Vision problem." + Environment.NewLine
                    //                + "Maybe NCR has corrected problem." + Environment.NewLine
                    //                + " Please check and report"
                    //                 );
                }
            }




            //*******************************
            Mode = 5; // Updating Action 
            ProcessName = "LoadingOfFiles";
            Message = "Loading Of Files starts. Cycle:.." + WReconcCycleNo.ToString();
            SavedStartDt = DateTime.Now;

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);
            //******************************
            // ************************
            // UPDATE CORRECT JOURNAL AND CREATE NOT PRESENT ATMS
            // ************************

            Jc.ReadJournal_tmpATMs_Journal_TypeAndUpdateAtms(WOperator);

            // Truncate Table 
            WFile = "[ATM_MT_Journals_AUDI].[dbo].[tmpATMs_Journal_Type]";

            Jc.TruncateTempTable(WFile);


            // 
            //IST 01 / 07

            //if (W_Application == "e_MOBILE")
            //{
            //    // Get only the e_MOBILE
            //    string Filter1 = "Operator = '" + WOperator + "' AND Enabled = 1 AND TableStructureId = 'MOBILE_WALLET' ";

            //    Rs.ReadReconcSourceFilesToFillDataTableForExistanceInDir(WOperator, Filter1, WReconcCycleNo, WCut_Off_Date);

            //}
            //else
            //{
            // Normal Case
            string Filter1 = "Operator = '" + WOperator + "' AND Enabled = 1  AND TableStructureId = 'Atms And Cards' ";

            Rs.ReadReconcSourceFilesToFillDataTableForExistanceInDir(WOperator, Filter1, WReconcCycleNo, InCutOffDate);

            //}

            SavedSourceFilesDataTable = Rs.Table_Files_In_Dir;

            //buttonLoad.Show();

            // LOAD FILES
            //******************************
            //
            METHOD_LoadFiles(InCutOffDate);
            //
            //****************************
            //
            Mode = 5; // Updating Action 
            ProcessName = "LoadingOfFiles";
            Message = "Loading Of Files Finishes. ";

            Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
            //*********************************
            Flog.ReadLoadedFiles_Fill_Table(WOperator, WReconcCycleNo);

            if (Flog.IsFileNOTLoaded == true)

            {
                // Problem 
                IsFilesLoaded = "NO";
            }
            else
            {
                IsFilesLoaded = "YES"; 
            }
            

        }

        int FlogSeqNo;
        private void METHOD_LoadFiles(DateTime WCut_Off_Date)
        {
            RRDMMatchingSourceFiles Ms = new RRDMMatchingSourceFiles();
            RRDMMatchingCategoriesVsSourcesFiles Mcf = new RRDMMatchingCategoriesVsSourcesFiles();

            // Start Services 
            // Based on Directories with Files start the services 
            try
            {

                int I = 0;

                while (I <= (SavedSourceFilesDataTable.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    string SourceFileId = (string)SavedSourceFilesDataTable.Rows[I]["SourceFileId"];
                    string FullFileName = (string)SavedSourceFilesDataTable.Rows[I]["FullFileName"];
                    bool IsPresent = (bool)SavedSourceFilesDataTable.Rows[I]["IsPresent"];
                    string IsGood = (string)SavedSourceFilesDataTable.Rows[I]["IsGood"];
                    string DateExpected = (string)SavedSourceFilesDataTable.Rows[I]["DateExpected"];
                    string HASHValue = (string)SavedSourceFilesDataTable.Rows[I]["HASHValue"];

                    // FOR DEALING WITH IST TEMPORARY PROBLEM

                    // Update with -1 = ready for Matched 

                    //if (SourceFileId == "Switch_IST_Txns")
                    //{
                    //    // DUE TO IST PROBLEM DO ALWAYS THIS
                    //    Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne(SourceFileId, WReconcCycleNo);
                    //}

                    if (IsPresent == true & IsGood == "YES" & SourceFileId != "Atms_Journals_Txns")
                    {
                        // Check if still exist in Directory

                            if (IsGood == "YES")
                            {
                                // THIS IS GOOD TO LOAD
                                // THIS IS TEMPORARY - ALECOS WILL DO IT
                                //
                                // Check if already exists with same hash value with success 
                                //

                                Flog.GetRecordByFileHASH(HASHValue);
                                if (Flog.RecordFound)
                                {
                                    // FILE READ BEFORE
                                    //MessageBox.Show("File read before under the name of: " + Environment.NewLine
                                    //    + Flog.ArchivedPath
                                    //    );
                                }

                                RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
                                Mf.ReadReconcSourceFilesByFileId(SourceFileId);
                                // If WORKING WITH SERVICE ALECOS WILL INSERT THIS 
                                Flog.SystemOfOrigin = Mf.SystemOfOrigin;
                                Flog.RMCycleNo = WReconcCycleNo;
                                Flog.SourceFileID = SourceFileId;
                                Flog.StatusVerbose = "";
                                Flog.FileName = FullFileName;
                                Flog.FileSize = 0;
                                Flog.DateTimeReceived = DateTime.Now;
                                Flog.DateExpected = WCut_Off_Date;
                                //Flog.DateOfFile = WCut_Off_Date.Date.Year + "-" + WCut_Off_Date.Date.Month+"-"+WCut_Off_Date.Date.Day;
                                Flog.DateOfFile = WCut_Off_Date.ToString("yyyy-MM-dd");
                                Flog.FileHASH = HASHValue;
                                Flog.LineCount = 999;
                                Flog.stpFuid = 0;
                                Flog.ArchivedPath = FullFileName;
                                Flog.ExceptionPath = "Exception Path";
                                Flog.Status = 0;

                                FlogSeqNo = Flog.Insert(); // WReconcCycleNo
                                                           // LOAD FILE 

                                Lf_BDC.InsertRecordsInTableFromTextFile_InBulk(WOperator, SourceFileId, FullFileName, Ms.InportTableName, Ms.Delimiter, FlogSeqNo);

                                // IT IS TEMPORARY 
                                // ALECOS WILL INSERT THIS

                                //if (Environment.UserInteractive)
                                //{

                                Flog.ReadLoadedFilesBySeqNo(FlogSeqNo);

                                Flog.StatusVerbose = "";

                                Flog.LineCount = Lf_BDC.stpLineCount;

                                Flog.stpReturnCode = Lf_BDC.stpReturnCode;
                                Flog.stpErrorText = Lf_BDC.stpErrorText;
                                Flog.stpReferenceCode = Lf_BDC.stpReferenceCode;

                                if (Lf_BDC.stpReturnCode == 0)
                                    Flog.Status = 1; // Success
                                else Flog.Status = 0; //Failure
                                // Update Flog
                                //Flog.Update(Lf_BDC.WFlogSeqNo);

                                Flog.Update(FlogSeqNo);

                                Flog.Update_MAX_DATE(Flog.SourceFileID, FlogSeqNo, WReconcCycleNo);

                                // Update with -1 = ready for Matched if File is good
                                if (Lf_BDC.stpReturnCode == 0)
                                {
                                    Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne(SourceFileId, WReconcCycleNo);

                                    if (SourceFileId == "Switch_IST_Txns" || SourceFileId == "Flexcube")
                                    {
                                        // LEAVE IT HERE to cover the Twin that are created from Switch_IST
                                        // Make Twin 
                                        Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne("Switch_IST_Txns_TWIN", WReconcCycleNo);

                                    }

                                    if (SourceFileId == "NCR_FOREX")
                                    {
                                        // LEAVE IT HERE to cover if testing with only FOREX
                                        // Make Twin 
                                        Mcf.UpdateReconcCategoryVsSourceRecordProcessCodeToMinusOne("Switch_IST_Txns", WReconcCycleNo);

                                    }
                                }

                                int r = Lf_BDC.stpReturnCode;
                                string D = Lf_BDC.stpErrorText;
                            }

                        

                    }

                    I++; // Read Next entry of the table 
                }

                // FIRST UPDATE TRACE
                //*****************************************************
                // UPDATE TRACES and other infor In Order to sychronise files and have journal lines during reconciliation
                //*****************************************************
                //MessageBox.Show("ALL Files Loaded");
                //MessageBox.Show("We Start update records with traces");
                string text = "Extra work during loading in progress";
                string caption = "LOADING OF FILES";
                int timeout = 2000;
                AutoClosingMessageBox.Show(text, caption, timeout);

                Lf_BDC.UpdateRecordsWithTraceAndOther(WOperator, WReconcCycleNo, FlogSeqNo, 1, WCut_Off_Date);
                //
                // EXTRA FIELDS
                //MessageBox.Show("We Start extra fields");
                Lf_BDC.UpdateFiles_With_EXTRA(WOperator, WReconcCycleNo);
                //MessageBox.Show("We finish extra fields");

            }
            catch (Exception ex)
            {
                CatchDetails(ex);
            }

            // textBoxLoadedFiles.Hide();

        }
        //
        // Do matching only after checking that the minimum files are loaded
        // 
        //
        int AgingDays_HST; 
        public void Method_DoMatching(string InUserId, int InRRDMCycle, DateTime InCutOffDate)
        {
           
                
                // Find the ready categories

                Mt.MatchReadyCategoriesEnquiry(WOperator, WSignedId,
                                         WReconcCycleNo);

                RRDMGasParameters Gp = new RRDMGasParameters();
                string ParamId;

                string OccuranceId;

                DateTime DeleteDate = NullPastDate;

               
                    // YES Proceed

                    // FIND CURRENT HISTORY DATE

                    int TempMode = 0;

                    ParamId = "853";
                    OccuranceId = "5"; // HST

                    Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

                    if (Gp.RecordFound == true)
                    {
                        AgingDays_HST = (int)Gp.Amount; // 

                        //AgingDays_HST = 0; 

                        // Current CutOffdate
                        string WSelection = " WHERE JobCycle =" + WReconcCycleNo;
                        Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

                        DateTime WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-AgingDays_HST);

                        Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(WAgingDateLimit_HST);

                        if (Rjc.RecordFound == true)
                        {
                            DeleteDate = Rjc.Cut_Off_Date.AddDays(1); // leave it here to cover cases of Undo particularly for POS
                            TempMode = 2;
                        }
                        else
                        {
                            DeleteDate = WFirstCut_Off_Date; // Trans before this date will be deleted
                            TempMode = 1;
                        }
                    }
                    else
                    {
                        DeleteDate = WFirstCut_Off_Date; // Trans before this date will be deleted
                        TempMode = 1;
                    }

                    //radioButtonMaster.Checked = true; 

                    // DELETE UNWANTED RECORDS FROM TABLES 
                    // IF WITH TODAYS LOADING TRANSACTIONS COME BEFORE THAT DATE WE DO NOT WANT THEM
                    // WE DELETE THEM NOT TO TAKE PART IN TODAY's LOADING
                    RRDMMatchingTxns_InGeneralTables_BDC Mgt = new RRDMMatchingTxns_InGeneralTables_BDC();

                    Lf_BDC.DeleteRecordsToSetStartingPoint(WOperator, DeleteDate, WCut_Off_Date, WReconcCycleNo, TempMode);
               

                //textBoxMsgBoard.Text = "Current Status : Matching Process";
                // FIRST UPDATE TRACE
                //*****************************************************
                // UPDATE TRACES and other infor In Order to sychronise files and have journal lines during reconciliation
                //*****************************************************

                Lf_BDC.UpdateRecordsWithTraceAndOther(WOperator, WReconcCycleNo, 0, 2, WCut_Off_Date);

                //
                // Before start we check the sign on users.
                //
                //bool IsAllowedToSignIn = false;
                //bool ThereAreUsersInSystem = CheckForSignInUsers(IsAllowedToSignIn);

                //if (ThereAreUsersInSystem == true)
                //{
                //    // Decide whether to move forward or not. 
                //}

                // Clear Tables 
                RRDMAtmsMinMax Mm = new RRDMAtmsMinMax();
                Mm.DeleteTableATMsMinMax();

                //text = "Matching Starts Now";
                //caption = "MATCHING";
                //timeout = 5000;
                //AutoClosingMessageBox.Show(text, caption, timeout);

                //MessageBox.Show("Matching Starts Now");

                //Thread thr16 = new Thread(Method16);
                //thr16.Start();
                //*******************************
                Mode = 5; // Updating Action 
                ProcessName = "MatchingProcess";
                Message = "Matching Process Starts. Cycle:.." + WReconcCycleNo.ToString();
                SavedStartDt = DateTime.Now;

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);
                //******************************


                int WMode = 2; // Do matching for all ready categories 
                               // MATCHING
                               // MATCHING 
                Mt.MatchReadyCategoriesUpdate(WOperator, WSignedId,
                                               WReconcCycleNo);
                // *****************************

                Mode = 5; // Updating Action 
                ProcessName = "MatchingProcess";
                Message = "Matching Process Finishes.";

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
                //*********************************

                bool MatchedPressed = true;
                //
                // UPDATE GL RECORDS 
                //

                int WWMode;
                //Mgt.CreateRecords_CAP_DATE_For_Category(WOperator, WCut_Off_Date, WReconcCycleNo, WWMode);
                WWMode = 2;
                Mgt.CreateRecords_CAP_DATE_For_ATMs(WOperator, WCut_Off_Date, WReconcCycleNo, WWMode);
                // Check if already exists = Already updated for this cycle 

                // UPDATE Mpa with 818 where we Have EGP
                Mpa.UpdateMatchingTxnsMasterPoolATMsCurrency(WOperator);

                bool MasterTwoCurrencies;

                DateTime TwoCcyNewVersionDt = new DateTime(2050, 03, 24);
                string ParId = "822"; // When version of files changes 
                string OccurId = "03"; // For IST and flexube and Meeza Global LCL  
                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");
                if (Gp.RecordFound)
                {
                    try
                    {
                        TwoCcyNewVersionDt = Convert.ToDateTime(Gp.OccuranceNm);

                        MasterTwoCurrencies = true;

                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("822 parameter date is wrong for two currency");
                        MasterTwoCurrencies = false;
                        CatchDetails(ex);
                    }


                    // MessageBox.Show("Master"); 

                    //DateTime NewVersion3 = Convert.ToDateTime("24/03/2021");
                    // date of change 
                }
                else
                {
                    // Not found 
                    MasterTwoCurrencies = false;
                }

                if (MasterTwoCurrencies == true)
                {
                    // Update second currency amount in spare field
                    Lf_BDC.UpdateMasterAfterMatchingWithSecondCurrency(WOperator, WReconcCycleNo);
                }
                //
                // SETTLEMENT DEPARTMENT 
                // Declare fields
                //
                string WFileId = "";
                string WCategories = "";
                string WIdentity = "";

                RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM Cgl = new RRDM_CAP_DATE_GL_PER_CATEGORY_AND_ATM();
                string WMatchingCateg = PRX + "210";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true)
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    // Create entries
                    WFileId = "Egypt_123_NET";
                    // For Categories BDC210 and BDC211
                    WCategories = "('" + PRX + "210','" + PRX + "211'" + ")"; // Issuer
                    WIdentity = "123 TXNS_Bank_Is_Issuer";

                    WWMode = 1;
                    //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);
                }

                WMatchingCateg = PRX + "215";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true)
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    WFileId = "Egypt_123_NET";
                    // For Category BDC215
                    WCategories = "('" + PRX + "215' )"; // Acquirer
                    WIdentity = "123 TXNS_Bank_Is_Acquirer";
                    WWMode = 2;
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

                }

                // ΜΕΕΖΑ
                // ΜΕΕΖΑ
                //
                WMatchingCateg = PRX + "270";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true)
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    // Create entries
                    WFileId = "MEEZA_OTHER_ATMS";
                    // For Categories BDC270 and BDC271
                    WCategories = "('" + PRX + "270','" + PRX + "271'" + ")"; // Issuer
                    WIdentity = "MEEZA TXNS_Bank_Is_Issuer";
                    WWMode = 4;
                    //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

                }
                //
                // NEW ΜΕΕΖΑ GLOBAL LCL
                // ΜΕΕΖΑ - ISSUER 
                //
                WMatchingCateg = PRX + "277";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true)
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    // Create entries
                    WFileId = "MEEZA_GLOBAL_LCL";
                    // For Categories BDC277 and BDC278
                    WCategories = "('" + PRX + "277','" + PRX + "278'" + ")"; // Issuer
                    WIdentity = "MEEZA TXNS_Bank_Is_Issuer";
                    WWMode = 4;
                    //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

                }

                // NEW ΜΕΕΖΑ GLOBAL LCL
                // ΜΕΕΖΑ - ISSUER - //TELDA
                //
                WMatchingCateg = PRX + "279";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true)
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    // Create entries
                    WFileId = "MEEZA_GLOBAL_LCL";
                    // For Categories BDC277 and BDC278
                    WCategories = "('" + PRX + "279' )";  // Issuer
                    WIdentity = "MEEZA TXNS_Bank_Is_Issuer_TELDA";
                    WWMode = 5;
                    //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

                }

                WMatchingCateg = PRX + "272";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true)
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    // Create entries
                    WFileId = "MEEZA_POS";
                    // For Categories BDC272 and BDC273
                    WCategories = "('" + PRX + "272','" + PRX + "273'" + ")"; // Issuer POS
                    WIdentity = "MEEZA POS TXNS_Bank_Is_Issuer";
                    WWMode = 4; // New one based on extented BIN
                                //WWMode = 1; OLD ONE based on BIN
                                //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

                }

                WMatchingCateg = PRX + "275";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true)
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    // Create entries
                    WFileId = "MEEZA_OWN_ATMS";
                    // For Category BDC215
                    WCategories = "('" + PRX + "275' )"; // Acquirer
                    WIdentity = "MEEZA TXNS_Bank_Is_Acquirer";
                    WWMode = 2;
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

                }
                //
                // New MEEZA GLOBAL LCL 
                //
                WMatchingCateg = PRX + "279";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true & Cgl.W_Identity == "MEEZA TXNS_Bank_Is_Acquirer")
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    // Create entries
                    WFileId = "MEEZA_GLOBAL_LCL";
                    // For Category BDC279
                    WCategories = "('" + PRX + "279' )"; // Acquirer
                    WIdentity = "MEEZA TXNS_Bank_Is_Acquirer";
                    WWMode = 2;
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

                }

                // MASTER CARD - Bank Is Issuer
                // MASTER CARD
                //
                WMatchingCateg = PRX + "230";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true)
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    // Create entries
                    WFileId = "MASTER_CARD";
                    // For Categories BDC230 and BDC232
                    WCategories = "('" + PRX + "230','" + PRX + "232'" + ")"; // Issuer
                    WIdentity = "MASTER TXNS_Bank_Is_Issuer";
                    WWMode = 1;
                    //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

                }

                // MASTER CARD BANK is Acquirer
                // MASTER CARD
                //
                WMatchingCateg = PRX + "235";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true)
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    // Create entries
                    WFileId = "MASTER_CARD";
                    // For Categories BDC235 and BDC236? 
                    WCategories = "('" + PRX + "235','" + PRX + "236'" + ")"; // Acquirer
                                                                              // WCategories = "('" + PRX + "235' )"; // Acquirer
                    WIdentity = "MASTER TXNS_Bank_Is_Acquirer";
                    WWMode = 2;
                    //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);
                }

                WMatchingCateg = PRX + "231";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true)
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    // Create entries
                    WFileId = "MASTER_POS";
                    // For Categories BDC272 and BDC273
                    WCategories = "('" + PRX + "231','" + PRX + "233'" + ")"; // Issuer POS
                    WIdentity = "MASTER POS TXNS_Bank_Is_Issuer";
                    WWMode = 4; // New one based on extented BIN
                                //WWMode = 1; OLD ONE based on BIN
                                //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

                }

                // Credit Card
                // 
                //
                WMatchingCateg = PRX + "240";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true)
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    // Create entries
                    WFileId = "Credit_Card";
                    // For Categories BDC240
                    WCategories = "('" + PRX + "240' )"; // 
                    WIdentity = "Credit Card Txns";
                    WWMode = 3;
                    //Mgt.CreateRecords_GL_ENTRIES_For_Category_SecondVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WWMode);
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);
                }

                //
                // VISA 
                //
                WMatchingCateg = PRX + "225";
                Cgl.Read_SettlementDate_ByCycleNo(WReconcCycleNo, WMatchingCateg);

                if (Cgl.RecordFound == true)
                {
                    // Already Created
                    // You should not create it
                }
                else
                {
                    WFileId = "VISA_CARD";
                    // For Category BDC215
                    WCategories = "('" + PRX + "225' )"; // Acquirer
                    WIdentity = "VISA TXNS_Bank_Is_Acquirer";
                    WWMode = 2;
                    Mgt.CreateRecords_GL_ENTRIES_For_Category_ThirdVersion(WOperator, WFileId, WCategories, WReconcCycleNo, WIdentity, WWMode);

                }



                //UPDATE Mpa Replenishment Cycle After Matching for the 01 and 011
                // *********************************

                Lf_BDC.UPDATE_Mpa_After_Matching_With_ReplCycle(WOperator, WReconcCycleNo);

                // Exclude presenter if so 
                //
                // Presenter
                bool Is_Presenter_InReconciliation = false;
                ParId = "946";
                OccurId = "1";

                Gp.ReadParametersSpecificId(WOperator, ParId, OccurId, "", "");

                if (Gp.OccuranceNm == "YES")
                {
                    Is_Presenter_InReconciliation = true;
                }
                else
                {
                    // Exclude them 

                    Mpa.ReadPoolAndFindTotals_Presenter_Unmatched(WOperator, WReconcCycleNo);
                }

                //text = "Matching has Finished" + Environment.NewLine
                //                 + "Process of Moving records to Matched starts.";
                //caption = "MATCHING";
                //timeout = 5000;
                //AutoClosingMessageBox.Show(text, caption, timeout);

                //MessageBox.Show("Matching has Finished" + Environment.NewLine
                //+ "Process of Moving records to Matched starts."
                //);


                //******************************
                // 
                // MOVE MATCHED TXNS POOL 
                // stp_00_MOVE_TXNS_TO_MATCHED_DB_01_POOL
                //******************************
                //textBoxMsgBoard.Text = "Current Status : Moving Records Process";

                Mode = 5; // Updating Action 
                ProcessName = "Moving Records To MATCHED Data Base";
                Message = "Moving Records to MATCHED Process Starts. Cycle:.." + WReconcCycleNo.ToString();
                SavedStartDt = DateTime.Now;

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);


                TotalProgressText = "MOVE TO MATCHED Cycle: ..."
                                         + WReconcCycleNo.ToString() + Environment.NewLine;
                RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
                RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
                string WSelectionCriteria = " WHERE Operator = @Operator ";
                Mf.ReadReconcSourceFilesToFillDataTable_FULL(WOperator, WSelectionCriteria);

                int I = 0;
                int K = 0;
                string WFileName;

                while (I <= (Mf.SourceFilesDataTable.Rows.Count - 1))
                {
                    //    RecordFound = true;
                    int SeqNo = (int)Mf.SourceFilesDataTable.Rows[I]["SeqNo"];
                    Mf.ReadReconcSourceFilesBySeqNo(SeqNo);

                    if (Mf.IsMoveToMatched == true || Mf.SourceFileId == "Atms_Journals_Txns") // the indication that this table is a moving table 
                    {
                        if (Mf.SourceFileId == "Atms_Journals_Txns")
                        {
                            WFileName = "tblMatchingTxnsMasterPoolATMs";
                        }
                        else
                        {
                            WFileName = Mf.SourceFileId;
                        }

                        // Check That File Exist in target data base 
                        string TargetDB = "[RRDM_Reconciliation_MATCHED_TXNS]";
                        Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                        if (Cv.RecordFound == true)
                        {
                            // File Exist
                        }
                        else
                        {
                            // File do not exist
                            //MessageBox.Show("File.." + WFileName + Environment.NewLine
                            //    + "DOES NOT EXIST In MATCHED_TXNS Data Base."
                            //    + "REPORT TO THE HELP DESK."
                            //    );
                            I = I + 1;
                            continue;
                        }

                        Cv.MOVE_ITMX_TXNS_TO_MATCHED(WFileName, WReconcCycleNo);

                        if (Cv.ret == 0)
                        {
                            // GOOD
                            K = K + 1;
                            TotalProgressText = TotalProgressText + Cv.ProgressText;
                        }
                        else
                        {
                            // NO GOOD
                            // public string ProgressText;
                            //public string ErrorReference;
                            //public int ret;
                            //MessageBox.Show("VITAL SYSTEM ERROR" + Environment.NewLine
                            //               + "PROGRESS TEXT.." + Cv.ProgressText + Environment.NewLine
                            //               + "ERROR REFERENCE.." + Cv.ErrorReference + Environment.NewLine
                            //               + ""
                            //               );
                            I = I + 1;
                            continue;

                        }

                    }

                    I = I + 1;
                }

                TotalProgressText = TotalProgressText + DateTime.Now + " Moving of TXNS to matched has finished" + Environment.NewLine;
                TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;

                // *****************************

                Mode = 5; // Updating Action 
                ProcessName = "Moving Records To MATCHED Data Base";
                Message = "Moving Records to Matched Process Finishes.";

                Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
                //*********************************

                //MessageBox.Show(TotalProgressText);

                //text = "Moving records to Matched has finished";
                //caption = "MOVING RECORDS";
                //timeout = 5000;
                //AutoClosingMessageBox.Show(text, caption, timeout);

                //MessageBox.Show("Moving records to Matched has finished" + Environment.NewLine
                //                + ""
                //                );

                // FIND LIMIT DATE FOR HISTORY 

                // MOVE FROM MATCHED TO MATCHED_HST
                //WSelection = " WHERE JobCycle =" + InReconcCycleNo;
                //Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);
                bool MoveToHistory = false;
                ParamId = "853";
                OccuranceId = "5"; // HST
                DateTime DatefromDeletion = NullPastDate;

                Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);

                if (Gp.RecordFound == true)
                {
                    MoveToHistory = true;
                }

                //AgingDays_HST = (int)Gp.Amount; // 

                ////AgingDays_HST = 0; 

                //// Current CutOffdate
                //string WSelection = " WHERE JobCycle =" + WReconcCycleNo;
                //Rjc.ReadReconcJobCyclesBySelectionCriteria(WSelection);

                //DateTime WAgingDateLimit_HST = Rjc.Cut_Off_Date.AddDays(-AgingDays_HST);

                //Rjc.ReadReconcJobCyclesByCutOffDateEqualOrLess(WAgingDateLimit_HST);

                //if (Rjc.RecordFound == true)
                //{
                //    string ReversedCut_Off_Date = Rjc.Cut_Off_Date.ToString("yyyy-MM-dd");

                //    ParamId = "853";
                //    OccuranceId = "6"; // HST

                //    Gp.ReadParameterByOccuranceId(ParamId, OccuranceId);
                //    if (Gp.RecordFound == true)
                //    {
                //        int Int_DeleteFrom_HST = (int)Gp.Amount; // 
                //        DatefromDeletion = WCut_Off_Date.AddDays(-Int_DeleteFrom_HST); 
                //    }

                //    MessageBox.Show("Moving records to History Starts" + Environment.NewLine
                //           + "For date equal or less than.." + ReversedCut_Off_Date + Environment.NewLine
                //            + "Also Deletion of Records from HST will be done." + DatefromDeletion.ToShortDateString()
                //           );
                //    MoveToHistory = true;
                //}
                //}
                //MoveToHistory = true;
                if (MoveToHistory == true)
                {
                    //******************************
                    // 
                    // MOVE TO HST
                    // stp_00_MOVE_TXNS_TO_HISTORY_DB_01_POOL
                    //******************************
                    Mode = 5; // Updating Action 
                    ProcessName = "Moving To HST And Delete From HST";
                    Message = "Moving To HST And Delete From HST Process Starts. Cycle:.." + WReconcCycleNo.ToString();

                    //text = "Moving records to History Starts";
                    //caption = "MOVING RECORDS";
                    //timeout = 5000;
                    //AutoClosingMessageBox.Show(text, caption, timeout);

                    //MessageBox.Show("Moving records to History Starts" + Environment.NewLine
                    //            + ""
                    //            );

                    SavedStartDt = DateTime.Now;

                    Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, SavedStartDt, Message, WSignedId, WReconcCycleNo);


                    TotalProgressText = "MOVE TO HISTORY Cycle: ..."
                                             + WReconcCycleNo.ToString() + Environment.NewLine;
                    //RRDM_Copy_Txns_From_IST_ToMatched_And_Vice Cv = new RRDM_Copy_Txns_From_IST_ToMatched_And_Vice();
                    //RRDMMatchingSourceFiles Mf = new RRDMMatchingSourceFiles();
                    WSelectionCriteria = " WHERE Operator = @Operator ";
                    Mf.ReadReconcSourceFilesToFillDataTable_FULL(WOperator, WSelectionCriteria);

                    I = 0;
                    K = 0;

                    while (I <= (Mf.SourceFilesDataTable.Rows.Count - 1))
                    {
                        //    RecordFound = true;
                        int SeqNo = (int)Mf.SourceFilesDataTable.Rows[I]["SeqNo"];
                        Mf.ReadReconcSourceFilesBySeqNo(SeqNo);

                        if (Mf.IsMoveToMatched == true || Mf.SourceFileId == "Atms_Journals_Txns") // the indication that this table is a moving table 
                        {
                            if (Mf.SourceFileId == "Atms_Journals_Txns")
                            {
                                WFileName = "tblMatchingTxnsMasterPoolATMs";
                            }
                            else
                            {
                                WFileName = Mf.SourceFileId;
                            }

                            // Check That File Exist in target data base 
                            string TargetDB = "[RRDM_Reconciliation_ITMX_HST]";
                            Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                            if (Cv.RecordFound == true)
                            {
                                // File Exist
                            }
                            else
                            {
                                // File do not exist
                                //MessageBox.Show("File.." + WFileName + Environment.NewLine
                                //    + "DOES NOT EXIST In ITMX_HST Data Base."
                                //    + "REPORT TO THE HELP DESK."
                                //    );
                                I = I + 1;
                                continue;
                            }

                            // Check That File Exist in target data base 
                            TargetDB = "[RRDM_Reconciliation_MATCHED_TXNS_HST]";
                            Cv.ReadTableToSeeIfExist(TargetDB, WFileName);

                            if (Cv.RecordFound == true)
                            {
                                // File Exist
                            }
                            else
                            {
                                // File do not exist
                                //MessageBox.Show("File.." + WFileName + Environment.NewLine
                                //    + "DOES NOT EXIST In MATCHED_TXNS_HST Data Base."
                                //    + "REPORT TO THE HELP DESK."
                                //    );
                                I = I + 1;
                                continue;
                            }

                            // MessageBox.Show("Start Moving file.." + WFileName); 

                            Cv.MOVE_ITMX_TXNS_TO_HST(WFileName, WReconcCycleNo);

                            if (Cv.ret == 0)
                            {
                                // GOOD
                                K = K + 1;
                                TotalProgressText = TotalProgressText + Cv.ProgressText;
                            }
                            else
                            {
                                // NO GOOD
                                // public string ProgressText;
                                //public string ErrorReference;
                                //public int ret;
                                //MessageBox.Show("VITAL SYSTEM ERROR DURING MOVING TO HISTORY" + Environment.NewLine
                                //               + "PROGRESS TEXT.." + Cv.ProgressText + Environment.NewLine
                                //               + "ERROR REFERENCE.." + Cv.ErrorReference + Environment.NewLine
                                //               + ""
                                //               );

                                //  return;

                                I = I + 1;
                                continue;

                            }

                        }

                        I = I + 1;
                    }

                    TotalProgressText = TotalProgressText + DateTime.Now + " Moving of TXNS to HST has finished" + Environment.NewLine;
                    TotalProgressText = TotalProgressText + DateTime.Now + " Number of moved tables..." + K.ToString() + Environment.NewLine;

                    // *****************************
                    //
                    // DELETE RECORDS FROM HISTORY DATA BASES BASED ON PARAMETER 853
                    //
                    //text = "Moving Records to History Finishes" + Environment.NewLine
                    //            + "Delete Records From History Starts if any proper parameneter present";
                    //caption = "MOVING RECORDS";
                    //timeout = 5000;
                    //AutoClosingMessageBox.Show(text, caption, timeout);

                    //MessageBox.Show("Moving Records to History Finishes" + Environment.NewLine
                    //            + "Delete Records From History Starts if any proper parameneter present"
                    //            );
                    int WWWMode = 0;
                    Cv.DELETE_DELETE_TXNS_FROM_HST_MAIN(WOperator, WSignedId, WReconcCycleNo, WWWMode);

                    Mode = 5; // Updating Action 
                    ProcessName = "Moving To HST And Delete From HST";
                    Message = "Moving To HST And Delete From HST.";

                    //text = "Delete From HST has finished";
                    //caption = "MOVING RECORDS";
                    //timeout = 5000;
                    //AutoClosingMessageBox.Show(text, caption, timeout);
                    //MessageBox.Show("Delete From HST has finished" + Environment.NewLine
                    //             + ""
                    //             );

                    //textBoxMsgBoard.Text = "Current Status : Ready";

                    Pt.InsertPerformanceTrace_With_USER(WOperator, WOperator, Mode, ProcessName, "", SavedStartDt, DateTime.Now, Message, WSignedId, WReconcCycleNo);
                    //*********************************

                    //  MessageBox.Show(TotalProgressText);

                }

                // MATCHING AND MOVING RECORDS HAS FINISHED 


                // AT the END UPDATE STATS

                // 
                // 
                string connectionStringITMX = _staticConfiguration.GetConnectionString("ReconConnectionString");

                string RCT = "[RRDM_Reconciliation_ITMX].[dbo].[Stp_00_UPDATE_DB_System_Stats]";

                using (SqlConnection conn =
                   new SqlConnection(connectionStringITMX))
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
                        //MessageBox.Show(string.Format("An error occurred: {0}", ex.Message));
                    }

                // MATCHING AND MOVING RECORDS HAS FINISHED 
                // ALLOW users to sign In. 
                //
                //IsAllowedToSignIn = true;
                //CheckForSignInUsers(IsAllowedToSignIn);

                //if (ThereAreUsersInSystem == true)
                //{
                //    // Decide whether to move forward or not. 
                //}
                //text = "Matching and movings of records process has finished";
                //caption = "MATCHING";
                //timeout = 5000;
                //AutoClosingMessageBox.Show(text, caption, timeout);
                //MessageBox.Show("Matching and movings of records process has finished");

                //Form502_Load(this, new EventArgs());

            
            //


        }

        // Service status
        string serviceStatus;
        private bool ServiceAvailableStatus(string InServiceId)
        {

            // Check Status Of Last Service
            bool ServiceAvailable = false;

            RRDMAgentQueue Aq = new RRDMAgentQueue();

            string WServiceId = InServiceId; // 

            // Find Status
            Aq.ReqDateTime = DateTime.Now;
            Aq.RequestorID = WSignedId;
            Aq.RequestorMachine = Environment.MachineName;
            Aq.Command = "SERVICE_STATUS";
            Aq.ServiceId = WServiceId;
            // 
            string ParamId = "915";

            Gp.ReadParameterByOccuranceId(ParamId, Aq.ServiceId);
            if (Gp.RecordFound == true)
            {
                Aq.ServiceName = Gp.OccuranceNm;
            }
            else
            {
                Aq.ServiceName = "Not Specified";
            }

            Aq.Priority = 0; // Highest
            Aq.Operator = WOperator;

            Aq.OriginalReqID = 0;
            Aq.OriginalRequestorID = WSignedId;

            WServiceReqID = Aq.InsertNewRecordInAgentQueue();
            string SelectionCriteria = " WHERE ReqID = " + WServiceReqID;
            int retries = 10;
            bool agentError = false;
            serviceStatus = "No response. Check RRDM Agent too.";
            do
            {
                Thread.Sleep(1000); // milliseconds=1 sec
                Aq.ReadAgentQueueBySelectionCriteria(SelectionCriteria);
                if (Aq.RecordFound == true)
                {
                    if (Aq.ReqStatusCode == AgentStatus.Req_Finished)
                    {
                        serviceStatus = Aq.CmdStatusMessage;
                        if (Aq.CmdStatusCode == AgentProcessingResult.Cmd_Success)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    agentError = true;
                    break;
                }
                retries--;
            }
            while (retries >= 0);

            if (agentError == true)
            {
                /*
                MessageBox.Show("Service with name.." + Aq.ServiceName
                                                   + Environment.NewLine
                                                   + "Is OutStanding " + Environment.NewLine
                                                   + "With ReqStatusCode" + Aq.ReqStatusCode + Environment.NewLine
                                                   + "AND " + Environment.NewLine
                                                   + "With CmdStatusCode" + Aq.CmdStatusCode + Environment.NewLine
                                                   );
                                                   */
                //  return;

            }

            if (serviceStatus == "Stopped")
            {
                //OK
                //  
                ServiceAvailable = true;
            }
            else
            {
                ServiceAvailable = false;

            }
            return ServiceAvailable;
        }


    }

}
