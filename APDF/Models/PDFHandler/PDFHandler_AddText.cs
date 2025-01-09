using iText.Layout.Properties;
using System.ComponentModel.DataAnnotations;

namespace APDF.Models.PDFHandler
{
    public class PDFHandler_AddText
    {
        /// <summary>
        /// newline: (newline)
        /// </summary>
        [Required]
        public string Text { get; set; } = default!;

        /// <summary>
        /// Unit: mm
        /// </summary>
        [Required]
        public float XPosition { get; set; }

        /// <summary>
        /// Unit: mm
        /// </summary>
        [Required]
        public float YPosition { get; set; }

        [Required]
        public int Page { get; set; } = 1;

        [Required]
        public int PaperSize { get; set; } = 1;

        public int? ManualPaperSize { get; set; }

        public iText.Kernel.Colors.Color? BackgroundColor { get; set; }

        public iText.Kernel.Colors.Color TextColor { get; set; } = iText.Kernel.Colors.ColorConstants.BLACK;

        public TextAlignment? TextAlignment { get; set; }

        public VerticalAlignment? VerticalAlignment { get; set; }

        public float RadAngle { get; set; } = 0;

        public int FontSize { get; set; } = 13;
    }
}
