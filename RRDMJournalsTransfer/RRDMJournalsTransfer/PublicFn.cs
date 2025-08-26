using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RRDM4ATMs
{
    static class PublicFn
    {
        public static bool DeleteFiles(string sFolder)
        {
            // loop through each file in the target directory
            foreach (string oFile in Directory.GetFiles(sFolder))
            {
                // delete the file if possible...otherwise skip it
                try
                {
                    File.Delete(oFile);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

        public static string fnRight(string sString, int iLength)
        {
            return sString.Substring(sString.Length - iLength, iLength);
        }
    }
}
