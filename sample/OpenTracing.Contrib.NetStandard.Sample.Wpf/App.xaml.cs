using Jaeger;
using Jaeger.Samplers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing.Util;
using System;
using System.Net;
using System.Threading;
using System.Windows;

namespace OpenTracing.Contrib.NetStandard.Sample.Wpf
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public static IServiceProvider ServiceProvider;

		public App()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton<ILoggerFactory, LoggerFactory>();
			serviceCollection.AddSingleton<ITracer>(s =>
			{
				var sampler = new ConstSampler(true);
				var tracer = new Tracer.Builder("wpf tracing")
											.WithLoggerFactory(s.GetRequiredService<ILoggerFactory>())
											.WithSampler(sampler)
											.WithTag("agent", "wpf")
											.Build();

				GlobalTracer.Register(tracer);
				return tracer;
			});
			serviceCollection.AddSingleton<ITracingService>(s =>
				OpenTracingServiceCollectionExtensions.AddOpenTracing(
					s.GetRequiredService<ITracer>(),
					s.GetRequiredService<ILoggerFactory>(),
					openTracingBuilder => 
						openTracingBuilder.ConfigureHttpClientHandler(options => options.OperationNameResolver = httpRequest => $"{httpRequest.Method.Method}-{httpRequest.RequestUri.AbsoluteUri}")
										  .ConfigureDesktopHttpClientHandler(options => options.OperationNameResolver = webRequest => $"{webRequest.Method}-{webRequest.RequestUri.AbsoluteUri}")
				)
			);
			ServiceProvider = serviceCollection.BuildServiceProvider();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			ServiceProvider.GetRequiredService<ITracingService>().StartAsync(CancellationToken.None);
			base.OnStartup(e);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			ServiceProvider.GetRequiredService<ITracingService>().StopAsync(CancellationToken.None).Wait();
			base.OnExit(e);
		}
	}
}
