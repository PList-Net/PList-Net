using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PListNet.Nodes;

namespace PListNet.Internal
{
	/// <summary>
	/// A class, used to read binary formated <see cref="T:PListNet.PNode"/> from a stream
	/// </summary>
	internal class PListBinaryReader
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PListBinaryReader"/> class.
		/// </summary>
		internal PListBinaryReader()
		{
		}

		/// <summary>
		/// Reads a binary formated <see cref="T:PListNet.PNode"/> from the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>The <see cref="T:PListNet.PNode"/>, read from the specified stream</returns>
		public PNode Read(Stream stream)
		{
			// read in file header
			var header = ReadHeader(stream);

			// read in node offsets
			var nodeOffsets = ReadNodeOffsets(stream, header);

			var indexSize = header[7];

			var readerState = new ReaderState(stream, nodeOffsets, indexSize);

			var topNode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 20));
			return ReadInternal(readerState, topNode);
		}

		private static byte[] ReadHeader(Stream stream)
		{
			var header = new byte[32];

			// header is 32 bytes at the end of file
			stream.Seek(-32, SeekOrigin.End);
			if (stream.Read(header, 0, header.Length) != header.Length)
			{
				throw new PListFormatException("Invalid Header Size");
			}

			return header;
		}

		private static int[] ReadNodeOffsets(Stream stream, byte[] header)
		{
			var offsetSize = header[6];

			var nodeCount = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 12));
			var offsetTableOffset = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 28));
			var offsetTableBuffer = new byte[nodeCount * offsetSize];

			stream.Seek(offsetTableOffset, SeekOrigin.Begin);
			if (stream.Read(offsetTableBuffer, 0, offsetTableBuffer.Length) != offsetTableBuffer.Length)
			{
				throw new PListFormatException("Invalid offsetTable Size");
			}

			var nodeOffsets = new int[nodeCount];
			for (int i = 0; i < nodeCount; i++)
			{
				var cur = new byte[sizeof(uint)];
				for (int j = 0; j < offsetSize; j++)
				{
					cur[offsetSize - 1 - j] = offsetTableBuffer[i * offsetSize + j];
				}
				nodeOffsets[i] = BitConverter.ToInt32(cur, 0);
			}

			return nodeOffsets;
		}

		/// <summary>
		/// Reads the <see cref="T:PListNet.PNode"/> at the specified idx.
		/// </summary>
		/// <param name="readerState">Reader state.</param>
		/// <param name="elemIdx">The elem idx.</param>
		/// <returns>The <see cref="T:PListNet.PNode"/> at the specified idx.</returns>
		private PNode ReadInternal(ReaderState readerState, int elemIdx)
		{
			readerState.Stream.Seek(readerState.NodeOffsets[elemIdx], SeekOrigin.Begin);
			return ReadInternal(readerState);
		}

		/// <summary>
		/// Reads the <see cref="T:PListNet.PNode"/> at the current stream position.
		/// </summary>
		/// <param name="readerState">Reader state.</param>
		/// <returns>The <see cref="T:PListNet.PNode"/> at the current stream position.</returns>
		private PNode ReadInternal(ReaderState readerState)
		{
			var buf = new byte[1];
			if (readerState.Stream.Read(buf, 0, buf.Length) != 1)
			{
				throw new PListFormatException("Couldn't read type Byte");
			}

			var objectLength = buf[0] & 0x0F;
			var tag = (Byte) ((buf[0] >> 4) & 0x0F);

			if (tag != 0 && objectLength == 0x0F)
			{
				var lengthNode = ReadInternal(readerState);
				if (!(lengthNode is IntegerNode))
				{
					throw new PListFormatException("Length is not an integer.");
				}

				objectLength = (int) ((IntegerNode) lengthNode).Value;
			}

			var node = NodeFactory.Create(tag, objectLength);

			// array and dictionary are special-cased here
			// while primitives handle their own loading
			var arrayNode = node as ArrayNode;
			if (arrayNode != null)
			{
				ReadInArray(arrayNode, objectLength, readerState);
				return node;
			}

			var dictionaryNode = node as DictionaryNode;
			if (dictionaryNode != null)
			{
				ReadInDictionary(dictionaryNode, objectLength, readerState);
				return node;
			}

			node.ReadBinary(readerState.Stream, objectLength);

			return node;
		}

		private void ReadInArray(ICollection<PNode> node, int nodeLength, ReaderState readerState)
		{
			var buf = new byte[nodeLength * readerState.IndexSize];
			if (readerState.Stream.Read(buf, 0, buf.Length) != buf.Length)
			{
				throw new PListFormatException();
			}

			for (int i = 0; i < nodeLength; i++)
			{
				var topNode = readerState.IndexSize == 1
					? buf[i]
					: IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buf, 2 * i));
				node.Add(ReadInternal(readerState, topNode));
			}
		}

		private void ReadInDictionary(IDictionary<string, PNode> node, int nodeLength, ReaderState readerState)
		{
			var bufKeys = new byte[nodeLength * readerState.IndexSize];
			var bufVals = new byte[nodeLength * readerState.IndexSize];

			if (readerState.Stream.Read(bufKeys, 0, bufKeys.Length) != bufKeys.Length)
			{
				throw new PListFormatException();
			}

			if (readerState.Stream.Read(bufVals, 0, bufVals.Length) != bufVals.Length)
			{
				throw new PListFormatException();
			}

			for (int i = 0; i < nodeLength; i++)
			{
				var topNode = readerState.IndexSize == 1
					? bufKeys[i]
					: IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bufKeys, 2 * i));
				var plKey = ReadInternal(readerState, topNode);

				var stringKey = plKey as StringNode;
				if (stringKey == null)
				{
					throw new PListFormatException("Key is not a string");
				}

				topNode = readerState.IndexSize == 1
					? bufVals[i]
					: IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bufVals, 2 * i));
				var plVal = ReadInternal(readerState, topNode);

				node.Add(stringKey.Value, plVal);
			}
		}

		private class ReaderState
		{
			public Stream Stream { get; private set; }
			public int[] NodeOffsets { get; private set; }
			public int IndexSize { get; private set; }

			public ReaderState(Stream stream, int[] nodeOffsets, int indexSize)
			{
				Stream = stream;
				NodeOffsets = nodeOffsets;
				IndexSize = indexSize;
			}
		}
	}
}
