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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

//    System is not used for making origin more obvious.
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

// This code is intentionally placed into the YAT namespace even though the file is located in
// YAT.Application since the class name already contains 'Application'.
namespace YAT
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Semantic of readonly fields is constant.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public static class ApplicationEx
	{
	#if (!WITH_SCRIPTING)
		/// <summary>The common name, always "YAT", i.e. not "YAT" *or* "YATConsole".</summary>
		public const string CommonName = "YAT";
	#else
		/// <summary>The common name, always "Albatros", i.e. not "Albatros" *or* "AlbatrosConsole".</summary>
		public const string CommonName = "Albatros"; // Note that making name 'Albatros' publicly visible doesn't matter anymore, several SourceForge tickets created by UFi contain "Albatros" anyway.
	#endif

		/// <summary>The long variant of the common name.</summary>
	#if (!WITH_SCRIPTING)
		public const string CommonNameLong = CommonName + " - Yet Another Terminal";
	#else
		public const string CommonNameLong = CommonName + " - YAT with Scripting";
	#endif

		/// <summary>
		/// Constant string to expand the application's product name in places where neither
		/// <see cref="ProductName"/> nor <see cref="System.Windows.Forms.Application.ProductName"/>
		/// can be used, e.g. in case of attribute arguments.
		/// </summary>
	#if (!WITH_SCRIPTING)
		public const string ProductNameConstWorkaround = "YAT"; // Should be "YAT" or "YATConsole", but fixed for simplicity.
	#else                       // Note that making name 'Albatros' publicly visible is doesn't matter anymore, several SourceForge tickets created by UFi contain "Albatros" anyway.
		public const string ProductNameConstWorkaround = "Albatros"; // Should be "Albatros" or "AlbatrosConsole", but fixed for simplicity.
	#endif

		/// <summary>The product name.</summary>
		public static readonly string ProductName = System.Windows.Forms.Application.ProductName;

		/// <summary>The build designation.</summary>
	#if (!WITH_SCRIPTING)
	////public const string ProductBuildDesignation = "";
		public const string ProductBuildDesignation = " 2.2.0 Beta Version";
	#else
		public const string ProductBuildDesignation = "";
	#endif

		/// <summary>The product caption that combines product name and build designation.</summary>
		public static readonly string ProductCaption = ProductName + ProductBuildDesignation;

		/// <summary>The product version.</summary>
		public static readonly string ProductVersion = System.Windows.Forms.Application.ProductVersion;

		/// <summary>
		/// The product version including <see cref="System.Version.Revision"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="ProductVersion"/> uses the <see cref="System.Reflection.AssemblyInformationalVersionAttribute"/>
		/// that hides the <see cref="System.Version.Revision"/> part.
		/// </remarks>
		public static System.Version ProductFullVersionValue
		{
			get { return (System.Reflection.Assembly.GetEntryAssembly().GetName().Version); }
		}

		/// <summary>
		/// The product version including <see cref="System.Version.Revision"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="ProductVersion"/> uses the <see cref="System.Reflection.AssemblyInformationalVersionAttribute"/>
		/// that hides the <see cref="System.Version.Revision"/> part.
		/// </remarks>
		public static string ProductFullVersion
		{
			get { return (ProductFullVersionValue.ToString()); }
		}

		/// <summary>The <see cref="System.Version.Revision"/> part of the product version.</summary>
		public static string ProductRevision
		{
			get { return (ProductFullVersionValue.Revision.ToString(CultureInfo.InvariantCulture)); } // Version identification is invariant.
		}

		/// <summary>The version designation.</summary>
		public const string ProductVersionStabilityIndication = "";
	////public const string ProductVersionStabilityIndication = " Preliminary";
	////public const string ProductVersionStabilityIndication = " Development";

		/// <summary>The product version that combines product version and version stability indication.</summary>
		public static readonly string ProductVersionWithStabilityIndication = ProductVersion + ProductVersionStabilityIndication;

		/// <summary>
		/// The product caption and version.
		/// </summary>
		/// <remarks>
		/// No longer using "Version" inbetween as many other applications which don't, e.g.
		///  > TortoiseSVN 1.9.7, Build 27907
		///  > Syncovery 7.68 build 446
		///  > FreeFileSync 9.9 [2018-03-09]
		///  > Firefox 59.0.2 (64-Bit) + Thunderbird 52.7.0 (32-Bit)
		/// Release sections in release notes no longer use "Version" either.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Syncovery' is a product name.")]
		public static readonly string ProductCaptionAndVersion = ProductCaption + " " + ProductVersionWithStabilityIndication;

		/// <summary>The complete logo (text) of the product.</summary>
		[SuppressMessage("Microsoft.Security", "CA2105:ArrayFieldsShouldNotBeReadOnly", Justification = "Nobody will modify this array, don't worry...")]
		public static readonly string[] ProductLogo =
		{
			CommonNameLong + ".", // Fixed to "YAT - Yet Another Terminal".
			"Engineering, testing and debugging of serial communications.",
			"Supports RS-232/422/423/485 as well as...",
			"...TCP/IP Client/Server/AutoSocket,...",
			"...UDP/IP Client/Server/PairSocket and...",
			"...USB Ser/HID.",
	#if (WITH_SCRIPTING)
			"Integrated C# scripting functionality.",
	#endif
			"",
			"Visit YAT at https://sourceforge.net/projects/y-a-terminal.",
	#if (!WITH_SCRIPTING)
			"Contact YAT by mailto:y-a-terminal@users.sourceforge.net.",
	#else
			"Contact Albatros by mailto:matthias.klaey@mt.com.",
	#endif
			"",
	#if (!WITH_SCRIPTING)
			"Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.",
			"Copyright © 2003-2020 Matthias Kläy.",
	#else
			"YAT copyright © 2003-2004 HSR Hochschule für Technik Rapperswil and © 2003-2020 Matthias Kläy.",
			"Albatros copyright © 2008-2020 Mettler-Toledo.",
	#endif
			"All rights reserved.",
	#if (!WITH_SCRIPTING)
			"",
			"YAT is licensed under the GNU LGPL.", // Note that source files state "This source code is licensed under the GNU LGPL." to emphasize the context.
			"See http://www.gnu.org/licenses/lgpl.html for license details."
	#endif
		};

		/// <summary>The executable name.</summary>
		public static readonly string ExecutableNameWithoutExtension = Path.GetFileNameWithoutExtension(System.Windows.Forms.Application.ExecutablePath);

		/// <summary>The .NET Framework prerequisite.</summary>
		public static readonly string PrerequisiteFramework = ".NET Framework 4.8";

		/// <summary>The Windows operating system prerequisite.</summary>
		public static readonly string PrerequisiteWindows = "Windows 7 SP1 or Windows Server 2008 R2 SP1 or later";

		/// <summary>The Linux operating system prerequisite.</summary>
		public static readonly string PrerequisiteLinux = "Linux with Mono 4.8.0 or later";
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
