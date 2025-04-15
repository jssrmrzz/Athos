using Athos.Api.Services;
using Microsoft.EntityFrameworkCore;
using Athos.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Force Kestrel to use both ports
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP - for development testing
    options.ListenAnyIP(5285);

    // HTTPS - the one Swagger tries to use
    options.ListenAnyIP(7157, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

// Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseSqlite("Data Source=reviews.db"));
builder.Services.AddScoped<ReviewPollingService>();

var app = builder.Build();

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

