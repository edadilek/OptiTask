using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using OptiTask.Services;
using DataAccessLayer.Interface;
using DataAccessLayer.Repository;
using OptiTask.Middlewares;

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


// **2. Redis Baðlantýsýný Yapýlandýrýn**
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnection = builder.Configuration.GetConnectionString("RedisConnection");
    try
    {
        return ConnectionMultiplexer.Connect(redisConnection);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Redis baðlantýsý baþarýsýz oldu: {redisConnection}", ex);
    }
});


// **3. Servisleri DI Konteynerine Ekleyin**


builder.Services.AddScoped<WorkloadService>();
builder.Services.AddScoped<TaskService>();


// **4. Varsayýlan Ayarlarý Ekleyin**
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// **5. HTTP Request Pipeline'ý Yapýlandýrýn**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
