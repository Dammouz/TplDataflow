using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace TplDataflow
{
    public class Startup
    {
        private const string _version = "v1";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                    // Remove null property to ouput.
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.IgnoreNullValues = true;
                    });

            // Register the Swagger generator, defining 1 or more Swagger documents.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(_version, new OpenApiInfo
                {
                    Version = _version,
                    Title = "TPL Dataflow",
                    Description = "TPL Dataflow with Swagger C# API",
                    //TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Dammouz",
                        Url = new Uri("https://github.com/Dammouz/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "GNU GENERAL PUBLIC LICENSE",
                        Url = new Uri("https://www.gnu.org/licenses/"),
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{_version}/swagger.json", $"TplDataflow {_version}");
                //c.SupportedSubmitMethods(new SubmitMethod[]
                //{
                //    SubmitMethod.Get
                //});
                //c.RoutePrefix = "RoutePrefixTest";
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Use HTTPS.
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
