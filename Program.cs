using Inventory.Api.Common;
using Inventory.Api.Data;
using Inventory.Api.Services;
using InventoryManagement6th.Service;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));

// For Bulk Upload
builder.Services.AddScoped<ExcelTemplateService>();
builder.Services.AddScoped<ExcelUploadService>();

// Register your services
builder.Services.AddScoped<IAPInvoiceService, APInvoiceService>();

builder.Services.AddControllers()
    .AddJsonOptions(o => { o.JsonSerializerOptions.PropertyNamingPolicy = null; });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy
            .WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        );
});

// Custom validation response
builder.Services.Configure<ApiBehaviorOptions>(o =>
{
    o.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Global exception handler
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async ctx =>
    {
        var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
        ctx.Response.StatusCode = 500;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(
            ApiResponse.Fail<object>(500, ex?.Message ?? "Unexpected error")
        );
    });
});

// Use CORS early
app.UseCors("AllowAngular");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();