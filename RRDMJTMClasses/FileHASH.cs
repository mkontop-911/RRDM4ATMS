
//
using System.IO;
using System.Security.Cryptography;

namespace RRDMJTMClasses
{
    public class FileHASH
    { 
        // Compute the file's hash.
        public static byte[] GetHashSha256(string filename)
        {
            byte[] hashval;
            // The cryptographic service provider.
            SHA256 Sha256 = SHA256.Create();

            using (FileStream stream = File.OpenRead(filename))
            {
                hashval = Sha256.ComputeHash(stream);
                stream.Close();
                return (hashval);
            }
            // Exceptions will propagete upstream
        }

        // Return a byte array as a sequence of hex values.
        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }

    }

}
