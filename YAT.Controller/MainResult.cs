using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Controller
{
	/// <summary>
	/// Enummeration of all the main result return codes.
	/// </summary>
	public enum MainResult
	{
		OK = 0,
		ApplicationSettingsError = -1,
		CommandLineArgsError = -2,
		UnhandledException = -3,
	}
}
