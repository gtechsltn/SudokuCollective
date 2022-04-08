using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SudokuCollective.Api.Models;
using SudokuCollective.Cache;
using SudokuCollective.Core.Interfaces.Cache;
using SudokuCollective.Core.Models;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Data.Models;
using SudokuCollective.Data.Models.Authentication;
using SudokuCollective.Data.Services;
using SudokuCollective.Repos;
using Swashbuckle.AspNetCore.SwaggerGen;
using SudokuCollective.Core.Interfaces.ServiceModels;

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
                    !_environment.IsStaging() ? Configuration.GetConnectionString("DatabaseConnection") : GetHerokuPostgresConnectionString(),
                    b => b.MigrationsAssembly("SudokuCollective.Api")));

            var swaggerDescription = !_environment.IsStaging() ? 
                Configuration.GetSection("SwaggerDocs:Description").Value : 
                Environment.GetEnvironmentVariable("SWAGGERDOC_DESCRIPTION");

            var sandboxLicense = !_environment.IsStaging() ?
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
                        Description = string.Format("{0} \r\n\r\n For testing purposes please use the Sudoku Collective Sandbox App if you haven't created your own app: \r\n\r\n Id: 3 \r\n\r\n  license: {1}", 
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
                swagger.DocumentFilter<PathLowercaseDocumentFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var filePath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(filePath);
            });

            var tokenManagement = !_environment.IsStaging() ? 
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

            services
                .AddMvc(options => options.EnableEndpointRouting = false);

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

            var emailMetaData = !_environment.IsStaging() ? 
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

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = !_environment.IsStaging() ? Configuration.GetConnectionString("CacheConnection") : GetHerokuRedisConnectionString();
                options.InstanceName = "SudokuCollective";
            });

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
            });

            // Initialize and set the path for the welcome page saved in wwwroot
            DefaultFilesOptions defaultFile = new DefaultFilesOptions();
            defaultFile.DefaultFileNames.Clear();
            defaultFile.DefaultFileNames.Add("index.html");

            app.UseDefaultFiles(defaultFile);

            app.UseStaticFiles();

            app.UseMvc();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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
}
