//==================================================================================================
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
	/// Provides static methods to write diagnostics output to <see cref="System.Diagnostics.Debug"/>.
	/// </summary>
	/// <remarks>
	/// Implementation gets optimized on non-debug by not creating the debug wrapper.
	/// </remarks>
	/// <remarks>
	/// There also are <see cref="System.Diagnostics.Trace"/> variants of these methods available
	/// in <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
	/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
	/// output writers. Therefore, the two implementations use writer wrappers.
	/// </remarks>
	public static class XDebug
	{
		#if (DEBUG)
			private static DebugWrapper debugWrapper = new DebugWrapper();
		#endif

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteException(object obj, Exception ex)
		{
			WriteException(obj, ex, "");
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteException(object obj, Exception ex, string additionalMessage)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteException(debugWrapper, obj, ex, additionalMessage);
		#endif
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteStack(object obj)
		{
			WriteStack(obj, new StackTrace(), "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteStack(object obj, StackTrace st)
		{
			WriteStack(obj, st, "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteStack(object obj, StackTrace st, string additionalMessage)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteStack(debugWrapper, obj, st, additionalMessage);
		#endif
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteWindowsFormsMessage(object obj, Message m)
		{
			WriteWindowsFormsMessage(obj, m, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteWindowsFormsMessage(object obj, Message m, string additionalMessage)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteWindowsFormsMessage(debugWrapper, obj, m, additionalMessage);
		#endif
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteFileStream(object obj, FileStream fs)
		{
			WriteFileStream(obj, fs, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteFileStream(object obj, FileStream fs, string additionalMessage)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteFileStream(debugWrapper, obj, fs, additionalMessage);
		#endif
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
