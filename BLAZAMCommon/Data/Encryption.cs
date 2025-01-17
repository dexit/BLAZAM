﻿using BLAZAM.Helpers;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace BLAZAM.Common.Data
{
    public class Encryption
    {
        private const string OldSalt = "BLAZAM_SALT";
        public static Encryption Instance;

        private void SetSalt(int maximumSaltLength = 32)
        {
            var salt = new byte[maximumSaltLength];

            RandomNumberGenerator.Create().GetNonZeroBytes(salt);
            

            Salt = salt;
        }

        /// <summary>
        /// String used to generate the key bytes
        /// </summary>
        /// <remarks>
        /// This is not the actual key used but will be transformed
        /// in a consistent manner, into a reusable key
        /// </remarks>
        public string KeySeedString { get; set; }

        /// <summary>
        /// The size of the key in bits
        /// </summary>
        private int KeySize { get; set; }

        /// <summary>
        /// The key that is <see cref="KeySize"/> bits long and was 
        /// generated from the <see cref="KeySeedString"/> 
        /// </summary>
        public byte[] Key { get; set; }
        private byte[] Salt { get; set; }



        /// <summary>
        /// Initializes the Encryption service that
        /// can both encrypt and decrypt any serializable object.
        /// </summary>
        /// <param name="keySeedString">Any length string that will be
        /// processed into a repeatable generated private key</param>
        /// <param name="keySize">How large a key to generate in bits</param>
        /// 
        /// <exception cref="ApplicationException">Throws an exception when no keySeedString is provided.</exception>
        public Encryption(string? keySeedString, int keySize = 256)
        {
            Instance = this;

            //if (keySeedString==null || keySeedString == "") throw new ApplicationException("An ecryption seedsting must be provided");
            if (keySeedString == null || keySeedString == "") return;
            KeySeedString = keySeedString;
            KeySize = keySize;
            SetSalt();
            GenerateOldKeyFromSeedString();
            GenerateKeyFromSeedString(Salt);
        }

        /// <summary>
        /// Generates a key of the configured key size, seeding the
        /// key from the appsettings configuration value "EncryptionKey"
        /// </summary>
        /// <remarks>
        /// Sets the local <see cref="Key"/> value to the newly generated key
        /// </remarks>
        /// <returns>The key based on the <see cref="KeySeedString"/></returns>
        [Obsolete("This method uses the old static salt value, use the parametrized version instead.")]
        private byte[] GenerateOldKeyFromSeedString()
        {
            // Use a key derivation function to generate a repeatable key
            // var salt = GetSalt(11);
            var salt = Encoding.UTF8.GetBytes(OldSalt);
            var keyGenerator = new Rfc2898DeriveBytes(KeySeedString, salt, 1000);
            Key = keyGenerator.GetBytes(KeySize / 8);
            return Key;
        }
        /// <summary>
        /// Generates a key of the configured key size, seeding the
        /// key from the appsettings configuration value "EncryptionKey"
        /// </summary>
        /// <remarks>
        /// Sets the local <see cref="Key"/> value to the newly generated key
        /// </remarks>
        /// <returns>The key based on the <see cref="KeySeedString"/></returns>
        private byte[] GenerateKeyFromSeedString(byte[] salt)
        {
            // Use a key derivation function to generate a repeatable key
            var keyGenerator = new Rfc2898DeriveBytes(KeySeedString, salt, 1000);

            return keyGenerator.GetBytes(KeySize / 8); ;
        }

        /// <summary>
        /// Decrypts cipher-text
        /// </summary>
        /// <typeparam name="T">The serializable type that should
        /// represent the decrypted cipher object</typeparam>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private T? DecryptSaltedObject<T>(string? cipherText)
        {

            try
            {
                var saltCipherArray = cipherText.Split(',');
                byte[] buffer = Convert.FromBase64String(saltCipherArray[1]);
                byte[] cipherSalt = Convert.FromBase64String(saltCipherArray[0]);
                var key = GenerateKeyFromSeedString(cipherSalt);


                byte[] iv = buffer.Take(16).ToArray<byte>();
                buffer = buffer.Skip(16).ToArray<byte>();
                using Aes aes = Aes.Create();
                aes.Key = GenerateKeyFromSeedString(cipherSalt);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using MemoryStream memoryStream = new MemoryStream(buffer);

                using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                using StreamReader streamReader = new StreamReader(cryptoStream);

                var decrypted= JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd());
                return decrypted;



            }
            catch (FormatException)
            {
                //If any issues occur while creating
                //the decrypted text, return the "encrypted"
                //text
                if (cipherText is T tText)
                {
                    return tText;
                }
            }
            throw new ApplicationException("Unable to decrypt cipherText");

        }
        /// <summary>
        /// Decrypts cipher-text
        /// </summary>
        /// <typeparam name="T">The serializable type that should
        /// represent the decrypted cipher object</typeparam>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public T? DecryptObject<T>(string? cipherText)
        {

            if (cipherText == null) return default;

            try
            {
                var newDecrypted = DecryptSaltedObject<T>(cipherText);
                return newDecrypted;

                throw new ApplicationException("Unable to decrypt cipherText");


            }
            catch
            {
                try
                {
                    byte[] buffer = Convert.FromBase64String(cipherText);

                    byte[] iv = buffer.Take(16).ToArray<byte>();
                    buffer = buffer.Skip(16).ToArray<byte>();
                    using Aes aes = Aes.Create();
                    aes.Key = Key;
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using MemoryStream memoryStream = new MemoryStream(buffer);

                    using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                    using StreamReader streamReader = new StreamReader(cryptoStream);

                    return JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd());




                }
                catch (FormatException)
                {
                    //If any issues occur while creating
                    //the decrypted text, return the "encypted"
                    //text
                    if (cipherText is T tText)
                    {
                        return tText;
                    }
                }
            }

            throw new ApplicationException("Unable to decrypt cipherText");
        }
        /// <summary>
        /// Encrypt any serializable object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string EncryptObject(object obj)
        {
            Random rand = new Random();
            int ivSeed = rand.Next(0, 65535);

            var salt = Encoding.UTF8.GetBytes(OldSalt);

            var keyGenerator = new Rfc2898DeriveBytes(ivSeed.ToByteArray(), salt, 1000);
            byte[] iv = keyGenerator.GetBytes(16);
            byte[] encryptedBytes;
            using Aes aes = Aes.Create();

            aes.Key = GenerateKeyFromSeedString(Salt);
            aes.IV = iv;
            ICryptoTransform encryptor = aes.CreateEncryptor();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(
                    memoryStream,
                    encryptor,
                    CryptoStreamMode.Write))
                {

                    var serialized = JsonConvert.SerializeObject(obj);
                    byte[] data = Encoding.UTF8.GetBytes(serialized);
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();

                    encryptedBytes = memoryStream.ToArray();

                    var encryptedMessage = iv;


                    for (int i = 0; i < encryptedBytes.Length; i++)
                    {

                        encryptedMessage = encryptedMessage.Append(encryptedBytes[i]).ToArray();
                    }
                    var cipherText = Convert.ToBase64String(encryptedMessage);
                    cipherText = Convert.ToBase64String(Salt) + "," + cipherText;

                    

                    return cipherText;

                }
            }
        }

    }



}