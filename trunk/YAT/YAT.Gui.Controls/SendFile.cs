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
// YAT 2.0 Beta 4 Candidate 2 Version 1.99.30
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using MKY;
using MKY.Recent;
using MKY.Windows.Forms;

using YAT.Model.Types;
using YAT.Settings;
using YAT.Settings.Application;

#endregion

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("SendFileCommandRequest")]
	public partial class SendFile : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const Domain.TerminalType TerminalTypeDefault = Domain.TerminalType.Text;
		private const bool TerminalIsReadyToSendDefault = false;
		private const float SplitterRatioDefault = (float)0.75;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Command fileCommand = new Command();
		private RecentItemCollection<Command> recents;
		private Domain.TerminalType terminalType = TerminalTypeDefault;
		private bool terminalIsReadyToSend = TerminalIsReadyToSendDefault;
		private float splitterRatio = SplitterRatioDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the FileCommand property is changed.")]
		public event EventHandler FileCommandChanged;

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when sending the file is requested.")]
		public event EventHandler SendFileCommandRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Justification = "The initialization of 'terminalIsReadyToSend' is not unnecesary, it is based on a constant that contains a default value!")]
		public SendFile()
		{
			InitializeComponent();
			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Command always returns a Command object, it never returns <c>null</c>.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Command FileCommand
		{
			get { return (this.fileCommand); }
			set
			{
				if (value != null)
					this.fileCommand = value;
				else
					this.fileCommand = new Command();

				OnFileCommandChanged(new EventArgs());
				SetControls();
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Setter is intended.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<Command> RecentCommands
		{
			set
			{
				this.recents = value;
				SetControls();
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Domain.TerminalType TerminalType
		{
			set
			{
				this.terminalType = value;
				SetControls();
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool TerminalIsReadyToSend
		{
			set
			{
				this.terminalIsReadyToSend = value;
				SetControls();
			}
		}

		/// <summary></summary>
		[DefaultValue(SplitterRatioDefault)]
		public virtual float SplitterRatio
		{
			get { return (this.splitterRatio); }
			set
			{
				this.splitterRatio = value;
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void pathComboBox_FilePath_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				if (pathComboBox_FilePath.SelectedItem != null)
					SetFileCommand((Command)((RecentItem<Command>)pathComboBox_FilePath.SelectedItem));
			}
		}

		private void button_SetFile_Click(object sender, EventArgs e)
		{
			ShowOpenFileDialog();
		}

		private void button_SendFile_Click(object sender, EventArgs e)
		{
			RequestSendCommand();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls.Enter();

			splitContainer.SplitterDistance = Int32Ex.LimitToBounds((int)(this.splitterRatio * splitContainer.Width), 0, splitContainer.Width);

			pathComboBox_FilePath.Items.Clear();

			if ((this.recents != null) && (this.recents.Count > 0))
				pathComboBox_FilePath.Items.AddRange(this.recents.ToArray());
			else
				pathComboBox_FilePath.Items.Add(Command.UndefinedFilePathText);

			if (this.fileCommand.IsFilePath)
			{
				pathComboBox_FilePath.ForeColor = SystemColors.ControlText;
				pathComboBox_FilePath.Font      = SystemFonts.DefaultFont;

				int index = 0;
				for (int i = 0; i < pathComboBox_FilePath.Items.Count; i++)
				{
					RecentItem<Command> r = pathComboBox_FilePath.Items[i] as RecentItem<Command>;
					if ((r != null) && (r.Item == this.fileCommand))
					{
						index = i;
						break;
					}
				}
				pathComboBox_FilePath.SelectedIndex = index;
			}
			else
			{
				pathComboBox_FilePath.ForeColor     = SystemColors.GrayText;
				pathComboBox_FilePath.Font          = Utilities.Drawing.ItalicDefaultFont;
				pathComboBox_FilePath.SelectedIndex = 0; // Results in Command.UndefinedFilePathText.
			}

			if (this.fileCommand.IsValidFilePath)
				button_SendFile.Enabled = this.terminalIsReadyToSend;
			else
				button_SendFile.Enabled = false;

			this.isSettingControls.Leave();
		}

		private void SetFileCommand(Command fileCommand)
		{
			this.fileCommand = fileCommand;

			if (!this.recents.Contains(fileCommand))
				this.recents.Add(fileCommand);

			SetControls();
			OnFileCommandChanged(new EventArgs());
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private bool ShowOpenFileDialog()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Set File";
			switch (this.terminalType)
			{
				case Domain.TerminalType.Binary:
				{
					ofd.Filter      = ExtensionSettings.BinaryFilesFilter;
					ofd.FilterIndex = ExtensionSettings.BinaryFilesFilterDefault;
					ofd.DefaultExt  = ExtensionSettings.BinaryFilesDefault;
					break;
				}
				default: // Includes Domain.TerminalType.Text:
				{
					ofd.Filter      = ExtensionSettings.TextFilesFilter;
					ofd.FilterIndex = ExtensionSettings.TextFilesFilterDefault;
					ofd.DefaultExt  = ExtensionSettings.TextFilesDefault;
					break;
				}
			}
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.SendFilesPath;
			bool success = ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName.Length > 0));
			if (success)
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Paths.SendFilesPath = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				SetFileCommand(new Command(ofd.FileName, true, ofd.FileName));
			}
			else
			{
				SetControls();
			}

			button_SendFile.Select();
			return (success);
		}

		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an explicit user interaction.")]
		private void RequestSendCommand()
		{
			if (!this.fileCommand.IsValidFilePath)
			{
				if (MessageBoxEx.Show
					(
					this,
					"File does not exist, set file?",
					"No File",
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button1
					)
					== DialogResult.Yes)
				{
					if (!ShowOpenFileDialog())
						return;
				}
				else
				{
					return;
				}
			}

			if (this.fileCommand.IsValidFilePath)
			{
				OnSendFileCommandRequest(new EventArgs());
			}
			else
			{
				MessageBoxEx.Show
					(
					this,
					"File does not exist, no data has been sent!",
					"No File",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);
			}
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnFileCommandChanged(EventArgs e)
		{
			EventHelper.FireSync(FileCommandChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnSendFileCommandRequest(EventArgs e)
		{
			EventHelper.FireSync(SendFileCommandRequest, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
