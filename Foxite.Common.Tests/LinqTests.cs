using System.Linq;
using NUnit.Framework;

namespace Foxite.Common.Tests;

public class LinqTests {
	[Test]
	public void ShuffleTest1() {
		int[] left = new int[1000];
		int[] right = new int[1000];
		for (int i = 0; i < left.Length; i++) {
			left[i] = i;
			right[i] = i;
		}

		left.Shuffle();
		int matches = 0;
		for (int i = 0; i < left.Length; i++) {
			if (left[i] == right[i]) {
				matches++;
			}
		}

		// Has a chance to fail, I can't be arsed to figure it out but it is VERY low.
		if (matches > left.Length * 0.05) {
			Assert.Fail("Too many matches");
		}
	}

	[Test]
	public void ShuffleTest2() {
		const int Tries = 50;
		bool startPass = false;
		bool endPass = false;
		for (int i = 0; i < Tries; i++) {
			int[] left = new int[1000];
			int[] right = new int[1000];
			for (int j = 0; j < left.Length; j++) {
				left[j] = j;
				right[j] = j;
			}

			left.Shuffle();
			if (left[0] != right[0]) {
				startPass = true;
			}

			if (left[^1] != right[^1]) {
				endPass = true;
			}

			if (startPass && endPass) {
				Assert.Pass();
			}
		}

		if (startPass) { // && !endPass
			Assert.Fail("Shuffle did not change the last item in the list within {0} tries", Tries);
		} else { // !startPass && endPass
			Assert.Fail("Shuffle did not change the first item in the list within {0} tries", Tries);
		}
	}
}
