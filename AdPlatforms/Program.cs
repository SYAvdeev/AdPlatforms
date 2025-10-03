using AdPlatforms.Application.Abstractions;
using AdPlatforms.Application.Services;
using AdPlatforms.Domain.Entities;
using AdPlatforms.Infrastructure.Implementations;
using AdPlatforms.Mappings;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(options => 
{
    options.MultipartBodyLengthLimit = 1024 * 1024 * 1024; // 1 GB
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMapping();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ITrieCacheBuilder<string, Advertising>, TrieCacheBuilder<string, Advertising>>();
builder.Services.AddSingleton<IAdvertisingService, AdvertisingService>();
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Run();