using DataAccessLayer;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OptiTask.Middlewares;
using OptiTask.Services;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// **1. Veritabaný Baðlantýsýný Yapýlandýrýn (PostgreSQL için)**
builder.Services.AddNpgsql<AppDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"), null, optionsAction =>
{
    optionsAction.UseNpgsql(builder =>
    {
        builder.MigrationsAssembly("DataAccessLayer");

        //builder.MigrationsAssembly(typeof(Program).Assembly.GetName().Name);
    });
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITasksRepository, TasksRepository>();
builder.Services.AddScoped<ITaskAssignmentRepository, TaskAssignmentRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
builder.Services.AddScoped<ITeamProjectRepository, TeamProjectRepository>();
builder.Services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();


// **2. Redis Baðlantýsýný Yapýlandýrýn**


//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//{
//    var redisConnection = builder.Configuration.GetConnectionString("RedisConnection");
//    try
//    {
//        return ConnectionMultiplexer.Connect(redisConnection);
//    }
//    catch (Exception ex)
//    {
//        throw new InvalidOperationException($"Redis baðlantýsý baþarýsýz oldu: {redisConnection}", ex);
//    }
//});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var redisConnection = configuration.GetConnectionString("RedisConnection");

    if (string.IsNullOrEmpty(redisConnection))
        throw new InvalidOperationException("Redis connection string is not configured");

    var options = ConfigurationOptions.Parse(redisConnection);
    options.AbortOnConnectFail = false; // Baðlantý hatalarýna karþý daha toleranslý ol

    return ConnectionMultiplexer.Connect(options);
});







// **3. Servisleri DI Konteynerine Ekleyin**


builder.Services.AddScoped<WorkloadService>();
builder.Services.AddScoped<TaskService>();
//builder.Services.AddScoped<TaskService>();


// **4. Varsayýlan Ayarlarý Ekleyin**
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'ýnýzý Bearer <token> formatýnda girin."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Authentication and Authorization
var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents()
    {
        OnMessageReceived = ctx =>
        {
            if (ctx.Request.Cookies.TryGetValue("jwt", out var token))
            {
                var handler = new JwtSecurityTokenHandler();

                var readToken = handler.ReadJwtToken(token);
                Console.WriteLine($"Token ValidTo: {readToken.ValidTo} | System UTC Now: {DateTime.UtcNow}");

                try
                {
                    var claims = handler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyAmAboutToGoCr@zySickOfThisRules")),
                        ValidateIssuerSigningKey = true
                    }, out var validatedToken);

                    Console.WriteLine("Token manually validated.");
                    ctx.Principal = new ClaimsPrincipal(claims);
                    ctx.Success();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Manual validation failed: {ex.Message}");
                }
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine($"Token authentication failed: {ctx.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// **5. HTTP Request Pipeline'ý Yapýlandýrýn**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Console.WriteLine(app.Configuration["Jwt:Key"]);

app.UseCors("AllowAll");

app.Use(async (context, next) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        var roles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        Console.WriteLine($"Roles: {string.Join(", ", roles)}");
    }
    await next.Invoke();
});


app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
