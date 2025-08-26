using System;
using System.Threading; //

using System.Diagnostics;
using System.Runtime.InteropServices;

using RRDM4ATMs;

namespace JTMReqSimulator
{
    class Program
    {
        static int MaxVMs = -1;

        static bool CloseOrBreakKey = false;

        const string PROGRAM_MUTEX_NAME = "JTMREQUESTSIMULATOR";
        const string bankID = "ETHNCY2N";

        // Program Files\NCR APTRA\Advance NDC\data\EJDATA.LOG

        static void Main(string[] args)
        {
            string msg;
            Random rnd = new Random();
            Random rndA = new Random();
            Random rndW = new Random();


            bool IsTheOnlyInstance;
            Mutex InstanceMutex = new Mutex(true, PROGRAM_MUTEX_NAME, out IsTheOnlyInstance);

            if (IsTheOnlyInstance == false)
            {
                Console.WriteLine("  Only one instance of this program is allowed!!  Press ENTER to terminate...");
                Console.ReadLine();
                return;
            }

            // Prepare for handling CTRL+C and CTRL+BREAK
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

            if (args.Length != 1)
            {
                Console.WriteLine("Missing argument! The argument must be in the form -V# \n where # is a number in the range of 1 to 9\nSetting to default, 6!");
                MaxVMs = 6;
            }
            else
            {
                string sVMs = args[0].Substring(2);
                // input.Substring(0, 3);
                int nVMs = 0;
                bool rc = int.TryParse(sVMs, out nVMs);
                if (rc == true)
                {
                    if (nVMs > 0 && nVMs < 10)
                        MaxVMs = nVMs;
                }

                if (MaxVMs == -1)
                {
                    Console.WriteLine("Argument passed is : {0}", args[0]);
                    Console.WriteLine("Invalid argument! The argument must be in the form -V# \nwhere # is a number in the range of 1 to 9\nSetting to the default value 6!");
                    Console.WriteLine("Press ENTER to continue...");
                    Console.ReadLine();
                    MaxVMs = 6;
                }
            }


            while (true)
            {
                Console.Clear();
                Console.WriteLine("\nSimulator is configured for {0} VMs", MaxVMs);
                msg = "\nOptions: \n" +
                      "[1]: FETCH Single ATM\n" +
                      /* "[2]: FETCHDEL Single ATM\n" +*/
                      "[3]: ATMSTATUS Single ATM\n" +
                      "[4]: Batch FETCH\n" +
                      /* "[5]: Batch FETCHDEL\n" + */
                      "[6]: Batch ATMSTATUS\n" +
                      "\n" +
                      "[S]: Stress Test\n" +
                      // "[K]: Kill Threads\n" +
                      "[I]: Initialize_Tables\n\n" +
                      "\n" +
                      // "[L]: Restore file on a VM  (Alecos only...)\n" +
                      // "[M]: Restore file on ALL VMs  (Alecos only...)\n" +
                      "[V]: Shutdown Single VM\n" +
                      "[W]: Shutdown ALL VMs\n" +
                      "\n" +
                      "[X]: Exit";

                Console.WriteLine(msg);
                ConsoleKeyInfo option = Console.ReadKey(true);
                switch (option.KeyChar)
                {

                    case '1': //Single FETCH
                        {
                            int indx = SelectATM();
                            if (indx != -1)
                            {
                                InsertFETCHRequest(indx, "FETCH", "");
                            }
                            break;
                        }
                    //case '2': //Single FETCHDEL
                    //    {
                    //        int indx = SelectATM();
                    //        if (indx != -1)
                    //        {
                    //            InsertFETCHRequest(indx, "FETCHDEL", "");
                    //        }
                    //        break;
                    //    }
                    case '3': //Single ATMSTATUS
                        {
                            int indx = SelectATM();
                            if (indx != -1)
                            {
                                InsertSTATUSRequest(indx, "");
                            }
                            break;
                        }
                    case '4': //Batch FETCH
                        {
                            int bindx = rnd.Next(1, 1000000); // number between 1 and 999.999 to use in BatchId
                            string batchid = string.Format("BATCH-{0}", bindx.ToString("000000"));
                            Console.WriteLine("Your Option: Batch will generate 10 requests as {0} ", batchid);

                            for (int i = 1; i <= MaxVMs; i++)
                            {
                                InsertFETCHRequest(i, "FETCH", batchid);
                            }
                            Thread.Sleep(1000);
                            break;
                        }
                    //case '5': //Batch FETCHDEL
                    //    {
                    //        int bindx = rnd.Next(1, 1000000); // number between 1 and 999.999 to use in BatchId
                    //        string batchid = string.Format("BATCH-{0}", bindx.ToString("000000"));
                    //        Console.WriteLine("Your Option: Batch FETCHDEL will generate 10 requests as {0}.. ", batchid);

                    //        for (int i = 1; i <= MaxVMs; i++)
                    //        {
                    //            InsertFETCHRequest(i, "FETCHDEL", batchid);
                    //        }
                    //        Thread.Sleep(1000);
                    //        break;
                    //    }
                    case '6': //Batch ATMSTATUS
                        {
                            int bindx = rnd.Next(1, 1000000); // number between 1 and 999.999 to use in BatchId
                            string batchid = string.Format("BATCH-{0}", bindx.ToString("000000"));
                            Console.WriteLine("Your Option: Batch ATMSTATUS will generate 10 requests as {0}.. ", batchid);

                            for (int i = 1; i <= MaxVMs; i++)
                            {
                                InsertSTATUSRequest(i, batchid);
                            }
                            Thread.Sleep(1000);
                            break;
                        }
                    //case 'k': // RESET (kill running threads)
                    //case 'K':
                    //    {
                    //        Console.WriteLine(" Your Option: RESET ");
                    //        InsertRESETRequest();
                    //        break;
                    //    }
                    case 'i': // Initilalize
                    case 'I':
                        {
                            Console.WriteLine(" Your Option: Initialize the JTMQueue Table");
                            Console.WriteLine(" \n    !!!  This option deletes all records in the JTMQueue table  !!!");
                            Console.WriteLine(" \n         Press Y to proceed...  Any other key to cancel..");
                            ConsoleKeyInfo confirm = Console.ReadKey(true);
                            if (confirm.KeyChar == 'Y' || confirm.KeyChar == 'y')
                            {
                                int rowsDeleted = 0;
                                rowsDeleted = ClearJTMQueue();
                                Console.WriteLine("\n ---------- Deleted {0} rows!", rowsDeleted);
                            }

                            Console.WriteLine(" \nThis options will also INSERT new records in JTMIdentificationDetaisl table..");
                            Console.WriteLine(" \n    !!!  The table must have been emptied before running this...  !!!");
                            Console.WriteLine(" \n         Press Y to proceed...  Any other key to cancel..");
                            confirm = Console.ReadKey(true);
                            if (confirm.KeyChar == 'Y' || confirm.KeyChar == 'y')
                            {
                                for (int i = 0; i < MaxVMs; i++)
                                {
                                    InsertIdentificationDetails(i + 1, "");
                                    // Console.WriteLine(" Deleted {0} rows!", rowsDeleted);
                                }
                            }

                            Thread.Sleep(1000);
                            break;
                        }
                    //case 'l': // Restore Single VM
                    //case 'L':
                    //    {
                    //        int indx = SelectATM();
                    //        if (indx != -1)
                    //        {
                    //            Console.WriteLine("Your Option: Restore ATMXP-{0}", (indx).ToString("00"));
                    //            RestoreATM(indx);
                    //        }
                    //        Thread.Sleep(1000);
                    //        break;
                    //    }
                    case 's': // StressTest )
                    case 'S':
                        {
                            Console.WriteLine(" Your Option: Stress Test");
                            Console.WriteLine(" \n    !!!  This option will generate a random new FETCH request every x (10 < x < 1000) milliseconds !!!");
                            Console.WriteLine(" \n         Press Y to proceed...  Any other key to cancel..");
                            ConsoleKeyInfo confirm = Console.ReadKey(true);
                            if (confirm.KeyChar == 'Y' || confirm.KeyChar == 'y')
                            {
                                int rA;
                                int rW;

                                SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

                                Console.WriteLine("\n         Press CTRL+BREAK / CTRL+PAUSE to stop..\n");
                                CloseOrBreakKey = false;
                                while (!CloseOrBreakKey)
                                {
                                    rA = rndA.Next(1, MaxVMs + 1);
                                    rW = rndW.Next(1, 1001);

                                    InsertFETCHRequest(rA, "FETCH", "");

                                    Console.WriteLine("Generated FETCH request for AB1{0}", rA.ToString("00"));
                                    Thread.Sleep(rW);
                                }
                                Thread.Sleep(1000);
                            }
                            break;
                        }
                    //case 'm': // Restore ALL VMs
                    //case 'M':
                    //    {
                    //        Console.WriteLine("Your Option: Restore ALL ...");
                    //        for (int i = 1; i <= MaxVMs; i++)
                    //        {
                    //            RestoreATM(i);
                    //        }
                    //        Thread.Sleep(1000);
                    //        break;
                    //    }
                    case 'v': // Shutdown Single VM
                    case 'V':
                        {
                            int indx = SelectATM();
                            if (indx != -1)
                            {
                                Console.WriteLine("Your Option: Shutdown ATMXP-{0}", (indx).ToString("00"));
                                ShutDownATM(indx);
                            }
                            Thread.Sleep(1000);
                            break;
                        }
                    case 'w': // Shutdown ALL VMs
                    case 'W':
                        {
                            Console.WriteLine("Your Option: ShutDown ALL VMs ...");
                            for (int i = 1; i <= MaxVMs; i++)
                            {
                                ShutDownATM(i);
                            }
                            Thread.Sleep(1000);
                            break;
                        }
                    case 'x':
                    case 'X':
                        {
                            InstanceMutex.ReleaseMutex();
                            return;
                        }
                    default:

                        Console.WriteLine("    \nInvalid ....");
                        break;
                }
                //Console.WriteLine("\n     Terminating... Press ENTER to exit");
                //Console.ReadLine();
            }

        }

        #region InsertFETCHRequest(int Index, string Cmd, string BatchId)
        static void InsertFETCHRequest(int Index, string Cmd, string BatchId)
        {
            // int priority = 
            // Console.WriteLine("Index {0}:", Index);

            RRDMJTMQueue JtmQ = new RRDMJTMQueue();

            JtmQ.MsgDateTime = DateTime.Now;
            JtmQ.RequestorID = string.Format("ALEX-{0}", Index.ToString("00"));
            JtmQ.RequestorMachine = string.Format("TESTPC-{0}", Index.ToString("00"));
            JtmQ.Command = Cmd;
            JtmQ.Priority = 1;
            if (BatchId.Length >= 1)
            {
                JtmQ.Priority = 2;
            }
            //JtmQ.BatchID = BatchId;
            switch (Index)
            {
                case 2:
                    {
                        JtmQ.AtmNo = "00147128";
                        JtmQ.TypeOfJournal = "DBLD01";
                        JtmQ.SourceFileName = "EDCLocal.dat";
                        JtmQ.SourceFilePath = @"c:\Diebold\EDC";
                        break;
                    }
                case 4:
                    {
                        JtmQ.AtmNo = "00005128";
                        JtmQ.TypeOfJournal = "DBLD01";
                        JtmQ.SourceFileName = "EDCLocal.dat";
                        JtmQ.SourceFilePath = @"c:\Diebold\EDC";
                        break;
                    }
                default:
                    {
                        JtmQ.AtmNo = string.Format("AB1{0}", Index.ToString("00"));
                        JtmQ.TypeOfJournal = "NCR01";
                        JtmQ.SourceFileName = "EJData.LOG";
                        JtmQ.SourceFilePath = @"c:\Program Files\Advance NDC\Data";
                        break;
                    }
            }
            JtmQ.BankID = bankID;
            JtmQ.BranchNo = string.Format("BR0{0}", Index.ToString("00"));
            JtmQ.ATMIPAddress = string.Format("192.168.10.2{0}", (Index + 20).ToString("00"));
            JtmQ.ATMMachineName = string.Format("ATMXP-{0}", Index.ToString("00"));
            JtmQ.ATMWindowsAuth = false;
            JtmQ.ATMAccessID = "Capitan";
            JtmQ.ATMAccessPassword = "Alejandr0";

            JtmQ.DestnFileName = "";
            // JtmQ.DestnFilePath = string.Format(@"C:\RRDM\FilePool\ATMs\{0}", JtmQ.AtmNo);
            JtmQ.DestnFilePath = string.Format(@"C:\RRDM\FilePool\ATMs");
            JtmQ.DestnFileHASH = "";
            JtmQ.Stage = 0;
            JtmQ.Operator = bankID;

            JtmQ.InsertNewRecordInJTMQueue();
            if (JtmQ.ErrorFound)
            {
                Console.WriteLine("INSERT [{0}]:{1} returned: {2}", Index, Cmd, JtmQ.ErrorOutput);
            }
        }
        #endregion

        #region InsertSTATUSRequest(int Index, string batchID)
        static void InsertSTATUSRequest(int Index, string batchID)
        {
            // int priority = 
            // Console.WriteLine("Index {0}:", Index);

            RRDMJTMQueue JtmQ = new RRDMJTMQueue();

            JtmQ.MsgDateTime = DateTime.Now;
            JtmQ.RequestorID = string.Format("ALEX-{0}", Index.ToString("00"));
            JtmQ.RequestorMachine = string.Format("TESTPC-{0}", Index.ToString("00"));
            JtmQ.Command = "ATMSTATUS";
            JtmQ.Priority = 0;
            if (batchID.Length >= 1)
            {
                JtmQ.Priority = 2;
            }
            //JtmQ.BatchID = batchID;
            JtmQ.AtmNo = string.Format("AB1{0}", Index.ToString("00"));
            JtmQ.BankID = bankID;
            JtmQ.BranchNo = string.Format("BR0{0}", Index.ToString("00"));
            JtmQ.ATMIPAddress = string.Format("192.168.10.2{0}", (Index + 20).ToString("00"));
            JtmQ.ATMMachineName = string.Format("ATMXP-{0}", Index.ToString("00"));
            JtmQ.ATMWindowsAuth = false;
            JtmQ.ATMAccessID = "Capitan";
            JtmQ.ATMAccessPassword = "Alejandr0";
            JtmQ.TypeOfJournal = "";
            JtmQ.SourceFileName = "";
            JtmQ.SourceFilePath = "";
            JtmQ.DestnFileName = "";
            JtmQ.DestnFilePath = ""; // string.Format(@"C:\RRDM\FilePool\ATMs\{0}", JtmQ.AtmNo);
            JtmQ.DestnFileHASH = "";
            JtmQ.Stage = 0;
            JtmQ.Operator = bankID;

            JtmQ.InsertNewRecordInJTMQueue();
            if (JtmQ.ErrorFound)
            {
                Console.WriteLine("INSERT [{0}] returned: {1}", Index, JtmQ.ErrorOutput);
            }
        }
        #endregion

        #region InsertRESETRequest(int Index, string Command)
        static void InsertRESETRequest()
        {
            //Console.WriteLine("Random: {0}:", Index);

            RRDMJTMQueue JtmQ = new RRDMJTMQueue();

            JtmQ.MsgDateTime = DateTime.Now;
            JtmQ.RequestorID = string.Format("ALEX-{0}", "00");
            JtmQ.RequestorMachine = string.Format("TESTPC-{0}", "00");
            JtmQ.Command = "RESET";
            JtmQ.Priority = 0;
            JtmQ.BankID = bankID;

            // ToDo - should not need to enter these for CLEAR/RESET
            //JtmQ.BatchID = ""; //TODO
            JtmQ.AtmNo = "";
            JtmQ.BranchNo = "";
            JtmQ.ATMIPAddress = "";
            JtmQ.ATMMachineName = "";
            JtmQ.ATMWindowsAuth = false;
            JtmQ.ATMAccessID = "";
            JtmQ.ATMAccessPassword = "";
            JtmQ.TypeOfJournal = "";
            JtmQ.SourceFileName = "";
            JtmQ.SourceFilePath = "";
            //JtmQ.DestnFileName = "";
            JtmQ.DestnFilePath = "";
            //JtmQ.DestnFileHASH = "";
            //JtmQ.Stage = 0;
            //JtmQ.ResultCode = 0;
            //JtmQ.ResultMessage = ResultMessage;
            //JtmQ.FileUploadStart = FileUploadStart;
            //JtmQ.FileUploadEnd = FileUploadEnd;
            //JtmQ.FileParseStart = FileParseStart;
            //JtmQ.FileParseEnd = FileParseEnd;

            JtmQ.Operator = bankID;


            JtmQ.InsertNewRecordInJTMQueue();
            if (JtmQ.ErrorFound)
            {
                Console.WriteLine("\nINSERT RESET returned: {0}\n", JtmQ.ErrorOutput);
            }
        }
        #endregion

        #region  ClearJTMQueue()
        static int ClearJTMQueue()
        {
            int rowsDeleted = 0;
            RRDMJTMQueue JtmQ = new RRDMJTMQueue();

            rowsDeleted = JtmQ.DeleteAllRecordsInJTMQueue();
            if (JtmQ.ErrorFound)
            {
                Console.WriteLine("\nClearJTMQueue returned: {1}", JtmQ.ErrorOutput);
            }

            return (rowsDeleted);
        }
        #endregion

        #region InsertIdentificationDetails(int Index, string BatchId)
        static void InsertIdentificationDetails(int Index, string BatchId)
        {
            // int priority = 
            // Console.WriteLine("Index {0}:", Index);

            RRDMJTMIdentificationDetailsClass IdD = new RRDMJTMIdentificationDetailsClass();

            IdD.AtmNo = string.Format("AB1{0}", Index.ToString("00"));
            IdD.DateLastUpdated = DateTime.MinValue;
            IdD.UserId = string.Format("ALEX-{0}", Index.ToString("00"));
            IdD.LoadingScheduleID = BatchId;
            IdD.ATMIPAddress = string.Format("192.168.10.2{0}", (Index + 20).ToString("00"));
            IdD.ATMMachineName = string.Format("ATMXP-{0}", Index.ToString("00"));
            IdD.ATMWindowsAuth = false;
            IdD.ATMAccessID = "Capitan";
            IdD.ATMAccessPassword = "Alejandr0";

            switch (Index % 2)
            {
                // Even number = Diebold
                // Odd  number = NCR
                case 0:
                    {
                        IdD.TypeOfJournal = "DBLD01";
                        IdD.SourceFileName = "EDCLocal.dat";
                        IdD.SourceFilePath = @"c:\Diebold\EDC";
                        break;
                    }
                default:
                    {
                        IdD.TypeOfJournal = "NCR01";
                        IdD.SourceFileName = "EJData.LOG";
                        IdD.SourceFilePath = @"c:\Program Files\Advance NDC\Data";
                        break;
                    }
            }
            IdD.DestnFilePath = @"C:\RRDM\FilePool\ATMs";
            IdD.FileUploadRequestDt = DateTime.MinValue;
            IdD.FileParseEnd = DateTime.MinValue;
            IdD.LoadingCompleted = DateTime.MinValue;
            IdD.NextLoadingDtTm = DateTime.MaxValue;
            IdD.Operator = bankID;

            IdD.InsertNewRecordInJTMIdentificationDetails();
            if (IdD.ErrorFound)
            {
                Console.WriteLine("InsertNewRecordInJTMIdentificationDetails() INSERT [{0}] returned: {1}", Index, IdD.ErrorOutput);
                Console.ReadLine();
            }
            else
            {
                string msg = string.Format("Inserted new record for {0} in JTMIdentificationDetails table", IdD.ATMMachineName);
                Console.WriteLine(msg);
                // Console.ReadLine();
            }
        }
        #endregion

        #region PSEXEC - Restore ATM
        static void RestoreATM(int Index)
        {
            //psexec \\ATMXP-xx -i -u Capitan -p Alejandr0 c:\JPool\Cerv.bat
            string ATMUser = "Capitan";
            string ATMPassword = "Alejandr0";
            string ATMMachineName = string.Format("ATMXP-{0}", Index.ToString("00"));
            string PSXeqArg = string.Format("\\\\{0} -u {1} -p {2} c:\\JPool\\Cerv.bat", ATMMachineName, ATMUser, ATMPassword);
            Console.WriteLine("Executing PSEXEC with Arguments: {0}", PSXeqArg);
            // Console.ReadLine();
            StartPSExec(PSXeqArg);
        }
        #endregion

        #region PSEXEC - ShutDown ATM
        static void ShutDownATM(int Index)
        {
            // psexec \\ATMXP-01 -d -u Capitan -p Alejandr0  cmd /c "shutdown /s /t 30"
            string ATMUser = "Capitan";
            string ATMPassword = "Alejandr0";
            string ATMMachineName = string.Format("ATMXP-{0}", Index.ToString("00"));
            string PSXeqArg = string.Format("\\\\{0} -u {1} -p {2} cmd /c \"shutdown /s /t 3\"", ATMMachineName, ATMUser, ATMPassword);
            Console.WriteLine("Executing PSEXEC with Arguments: {0}", PSXeqArg);
            // Console.ReadLine();
            StartPSExec(PSXeqArg);
        }
        #endregion

        #region PSEXEC - Start PSExec
        static void StartPSExec(string arguments)
        {
            // "\\10.10.1.255 -u user -p pass -c -f "D:\MyApplications\MyExecutable.exe"";
            Process oProcess = new Process();
            oProcess.EnableRaisingEvents = false;
            oProcess.StartInfo.CreateNoWindow = true;
            oProcess.StartInfo.UseShellExecute = false;
            oProcess.StartInfo.RedirectStandardOutput = true;
            oProcess.StartInfo.FileName = @"c:\Tools\SysInternal\PSexec.exe";
            oProcess.StartInfo.Arguments = arguments;
            oProcess.Start();
        }
        #endregion

        #region  SelectATM()
        public static int SelectATM()
        {
            int indx = -1;
            Console.WriteLine(" Select an ATM machine: ");
            for (int i = 1; i < MaxVMs; i++)
            {
                Console.WriteLine("     {0} - ATMXP-{1}", i, (i).ToString("00"));
            }
            if (MaxVMs == 10)
                Console.WriteLine("     {0} - ATMXP-{1}", "0", "10");
            else
                Console.WriteLine("     {0} - ATMXP-{1}", MaxVMs.ToString("0"), MaxVMs.ToString("00"));

            ConsoleKeyInfo num = Console.ReadKey(true);
            indx = (int)Char.GetNumericValue(num.KeyChar);
            if (indx <= 0 || indx > MaxVMs)
            {
                string msg = string.Format("Select a number between 1 and {0}", MaxVMs);
                Console.WriteLine(msg);
                Thread.Sleep(2000);
            }
            else
            {
                Console.WriteLine("ATM Selected: ATMXP-{0}", (indx).ToString("00"));
                Thread.Sleep(1000);
            }
            return (indx);
        }
        #endregion

        #region ConsoleCtrlCheck

        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            // Put your own handler here
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    {
                        CloseOrBreakKey = true;
                        Console.WriteLine("\n      CTRL+C received!");
                        break;
                    }
                case CtrlTypes.CTRL_BREAK_EVENT:
                    {
                        CloseOrBreakKey = true;
                        Console.WriteLine("\n      CTRL+BREAK received!");
                        break;
                    }
            }
            return true;
        }
        #endregion

    }
}
