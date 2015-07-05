using System;

namespace PListNet.Exceptions {
	/// <summary>
	/// PListNet format exception.
	/// </summary>
    [global::System.Serializable]
    public class PListFormatException : PListException {
		/// <summary>
		/// Initializes a new instance of the <see cref="PListNet.Exceptions.PListFormatException"/> class.
		/// </summary>
        public PListFormatException() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="PListNet.Exceptions.PListFormatException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		public PListFormatException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="PListNet.Exceptions.PListFormatException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="inner">Inner.</param>
		public PListFormatException(string message, Exception inner) : base(message, inner) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="PListNet.Exceptions.PListFormatException"/> class.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
        protected PListFormatException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
