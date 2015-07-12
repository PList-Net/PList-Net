using System.IO;
using System.Xml;
using PListNet.Collections;

namespace PListNet.Internal
{
	public static class XmlFormatReader
	{
		public static PNode Read(Stream stream)
		{
			var settings = new XmlReaderSettings();
			using (var reader = XmlReader.Create(stream, settings))
			{
				reader.ReadStartElement("plist");

				var node = NodeFactory.Create(reader.LocalName);
				node.ReadXml(reader);

				reader.ReadEndElement();

				return node;
			}
		}
	}
}
