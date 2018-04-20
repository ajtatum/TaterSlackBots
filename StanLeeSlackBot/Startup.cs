using System;
using System.IO;
using Common.Logging;
using Common.Logging.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using StanLeeSlackBot.Configuration;
using StanLeeSlackBot.Logging;

namespace StanLeeSlackBot
{
    public class Startup
    {
		public IConfiguration Configuration { get; }

		public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            
            var logConfiguration = new LogConfiguration();
            Configuration.GetSection("LogConfiguration").Bind(logConfiguration);
            LogManager.Configure(logConfiguration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
			services.AddSingleton<IMemoryLog, MemoryLog>();
	        services.AddSingleton<IConfiguration>(Configuration);
			services.AddDirectoryBrowser();
		}
        
        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, IHostingEnvironment env, IMemoryLog log)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			app.UseDirectoryBrowser("/data");

			var noobHost = new NoobotHost(new ConfigReader(Configuration.GetSection("Bot")), log);
            applicationLifetime.ApplicationStarted.Register(() => noobHost.Start());
            applicationLifetime.ApplicationStopping.Register(noobHost.Stop);

			app.Run(async context =>
            {
                await context.Response.WriteAsync(string.Join(Environment.NewLine, log.FullLog()));
            });
        }
    }
}