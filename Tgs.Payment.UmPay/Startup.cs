using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;

namespace Tgs.Payment.UmPay
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
                endpoints.MapProfileViewer();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
               .AddUmPay((options, sp) =>
               {
                   var config = sp.GetRequiredService<IConfiguration>();

                   config.GetSection("UmPay").Bind(options);

                   //var jsonOptions = sp.GetRequiredService<IOptions<JsonOptions>>().Value;

                   //options.SystemSerializerOptions = jsonOptions.JsonSerializerOptions;
               })
               .AddSingleton<UmPaySerializer>();

            services
               .AddUmPayRepositories((sp, builder) =>
               {
                   var config = sp.GetRequiredService<IConfiguration>();

                   builder
                       .UseNpgsql(config.GetConnectionString("Tgs"));
               });

            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            services.AddTransient(sp =>
            {
                var options = sp.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;

                return options;
            });

            services.AddOpenTelemetryTracing(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddProfileViewExporter();
            });

            services.AddProfileViewer();
        }
    }
}
