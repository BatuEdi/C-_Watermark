using Microsoft.EntityFrameworkCore;
using RabbitMq.Watermark.BackgroundServices;
using RabbitMq.Watermark.Models;
using RabbitMq.Watermark.Services;
using RabbitMQ.Client;
using Serilog;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: DateTime.Now.ToString())
    .CreateLogger();


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri
    (builder.Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true});  //ASync kullandýðýmýz için bu kýsmý true yaptýk
builder.Services.AddSingleton<RabbitMQClientService>();
builder.Services.AddHostedService<ImageWatermarkProcessBackgroundService>();
builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("productDb");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
