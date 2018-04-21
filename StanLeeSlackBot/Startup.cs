using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using StanLeeSlackBot.Configuration;

namespace StanLeeSlackBot
{
	public class Startup
	{
		//public IConfiguration Configuration { get; }
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		//public Startup(IHostingEnvironment env)
		//{
		//var builder = new ConfigurationBuilder()
		//	.SetBasePath(env.ContentRootPath)
		//	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
		//	.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
		//	.AddApplicationInsightsSettings()
		//	.AddEnvironmentVariables();

		//if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Development)
		//{
		//	builder.AddUserSecrets<StanLeeSlackBot.Program>(false);
		//}

		//Configuration = builder.Build();

		//Log.Logger = new LoggerConfiguration()
		//	.ReadFrom.Configuration(Configuration)
		//	.Enrich.FromLogContext()
		//	.Enrich.WithThreadId()
		//	.Enrich.WithProcessName()
		//	.Enrich.WithProcessId()
		//	.Enrich.WithExceptionDetails()
		//	.Enrich.With<AzureWebAppsNameEnricher>()
		//	.Enrich.WithProperty("Application", "StanLeeSlackBot")
		//	.WriteTo.Console()
		//	.WriteTo.RollingFile(new CompactJsonFormatter(), "StanLeeLog-{Date}.txt", retainedFileCountLimit: 5)
		//	.WriteTo.ApplicationInsightsEvents(new TelemetryClient())
		//	.CreateLogger();
		//}

		public void ConfigureServices(IServiceCollection services)
		{
			//services.AddOptions();
			//services.AddSingleton<IConfiguration>(Configuration);

			//services.AddApplicationInsightsTelemetry(Configuration);
			services.AddDirectoryBrowser();
		}

		public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseDirectoryBrowser("/data");
			app.UseDirectoryBrowser("/Logs");

			var noobHost = new NoobotHost(new ConfigReader(Configuration.GetSection("Bot")), Log.ForContext<NoobotHost>());
			applicationLifetime.ApplicationStarted.Register(() => noobHost.Start());
			applicationLifetime.ApplicationStopping.Register(noobHost.Stop);

			app.Run(async context =>
			{
				await context.Response.WriteAsync("StanLeeSlackBot coming online.");
			});
		}
	}
}