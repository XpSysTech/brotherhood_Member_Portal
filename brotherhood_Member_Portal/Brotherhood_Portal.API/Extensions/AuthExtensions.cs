using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Brotherhood_Portal.API.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // var jwtSettings = configuration
        //     .GetSection("Jwt")
        //     .Get<JwtSettings>()
        //     ?? throw new Exception("JWT configuration missing");

        var jwtSettings = new JwtSettings
        {
            TokenKey = configuration["Jwt:TokenKey"],
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            ExpiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"] ?? "15")
        };

        if (string.IsNullOrWhiteSpace(jwtSettings.TokenKey))
            throw new Exception("JWT TokenKey is missing");

﻿using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Brotherhood_Portal.API.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // var jwtSettings = configuration
        //     .GetSection("Jwt")
        //     .Get<JwtSettings>()
        //     ?? throw new Exception("JWT configuration missing");

        var jwtSettings = new JwtSettings
        {
            TokenKey = configuration["Jwt:TokenKey"],
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            ExpiryMinutes = int.Parse(configuration["Jwt:ExpiryMinutes"] ?? "15")
        };

        if (string.IsNullOrWhiteSpace(jwtSettings.TokenKey))
            throw new Exception("JWT TokenKey is missing");

	services.AddAuthentication(options =>
	{
	    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
	    options.TokenValidationParameters = new TokenValidationParameters
	    {
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(
		    Encoding.UTF8.GetBytes(jwtSettings.TokenKey)),

		ValidateIssuer = true,
		ValidIssuer = jwtSettings.Issuer,

		ValidateAudience = true,
		ValidAudience = jwtSettings.Audience,

		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero
	    };
	});

        return services;
    }
}

        return services;
    }
}
