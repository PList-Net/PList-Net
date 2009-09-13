/* =================================================================================
 * File:   PListBinaryWriter.cs
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
using System.Diagnostics;
using System.IO;
using System.Net;

namespace CE.iPhone.PList.Internal {
    /// <summary>
    /// A class, used to write a <see cref="T:CE.iPhone.IPListElemnet"/>  binary formated to a stream
    /// </summary>
    public class PListBinaryWriter {
        /// <summary>
        /// The Header (bplist00)
        /// </summary>
        private static readonly Byte[] s_PListHeader = new Byte[] {
            0x62, 0x70, 0x6c, 0x69, 0x73, 0x74, 0x30, 0x30};

        /// <summary>
        /// Gets the basestream.
        /// </summary>
        /// <value>The basestream.</value>
        internal Stream BaseStream { get; private set; }

        /// <summary>
        /// Gets or sets the size of the element idx.
        /// </summary>
        /// <value>The size of the element idx.</value>
        internal Byte ElementIdxSize { get; private set; }

        /// <summary>
        /// Gets the offset table.
        /// </summary>
        /// <value>The offset table.</value>
        internal List<Int32> Offsets { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PListBinaryWriter"/> class.
        /// </summary>
        internal PListBinaryWriter() { }

        /// <summary>
        /// Writers a <see cref="T:CE.iPhone.IPListElemnet"/> to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="element">The element.</param>
        public void Writer(Stream stream, IPListElement element) {
            BaseStream = stream;
            Offsets = new List<int>();
            BaseStream.Write(s_PListHeader, 0, s_PListHeader.Length);
            int elemCnt = element.GetPListElementCount();
            if (elemCnt <= Byte.MaxValue) ElementIdxSize = sizeof(Byte);
            else if (elemCnt <= Int16.MaxValue) ElementIdxSize = sizeof(Int16);
            else ElementIdxSize = sizeof(Int32);

            int topOffestIdx = WriteInternal(element);
            int offsetTableOffset = (int)BaseStream.Position;


            Byte offsetSize = 0;
            if (offsetTableOffset <= Byte.MaxValue) offsetSize = sizeof(Byte);
            else if (offsetTableOffset <= Int16.MaxValue) offsetSize = sizeof(Int16);
            else offsetSize = sizeof(Int32);

            for (int i = 0; i < Offsets.Count; i++) {
                Byte[] buf = null;
                switch (offsetSize) {
                    case 1: buf = new Byte[] { (Byte)Offsets[i] }; break;
                    case 2: buf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16)Offsets[i])); break;
                    case 4: buf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int32)Offsets[i])); break;
                }
                BaseStream.Write(buf, 0, buf.Length);
            }


            //BaseStream.Write(s_PListHeader, 0, s_PListHeader.Length);

            Byte[] header = new Byte[32];
            header[6] = offsetSize;
            header[7] = ElementIdxSize;

            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(elemCnt)).CopyTo(header, 12);
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(topOffestIdx)).CopyTo(header, 20);
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(offsetTableOffset)).CopyTo(header, 28);

            BaseStream.Write(header, 0, header.Length);
        }

        /// <summary>
        /// Formats an element idx based on the ElementIdxSize.
        /// </summary>
        /// <param name="idx">The idx.</param>
        /// <returns>The formated idx.</returns>
        internal Byte[] FormatIdx(int idx) {
            Byte[] res;
            switch (ElementIdxSize) {
                case 1: res = new Byte[] { (Byte)idx }; break;
                case 2: res = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int16)idx)); break;
                case 4: res = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((Int32)idx)); break;
                default:
                    throw new PListFormatException("Invalid ElementIdxSize");
            }
            return res;
        }

        /// <summary>
        /// Writers a <see cref="T:CE.iPhone.IPListElemnet"/> to the current stream position
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The Inx of the written element</returns>
        internal int WriteInternal(IPListElement element) {
            int offset = (int)BaseStream.Position;
            Offsets.Add(offset);
            int elementIdx = Offsets.Count;
            int len = element.GetPListElementLength();
            Byte typeCode = (Byte)(element.TypeCode << 4 | (len < 0x0F ? len : 0x0F));
            BaseStream.WriteByte(typeCode);
            if (len >= 0x0F) {
                IPListElement extLen = PListElementFactory.Instance.CreateLengthElement(len);
                Byte extLenTypeCode = (Byte)(extLen.TypeCode << 4 | extLen.GetPListElementLength());
                BaseStream.WriteByte(extLenTypeCode);
                extLen.WriteBinary(this);
            }
            element.WriteBinary(this);
            return elementIdx-1;
        }
    }
}
