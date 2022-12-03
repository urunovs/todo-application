using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using todo_domain_entities;
using todo_aspnetmvc_ui.Models.Services;
using Microsoft.EntityFrameworkCore;

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
                    Configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]);
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
                endpoints.MapControllerRoute("fetchByDuedate", 
                    "{duedate}",
                    new { Controller = "Home", action = "Index", page = 1 });

                endpoints.MapControllerRoute("openTodolist",
                    "todolists/{toDoListId:int}",
                    new { Controller = "ToDoLists", action = "OpenToDoList" });

                endpoints.MapDefaultControllerRoute();
            });

            app.ApplicationServices
                .CreateScope().ServiceProvider
                .GetRequiredService<IToDoServices>()
                .EnsurePopulatedWithDemoData();
        }
    }
}
