
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Core.DomainLayer.Contracts;
using OrderManagement.Core.Service;
using OrderManagement.Core.ServiceAbstraction;
using OrderMangement.Api.Configuration;
using OrderMangement.Api.Services;
using Persistence.Data;
using Persistence.Repository;
using System.Text;

namespace OrderMangement.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container.

            // Controllers
            builder.Services.AddControllers();

            // API Explorer
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Database - Using In-Memory for simplicity
            builder.Services.AddDbContext<OrderManagementDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: "OrderManagementDb"));

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(OrderManagement.Core.Service.MappingProfile.MappingProfile));

            // Configuration
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

            // Unit of Work Registration
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // JWT Token Service Registration
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Service Registration
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<IUserService, UserService>();

            // JWT Authentication
            if (jwtSettings?.Secret != null)
            {
                var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = !string.IsNullOrEmpty(jwtSettings.Issuer),
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = !string.IsNullOrEmpty(jwtSettings.Audience),
                        ValidAudience = jwtSettings.Audience,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            }

            builder.Services.AddAuthorization();

            #endregion

            var app = builder.Build();

            #region Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}
