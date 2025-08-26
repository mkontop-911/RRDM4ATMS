using System;
using System.Diagnostics;

namespace RRDMRFM_Journal_Classes
{
    public class EventLogging
    {
        public static void WriteEventLog(EventLogEntryType LogType, Exception ex)
        {
            // Create an EventLog instance and assign its source.
            EventLog Log = new EventLog();
            Log.Source = "RRDMFileMonitor";

            // Format the message to be writen
            string Msg = DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim();

            try
            {
                if (EventLog.SourceExists("RRDMFileMonitor"))
                {
                    EventLog.WriteEntry("RRDMFileMonitor", Msg, LogType);
                }
                Log.Dispose();
            }
            catch (Exception exc)
            {
                string s = exc.Source.ToString();
                // ToDo: Handle exception
            }
        }

        public static void WriteEventLog(EventLogEntryType LogType, string Message)
        {
            // Create an EventLog instance and assign its source.
            EventLog Log = new EventLog();
            Log.Source = "RRDMFileMonitor";

            // Format the message to be writen
            string Msg = DateTime.Now.ToString() + ": " + Message;

            try
            {
                if (EventLog.SourceExists("RRDMFileMonitor"))
                {
                    EventLog.WriteEntry("RRDMFileMonitor", Msg, LogType);
                }
                Log.Dispose();
            }
            catch (Exception exc)
            {
                string s = exc.Source.ToString();
                // ToDo: Handle exception
            }

        }

        public static void RecordEventMsg(string msg, EventLogEntryType errType)
        {
            // WriteEventLog(RfmjServer.RfmjOp.RfmjEventSource, errType, msg);
            WriteEventLog(errType, msg);
        }

        public static void MessageOut (string Message, EventLogEntryType LogType)
        {
            WriteEventLog(LogType, Message);
        }
    }
}
