using dpWorld.API.Filter;
using dpWorld.Application.Mapping;
using dpWorld.Application.Services.Abstruct;
using dpWorld.Application.Services.Implmentation;
using dpWorld.Data.Context;
using dpWorld.Data.Models;
using dpWorld.Infrastructure;
using dpWorld.Infrastructure.Auth;
using dpWorld.Infrastructure.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace dpWorld.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //////////////////////////////////////////////////////////////
            // Add Services

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // EF Core DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // JWT Settings
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("JwtSettings"));


            ///add Application Services to add employee
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();


           // builder.Services.AddScoped<IAttendanceService, AttendanceService>();

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key) || Encoding.UTF8.GetBytes(jwtSettings.Key).Length < 32)
            {
                throw new Exception("Invalid or weak JWT Key. Key must be at least 256 bits (32 bytes).");
            }

            // JWT Token Generator
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // Infrastructure (Repositories, Services, etc.)
            builder.Services.AddInfrastructure();

            //////////////////////////////////////////////////////////////
            // Add API and Swagger

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                // ... your existing config ...
                c.OperationFilter<FileUploadOperationFilter>();
            });
            var app = builder.Build();

            //////////////////////////////////////////////////////////////
            // Seed Roles + Admin User on Startup

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await AppSeeder.SeedRolesAsync(services);
                await AppSeeder.SeedAdminUserAsync(services);
            }

            //////////////////////////////////////////////////////////////
            // Configure HTTP Pipeline

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // Enable JWT Auth
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
