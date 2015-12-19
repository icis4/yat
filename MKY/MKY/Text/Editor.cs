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
// MKY Development Version 1.0.14
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
using System.Diagnostics;

namespace MKY.Text
{
	/// <summary>
	/// Editor utility methods.
	/// </summary>
	public static class Editor
	{
		/// <summary>
		/// Tries to open the given file with the system's default editor.
		/// </summary>
		/// <param name="filePath">File to open.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		public static bool TryOpenFile(string filePath)
		{
			Exception exception;
			return (TryOpenFile(filePath, out exception));
		}

		/// <summary>
		/// Tries to open the given file with the system's default editor.
		/// </summary>
		/// <param name="filePath">File to open.</param>
		/// <param name="exception">Exception object, in case of failure.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		public static bool TryOpenFile(string filePath, out Exception exception)
		{
			try
			{
				Process.Start(filePath);
				exception = null;
				return (true);
			}
			catch (Exception ex)
			{
				exception = ex;
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
