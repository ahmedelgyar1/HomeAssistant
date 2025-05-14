using HomeAssistant.Controllers;
using Microsoft.EntityFrameworkCore;
using HomeAssistant.Services;
using System.Net.Http.Headers;


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
           options.UseSqlServer("Server=db18834.public.databaseasp.net; Database=db18834; User Id=db18834; Password=Lp5#@Xc8m+6B; Encrypt=False; MultipleActiveResultSets=True;"));
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient<MLService>()
     .ConfigurePrimaryHttpMessageHandler(() =>
     {
         return new HttpClientHandler
         {
             ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
         };
     });
            builder.Services.AddScoped<DeviceService>();
            builder.Services.AddScoped<RoomService>();
            builder.Services.AddScoped<OtpService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<UserService>();

            var configuration = builder.Configuration;
         
            var app = builder.Build();
            app.UseCors("AllowAll");
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
