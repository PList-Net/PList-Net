using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PListNet.Test {
    class Program {
        static void Main(string[] args) {
            PListRoot root = PListRoot.Load("com.apple.springboard.plist");
            using (MemoryStream memStream =new MemoryStream()) {
                root.Save(memStream, PListFormat.Xml);
                Console.Write(Encoding.UTF8.GetString(memStream.ToArray()));
            }
            root.Save("com.apple.springboard.XML.plist", PListFormat.Xml);
            root.Save("com.apple.springboard.BIN.plist", PListFormat.Binary);

        }
    }
}
