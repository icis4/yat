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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
//// 'System.Threading' is not used due to ambiguity with 'System.Windows.Forms.Timer' and 'StatusBox.Timeout'.
using System.Windows.Forms;

using MKY.Diagnostics;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Provides a status box similar to <see cref="MessageBox"/>.
	/// </summary>
	public partial class StatusBox : Form
	{
		#region Delegates
		//==========================================================================================
		// Delegates
		//==========================================================================================

		private delegate void StringMethodDelegate(string value);
		private delegate void DialogResultMethodDelegate(DialogResult value);

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
		/// <param name="caption">
		/// The text to display in the title bar of the status box.
		/// </param>
		/// <param name="status1">
		/// The text to display in the first line of status box.
		/// </param>
		/// <param name="status2">
		/// The text to display in the second line of status box.
		/// </param>
		/// <returns>
		/// One of the System.Windows.Forms.DialogResult values.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[ModalBehaviorContract(ModalBehavior.Always)]
		public static DialogResult Show(IWin32Window owner, string caption, string status1, string status2 = null)
		{
			bool setting = false;
			return (Show(owner, caption, status1, status2, null, ref setting));
		}

		/// <summary>
		/// Displays a status box in front of the specified object and with the specified
		/// status and caption and returns the result.
		/// </summary>
		/// <param name="owner">
		/// An implementation of System.Windows.Forms.IWin32Window that will own the modal dialog box.
		/// </param>
		/// <param name="caption">
		/// The text to display in the title bar of the status box.
		/// </param>
		/// <param name="status1">
		/// The text to display in the first line of status box.
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
		/// <param name="showCancel">
		/// Flag whether the cancel button shall be shown.
		/// </param>
		/// <param name="timeout">
		/// Optional timeout.
		/// </param>
		/// <returns>
		/// One of the System.Windows.Forms.DialogResult values.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "5#", Justification = "Setting is required to be received, modified and returned.")]
		[ModalBehaviorContract(ModalBehavior.Always)]
		public static DialogResult Show(IWin32Window owner, string caption, string status1, string status2, string settingText, ref bool setting, bool showCancel = true, int timeout = System.Threading.Timeout.Infinite)
		{
			StatusBox sb = new StatusBox();
			return (sb.ShowDialog(owner, caption, status1, status2, settingText, ref setting, showCancel, timeout));
		}

		/// <summary></summary>
		[ModalBehaviorContract(ModalBehavior.Never)]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public static StatusBox Create(string caption, string status1, string status2 = null)
		{
			return (new StatusBox(caption, status1, status2));
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private int timeout = System.Threading.Timeout.Infinite;
		private bool isShowing; // = false;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		/// <remarks>Default constructor needed for designer support.</remarks>
		public StatusBox()
		{
			InitializeComponent();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		protected StatusBox(string caption, string status1, string status2 = null)
			: this(caption, status1, status2, null, false, true)
		{
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		protected StatusBox(string caption, string status1, string status2, string settingText, bool setting, bool showCancel = true, int timeout = System.Threading.Timeout.Infinite)
		{
			InitializeComponent();
			Initialize(caption, status1, status2, settingText, setting, showCancel, timeout);
		}

		private void Initialize(string caption, string status1, string status2, string settingText, bool setting, bool showCancel, int timeout)
		{
			Caption = caption;
			Status1 = status1;
			Status2 = status2;

			InitializeSetting(settingText, setting);

			ShowCancel = showCancel;
			Timeout    = timeout;
		}

		private void InitializeSetting(string settingText, bool setting)
		{
			if (string.IsNullOrEmpty(settingText))
			{
				checkBox_Setting.Visible = false;
				checkBox_Setting.Text    = null;
				checkBox_Setting.Checked = false;
				Height = 130;
			}
			else
			{
				checkBox_Setting.Visible = true;
				checkBox_Setting.Text    = settingText;
				checkBox_Setting.Checked = setting;
				Height = 160;
			}
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual string Caption
		{
			get { return (Text); }
			set { Text = value;  }
		}

		/// <summary></summary>
		public virtual string Status1
		{
			get { return (label_Status1.Text); }
			set { label_Status1.Text = value;  }
		}

		/// <summary></summary>
		public virtual string Status2
		{
			get { return (label_Status2.Text); }
			set { label_Status2.Text = value;  }
		}

		/// <summary></summary>
		protected virtual bool Setting
		{
			get { return (checkBox_Setting.Checked); }
		}

		/// <summary></summary>
		public virtual bool ShowCancel
		{
			get { return (button_Cancel.Visible); }
			set { button_Cancel.Visible = value;  }
		}

		/// <summary></summary>
		public virtual int Timeout
		{
			get { return (this.timeout); }
			set { this.timeout = value;  }
		}

		/// <summary></summary>
		public virtual bool IsShowing
		{
			get { return (this.isShowing); }
			set { this.isShowing = value;  }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[ModalBehaviorContract(ModalBehavior.Always)]
		public DialogResult ShowDialog(IWin32Window owner, string caption, string status1, string status2 = null)
		{
			bool setting = false;
			return (ShowDialog(owner, caption, status1, status2, null, ref setting));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Setting is required to be received, modified and returned.")]
		[ModalBehaviorContract(ModalBehavior.Always)]
		public DialogResult ShowDialog(IWin32Window owner, string settingText, ref bool setting, bool showCancel = true, int timeout = System.Threading.Timeout.Infinite)
		{
			return (ShowDialog(owner, Caption, Status1, Status2, settingText, ref setting, showCancel, timeout));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "5#", Justification = "Setting is required to be received, modified and returned.")]
		[ModalBehaviorContract(ModalBehavior.Always)]
		public DialogResult ShowDialog(IWin32Window owner, string caption, string status1, string status2, string settingText, ref bool setting, bool showCancel = true, int timeout = System.Threading.Timeout.Infinite)
		{
			DialogResult dr;
			Initialize(caption, status1, status2, settingText, setting, showCancel, timeout);

			var callback = new System.Threading.TimerCallback(timeoutTimer_OneShot_Elapsed);
			var dueTime = this.timeout;
			var period = System.Threading.Timeout.Infinite; // One-Shot!

			using (var timeoutTimer = new System.Threading.Timer(callback, null, dueTime, period))
			{
				this.isShowing = true;

				ContextMenuStripShortcutModalFormWorkaround.EnterModalForm();
				try
				{
					dr = ShowDialog(owner);
				}
				finally
				{
					ContextMenuStripShortcutModalFormWorkaround.LeaveModalForm();
				}

				this.isShowing = false;
			}

			setting = Setting;
			return (dr);
		}

		private void timeoutTimer_OneShot_Elapsed(object obj)
		{
			// Non-periodic timer, only a single callback can be active at a time.
			// There is no need to synchronize concurrent callbacks to this event handler.

			CloseSynchronized(DialogResult.Abort);
		}

		/// <summary>
		/// Updates the first status line of the status box.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that debug output is written in any case.")]
		public void SetStatus1Synchronized(string status)
		{
			try
			{
				var sinkTarget = (this as ISynchronizeInvoke);
				if (sinkTarget != null)
				{
					if (sinkTarget.InvokeRequired)
					{
						var sink = new StringMethodDelegate(SetStatus1);
						object[] args = { status };
						sinkTarget.Invoke(sink, args);
					}
					else
					{
						SetStatus1(status);
					}
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(StatusBox), ex);

				throw; // Rethrow!
			}
		}

		/// <summary></summary>
		protected virtual void SetStatus1(string value)
		{
			Status1 = value;
		}

		/// <summary>
		/// Updates the second status line of the status box.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that debug output is written in any case.")]
		public void SetStatus2Synchronized(string status)
		{
			try
			{
				var sinkTarget = (this as ISynchronizeInvoke);
				if (sinkTarget != null)
				{
					if (sinkTarget.InvokeRequired)
					{
						var sink = new StringMethodDelegate(SetStatus2);
						object[] args = { status };
						sinkTarget.Invoke(sink, args);
					}
					else
					{
						SetStatus2(status);
					}
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(StatusBox), ex);

				throw; // Rethrow!
			}
		}

		/// <summary></summary>
		protected virtual void SetStatus2(string value)
		{
			Status2 = value;
		}

		/// <summary>
		/// Closes the status box.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that debug output is written in any case.")]
		public void CloseSynchronized(DialogResult dr = DialogResult.None)
		{
			try
			{
				var sinkTarget = (this as ISynchronizeInvoke);
				if (sinkTarget != null)
				{
					if (sinkTarget.InvokeRequired)
					{
						var sink = new DialogResultMethodDelegate(Close);
						object[] args = { dr };
						sinkTarget.Invoke(sink, args);
					}
					else
					{
						Close(dr);
					}
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(StatusBox), ex);

				throw; // Rethrow!
			}
		}

		/// <summary></summary>
		protected virtual void Close(DialogResult dr)
		{
			DialogResult = dr;
			Close();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
