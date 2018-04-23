using System;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace SB.StanLee.Services
{
    public class LogService
    {
	    public static ILogger GetLogger()
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
			 .Enrich.WithProperty("Application", "StanLeeSlackBot")
			 .WriteTo.Async(a =>
			  {
				  a.Console();

				  a.File(
					new CompactJsonFormatter(),
					@"D:\home\LogFiles\Application\Console.StanLeeSlackBot.txt",
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
