using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using PListNet.Internal;

namespace PListNet.Nodes
{
	/// <summary>
	/// Represents an dictionary with <see cref="T:System.String"/> keys and <see cref="T:PListNet.PNode"/> values
	/// </summary>
	public class DictionaryNode : PNode, IDictionary<string, PNode>
	{
		private readonly IDictionary<string, PNode> _dictionary = new Dictionary<string, PNode>();

		/// <summary>
		/// Gets the Xml tag of this element.
		/// </summary>
		/// <value>The Xml tag of this element.</value>
		internal override string XmlTag => "dict";

	    /// <summary>
		/// Gets the binary typecode of this element.
		/// </summary>
		/// <value>The binary typecode of this element.</value>
		internal override byte BinaryTag => 0x0D;

	    /// <summary>
		/// Gets the length of this PList node.
		/// </summary>
		/// <returns>The length of this PList node.</returns>
		internal override int BinaryLength => Count;

	    /// <summary>
		/// Gets a value indicating whether this instance is written only once in binary mode.
		/// </summary>
		/// <value>
		/// 	<c>true</c> this instance is written only once in binary mode; otherwise, <c>false</c>.
		/// </value>
		internal override bool IsBinaryUnique => false;

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

			reader.MoveToContent();

			while (reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement("key");
				string key = reader.ReadContentAsString();
				reader.ReadEndElement();

				reader.MoveToContent();
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

		/// <summary>
		/// Determines whether the current instance contains an entry with the specified key.
		/// </summary>
		/// <returns><c>true</c>, if key was containsed, <c>false</c> otherwise.</returns>
		/// <param name="key">Key.</param>
		public bool ContainsKey(string key)
		{
			return _dictionary.ContainsKey(key);
		}

		/// <summary>
		/// Add the specified value with specified key.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public void Add(string key, PNode value)
		{
			_dictionary.Add(key, value);
		}

		/// <summary>
		/// Remove value at the specified key.
		/// </summary>
		/// <param name="key">Key.</param>
		public bool Remove(string key)
		{
			return _dictionary.Remove(key);
		}

		/// <summary>
		/// Attempts to retrieve value at the specified key.
		/// </summary>
		/// <returns><c>true</c>, if key was found, <c>false</c> otherwise.</returns>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public bool TryGetValue(string key, out PNode value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		/// <summary>
		/// Gets or sets the <see cref="PListNet.PNode"/> at the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public PNode this[string index]
		{
			get => _dictionary[index];
		    set => _dictionary[index] = value;
		}

		/// <summary>
		/// Gets the keys.
		/// </summary>
		/// <value>The keys.</value>
		public ICollection<string> Keys => _dictionary.Keys;

	    /// <summary>
		/// Gets the values.
		/// </summary>
		/// <value>The values.</value>
		public ICollection<PNode> Values => _dictionary.Values;

	    #endregion

		#region ICollection implementation

		/// <summary>
		/// Add the specified key/value pair.
		/// </summary>
		/// <param name="item">Item.</param>
		public void Add(KeyValuePair<string, PNode> item)
		{
			_dictionary.Add(item);
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			_dictionary.Clear();
		}

		/// <summary>
		/// Contains the specified key/value pair.
		/// </summary>
		/// <param name="item">Item.</param>
		public bool Contains(KeyValuePair<string, PNode> item)
		{
			return _dictionary.Contains(item);
		}

		/// <summary>
		/// Copies values.
		/// </summary>
		/// <param name="array">Array.</param>
		/// <param name="arrayIndex">Array index.</param>
		public void CopyTo(KeyValuePair<string, PNode>[] array, int arrayIndex)
		{
			_dictionary.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Remove the specified key/value pair.
		/// </summary>
		/// <param name="item">Item.</param>
		public bool Remove(KeyValuePair<string, PNode> item)
		{
			return _dictionary.Remove(item);
		}

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public int Count => _dictionary.Count;

	    /// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
		public bool IsReadOnly => false;

	    #endregion

		#region IEnumerable implementation

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<KeyValuePair<string, PNode>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		#endregion
	}
}
