using System.IO;
using System.Xml.Serialization;

namespace CannedBytes.Tools.DgmlBuilder
{
    public class DgmlSerializer
    {
        public const string Namespace = "http://schemas.microsoft.com/vs/2009/dgml";

        private static readonly XmlSerializer _serializer = new(typeof(DirectedGraph), Namespace);

        public void Serialize(Stream targetStream, DirectedGraph graph)
        {
            _serializer.Serialize(targetStream, graph);
        }
    }
}
