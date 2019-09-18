using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OpenTracing.Contrib.NetStandard.CoreFx;
using OpenTracing.Contrib.NetStandard.Internal;
using OpenTracing.Contrib.NetStandard.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OpenTracingBuilderExtensions
    {
        internal static IOpenTracingBuilder AddDiagnosticSubscriber<TDiagnosticSubscriber>(this IOpenTracingBuilder builder)
            where TDiagnosticSubscriber : DiagnosticObserver
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<DiagnosticObserver, TDiagnosticSubscriber>());

            return builder;
        }

		/// <summary>
		/// Adds instrumentation for the .NET framework BCL.
		/// </summary>
		public static IOpenTracingBuilder AddHttpClientHandler(this IOpenTracingBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			builder.AddDiagnosticSubscriber<GenericDiagnostics>();

			builder.AddDiagnosticSubscriber<HttpDiagnostics>();
			builder.ConfigureGenericDiagnostics(options => options.IgnoredListenerNames.Add(HttpDiagnostics.DiagnosticListenerName));

			builder.AddDiagnosticSubscriber<HttpHandlerDiagnostics>();
			builder.ConfigureGenericDiagnostics(options => options.IgnoredListenerNames.Add(HttpHandlerDiagnostics.DiagnosticListenerName));

            return builder;
        }

		/// <summary>
		/// Configuration instrumentation for the .NET framework BCL.
		/// </summary>
		public static IOpenTracingBuilder ConfigureHttpClientHandler(this IOpenTracingBuilder builder, Action<HttpHandlerDiagnosticOptions> options)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			if (options != null)
			{
				builder.Services.Configure(options);
			}

			return builder;
		}

		public static IOpenTracingBuilder ConfigureDesktopHttpClientHandler(this IOpenTracingBuilder builder, Action<HttpDiagnosticOptions> options)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			if (options != null)
			{
				builder.Services.Configure(options);
			}

			return builder;
		}

		public static IOpenTracingBuilder ConfigureGenericDiagnostics(this IOpenTracingBuilder builder, Action<GenericDiagnosticOptions> options)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (options != null)
            {
                builder.Services.Configure(options);
            }

            return builder;
        }

        public static IOpenTracingBuilder AddLoggerProvider(this IOpenTracingBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, OpenTracingLoggerProvider>());
            builder.Services.Configure<LoggerFilterOptions>(options =>
            {
                // All interesting request-specific logs are instrumented via DiagnosticSource.
                options.AddFilter<OpenTracingLoggerProvider>("Microsoft.AspNetCore.Hosting", LogLevel.None);
            });

            return builder;
        }
    }
}
