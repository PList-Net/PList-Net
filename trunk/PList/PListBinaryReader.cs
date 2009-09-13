/* =================================================================================
 * File:   PListBinaryReader.cs
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
using System.Diagnostics;
using System.IO;
using System.Net;

namespace CE.iPhone.PList.Internal {
    /// <summary>
    /// A class, used to read binary formated <see cref="T:CE.iPhone.IPListElemnet"/> from a stream
    /// </summary>
    public class PListBinaryReader {
        private Int32[] m_Offsets;

        /// <summary>
        /// Gets the basestream.
        /// </summary>
        /// <value>The basestream.</value>
        internal Stream BaseStream { get; private set; }

        /// <summary>
        /// Gets or sets the size of the element idx.
        /// </summary>
        /// <value>The size of the element idx.</value>
        internal Byte ElementIdxSize {get; private set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="PListBinaryReader"/> class.
        /// </summary>
        internal PListBinaryReader() { }

        /// <summary>
        /// Reads a binary formated <see cref="T:CE.iPhone.IPListElemnet"/> from the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="T:CE.iPhone.IPListElemnet"/>, read from the specified stream</returns>
        public IPListElement Read(Stream stream) {
            BaseStream = stream;
            Byte[] header = new Byte[32];
            BaseStream.Seek(-32, SeekOrigin.End);
            if (BaseStream.Read(header, 0, header.Length) != header.Length)
                throw new PListFormatException("Invalid Header Size");

            Byte offsetSize = header[6];
            ElementIdxSize = header[7];
            Int32 elementCnt = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 12));
            Int32 topElement = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 20)); ;
            Int32 offsetTableOffset = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 28));

            Byte[] offsetTableBuf = new Byte[elementCnt * offsetSize];
            BaseStream.Seek(offsetTableOffset, SeekOrigin.Begin);
            if (BaseStream.Read(offsetTableBuf, 0, offsetTableBuf.Length) != offsetTableBuf.Length)
                throw new PListFormatException("Invalid offsetTable Size");

            m_Offsets = new Int32[elementCnt];
            for (int i = 0; i < m_Offsets.Length; i++) {
                Byte[] cur = new Byte[sizeof(UInt32)];
                for (int j = 0; j < offsetSize; j++) {
                    cur[offsetSize - 1 - j] = offsetTableBuf[i * offsetSize + j];
                }
                m_Offsets[i] = BitConverter.ToInt32(cur, 0);
            }

            return ReadInternal(topElement);

        }

        /// <summary>
        /// Gets the current element type code.
        /// </summary>
        /// <value>The current element type code.</value>
        internal Byte CurrentElementTypeCode { get; private set; }

        /// <summary>
        /// Gets the length of the current element.
        /// </summary>
        /// <value>The length of the current element.</value>
        internal Int32 CurrentElementLength { get; private set; }

        /// <summary>
        /// Reads the <see cref="T:CE.iPhone.IPListElemnet"/> at the specified idx.
        /// </summary>
        /// <param name="elemNr">The elem idx.</param>
        /// <returns>The <see cref="T:CE.iPhone.IPListElemnet"/> at the specified idx.</returns>
        internal IPListElement ReadInternal(int elemIdx) {
            BaseStream.Seek(m_Offsets[elemIdx], SeekOrigin.Begin);
            return ReadInternal();
        }

        /// <summary>
        /// Reads the <see cref="T:CE.iPhone.IPListElemnet"/> at the current stream position.
        /// </summary>
        /// <returns>The <see cref="T:CE.iPhone.IPListElemnet"/> at the current stream position.</returns>
        internal IPListElement ReadInternal() {
            Byte[] buf = new Byte[1];
            if (BaseStream.Read(buf, 0, buf.Length) != 1)
                throw new PListFormatException("Didn't read type Byte");

            Int32 objLen = buf[0] & 0x0F;
            Byte typeCode = (Byte)((buf[0] >> 4) & 0x0F);

            if (typeCode != 0 && objLen == 0x0F) {
                IPListElement lenElem = ReadInternal();
                if (!(lenElem is PListInteger))
                    throw new PListFormatException("Element Len is no Integer");
                objLen = (Int32)((PListInteger)lenElem).Value;
            }

            IPListElement elem = PListElementFactory.Instance.Create(typeCode, objLen);

            Byte tempElementTypeCode = CurrentElementTypeCode;
            Int32 tempElementLength = CurrentElementLength;

            CurrentElementTypeCode = typeCode;
            CurrentElementLength = objLen;

            elem.ReadBinary(this);

            CurrentElementTypeCode = tempElementTypeCode;
            CurrentElementLength = tempElementLength;

            return elem;
        }
    }
}
