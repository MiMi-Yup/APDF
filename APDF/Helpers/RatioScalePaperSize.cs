using iText.Kernel.Geom;

namespace APDF.Helpers
{
    internal static class RatioScalePaperSize
    {
        /// <summary>
        /// Require; Unit of rectangle is uu
        /// </summary>
        /// <param name="fromSize"></param>
        /// <param name="toRectangleSize"></param>
        /// <param name="fromRectangle"></param>
        /// <returns></returns>
        public static (Point addPoint, float offsetRadAngle) ConvertFormat(int fromSize, Rectangle toRectangleSize, Point fromRectangle)
        {
            float toWidth = toRectangleSize.GetWidth(), toHeight = toRectangleSize.GetHeight();
            double fromWidth = PaperSizeHelper.GetWidth(fromSize), fromHeight = PaperSizeHelper.GetHeight(fromSize);

            bool swap = toWidth < toHeight;

            var ratioX = (swap ? toHeight : toWidth) / fromWidth;
            var ratioY = (swap ? toWidth : toHeight) / fromHeight;

            if (swap)
                return (new Point((fromHeight - fromRectangle.GetY()) * ((float)ratioX), fromRectangle.GetX() * ((float)ratioY)), (float)Math.PI / 2);
            else
                return (new Point(fromRectangle.GetX() * ((float)ratioX), fromRectangle.GetY() * ((float)ratioY)), 0f);
        }
    }
}
