using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Foxite.Common.Notifications {
	public class NotificationService {
		private readonly Dictionary<Type, INotificationSender> m_Senders;
		private readonly ILogger<NotificationService> m_Logger;

		public NotificationService(ILogger<NotificationService> logger) {
			m_Logger = logger;
			m_Senders = new Dictionary<Type, INotificationSender>();
		}

		public bool AddSender(INotificationSender sender) {
			return m_Senders.TryAdd(sender.GetType(), sender);
		}

		public Task SendNotificationAsync(FormattableString message, Exception? exception = null) {
			return SendNotificationAsync(message.ToString(), exception);
		}

		public async Task SendNotificationAsync(string message, Exception? exception = null) {
			// Cannot log the notification here because it should be done using the ILogger<> of the caller.
			foreach ((Type type, INotificationSender sender) in m_Senders) {
				try {
					await sender.SendNotificationAsync(message, exception);
				} catch (Exception e) {
					m_Logger.LogCritical(e, $"Unable to send notification via sender {type.FullName}");
				}
			}
		}
	}
}
