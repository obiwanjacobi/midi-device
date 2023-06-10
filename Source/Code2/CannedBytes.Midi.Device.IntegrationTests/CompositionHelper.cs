using System;
using CannedBytes.ComponentModel.Composition;

namespace CannedBytes.Midi.Device.IntegrationTests;

internal static class CompositionHelper
{
    public static CompositionContext CreateCompositionContext()
    {
        CompositionContextBuilder builder = new();

        builder.AddDirectory(Environment.CurrentDirectory);
        builder.AddAssembly(typeof(CompositionHelper).Assembly);

        return builder.ToCompositionContext();
    }
}
