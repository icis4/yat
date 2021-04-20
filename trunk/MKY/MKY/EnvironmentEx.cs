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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2021 Matthias Kläy.
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
		/// Constant string to use "NewLine" in places where <see cref="Environment.NewLine"/>
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
		/// Tries to get value from environment variable.
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
		/// Tries to get file path from environment variable and verify.
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

		/// <summary>
		/// Tries to open/start the given file with the system's default application.
		/// </summary>
		/// <remarks>
		/// Named 'Start' rather than 'Open' for not confusing with e.g. <see cref="File.Open(string, FileMode)"/>.
		/// </remarks>
		/// <param name="filePath">File to open/start.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool TryStartFile(string filePath)
		{
			Exception exceptionOnFailure;
			return (TryStartFile(filePath, out exceptionOnFailure));
		}

		/// <summary>
		/// Tries to open/start the given file with the system's default application.
		/// </summary>
		/// <remarks>
		/// Named 'Start' rather than 'Open' for not confusing with e.g. <see cref="File.Open(string, FileMode)"/>.
		/// </remarks>
		/// <param name="filePath">File to open/start.</param>
		/// <param name="exceptionOnFailure">Exception object, in case of failure.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static bool TryStartFile(string filePath, out Exception exceptionOnFailure)
		{
			try
			{
				Process.Start(filePath);
				exceptionOnFailure = null;
				return (true);
			}
			catch (Exception ex)
			{
				exceptionOnFailure = ex;
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
