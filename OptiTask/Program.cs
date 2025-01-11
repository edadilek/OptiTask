using DataAccessLayer;
using DataAccessLayer.Entity;
using DataAccessLayer.Interface;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OptiTask.Middlewares;
using OptiTask.Services;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<IWorkloadRepository, WorkloadRepository>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<WorkloadService>();
builder.Services.AddScoped<TaskService>();


// **2. Redis Baðlantýsýný Yapýlandýrýn**
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
.AddJwtBearer();

var app = builder.Build();

var logger = app.Logger;

// **5. HTTP Request Pipeline'ý Yapýlandýrýn**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();


    var application = app.Services.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

    var pendingMigrations = await application.Database.GetPendingMigrationsAsync();
    if (pendingMigrations != null)
        await application.Database.MigrateAsync();
}

Console.WriteLine(app.Configuration["Jwt:Key"]);

app.UseCors("AllowAll");

app.Use(async (context, next) =>
{
    logger.LogInformation("Authentication Middleware");
    var jwtToken = context.Request.Cookies["jwt"];
    if (jwtToken != null)
    {
        var jwtHandler = new JwtSecurityTokenHandler();

        var validateParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("SuperSecretKeyAmAboutToGoCr@zySickOfThisRules")),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateAudience = false,
            ValidateIssuer = false,
        };

        var principalToken = jwtHandler.ValidateToken(jwtToken, validateParams, out SecurityToken validatedToken);
        if (validatedToken != null)
        {
            context.Response.HttpContext.User = principalToken;
        }
    }
    await next.Invoke();
});


app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
