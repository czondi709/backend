using KozossegAPI.Auth;
using KozossegAPI.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace KozossegAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("probasql");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ApplicationException("Adatbázis kapcsolat nem található!");
            }
            
            builder.Services.AddDbContext<KozossegDbContext>(options => 
                options.UseMySQL(connectionString));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options => 
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            AddJwtAuthentication(builder);
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();

        }

        private static void AddJwtAuthentication(WebApplicationBuilder builder)
        {
            var secretKey = builder.Configuration["Auth:Jwt:Key"];
            var issuer = builder.Configuration["Auth:Jwt:Issuer"];
            var audience = builder.Configuration["Auth:Jwt:Audience"];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new ApplicationException("Hiba: Hiányzik a JWT konfiguráció az appsettings.json-bõl!");
            }

            var tokenManager = new TokenManager(builder.Configuration);
            builder.Services.AddSingleton(tokenManager);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                foreach (var permission in tokenManager.Permissions)
                {
                    options.AddPolicy(permission, policy => policy.RequireClaim("permission", permission));
                }
            });


            

            builder.Services.AddSwaggerGen(
                options =>
                {
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                });
        }
    }
}