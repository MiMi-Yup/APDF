namespace APDF.DTOs.Requests.PDF
{
    public class PDF_ReadPORequest
    {
        public string FilePath { get; set; } = default!;

        public int[] NOs { get; set; } = [];
    }
}
