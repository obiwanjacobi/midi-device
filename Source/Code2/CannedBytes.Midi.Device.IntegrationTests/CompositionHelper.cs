using CannedBytes.ComponentModel.Composition;
using System;

namespace CannedBytes.Midi.Device.IntegrationTests
{
    internal static class CompositionHelper
    {
        public static CompositionContext CreateCompositionContext()
        {
            var builder = new CompositionContextBuilder();

            builder.AddDirectory(Environment.CurrentDirectory);
            builder.AddAssembly(typeof(CompositionHelper).Assembly);

            return builder.ToCompositionContext();
        }
    }
}
