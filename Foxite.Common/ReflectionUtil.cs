using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Foxite.Common {
	public static class ReflectionUtil {
		public static IReadOnlyList<T> GetCustomAttributes<T>(this ICustomAttributeProvider icap, bool inherit) => icap.GetCustomAttributes(typeof(T), inherit).ListCast<T>()!;
	}
}
