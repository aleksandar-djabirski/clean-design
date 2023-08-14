using Claims.Infrastructure; // Make sure to add this reference at the top
using Claims.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Claims.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddApplicationLayerServices();

// Register infrastructure services, repositories, and DB contexts
builder.Services.AddInfrastructure(builder.Configuration);

// Swagger setup
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

// Ensure EF migrations are applied
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
    context.Database.Migrate();
}

app.Run();

public partial class Program { }