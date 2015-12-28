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
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

// This code is intentionally placed into the MKY namespace even though the file is located in
// MKY.Types for consistency with the Sytem namespace.
namespace MKY
{
	/// <summary>
	/// Int32/int utility methods.
	/// </summary>
	/// <remarks>
	/// Possible extensions:
	/// - ParseInside: get integer values inside strings (e.g. "COM5 (Device123B)" returns {5;123})
	/// 
	/// Saying hello to StyleCop ;-.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Int32 just *is* 'int'...")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class Int32Ex
	{
		/// <summary>
		/// Limits <paramref name="value"/> to the boundaries specified.
		/// </summary>
		public static int LimitToBounds(int value, int lower, int upper)
		{
			if (value < lower)
				return (lower);

			if (value > upper)
				return (upper);

			return (value);
		}

		/// <summary>
		/// Returns whether <paramref name="value"/> is within the boundaries specified.
		/// </summary>
		public static bool IsWithin(int value, int lower, int upper)
		{
			return ((value >= lower) && (value <= upper));
		}

		/// <summary>
		/// Returns whether <paramref name="value"/> is even.
		/// </summary>
		public static bool IsEven(int value)
		{
			return ((value % 2) == 0);
		}

		/// <summary>
		/// Returns whether <paramref name="value"/> is odd.
		/// </summary>
		public static bool IsOdd(int value)
		{
			return ((value % 2) != 0);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
