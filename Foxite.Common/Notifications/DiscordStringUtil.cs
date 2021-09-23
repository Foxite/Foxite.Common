namespace Foxite.Common.Notifications {
	public static class DiscordStringUtil {
		public static string CapLength(string input, int maxLength, bool preformat = false) {
			if (input.Length > maxLength) {
				string ret = input;
				if (preformat) {
					ret = "```\n" + ret;
				}
				ret = $"[Truncated to {maxLength}]\n{ret}";
				if (preformat) {
					maxLength -= 4;
				}
				ret = ret[..maxLength];
				if (preformat) {
					ret += "```";
				}
				return ret;
			} else {
				return input;
			}
		}
	}
}
