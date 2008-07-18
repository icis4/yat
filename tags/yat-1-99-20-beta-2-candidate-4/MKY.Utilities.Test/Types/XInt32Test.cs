using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using MKY.Utilities.Types;

namespace MKY.Utilities.Test.Types
{
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

		private readonly TestSet[] _testSets =
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

		#region Tests > Min()
		//------------------------------------------------------------------------------------------
		// Tests > Min()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestMin()
		{
			foreach (TestSet ts in _testSets)
			{
				Assert.AreEqual(ts.Min, XInt32.Min(ts.Min, ts.Max));
				Assert.AreEqual(ts.Min, XInt32.Min(ts.Max, ts.Min));
			}
		}

		#endregion

		#region Tests > Max()
		//------------------------------------------------------------------------------------------
		// Tests > Max()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestMax()
		{
			foreach (TestSet ts in _testSets)
			{
				Assert.AreEqual(ts.Max, XInt32.Max(ts.Min, ts.Max));
				Assert.AreEqual(ts.Max, XInt32.Max(ts.Max, ts.Min));
			}
		}

		#endregion

		#region Tests > LimitToBounds()
		//------------------------------------------------------------------------------------------
		// Tests > LimitToBounds()
		//------------------------------------------------------------------------------------------

		[Test]
		public void TestLimitToBounds()
		{
			foreach (TestSet ts in _testSets)
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
