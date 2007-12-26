using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace YAT.Utilities
{
	public static class ApplicationInfo
	{
		//public const string ProductNamePostFix = "";
		//public const string ProductNamePostFix = " Beta 2";
		//public const string ProductNamePostFix = " Beta 2 Candidate 1";
		public const string ProductNamePostFix = " Beta 2 Preliminary";

		public static readonly string ProductName = Application.ProductName + ProductNamePostFix;
	}
}
