using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Foxite.Common.Email {
	public class DummyEmailSender : IEmailSender {
		private readonly ILogger<DummyEmailSender> m_Logger;
		
		public DummyEmailSender(ILogger<DummyEmailSender> logger) {
			m_Logger = logger;
		}

		public Task SendEmailAsync(string email, string subject, string htmlMessage, IDictionary<string, string>? headers) {
			string message = $"Not sending email to {email} with subject {subject}:\n";

			if (headers != null) {
				message += string.Join('\n', headers.Select(kvp => $"{kvp.Key}: {kvp.Value}")) + "\n\n";
			}

			message += htmlMessage;
			m_Logger.LogInformation(message);
			return Task.CompletedTask;
		}
	}
}
