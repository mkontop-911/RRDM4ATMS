using System;
using System.Threading; //

using System.Diagnostics;
using System.Runtime.InteropServices;

using RRDM4ATMs;

namespace RRDMAgent_Simulator
{
    class Program
    {
        static bool CloseOrBreakKey = false;
        static string argOperator;

        private struct ServiceStruct
        {
            public int index;
            public string serviceId;
            public string origin;
            public string sourceId;
            public ServiceStruct(int index, string serviceId, string origin, string sourceId)
            {
                this.index = index;
                this.serviceId = serviceId; // as defined in GAS_Parameters (915)
                this.origin = origin;
                this.sourceId = sourceId;
            }
        }

        private static ServiceStruct[] SERVICE_LIST = new ServiceStruct[]
        {
            new ServiceStruct(0, "10", "ATMs", "Atms_Journals_Txns"),
            new ServiceStruct(1, "11", "Banking_System", "Flexcube"),
            new ServiceStruct(2, "12", "Bank's_Switch", "Switch_IST_Txns"),
            new ServiceStruct(3, "13", "National_Switch", "Egypt_123_NET"),
            new ServiceStruct(4, "14", "Visa_International", "VISA_CARD"),
            new ServiceStruct(5, "15", "MasterCard_International", "MASTER_CARD")
        };


        static void Main(string[] args)
        {
            string msg;

            #region Read parameters passed to the program
            if (args.Length != 1)
            {
                msg = "Processing stopped!\nMissing 'Operator' parameter\n\nPress OK to exit...";
                Console.WriteLine(msg);
                Console.ReadLine();
                return;
            }
            else
            {
                argOperator = args[0];
            }
            #endregion

            #region Set Title
            if (Environment.UserInteractive)
            {
                // Set the console window title
                string m = string.Format("RRDM Agent Simulator - Operator:{0}", argOperator);
                Console.Title = m;
            }
            #endregion

            // Prepare for handling CTRL+C and CTRL+BREAK
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

            while (!CloseOrBreakKey)
            {

                Console.Clear();
                string menuDescr = "\nInsert a record in AgentQueue to: \n";
                Console.WriteLine(menuDescr);
                int numberOfServices = SERVICE_LIST.Length;
                for (int i = 0; i < numberOfServices; i++)
                {
                    Console.WriteLine("[0{0}] Start Service 'RRDM RFM {1} ({2})", SERVICE_LIST[i].index, SERVICE_LIST[i].origin, SERVICE_LIST[i].sourceId);
                }
                menuDescr = "or to:";
                Console.WriteLine(menuDescr);
                for (int i = 0; i < numberOfServices; i++)
                {
                    Console.WriteLine("[{0}] Stop Service 'RRDM RFM {1} ({2})", SERVICE_LIST[i].index + 10, SERVICE_LIST[i].origin, SERVICE_LIST[i].sourceId);
                }
                menuDescr = "or to:";
                Console.WriteLine(menuDescr);
                for (int i = 0; i < numberOfServices; i++)
                {
                    Console.WriteLine("[{0}] Get the Status of Service 'RRDM RFM {1} ({2})", SERVICE_LIST[i].index + 20, SERVICE_LIST[i].origin, SERVICE_LIST[i].sourceId);
                }
                menuDescr = "or to:";
                Console.WriteLine(menuDescr);
                for (int i = 0; i < numberOfServices; i++)
                {
                    Console.WriteLine("[{0}] Start and Monitor Service 'RRDM RFM {1} ({2})", SERVICE_LIST[i].index + 30, SERVICE_LIST[i].origin, SERVICE_LIST[i].sourceId);
                }
                menuDescr = "\nor \n";
                Console.WriteLine(menuDescr);
                menuDescr = "[I]: Initialize the Agent Queue";
                Console.WriteLine(menuDescr);
                menuDescr = "[X]: Exit";
                Console.WriteLine(menuDescr);

                string option = Console.ReadLine();
                int index = -1;
                string req = "";
                switch (option)
                {
                    case "0":
                    case "00":
                        {
                            req = "SERVICE_START";
                            index = 0;
                        }
                        break;

                    case "1":
                    case "01":
                        {
                            req = "SERVICE_START";
                            index = 1;
                        }
                        break;
                    case "2":
                    case "02":
                        {
                            req = "SERVICE_START";
                            index = 2;
                        }
                        break;
                    case "3":
                    case "03":
                        {
                            req = "SERVICE_START";
                            index = 3;
                        }
                        break;
                    case "4":
                    case "04":
                        {
                            req = "SERVICE_START";
                            index = 4;
                        }
                        break;
                    case "5":
                    case "05":
                        {
                            req = "SERVICE_START";
                            index = 5;
                        }
                        break;

                    case "10":
                        {
                            req = "SERVICE_STOP";
                            index = 0;
                        }
                        break;

                    case "11":
                        {
                            req = "SERVICE_STOP";
                            index = 1;
                        }
                        break;
                    case "12":
                        {
                            req = "SERVICE_STOP";
                            index = 2;
                        }
                        break;
                    case "13":
                        {
                            req = "SERVICE_STOP";
                            index = 3;
                        }
                        break;
                    case "14":
                        {
                            req = "SERVICE_STOP";
                            index = 4;
                        }
                        break;
                    case "15":
                        {
                            req = "SERVICE_STOP";
                            index = 5;
                        }
                        break;

                    case "20":
                        {
                            req = "SERVICE_STATUS";
                            index = 0;
                        }
                        break;

                    case "21":
                        {
                            req = "SERVICE_STATUS";
                            index = 1;
                        }
                        break;
                    case "22":
                        {
                            req = "SERVICE_STATUS";
                            index = 2;
                        }
                        break;
                    case "23":
                        {
                            req = "SERVICE_STATUS";
                            index = 3;
                        }
                        break;
                    case "24":
                        {
                            req = "SERVICE_STATUS";
                            index = 4;
                        }
                        break;
                    case "25":
                        {
                            req = "SERVICE_STATUS";
                            index = 5;
                        }
                        break;

                    case "30":
                        {
                            req = "SERVICE_START_AND_MONITOR";
                            index = 0;
                        }
                        break;

                    case "31":
                        {
                            req = "SERVICE_START_AND_MONITOR";
                            index = 1;
                        }
                        break;
                    case "32":
                        {
                            req = "SERVICE_START_AND_MONITOR";
                            index = 2;
                        }
                        break;
                    case "33":
                        {
                            req = "SERVICE_START_AND_MONITOR";
                            index = 3;
                        }
                        break;
                    case "34":
                        {
                            req = "SERVICE_START_AND_MONITOR";
                            index = 4;
                        }
                        break;
                    case "35":
                        {
                            req = "SERVICE_START_AND_MONITOR";
                            index = 5;
                        }
                        break;

                    case "i": // Initilalize
                    case "I":
                        {
                            Console.WriteLine(" Your Option: Initialize the AgentQueue Table");
                            Console.WriteLine(" \n    !!!  This option deletes all records in the AgentQueue table  !!!");
                            Console.WriteLine(" \n         Press Y to proceed...  Any other key to cancel..");
                            ConsoleKeyInfo confirm = Console.ReadKey(true);
                            if (confirm.KeyChar == 'Y' || confirm.KeyChar == 'y')
                            {
                                int rowsDeleted = 0;
                                rowsDeleted = ClearAgentQueue();
                                Console.WriteLine("\n ---------- Deleted {0} rows!", rowsDeleted);
                            }

                            Thread.Sleep(1000);
                            break;
                        }
                    case "x":
                    case "X":
                        {
                            return;
                        }
                    default:

                        Console.WriteLine("    \nInvalid request....");
                        break;
                }
                if (index > -1)
                {
                    string svcName = string.Format("RRDM RFM {0}", SERVICE_LIST[index].origin);
                    InsertServiceRequest(req, SERVICE_LIST[index].serviceId, svcName);
                }
            }
        }


        #region InsertServiceRequest(string Cmd, string ServiceName)
        static void InsertServiceRequest(string Cmd, string ServiceID, string ServiceName)
        {
            Console.WriteLine("Your option:\n   Command: {0}\n   Service Name: {1}", Cmd, ServiceName);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();


            RRDMAgentQueue AgntQ = new RRDMAgentQueue();

            AgntQ.ReqDateTime = DateTime.Now;
            AgntQ.RequestorID = "Simulator";
            AgntQ.RequestorMachine = "TestPC";
            AgntQ.Command = Cmd;
            AgntQ.ServiceId = ServiceID;
            AgntQ.ServiceName = ServiceName;
            AgntQ.Priority = 1;
            AgntQ.ReqStatusCode = 0;
            AgntQ.Operator = argOperator;
            AgntQ.OriginalReqID = 0;
            AgntQ.OriginalRequestorID = "myself";


            AgntQ.InsertNewRecordInAgentQueue();
            if (AgntQ.ErrorFound)
            {
                Console.WriteLine("INSERT:{0} returned: {1}", Cmd, AgntQ.ErrorOutput);
            }
        }
        #endregion

        #region  ClearAgentQueue()
        static int ClearAgentQueue()
        {
            int rowsDeleted = 0;
            RRDMAgentQueue AgntQ = new RRDMAgentQueue();

            rowsDeleted = AgntQ.DeleteAllRecordsInAgentQueue();
            if (AgntQ.ErrorFound)
            {
                Console.WriteLine("\nClearAgentQueue returned: {1}", AgntQ.ErrorOutput);
            }

            return (rowsDeleted);
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
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    {
                        CloseOrBreakKey = true;
                        Console.WriteLine("\n      CTRL+C received!");
                        Console.WriteLine("Press ENTER to to exit...");
                        break;
                    }
                case CtrlTypes.CTRL_BREAK_EVENT:
                    {
                        CloseOrBreakKey = true;
                        Console.WriteLine("\n      CTRL+BREAK received!");
                        Console.WriteLine("Press ENTER to to exit...");
                        break;
                    }
            }
            return true;
        }
        #endregion

    }
}
