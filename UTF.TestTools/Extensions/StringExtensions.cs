using System;
using System.Security.Cryptography;
using System.Text;

namespace UTF.TestTools
{
    public static class StringExtensions
    {
        /// <summary>
        /// Create an hash code based on the SHA256 hashing
        /// </summary>
        /// <param name="str">the string itself</param>
        /// <returns>a string that is the hash code generated</returns>
        public static string GetSha256HashCode(this string str)
        {
            SHA256 shaHash;
            shaHash = SHA256.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(str));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder stringBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return stringBuilder.ToString();
        }
    }
}
