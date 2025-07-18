using Athos.ReviewAutomation.Api.Extensions;
using Athos.ReviewAutomation.Api.Middleware;

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
        policy.WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "http://10.0.0.22:5173" // Allows mobile device access
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Configure authentication with optional Google OAuth
var authBuilder = builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/auth/google";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/auth/access-denied";
    });

// Only add Google OAuth if credentials are configured
var googleClientId = builder.Configuration["GoogleOAuth:ClientId"];
var googleClientSecret = builder.Configuration["GoogleOAuth:ClientSecret"];

if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    authBuilder.AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = googleClientId;
        googleOptions.ClientSecret = googleClientSecret;
        googleOptions.CallbackPath = "/auth/google/callback";
        
        // Add Google My Business API scopes
        googleOptions.Scope.Add("https://www.googleapis.com/auth/business.manage");
        googleOptions.Scope.Add("https://www.googleapis.com/auth/business.reviews");
        googleOptions.Scope.Add("https://www.googleapis.com/auth/business.profile");
        
        // Save tokens to access Google APIs
        googleOptions.SaveTokens = true;
    });
}

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

// Authentication middleware (when implemented)
app.UseAuthentication();
app.UseAuthorization();

// Business context middleware for multi-tenant support
app.UseMiddleware<BusinessContextMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();