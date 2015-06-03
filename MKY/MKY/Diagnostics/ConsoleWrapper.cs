﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.12
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.IO;
using System.Text;

namespace MKY.Diagnostics
{
	/// <summary>
	/// Wraps part of the interface of <see cref="System.Console"/> to the common
	/// interface <see cref="IDiagnosticsWriter"/>.
	/// </summary>
	public class ConsoleWrapper : IDiagnosticsWriter
	{
		#region Writer
		//==========================================================================================
		// Writer
		//==========================================================================================

		private TextWriter console;

		/// <summary>
		/// Initializes this wrapper and sets the console writer.
		/// Standard writers are <see cref="Console.Out"/> and <see cref="Console.Error"/>.
		/// </summary>
		public ConsoleWrapper(TextWriter console)
		{
			this.console = console;
		}

		#endregion

		#region Indent
		//==========================================================================================
		// Indent
		//==========================================================================================

		private int indentLevel; // = 0;
		private int indentSize = 4;
		private string indentString = "    ";

		/// <summary>
		/// Gets or sets the indent level.
		/// </summary>
		/// <value>
		/// The indent level. The default is zero.
		/// </value>
		public virtual int IndentLevel
		{
			get { return (this.indentLevel); }
			set { this.indentLevel = value; }
		}

		/// <summary>
		/// Gets or sets the number of spaces in an indent.
		/// </summary>
		/// <value>
		/// The number of spaces in an indent. The default is four.
		/// </value>
		public virtual int IndentSize
		{
			get { return (this.indentSize); }
			set
			{
				this.indentSize = value;

				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < this.indentSize; i++)
					sb.Append(" ");
				this.indentString = sb.ToString();
			}
		}

		/// <summary>
		/// Increases the current <see cref="IndentLevel"/> by one.
		/// </summary>
		public virtual void Indent()
		{
			this.indentLevel++;
		}

		/// <summary>
		/// Decreases the current <see cref="IndentLevel"/> by one.
		/// </summary>
		public virtual void Unindent()
		{
			this.indentLevel--;
		}

		#endregion

		#region Write
		//==========================================================================================
		// Write
		//==========================================================================================

		private bool beginOfLine = true;

		/// <summary>
		/// Writes a message to the diagnostics listeners.
		/// </summary>
		/// <param name="message">A message to write.</param>
		public virtual void Write(string message)
		{
			if (this.beginOfLine)
				WriteIndent();

			if (this.console != null)
				this.console.Write(message);
		}

		/// <summary>
		/// Writes a message to the diagnostics listeners.
		/// </summary>
		/// <param name="message">A message to write.</param>
		public virtual void WriteLine(string message)
		{
			if (this.beginOfLine)
				WriteIndent();

			if (this.console != null)
				this.console.WriteLine(message);

			this.beginOfLine = true;
		}

		/// <summary>
		/// Writes <see cref="IndentLevel"/> times <see cref="IndentSize"/> spaces to the
		/// diagnostics listeners.
		/// </summary>
		protected virtual void WriteIndent()
		{
			if (this.console != null)
			{
				for (int i = 0; i < this.indentLevel; i++)
					this.console.Write(this.indentString);
			}

			this.beginOfLine = false;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
