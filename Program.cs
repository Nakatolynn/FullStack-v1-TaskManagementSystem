using Microsoft.EntityFrameworkCore;
using System;
using TaskManagementAPI.Data;
using TaskManagementAPI.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ? Register CORS policy *before* building
builder.Services.AddCors(options =>
{
    options.AddPolicy("taskmanagement", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:3000", "http://localhost:3001")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ? Register DbContext *before* building the app
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDataContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<ITaskManagementService, TaskManagementService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManagement API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


app.UseHttpsRedirection();

app.UseCors("taskmanagement");

app.UseAuthorization();

app.MapControllers();

app.Run();
