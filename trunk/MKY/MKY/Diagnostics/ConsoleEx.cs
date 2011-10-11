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
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
	/// Provides static methods to write diagnostics output to <see cref="System.Console"/>.
	/// </summary>
	public static class ConsoleEx
	{
		private static ConsoleWrapper consoleWrapper = new ConsoleWrapper();

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteException(Type type, Exception ex)
		{
			WriteException(type, ex, "");
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteException(Type type, Exception ex, string additionalMessage)
		{
			DiagnosticsWriterOutput.WriteException(consoleWrapper, type, ex, additionalMessage);
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteStack(Type type)
		{
			WriteStack(type, new StackTrace(), "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteStack(Type type, StackTrace st)
		{
			WriteStack(type, st, "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteStack(Type type, StackTrace st, string additionalMessage)
		{
			DiagnosticsWriterOutput.WriteStack(consoleWrapper, type, st, additionalMessage);
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteWindowsFormsMessage(Type type, Message m)
		{
			WriteWindowsFormsMessage(type, m, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteWindowsFormsMessage(Type type, Message m, string additionalMessage)
		{
			DiagnosticsWriterOutput.WriteWindowsFormsMessage(consoleWrapper, type, m, additionalMessage);
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteFileStream(Type type, FileStream fs)
		{
			WriteFileStream(type, fs, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to <see cref="System.Console"/>.
		/// </summary>
		public static void WriteFileStream(Type type, FileStream fs, string additionalMessage)
		{
			DiagnosticsWriterOutput.WriteFileStream(consoleWrapper, type, fs, additionalMessage);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
