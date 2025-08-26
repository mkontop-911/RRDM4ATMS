using System;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Threading;

using RRDM4ATMs;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace RRDMAgent_Classes
{

    #region ServiceRec struct 
    public struct ServiceRec // Structure containing information for monitoring a service
    {
        public bool inUse;
        public int originalReqID;
        public string originalRequestorID;
        public string serviceID;
        public string serviceName;
        public string imagePath;
        public string paramOrigin;
        public string paramSourceId;
        public string paramOperator;
        public string filePool;
        public string fileMask;
        public DateTime StartTime;
    }
    #endregion

    public class RRDMAgentServer
    {
        public static int MonitorSlots = 25; // read from parameters ?
        public static volatile ServiceRec[] MonitorServiceList = new ServiceRec[MonitorSlots];
        public static volatile int ServiceRecIndx = 0;


        public Thread StartRRDMAgentThread(string operatr)
        {
            RRDMAgentStartPoint.argOperator = operatr;

            // Event Handle to wait on for thread to start 
            AutoResetEvent hThreadStartedEvent = new AutoResetEvent(false);

            // Start the RRDMAgent Server thread
            ParameterizedThreadStart ts = new ParameterizedThreadStart(RRDMAgentStartPoint.RRDMAgentStartup);

            Thread oT = new Thread(ts);
            oT.Name = "RRDMAgentThread";
            oT.Start(hThreadStartedEvent); // pass the event handler by which the thread will signal back...

            // Wait for the thread to signal back 
            // this is how we make sure that the thread initialized properly and is ready to go..
            bool ThrStarted = hThreadStartedEvent.WaitOne(10000);
            if (ThrStarted)
                return (oT);
            else
                return null;
        }
    }

    public static class RRDMAgentStartPoint
    {
        #region Static members
        const string PROGRAM_MUTEX_NAME = "Global\\RRDMAGENT";
        private static string instanceMutexName = "";

        public static string argOperator = "";

        public static int glbRRDMAgentSleep;  // read from parameters
        public static int glbRRDMAgentServiceTimeout;  // read from parameters
        public static int glbRRDMAgentMaxServicesToMonitor = RRDMAgentServer.MonitorSlots;


        public volatile static bool SignalToStop = false; // For handling termination in case of fatal errors
        #endregion


        public static void RRDMAgentStartup(object hwStartedEvent)
        {
            // These have been set earlier by the caller
            //argOperator = _argOperator;

            bool IsTheOnlyInstance;
            instanceMutexName = PROGRAM_MUTEX_NAME;
            try
            {
                Mutex InstanceMutex = new Mutex(true, instanceMutexName, out IsTheOnlyInstance);
            }
            catch (UnauthorizedAccessException ex)
            {
                string msg = string.Format("MUTEX exception: {0}", ex.Message);
                AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                IsTheOnlyInstance = false;
            }

            if (IsTheOnlyInstance == false)
            {
                string msg = string.Format("{0}\r\nAnother instance of this program is already running!\r\nOnly one instance is allowed to run at any time!",
                                           instanceMutexName);
                if (Environment.UserInteractive)
                {
                    MessageBox.Show(msg, "RRDMAGENT", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                }
                return;
            }

            // Write an 'information' event in the EventLog
            string mesg = string.Format("RRDMAgentStartup: Starting RRDM Agent for Operator: {0} ", argOperator);
            AgentEventLogging.RecordEventMsg(mesg, EventLogEntryType.Information);


            #region Read the Sleep parameters from database and validate
            RRDMGasParameters Gp = new RRDMGasParameters();
            // How long to sleep before searching for new commands to execute (seconds)
            Gp.ReadParametersSpecificId(argOperator, "915", "1", "", "");
            if (!Gp.ErrorFound)
                if (Gp.RecordFound)
                {
                    glbRRDMAgentSleep = (int)Gp.Amount;
                }
                else
                {
                    glbRRDMAgentSleep = 10; // set to 10 sec
                }
            else
            {
                // Database error!
                string msg = string.Format("Processing stopped!\nError reading parameter: 915 Occur: 1 from parameters table!\nOperator is: '{0}'", argOperator);
                AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return;
            }
            #endregion

            #region Read the Service Start/Stop timeout parameter from database and validate
            // RRDMGasParameters Gp = new RRDMGasParameters();
            // How long to wait for a service to star/stop (seconds)
            Gp.ReadParametersSpecificId(argOperator, "915", "2", "", "");
            if (!Gp.ErrorFound)
                if (Gp.RecordFound)
                {
                    glbRRDMAgentServiceTimeout = (int)Gp.Amount;
                }
                else
                {
                    glbRRDMAgentServiceTimeout = 10; // set to 10 sec
                }
            else
            {
                // Database error!
                string msg = string.Format("Processing stopped!\nError reading parameter: 915 Occur: 2 from parameters table!\nOperator is: '{0}'", argOperator);
                AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                return;
            }
            #endregion

            // Signal back that this thread has started
            ((AutoResetEvent)hwStartedEvent).Set();


            #region Show signs that the program started and is waiting for commands
            if (Environment.UserInteractive)
            {
                Console.WriteLine("");
                Console.WriteLine(string.Format("  Ready to process incoming requests"));
                Console.WriteLine("");
                Console.WriteLine("  Press [ CTRL + C ] to end this program....");
                Console.WriteLine("");
            }
            #endregion

            #region Loop in search of requests in AgentQueue until CTRL+C or program receives termination signal
            while (!SignalToStop)
            {
                if (SignalToStop)
                {
                    string msg = string.Format("RRDMAgentStartup: RRDM Agent received a signal to stop...");
                    AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Warning);
                    return;
                }

                // Read from the AgentQueue table
                RRDMAgentQueue AgntQ = new RRDMAgentQueue();
                AgntQ.ReadSingleAgentRequestByPriority(); // Members include the AgentQueueStruct element

                // If SQL returned error, Log and throw a new JTMCustomException to terminate
                if (AgntQ.ErrorFound)
                {
                    string msg = string.Format("RRDMAgentStartup: Scanning for new requests from SQL returned an error!!\nThe error reads:\n{0}",
                                        AgntQ.ErrorOutput);
                    AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                }
                else if (AgntQ.RecordFound)
                {

                    if (Environment.UserInteractive)
                    {
                        Console.WriteLine("");
                        Console.WriteLine(string.Format("Intercepted new request: {0} - {1}", AgntQ.Command, AgntQ.ServiceName));
                    }
                    #region Process the request
                    // Update record in Agent Queue
                    AgntQ.ReqStatusCode = AgentStatus.Req_WorkInProgress;
                    AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                    switch (AgntQ.Command)
                    {
                        case AgentRequestCommand.Cmd_ServiceStart:
                        case AgentRequestCommand.Cmd_ServiceStartMonitor:
                            {
                                AgntQ.CmdExecStarted = DateTime.Now;

                                AgentServiceRequest asr = new AgentServiceRequest();
                                asr.StartService(AgntQ.ServiceName, glbRRDMAgentServiceTimeout);

                                switch (asr.ResultCode)
                                {
                                    case ServiceRequestResult.Success:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Finished;
                                            AgntQ.CmdStatusCode = AgentProcessingResult.Cmd_Success;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.CmdExecFinished = DateTime.Now;
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("The request to Start Service {0} was processed successfully!", AgntQ.ServiceName);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Information);

                                            if (AgntQ.Command == AgentRequestCommand.Cmd_ServiceStartMonitor)
                                            {
                                                bool rc = asr.AddToAgentMonitorList(AgntQ.ServiceId, AgntQ.ServiceName, AgntQ.ReqID, AgntQ.OriginalRequestorID);
                                                if (rc == false)
                                                {
                                                    msg = string.Format("Service {0} in the SERVICE_START_AND_MONITOR request was started successfully, " +
                                                        "but the Agent failed to add it in the MonitoringList", AgntQ.ServiceName);
                                                    AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                                }
                                            }

                                            break;
                                        }
                                    case ServiceRequestResult.ServiceNotInstalled:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("Request to Start Service {0} failed because the service does not exist!\nThe exception reads:\n{1}",
                                                        AgntQ.ServiceName, asr.exceptionText);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                    case ServiceRequestResult.ServiceAlreadyStarted:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("Request to Start Service {0} failed because the service was already started!\nThe exception reads:\n{1}",
                                                        AgntQ.ServiceName, asr.exceptionText);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                    case ServiceRequestResult.ServiceStartTimeout:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("Failed to start Service {0} within the specified timeout interval of {1} seconds!\nThe exception reads:\n{2}",
                                                        AgntQ.ServiceName, glbRRDMAgentServiceTimeout, asr.exceptionText);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                    case ServiceRequestResult.ServiceFailedToStart:
                                    case ServiceRequestResult.Error_Win32:
                                    case ServiceRequestResult.Error_DotNet:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("An excepiton was raised while starting Service {0}\nThe exception messages is: {1}", AgntQ.ServiceName, asr.exceptionText);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                    default:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = "Program error";
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("Unexpected return code [{0}] received while starting Service [{1}]\n", asr.ResultCode, AgntQ.ServiceName);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                }
                                break;
                            }
                        case AgentRequestCommand.Cmd_ServiceStop:
                            {
                                AgntQ.CmdExecStarted = DateTime.Now;

                                AgentServiceRequest asr = new AgentServiceRequest();
                                asr.StopService(AgntQ.ServiceName, glbRRDMAgentServiceTimeout);

                                switch (asr.ResultCode)
                                {
                                    case ServiceRequestResult.Success:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Finished;
                                            AgntQ.CmdStatusCode = AgentProcessingResult.Cmd_Success;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.CmdExecFinished = DateTime.Now;

                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("Request to Stop the service \"{0}\" was processed successfully!", AgntQ.ServiceName);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Information);
                                            break;
                                        }
                                    case ServiceRequestResult.ServiceNotInstalled:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("Request to Stop Service {0} failed because the service does not exist!\nThe exception reads:\n{1}",
                                                        AgntQ.ServiceName, asr.exceptionText);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                    case ServiceRequestResult.ServiceAlreadyStoped:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("Request to Stop Service {0} failed because the service was already stopped!\nThe exception reads:\n{1}",
                                                        AgntQ.ServiceName, asr.exceptionText);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                    case ServiceRequestResult.ServiceStopTimeout:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("Failed to stop Service {0} within the specified timeout interval of {1} seconds!\nThe exception reads:\n{2}",
                                                        AgntQ.ServiceName, glbRRDMAgentServiceTimeout, asr.exceptionText);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                    case ServiceRequestResult.ServiceFailedToStop:
                                    case ServiceRequestResult.Error_Win32:
                                    case ServiceRequestResult.Error_DotNet:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("An exception was raised while stopping Service [{0}]\nThe exception messages is: {1}", AgntQ.ServiceName, asr.exceptionText);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                    default:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = "Program error";
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("Unexpected return code [{0}] received while stopping Service [{1}]\n", asr.ResultCode, AgntQ.ServiceName);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                }
                                break;
                            }
                        case AgentRequestCommand.Cmd_ServiceStatus:
                            {
                                AgentServiceRequest asr = new AgentServiceRequest();
                                asr.GetServiceStatus(AgntQ.ServiceName);

                                switch (asr.ResultCode)
                                {
                                    case ServiceRequestResult.Success:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Finished;
                                            AgntQ.CmdStatusCode = AgentProcessingResult.Cmd_Success;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);
                                            string msg = string.Format("Request to get the status of service \"{0}\" was processed successfully!", AgntQ.ServiceName);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Information);
                                            break;
                                        }
                                    case ServiceRequestResult.ServiceNotInstalled:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("Request to get the status of service \"{0}\" failed because the service does not exist!\nThe exception messages is: {1}",
                                                                        AgntQ.ServiceName, asr.exceptionText);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                    case ServiceRequestResult.Error_Win32:
                                    case ServiceRequestResult.Error_DotNet:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = asr.ResultText;
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("An exception was raised while getting the status of service [{0}]\nThe exception messages is: {1}",
                                                                        AgntQ.ServiceName, asr.exceptionText);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                    default:
                                        {
                                            // Update record in Agent Queue
                                            AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                            AgntQ.CmdStatusCode = (int)asr.ResultCode;
                                            AgntQ.CmdStatusMessage = "Program error";
                                            AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);

                                            string msg = string.Format("Unexpected return code [{0}] received while getting the status of Service [{1}]\n", asr.ResultCode, AgntQ.ServiceName);
                                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                            break;
                                        }
                                }
                                break;
                            }
                        default:
                            {
                                // Invalid command - Log and discard
                                string msg = string.Format("Invalid command: [{0}]!", AgntQ.Command);
                                AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                                AgntQ.ReqStatusCode = AgentStatus.Req_Aborted;
                                AgntQ.CmdStatusCode = AgentProcessingResult.InvalidRequest;
                                AgntQ.CmdStatusMessage = msg;
                                AgntQ.UpdateRecordInAgentQueueByReqID(AgntQ.ReqID);
                                break;
                            }
                    }
                    #endregion
                }
                else // No record found
                {
                    string fileMask;

                    // Check running services and stop if condition met
                    for (int i = 0; i < RRDMAgentServer.MonitorSlots; i++)
                    {
                        if (RRDMAgentServer.MonitorServiceList[i].inUse == true)
                        {
                            if (RRDMAgentServer.MonitorServiceList[i].paramSourceId == "Atms_Journals_Txns")
                            {
                                fileMask = RRDMAgentServer.MonitorServiceList[i].fileMask;
                                string[] FileArray = GetArrayofFiles(RRDMAgentServer.MonitorServiceList[i].filePool, RRDMAgentServer.MonitorServiceList[i].fileMask);
                                // Stop the service if no files are found, by writting a 'STOP' record in the Agent Queue (Will be done in the next Iteration)
                                if (FileArray.Length == 0)
                                {
                                    InsertSTOPServiceRequest(RRDMAgentServer.MonitorServiceList[i].serviceID,
                                                            RRDMAgentServer.MonitorServiceList[i].serviceName,
                                                            RRDMAgentServer.MonitorServiceList[i].paramOperator,
                                                            RRDMAgentServer.MonitorServiceList[i].originalReqID,
                                                            RRDMAgentServer.MonitorServiceList[i].originalRequestorID
                                                            );
                                    RRDMAgentServer.MonitorServiceList[i].inUse = false;
                                }
                            }
                            else
                            {
                                int filesFound = 0;
                                string pattern = RRDMAgentServer.MonitorServiceList[i].paramSourceId + "_[0-9]{8}\\.[0-9]{3}";

                                fileMask = RRDMAgentServer.MonitorServiceList[i].paramSourceId + "_????????.???";
                                string[] FileArray = GetArrayofFiles(RRDMAgentServer.MonitorServiceList[i].filePool, fileMask);
                                foreach (string fullFileName in FileArray)
                                {
                                    // match using regular expression (fileMask)
                                    if (Regex.IsMatch(fullFileName, pattern))
                                    {
                                        filesFound++;
                                    }
                                }
                                if (filesFound == 0)
                                {
                                    InsertSTOPServiceRequest(RRDMAgentServer.MonitorServiceList[i].serviceID,
                                                            RRDMAgentServer.MonitorServiceList[i].serviceName,
                                                            RRDMAgentServer.MonitorServiceList[i].paramOperator,
                                                            RRDMAgentServer.MonitorServiceList[i].originalReqID,
                                                            RRDMAgentServer.MonitorServiceList[i].originalRequestorID
                                                            );
                                    RRDMAgentServer.MonitorServiceList[i].inUse = false;
                                }
                            }
                        }
                    }

                    // Nothing to process! Sleep (glbRRDMAgentSleep is in seconds, so loop as many times and sleep for one second each)
                    int loopCount = glbRRDMAgentSleep; // 
                    for (int i = 0; i < loopCount; i++)
                    {
                        if (!SignalToStop)
                        {
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            string msg = string.Format("RRDMAgentStartup: RRDM Agent received a signal to stop while procesing the MonitorList...");
                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Warning);
                            break;
                        }
                    }
                } // end while            }
            } // End of loop
            #endregion
        }

        #region InsertSTOPServiceRequest(string ServiceID, string ServiceName, string Operator)
        static void InsertSTOPServiceRequest(string ServiceID, string ServiceName, string Operator, int originalReID, string originalRequestorID)
        {
            RRDMAgentQueue AgntQ = new RRDMAgentQueue();

            AgntQ.ReqDateTime = DateTime.Now;
            AgntQ.RequestorID = "RRDMAgent";
            AgntQ.RequestorMachine = Environment.MachineName;
            AgntQ.Command = AgentRequestCommand.Cmd_ServiceStop;
            AgntQ.ServiceId = ServiceID;
            AgntQ.ServiceName = ServiceName;
            AgntQ.Priority = 1;
            AgntQ.ReqStatusCode = 0;
            AgntQ.Operator = Operator;
            AgntQ.OriginalReqID = originalReID;
            AgntQ.OriginalRequestorID = originalRequestorID;
            // Is Stopped by monitor

            AgntQ.InsertNewRecordInAgentQueue();
            if (AgntQ.ErrorFound)
            {
                string msg = string.Format("The Agent failed to insert a 'STOP' record in the queue for service: {0}!",
                                            ServiceName);
                AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
            }
        }
        #endregion

        #region Get List of files in the pool
        private static string[] GetArrayofFiles(string pool, string filemask)
        {
            string[] fileArr = new string[] { };

            try
            {
                fileArr = Directory.GetFiles(pool, filemask);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Exception ex1 = ex.InnerException;
                while (ex1 != null)
                {
                    msg += ex1.Message;
                    ex1 = ex1.InnerException;
                }
                AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
            }
            return fileArr;
        }
        #endregion
    }

}


