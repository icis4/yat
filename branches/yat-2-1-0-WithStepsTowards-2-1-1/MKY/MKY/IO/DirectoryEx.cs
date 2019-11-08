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
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace MKY.IO
{
	/// <summary>
	/// Utility methods for <see cref="System.IO.Directory"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class DirectoryEx
	{
		/// <summary>
		/// Determines whether the given path can be written to.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public static bool IsWritable(string path)
		{
			// \remind (2018-01-05 / MKY) to be changed as soon as upgraded to .NET 4+
		////var permissionSet = new PermissionSet(PermissionState.None);
		////var writePermission = new FileIOPermission(FileIOPermissionAccess.Write, path);
		////permissionSet.AddPermission(writePermission);
		////return (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet));

			DirectorySecurity acl;
			try {
				acl = Directory.GetAccessControl(path);
			}
			catch {
				return (false);
			}

			if (acl == null) {
				return (false);
			}

			AuthorizationRuleCollection rules;
			try {
				rules = acl.GetAccessRules(true, true, typeof(SecurityIdentifier));
			}
			catch {
				return (false);
			}

			if (rules == null) {
				return (false);
			}

			bool allow = false;
			bool deny  = false;

			foreach (FileSystemAccessRule rule in rules)
			{
				if ((rule.FileSystemRights & FileSystemRights.Write) == 0)
					continue; // Ignore other rules than 'Write'.

				switch (rule.AccessControlType)
				{
					case AccessControlType.Allow: allow = true; break;
					case AccessControlType.Deny:  deny  = true; break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + rule.AccessControlType.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			return (allow && !deny);
		}

		/// <summary>
		/// Tries to open the given path with the system's file browser/explorer.
		/// </summary>
		/// <remarks>
		/// Named "browse" instead of "open" to emphasize that system browser/explorer is used.
		/// </remarks>
		/// <param name="path">Directory to browse.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool TryBrowse(string path)
		{
			Exception exception;
			return (TryBrowse(path, out exception));
		}

		/// <summary>
		/// Tries to open the given path with the system's file browser/explorer.
		/// </summary>
		/// <remarks>
		/// Named "browse" instead of "open" to emphasize that system browser/explorer is used.
		/// </remarks>
		/// <param name="path">Directory to browse.</param>
		/// <param name="exception">Exception object, in case of failure.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static bool TryBrowse(string path, out Exception exception)
		{
			try
			{
				Process.Start(path);
				exception = null;
				return (true);
			}
			catch (Exception ex)
			{
				exception = ex;
				return (false);
			}
		}

		/// <summary>
		/// Makes all files within a directory writable, including or excluding sub-directories.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static void MakeAllFilesWritable(string path, bool recursive = true)
		{
			if (Directory.Exists(path))
			{
				// Recurse into sub-directories:
				if (recursive)
				{
					foreach (string directoryName in Directory.GetDirectories(path))
						MakeAllFilesWritable(Path.Combine(path, directoryName), recursive); // Recursion!
				}

				// Make files of this directory writable:
				foreach (string fileName in Directory.GetFiles(path))
				{
					File.SetAttributes(Path.Combine(path, fileName), FileAttributes.Normal);
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
