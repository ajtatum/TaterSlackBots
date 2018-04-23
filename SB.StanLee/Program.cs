using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Autofac.Extensions.DependencyInjection;
using DasMulli.Win32.ServiceUtils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using DasMulli.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using SB.StanLee.Services;
using Serilog;
using static System.Console;

namespace SB.StanLee
{
	class Program
	{
		private const string RunAsServiceFlag = "--run-as-service";
		private const string ServiceWorkingDirectoryFlag = "--working-directory";
		private const string RegisterServiceFlag = "--register-service";
		private const string PreserveWorkingDirectoryFlag = "--preserve-working-directory";
		private const string UnregisterServiceFlag = "--unregister-service";
		private const string InteractiveFlag = "--interactive";

		private const string ServiceName = "StanLeeSlackBot";
		private const string ServiceDisplayName = "Stan Lee SlackBot";
		private const string ServiceDescription = "An unofficial SlackBot for Stan Lee.";

		public static void Main(string[] args)
		{
			try
			{
				Log.Logger = LogService.GetLogger();

				if (args.Contains(RunAsServiceFlag))
				{
					Log.Information("RunAsService");
					args = args.Where(a => a != RunAsServiceFlag).ToArray();
					RunAsService(args);
				}
				else if (args.Contains(RegisterServiceFlag))
				{
					Log.Information("RegisterService");
					RegisterService();
				}
				else if (args.Contains(UnregisterServiceFlag))
				{
					Log.Information("UnregisterService");
					UnregisterService();
				}
				else if (args.Contains(InteractiveFlag))
				{
					Log.Information("RunInteractive");
					RunInteractive(args);
				}
				else
				{
					Log.Information("DisplayHelp");
					DisplayHelp();
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "An error occured.");
				WriteLine($"An error ocurred: {ex.Message}");
			}
		}

		private static void RunAsService(string[] args)
		{
			// easy fix to allow using default web host builder without changes
			var wdFlagIndex = Array.IndexOf(args, ServiceWorkingDirectoryFlag);
			if (wdFlagIndex >= 0 && wdFlagIndex < args.Length - 1)
			{
				var workingDirectory = args[wdFlagIndex + 1];
				Directory.SetCurrentDirectory(workingDirectory);
				// remove the flat + argument from the args passed to the web host builder
				args = args.Take(wdFlagIndex).Concat(args.Skip(wdFlagIndex + 2)).ToArray();
			}
			else
			{
				Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			}

			Log.Information($@"Starting ""{ServiceName}"" As a Service with {string.Join(", ", args)}");
			BuildWebHost(args).RunAsService(ServiceName);
		}

		private static void RunInteractive(string[] args)
		{
			Log.Information($@"Starting ""{ServiceName}"" with {string.Join(", ", args)}");
			BuildWebHost(args.Where(a => a != InteractiveFlag).ToArray()).Run();
		}

		private static void RegisterService()
		{
			// Environment.GetCommandLineArgs() includes the current DLL from a "dotnet my.dll --register-service" call, which is not passed to Main()
			var commandLineArgs = Environment.GetCommandLineArgs();

			var serviceArgs = commandLineArgs
				.Where(arg => arg != RegisterServiceFlag && arg != PreserveWorkingDirectoryFlag)
				.Select(EscapeCommandLineArgument)
				.Append(RunAsServiceFlag);

			var host = Process.GetCurrentProcess().MainModule.FileName;

			if (!host.EndsWith("dotnet.exe", StringComparison.OrdinalIgnoreCase))
			{
				// For self-contained apps, skip the dll path
				serviceArgs = serviceArgs.Skip(1);
			}

			if (commandLineArgs.Contains(PreserveWorkingDirectoryFlag))
			{
				serviceArgs = serviceArgs
					.Append(ServiceWorkingDirectoryFlag)
					.Append(EscapeCommandLineArgument(Directory.GetCurrentDirectory()));
			}

			var fullServiceCommand = host + " " + string.Join(" ", serviceArgs);

			// Do not use LocalSystem in production.. but this is good for demos as LocalSystem will have access to some random git-clone path
			// Note that when the service is already registered and running, it will be reconfigured but not restarted
			var serviceDefinition = new ServiceDefinitionBuilder(ServiceName)
				.WithDisplayName(ServiceDisplayName)
				.WithDescription(ServiceDescription)
				.WithBinaryPath(fullServiceCommand)
				.WithCredentials(Win32ServiceCredentials.LocalSystem)
				.WithAutoStart(true)
				.Build();

			new Win32ServiceManager().CreateOrUpdateService(serviceDefinition, startImmediately: true);

			Log.Information($@"Successfully registered and started service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
			WriteLine($@"Successfully registered and started service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
		}

		private static void UnregisterService()
		{
			new Win32ServiceManager()
				.DeleteService(ServiceName);

			Log.Information($@"Successfully unregistered service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
			WriteLine($@"Successfully unregistered service ""{ServiceDisplayName}"" (""{ServiceDescription}"")");
		}

		private static void DisplayHelp()
		{
			WriteLine(ServiceDescription);
			WriteLine();
			WriteLine("This demo application is intened to be run as windows service. Use one of the following options:");
			WriteLine();
			WriteLine("  --register-service            Registers and starts this program as a windows service named \"" + ServiceDisplayName + "\"");
			WriteLine("                                All additional arguments will be passed to ASP.NET Core's WebHostBuilder.");
			WriteLine();
			WriteLine("  --preserve-working-directory  Saves the current working directory to the service configuration.");
			WriteLine("                                Set this wenn running via 'dotnet run' or when the application content");
			WriteLine("                                is not located nex to the application.");
			WriteLine();
			WriteLine("  --unregister-service          Removes the windows service creatd by --register-service.");
			WriteLine();
			WriteLine("  --interactive                 Runs the underlying asp.net core app. Useful to test arguments.");
		}

		private static string EscapeCommandLineArgument(string arg)
		{
			// http://stackoverflow.com/a/6040946/784387
			arg = Regex.Replace(arg, @"(\\*)" + "\"", @"$1$1\" + "\"");
			arg = "\"" + Regex.Replace(arg, @"(\\+)$", @"$1$1") + "\"";
			return arg;
		}

		public static IWebHost BuildWebHost(string[] args)
		{
			Log.Information($"Building Web Host with {string.Join(", ", args)}");

			/* 
             * create an override configuration based on the command line args
             * to work around ASP.NET Core issue https://github.com/aspnet/MetaPackages/issues/221
             * wich should be fixed in 2.1.0.
             */
			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			if (string.IsNullOrWhiteSpace(environment))
				throw new NullReferenceException("Environment not found in ASPNETCORE_ENVIRONMENT");

			WriteLine("Environment: {0}", environment);
			
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddCommandLine(args)
				.Build();

			return WebHost.CreateDefaultBuilder(args)
				.UseConfiguration(builder)
				.ConfigureAppConfiguration((builderContext, config) =>
				{
					config.AddJsonFile("appsettings.json", false, true);
					config.AddJsonFile($"appsettings.{environment}.json", true, true);

					config.AddEnvironmentVariables();
				})
				.UseContentRoot(Directory.GetCurrentDirectory())
				.ConfigureServices(services =>
				{
					services.AddAutofac();
				})
				.UseStartup<Startup>()
				.UseSerilog((hostingContext, loggerConfiguration) => LogService.GetLogger())
				.Build();
		}
	}
}
