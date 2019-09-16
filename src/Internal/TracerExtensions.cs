﻿using OpenTracing.Noop;
using OpenTracing.Util;

namespace OpenTracing.Contrib.NetStandard.Internal
{
    internal static class TracerExtensions
    {
        public static bool IsNoopTracer(this ITracer tracer)
        {
            // TODO Change if https://github.com/opentracing/opentracing-csharp/pull/77 gets released.
            if (tracer == NoopTracerFactory.Create())
                return true;

            // There's no way to check the underlying tracer on the instance so we have to check the static method.
            if (tracer is GlobalTracer && !GlobalTracer.IsRegistered())
                return true;

            return false;
        }
    }
}
