//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

namespace MKY.Test.Equality
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Internal configuration to enable/disable tracing, must be a field to prevent compilation warnings.")]
	public static class Configuration
	{
		/// <summary>
		/// Choose whether the calling sequence shall be output onto the trace console.
		/// </summary>
		/// <remarks>
		/// Must be a variable to prevent "Unreachable code detected" warnings where used.
		/// </remarks>
		public static bool TraceCallingSequence = false;
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
