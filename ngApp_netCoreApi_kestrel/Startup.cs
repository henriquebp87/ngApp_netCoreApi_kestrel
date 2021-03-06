using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ngApp_netCoreApi_kestrel
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Map the #SpaSettings section to the <see cref=SpaSettings /> class
            services.Configure<SpaSettings>(_configuration.GetSection("SpaSettings"));

            var isDocker = Environment.GetEnvironmentVariable("DOCKER");

            if (bool.TryParse(isDocker, out var result))
            {
                var redis = ConnectionMultiplexer.Connect("172.17.0.3:6379");
                services.AddScoped(x => redis.GetDatabase());
            }

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<SpaSettings> spaSettings)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            ConfigureRoutes(app, spaSettings.Value);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureRoutes(IApplicationBuilder app, SpaSettings spaSettings)
        {
            // If the route contains 'api' then assume a controller action to be served
            // if the route is spa route then let it fall through to the
            // spa index file and have it resolved by the spa application
            app.MapWhen(context =>
                {
                    var path = context.Request.Path.Value;
                    return !path.Contains("api");
                },
                spa =>
                {
                    spa.Use((context, next) =>
                    {
                        context.Request.Path = new PathString("/" + spaSettings.DefaultPage);
                        return next();
                    });

                    spa.UseStaticFiles();
                });

            // reserved for custom routes: internationalization etc.
            // var routeBuilder = new RouteBuilder(app);
            // app.UseRouter(routeBuilder.Build());
        }
    }
}
