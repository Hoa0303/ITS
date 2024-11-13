using ITS_BE.Data;
using ITS_BE.DataSeeding;
using ITS_BE.Library;
using ITS_BE.Mapping;
using ITS_BE.Models;
using ITS_BE.Repository.BrandRepository;
using ITS_BE.Repository.CartItemRepository;
using ITS_BE.Repository.CategoryRepository;
using ITS_BE.Repository.ColorRespository;
using ITS_BE.Repository.FavoriteRepository;
using ITS_BE.Repository.ImageRepository;
using ITS_BE.Repository.LogRepository;
using ITS_BE.Repository.OrderRepository;
using ITS_BE.Repository.ProductColorRepository;
using ITS_BE.Repository.ProductDetailRepository;
using ITS_BE.Repository.ProductRepository;
using ITS_BE.Repository.ReceiptRepository;
using ITS_BE.Repository.ReviewRepository;
using ITS_BE.Repository.UserRepository;
using ITS_BE.Services.AuthSevices;
using ITS_BE.Services.Brands;
using ITS_BE.Services.Caching;
using ITS_BE.Services.Carts;
using ITS_BE.Services.Categories;
using ITS_BE.Services.Colors;
using ITS_BE.Services.History;
using ITS_BE.Services.Orders;
using ITS_BE.Services.Payment;
using ITS_BE.Services.Products;
using ITS_BE.Services.Receipts;
using ITS_BE.Services.Reviews;
using ITS_BE.Services.SendEmail;
using ITS_BE.Services.Statistics;
using ITS_BE.Services.Users;
using ITS_BE.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(e =>
{
    e.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer abcdef12345\""
    });
    e.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[] { }
            }
        });
});

builder.Services.AddCors(opt => opt.AddPolicy("MyCors", opt =>
{
    opt.WithOrigins("http://localhost:5173", "http://localhost:5174")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials();
}));

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddIdentity<User, Role>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 6;
    opt.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<MyDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ?? ""))
    };
});

builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<ISendEmailService, SendEmailService>();
builder.Services.AddSingleton<ICachingService, CachingService>();

builder.Services.AddAutoMapper(typeof(Mapping));

builder.Services.AddScoped<IFileStorage, FileStorage>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICateroryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IColorService, ColorService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDeliveryAddressRepository, DeliveryAddressRepository>();
builder.Services.AddScoped<IProductColorRepository, ProductColorRepository>();
builder.Services.AddScoped<IProductDetailRepository, ProductDetailRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<ICateroryRepository, CaterogyRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IFavoriterRepository, FavoriterRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IColorRepository, ColorRepository>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
builder.Services.AddScoped<IReceiptDetailRepository, ReceiptDetailRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogDetailRepository, LogDetailRepository>();

builder.Services.AddScoped<IVNPayLibrary, VNPayLibrary>();

var app = builder.Build();

DataSeeding.Initialize(app.Services).Wait();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseCors("MyCors");
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();