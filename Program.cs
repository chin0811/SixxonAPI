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
//���UDI
var serviceAssembly = typeof(IUserService).Assembly;

foreach (var type in serviceAssembly.GetTypes())
{
    var interfaces = type.GetInterfaces();
    foreach (var iface in interfaces)
    {
        if (iface.Name == $"I{type.Name}") // �Ҧp�GIUserService �� UserService
        {
            builder.Services.AddScoped(iface, type);
        }
    }
}
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IUserRepository, FakeUserRepository>();
//���
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


app.UseDefaultFiles();   // ���� index.html
app.UseStaticFiles();    // �����R�A�ɮ�
app.MapFallbackToFile("index.html"); // SPA �䴩�e�ݸ���

app.Run();
