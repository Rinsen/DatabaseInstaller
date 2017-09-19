using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Rinsen.DatabaseInstaller;

namespace Rinsen.DatabaseInstallerWeb
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; set; }
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseInstaller(Configuration["Data:DefaultConnection:ConnectionString"]);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDatabaseInstaller(options =>
            {
                options.DatabaseVersions.Add(new MyVersion());
            });

            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }

    public class MyVersion : DatabaseVersion
    {
        public MyVersion() 
            : base(1)
        {

        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            throw new NotImplementedException();
        }
    }
}
