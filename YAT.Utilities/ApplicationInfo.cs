//==================================================================================================
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
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System.Windows.Forms;

namespace YAT.Utilities
{
	public static class ApplicationInfo
	{
	////public const string ProductNamePostFix = "";
	////public const string ProductNamePostFix = " Beta 3";
		public const string ProductNamePostFix = " Beta 3 Candidate 2";
	////public const string ProductNamePostFix = " Beta 3 Preliminary";

		public static readonly string ProductName = Application.ProductName + ProductNamePostFix;
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
