using System;
using System.IO;
using PListNet.Exceptions;

namespace PListNet.Primitives
{
	/// <summary>
	/// Represents a Byte[] Value from a PList
	/// </summary>
	public class PListData : PNode<byte[]>
	{
		/// <summary>
		/// Gets the Xml tag of this element.
		/// </summary>
		/// <value>The Xml tag of this element.</value>
		internal override string XmlTag { get { return "data"; } }

		/// <summary>
		/// Gets the binary typecode of this element.
		/// </summary>
		/// <value>The binary typecode of this element.</value>
		internal override byte BinaryTag { get { return 4; } }

		/// <summary>
		/// Gets the length of this PList element.
		/// </summary>
		internal override int BinaryLength { get { return Value.Length; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="PListData"/> class.
		/// </summary>
		public PListData()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PListData"/> class.
		/// </summary>
		/// <param name="value">The value of this element.</param>
		public PListData(byte[] value)
		{
			Value = value;
		}

		/// <summary>
		/// Parses the specified value from a given string (encoded as Base64), read from Xml.
		/// </summary>
		/// <param name="data">The string whis is parsed.</param>
		internal override void Parse(string data)
		{
			Value = Convert.FromBase64String(data);
		}

		/// <summary>
		/// Gets the XML string representation of the Value.
		/// </summary>
		/// <returns>
		/// The XML string representation of the Value (encoded as Base64).
		/// </returns>
		internal override string ToXmlString()
		{
			return Convert.ToBase64String(Value);
		}

		/// <summary>
		/// Reads this element binary from the reader.
		/// </summary>
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			Value = new byte[nodeLength];
			if (stream.Read(Value, 0, Value.Length) != Value.Length)
			{
				throw new PListFormatException();
			}
		}

		/// <summary>
		/// Writes this element binary to the writer.
		/// </summary>
		internal override void WriteBinary(Stream stream)
		{
			stream.Write(Value, 0, Value.Length);
		}
	}
}
