using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using PListNet.Internal;

namespace PListNet.Collections
{
	/// <summary>
	/// Represents an array of an <see cref="T:PListNet.PNode"/> objects
	/// </summary>
	public class PListArray : PNode, IList<PNode>
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
		public int IndexOf(PNode item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, PNode item)
		{
			_list.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_list.RemoveAt(index);
		}

		public PNode this[int index]
		{
			get { return _list[index]; }
			set { _list[index] = value; }
		}
		#endregion

		#region ICollection implementation
		public void Add(PNode item)
		{
			_list.Add(item);
		}

		public void Clear()
		{
			_list.Clear();
		}

		public bool Contains(PNode item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(PNode[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public bool Remove(PNode item)
		{
			return _list.Remove(item);
		}

		public int Count
		{
			get { return _list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}
		#endregion

		#region IEnumerable implementation
		public IEnumerator<PNode> GetEnumerator()
		{
			return _list.GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}
		#endregion
	}
}
