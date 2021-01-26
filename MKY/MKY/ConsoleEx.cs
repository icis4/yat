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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;

using MKY.Diagnostics;

namespace MKY
{
	/// <summary>
	/// Provides static methods to write diagnostics output to <see cref="Console.Out"/> and
	/// <see cref="Console.Error"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ConsoleEx
	{
		#region TextWriterEx
		//==========================================================================================
		// TextWriterEx
		//==========================================================================================

		/// <summary>
		/// A console writer extension to <see cref="TextWriter"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Required by design to properly extend 'System.Console'.")]
		[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
		public class TextWriterEx
		{
			private IDiagnosticsWriter writer;

			/// <summary>
			/// Initializes writer and sets the diagnostics writer that wraps the console writer.
			/// </summary>
			public TextWriterEx(IDiagnosticsWriter writer)
			{
				this.writer = writer;
			}

			/// <summary>
			///  Writes location to the output writer of this object.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public void WriteLocation(string message = null)
			{
				DiagnosticsWriterOutput.WriteLocation(writer, new StackTrace(), 1, message);
			}

			/// <summary>
			/// Writes time stamp and location to the output writer of this object.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public void WriteTimeStamp(string message = null)
			{
				DiagnosticsWriterOutput.WriteTimeStamp(writer, new StackTrace(), 1, message);
			}

			/// <summary>
			/// Writes source, type, message and stack of the given exception and its inner
			/// exceptions to the output writer of this object.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public void WriteException(Type type, Exception ex, string leadMessage = null)
			{
				DiagnosticsWriterOutput.WriteExceptionLines(writer, type, ex, leadMessage);
			}

			/// <summary>
			/// Writes a <see cref="StackTrace"/> to the output writer of this object.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public void WriteStack(Type type, string leadMessage = null)
			{
				WriteStack(type, new StackTrace(), leadMessage);
			}

			/// <summary>
			/// Writes a <see cref="StackTrace"/> to the output writer of this object.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public void WriteStack(Type type, StackTrace st, string leadMessage = null)
			{
				DiagnosticsWriterOutput.WriteStackLines(writer, type, st, leadMessage);
			}

			/// <summary>
			/// Writes the properties of a <see cref="Message"/> to the output writer of this object.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "m", Justification = "Naming according to parameter 'm' of NativeWindow methods.")]
			public void WriteWindowsFormsMessage(Type type, Message m, string leadMessage = null)
			{
				DiagnosticsWriterOutput.WriteWindowsFormsMessageLines(writer, type, m, leadMessage);
			}

			/// <summary>
			/// Writes the properties of a <see cref="FileStream"/> to the output writer of this object.
			/// </summary>
			[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
			public void WriteFileStream(Type type, FileStream fs, string leadMessage = null)
			{
				DiagnosticsWriterOutput.WriteFileStreamLines(writer, type, fs, leadMessage);
			}
		}

		#endregion

		private static TextWriterEx staticOut   = new TextWriterEx(new ConsoleWrapper(Console.Out));
		private static TextWriterEx staticError = new TextWriterEx(new ConsoleWrapper(Console.Error));

		/// <summary>
		/// Gets the standard output stream wrapped to output diagnostics.
		/// </summary>
		/// <returns>
		/// A <see cref="TextWriterEx"/> that represents the standard output stream.
		/// </returns>
		public static TextWriterEx Out
		{
			get { return (staticOut); }
		}

		/// <summary>
		/// Gets the standard error stream wrapped to output diagnostics.
		/// </summary>
		/// <returns>
		/// A <see cref="TextWriterEx"/> that represents the standard error stream.
		/// </returns>
		public static TextWriterEx Error
		{
			get { return (staticError); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
