using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using todo_domain_entities;
using todo_aspnetmvc_ui.Models.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using todo_aspnetmvc_ui.Models.Data;
using todo_aspnetmvc_ui.Infrastructure;
using Newtonsoft.Json;

namespace todo_aspnetmvc_ui
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
            services.AddControllersWithViews();
            services.AddDbContext<AppDbContext>(opts => {
                opts.UseSqlServer(
                    Configuration["ConnectionStrings:ToDoListDbConnection"]);
            });

            services.AddScoped<IToDoServices, ToDoEFCoreServicesProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("catpage",
                    "{category}/Page{todoListPage:int}",
                    new { Controller = "Home", action = "Index" });

                endpoints.MapControllerRoute("page", 
                    "Page{todoListPage:int}",
                    new { Controller = "Home", action = "Index", todoListPage = 1 });

                endpoints.MapControllerRoute("category", 
                    "{category}",
                    new { Controller = "Home", action = "Index", todoListPage = 1 });

                /*endpoints.MapControllerRoute("pagination",
                    "ToDoLists/Page{todoListPage}",
                    new { Controller = "Home", action = "Index" });*/

                endpoints.MapDefaultControllerRoute();
            });

            SeedData.EnsurePopulated(app);
        }
    }
}
