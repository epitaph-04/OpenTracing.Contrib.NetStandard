using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace OpenTracing.Contrib.NetStandard.Internal
{
    internal abstract class DiagnosticObserver
    {
        protected ILogger Logger { get; }

        protected ITracer Tracer { get; }

        protected bool IsLogLevelTraceEnabled { get; }

        protected DiagnosticObserver(ILoggerFactory loggerFactory, ITracer tracer)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));
			Logger = loggerFactory.CreateLogger(GetType());
            Tracer = tracer ?? throw new ArgumentNullException(nameof(tracer));

            IsLogLevelTraceEnabled = Logger.IsEnabled(LogLevel.Trace);
        }

        public virtual bool IsSubscriberEnabled()
        {
            return true;
        }

        public abstract IDisposable SubscribeIfMatch(DiagnosticListener diagnosticListener);
    }
}
