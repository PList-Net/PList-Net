using System;
using NUnit.Framework;
using PListNet.Collections;

namespace PListNet.Tests
{
	[TestFixture]
	public class XmlReaderTests
	{
		[Test]
		public void WhenParsingADocumentWithSingleDictionary_ThenItIsParsedCorrectly()
		{
			using (var stream = TestFileHelper.GetTestFileStream("TestFiles/asdf-Info.plist"))
			{
				var node = PList.Load(stream);

				Assert.IsNotNull(node);

				var dictionary = node as PListDict;
				Assert.IsNotNull(dictionary);

				Assert.AreEqual(14, dictionary.Count);
			}
		}
	}
}
