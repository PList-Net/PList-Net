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

			// compute resource name suffix
			var relativeName = "." + relativeFilePath
				.Replace(Path.DirectorySeparatorChar, namespaceSeparator)
				.Replace(' ', '_');

			// get resource stream
			var fullName = assembly
				.GetManifestResourceNames()
				.FirstOrDefault(name => name.EndsWith(relativeName, StringComparison.InvariantCulture));
			if (fullName == null)
			{
				throw new Exception(string.Format("Unable to find resource for path \"{0}\". Resource with name ending on \"{1}\" was not found in assembly.", relativeFilePath, relativeName));
			}

			var stream = assembly.GetManifestResourceStream(fullName);
			if (stream == null)
			{
				throw new Exception(string.Format("Unable to find resource for path \"{0}\". Resource named \"{1}\" was not found in assembly.", relativeFilePath, fullName));
			}

			return stream;
		}
	}
}
