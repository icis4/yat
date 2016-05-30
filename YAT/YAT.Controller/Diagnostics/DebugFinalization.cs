//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
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
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics;

namespace YAT.Controller.Diagnostics
{
	/// <summary>
	/// Provides static methods to help with finalization.
	/// </summary>
	public static class DebugFinalization
	{
		/// <summary></summary>
		[Conditional("DEBUG")]
		public static void DebugNotifyAllowedStaticObjects()
		{
		////Debug.WriteLine("Late finalizer calls are allowed for the following static objects:");
		////Debug.Indent();
		////Debug.WriteLine("<TBD>");
		////Debug.Unindent();

			Debug.WriteLine("Detection of late finalizer calls has been disabled until fix of bugs #243, #263 and #336 continues.");
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
