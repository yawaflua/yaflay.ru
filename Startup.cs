
using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using DotNetEd.CoreAdmin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using yaflay.ru.Auth;
using yaflay.ru.Models;


namespace yaflay.ru
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        public static CookieContainer cookieContainer = new();
        public static HttpClientHandler handler = new() { CookieContainer = cookieContainer};
        public static HttpClient client = new(handler);
        public static AppDbContext? dbContext;
        public static string? clientId { get; set; } = null;
        public static string? clientSecret { get; set; } = null;
        public static string? redirectUrl { get; set; } = null;
        public static string[]? ownerId { get; set; } = null;
        public static string? readmeFile { get; set; } = null;
        public static string? connectionString { private get; set; } = null;
        public Startup()
        {
            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "m.")
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
            if (clientId == null | clientSecret == null | redirectUrl == null) 
            {
                clientId = configuration.GetValue<string>("clientId");
                clientSecret = configuration.GetValue<string>("clientSecret");
                redirectUrl = configuration.GetValue<string>("redirectUrl");
            }
            if (connectionString == null)
            {
                connectionString = configuration.GetValue<string>("connectionString");
                Console.WriteLine("Connectionstring" + connectionString);
                if (connectionString == null)
                {
                    throw new ArgumentException("ConnectionString is null!");
                }
            }
            if (ownerId == null)
            {
                ownerId = new[] { configuration.GetValue<string>("ownerId") };
                if (ownerId?.Length == 0)
                {
                    throw new ArgumentException("Owner id is null!");
                }
            }
            if (readmeFile == null)
            {
                readmeFile = configuration.GetValue<string>("readmeFile");
                if (readmeFile == null)
                {
                    throw new ArgumentException("ReadmeFile link is null");
                }
            }
            
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/RobotsTxt", "/Robots.txt");
                    options.Conventions.AddPageRoute("/RobotsTxt", "/robots.txt");
                    options.Conventions.AddPageRoute("/NotFound", "/404");
                    options.Conventions.AddPageRoute("/IternalErrorPage", "/500");
                    options.Conventions.AddPageRoute("/Authorize", "/authorize");
                });
            services
                .AddCors(k => { k.AddDefaultPolicy(l => { l.AllowAnyHeader(); l.AllowAnyMethod(); l.AllowAnyOrigin(); }); })
                .AddRouting()
                .AddTransient<ApiKeyAuthantication>()
                .AddSingleton(configuration)
                .AddDbContext<AppDbContext>(c => c.UseNpgsql(connectionString: connectionString))
                .AddAuthorization(k =>
                {
                    k.AddPolicy("DISCORD-OAUTH-PUBLIC", policyBuilder => {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.RequireClaim("Bearer", "Public");
                    });
                    k.AddPolicy("DISCORD-OAUTH-PRIVATE", policyBuilder => {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.RequireClaim("Bearer", "Private");
                    });
                })
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Bearer";
                    options.DefaultChallengeScheme = "Bearer";
                    options.AddScheme<ApiKeyAuthantication>("DISCORD-OAUTH-PRIVATE", "DISCORD-OAUTH-PRIVATE");
                    options.AddScheme<ApiKeyAuthantication>("DISCORD-OAUTH-PUBLIC", "DISCORD-OAUTH-PUBLIC");
                }).AddScheme<AuthenticationSchemeOptions, ApiKeyAuthantication>("Bearer", options => {});
            services.AddMvc()
                    .AddRazorPagesOptions(options =>
                    {
                        options.Conventions.AddPageRoute("/RobotsTxt", "/Robots.txt");
                        options.Conventions.AddPageRoute("/RobotsTxt", "/robots.txt");
                        options.Conventions.AddPageRoute("/NotFound", "/404");
                        options.Conventions.AddPageRoute("/IternalErrorPage", "/500");
                        options.Conventions.AddPageRoute("/Authorize", "/authorize");
                    });
            services.AddRazorPages();
            

            dbContext = services.BuildServiceProvider().GetRequiredService<AppDbContext>();
#if DEBUG == true
            services.AddCoreAdmin("admin");
#endif
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Add services to the container.
            // app.Services.AddRazorPages();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHsts();
            }

            // Configure the HTTP request pipeline.
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
#if DEBUG
            app.UseCoreAdminCustomTitle("yawaflua");
            app.UseCoreAdminCustomAuth((k) => Task.FromResult(true));
            app.UseCoreAdminCustomUrl("admin/coreadmin");
#endif
            app.UseCors(k => { k.AllowAnyMethod(); k.AllowAnyOrigin(); k.AllowAnyHeader(); });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

        }
    }
}
