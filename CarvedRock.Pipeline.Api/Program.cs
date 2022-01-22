using CarvedRock.Api.Domain;
using CarvedRock.Api.Interfaces;
using CarvedRock.Api.Middleware;
using CarvedRock.Pipeline.Api.Middleware;
using Microsoft.AspNetCore.StaticFiles;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// pjmx - start
var seqIngestionPath = $"http://{Environment.GetEnvironmentVariable("MY_SEQ_SERVICE_HOST")}";
var ingestionPort = Environment.GetEnvironmentVariable("MY_SEQ_SERVICE_PORT_INGESTION");
if (string.IsNullOrWhiteSpace(ingestionPort) == false)
{
    seqIngestionPath += $":{ingestionPort}";
}

// pjmx - end

var name = typeof(Program).Assembly.GetName().Name;
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithProperty("Assembly", name)
    .WriteTo.Seq(serverUrl: seqIngestionPath)
    .WriteTo.Console()
    .CreateLogger();

// Add services to the container.
// pjm - start
builder.Services.AddScoped<IProductLogic, ProductLogic>();
builder.Services.AddScoped<IQuickOrderLogic, QuickOrderLogic>();
// pjm - end



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();
app.UseRouting();
app.UseMiddleware<GlobalErrorHandlerMiddleware>()
   .UseSerilogRequestLogging(opts =>
   {
       opts.GetLevel = LogHelper.CustomGetLevel;
   })
   .UseEndpoints(endpoints =>
   {
       endpoints.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
       {
           Predicate = (_) => false
       });
       endpoints.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
       {
           Predicate = (_) => false
       });
       endpoints.MapHealthChecks("/health/startup", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
       {
           Predicate = (_) => false
       });
   });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".yaml"] = "application/yaml";

app.UseStaticFiles(new StaticFileOptions()
{
    ContentTypeProvider = provider
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
