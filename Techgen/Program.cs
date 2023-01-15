using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Filters;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Techgen;
using Techgen.Common.Constants;
using Techgen.Common.Helpers.SwaggerFilters;
using Techgen.Common.Utilities;
using Techgen.DAL;
using Techgen.DAL.Abstract;
using Techgen.Domain.DB;
using Techgen.Domain.Entities.Identity;
using Techgen.EmailService;
using Techgen.Services;

var builder = WebApplication.CreateBuilder(args);

var sp = builder.Services.BuildServiceProvider();
var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

IConfiguration configuration = builder.Configuration;

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
                    //SignatureValidator = (string token, TokenValidationParameters validationParameters) => {

                    //    var jwt = new JwtSecurityToken(token);

                    //    var signKey = AuthOptions.GetSigningCredentials().Key as SymmetricSecurityKey;

                    //    var encodedData = jwt.EncodedHeader + "." + jwt.EncodedPayload;
                    //    var compiledSignature = Encode(encodedData, signKey.Key);

                    //    //Validate the incoming jwt signature against the header and payload of the token
                    //    if (compiledSignature != jwt.RawSignature)
                    //    {
                    //        throw new Exception("Token signature validation failed.");
                    //    }

                    //    /// TO DO: initialize user claims

                    //    return jwt;
                    //},
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
                                    return scope.ServiceProvider.GetService<IRepository<UserToken>>().FindOne(t => t.AccessTokenHash == hash && t.IsActive) != null;
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

    //ToDo:
    // resolve the IApiVersionDescriptionProvider service
    var provider = sp.GetRequiredService<IApiVersionDescriptionProvider>();

    // add a swagger document for each discovered API version
    // note: you might choose to skip or document deprecated API versions differently
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
    }

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



//EmailService
var emailConfig = configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

//Database
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var databaseSettings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    var mongoDbClient = new MongoClient(databaseSettings.ConnectionString);
    var mongoDb = mongoDbClient.GetDatabase(databaseSettings.DatabaseName);

    return mongoDb;
});
builder.Services.AddScoped<IDataContext>(provider => provider.GetService<DataContext>());
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//impliment identity
var mongoDbSettings = builder.Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+#=";
})
.AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
(
    mongoDbSettings.ConnectionString, mongoDbSettings.DatabaseName
);

//impliment auto mapper config
var config = new AutoMapper.MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AutoMapperProfileConfiguration());
});
builder.Services.AddSingleton(config.CreateMapper());

builder.Services.InitializeRepositories();
builder.Services.InitializeServices();



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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

app.Run();

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
