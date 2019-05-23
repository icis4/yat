﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Delta Version 1.99.80
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

namespace YAT.Model
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Indices", Justification = "'Indices' is a correct English term and used throughout the .NET framework.")]
	public static class Indices
	{
		/// <remarks>
		/// 'Normal' indices are 0 based.
		/// </remarks>
		public const int FirstIndex = 0;

		/// <remarks>
		/// 'Normal' indices are 0 based.
		/// Index -1 means 'invalid'.
		/// </remarks>
		public const int InvalidIndex = -1;

		/// <remarks>
		/// Indices are 1 (not 0) based for consistency with "Terminal1"...
		/// </remarks>
		public const int FirstDynamicIndex = 1;

		/// <remarks>
		/// Indices are 1 (not 0) based for consistency with "Terminal1"...
		/// Index 0 means 'default' = the active terminal.
		/// </remarks>
		public const int DefaultDynamicIndex = 0;

		/// <remarks>
		/// Indices are 1 (not 0) based for consistency with "Terminal1"...
		/// Index -1 means 'invalid'.
		/// </remarks>
		public const int InvalidDynamicIndex = -1;

		/// <remarks>
		/// Indices are 1 (not 0) based for consistency with "Terminal1"...
		/// </remarks>
		public const int FirstSequentialIndex = 1;

		/// <remarks>
		/// Indices are 1 (not 0) based for consistency with "Terminal1"...
		/// Index 0 means 'default' = the active terminal.
		/// </remarks>
		public const int DefaultSequentialIndex = 0;

		/// <remarks>
		/// Indices are 1 (not 0) based for consistency with "Terminal1"...
		/// Index -1 means 'invalid'.
		/// </remarks>
		public const int InvalidSequentialIndex = -1;

		/// <remarks>
		/// Indices are 1 (not 0) based for consistency with "Terminal1"...
		/// </remarks>
		public const int FirstFixedIndex = Settings.TerminalSettingsItem.FirstFixedIndex;

		/// <remarks>
		/// Indices are 1 (not 0) based for consistency with "Terminal1"...
		/// Index 0 means 'default' = the active terminal.
		/// </remarks>
		public const int DefaultFixedIndex = Settings.TerminalSettingsItem.DefaultFixedIndex;

		/// <remarks>
		/// Indices are 1 (not 0) based for consistency with "Terminal1"...
		/// Index -1 means 'invalid'.
		/// </remarks>
		public const int InvalidFixedIndex = Settings.TerminalSettingsItem.InvalidFixedIndex;

		/// <summary>
		/// Returns the 'normal' index of the given dynamic index.
		/// </summary>
		public static int IndexToDynamicIndex(int index)
		{
			if (index >= FirstIndex)
				return (FirstDynamicIndex + (index - FirstIndex));
			else
				return (InvalidDynamicIndex);
		}

		/// <summary>
		/// Returns the dynamic index of the given 'normal' index.
		/// </summary>
		public static int DynamicIndexToIndex(int dynamicIndex)
		{
			if (dynamicIndex >= FirstDynamicIndex)
				return (FirstIndex + (dynamicIndex - FirstDynamicIndex));
			else
				return (InvalidIndex);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================