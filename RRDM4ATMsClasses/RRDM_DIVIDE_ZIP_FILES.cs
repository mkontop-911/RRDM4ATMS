
using System;
using System.IO;
using System;
using System.Text;
using System.Xml;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Linq;


namespace RRDM4ATMs
{
    public class RRDM_DIVIDE_ZIP_FILES : Logger
    {
        public RRDM_DIVIDE_ZIP_FILES() : base() { }
        // Spilt File 
        public void SplitZip(string inputZipPath, int partSizeMB)
        {
            if (!File.Exists(inputZipPath))
            {
                //Console.WriteLine("Input ZIP file not found.");
                return;
            }

            const int bufferSize = 1024 * 1024;
            var partSizeBytes = partSizeMB * 1024 * 1024;
            var buffer = new byte[bufferSize];

            System.IO.FileStream input = new FileStream(inputZipPath, FileMode.Open, FileAccess.Read);
            int partNumber = 0;
            long totalBytesRead = 0;

            while (totalBytesRead < input.Length)
            {
                string partFileName = $"{inputZipPath}.part{partNumber:D4}";

                System.IO.FileStream output = new FileStream(partFileName, FileMode.Create, FileAccess.Write);

                long bytesWritten = 0;
                while (bytesWritten < partSizeBytes && totalBytesRead < input.Length)
                {
                    int bytesToRead = (int)Math.Min(bufferSize, Math.Min(partSizeBytes - bytesWritten, input.Length - totalBytesRead));
                    int bytesRead = input.Read(buffer, 0, bytesToRead);
                    if (bytesRead == 0) break;

                    output.Write(buffer, 0, bytesRead);
                    bytesWritten += bytesRead;
                    totalBytesRead += bytesRead;
                }

                //Console.WriteLine($"Created: {partFileName} ({bytesWritten} bytes)");
                partNumber++;
                output.Close();
            }
            input.Close();
            //  Console.WriteLine("Split completed.");
            MessageBox.Show("The Split File has Been completed" + Environment.NewLine
                + "The Naumber of created files are " + (partNumber - 1).ToString()
                ); 
        }
        // Reconstruct file 
        public void ReconstructZip(string outputZipPath, string partPrefix)
        {
            var partFiles = Directory.GetFiles(Path.GetDirectoryName(outputZipPath) ?? ".", $"{Path.GetFileName(partPrefix)}.part*")
                                     .OrderBy(f => f)
                                     .ToList();

            if (!partFiles.Any())
            {
                //Console.WriteLine("No part files found to reconstruct.");
                return;
            }

            System.IO.FileStream output = new FileStream(outputZipPath, FileMode.Create, FileAccess.Write);
            var buffer = new byte[1024 * 1024];

            foreach (var part in partFiles)
            {
                System.IO.FileStream input = new FileStream(part, FileMode.Open, FileAccess.Read);
                int bytesRead;
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, bytesRead);
                }
                input.Close();
                //Console.WriteLine($"Merged: {part}");
            }
            output.Close();

            MessageBox.Show("The reconstraction of files has been completed" + Environment.NewLine
              //+ "The Naumber of created files are " + (partNumber - 1).ToString()
              );
            //Console.WriteLine("Reconstruction completed.");
        }
    


    }
}





//


        
        


    


