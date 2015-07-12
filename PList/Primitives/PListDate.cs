using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using PListNet.Exceptions;

namespace PListNet.Primitives
{
	/// <summary>
	/// Represents a DateTime Value from a PList
	/// </summary>
	public class PListDate : PNode<DateTime>
	{
		/// <summary>
		/// Gets the Xml tag of this element.
		/// </summary>
		/// <value>The Xml tag of this element.</value>
		internal override string XmlTag { get { return "date"; } }

		/// <summary>
		/// Gets the binary typecode of this element.
		/// </summary>
		/// <value>The binary typecode of this element.</value>
		internal override byte BinaryTag { get { return 3; } }

		internal override int BinaryLength { get { return 3; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="PListDate"/> class.
		/// </summary>
		public PListDate()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PListDate"/> class.
		/// </summary>
		/// <param name="value">The value of this element.</param>
		public PListDate(DateTime value)
		{
			Value = value;
		}

		/// <summary>
		/// Parses the specified value from a given string, read from Xml.
		/// </summary>
		/// <param name="data">The string whis is parsed.</param>
		internal override void Parse(string data)
		{
			Value = DateTime.Parse(data, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Gets the XML string representation of the Value.
		/// </summary>
		/// <returns>
		/// The XML string representation of the Value.
		/// </returns>
		internal override string ToXmlString()
		{
			return Value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.ffffffZ");
		}

		/// <summary>
		/// Reads this element binary from the reader.
		/// </summary>
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			Debug.WriteLine("Unverified", "WARNING");

			var buf = new byte[1 << nodeLength];
			if (stream.Read(buf, 0, buf.Length) != buf.Length)
				throw new PListFormatException();

			double ticks;
			switch (nodeLength)
			{
				case 0:
					throw new PListFormatException("Date < 32Bit");
				case 1:
					throw new PListFormatException("Date < 32Bit");
				case 2:
					ticks = BitConverter.ToSingle(buf.Reverse().ToArray(), 0);
					break;
				case 3:
					ticks = BitConverter.ToDouble(buf.Reverse().ToArray(), 0);
					break;
				default:
					throw new PListFormatException("Date > 64Bit");
			}

			Value = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(ticks);
		}

		/// <summary>
		/// Writes this element binary to the writer.
		/// </summary>
		internal override void WriteBinary(Stream stream)
		{
			Debug.WriteLine("Unverified", "WARNING");

			var start = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			TimeSpan ts = Value - start;
			var buf = BitConverter.GetBytes(ts.TotalSeconds).Reverse().ToArray();
			stream.Write(buf, 0, buf.Length);
		}
	}
}
