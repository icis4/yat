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
	/// A collection of methods to write the state of certain objects to a given diagnostics writer.
	/// </summary>
	public static class DiagnosticsWriterOutput
	{
		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

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
		/// <see cref="MKY.Diagnostics.ConsoleEx"/>.
		/// </remarks>
		public static void WriteException(IDiagnosticsWriter writer, Type type, Exception ex, string leadMessage)
		{
			if (type != null)
			{
				writer.Write("Exception in ");
				writer.WriteLine(type.FullName);
			}
			else
			{
				writer.WriteLine("Exception");
			}

			writer.Indent();
			{
				WriteMessage(writer, leadMessage);
				WriteException(writer, ex);
			}
			writer.Unindent();
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
		/// <see cref="MKY.Diagnostics.ConsoleEx"/>.
		/// </remarks>
		public static void WriteStack(IDiagnosticsWriter writer, Type type, StackTrace st, string leadMessage)
		{
			if (type != null)
			{
				writer.Write("Stack trace in ");
				writer.WriteLine(type.FullName);
			}
			else
			{
				writer.WriteLine("Stack trace");
			}

			writer.Indent();
			{
				WriteMessage(writer, leadMessage);
				WriteStack(writer, st.ToString());
			}
			writer.Unindent();
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
		/// <see cref="MKY.Diagnostics.ConsoleEx"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "m", Justification = "Naming according to parameter 'm' of NativeWindow methods.")]
		public static void WriteWindowsFormsMessage(IDiagnosticsWriter writer, Type type, Message m, string leadMessage)
		{
			if (type != null)
			{
				writer.Write("Windows.Forms.Message in ");
				writer.WriteLine(type.FullName);
			}
			else
			{
				writer.WriteLine("Windows.Forms.Message");
			}

			writer.Indent();
			{
				WriteMessage(writer, leadMessage);
				WriteWindowsFormsMessage(writer, m);
			}
			writer.Unindent();
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
		/// <see cref="MKY.Diagnostics.ConsoleEx"/>.
		/// </remarks>
		public static void WriteFileStream(IDiagnosticsWriter writer, Type type, FileStream fs, string leadMessage)
		{
			if (type != null)
			{
				writer.Write("FileStream in ");
				writer.WriteLine(type.FullName);
			}
			else
			{
				writer.WriteLine("FileStream");
			}

			writer.Indent();
			{
				WriteMessage(writer, leadMessage);
				WriteFileStream(writer, fs);
			}
			writer.Unindent();
		}

		#endregion

		#region Private Static Methods
		//==========================================================================================
		// Private Static Methods
		//==========================================================================================

		private static void WriteMessage(IDiagnosticsWriter writer, string message)
		{
			if (!string.IsNullOrEmpty(message))
			{
				writer.WriteLine("Message:");
				writer.Indent();
				{
					StringReader sr = new StringReader(message);
					string line;
					do
					{
						line = sr.ReadLine();
						if (line != null)
							writer.WriteLine(line);
					}
					while (line != null);
				}
				writer.Unindent();
			}
		}

		private static void WriteException(IDiagnosticsWriter writer, Exception ex)
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
					WriteType(writer, exception.GetType());
					WriteMessage(writer, exception.Message);
					WriteSource(writer, exception.Source);
					WriteStack(writer, exception.StackTrace);
				}
				writer.Unindent();

				exception = exception.InnerException;
				exceptionLevel++;
			}
		}

		private static void WriteType(IDiagnosticsWriter writer, Type type)
		{
			writer.Write("Type: ");
			writer.WriteLine(type.ToString());
		}

		private static void WriteSource(IDiagnosticsWriter writer, string source)
		{
			if (!string.IsNullOrEmpty(source))
			{
				writer.Write("Source: ");
				writer.WriteLine(source);
			}
		}

		private static void WriteStack(IDiagnosticsWriter writer, string stackTrace)
		{
			if (!string.IsNullOrEmpty(stackTrace))
			{
				writer.WriteLine("Stack:");

				// Stack trace is already indented. No need to indent again.

				StringReader sr = new StringReader(stackTrace.ToString());
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

		private static void WriteWindowsFormsMessage(IDiagnosticsWriter writer, Message m)
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

		private static void WriteFileStream(IDiagnosticsWriter writer, FileStream fs)
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
