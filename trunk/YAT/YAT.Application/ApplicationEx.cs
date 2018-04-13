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
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
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

	/////// <summary>The product name postfix that describes the build.</summary>
	////public static readonly string ProductBuildName = " Almost Final";

	/////// <summary>The product name postfix that describes the build.</summary>
	////public static readonly string ProductBuildName = " Epsilon";

	/////// <summary>The product name postfix that describes the build.</summary>
	////public static readonly string ProductBuildName = " Epsilon Preliminary";

	/////// <summary>The product name postfix that describes the build.</summary>
	////public static readonly string ProductBuildName = " Epsilon Development";

	/////// <summary>The product name including the build description.</summary>
	////public static readonly string ProductNameAndBuildName = ProductName + ProductBuildName;

		/// <summary>The product version.</summary>
		public static readonly string ProductVersion = System.Windows.Forms.Application.ProductVersion;

	/////// <summary>The product name and its version.</summary>
	////public static readonly string ProductNameAndVersion = ProductName + " Version " + ProductVersion;

		/// <summary>The product name and its version.</summary>
		public static readonly string ProductNameAndVersion = ProductName + " Version " + ProductVersion + " Development";

	/////// <summary>The complete product name including build description and its version.</summary>
	////public static readonly string ProductNameAndBuildNameAndVersion = ProductNameAndBuildName + " Version " + ProductVersion;

		/// <summary>The long variant of the product name.</summary>
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
			"Copyright © 2003-2018 Matthias Kläy.",
			"All rights reserved.",
			"",
			"YAT is licensed under the GNU LGPL.", // Note that source files state "This source code is licensed under the GNU LGPL." to emphasize the context.
			"See http://www.gnu.org/licenses/lgpl.html for license details."
		};

		/// <summary>The .NET Framework prerequisite.</summary>
		public static readonly string PrerequisiteFramework = ".NET Framework 3.5 Service Pack 1";

		/// <summary>The Windows operating system prerequisite.</summary>
		public static readonly string PrerequisiteWindowsOS = "Windows 2000 or later";

		/// <summary>The other operating system prerequisite.</summary>
		public static readonly string PrerequisiteOtherOS = "Linux with Mono 3.12.x or later";
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
