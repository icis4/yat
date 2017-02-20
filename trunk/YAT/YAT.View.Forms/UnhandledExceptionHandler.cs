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
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
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
		private static Type staticExceptionTypeToIgnore; // = null;

		/// <summary>
		/// Evaluates whether the given <paramref name="type"/> is currently being ignored.
		/// </summary>
		/// <remarks>
		/// Used to prevent that multiple unhandled exceptions also generate multiple unhandled exception dialogs.
		/// Any exception of the same type of a type that <see cref="Type.IsAssignableFrom(Type)"/> will be ignored.
		/// </remarks>
		public static bool ExceptionTypeIsIgnored(Type type)
		{
			if (staticExceptionTypeToIgnore != null)
				return (staticExceptionTypeToIgnore.IsAssignableFrom(type));
			else
				return (false);
		}

		/// <summary>
		/// Resets the ignored exception type.
		/// </summary>
		public static void ResetIgnoredExceptionType()
		{
			staticExceptionTypeToIgnore = null;
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
			if (exception.GetType().IsAssignableFrom(staticExceptionTypeToIgnore))
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
					"Select [Cancel/Abort] to exit " + productName + " now." + Environment.NewLine +
					"Or would you like to continue and [Retry] anyway?" + Environment.NewLine +
					"Or would you like to continue but [Ignore] such unhandled exceptions?";

				UnhandledExceptionResult result;
				switch (MessageBoxEx.Show(owner, message, title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation))
				{
					case DialogResult.Retry:                                                     result = UnhandledExceptionResult.Continue; break;
					case DialogResult.Ignore: staticExceptionTypeToIgnore = exception.GetType(); result = UnhandledExceptionResult.Continue; break; // Intentionally ignore further exceptions.
					default:                  staticExceptionTypeToIgnore = typeof(Exception);   result = UnhandledExceptionResult.Exit;     break; // Don't care about any exceptions anymore.
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

				staticExceptionTypeToIgnore = typeof(Exception); // Ensure that no more exceptions are shown while exiting.
				return (result);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
