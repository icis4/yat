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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of state changes and validation related to the handled command:
////#define DEBUG_COMMAND

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;

using MKY;
using MKY.Drawing;
using MKY.IO;
using MKY.Recent;
using MKY.Windows.Forms;

using YAT.Application.Utilities;
using YAT.Model.Types;
using YAT.Settings.Application;

#endregion

namespace YAT.View.Controls
{
	/// <remarks>
	/// Note that similar code exists in <see cref="SendText"/> and <see cref="PredefinedCommandSettingsSet"/>.
	/// The diff among these three implementations shall be kept as small as possible.
	/// 
	/// For a future refactoring, consider to separate the common code into a common view-model.
	/// </remarks>
	[DefaultEvent("SendCommandRequest")]
	public partial class SendFile : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const Domain.TerminalType TerminalTypeDefault = Domain.Settings.TerminalSettings.TerminalTypeDefault;

		private const bool TerminalIsReadyToSendDefault = false;

		/// <remarks>
		/// The designer requires that this is a constant.
		/// Set same value as splitContainer.SplitterDistance is designed.
		/// </remarks>
		private const int SendSplitterDistanceDefault = 356;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Command command = new Command();
		private RecentItemCollection<Command> recent;

		private Domain.TerminalType terminalType = TerminalTypeDefault;
		private bool useExplicitDefaultRadix = Domain.Settings.SendSettings.UseExplicitDefaultRadixDefault;

		private bool terminalIsReadyToSend = TerminalIsReadyToSendDefault;

		private int sendSplitterDistance = SendSplitterDistanceDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the Command property is changed.")]
		public event EventHandler CommandChanged;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when sending the file is requested.")]
		public event EventHandler SendCommandRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public SendFile()
		{
			InitializeComponent();

			InitializeControls();
		////Set...Controls() is initially called in the 'Paint' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>
		/// This property always returns a <see cref="Command"/> object, it never returns <c>null</c>.
		/// </remarks>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Command Command
		{
			get { return (this.command); }
			set
			{
				if (this.command != value)
				{
					DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

					if (value != null)
						this.command = value;
					else
						this.command = new Command();

					SetRecentAndCommandControls();
					OnCommandChanged(EventArgs.Empty);

					DebugCommandLeave();
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Setter is intended.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<Command> Recent
		{
			set
			{
				DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

				// Do not check if (this.recent != value) because the collection will always be the same!

				this.recent = value;
				SetRecentAndCommandControls(); // Recent must immediately be updated, otherwise order will be wrong on arrow-up/down.

				DebugCommandLeave();
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.TerminalType TerminalType
		{
			set
			{
				if (this.terminalType != value)
				{
					this.terminalType = value;
					SetSendControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Command")]
		[Description("Whether to use an explicit default radix.")]
		[DefaultValue(Domain.Settings.SendSettings.UseExplicitDefaultRadixDefault)]
		public virtual bool UseExplicitDefaultRadix
		{
			get { return (this.useExplicitDefaultRadix); }
			set
			{
				if (this.useExplicitDefaultRadix != value)
				{
					this.useExplicitDefaultRadix = value;

					if (value) // Explicit => Refresh the command controls.
						SetRecentAndCommandControls();

					SetExplicitDefaultRadixControls();

					if (!value) // Implicit => Reset default radix.
						this.command.DefaultRadix = Command.DefaultRadixDefault;
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool TerminalIsReadyToSend
		{
			set
			{
				if (this.terminalIsReadyToSend != value)
				{
					this.terminalIsReadyToSend = value;
					SetSendControls();
				}
			}
		}

		/// <summary></summary>
		[DefaultValue(SendSplitterDistanceDefault)]
		public virtual int SendSplitterDistance
		{
			get { return (this.sendSplitterDistance); }
			set
			{
				// Do not check if (this.splitterDistance != value) because the distance (position)
				// will be limited to the control's width, and that may change AFTER the distance
				// has been set.

				this.sendSplitterDistance = value;

				// No need to call SetControls(); as only the splitter will be moved, and that will
				// not be accessed anywhere else.

				splitContainer_Send.SplitterDistance = Int32Ex.Limit((this.sendSplitterDistance - splitContainer_Send.Left), 0, (splitContainer_Send.Width - 1));
			}
		}

		#endregion

		#region Control Special Keys
		//==========================================================================================
		// Control Special Keys
		//==========================================================================================

		/// <summary></summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Enter)
			{
				if (button_Send.Enabled)
				{
					RequestSendCommand();
					return (true);
				}
			}

			return (base.ProcessCmdKey(ref msg, keyData));
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// Use paint event to ensure that message boxes in case of errors (e.g. validation errors)
		/// are shown on top of a properly painted control or form.
		/// </remarks>
		private void SendFile_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetExplicitDefaultRadixControls();
				SetRecentAndCommandControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		/// <remarks>Using 'Validation' instead of 'SelectedIndexChanged' for symmetricity with <see cref="SendText"/>.</remarks>
		private void comboBox_ExplicitDefaultRadix_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				Domain.Radix radix = this.command.DefaultRadix;
				Domain.RadixEx selectedItem = comboBox_ExplicitDefaultRadix.SelectedItem as Domain.RadixEx;
				if (selectedItem != null) // Can be 'null' when validating all controls before an item got selected.
					radix = selectedItem;

				// No need to validate the radix, simply set and confirm it:
				this.command.DefaultRadix = radix;
				ConfirmCommand();
			}
		}

		private void pathComboBox_FilePath_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);

				if (pathComboBox_FilePath.SelectedItem != null)
				{
					var ri = (pathComboBox_FilePath.SelectedItem as RecentItem<Command>);
					if (ri != null)
						ConfirmCommand(ri.Item.FilePath);
				}

				DebugCommandLeave();
			}
		}

		private void button_OpenFile_Click(object sender, EventArgs e)
		{
			ShowOpenFileDialog();
		}

		private void button_Send_Click(object sender, EventArgs e)
		{
			RequestSendCommand();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		#region Private Methods > Controls
		//------------------------------------------------------------------------------------------
		// Private Methods > Controls
		//------------------------------------------------------------------------------------------

		private void InitializeControls()
		{
			this.isSettingControls.Enter();

			comboBox_ExplicitDefaultRadix.Items.Clear();
			comboBox_ExplicitDefaultRadix.Items.AddRange(Domain.RadixEx.GetItems());

			this.isSettingControls.Leave();
		}

		private void SetExplicitDefaultRadixControls()
		{
			this.isSettingControls.Enter();

			splitContainer_ExplicitDefaultRadix.Panel1Collapsed = !this.useExplicitDefaultRadix;

			this.isSettingControls.Leave();
		}

		private void SetRecentAndCommandControls()
		{
			DebugCommandEnter(System.Reflection.MethodBase.GetCurrentMethod().Name);
			this.isSettingControls.Enter();

			if (this.useExplicitDefaultRadix)
				SelectionHelper.Select(comboBox_ExplicitDefaultRadix, (Domain.RadixEx)this.command.DefaultRadix, (Domain.RadixEx)this.command.DefaultRadix);
			else
				SelectionHelper.Deselect(comboBox_ExplicitDefaultRadix);

			pathComboBox_FilePath.Items.Clear();

			// Fill the drop down list, depending on the amount of recent files:
			if ((this.recent != null) && (this.recent.Count > 0))
			{
				// Add the current command, or "<Set a file...>", to the top of the list:
				if (this.command.IsFilePath)
				{
					// Add the current command only if not already contained in recent files:
					if (!this.recent.Contains(this.command))
						pathComboBox_FilePath.Items.Add(this.command);
				}
				else
				{
					pathComboBox_FilePath.Items.Add(Command.UndefinedFilePathText);
				}

				// Add the recent files:
				pathComboBox_FilePath.Items.AddRange(this.recent.ToArray());
			}
			else
			{
				if (this.command.IsFilePath)
					pathComboBox_FilePath.Items.Add(this.command);
				else
					pathComboBox_FilePath.Items.Add(Command.UndefinedFilePathText);
			}

			// Restore the current command and set the combo box' properties:
			int selectedIndex = ControlEx.InvalidIndex;

			if (this.command.IsFilePath)
			{
				for (int i = 0; i < pathComboBox_FilePath.Items.Count; i++)
				{
					var c = (pathComboBox_FilePath.Items[i] as Command);
					if ((c != null) && (c == this.command))
					{
						selectedIndex = i;
						break;
					}

					var r = (pathComboBox_FilePath.Items[i] as RecentItem<Command>);
					if ((r != null) && (r.Item == this.command))
					{
						selectedIndex = i;
						break;
					}
				}
			}

			if (selectedIndex != ControlEx.InvalidIndex)
			{
				if (pathComboBox_FilePath.ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
					pathComboBox_FilePath.ForeColor = SystemColors.ControlText;

				if (pathComboBox_FilePath.Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
					pathComboBox_FilePath.Font = SystemFonts.DefaultFont;

				if (pathComboBox_FilePath.SelectedIndex != selectedIndex) // Improve performance by only assigning if different.
					pathComboBox_FilePath.SelectedIndex = selectedIndex;
			}
			else
			{
				if (pathComboBox_FilePath.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
					pathComboBox_FilePath.ForeColor = SystemColors.GrayText;

				if (pathComboBox_FilePath.Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
					pathComboBox_FilePath.Font = DrawingEx.DefaultFontItalic;

				if (pathComboBox_FilePath.SelectedIndex != 0) // Improve performance by only assigning if different.
					pathComboBox_FilePath.SelectedIndex = 0; // Results in Command.UndefinedFilePathText.
			}

			SetSendControls();

			this.isSettingControls.Leave();
			DebugCommandLeave();
		}

		private void SetSendControls()
		{
			this.isSettingControls.Enter();

			if (this.command.IsValidFilePath)
				button_Send.Enabled = this.terminalIsReadyToSend;
			else
				button_Send.Enabled = false;

			this.isSettingControls.Leave();
		}

		#endregion

		#region Private Methods > Open File
		//------------------------------------------------------------------------------------------
		// Private Methods > Open File
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private bool ShowOpenFileDialog()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Set File";

			string initialExtension;
			switch (this.terminalType)
			{
				case Domain.TerminalType.Binary:
				{
					initialExtension = ApplicationSettings.LocalUserSettings.Extensions.BinarySendFiles;

					ofd.Filter      = ExtensionHelper.BinaryFilesFilter;
					ofd.FilterIndex = ExtensionHelper.BinaryFilesFilterHelper(initialExtension);
					break;
				}

				case Domain.TerminalType.Text:
				default:
				{
					initialExtension = ApplicationSettings.LocalUserSettings.Extensions.TextSendFiles;

					ofd.Filter      = ExtensionHelper.TextFilesFilter;
					ofd.FilterIndex = ExtensionHelper.TextFilesFilterHelper(initialExtension);
					break;
				}
			}

			ofd.DefaultExt = PathEx.DenormalizeExtension(initialExtension);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.SendFiles;

			bool success = ((ofd.ShowDialog(this) == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)));
			if (success)
			{
				Refresh();

				switch (this.terminalType)
				{
					case Domain.TerminalType.Binary:
					{
						ApplicationSettings.LocalUserSettings.Extensions.BinarySendFiles = Path.GetExtension(ofd.FileName);
						break;
					}

					case Domain.TerminalType.Text:
					default:
					{
						ApplicationSettings.LocalUserSettings.Extensions.TextSendFiles = Path.GetExtension(ofd.FileName);
						break;
					}
				}

				ApplicationSettings.LocalUserSettings.Paths.SendFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				ConfirmCommand(ofd.FileName);
			}
			else
			{
				SetRecentAndCommandControls();
			////OnCommandChanged() is not called, nothing has changed.
			}

			button_Send.Select();
			return (success);
		}

		#endregion

		#region Private Methods > Handle Command
		//------------------------------------------------------------------------------------------
		// Private Methods > Handle Command
		//------------------------------------------------------------------------------------------

		private void ConfirmCommand()
		{
			SetRecentAndCommandControls();
			OnCommandChanged(EventArgs.Empty);
		}

		private void ConfirmCommand(string filePath)
		{
			this.command.FilePath = filePath;

			SetRecentAndCommandControls();
			OnCommandChanged(EventArgs.Empty);
		}

		#endregion

		#region Private Methods > Request Send
		//------------------------------------------------------------------------------------------
		// Private Methods > Request Send
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an explicit user interaction.")]
		private void RequestSendCommand()
		{
			if (this.command.IsValidFilePath)
			{
				ConfirmCommand(); // Required to invoke OnCommandChanged().
				InvokeSendCommandRequest();
			}
			else
			{
				DialogResult dr = MessageBoxEx.Show
				(
					this,
					"File does not exist, set file?",
					"No File",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button1
				);

				if (dr == DialogResult.Yes)
				{
					if (ShowOpenFileDialog()) // ConfirmCommand() gets called here.
						InvokeSendCommandRequest();
				}
			}
		}

		private void InvokeSendCommandRequest()
		{
			OnSendCommandRequest(EventArgs.Empty);
		}

		#endregion

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnCommandChanged(EventArgs e)
		{
			EventHelper.FireSync(CommandChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnSendCommandRequest(EventArgs e)
		{
			EventHelper.FireSync(SendCommandRequest, this, e);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[Conditional("DEBUG_COMMAND")]
		protected virtual void DebugCommandEnter(string methodName)
		{
			Debug.WriteLine(methodName);
			Debug.Indent();

			DebugCommandState();
		}

		/// <summary></summary>
		[Conditional("DEBUG_COMMAND")]
		protected virtual void DebugCommandLeave()
		{
			DebugCommandState();

			Debug.Unindent();
		}

		/// <summary></summary>
		[Conditional("DEBUG_COMMAND")]
		protected virtual void DebugCommandState()
		{
			Debug.WriteLine("Text    = " + pathComboBox_FilePath.Text);

			if (this.recent != null)
				Debug.WriteLine("Recent = " + ArrayEx.ElementsToString(this.recent.ToArray()));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
