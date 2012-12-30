﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

#region Module-level StyleCop suppressions
//==================================================================================================
// Module-level StyleCop suppressions
//==================================================================================================

[module: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1404:CodeAnalysisSuppressionMustHaveJustification", Justification = "Large blocks of module-level FxCop suppressions which were copy-pasted out of FxCop.")]

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Constants+NativeConstants.#WAIT_OBJECT_0")]
[module: SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Scope = "member", Target = "MKY.Win32.Constants+NativeConstants.#WAIT_TIMEOUT")]

// Justification = "Naming is defined by the Win32 API."
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Constants+NativeConstants.#WAIT_OBJECT_0", MessageId = "OBJECT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Constants+NativeConstants.#WAIT_OBJECT_0", MessageId = "WAIT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Constants+NativeConstants.#WAIT_TIMEOUT", MessageId = "TIMEOUT")]
[module: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "MKY.Win32.Constants+NativeConstants.#WAIT_TIMEOUT", MessageId = "WAIT")]

#endregion

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates general constants of the Win32 API.
	/// </summary>
	public static class Constants
	{
		#region Native
		//==========================================================================================
		// Native
		//==========================================================================================

		#region Native > Constants
		//------------------------------------------------------------------------------------------
		// Native > Constants
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Native items are nested on purpose, to emphasize their native nature.")]
		public static class NativeConstants
		{
			/// <summary></summary>
			public const int WAIT_TIMEOUT = 0x0102;
		
			/// <summary></summary>
			public const int WAIT_OBJECT_0 = 0;
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
