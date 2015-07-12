///* =================================================================================
// * File:   PListElement.cs
// * Author: Christian Ecker
// *
// * Major Changes:
// * yyyy-mm-dd   Author               Description
// * ----------------------------------------------------------------
// * 2009-09-13   Christian Ecker      Created
// *
// * =================================================================================
// * Copyright (c) 2009, Christian Ecker
// * All rights reserved.
// * 
// * Redistribution and use in source and binary forms, with or without modification, 
// * are permitted provided that the following conditions are met:
// * 
// *  - Redistributions of source code must retain the above copyright notice, 
// *    this list of conditions and the following disclaimer.
// * 
// *  - Redistributions in binary form must reproduce the above copyright notice, 
// *    this list of conditions and the following disclaimer in the documentation
// *    and/or other materials provided with the distribution.
// *    
// * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE 
// * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// * THE POSSIBILITY OF SUCH DAMAGE.
// * =================================================================================
// */
//using System;
//using System.Xml;
//using System.Xml.Schema;
//
//namespace PListNet.Internal {
//    /// <summary>
//    /// An abstract base class for primitive PList types
//    /// </summary>
//    /// <typeparam name="T">The .Net equivalent to the PList type</typeparam>
//    public abstract class PListElement<T> : IPListElement, IEquatable<IPListElement> {
//
//        /// <summary>
//        /// Gets the Xml tag of this element.
//        /// </summary>
//        /// <value>The Xml tag of this element.</value>
//        public abstract string Tag { get; }
//
//        /// <summary>
//        /// Gets the binary typecode of this element.
//        /// </summary>
//        /// <value>The binary typecode of this element.</value>
//        public abstract Byte TypeCode { get; }
//
//        /// <summary>
//        /// Gets a value indicating whether this instance is written only once in binary mode.
//        /// </summary>
//        /// <value>
//        /// 	<c>true</c> this instance is written only once in binary mode; otherwise, <c>false</c>.
//        /// </value>
//        public virtual bool IsBinaryUnique { get { return true; } }
//
//        /// <summary>
//        /// Gets or sets the value of this element.
//        /// </summary>
//        /// <value>The value of this element.</value>
//        public abstract T Value { get; set; }
//
//        /// <summary>
//        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, 
//        /// you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a 
//        /// custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> 
//        /// to the class.
//        /// </summary>
//        /// <returns>
//        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is
//        /// produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> 
//        /// method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/>
//        /// method.
//        /// </returns>
//        public virtual XmlSchema GetSchema() { return null; }
//
//        /// <summary>
//        /// Generates an object from its XML representation.
//        /// </summary>
//        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
//        public virtual void ReadXml(XmlReader reader) {
//            reader.ReadStartElement();
//            Parse(reader.ReadString());
//            reader.ReadEndElement();
//        }
//
//        /// <summary>
//        /// Converts an object into its XML representation.
//        /// </summary>
//        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
//        public virtual void WriteXml(XmlWriter writer) {
//            writer.WriteStartElement(Tag);
//            writer.WriteValue(ToXmlString());
//            writer.WriteEndElement();
//        }
//
//        /// <summary>
//        /// Parses the specified value from a given string, read from Xml.
//        /// </summary>
//        /// <param name="value">The string whis is parsed.</param>
//        protected abstract void Parse(string value);
//        
//        /// <summary>
//        /// Gets the XML string representation of the Value.
//        /// </summary>
//        /// <returns>The XML string representation of the Value.</returns>
//        protected abstract string ToXmlString();
//
//        /// <summary>
//		/// Performs an implicit conversion from <see cref="T:PListNet.PListElement&lt;T&gt;"/> to <see cref="T"/>.
//        /// </summary>
//        /// <param name="element">The elem.</param>
//        /// <returns>The result of the conversion.</returns>
//        public static implicit operator T(PListElement<T> element) {
//            return element.Value;
//        }
//
//        /// <summary>
//        /// Returns a <see cref="System.string"/> that represents this instance.
//        /// </summary>
//        /// <returns>
//        /// A <see cref="System.string"/> that represents this instance.
//        /// </returns>
//        public override string ToString() {
//            return string.Format("{0}: {1}", Tag, Value);
//        }
//
//        /// <summary>
//        /// Gets the count of PList elements in this element.
//        /// </summary>
//        /// <returns>
//        /// The count of PList elements in this element.
//        /// </returns>
//        /// <remarks>Provided for internal use only.</remarks>
//        public virtual int GetPListElementCount() {
//            return 1;
//        }
//
//        /// <summary>
//        /// Gets the length of this PList element.
//        /// </summary>
//        /// <returns>The length of this PList element.</returns>
//        /// <remarks>Provided for internal use only.</remarks>
//        public abstract int GetPListElementLength();
//
//        /// <summary>
//        /// Reads this element binary from the reader.
//        /// </summary>
//        /// <param name="reader">The <see cref="T:PListNet.Internal.PListBinaryReader"/> from which the element is read.</param>
//        /// <remarks>Provided for internal use only.</remarks>
//        public abstract void ReadBinary(PListBinaryReader reader);
//
//        /// <summary>
//        /// Writes this element binary to the writer.
//        /// </summary>
//        /// <param name="writer">The <see cref="T:PListNet.Internal.PListBinaryWriter"/> to which the element is written.</param>
//        /// <remarks>Provided for internal use only.</remarks>
//        public abstract void WriteBinary(PListBinaryWriter writer);
//
//        #region IEquatable<PListString> Members
//
//        /// <summary>
//        /// Indicates whether the current object is equal to another object of the same type.
//        /// </summary>
//        /// <param name="other">An object to compare with this object.</param>
//        /// <returns>
//        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
//        /// </returns>
//        public bool Equals(IPListElement other) {
//            return (other is PListElement<T>) && (Value.Equals(((PListElement<T>)other).Value));
//        }
//
//		/// <summary>
//		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="T:PListNet.Internal.PListElement`1"/>.
//		/// </summary>
//		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="T:PListNet.Internal.PListElement`1"/>.</param>
//		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
//		/// <see cref="T:PListNet.Internal.PListElement`1"/>; otherwise, <c>false</c>.</returns>
//        public override bool Equals(object obj) {
//            if (!(obj is IPListElement)) return false;
//            return Equals((IPListElement)obj);
//        }
//
//		/// <summary>
//		/// Serves as a hash function for a <see cref="T:PListNet.Internal.PListElement`1"/> object.
//		/// </summary>
//		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
//        public override int GetHashCode() {
//            return Value.GetHashCode();
//        }
//        #endregion
//    }
//}
