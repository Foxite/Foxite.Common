using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foxite.Common.Email {
	public class EmailService {
		private readonly IEmailSender m_EmailSender;
		
		public EmailService(IEmailSender emailSender) {
			m_EmailSender = emailSender;
		}

		public async Task SendEmailAsync(string recipient, string subject, string htmlMessage, IDictionary<string, string>? additionalHeaders = null) {
			try {
				await m_EmailSender.SendEmailAsync(recipient, subject, htmlMessage, additionalHeaders);
			} catch (Exception e) {
				throw new EmailException($"Could not send an email to {recipient}", e);
			}
		}
	}
}
