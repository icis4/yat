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
	/// Provides static methods to write diagnostics output to <see cref="System.Console"/>.
	/// </summary>
	public static class XConsole
	{
		private static ConsoleWrapper consoleWrapper = new ConsoleWrapper();

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteException(object obj, Exception ex)
		{
			WriteException(obj, ex, "");
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteException(object obj, Exception ex, string additionalMessage)
		{
			DiagnosticsWriterOutput.WriteException(consoleWrapper, obj, ex, additionalMessage);
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteStack(object obj)
		{
			WriteStack(obj, new StackTrace(), "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteStack(object obj, StackTrace st)
		{
			WriteStack(obj, st, "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteStack(object obj, StackTrace st, string additionalMessage)
		{
			DiagnosticsWriterOutput.WriteStack(consoleWrapper, obj, st, additionalMessage);
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteWindowsFormsMessage(object obj, Message m)
		{
			WriteWindowsFormsMessage(obj, m, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteWindowsFormsMessage(object obj, Message m, string additionalMessage)
		{
			DiagnosticsWriterOutput.WriteWindowsFormsMessage(consoleWrapper, obj, m, additionalMessage);
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteFileStream(object obj, FileStream fs)
		{
			WriteFileStream(obj, fs, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteFileStream(object obj, FileStream fs, string additionalMessage)
		{
			DiagnosticsWriterOutput.WriteFileStream(consoleWrapper, obj, fs, additionalMessage);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
