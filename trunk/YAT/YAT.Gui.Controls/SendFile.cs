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
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
	[DefaultEvent("SendCommandRequest")]
	public partial class SendFile : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const Domain.TerminalType TerminalTypeDefault = Domain.Settings.TerminalSettings.TerminalTypefault;
		private const bool TerminalIsReadyToSendDefault = false;
		private const int SplitterDistanceDefault = 356; // Designer requires that this is a constant.
		                                                 // Set same value as splitContainer.SplitterDistance is designed.
		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Command command = new Command();
		private RecentItemCollection<Command> recents;
		private Domain.TerminalType terminalType = TerminalTypeDefault;
		private bool terminalIsReadyToSend = TerminalIsReadyToSendDefault;
		private int splitterDistance = SplitterDistanceDefault;

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
					if (value != null)
						this.command = value;
					else
						this.command = new Command();

					SetControls();
					OnCommandChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Setter is intended.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is intended.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual RecentItemCollection<Command> Recents
		{
			set
			{
				// Do not check if (this.recents != value) because the collection will always be the same!

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
				if (this.terminalType != value)
				{
					this.terminalType = value;
					SetControls();
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
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[DefaultValue(SplitterDistanceDefault)]
		public virtual int SplitterDistance
		{
			get { return (this.splitterDistance); }
			set
			{
				if (this.splitterDistance != value)
				{
					this.splitterDistance = value;
					SetControls();
				}
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
				{
					RecentItem<Command> ri = (pathComboBox_FilePath.SelectedItem as RecentItem<Command>);
					if (ri != null)
						SetCommand(ri.Item);
				}
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

			splitContainer.SplitterDistance = Int32Ex.LimitToBounds((this.splitterDistance - splitContainer.Left), 0, (splitContainer.Width - 1));

			pathComboBox_FilePath.Items.Clear();

			if ((this.recents != null) && (this.recents.Count > 0))
				pathComboBox_FilePath.Items.AddRange(this.recents.ToArray());
			else
				pathComboBox_FilePath.Items.Add(Command.UndefinedFilePathText);

			if (this.command.IsFilePath)
			{
				pathComboBox_FilePath.ForeColor = SystemColors.ControlText;
				pathComboBox_FilePath.Font      = SystemFonts.DefaultFont;

				int index = 0;
				for (int i = 0; i < pathComboBox_FilePath.Items.Count; i++)
				{
					RecentItem<Command> r = pathComboBox_FilePath.Items[i] as RecentItem<Command>;
					if ((r != null) && (r.Item == this.command))
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

			if (this.command.IsValidFilePath)
				button_SendFile.Enabled = this.terminalIsReadyToSend;
			else
				button_SendFile.Enabled = false;

			this.isSettingControls.Leave();
		}

		private void SetCommand(Command command)
		{
			this.command = command;

			if (!this.recents.Contains(command))
				this.recents.Add(command);

			SetControls();
			OnCommandChanged(EventArgs.Empty);
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

				SetCommand(new Command(ofd.FileName, true, ofd.FileName));
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
			if (!this.command.IsValidFilePath)
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

			if (this.command.IsValidFilePath)
			{
				OnSendCommandRequest(EventArgs.Empty);
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
