using FoodDelivery.Repositories;
using FoodDelivery.Repositories.Implementations;
using FoodDelivery.Repositories.Interfaces;
using FoodDelivery.Service.Implementations;
using FoodDelivery.Service.Implements;
using FoodDelivery.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
/*using System.Security.Cryptography;
using System.Text;

var password = "12345678";

using var hmac = new HMACSHA512();
var salt = hmac.Key;
var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

Console.WriteLine("Salt (Base64):");
Console.WriteLine(Convert.ToBase64String(salt));

Console.WriteLine("Hash (Base64):");
Console.WriteLine(Convert.ToBase64String(hash));

return; // chặn app chạy tiếp
*/

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "FoodDelivery API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


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

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAddressRepository,AddressRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IFileService,FileService>();
builder.Services.AddScoped<IAddressService,AddressService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService,CartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();


// Review service
builder.Services.AddScoped<FoodDelivery.Service.Interfaces.IReviewService, FoodDelivery.Service.Implements.ReviewService>();
builder.Services.AddScoped<IShipperService, ShipperService>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        // 1. Xác minh Khóa Bí mật (Khóa quan trọng nhất!)
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenSecretKey"] ?? "default_secret_key_2025_food_delivery")),

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
//Kích hoạt CORS (Phải đặt trước UseAuthorization)
app.UseCors("AllowReactApp");
app.UseAuthentication(); //Kiểm tra bạn là ai 
app.UseAuthorization();// Kiểm tra bạn có quyền truy cập gì
app.MapControllers();// Kết nối các đường dẫn (Route) tới Controller

app.Run();

