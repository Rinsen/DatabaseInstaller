using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Rinsen.DatabaseInstaller;

namespace Rinsen.DatabaseInstallerWeb
{
    public class Startup
    {
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

            app.UseRouting();

            // Add MVC to the request pipeline.
            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
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
