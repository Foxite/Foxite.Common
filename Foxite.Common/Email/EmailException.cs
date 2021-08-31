using System;
using System.Runtime.Serialization;

namespace Foxite.Common.Email {
	public class EmailException : Exception {
		public EmailException() { }
		protected EmailException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public EmailException(string message) : base(message) { }
		public EmailException(string message, Exception innerException) : base(message, innerException) { }
	}
}
