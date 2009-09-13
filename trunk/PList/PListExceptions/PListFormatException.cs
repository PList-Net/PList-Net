using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CE.iPhone.PList {
    [global::System.Serializable]
    public class PListFormatException : PListException {
        public PListFormatException() { }
        public PListFormatException(string message) : base(message) { }
        public PListFormatException(string message, Exception inner) : base(message, inner) { }
        protected PListFormatException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
