using System;

namespace PListNet.Internal
{
	internal static class EndianConverter
	{
		public static long NetworkToHostOrder(long network)
		{
			return !BitConverter.IsLittleEndian ? network : SwapLong(network);
		}

		public static int NetworkToHostOrder(int network)
		{
			return !BitConverter.IsLittleEndian ? network : SwapInt(network);
		}

		public static short NetworkToHostOrder(short network)
		{
			return !BitConverter.IsLittleEndian ? network : SwapShort(network);
		}

		public static short HostToNetworkOrder(short host)
		{
			return !BitConverter.IsLittleEndian ? (host) : SwapShort(host);
		}

		public static int HostToNetworkOrder(int host)
		{
			return !BitConverter.IsLittleEndian ? (host) : SwapInt(host);
		}

		public static long HostToNetworkOrder(long host)
		{
			return !BitConverter.IsLittleEndian ? (host) : SwapLong(host);
		}

		private static short SwapShort(short number)
		{
			return (short) (((number >> 8) & 0xFF) | ((number << 8) & 0xFF00));
		}

		private static int SwapInt(int number)
		{
			return (((number >> 24) & 0xFF)
			| ((number >> 08) & 0xFF00)
			| ((number << 08) & 0xFF0000)
			| ((number << 24)));
		}

		private static long SwapLong(long number)
		{
			return (((number >> 56) & 0xFF)
			| ((number >> 40) & 0xFF00)
			| ((number >> 24) & 0xFF0000)
			| ((number >> 08) & 0xFF000000)
			| ((number << 08) & 0xFF00000000)
			| ((number << 24) & 0xFF0000000000)
			| ((number << 40) & 0xFF000000000000)
			| ((number << 56)));
		}
	}
}
