using System;
using System.Collections.Generic;
using System.Text;

namespace PListNet.Internal
{
    internal class ExtendedEncoding : UTF8Encoding
    {
        public override string WebName
        {
            get { return base.WebName.ToUpper(); }
        }

        private ExtendedEncoding()
        {
        }

        public static readonly ExtendedEncoding UpperCaseUTF8 = new ExtendedEncoding();
    }
}
