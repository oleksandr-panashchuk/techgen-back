
namespace Techgen.Common.Utilities
{
    public static class DigitIdUtility
    {
        private static int DigitID = 0;
        public static int GetDigitID => ++DigitID;
    }
}
