using System.Threading.Tasks;
using DiscordWebhookLib;
using Microsoft.Extensions.Options;

namespace Foxite.Common.Notifications {
	public class DiscordWebhookLibNotificationSender : INotificationSender {
		private readonly DiscordWebhookExecutor m_Executor;
		
		public DiscordWebhookLibNotificationSender(IOptions<Config> config) {
			m_Executor = new DiscordWebhookExecutor(config.Value.WebhookUrl);
		}
		
		public Task SendNotificationAsync(string message) {
			return m_Executor.Execute(DiscordStringUtil.CapLength(message, 1999));
		}

		public class Config {
			public string WebhookUrl { get; set; } = null!;
		}
	}
}
