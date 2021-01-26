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
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool TryOpenFile(string filePath)
		{
			Exception exceptionOnFailure;
			return (TryOpenFile(filePath, out exceptionOnFailure));
		}

		/// <summary>
		/// Tries to open the given file with the system's default editor.
		/// </summary>
		/// <param name="filePath">File to open.</param>
		/// <param name="exceptionOnFailure">Exception object, in case of failure.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static bool TryOpenFile(string filePath, out Exception exceptionOnFailure)
		{
			try
			{
				Process.Start(filePath);
				exceptionOnFailure = null;
				return (true);
			}
			catch (Exception ex)
			{
				exceptionOnFailure = ex;
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
