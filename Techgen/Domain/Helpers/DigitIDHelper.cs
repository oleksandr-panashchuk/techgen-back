namespace Techgen.Domain.Helpers
{
    public static class DigitIDHelper
    {
        private static int DigitID = 0;
        public static int GetDigitID => ++DigitID;
    }
}
