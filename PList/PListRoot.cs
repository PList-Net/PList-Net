/* =================================================================================
 * File:   PListRoot.cs
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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PListNet.Internal;

namespace PListNet {
    /// <summary>
    /// Represents a PList File
    /// </summary>
    [XmlRoot("plist")]
    public class PListRoot : IXmlSerializable {

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PListRoot"/> is stored in binary format.
        /// </summary>
        /// <value><c>true</c> if stored in binary format; otherwise, <c>false</c>.</value>
        public PListFormat Format { get; set; }

        /// <summary>
        /// Loads the PList from specified file.
        /// </summary>
        /// <param name="fileName">The path of the PList.</param>
        /// <returns>A <see cref="PListRoot"/> object loaded from the file</returns>
        public static PListRoot Load(String fileName) {
            using (FileStream fs = new FileStream(fileName, FileMode.Open)) {
                return Load(fs);
            }
        }


        /// <summary>
        /// Loads the PList from specified stream.
        /// </summary>
        /// <param name="stream">The stream containing the PList.</param>
        /// <returns>A <see cref="PListRoot"/> object loaded from the stream</returns>
        public static PListRoot Load(Stream stream) {
            PListRoot root= null;
            Byte[] buf = new Byte[8];
            stream.Read(buf, 0, buf.Length);
            stream.Seek(0, SeekOrigin.Begin);
            if (Encoding.Default.GetString(buf) == "bplist00") {
                PListBinaryReader reader = new PListBinaryReader();
                root = new PListRoot();
                root.Format = PListFormat.Binary;
                root.Root = reader.Read(stream);
            } else { 
				// set resolver to null in order to avoid calls to apple.com to resolve DTD
				var settings = new XmlReaderSettings {
					XmlResolver = null
				};
				using (var reader = XmlReader.Create(stream, settings)) {
					var serializer = new XmlSerializer(typeof(PListRoot));
					root = (PListRoot) serializer.Deserialize(reader);
					root.Format = PListFormat.Xml;
				}
            }

            return root;
        }

        /// <summary>
        /// Saves the PList to the specified path.
        /// </summary>
        /// <param name="fileName">The path of the PList.</param>
        /// <param name="format">The format of the PList (Binary/Xml).</param>
        public void Save(String fileName, PListFormat format){
            using (FileStream fs = new FileStream(fileName, FileMode.Create)) {
                Save(fs, format);                
            }
        }

        /// <summary>
        /// Saves the PList to the specified path.
        /// </summary>
        /// <param name="fileName">The path of the PList.</param>
        public void Save(String fileName) {
            Save(fileName, Format);
        }

        /// <summary>
        /// Saves the PList to the specified stream.
        /// </summary>
        /// <param name="stream">The stream in which the PList is saves.</param>
        public void Save(Stream stream) {
            Save(stream, Format);
        }

        /// <summary>
        /// Saves the PList to the specified stream.
        /// </summary>
        /// <param name="stream">The stream in which the PList is saves.</param>
        /// <param name="format">The format of the PList (Binary/Xml).</param>
        public void Save(Stream stream, PListFormat format) {
            if (format == PListFormat.Xml) {
                XmlWriterSettings sets = new XmlWriterSettings();
                sets.Encoding = Encoding.UTF8;
                sets.Indent = true;
                sets.IndentChars = "\t";
                sets.NewLineChars = "\n";

                XmlWriter xmlWriter = XmlTextWriter.Create(stream, sets);

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteDocType("plist", "-//Apple Computer//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);

                WriteXml(xmlWriter);
                xmlWriter.Flush();
            } else {
                PListBinaryWriter writer = new PListBinaryWriter();
                writer.Write(stream, Root);
            }
        }

        /// <summary>
        /// Gets or sets the root PList-Element.
        /// </summary>
        /// <value>The root PList-Element.</value>
        public IPListElement Root { get; set; }

        #region IXmlSerializable Members

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, 
        /// you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a 
        /// custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> 
        /// to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is
        /// produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> 
        /// method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/>
        /// method.
        /// </returns>
        public XmlSchema GetSchema() { return null; }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader) {

            reader.ReadStartElement("plist");

            Root = PListElementFactory.Instance.Create(reader.LocalName);
            Root.ReadXml(reader);

            reader.ReadEndElement();
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("plist");
            writer.WriteAttributeString("version", "1.0");
            if (Root != null) Root.WriteXml(writer);

            writer.WriteEndElement();
        }

        #endregion
    }
}
