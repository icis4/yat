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
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace MKY.Utilities.Diagnostics
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
		/// <see cref="MKY.Utilities.Diagnostics.XDebug"/> and
		/// <see cref="MKY.Utilities.Diagnostics.XTrace"/> and
		/// <see cref="MKY.Utilities.Diagnostics.XConsole"/>.
		/// </remarks>
		public static void WriteException(IDiagnosticsWriter writer, object obj, Exception ex, string additionalMessage)
		{
			if (obj != null)
			{
				writer.Write("Exception in ");
				writer.WriteLine(obj.GetType().FullName);
			}
			else
			{
				writer.WriteLine("Exception");
			}

			writer.Indent();
			{
				WriteMessage(writer, additionalMessage);
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
		/// <see cref="MKY.Utilities.Diagnostics.XDebug"/> and
		/// <see cref="MKY.Utilities.Diagnostics.XTrace"/> and
		/// <see cref="MKY.Utilities.Diagnostics.XConsole"/>.
		/// </remarks>
		public static void WriteStack(IDiagnosticsWriter writer, object obj, StackTrace st, string additionalMessage)
		{
			if (obj != null)
			{
				writer.Write("Stack trace in ");
				writer.WriteLine(obj.GetType().FullName);
			}
			else
			{
				writer.WriteLine("Stack trace");
			}

			writer.Indent();
			{
				WriteMessage(writer, additionalMessage);
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
		/// <see cref="MKY.Utilities.Diagnostics.XDebug"/> and
		/// <see cref="MKY.Utilities.Diagnostics.XTrace"/> and
		/// <see cref="MKY.Utilities.Diagnostics.XConsole"/>.
		/// </remarks>
		public static void WriteWindowsFormsMessage(IDiagnosticsWriter writer, object obj, Message m, string additionalMessage)
		{
			if (obj != null)
			{
				writer.Write("Windows.Forms.Message in ");
				writer.WriteLine(obj.GetType().FullName);
			}
			else
			{
				writer.WriteLine("Windows.Forms.Message");
			}

			writer.Indent();
			{
				WriteMessage(writer, additionalMessage);
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
		/// <see cref="MKY.Utilities.Diagnostics.XDebug"/> and
		/// <see cref="MKY.Utilities.Diagnostics.XTrace"/> and
		/// <see cref="MKY.Utilities.Diagnostics.XConsole"/>.
		/// </remarks>
		public static void WriteFileStream(IDiagnosticsWriter writer, object obj, FileStream fs, string additionalMessage)
		{
			if (obj != null)
			{
				writer.Write("FileStream in ");
				writer.WriteLine(obj.GetType().FullName);
			}
			else
			{
				writer.WriteLine("FileStream");
			}

			writer.Indent();
			{
				WriteMessage(writer, additionalMessage);
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
			if ((message != null) && (message.Length > 0))
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
			if ((source != null) && (source.Length > 0))
			{
				writer.Write("Source: ");
				writer.WriteLine(source);
			}
		}

		private static void WriteStack(IDiagnosticsWriter writer, string stackTrace)
		{
			if ((stackTrace != null) && (stackTrace.Length > 0))
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
