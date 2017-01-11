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
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the YAT namespace even though the file is located in
// YAT.Application since the class name already contains 'Application'.
namespace YAT
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public static class ApplicationEx
	{
		/// <summary>
		/// Constant string to expand the application's product name in places where neither
		/// <see cref="ProductName"/> nor <see cref="System.Windows.Forms.Application.ProductName"/>
		/// can be used, e.g. in case of attribute arguments.
		/// </summary>
		public const string ProductNameConstWorkaround = "YAT";

		/// <summary>The product name.</summary>
		public static readonly string ProductName = System.Windows.Forms.Application.ProductName;

		/////// <summary>The product name postfix that describes the build.</summary>
		////public static readonly string ProductBuildName = " Final";

		/// <summary>The product name postfix that describes the build.</summary>
		public static readonly string ProductBuildName = " Gamma 2''";

		/////// <summary>The product name postfix that describes the build.</summary>
		////public static readonly string ProductBuildName = " Gamma 2 Development";

		/////// <summary>The product name postfix that describes the build.</summary>
		////public static readonly string ProductBuildName = " Gamma 2 Preliminary";

		/// <summary>The product name including the build description.</summary>
		public static readonly string ProductNameAndBuildName = ProductName + ProductBuildName;

		/// <summary>The product version.</summary>
		public static readonly string ProductVersion = System.Windows.Forms.Application.ProductVersion;

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
			"Supports RS-232/422/423/485 as well as...",
			"...TCP/IP Client/Server/AutoSocket,...",
			"...UDP/IP Client/Server/PairSocket and...",
			"...USB Ser/HID.",
			"",
			"Visit YAT at https://sourceforge.net/projects/y-a-terminal.",
			"Contact YAT by mailto:y-a-terminal@users.sourceforge.net.",
			"",
			"Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.",
			"Copyright © 2003-2017 Matthias Kläy.",
			"All rights reserved.",
			"",
			"YAT is licensed under the GNU LGPL.", // Note that source files state "This source code is licensed under the GNU LGPL." to emphasize the context.
			"See http://www.gnu.org/licenses/lgpl.html for license details."
		};
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
