using System.Threading.Tasks;

namespace Foxite.Common.Sms {
	public interface ISmsSender {
		public Task SendSmsAsync(string number, string content);
	}
}
