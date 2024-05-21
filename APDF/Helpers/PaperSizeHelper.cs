namespace APDF.Helpers
{
    /// <summary>
    /// Convert Paper format: A0, A1,... to uu
    /// </summary>
    public static class PaperSizeHelper
    {
        public static double GetHeight(int size) => UnitHelper.mm2uu((float)(Math.Pow(2d, 0.25d) * Math.Pow(1.0d / Math.Sqrt(2), size + 1) * 1000.0d));

        public static double GetWidth(int size) => UnitHelper.mm2uu((float)(Math.Pow(2d, 0.25d) * Math.Pow(1.0d / Math.Sqrt(2), size) * 1000.0d));
    }
}
