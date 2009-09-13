using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CE.iPhone.PList {
    [global::System.Serializable]
    public class PListException : Exception {
        public PListException() { }
        public PListException(string message) : base(message) { }
        public PListException(string message, Exception inner) : base(message, inner) { }
        protected PListException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
