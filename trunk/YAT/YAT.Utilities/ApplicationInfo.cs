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
// YAT 2.0 Gamma 1'' Version 1.99.34
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace YAT.Utilities
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	public static class ApplicationInfo
	{
		/// <summary>The product name.</summary>
		public const string ProductName = "YAT";

		/////// <summary>The product name postfix that describes the build.</summary>
		////public const string ProductBuildName = " Final";

		/// <summary>The product name postfix that describes the build.</summary>
		public const string ProductBuildName = " Gamma 1''";

		/////// <summary>The product name postfix that describes the build.</summary>
		////public const string ProductBuildName = " Gamma 2 Development";

		/////// <summary>The product name postfix that describes the build.</summary>
		////public const string ProductBuildName = " Gamma 2 Preliminary";

		/// <summary>The product name including the build description.</summary>
		public static readonly string ProductNameAndBuildName = ProductName + ProductBuildName;

		/// <summary>The product version.</summary>
		public static readonly string ProductVersion = Application.ProductVersion;

		/// <summary>The complete product name including build description and version.</summary>
		public static readonly string ProductNameAndBuildNameAndVersion = ProductNameAndBuildName + " Version " + ProductVersion;

		/// <summary>The long variant of the complete product name including build description and version.</summary>
		public static readonly string ProductNameLong = ProductName + " - Yet Another Terminal";

		/// <summary>The complete logo (text) of the product.</summary>
		[SuppressMessage("Microsoft.Security", "CA2105:ArrayFieldsShouldNotBeReadOnly", Justification = "Nobody will modify this array, don't worry...")]
		public static readonly string[] ProductLogo =
		{
			ProductNameLong + ".",
			"Engineering, testing and debugging of serial communications.",
			"Supports RS-232/422/423/485...",
			"...as well as TCP-Client/Server/AutoSocket, UDP and USB Ser/HID",
			"",
			"Visit YAT at http://sourceforge.net/projects/y-a-terminal.",
			"Contact YAT by mailto:y-a-terminal@users.sourceforge.net.",
			"",
			"Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.",
			"Copyright © 2003-2015 Matthias Kläy.",
			"All rights reserved.",
			"",
			"YAT is licensed under the GNU LGPL.",
			"See http://www.gnu.org/licenses/lgpl.html for license details."
		};
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
