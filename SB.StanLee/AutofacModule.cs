using Autofac;
using AutofacSerilogIntegration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SB.StanLee.Bots;
using SB.StanLee.Classes;
using SB.StanLee.Services;

namespace SB.StanLee
{
	public class AutofacModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//builder.Register(ctx => ctx.Resolve<IOptions<AppSettings>>());

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
				.SingleInstance()
				.OnActivating(async args => await args.Instance.Start());
		}
	}
}
