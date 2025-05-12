using Microsoft.EntityFrameworkCore;
using WHUChat.Server.Hubs;
using WHUChat.Server.Data;
using WHUChat.Server.Mappings;
using WHUChat.Server.Repositories;
using WHUChat.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// ��ӿ�������SignalR��Swagger��AutoMapper
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// ������ݿ������ģ�ʹ�� Pomelo��
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))));

// ע�����
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// ��̬��Դ
app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");
app.MapHub<ChatHub>("/chatHub");

app.Run();
