using Autofac;
using AutofacSerilogIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StanLeeSlackBot.Classes;
using StanLeeSlackBot.Middleware;

namespace StanLeeSlackBot
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
				.AddApplicationInsightsSettings()
				.AddEnvironmentVariables();

			if (env.IsDevelopment())
			{
				builder.AddUserSecrets<Program>(false);
			}

			this.Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; private set; }

		public ILogger Logger { get; private set; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();
			services.AddMvc();
			services.AddDirectoryBrowser();
			services.AddSingleton(Configuration);
			services.Configure<AppSettings>(Configuration);
		}

		public void ConfigureContainer(ContainerBuilder builder)
		{
			var appInsight = Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey");
			Logger = ConfigureLogger.CreateLogger(appInsight);

			builder.RegisterLogger(autowireProperties: true, logger: Logger);
		}

		public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMiddleware<SerilogMiddleware>();

			app.UseMvc();

			app.Run(async context =>
			{
				await context.Response.WriteAsync("StanLeeSlackBot coming online.");
			});
		}
	}
}