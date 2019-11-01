using System;
using System.IO;
using System.Xml;

namespace PListNet
{
	/// <summary>
	/// PList document node.
	/// </summary>
	public abstract class PNode
	{
		/// <summary>
		/// 	Gets the xml tag.
		/// </summary>
		/// <value>The xml tag.</value>
		internal abstract string XmlTag { get; }

		/// <summary>
		/// 	Gets the binary tag.
		/// </summary>
		/// <value>The binary tag.</value>
		internal abstract byte BinaryTag { get; }

		/// <summary>
		/// 	Gets the length of the binary representation.
		/// </summary>
		/// <value>The length of the binary.</value>
		internal abstract int BinaryLength { get; }

		internal abstract bool IsBinaryUnique { get; }

		internal abstract void ReadXml(XmlReader reader);

		internal abstract void WriteXml(XmlWriter writer);

		internal abstract void ReadBinary(Stream stream, int nodeLength);

		internal abstract void WriteBinary(Stream stream);
	}

	/// <summary>
	/// PList node.
	/// </summary>
	public abstract class PNode<T> : PNode, IEquatable<PNode>
	{
		/// <summary>
		/// 	Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public virtual T Value { get; set; }

		internal override bool IsBinaryUnique => true;

	    /// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
		internal override void ReadXml(XmlReader reader)
		{
            var isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (!isEmptyElement)
            {
                Parse(reader.ReadContentAsString());
                reader.ReadEndElement();
            }
            else
            {
                Parse(String.Empty);
            }
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
		internal override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(XmlTag);
			writer.WriteValue(ToXmlString());
			writer.WriteEndElement();
		}

		internal abstract void Parse(string data);

		internal abstract string ToXmlString();

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		public bool Equals(PNode other)
		{
			return (other is PNode<T>) && (Value.Equals(((PNode<T>) other).Value));
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="T:PListNet.PNode`1"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="T:PListNet.PNode`1"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="T:PListNet.PNode`1"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			var node = obj as PNode;
			return node != null && Equals(node);
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="T:PListNet.PNode`1"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="T:PListNet.PNode`1"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="T:PListNet.PNode`1"/>.</returns>
		public override string ToString()
		{
			return $"{XmlTag}: {Value}";
		}
	}
}
