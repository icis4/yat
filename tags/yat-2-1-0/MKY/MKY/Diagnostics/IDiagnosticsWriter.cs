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
// MKY Version 1.0.27
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;

namespace MKY.Diagnostics
{
	/// <summary>
	/// Provides a common interface to write to <see cref="Debug"/>, <see cref="Trace"/>,
	/// <see cref="Console"/> and other output writers via <see cref="AnyWriter"/>.
	/// </summary>
	public interface IDiagnosticsWriter
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
		int IndentLevel
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the number of spaces in an indent.
		/// </summary>
		/// <value>
		/// The number of spaces in an indent. The default is four (same as the default
		/// of <see cref="Debug.IndentSize"/> and <see cref="Trace.IndentSize"/>).
		/// </value>
		int IndentSize
		{
			get; set;
		}

		/// <summary>
		/// Increases the current <see cref="IndentLevel"/> by one.
		/// </summary>
		void Indent();

		/// <summary>
		/// Decreases the current <see cref="IndentLevel"/> by one.
		/// </summary>
		void Unindent();

		#endregion

		#region Write
		//==========================================================================================
		// Write
		//==========================================================================================

		/// <summary>
		/// Writes a message to the diagnostics listeners.
		/// </summary>
		/// <param name="message">A message to write.</param>
		void Write(string message);

		/// <summary>
		/// Writes a message to the diagnostics listeners.
		/// </summary>
		/// <param name="message">A message to write.</param>
		void WriteLine(string message);

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
