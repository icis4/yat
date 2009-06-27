//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Diagnostics
{
	/// <summary>
	/// Wraps part of the interface of <see cref="System.Console"/> to the common
	/// interface <see cref="IDiagnosticsWriter"/>.
	/// </summary>
	public class ConsoleWrapper : IDiagnosticsWriter
	{
		#region Indent
		//==========================================================================================
		// Indent
		//==========================================================================================

		private int _indentLevel = 0;
		private int _indentSize = 4;
		private string _indentString = "    ";

		/// <summary>
		/// Gets or sets the indent level.
		/// </summary>
		/// <value>
		/// The indent level. The default is zero.
		/// </value>
		public int IndentLevel
		{
			get { return (_indentLevel); }
			set { _indentLevel = value; }
		}

		/// <summary>
		/// Gets or sets the number of spaces in an indent.
		/// </summary>
		/// <value>
		/// The number of spaces in an indent. The default is four.
		/// </value>
		public int IndentSize
		{
			get { return (_indentSize); }
			set
			{
				_indentSize = value;

				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < _indentSize; i++)
					sb.Append(" ");
				_indentString = sb.ToString();
			}
		}

		/// <summary>
		/// Increases the current <see cref="IndentLevel"/> by one.
		/// </summary>
		public void Indent()
		{
			_indentLevel++;
		}

		/// <summary>
		/// Decreases the current <see cref="IndentLevel"/> by one.
		/// </summary>
		public void Unindent()
		{
			_indentLevel--;
		}

		#endregion

		#region Write
		//==========================================================================================
		// Write
		//==========================================================================================

		private bool _beginOfLine = true;

		/// <summary>
		/// Writes a message to the diagnostics listeners.
		/// </summary>
		/// <param name="message">A message to write.</param>
		public void Write(string message)
		{
			if (_beginOfLine)
				WriteIndent();

			Console.Write(message);
		}

		/// <summary>
		/// Writes a message to the diagnostics listeners.
		/// </summary>
		/// <param name="message">A message to write.</param>
		public void WriteLine(string message)
		{
			if (_beginOfLine)
				WriteIndent();

			Console.WriteLine(message);
			_beginOfLine = true;
		}

		/// <summary>
		/// Writes <see cref="IndentLevel"/> times <see cref="IndentSize"/> spaces to the
		/// diagnostics listeners.
		/// </summary>
		public void WriteIndent()
		{
			for (int i = 0; i < _indentLevel; i++)
				Console.Write(_indentString);

			_beginOfLine = false;
		}

		#endregion
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
