using System;
using System.IO;
using System.Xml;

namespace PListNet.Nodes
{
	/// <summary>
	/// Represents a Boolean Value from a PList
	/// </summary>
	public class BooleanNode : PNode<bool>
	{
		/// <summary>
		/// Gets the Xml tag of this element.
		/// </summary>
		/// <value>The Xml tag of this element.</value>
		internal override string XmlTag { get { return "boolean"; } }

		/// <summary>
		/// Gets the binary typecode of this element.
		/// </summary>
		/// <value>The binary typecode of this element.</value>
		internal override byte BinaryTag { get { return 0; } }

		internal override int BinaryLength { get { return Value ? 9 : 8; } }

		/// <summary>
		/// Gets a value indicating whether this instance is written only once in binary mode.
		/// </summary>
		/// <value>
		/// 	<c>true</c> this instance is written only once in binary mode; otherwise, <c>false</c>.
		/// </value>
		internal override bool IsBinaryUnique { get { return true; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="BooleanNode"/> class.
		/// </summary>
		public BooleanNode()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BooleanNode"/> class.
		/// </summary>
		/// <param name="value">The Value of this element</param>
		public BooleanNode(Boolean value)
		{
			Value = value;
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
		internal override void ReadXml(XmlReader reader)
		{
			Parse(reader.LocalName);
			reader.ReadStartElement();
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
		internal override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(ToXmlString());
			writer.WriteEndElement();
		}

		/// <summary>
		/// Parses the specified value from a given string, read from Xml.
		/// </summary>
		/// <param name="data">The string whis is parsed.</param>
		internal override void Parse(string data)
		{
			Value = data == "true";
		}

		/// <summary>
		/// Gets the XML string representation of the Value.
		/// </summary>
		/// <returns>
		/// The XML string representation of the Value.
		/// </returns>
		internal override string ToXmlString()
		{
			return Value ? "true" : "false";
		}

		/// <summary>
		/// Reads this element binary from the reader.
		/// </summary>
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			if (nodeLength != 8 && nodeLength != 9)
			{
				throw new PListFormatException();
			}

			Value = nodeLength == 9;
		}

		/// <summary>
		/// Writes this element binary to the writer.
		/// </summary>
		internal override void WriteBinary(Stream stream)
		{
		}
	}
}
