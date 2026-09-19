using System.Text;
using Blog.Notifications.Consumers;
using Blog.Notifications.Hubs;
using Blog.Notifications.Options;
using Blog.Notifications.SignalR;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = new JwtSettings();
builder.Configuration.Bind(nameof(JwtSettings), jwtSettings);

builder.Services
    .AddAuthentication(a =>
    {
        a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        a.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(jwt =>
    {
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SigningKey
                ?? throw new InvalidOperationException())),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudiences = jwtSettings.Audiences,
            RequireExpirationTime = false,
            ValidateLifetime = true
        };
        jwt.Audience = jwtSettings.Audiences?[0];
        jwt.ClaimsIssuer = jwtSettings.Issuer;
        jwt.Events = new JwtBearerEvents
        {
            // Same reasoning as Blog.API's IdentityRegistrar — browsers
            // can't set an Authorization header on a WebSocket handshake,
            // so the SignalR client sends the token as an access_token
            // query parameter instead, only for the hub's own path.
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) &&
                    context.HttpContext.Request.Path.StartsWithSegments("/notification"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, UserProfileIdUserIdProvider>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(_ => true);
    });
});

var rabbitMqHost = builder.Configuration["RabbitMQ:Host"];

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<NewCommentNotificationConsumer>();

    if (!string.IsNullOrEmpty(rabbitMqHost))
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(rabbitMqHost);
            ConfigureResilience(cfg);
            cfg.ConfigureEndpoints(context);
        });
    }
    else
    {
        x.UsingInMemory((context, cfg) =>
        {
            ConfigureResilience(cfg);
            cfg.ConfigureEndpoints(context);
        });
    }
});

// 9.4 — resilience for the consumer pipeline, not inherently tied to
// RabbitMQ, so applied on both transport branches.
static void ConfigureResilience(IBusFactoryConfigurator cfg)
{
    // Retry a failed consume with a growing delay before giving up and
    // moving the message to its error queue — covers transient failures
    // (a brief hub delivery hiccup, a momentary connection blip) without
    // retrying forever.
    cfg.UseMessageRetry(r => r.Intervals(
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(15)));

    // If the consumer keeps failing well beyond what retry alone can
    // paper over, stop attempting new deliveries for a cooldown window
    // instead of hammering an already-struggling dependency.
    cfg.UseCircuitBreaker(cb =>
    {
        cb.TrackingPeriod = TimeSpan.FromMinutes(1);
        cb.TripThreshold = 15;
        cb.ActiveThreshold = 10;
        cb.ResetInterval = TimeSpan.FromMinutes(5);
    });
}

var app = builder.Build();

// Deliberately no UseHttpsRedirection() — this service is Docker
// HTTP-only by design, same as Blog.API. Redirecting with no HTTPS
// endpoint configured is exactly the bug fixed in
// learning-notes/notes/39-docker-containerization.md; simplest correct
// choice here is to just not add it.
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<MessageHub>("/notification");

app.Run();
