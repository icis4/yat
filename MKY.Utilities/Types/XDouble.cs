using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Types
{
	/// <summary>
	/// Double utility methods.
	/// </summary>
	public static class XDouble
	{
		/// <summary>
		/// Limits "value" to the boundaries specified.
		/// </summary>
		public static double LimitToBounds(double value, double lower, double upper)
		{
			if (value < lower)
				return (lower);
			if (value > upper)
				return (upper);
			return (value);
		}
	}
}
