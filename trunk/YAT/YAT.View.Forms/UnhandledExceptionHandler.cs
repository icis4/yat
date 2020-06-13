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
// YAT Version 2.2.0 Development
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Windows.Forms;

using MKY;
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
		AsynchronousNonSynchronized
	}

	/// <summary>
	/// Result of unhandled exception operations.
	/// </summary>
	public enum UnhandledExceptionResult
	{
		Exit,
		ExitAndRestart,
		Continue,
		ContinueAndIgnore
	}

	#pragma warning restore 1591

	/// <summary></summary>
	public static class UnhandledExceptionHandler
	{
		#region ExceptionHelper
		//==========================================================================================
		// ExceptionHelper
		//==========================================================================================

		private static ExceptionHelper staticExceptionHelper = new ExceptionHelper(typeof(UnhandledExceptionHandler).FullName);

		/// <summary>
		/// Ignores the given exception type.
		/// </summary>
		public static void IgnoreExceptionType(Type type)
		{
			staticExceptionHelper.IgnoreExceptionType(type);
		}

		/// <summary>
		/// No longer ignores the given exception type.
		/// </summary>
		public static void RevertIgnoredExceptionType(Type type)
		{
			staticExceptionHelper.RevertIgnoredExceptionType(type);
		}

		/// <summary>
		/// Evaluates whether the given <paramref name="type"/> is currently being ignored.
		/// </summary>
		/// <remarks>
		/// Used to prevent that multiple unhandled exceptions also generate multiple unhandled exception dialogs.
		/// Any exception of the same type of a type that <see cref="Type.IsAssignableFrom(Type)"/> will be ignored.
		/// </remarks>
		public static bool ExceptionTypeIsIgnored(Type type)
		{
			return (staticExceptionHelper.ExceptionTypeIsIgnored(type));
		}

		/// <summary>
		/// Terminates the exception ignoring, e.g. during shutdown, or if the user requests to disable it.
		/// </summary>
		public static void TerminateExceptionIgnoring()
		{
			staticExceptionHelper.TerminateExceptionIgnoring();
		}

		#endregion

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static UnhandledExceptionResult ProvideExceptionToUser(Exception exception, string originMessage, UnhandledExceptionType exceptionType, bool mayBeContinued)
		{
			return (ProvideExceptionToUser(null, exception, originMessage, exceptionType, mayBeContinued));
		}

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static UnhandledExceptionResult ProvideExceptionToUser(IWin32Window owner, Exception exception, string originMessage, UnhandledExceptionType exceptionType, bool mayBeContinued)
		{
			// Potentially reset cursor and status:
			ResetCursorAndStatus();

			// Continue if exception is already being ignored:
			if (ExceptionTypeIsIgnored(exception.GetType()))
				return (UnhandledExceptionResult.Continue);

			// Temporarily ignore all exceptions to prevent subsequent messages:
			IgnoreExceptionType(typeof(Exception));

			// Provide the exception to the user:
			UnhandledExceptionResult userResult;
			var formsCount = System.Windows.Forms.Application.OpenForms.Count;
			if (formsCount > 0)
			{
				var f = System.Windows.Forms.Application.OpenForms[0]; // This typically is the main form.
				if (f.InvokeRequired)
				{
					var invoker = new ProvideExceptionToUserDelegate(ProvideExceptionToUserInvocation);
					userResult = (UnhandledExceptionResult)f.Invoke(invoker, owner, exception, originMessage, exceptionType, mayBeContinued);
				}
				else
				{
					userResult = ProvideExceptionToUserInvocation(owner, exception, originMessage, exceptionType, mayBeContinued);
				}
			}
			else
			{
				userResult = ProvideExceptionToUserInvocation(owner, exception, originMessage, exceptionType, mayBeContinued);
			}

			// Evalute the user result:
			switch (userResult)
			{
				case UnhandledExceptionResult.Continue:
					RevertIgnoredExceptionType(typeof(Exception));
					break;

				case UnhandledExceptionResult.ContinueAndIgnore:
					IgnoreExceptionType(exception.GetType()); // Add specific type before reverting general type to prevent glitches.
					RevertIgnoredExceptionType(typeof(Exception));
					break;

				case UnhandledExceptionResult.Exit:
				case UnhandledExceptionResult.ExitAndRestart:
				default:
					TerminateExceptionIgnoring();
					break;
			}

			return (userResult);
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private static void ResetCursorAndStatus()
		{
			try
			{
				foreach (var form in System.Windows.Forms.Application.OpenForms)
				{
					try
					{
						var formAsForm = form as Form;
						if (formAsForm != null)
						{
							if (formAsForm.InvokeRequired)
							{
								var invoker = new ResetCursorToDefaultDelegate(ResetCursorToDefaultInvocation);
								formAsForm.Invoke(invoker);
							}
							else
							{
								ResetCursorToDefaultInvocation(formAsForm);
							}
						}
					}
					catch { } // Best effort! Catch any subsequent exception!
				}
			}
			catch { } // Best effort! Catch any subsequent exception!

			try
			{
				foreach (var form in System.Windows.Forms.Application.OpenForms)
				{
					try
					{
						var formAsMain = form as Main;
						if (formAsMain != null)
						{
							if (formAsMain.InvokeRequired)
							{
								var invoker = new ResetStatusTextDelegate(ResetCursorToDefaultInvocation);
								formAsMain.Invoke(invoker);
							}
							else
							{
								ResetCursorToDefaultInvocation(formAsMain);
							}

							break; // There is only one main form.
						}
					}
					catch { } // Best effort! Catch any subsequent exception!
				}
			}
			catch { } // Best effort! Catch any subsequent exception!
		}

		private delegate void ResetCursorToDefaultDelegate(Form form);

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private static void ResetCursorToDefaultInvocation(Form form)
		{
			try
			{
				form.Cursor = Cursors.Default;
			}
			catch { } // Best effort! Catch any subsequent exception!
		}

		private delegate void ResetStatusTextDelegate(Main main);

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private static void ResetStatusTextInvocation(Main main)
		{
			try
			{
				main.ResetStatusText();
			}
			catch { } // Best effort! Catch any subsequent exception!
		}

		private delegate UnhandledExceptionResult ProvideExceptionToUserDelegate(IWin32Window owner, Exception exception, string originMessage, UnhandledExceptionType exceptionType, bool mayBeContinued);

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private static UnhandledExceptionResult ProvideExceptionToUserInvocation(IWin32Window owner, Exception exception, string originMessage, UnhandledExceptionType exceptionType, bool mayBeContinued)
		{
			try
			{
				var productName = ApplicationEx.ProductName;       // "YAT" or "YATConsole", as indicated in main title bar.
				                                                   // Note that 'ProductCaptionAndVersion' will be used when
				var titleBuilder = new StringBuilder(productName); // composing exception text in the 'UnhandledException' form.
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
				var title = titleBuilder.ToString();

				if (exception != null)
				{
					var message =
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
						var f = new UnhandledException(exception, title, originMessage);
						ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, owner);
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
					var message =
						"After this unhandled exception you are advised to exit/restart " + productName + "." + Environment.NewLine + Environment.NewLine +
						"Select [Cancel/Abort] to exit/restart " + productName + "." + Environment.NewLine +
						"Or would you like to continue and [Retry] anyway?" + Environment.NewLine +
						"Or would you like to continue but [Ignore] such unhandled exceptions?";

					UnhandledExceptionResult result;
					switch (MessageBoxEx.Show(owner, message, title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation))
					{
						case DialogResult.Retry:  result = UnhandledExceptionResult.Continue;          break;
						case DialogResult.Ignore: result = UnhandledExceptionResult.ContinueAndIgnore; break; // Intentionally ignore further exceptions.
						default:                  result = UnhandledExceptionResult.Exit;              break; // Don't care about any exceptions anymore.
					}

					if (result == UnhandledExceptionResult.Exit)
					{
						message = "Would you like to restart " + productName + " right away?";
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
					var message =
						"After this unhandled exception " + productName + " will have to exit." + Environment.NewLine +
						"Would you like to restart " + productName + " after exit?";

					UnhandledExceptionResult result;
					switch (MessageBoxEx.Show(owner, message, productName + " Restart", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
					{
						case DialogResult.Yes: result = UnhandledExceptionResult.ExitAndRestart; break;
						default:               result = UnhandledExceptionResult.Exit;           break;
					}

					return (result);
				}
			}
			catch // Best effort! Catch any subsequent exception!
			{
				return (UnhandledExceptionResult.Exit);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
