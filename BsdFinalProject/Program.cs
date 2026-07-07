using BsdFinalProject.Data;
using BsdFinalProject.IRepository;
using BsdFinalProject.IRepository;
using BsdFinalProject.IService;
using BsdFinalProject.IService;
using BsdFinalProject.Repositories;
using BsdFinalProject.Services;
using FinalProject.Repositories;
using FinalProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;
using Confluent.Kafka;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Add services to the container.
builder.Host.UseSerilog(); // השתמש ב-Serilog

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

// Configure Redis
var redisHost = builder.Configuration["Redis:Host"] ?? "localhost";
var redisPort = int.Parse(builder.Configuration["Redis:Port"] ?? "6379");
var redisPassword = builder.Configuration["Redis:Password"] ?? "";

var redisConnectionString = $"{redisHost}:{redisPort}";
if (!string.IsNullOrEmpty(redisPassword))
{
    redisConnectionString += $",password={redisPassword}";
}

var redis = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddScoped<ICacheService, CacheService>();

// Rate limiting - Sliding Window per-client (by IP). Values can be configured in appsettings.json
var permitLimit = int.TryParse(builder.Configuration["RateLimiting:RequestLimit"], out var rl) ? rl : 100;
var windowMinutes = int.TryParse(builder.Configuration["RateLimiting:TimeWindowMinutes"], out var wm) ? wm : 1;
var segmentsPerWindow = 4; // segmentation for sliding window

builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        return new ValueTask();
    };

    options.AddPolicy<string>("SlidingWindow", httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString()
                ?? httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? "anonymous",
            _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromMinutes(windowMinutes),
                SegmentsPerWindow = segmentsPerWindow,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

// Add repository and service registrations
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGiftService, GiftService>();
builder.Services.AddScoped<IGiftRepository, GiftRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDonorRepository, DonorRepository>();
builder.Services.AddScoped<IDonorService, DonorService>();
builder.Services.AddScoped<IWinnerRepository, WinnerRepository>();
builder.Services.AddScoped<IWinnerService, WinnerService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

// Kafka Producer and Consumer
builder.Services.AddSingleton<BsdFinalProject.Services.IKafkaProducer, BsdFinalProject.Services.KafkaProducer>();
builder.Services.AddHostedService<BsdFinalProject.HostedServices.KafkaConsumerHostedService>();

// DbContext
builder.Services.AddDbContext<SaleContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT configuration
var key = builder.Configuration["Jwt:Key"];
if (!string.IsNullOrEmpty(key))
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrEmpty(builder.Configuration["Jwt:Issuer"]),
            ValidateAudience = !string.IsNullOrEmpty(builder.Configuration["Jwt:Audience"]),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Swagger UI at app root
    });
}

app.UseHttpsRedirection();
app.UseRouting();
// If a JWT is stored in an HttpOnly cookie, copy it into the Authorization header
// so the JWT Bearer middleware can authenticate it.
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("Authorization") && context.Request.Cookies.ContainsKey("access_token"))
    {
        var token = context.Request.Cookies["access_token"];
        if (!string.IsNullOrEmpty(token))
        {
            context.Request.Headers.Append("Authorization", "Bearer " + token);
        }
    }

    await next();
});

// Rate limiting middleware - run before authentication to avoid extra work on over-limit clients
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


