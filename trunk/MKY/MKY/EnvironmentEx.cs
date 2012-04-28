//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MKY
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class EnvironmentEx
	{
		/// <summary>
		/// Constant string to use 'NewLine' in places where <see cref="Environment.NewLine"/>
		/// cannot be used, e.g. in case of attribute arguments.
		/// </summary>
		public const string NewLineConstWorkaround = "\n";

		/// <summary>
		/// Returns <c>true</c> if operating system is Win32 or Win64 or compatible.
		/// </summary>
		public static bool IsWindows
		{
			get
			{
				switch (Environment.OSVersion.Platform)
				{
					case PlatformID.Win32S:
					case PlatformID.Win32Windows:
					case PlatformID.Win32NT: // Also covers Win64!
						return (true);

					case PlatformID.WinCE:
					case PlatformID.Unix:
					case PlatformID.Xbox:
					case PlatformID.MacOSX:
					default:
						return (false);
				}
			}
		}

		/// <summary>
		/// Returns <c>true</c> if operating system is Win64 or compatible.
		/// </summary>
		public static bool IsWindows64
		{
			get
			{
				return (IsWindows && (IntPtr.Size == 8));
			}
		}

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
