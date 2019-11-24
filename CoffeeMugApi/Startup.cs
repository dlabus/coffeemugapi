using AutoMapper;
using CoffeeMugApi.DA.Repositories;
using CoffeeMugApi.DL.Services.ProductService;
using CoffeeMugApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace CoffeeMugApi
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
            services.AddControllers();


            //Add mapping for Automapper
            services.AddAutoMapper(typeof(Startup));

            //Add custom services
            services.AddSingleton(new MongoClient(Configuration["Config:ConnectionString"]));
            services.AddTransient<IMongoClient, MongoClient>();
            services.AddTransient<IRepository<DA.DtoModels.Product>, ProductRepository>(
                servicesProvider => new ProductRepository(servicesProvider.GetService<IMongoClient>(), Configuration["Config:Database"]));
            services.AddTransient<IProductService, ProductService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
