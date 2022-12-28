using PasswordGenerator;

namespace Techgen.Domain.Helpers
{
    public static class CreatePasswordHelper
    {
        public static string GeneratePassword()
        {
            var pwd = new Password(includeLowercase: true, includeUppercase: true,
                includeNumeric: true, includeSpecial: false, passwordLength: 21);
            return pwd.Next();
        }
    }
}
