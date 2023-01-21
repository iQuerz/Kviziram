using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Neo4jClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.ConfigureCors();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Kviziram Server", Version = "v1" });
                c.OperationFilter<CustomHeaderSwaggerAttribute>();
            });
builder.Services.AddSignalR();

#region Server Connections
// Redis
ConnectionMultiplexer redisClient = ConnectionMultiplexer.Connect(KviziramConfig.redis);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisClient);

// Neo4J
GraphClient neoClient = new GraphClient(new Uri(KviziramConfig.NJ_ADDRESS), KviziramConfig.NJ_USER, KviziramConfig.NJ_PASS);
neoClient.ConnectAsync().Wait();
builder.Services.AddSingleton<IGraphClient>(neoClient);
#endregion

// Context
builder.Services.AddScoped<KviziramContext, KviziramContext>();

// Services
builder.Services.AddScoped<ILoginRegisterService, LoginRegisterService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IAdService, AdService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IGameService, GameService>();

// Utility for god knows what
builder.Services.AddScoped<Utility, Utility>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCors("CORS");

app.UseAuthorization();

app.UseMiddleware<Auth>();

app.MapControllers();

app.MapHub<GameHub>("/hubs/game");
app.MapHub<NotificationHub>("/hubs/notification");


app.Run();
