//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using MKY.Utilities.Event;

using YAT.Model.Types;

using YAT.Settings;
using YAT.Settings.Application;

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
		private const bool TerminalIsOpenDefault = false;
		private const float SplitterRatioDefault = (float)0.75;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Command fileCommand = new Command();
		private Domain.TerminalType terminalType = TerminalTypeDefault;
		private bool terminalIsOpen = TerminalIsOpenDefault;
		private float splitterRatio = SplitterRatioDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when the FileCommand property is changed.")]
		public event EventHandler FileCommandChanged;

		[Category("Action")]
		[Description("Event raised when sending the file is requested.")]
		public event EventHandler SendFileCommandRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool TerminalIsOpen
		{
			set
			{
				this.terminalIsOpen = value;
				SetControls();
			}
		}

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

		private void pathLabel_FilePath_Click(object sender, EventArgs e)
		{
			ShowOpenFileDialog();
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
			splitContainer.SplitterDistance = (int)(this.splitterRatio * splitContainer.Width);

			if (this.fileCommand.IsFilePath)
			{
				pathLabel_FilePath.Text      = this.fileCommand.FilePath;
				pathLabel_FilePath.ForeColor = SystemColors.ControlText;
				pathLabel_FilePath.Font      = SystemFonts.DefaultFont;
			}
			else
			{
				pathLabel_FilePath.Text      = Command.UndefinedFilePathText;
				pathLabel_FilePath.ForeColor = SystemColors.GrayText;
				pathLabel_FilePath.Font      = Utilities.Drawing.ItalicDefaultFont;
			}

			if (this.fileCommand.IsValidFilePath)
				button_SendFile.Enabled = this.terminalIsOpen;
			else
				button_SendFile.Enabled = false;
		}

		private bool ShowOpenFileDialog()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Set File";
			switch (this.terminalType)
			{
				case Domain.TerminalType.Binary:
				{
					ofd.Filter = ExtensionSettings.BinaryFilesFilter;
					ofd.DefaultExt = ExtensionSettings.BinaryFilesDefault;
					break;
				}
				default: // includes Domain.TerminalType.Text:
				{
					ofd.Filter = ExtensionSettings.TextFilesFilter;
					ofd.DefaultExt = ExtensionSettings.TextFilesDefault;
					break;
				}
			}
			ofd.InitialDirectory = ApplicationSettings.LocalUser.Paths.SendFilesPath;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.SendFilesPath = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				this.fileCommand.IsFilePath = true;
				this.fileCommand.FilePath = ofd.FileName;

				SetControls();
				button_SendFile.Select();

				OnFileCommandChanged(new EventArgs());

				return (true);
			}
			return (false);
		}

		private void RequestSendCommand()
		{
			if (!this.fileCommand.IsValidFilePath)
			{
				if (MessageBox.Show
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
				MessageBox.Show
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

		protected virtual void OnFileCommandChanged(EventArgs e)
		{
			EventHelper.FireSync(FileCommandChanged, this, e);
		}

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
