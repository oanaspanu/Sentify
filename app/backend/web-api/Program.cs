using DotNetEnv;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AnalysisAPI.Data;
using AnalysisAPI.Services;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
ConfigureServices(builder.Services);
var app = builder.Build();
ConfigureApp(app);
app.Run();


void ConfigureServices(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
    });

    var youtubeApiKey = Environment.GetEnvironmentVariable("YOUTUBE_API_KEY")
        ?? throw new Exception("YOUTUBE_API_KEY is not set in the environment variables.");

    string dbConnection = Environment.GetEnvironmentVariable("DATABASE_URL")
        ?? throw new Exception("DATABASE_URL is not set in the environment variables.");
    services.AddDbContext<AppDbContext>(options => options.UseSqlite(dbConnection));

    var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
        ?? throw new Exception("JWT_SECRET_KEY is not set in the environment variables.");
    var key = Encoding.ASCII.GetBytes(jwtSecret);
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });


    services.AddScoped<PythonService>();
    services.AddScoped<AuthService>();
    services.AddHttpClient();
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}

void ConfigureApp(WebApplication app)
{

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
        app.UseHttpsRedirection();
    }
    app.UseCors("AllowFrontend");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }
}