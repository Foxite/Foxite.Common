using System;
using System.Threading.Tasks;

namespace Foxite.Common.Notifications {
	public interface INotificationSender {
		public Task SendNotificationAsync(string message, Exception? exception);
	}
}
