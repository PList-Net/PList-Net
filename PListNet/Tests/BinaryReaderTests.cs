using NUnit.Framework;
using PListNet.Nodes;

namespace PListNet.Tests
{
	[TestFixture]
	public class BinaryReaderTests
	{
		[Test]
		public void WhenParsingBinaryDocumentWithSingleDictionary_ThenItIsParsedCorrectly()
		{
			using (var stream = TestFileHelper.GetTestFileStream("TestFiles/asdf-Info.bin.plist"))
			{
				var node = PList.Load(stream);

				Assert.IsNotNull(node);

				var dictionary = node as DictionaryNode;
				Assert.IsNotNull(dictionary);

				Assert.AreEqual(14, dictionary.Count);
			}
		}
	}
}
