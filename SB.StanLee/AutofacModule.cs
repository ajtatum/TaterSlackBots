using Autofac;
using AutofacSerilogIntegration;
using Microsoft.Extensions.Logging;
using SB.StanLee.Bots;
using TaterSlackBots.Common.Services;
using TaterSlackBots.Common.Settings;

namespace SB.StanLee
{
	public class AutofacModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//builder.Register(ctx => ctx.Resolve<IOptions<AppSettings>>());

			builder.RegisterLogger();
			
			builder.Register(c => new MarvelService(c.Resolve<IAppSettings>(), c.Resolve<Serilog.ILogger>()))
				.As<IMarvelService>()
				.InstancePerLifetimeScope();

			builder.Register(c =>
					new StanLeeBot(c.Resolve<IAppSettings>(),
						c.Resolve<ILoggerFactory>(),
						c.Resolve<Serilog.ILogger>(),
						c.Resolve<IMarvelService>()))
				.As<ITaterSlackBot>()
				.SingleInstance()
				.OnActivating(async args => await args.Instance.Start());
		}
	}
}
