using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using OpenTracing.Propagation;

namespace OpenTracing.Contrib.NetStandard.CoreFx
{
    internal sealed class HttpHeadersInjectAdapter : ITextMap
    {
        private readonly HttpHeaders _headers;
		private readonly WebHeaderCollection _headerCollection;

		public HttpHeadersInjectAdapter(HttpHeaders headers)
        {
            _headers = headers ?? throw new ArgumentNullException(nameof(headers));
        }

		public HttpHeadersInjectAdapter(WebHeaderCollection headerCollection)
		{
			_headerCollection = headerCollection ?? throw new ArgumentNullException(nameof(headerCollection));
		}

		public void Set(string key, string value)
        {
			if(_headers != null)
			{
				if (_headers.Contains(key))
				{
					_headers.Remove(key);
				}

				_headers.Add(key, value);
			}
            if(_headerCollection != null)
			{
				if (_headerCollection.AllKeys.Contains(key))
				{
					_headerCollection.Remove(key);
				}
				_headerCollection.Add(key, value);
			}
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new NotSupportedException("This class should only be used with ITracer.Inject");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
