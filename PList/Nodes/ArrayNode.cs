using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using PListNet.Internal;

namespace PListNet.Nodes
{
	/// <summary>
	/// Represents an array of an <see cref="T:PListNet.PNode"/> objects
	/// </summary>
	public class ArrayNode : PNode, IList<PNode>
	{
		private readonly IList<PNode> _list = new List<PNode>();

		/// <summary>
		/// Gets the Xml tag of this element.
		/// </summary>
		/// <value>The Xml tag of this element.</value>
		internal override string XmlTag { get { return "array"; } }

		/// <summary>
		/// Gets the binary typecode of this element.
		/// </summary>
		/// <value>The binary typecode of this element.</value>
		internal override byte BinaryTag { get { return 0x0A; } }

		internal override int BinaryLength { get { return _list.Count; } }

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
				var plelem = NodeFactory.Create(reader.LocalName);
				plelem.ReadXml(reader);
				Add(plelem);
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
			for (int i = 0; i < Count; i++)
			{
				this[i].WriteXml(writer);
			}
			writer.WriteEndElement();
		}

		#region IList implementation

		/// <summary>
		/// Determines the index of a specific item in the current instance.
		/// </summary>
		/// <returns>The index.</returns>
		/// <param name="item">Item.</param>
		public int IndexOf(PNode item)
		{
			return _list.IndexOf(item);
		}

		/// <summary>
		/// Insert the specified item at index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		public void Insert(int index, PNode item)
		{
			_list.Insert(index, item);
		}

		/// <summary>
		/// Removes the item at index.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemoveAt(int index)
		{
			_list.RemoveAt(index);
		}

		/// <summary>
		/// Gets or sets the <see cref="PListNet.PNode"/> at the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public PNode this[int index]
		{
			get { return _list[index]; }
			set { _list[index] = value; }
		}
		#endregion

		#region ICollection implementation
		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public void Add(PNode item)
		{
			_list.Add(item);
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			_list.Clear();
		}

		/// <summary>
		/// Determines whether the current collection contains a specific value.
		/// </summary>
		/// <param name="item">Item.</param>
		public bool Contains(PNode item)
		{
			return _list.Contains(item);
		}

		/// <summary>
		/// Copies array.
		/// </summary>
		/// <param name="array">Array.</param>
		/// <param name="arrayIndex">Array index.</param>
		public void CopyTo(PNode[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public bool Remove(PNode item)
		{
			return _list.Remove(item);
		}

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get { return _list.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
		public bool IsReadOnly
		{
			get { return false; }
		}
		#endregion

		#region IEnumerable implementation
		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<PNode> GetEnumerator()
		{
			return _list.GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation
		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}
		#endregion
	}
}
