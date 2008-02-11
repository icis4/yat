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
		CommandLineArgsError = -1,
		ApplicationSettingsError = -2,
		UnhandledException = -3,
	}
}
