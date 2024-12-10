using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using OptiTask.Services;
using DataAccessLayer.Interface;
using DataAccessLayer.Repository;
using OptiTask.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();

// **2. Redis Baðlantýsýný Yapýlandýrýn**
//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//{
//    var configuration = builder.Configuration.GetConnectionString("RedisConnection");
//    return ConnectionMultiplexer.Connect(configuration);
//});

// **3. Servisleri DI Konteynerine Ekleyin**


//builder.Services.AddScoped<WorkloadService>();

// **4. Varsayýlan Ayarlarý Ekleyin**
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Authentication and Authorization
var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                var token = context.HttpContext.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// **5. HTTP Request Pipeline'ý Yapýlandýrýn**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
