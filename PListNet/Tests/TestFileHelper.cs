using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PListNet.Tests
{
	public static class TestFileHelper
	{
		public static Stream GetTestFileStream(string relativeFilePath)
		{
			const char namespaceSeparator = '.';

			// get calling assembly
			var assembly = Assembly.GetCallingAssembly();

			// compute resource name suffix (replace Windows/Unix directory separators with namespace separator)
			var relativeName = "." + relativeFilePath
				.Replace('/', namespaceSeparator)
				.Replace('\\', namespaceSeparator)
				.Replace(' ', '_');

			// get resource stream
			var fullName = assembly
				.GetManifestResourceNames()
				.FirstOrDefault(name => name.EndsWith(relativeName, StringComparison.InvariantCulture));
			if (fullName == null)
			{
				throw new Exception($"Unable to find resource for path \"{relativeFilePath}\". Resource with name ending on \"{relativeName}\" was not found in assembly.");
			}

			var stream = assembly.GetManifestResourceStream(fullName);
			if (stream == null)
			{
				throw new Exception($"Unable to find resource for path \"{relativeFilePath}\". Resource named \"{fullName}\" was not found in assembly.");
			}

			return stream;
		}


		/// <summary>
		/// 	Compares contents of two streams and returns true if they are equal, false otherwise.
		/// </summary>
		/// <returns><c>true</c>, if stream contents equal was ared, <c>false</c> otherwise.</returns>
		/// <param name="stream1">Stream1.</param>
		/// <param name="stream2">Stream2.</param>
		public static bool AreStreamContentsEqual(Stream stream1, Stream stream2)
		{
			int file1byte;
			int file2byte;

			// check stream sizes. If they are not the same, the streams 
			// are not the same.
			if (stream1.Length != stream2.Length)
			{
				// return false to indicate files are different
				return false;
			}

			// remember current position and rewind
			var stream1Position = stream1.Position;
			var stream2Position = stream2.Position;
			stream1.Seek(0, SeekOrigin.Begin);
			stream2.Seek(0, SeekOrigin.Begin);

			// read and compare a byte from each file until either a
			// non-matching set of bytes is found or until the end of
			// file1 is reached.
			do 
			{
				// read one byte from each file.
				file1byte = stream1.ReadByte();
				file2byte = stream2.ReadByte();
			}
			while ((file1byte == file2byte) && (file1byte != -1));

			// reset streams to original positions
			stream1.Seek(stream1Position, SeekOrigin.Begin);
			stream2.Seek(stream2Position, SeekOrigin.Begin);

			// return the success of the comparison. "file1byte" is 
			// equal to "file2byte" at this point only if the streams are 
			// the same.
			return (file1byte == file2byte);
		}
	}
}
