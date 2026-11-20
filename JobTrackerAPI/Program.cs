using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using JobTrackerAPI.Data;
using JobTrackerAPI.Repositories.Implementations;
using JobTrackerAPI.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configure Swagger / OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JobTracking API",
        Version = "v1",
        Description = "ASP.NET Core Web API for tracking job applications using the Repository Pattern and Entity Framework Core."
    });

    // Include XML comments in Swagger UI
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Register Entity Framework Core DbContext
var connectionString = builder.Configuration.GetConnectionString("JobTrackingDb")
    ?? throw new InvalidOperationException("Connection string 'JobTrackingDb' not found in configuration.");

builder.Services.AddDbContext<JobTrackingDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Register Repository & Unit of Work dependencies
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobNotesRepository, JobNotesRepository>();
builder.Services.AddScoped<IJobTypeRepository, JobTypeRepository>();
builder.Services.AddScoped<IJobLocationRepository, JobLocationRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "JobTracking API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "JobTracking API Documentation";
    });

    // Redirect root URL to Swagger UI
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
