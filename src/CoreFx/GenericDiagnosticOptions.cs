﻿using System.Collections.Generic;

namespace OpenTracing.Contrib.NetStandard.CoreFx
{
    public sealed class GenericDiagnosticOptions
    {
        public HashSet<string> IgnoredListenerNames { get; } = new HashSet<string>();

        public Dictionary<string, HashSet<string>> IgnoredEvents { get; } = new Dictionary<string, HashSet<string>>();

        public void IgnoreEvent(string diagnosticListenerName, string eventName)
        {
            if (diagnosticListenerName == null || eventName == null)
                return;

            HashSet<string> ignoredListenerEvents;

            if (!IgnoredEvents.TryGetValue(diagnosticListenerName, out ignoredListenerEvents))
            {
                ignoredListenerEvents = new HashSet<string>();
                IgnoredEvents.Add(diagnosticListenerName, ignoredListenerEvents);
            }

            ignoredListenerEvents.Add(eventName);
        }
    }
}
