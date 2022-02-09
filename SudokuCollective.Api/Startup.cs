using System;
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
using SudokuCollective.Data.Models.Authentication;
using SudokuCollective.Core.Interfaces.Services;
using SudokuCollective.Data.Services;
using SudokuCollective.Core.Interfaces.Repositories;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Repositories;
using SudokuCollective.Data.Models;
using SudokuCollective.Api.Models;

namespace SudokuCollective.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DatabaseConnection"),
                    b => b.MigrationsAssembly("SudokuCollective.Api")));

            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "SudokuCollective.Api", Version = "v1" });
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
                        System.Array.Empty<string>()
                    }
                });
            });

            services.Configure<TokenManagement>(Configuration.GetSection("TokenManagement"));
            var token = Configuration.GetSection("tokenManagement").Get<TokenManagement>();
            var secret = Encoding.ASCII.GetBytes(token.Secret);

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                    ValidIssuer = token.Issuer,
                    ValidAudience = token.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    LifetimeValidator = LifetimeValidator,
                };
            });

            services.AddControllers()
                .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.IncludeFields = false;
                    x.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                });


            var emailMetaData = Configuration.GetSection("emailMetaData").Get<EmailMetaData>();

            services.AddSingleton(emailMetaData);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SudokuCollective.Api v1"));

            // Initialize and set the path for the welcome page saved in wwwroot
            DefaultFilesOptions defaultFile = new DefaultFilesOptions();
            defaultFile.DefaultFileNames.Clear();
            defaultFile.DefaultFileNames.Add("index.html");

            app.UseDefaultFiles(defaultFile);

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SeedData.EnsurePopulated(app, Configuration);
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
}
