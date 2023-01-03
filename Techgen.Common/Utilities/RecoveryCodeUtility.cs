using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Common.Utilities
{
    public static class RecoveryCodeUtility
    {
        static HashSet<string> recoveryCodes = new HashSet<string>();

        public static string GenereteRecoveryCode()
        {
            Random generator = new Random();
            while (true)
            {
                string recoveryCode = generator.Next(0, 1000000).ToString("D6");               
                if (recoveryCodes.Add(recoveryCode))
                {
                    return recoveryCode;
                }     
                
                continue;
            }
        }
    }
}
