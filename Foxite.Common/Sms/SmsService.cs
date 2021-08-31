using System;
using System.Threading.Tasks;

namespace Foxite.Common.Sms {
	public class SmsService {
		private readonly ISmsSender m_Sender;
		
		public SmsService(ISmsSender sender) {
			m_Sender = sender;
		}

		public async Task SendSmsAsync(string number, string content) {
			try {
				await m_Sender.SendSmsAsync(number, content);
			} catch (Exception e) {
				throw new SmsException($"Could not send SMS with {content.Length} characters to number: {number}", e);
			}
		}
	}
}
