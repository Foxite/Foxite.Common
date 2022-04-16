using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RestSharp.Extensions;

namespace Foxite.Common {
	public static class DependencyInjectionUtils {
		public static object ActivateType(this IServiceProvider isp, Type type) {
			ConstructorInfo[] allConstructors = type.GetConstructors(BindingFlags.Public);
			var candidateConstructors = new LinkedList<ConstructorInfo>();

			foreach (ConstructorInfo constructor in allConstructors) {
				bool usable = true;
				foreach (ParameterInfo parameter in constructor.GetParameters()) {
					// TODO find a way to test if IServiceProvider CAN provide a service without actually doing so
					object? service = isp.GetService(parameter.ParameterType);
					if (service is null) {
						usable = false;
						break;
					}
					// if service is IDisposable
					// Does that even happen? Singleton IDisposable will be disposed during shutdown, scoped ones cannot be acquired outside of a scope
					// Are you even supposed to make disposable transient services?
				}
				if (usable) {
					candidateConstructors.AddLast(constructor);
				}
			}
			ConstructorInfo chosenConstructor = candidateConstructors.MaxBy(ctor => ctor.GetParameters().Length);
			return chosenConstructor.Invoke(chosenConstructor.GetParameters().Select(param => isp.GetService(param.ParameterType)).ToArray());
		}
	}
}
