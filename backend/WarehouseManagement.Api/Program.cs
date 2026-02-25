using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Api.Extensions;
using WarehouseManagement.Api.Middleware;
using WarehouseManagement.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddWarehouseManagementControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddWarehouseManagementDatabase(builder.Configuration);
builder.Services.AddWarehouseManagementMappings();
builder.Services.AddWarehouseManagementAuthentication(builder.Configuration);
builder.Services.AddWarehouseManagementCors();
builder.Services.AddWarehouseManagementRepositories();
builder.Services.AddWarehouseManagementApplicationServices();

var app = builder.Build();

// Apply migrations on app start
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors(WarehouseManagementServiceCollectionExtensions.AllowFrontendPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
