using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Newtonsoft.Json;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using NLog.Web;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Techgen;
using Techgen.Common.Constants;
using Techgen.Common.Exceptions;
using Techgen.Common.Helpers.SwaggerFilters;
using Techgen.Common.Utilities;
using Techgen.DAL;
using Techgen.DAL.Abstract;
using Techgen.DAL.Migrations;
using Techgen.Domain.Entities.Identity;
using Techgen.EmailService;
using Techgen.Helpers;
using Techgen.Models.ResponseModels.Base;
using Techgen.ResourceLibrary;
using Techgen.Services;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    var sp = builder.Services.BuildServiceProvider();
    var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

    IConfiguration configuration = builder.Configuration;

    builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlite(configuration.GetConnectionString("Connection"));
        options.EnableSensitiveDataLogging(false);
    });


    // Adding Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
                .AddJwtBearer(/*JwtBearerDefaults.AuthenticationScheme, */options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,
                        ValidateAudience = true,
                        ValidateActor = false,
                        ValidAudience = AuthOptions.AUDIENCE,
                        ValidateLifetime = true,
                        LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
                        {
                            var jwt = securityToken as JwtSecurityToken;

                            if (!notBefore.HasValue || !expires.HasValue || DateTime.Compare(expires.Value, DateTime.UtcNow) <= 0)
                            {
                                return false;
                            }

                            if (jwt == null)
                                return false;

                            var isRefresStr = jwt.Claims.FirstOrDefault(t => t.Type == "isRefresh")?.Value;

                            if (isRefresStr == null)
                                return false;

                            var isRefresh = Convert.ToBoolean(isRefresStr);

                            if (!isRefresh)
                            {
                                try
                                {
                                    using (var scope = serviceScopeFactory.CreateScope())
                                    {
                                        var hash = HashUtility.GetHash(jwt.RawData);
                                        return scope.ServiceProvider.GetService<IRepository<UserToken>>().Find(t => t.AccessTokenHash == hash && t.IsActive) != null;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var logger = sp.GetService<ILogger<Program>>();
                                    logger.LogError(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") + ": Exception occured in token validator. Exception message: " + ex.Message + ". InnerException: " + ex.InnerException?.Message);
                                    return false;
                                }
                            }

                            return false;
                        },
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                });

    builder.Services.AddMvc()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    })
    .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true)
    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

    builder.Services.AddControllers();
    builder.Services.AddRazorPages();



    // Add ApiExplorer to discover versions
    builder.Services.AddVersionedApiExplorer(setup =>
    {
        setup.GroupNameFormat = "'v'VVV";
        setup.SubstituteApiVersionInUrl = true;
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddApiVersioning(opt =>
    {
        opt.DefaultApiVersion = new ApiVersion(1, 0);
        opt.AssumeDefaultVersionWhenUnspecified = true;
        opt.ReportApiVersions = true;
        opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                        new HeaderApiVersionReader("x-api-version"),
                                                        new MediaTypeApiVersionReader("x-api-version"));
    });

    builder.Services.AddSwaggerGen(options =>
    {

        var basePath = PlatformServices.Default.Application.ApplicationBasePath;
        var fileName = typeof(Program).GetTypeInfo().Assembly.GetName().Name + ".xml";

        string XmlCommentsFilePath = Path.Combine(basePath, fileName);

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            In = ParameterLocation.Header,
            Description = "Access token",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

        options.OrderActionsBy(x => x.ActionDescriptor.DisplayName);

        // add a custom operation filter which sets default values

        // integrate xml comments
        options.IncludeXmlComments(XmlCommentsFilePath);
        options.IgnoreObsoleteActions();

        options.OperationFilter<DefaultValues>();
        options.OperationFilter<SecurityRequirementsOperationFilter>("Bearer");

        // for deep linking
        options.CustomOperationIds(e => $"{e.HttpMethod}_{e.RelativePath.Replace('/', '-').ToLower()}");

    });

    // instead of options.DescribeAllEnumsAsStrings()
    builder.Services.AddSwaggerGenNewtonsoftSupport();

    builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

    //EmailService
    var emailConfig = configuration
            .GetSection("EmailConfiguration")
            .Get<EmailConfiguration>();
    builder.Services.AddSingleton(emailConfig);


    builder.Services.AddScoped<IDataContext, DataContext>();
    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+#=";
    }).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

    builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
    {
        o.Name = "Default";
        o.TokenLifespan = TimeSpan.FromHours(12);
    }); ;

    //impliment auto mapper config
    var config = new AutoMapper.MapperConfiguration(cfg =>
    {
        cfg.AddProfile(new AutoMapperProfileConfiguration());
    });
    builder.Services.AddSingleton(config.CreateMapper());

    builder.Services.InitializeRepositories();
    builder.Services.InitializeServices();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();
    #region Cookie auth

    app.UseCookiePolicy(new CookiePolicyOptions
    {
        MinimumSameSitePolicy = SameSiteMode.Strict,
        HttpOnly = HttpOnlyPolicy.Always,
        Secure = CookieSecurePolicy.Always
    });

    app.Use(async (context, next) =>
    {
        var token = context.Request.Cookies[".AspNetCore.Application.Id"];
        if (!string.IsNullOrEmpty(token))
            context.Request.Headers.Add("Authorization", "Bearer " + token);

        await next();
    });

    #endregion

    var appBasePath = System.IO.Directory.GetCurrentDirectory();
    NLog.GlobalDiagnosticsContext.Set("appbasepath", appBasePath);

    try
    {
        foreach (FileTarget target in LogManager.Configuration.AllTargets)
        {
            target.FileName = appBasePath + "/" + ((SimpleLayout)target.FileName).OriginalText;
        }

        LogManager.ReconfigExistingLoggers();

        logger.Debug("init main");

        try
        {
            var context = app.Services.GetRequiredService<DataContext>();
            DbInitializer.Initialize(context, configuration, app.Services);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while seeding the database.");
        }
    }
    catch (Exception ex)
    {
        //NLog: catch setup errors
        logger.Error(ex, "Stopped program because of exception");
        throw;
    }
    finally
    {
        // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
        NLog.LogManager.Shutdown();
    }

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger(options =>
        {
            options.PreSerializeFilters.Add((swagger, httpReq) =>
            {
                //swagger.Host = httpReq.Host.Value;

                var ampersand = "&amp;";

                foreach (var path in swagger.Paths)
                {
                    if (path.Value.Operations.Any(x => x.Key == OperationType.Get && x.Value.Deprecated))
                        path.Value.Operations.First(x => x.Key == OperationType.Get).Value.Description = path.Value.Operations.First(x => x.Key == OperationType.Get).Value.Description.Replace(ampersand, "&");

                    if (path.Value.Operations.Any(x => x.Key == OperationType.Delete && x.Value?.Description != null))
                        path.Value.Operations.First(x => x.Key == OperationType.Delete).Value.Description = path.Value.Operations.First(x => x.Key == OperationType.Delete).Value.Description.Replace(ampersand, "&");
                }

                var paths = swagger.Paths.ToDictionary(p => p.Key, p => p.Value);
                foreach (KeyValuePair<string, OpenApiPathItem> path in paths)
                {
                    swagger.Paths.Remove(path.Key);
                    swagger.Paths.Add(path.Key.ToLowerInvariant(), path.Value);
                }
            });
        });

        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
        app.UseSwaggerUI(options =>
        {
            options.IndexStream = () => File.OpenRead("Views/Swagger/swagger-ui.html");
            options.InjectStylesheet("/Swagger/swagger-ui.style.css");

            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());

            options.EnableFilter();

            // for deep linking
            options.EnableDeepLinking();
            options.DisplayOperationId();
        });

    }

    app.UseCors(builder =>
    {
        if (app.Environment.IsDevelopment())
            builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials();

        if (app.Environment.IsStaging())
            builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials();

        if (app.Environment.IsProduction())
            builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    #region Error handler

    // Different middleware for api and ui requests  
    app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), appBuilder =>
    {
        var localizer = sp.GetService<IStringLocalizer<ErrorsResource>>();
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalErrorHandling");

        // Exception handler - show exception data in api response
        appBuilder.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandler = async context =>
            {
                var errorModel = new ErrorResponseModel(localizer);
                var result = new ContentResult();

                var exception = context.Features.Get<IExceptionHandlerPathFeature>();

                if (exception.Error is CustomException)
                {
                    var ex = (CustomException)exception.Error;

                    result = errorModel.Error(ex);
                }
                else
                {
                    var message = exception.Error.InnerException?.Message ?? exception.Error.Message;
                    logger.LogError($"{exception.Path} - {message}");

                    errorModel.AddError("general", message);
                    result = errorModel.InternalServerError(app.Environment.IsDevelopment() ? exception.Error.StackTrace : null);
                }

                context.Response.StatusCode = result.StatusCode.Value;
                context.Response.ContentType = result.ContentType;

                await context.Response.WriteAsync(result.Content);
            }
        });

        // Handles responses with status codes (correctly executed requests, without any exceptions)
        appBuilder.UseStatusCodePages(async context =>
        {
            var errorResponse = ErrorHelper.GetError(localizer, context.HttpContext.Response.StatusCode);

            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        });
    });

    app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
    {
        appBuilder.UseExceptionHandler("/Error");
        appBuilder.UseStatusCodePagesWithReExecute("/Error", "?statusCode={0}");
    });

    #endregion

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
    });

    app.Run();

}
catch (Exception ex) 
{
    // NLog: catch setup errors
    logger.Error(ex, "Stopped program because of exception");
    throw;
}

static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
{
    var info = new OpenApiInfo()
    {
        Title = $"ApplicationAuth API {description.ApiVersion}",
        Version = description.ApiVersion.ToString(),
        Description = "The ApplicationAuth application with Swagger and API versioning."
    };

    if (description.IsDeprecated)
    {
        info.Description += " This API version has been deprecated.";
    }

    return info;
}

string Encode(string input, byte[] key)
{
    HMACSHA256 myhmacsha = new HMACSHA256(key);
    byte[] byteArray = Encoding.UTF8.GetBytes(input);
    MemoryStream stream = new MemoryStream(byteArray);
    byte[] hashValue = myhmacsha.ComputeHash(stream);
    return Base64UrlEncoder.Encode(hashValue);
}
