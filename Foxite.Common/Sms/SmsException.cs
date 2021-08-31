using System;
using System.Runtime.Serialization;

namespace Foxite.Common.Sms {
	public class SmsException : Exception {
		public SmsException() { }
		protected SmsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public SmsException(string message) : base(message) { }
		public SmsException(string message, Exception innerException) : base(message, innerException) { }
	}
}
