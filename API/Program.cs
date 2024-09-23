using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.Design;
using API.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
namespace API {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.IncludeFields = true;
#if DEBUG
                options.JsonSerializerOptions.WriteIndented = true;
#endif
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAuthentication();

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
