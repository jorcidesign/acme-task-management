using AcmeTaskApi.Application.Interfaces.Repositories;
using AcmeTaskApi.Application.Interfaces.Services;
using AcmeTaskApi.Application.Services;
using AcmeTaskApi.Domain.Enums;
using AcmeTaskApi.Infrastructure.Persistence;
using AcmeTaskApi.Infrastructure.Repositories;
using AcmeTaskApi.Infrastructure.Services;
using AcmeTaskApi.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración JWT desde appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() 
                  ?? new JwtSettings(); // Fallback por si no lo encuentra

var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

// 2. Base de Datos y Enum (Npgsql 8+)
// Nota: Reemplaza "DefaultConnection" por el nombre de variable que use tu DevOps,
// usualmente en contenedores se inyecta directo, pero asumiremos que existe.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=acme-db;Database=acme_task_db;Username=acme_user;Password=acme_pass";

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.MapEnum<TaskStatus>("task_status");
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dataSource));

// 3. Inyección de Dependencias (El corazón de la Arquitectura Limpia)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// 4. Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// 5. Controladores y Formateo JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Esto permite que el enum TaskStatus se vea como "InProgress" en Postman, no como "1"
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

// 6. Pipeline de Middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>(); // Manejo global de errores (404, 401, 500)
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); // Cors para Angular
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();