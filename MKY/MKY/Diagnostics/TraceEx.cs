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

namespace MKY.Diagnostics
{
	/// <summary>
	/// Provides static methods to write diagnostics output to <see cref="System.Diagnostics.Trace"/>.
	/// </summary>
	/// <remarks>
	/// Implementation gets optimized on non-trace by not creating the trace wrapper.
	/// </remarks>
	/// <remarks>
	/// There also are <see cref="System.Diagnostics.Debug"/> variants of these methods available
	/// in <see cref="DebugEx"/>.
	/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
	/// output writers. Therefore, the two implementations use writer wrappers.
	/// </remarks>
	public static class TraceEx
	{
	#if (TRACE)
		private static TraceWrapper traceWrapper = new TraceWrapper();
	#endif

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteException(Type type, Exception ex)
		{
			WriteException(type, ex, "");
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteException(Type type, Exception ex, string additionalMessage)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteException(traceWrapper, type, ex, additionalMessage);
		#endif
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteStack(Type type)
		{
			WriteStack(type, new StackTrace(), "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteStack(Type type, StackTrace st)
		{
			WriteStack(type, st, "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteStack(Type type, StackTrace st, string additionalMessage)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteStack(traceWrapper, type, st, additionalMessage);
		#endif
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteWindowsFormsMessage(Type type, Message m)
		{
			WriteWindowsFormsMessage(type, m, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteWindowsFormsMessage(Type type, Message m, string additionalMessage)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteWindowsFormsMessage(traceWrapper, type, m, additionalMessage);
		#endif
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteFileStream(Type type, FileStream fs)
		{
			WriteFileStream(type, fs, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[Conditional("TRACE")]
		public static void WriteFileStream(Type type, FileStream fs, string additionalMessage)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteFileStream(traceWrapper, type, fs, additionalMessage);
		#endif
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
