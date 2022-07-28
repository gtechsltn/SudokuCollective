using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Hangfire;
using Hangfire.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using SudokuCollective.Api.Models;
using SudokuCollective.Cache;
using SudokuCollective.Core.Interfaces.Cache;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Interfaces.ServiceModels;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Authentication;
using SudokuCollective.Data.Services;
using SudokuCollective.Repos;
using SudokuCollective.Core.Interfaces.Jobs;
using SudokuCollective.Data.Jobs;
using SudokuCollective.Data.Models.Payloads;
using SudokuCollective.Data.Models.Requests;
using Amazon.Extensions.NETCore.Setup;

namespace SudokuCollective.Api
{
    /// <summary>
    /// Startup Class
    /// </summary>
    public class Startup
    {
        private IWebHostEnvironment _environment;

        /// <summary>
        /// Startup Class Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Startup Class Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(
                    _environment.IsDevelopment() ? 
                        Configuration.GetConnectionString("DatabaseConnection") : 
                        _environment.IsProduction() ?
                            Environment.GetEnvironmentVariable("DatabaseConnection") :
                            GetHerokuPostgresConnectionString(),
                    b => b.MigrationsAssembly("SudokuCollective.Api")));

            var swaggerDescription = _environment.IsDevelopment() ? 
                Configuration.GetSection("MissionStatement").Value : 
                Environment.GetEnvironmentVariable("MISSIONSTATEMENT");

            var sandboxLicense = _environment.IsDevelopment() ?
                Configuration.GetSection("DefaultSandboxApp:License").Value :
                Environment.GetEnvironmentVariable("SANDBOX_APP_LICENSE");

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc(
                    "v1", 
                    new OpenApiInfo 
                    {
                        Version = "v1",
                        Title = "SudokuCollective API",
                        Description = string.Format("{0} \r\n\r\n For testing purposes please use the Sudoku Collective Sandbox App if you haven't created your own app: \r\n\r\n Id: 3 \r\n\r\n  License: {1}", 
                            swaggerDescription, 
                            sandboxLicense)
                    });
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                swagger.MapType(typeof(JsonElement), () => new OpenApiSchema {
                    Type = "object",
                    Example = new OpenApiString("{}")
                });

                var swashbucklePayloadArray = new OpenApiArray();
                swashbucklePayloadArray.Add(new OpenApiObject());

                swagger.MapType(typeof(List<object>), () => new OpenApiSchema {
                    Type = "array",
                    Example = swashbucklePayloadArray
                });

                swagger.DocumentFilter<PathLowercaseDocumentFilter>();
                swagger.DocumentFilter<CustomModelDocumentFilter<App>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<AuthenticatedUser>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<Difficulty>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<Game>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<Role>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<SMTPServerSettings>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<SudokuCell>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<SudokuMatrix>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<SudokuSolution>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<User>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<AppPayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<CreateDifficultyPayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<UpdateDifficultyPayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<CreateGamePayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<GamePayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<GamesPayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<LicensePayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<CreateRolePayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<UpdateRolePayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<AddSolutionsPayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<SolutionPayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<PasswordResetPayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<RequestPasswordResetPayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<UpdateUserPayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<UpdateUserRolePayload>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<AnnonymousGameRequest>>();
                swagger.DocumentFilter<CustomModelDocumentFilter<UpdatePasswordRequest>>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var filePath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(filePath);
            });

            var tokenManagement = _environment.IsDevelopment() ? 
                Configuration.GetSection("tokenManagement").Get<TokenManagement>() : 
                new TokenManagement 
                { 
                    Secret = Environment.GetEnvironmentVariable("TOKEN_SECRET"),
                    Issuer = Environment.GetEnvironmentVariable("TOKEN_ISSUER"),
                    Audience = Environment.GetEnvironmentVariable("TOKEN_AUDIENCE"),
                    AccessExpiration = Convert.ToInt32(Environment.GetEnvironmentVariable("TOKEN_ACCESS_EXPIRATION")),
                    RefreshExpiration = Convert.ToInt32(Environment.GetEnvironmentVariable("TOKEN_REFRESH_EXPIRATION"))
                };
                
            var secret = Encoding.ASCII.GetBytes(tokenManagement.Secret);

            services.AddSingleton<ITokenManagement>(tokenManagement);

            var redisConnection = _environment.IsDevelopment() ? 
                Configuration.GetConnectionString("CacheConnection") : 
                _environment.IsProduction() ?
                    Environment.GetEnvironmentVariable("CacheConnection") :
                    GetHerokuRedisConnectionString();

            // Add cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "SudokuCollective";
            });

            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseRedisStorage(redisConnection, new RedisStorageOptions
                {
                    InvisibilityTimeout = TimeSpan.FromMinutes(30),
                    FetchTimeout = TimeSpan.FromHours(1),
                    Prefix = "Hangfire:SudokuCollective:",
                    Db = 10,
                    SucceededListSize = 10000,
                    DeletedListSize = 1000
                }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services
                .AddMvc(options => options.EnableEndpointRouting = false);

            AWSOptions awsoptions;

            if (_environment.IsDevelopment())
            {
                awsoptions = Configuration.GetAWSOptions();
            }
            else
            {
                awsoptions = new AWSOptions
                {
                    Region = Amazon.RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AWS_REGION")),
                    Profile = Environment.GetEnvironmentVariable("AWS_PROFILE")
                };
            }

            services.AddDefaultAWSOptions(awsoptions);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secret),
                    ValidIssuer = tokenManagement.Issuer,
                    ValidAudience = tokenManagement.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    LifetimeValidator = LifetimeValidator,
                };
            });

            services.AddControllers()
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.AllowTrailingCommas = true;
                    x.JsonSerializerOptions.IncludeFields = false;
                    x.JsonSerializerOptions.IgnoreReadOnlyProperties = false;
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });

            var emailMetaData = _environment.IsDevelopment() ? 
                Configuration.GetSection("emailMetaData").Get<EmailMetaData>() :
                new EmailMetaData
                {
                    SmtpServer = Environment.GetEnvironmentVariable("SMTP_SMTP_SERVER"),
                    Port = Convert.ToInt32(Environment.GetEnvironmentVariable("SMTP_PORT")),
                    UserName = Environment.GetEnvironmentVariable("SMTP_USERNAME"),
                    Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD"),
                    FromEmail = Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL")
                };

            services.AddSingleton<IEmailMetaData>(emailMetaData);
            services.AddSingleton<ICacheKeys, CacheKeys>();
            services.AddSingleton<ICachingStrategy, CachingStrategy>();

            services.AddScoped<IDataJobs, DataJobs>();
            services.AddScoped<IAppsRepository<App>, AppsRepository<App>>();
            services.AddScoped<IUsersRepository<User>, UsersRepository<User>>();
            services.AddScoped<IAppAdminsRepository<AppAdmin>, AppAdminsRepository<AppAdmin>>();
            services.AddScoped<IGamesRepository<Game>, GamesRepository<Game>>();
            services.AddScoped<IDifficultiesRepository<Difficulty>, DifficultiesRepository<Difficulty>>();
            services.AddScoped<IRolesRepository<Role>, RolesRepository<Role>>();
            services.AddScoped<ISolutionsRepository<SudokuSolution>, SolutionsRepository<SudokuSolution>>();
            services.AddScoped<IEmailConfirmationsRepository<EmailConfirmation>, EmailConfirmationsRepository<EmailConfirmation>>();
            services.AddScoped<IPasswordResetsRepository<Core.Models.PasswordReset>, PasswordResetsRepository<Core.Models.PasswordReset>>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IAppsService, AppsService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IGamesService, GamesService>();
            services.AddScoped<IDifficultiesService, DifficultiesService>();
            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<ISolutionsService, SolutionsService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IValuesService, ValuesService>();

            services.AddHttpContextAccessor();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline...
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(swaggerUI =>
            {
                var swaggerTitle = "SudokuCollective API v1";
                swaggerUI.DocumentTitle = swaggerTitle;
                swaggerUI.SwaggerEndpoint("/swagger/v1/swagger.json", swaggerTitle);
                swaggerUI.DocExpansion(DocExpansion.None);
            });

            // Initialize and set the path for the welcome page saved in wwwroot
            DefaultFilesOptions defaultFile = new DefaultFilesOptions();
            defaultFile.DefaultFileNames.Clear();
            defaultFile.DefaultFileNames.Add("index.html");

            app.UseDefaultFiles(defaultFile);

            app.UseHangfireDashboard();

            app.UseStaticFiles();

            app.UseMvc();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

            app.Use(async (context, next) => {
                context.Request.EnableBuffering();

                await next();
            });

            SeedData.EnsurePopulated(
                app, 
                Configuration,
                env);
        }

        private static string GetHerokuPostgresConnectionString()
        {
            // get the connection string from the ENV variables
            var connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            // parse the connection string
            var databaseUri = new Uri(connectionUrl);

            var db = databaseUri.LocalPath.TrimStart('/');
            string[] userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);

            return $"User ID={userInfo[0]};Password={userInfo[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={db};Pooling=true;SSL Mode=Require;Trust Server Certificate=True;";
        }

        private static string GetHerokuRedisConnectionString()
        {
            // Get the connection string from the ENV variables
            var redisUrlString = Environment.GetEnvironmentVariable("REDIS_URL");

            // parse the connection string
            var redisUri = new Uri(redisUrlString);
            var userInfo = redisUri.UserInfo.Split(':');

            return $"{redisUri.Host}:{redisUri.Port},password={userInfo[1]}";
        }

        private bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters @params)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow;
            }
            return false;
        }
    }

    /// <summary>
    /// A filter which displays api paths in lower case.
    /// </summary>
    public class PathLowercaseDocumentFilter : IDocumentFilter
    {
        /// <summary>
        /// A method which applies the filter.
        /// </summary>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var dictionaryPath = swaggerDoc.Paths.ToDictionary(x => ToLowercase(x.Key), x => x.Value);
            var newPaths = new OpenApiPaths();
            foreach(var path in dictionaryPath)
            {
                newPaths.Add(path.Key, path.Value);
            }
            swaggerDoc.Paths = newPaths;
        }
        
        private static string ToLowercase(string key)
        {
            var parts = key.Split('/').Select(part => part.Contains('}') ? part : part.ToLowerInvariant());
            return string.Join('/', parts);
        }
    }

    /// <summary>
    /// A custom document filter to include models in the swagger documentation.
    /// </summary>
    public class CustomModelDocumentFilter<T> : IDocumentFilter where T : class
    {
        /// <summary>
        /// A method to apply the filter
        /// </summary>
        public void Apply(OpenApiDocument openapiDoc, DocumentFilterContext context)
        {
            context.SchemaGenerator.GenerateSchema(typeof(T), context.SchemaRepository);
        }
    }
}
