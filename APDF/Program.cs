using APDF.Core.Implements;
using APDF.Core.Interfaces;
using APDF.Helpers;
using System.Reflection;

namespace APDF
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<ILicense, License>();

            var app = builder.Build();

            var license = app.Services.GetRequiredService<ILicense>();
            var checkLicense = license.Validate();
            if (checkLicense.IsValid)
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseHttpsRedirection();

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            else
            {
                app.Logger.LogError(checkLicense.Message);
                app.Logger.LogInformation($"Please contact developer to get license. Your license UUID: {HardwareHelper.GenerateUID(Assembly.GetExecutingAssembly().GetName().Name)}");
            }
        }
    }
}
