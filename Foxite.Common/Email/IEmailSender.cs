using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foxite.Common.Email {
	public interface IEmailSender {
		Task SendEmailAsync(string email, string subject, string htmlMessage, IDictionary<string, string> additionalHeaders = null);
	}
}
