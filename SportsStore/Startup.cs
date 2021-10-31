using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using Microsoft.AspNetCore.Identity;

namespace SportsStore
{
    public class Startup
    {

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }
        private IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // ConfigureServices sets up objects (services) which can be used throughout the application.
            // Services are accessed through dependency injection.

            // Set up the shared objects required by applications using the MVC Framework and the Razor view engine
            services.AddControllersWithViews();

            // Configures Entity Framework Core (registers the db context class and configures relationship w/ DB)
            services.AddDbContext<StoreDbContext>(opts => {
                opts.UseSqlServer(
                    Configuration["ConnectionStrings:SportsStoreConnection"]);
            });

            // Add Scoped creates a service where each HTTP request gets its own repository object, which is typical for EF Core
            services.AddScoped<IStoreRepository, EFStoreRepository>();

            services.AddScoped<IOrderRepository, EFOrderRepository>();

            // Enables Razor pages
            services.AddRazorPages();

            // Sessions
            // sets up the in-memory data store.
            services.AddDistributedMemoryCache();
            // registers the services used to access session data
            services.AddSession();

            // specifies that the same object should be used to satisfy related requests for Cart instances.
            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
            // specifies that the same object should always be used.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // creates services used by blazor
            services.AddServerSideBlazor();

            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(
                    Configuration["ConnectionStrings:IdentityConnection"]));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsProduction())
            {
                app.UseExceptionHandler("/error");
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }

            // This extension method displays details of exceptions that occur in the application. Remove before productionalizing.
            app.UseDeveloperExceptionPage();
            // This extension method adds a simple message to HTTP responses that would not otherwise have a body, such as 404 - Not Found responses. 
            app.UseStatusCodePages();
            // This extension method enables support for serving static content from the wwwroot folder.
            app.UseStaticFiles();
            // Sessions - allows the session system to automatically associate requests with sessions when they arrive from the client.
            app.UseSession();
            // Adds endpoint routing feature and registers MVC Framework as source of endpoints
            app.UseRouting();

            // Sets up the middleware components that implement security policy (must appear between use routing and endpoints)
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("catpage", "{category}/Page{productPage:int}", new { Controller = "Home", action = "Index" });
                endpoints.MapControllerRoute("page", "Page{productPage:int}", new { Controller = "Home", action = "Index", productPage = 1 });
                endpoints.MapControllerRoute("category", "{category}", new { Controller = "Home", action = "Index", productPage = 1 });
                endpoints.MapControllerRoute("pagination", "Products/Page{productPage}", new { Controller = "Home", action = "Index", productPage = 1 });

                // The config this method declares, declares that the Index action method defined by the Home controller will be used to handle requests
                endpoints.MapDefaultControllerRoute();

                // registers razor pages as endpoints that the url routing system can use to handle requests
                endpoints.MapRazorPages();

                // registers blazor middleware components
                endpoints.MapBlazorHub();

                // finise routing system
                endpoints.MapFallbackToPage("/admin/{*catchall}", "/Admin/Index");

            });

            // Ensures that there is some data available
            SeedData.EnsurePopulated(app);
            IdentitySeedData.EnsurePopulated(app);


            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //app.UseRouting();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
        }
    }
}
