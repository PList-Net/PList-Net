using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PListNet.Exceptions;
using PListNet.Primitives;
using PListNet.Collections;

namespace PListNet.Internal
{
	/// <summary>
	/// A class, used to write a <see cref="T:PListNet.PNode"/>  binary formated to a stream
	/// </summary>
	public class PListBinaryWriter
	{
		/// <summary>
		/// The Header (bplist00)
		/// </summary>
		private static readonly byte[] _header = {
			0x62, 0x70, 0x6c, 0x69, 0x73, 0x74, 0x30, 0x30
		};

		private readonly Dictionary<byte, Dictionary<PNode, int>> _uniqueElements = new Dictionary<byte, Dictionary<PNode, int>>();

		/// <summary>
		/// Gets the offset table.
		/// </summary>
		/// <value>The offset table.</value>
		internal List<Int32> Offsets { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PListBinaryWriter"/> class.
		/// </summary>
		internal PListBinaryWriter()
		{
		}

		/// <summary>
		/// Writers a <see cref="T:PListNet.PNode"/> to the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="node">The PList node.</param>
		public void Write(Stream stream, PNode node)
		{
			stream.Write(_header, 0, _header.Length);

			Offsets = new List<int>();

			int nodeCount = GetNodeCount(node);

			byte nodeIndexSize;
			if (nodeCount <= byte.MaxValue) nodeIndexSize = sizeof(byte);
			else if (nodeCount <= Int16.MaxValue) nodeIndexSize = sizeof(Int16);
			else nodeIndexSize = sizeof(Int32);

			int topOffestIdx = WriteInternal(stream, node);
			nodeCount = Offsets.Count;
            
			int offsetTableOffset = (int) stream.Position;


			byte offsetSize;
			if (offsetTableOffset <= Byte.MaxValue) offsetSize = sizeof(Byte);
			else if (offsetTableOffset <= Int16.MaxValue) offsetSize = sizeof(Int16);
			else offsetSize = sizeof(Int32);

			for (int i = 0; i < Offsets.Count; i++)
			{
				byte[] buf = null;
				switch (offsetSize)
				{
					case 1:
						buf = new [] { (byte) Offsets[i] };
						break;
					case 2:
						buf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16) Offsets[i]));
						break;
					case 4:
						buf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Offsets[i]));
						break;
				}
				stream.Write(buf, 0, buf.Length);
			}

			var header = new byte[32];
			header[6] = offsetSize;
			header[7] = nodeIndexSize;

			BitConverter.GetBytes(IPAddress.HostToNetworkOrder(nodeCount)).CopyTo(header, 12);
			BitConverter.GetBytes(IPAddress.HostToNetworkOrder(topOffestIdx)).CopyTo(header, 20);
			BitConverter.GetBytes(IPAddress.HostToNetworkOrder(offsetTableOffset)).CopyTo(header, 28);

			stream.Write(header, 0, header.Length);
		}

		/// <summary>
		/// Formats an element idx based on the ElementIdxSize.
		/// </summary>
		/// <param name="idx">The idx.</param>
		/// <param name="nodeIndexSize">The node index size.</param>
		/// <returns>The formated idx.</returns>
		internal Byte[] FormatIdx(int idx, int nodeIndexSize)
		{
			byte[] res;
			switch (nodeIndexSize)
			{
				case 1:
					res = new [] { (byte) idx };
					break;
				case 2:
					res = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16) idx));
					break;
				case 4:
					res = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(idx));
					break;
				default:
					throw new PListFormatException("Invalid node index size.");
			}
			return res;
		}

		/// <summary>
		/// Writers a <see cref="T:PListNet.PNode"/> to the current stream position
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="node">The PList node.</param>
		/// <returns>The Idx of the written node</returns>
		internal int WriteInternal(Stream stream, PNode node)
		{
			int elementIdx = Offsets.Count;
			if (node.IsBinaryUnique && node is IEquatable<PNode>)
			{
				if (!_uniqueElements.ContainsKey(node.BinaryTag)) _uniqueElements.Add(node.BinaryTag, new Dictionary<PNode, int>());
				if (!_uniqueElements[node.BinaryTag].ContainsKey(node)) _uniqueElements[node.BinaryTag][node] = elementIdx;
				else
				{
					if (node is PListBool) elementIdx = _uniqueElements[node.BinaryTag][node];
					else return _uniqueElements[node.BinaryTag][node];
				}
			}

			int offset = (int) stream.Position;
			Offsets.Add(offset);
			int len = node.BinaryLength;
			var typeCode = (byte) (node.BinaryTag << 4 | (len < 0x0F ? len : 0x0F));
			stream.WriteByte(typeCode);
			if (len >= 0x0F)
			{
				var extLen = NodeFactory.CreateLengthElement(len);
				var binaryTag = (byte) (extLen.BinaryTag << 4 | extLen.BinaryLength);
				stream.WriteByte(binaryTag);
				extLen.WriteBinary(stream);
			}

			var arrayNode = node as PListArray;
			if (arrayNode != null)
			{
				WriteInternal(stream, arrayNode);
				return elementIdx;
			}

			if (node is PListDict)
			{
				//
				return elementIdx;
			}
				
			node.WriteBinary(stream);
			return elementIdx;
		}

		private void WriteInternal(Stream stream, int nodeIndexSize, PListArray array)
		{
			var nodes = new byte[nodeIndexSize * array.Count];
			var streamPos = stream.Position;

			stream.Write(nodes, 0, nodes.Length);
			for (int i = 0; i < array.Count; i++)
			{
				int elementIdx = WriteInternal(stream, array[i]);
				FormatIdx(elementIdx, nodeIndexSize).CopyTo(nodes, nodeIndexSize * i);
			}

			stream.Seek(streamPos, SeekOrigin.Begin);
			stream.Write(nodes, 0, nodes.Length);
			stream.Seek(0, SeekOrigin.End);
		}

		private static int GetNodeCount(PNode node)
		{
			return 1;
		}

		private static int GetNodeCount(PListArray array)
		{
			int count = 1;
			foreach (var node in array)
			{
				count += GetNodeCount(node);
			}
			return count;
		}

		private static int GetNodeCount(IDictionary<string, PNode> dictionary)
		{
			int count = 1;
			foreach (var node in dictionary.Values)
			{
				count += GetNodeCount(node);
			}
			count += dictionary.Keys.Count;
			return count;
		}
	}
}
