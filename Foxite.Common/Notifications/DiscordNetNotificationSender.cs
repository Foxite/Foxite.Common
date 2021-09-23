/* // Archived - DNET causes a problem to anyone using Entity Framework (https://github.com/dotnet/efcore/issues/18124)
   // Decided to use a more lightweight webhooks library.using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Foxite.Common.Notifications {
	public class DiscordNetNotificationSender : INotificationSender {
		private readonly DiscordWebhookClient m_Client;

		public DiscordNetNotificationSender(IOptions<Config> options) {
			m_Client = new DiscordWebhookClient(options.Value.WebhookUrl, options.Value.DiscordRestConfig);
		}

		public async Task SendNotificationAsync(string message) {
			await m_Client.SendMessageAsync(DiscordStringUtil.CapLength(message, 1999));
		}

		public class Config {
			public string WebhookUrl { get; set; } = null!;
			public DiscordRestConfig DiscordRestConfig { get; set; } = new DiscordRestConfig();
		}
	}
}
//*/
