using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Foxite.Common.Sms {
	public static class SmsExtensions {
		public static ISmsServiceBuilder AddSms(this IServiceCollection services) {
			if (services == null) {
				throw new ArgumentNullException(nameof(services));
			}
			
			services.TryAddTransient<SmsService>();

			return new SmsServiceBuilder(services);
		}
		
		public static ISmsServiceBuilder AddSpryng(this ISmsServiceBuilder builder, IConfiguration config) {
			builder.Services.Configure<SpryngOptions>(config.GetSection("SmsConfiguration"));

			builder.Services.TryAddTransient<ISmsSender, SpryngSmsSender>();

			return builder;
		}

		public static ISmsServiceBuilder AddDummySms(this ISmsServiceBuilder builder) {
			builder.Services.AddTransient<ISmsSender, DummySmsSender>();
			return builder;
		}
	}

	public interface ISmsServiceBuilder {
		public IServiceCollection Services { get; }
	}

	public class SmsServiceBuilder : ISmsServiceBuilder {
		public IServiceCollection Services { get; }
		
		internal SmsServiceBuilder(IServiceCollection services) {
			Services = services;
		}
	}
}
