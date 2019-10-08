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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using MKY.IO;

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
		/// <remarks>
		/// "\n" should be sufficient and work in case of Windows ("\r\n") as well as Unixoids and
		/// Mac since OS X ("\n"). Other operating systems are not supported by this workaround.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Unixoids' is a proper technical term.")]
		public const string NewLineConstWorkaround = "\n";

		/// <summary>
		/// Returns <c>true</c> if operating system is any Microsoft Windows including CE or Xbox.
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
					case PlatformID.WinCE:
					case PlatformID.Xbox:
						return (true);

					case PlatformID.Unix:
					case PlatformID.MacOSX:
					default:
						return (false);
				}
			}
		}

		/// <summary>
		/// Returns <c>true</c> if operating system is Win32 or Win64 or compatible.
		/// </summary>
		public static bool IsStandardWindows
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
		public static bool IsStandardWindows64
		{
			get
			{
				return (IsStandardWindows && (IntPtr.Size == 8));
			}
		}

		/// <summary>
		/// Returns <c>true</c> if operating system is Windows Vista or later.
		/// </summary>
		public static bool IsWindowsVistaOrLater
		{
			get
			{
				if (!IsStandardWindows)
					return (false);

				// Windows Vista is version 6.0.
				Version versionXP = new Version(6, 0);

				Version environmentVersion = Environment.OSVersion.Version;
				return (environmentVersion >= versionXP);
			}
		}

		/// <summary>
		/// Returns <c>true</c> if operating system is Windows XP or later.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Xp", Justification = "Would 'IsWindowsXPOrLater' be better? Not really...")]
		public static bool IsWindowsXpOrLater
		{
			get
			{
				if (!IsStandardWindows)
					return (false);

				// Windows XP is version 5.1.
				Version versionXP = new Version(5, 1);

				Version environmentVersion = Environment.OSVersion.Version;
				return (environmentVersion >= versionXP);
			}
		}

		/// <summary>
		/// Returns <c>true</c> if operating system is Windows 98 Standard Edition.
		/// </summary>
		public static bool IsWindows98Standard
		{
			get
			{
				if (!IsStandardWindows)
					return (false);

				// Windows 98 Standard Edition is version 4.10 with a build number less than 2183.
				Version version98 = new Version(4, 10);
				Version versionAbove98 = new Version(4, 10, 2183);

				Version environmentVersion = Environment.OSVersion.Version;
				return ((environmentVersion >= version98) && (environmentVersion < versionAbove98));
			}
		}

		/// <summary>
		/// Resolves the absolute location to the given file path.
		///  - If the path is rooted, simply expand environment variables.
		///  - If the path is relative, expand environment variables and combine it with the <see cref="Environment.CurrentDirectory"/>.
		/// </summary>
		public static string ResolveAbsolutePath(string filePath)
		{
			return (ResolveAbsolutePath(filePath, Environment.CurrentDirectory));
		}

		/// <summary>
		/// Resolves the absolute location to the given file path.
		///  - If the path is rooted, simply expand environment variables.
		///  - If the path is relative, expand environment variables and combine it with the given <paramref name="rootDirectory"/>.
		/// </summary>
		public static string ResolveAbsolutePath(string filePath, string rootDirectory)
		{
			if (Path.IsPathRooted(filePath))
				return (Environment.ExpandEnvironmentVariables(filePath));
			else
				return (PathEx.CombineDirectoryAndFilePaths(rootDirectory, Environment.ExpandEnvironmentVariables(filePath)));
		}

		/// <summary>
		/// Tries the get value from environment variable.
		/// </summary>
		/// <param name="environmentVariableName">Name of the environment variable.</param>
		/// <param name="result">The resulting value.</param>
		public static bool TryGetValueFromEnvironmentVariable(string environmentVariableName, out string result)
		{
			string content = Environment.GetEnvironmentVariable(environmentVariableName);
			if (content != null)
			{
				result = content;
				return (true);
			}

			Debug.Write    ("Environment variable ");
			Debug.Write    (                      environmentVariableName);
			Debug.WriteLine(                                            " could not be retrieved.");
			Debug.WriteLine("If environment variable was added after Visual Studio had been started, close and reopen Visual Studio and retry.");

			result = "";
			return (false);
		}

		/// <summary>
		/// Tries the get file path from environment variable and verify.
		/// </summary>
		/// <param name="environmentVariableName">Name of the environment variable.</param>
		/// <param name="filePath">The file path.</param>
		public static bool TryGetFilePathFromEnvironmentVariableAndVerify(string environmentVariableName, out string filePath)
		{
			string result;
			if (TryGetValueFromEnvironmentVariable(environmentVariableName, out result))
			{                                                            // May be absolute or relative to current directory.
				if (File.Exists(Environment.ExpandEnvironmentVariables(result)))
				{
					filePath = result;
					return (true);
				}
				else
				{
					Debug.Write    ("Environment variable ");
					Debug.Write    (                      environmentVariableName);
					Debug.WriteLine(                                            " could be retrieved but file stated below doesn't exist.");
					Debug.WriteLine(result);

					filePath = null;
					return (false);
				}
			}

			// Debug message already output by TryGetValueFromEnvironmentVariable().

			filePath = null;
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
