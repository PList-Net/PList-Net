/* =================================================================================
 * File:   PListElementFactory.cs
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

namespace CE.iPhone.PList.Internal {
    /// <summary>
    /// Singleton class which generates concrete <see cref="T:CE.iPhone.IPListElement"/> from the Tag or TypeCode
    /// </summary>
    internal class PListElementFactory {
        private static PListElementFactory s_Instance;
        private Dictionary<String, Type> m_PListElementTags = new Dictionary<String, Type>();
        private Dictionary<Byte, Type> m_PListElementTypeCodes = new Dictionary<Byte, Type>();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static PListElementFactory Instance {
            get {
                if (s_Instance == null) s_Instance = new PListElementFactory();
                return s_Instance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PListElementFactory"/> class.
        /// </summary>
        private PListElementFactory() {
            Register(new PListDict());
            Register(new PListInteger());
            Register(new PListReal());
            Register(new PListString());
            Register(new PListArray());
            Register(new PListData());
            Register(new PListDate());

            Register("string", 5, new PListString());
            Register("ustring", 6, new PListString());

            Register("true", 0, new PListBool());
            Register("false", 0, new PListBool());
        }

        /// <summary>
        /// Registers the specified element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        private void Register<T>(T element) where T : IPListElement, new() {
            if (!m_PListElementTags.ContainsKey(element.Tag)) m_PListElementTags.Add(element.Tag, element.GetType());
            if (!m_PListElementTypeCodes.ContainsKey(element.TypeCode)) m_PListElementTypeCodes.Add(element.TypeCode, element.GetType());
        }
       
        /// <summary>
        /// Registers the specified element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag">The tag.</param>
        /// <param name="typeCode">The type code.</param>
        /// <param name="element">The element.</param>
        private void Register<T>(String tag, Byte typeCode, T element) where T : IPListElement, new() {
            if (!m_PListElementTags.ContainsKey(tag)) m_PListElementTags.Add(tag, element.GetType());
            if (!m_PListElementTypeCodes.ContainsKey(typeCode)) m_PListElementTypeCodes.Add(typeCode, element.GetType());
        }

        /// <summary>
        /// Creates a concrete <see cref="T:CE.iPhone.IPListElement"/> object secified specified by it's typecode.
        /// </summary>
        /// <param name="typeCode">The typecode of the element.</param>
        /// <param name="length">The length of the element 
        /// (required only for <see cref="T:CE.iPhone.PListBool"/>, <see cref="T:CE.iPhone.PListNull"/> and <see cref="T:CE.iPhone.PListFill"/>).</param>
        /// <returns>The created <see cref="T:CE.iPhone.IPListElement"/> object</returns>
        public IPListElement Create(Byte typeCode, Int32 length) {
            if (typeCode == 0 && length == 0x00) return new PListNull();
            if (typeCode == 0 && length == 0x0F) return new PListFill();

            if (m_PListElementTypeCodes.ContainsKey(typeCode))
                return (IPListElement)Activator.CreateInstance(m_PListElementTypeCodes[typeCode]);            
            else
                throw new PListFormatException(string.Format("Unknown PList - TypeCode ({0})", typeCode));
        }

        /// <summary>        
        /// Creates a concrete <see cref="T:CE.iPhone.IPListElement"/> object secified specified by it's tag.
        /// </summary>
        /// <param name="tag">The tag of the element.</param>
        /// <returns>The created <see cref="T:CE.iPhone.IPListElement"/> object</returns>
        public IPListElement Create(String tag) {
            if (m_PListElementTags.ContainsKey(tag))
                return (IPListElement)Activator.CreateInstance(m_PListElementTags[tag]);  
            else
                throw new PListFormatException(string.Format("Unknown PList - Tag ({0})", tag));
        }

        /// <summary>
        /// Creates a <see cref="T:CE.iPhone.IPListElement"/> object used for exteded length information.
        /// </summary>
        /// <param name="length">The exteded length information.</param>
        /// <returns>The <see cref="T:CE.iPhone.IPListElement"/> object used for exteded length information.</returns>
        public IPListElement CreateLengthElement(int length) {
            return new PListInteger(length);
        }

        /// <summary>
        /// Creates a <see cref="T:CE.iPhone.IPListElement"/> object used for dictionary keys.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="T:CE.iPhone.IPListElement"/> object used for dictionary keys.</returns>
        public IPListElement CreateKeyElement(String key) {
            return new PListString(key);
        }
    }
}
