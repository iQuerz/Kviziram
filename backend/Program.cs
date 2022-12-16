using backend;
using backend.Data;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Redis
ConnectionMultiplexer redisConnection = ConnectionMultiplexer.Connect(KviziramConfig.redis);
builder.Services.AddScoped(r => redisConnection.GetDatabase());

// Neo4J

// Additional
builder.Services.AddScoped<KviziramContext, KviziramContext>();
builder.Services.AddScoped<KviziramService, KviziramService>();
builder.Services.AddScoped<Utility, Utility>();


builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Kviziram Server", Version = "v1" });
                c.OperationFilter<CustomHeaderSwaggerAttribute>();
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

app.UseMiddleware<Auth>();

app.MapControllers();

app.Run();
