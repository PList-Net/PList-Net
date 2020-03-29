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
			using (var stream = TestFileHelper.GetTestFileStream("TestFiles/asdf-Info-bin.plist"))
			{
				var node = PList.Load(stream);

				Assert.IsNotNull(node);

				var dictionary = node as DictionaryNode;
				Assert.IsNotNull(dictionary);

				Assert.AreEqual(14, dictionary.Count);
			}
		}

        [Test]
        public void ReadingFile_With_UID_Field_Fail()
        {
            using (var stream = TestFileHelper.GetTestFileStream("TestFiles/uid-test.plist"))
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

		[Test]
		public void WhenReadingUid_UidNodeIsParsed()
		{
			using (var stream = TestFileHelper.GetTestFileStream("TestFiles/github-7-binary.plist"))
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

		[Test]
		public void WhenReadingFileWithUid_UidValueIsParsed()
		{
			// this binary .plist file came from https://bugs.python.org/issue26707
			using (var stream = TestFileHelper.GetTestFileStream("TestFiles/github-7-binary-2.plist"))
			{
				var root = PList.Load(stream) as DictionaryNode;

				Assert.IsNotNull(root);
				Assert.AreEqual(4, root.Count);

				var dict = root["$top"] as DictionaryNode;
				Assert.IsNotNull(dict);

				var uid = dict["data"] as UidNode;
				Assert.IsNotNull(uid);

				Assert.AreEqual(1, uid.Value);
			}
		}

		[Test]
	    public void ReadingFile_With_16bit_Integers_Fail()
	    {
	        using (var stream = TestFileHelper.GetTestFileStream("TestFiles/unity.binary.plist"))
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

	    [Test]
	    public void ReadingFile_GitHub_Issue9_Fail()
	    {
	        using (var stream = TestFileHelper.GetTestFileStream("TestFiles/github-9.plist"))
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

		[Test]
		public void ReadingFile_GitHub_Issue15_FailReadingLargeDictionary()
		{
			using (var stream = TestFileHelper.GetTestFileStream("TestFiles/github-15-medium-binary.plist"))
			{
				try
				{
					var node = PList.Load(stream);

					var dictNode = node as DictionaryNode;
					Assert.IsNotNull(dictNode);
					Assert.AreEqual(16384, dictNode.Keys.Count);
				}
				catch (PListFormatException ex)
				{
					Assert.Fail(ex.Message);
				}
			}
		}

		[Test]
		public void ReadingFile_GitHub_Issue15_FailReadingLargeArray()
		{
			using (var stream = TestFileHelper.GetTestFileStream("TestFiles/github-15-large-binary.plist"))
			{
				try
				{
					var node = PList.Load(stream);

					var dictNode = node as DictionaryNode;
					Assert.IsNotNull(dictNode);
					Assert.AreEqual(32768, dictNode.Keys.Count);
				}
				catch (PListFormatException ex)
				{
					Assert.Fail(ex.Message);
				}
			}
		}
	}
}
