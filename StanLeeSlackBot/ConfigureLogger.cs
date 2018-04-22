using System;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers.AzureWebApps;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace StanLeeSlackBot
{
    public class ConfigureLogger
    {
	    public static Logger CreateLogger()
	    {
		    var seriLogger = new LoggerConfiguration()
			    .MinimumLevel.Information()
			    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
			    .MinimumLevel.Override("System", LogEventLevel.Information)
			    .Enrich.FromLogContext()
			    .Enrich.WithThreadId()
			    .Enrich.WithProcessName()
			    .Enrich.WithProcessId()
			    .Enrich.WithExceptionDetails()
			    .Enrich.With<AzureWebAppsNameEnricher>()
			    .Enrich.WithProperty("Application", "StanLeeSlackBot")
			    .WriteTo.Async(a =>
				    {
					    a.Console();

						a.File(
						    new CompactJsonFormatter(),
						    @"D:\home\LogFiles\Application\StanLeeLog.txt",
						    fileSizeLimitBytes: 1_000_000,
						    rollOnFileSizeLimit: true,
						    shared: true,
						    rollingInterval: RollingInterval.Day,
						    flushToDiskInterval: TimeSpan.FromSeconds(1));
				    },
				    bufferSize: 500)
			    .CreateLogger();

		    return seriLogger;
	    }
    }
}
