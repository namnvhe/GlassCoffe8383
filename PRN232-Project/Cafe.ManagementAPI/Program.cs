using Cafe.BusinessObjects.Models;
using Cafe.DataAccess.DAO;
using Cafe.Repositories.IRepository;
using Cafe.Repositories.Repository;
using Cafe.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Initialize JWT Configuration Service
JwtConfigurationService.Initialize(builder.Configuration);
var businessObjectsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Cafe.BusinessObjects", "appsettings.json");
Console.WriteLine($"Checking path: {businessObjectsPath}");
if (File.Exists(businessObjectsPath))
{
    builder.Configuration.AddJsonFile(businessObjectsPath, optional: false, reloadOnChange: true);
}
else
{
    throw new FileNotFoundException("appsettings.json not found in Cafe.BusinessObjects directory.");
}

// Load the local appsettings.json (for logging and AllowedHosts)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}");
builder.Services.AddDbContext<CoffeManagerContext>(option =>
{
    option.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string not found."));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("CORSPolicy", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed((host) => true));
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Coffee Manager API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Vui lòng nhập token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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

builder.Services
    .AddAuthentication(config =>
    {
        config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<AuthDAO>();
builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<CoffeeTableDAO>();
builder.Services.AddScoped<DiscountDAO>();
builder.Services.AddScoped<DrinkTypeDAO>();
builder.Services.AddScoped<IngredientDAO>();
builder.Services.AddScoped<MenuItemDAO>();
builder.Services.AddScoped<MenuItemImageDAO>();
builder.Services.AddScoped<OrderDAO>();
builder.Services.AddScoped<OrderItemDAO>();
builder.Services.AddScoped<OrderItemToppingDAO>();
builder.Services.AddScoped<ToppingDAO>();
// Scoped - Tạo mới cho mỗi HTTP request
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICoffeeTableRepository, CoffeeTableRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IDrinkTypeRepository, DrinkTypeRepository>();
builder.Services.AddScoped<IIngredientRepository, IngredientRepository>();
builder.Services.AddScoped<IMenuItemImageRepository, MenuItemImageRepository>();
builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderItemToppingRepository, OrderItemToppingRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IToppingRepository, ToppingRepository>();

// Transient - Tạo mới mỗi lần inject
builder.Services.AddTransient<EmailService>();

// Singleton - Chỉ tạo một instance duy nhất
builder.Services.AddSingleton<JwtConfigurationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CoffeManagerContext>();
    // Đảm bảo cơ sở dữ liệu được tạo
    context.Database.Migrate();
    // Kiểm tra xem tài khoản quản trị viên có tồn tại không
    if (!context.Users.Any(u => u.Email == "vulqhe170481@fpt.edu.vn"))
    {
        // Create the Admin account with hashed password
        var adminPassword = "AdminPassword123!";
        var hashedPassword = AuthDAO.HashPassword(adminPassword); 
        var adminUser = new User
        {
            FullName = "System Administrator",
            Email = "vulqhe170481@fpt.edu.vn",
            Phone = null,
            PasswordHash = hashedPassword,
            IsActive = true,
            Role = "Admin",
            PasswordResetToken = null,
            PasswordResetTokenExpiry = null,
            LastLoginAt = DateTime.Now,
            IsEmailVerified = true
        };
        context.Users.Add(adminUser);
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CORSPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();