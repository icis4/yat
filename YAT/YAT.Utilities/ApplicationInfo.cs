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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
		/// <summary>The product name.</summary>
		public static readonly string ProductName = Application.ProductName;

		/////// <summary>The product name postfix that describes the build.</summary>
		////public const string ProductBuildName = " Final";

		/////// <summary>The product name postfix that describes the build.</summary>
		////public const string ProductBuildName = " Beta 4";

		/////// <summary>The product name postfix that describes the build.</summary>
		////public const string ProductBuildName = " Beta 4 Candidate 1";

		/// <summary>The product name postfix that describes the build.</summary>
		public const string ProductBuildName = " Beta 4 Candidate 2 Development";

		/////// <summary>The product name postfix that describes the build.</summary>
		////public const string ProductBuildName = " Beta 4 Preliminary";

		/// <summary>The product name including the build description.</summary>
		public static readonly string ProductNameAndBuildName = Application.ProductName + ProductBuildName;

		/// <summary>The product version.</summary>
		public static readonly string ProductVersion = Application.ProductVersion;

		/// <summary>The complete product name including build description and version.</summary>
		public static readonly string ProductNameAndBuildNameAndVersion = ProductNameAndBuildName + " Version " + ProductVersion;

		/// <summary>The long variant of the complete product name including build description and version.</summary>
		public static readonly string ProductNameLong = Application.ProductName + " - Yet Another Terminal";
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
