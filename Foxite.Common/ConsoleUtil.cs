using System;
using System.Collections.Generic;

namespace Foxite.Common {
	/// <summary>
	/// Utilities for console programs.
	/// </summary>
	public static class ConsoleUtil {
		/// <summary>
		/// Presents the user with a list of options and returns the zero-indexed number of the option they chose.
		/// </summary>
		public static int ConsoleChoiceMenu(string question, params string[] options) => ConsoleChoiceMenu(question, options);
		
		/// <summary>
		/// Presents the user with a list of options and returns the zero-indexed number of the option they chose.
		/// </summary>
		public static int ConsoleChoiceMenu(string question, IReadOnlyList<string> options) {
			Console.WriteLine(question);
			for (int i = 0; i < options.Count; i++) {
				Console.WriteLine($"[{i + 1}] {options[i]}");
			}

			int choice;
			while (!(int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= options.Count)) {
				Console.WriteLine("Enter the number of the option you want.");
			}

			return choice - 1;
		}
	}
}
