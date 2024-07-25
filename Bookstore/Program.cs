
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
            builder.Services.AddDbContext<BookstoreDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
            builder.Services.AddSingleton(jwtOptions);
            builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, op =>
            {
                op.SaveToken = true;
                op.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))

                };
            });

            builder.Services.AddAuthorization(op =>
            {
                op.AddPolicy("AllowCustomer", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        return context.User.FindFirst(ClaimTypes.Role).Value == "Customer";
                    });
                });
            });
            //.Services.AddLogging();
            //builder.Logging.AddConsole();
            builder.Services.AddMemoryCache();

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
