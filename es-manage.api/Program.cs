// Tujuan: Program utama yang dijalankan untuk API.

// Import library yang dibutuhkan
using es_manage.api.Repositories;
using es_manage.api.Context;
using es_manage.api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("Main") ?? throw new InvalidOperationException("String koneksi DB 'Main' tidak ditemukan / invalid. Cek file appsettings.json.");

// Koneksi ke database
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// Konfigurasi JWT
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
            ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value)),
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };
    });
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<ItemDepartmentRepository>();
builder.Services.AddScoped<ItemRepository>();
builder.Services.AddScoped<ItemSupplierRepository>();
builder.Services.AddScoped<ItemSupplier_TransactionRepository>();
builder.Services.AddScoped<SupplierRepository>();
builder.Services.AddScoped<BrandRepository>();
builder.Services.AddScoped<PasswordChangeRepository>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<HashingService>();

builder.Services.AddTransient<IClaimsTransformation, RoleClaimService>();

// Option untuk CORS (Cross-Origin Resource Sharing)
var allOrigins = "allowOrigins";
builder.Services.AddCors(opt => opt.AddPolicy(allOrigins, policy =>
{
	policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(allOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
