using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Foxite.Common.Sms {
	/// <summary>
	/// SMS sender that doesn't send SMS; for testing.
	/// </summary>
	public class DummySmsSender : ISmsSender {
		private readonly ILogger<DummySmsSender> m_Logger;
		
		public DummySmsSender(ILogger<DummySmsSender> logger) {
			m_Logger = logger;
		}

		public Task SendSmsAsync(string[] recipients, string content) {
			m_Logger.LogInformation($"Not sending SMS to {string.Join(", ", recipients)}: {content}");
			return Task.CompletedTask;
		}
	}
}
