using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Foxite.Common {
	public static class HttpClientExtensions {
		public static async Task<string> GetStringAsync(this HttpClient http, string url, bool ensureSuccess, CancellationToken cancellationToken = default) {
			using HttpResponseMessage response = await http.GetAsync(url, cancellationToken);
			if (ensureSuccess) {
				response.EnsureSuccessStatusCode();
			}
			return await response.Content.ReadAsStringAsync(cancellationToken);
		}

		public static async Task<Stream> GetStreamAsync(this HttpClient http, string url, bool ensureSuccess, CancellationToken cancellationToken = default) {
			using HttpResponseMessage response = await http.GetAsync(url, cancellationToken);
			if (ensureSuccess) {
				response.EnsureSuccessStatusCode();
			}
			return await response.Content.ReadAsStreamAsync(cancellationToken); // TODO check if the using declaration doesn't close the stream
		}
	}
}
