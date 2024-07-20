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

        public LicenseResponse Validate()
        {
            var trial = Environment.GetEnvironmentVariable("Microsoft", EnvironmentVariableTarget.User);
            if (string.IsNullOrEmpty(trial))
            {
                trial = DateTime.Now.ToString();
                Environment.SetEnvironmentVariable("Microsoft", trial, EnvironmentVariableTarget.User);
            }

            if ((DateTime.TryParse(trial, out DateTime date) && DateTime.Now.Subtract(date) < TimeSpan.FromDays(30)))
            {
                return new LicenseResponse
                {
                    IsValid = true,
                    Message = "License: Trial"
                };
            }
            else
            {
                string? file = Environment.GetEnvironmentVariable("APDF.License", EnvironmentVariableTarget.User);
                if (string.IsNullOrEmpty(file) || !File.Exists(file))
                    return new LicenseResponse
                    {
                        IsValid = false,
                        Message = "Not found license. Please set license path in environment"
                    };

                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096))
                {
                    var license = Standard.Licensing.License.Load(stream);

                    license.AdditionalAttributes.Add("UUID", HardwareHelper.GenerateUID(Assembly.GetExecutingAssembly().GetName().Name));
                    license.ProductFeatures.Add("Controllers", "*");
                    license.ProductFeatures.Add("Actions", "*");

                    var validationFailures = license.Validate()
                                    .ExpirationDate(systemDateTime: DateTime.Now)
                                    .When(lic => lic.Type == LicenseType.Trial)
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
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
