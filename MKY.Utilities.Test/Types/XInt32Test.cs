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
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MKY.Utilities.Types;

namespace MKY.Utilities.Test.Types
{
	/// <summary></summary>
	[TestFixture]
	public class XInt32Test
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private struct TestSet
		{
			public readonly int Min;
			public readonly int Max;
			public readonly int Value;
			public readonly bool ValueMinimized;
			public readonly bool ValueMaximized;

			public TestSet(int min, int max, int value, bool valueMinimized, bool valueMaximized)
			{
				Min = min;
				Max = max;
				Value = value;
				ValueMinimized = valueMinimized;
				ValueMaximized = valueMaximized;
			}
		};

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private readonly TestSet[] testSets =
		{
			new TestSet(  0,  0, -1,  true,  true ),
			new TestSet(  0,  0,  0,  true,  true ),
			new TestSet(  0,  0,  1,  true,  true ),

			new TestSet(  0,  1, -1,  true, false ),
			new TestSet(  0,  1,  0,  true, false ),
			new TestSet(  0,  1,  1, false,  true ),
			new TestSet(  0,  1,  2, false,  true ),

			new TestSet( -1,  0, -2,  true, false ),
			new TestSet( -1,  0, -1,  true, false ),
			new TestSet( -1,  0,  0, false,  true ),
			new TestSet( -1,  0,  1, false,  true ),

			new TestSet( -1,  1, -2,  true, false ),
			new TestSet( -1,  1, -1,  true, false ),
			new TestSet( -1,  1,  0, false, false ),
			new TestSet( -1,  1,  1, false,  true ),
			new TestSet( -1,  1,  2, false,  true ),

			new TestSet( int.MinValue, int.MaxValue,            0, false, false ),
			new TestSet( int.MinValue, int.MaxValue, int.MinValue,  true, false ),
			new TestSet( int.MinValue, int.MaxValue, int.MaxValue, false,  true ),

			new TestSet( int.MinValue, int.MaxValue, int.MinValue + 1, false, false ),
			new TestSet( int.MinValue, int.MaxValue, int.MaxValue - 1, false, false ),

			new TestSet( int.MinValue + 1, int.MaxValue - 1, int.MinValue    ,  true, false ),
			new TestSet( int.MinValue + 1, int.MaxValue - 1, int.MinValue + 1,  true, false ),
			new TestSet( int.MinValue + 1, int.MaxValue - 1, int.MinValue + 2, false, false ),
			new TestSet( int.MinValue + 1, int.MaxValue - 1, int.MaxValue - 2, false, false ),
			new TestSet( int.MinValue + 1, int.MaxValue - 1, int.MaxValue - 1, false,  true ),
			new TestSet( int.MinValue + 1, int.MaxValue - 1, int.MaxValue    , false,  true ),
		};

		#endregion

		#region Tests > LimitToBounds()
		//------------------------------------------------------------------------------------------
		// Tests > LimitToBounds()
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[Test]
		public virtual void TestLimitToBounds()
		{
			foreach (TestSet ts in this.testSets)
			{
				int limited = XInt32.LimitToBounds(ts.Value, ts.Min, ts.Max);

				if (ts.ValueMinimized)
					Assert.AreEqual(ts.Min, limited);

				if (ts.ValueMaximized)
					Assert.AreEqual(ts.Max, limited);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
