using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.Utilities.Types
{
	/// <summary>
	/// Decimal utility methods.
	/// </summary>
	public static class XDecimal
	{
		/// <summary>
		/// Limits "value" to the boundaries specified.
		/// </summary>
		public static decimal LimitToBounds(decimal value, decimal lower, decimal upper)
		{
			if (value < lower)
				return (lower);
			if (value > upper)
				return (upper);
			return (value);
		}
	}
}
