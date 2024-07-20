namespace APDF.DTOs.Responses.License
{
    public class LicenseResponse
    {
        public bool IsValid { get; set; } = false;

        public string Message { get; set; } = default!;
    }
}
