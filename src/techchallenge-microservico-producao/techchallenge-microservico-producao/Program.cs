using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using Infra.DatabaseConfig;
using techchallenge_microservico_producao.Repositories;
using techchallenge_microservico_producao.Repositories.Interfaces;
using techchallenge_microservico_producao.Services;
using techchallenge_microservico_producao.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Amazon.S3;
using Amazon.SQS;
using LocalStack.Client.Extensions;
using Infra.SQS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());
});

builder.Services.AddTransient<IProducaoRepository, ProducaoRepository>();
builder.Services.AddTransient<IProducaoService, ProducaoService>();

builder.Services.AddLocalStack(builder.Configuration);
builder.Services.AddAWSServiceLocalStack<IAmazonSQS>();
builder.Services.AddAWSServiceLocalStack<IAmazonS3>();
builder.Services.AddTransient<ISQSConfiguration, SQSConfiguration>();
builder.Services.AddHostedService<SqsListenerService>();

builder.Services.AddControllers();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.AddSwaggerGen(opts => opts.EnableAnnotations());

var connectionString = builder.Configuration.GetConnectionString("MyAppCs");
builder.Services.AddDbContextFactory<EFDbconfig>(options =>
   options.UseSqlServer(connectionString, b => b.MigrationsAssembly("techchallenge-microservico-producao")));


var app = builder.Build();

app.Use(async (context, next) =>
{
    // Content Security Policy (CSP) Header
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self'; connect-src 'self'; font-src 'self'; object-src 'none'; frame-src 'none';");

    // Anti-clickjacking Header
    context.Response.Headers.Add("X-Frame-Options", "DENY");

    // Strict-Transport-Security Header
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

    // X-Content-Type-Options Header
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

    // Cache-Control Headers
    context.Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, post-check=0, pre-check=0");
    context.Response.Headers.Add("Pragma", "no-cache");
    context.Response.Headers.Add("Expires", "0");

    // Remove unnecessary headers
    context.Response.Headers.Remove("X-Powered-By");
    context.Response.Headers.Remove("Server");

    // Remove Date header
    context.Response.Headers.Remove("Date");

    await next();
});

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<EFDbconfig>();

// Apply pending migrations
dbContext.Database.Migrate();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");
app.UsePathBase(builder.Configuration["App:Pathbase"]);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
