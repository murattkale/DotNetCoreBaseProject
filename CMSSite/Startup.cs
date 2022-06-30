using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CMSSite
{
    public class Startup
    {
        private readonly IConfiguration Configuration;
        private readonly IWebHostEnvironment HostEnvironment;

        public Startup(IConfiguration configuration,
            IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region BaseServices

            //add controller with views support
            var mvcBuilder = services.AddControllersWithViews();
            if (HostEnvironment.IsDevelopment())
                mvcBuilder.AddRazorRuntimeCompilation();


            mvcBuilder.AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = null,
                };
            });


            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = null,
                }
            };



            services.AddMvc(option => option.EnableEndpointRouting = false);

            //services.AddDistributedMemoryCache();//To Store session in Memory, This is default implementation of IDistributedCache    
            services.AddSession(s => s.IdleTimeout = TimeSpan.FromHours(1));
            //services.ConfigureApplicationCookie(o =>
            //{
            //    o.ExpireTimeSpan = TimeSpan.FromHours(1);
            //    //o.SlidingExpiration = true;
            //});

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);


            services.AddSingleton<IBaseModel, BaseModel>();

            services.AddHttpClient<IHttpClientWrapper, HttpClientWrapper>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddSingleton<ISendMail, SendMail>();


            services.AddHttpContextAccessor();
            #endregion




        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IConfiguration config)
        {
            if (HostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseMiddleware<ErrorMid>();

            SessionRequest.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());

            app.UseMvc(routes =>
            {

                routes.MapRoute(
                  "Ajax",
                   "/Base/{action}/{id?}",
                  defaults: new { site = "", controller = "Base", action = "", link = "", id = "" }
                 );


                routes.MapRoute(
                   name: "BaseContent",
                   template: "{*link}",
                   constraints: new { site = new DynamicRouting() },
                   defaults: new { site = "", controller = "Base", action = "BaseContent", link = "" }
               );


                routes.MapRoute(name: "default", template: "{controller=Base}/{action=Index}/{Id?}");
            });


        }
    }
}
