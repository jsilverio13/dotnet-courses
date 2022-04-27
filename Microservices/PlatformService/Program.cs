using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
var env = builder.Environment;
bool isProd = env.IsProduction();


if (isProd)
{
    Console.WriteLine("--> Using SqlServer Db");
    services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Using InMem Db");
    services.AddDbContext<AppDbContext>(opt =>
         opt.UseInMemoryDatabase("InMem"));
}

services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
services.AddScoped<IPlatformRepo, PlatformRepo>();
services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
services.AddSingleton<IMessageBusClient, MessageBusClient>();
services.AddControllers();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
});

builder.Logging.AddConsole();

Console.WriteLine($"--> CommandService Endpoint {configuration["CommandService"]}");



var app = builder.Build();


app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));


app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    // endpoints.MapGrpcService<GrpcPlatformService>();

    // endpoints.MapGet("/protos/platforms.proto", async context =>
    // {
    //     await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
    // });

});

PrepDb.PrepPopulation(app, isProd);

app.Run();

