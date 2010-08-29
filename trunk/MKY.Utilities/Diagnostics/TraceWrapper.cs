//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics;

namespace MKY.Utilities.Diagnostics
{
	/// <summary>
	/// Wraps part of the interface of <see cref="System.Diagnostics.Trace"/> to the common
	/// interface <see cref="IDiagnosticsWriter"/>.
	/// </summary>
	public class TraceWrapper : IDiagnosticsWriter
	{
		#region Indent
		//==========================================================================================
		// Indent
		//==========================================================================================

		/// <summary>
		/// Gets or sets the indent level.
		/// </summary>
		/// <value>
		/// The indent level. The default is zero.
		/// </value>
		public virtual int IndentLevel
		{
			get { return (Trace.IndentLevel); }
			set { Trace.IndentLevel = value;  }
		}

		/// <summary>
		/// Gets or sets the number of spaces in an indent.
		/// </summary>
		/// <value>
		/// The number of spaces in an indent. The default is four.
		/// </value>
		public virtual int IndentSize
		{
			get { return (Trace.IndentSize); }
			set { Trace.IndentSize = value;  }
		}

		/// <summary>
		/// Increases the current <see cref="IndentLevel"/> by one.
		/// </summary>
		public virtual void Indent()
		{
			Trace.Indent();
		}

		/// <summary>
		/// Decreases the current <see cref="IndentLevel"/> by one.
		/// </summary>
		public virtual void Unindent()
		{
			Trace.Unindent();
		}

		#endregion

		#region Write
		//==========================================================================================
		// Write
		//==========================================================================================

		/// <summary>
		/// Writes a message to the diagnostics listeners.
		/// </summary>
		/// <param name="message">A message to write.</param>
		public virtual void Write(string message)
		{
			Trace.Write(message);
		}

		/// <summary>
		/// Writes a message to the diagnostics listeners.
		/// </summary>
		/// <param name="message">A message to write.</param>
		public virtual void WriteLine(string message)
		{
			Trace.WriteLine(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
