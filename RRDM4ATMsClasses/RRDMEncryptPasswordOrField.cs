using System;

using System.Text;
using System.Security.Cryptography;

using RRDMEncrypt;

namespace RRDM4ATMs
{
    public class RRDMEncryptPasswordOrField : Logger
    {
        public RRDMEncryptPasswordOrField() : base() { }
        // Declarations 

        //string connectionString = ConfigurationManager.ConnectionStrings

        //
        // Generate Password 
        //

        public string GetUniqueKey(int minSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[minSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(minSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

     
        //public string CreatePassword(int length)
        //{
        //    StringBuilder res = new StringBuilder();
        //    try
        //    {
        //        const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
               
        //        Random rnd = new Random();
        //        while (0 < length--)
        //        {
        //            res.Append(valid[rnd.Next(valid.Length)]);
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        CatchMethod(ex);

        //    }

        //    return res.ToString();
        //}
        //////
        ////// ENCRYPT Field 
        //////
        string WField;
        public string EncryptField(string InField)
        {
            
            try
            {
                //Password parameters
                //
                string passPhrase = "Pas5pr@seAthos11Aramis11Porthos11";
                string initVectorOrg = "@1B09FgHH8#"; // must be up to 16 bytes
                byte[] initVector = Encoding.ASCII.GetBytes(initVectorOrg);
                string initVectorTable = string.Empty;
                string EncryptedString = string.Empty;
                string DecryptedString = string.Empty;

                RRDM_Encryption cEncryption = new RRDM_Encryption(passPhrase, Convert.ToBase64String(initVector));

                WField = InField;

                WField = cEncryption.Encrypt(this.WField);
            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }
        
            return WField;
        }
        //////
        ////// DECRYPT PASSWORD
        //////
        public string DecryptField(string InField)
        {
           
            try
            {
                //Password parameters
                //
                string passPhrase = "Pas5pr@seAthos11Aramis11Porthos11";
                string initVectorOrg = "@1B09FgHH8#"; // must be up to 16 bytes
                byte[] initVector = Encoding.ASCII.GetBytes(initVectorOrg);
                string initVectorTable = string.Empty;
                string EncryptedString = string.Empty;
                string DecryptedString = string.Empty;

                RRDM_Encryption cEncryption = new RRDM_Encryption(passPhrase, Convert.ToBase64String(initVector));

                WField = InField;

                WField = cEncryption.Decrypt(this.WField);
            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }

            return WField;
        }

        //////
        ////// Check Input Field to be same as Decrypted
        //////
        bool Matched;
        string DecryptedPassword;

        public object Path { get; private set; }

        public bool CheckPassword(string InField, string EncryptedField)
        {
            try
            {
                // Alecos
                //DecryptedPassword = DecryptField(EncryptedField);

                //if (InField == DecryptedPassword)
                //{
                //    Matched = true; 
                //}
                //else
                //{
                //    Matched = false;  
                //}

                DecryptedPassword = DecryptField(EncryptedField);
                Matched  = RRDM_Encryption.VerifyHash(InField, "SHA256", DecryptedPassword);
            }
            catch (Exception ex)
            {

                CatchDetails(ex);

            }

            return Matched;
        }




    }
}
