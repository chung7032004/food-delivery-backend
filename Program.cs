
using FoodDelivery.Repositories;
using FoodDelivery.Repositories.Implementations;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Implementations;
using FoodDelivery.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Cấu hình kết nối Database Postgresql
var configuration = builder.Configuration;
builder.Services.AddDbContext<FoodContext>(options =>
    options.UseNpgsql(
        configuration.GetConnectionString("Default"),
        npgsqlOptions => npgsqlOptions.EnableRetryOnFailure()
    )
);

//Định nghĩa chính sách CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:5173") // Port của React
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        // 1. Xác minh Khóa Bí mật (Khóa quan trọng nhất!)
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenSecretKey"] ?? "default_secret_key_2025")),

        // 2. Xác minh Issuer và Audience
        ValidateIssuer = true,
        ValidIssuer = "foodApi",

        ValidateAudience = true,
        ValidAudience = "foodApi",
        
        // 3. Xác minh Thời gian sống (hết hạn)
        ValidateLifetime = true,
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseRouting();
//Kích hoạt CORS (Phải đặt trước UseAuthorization)
app.UseCors("AllowReactApp");
app.UseAuthentication(); //Kiểm tra bạn là ai 
app.UseAuthorization();// Kiểm tra bạn có quyền truy cập gì
app.MapControllers();// Kết nối các đường dẫn (Route) tới Controller

app.Run();
