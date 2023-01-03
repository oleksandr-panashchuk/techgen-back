using System;

namespace Techgen.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static void ThrowsWhenNull(this object obj, string paramName = null)
        {
            if (obj == null)
            {
                if (string.IsNullOrEmpty(paramName))
                    throw new ArgumentNullException();
                else
                    throw new ArgumentNullException(paramName);
            }
        }

        public static void ThrowsWhenLessZero(this int @int, string paramName = null)
        {
            if (@int < 0)
            {
                if (string.IsNullOrEmpty(paramName))
                    throw new ArgumentException();
                else
                    throw new ArgumentException(string.Empty, paramName);
            }
        }
    }
}
