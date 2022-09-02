using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace CargaAmbulatoria.Services.Services
{
    public class PasswordService
    {
        private readonly IConfiguration _configuration;
        public PasswordService(IConfiguration configuration) : this(configuration["Application:Secret"], CipherMode.CBC, 128, 256, PaddingMode.PKCS7)
        {
            this._configuration = configuration;
        }

        #region AES PASSWORD MANAGEMENT
        private readonly byte[] _Iv = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
        private byte[] _key = null;
        private Aes encryptor;
        private MemoryStream memoryStream;
        private ICryptoTransform cryptoTransform;
        private CryptoStream cryptoStream;

        public PasswordService(string password, CipherMode cipherMode, int blockSize, int keySize, PaddingMode paddingMode)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException("password");
            this._key = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(password));
            // Instantiate a new Aes object to perform string symmetric encryption
            this.encryptor = Aes.Create();
            if (this.encryptor == null) throw new Exception("Cannot create Advanced Encryption Standard instance");
            this.encryptor.Mode = cipherMode;
            this.encryptor.BlockSize = blockSize;
            this.encryptor.KeySize = keySize;
            this.encryptor.Padding = paddingMode;
            this.encryptor.Key = this._key;
            this.encryptor.IV = this._Iv;
        }

        public string Encrypt(string plainText)
        {
            return this.EncryptDecrypt(true, plainText);
        }

        public string Decrypt(string cipherText)
        {
            string decrypted = this.EncryptDecrypt(false, cipherText);
            if (!IsASCII(decrypted)) throw new Exception("Invalid Password");
            return decrypted;
        }

        public static bool IsASCII(string value)
        {
            // ASCII encoding replaces non-ascii with question marks, so we use UTF8 to see if multi-byte sequences are there
            return Encoding.UTF8.GetByteCount(value) == value.Length;
        }

        private string EncryptDecrypt(bool encrypt, string text)
        {
            string convertedText;
            try
            {
                // Instantiate a new MemoryStream object to contain the encrypted bytes
                this.memoryStream = new MemoryStream();

                // Instantiate a new encryptor/decryptor from our Aes object
                this.cryptoTransform = encrypt ? this.encryptor.CreateEncryptor() : this.encryptor.CreateDecryptor();

                // Instantiate a new CryptoStream object to process the data and write it to the 
                // memory stream
                this.cryptoStream = new CryptoStream(this.memoryStream, this.cryptoTransform, CryptoStreamMode.Write);

                // Convert the plainText string into a byte array
                byte[] contentBytes = encrypt ? Encoding.ASCII.GetBytes(text) : Convert.FromBase64String(text);

                // Write the bytes in CryptoStream
                this.cryptoStream.Write(contentBytes, 0, contentBytes.Length);

                // Complete the encryption/decryption process
                this.cryptoStream.FlushFinalBlock();

                // Convert the encrypted / decrypted data from a MemoryStream to a byte array
                byte[] resultBytes = this.memoryStream.ToArray();

                //Convert the byte array to string
                convertedText = encrypt ? Convert.ToBase64String(resultBytes, 0, resultBytes.Length) : Encoding.ASCII.GetString(resultBytes, 0, resultBytes.Length);
            }
            finally
            {
                //Dispose objects
                this.memoryStream?.Dispose();
                this.cryptoStream?.Dispose();
            }

            return convertedText;
        }
        #endregion

    }
}
