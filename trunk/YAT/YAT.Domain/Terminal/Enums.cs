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
// YAT Version 2.1.1 Development
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

using System.Diagnostics.CodeAnalysis;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum RepositoryType
	{
		None,

		Tx,
		Bidir,
		Rx
	}

	/// <summary></summary>
	public enum SendMode
	{
		Text,
		File
	}

	/// <summary></summary>
	public enum LinePosition
	{
		Begin,
		Content,
		ContentExceeded,
		End
	}


	/// <remarks>
	/// So far there can only be one attribute, thus named "Attribute" and not marked [Flags].
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Enum actually implements an attribute, an attribute related to display elements.")]
	public enum LineChunkAttribute
	{
		None = 0,

		/// <summary>Resulting line shall be highlighted.</summary>
		Highlight,

		/// <summary>Filtering is active; resulting line shall be included.</summary>
		Filter,

		/// <summary>Filtering is active; resulting line may be excluded.</summary>
		SuppressIfNotFiltered,

		/// <summary>Suppressing is active; resulting line may be excluded.</summary>
		SuppressIfSubsequentlyTriggered,

		/// <summary>Suppressing is active; resulting line shall be excluded.</summary>
		Suppress
	}

	#pragma warning restore 1591
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
