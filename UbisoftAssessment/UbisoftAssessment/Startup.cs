using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Filters;
using UbisoftAssessment.Data;
using UbisoftAssessment.Data.Interfaces;
using UbisoftAssessment.Services;
using UbisoftAssessment.Services.Interfaces;

namespace UbisoftAssessment
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
            services.AddMvc().AddViewLocalization(o => o.ResourcesPath = "Resources");

            services.AddScoped<IFeedbackContext, FeedbackContext>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddSingleton<CommonLocalizationService>();


            services.AddControllers();
            services.AddSwaggerExamples();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "Feedback API", Version = "V1" });
                config.ExampleFilters();
                var filePath = Path.Combine(AppContext.BaseDirectory, "UbisoftAssessment.xml");
                config.IncludeXmlComments(filePath);
            });

            // Register the MongoClient and the index configuration service
            services.AddSingleton<IMongoClient>(new MongoClient(Configuration["DatabaseSettings:ConnectionString"]));
            services.AddHostedService<ConfigureMongoDbIndexesService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
