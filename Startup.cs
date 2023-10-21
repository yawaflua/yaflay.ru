using Microsoft.Extensions.Options;
using HeyRed.OEmbed;
using HeyRed.OEmbed.Providers;

namespace yaflay.ru
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly OEmbedOptions options;
        public Startup()
        {
            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "m.")
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
            options = new OEmbedOptions()
            {
                EnableCache = true
            };
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRouting();
            services.AddRazorPages();
            services.AddOEmbed(options =>
            {
                options.EnableCache = true; // true by default
                options.EnsureNotNull();
            })
                .ClearProviders()
                .AddProvider<YoutubeProvider>()
                .AddProvider<YandexMusicProvider>()
                .AddProvider<FlickrProvider>()
                .AddProvider<TwitterProvider>(options =>
                {
                    options.Parameters = new Dictionary<string, string?>
                    {
                        ["theme"] = "dark"
                    };
                })
                .AddProvider<FacebookProvider>(options =>
                {
                    options.Parameters = new Dictionary<string, string?>
                    {
                        ["access_token"] = "app_id|token"
                    };
                });
            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/robotstxt", "/Robots.Txt");
                });
            //services.AddDirectoryBrowser();



        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Add services to the container.
            // app.Services.AddRazorPages();


            // Configure the HTTP request pipeline.

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                
            });

        }
    }
}
