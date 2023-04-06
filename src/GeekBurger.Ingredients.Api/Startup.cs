using AutoMapper;
using GeekBurger.Ingredients.Api.AutoMapper;
using GeekBurger.Ingredients.Api.Data;
using GeekBurger.Ingredients.Api.Data.Context;
using GeekBurger.Ingredients.Api.Data.Intefaces;
using GeekBurger.Ingredients.Api.Events;
using GeekBurger.Ingredients.Api.Events.Interfaces;
using GeekBurger.Ingredients.Api.Models;
using GeekBurger.Ingredients.Api.Services;
using GeekBurger.Ingredients.Api.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Threading.Tasks;

namespace GeekBurger.Ingredients.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public async void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(m => m.AddProfile(new ApplicationProfile()));

            RegisterDependencies(services);

            await RunBackgroundTasks(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Geek Burger Ingredients API", Version = "v1" });
            });

            services.AddMvc();
        }

        private void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<GeekBurgerContext>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddSingleton<ILabelImageReceived, LabelImageReceived>();
            services.AddSingleton<IProductChanged, ProductChanged>();

            var configuration = Configuration.Get<Configuration>();
            services.AddSingleton(configuration);
        }

        private async Task RunBackgroundTasks(IServiceCollection services)
        {
            IServiceProvider provider = services.BuildServiceProvider();
            var labelImageReceived = provider.GetService<ILabelImageReceived>();
            var productChanged = provider.GetService<IProductChanged>();

            await labelImageReceived.ProcessMessages();
            await productChanged.ProcessMessages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Geek Burger Ingredients API");
            });

            app.UseCors((c) => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseMvc();
        }
    }
}
