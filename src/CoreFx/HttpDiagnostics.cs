using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing.Contrib.NetStandard.Internal;
using OpenTracing.Propagation;
using OpenTracing.Tag;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpenTracing.Contrib.NetStandard.CoreFx
{
	internal sealed class HttpDiagnostics : DiagnosticListenerObserver
	{
		public const string DiagnosticListenerName = "System.Net.Http.Desktop";

		private const string PropertiesKey = "ot-Span";

		private static readonly PropertyFetcher _activityStart_RequestFetcher = new PropertyFetcher("Request");
		private static readonly PropertyFetcher _activityStop_RequestFetcher = new PropertyFetcher("Request");
		private static readonly PropertyFetcher _activityStop_ResponseFetcher = new PropertyFetcher("Response");
		private static readonly PropertyFetcher _activityStop_RequestTaskStatusFetcher = new PropertyFetcher("RequestTaskStatus");
		private static readonly PropertyFetcher _exception_RequestFetcher = new PropertyFetcher("Request");
		private static readonly PropertyFetcher _exception_ExceptionFetcher = new PropertyFetcher("Exception");

		private readonly HttpDiagnosticOptions _options;

		protected override string GetListenerName() => DiagnosticListenerName;

		private ConcurrentDictionary<string, ISpan> spanContainer;

		public HttpDiagnostics(
			ILoggerFactory loggerFactory,
			ITracer tracer,
			IOptions<HttpDiagnosticOptions> options,
			IOptions<GenericEventOptions> genericEventOptions)
			: base(loggerFactory, tracer, genericEventOptions.Value)
		{
			_options = options?.Value ?? throw new ArgumentNullException(nameof(options));
			spanContainer = new ConcurrentDictionary<string, ISpan>();
		}

		protected override void OnNext(string eventName, object arg)
		{
			switch (eventName)
			{
				case "System.Net.Http.Desktop.HttpRequestOut.Start":
					{
						var request = (HttpWebRequest)_activityStart_RequestFetcher.Fetch(arg);

						string operationName = _options.OperationNameResolver(request);

						ISpan span = Tracer.BuildSpan(operationName)
							.WithTag(Tags.SpanKind, Tags.SpanKindClient)
							.WithTag(Tags.Component, _options.ComponentName)
							.WithTag(Tags.HttpMethod, request.Method.ToString())
							.WithTag(Tags.HttpUrl, request.RequestUri.ToString())
							.WithTag(Tags.PeerHostname, request.RequestUri.Host)
							.WithTag(Tags.PeerPort, request.RequestUri.Port)
							.Start();

						_options.OnRequest?.Invoke(span, request);

						if (_options.InjectEnabled?.Invoke(request) ?? true)
						{
							Tracer.Inject(span.Context, BuiltinFormats.HttpHeaders, new HttpHeadersInjectAdapter(request.Headers));
						}
						spanContainer.TryAdd(request.Headers["uber-trace-id"], span);
					}
					break;

				case "System.Net.Http.Exception":					
					break;

				case "System.Net.Http.Desktop.HttpRequestOut.Stop":
					{
						var request = (HttpWebRequest)_activityStop_RequestFetcher.Fetch(arg);

						if(spanContainer.TryRemove(request.Headers["uber-trace-id"], out var span))
						{
							var response = (HttpWebResponse)_activityStop_ResponseFetcher.Fetch(arg);

							if (response != null)
							{
								span.SetTag(Tags.HttpStatus, (int)response.StatusCode);
							}
							span.Finish();
						}
					}
					break;
			}
		}
	}
}
