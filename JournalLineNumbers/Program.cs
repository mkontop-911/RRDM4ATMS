using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalLineNumbers
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName_In;
            string fileName_Out;
            string fileDir;
            string msg;
            ConsoleKeyInfo cki;

            #region Read parameters passed to the program
            if (args.Length != 1)
            {
                msg = "Processing stopped!\nYou must pass a file name as parameter\nPress OK to exit...";
                Console.WriteLine(msg);
                Console.ReadLine();
                return;
            }
            string fullFileName = fileName_In = args[0];
            #endregion

            if (!File.Exists(fullFileName))
            {
                msg = string.Format("Processing stopped!\n\rThe file [{0}] does not exist!\n\rPress ENTER to exit...", fullFileName);
                Console.WriteLine(msg);
                Console.ReadLine();
                return;
            }
            fileDir = Path.GetDirectoryName(fullFileName);
            string fileOut = Path.GetFileName(fullFileName);
            fileOut += ".jln";
            fileName_Out = Path.Combine(fileDir, fileOut);

            msg = string.Format("\n\rConverting file \n\r[{0}] to \n\r[{1}]!\n\rPress 'Y' to proceed... any other key to abort", fileName_In, fileName_Out);
            Console.WriteLine(msg);
            cki = Console.ReadKey();

            if (cki.Key.ToString() != "Y" && cki.Key.ToString() != "y")
            {
                return;
            }

            Console.WriteLine("Started: {0}", DateTime.Now.ToString());
            // FileStream fs = null;
            int lineNumber = 1000000;

            // Read in lines from file.
            string[] lineArrayIn = File.ReadAllLines(fileName_In);
            int lineCount = lineArrayIn.Length;
            string[] lineArrayOut = new string[lineCount];
            
            for (int i=0;i<lineCount;i++)
            { 
                lineNumber++;
                lineArrayOut[i] = string.Format("{0,0000000} {1}", lineNumber, lineArrayIn[i]);
                // Console.WriteLine(lineArrayOut[i]);
            }
            File.WriteAllLines(fileName_Out,lineArrayOut, Encoding.GetEncoding(1253));

            Console.WriteLine("Finished: {0}", DateTime.Now.ToString());
            Console.ReadLine();
        }
    }
}

