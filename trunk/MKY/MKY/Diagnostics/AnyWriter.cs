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
// MKY Development Version 1.0.8
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
	/// Provides static methods to write diagnostics output to any <see cref="TextWriter"/>.
	/// </summary>
	public static class AnyWriter
	{
		private static AnyWriterWrapper anyWriterWrapper = new AnyWriterWrapper();

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteException(TextWriter writer, Type type, Exception ex)
		{
			WriteException(writer, type, ex, "");
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteException(TextWriter writer, Type type, Exception ex, string additionalMessage)
		{
			anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteException(anyWriterWrapper, type, ex, additionalMessage);
			anyWriterWrapper.SetWriter(null);
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteStack(TextWriter writer, Type type)
		{
			WriteStack(writer, type, new StackTrace(), "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteStack(TextWriter writer, Type type, StackTrace st)
		{
			WriteStack(writer, type, st, "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteStack(TextWriter writer, Type type, StackTrace st, string additionalMessage)
		{
			anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteStack(anyWriterWrapper, type, st, additionalMessage);
			anyWriterWrapper.SetWriter(null);
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteWindowsFormsMessage(TextWriter writer, Type type, Message m)
		{
			WriteWindowsFormsMessage(writer, type, m, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteWindowsFormsMessage(TextWriter writer, Type type, Message m, string additionalMessage)
		{
			anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteWindowsFormsMessage(anyWriterWrapper, type, m, additionalMessage);
			anyWriterWrapper.SetWriter(null);
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteFileStream(TextWriter writer, Type type, FileStream fs)
		{
			WriteFileStream(writer, type, fs, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteFileStream(TextWriter writer, Type type, FileStream fs, string additionalMessage)
		{
			anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteFileStream(anyWriterWrapper, type, fs, additionalMessage);
			anyWriterWrapper.SetWriter(null);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
