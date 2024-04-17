
using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using DotNetEd.CoreAdmin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using yawaflua.ru.Auth;
using yawaflua.ru.Models;


namespace yawaflua.ru
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
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
            
            clientId = configuration.GetValue<string>("clientId");
            clientSecret = configuration.GetValue<string>("clientSecret");
            redirectUrl = configuration.GetValue<string>("redirectUrl");
            connectionString = configuration.GetValue<string>("connectionString");
            ownerId = configuration.GetValue<string[]>("ownerId");
            readmeFile = configuration.GetValue<string>("readmeFile");
            
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
                .AddSwaggerGen()
                .AddSingleton(new MemoryCache(new MemoryCacheOptions()))
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
                    options.RequireAuthenticatedSignIn = false;
                }).AddScheme<AuthenticationSchemeOptions, ApiKeyAuthantication>("Bearer", options => {});
            services.AddMvc()
                    .AddRazorPagesOptions(options =>
                    {
                        options.Conventions.AddPageRoute("/RobotsTxt", "/Robots.txt");
                        options.Conventions.AddPageRoute("/rrobotsTxt", "/robots.txt");
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
            app.UseSwagger((options) =>
            {
                options.RouteTemplate = "swagger/v1/swagger.json";
            });
            app.UseSwaggerUI();
#if DEBUG
            app.UseCoreAdminCustomTitle("yawaflua");
            app.UseCoreAdminCustomAuth((k) => Task.FromResult(true));
            app.UseCoreAdminCustomUrl("admin/coreadmin");
#endif
            app.UseCors(k => { k.WithMethods("POST", "GET", "PATCH", "PUT"); k.AllowAnyOrigin(); k.AllowAnyHeader(); });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute().AllowAnonymous();
                endpoints.MapFallbackToFile("/index.html").AllowAnonymous();
                endpoints.MapSwagger();
                endpoints.MapRazorPages();
                endpoints.MapControllers().AllowAnonymous();
            });

        }
    }
}
