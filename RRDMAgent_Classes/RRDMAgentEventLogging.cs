using System;
using System.Diagnostics;

namespace RRDMAgent_Classes
{
    public class AgentEventLogging
    {
        public static void LogException(string eventSource, EventLogEntryType LogType, Exception ex)
        {
            // Get message text from excepion

            string exMsg = "MESSAGE: " + ex.Message;
            Exception innerEx = ex.InnerException;
            while (innerEx != null)
            {
                exMsg = exMsg + Environment.NewLine + "INNER: " + innerEx.Message;
            }

            // Create an EventLog instance and assign its source.
            EventLog Log = new EventLog();
            Log.Source = eventSource;


            // Format the message to be writen
            // string Msg = DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim();
            string Msg = DateTime.Now.ToString() + ": " + exMsg;

            try
            {
                if (EventLog.SourceExists(eventSource))
                {
                    EventLog.WriteEntry(eventSource, Msg, LogType);
                }
                Log.Dispose();
            }
            catch (Exception exc)
            {
                string s = exc.Source.ToString();
                // ToDo: Handle exception
            }
        }

        public static void WriteEventLog(string eventSource, EventLogEntryType LogType, string Message)
        {
            // Create an EventLog instance and assign its source.
            EventLog Log = new EventLog();
            Log.Source = eventSource;

            // Format the message to be writen
            string Msg = DateTime.Now.ToString() + ": " + Message;

            try
            {
                if (EventLog.SourceExists(eventSource))
                {
                    EventLog.WriteEntry(eventSource, Msg, LogType);
                }
                Log.Dispose();
            }
            catch (Exception exc)
            {
                string s = exc.Source.ToString();
                // ToDo: Handle exception
            }

        }

        #region RecordEventMsg
        public static void RecordEventMsg(string msg, EventLogEntryType type)
        {
            AgentEventLogging.WriteEventLog("RRDMAgent", type, msg);
            if (Environment.UserInteractive)
                Console.WriteLine(msg);
        }
        public static void RecordEventMsg(string msg, EventLogEntryType type, bool SkipWinEvent)
        {
            if (!SkipWinEvent)
                AgentEventLogging.WriteEventLog("RRDMAgent", type, msg);
            if (Environment.UserInteractive)
                Console.WriteLine(msg);
        }
        #endregion

    }
}
