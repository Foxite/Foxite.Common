using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.OptionsModel;
using MimeKit;
using MimeKit.Text;

namespace Foxite.Common.Email {
	public class MailkitEmailSender : IEmailSender {
		private readonly MailkitEmailOptions m_Options;

		public MailkitEmailSender(IOptions<MailkitEmailOptions> optionsAccessor) {
			m_Options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage, IDictionary<string, string> headers = null) {
			var msg = new MimeMessage();

			if (headers != null) {
				foreach (KeyValuePair<string, string> kvp in headers) {
					msg.Headers[kvp.Key] = kvp.Value;
				}
			}

			msg.From.Add(new MailboxAddress(m_Options.SenderDisplayName, m_Options.SenderAddress));
			msg.Subject = subject;
			msg.Body = new TextPart(TextFormat.Html) { Text = htmlMessage ?? "" };

			msg.To.Add(InternetAddress.Parse(email));

			using var client = new SmtpClient();
			try {
				await client.ConnectAsync(m_Options.SmtpServerAddress, m_Options.SmtpServerPort, m_Options.EnableSsl);
				await client.AuthenticateAsync(m_Options.SenderAccount, m_Options.SenderPassword);

				await client.SendAsync(msg);
			} finally {
				await client.DisconnectAsync(true);
			}
		}
	}

	public class MailkitEmailOptions {
		public string SmtpServerAddress { get; set; }
		public int SmtpServerPort { get; set; } = 465;
		public bool EnableSsl { get; set; } = true;
		public string SenderAccount { get; set; }
		public string SenderPassword { get; set; }
		public string SenderDisplayName { get; set; }
		public string SenderAddress { get; set; }
	}
}