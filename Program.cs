using Oracle.ManagedDataAccess.Client;
using sixxonAPI.Interfaces;
using sixxonAPI.Repositories;
using sixxonAPI.Services;
using SixxonAPI.Interfaces;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//註冊DI
var serviceAssembly = typeof(IUserService).Assembly;

foreach (var type in serviceAssembly.GetTypes())
{
    var interfaces = type.GetInterfaces();
    foreach (var iface in interfaces)
    {
        if (iface.Name == $"I{type.Name}") // 例如：IUserService 對 UserService
        {
            builder.Services.AddScoped(iface, type);
        }
    }
}
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IUserRepository, FakeUserRepository>();
//跨域
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalDev", policy =>
    {
        policy.WithOrigins(
                "https://localhost:3000",
                "https://localhost:3001",
                "https://localhost:3443"
            )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
//DB
builder.Services.AddScoped<IDbConnection>(sp =>
    new OracleConnection(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowLocalDev");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.UseDefaultFiles();   // 提供 index.html
app.UseStaticFiles();    // 提供靜態檔案
app.MapFallbackToFile("index.html"); // SPA 支援前端路由

app.Run();
