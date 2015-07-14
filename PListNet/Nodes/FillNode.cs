using System.IO;

namespace PListNet.Nodes
{
	/// <summary>
	/// Represents a fill element in a PList
	/// </summary>
	/// <remarks>Is skipped in Xml-Serialization</remarks>
	public class FillNode : PNode
	{
		/// <summary>
		/// Gets the Xml tag of this element.
		/// </summary>
		/// <value>The Xml tag of this element.</value>
		internal override string XmlTag { get { return "fill"; } }

		/// <summary>
		/// Gets the binary typecode of this element.
		/// </summary>
		/// <value>The binary typecode of this element.</value>
		internal override byte BinaryTag { get { return 0; } }

		/// <summary>
		/// Gets the length of this PList node.
		/// </summary>
		internal override int BinaryLength { get { return 0x0F; } }

		/// <summary>
		/// Gets a value indicating whether this instance is written only once in binary mode.
		/// </summary>
		/// <value>
		/// 	<c>true</c> this instance is written only once in binary mode; otherwise, <c>false</c>.
		/// </value>
		internal override bool IsBinaryUnique { get { return false; } }

		/// <summary>
		/// Reads this element binary from the reader.
		/// </summary>
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			if (nodeLength != 0x0F) throw new PListFormatException();
		}

		/// <summary>
		/// Writes this node binary to the writer.
		/// </summary>
		internal override void WriteBinary(Stream stream)
		{
		}

		#region IXmlSerializable Members

		/// <summary>
		/// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
		/// </returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
		internal override void ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement(XmlTag);
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
		internal override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement(XmlTag);
			writer.WriteEndElement();
		}

		#endregion

	}
}