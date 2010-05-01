//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.IO;

namespace MKY.Utilities
{
	/// <summary></summary>
	public static class XEnvironment
	{
		/// <summary>
		/// Tries the get value from environment variable.
		/// </summary>
		/// <param name="environmentVariableName">Name of the environment variable.</param>
		/// <param name="value">The value.</param>
		public static bool TryGetValueFromEnvironmentVariable(string environmentVariableName, out string value)
		{
			string content = Environment.GetEnvironmentVariable(environmentVariableName);
			if (content != null)
			{
				value = content;
				return (true);
			}

			Debug.Write("Environment variable ");
			Debug.Write(                      environmentVariableName);
			Debug.WriteLine(                                        " could not be retrieved.");
			Debug.WriteLine("If environment variable was added after Visual Studio had been started, close and reopen Visual Studio and retry.");

			value = "";
			return (false);
		}

		/// <summary>
		/// Tries the get file path from environment variable and verify.
		/// </summary>
		/// <param name="environmentVariableName">Name of the environment variable.</param>
		/// <param name="filePath">The file path.</param>
		public static bool TryGetFilePathFromEnvironmentVariableAndVerify(string environmentVariableName, out string filePath)
		{
			string value;
			if (TryGetValueFromEnvironmentVariable(environmentVariableName, out value))
			{
				if (File.Exists(value))
				{
					filePath = value;
					return (true);
				}
			}
			filePath = "";
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
