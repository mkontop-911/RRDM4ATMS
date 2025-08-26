using System;

using System.Diagnostics;

namespace RRDMEventSourceInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exitProg = false;

            Console.WriteLine("RRDMEventLogInstaller started....\n");
            Console.WriteLine("It will register the following EventSources in the RRDMSolutions event log:\nRRDMFileMonitor\nRRDMAgent\nRRDMJTM");
            Console.WriteLine("The first time this program must run with Administrator priviledges...\n");
            Console.WriteLine("Press ENTER to continue or CTRL+C to exit...");
            Console.ReadLine();

            // An event log source cannot be used immediately after creation.
            // It should be created prior to executing the application that uses the source.
            // Execute this program a second time to test the new event source.

            try
            {
                if (!EventLog.SourceExists("RRDMAgent")) // Create the source, if it does not already exist.
                {
                    try
                    {
                        // EventLog = "RRDMSolutions"
                        // EventSource = "RRDMAgent"
                        EventLog.CreateEventSource("RRDMAgent", "RRDMSolutions");
                        Console.WriteLine("\nCreated EventSource 'RRDMAgent' in EventLog 'RRDMSolutions'\n");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    exitProg = true;
                }

                if (!EventLog.SourceExists("RRDMFileMonitor")) // Create the source, if it does not already exist.
                {
                    try
                    { 
                    // EventLog = "RRDMSolutions"
                    // EventSource = "RRDMFileMonitor"
                    EventLog.CreateEventSource("RRDMFileMonitor", "RRDMSolutions");
                    Console.WriteLine("\nCreated EventSource 'RRDMFileMonitor' in EventLog 'RRDMSolutions'\n");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    exitProg = true;
                }

                if (!EventLog.SourceExists("RRDMJTM")) // Create the source, if it does not already exist.
                {
                    try
                    {
                        // EventLog = "RRDMSolutions"
                        // EventSource = "RRDMJTM"
                        EventLog.CreateEventSource("RRDMJTM", "RRDMSolutions");
                        Console.WriteLine("\nCreated EventSource 'RRDMJTM' in EventLog 'RRDMSolutions'\n");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    exitProg = true;
                }

                // The source is created.  Exit the application to allow it to be registered.
                if (exitProg)
                {
                    Console.WriteLine("Exiting, execute the application a second time to create a test entry....\n");
                    Console.WriteLine("(Your may need to restart this computer for the new EventLog to appear in the EventViewer console...)");
                    Console.ReadLine();
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            // Create an EventLog instance and assign its source.
            EventLog myLog = new EventLog();
            // Write an informational entry to the event log.    
            myLog.Source = "RRDMFileMonitor";
            myLog.WriteEntry("Test entry for RRDMFileMonitor source in RRDMSolutions event log.");

            myLog.Source = "RRDMAgent";
            myLog.WriteEntry("Test entry for RRDMAgent source in RRDMSolutions event log.");

            myLog.Source = "RRDMJTM";
            myLog.WriteEntry("Test entry for RRDMJTM source in RRDMSolutions event log.");

            Console.WriteLine("Test entries have been writen in the 'RRDMSolutions' event log; Their sources are 'RRDMFileMonitor', 'RRDMAgent' and 'RRDMJTM. ");
            Console.WriteLine("Press Enter to end the program....");
            Console.ReadLine();

        }
    }
}
