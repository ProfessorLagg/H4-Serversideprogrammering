using API.Data;
using API.Services;

using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

using System.Security.Authentication;
namespace API {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IHashingService, LocalHashingService>();
            builder.Services.AddScoped<IPageContentService, FilePageContentService>();
            builder.Services.AddScoped<H4AuthService>();
            builder.Services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.IncludeFields = true; // How and why this is not on by default pains me to no end
#if DEBUG
                options.JsonSerializerOptions.WriteIndented = true;
#endif
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication();
            builder.Services.AddHttpLogging(options => {
                options.LoggingFields = HttpLoggingFields.All;
            });
            //builder.Services.AddHttpLogging(o => { });

            string? connectionString = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<H4serversideTodoContext>(options =>
                options.UseSqlServer(connectionString)
            );

            builder.WebHost.UseKestrel((context, serverOptions) => {
                serverOptions
                .Configure(context.Configuration.GetSection("Kestrel"))
                .Endpoint("HTTPS", listenOptions => {
                    listenOptions.HttpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                });
            });

            var app = builder.Build();
            app.UseHttpLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.UseFileServer();

            app.Run();
        }
    }
}
