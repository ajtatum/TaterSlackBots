using Autofac;
using AutofacSerilogIntegration;

namespace StanLeeSlackBot
{
	public class AutofacModule : Module
	{
		private static string ApplicationInsightsKey { get; set; }

		public AutofacModule(string applicationInsightsKey)
		{
			ApplicationInsightsKey = applicationInsightsKey;
		}

		protected override void Load(ContainerBuilder builder)
		{
			
		}
	}
}
