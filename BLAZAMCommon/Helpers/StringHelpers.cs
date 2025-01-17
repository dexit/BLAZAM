﻿
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLAZAM.Helpers
{
    public static class StringHelpers
    {
        public static MarkupString ToMarkupString(this string input)
        {
            return (MarkupString)input.Replace("\r\n","<br>").Replace("\n","<br>");
        }
        /// <summary>
        /// Hash code that doesn't change with application
        /// restarts
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int GetAppHashCode(this string input)
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                foreach (char c in input)
                {
                    hash = hash * 23 + c;
                }
                return hash;
            }
        }   
        




        public static bool IsNullOrEmpty(this string? str)
        {
            return str == null || str.Length < 1;
        }

        public static bool IsUrlLocalToHost(this string url)
        {
            if (url.StartsWith("https://localhost")) return true;
            if (url == "") return true;
            return url[0] == '/' && (url.Length == 1 ||
                    url[1] != '/' && url[1] != '\\') ||   // "/" or "/foo" but not "//" or "/\"
                    url.Length > 1 &&
                     url[0] == '~' && url[1] == '/';   // "~/" or "~/foo"
        }

        public static Guid ToGuid(this string input)
        {
            // Use MD5 hash to get a 16-byte hash of the string
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                // Create a new Guid using the hash
                return new Guid(hash);
            }
        }

        public static string ToPlainText(this SecureString? secureString)
        {
            if (secureString == null) return string.Empty;
            IntPtr bstrPtr = Marshal.SecureStringToBSTR(secureString);
            try
            {
                var plainText = Marshal.PtrToStringBSTR(bstrPtr);
                if (plainText == null)
                    plainText = string.Empty;
                return plainText;

            }
            finally
            {
                Marshal.ZeroFreeBSTR(bstrPtr);
            }
        }
        public static SecureString ToSecureString(this string plainText)
        {
            return new NetworkCredential("", plainText).SecurePassword;
        }

        public static string? ToPrettyOu(this string? ou)
        {
            if (ou == null) return null;
            var ouComponents = Regex.Matches(ou, @"OU=([^,]*)")
                .Select(m => m.Groups[1].Value)
                .ToList();
            ouComponents.Reverse();
            return "/"+string.Join("/", ouComponents);
        }
        public static string FqdnToDN(this string fqdn)
        {
            // Split the FQDN into its domain components
            string[] domainComponents = fqdn.Split('.');



            // Build the DN by appending each reversed domain component as a RDN (relative distinguished name)
            StringBuilder dnBuilder = new StringBuilder();
            foreach (string dc in domainComponents)
            {
                dnBuilder.Append("DC=");
                dnBuilder.Append(dc);
                dnBuilder.Append(",");
            }

            // Remove the last comma
            dnBuilder.Length--;

            // Return the DN
            return dnBuilder.ToString();
        }
    }
}
