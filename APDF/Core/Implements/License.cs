using APDF.Core.Interfaces;
using APDF.DTOs.Responses.License;
using APDF.Helpers;
using Standard.Licensing;
using Standard.Licensing.Validation;
using System.Reflection;

namespace APDF.Core.Implements
{
    internal class License : ILicense
    {
        private readonly string PUBLIC_KEY = "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEFLRE9tEYkdoFv6q9zE1LrtnmfRA19tiHO89pOErX8kIDvGxAbVbAeIJxOiDEpjAVD1zIWTSpeIEI9oZUhTiLJA==";
        private readonly string FILE_LICENSE = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "patch.obj");

        public LicenseResponse Validate()
        {
            if (!File.Exists(Path.Combine(Assembly.GetExecutingAssembly().Location,FILE_LICENSE)))
            {
                return new LicenseResponse
                {
                    IsValid = false,
                    Message = "Not found license. Please set license path in environment"
                };
            }

            using (var stream = new FileStream(FILE_LICENSE, FileMode.Open, FileAccess.Read, FileShare.Read, 4096))
            {
                var license = Standard.Licensing.License.Load(stream);

                license.AdditionalAttributes.Add("UUID", HardwareHelper.GenerateUID(Assembly.GetExecutingAssembly().GetName().Name));
                license.ProductFeatures.Add("Controllers", "*");
                license.ProductFeatures.Add("Actions", "*");

                var validationFailures = license.Validate()
                                .ExpirationDate(systemDateTime: DateTime.Now)
                                .When(lic => lic.Type == LicenseType.Standard)
                                .And()
                                .Signature(PUBLIC_KEY)
                                .AssertValidLicense();

                return new LicenseResponse
                {
                    IsValid = !validationFailures.Any(),
                    Message = validationFailures.Any() ? string.Join("\n", validationFailures.Select(item => $"{item.Message} - Resolve: {item.HowToResolve}")) : "License: OK"
                };
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
