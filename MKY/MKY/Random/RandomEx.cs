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
// MKY Version 1.0.28 Development
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

using System;
using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Times for consistency with the System namespace.
namespace MKY
{
	/// <summary>
	/// Random utility methods.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class RandomEx
	{
		private static Random staticRandomSeed = new Random();

		/// <summary>
		/// Gets the global <see cref="Random"/> object that can be used without having to create a
		/// local <see cref="Random"/> object.
		/// </summary>
		/// <remarks>
		/// This approach also solves the issue described in <see cref="NextPseudoRandomSeed"/>.
		/// </remarks>
		public static Random GlobalObject { get; } = new Random(NextPseudoRandomSeed());

		/// <summary>
		/// This method solves an issue described in the MSDN description of <see cref="Random"/>:
		/// The default seed value is derived from the system clock and has finite resolution. As a
		/// result, different <see cref="Random"/> objects that are created in close succession by a
		/// call to the default constructor will have identical default seed values and, therefore,
		/// will produce identical sets of random numbers. This problem can be avoided by using a
		/// single <see cref="Random"/> object to generate the seed for all random generators.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "PseudoRandom", Justification = "What's the problem...")]
		public static int NextPseudoRandomSeed()
		{
			return (staticRandomSeed.Next());
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
