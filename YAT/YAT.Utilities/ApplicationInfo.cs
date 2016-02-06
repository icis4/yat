﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
	public static class ApplicationInfo
	{
		/// <summary>
		/// Constant string to expand the application's product name in places where neither
		/// <see cref="ProductName"/> nor <see cref="Application.ProductName"/> can be used,
		/// e.g. in case of attribute arguments.
		/// </summary>
		public const string ProductNameConstWorkaround = "YAT";

		/// <summary>The product name.</summary>
		public static readonly string ProductName = Application.ProductName;

		/////// <summary>The product name postfix that describes the build.</summary>
		////public static readonly string ProductBuildName = " Final";

		/// <summary>The product name postfix that describes the build.</summary>
		////public static readonly string ProductBuildName = " Gamma 2";

		/////// <summary>The product name postfix that describes the build.</summary>
		public static readonly string ProductBuildName = " Gamma 2 Development";

		/////// <summary>The product name postfix that describes the build.</summary>
		////public static readonly string ProductBuildName = " Gamma 2 Preliminary";

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
			"Visit YAT at https://sourceforge.net/projects/y-a-terminal.",
			"Contact YAT by mailto:y-a-terminal@users.sourceforge.net.",
			"",
			"Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.",
			"Copyright © 2003-2016 Matthias Kläy.",
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
