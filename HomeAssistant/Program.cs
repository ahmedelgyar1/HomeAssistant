using HomeAssistant.Controllers;
using Microsoft.EntityFrameworkCore;
using HomeAssistant.Services;


namespace HomeAssistant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();
            builder.Services.AddControllers()

    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
            builder.Services.AddDbContext<AppDbContext>(options =>
           options.UseSqlServer("Data source= DESKTOP-S2PU99M\\SQLSERVER;Initial catalog =HomeAssistant;Integrated security = true;TrustServerCertificate=True"));
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<DeviceService>();
            builder.Services.AddScoped<RoomService>();
            builder.Services.AddScoped<OtpService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<UserService>();
            var configuration = builder.Configuration;

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
