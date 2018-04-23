using System;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SB.StanLee.Bots;
using SB.StanLee.Classes;
using SB.StanLee.Middleware;
using Serilog;

namespace SB.StanLee
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; private set; }

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

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IStanLeeBot stanLeeBot)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMiddleware<SerilogMiddleware>();

			app.Run(async context =>
			{
				await context.Response.WriteAsync("Hey Stan! You're up!");
			});
		}
	}
}
