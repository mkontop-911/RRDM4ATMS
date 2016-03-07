using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Runtime.Serialization;


namespace RRDMJTMClasses
{
    #region EventLogging
    public class EventLogging
    {
        public const string JTMConsoleDisplFile = @".\JTMTemp.Log";
        private static object FileLock = new object();

        public static void WriteEventLog(string eventSource, EventLogEntryType LogType, Exception ex)
        {
            // Create an EventLog instance and assign its source.
            EventLog RLog = new EventLog();
            RLog.Log = "RRDMSolutions";
            RLog.Source = eventSource;

            // Format the message to be writen
            string Msg = string.Format("{0}: Exception Source: {1} --> {2}",
                DateTime.Now.ToString(),
                ex.Source.ToString().Trim(),
                ex.Message);

            try
            {
                if (EventLog.SourceExists(eventSource))
                {
                    RLog.WriteEntry(Msg, LogType);
                }
                RLog.Dispose();
            }
            catch
            {
                RLog.Dispose();
            }
        }

        public static void WriteEventLog(string eventSource, EventLogEntryType LogType, string Message)
        {
            // Create an EventLog instance and assign its source.
            EventLog RLog = new EventLog();
            RLog.Source = eventSource;
            RLog.Log = "RRDMSolutions";

            // Format the message to be writen
            //string Msg = DateTime.Now.ToString() + ": " + Message;
            string Msg = string.Format("{0}: --> {1}", DateTime.Now.ToString("yyyyMMdd-HHmmss.ffffff"), Message);
            try
            {
                if (EventLog.SourceExists(eventSource))
                {
                    RLog.WriteEntry(Msg, LogType);
                }
                RLog.Dispose();
            }
            catch
            {
                RLog.Dispose();
            }

        }

        public static void MessageOut(string EventSource, string msg, EventLogEntryType Type)
        {
            string formattedMsg = msg + "\n";
            WriteEventLog(EventSource, Type, msg);

            if (Environment.UserInteractive)
            {
                if (JTMThreadRegistry.ConsoleDisplayLock != null) 
                {
                    lock (JTMThreadRegistry.ConsoleDisplayLock)
                    {
                        File.AppendAllText(JTMConsoleDisplFile, formattedMsg);
                    }
                }
                else // not yet initialized?
                {
                    File.AppendAllText(JTMConsoleDisplFile, formattedMsg);
                }
            }
#if DEBUG
            WriteDebugInfo(msg);
#endif
        }

#if DEBUG
        #region WriteDebugInfo ..
        public static void WriteDebugInfo(string msg)
        {
            string path = @".\JTMWorker.txt";
            string msgout = string.Format("\n[ {0} ]: {1}", DateTime.Now.TimeOfDay, msg); //.Replace("\n", "-"));
            lock (FileLock)
            {
                File.AppendAllText(path, msgout);
            }
        }
        #endregion
#endif

        public static void DisplayFileContents()
        {
            if (File.Exists(JTMConsoleDisplFile))
            {
                string[] lines = File.ReadAllLines(JTMConsoleDisplFile);
                Console.WriteLine("\n");
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
                File.Delete(JTMConsoleDisplFile);
            }
        }

        public static void ClearFileContents()
        {
            if (File.Exists(JTMConsoleDisplFile)) 
               File.Delete(JTMConsoleDisplFile);
        }

    }
    #endregion

    #region JTMCustomException
    [Serializable]
    public class JTMCustomException : Exception
    {
        private int _JTMcode;
        private string _JTMsrc;
        private string _JTMmsg;
        private bool _JTMFatal;
        public int JTMCode
        {
            get { return _JTMcode; }
            set { _JTMcode = value; }
        }

        public string JTMSource
        {
            get { return _JTMsrc; }
            set { _JTMsrc = value; }
        }

        public string JTMMessage
        {
            get { return _JTMmsg; }
            set { _JTMmsg = value; }
        }

        public bool JTMFatal
        {
            get { return _JTMFatal; }
            set { _JTMFatal = value; }
        }

        public JTMCustomException()
            : base()
        {
        }

        public JTMCustomException(string message)
            : base(message)
        {
        }

        public JTMCustomException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public JTMCustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public JTMCustomException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }

        protected JTMCustomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
    #endregion
}
