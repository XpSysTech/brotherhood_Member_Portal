using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Application.Services;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Context;
using Brotherhood_Portal.Infrastructure.Data;
using Brotherhood_Portal.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDBContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddIdentityCore<AppUser>(opt => 
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDBContext>();

builder.Services.AddCors();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Token key not found in configuration - Program.cs");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
    .AddPolicy("RequirePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Configure CORS to allow requests from the Angular client
app.UseCors(policy =>
    policy.WithOrigins("http://localhost:4200", "https://localhost:4200") // Angular dev server URL
          .AllowAnyHeader()
          .AllowAnyMethod()
    );
app.UseAuthentication(); //Who are you?
app.UseAuthorization(); //Are you allowed?

app.MapControllers();

//using var scope = app.Services.CreateScope();
//var services = scope.ServiceProvider;
//try
//{
//    var context = services.GetRequiredService<AppDBContext>();
//    var userManager = services.GetRequiredService<UserManager<AppUser>>();
//    await context.Database.MigrateAsync();
//    await Seed.SeedUsers(userManager);
//}
//catch (Exception ex)
//{
//    var logger = services.GetRequiredService<ILogger<Program>>();
//    logger.LogError(ex, "An error occured during migration");
//}

app.Run();
