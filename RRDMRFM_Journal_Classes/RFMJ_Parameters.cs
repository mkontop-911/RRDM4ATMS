using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using RRDM4ATMs;
using System.Diagnostics;

namespace RRDMRFM_Journal_Classes
{

    public class RfmjParameters
    {
        #region Members
        public bool IsSuccess { get; set; }
        public string errorMsg { get; set; }

        public string Operator { get; set; }

        // Thread Parameters
        public int RFMJ_MaxThreadNumber { get; set; }
        public int RFMJ_SleepWaitEmptyThreadSlot { get; set; }
        public int RFMJ_MaxThreadLifeSpan { get; set; }
        public int RFMJ_ThreadAbortWait { get; set; }
        public int RFMJ_StartWorkerThreadTimeout { get; set; }
        public int RFMJ_ThreadMonitorInterval { get; set; }

        // Refresh Rates
        public int RFMJ_RefreshInterval { get; set; }

        // Stored Procedure
        public string Rfmj_StoredProcedure { get; set; }

        // Relative path of Filepool as seen from the Stored Procedure
        public string SQLRelativeFilePoolPath { get; set; }

        #endregion


        #region Contructor: RFMJ Parameters ()
        public RfmjParameters(string _operator)
        {
            Operator = _operator;

            string msg = "";
            string ValMsg = "";
            string ValMsgFmt = @"ParamId:{0}, OccuranceId:{1} --> {2}";
            // EventLogging.MessageOut("RfmjParameters: start", EventLogEntryType.Information);
            try
            {
                #region Get AppSettings["SQLRelativeFilePoolPath"] (app.config)
                SQLRelativeFilePoolPath = null;
                try
                {
                    EventLogging.MessageOut("RfmjParameters: SQLRelativeFilePoolPath", EventLogEntryType.Information);
                    SQLRelativeFilePoolPath = ConfigurationManager.AppSettings["SQLRelativeFilePoolPath"];
                }
                catch (ConfigurationErrorsException ex)
                {
                    if (ex.InnerException != null)
                    {
                        msg += ex.InnerException.Message;
                    }
                    string msg1 = string.Format("Terminating because of a configuration file error while reading AppSettings values. The error reads:\n{0}", msg);
                    EventLogging.MessageOut(msg1, EventLogEntryType.Error);
                    // the ConsoleDisplay thread is not started yet, so we display the message ourselves
                    if (Environment.UserInteractive)
                        Console.WriteLine(msg1);
                    return;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        msg += ex.InnerException.Message;
                    }
                    string msg1 = string.Format("Terminating because of a configuration file error! The error reads:\n{0}", msg);
                    EventLogging.MessageOut(msg1, EventLogEntryType.Error);
                    // the ConsoleDisplay thread is not started yet, so we display the message ourselves
                    if (Environment.UserInteractive)
                        Console.WriteLine(msg1);
                    return;
                }
                #endregion

                #region Read Parameters from database and validate

                // TODO Consider reading all parameters in one go...

                RRDMGasParameters Gp = new RRDMGasParameters();
                
                // Max Threads to start
                Gp.ReadParametersSpecificId(Operator, "914", "1", "", "");
                RFMJ_MaxThreadNumber = (int)Gp.Amount;
                if (RFMJ_MaxThreadNumber == 0)
                {
                    ValMsg = ValMsg + string.Format(ValMsgFmt, "914", "1", "RFMJ_MaxThreadNumber!\n");
                }
                // How long to wait for an available thread slot
                Gp.ReadParametersSpecificId(Operator, "914", "2", "", "");
                RFMJ_SleepWaitEmptyThreadSlot = (int)Gp.Amount;
                if (RFMJ_SleepWaitEmptyThreadSlot < 0)
                {
                    ValMsg = ValMsg + string.Format(ValMsgFmt, "914", "2", "RFMJ_SleepWaitEmptyThreadSlot!\n");
                }
                // How long to wait for thread to start (ms)
                Gp.ReadParametersSpecificId(Operator, "914", "3", "", "");
                RFMJ_StartWorkerThreadTimeout = (int)Gp.Amount;
                if (RFMJ_StartWorkerThreadTimeout < 0)
                {
                    ValMsg = ValMsg + string.Format(ValMsgFmt, "914", "3", "RFMJ_StartWorkerThreadTimeout!\n");
                }
                // ThreadWatch iteration interval (ms)
                Gp.ReadParametersSpecificId(Operator, "914", "4", "", "");
                RFMJ_ThreadMonitorInterval = (int)Gp.Amount;
                if (RFMJ_ThreadMonitorInterval < 0)
                {
                    ValMsg = ValMsg + string.Format(ValMsgFmt, "914", "4", "RFMJ_ThreadMonitorInterval!\n");
                }
                // How long to wait for threads to finish after Abort_Abort is set
                Gp.ReadParametersSpecificId(Operator, "914", "5", "", "");
                RFMJ_ThreadAbortWait = (int)Gp.Amount;
                if (RFMJ_ThreadAbortWait < 0)
                {
                    ValMsg = ValMsg + string.Format(ValMsgFmt, "914", "5", "RFMJ_ThreadAbortWait!\n");
                }
                // Max Life span of thread (in seconds)
                Gp.ReadParametersSpecificId(Operator, "914", "6", "", "");
                RFMJ_MaxThreadLifeSpan = (int)Gp.Amount;
                if (RFMJ_MaxThreadLifeSpan <= 0)
                {
                    ValMsg = ValMsg + string.Format(ValMsgFmt, "914", "6", "RFMJ_MaxThreadLifeSpan!\n");
                }
                // Refresh Interval (in seconds) - for the file list in monitored dir
                Gp.ReadParametersSpecificId(Operator, "914", "7", "", "");
                RFMJ_RefreshInterval = (int)Gp.Amount;
                if (RFMJ_RefreshInterval <= 0)
                {
                    ValMsg = ValMsg + string.Format(ValMsgFmt, "914", "7", "RFMJ_RefreshInterval!\n");
                }
                // Stored Procedure for Parsing
                Gp.ReadParametersSpecificId(Operator, "914", "8", "", "");
                Rfmj_StoredProcedure = Gp.OccuranceNm;
                if (string.IsNullOrEmpty(Rfmj_StoredProcedure))
                {
                    ValMsg = ValMsg + string.Format(ValMsgFmt, "914", "8", "Invalid name of Parser Stored Procedure!\n");
                }

                // Check if any of the above caused an error...
                if (!string.IsNullOrEmpty(ValMsg))
                {
                    msg = string.Format("The program encountered invalid parameters. Details:\n{0}", ValMsg);
                    EventLogging.MessageOut(msg, EventLogEntryType.Error);
                    // the ConsoleDisplay thread is not started yet, so we display the message ourselves
                    if (Environment.UserInteractive)
                        Console.WriteLine(msg);
                    return;
                }

                #endregion

                IsSuccess = true;
                errorMsg = "";

            }
            catch (Exception ex)
            {
                string exMsg = ex.Message;
                Exception ex1 = ex.InnerException;
                while (ex1 != null)
                {
                    exMsg += "\n" + ex1.Message;
                    ex1 = ex1.InnerException;
                }
                IsSuccess = false;
                errorMsg = exMsg;
            }
        }
        #endregion
    }
}
