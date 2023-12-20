
using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using DotNetEd.CoreAdmin;
using Microsoft.EntityFrameworkCore;
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
        public static string? clientId = null;
        public static string? clientSecret = null;
        public static string? redirectUrl = null;
        public static string? ownerId = null;
        public static string? readmeFile = null;
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
            }
            if (ownerId == null)
            {
                ownerId = configuration.GetValue<string>("ownerId");
            }
            if (readmeFile == null)
            {
                readmeFile = configuration.GetValue<string>("readmeFile");
            }
            
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/RobotsTxt", "/Robots.txt");
                    options.Conventions.AddPageRoute("/NotFound", "/404");
                    options.Conventions.AddPageRoute("/IternalErrorPage", "/500");
                    options.Conventions.AddPageRoute("/Authorize", "/authorize");
                });
            services
                .AddAuthentication();
            services
                .AddRouting()
                .AddSingleton(configuration)
                .AddDbContext<AppDbContext>(c => c.UseNpgsql(connectionString: connectionString));
            services.AddRazorPages();
            services.AddCors(k => { k.AddDefaultPolicy(l => { l.AllowAnyHeader(); l.AllowAnyMethod(); l.AllowAnyOrigin(); }); })
                    .AddMvc()
                        .AddRazorPagesOptions(options =>
                        {
                            options.Conventions.AddPageRoute("/RobotsTxt", "/Robots.txt");
                            options.Conventions.AddPageRoute("/NotFound", "/404");
                            options.Conventions.AddPageRoute("/IternalErrorPage", "/500");
                            options.Conventions.AddPageRoute("/Authorize", "/authorize");
                        });

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
