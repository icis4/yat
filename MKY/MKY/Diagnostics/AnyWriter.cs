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
	/// Provides static methods to write diagnostics output to any <see cref="TextWriter"/>.
	/// </summary>
	public static class AnyWriter
	{
		private static AnyWriterWrapper anyWriterWrapper = new AnyWriterWrapper();

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteException(TextWriter writer, object obj, Exception ex)
		{
			WriteException(writer, obj, ex, "");
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteException(TextWriter writer, object obj, Exception ex, string additionalMessage)
		{
			anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteException(anyWriterWrapper, obj, ex, additionalMessage);
			anyWriterWrapper.SetWriter(null);
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteStack(TextWriter writer, object obj)
		{
			WriteStack(writer, obj, new StackTrace(), "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteStack(TextWriter writer, object obj, StackTrace st)
		{
			WriteStack(writer, obj, st, "");
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteStack(TextWriter writer, object obj, StackTrace st, string additionalMessage)
		{
			anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteStack(anyWriterWrapper, obj, st, additionalMessage);
			anyWriterWrapper.SetWriter(null);
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteWindowsFormsMessage(TextWriter writer, object obj, Message m)
		{
			WriteWindowsFormsMessage(writer, obj, m, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteWindowsFormsMessage(TextWriter writer, object obj, Message m, string additionalMessage)
		{
			anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteWindowsFormsMessage(anyWriterWrapper, obj, m, additionalMessage);
			anyWriterWrapper.SetWriter(null);
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteFileStream(TextWriter writer, object obj, FileStream fs)
		{
			WriteFileStream(writer, obj, fs, "");
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteFileStream(TextWriter writer, object obj, FileStream fs, string additionalMessage)
		{
			anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteFileStream(anyWriterWrapper, obj, fs, additionalMessage);
			anyWriterWrapper.SetWriter(null);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
