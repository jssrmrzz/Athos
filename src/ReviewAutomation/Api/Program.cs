using Athos.ReviewAutomation.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register services using extension methods
builder.Services
    .AddInfrastructure("Data Source=reviews.db", builder.Configuration)
    .AddApplicationServices(); // ✅ properly chained now

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();