//==================================================================================================
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MKY.Utilities.Diagnostics
{
	/// <summary>
	/// Wraps part of the interface of <see cref="System.Diagnostics.Debug"/> to the common
	/// interface <see cref="IDiagnosticsWriter"/>.
	/// </summary>
	public class DebugWrapper : IDiagnosticsWriter
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
			get { return (Debug.IndentLevel); }
			set { Debug.IndentLevel = value;  }
		}

		/// <summary>
		/// Gets or sets the number of spaces in an indent.
		/// </summary>
		/// <value>
		/// The number of spaces in an indent. The default is four.
		/// </value>
		public virtual int IndentSize
		{
			get { return (Debug.IndentSize); }
			set { Debug.IndentSize = value;  }
		}

		/// <summary>
		/// Increases the current <see cref="IndentLevel"/> by one.
		/// </summary>
		public virtual void Indent()
		{
			Debug.Indent();
		}

		/// <summary>
		/// Decreases the current <see cref="IndentLevel"/> by one.
		/// </summary>
		public virtual void Unindent()
		{
			Debug.Unindent();
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
			Debug.Write(message);
		}

		/// <summary>
		/// Writes a message to the diagnostics listeners.
		/// </summary>
		/// <param name="message">A message to write.</param>
		public virtual void WriteLine(string message)
		{
			Debug.WriteLine(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
