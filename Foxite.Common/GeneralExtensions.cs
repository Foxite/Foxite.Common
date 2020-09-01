using System;

namespace Foxite.Common {
	/// 
	public static class GeneralExtensions {
		/// <summary>
		/// Creates a <see cref="Progress{T}"/> that represents a fraction of the progress of <paramref name="parent"/>.
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="start">Zero progress on the returned object represents this much progress on the parent progress.</param>
		/// <param name="total">The parent progress goes from zero to this value.</param>
		/// <param name="factor">A progress value of one on the returned object represents an increase of this much progress, as opposed to zero on the returned object.</param>
		/// <returns></returns>
		public static Progress<float> GetSubProgress(this IProgress<float> parent, float start, float total, float factor) =>
			new Progress<float>(p => parent.Report(start / total + p / total * factor));
	}
}
