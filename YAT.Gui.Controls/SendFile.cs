using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MKY.Utilities.Event;

using YAT.Model.Types;
using YAT.Settings;
using YAT.Settings.Application;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("SendFileCommandRequest")]
	public partial class SendFile : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const Domain.TerminalType _TerminalTypeDefault = Domain.TerminalType.Text;
		private const bool _TerminalIsOpenDefault = false;
        private const float _SplitterRatioDefault = (float)0.75;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Command _fileCommand = new Command();
		private Domain.TerminalType _terminalType = _TerminalTypeDefault;
		private bool _terminalIsOpen = _TerminalIsOpenDefault;
        private float _splitterRatio = _SplitterRatioDefault;

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
		/// Command always returns a Command object, it never returns null.
		/// </summary>
		[Browsable(false)]
		public Command FileCommand
		{
			get { return (_fileCommand); }
			set
			{
				if (value != null)
					_fileCommand = value;
				else
					_fileCommand = new Command();

				OnFileCommandChanged(new EventArgs());
				SetControls();
			}
		}

		[Browsable(false)]
		[DefaultValue(_TerminalTypeDefault)]
		public Domain.TerminalType TerminalType
		{
			set
			{
				_terminalType = value;
				SetControls();
			}
		}

		[Browsable(false)]
		[DefaultValue(_TerminalIsOpenDefault)]
		public bool TerminalIsOpen
		{
			set
			{
				_terminalIsOpen = value;
				SetControls();
			}
		}

        [DefaultValue(_SplitterRatioDefault)]
        public float SplitterRatio
        {
            get { return (_splitterRatio); }
            set
            {
                _splitterRatio = value;
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
            splitContainer.SplitterDistance = (int)(_splitterRatio * splitContainer.Width);

			if (_fileCommand.IsFilePath)
				pathLabel_FilePath.Text = _fileCommand.FilePath;
			else
				pathLabel_FilePath.Text = Command.UndefinedFilePathText;

			if (_fileCommand.IsValidFilePath)
				button_SendFile.Enabled = _terminalIsOpen;
			else
				button_SendFile.Enabled = false;
        }

		private bool ShowOpenFileDialog()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Set File";
			switch (_terminalType)
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
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName != ""))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.SendFilesPath = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.SaveLocalUser();

				_fileCommand.IsFilePath = true;
				_fileCommand.FilePath = ofd.FileName;

				SetControls();
				button_SendFile.Select();

				OnFileCommandChanged(new EventArgs());

				return (true);
			}
			return (false);
		}

		private void RequestSendCommand()
		{
			if (!_fileCommand.IsValidFilePath)
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

			if (_fileCommand.IsValidFilePath)
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
