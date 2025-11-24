using ImageConversion.Data;
using ImageConversion.Services.Hubs;
using ImageConversion.Services.ImageFile.Extensions;
using ImageConversion.Services.RabbitMQ.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// PostgreSQL connection string
var connectionString = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<ImageConversionContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddImageFileServices();
builder.Services.AddRabbitMQServices();

builder.Services.AddControllers();
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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
app.UseCors("AllowAngularApp");
app.MapHub<ImageConversionHub>("/hubs/imageConversion");
app.Run();
