using RRDM4ATMs;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequenceJournal
{
    class Program
    {
        static void Main(string[] args)
        {
            long fileSize = 0;
            int LineCount = 0;
            string fileFullPath;

            if (args.Length != 1)
            {
                Console.WriteLine("Processing stopped!\nPlease enter the the file name to process...");
                Console.WriteLine("Press enter to retry...");
                Console.ReadLine();
                return;
            }


            fileFullPath = args[0];
            if (File.Exists(fileFullPath))
            {
                string jlnFullPathName;
                RRDMJournalReadTxns_Text_Class Jrt = new RRDMJournalReadTxns_Text_Class();
                jlnFullPathName = Jrt.ConvertJournal(fileFullPath); // Converted File 
                LineCount = Jrt.LineCounter;

                Console.WriteLine("Input File : {0}", fileFullPath);
                Console.WriteLine("Output File: {0}", jlnFullPathName);
                Console.WriteLine("No of Lines: {0}", LineCount);
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Input File {0} not found!...", fileFullPath);
                Console.WriteLine("Press enter to retry...");
                Console.ReadLine();
            }
            return;

        }
    }
}
