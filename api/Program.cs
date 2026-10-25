using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Origins allowed to call this API from a browser (set in appsettings or env var).
// Env var form: AllowedOrigins__0=https://my-site.pages.dev
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .WithMethods("GET")
              .AllowAnyHeader());
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    // Caddy runs on the same VM (loopback, trusted by default) and forwards the original scheme/host.
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseCors("Frontend");

var startedAt = DateTimeOffset.UtcNow;

var projects = new[]
{
    new Project(1, "Commerce", ".NET 10 Clean Architecture e-commerce backend", ["C#", "PostgreSQL", "RabbitMQ"]),
    new Project(2, "OAuth Learn", "Login with GitHub using OAuth 2.0 + PKCE", ["Vue", ".NET", "Docker"]),
    new Project(3, "Progress Dashboard", "Personal growth data pipelines into Grafana", ["Python", "Postgres", "Grafana"]),
};

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/hello", () => new
{
    message = "Hello from the .NET 10 API running on Oracle Cloud",
    serverTimeUtc = DateTimeOffset.UtcNow,
    uptimeSeconds = (long)(DateTimeOffset.UtcNow - startedAt).TotalSeconds,
    runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
});

app.MapGet("/api/projects", () => projects);

app.MapGet("/api/projects/{id:int}", (int id) =>
    projects.FirstOrDefault(p => p.Id == id) is { } project
        ? Results.Ok(project)
        : Results.NotFound());

app.Run();

record Project(int Id, string Name, string Description, string[] Tags);
