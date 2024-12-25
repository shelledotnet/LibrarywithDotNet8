using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Books.Domain.Service
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class SHA512Converter
    {
        public static string GenerateSHA512String(string inputString)
        {
            using (SHA512 sha512 = SHA512.Create()) // Modern usage
            {
                byte[] bytes = Encoding.UTF8.GetBytes(inputString);
                byte[] hash = sha512.ComputeHash(bytes);
                return GetStringFromHash(hash);
            }
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
            {
                result.Append(b.ToString("X2"));
            }
            return result.ToString();
        }
    }

}
