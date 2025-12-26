using System;
using RRDMEncrypt;

namespace RRDM_Verification
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting RRDM_Encryption Verification...");
            bool allTestsPassed = true;

            // Test 1: ComputeHash Default (SHA512)
            try
            {
                Console.WriteLine("\n[Test 1] ComputeHash Default (SHA512)");
                string plainText = "Hello World";
                string hash = RRDM_Encryption.ComputeHash(plainText, null, null);
                Console.WriteLine($"Hash: {hash.Substring(0, 20)}...");
                
                if (RRDM_Encryption.VerifyHash(plainText, "SHA512", hash))
                {
                    Console.WriteLine("SUCCESS: Verification passed.");
                }
                else
                {
                    Console.WriteLine("FAILURE: Verification failed.");
                    allTestsPassed = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILURE: Exception: {ex.Message}");
                allTestsPassed = false;
            }

            // Test 2: ComputeHash Explicit MD5
            try
            {
                Console.WriteLine("\n[Test 2] ComputeHash Explicit MD5");
                string plainText = "Hello MD5";
                string hash = RRDM_Encryption.ComputeHash(plainText, "MD5", null);
                Console.WriteLine($"Hash: {hash.Substring(0, 20)}...");
                
                if (RRDM_Encryption.VerifyHash(plainText, "MD5", hash))
                {
                    Console.WriteLine("SUCCESS: Verification passed.");
                }
                else
                {
                    Console.WriteLine("FAILURE: Verification failed.");
                    allTestsPassed = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILURE: Exception: {ex.Message}");
                allTestsPassed = false;
            }

            // Test 3: Encryption/Decryption Roundtrip
            try
            {
                Console.WriteLine("\n[Test 3] Encryption/Decryption Roundtrip");
                string passPhrase = "SecretPassphrase";
                string initVector = "1234567890123456"; // 16 bytes for AES
                RRDM_Encryption encryptor = new RRDM_Encryption(passPhrase, initVector);

                string originalText = "Sensitive Data 123";
                string encrypted = encryptor.Encrypt(originalText);
                Console.WriteLine($"Encrypted: {encrypted}");

                string decrypted = encryptor.Decrypt(encrypted);
                Console.WriteLine($"Decrypted: {decrypted}");

                if (originalText == decrypted)
                {
                    Console.WriteLine("SUCCESS: Decrypted text matches original.");
                }
                else
                {
                    Console.WriteLine($"FAILURE: Mismatch. Expected '{originalText}', got '{decrypted}'");
                    allTestsPassed = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILURE: Exception: {ex.Message}");
                allTestsPassed = false;
            }

             // Test 4: Encryption/Decryption Roundtrip with Salt
            try
            {
                Console.WriteLine("\n[Test 4] Encryption/Decryption Roundtrip with Salt");
                string passPhrase = "SecretPassphraseSalt";
                string initVector = "1234567890123456"; 
                 // passPhrase, initVector, minSaltLen, maxSaltLen, keySize, hashAlgorithm, saltValue, passwordIterations
                RRDM_Encryption encryptor = new RRDM_Encryption(passPhrase, initVector, 4, 8, 256, "SHA256", "SaltValue", 2);

                string originalText = "Salted Data 456";
                string encrypted = encryptor.Encrypt(originalText);
                Console.WriteLine($"Encrypted: {encrypted}");

                string decrypted = encryptor.Decrypt(encrypted);
                Console.WriteLine($"Decrypted: {decrypted}");

                if (originalText == decrypted)
                {
                    Console.WriteLine("SUCCESS: Decrypted text matches original.");
                }
                else
                {
                    Console.WriteLine($"FAILURE: Mismatch. Expected '{originalText}', got '{decrypted}'");
                    allTestsPassed = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILURE: Exception: {ex.Message}");
                allTestsPassed = false;
            }

            Console.WriteLine("\n--------------------------------------------------");
            if (allTestsPassed)
            {
                Console.WriteLine("OVERALL RESULT: SUCCESS");
            }
            else
            {
                Console.WriteLine("OVERALL RESULT: FAILURE");
            }
        }
    }
}
