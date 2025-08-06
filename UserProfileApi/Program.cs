using StackExchange.Redis;
using UserProfileApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));  // Add Redis connection
builder.Services.AddScoped<UserProfile>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();  // Register exception middleware
app.UseMiddleware<UserMiddleware>();      // Register UserMiddleware


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();


app.Run();

