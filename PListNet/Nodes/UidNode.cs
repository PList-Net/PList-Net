using System;

namespace PListNet.Nodes
{
    /// <summary>
    /// Represents a UID value from a PList
    /// </summary>
    public class UidNode : IntegerNode
    {
        internal override string XmlTag => "uid";

        internal override byte BinaryTag => 8;

        internal override void Parse(string data)
        {
            throw new NotImplementedException();
        }

        internal override string ToXmlString()
        {
            throw new NotImplementedException();
        }
    }
}
