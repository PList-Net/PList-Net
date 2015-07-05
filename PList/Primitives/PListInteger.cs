/* =================================================================================
 * File:   PListInteger.cs
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
using System.Globalization;
using System.Net;
using PListNet.Exceptions;
using PListNet.Internal;

namespace PListNet.Primitives {
    /// <summary>
    /// Represents an integer Value from a PList
    /// </summary>
    public class PListInteger : PListElement<Int64> {
        /// <summary>
        /// Gets the Xml tag of this element.
        /// </summary>
        /// <value>The Xml tag of this element.</value>
        public override String Tag { get { return "integer"; } }

        /// <summary>
        /// Gets the binary typecode of this element.
        /// </summary>
        /// <value>The binary typecode of this element.</value>
        public override Byte TypeCode { get { return 1; } }

        /// <summary>
        /// Gets or sets the value of this element.
        /// </summary>
        /// <value>The value of this element.</value>
        public override Int64 Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PListInteger"/> class.
        /// </summary>
        public PListInteger() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PListInteger"/> class.
        /// </summary>
        /// <param name="value">The value of this element.</param>
        public PListInteger(Int64 value) { Value = value; }

        /// <summary>
        /// Parses the specified value from a given String, read from Xml.
        /// </summary>
        /// <param name="value">The String whis is parsed.</param>
        protected override void Parse(String value) {
            Value = Int64.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the XML String representation of the Value.
        /// </summary>
        /// <returns>
        /// The XML String representation of the Value.
        /// </returns>
        protected override String ToXmlString() {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Reads this element binary from the reader.
        /// </summary>
        /// <param name="reader">The <see cref="T:PListNet.Internal.PListBinaryReader"/> from which the element is read.</param>
        /// <remarks>Provided for internal use only.</remarks>
        public override void ReadBinary(PListBinaryReader reader) {

            Byte[] buf = new Byte[1 << (int)reader.CurrentElementLength];
            if (reader.BaseStream.Read(buf, 0, buf.Length) != buf.Length)
                throw new PListFormatException();


            switch (reader.CurrentElementLength) {
                case 0: Value = buf[0]; break;
                case 1: Value = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buf, 0)); break;
                case 2: Value = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buf, 0)); break;
                case 3: Value = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buf, 0)); break;
                default: throw new PListFormatException("Int > 64Bit");
            }
        }

        /// <summary>
        /// Gets the length of this PList element.
        /// </summary>
        /// <returns>The length of this PList element.</returns>
        /// <remarks>Provided for internal use only.</remarks>
        public override int GetPListElementLength() {
            if (Value >= Byte.MinValue && Value <= Byte.MaxValue) return 0;
            if (Value >= Int16.MinValue && Value <= Int16.MaxValue) return 1;
            if (Value >= Int32.MinValue && Value <= Int32.MaxValue) return 2;
            if (Value >= Int64.MinValue && Value <= Int64.MaxValue) return 3;
            return -1;
        }

        /// <summary>
        /// Writes this element binary to the writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:PListNet.Internal.PListBinaryWriter"/> to which the element is written.</param>
        /// <remarks>Provided for internal use only.</remarks>
        public override void WriteBinary(PListBinaryWriter writer) {
            int length = GetPListElementLength();
            Byte[] buf = null;
            switch (length) {
                case 0: buf = new Byte[] { (Byte)Value }; break;
                case 1: buf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16)Value)); break;
                case 2: buf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int32)Value)); break;
                case 3: buf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int64)Value)); break;
            }
            writer.BaseStream.Write(buf, 0, buf.Length);
        }
    }
}
