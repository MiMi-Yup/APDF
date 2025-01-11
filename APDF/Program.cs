using APDF.Core.Implements;
using APDF.Core.Interfaces;
using APDF.Helpers;
using System.Reflection;

namespace APDF
{
    internal class Program
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
            if (checkLicense.IsValid || true)
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
                app.Logger.LogError($"Error code: {HardwareHelper.GenerateUID(Assembly.GetExecutingAssembly().GetName().Name)}");
            }
        }
    }
}
