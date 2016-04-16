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
// MKY Development Version 1.0.14
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;

#endregion

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
			WriteException(writer, type, ex, null);
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteException(TextWriter writer, Type type, Exception ex, string leadMessage)
		{
			anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteException(anyWriterWrapper, type, ex, leadMessage);
			anyWriterWrapper.SetWriter(null);
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteStack(TextWriter writer, Type type)
		{
			WriteStack(writer, type, new StackTrace(), null);
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteStack(TextWriter writer, Type type, string leadMessage)
		{
			WriteStack(writer, type, new StackTrace(), leadMessage);
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteStack(TextWriter writer, Type type, StackTrace st)
		{
			WriteStack(writer, type, st, null);
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteStack(TextWriter writer, Type type, StackTrace st, string leadMessage)
		{
			anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteStack(anyWriterWrapper, type, st, leadMessage);
			anyWriterWrapper.SetWriter(null);
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "m", Justification = "Naming according to parameter 'm' of NativeWindow methods.")]
		public static void WriteWindowsFormsMessage(TextWriter writer, Type type, Message m)
		{
			WriteWindowsFormsMessage(writer, type, m, null);
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "m", Justification = "Naming according to parameter 'm' of NativeWindow methods.")]
		public static void WriteWindowsFormsMessage(TextWriter writer, Type type, Message m, string leadMessage)
		{
			anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteWindowsFormsMessage(anyWriterWrapper, type, m, leadMessage);
			anyWriterWrapper.SetWriter(null);
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteFileStream(TextWriter writer, Type type, FileStream fs)
		{
			WriteFileStream(writer, type, fs, null);
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to the given <see cref="TextWriter"/>.
		/// </summary>
		public static void WriteFileStream(TextWriter writer, Type type, FileStream fs, string leadMessage)
		{
			anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteFileStream(anyWriterWrapper, type, fs, leadMessage);
			anyWriterWrapper.SetWriter(null);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
