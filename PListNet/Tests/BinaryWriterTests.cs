using System.IO;
using NUnit.Framework;
using PListNet.Nodes;

namespace PListNet.Tests
{
	[TestFixture]
	public class BinaryWriterTests
	{
		[Test]
		public void WhenXmlFormatIsResavedAsBinaryAndOpened_ThenParsedDocumentMatchesTheOriginal()
		{
			using (var stream = TestFileHelper.GetTestFileStream("TestFiles/asdf-Info.plist"))
			{
				var node = PList.Load(stream);

				using (var outStream = new MemoryStream())
				{
					PList.Save(node, outStream, PListFormat.Binary);

					// rewind and reload
					outStream.Seek(0, SeekOrigin.Begin);
					var newNode = PList.Load(outStream);

					// compare
					Assert.AreEqual(node.GetType().Name, newNode.GetType().Name);

					var oldDict = node as DictionaryNode;
					var newDict = newNode as DictionaryNode;

					Assert.AreEqual(oldDict.Count, newDict.Count);

					foreach (var key in oldDict.Keys)
					{
						Assert.IsTrue(newDict.ContainsKey(key));

						var oldValue = oldDict[key];
						var newValue = newDict[key];

						Assert.AreEqual(oldValue.GetType().Name, newValue.GetType().Name);
						Assert.AreEqual(oldValue, newValue);
					}
				}
			}
		}
	}
}
