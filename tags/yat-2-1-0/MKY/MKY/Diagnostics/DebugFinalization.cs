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

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MKY.Diagnostics
{
	/// <summary>
	/// Provides static methods to help with finalization.
	/// </summary>
	public static class DebugFinalization
	{
		private static bool staticFinalizationShouldHaveCompleted; // = false;

		/// <summary></summary>
		public static bool FinalizationShouldHaveCompleted
		{
			get { return (staticFinalizationShouldHaveCompleted); }
			set { staticFinalizationShouldHaveCompleted = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		[Conditional("DEBUG")]
		public static void DebugNotifyFinalizerAndCheckWhetherOverdue(object obj)
		{
			if (staticFinalizationShouldHaveCompleted)
				Debug.WriteLine("The finalizer of this '" + obj.GetType().FullName + "' has been called too late! If this is not a static object, ensure to de-reference it early enough!");
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
