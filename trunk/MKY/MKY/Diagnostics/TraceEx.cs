//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class TraceEx
	{
	#if (TRACE)
		private static TraceWrapper traceWrapper = new TraceWrapper();
	#endif

		/// <summary>
		/// Writes location to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[Conditional("TRACE")]
		public static void WriteLocation(string message = null)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteLocation(traceWrapper, new StackTrace(), 1, message);
		#endif
		}

		/// <summary>
		/// Writes time stamp and location to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[Conditional("TRACE")]
		public static void WriteTimeStamp(string message = null)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteTimeStamp(traceWrapper, new StackTrace(), 1, message);
		#endif
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[Conditional("TRACE")]
		public static void WriteException(Type type, Exception ex, string leadMessage = null)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteExceptionLines(traceWrapper, type, ex, leadMessage);
		#endif
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[Conditional("TRACE")]
		public static void WriteStack(Type type, string leadMessage = null)
		{
			WriteStack(type, new StackTrace(), leadMessage);
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[Conditional("TRACE")]
		public static void WriteStack(Type type, StackTrace st, string leadMessage = null)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteStackLines(traceWrapper, type, st, leadMessage);
		#endif
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "m", Justification = "Naming according to parameter 'm' of NativeWindow methods.")]
		[Conditional("TRACE")]
		public static void WriteWindowsFormsMessage(Type type, Message m, string leadMessage = null)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteWindowsFormsMessageLines(traceWrapper, type, m, leadMessage);
		#endif
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[Conditional("TRACE")]
		public static void WriteFileStream(Type type, FileStream fs, string leadMessage = null)
		{
		#if (TRACE)
			DiagnosticsWriterOutput.WriteFileStreamLines(traceWrapper, type, fs, leadMessage);
		#endif
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
