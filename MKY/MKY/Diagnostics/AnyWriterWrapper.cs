//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.10
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.IO;
using System.Text;

namespace MKY.Diagnostics
{
	/// <summary>
	/// Wraps part of the interface of a <see cref="System.IO.TextWriter"/> to the common
	/// interface <see cref="IDiagnosticsWriter"/>.
	/// </summary>
	public class AnyWriterWrapper : IDiagnosticsWriter
	{
		#region Writer
		//==========================================================================================
		// Writer
		//==========================================================================================

		private TextWriter writer;

		/// <summary>
		/// Sets a writer to write to. Set <param name="writer"></param> to <c>null</c> after writing.
		/// </summary>
		public virtual void SetWriter(TextWriter writer)
		{
			this.writer = writer;
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

			if (this.writer != null)
				this.writer.Write(message);
		}

		/// <summary>
		/// Writes a message to the diagnostics listeners.
		/// </summary>
		/// <param name="message">A message to write.</param>
		public virtual void WriteLine(string message)
		{
			if (this.beginOfLine)
				WriteIndent();

			if (this.writer != null)
				this.writer.WriteLine(message);

			this.beginOfLine = true;
		}

		/// <summary>
		/// Writes <see cref="IndentLevel"/> times <see cref="IndentSize"/> spaces to the
		/// diagnostics listeners.
		/// </summary>
		public virtual void WriteIndent()
		{
			if (this.writer != null)
			{
				for (int i = 0; i < this.indentLevel; i++)
					this.writer.Write(this.indentString);
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
