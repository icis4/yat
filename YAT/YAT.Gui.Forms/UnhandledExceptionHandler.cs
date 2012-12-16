//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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

namespace YAT.Gui.Forms
{
	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

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
		public static UnhandledExceptionResult ProvideExceptionToUser(string originMessage, bool isAsync, bool mayBeContinued)
		{
			return (ProvideExceptionToUser(null, null, originMessage, isAsync, mayBeContinued));
		}

		/// <summary></summary>
		public static UnhandledExceptionResult ProvideExceptionToUser(Exception exception, string originMessage, bool isAsync, bool mayBeContinued)
		{
			return (ProvideExceptionToUser(null, exception, originMessage, isAsync, mayBeContinued));
		}

		/// <summary></summary>
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		public static UnhandledExceptionResult ProvideExceptionToUser(IWin32Window owner, Exception exception, string originMessage, bool isAsync, bool mayBeContinued)
		{
			if (!staticHandleExceptions)
				return (UnhandledExceptionResult.Continue);

			string productName = Application.ProductName;

			StringBuilder titleBuilder = new StringBuilder(productName);
			{
				titleBuilder.Append(" Unhandled");

				if (isAsync)
					titleBuilder.Append(" Asynchronous");
				else
					titleBuilder.Append(" Synchronous");

				titleBuilder.Append(" Exception");
			}
			string title = titleBuilder.ToString();

			if (exception != null)
			{
				if (MessageBox.Show(owner,
									originMessage + Environment.NewLine +
									"Show detailed information?",
									title,
									MessageBoxButtons.YesNo,
									MessageBoxIcon.Error) == DialogResult.Yes)
				{
					UnhandledException f = new UnhandledException(exception, title, originMessage);
					f.ShowDialog(owner);
				}
			}
			else
			{
				MessageBox.Show(owner,
								title + Environment.NewLine +
								originMessage,
								productName,
								MessageBoxButtons.OK,
								MessageBoxIcon.Error);
			}

			if (mayBeContinued)
			{
				UnhandledExceptionResult result;
				switch (MessageBox.Show(owner,
										"After an unhandled exception you are advised to exit and restart " + productName + "." + Environment.NewLine + Environment.NewLine +
										"Select cancel/abort to exit " + productName + " now." + Environment.NewLine +
										"Or would you like to continue/retry anyway?" + Environment.NewLine +
										"Or would you like to continue but ignore any additional unhandled exceptions?",
										title,
										MessageBoxButtons.AbortRetryIgnore,
										MessageBoxIcon.Exclamation))
				{
					case DialogResult.Retry:                                  result = UnhandledExceptionResult.Continue; break;
					case DialogResult.Ignore: staticHandleExceptions = false; result = UnhandledExceptionResult.Continue; break; // Intentionally ignore further exceptions.
					default:                  staticHandleExceptions = false; result = UnhandledExceptionResult.Exit;     break; // Don't care about any exceptions anymore.
				}
				if (result == UnhandledExceptionResult.Exit)
				{
					switch (MessageBox.Show(owner,
											"Would you like to restart " + productName + " after exit?",
											productName + " Restart",
											MessageBoxButtons.YesNo,
											MessageBoxIcon.Question))
					{
						case DialogResult.Yes: result = UnhandledExceptionResult.ExitAndRestart; break;
						default:               result = UnhandledExceptionResult.Exit;           break;
					}
				}
				return (result);
			}
			else
			{
				UnhandledExceptionResult result;
				switch (MessageBox.Show(owner,
										"After this unhandled exception " + productName + " will have to exit." + Environment.NewLine +
										"Would you like to restart " + productName + " after exit?",
										productName + " Restart",
										MessageBoxButtons.YesNo,
										MessageBoxIcon.Question))
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
