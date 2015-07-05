/* =================================================================================
 * File:   PListData.cs
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
using PListNet.Exceptions;
using PListNet.Internal;

namespace PListNet.Primitives {
    /// <summary>
    /// Represents a Byte[] Value from a PList
    /// </summary>
    public class PListData : PListElement<Byte[]>{
        /// <summary>
        /// Gets the Xml tag of this element.
        /// </summary>
        /// <value>The Xml tag of this element.</value>
        public override String Tag { get { return "data"; } }

        /// <summary>
        /// Gets the binary typecode of this element.
        /// </summary>
        /// <value>The binary typecode of this element.</value>
        public override Byte TypeCode { get { return 4; } }

        /// <summary>
        /// Gets or sets the value of this element.
        /// </summary>
        /// <value>The value of this element.</value>
        public override Byte[] Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PListData"/> class.
        /// </summary>
        public PListData() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PListData"/> class.
        /// </summary>
        /// <param name="value">The value of this element.</param>
        public PListData(Byte[] value) { Value = value; }

        /// <summary>
        /// Parses the specified value from a given String (encoded as Base64), read from Xml.
        /// </summary>
        /// <param name="value">The String whis is parsed.</param>
        protected override void Parse(String value) {
            Value = Convert.FromBase64String(value);
        }

        /// <summary>
        /// Gets the XML String representation of the Value.
        /// </summary>
        /// <returns>
        /// The XML String representation of the Value (encoded as Base64).
        /// </returns>
        protected override String ToXmlString() {
            return Convert.ToBase64String(Value);
        }

        /// <summary>
        /// Reads this element binary from the reader.
        /// </summary>
        /// <param name="reader">The <see cref="T:PListNet.Internal.PListBinaryReader"/> from which the element is read.</param>
        /// <remarks>Provided for internal use only.</remarks>
        public override void ReadBinary(PListBinaryReader reader) {
            Value = new Byte[reader.CurrentElementLength];
            if (reader.BaseStream.Read(Value, 0, Value.Length) != Value.Length)
                throw new PListFormatException();
        }

        /// <summary>
        /// Gets the length of this PList element.
        /// </summary>
        /// <returns>The length of this PList element.</returns>
        /// <remarks>Provided for internal use only.</remarks>
        public override int GetPListElementLength() {
            return Value.Length;
        }

        /// <summary>
        /// Writes this element binary to the writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:PListNet.Internal.PListBinaryWriter"/> to which the element is written.</param>
        /// <remarks>Provided for internal use only.</remarks>
        public override void WriteBinary(PListBinaryWriter writer) {
            writer.BaseStream.Write(Value, 0, Value.Length);
        }
    }
}
