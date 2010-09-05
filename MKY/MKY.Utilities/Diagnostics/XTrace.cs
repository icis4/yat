//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MKY.Utilities.Diagnostics
{
	/// <summary>
	/// Provides static methods to write diagnostics output to <see cref="System.Diagnostics.Trace"/>.
	/// </summary>
	/// <remarks>
	/// Implementation gets optimized on non-trace by not creating the trace wrapper.
	/// </remarks>
	/// <remarks>
	/// There also are <see cref="System.Diagnostics.Debug"/> variants of these methods available
	/// in <see cref="MKY.Utilities.Diagnostics.XDebug"/>.
	/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
	/// output writers. Therefore, the two implementations use writer wrappers.
	/// </remarks>
	public static class XTrace
	{
		#if (TRACE)
			private static TraceWrapper traceWrapper = new TraceWrapper();
		#endif

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteException(object obj, Exception ex)
		{
			WriteException(obj, ex, "");
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteException(object obj, Exception ex, string additionalMessage)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteException(traceWrapper, obj, ex, additionalMessage);
		#endif
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteStack(object obj)
		{
			WriteStack(obj, new StackTrace(), "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteStack(object obj, StackTrace st)
		{
			WriteStack(obj, st, "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteStack(object obj, StackTrace st, string additionalMessage)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteStack(traceWrapper, obj, st, additionalMessage);
		#endif
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteWindowsFormsMessage(object obj, Message m)
		{
			WriteWindowsFormsMessage(obj, m, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteWindowsFormsMessage(object obj, Message m, string additionalMessage)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteWindowsFormsMessage(traceWrapper, obj, m, additionalMessage);
		#endif
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteFileStream(object obj, FileStream fs)
		{
			WriteFileStream(obj, fs, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteFileStream(object obj, FileStream fs, string additionalMessage)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteFileStream(traceWrapper, obj, fs, additionalMessage);
		#endif
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
