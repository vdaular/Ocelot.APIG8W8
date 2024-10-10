using Ocelot.APIG8W8.Handler;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddCors();

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", token =>
    {
        token.Authority = builder.Configuration["Auth:Authority"];
        token.TokenValidationParameters = new ()
        {
            ValidateAudience = false
        };
    });

builder.Services.AddOcelot(builder.Configuration)
    .AddCacheManager(cache =>
    {
        cache.WithDictionaryHandle();
    })
    .AddDelegatingHandler<HeadersInjectorDelegatingHandler>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
}, uiOptions =>
{
    uiOptions.DocumentTitle = "Documentación de API Gateway";
});

if (!string.IsNullOrEmpty(builder.Configuration.GetSection("AllowedOrigins").Value))
{
    var origins = builder.Configuration.GetSection("AllowedOrigins").Value!.Split(";");

    app.UseCors(options =>
    {
        options.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
}

app.UseHttpsRedirection();

await app.UseOcelot();

app.MapGet("/", () => "API Gateway");

app.Run();