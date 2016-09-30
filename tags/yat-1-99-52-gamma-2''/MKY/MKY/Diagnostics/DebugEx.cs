//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
	/// Provides static methods to write diagnostics output to <see cref="System.Diagnostics.Debug"/>.
	/// </summary>
	/// <remarks>
	/// Implementation gets optimized on non-debug by not creating the debug wrapper.
	/// </remarks>
	/// <remarks>
	/// There also are <see cref="System.Diagnostics.Trace"/> variants of these methods available
	/// in <see cref="TraceEx"/>.
	/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
	/// output writers. Therefore, the two implementations use writer wrappers.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class DebugEx
	{
	#if (DEBUG)
		private static DebugWrapper debugWrapper = new DebugWrapper();
	#endif

		/// <summary>
		/// Writes source, type and time stamp to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		[Conditional("DEBUG")]
		public static void WriteTimeStamp(Type type, string callerMemberName = null, string message = null)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteTimeStamp(debugWrapper, type, callerMemberName, message);
		#endif
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		[Conditional("DEBUG")]
		public static void WriteException(Type type, Exception ex, string leadMessage = null)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteException(debugWrapper, type, ex, leadMessage);
		#endif
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		[Conditional("DEBUG")]
		public static void WriteStack(Type type, string leadMessage = null)
		{
			WriteStack(type, new StackTrace(), leadMessage);
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		[Conditional("DEBUG")]
		public static void WriteStack(Type type, StackTrace st, string leadMessage = null)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteStack(debugWrapper, type, st, leadMessage);
		#endif
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "m", Justification = "Naming according to parameter 'm' of NativeWindow methods.")]
		[Conditional("DEBUG")]
		public static void WriteWindowsFormsMessage(Type type, Message m, string leadMessage = null)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteWindowsFormsMessage(debugWrapper, type, m, leadMessage);
		#endif
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		[Conditional("DEBUG")]
		public static void WriteFileStream(Type type, FileStream fs, string leadMessage = null)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteFileStream(debugWrapper, type, fs, leadMessage);
		#endif
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
