using Autofac;
using AutofacSerilogIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using StanLeeSlackBot.Bots;
using StanLeeSlackBot.Classes;
using StanLeeSlackBot.Services;

namespace StanLeeSlackBot
{
	public class AutofacModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterLogger();

			builder.Register(c => new MarvelService(c.Resolve<IOptions<AppSettings>>(), c.Resolve<Serilog.ILogger>()))
				.As<IMarvelService>()
				.InstancePerLifetimeScope();

			builder.Register(c =>
					new StanLeeBot(c.Resolve<IOptions<AppSettings>>(),
						c.Resolve<ILoggerFactory>(),
						c.Resolve<Serilog.ILogger>(),
						c.Resolve<IMarvelService>()))
				.As<IStanLeeBot>()
				.SingleInstance();
		}
	}
}
