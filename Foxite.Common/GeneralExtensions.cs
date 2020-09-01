using System;

namespace Foxite.Common {
	public static class GeneralExtensions {
		public static Progress<float> GetSubProgress(this IProgress<float> parent, int i, int count, float factor) =>
			new Progress<float>(p => parent.Report((float) i / count + p / count * factor));
	}
}
