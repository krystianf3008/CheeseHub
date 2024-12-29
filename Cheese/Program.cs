using CheeseHub.Data;
using CheeseHub.Extensions;
using CheeseHub.Interfaces.Services;
using CheeseHub.Models.Role.DTOs;
using CheeseHub.Models.Role.Validators;
using CheeseHub.Models.User.DTOs;
using CheeseHub.Models.User.Validators;
using CheeseHub.Models.Video.DTOs;
using CheeseHub.Models.Video.Validators;
using CheeseHub.Services;
using FluentValidation;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using Microsoft.OpenApi.Models;
using System.Text;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Swashbuckle.AspNetCore.Swagger;

namespace CheeseHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            // Add services to the container.
            AuthenticationSettings authenticationSettings = new AuthenticationSettings();
            configuration.GetSection("JWT").Bind(authenticationSettings);
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });
            var connectionString = builder.Configuration.GetConnectionString("VideoDb");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
            builder.Services.AddSingleton(authenticationSettings);
            builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            builder.Services.AddScoped<IVideoService, VideoService>();
            builder.Services.AddScoped<IValidator<CreateVideoDTO>, CreateVideoDTOValidator>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IValidator<CreateRoleDTO>, CreateRoleDTOValidator>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IValidator<RegisterUserDTO>, RegisterUserDTOValidator>();
            builder.Services.AddScoped<IValidator<LoginUserDto>, LoginUserDtoValidator>();
            builder.Services.AddScoped<IJwtService, JwtService>();

            builder.Services.AddOpenApiDocument();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = authenticationSettings.Issuer,
                    ValidAudience = authenticationSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.Key))
                };
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });



            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                    options.RoutePrefix = string.Empty; 
                });
                DatabaseManagmentService.MigrationInitialisation(app);
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAllOrigins");

            app.UseHttpsRedirection();



            app.MapControllers();

            app.Run();
        }
    }
}
