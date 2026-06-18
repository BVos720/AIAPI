using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using AIAPI.Data;
using AIAPI.Interfaces;
using AIAPI.Repositories;
using AIAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<SensoringDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IDetectionRepository, DetectionRepository>();
builder.Services.AddHttpClient<IMonitoringService, MonitoringService>();
builder.Services.AddHttpClient<IGeocodingService, GeocodingService>();

var app = builder.Build();

// Automatisch migrations uitvoeren bij opstarten
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SensoringDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
