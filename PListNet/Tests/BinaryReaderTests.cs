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
    }
}
