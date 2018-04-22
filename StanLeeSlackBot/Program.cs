using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace StanLeeSlackBot
{
	public class Program
	{
		public static int Main(string[] args)
		{
			try
			{
				Log.Information("Starting web host");
				BuildWebHost(args).Run();
				return 0;
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Host terminated unexpectedly");
				return 1;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IWebHost BuildWebHost(string[] args)
		{
			return new WebHostBuilder()
				.UseKestrel()
				.ConfigureServices(services => services.AddAutofac())
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseStartup<Startup>()
				.UseSerilog()
				.UseApplicationInsights()
				.UseAzureAppServices()
				.Build();
		}
	}
}