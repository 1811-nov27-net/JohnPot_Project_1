using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PizzaStoreData.DataAccess;
using PizzaStoreData.DataAccess.Repositories;
using db = PizzaStoreData.DataAccess.Models;

namespace PizzaStoreMVC.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var options = new DbContextOptionsBuilder<PizzaStoreDBContext>()
                .UseSqlServer(Configuration.GetConnectionString("MVCPizzaStore")).Options;
            var database = new PizzaStoreDBContext(options);

            db.Mapper.InitializeMapper(database);
            SyncAllFromDatabase(database);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<PizzaStoreDBContext>(optionsBuilder =>
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("MVCPizzaStore")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddScoped<LocationRepository>();
            services.AddScoped<IngredientRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<OrderRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseCookiePolicy();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public static void SyncAllFromDatabase(PizzaStoreDBContext database)
        {
            // Order here matters.
            Sync.IngredientProcessor.Initialize(database);
            Sync.IngredientProcessor.SyncFromDatabase();
            // Location needs to know what ingredients are availabe
            Sync.LocationProcessor.Initialize(database);
            Sync.LocationProcessor.SyncFromDatabase();

            Sync.UserProcessor.Initialize(database);
            Sync.UserProcessor.SyncFromDatabase();


        }

    }
}
