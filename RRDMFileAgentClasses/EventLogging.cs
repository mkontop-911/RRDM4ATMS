using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace RRDMFileAgentClasses
{
    public class EventLogging
    {
        public static void WriteEventLog(string eventSource, EventLogEntryType LogType, Exception ex)
        {
            // Create an EventLog instance and assign its source.
            EventLog Log = new EventLog();
            Log.Source = eventSource;

            // Format the message to be writen
            string Msg = DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim();

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
                // ToDo
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
                // ToDo
            }

        }
    }
}
