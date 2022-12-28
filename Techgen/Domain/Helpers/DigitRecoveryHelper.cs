using System.Runtime.CompilerServices;
using Techgen.DAL.Interfaces;

namespace Techgen.Domain.Helpers
{
  
    public static class DigitRecoveryHelper
    {   
        public static string GenereteCodeRecovery(List<string> recoveryCodes)
        {
            Random generator = new Random();
            while (true)
            {                
                string recoveryCode = generator.Next(0, 1000000).ToString("D6");
                if (!recoveryCodes.Contains(recoveryCode))
                {
                    return recoveryCode;
                }
                continue;
            }           
        }
    }
}
