using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MKY.Utilities.Diagnostics
{
	/// <summary>
	/// Provides static methods to write diagnostics output to <see cref="System.Diagnostics.Trace"/>.
	/// </summary>
	public static class XTrace
	{
		private static TraceWrapper _traceWrapper = new TraceWrapper();

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		/// <remarks>
		/// There also is a <see cref="System.Diagnostics.Debug"/> variant of this method available
		/// in <see cref="MKY.Utilities.Diagnostics.XDebug"/>.
		/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
		/// output writers. Therefore, the two implementations use writer wrappers.
		/// </remarks>
		[Conditional("TRACE")]
		public static void WriteException(object obj, Exception ex)
		{
			DiagnosticsWriterOutput.WriteException(_traceWrapper, obj, ex);
		}
	}
}
