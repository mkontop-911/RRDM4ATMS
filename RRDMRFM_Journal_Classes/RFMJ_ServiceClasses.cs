using System;
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

// using ClassUtilities;

namespace RRDMRFM_Journal_Classes
{
    #region Enumeration of the states a worker thread can be in
    public enum StatusOfThread
    {
        Unknown,    // Reserved for queries with invalid Index
        Available,  // Constructor sets the array slots to this
        Reserved,   // Set when allocating a free slot in ThreadArray
        Running,
        Stopping,   // Set for normal termination
        Canceled,   // Set when thread stops because of Abort_Abort
        Aborting,   // Set after an exception
        Finished
    }
    #endregion

    #region Enumeration of Action results
    public enum RfmjResult
    {
        Success,
        Error
    }
    #endregion

    #region RFMJ Action Stages Class
    // (The stage/step the action is in)
    public static class RfmjActionStage
    {
        public const int Const_Intercepted = 0;
        public const int Const_WorkInProgress = 1;
        public const int Const_Step_1_InProgress = 2;
        public const int Const_Step_2_InProgress = 3;
        public const int Const_Step_3_InProgress = 4;
        public const int Const_Step_4_InProgress = 5;
        public const int Const_Step_4_Finished = 6;
        public const int Const_Step_3_Finished = 7;
        public const int Const_Step_2_Finished = 8;
        public const int Const_Step_1_Finished = 9;
        public const int Const_Aborted = 98;
        public const int Const_Finished = 99;


        private struct RfmjStageStruct
        {
            public int num;
            public string message;
            public RfmjStageStruct(int num, string message)
            {
                this.num = num;
                this.message = message;
            }
        }

        private static RfmjStageStruct[] STAGE_LIST = new RfmjStageStruct[]
        {
            new RfmjStageStruct(Const_Intercepted, "Intercepted"),
            new RfmjStageStruct(Const_WorkInProgress, "Processing Started"),
            new RfmjStageStruct(Const_Step_1_InProgress, "Worker Thread In Progress"),
            new RfmjStageStruct(Const_Step_1_Finished, "Worker Thread Finished"),
            new RfmjStageStruct(Const_Step_2_InProgress, "Preparation In Progress"),
            new RfmjStageStruct(Const_Step_2_Finished, "Preparation Finished"),
            new RfmjStageStruct(Const_Step_3_InProgress, "Journal Processing in Progress"),
            new RfmjStageStruct(Const_Step_3_Finished, "Journal Processsing Finished"),
            new RfmjStageStruct(Const_Step_4_InProgress, "Parsing in Progress"),
            new RfmjStageStruct(Const_Step_4_Finished, "Parsing Finished"),
            new RfmjStageStruct(Const_Aborted, "Aborted Due To Error"),
            new RfmjStageStruct(Const_Finished, "Processing Finished")
        };

        public static string getStageFromNumber(int stageNum)
        {
            foreach (RfmjStageStruct elem in STAGE_LIST)
            {
                if (elem.num == stageNum) return elem.message;
            }
            return "Stage: Unknown, " + stageNum;
        }
    }
    #endregion

    public static class RfmjFunctions
    {
        private static Object LockObj = new Object();

        #region Generate Transaction ID (last 8 characters od a GUID)
        public static string GenerateNewID()
        {
            Guid g;
            g = Guid.NewGuid();

            string g1 = g.ToString("N");
            string trxID = g1.Substring(24);
            return (trxID);
        }
        #endregion

        #region // FileHASH
        //public class FileHASH
        //{
        //    // Compute the file's hash.
        //    public static byte[] GetHashSha256(string filename)
        //    {
        //        byte[] hashval;
        //        // The cryptographic service provider.
        //        SHA256 Sha256 = SHA256.Create();

        //        using (FileStream stream = File.OpenRead(filename))
        //        {
        //            hashval = Sha256.ComputeHash(stream);
        //            stream.Close();
        //            return (hashval);
        //        }
        //    }

        //    // Return a byte array as a sequence of hex values.
        //    public static string BytesToString(byte[] bytes)
        //    {
        //        string result = "";
        //        foreach (byte b in bytes) result += b.ToString("x2");
        //        return result;
        //    }

        //}
        #endregion

        #region // Encrypt/Decrypt
        //public static string EncryptString(string InStr)
        //{
        //    string WField = "";
        //    try
        //    {
        //        string passPhrase = "KeiosKreio$IapetosT1tanesMnemosyn";
        //        string initVectorOrg = "#2xGT93PXeQ";
        //        byte[] initVector = Encoding.ASCII.GetBytes(initVectorOrg);
        //        string initVectorTable = string.Empty;
        //        string EncryptedString = string.Empty;
        //        string DecryptedString = string.Empty;

        //        eBizEncryption cEncryption = new eBizEncryption(passPhrase, Convert.ToBase64String(initVector));

        //        WField = cEncryption.Encrypt(InStr);
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = ex.Message;
        //    }

        //    return WField;
        //}
        //public static string DecryptString(string InStr)
        //{
        //    string WField = "";

        //    try
        //    {
        //        string passPhrase = "KeiosKreio$IapetosT1tanesMnemosyn";
        //        string initVectorOrg = "#2xGT93PXeQ";
        //        byte[] initVector = Encoding.ASCII.GetBytes(initVectorOrg);
        //        string initVectorTable = string.Empty;
        //        string EncryptedString = string.Empty;
        //        string DecryptedString = string.Empty;

        //        eBizEncryption cEncryption = new eBizEncryption(passPhrase, Convert.ToBase64String(initVector));


        //        WField = cEncryption.Decrypt(InStr);
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = ex.Message;
        //    }

        //    return WField;
        //}
        #endregion

        #region Sanitize file path (replace invalid charactes with '_')
        public static string SanitizeFilePath(string filename)
        {
            lock(LockObj)
            {
                return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
            }
        }
        #endregion


        #region RfmjCustomException
        [Serializable]
        public class RfmjCustomException : Exception
        {
            public int cexCode { get; set; }
            public string cexSource { get; set; }
            public string cexMessage { get; set; }
            public bool cexFatal { get; set; }

            public RfmjCustomException() : base() { }
            public RfmjCustomException(string message) : base(message) { }
            public RfmjCustomException(string format, params object[] args) : base(string.Format(format, args)) { }
            public RfmjCustomException(string message, Exception innerException) : base(message, innerException) { }
            public RfmjCustomException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
            protected RfmjCustomException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }
        #endregion
    }
}
