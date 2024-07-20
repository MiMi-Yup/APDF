using APDF.DTOs.Responses.License;

namespace APDF.Core.Interfaces
{
    public interface ILicense : IDisposable
    {
        LicenseResponse Validate();
    }
}
