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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

namespace MKY.Windows.Forms
{
	#region Enum ModalBehavior

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	public enum ModalBehavior
	{
		Never,
		Always,

		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "NonUser", Justification = "What's the problem...")]
		InCaseOfNonUserError,

		OnlyInCaseOfUserInteraction
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// This <see cref="System.Windows.Forms"/> contract attribute can be used to emphasize that a
	/// method is always or sometimes shown modal. The attribute can then be used to find all modal
	/// and potentially modal locations within a the application.
	/// </summary>
	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ModalBehaviorContractAttribute : Attribute
	{
		/// <summary></summary>
		public ModalBehavior Behavior { get; }

		/// <summary></summary>
		public string Approval { get; set; }

		/// <summary></summary>
		public ModalBehaviorContractAttribute(ModalBehavior behavior)
		{
			Behavior = behavior;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
