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

namespace MKY.Diagnostics
{
	/// <summary>
	/// Provides static methods to help with disposal.
	/// </summary>
	public static class FinalizationHelper
	{
		private static bool staticFinalizationShouldHaveCompleted; // = false

		/// <summary></summary>
		public static bool FinalizationShouldHaveCompleted
		{
			get { return (staticFinalizationShouldHaveCompleted); }
			set { staticFinalizationShouldHaveCompleted = value;  }
		}

		/// <summary></summary>
		public static void DebugNotifyFinalizerAndCheckWhetherOverdue(object obj)
		{
			if (staticFinalizationShouldHaveCompleted)
				Debug.WriteLine("The finalizer of this '" + obj.GetType().FullName + "' has been called too late! Ensure to de-reference the object early enough!");
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
