﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Text;
using System.Windows.Forms;

using MKY.Windows.Forms;

#endregion

namespace YAT.View.Forms
{
	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary>
	/// Result of unhandled exception operations.
	/// </summary>
	public enum UnhandledExceptionType
	{
		Synchronous,
		AsynchronousSynchronized,
		AsynchronousNonSynchronized,
	}

	/// <summary>
	/// Result of unhandled exception operations.
	/// </summary>
	public enum UnhandledExceptionResult
	{
		Exit,
		ExitAndRestart,
		Continue,
	}

	#pragma warning restore 1591

	/// <summary></summary>
	public static class UnhandledExceptionHandler
	{
		/// <summary>
		/// Used to prevent that multiple unhandled exceptions also generate multiple unhandled exception dialogs.
		/// </summary>
		private static bool staticHandleExceptions = true;

		/// <summary></summary>
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static UnhandledExceptionResult ProvideExceptionToUser(string originMessage, UnhandledExceptionType exceptionType, bool mayBeContinued)
		{
			return (ProvideExceptionToUser(null, null, originMessage, exceptionType, mayBeContinued));
		}

		/// <summary></summary>
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static UnhandledExceptionResult ProvideExceptionToUser(Exception exception, string originMessage, UnhandledExceptionType exceptionType, bool mayBeContinued)
		{
			return (ProvideExceptionToUser(null, exception, originMessage, exceptionType, mayBeContinued));
		}

		/// <summary></summary>
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static UnhandledExceptionResult ProvideExceptionToUser(IWin32Window owner, Exception exception, string originMessage, UnhandledExceptionType exceptionType, bool mayBeContinued)
		{
			if (System.Windows.Forms.Application.OpenForms.Count > 0)
			{
				Form f = System.Windows.Forms.Application.OpenForms[0];
				if (f.InvokeRequired)
				{
					ProvideExceptionToUserDelegate invoker = new ProvideExceptionToUserDelegate(ProvideExceptionToUserInvocation);
					return ((UnhandledExceptionResult)f.Invoke(invoker, owner, exception, originMessage, exceptionType, mayBeContinued));
				}
			}
			return (ProvideExceptionToUserInvocation(owner, exception, originMessage, exceptionType, mayBeContinued));
		}

		private delegate UnhandledExceptionResult ProvideExceptionToUserDelegate(IWin32Window owner, Exception exception, string originMessage, UnhandledExceptionType exceptionType, bool mayBeContinued);

		private static UnhandledExceptionResult ProvideExceptionToUserInvocation(IWin32Window owner, Exception exception, string originMessage, UnhandledExceptionType exceptionType, bool mayBeContinued)
		{
			if (!staticHandleExceptions)
				return (UnhandledExceptionResult.Continue);

			string productName = ApplicationEx.ProductName;

			StringBuilder titleBuilder = new StringBuilder(productName);
			{
				titleBuilder.Append(" Unhandled");
				switch (exceptionType)
				{
					case UnhandledExceptionType.Synchronous:                 titleBuilder.Append(" Synchronous");                   break;
					case UnhandledExceptionType.AsynchronousSynchronized:    titleBuilder.Append(" Asynchronous Synchronized");     break;
					case UnhandledExceptionType.AsynchronousNonSynchronized: titleBuilder.Append(" Asynchronous Non-Synchronized"); break;
				}
				titleBuilder.Append(" Exception");
			}
			string title = titleBuilder.ToString();

			if (exception != null)
			{
				string message =
					originMessage + Environment.NewLine + Environment.NewLine +
					"Show detailed information?";

				if (MessageBoxEx.Show
					(
					owner,
					message,
					title,
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Error
					)
					== DialogResult.Yes)
				{
					UnhandledException f = new UnhandledException(exception, title, originMessage);
					f.ShowDialog(owner);
				}
			}
			else
			{
				MessageBoxEx.Show
				(
					owner,
					originMessage,
					title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}

			if (mayBeContinued)
			{
				string message =
					"After an unhandled exception you are advised to exit and restart " + productName + "." + Environment.NewLine + Environment.NewLine +
					"Select cancel/abort to exit " + productName + " now." + Environment.NewLine +
					"Or would you like to continue/retry anyway?" + Environment.NewLine +
					"Or would you like to continue but ignore any additional unhandled exceptions?";

				UnhandledExceptionResult result;
				switch (MessageBoxEx.Show(owner, message, title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation))
				{
					case DialogResult.Retry:                                  result = UnhandledExceptionResult.Continue; break;
					case DialogResult.Ignore: staticHandleExceptions = false; result = UnhandledExceptionResult.Continue; break; // Intentionally ignore further exceptions.
					default:                  staticHandleExceptions = false; result = UnhandledExceptionResult.Exit;     break; // Don't care about any exceptions anymore.
				}

				if (result == UnhandledExceptionResult.Exit)
				{
					message = "Would you like to restart " + productName + " after exit?";
					switch (MessageBoxEx.Show(owner, message, productName + " Restart", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
					{
						case DialogResult.Yes: result = UnhandledExceptionResult.ExitAndRestart; break;
						default:               result = UnhandledExceptionResult.Exit;           break;
					}
				}

				return (result);
			}
			else
			{
				string message =
					"After this unhandled exception " + productName + " will have to exit." + Environment.NewLine +
					"Would you like to restart " + productName + " after exit?";

				UnhandledExceptionResult result;
				switch (MessageBoxEx.Show(owner, message, productName + " Restart", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
				{
					case DialogResult.Yes: result = UnhandledExceptionResult.ExitAndRestart; break;
					default:               result = UnhandledExceptionResult.Exit;           break;
				}

				staticHandleExceptions = false; // Ensure that no more exceptions are shown while exiting.
				return (result);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================