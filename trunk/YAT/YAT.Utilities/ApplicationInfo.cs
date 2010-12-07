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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Windows.Forms;

namespace YAT.Utilities
{
	/// <summary></summary>
	public static class ApplicationInfo
	{
	/////// <summary></summary>
	////public const string ProductNamePostFix = "";

	/////// <summary></summary>
	////public const string ProductNamePostFix = " Beta 3";

		/// <summary></summary>
		public const string ProductNamePostFix = " Beta 3 Candidate 4";

	/////// <summary></summary>
	////public const string ProductNamePostFix = " Beta 3 Preliminary";

		/// <summary></summary>
		public static readonly string ProductName = Application.ProductName + ProductNamePostFix;
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
