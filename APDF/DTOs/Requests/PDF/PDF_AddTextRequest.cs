using System.ComponentModel.DataAnnotations;

namespace APDF.DTOs.Requests.PDF
{
    public class PDF_AddTextRequest
    {
        [Required]
        public string InputFile { get; set; } = default!;

        [Required]
        public string OutputFile { get; set; } = default!;

        [Required]
        public ICollection<PDF_AddTextDetailRequest> Details { get; set; } = new List<PDF_AddTextDetailRequest>();

        [Required]
        public int PaperSize { get; set; } = 1;

        public int? ManualPaperSize { get; set; }
    }

    public class PDF_AddTextDetailRequest
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

        public float RadAngle { get; set; } = 0;

        public int FontSize { get; set; } = 13;
    }
}
