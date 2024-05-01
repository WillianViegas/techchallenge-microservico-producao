using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using techchallenge_microservico_producao.DatabaseConfig;
using techchallenge_microservico_producao.Models;
using techchallenge_microservico_producao.Repositories;
using techchallenge_microservico_producao.Repositories.Interfaces;
using techchallenge_microservico_producao.Services;
using techchallenge_microservico_producao.Services.Interfaces;

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

builder.Services.AddControllers();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var connectionString = builder.Configuration.GetConnectionString("MyAppCs");
builder.Services.AddDbContext<EFDbconfig>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

app.UseCors("AllowAll");
app.UsePathBase(builder.Configuration["App:Pathbase"]);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
