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
// Copyright © 2010-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.Contracts
{
	/// <summary>
	/// Attribute to indicate the nature of a class or struct.
	/// </summary>
	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
	public sealed class ImmutableContractAttribute : Attribute
	{
		/// <summary>
		/// Indicates that the class or struct is immutable.
		/// </summary>
		public bool IsImmutable { get; }

		/// <summary></summary>
		public ImmutableContractAttribute(bool isImmutable)
		{
			IsImmutable = isImmutable;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
