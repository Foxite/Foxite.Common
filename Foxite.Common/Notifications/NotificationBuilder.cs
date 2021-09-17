using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Foxite.Common.Notifications;

namespace Microsoft.Extensions.DependencyInjection {
	public static class NotificationUtil {
		public static INotificationBuilder AddNotifications(this IServiceCollection services) {
			var ret = new NotificationBuilder(services);
			services.AddSingleton(isp => ret.Build(isp));
			return ret;
		}

		public static INotificationBuilder AddDiscord(this INotificationBuilder builder, IConfiguration discordConfiguration) {
			Console.WriteLine(string.Join('\n', discordConfiguration.AsEnumerable().Select(kvp => kvp.ToString())));
			builder.Services.Configure<DiscordNotificationSender.Config>(discordConfiguration);
			builder.AddSender<DiscordNotificationSender>();
			return builder;
		}
	}
}

namespace Foxite.Common.Notifications {
	public interface INotificationBuilder {
		public IServiceCollection Services { get; }

		public void AddSender<TSender>() where TSender : INotificationSender => AddSender(typeof(TSender));
		public void AddSender(Type sender);
	}
	
	public class NotificationBuilder : INotificationBuilder {
		private readonly HashSet<Type> m_Senders;
		
		public IServiceCollection Services { get; }

		public NotificationBuilder(IServiceCollection services) {
			Services = services;
			m_Senders = new HashSet<Type>();
		}

		public void AddSender(Type sender) {
			if (!typeof(INotificationSender).IsAssignableFrom(sender)) {
				throw new ArgumentException("sender must derive from " + nameof(INotificationSender), nameof(sender));
			}
			
			m_Senders.Add(sender);
			Services.AddSingleton(sender);
		}
		
		public NotificationService Build(IServiceProvider isp) {
			var ret = new NotificationService(isp.GetRequiredService<ILogger<NotificationService>>());
			foreach (Type type in m_Senders) {
				ret.AddSender((INotificationSender) isp.GetRequiredService(type));
			}
			return ret;
		}
	}
}
