using Bitacora.Api;
using Bitacora.Application;
using Bitacora.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var databaseDirectory = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(databaseDirectory);

var connectionSettings = new SqliteConnectionStringBuilder(
    builder.Configuration.GetConnectionString("Bitacora") ?? string.Empty)
{
    ForeignKeys = true
};
if (string.IsNullOrWhiteSpace(connectionSettings.DataSource))
{
    connectionSettings.DataSource = Path.Combine(databaseDirectory, "bitacora.db");
}

builder.Services.AddDbContext<BitacoraDbContext>(options =>
    options.UseSqlite(connectionSettings.ToString()));

builder.Services.AddScoped<IRegistroRepository, RegistroRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddScoped<RegistroService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("Falta Jwt:Key en user-secrets");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "proyectosic.lat";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "proyectosic.lat";

builder.Services.AddScoped<AuthService>(provider => 
    new AuthService(
        provider.GetRequiredService<IAuthRepository>(), 
        jwtKey, jwtIssuer, jwtAudience));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero 
        };
    });

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
    options.RespectBrowserAcceptHeader = true;
}).ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problem = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Revisa los datos del registro.",
            Instance = context.HttpContext.Request.Path
        };
        problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        return new BadRequestObjectResult(problem)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
        context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
});
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.Use(async (context, next) =>
{
    await next(context);

    if (context.Response.StatusCode < 400 || context.Response.HasStarted ||
        context.Response.ContentLength is > 0)
    {
        return;
    }

    var status = context.Response.StatusCode;
    var title = status switch
    {
        401 => "No autorizado. Debes iniciar sesión.",
        403 => "Prohibido. No tienes permisos.",
        404 => "No se encontró la dirección solicitada.",
        405 => "El método HTTP no está permitido en esta dirección.",
        406 => "La API devuelve datos en formato JSON. Usa Accept: application/json.",
        415 => "Envía los datos con Content-Type: application/json.",
        _ => "No se pudo completar la solicitud."
    };
    context.Response.ContentLength = null;
    var problem = new ProblemDetails
    {
        Status = status,
        Title = title,
        Instance = context.Request.Path
    };
    problem.Extensions["traceId"] = context.TraceIdentifier;
    
    await Results.Json(problem, contentType: "application/problem+json",
        statusCode: status).ExecuteAsync(context);
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Bitácora de campo v1");
        options.DocumentTitle = "Bitácora de campo - API";
    });
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();