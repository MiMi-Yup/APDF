namespace APDF.DTOs.Requests.PDF
{
    /// <summary>
    /// Unit of measure is mm
    /// </summary>
    public class PDF_ExtractInfoRequest
    {
        public float XPosition { get; set; }
        public float YPosition { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int PaperSize { get; set; } = 1;
        public string FilePath { get; set; } = default!;
    }
}
