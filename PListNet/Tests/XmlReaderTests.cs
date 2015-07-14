using NUnit.Framework;
using PListNet.Nodes;
using System.Linq;

namespace PListNet.Tests
{
	[TestFixture]
	public class XmlReaderTests
	{
		[Test]
		public void WhenParsingXmlDocumentWithSingleDictionary_ThenItIsParsedCorrectly()
		{
			using (var stream = TestFileHelper.GetTestFileStream("TestFiles/asdf-Info.plist"))
			{
				var node = PList.Load(stream);

				Assert.IsNotNull(node);

				var dictionary = node as DictionaryNode;
				Assert.IsNotNull(dictionary);

				Assert.AreEqual(14, dictionary.Count);
			}
		}

		[Test]
		public void WhenDocumentContainsNestedCollections_ThenDocumentIsParsedCorrectly()
		{
			using (var stream = TestFileHelper.GetTestFileStream("TestFiles/dict-inside-array.plist"))
			{
				var node = PList.Load(stream);

				Assert.IsNotNull(node);
				Assert.IsInstanceOf<DictionaryNode>(node);

				var array = ((DictionaryNode) node).Values.First() as ArrayNode;
				Assert.IsNotNull(array);
				Assert.AreEqual(1, array.Count);

				var dictionary = array[0] as DictionaryNode;
				Assert.IsNotNull(dictionary);

				Assert.AreEqual(4, dictionary.Count);
			}
		}
	}
}
