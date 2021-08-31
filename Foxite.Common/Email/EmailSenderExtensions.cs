using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Foxite.Common.Email {
	public static class EmailSenderExtensions {
		public static IEmailServiceBuilder AddEmail(this IServiceCollection services) {
			if (services == null) {
				throw new ArgumentNullException(nameof(services));
			}
			
			services.TryAddTransient<EmailService>();

			return new EmailServiceBuilder(services);
		}
		
		public static IEmailServiceBuilder AddMailkit(this IEmailServiceBuilder builder, IConfiguration config) {
			builder.Services.Configure<MailkitEmailOptions>(config.GetSection("EmailConfiguration"));

			builder.Services.TryAddTransient<IEmailSender, MailkitEmailSender>();

			return builder;
		}

		public static IEmailServiceBuilder AddDummyEmail(this IEmailServiceBuilder builder) {
			builder.Services.AddTransient<IEmailSender, DummyEmailSender>();
			return builder;
		}
	}

	public interface IEmailServiceBuilder {
		public IServiceCollection Services { get; }
	}

	public class EmailServiceBuilder : IEmailServiceBuilder {
		public IServiceCollection Services { get; }
		
		internal EmailServiceBuilder(IServiceCollection services) {
			Services = services;
		}
	}
}
