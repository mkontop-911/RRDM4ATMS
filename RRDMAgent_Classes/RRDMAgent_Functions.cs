using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using Microsoft.Win32;
using RRDM4ATMs;

namespace RRDMAgent_Classes
{
    #region Enumerations and structures
    public enum ServiceRequestResult
    {
        Success,
        ServiceNotInstalled,
        ServiceAlreadyStarted,
        ServiceAlreadyStoped,
        ServiceFailedToStart,
        ServiceFailedToStop,
        ServiceFailedStatus,
        Error_Win32,
        Error_DotNet,
        ServiceStartTimeout,
        ServiceStopTimeout,
        InvalidRequest
    }

    public static class AgentProcessingResult
    {
        public const int Cmd_Success = (int)ServiceRequestResult.Success;
        public const int ServiceNotInstalled = (int)ServiceRequestResult.ServiceNotInstalled;
        public const int ServiceAlreadyStarted = (int)ServiceRequestResult.ServiceAlreadyStarted;
        public const int ServiceAlreadyStopped = (int)ServiceRequestResult.ServiceAlreadyStoped;
        public const int ServiceFailedToStart = (int)ServiceRequestResult.ServiceFailedToStart;
        public const int ServiceFailedToStop = (int)ServiceRequestResult.ServiceFailedToStop;
        public const int ServiceFailedStatus = (int)ServiceRequestResult.ServiceFailedStatus;
        public const int ServiceWin32Exception = (int)ServiceRequestResult.Error_Win32;
        public const int ServiceDotNetException = (int)ServiceRequestResult.Error_DotNet;
        public const int ServiceStartTimeout = (int)ServiceRequestResult.ServiceStartTimeout;
        public const int ServiceStopTimeout = (int)ServiceRequestResult.ServiceStopTimeout;
        public const int InvalidRequest = (int)ServiceRequestResult.InvalidRequest;

        private struct AgentRequestResultStruct
        {
            public int retCode;
            public string retMessage;
            public AgentRequestResultStruct(int retCode, string retMessage)
            {
                this.retCode = retCode;
                this.retMessage = retMessage;
            }
        }

        private static AgentRequestResultStruct[] RESULT_LIST = new AgentRequestResultStruct[]
        {
            new AgentRequestResultStruct(Cmd_Success, "Command Processed Successfully"),
            new AgentRequestResultStruct(ServiceNotInstalled, "Service NOT Installed"),
            new AgentRequestResultStruct(ServiceAlreadyStarted, "Service Already Started"),
            new AgentRequestResultStruct(ServiceAlreadyStopped, "Service Already Stopped"),
            new AgentRequestResultStruct(ServiceFailedToStart, "Agent Failed to Start the Service"),
            new AgentRequestResultStruct(ServiceFailedToStop, "Agent Failed to Stop the Service"),
            new AgentRequestResultStruct(ServiceFailedStatus, "Agent Failed to get Service Status"),
            new AgentRequestResultStruct(ServiceWin32Exception, "Win32 Exception"),
            new AgentRequestResultStruct(ServiceDotNetException, ".Net Framework Exception"),
            new AgentRequestResultStruct(ServiceStartTimeout, "Service Failed to Start (timeout)"),
            new AgentRequestResultStruct(ServiceStopTimeout, "Service Failed to Stop (timeout)"),
            new AgentRequestResultStruct(InvalidRequest, "Invalid Request Command")
        };

        public static string getResultMessageFromCode(int code)
        {
            foreach (AgentRequestResultStruct res in RESULT_LIST)
            {
                if (res.retCode == code) return res.retMessage;
            }
            return "Result: Unknown Result Code, " + code;
        }
    }
    #endregion


    #region Class to handle Requests for Services (Start/Stop/Status)
    public class AgentServiceRequest
    {
        public ServiceControllerStatus ServiceStatus;
        public ServiceRequestResult ResultCode;
        public string ResultText;
        public string exceptionText;

        #region GetServiceStatus(string svcDisplayName)
        public void GetServiceStatus(string svcDisplayName)
        {
            ServiceController sc = new ServiceController(svcDisplayName);
            try
            {
                // try to get a property of the service:
                //   - if the service is not installed, we get 'InvalidOperationException'
                //   - access rights issues will throw a 'Win32Exception'
                string ServiceName = sc.DisplayName;
                ServiceStatus = sc.Status;
                ResultCode = ServiceRequestResult.Success;
                ResultText = sc.Status.ToString();
            }
            catch (InvalidOperationException ex)
            {
                ResultCode = ServiceRequestResult.ServiceNotInstalled;
                ResultText = AgentProcessingResult.getResultMessageFromCode((int)ResultCode); // "NotInstalled";
                string exMsg = ex.Message;
                Exception ex1 = ex.InnerException;
                while (ex1 != null)
                {
                    exMsg += Environment.NewLine + ex1.Message;
                    ex1 = ex1.InnerException;
                }
                exceptionText = exMsg;
            }
            catch (Win32Exception ex)
            {
                ResultCode = ServiceRequestResult.Error_Win32;
                ResultText = AgentProcessingResult.getResultMessageFromCode((int)ServiceRequestResult.ServiceFailedStatus); // "FailedToGetStatus";
                string exMsg = string.Format("Win32 ErrorCode: {0}\n Exception Message:\n{1}", ex.NativeErrorCode.ToString(), ex.Message);
                Exception ex1 = ex.InnerException;
                while (ex1 != null)
                {
                    exMsg += Environment.NewLine + ex1.Message;
                    ex1 = ex1.InnerException;
                }
                exceptionText = exMsg;
            }
            catch (Exception ex)
            {
                ResultCode = ServiceRequestResult.Error_DotNet;
                ResultText = AgentProcessingResult.getResultMessageFromCode((int)ServiceRequestResult.ServiceFailedStatus); // "ServiceStatusException";
                string exMsg = ex.Message;
                Exception ex1 = ex.InnerException;
                while (ex1 != null)
                {
                    exMsg += Environment.NewLine + ex1.Message;
                    ex1 = ex1.InnerException;
                }
                exceptionText = exMsg;
            }
            finally
            {
                sc.Close();
            }
            return;
        }
        #endregion

        #region StartService(string serviceDisplayName, int timeoutSeconds)
        public void StartService(string serviceDisplayName, int timeoutSeconds)
        {
            this.GetServiceStatus(serviceDisplayName);
            if (this.ResultCode != ServiceRequestResult.Success)
            {
                // ResultText and exceptionText are set in the GetServiceStatus method
                return;
            }
            else
            {
                if (this.ServiceStatus == ServiceControllerStatus.Stopped)
                {
                    ServiceController sc = new ServiceController(serviceDisplayName);
                    try
                    {
                        TimeSpan timeout = TimeSpan.FromSeconds(timeoutSeconds);

                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running, timeout);
                        this.ResultCode = ServiceRequestResult.Success;
                        this.ResultText = AgentProcessingResult.getResultMessageFromCode((int)ResultCode);
                    }
                    catch (System.ServiceProcess.TimeoutException ex)
                    {
                        this.ResultCode = ServiceRequestResult.ServiceStartTimeout;
                        this.ResultText = AgentProcessingResult.getResultMessageFromCode((int)ResultCode);
                        string exMsg = ex.Message;
                        Exception ex1 = ex.InnerException;
                        while (ex1 != null)
                        {
                            exMsg += Environment.NewLine + ex1.Message;
                            ex1 = ex1.InnerException;
                        }
                        this.exceptionText = exMsg;

                    }
                    catch (Exception ex)
                    {
                        this.ResultCode = ServiceRequestResult.Error_DotNet;
                        this.ResultText = AgentProcessingResult.getResultMessageFromCode((int)ResultCode);
                        string exMsg = ex.Message;
                        Exception ex1 = ex.InnerException;
                        while (ex1 != null)
                        {
                            exMsg += Environment.NewLine + ex1.Message;
                            ex1 = ex1.InnerException;
                        }
                        this.exceptionText = exMsg;
                    }
                    finally
                    {
                        sc.Close();
                    }
                    return;
                }
                else if (this.ServiceStatus == ServiceControllerStatus.Running)
                {
                    this.ResultCode = ServiceRequestResult.ServiceAlreadyStarted;
                    this.ResultText = AgentProcessingResult.getResultMessageFromCode((int)ResultCode); // 
                    this.exceptionText = string.Format("The Service Start request was Canceled! Service {0} is already {1}", serviceDisplayName, this.ServiceStatus.ToString());
                }
                else
                {
                    this.ResultCode = ServiceRequestResult.ServiceFailedToStart;
                    this.ResultText = AgentProcessingResult.getResultMessageFromCode((int)ResultCode); // 
                    this.exceptionText = string.Format("The Service Start request Failed! The Current Status of Service {0} is {1}", serviceDisplayName, this.ServiceStatus.ToString());
                }
            }
        }

        public bool AddToAgentMonitorList(string serviceId, string serviceDisplayName, int originalReqID, string originalRequestorID)
        {
            int MonitorSlots = RRDMAgentServer.MonitorSlots;
            bool rc = false;
            string imgPath;
            string[] paramList;
            int paramNumber = 0;

            try
            {
                imgPath = GetServiceInstallPath(serviceDisplayName);
                if (!string.IsNullOrEmpty(imgPath))
                {
                    paramList = ExtractParametersFromImagePath(imgPath);
                    paramNumber = paramList.Length;
                    if (paramNumber == 3)
                    {
                        // Get  available structure array element
                        for (int i = 0; i < MonitorSlots; i++)
                        {
                            if (!RRDMAgentServer.MonitorServiceList[i].inUse)
                            {
                                RRDMAgentServer.ServiceRecIndx = i;
                                break;
                            }
                        }

                        // Read the record from from RRDMMatchingSourceFiles
                        RRDMMatchingSourceFiles rsf = new RRDMMatchingSourceFiles();
                        rsf.ReadSourceFileRecordByOriginAndFileID(paramList[0], paramList[1]);
                        if (rsf.ErrorFound || !rsf.RecordFound)
                        {
                            string msg = string.Format("Error reading SourceID: {0}  of  Origin: {1} from the [MatchingSourceFiles] table! \nThe received error message is: \n{2}",
                                                        paramList[1], paramList[0], rsf.ErrorOutput);
                            AgentEventLogging.RecordEventMsg(msg, EventLogEntryType.Error);
                            rc = false;
                        }
                        else
                        {
                            RRDMAgentServer.MonitorServiceList[RRDMAgentServer.ServiceRecIndx].serviceID = serviceId;
                            RRDMAgentServer.MonitorServiceList[RRDMAgentServer.ServiceRecIndx].serviceName = serviceDisplayName;
                            RRDMAgentServer.MonitorServiceList[RRDMAgentServer.ServiceRecIndx].imagePath = imgPath;
                            RRDMAgentServer.MonitorServiceList[RRDMAgentServer.ServiceRecIndx].paramOrigin = paramList[0];
                            RRDMAgentServer.MonitorServiceList[RRDMAgentServer.ServiceRecIndx].paramSourceId = paramList[1];
                            RRDMAgentServer.MonitorServiceList[RRDMAgentServer.ServiceRecIndx].paramOperator = paramList[2];
                            RRDMAgentServer.MonitorServiceList[RRDMAgentServer.ServiceRecIndx].StartTime = DateTime.Now;
                            RRDMAgentServer.MonitorServiceList[RRDMAgentServer.ServiceRecIndx].filePool = rsf.SourceDirectory;
                            RRDMAgentServer.MonitorServiceList[RRDMAgentServer.ServiceRecIndx].fileMask = rsf.FileNameMask;
                            RRDMAgentServer.MonitorServiceList[RRDMAgentServer.ServiceRecIndx].inUse = true;
                            RRDMAgentServer.MonitorServiceList[RRDMAgentServer.ServiceRecIndx].originalReqID = originalReqID;
                            RRDMAgentServer.MonitorServiceList[RRDMAgentServer.ServiceRecIndx].originalRequestorID = originalRequestorID;
                            rc = true;
                        }
                    }
                    else
                    {
                        rc = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string exMsg = ex.Message;
                Exception ex1 = ex.InnerException;
                while (ex1 != null)
                {
                    exMsg += Environment.NewLine + ex1.Message;
                    ex1 = ex1.InnerException;
                }

                string msg = string.Format("Request to Start Service {0} was succesfull but the Agent failed to save the service's parameters in its service monitor list!\nThe exception reads:\n{1}",
                             serviceDisplayName, exMsg);
                AgentEventLogging.RecordEventMsg(exMsg, EventLogEntryType.Error);
            }
            return (rc);
        }
        #endregion

        #region StopService(string serviceDisplayName, int timeoutSeconds)
        public void StopService(string serviceDisplayName, int timeoutSeconds)
        {
            this.GetServiceStatus(serviceDisplayName);
            if (this.ResultCode != ServiceRequestResult.Success)
            {
                return;
            }
            else
            {
                if (this.ServiceStatus == ServiceControllerStatus.Running)
                {
                    ServiceController sc = new ServiceController(serviceDisplayName);
                    try
                    {
                        TimeSpan timeout = TimeSpan.FromSeconds(timeoutSeconds);

                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                        this.ResultCode = ServiceRequestResult.Success;
                        this.ResultText = AgentProcessingResult.getResultMessageFromCode((int)ResultCode);
                    }
                    catch (System.ServiceProcess.TimeoutException ex)
                    {
                        this.ResultCode = ServiceRequestResult.ServiceFailedToStop;
                        this.ResultText = AgentProcessingResult.getResultMessageFromCode((int)ResultCode);
                        string exMsg = ex.Message;
                        Exception ex1 = ex.InnerException;
                        while (ex1 != null)
                        {
                            exMsg += Environment.NewLine + ex1.Message;
                            ex1 = ex1.InnerException;
                        }
                        this.exceptionText = exMsg;
                    }
                    catch (Exception ex)
                    {
                        this.ResultCode = ServiceRequestResult.Error_DotNet;
                        ResultText = AgentProcessingResult.getResultMessageFromCode((int)ResultCode);
                        string exMsg = ex.Message;
                        Exception ex1 = ex.InnerException;
                        while (ex1 != null)
                        {
                            exMsg += Environment.NewLine + ex1.Message;
                            ex1 = ex1.InnerException;
                        }
                        this.exceptionText = exMsg;
                    }
                    finally
                    {
                        sc.Close();
                    }
                    return;
                }
                else if (this.ServiceStatus == ServiceControllerStatus.Stopped)
                {
                    this.ResultCode = ServiceRequestResult.ServiceAlreadyStoped;
                    this.ResultText = AgentProcessingResult.getResultMessageFromCode((int)ResultCode); // 
                    this.exceptionText = string.Format("The Service Stop request was Canceled! Service {0} is already {1}", serviceDisplayName, this.ServiceStatus.ToString());
                }
                else
                {
                    this.ResultCode = ServiceRequestResult.ServiceFailedToStop;
                    this.ResultText = AgentProcessingResult.getResultMessageFromCode((int)ResultCode); // 
                    this.exceptionText = string.Format("The Service Stop request Failed! The Current Status of Service {0} is {1}", serviceDisplayName, this.ServiceStatus.ToString());
                }
            }
        }
        #endregion

        #region Service Image path and Parameters
        private static string GetServiceInstallPath(string serviceName)
        {
            RegistryKey regkey;
            regkey = Registry.LocalMachine.OpenSubKey(string.Format(@"SYSTEM\CurrentControlSet\services\{0}", serviceName));

            if (regkey.GetValue("ImagePath") == null)
                return "Not Found";
            else
                return regkey.GetValue("ImagePath").ToString();
        }

        private static string[] ExtractParametersFromImagePath(string imgPath)
        {
            var indexOfParams = imgPath.IndexOf(' ');
            string paramString = imgPath.Substring(indexOfParams + 1);

            string[] paramList = paramString.Split(' ');
            return (paramList);
        }
        #endregion

        #endregion
    }
}