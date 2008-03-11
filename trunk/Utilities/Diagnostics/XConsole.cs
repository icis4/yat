using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Diagnostics
{
	/// <summary>
	/// Provides static methods to write diagnostics output to <see cref="System.Console"/>.
	/// </summary>
	public static class XConsole
	{
		private static ConsoleWrapper _consoleWrapper = new ConsoleWrapper();

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteException(object obj, Exception ex)
		{
			DiagnosticsWriterOutput.WriteException(_consoleWrapper, obj, ex);
		}

		/// <summary>
		/// Writes message and stack to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteStack(object obj, string message)
		{
			DiagnosticsWriterOutput.WriteStack(_consoleWrapper, obj, message);
		}
	}
}
