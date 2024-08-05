using DepartmentStatusService.Application.Services;
using DepartmentStatusService.Domain.Abstractions;
using DepartmentStatusService.Persistence;
using DepartmentStatusService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DepartmentStatusDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(DepartmentStatusDbContext)));
    });


builder.Services.AddScoped<IDepartmentStatusRepository, DepartmentStatusRepository>();
builder.Services.AddScoped<IDepartmentStatusService, DepartmentStatusService.Application.Services.DepartmentStatusService>();
builder.Services.AddHostedService<DepartmentStatusBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors(x =>
{
    x.WithHeaders().AllowAnyHeader();
    x.WithOrigins("http://localhost:5229");
    x.WithMethods().AllowAnyMethod();
});
app.Run();