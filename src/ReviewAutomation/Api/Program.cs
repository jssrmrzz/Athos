using Athos.ReviewAutomation.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register services using extension methods
builder.Services
    .AddInfrastructure("Data Source=reviews.db", builder.Configuration)
    .AddApplicationServices();

// ✅ Add CORS policy to allow frontend requests
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Use CORS middleware before routing
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.MapControllers();

app.Run();