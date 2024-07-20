using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace APDF.Core.Implements
{
    internal class LocationTextExtractionStrategyV2 : LocationTextExtractionStrategy
    {
        private readonly Rectangle? overlap;

        public LocationTextExtractionStrategyV2()
        {
            overlap = null;
        }

        public LocationTextExtractionStrategyV2(Rectangle? rect)
        {
            overlap = rect;
        }

        /// <summary>
        /// Unit: uu
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public LocationTextExtractionStrategyV2(float x, float y, float width, float height)
        {
            overlap = new Rectangle(x, y, width, height);
        }

        public override void EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_TEXT && overlap != null)
            {
                TextRenderInfo renderInfo = (TextRenderInfo)data;
                Rectangle rect = renderInfo.GetDescentLine().GetBoundingRectangle();
                if (!overlap.Overlaps(rect))
                    return;
            }
            base.EventOccurred(data, type);
        }
    }
}
