using System.IO;
using System.Xml;

namespace PListNet.Internal
{
	/// <summary>
	/// Reader for XML format PList documents.
	/// </summary>
	public static class XmlFormatReader
	{
		/// <summary>
		/// Read document from the specified stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
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
