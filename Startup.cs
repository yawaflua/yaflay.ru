
namespace yaflay.ru
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        public Startup()
        {
            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "m.")
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
            
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/RobotsTxt", "/Robots.txt");
                    options.Conventions.AddPageRoute("/ErrorPage", "/404");
                });
            services.AddRouting();
            services.AddRazorPages();
            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/RobotsTxt", "/Robots.txt");
                    options.Conventions.AddPageRoute("/ErrorPage", "/404");
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
