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
using System.Windows.Forms;

using MKY.Threading;

namespace MKY.Windows.Forms
{
	// YAT.View.Forms.Terminal.ShowPredefinedCommandSettings()
	// YAT.View.Forms.Terminal.SendPredefined()
	// YAT.View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_Command_Click() => Dialog is opened again => NOK !
	// [External Code]
	// YAT.View.Forms.PredefinedCommandSettings.ProcessCmdKey()
	// [External Code]
	// YAT.View.Forms.Terminal.ShowPredefinedCommandSettings()
	// YAT.View.Forms.Terminal.SendPredefined()
	// YAT.View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_Command_Click() => Dialog is opened again => NOK !
	// [External Code]
	// YAT.View.Forms.PredefinedCommandSettings.ProcessCmdKey()
	// [External Code]
	// YAT.View.Forms.Terminal.ShowPredefinedCommandSettings()
	// YAT.View.Forms.Terminal.toolStripMenuItem_PredefinedContextMenu_Define_Click() => Dialog is opened 'normally' => OK
	// [External Code]
	// YAT.Application.Main.RunFullyWithView()
	// YAT.Application.Main.Run()
	// YAT.Application.Main.Run()
	// YAT.Application.Main.Run()
	// YAT.YAT.Main()

	/// <summary>
	/// Implementation of a workaround to fix a bug in Windows.Forms. The bug appears when using
	/// shortcuts in context menus and a dialog is open. Such shortcuts are processed and executed
	/// even when a modal form is open. An example stack trace demonstrating this issue is shown
	/// above (out of doc tag due to words not recognized by StyleCop).
	///
	/// It doesn't matter whether the initial dialog has been opened via the context menu shortcut
	/// workaround below or the main menu as show by the stack trace above. And it doesn't matter
	/// which dialog is open, it can be reproduced by e.g. opening the 'PredefinedCommandSettings'
	/// or the 'TerminalSettings', in both cases shortcuts Shift+F? or Ctrl+Shift+ArrowLeft/Right
	/// are still executed.
	///
	/// As a consequence:
	///  > A workaround to this issue cannot be implemented in a simple way, e.g. inherently by
	///    the <see cref="ContextMenuStripShortcutTargetWorkaround"/>.
	///  > A workaround to this issue can only be implemented by setting and checking a flag at
	///    each potentially called 'ShowDialog' and context menu handler.
	/// </summary>
	/// <remarks>
	/// Note bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround".
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Boolean just *is* 'bool'...")]
	public static class ContextMenuStripShortcutModalFormWorkaround
	{
		private static int staticCounter; // = 0;

		/// <summary>
		/// Gets a value indicating whether a modal dialog is currently being shown.
		/// </summary>
		/// <value>
		/// <c>true</c> if a modal dialog is currently being shown; otherwise, <c>false</c>.
		/// </value>
		public static bool IsCurrentlyShowingModalForm
		{
			get
			{
				DebugAssertIsMainThread("Property called from non-main thread!");

				return (staticCounter > 0); // No need to use 'Interlocked.Read()' as access to
			}                               // 'Windows.Forms' must be synchronized anyway.
		}

		/// <summary>
		/// To be called each time before showing a modal dialog using <see cref="Form.ShowDialog(IWin32Window)"/>.
		/// </summary>
		/// <remarks>
		/// Showing a common dialog using <see cref="CommonDialog.ShowDialog()"/> works fine, thus
		/// no workaround is needed for common dialogs. Also, <see cref="MessageBox.Show(string)"/>
		/// works fine.
		/// </remarks>
		public static void EnterModalForm()
		{
			DebugAssertIsMainThread("Method called from non-main thread!");

			staticCounter++; // No need to use 'Interlocked.Increment()' as access to
		}                    // 'Windows.Forms' must be synchronized anyway.

		/// <summary>
		/// To be called each time after showing a modal dialog using <see cref="Form.ShowDialog()"/>.
		/// </summary>
		/// <remarks>
		/// Showing a common dialog using <see cref="CommonDialog.ShowDialog()"/> works fine, thus
		/// no workaround is needed for common dialogs. Also, <see cref="MessageBox.Show(string)"/>
		/// works fine.
		/// </remarks>
		public static void LeaveModalForm()
		{
			DebugAssertIsMainThread("Method called from non-main thread!");

			staticCounter--; // No need to use 'Interlocked.Decrement()' as access to
			                 // 'Windows.Forms' must be synchronized anyway.
			if (staticCounter < 0)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Counter has fallen below 0!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary>
		/// Invokes <see cref="Form.Show(IWin32Window)"/> on the given form.
		/// </summary>
		public static DialogResult InvokeShowDialog(Form form, IWin32Window owner)
		{
			DialogResult dr;

			EnterModalForm();
			try
			{
				dr = form.ShowDialog(owner);
			}
			finally
			{
				LeaveModalForm();
			}

			return (dr);
		}

		[Conditional("Debug")]
		private static void DebugAssertIsMainThread(string message)
		{
			if (MainThreadHelper.IsInitialized)
				Debug.Assert(MainThreadHelper.IsMainThread, message, "This workaround requires that it is always called on the main thread, because .NET WinForms require that all UI operation is done on the UI main thread.");
		////else
			////There is no way to assert whether this is the main thread.
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
