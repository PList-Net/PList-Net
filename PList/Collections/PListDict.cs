using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using PListNet.Internal;

namespace PListNet.Collections
{
	/// <summary>
	/// Represents an dictionary with <see cref="T:System.String"/> keys and <see cref="T:PListNet.PNode"/> values
	/// </summary>
	public class PListDict : PNode, IDictionary<string, PNode>
	{
		private readonly IDictionary<string, PNode> _dictionary = new Dictionary<string, PNode>();

		/// <summary>
		/// Gets the Xml tag of this element.
		/// </summary>
		/// <value>The Xml tag of this element.</value>
		internal override string XmlTag { get { return "dict"; } }

		/// <summary>
		/// Gets the binary typecode of this element.
		/// </summary>
		/// <value>The binary typecode of this element.</value>
		internal override byte BinaryTag { get { return 0x0D; } }

		/// <summary>
		/// Gets the length of this PList node.
		/// </summary>
		/// <returns>The length of this PList node.</returns>
		internal override int BinaryLength { get { return Count; } }

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
			throw new NotImplementedException("This type of node does not do it's own reading, refer to the binary reader.");
		}

		/// <summary>
		/// Writes this element binary to the writer.
		/// </summary>
		internal override void WriteBinary(Stream stream)
		{
			throw new NotImplementedException("This type of node does not do it's own writing, refer to the binary writer.");
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
		internal override void ReadXml(XmlReader reader)
		{

			bool wasEmpty = reader.IsEmptyElement;
			reader.Read();

			if (wasEmpty) return;

			while (reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement("key");
				string key = reader.ReadString();
				reader.ReadEndElement();

				var node = NodeFactory.Create(reader.LocalName);
				node.ReadXml(reader);
				Add(key, node);

				reader.MoveToContent();
			}

			reader.ReadEndElement();
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
		internal override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(XmlTag);

			foreach (var key in Keys)
			{
				writer.WriteStartElement("key");
				writer.WriteValue(key);
				writer.WriteEndElement();

				this[key].WriteXml(writer);
			}

			writer.WriteEndElement();
		}

		#region IDictionary implementation

		public bool ContainsKey(string key)
		{
			return _dictionary.ContainsKey(key);
		}

		public void Add(string key, PNode value)
		{
			_dictionary.Add(key, value);
		}

		public bool Remove(string key)
		{
			return _dictionary.Remove(key);
		}

		public bool TryGetValue(string key, out PNode value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		public PNode this[string index]
		{
			get { return _dictionary[index]; }
			set { _dictionary[index] = value; }
		}

		public ICollection<string> Keys
		{
			get { return _dictionary.Keys; }
		}

		public ICollection<PNode> Values
		{
			get { return _dictionary.Values; }
		}

		#endregion

		#region ICollection implementation

		public void Add(KeyValuePair<string, PNode> item)
		{
			_dictionary.Add(item);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public bool Contains(KeyValuePair<string, PNode> item)
		{
			return _dictionary.Contains(item);
		}

		public void CopyTo(KeyValuePair<string, PNode>[] array, int arrayIndex)
		{
			_dictionary.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<string, PNode> item)
		{
			return _dictionary.Remove(item);
		}

		public int Count
		{
			get { return _dictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<KeyValuePair<string, PNode>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		#endregion
	}
}
