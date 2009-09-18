/* =================================================================================
 * File:   PListNull.cs
 * Author: Christian Ecker
 *
 * Major Changes:
 * yyyy-mm-dd   Author               Description
 * ----------------------------------------------------------------
 * 2009-09-13   Christian Ecker      Created
 *
 * =================================================================================
 * Copyright (c) 2009, Christian Ecker
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 *  - Redistributions of source code must retain the above copyright notice, 
 *    this list of conditions and the following disclaimer.
 * 
 *  - Redistributions in binary form must reproduce the above copyright notice, 
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *    
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
 * THE POSSIBILITY OF SUCH DAMAGE.
 * =================================================================================
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using CE.iPhone.PList.Internal;

namespace CE.iPhone.PList {
    /// <summary>
    /// Represents a null element in a PList
    /// </summary>
    /// <remarks>Is skipped in Xml-Serialization</remarks>
    public class PListNull : IPListElement {
        #region IPListElement Members

        /// <summary>
        /// Gets the Xml tag of this element.
        /// </summary>
        /// <value>The Xml tag of this element.</value>
        public String Tag { get { return "null"; } }

        /// <summary>
        /// Gets the binary typecode of this element.
        /// </summary>
        /// <value>The binary typecode of this element.</value>
        public Byte TypeCode { get { return 0; } }

        /// <summary>
        /// Gets a value indicating whether this instance is written only once in binary mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> this instance is written only once in binary mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsBinaryUnique { get { return false; } }

        /// <summary>
        /// Reads this element binary from the reader.
        /// </summary>
        /// <param name="reader">The <see cref="T:CE.iPhone.PListBinaryReader"/> from which the element is read.</param>
        /// <remarks>Provided for internal use only.</remarks>
        public void ReadBinary(PListBinaryReader reader) {
            if (reader.CurrentElementLength != 0x00)
                throw new PListFormatException();

        }

        /// <summary>
        /// Gets the length of this PList element.
        /// </summary>
        /// <returns>The length of this PList element.</returns>
        /// <remarks>Provided for internal use only.</remarks>
        public int GetPListElementLength() {
            return 0;
        }

        /// <summary>
        /// Gets the count of PList elements in this element.
        /// </summary>
        /// <returns>
        /// The count of PList elements in this element.
        /// </returns>
        /// <remarks>Provided for internal use only.</remarks>
        public int GetPListElementCount() {
            return 1;
        }

        /// <summary>
        /// Writes this element binary to the writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:CE.iPhone.PListBinaryWriter"/> to which the element is written.</param>
        /// <remarks>Provided for internal use only.</remarks>
        public void WriteBinary(PListBinaryWriter writer) { }
        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        public void ReadXml(System.Xml.XmlReader reader) { reader.ReadStartElement(Tag); }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(System.Xml.XmlWriter writer) {
            writer.WriteStartElement(Tag);
            writer.WriteEndElement();
        }

        #endregion

    }
}
