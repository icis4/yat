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
using System.Windows.Forms;

namespace MKY.Utilities.Diagnostics
{
	/// <summary>
	/// Provides static methods to write diagnostics output to <see cref="System.Diagnostics.Debug"/>.
	/// </summary>
	/// <remarks>
	/// Implementation gets optimized on non-debug by not creating the debug wrapper.
	/// </remarks>
	public static class XDebug
	{
		#if (DEBUG)
			private static DebugWrapper _debugWrapper = new DebugWrapper();
		#endif

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		/// <remarks>
		/// There also is a <see cref="System.Diagnostics.Trace"/> variant of this method available
		/// in <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
		/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
		/// output writers. Therefore, the two implementations use writer wrappers.
		/// </remarks>
		[Conditional("DEBUG")]
		public static void WriteException(object obj, Exception ex)
		{
			WriteException(obj, ex, "");
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		/// <remarks>
		/// There also is a <see cref="System.Diagnostics.Trace"/> variant of this method available
		/// in <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
		/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
		/// output writers. Therefore, the two implementations use writer wrappers.
		/// </remarks>
		[Conditional("DEBUG")]
		public static void WriteException(object obj, Exception ex, string additionalMessage)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteException(_debugWrapper, obj, ex, additionalMessage);
		#endif
		}

		/// <summary>
		/// Writes message and stack to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		/// <remarks>
		/// There also is a <see cref="System.Diagnostics.Trace"/> variant of this method available
		/// in <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
		/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
		/// output writers. Therefore, the two implementations use writer wrappers.
		/// </remarks>
		[Conditional("DEBUG")]
		public static void WriteStack(object obj, StackTrace st)
		{
			WriteStack(obj, st, "");
		}

		/// <summary>
		/// Writes message and stack to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		/// <remarks>
		/// There also is a <see cref="System.Diagnostics.Trace"/> variant of this method available
		/// in <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
		/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
		/// output writers. Therefore, the two implementations use writer wrappers.
		/// </remarks>
		[Conditional("DEBUG")]
		public static void WriteStack(object obj, StackTrace st, string additionalMessage)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteStack(_debugWrapper, obj, st, additionalMessage);
		#endif
		}

		/// <summary>
		/// Writes a windows forms message to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		/// <remarks>
		/// There also is a <see cref="System.Diagnostics.Trace"/> variant of this method available
		/// in <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
		/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
		/// output writers. Therefore, the two implementations use writer wrappers.
		/// </remarks>
		[Conditional("DEBUG")]
		public static void WriteWindowsFormsMessage(object obj, Message m)
		{
			WriteWindowsFormsMessage(obj, m, "");
		}

		/// <summary>
		/// Writes a windows forms message to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		/// <remarks>
		/// There also is a <see cref="System.Diagnostics.Trace"/> variant of this method available
		/// in <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
		/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
		/// output writers. Therefore, the two implementations use writer wrappers.
		/// </remarks>
		[Conditional("DEBUG")]
		public static void WriteWindowsFormsMessage(object obj, Message m, string additionalMessage)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteWindowsFormsMessage(_debugWrapper, obj, m, additionalMessage);
		#endif
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
