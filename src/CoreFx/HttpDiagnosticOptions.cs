using OpenTracing;
using System;
using System.Net;

namespace OpenTracing.Contrib.NetStandard.CoreFx
{
	public class HttpDiagnosticOptions
	{
		public const string PropertyIgnore = "ot-ignore";

		public const string DefaultComponent = "HttpOut";

		private string _componentName;
		private Func<HttpWebRequest, string> _operationNameResolver;

		/// <summary>
		/// Allows changing the "component" tag of created spans.
		/// </summary>
		public string ComponentName
		{
			get => _componentName;
			set => _componentName = value ?? throw new ArgumentNullException(nameof(ComponentName));
		}

		/// <summary>
		/// A delegates that defines on what requests tracing headers are propagated.
		/// </summary>
		public Func<HttpWebRequest, bool> InjectEnabled { get; set; }

		/// <summary>
		/// A delegate that returns the OpenTracing "operation name" for the given request.
		/// </summary>
		public Func<HttpWebRequest, string> OperationNameResolver
		{
			get => _operationNameResolver;
			set => _operationNameResolver = value ?? throw new ArgumentNullException(nameof(OperationNameResolver));
		}

		/// <summary>
		/// Allows the modification of the created span to e.g. add further tags.
		/// </summary>
		public Action<ISpan, HttpWebRequest> OnRequest { get; set; }

		/// <summary>
		/// Allows the modification of the created span when error occured to e.g. add further tags.
		/// </summary>
		public Action<ISpan, Exception, HttpWebRequest> OnError { get; set; }

		public HttpDiagnosticOptions()
		{
			// Default settings
			ComponentName = DefaultComponent;
			OperationNameResolver = (request) =>
			{
				return "HTTP " + request.Method;
			};
		}
	}
}
