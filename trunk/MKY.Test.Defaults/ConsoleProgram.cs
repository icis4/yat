//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/YAT/YAT.cs $
// $Author: maettu_this $
// $Date: 2010-04-11 19:35:51 +0200 (So, 11 Apr 2010) $
// $Revision: 285 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MKY.Test.Defaults
{
	/// <summary>
	/// Creates the defaults of the test settings in the solution directory.
	/// </summary>
	/// <remarks>
	/// Sealed to prevent FxCop "CA1052:StaticHolderTypesShouldBeSealeds".
	/// </remarks>
	sealed public class ConsoleProgram
	{
		/// <remarks>
		/// Prevent FxCop "CA1053:StaticHolderTypesShouldNotHaveConstructors".
		/// </remarks>
		private ConsoleProgram()
		{
		}

		/// <summary></summary>
		[STAThread]
		public static void Main()
		{
			//List<MemberInfo> mi = SettingAttribute.Settings;
		}
	}
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/YAT/YAT.cs $
//==================================================================================================
