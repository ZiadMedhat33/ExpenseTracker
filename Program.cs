using ExpenseTracker.Model;
using ExpenseTracker.Model.repository;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration and DB setup
var config = builder.Configuration;
var connectionString = config.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories & Services
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ExpenseRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ExpenseService>();

// JWT Service
var jwtService = new JwtService(config);
builder.Services.AddSingleton(jwtService);

// Controllers
builder.Services.AddControllers();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = jwtService.GetValidationParameters();
    });

builder.Services.AddAuthorization();

// ✅ Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Expense Tracker API",
        Version = "v1"
    });

    // ✅ JWT bearer token support in Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter 'Bearer {your JWT token here}'"
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

var app = builder.Build();

// ⚠️ For demo: delete and recreate DB on startup
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
await context.Database.EnsureDeletedAsync();   // Deletes the database
await context.Database.EnsureCreatedAsync();   // Recreates it

// ✅ Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Expense Tracker API v1");
    options.RoutePrefix = "swagger"; // Swagger UI available at /swagger
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
