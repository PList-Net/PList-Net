using System.IO;
using System.Text;
using System.Xml;
using PListNet.Internal;

namespace PListNet
{
	/// <summary>
	/// Parses, saves, and creates a PList File
	/// </summary>
	public static class PList
	{
		/// <summary>
		/// Loads the PList from specified stream.
		/// </summary>
		/// <param name="stream">The stream containing the PList.</param>
		/// <returns>A <see cref="PNode"/> object loaded from the stream</returns>
		public static PNode Load(Stream stream)
		{
			var isBinary = IsFormatBinary(stream);

			// Detect binary format, and read using the appropriate method
			return (isBinary)
				? LoadAsBinary(stream)
				: LoadAsXml(stream);
		}

		private static bool IsFormatBinary(Stream stream)
		{
			var buf = new byte[8];

			// read in first 8 bytes
			stream.Read(buf, 0, buf.Length);

			// rewind
			stream.Seek(0, SeekOrigin.Begin);

			// compare to known indicator (TODO: validate version as well)
			return Encoding.UTF8.GetString(buf, 0, 6) == "bplist";
		}

		private static PNode LoadAsBinary(Stream stream)
		{
			var reader = new BinaryFormatReader();
			return reader.Read(stream);
		}

		private static PNode LoadAsXml(Stream stream)
		{
			// set resolver to null in order to avoid calls to apple.com to resolve DTD
			var settings = new XmlReaderSettings
				{
					DtdProcessing = DtdProcessing.Ignore,
				};

			using (var reader = XmlReader.Create(stream, settings))
			{
				reader.MoveToContent();
				reader.ReadStartElement("plist");

				reader.MoveToContent();
				var node = NodeFactory.Create(reader.LocalName);
				node.ReadXml(reader);

				reader.ReadEndElement();

				return node;
			}
		}

		/// <summary>
		/// Saves the PList to the specified stream.
		/// </summary>
		/// <param name="rootNode">Root node of the PList structure.</param>
		/// <param name="stream">The stream in which the PList is saves.</param>
		/// <param name="format">The format of the PList (Binary/Xml).</param>
		/// <param name="encoding">For Xml output, the character encoding to use; defaults to UTF8. ASCII is required for code-signing entitlements.</param>
		public static void Save(PNode rootNode, Stream stream, PListFormat format, Encoding encoding = null)
		{
			if (format == PListFormat.Xml)
			{
				const string newLine = "\n";

				var sets = new XmlWriterSettings
					{
						Encoding = encoding ?? Encoding.UTF8,
						Indent = true,
						IndentChars = "\t",
						NewLineChars = newLine,
					};

				using (var tmpStream = new MemoryStream())
				{
					using (var xmlWriter = XmlWriter.Create(tmpStream, sets))
					{
						xmlWriter.WriteStartDocument();
						xmlWriter.WriteDocType("plist", "-//Apple Computer//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);

						// write out nodes, wrapped in plist root element
						xmlWriter.WriteStartElement("plist");
						xmlWriter.WriteAttributeString("version", "1.0");
						rootNode.WriteXml(xmlWriter);
						xmlWriter.WriteEndElement();
						xmlWriter.Flush();
					}

					// XmlWriter always inserts a space before element closing (e.g. <true />)
					// whereas the Apple parser can't deal with the space and expects <true/>
					tmpStream.Seek(0, SeekOrigin.Begin);
					using (var reader = new StreamReader(tmpStream))
					using (var writer = new StreamWriter(stream, encoding ?? Encoding.UTF8, 4096, true))
					{
						writer.NewLine = newLine;
						for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
						{
							if (line.Trim() == "<true />") line = line.Replace("<true />", "<true/>");
							if (line.Trim() == "<false />") line = line.Replace("<false />", "<false/>");

							writer.WriteLine(line);
						}
					}
				}

			}
			else
			{
				var writer = new BinaryFormatWriter();
				writer.Write(stream, rootNode);
			}
		}
/*
		#region IXmlSerializable Members

		/// <summary>
		/// This method is reserved and should not be used. When implementing the IXmlSerializable interface, 
		/// you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a 
		/// custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> 
		/// to the class.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is
		/// produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> 
		/// method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/>
		/// method.
		/// </returns>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement("plist");

			Root = PListElementFactory.Instance.Create(reader.LocalName);
			Root.ReadXml(reader);

			reader.ReadEndElement();
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("plist");
			writer.WriteAttributeString("version", "1.0");
			if (Root != null) Root.WriteXml(writer);

			writer.WriteEndElement();
		}

		#endregion
*/
	}
}
