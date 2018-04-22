using Autofac;
using AutofacSerilogIntegration;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using SlackBotNet;
using StanLeeSlackBot.Bots;
using StanLeeSlackBot.Classes;
using StanLeeSlackBot.Middleware;
using StanLeeSlackBot.Services;

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

			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; private set; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();
			services.AddSingleton(Configuration);
			services.Configure<AppSettings>(Configuration);
		}

		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterModule(new AutofacModule());
		}

		//private static void HandleStanLeeBot(IApplicationBuilder app, IStanLeeBot stanLeeBot)
		//{
		//	app.Run(async context =>
		//	{
		//		await stanLeeBot.Start();
		//		await context.Response.WriteAsync("StanLeeSlackBot coming online.");
		//	});
		//}

		public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, IHostingEnvironment env, IStanLeeBot stanLeeBot)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMiddleware<SerilogMiddleware>();

			//app.Map("/stanlee", c => HandleStanLeeBot(app, stanLeeBot));

			app.Run(async context =>
			{
				await context.Response.WriteAsync("Whatcha looking for?");
			});
		}
	}
}