using System;
using Microsoft.Extensions.Logging;

namespace Foxite.Common {
	public static class LoggerUtil {
		public static void LogTrace(this ILogger logger, FormattableString formattableString) {
			LogTrace(logger, null, default, formattableString);
		}
		public static void LogTrace(this ILogger logger, Exception exception, FormattableString formattableString) {
			LogTrace(logger, exception, default, formattableString);
		}
		public static void LogTrace(this ILogger logger, EventId eventId, FormattableString formattableString) {
			LogTrace(logger, null, eventId, formattableString);
		}
		public static void LogTrace(this ILogger logger, Exception exception, EventId eventId, FormattableString formattableString) {
			logger.Log(LogLevel.Trace, eventId, formattableString, exception, (obj, ex) => obj.ToString()); // Exactly how the builtin LoggerExtensions does it
		}

		public static void LogDebug(this ILogger logger, FormattableString formattableString) {
			LogDebug(logger, null, default, formattableString);
		}
		public static void LogDebug(this ILogger logger, Exception exception, FormattableString formattableString) {
			LogDebug(logger, exception, default, formattableString);
		}
		public static void LogDebug(this ILogger logger, EventId eventId, FormattableString formattableString) {
			LogDebug(logger, null, eventId, formattableString);
		}
		public static void LogDebug(this ILogger logger, Exception exception, EventId eventId, FormattableString formattableString) {
			logger.Log(LogLevel.Debug, eventId, formattableString, exception, (obj, ex) => obj.ToString()); // Exactly how the builtin LoggerExtensions does it
		}
		
		public static void LogInformation(this ILogger logger, FormattableString formattableString) {
			LogInformation(logger, null, default, formattableString);
		}
		public static void LogInformation(this ILogger logger, Exception exception, FormattableString formattableString) {
			LogInformation(logger, exception, default, formattableString);
		}
		public static void LogInformation(this ILogger logger, EventId eventId, FormattableString formattableString) {
			LogInformation(logger, null, eventId, formattableString);
		}
		public static void LogInformation(this ILogger logger, Exception exception, EventId eventId, FormattableString formattableString) {
			logger.Log(LogLevel.Information, eventId, formattableString, exception, (obj, ex) => obj.ToString()); // Exactly how the builtin LoggerExtensions does it
		}
		
		public static void LogWarning(this ILogger logger, FormattableString formattableString) {
			LogWarning(logger, null, default, formattableString);
		}
		public static void LogWarning(this ILogger logger, Exception exception, FormattableString formattableString) {
			LogWarning(logger, exception, default, formattableString);
		}
		public static void LogWarning(this ILogger logger, EventId eventId, FormattableString formattableString) {
			LogWarning(logger, null, eventId, formattableString);
		}
		public static void LogWarning(this ILogger logger, Exception exception, EventId eventId, FormattableString formattableString) {
			logger.Log(LogLevel.Warning, eventId, formattableString, exception, (obj, ex) => obj.ToString()); // Exactly how the builtin LoggerExtensions does it
		}
		
		public static void LogError(this ILogger logger, FormattableString formattableString) {
			LogError(logger, null, default, formattableString);
		}
		public static void LogError(this ILogger logger, Exception exception, FormattableString formattableString) {
			LogError(logger, exception, default, formattableString);
		}
		public static void LogError(this ILogger logger, EventId eventId, FormattableString formattableString) {
			LogError(logger, null, eventId, formattableString);
		}
		public static void LogError(this ILogger logger, Exception exception, EventId eventId, FormattableString formattableString) {
			logger.Log(LogLevel.Error, eventId, formattableString, exception, (obj, ex) => obj.ToString()); // Exactly how the builtin LoggerExtensions does it
		}
		
		public static void LogCritical(this ILogger logger, FormattableString formattableString) {
			LogCritical(logger, null, default, formattableString);
		}
		public static void LogCritical(this ILogger logger, Exception exception, FormattableString formattableString) {
			LogCritical(logger, exception, default, formattableString);
		}
		public static void LogCritical(this ILogger logger, EventId eventId, FormattableString formattableString) {
			LogCritical(logger, null, eventId, formattableString);
		}
		public static void LogCritical(this ILogger logger, Exception exception, EventId eventId, FormattableString formattableString) {
			logger.Log(LogLevel.Critical, eventId, formattableString, exception, (obj, ex) => obj.ToString()); // Exactly how the builtin LoggerExtensions does it
		}
		
	}
}
