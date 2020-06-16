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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Security;

namespace MKY.IO
{
	/// <summary>
	/// Utility methods for <see cref="System.IO.File"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class FileEx
	{
		/// <summary>
		/// Default extension for backup files.
		/// </summary>
		public const string BackupFileExtension = ".bak";

		/// <summary>
		/// Returns the size of the given file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>
		/// The size of the given file.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public static long Size(string filePath)
		{
			if (!File.Exists(filePath))
				return (0);

			try
			{
				FileInfo fi = new FileInfo(filePath);
				return (fi.Length);
			}
			catch
			{
				return (0);
			}
		}

		/// <summary>
		/// Determines whether the given file is readable, i.e. exists and can be accessed.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>
		/// Returns <c>true</c> if file is readable.
		/// </returns>
		public static bool IsReadable(string filePath)
		{
			if (!File.Exists(filePath))
				return (false);

			try
			{
				FileInfo fi = new FileInfo(filePath); // Probe for exceptions caught below.
				return (fi.Exists);
			}
			catch (SecurityException) // The caller does not have the required permission.
			{
				return (false);
			}
			catch (UnauthorizedAccessException) // Access to fileName is denied.
			{
				return (false);
			}
		}

		/// <summary>
		/// Determines whether the given file is read-only, i.e. exists and is read-only.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>
		/// Returns <c>true</c> if file is read-only.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public static bool IsReadOnly(string filePath)
		{
			if (!File.Exists(filePath))
				return (false);

			try
			{
				FileInfo fi = new FileInfo(filePath); // Probe for exceptions caught below.
				return (fi.Exists && fi.IsReadOnly);
			}
			catch (SecurityException) // The caller does not have the required permission.
			{
				return (false);
			}
			catch (UnauthorizedAccessException) // Access to fileName is denied.
			{
				return (false);
			}
		}

		/// <summary>
		/// Determines whether the given file path is writeable, i.e. is not read-only or the file doesn't exist yet.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>
		/// Returns <c>true</c> if file is writeable.
		/// </returns>
		public static bool IsWritable(string filePath)
		{
			if (File.Exists(filePath))
				return (!IsReadOnly(filePath));
			else
				return (PathEx.IsValid(filePath));
		}

		/// <summary>
		/// Determines whether the given file is findable, e.g. via the system's PATH variable.
		/// </summary>
		/// <param name="fileName">The file name, typically an executable.</param>
		/// <returns>
		/// Returns <c>true</c> if file is findable.
		/// </returns>
		public static bool IsFindable(string fileName)
		{
			string filePath;
			return (IsFindable(fileName, out filePath));
		}

		/// <summary>
		/// Determines whether the given file is findable, e.g. via the system's PATH variable.
		/// </summary>
		/// <param name="fileName">The file name, typically an executable.</param>
		/// <param name="filePath">The path to the file.</param>
		/// <returns>
		/// Returns <c>true</c> if file is findable.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public static bool IsFindable(string fileName, out string filePath)
		{
			try
			{
				var p = new Process();
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.FileName = "WHERE";
				p.StartInfo.Arguments = fileName;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.CreateNoWindow = true;
				p.Start();

				string whereResult = p.StandardOutput.ReadToEnd();

				p.WaitForExit();

				if (p.ExitCode == 0)
				{
					filePath = whereResult.Substring(0, whereResult.IndexOf(Environment.NewLine, StringComparison.CurrentCultureIgnoreCase));
					return (true);
				}
				else
				{
					filePath = null;
					return (false);
				}
			}
			catch
			{
				filePath = null;
				return (false);
			}
		}

		/// <summary>
		/// Tries to delete file <paramref name="filePath"/>.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if file successfully saved.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static bool TryDelete(string filePath)
		{
			// No need to check whether string is valid, 'File.Exists()' returns <c>false</c>
			// in such cases.
			if (File.Exists(filePath))
			{
				try
				{
					File.Delete(filePath);
					return (true);
				}
				catch
				{
					return (false);
				}
			}
			else
			{
				return (false);
			}
		}

		/// <summary>
		/// Swaps two existing files.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static void Swap(string filePathA, string filePathB)
		{
			string filePathTemp = PathEx.GetUniqueTempPath(); // No extension needed.

			// Step 1 (A => Temp):
			try
			{
				File.Move(filePathA, filePathTemp);
			}
			catch
			{
				throw; // Simply re-throw, no cleanup needed if step 1 throws.
			}

			// Step 2 (B => A):
			try
			{
				File.Move(filePathB, filePathA);
			}
			catch
			{
				// Best-effort cleanup (revert A):
				try { File.Move(filePathTemp, filePathA); } catch { }

				throw; // Rethrow!
			}

			// Step 3 (Temp => B):
			try
			{
				File.Move(filePathTemp, filePathB);
			}
			catch
			{
				// Best-effort cleanup (revert B and A):
				try { File.Move(filePathA,    filePathB); } catch { }
				try { File.Move(filePathTemp, filePathA); } catch { }

				throw; // Rethrow!
			}
		}

		/// <summary>
		/// Returns a unique file path for the initial file path specified, unique part is separated by optional separator string.
		/// </summary>
		/// <remarks>
		/// <see cref="PathEx.GetUniqueTempPath"/> offers a similar method using a globally unique file name.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static string GetUniqueFilePath(string initialFilePath, string separator = null)
		{
			if (File.Exists(initialFilePath)) // Name needs to be changed:
			{
				if (separator == null)
					separator = ""; // Allow '+' concatenation below.

				string extension = Path.GetExtension(initialFilePath);
				string filePathWithoutExtension = Path.ChangeExtension(initialFilePath, null);
				string uniqueFilePath;

				int i = -1; // Initialize to -1 and increment before first use.
				string postfix = "";
				do
				{
					i++; // No need to check for overflow, that is checked by the .NET runtime.
					postfix = i.ToString(CultureInfo.InvariantCulture);
					uniqueFilePath = filePathWithoutExtension + separator + postfix + extension;
				}
				while (File.Exists(uniqueFilePath));

				return (uniqueFilePath);
			}
			else // File doesn't exist, name does not need to be changed:
			{
				return (initialFilePath);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
