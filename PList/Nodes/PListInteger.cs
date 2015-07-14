using System;
using System.Globalization;
using System.IO;
using System.Net;
using PListNet.Exceptions;

namespace PListNet.Primitives
{
	/// <summary>
	/// Represents an integer Value from a PList
	/// </summary>
	public class PListInteger : PNode<Int64>
	{
		/// <summary>
		/// Gets the Xml tag of this element.
		/// </summary>
		/// <value>The Xml tag of this element.</value>
		internal override string XmlTag { get { return "integer"; } }

		/// <summary>
		/// Gets the binary typecode of this element.
		/// </summary>
		/// <value>The binary typecode of this element.</value>
		internal override byte BinaryTag { get { return 1; } }

		/// <summary>
		/// Gets the length of this PList element.
		/// </summary>
		/// <returns>The length of this PList element.</returns>
		/// <remarks>Provided for internal use only.</remarks>
		internal override int BinaryLength
		{
			get
			{
				if (Value >= Byte.MinValue && Value <= Byte.MaxValue) return 0;
				if (Value >= Int16.MinValue && Value <= Int16.MaxValue) return 1;
				if (Value >= Int32.MinValue && Value <= Int32.MaxValue) return 2;
				if (Value >= Int64.MinValue && Value <= Int64.MaxValue) return 3;
				return -1;
			}
		}

		/// <summary>
		/// Gets or sets the value of this element.
		/// </summary>
		/// <value>The value of this element.</value>
		public override Int64 Value { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PListInteger"/> class.
		/// </summary>
		public PListInteger()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PListInteger"/> class.
		/// </summary>
		/// <param name="value">The value of this element.</param>
		public PListInteger(Int64 value)
		{
			Value = value;
		}

		/// <summary>
		/// Parses the specified value from a given string, read from Xml.
		/// </summary>
		/// <param name="data">The string whis is parsed.</param>
		internal override void Parse(string data)
		{
			Value = Int64.Parse(data, CultureInfo.InvariantCulture);
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
		/// Reads the binary stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
		/// <param name="nodeLength">Node length.</param>
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
					Value = buf[0];
					break;
				case 1:
					Value = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buf, 0));
					break;
				case 2:
					Value = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buf, 0));
					break;
				case 3:
					Value = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buf, 0));
					break;
				default:
					throw new PListFormatException("Int > 64Bit");
			}
		}

		/// <summary>
		/// Writes this element binary to the writer.
		/// </summary>
		internal override void WriteBinary(Stream stream)
		{
			byte[] buf = null;
			switch (BinaryLength)
			{
				case 0:
					buf = new [] { (byte) Value };
					break;
				case 1:
					buf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16) Value));
					break;
				case 2:
					buf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int32) Value));
					break;
				case 3:
					buf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Value));
					break;
			}
			stream.Write(buf, 0, buf.Length);
		}
	}
}
