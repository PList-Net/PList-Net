using System;

namespace PListNet.Exceptions
{
	/// <summary>
	/// PListNet exception.
	/// </summary>
	[global::System.Serializable]
	public class PListException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PListNet.Exceptions.PListException"/> class.
		/// </summary>
		public PListException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PListNet.Exceptions.PListException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		public PListException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PListNet.Exceptions.PListException"/> class.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="inner">Inner.</param>
		public PListException(string message, Exception inner) : base(message, inner)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PListNet.Exceptions.PListException"/> class.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
		protected PListException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
	}
}
