using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Techgen.Common.Utilities.Interfaces;

namespace Techgen.Common.Utilities
{
    public class HashUtility : IHashUtility
    {
        public string GetHash(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return "";

            byte[] data = Encoding.ASCII.GetBytes(inputString);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            string hash = Encoding.ASCII.GetString(data);
            return hash;
        }
    }
}
