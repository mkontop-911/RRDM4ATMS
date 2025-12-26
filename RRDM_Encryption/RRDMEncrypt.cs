using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace RRDMEncrypt
{
    public class RRDM_Encryption
    {
        #region "SHA512 and back only encrypt and Verify"

        /// <summary>
        /// Generates a hash for the given plain text value and returns a
        /// base64-encoded result. Before the hash is computed, a random salt
        /// is generated and appended to the plain text. This salt is stored at
        /// the end of the hash value, so it can be used later for hash
        /// verification.
        /// </summary>
        /// <param name="plainText"></param>
        ///  Plaintext value to be hashed. The function does not check whether
        /// this parameter is null.
        /// <param name="hashAlgorithm"></param>
        ///  Name of the hash algorithm. Allowed values are: "MD5", "SHA1",
        /// "SHA256", "SHA384", and "SHA512" (if any other value is specified
        /// MD5 hashing algorithm will be used). This value is case-insensitive.
        /// <param name="saltBytes"></param>
        /// Salt bytes. This parameter can be null, in which case a random salt
        /// value will be generated.
        /// <returns></returns>
        ///  Hash value formatted as a base64-encoded string.
        /// <remarks></remarks>
        public static string ComputeHash(string plainText, string hashAlgorithm, byte[] saltBytes)
        {

            // If salt is not specified, generate it on the fly.

            if ((saltBytes == null))
            {
                // Define min and max salt sizes.
                int minSaltSize = 0;
                int maxSaltSize = 0;

                minSaltSize = 4;
                maxSaltSize = 8;

                // Generate a random number for the size of the salt.
                Random random = default(Random);
                random = new Random();

                int saltSize = 0;
                saltSize = random.Next(minSaltSize, maxSaltSize);

                // Allocate a byte array, which will hold the salt.
                saltBytes = new byte[saltSize];

                // Initialize a random number generator.
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    // Fill the salt with cryptographically strong byte values.
                    rng.GetBytes(saltBytes);
                }
            }

            // Convert plain text into a byte array.
            byte[] plainTextBytes = null;
            plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Allocate array, which will hold plain text and salt.
            byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy plain text bytes into resulting array.
            int I = 0;
            for (I = 0; I <= plainTextBytes.Length - 1; I++)
            {
                plainTextWithSaltBytes[I] = plainTextBytes[I];
            }

            // Append salt bytes to the resulting array.
            for (I = 0; I <= saltBytes.Length - 1; I++)
            {
                plainTextWithSaltBytes[plainTextBytes.Length + I] = saltBytes[I];
            }

            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            HashAlgorithm hash = default(HashAlgorithm);

            // Make sure hashing algorithm name is specified.
            if ((hashAlgorithm == null))
            {
                hashAlgorithm = "";
            }

            // Initialize appropriate hashing algorithm class.
            switch (hashAlgorithm.ToUpper())
            {

                case "SHA1":
                    hash = SHA1.Create();

                    break;
                case "SHA256":
                    hash = SHA256.Create();

                    break;
                case "SHA384":
                    hash = SHA384.Create();

                    break;
                case "SHA512":
                    hash = SHA512.Create();

                    break;
                case "MD5":
                    hash = MD5.Create();
                    break;
                default:
                    hash = SHA512.Create();

                    break;
            }

            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = null;
            hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            byte[] hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (I = 0; I <= hashBytes.Length - 1; I++)
            {
                hashWithSaltBytes[I] = hashBytes[I];
            }

            // Append salt bytes to the result.
            for (I = 0; I <= saltBytes.Length - 1; I++)
            {
                hashWithSaltBytes[hashBytes.Length + I] = saltBytes[I];
            }

            // Convert result into a base64-encoded string.
            string hashValue = null;
            hashValue = Convert.ToBase64String(hashWithSaltBytes);

            // Return the result.
            return hashValue;
        }

        /// <summary>
        ///   Compares a hash of the specified plain text value to a given hash
        /// value. Plain text is hashed with the same salt value as the original
        /// hash.
        /// </summary>
        /// <param name="plainText"></param>
        /// Plain text to be verified against the specified hash. The function
        /// does not check whether this parameter is null.
        /// <param name="hashAlgorithm"></param>
        /// Name of the hash algorithm. Allowed values are: "MD5", "SHA1",
        /// "SHA256", "SHA384", and "SHA512" (if any other value is specified
        /// MD5 hashing algorithm will be used). This value is case-insensitive.
        /// <param name="hashValue"></param>
        /// Base64-encoded hash value produced by ComputeHash function. This value
        /// includes the original salt appended to it.
        /// <returns></returns>
        /// If computed hash mathes the specified hash the function the return
        /// value is true; otherwise, the function returns false.
        /// <remarks></remarks>
        public static bool VerifyHash(string plainText, string hashAlgorithm, string hashValue)
        {
            bool functionReturnValue = false;

            // Convert base64-encoded hash value into a byte array.
            byte[] hashWithSaltBytes = null;
            hashWithSaltBytes = Convert.FromBase64String(hashValue);

            // We must know size of hash (without salt).
            int hashSizeInBits = 0;
            int hashSizeInBytes = 0;

            // Make sure that hashing algorithm name is specified.
            if ((hashAlgorithm == null))
            {
                hashAlgorithm = "";
            }

            // Size of hash is based on the specified algorithm.
            switch (hashAlgorithm.ToUpper())
            {

                case "SHA1":
                    hashSizeInBits = 160;

                    break;
                case "SHA256":
                    hashSizeInBits = 256;

                    break;
                case "SHA384":
                    hashSizeInBits = 384;

                    break;
                case "SHA512":
                    hashSizeInBits = 512;

                    break;
                case "MD5":
                    hashSizeInBits = 128;
                    break;
                default:
                    // Default changed to SHA512
                    hashSizeInBits = 512;

                    break;
            }

            // Convert size of hash from bits to bytes.
            hashSizeInBytes = hashSizeInBits / 8;

            // Make sure that the specified hash value is long enough.
            if ((hashWithSaltBytes.Length < hashSizeInBytes))
            {
                functionReturnValue = false;
            }
            else
            {
                // Allocate array to hold original salt bytes retrieved from hash.
                byte[] saltBytes = new byte[hashWithSaltBytes.Length - hashSizeInBytes];

                // Copy salt from the end of the hash to the new array.
                int I = 0;
                for (I = 0; I <= saltBytes.Length - 1; I++)
                {
                    saltBytes[I] = hashWithSaltBytes[hashSizeInBytes + I];
                }

                // Compute a new hash string.
                string expectedHashString = null;
                expectedHashString = ComputeHash(plainText, hashAlgorithm, saltBytes);

                // If the computed hash matches the specified hash,
                // the plain text value must be correct.
                functionReturnValue = (hashValue == expectedHashString);
            }
            return functionReturnValue;
        }

        #endregion



        #region "SHA"
        //'http://www.obviex.com/samples/encryptionwithsalt.aspx

        // If hashing algorithm is not specified, use SHA-1.

        private static string DEFAULT_HASH_ALGORITHM = "SHA512";
        // If key size is not specified, use the longest 256-bit key.

        private static int DEFAULT_KEY_SIZE = 256;
        // Do not allow salt to be longer than 255 bytes, because we have only
        // 1 byte to store its length. 

        private static int MAX_ALLOWED_SALT_LEN = 255;
        // Do not allow salt to be smaller than 4 bytes, because we use the first
        // 4 bytes of salt to store its length. 

        private static int MIN_ALLOWED_SALT_LEN = 4;
        // Random salt value will be between 4 and 8 bytes long.
        private static int DEFAULT_MIN_SALT_LEN = MIN_ALLOWED_SALT_LEN;

        private static int DEFAULT_MAX_SALT_LEN = 8;
        // Use these members to save min and max salt lengths.
        private int minSaltLen = -1;

        private int maxSaltLen = -1;
        // These members will be used to perform encryption and decryption.
        private ICryptoTransform encryptor = null;

        private ICryptoTransform decryptor = null;
        // <summary>
        // Use this constructor if you are planning to perform encryption/
        // decryption with 256-bit key, derived using 1 password iteration,
        // hashing without salt, no initialization vector, electronic codebook
        // (ECB) mode, SHA-1 hashing algorithm, and 4-to-8 byte long salt.
        // </summary>
        // <param name="passPhrase">
        // Passphrase from which a pseudo-random password will be derived.
        // The derived password will be used to generate the encryption key.
        // Passphrase can be any string. In this example we assume that the
        // passphrase is an ASCII string. Passphrase value must be kept in
        // secret.
        // </param>
        // <remarks>
        // This constructor is not recommended because it does not use
        // initialization vector and uses the ECB cipher mode, which is less
        // secure than the CBC mode.
        // </remarks>
        public RRDM_Encryption(string passPhrase)
            : this(passPhrase, null)
        {
        }

        // <summary>
        // Use this constructor if you are planning to perform encryption/
        // decryption with 256-bit key, derived using 1 password iteration,
        // hashing without salt, cipher block chaining (CBC) mode, SHA-1
        // hashing algorithm, and 4-to-8 byte long salt.
        // </summary>
        // <param name="passPhrase">
        // Passphrase from which a pseudo-random password will be derived.
        // The derived password will be used to generate the encryption key.
        // Passphrase can be any string. In this example we assume that the
        // passphrase is an ASCII string. Passphrase value must be kept in
        // secret.
        // </param>
        // <param name="initVector">
        // Initialization vector (IV). This value is required to encrypt the
        // first block of plaintext data. For RijndaelManaged class IV must be
        // exactly 16 ASCII characters long. IV value does not have to be kept
        // in secret.
        // </param>
        public RRDM_Encryption(string passPhrase, string initVector)
            : this(passPhrase, initVector, -1)
        {
        }

        // <summary>
        // Use this constructor if you are planning to perform encryption/
        // decryption with 256-bit key, derived using 1 password iteration,
        // hashing without salt, cipher block chaining (CBC) mode, SHA-1 
        // hashing algorithm, and 0-to-8 byte long salt.
        // </summary>
        // <param name="passPhrase">
        // Passphrase from which a pseudo-random password will be derived.
        // The derived password will be used to generate the encryption key
        // Passphrase can be any string. In this example we assume that the
        // passphrase is an ASCII string. Passphrase value must be kept in
        // secret.
        // </param>
        // <param name="initVector">
        // Initialization vector (IV). This value is required to encrypt the
        // first block of plaintext data. For RijndaelManaged class IV must be
        // exactly 16 ASCII characters long. IV value does not have to be kept
        // in secret.
        // </param>
        // <param name="minSaltLen">
        // Min size (in bytes) of randomly generated salt which will be added at
        // the beginning of plain text before encryption is performed. When this
        // value is less than 4, the default min value will be used (currently 4
        // bytes).
        // </param>
        public RRDM_Encryption(string passPhrase, string initVector, int minSaltLen)
            : this(passPhrase, initVector, minSaltLen, -1)
        {
        }

        // <summary>
        // Use this constructor if you are planning to perform encryption/
        // decryption with 256-bit key, derived using 1 password iteration,
        // hashing without salt, cipher block chaining (CBC) mode, SHA-1
        // hashing algorithm. Use the minSaltLen and maxSaltLen parameters to
        // specify the size of randomly generated salt.
        // </summary>
        // <param name="passPhrase">
        // Passphrase from which a pseudo-random password will be derived.
        // The derived password will be used to generate the encryption key.
        // Passphrase can be any string. In this example we assume that the
        // passphrase is an ASCII string. Passphrase value must be kept in
        // secret.
        // </param>
        // <param name="initVector">
        // Initialization vector (IV). This value is required to encrypt the
        // first block of plaintext data. For RijndaelManaged class IV must be
        // exactly 16 ASCII characters long. IV value does not have to be kept
        // in secret.
        // </param>
        // <param name="minSaltLen">
        // Min size (in bytes) of randomly generated salt which will be added at
        // the beginning of plain text before encryption is performed. When this
        // value is less than 4, the default min value will be used (currently 4
        // bytes).
        // </param>
        // <param name="maxSaltLen">
        // Max size (in bytes) of randomly generated salt which will be added at
        // the beginning of plain text before encryption is performed. When this
        // value is negative or greater than 255, the default max value will be
        // used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
        // than the specified min value (which can be adjusted to default value),
        // salt will not be used and plain text value will be encrypted as is.
        // In this case, salt will not be processed during decryption either.
        // </param>
        public RRDM_Encryption(string passPhrase, string initVector, int minSaltLen, int maxSaltLen)
            : this(passPhrase, initVector, minSaltLen, maxSaltLen, -1)
        {
        }

        // <summary>
        // Use this constructor if you are planning to perform encryption/
        // decryption using the key derived from 1 password iteration,
        // hashing without salt, cipher block chaining (CBC) mode, and
        // SHA-1 hashing algorithm.
        // </summary>
        // <param name="passPhrase">
        // Passphrase from which a pseudo-random password will be derived.
        // The derived password will be used to generate the encryption key.
        // Passphrase can be any string. In this example we assume that the
        // passphrase is an ASCII string. Passphrase value must be kept in
        // secret.
        // </param>
        // <param name="initVector">
        // Initialization vector (IV). This value is required to encrypt the
        // first block of plaintext data. For RijndaelManaged class IV must be
        // exactly 16 ASCII characters long. IV value does not have to be kept
        // in secret.
        // </param>
        // <param name="minSaltLen">
        // Min size (in bytes) of randomly generated salt which will be added at
        // the beginning of plain text before encryption is performed. When this
        // value is less than 4, the default min value will be used (currently 4
        // bytes).
        // </param>
        // <param name="maxSaltLen">
        // Max size (in bytes) of randomly generated salt which will be added at
        // the beginning of plain text before encryption is performed. When this
        // value is negative or greater than 255, the default max value will be 
        // used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
        // than the specified min value (which can be adjusted to default value),
        // salt will not be used and plain text value will be encrypted as is.
        // In this case, salt will not be processed during decryption either.
        // </param>
        // <param name="keySize">
        // Size of symmetric key (in bits): 128, 192, or 256.
        // </param>
        public RRDM_Encryption(string passPhrase, string initVector, int minSaltLen, int maxSaltLen, int keySize)
            : this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize, null)
        {
        }

        // <summary>
        // Use this constructor if you are planning to perform encryption/
        // decryption using the key derived from 1 password iteration, hashing 
        // without salt, and cipher block chaining (CBC) mode.
        // </summary>
        // <param name="passPhrase">
        // Passphrase from which a pseudo-random password will be derived.
        // The derived password will be used to generate the encryption key.
        // Passphrase can be any string. In this example we assume that the
        // passphrase is an ASCII string. Passphrase value must be kept in
        // secret.
        // </param>
        // <param name="initVector">
        // Initialization vector (IV). This value is required to encrypt the
        // first block of plaintext data. For RijndaelManaged class IV must be
        // exactly 16 ASCII characters long. IV value does not have to be kept
        // in secret.
        // </param>
        // <param name="minSaltLen">
        // Min size (in bytes) of randomly generated salt which will be added at
        // the beginning of plain text before encryption is performed. When this
        // value is less than 4, the default min value will be used (currently 4
        // bytes).
        // </param>
        // <param name="maxSaltLen">
        // Max size (in bytes) of randomly generated salt which will be added at
        // the beginning of plain text before encryption is performed. When this
        // value is negative or greater than 255, the default max value will be
        // used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
        // than the specified min value (which can be adjusted to default value),
        // salt will not be used and plain text value will be encrypted as is.
        // In this case, salt will not be processed during decryption either.
        // </param>
        // <param name="keySize">
        // Size of symmetric key (in bits): 128, 192, or 256.
        // </param>
        // <param name="hashAlgorithm">
        // Hashing algorithm: "MD5" or "SHA1". SHA1 is recommended.
        // </param>
        public RRDM_Encryption(string passPhrase, string initVector, int minSaltLen, int maxSaltLen, int keySize, string hashAlgorithm)
            : this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize, hashAlgorithm, null)
        {
        }

        // <summary>
        // Use this constructor if you are planning to perform encryption/
        // decryption using the key derived from 1 password iteration, and
        // cipher block chaining (CBC) mode.
        // </summary>
        // <param name="passPhrase">
        // Passphrase from which a pseudo-random password will be derived.
        // The derived password will be used to generate the encryption key.
        // Passphrase can be any string. In this example we assume that the
        // passphrase is an ASCII string. Passphrase value must be kept in
        // secret.
        // </param>
        // <param name="initVector">
        // Initialization vector (IV). This value is required to encrypt the
        // first block of plaintext data. For RijndaelManaged class IV must be
        // exactly 16 ASCII characters long. IV value does not have to be kept
        // in secret.
        // </param>
        // <param name="minSaltLen">
        // Min size (in bytes) of randomly generated salt which will be added at
        // the beginning of plain text before encryption is performed. When this
        // value is less than 4, the default min value will be used (currently 4
        // bytes).
        // </param>
        // <param name="maxSaltLen">
        // Max size (in bytes) of randomly generated salt which will be added at
        // the beginning of plain text before encryption is performed. When this
        // value is negative or greater than 255, the default max value will be
        // used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
        // than the specified min value (which can be adjusted to default value),
        // salt will not be used and plain text value will be encrypted as is.
        // In this case, salt will not be processed during decryption either.
        // </param>
        // <param name="keySize">
        // Size of symmetric key (in bits): 128, 192, or 256.
        // </param>
        // <param name="hashAlgorithm">
        // Hashing algorithm: "MD5" or "SHA1". SHA1 is recommended.
        // </param>
        // <param name="saltValue">
        // Salt value used for password hashing during key generation. This is
        // not the same as the salt we will use during encryption. This parameter
        // can be any string.
        // </param>
        public RRDM_Encryption(string passPhrase, string initVector, int minSaltLen, int maxSaltLen, int keySize, string hashAlgorithm, string saltValue)
            : this(passPhrase, initVector, minSaltLen, maxSaltLen, keySize, hashAlgorithm, saltValue, 1)
        {
        }

        // <summary>
        // Use this constructor if you are planning to perform encryption/
        // decryption with the key derived from the explicitly specified
        // parameters.
        // </summary>
        // <param name="passPhrase">
        // Passphrase from which a pseudo-random password will be derived.
        // The derived password will be used to generate the encryption key
        // Passphrase can be any string. In this example we assume that the
        // passphrase is an ASCII string. Passphrase value must be kept in
        // secret.
        // </param>
        // <param name="initVector">
        // Initialization vector (IV). This value is required to encrypt the
        // first block of plaintext data. For RijndaelManaged class IV must be
        // exactly 16 ASCII characters long. IV value does not have to be kept
        // in secret.
        // </param>
        // <param name="minSaltLen">
        // Min size (in bytes) of randomly generated salt which will be added at
        // the beginning of plain text before encryption is performed. When this
        // value is less than 4, the default min value will be used (currently 4
        // bytes).
        // </param>
        // <param name="maxSaltLen">
        // Max size (in bytes) of randomly generated salt which will be added at
        // the beginning of plain text before encryption is performed. When this
        // value is negative or greater than 255, the default max value will be
        // used (currently 8 bytes). If max value is 0 (zero) or if it is smaller
        // than the specified min value (which can be adjusted to default value),
        // salt will not be used and plain text value will be encrypted as is.
        // In this case, salt will not be processed during decryption either.
        // </param>
        // <param name="keySize">
        // Size of symmetric key (in bits): 128, 192, or 256.
        // </param>
        // <param name="hashAlgorithm">
        // Hashing algorithm: "MD5" or "SHA1". SHA1 is recommended.
        // </param>
        // <param name="saltValue">
        // Salt value used for password hashing during key generation. This is
        // not the same as the salt we will use during encryption. This parameter
        // can be any string.
        // </param>
        // <param name="passwordIterations">
        // Number of iterations used to hash password. More iterations are
        // considered more secure but may take longer.
        // </param>

        public RRDM_Encryption(string passPhrase, string initVector, int minSaltLen, int maxSaltLen, int keySize, string hashAlgorithm, string saltValue, int passwordIterations)
        {
            // Save min salt length; set it to default if invalid value is passed.
            if ((minSaltLen < MIN_ALLOWED_SALT_LEN))
            {
                this.minSaltLen = DEFAULT_MIN_SALT_LEN;
            }
            else
            {
                this.minSaltLen = minSaltLen;
            }

            // Save max salt length; set it to default if invalid value is passed.
            if ((maxSaltLen < 0 | maxSaltLen > MAX_ALLOWED_SALT_LEN))
            {
                this.maxSaltLen = DEFAULT_MAX_SALT_LEN;
            }
            else
            {
                this.maxSaltLen = maxSaltLen;
            }

            // Set the size of cryptographic key.
            if ((keySize <= 0))
            {
                keySize = DEFAULT_KEY_SIZE;
            }

            // Set the name of algorithm. Make sure it is in UPPER CASE and does
            // not use dashes, e.g. change "sha-1" to "SHA1".
            if ((hashAlgorithm == null))
            {
                hashAlgorithm = DEFAULT_HASH_ALGORITHM;
            }
            else
            {
                hashAlgorithm = hashAlgorithm.ToUpper().Replace("-", "");
            }

            // Initialization vector converted to a byte array.
            byte[] initVectorBytes = null;

            // Salt used for password hashing (to generate the key, not during
            // encryption) converted to a byte array.
            byte[] saltValueBytes = null;

            // Get bytes of initialization vector.
            if ((initVector == null))
            {
                initVectorBytes = new byte[] { };
            }
            else
            {
                initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            }

            // Get bytes of salt (used in hashing).
            if ((saltValue == null))
            {
                saltValueBytes = new byte[] { };
            }
            else
            {
                saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            }

            // Generate password, which will be used to derive the key.
            // WARNING: Replaced PasswordDeriveBytes (PBKDF1) with Rfc2898DeriveBytes (PBKDF2). This BREAKS COMPATIBILITY with old keys.
            // Mapping algorithm name to known names for Rfc2898DeriveBytes
            HashAlgorithmName hashAlgoName = HashAlgorithmName.SHA1;
            if (!string.IsNullOrEmpty(hashAlgorithm))
            {
                 switch(hashAlgorithm.ToUpper()) {
                     case "SHA256": hashAlgoName = HashAlgorithmName.SHA256; break;
                     case "SHA384": hashAlgoName = HashAlgorithmName.SHA384; break;
                     case "SHA512": hashAlgoName = HashAlgorithmName.SHA512; break;
                     case "MD5": hashAlgoName = HashAlgorithmName.MD5; break;
                 }
            }

            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations, hashAlgoName))
            {
                // Convert key to a byte array adjusting the size from bits to bytes.
                byte[] keyBytes = password.GetBytes(keySize / 8);

                // Initialize Rijndael key object.
                // Replaced RijndaelManaged with Aes.Create(). AES is Rijndael with 128-bit block size.
                using (Aes symmetricKey = Aes.Create())
                {
                    // If we do not have initialization vector, we cannot use the CBC mode.
                    // The only alternative is the ECB mode (which is not as good).
                    if ((initVectorBytes.Length == 0))
                    {
                        symmetricKey.Mode = CipherMode.ECB;
                    }
                    else
                    {
                        symmetricKey.Mode = CipherMode.CBC;
                    }

                    // Create encryptor and decryptor, which we will use for cryptographic
                    // operations.
                    encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                    decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                }
            }
        }

        // <summary>
        // Encrypts a string value generating a base64-encoded string.
        // </summary>
        // <param name="plainText">
        // Plain text string to be encrypted.
        // </param>
        // <returns>
        // Cipher text formatted as a base64-encoded string.
        // </returns>
        public string Encrypt(string plainText)
        {
            return Encrypt(Encoding.UTF8.GetBytes(plainText));
        }

        // <summary>
        // Encrypts a byte array generating a base64-encoded string.
        // </summary>
        // <param name="plainTextBytes">
        // Plain text bytes to be encrypted.
        // </param>
        // <returns>
        // Cipher text formatted as a base64-encoded string.
        // </returns>
        public string Encrypt(byte[] plainTextBytes)
        {
            return Convert.ToBase64String(EncryptToBytes(plainTextBytes));
        }

        // <summary>
        // Encrypts a string value generating a byte array of cipher text.
        // </summary>
        // <param name="plainText">
        // Plain text string to be encrypted.
        // </param>
        // <returns>
        // Cipher text formatted as a byte array.
        // </returns>
        public byte[] EncryptToBytes(string plainText)
        {
            return EncryptToBytes(Encoding.UTF8.GetBytes(plainText));
        }

        // <summary>
        // Encrypts a byte array generating a byte array of cipher text.
        // </summary>
        // <param name="plainTextBytes">
        // Plain text bytes to be encrypted.
        // </param>
        // <returns>
        // Cipher text formatted as a byte array.
        // </returns>
        public byte[] EncryptToBytes(byte[] plainTextBytes)
        {
            byte[] functionReturnValue = null;

            // Add salt at the beginning of the plain text bytes (if needed).
            byte[] plainTextBytesWithSalt = AddSalt(plainTextBytes);

            // Encryption will be performed using memory stream.
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = null;

            // Let's make cryptographic operations thread-safe.
            lock (this)
            {
                // To perform encryption, we must use the Write mode.
                cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                // Start encrypting data.
                cryptoStream.Write(plainTextBytesWithSalt, 0, plainTextBytesWithSalt.Length);

                // Finish the encryption operation.
                cryptoStream.FlushFinalBlock();

                // Move encrypted data from memory into a byte array.
                byte[] cipherTextBytes = memoryStream.ToArray();

                // Close memory streams.
                memoryStream.Close();
                cryptoStream.Close();

                // Return encrypted data.
                functionReturnValue = cipherTextBytes;
            }
            return functionReturnValue;
        }

        // <summary>
        // Decrypts a base64-encoded cipher text value generating a string result.
        // </summary>
        // <param name="cipherText">
        // Base64-encoded cipher text string to be decrypted.
        // </param>
        // <returns>
        // Decrypted string value.
        // </returns>
        public string Decrypt(string cipherText)
        {
            return Decrypt(Convert.FromBase64String(cipherText));
        }

        // <summary>
        // Decrypts a byte array containing cipher text value and generates a
        // string result.
        // </summary>
        // <param name="cipherTextBytes">
        // Byte array containing encrypted data.
        // </param>
        // <returns>
        // Decrypted string value.
        // </returns>
        public string Decrypt(byte[] cipherTextBytes)
        {
            return Encoding.UTF8.GetString(DecryptToBytes(cipherTextBytes));
        }

        // <summary>
        // Decrypts a base64-encoded cipher text value and generates a byte array
        // of plain text data.
        // </summary>
        // <param name="cipherText">
        // Base64-encoded cipher text string to be decrypted.
        // </param>
        // <returns>
        // Byte array containing decrypted value.
        // </returns>
        public byte[] DecryptToBytes(string cipherText)
        {
            return DecryptToBytes(Convert.FromBase64String(cipherText));
        }

        // <summary>
        // Decrypts a base64-encoded cipher text value and generates a byte array
        // of plain text data.
        // </summary>
        // <param name="cipherTextBytes">
        // Byte array containing encrypted data.
        // </param>
        // <returns>
        // Byte array containing decrypted value.
        // </returns>
        public byte[] DecryptToBytes(byte[] cipherTextBytes)
        {

            byte[] decryptedBytes = null;
            byte[] plainTextBytes = null;
            int decryptedByteCount = 0;
            int saltLen = 0;

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Since we do not know how big decrypted value will be, use the same
            // size as cipher text. Cipher text is always longer than plain text
            // (in block cipher encryption), so we will just use the number of
            // decrypted data byte after we know how big it is.
            // decryptedBytes = new byte[cipherTextBytes.Length]; <-- discarding this allocation strategy slightly

            // Let's make cryptographic operations thread-safe.
            lock (this)
            {
                // To perform decryption, we must use the Read mode.
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                   // Create a temporary buffer to hold the decrypted data. 
                   // Since we don't know the exact size (padding removal), use a MemoryStream to collect it.
                   using (MemoryStream plainTextStream = new MemoryStream()) 
                   {
                        cryptoStream.CopyTo(plainTextStream);
                        decryptedBytes = plainTextStream.ToArray();
                        decryptedByteCount = decryptedBytes.Length;
                   }
                }
            }

            // If we are using salt, get its length from the first 4 bytes of plain
            // text data.
            if ((maxSaltLen > 0 & maxSaltLen >= minSaltLen))
            {
                saltLen = (decryptedBytes[0] & 0x3) | (decryptedBytes[1] & 0xc) | (decryptedBytes[2] & 0x30) | (decryptedBytes[3] & 0xc0);
            }

            // Allocate the byte array to hold the original plain text
            // (without salt).
            plainTextBytes = new byte[decryptedByteCount - saltLen];

            // Copy original plain text discarding the salt value if needed.
            Array.Copy(decryptedBytes, saltLen, plainTextBytes, 0, decryptedByteCount - saltLen);

            // Return original plain text value.
            return plainTextBytes;
        }

        // <summary>
        // Adds an array of randomly generated bytes at the beginning of the
        // array holding original plain text value.
        // </summary>
        // <param name="plainTextBytes">
        // Byte array containing original plain text value.
        // </param>
        // <returns>
        // Either original array of plain text bytes (if salt is not used) or a
        // modified array containing a randomly generated salt added at the 
        // beginning of the plain text bytes. 
        // </returns>
        private byte[] AddSalt(byte[] plainTextBytes)
        {
            byte[] functionReturnValue = null;

            // The max salt value of 0 (zero) indicates that we should not use 
            // salt. Also do not use salt if the max salt value is smaller than
            // the min value.
            if ((maxSaltLen == 0 | maxSaltLen < minSaltLen))
            {
                functionReturnValue = plainTextBytes;
                return functionReturnValue;
            }

            // Generate the salt.
            byte[] saltBytes = GenerateSalt();

            // Allocate array which will hold salt and plain text bytes.
            byte[] plainTextBytesWithSalt = new byte[plainTextBytes.Length + saltBytes.Length];
            // First, copy salt bytes.
            Array.Copy(saltBytes, plainTextBytesWithSalt, saltBytes.Length);

            // Append plain text bytes to the salt value.
            Array.Copy(plainTextBytes, 0, plainTextBytesWithSalt, saltBytes.Length, plainTextBytes.Length);

            functionReturnValue = plainTextBytesWithSalt;
            return functionReturnValue;
        }

        // <summary>
        // Generates an array holding cryptographically strong bytes.
        // </summary>
        // <returns>
        // Array of randomly generated bytes.
        // </returns>
        // <remarks>
        // Salt size will be defined at random or exactly as specified by the
        // minSlatLen and maxSaltLen parameters passed to the object constructor.
        // The first four bytes of the salt array will contain the salt length
        // split into four two-bit pieces.
        // </remarks>
        private byte[] GenerateSalt()
        {

            // We don't have the length, yet.
            int saltLen = 0;

            // If min and max salt values are the same, it should not be random.
            if ((minSaltLen == maxSaltLen))
            {
                saltLen = minSaltLen;
                // Use random number generator to calculate salt length.
            }
            else
            {
                saltLen = GenerateRandomNumber(minSaltLen, maxSaltLen);
            }

            // Allocate byte array to hold our salt.
            byte[] salt = new byte[saltLen];

            // Populate salt with cryptographically strong bytes.
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(salt);
            }

            // Split salt length (always one byte) into four two-bit pieces and
            // store these pieces in the first four bytes of the salt array.
            salt[0] = (byte)((salt[0] & 0xfc) | (saltLen & 0x3));
            salt[1] = (byte)((salt[1] & 0xf3) | (saltLen & 0xc));
            salt[2] = (byte)((salt[2] & 0xcf) | (saltLen & 0x30));
            salt[3] = (byte)((salt[3] & 0x3f) | (saltLen & 0xc0));

            return salt;
        }

        // <summary>
        // Generates random integer.
        // </summary>
        // <param name="minValue">
        // Min value (inclusive).
        // </param>
        // <param name="maxValue">
        // Max value (inclusive).
        // </param>
        // <returns>
        // Random integer value between the min and max values (inclusive).
        // </returns>
        // <remarks>
        // This methods overcomes the limitations of .NET Framework's Random
        // class, which - when initialized multiple times within a very short
        // period of time - can generate the same "random" number.
        // </remarks>
        private int GenerateRandomNumber(int minValue, int maxValue)
        {

            // We will make up an integer seed from 4 bytes of this array.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            // Generate 4 random bytes.
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            // Convert four random bytes into a positive integer value.
            int seed = ((randomBytes[0] & 0x7f) << 24) | (randomBytes[1] << 16) | (randomBytes[2] << 8) | (randomBytes[3]);

            // Now, this looks more like real randomization.
            Random random = new Random(seed);

            // Calculate a random number.
            return random.Next(minValue, maxValue + 1);
        }



        #endregion

    }
}
