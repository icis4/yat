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
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

namespace YAT.Model
{
	/// <summary></summary>
	public static class TerminalIds
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
		/// IDs are 1 (not 0) based for consistency with "Terminal1"...
		/// </remarks>
		public const int FirstDynamicId = 1;

		/// <remarks>
		/// IDs are 1 (not 0) based for consistency with "Terminal1"...
		/// ID 0 refers to the active terminal, i.e. the 'default' terminal.
		/// </remarks>
		public const int ActiveDynamicId = 0;

		/// <remarks>
		/// IDs are 1 (not 0) based for consistency with "Terminal1"...
		/// ID -1 means 'invalid', i.e. no terminal.
		/// </remarks>
		public const int InvalidDynamicId = -1;

		/// <remarks>
		/// IDs are 1 (not 0) based for consistency with "Terminal1"...
		/// </remarks>
		public const int FirstSequentialId = 1;

		/// <remarks>
		/// IDs are 1 (not 0) based for consistency with "Terminal1"...
		/// ID 0 refers to the active terminal, i.e. the 'default' terminal.
		/// </remarks>
		public const int ActiveSequentialId = 0;

		/// <remarks>
		/// IDs are 1 (not 0) based for consistency with "Terminal1"...
		/// ID -1 means 'invalid', i.e. no terminal.
		/// </remarks>
		public const int InvalidSequentialId = -1;

		/// <remarks>
		/// IDs are 1 (not 0) based for consistency with "Terminal1"...
		/// </remarks>
		public const int FirstFixedId = Settings.TerminalSettingsItem.FirstFixedId;

		/// <remarks>
		/// IDs are 1 (not 0) based for consistency with "Terminal1"...
		/// ID 0 refers to the active terminal, i.e. the 'default' terminal.
		/// </remarks>
		public const int ActiveFixedId = Settings.TerminalSettingsItem.ActiveFixedId;

		/// <remarks>
		/// IDs are 1 (not 0) based for consistency with "Terminal1"...
		/// ID -1 means 'invalid', i.e. no terminal.
		/// </remarks>
		public const int InvalidFixedId = Settings.TerminalSettingsItem.InvalidFixedId;

		/// <summary>
		/// Returns the 'normal' index of the given dynamic ID.
		/// </summary>
		public static int IndexToDynamicId(int index)
		{
			if (index >= FirstIndex)
				return (FirstDynamicId + (index - FirstIndex));
			else
				return (InvalidDynamicId);
		}

		/// <summary>
		/// Returns the dynamic ID of the given 'normal' index.
		/// </summary>
		public static int DynamicIdToIndex(int dynamicId)
		{
			if (dynamicId >= FirstDynamicId)
				return (FirstIndex + (dynamicId - FirstDynamicId));
			else
				return (InvalidIndex);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
