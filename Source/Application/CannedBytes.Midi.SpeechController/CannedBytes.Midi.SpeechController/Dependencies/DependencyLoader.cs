using System;
using System.Reflection;

namespace CannedBytes.Midi.SpeechController.Dependencies
{
    internal static class DependencyLoader
    {
        public static void Register()
        {
            //#if DEBUG
            //            // use normal binding in debug
            //#else
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            //#endif
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var resourceName = "CannedBytes.Midi.SpeechController.Dependencies." + new AssemblyName(args.Name).Name + ".dll";

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                Byte[] assemblyData = new Byte[stream.Length];

                stream.Read(assemblyData, 0, assemblyData.Length);

                return Assembly.Load(assemblyData);
            }
        }
    }
}