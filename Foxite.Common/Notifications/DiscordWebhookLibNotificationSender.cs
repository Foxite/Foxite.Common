using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DiscordWebhookLib;
using Microsoft.Extensions.Options;

namespace Foxite.Common.Notifications {
	public class DiscordWebhookLibNotificationSender : INotificationSender {
		private readonly DiscordWebhookExecutor m_Executor;
		
		public DiscordWebhookLibNotificationSender(IOptions<Config> config) {
			m_Executor = new DiscordWebhookExecutor(config.Value.WebhookUrl);
		}
		
		public Task SendNotificationAsync(string message, Exception? exception) {
			if (exception != null) {
				message += "\n```csharp\n" + exception.ToStringDemystified();
				message = DiscordStringUtil.CapLength(message, 1996);
				message += "```";
			} else {
				message = DiscordStringUtil.CapLength(message, 1999);
			}
			
			return m_Executor.Execute(message);
		}

		public class Config {
			public string WebhookUrl { get; set; } = null!;
		}
	}
}
