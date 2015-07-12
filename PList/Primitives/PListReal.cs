using System;
using System.Globalization;
using System.IO;
using System.Linq;
using PListNet.Exceptions;

namespace PListNet.Primitives
{
	/// <summary>
	/// Represents a double Value from a PList
	/// </summary>
	public class PListReal : PNode<double>
	{
		/// <summary>
		/// Gets the Xml tag of this element.
		/// </summary>
		/// <value>The Xml tag of this element.</value>
		internal override string XmlTag { get { return "real"; } }

		/// <summary>
		/// Gets the binary typecode of this element.
		/// </summary>
		/// <value>The binary typecode of this element.</value>
		internal override byte BinaryTag { get { return 2; } }

		internal override int BinaryLength { get { return 3; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="PListReal"/> class.
		/// </summary>
		public PListReal()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PListReal"/> class.
		/// </summary>
		/// <param name="value">The value of this element.</param>
		public PListReal(double value)
		{
			Value = value;
		}

		/// <summary>
		/// Parses the specified value from a given string, read from Xml.
		/// </summary>
		/// <param name="data">The string whis is parsed.</param>
		internal override void Parse(string data)
		{
			Value = double.Parse(data, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Gets the XML string representation of the Value.
		/// </summary>
		/// <returns>
		/// The XML string representation of the Value.
		/// </returns>
		internal override string ToXmlString()
		{
			return Value.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Reads this element binary from the reader.
		/// </summary>
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			var buf = new byte[1 << nodeLength];
			if (stream.Read(buf, 0, buf.Length) != buf.Length)
			{
				throw new PListFormatException();
			}

			switch (nodeLength)
			{
				case 0:
					throw new PListFormatException("Real < 32Bit");
				case 1:
					throw new PListFormatException("Real < 32Bit");
				case 2:
					Value = BitConverter.ToSingle(buf.Reverse().ToArray(), 0);
					break;
				case 3:
					Value = BitConverter.ToDouble(buf.Reverse().ToArray(), 0);
					break;
				default:
					throw new PListFormatException("Real > 64Bit");
			}
		}

		/// <summary>
		/// Writes this element binary to the writer.
		/// </summary>
		internal override void WriteBinary(Stream stream)
		{
			var buf = BitConverter.GetBytes(Value).Reverse().ToArray();
			stream.Write(buf, 0, buf.Length);
		}
	}
}
