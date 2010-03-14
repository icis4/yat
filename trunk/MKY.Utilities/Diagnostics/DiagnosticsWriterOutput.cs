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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

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
		public static void WriteException(IDiagnosticsWriter writer, object obj, Exception ex)
		{
			writer.Write("Exception in ");
			writer.WriteLine(obj.GetType().FullName);

			writer.Indent();
			{
				Exception exception = ex;
				int exceptionLevel = 0;
				string line;
				StringReader sr;

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
						writer.WriteLine("Message:");
						writer.Indent();
						{
							sr = new StringReader(exception.Message);
							do
							{
								line = sr.ReadLine();
								if (line != null)
									writer.WriteLine(line);
							}
							while (line != null);
						}
						writer.Unindent();
						writer.Write("Source: ");
						writer.WriteLine(exception.Source);
						writer.WriteLine("Stack:"); // stack trace is already indented
						sr = new StringReader(exception.StackTrace);
						do
						{
							line = sr.ReadLine();
							if (line != null)
								writer.WriteLine(line);
						}
						while (line != null);
					}
					writer.Unindent();

					exception = exception.InnerException;
					exceptionLevel++;
				}
			}
			writer.Unindent();

		} // WriteException()

		/// <summary>
		/// Writes message and stack to given writer.
		/// </summary>
		/// <remarks>
		/// There are also two predefined variants for the static objects
		/// <see cref="System.Diagnostics.Debug"/> and 
		/// <see cref="System.Diagnostics.Trace"/> available in
		/// <see cref="MKY.Utilities.Diagnostics.XDebug"/> and
		/// <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
		/// </remarks>
		public static void WriteStack(IDiagnosticsWriter writer, object obj, string message)
		{
			writer.Write("Stack trace in ");
			writer.WriteLine(obj.GetType().FullName);

			writer.Indent();
			{
				string line;
				StringReader sr;

				if ((message != null) && (message != ""))
				{
					writer.WriteLine("Message:");
					writer.Indent();
					{
						sr = new StringReader(message);
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

				writer.WriteLine("Stack:"); // stack trace is already indented
				StackTrace st = new StackTrace();
				sr = new StringReader(st.ToString());
				do
				{
					line = sr.ReadLine();
					if (line != null)
						writer.WriteLine(line);
				}
				while (line != null);
			}
			writer.Unindent();

		} // WriteStack()

	} // DiagnosticsWriterOutput
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
