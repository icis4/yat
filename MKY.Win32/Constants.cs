//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.Win32
{
	/// <summary>
	/// Encapsulates general constants of the Win32 API.
	/// </summary>
	public static class Constants
	{
		/// <summary>
		/// Class encapsulating native Win32 types, constants and functions.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1121:UseBuiltInTypeAlias", Justification = "Using explicit types to emphasize the type declared by the native element.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using exact native parameter names.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Using exact native parameter names.")]
		public static class Native
		{
			/// <summary></summary>
			public const int WAIT_TIMEOUT = 0x0102;
		
			/// <summary></summary>
			public const int WAIT_OBJECT_0 = 0;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
