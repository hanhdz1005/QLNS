using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using QLNS.Infrastructure.Data.Config;
using QLNS.Infrastructure.Respository;
using QLNSCore.Entities;
using QLNS.Core.Interfaces;
using QLNS.Core.Interfaces.IService;
using QLNS.Core.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QLNS.Infrastructure
{
    public static class InfrastructureRequistration
    {
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
                );

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<ISalaryRepository, SalaryRepository>();
            services.AddScoped<IBonusPenaltyRepository, BonusPenaltyRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<ILeaveBalanceRepository, LeaveBalanceRepository>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ISalaryService, SalaryService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IBonusPenaltyService, BonusPenaltyService>();
            services.AddScoped<IPayrollService, PayrollService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ILeaveApprovalService, LeaveApprovalService>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<ILeaveBalanceService, LeaveBalanceService>();
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.AddMemoryCache();
            services.AddScoped<UserManager<AppUser>>();
            services.AddScoped<RoleManager<IdentityRole>>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();

                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                context.Response.ContentType = "application/json";

                                var errorCode = "UNAUTHORIZED";
                                string message = "Bạn chưa đăng nhập hoặc phiên làm việc đã hết hạn.";
                                if (context.AuthenticateFailure is SecurityTokenExpiredException)
                                {
                                    errorCode = "TOKEN_EXPIRED";
                                    message = "Token đã hết hạn. Vui lòng làm mới token hoặc đăng nhập lại.";
                                }
                                else if (context.AuthenticateFailure != null)
                                {
                                    errorCode = "INVALID_TOKEN";
                                    message = "Token xác thực không hợp lệ. Vui lòng đăng nhập lại.";
                                }

                                var errorList = new List<string>();
                                if (!string.IsNullOrEmpty(context.ErrorDescription))
                                    errorList.Add(context.ErrorDescription);
                                else if (context.AuthenticateFailure != null)
                                    errorList.Add(context.AuthenticateFailure.Message);
                                else
                                    errorList.Add("Yêu cầu xác thực: Token không được cung cấp hoặc không hợp lệ.");

                                var responsePayload = Result<object>.Failure(
                                    message: message,
                                    errors: errorList,
                                    errorCode: errorCode
                                );

                                var jsonOptions = new JsonSerializerOptions
                                {
                                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                };
                                await context.Response.WriteAsync(JsonSerializer.Serialize(responsePayload, jsonOptions));
                            }
                        },
                        OnForbidden = async context =>
                        {
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                context.Response.ContentType = "application/json";

                                var responsePayload = Result<object>.Failure(
                                    message: "Bạn không có quyền truy cập chức năng này.",
                                    errors: new List<string> { "Access denied. You do not have permission to perform this action." },
                                    errorCode: "FORBIDDEN"
                                );

                                var jsonOptions = new JsonSerializerOptions
                                {
                                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                                };
                                await context.Response.WriteAsync(JsonSerializer.Serialize(responsePayload, jsonOptions));
                            }
                        }
                    };
                });
            return services;
        }

        public static async Task InfrastructureConfigMiddleware(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<AppUser>>();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            await IdentitySeed.SeedAdminAsync(userManager, roleManager);
        }
    }
}
