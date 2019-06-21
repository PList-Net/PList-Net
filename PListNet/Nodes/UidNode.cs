using System;

namespace PListNet.Nodes
{
    /// <summary>
    /// Represents a UID value from a PList
    /// </summary>
    public class UidNode : IntegerNode
    {
        private UInt64 _value;
        
        internal override string XmlTag => "uid";

        internal override byte BinaryTag => 8;

        internal override void Parse(string data)
        {
            throw new NotImplementedException();
        }

        internal override string ToXmlString()
        {
            return $"<dict><key>CF$UID</key><integer>{_value}</integer></dict>";
        }
    }
}
