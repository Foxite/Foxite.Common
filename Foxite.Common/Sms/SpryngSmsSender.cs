using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Foxite.Common.Sms {
	// Would have used a library for this, but the only one on Nuget uses the HTTP api, and we're using the rest api
	public class SpryngSmsSender : ISmsSender {
		private readonly HttpClient m_Http;
		private readonly string m_ApiKey;
		private readonly string m_Originator;
		private readonly string m_Route;
		private readonly string m_EndpointUrl;

		public SpryngSmsSender(HttpClient http, IOptions<SpryngOptions> optionsAccessor) {
			m_Http = http;
			
			SpryngOptions options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
			m_ApiKey = options.ApiKey;
			m_Originator = options.Originator;
			m_Route = options.Route;
			m_EndpointUrl = options.EndpointUrl;
		}
		
		public async Task SendSmsAsync(string[] recipients, string content) {
			var result = await m_Http.SendAsync(new HttpRequestMessage(HttpMethod.Post, m_EndpointUrl) {
				Headers = {
					Authorization = new AuthenticationHeaderValue("Bearer", m_ApiKey)
				},
				Content = JsonContent.Create(new {
					encoding = "auto",
					body = content,
					originator = m_Originator,
					route = m_Route,
					recipients = recipients
				})
			});

			if (!result.IsSuccessStatusCode) {
				string response = await result.Content.ReadAsStringAsync();

				throw new SmsException($"Request failed with status code {result.StatusCode} and body: {response}");
			}
		}
	}

	public class SpryngOptions {
		// Dont turn this into a constructor with parameters because then DI wont be able to construct it
		public string ApiKey { get; set; } = null!;
		public string Originator { get; set; } = null!;
		public string Route { get; set; } = null!;
		public string EndpointUrl { get; set; } = "https://rest.spryngsms.com/v1/messages";
	}
}
