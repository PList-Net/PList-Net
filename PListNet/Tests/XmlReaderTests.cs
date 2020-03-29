using System.Linq;
using NUnit.Framework;
using PListNet.Nodes;

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

		[Test]
		public void WhenDocumentContainsNestedCollectionsAndComplexText_ThenDocumentIsParsedCorrectly()
		{
			using (var stream = TestFileHelper.GetTestFileStream("TestFiles/Pods-acknowledgements.plist"))
			{
				var root = PList.Load(stream) as DictionaryNode;

				Assert.IsNotNull(root);
				Assert.AreEqual(3, root.Count);

				Assert.IsInstanceOf<StringNode>(root["StringsTable"]);
				Assert.IsInstanceOf<StringNode>(root["Title"]);

				var array = root["PreferenceSpecifiers"] as ArrayNode;
				Assert.IsNotNull(array);
				Assert.AreEqual(15, array.Count);

				foreach (var node in array)
				{
					Assert.IsInstanceOf<DictionaryNode>(node);

					var dictionary = (DictionaryNode) node;
					Assert.AreEqual(3, dictionary.Count);
				}
			}
		}

        [Test]
        public void WhenDocumentContainsEmptyArray_ThenDocumentIsParsedCorrectly()
        {
            using (var stream = TestFileHelper.GetTestFileStream("TestFiles/empty-array.plist"))
            {
                var root = PList.Load(stream) as DictionaryNode;

                Assert.IsNotNull(root);
                Assert.AreEqual(1, root.Count);

                Assert.IsInstanceOf<DictionaryNode>(root["Entitlements"]);
				var dict = root["Entitlements"] as DictionaryNode;

				var array = dict["com.apple.developer.icloud-container-identifiers"] as ArrayNode;
                Assert.IsNotNull(array);
                Assert.AreEqual(0, array.Count);
            }
        }

		[Test]
		public void WhenReadingUid_UidNodeIsParsed()
		{
			using (var stream = TestFileHelper.GetTestFileStream("TestFiles/github-7-xml.plist"))
			{
				try
				{
					var node = PList.Load(stream);
					Assert.Pass();
				}
				catch (PListFormatException ex)
				{
					Assert.Fail(ex.Message);
				}
			}
		}
	}
}
