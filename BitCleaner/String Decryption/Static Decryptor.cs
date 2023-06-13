using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BitCleaner.String_Decryption
{
    internal class StaticDecryptor
    {
        internal static string Decrypt(byte[] A_0, byte[] A_1, byte[] A_2)
        {
            byte[] array = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
                {
                    rijndaelManaged.KeySize = 256;
                    rijndaelManaged.BlockSize = 128;
                    Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(A_2, A_1, 1000);
                    rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
                    rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
                    rijndaelManaged.Mode = CipherMode.CBC;
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(A_0, 0, A_0.Length);
                        cryptoStream.Close();
                    }
                    array = memoryStream.ToArray();
                    rfc2898DeriveBytes.Dispose();
                }
            }
            return Encoding.UTF8.GetString(array);
        }
    }
}
