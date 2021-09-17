using System.Threading.Tasks;
using Discord.Rest;
using Discord.Webhook;
using Microsoft.Extensions.Options;

namespace Foxite.Common.Notifications {
	public class DiscordNotificationSender : INotificationSender {
		private readonly DiscordWebhookClient m_Client;

		public DiscordNotificationSender(IOptions<Config> options) {
			m_Client = new DiscordWebhookClient(options.Value.WebhookUrl, options.Value.DiscordRestConfig);
		}

		public async Task SendNotificationAsync(string message) {
			static string CapLength(string input, int maxLength, bool preformat = false) {
				if (input.Length > maxLength) {
					string ret = input;
					if (preformat) {
						ret = "```\n" + ret;
					}
					ret = $"[Truncated to {maxLength}]\n{ret}";
					if (preformat) {
						maxLength -= 4;
					}
					ret = ret[..maxLength];
					if (preformat) {
						ret += "```";
					}
					return ret;
				} else {
					return input;
				}
			}

			await m_Client.SendMessageAsync(CapLength(message, 1999));
		}

		public class Config {
			public string WebhookUrl { get; set; } = null!;
			public DiscordRestConfig DiscordRestConfig { get; set; } = new DiscordRestConfig();
		}
	}
}
