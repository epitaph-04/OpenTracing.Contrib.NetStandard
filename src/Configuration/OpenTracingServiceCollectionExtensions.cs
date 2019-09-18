using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Contrib.NetStandard;
using OpenTracing.Contrib.NetStandard.Configuration;
using OpenTracing.Contrib.NetStandard.Internal;
using OpenTracing.Util;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OpenTracingServiceCollectionExtensions
    {
		/// <summary>
		/// Adds OpenTracing instrumentation for CoreFx (BCL).
		/// </summary>
		public static ITracingService AddOpenTracing(ITracer tracer, ILoggerFactory loggerFactory, Action<IOpenTracingBuilder> builder = null)
		{
			IServiceCollection services = new ServiceCollection();
			services.AddSingleton<ITracer>(tracer);
			services.AddSingleton<ILoggerFactory>(loggerFactory);

			var serviceProvider = services.AddOpenTracing(builder).BuildServiceProvider();
			return serviceProvider.GetRequiredService<ITracingService>();
		}

		/// <summary>
		/// Adds OpenTracing instrumentation for CoreFx (BCL).
		/// </summary>
		internal static IServiceCollection AddOpenTracing(this IServiceCollection services, Action<IOpenTracingBuilder> builder = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services.AddOpenTracingCoreServices(otBuilder =>
            {
                otBuilder.AddHttpClientHandler()
						 .AddLoggerProvider();

                builder?.Invoke(otBuilder);
            });
        }

		/// <summary>
		/// Adds the core services required for OpenTracing without any actual instrumentations.
		/// </summary>
		internal static IServiceCollection AddOpenTracingCoreServices(this IServiceCollection services, Action<IOpenTracingBuilder> builder = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton(GlobalTracer.Instance);
            services.TryAddSingleton<IGlobalTracerAccessor, GlobalTracerAccessor>();

            services.TryAddSingleton<DiagnosticManager>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ITracingService, InstrumentationService>());

            var builderInstance = new OpenTracingBuilder(services);

            builder?.Invoke(builderInstance);

            return services;
        }
    }
}
