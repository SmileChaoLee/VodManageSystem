using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using VodManageSystem.Models.DataModels;
using VodManageSystem.Models.Dao;
using Newtonsoft.Json.Serialization;

namespace VodManageSystem
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
            services.AddMvc().AddNewtonsoftJson(
                options =>
                {
                    // the follwing is to keep the properties' name as the as they are defined
                    // when the model is Serialized
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    // for avoiding infinite loop when serializing
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                }
            );

            services.AddDistributedMemoryCache();
            services.AddSession();

            // For pomelo.EntityFrameworkCore.MySql
            var connectionString = Configuration.GetConnectionString("MySqlConnection");
            services.AddDbContext<KtvSystemDBContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddScoped<LanguagesManager>();  // languages management service
            services.AddScoped<SingareasManager>();  // singer areas management service
            services.AddScoped<SingersManager>();    // singers management service
            services.AddScoped<SongsManager>();      // songs management service
            services.AddScoped<PlayerscoreManager>();   // Playerscore Manager service
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
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
