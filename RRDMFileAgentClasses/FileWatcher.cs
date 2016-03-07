using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
// using System.Configuration;
using System.IO;


namespace RRDMFileAgentClasses
{
    public static class RRDMFileMonitor
    {
        /// <summary>
        /// Creates the FileSystemWatcher
        /// On return the caller must add the watcher handlers and enable monitoring..
        /// </summary>
        /// <param name="SourceDirectory"></param>
        /// <param name="FileNameMask"></param>
        /// <returns>FileSystemWatcher</returns>
        public static FileSystemWatcher CreateFileMonitor(string SourceDirectory, string FileNameMask)
        {
            string RRDMAgentPath; // { get; set; }
            string RRDMAgentMask; // { get; set; }

            FileSystemWatcher FileMon = new FileSystemWatcher();

            try
            {
                RRDMAgentPath = SourceDirectory.Trim();
                RRDMAgentMask = FileNameMask.Trim();

                //Set the path for the FileSystemWatcher to monitor.
                FileMon.Path = RRDMAgentPath;

                //Set the file type filter for the FileSystemWatcher to monitor.
                FileMon.Filter = RRDMAgentMask;

                //FileSystemWatcher should not monitor subdirectories.
                FileMon.IncludeSubdirectories = false;

                //Set the NotifyFilters for raising events.
                FileMon.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size;

                return (FileMon);
            }
            catch (Exception ex)
            {
                // ToDo
                string msg = ex.Source.ToString();
                return (null);
            }

        }

    }
}
