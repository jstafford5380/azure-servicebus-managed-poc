using AzureServiceBusPoc.Lib;
using AzureServiceBusPoc.Lib.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureServiceBusPoc.App1
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAzureServiceBus(config =>
                {
                    config.UseAppSettings(Configuration);
                    config.ScanAssemblies(Assemblies.ContainingTypes(typeof(Startup)));
                    config.UseTypeResolver<TestResolver>();
                    config.WithSubscriptions(builder =>
                    {
                        builder.ConsumeTopic("test-foo", "Test-Poc");
                        builder.ConsumeTopic("test-bar", "Test-Poc");
                        builder.ConsumeQueue(Configuration["AzureServiceBus:EndpointName"]);
                    });

                    if (Environment.IsEnvironment("Local"))
                        config.LimitConcurrencyTo(1);
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
