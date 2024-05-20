using System.ComponentModel.DataAnnotations;

namespace APDF.DTOs.Requests
{
    public class PDF_AddText
    {
        [Required]
        public string InputFile { get; set; } = default!;

        [Required]
        public string OutputFile { get; set; } = default!;

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
    }
}
