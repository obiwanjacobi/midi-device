using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace CannedBytes.Midi.Device.Schema.Xml;

internal class XmlResourceResolver : XmlUrlResolver
{
    public override object GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
    {
        object? result = null;

        try
        {
            result = base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }
        catch (DirectoryNotFoundException)
        { }
        catch (FileNotFoundException)
        { }

        if (result is null)
        {
            var fullPath = absoluteUri.LocalPath;

            var assemblyName = Path.GetDirectoryName(fullPath);
            var assembly = Assembly.LoadFrom(assemblyName + ".dll");

            if (assembly is not null)
            {
                result = assembly.GetManifestResourceStream(
                        assembly.GetName().Name + "." + Path.GetFileName(fullPath));
            }
        }

        return result!;
    }

    public override Uri ResolveUri(Uri? baseUri, string? relativeUri)
    {
        if (baseUri?.IsAbsoluteUri == true)
        {
            string? fullPath = baseUri.LocalPath;

            if (!Directory.Exists(fullPath))
            {
                fullPath = Path.GetDirectoryName(fullPath);
                baseUri = new Uri(fullPath!);
            }
        }

        var resultUri = base.ResolveUri(baseUri, relativeUri);

        return resultUri;
    }
}