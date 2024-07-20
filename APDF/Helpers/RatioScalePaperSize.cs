using iText.Kernel.Geom;

namespace APDF.Helpers
{
    internal static class RatioScalePaperSize
    {
        /// <summary>
        /// Require; Unit of rectangle is uu
        /// </summary>
        /// <param name="fromSize"></param>
        /// <param name="toSize"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static Rectangle ConvertFormat(int fromSize, int toSize, Rectangle rectangle)
        {
            var ratioX = PaperSizeHelper.GetHeight(toSize) / PaperSizeHelper.GetHeight(fromSize);
            var ratioY = PaperSizeHelper.GetWidth(toSize) / PaperSizeHelper.GetWidth(fromSize);

            return new Rectangle(rectangle.GetX() * ((float)ratioX),
                rectangle.GetY() * ((float)ratioY),
                rectangle.GetWidth() * ((float)ratioX),
                rectangle.GetHeight() * ((float)ratioY));
        }

        /// <summary>
        /// Require; Unit of rectangle is uu
        /// </summary>
        /// <param name="fromSize"></param>
        /// <param name="toRectangleSize"></param>
        /// <param name="fromRectangle"></param>
        /// <returns></returns>
        public static Rectangle ConvertFormat(int fromSize, Rectangle toRectangleSize, Rectangle fromRectangle)
        {
            var ratioX = toRectangleSize.GetWidth() / PaperSizeHelper.GetWidth(fromSize);
            var ratioY = toRectangleSize.GetHeight() / PaperSizeHelper.GetHeight(fromSize);

            return new Rectangle(fromRectangle.GetX() * ((float)ratioX),
                fromRectangle.GetY() * ((float)ratioY),
                fromRectangle.GetWidth() * ((float)ratioX),
                fromRectangle.GetHeight() * ((float)ratioY));
        }
    }
}
