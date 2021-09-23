using System.Threading.Tasks;
using DiscordWebhookLib;

namespace Foxite.Common.Notifications {
	public class DiscordWebhookLibNotificationSender : INotificationSender {
		private readonly DiscordWebhookExecutor m_Executor;
		
		public DiscordWebhookLibNotificationSender(Config config) {
			m_Executor= new DiscordWebhookExecutor(config.WebhookUrl);
		}
		
		public Task SendNotificationAsync(string message) {
			return m_Executor.Execute(DiscordStringUtil.CapLength(message, 1999));
		}

		public class Config {
			public string WebhookUrl { get; set; } = null!;
		}
	}
}
