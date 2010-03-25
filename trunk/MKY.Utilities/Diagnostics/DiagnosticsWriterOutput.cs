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
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace MKY.Utilities.Diagnostics
{
	/// <summary>
	/// Provides methods to write to <see cref="System.Diagnostics.Debug"/> and
	/// <see cref="System.Diagnostics.Trace"/> through appropriate wrappers.
	/// </summary>
	public static class DiagnosticsWriterOutput
	{
		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to to given writer.
		/// </summary>
		/// <remarks>
		/// There are also two predefined variants for the static objects
		/// <see cref="System.Diagnostics.Debug"/> and 
		/// <see cref="System.Diagnostics.Trace"/> available in
		/// <see cref="MKY.Utilities.Diagnostics.XDebug"/> and
		/// <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
		/// </remarks>
		public static void WriteException(IDiagnosticsWriter writer, object obj, Exception ex, string additionalMessage)
		{
			writer.Write("Exception in ");
			writer.WriteLine(obj.GetType().FullName);

			writer.Indent();
			{
				WriteMessage(writer, additionalMessage);
				WriteException(writer, ex);
			}
			writer.Unindent();
		}

		/// <summary>
		/// Writes additionalMessage and stack to given writer.
		/// </summary>
		/// <remarks>
		/// There are also two predefined variants for the static objects
		/// <see cref="System.Diagnostics.Debug"/> and 
		/// <see cref="System.Diagnostics.Trace"/> available in
		/// <see cref="MKY.Utilities.Diagnostics.XDebug"/> and
		/// <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
		/// </remarks>
		public static void WriteStack(IDiagnosticsWriter writer, object obj, StackTrace st, string additionalMessage)
		{
			writer.Write("Stack trace in ");
			writer.WriteLine(obj.GetType().FullName);

			writer.Indent();
			{
				WriteMessage(writer, additionalMessage);
				WriteStack(writer, st.ToString());
			}
			writer.Unindent();
		}

		/// <summary>
		/// Writes a windows forms message to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		/// <remarks>
		/// There are also two predefined variants for the static objects
		/// <see cref="System.Diagnostics.Debug"/> and 
		/// <see cref="System.Diagnostics.Trace"/> available in
		/// <see cref="MKY.Utilities.Diagnostics.XDebug"/> and
		/// <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
		/// </remarks>
		public static void WriteWindowsFormsMessage(IDiagnosticsWriter writer, object obj, Message m, string additionalMessage)
		{
			writer.Write("Message in ");
			writer.WriteLine(obj.GetType().FullName);

			writer.Indent();
			{
				WriteMessage(writer, additionalMessage);
				WriteWindowsFormsMessage(writer, m);
			}
			writer.Unindent();
		}

		private static void WriteMessage(IDiagnosticsWriter writer, string message)
		{
			if ((message != null) && (message != ""))
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
					writer.Write("Type: ");
					writer.WriteLine(exception.GetType().ToString());

					WriteMessage(writer, exception.Message);

					writer.Write("Source: ");
					writer.WriteLine(exception.Source);

					WriteStack(writer, exception.StackTrace);
				}
				writer.Unindent();

				exception = exception.InnerException;
				exceptionLevel++;
			}
		}

		private static void WriteStack(IDiagnosticsWriter writer, string stackTrace)
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

		private static void WriteWindowsFormsMessage(IDiagnosticsWriter writer, Message m)
		{
			writer.WriteLine("Windows Forms Message:");
			writer.Indent();
			{
				// msg=0x24 (WM_GETMINMAXINFO) hwnd=0x260bb2 wparam=0x0 lparam=0x7dad8e0 result=0x0
				writer.WriteLine(m.ToString());
			}
			writer.Unindent();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
