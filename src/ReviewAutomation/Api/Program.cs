using Athos.ReviewAutomation.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Clean architecture-style service registration
builder.Services
    .AddInfrastructure("Data Source=reviews.db")
    .AddApplicationServices();

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