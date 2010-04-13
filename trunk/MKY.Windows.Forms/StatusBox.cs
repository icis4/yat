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
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a status box similar to <see cref="MessageBox"/>.
	/// </summary>
	[DesignerCategory("Windows Forms")]
	public partial class StatusBox : Form
	{
		#region Delegates
		//==========================================================================================
		// Delegates
		//==========================================================================================

		private delegate void StringMethodDelegate(string value);
		private delegate void DialogResultMethodDelegate(DialogResult value);
		private delegate DialogResult ShowDialogDelegate(IWin32Window value);

		#endregion

		#region Static Methods
		//==========================================================================================
		// Static Methods
		//==========================================================================================

		/// <summary>
		/// Displays a status box in front of the specified object and with the specified
		/// status and caption and returns the result.
		/// </summary>
		/// <param name="owner">
		/// An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.
		/// </param>
		/// <param name="status1">
		/// The text to display in the first line of status box.
		/// </param>
		/// <param name="caption">
		/// The text to display in the title bar of the status box.
		/// </param>
		/// <param name="status2">
		/// The text to display in the second line of status box.
		/// </param>
		/// <returns>
		/// One of the System.Windows.Forms.DialogResult values.
		/// </returns>
		public static DialogResult Show(IWin32Window owner, string status1, string caption, string status2)
		{
			bool setting = false;
			return (Show(owner, status1, caption, status2, "", ref setting));
		}

		/// <summary>
		/// Displays a status box in front of the specified object and with the specified
		/// status and caption and returns the result.
		/// </summary>
		/// <param name="owner">
		/// An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.
		/// </param>
		/// <param name="status1">
		/// The text to display in the first line of status box.
		/// </param>
		/// <param name="caption">
		/// The text to display in the title bar of the status box.
		/// </param>
		/// <param name="status2">
		/// The text to display in the second line of status box.
		/// </param>
		/// <param name="settingText">
		/// The text of the setting check box.
		/// </param>
		/// <param name="setting">
		/// The value of the setting.
		/// </param>
		/// <returns>
		/// One of the System.Windows.Forms.DialogResult values.
		/// </returns>
		public static DialogResult Show(IWin32Window owner, string status1, string caption, string status2, string settingText, ref bool setting)
		{
			DialogResult dialogResult = DialogResult.OK;
			staticStatusBox = new StatusBox(status1, caption, status2, settingText, setting);

			ISynchronizeInvoke sinkTarget = owner as ISynchronizeInvoke;
			if (sinkTarget != null)
			{
				if (sinkTarget.InvokeRequired)
				{
					ShowDialogDelegate del = new ShowDialogDelegate(staticStatusBox.ShowDialog);
					object[] args = { owner };
					dialogResult = (DialogResult)sinkTarget.Invoke(del, args);
				}
				else
				{
					dialogResult = staticStatusBox.ShowDialog(owner);
				}
				setting = staticStatusBox.GetSetting();
			}

			staticStatusBox = null;
			return (dialogResult);
		}

		/// <summary>
		/// Updates the first status line of the status box.
		/// </summary>
		public static void UpdateStatus1(string status)
		{
			ISynchronizeInvoke sinkTarget = staticStatusBox as ISynchronizeInvoke;
			if (sinkTarget != null)
			{
				if (sinkTarget.InvokeRequired)
				{
					StringMethodDelegate del = new StringMethodDelegate(staticStatusBox.SetStatus1);
					object[] args = { status };
					sinkTarget.Invoke(del, args);
				}
				else
				{
					staticStatusBox.SetStatus1(status);
				}
			}
		}

		/// <summary>
		/// Updates the second status line of the status box.
		/// </summary>
		public static void UpdateStatus2(string status)
		{
			ISynchronizeInvoke sinkTarget = staticStatusBox as ISynchronizeInvoke;
			if (sinkTarget != null)
			{
				if (sinkTarget.InvokeRequired)
				{
					StringMethodDelegate del = new StringMethodDelegate(staticStatusBox.SetStatus2);
					object[] args = { status };
					sinkTarget.Invoke(del, args);
				}
				else
				{
					staticStatusBox.SetStatus1(status);
				}
			}
		}

		/// <summary>
		/// Closes the status box.
		/// </summary>
		public static void AcceptAndClose()
		{
			ISynchronizeInvoke sinkTarget = staticStatusBox as ISynchronizeInvoke;
			if (sinkTarget != null)
			{
				DialogResult dialogResult = DialogResult.OK;
				if (sinkTarget.InvokeRequired)
				{
					DialogResultMethodDelegate del = new DialogResultMethodDelegate(staticStatusBox.RequestClose);
					object[] args = { dialogResult };
					sinkTarget.Invoke(del, args);
				}
				else
				{
					staticStatusBox.RequestClose(dialogResult);
				}
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private static StatusBox staticStatusBox;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		/// <remarks>Default constructor needed for designer support</remarks>
		public StatusBox()
		{
			InitializeComponent();
		}

		/// <summary></summary>
		protected StatusBox(string status1, string caption, string status2, string settingText, bool setting)
		{
			InitializeComponent();

			Text = caption;
			label_Status1.Text = status1;
			label_Status2.Text = status2;

			if (settingText != "")
			{
				checkBox_Setting.Visible = true;
				checkBox_Setting.Text = settingText;
				checkBox_Setting.Checked = setting;
				Height = 154;
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void SetStatus1(string value)
		{
			label_Status1.Text = value;
		}

		/// <summary></summary>
		public virtual void SetStatus2(string value)
		{
			label_Status2.Text = value;
		}

		/// <summary></summary>
		public virtual bool GetSetting()
		{
			return (checkBox_Setting.Checked);
		}

		/// <summary></summary>
		public virtual void RequestClose(DialogResult value)
		{
			DialogResult = value;
			Close();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void button_Cancel_Click(object sender, EventArgs e)
		{
			// do nothing
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
