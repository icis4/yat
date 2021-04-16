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
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using MKY.Drawing;

using NUnit.Framework;

namespace MKY.Test.Drawing
{
	/// <summary></summary>
	[TestFixture]
	public class FontExTest
	{
		#region Tests
		//==========================================================================================
		// Test
		//==========================================================================================

		#region Tests > IsMonospaced()
		//------------------------------------------------------------------------------------------
		// Tests > IsMonospaced()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void IsMonospaced()
		{
			Assert.That(FontEx.IsMonospaced("Arial"), Is.False);
			Assert.That(FontEx.IsMonospaced("Courier New"), Is.True);
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
