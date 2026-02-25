using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Mappings;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Repositories.Implementations;
using WarehouseManagement.Infrastructure.Repositories.Interfaces;

namespace WarehouseManagement.Api.Extensions;

public static class WarehouseManagementServiceCollectionExtensions
{
    public const string AllowFrontendPolicy = "AllowFrontend";

    public static IServiceCollection AddWarehouseManagementControllers(this IServiceCollection services)
    {
        services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(entry => entry.Value?.Errors.Count > 0)
                        .ToDictionary(
                            entry => entry.Key,
                            entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

                    return new BadRequestObjectResult(new
                    {
                        statusCode = StatusCodes.Status400BadRequest,
                        message = "Validation failed",
                        errors
                    });
                };
            });

        return services;
    }

    public static IServiceCollection AddWarehouseManagementDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly("WarehouseManagement.Infrastructure")));

        return services;
    }

    public static IServiceCollection AddWarehouseManagementMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());
        return services;
    }

    public static IServiceCollection AddWarehouseManagementAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"] ?? "YourSecretKeyHere123456789012345678901234567890";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "WarehouseManagementApi";
        var jwtAudience = configuration["Jwt:Audience"] ?? "WarehouseManagementClient";

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = JsonSerializer.Serialize(new
                        {
                            statusCode = StatusCodes.Status401Unauthorized,
                            message = "Authentication is required to access this resource"
                        });

                        await context.Response.WriteAsync(response);
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = JsonSerializer.Serialize(new
                        {
                            statusCode = StatusCodes.Status403Forbidden,
                            message = "You do not have permission to access this resource"
                        });

                        await context.Response.WriteAsync(response);
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddWarehouseManagementCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(AllowFrontendPolicy, policy =>
            {
                policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddWarehouseManagementRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<IHistoryRepository, HistoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static IServiceCollection AddWarehouseManagementApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<IHistoryService, HistoryService>();

        return services;
    }
}
