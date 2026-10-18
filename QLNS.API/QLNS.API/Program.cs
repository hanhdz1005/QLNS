using Microsoft.OpenApi.Models;
using QLNS.Infrastructure;
using QLNS.Infrastructure.Respository;
using QLNS.Core.Interfaces;
using QLNS.Core.Dto;
using QLNSCore.Entities;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new QLNS.API.Converter.TimeSpanJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.InfrastructureConfiguration(builder.Configuration);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Authorization + Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Full.Access",
        policy => policy.RequireClaim("permission", "FULL.ACCESS"));
    options.AddPolicy("Asset.View",
        policy => policy.RequireClaim("permission", "ASSET.VIEW"));
    options.AddPolicy("Asset.Create",
        policy => policy.RequireClaim("permission", "ASSET.CREATE"));
    options.AddPolicy("Asset.Edit",
        policy => policy.RequireClaim("permission", "ASSET.EDIT"));
    options.AddPolicy("Asset.Delete",
        policy => policy.RequireClaim("permission", "ASSET.DELETE"));
});


// Swagger JWT
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:5173", // port Vue host
                    "https://localhost:5173",
                    "https://localhost:8008",  // port Blazor Host
                    "http://localhost:8008"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});


var app = builder.Build();

app.UseMiddleware<QLNS.API.Middleware.GlobalExceptionHandler>();

app.UseCors("AllowAll");
await InfrastructureRequistration.InfrastructureConfigMiddleware(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();
app.Run();
