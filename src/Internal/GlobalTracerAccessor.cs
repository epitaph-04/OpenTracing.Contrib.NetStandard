using OpenTracing.Util;

namespace OpenTracing.Contrib.NetStandard.Internal
{
    public class GlobalTracerAccessor : IGlobalTracerAccessor
    {
        public ITracer GetGlobalTracer()
        {
            return GlobalTracer.Instance;
        }
    }
}
