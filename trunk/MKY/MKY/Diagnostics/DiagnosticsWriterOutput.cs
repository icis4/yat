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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Globalization;
using System.IO;
using System.Windows.Forms;

#endregion

namespace MKY.Diagnostics
{
	/// <summary>
	/// A collection of methods to write the state of certain objects to a given diagnostics writer.
	/// </summary>
	public static class DiagnosticsWriterOutput
	{
		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Writes location to the given writer.
		/// </summary>
		/// <remarks>
		/// There are predefined variants for the static objects
		/// <see cref="System.Diagnostics.Debug"/> and
		/// <see cref="System.Diagnostics.Trace"/> and
		/// <see cref="System.Console"/> available in
		/// <see cref="MKY.Diagnostics.DebugEx"/> and
		/// <see cref="MKY.Diagnostics.TraceEx"/> and
		/// <see cref="MKY.ConsoleEx"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void WriteLocation(IDiagnosticsWriter writer, StackTrace st, int index, string message, bool appendLineBreak = true)
		{
			var sf = st.GetFrame(index);
			if (sf != null)
			{
				var m = sf.GetMethod();
				writer.Write(m.ReflectedType.FullName);
				writer.Write(".");
				writer.Write(m.Name);
				writer.Write("()");
			}

			if (!string.IsNullOrEmpty(message))
			{
				writer.Write(" : ");
				writer.Write(message);
			}

			if (appendLineBreak)
			{
				writer.WriteLine("");
			}
		}

		/// <summary>
		/// Writes time stamp and location to the given writer.
		/// </summary>
		/// <remarks>
		/// There are predefined variants for the static objects
		/// <see cref="System.Diagnostics.Debug"/> and
		/// <see cref="System.Diagnostics.Trace"/> and
		/// <see cref="System.Console"/> available in
		/// <see cref="MKY.Diagnostics.DebugEx"/> and
		/// <see cref="MKY.Diagnostics.TraceEx"/> and
		/// <see cref="MKY.ConsoleEx"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void WriteTimeStamp(IDiagnosticsWriter writer, StackTrace st, int index, string message, bool appendLineBreak = true)
		{
			writer.Write(DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo));

			if (st != null)
			{
				writer.Write(" @ ");
				WriteLocation(writer, st, index, null, false);
			}

			if (!string.IsNullOrEmpty(message))
			{
				writer.Write(" : ");
				writer.Write(message);
			}

			if (appendLineBreak)
			{
				writer.WriteLine("");
			}
		}

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to the given writer.
		/// </summary>
		/// <remarks>
		/// There are predefined variants for the static objects
		/// <see cref="System.Diagnostics.Debug"/> and
		/// <see cref="System.Diagnostics.Trace"/> and
		/// <see cref="System.Console"/> available in
		/// <see cref="MKY.Diagnostics.DebugEx"/> and
		/// <see cref="MKY.Diagnostics.TraceEx"/> and
		/// <see cref="MKY.ConsoleEx"/>.
		/// </remarks>
		public static void WriteExceptionLines(IDiagnosticsWriter writer, Type type, Exception ex, string leadMessage)
		{
			if (type != null)
			{
				writer.Write("Exception in ");
				writer.Write(type.FullName);
				writer.WriteLine(":");

				writer.Indent();
				{
					WriteLeadMessageLines(writer, leadMessage);
					WriteExceptionLines(writer, ex);
				}
				writer.Unindent();
			}
			else
			{
				WriteLeadMessageLines(writer, leadMessage);
				WriteExceptionLines(writer, ex);
			}
		}

		/// <summary>
		/// Writes a <see cref="StackTrace"/> to the given writer.
		/// </summary>
		/// <remarks>
		/// There are predefined variants for the static objects
		/// <see cref="System.Diagnostics.Debug"/> and
		/// <see cref="System.Diagnostics.Trace"/> and
		/// <see cref="System.Console"/> available in
		/// <see cref="MKY.Diagnostics.DebugEx"/> and
		/// <see cref="MKY.Diagnostics.TraceEx"/> and
		/// <see cref="MKY.ConsoleEx"/>.
		/// </remarks>
		public static void WriteStackLines(IDiagnosticsWriter writer, Type type, StackTrace st, string leadMessage)
		{
			if (type != null)
			{
				writer.Write("Stack trace in ");
				writer.Write(type.FullName);
				writer.WriteLine(":");

				writer.Indent();
				{
					WriteLeadMessageLines(writer, leadMessage);
					WriteStackLines(writer, st.ToString());
				}
				writer.Unindent();
			}
			else
			{
				WriteLeadMessageLines(writer, leadMessage);
				WriteStackLines(writer, st.ToString());
			}
		}

		/// <summary>
		/// Writes the properties of a <see cref="Message"/> to the given writer.
		/// </summary>
		/// <remarks>
		/// There are predefined variants for the static objects
		/// <see cref="System.Diagnostics.Debug"/> and
		/// <see cref="System.Diagnostics.Trace"/> and
		/// <see cref="System.Console"/> available in
		/// <see cref="MKY.Diagnostics.DebugEx"/> and
		/// <see cref="MKY.Diagnostics.TraceEx"/> and
		/// <see cref="MKY.ConsoleEx"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "m", Justification = "Naming according to parameter 'm' of NativeWindow methods.")]
		public static void WriteWindowsFormsMessageLines(IDiagnosticsWriter writer, Type type, Message m, string leadMessage)
		{
			if (type != null)
			{
				writer.Write("Windows.Forms.Message in ");
				writer.Write(type.FullName);
				writer.WriteLine(":");

				writer.Indent();
				{
					WriteLeadMessageLines(writer, leadMessage);
					WriteWindowsFormsMessageLines(writer, m);
				}
				writer.Unindent();
			}
			else
			{
				WriteLeadMessageLines(writer, leadMessage);
				WriteWindowsFormsMessageLines(writer, m);
			}
		}

		/// <summary>
		/// Writes the properties of a <see cref="FileStream"/> to the given writer.
		/// </summary>
		/// <remarks>
		/// There are predefined variants for the static objects
		/// <see cref="System.Diagnostics.Debug"/> and
		/// <see cref="System.Diagnostics.Trace"/> and
		/// <see cref="System.Console"/> available in
		/// <see cref="MKY.Diagnostics.DebugEx"/> and
		/// <see cref="MKY.Diagnostics.TraceEx"/> and
		/// <see cref="MKY.ConsoleEx"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "StreamLines", Justification = "What's the problem...")]
		public static void WriteFileStreamLines(IDiagnosticsWriter writer, Type type, FileStream fs, string leadMessage)
		{
			if (type != null)
			{
				writer.Write("FileStream in ");
				writer.Write(type.FullName);
				writer.WriteLine(":");

				writer.Indent();
				{
					WriteLeadMessageLines(writer, leadMessage);
					WriteFileStreamLines(writer, fs);
				}
				writer.Unindent();
			}
			else
			{
				WriteLeadMessageLines(writer, leadMessage);
				WriteFileStreamLines(writer, fs);
			}
		}

		#endregion

		#region Non-Public Static Methods
		//==========================================================================================
		// Non-Public Static Methods
		//==========================================================================================

		private static void WriteExceptionLines(IDiagnosticsWriter writer, Exception ex)
		{
			Exception exception = ex;
			int exceptionLevel = 0;

			while (exception != null)
			{
				if (exceptionLevel == 0)
					writer.WriteLine("Exception:");
				else
					writer.WriteLine("Inner exception level " + exceptionLevel + ":");

				writer.Indent();
				{
					WriteTypeLine    (writer, exception.GetType());
					WriteMessageLines(writer, exception.Message);
					WriteSourceLine  (writer, exception.Source);
					WriteStackLines  (writer, exception.StackTrace);
				}
			////writer.Unindent() is done at the end.

				exception = exception.InnerException;
				exceptionLevel++;
			}

			while (exceptionLevel > 0)
			{
				writer.Unindent();
				exceptionLevel--;
			}
		}

		private static void WriteTypeLine(IDiagnosticsWriter writer, Type type)
		{
			if (type != null)
			{
				writer.Write("Type: ");
				writer.WriteLine(type.ToString());
			}
		}

		private static void WriteLeadMessageLines(IDiagnosticsWriter writer, string leadMessage)
		{
			if (!string.IsNullOrEmpty(leadMessage))
			{
				using (var sr = new StringReader(leadMessage))
				{
					string line;
					do
					{
						line = sr.ReadLine();
						if (line != null)
							writer.WriteLine(line);
					}
					while (line != null);
				}
			}
		}

		private static void WriteMessageLines(IDiagnosticsWriter writer, string message)
		{
			if (!string.IsNullOrEmpty(message))
			{
				writer.WriteLine("Message:");
				writer.Indent();
				{
					using (var sr = new StringReader(message))
					{
						string line;
						do
						{
							line = sr.ReadLine();
							if (line != null)
								writer.WriteLine(line);
						}
						while (line != null);
					}
				}
				writer.Unindent();
			}
		}

		private static void WriteSourceLine(IDiagnosticsWriter writer, string source)
		{
			if (!string.IsNullOrEmpty(source))
			{
				writer.WriteLine("Source:");
				writer.Indent();
				{
					writer.WriteLine(source);
				}
				writer.Unindent();
			}
		}

		private static void WriteStackLines(IDiagnosticsWriter writer, string stackTrace)
		{
			if (!string.IsNullOrEmpty(stackTrace))
			{
				writer.WriteLine("Stack:");

				// Stack trace is already indented. No need to indent again.

				using (var sr = new StringReader(stackTrace.ToString()))
				{
					string line;
					do
					{
						line = sr.ReadLine();
						if (line != null)
							writer.WriteLine(line);
					}
					while (line != null);
				}
			}
		}

		private static void WriteWindowsFormsMessageLines(IDiagnosticsWriter writer, Message m)
		{
			if (m != null)
			{
				writer.WriteLine("Windows.Forms.Message:");
				writer.Indent();
				{
					// msg=0x24 (WM_GETMINMAXINFO) hwnd=0x260bb2 wparam=0x0 lparam=0x7dad8e0 result=0x0
					writer.WriteLine(m.ToString());
				}
				writer.Unindent();
			}
		}

		private static void WriteFileStreamLines(IDiagnosticsWriter writer, FileStream fs)
		{
			if (fs != null)
			{
				writer.WriteLine("FileStream:");
				writer.Indent();
				{
					writer.Write("Name:     ");
					writer.WriteLine(fs.Name);
					writer.Write("CanSeek:  ");
					writer.WriteLine(fs.CanSeek.ToString());
					writer.Write("CanRead:  ");
					writer.WriteLine(fs.CanRead.ToString());
					writer.Write("CanWrite: ");
					writer.WriteLine(fs.CanWrite.ToString());

					if (fs.SafeFileHandle != null)
					{
						writer.WriteLine("SafeFileHandle:");
						writer.Indent();
						{
							writer.Write("Handle:    ");
							writer.WriteLine(fs.SafeFileHandle.DangerousGetHandle().ToString());
							writer.Write("IsInvalid: ");
							writer.WriteLine(fs.SafeFileHandle.IsInvalid.ToString());
							writer.Write("IsClosed:  ");
							writer.WriteLine(fs.SafeFileHandle.IsClosed.ToString());
						}
						writer.Unindent();
					}
				}
				writer.Unindent();
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
