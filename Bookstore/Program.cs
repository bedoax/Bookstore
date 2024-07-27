using Bookstore.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Bookstore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Configure the Entity Framework Core context to use SQL Server with the connection string from configuration.
            builder.Services.AddDbContext<BookstoreDbContext>(option =>
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Retrieve JWT options from configuration and register them as a singleton service.
            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
            builder.Services.AddSingleton(jwtOptions);

            // Configure authentication to use JWT Bearer tokens.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(op =>
                {
                    // Save the token in the authentication properties.
                    op.SaveToken = true;
                    op.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Validate the issuer of the token.
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        // Validate the audience of the token.
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        // Validate the token's lifetime.
                        ValidateLifetime = true,

                        // Validate the token's signing key.
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
                    };
                });

            // Configure authorization policies.
            builder.Services.AddAuthorization(options =>
            {
                // Define a policy named "AllowCustomer" that checks for a specific role claim.
                options.AddPolicy("AllowCustomer", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        // Ensure the role claim exists and check if its value is "Customer".
                        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;
                        return roleClaim == "Customer";
                    });
                });
            });

            // Add in-memory caching services.
            builder.Services.AddMemoryCache();

            // Add controllers to the service collection.
            builder.Services.AddControllers();

            // Configure Swagger/OpenAPI for API documentation.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Enable Swagger in development mode.
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Enable HTTPS redirection.
            app.UseHttpsRedirection();

            // Enable authorization middleware.
            app.UseAuthorization();

            // Map controller routes.
            app.MapControllers();

            // Run the application.
            app.Run();
        }
    }
}
