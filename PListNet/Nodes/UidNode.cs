using System;
using System.IO;
using BitConverter;

namespace PListNet.Nodes
{
    /// <summary>
    /// Represents a UID value from a PList
    /// </summary>
    public class UidNode : PNode<ulong>
    {
        internal override string XmlTag => "uid";

        internal override byte BinaryTag => 8;

		internal override int BinaryLength
		{
			get
			{
				if (Value >= byte.MinValue && Value <= byte.MaxValue) return 0;
				if (Value >= ushort.MinValue && Value <= ushort.MaxValue) return 1;
				if (Value >= uint.MinValue && Value <= uint.MaxValue) return 2;
				if (Value >= ulong.MinValue && Value <= ulong.MaxValue) return 3;
				return -1;
			}
		}

		/// <summary>
		/// Gets or sets the value of this element.
		/// </summary>
		/// <value>The value of this element.</value>
		public override ulong Value { get; set; }

		/// <summary>
		/// Create a new UID node.
		/// </summary>
		public UidNode() { }

		/// <summary>
		///	Create a new UID node.
		/// </summary>
		/// <param name="value"></param>
		public UidNode(ulong value)
		{
			Value = value;
		}

		internal override void Parse(string data)
        {
            throw new NotImplementedException();
        }

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
					Value = EndianBitConverter.BigEndian.ToUInt16(buf, 0);
					break;
				case 2:
					Value = EndianBitConverter.BigEndian.ToUInt32(buf, 0);
					break;
				case 3:
					Value = EndianBitConverter.BigEndian.ToUInt64(buf, 0);
					break;
				default:
					throw new PListFormatException("Int > 64Bit");
			}
		}

		internal override string ToXmlString()
        {
            return $"<dict><key>CF$UID</key><integer>{Value}</integer></dict>";
        }

		internal override void WriteBinary(Stream stream)
		{
			byte[] buf;
			switch (BinaryLength)
			{
				case 0:
					buf = new[] { (byte) Value };
					break;
				case 1:
					buf = EndianBitConverter.BigEndian.GetBytes((ushort) Value);
					break;
				case 2:
					buf = EndianBitConverter.BigEndian.GetBytes((uint) Value);
					break;
				case 3:
					buf = EndianBitConverter.BigEndian.GetBytes(Value);
					break;

				default:
					throw new Exception($"Unexpected length: {BinaryLength}.");
			}

			stream.Write(buf, 0, buf.Length);
		}
	}
}
