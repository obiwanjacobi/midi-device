using System.Diagnostics;

namespace CannedBytes.Midi.Device.Schema
{
    internal static class Tracer
    {
        private const string AssemblyName = "CannedBytes.Midi.Device.Schema";
        private static readonly TraceSource _traceSource = new TraceSource(AssemblyName);

        [Conditional("TRACE")]
        public static void TraceEvent(TraceEventType eventType, string message, params object[] args)
        {
            _traceSource.TraceEvent(eventType, AssemblyName.GetHashCode(), message, args);
        }
    }
}
