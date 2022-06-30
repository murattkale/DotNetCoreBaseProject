using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using ThreeDPayment;

namespace API
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
            services.AddCors();
            services.AddControllers();


            services
                .AddMvc(options =>
                {
                    options.AllowEmptyInputInBodyModelBinding = true;
                }) //options => options.EnableEndpointRouting = false
                .AddJsonOptions(options =>
                {//Geriye dönen json nesnesinin isimlerini büyük küçük yapmasýn diye eklendi.
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;

                });

            //Newton json nesnesinin bazý özelliklerinin çevrilmesi eklendi.
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver()
                {
                    NamingStrategy = null,
                };
            });


            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;

            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
                options.AutomaticAuthentication = false;

            });



            services.AddEntityFrameworkSqlServer().AddDbContext<myDBContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("myDBContext"), b => b.MigrationsAssembly("myDBContext")));
          
            services.AddScoped(typeof(IBaseModel), typeof(BaseModel));

            services.AddScoped(typeof(IGenericRepo<IBaseModel>), typeof(GenericRepo<myDBContext, IBaseModel>));
            services.AddScoped(typeof(IUnitOfWork<myDBContext>), typeof(UnitOfWork<myDBContext>));



            //Dynamic Injection
            var allprops = AppDomain.CurrentDomain.GetAssemblies();
            var props = allprops.Where(o => o.ManifestModule.Name == "Services.dll").FirstOrDefault().DefinedTypes;
            var servicesAll = props.Where(o => (!o.IsInterface && o.BaseType.Name.Contains("GenericRepo"))).ToList();
            servicesAll.ForEach(baseService =>
            {
                services.AddScoped(baseService.GetInterface("I" + baseService.Name), baseService);
            });


            services.AddHttpClient();
            services.AddSingleton<IPaymentProviderFactory, PaymentProviderFactory>();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);


            services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            //API Swagger için dökümantasyon çýkartma iþlemi eklendi.
            services.AddSwaggerGen(c =>
            {
                //use fully qualified object names
                c.CustomSchemaIds(x => x.FullName);

                c.SwaggerDoc("api", new OpenApiInfo { Title = "API", Version = "api" });

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
                {
                    Description = "Basic Authorization header using the Bearer scheme. Example: \"Authorization: Basic {username:password}\"",
                    Name = "Authorization",
                    Scheme = "Basic",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Basic" }
                        },
                        new string[] {  }
                    }
                });


            });


            services.AddDistributedMemoryCache();//To Store session in Memory, This is default implementation of IDistributedCache    
            services.AddSession(s => { s.IdleTimeout = TimeSpan.FromMinutes(60); s.Cookie.HttpOnly = true; });
            services.AddHttpContextAccessor();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();


            //app.UseHsts();
            //app.UseHttpsRedirection();


            app.UseMiddleware<ErrorMid>();
            app.UseCors(x => { x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();


        

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger(c =>
            {
                //c.SerializeAsV2 = true;
                //c.RouteTemplate = live_path + "/swagger/{documentName}/swagger.json";
            });

            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));

            app.UseSwaggerUI(c =>
            {
                c.DefaultModelsExpandDepth(-1);
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.SwaggerEndpoint($"/swagger/api/swagger.json", "API");
            });

            app.InitializeDatabase();

        }
    }
}
