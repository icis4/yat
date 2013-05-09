//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.10
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
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
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteException(Type type, Exception ex)
		{
			WriteException(type, ex, "");
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteException(Type type, Exception ex, string leadMessage)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteException(debugWrapper, type, ex, leadMessage);
		#endif
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteStack(Type type)
		{
			WriteStack(type, new StackTrace(), "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteStack(Type type, string leadMessage)
		{
			WriteStack(type, new StackTrace(), leadMessage);
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteStack(Type type, StackTrace st)
		{
			WriteStack(type, st, "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteStack(Type type, StackTrace st, string leadMessage)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteStack(debugWrapper, type, st, leadMessage);
		#endif
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "m", Justification = "Naming according to parameter 'm' of NativeWindow methods.")]
		[Conditional("DEBUG")]
		public static void WriteWindowsFormsMessage(Type type, Message m)
		{
			WriteWindowsFormsMessage(type, m, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "m", Justification = "Naming according to parameter 'm' of NativeWindow methods.")]
		[Conditional("DEBUG")]
		public static void WriteWindowsFormsMessage(Type type, Message m, string leadMessage)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteWindowsFormsMessage(debugWrapper, type, m, leadMessage);
		#endif
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteFileStream(Type type, FileStream fs)
		{
			WriteFileStream(type, fs, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		[Conditional("DEBUG")]
		public static void WriteFileStream(Type type, FileStream fs, string leadMessage)
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
