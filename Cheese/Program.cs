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
using CheeseHub.Middlewares;
using CheeseHub.Models.Category.DTOs;
using CheeseHub.Models.Category.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation.AspNetCore;
using System.Reflection;

namespace CheeseHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            AuthenticationSettings authenticationSettings = new AuthenticationSettings();
            configuration.GetSection("JWT").Bind(authenticationSettings);

            ///var connectionString = builder.Configuration.GetConnectionString("VideoDb");
            ///DEPLOY NA POTRZEBY OCENY PROJEKTU - UTWORZONA BAZA
            var connectionString = "Host=46.171.218.181;Port=5433;Username=postgres;Password=Duzymis123456;Database=VideoDb;TrustServerCertificate=True;";

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlServer(connectionString)
            options.UseNpgsql(connectionString)
            
            
            );
            builder.Services.AddCors(options =>
            {
                    
                options.AddPolicy("AllowAllOrigins",

                    builder =>
                    {

                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .WithExposedHeaders("www-authenticate");
                    });
            });
            builder.Services.AddSingleton(authenticationSettings);
            builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
            builder.Services.AddScoped(typeof(IReactionService<>), typeof(ReactionService<>));
            builder.Services.AddScoped<IVideoService, VideoService>();
            builder.Services.AddScoped<IValidator<CreateVideoDTO>, CreateVideoDTOValidator>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IValidator<CreateRoleDTO>, CreateRoleDTOValidator>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<ICommentReactionService, CommentReactionService>();
            builder.Services.AddScoped<IVideoReactionService, VideoReactionService>();
            builder.Services.AddScoped<IVideoViewService, VideoViewService>();
            

            builder.Services.AddFluentValidationAutoValidation(configuration =>
            {
                configuration.DisableBuiltInModelValidation = true;

                configuration.ValidationStrategy = SharpGrip.FluentValidation.AutoValidation.Mvc.Enums.ValidationStrategy.All;
                configuration.EnableBodyBindingSourceAutomaticValidation = true;

                configuration.EnableFormBindingSourceAutomaticValidation = true;

                configuration.EnableQueryBindingSourceAutomaticValidation = true;

                configuration.EnablePathBindingSourceAutomaticValidation = true;

                configuration.EnableCustomBindingSourceAutomaticValidation = true;

            });
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


            builder.Services.AddOpenApiDocument();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            })
             ;
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
            app.UseCors("AllowAllOrigins");

            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });

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

            app.UseMiddleware<TokenValidationMiddleware>();

            app.UseHttpsRedirection();



            app.MapControllers();

            app.Run();
        }
    }
}
