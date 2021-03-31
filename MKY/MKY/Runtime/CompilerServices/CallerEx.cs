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
// MKY Version 1.0.29
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

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MKY.Runtime.CompilerServices
{
	/// <summary>
	/// Helper to use <see cref="CallerMemberNameAttribute"/>, <see cref="CallerFilePathAttribute"/>
	/// and <see cref="CallerLineNumberAttribute"/> from within a method.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class CallerEx
	{
		/// <remarks><para>
		/// Considering performance as described in
		/// https://stackoverflow.com/questions/1348643/how-performant-is-stackframe
		/// and
		/// https://stackoverflow.com/questions/16101152/is-performance-hit-by-using-caller-information-attributes
		/// calling this method is preferred over using reflection.
		/// <para></para>
		/// Considering maintenance and error-proneness calling this method is preferred over using
		/// the <code>nameof()</code> operator, as that operator requires to provide the very method
		/// name, whereas with this method the method does not need any identifier.
		/// </para></remarks>
		public static string GetCallerMemberName([CallerMemberName] string callerMemberName = "")
		{
			return (callerMemberName);
		}

		/// <summary></summary>
		public static string GetCallerFilePath(string callerFilePath = "")
		{
			return (callerFilePath);
		}

		/// <summary></summary>
		public static int GetCallerLineNumber(int callerLineNumber = 0)
		{
			return (callerLineNumber);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
