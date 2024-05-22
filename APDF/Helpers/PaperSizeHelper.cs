namespace APDF.Helpers
{
    /// <summary>
    /// Convert Paper format: A0, A1,... to uu
    /// </summary>
    public static class PaperSizeHelper
    {
        public static double GetHeight(int size) => UnitHelper.mm2uu((float)(Math.Pow(2d, 0.25d) * Math.Pow(1.0d / Math.Sqrt(2), size + 1) * 1000.0d));

        public static double GetWidth(int size) => UnitHelper.mm2uu((float)(Math.Pow(2d, 0.25d) * Math.Pow(1.0d / Math.Sqrt(2), size) * 1000.0d));

        public static int GetSize(float width, float height)
        {
            var sizeY = (int)Math.Round(Math.Log((UnitHelper.uu2mm(height) / 1000.0d) / Math.Pow(2d, 0.25d), (1d / Math.Sqrt(2))) - 1);
            var sizeX = (int)Math.Round(Math.Log((UnitHelper.uu2mm(width) / 1000.0d) / Math.Pow(2d, 0.25d), (1d / Math.Sqrt(2))));
            return sizeX == sizeY ? sizeX : 0;
        }
    }
}
