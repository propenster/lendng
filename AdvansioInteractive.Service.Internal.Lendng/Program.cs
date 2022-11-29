using AdvansioInteractive.Service.Internal.Lendng.Data;
using AdvansioInteractive.Service.Internal.Lendng.Helpers;
using AdvansioInteractive.Service.Internal.Lendng.Interfaces;
using AdvansioInteractive.Service.Internal.Lendng.Models;
using AdvansioInteractive.Service.Internal.Lendng.Services;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiVersioning(option =>
{
    option.ReportApiVersions = true;
    option.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    option.AssumeDefaultVersionWhenUnspecified = true;

});

BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));
builder.Services.AddHttpClient("LendngClient", x =>
{
    x.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    x.Timeout = TimeSpan.FromSeconds(180);

});
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

})
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })

               ;
var mongoConfigSection = builder.Configuration.GetSection(nameof(MongoDbConfig));
builder.Services.Configure<MongoDbConfig>(mongoConfigSection);
//var appSettingsSection = builder.Configuration.GetSection(nameof(MongoDbConfig));
//builder.Services.Configure<MongoDbConfig>(appSettingsSection);
// configure jwt authentication
var mongoConfig = mongoConfigSection.Get<MongoDbConfig>();

var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
{
    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = mongoConfig.ConnectionString,
        DatabaseName = mongoConfig.DatabaseName,

    },
    IdentityOptionsAction = options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 10;

        // ApplicationUser settings
        options.User.RequireUniqueEmail = true;
        //options.SignIn.RequireConfirmedEmail = true;
        //options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
    }

};

builder.Services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentityConfig)
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddDefaultTokenProviders();
builder.Services.AddSingleton<IDbClient, DbClient>();
builder.Services.AddTransient<ICoreService, CoreService>();
//add and configure authentication
var appSettingsSection = builder.Configuration.GetSection(nameof(JwtSettings));
builder.Services.Configure<JwtSettings>(appSettingsSection);
// configure jwt authentication
var appSettings = appSettingsSection.Get<JwtSettings>();
builder.Services.AddSingleton<JwtSettings>(appSettings);
var key = Encoding.UTF8.GetBytes(appSettings.Key);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = true;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = appSettings.Issuer,
                        ValidAudience = appSettings.Audience,
                        ClockSkew = TimeSpan.Zero

                    };
                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        },

                        OnTokenValidated = _ => Task.CompletedTask
                    };
                });

builder.Services.AddSwaggerGen(
    options =>
    {
        //options.ExampleFilters();
        options.DescribeAllParametersInCamelCase();
        options.EnableAnnotations();
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Lendng API", Version = "v1" });
        //options.AddServer(new OpenApiServer()
        //{
        //    Url = swaggerConfigSettings.OpenApiServerURI //"https://172.18.16.88/PanGeneration"
        //});
    }
    ).AddSwaggerGenNewtonsoftSupport();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    var prefix = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
    c.SwaggerEndpoint($"{prefix}/swagger/v1/swagger.json", "Lendng API");

});
LifetimeDBSeeder.SeedMongoIdentityDatabaseRoles(app).Wait();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
