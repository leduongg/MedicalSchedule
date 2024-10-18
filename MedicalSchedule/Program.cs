
using MedicalScheduleAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MedicalSchedule
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            var key = configuration["Jwt:Key"];
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = signingKey,
                        RequireSignedTokens = true,
                    };
                });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(opts =>
            {
                opts.AddPolicy("CORSPolicy", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((host) => true));
            });

            builder.Services.AddDbContext<MedicalScheduleContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<MedicalScheduleContext>();
            builder.Services.AddHttpClient();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("CORSPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}