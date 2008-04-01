using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MKY.Utilities.Diagnostics
{
	/// <summary>
	/// Provides static methods to write diagnostics output to any <see cref="System.IO.TextWriter"/>.
	/// </summary>
	public static class AnyWriter
	{
		private static AnyWriterWrapper _anyWriterWrapper = new AnyWriterWrapper();

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to the given <see cref="System.IO.TextWriter"/>.
		/// </summary>
		public static void WriteException(TextWriter writer, object obj, Exception ex)
		{
			_anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteException(_anyWriterWrapper, obj, ex);
			_anyWriterWrapper.SetWriter(null);
		}
	}
}
