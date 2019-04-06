using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PizzeriaOnline.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;

namespace PizzeriaOnline
{
    public class Startup
    {
        private IConfiguration Configuration; 

        public Startup( IConfiguration config)
        {
            Configuration = config; 
        } 
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSession(); 

                //options => {
                //    options.IdleTimeout = TimeSpan.FromMinutes(10);
                //    options.CookieName = ".PizzeriaOnlineSessID";
                //});
            services.AddDistributedMemoryCache();

            services.AddDbContext<TomasosContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DbOnlinePizz")));
            services.Configure<RouteOptions>(options => options.AppendTrailingSlash = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "Default",
                template: "{controller=Home}/{action=Index}/{id?}"
                    );

            }); 
        }
    }
}
