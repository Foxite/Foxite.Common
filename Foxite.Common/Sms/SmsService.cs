using System;
using System.Threading.Tasks;

namespace Foxite.Common.Sms {
	public class SmsService {
		private readonly ISmsSender m_Sender;
		
		public SmsService(ISmsSender sender) {
			m_Sender = sender;
		}

		public async Task SendSmsAsync(string[] recipients, string content) {
			try {
				await m_Sender.SendSmsAsync(recipients, content);
			} catch (Exception e) {
				throw new SmsException($"Could not send SMS with {content.Length} characters to recipients: {string.Join(", ", recipients)}", e);
			}
		}
	}
}
