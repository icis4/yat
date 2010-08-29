//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Event;
using MKY.Utilities.Time;
using MKY.Windows.Forms;

using YAT.Gui.Utilities;

namespace YAT.Gui.Controls
{
	#region MonitorActivityState Enum
	//==================================================================================================
	// MonitorActivityState Enum
	//==================================================================================================

	public enum MonitorActivityState
	{
		Inactive,
		Active,
		Pending,
	}

	#endregion

	/// <summary>
	/// This monitor implements a list box based terminal monitor in a speed optimized way.
	/// </summary>
	[DesignerCategory("Windows Forms")]
	public partial class Monitor : UserControl
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum OpacityState
		{
			Inactive,
			Incrementing,
			Decrementing,
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		// State
		private const Domain.RepositoryType RepositoryTypeDefault = Domain.RepositoryType.None;
		private const MonitorActivityState  ActivityStateDefault  = MonitorActivityState.Inactive;

		// Image
		private const double MinImageOpacity       =  0.00; //   0%
		private const double MaxImageOpacity       =  1.00; // 100%
		private const double ImageOpacityIncrement = +0.10; // +10%
		private const double ImageOpacityDecrement = -0.10; // -10%

		// Lines
		private const int MaxLineCountDefault = Domain.Settings.DisplaySettings.MaxLineCountDefault;

		// Time status
		private const bool ShowTimeStatusDefault = false;
		private const bool ShowCountStatusDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// State
		private Domain.RepositoryType repositoryType = RepositoryTypeDefault;
		private MonitorActivityState activityState = ActivityStateDefault;
		private MonitorActivityState activityStateOld = ActivityStateDefault;

		// Image
		private Image imageInactive = null;
		private Image imageActive = null;
		private OpacityState imageOpacityState = OpacityState.Inactive;
		private double imageOpacity = MinImageOpacity;

		// Lines
		private int maxLineCount = MaxLineCountDefault;
		private Model.Settings.FormatSettings formatSettings = new Model.Settings.FormatSettings();

		// Time status
		private bool showTimeStatus = ShowTimeStatusDefault;
		private TimeSpan connectTime;

		// Count status
		private bool showCountStatus = ShowCountStatusDefault;
		private int txByteCountStatus;
		private int rxByteCountStatus;
		private int txLineCountStatus;
		private int rxLineCountStatus;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Action")]
		[Description("Event raised when copying is requested.")]
		public event EventHandler CopyRequest;

		[Category("Action")]
		[Description("Event raised when printing is requested.")]
		public event EventHandler PrintRequest;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public Monitor()
		{
			InitializeComponent();
			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[Category("Monitor")]
		[Description("The repository type.")]
		[DefaultValue(RepositoryTypeDefault)]
		public virtual Domain.RepositoryType RepositoryType
		{
			get { return (this.repositoryType); }
			set
			{
				if (value != this.repositoryType)
				{
					this.repositoryType = value;
					SetControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The activity state.")]
		[DefaultValue(ActivityStateDefault)]
		public virtual MonitorActivityState ActivityState
		{
			get { return (this.activityState); }
			set
			{
				if (value != this.activityState)
				{
					this.activityState = value;
					SetControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The maxmimal number of lines to display.")]
		[DefaultValue(MaxLineCountDefault)]
		public virtual int MaxLineCount
		{
			get { return (this.maxLineCount); }
			set
			{
				if (value != this.maxLineCount)
				{
					this.maxLineCount = value;
					Reload();
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Model.Settings.FormatSettings FormatSettings
		{
			set
			{
				if (value != this.formatSettings)
				{
					this.formatSettings = value;
					SetFormatDependentControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("Show the time status.")]
		[DefaultValue(ShowTimeStatusDefault)]
		public virtual bool ShowTimeStatus
		{
			get { return (this.showTimeStatus); }
			set
			{
				if (value != this.showTimeStatus)
				{
					this.showTimeStatus = value;
					SetTimeStatusControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The connect time status.")]
		[DefaultValue(0)]
		public virtual TimeSpan ConnectTime
		{
			get { return (this.connectTime); }
			set
			{
				if (value != this.connectTime)
				{
					this.connectTime = value;
					SetTimeStatusControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("Show the count status.")]
		[DefaultValue(ShowCountStatusDefault)]
		public virtual bool ShowCountStatus
		{
			get { return (this.showCountStatus); }
			set
			{
				if (value != this.showCountStatus)
				{
					this.showCountStatus = value;
					SetCountStatusControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The Tx byte count status.")]
		[DefaultValue(0)]
		public virtual int TxByteCountStatus
		{
			get { return (this.txByteCountStatus); }
			set
			{
				if (value != this.txByteCountStatus)
				{
					this.txByteCountStatus = value;
					SetCountStatusControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The Tx line count status.")]
		[DefaultValue(0)]
		public virtual int TxLineCountStatus
		{
			get { return (this.txLineCountStatus); }
			set
			{
				if (value != this.txLineCountStatus)
				{
					this.txLineCountStatus = value;
					SetCountStatusControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The Rx byte count status.")]
		[DefaultValue(0)]
		public virtual int RxByteCountStatus
		{
			get { return (this.rxByteCountStatus); }
			set
			{
				if (value != this.rxByteCountStatus)
				{
					this.rxByteCountStatus = value;
					SetCountStatusControls();
				}
			}
		}

		[Category("Monitor")]
		[Description("The Rx line count status.")]
		[DefaultValue(0)]
		public virtual int RxLineCountStatus
		{
			get { return (this.rxLineCountStatus); }
			set
			{
				if (value != this.rxLineCountStatus)
				{
					this.rxLineCountStatus = value;
					SetCountStatusControls();
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		public virtual void AddElement(Domain.DisplayElement element)
		{
			FastListBox flb = fastListBox_Monitor;
			flb.BeginUpdate();

			AddElementToListBox(element);

			flb.ScrollToBottomIfNoItemsSelected();
			flb.EndUpdate();
		}

		public virtual void AddElements(List<Domain.DisplayElement> elements)
		{
			FastListBox flb = fastListBox_Monitor;
			flb.BeginUpdate();

			foreach (Domain.DisplayElement element in elements)
				AddElementToListBox(element);

			flb.ScrollToBottomIfNoItemsSelected();
			flb.EndUpdate();
		}

		public virtual void AddLine(Domain.DisplayLine line)
		{
			FastListBox flb = fastListBox_Monitor;
			flb.BeginUpdate();

			foreach (Domain.DisplayElement element in line)
				AddElementToListBox(element);

			flb.ScrollToBottomIfNoItemsSelected();
			flb.Refresh();
		}

		public virtual void AddLines(List<Domain.DisplayLine> lines)
		{
			FastListBox flb = fastListBox_Monitor;
			flb.BeginUpdate();

			foreach (Domain.DisplayLine line in lines)
				foreach (Domain.DisplayElement element in line)
					AddElementToListBox(element);

			flb.ScrollToBottomIfNoItemsSelected();
			flb.EndUpdate();
		}

		public virtual void Clear()
		{
			ClearListBox();
		}

		public virtual void Reload()
		{
			FastListBox flb = fastListBox_Monitor;

			// Retrieve lines from list box
			List<Domain.DisplayLine> lines = new List<YAT.Domain.DisplayLine>();
			foreach (object item in flb.Items)
			{
				Domain.DisplayLine line = item as Domain.DisplayLine;
				lines.Add(line);
			}

			// Clear everything and perform reload
			Clear();
			AddLines(lines);
		}

		public virtual void Reload(List<Domain.DisplayElement> elements)
		{
			Clear();
			AddElements(elements);
		}

		public virtual void Reload(List<Domain.DisplayLine> lines)
		{
			Clear();
			AddLines(lines);
		}

		public virtual void ResetTimeStatus()
		{
			this.connectTime = TimeSpan.Zero;

			SetTimeStatusControls();
		}

		public virtual void ResetCountStatus()
		{
			this.txByteCountStatus = 0;
			this.txLineCountStatus = 0;
			this.rxByteCountStatus = 0;
			this.rxLineCountStatus = 0;

			SetCountStatusControls();
		}

		public virtual void SelectAll()
		{
			FastListBox flb = fastListBox_Monitor;
			flb.BeginUpdate();

			for (int i = 0; i < flb.Items.Count; i++)
				flb.SelectedIndex = i;

			flb.EndUpdate();
		}

		public virtual void SelectNone()
		{
			FastListBox flb = fastListBox_Monitor;
			flb.BeginUpdate();

			flb.ClearSelected();

			flb.EndUpdate();
		}

		public virtual int SelectedLineCount
		{
			get { return (fastListBox_Monitor.SelectedItems.Count); }
		}

		public virtual List<YAT.Domain.DisplayLine> SelectedLines
		{
			get
			{
				FastListBox flb = fastListBox_Monitor;

				List<Domain.DisplayLine> selectedLines = new List<Domain.DisplayLine>();
				if (flb.SelectedItems.Count > 0)
				{
					foreach (int i in flb.SelectedIndices)
						selectedLines.Add(flb.Items[i] as Domain.DisplayLine);
				}
				else
				{
					for (int i = 0; i < flb.Items.Count; i++)
						selectedLines.Add(flb.Items[i] as Domain.DisplayLine);
				}
				return (selectedLines);
			}
		}

		#endregion

		#region Control Special Keys
		//==========================================================================================
		// Control Special Keys
		//==========================================================================================

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if      (keyData == (Keys.Control | Keys.A)) // Ctrl-A = Select All
			{
				SelectAll();
				return (true);
			}
			else if (keyData == (Keys.Control | Keys.N)) // Ctrl-N = Select None
			{
				SelectNone();
				return (true);
			}
			else if (keyData == (Keys.Control | Keys.C)) // Ctrl-C = Copy
			{
				OnCopyRequest(new EventArgs());
				return (true);
			}
			else if (keyData == (Keys.Control | Keys.P)) // Ctrl-P = Print
			{
				OnPrintRequest(new EventArgs());
				return (true);
			}
			return (base.ProcessCmdKey(ref msg, keyData));
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		private void Monitor_Resize(object sender, EventArgs e)
		{
			int middle = Width / 2;
			label_TimeStatus.Width = middle - 14;
			label_CountStatus.Left = middle + 14;
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void timer_Opacity_Tick(object sender, EventArgs e)
		{
			if (this.imageOpacityState != OpacityState.Inactive)
			{
				if (this.imageOpacityState == OpacityState.Incrementing)
				{
					this.imageOpacity += ImageOpacityIncrement;
					if (this.imageOpacity > MaxImageOpacity)
					{
						this.imageOpacity = MaxImageOpacity;
						this.imageOpacityState = OpacityState.Decrementing;
					}
				}
				else
				{
					this.imageOpacity += ImageOpacityDecrement;
					if (this.imageOpacity < MinImageOpacity)
					{
						this.imageOpacity = MinImageOpacity;
						this.imageOpacityState = OpacityState.Incrementing;
					}
				}
#if (FALSE)
				// \fixme Don't know how to alter image opacity yet
				pictureBox_Monitor.Image.Opacity = this.imageOpacity
#endif
				if (this.imageOpacity >= ((MaxImageOpacity - MinImageOpacity) / 2))
					pictureBox_Monitor.Image = this.imageActive;
				else
					pictureBox_Monitor.Image = null;
			}
		}

#if (FALSE)
		/// <remarks>
		/// Measures item height only, not needed for OwnerDrawnFixed.
		/// </remarks>
		private void listBox_Monitor_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			if (e.Index >= 0)
			{
				if (e.Index >= 0)
				{
					FastListBox flb = listBox_Monitor;

					SizeF size = Draw.MeasureItem((List<Domain.DisplayElement>)(flb.Items[e.Index]), this.formatSettings, e.Graphics, e.Bounds);

					int width  = (int)Math.Ceiling(size.Width);
					int height = (int)Math.Ceiling(size.Height);

					e.ItemWidth  = width;
					e.ItemHeight = height;

					if (width > flb.HorizontalExtent)
						flb.HorizontalExtent = width;

					if (height != flb.ItemHeight)
						flb.ItemHeight = height;
				}
			}
		}
#endif

		/// <remarks>
		///
		/// ListBox
		/// -------
		///
		/// Whether we like it or not, <see cref="System.Windows.Forms.ListBox.OnDrawItem()"/> calls
		/// this method pretty often. Actually it's called twice each time a new line is added. In
		/// addition, another call is needed for the next still empty line. Thus:
		/// 1st line received => 3 calls to DrawItem() at index 0 | 0 | 1
		/// 2nd line received => 5                     at index 0 | 1 | 0 | 1 | 2
		/// 3rd line received => 7                     at index 0 | 1 | 2 | 0 | 1 | 2 | 3
		/// ...
		/// Nth line received => 2*N + 1               at index 0 | 1 | 2...N | 0 | 1 | 2...N | N+1
		/// 
		/// Each call takes a 0..2ms. For 25 lines this results in something like:
		/// 51 x 2ms = 100ms per update!
		/// At least scrolling is handled properly, i.e. as soon as the listbox starts to scroll,
		/// the number of calls doesn't increase anymore.
		/// 
		/// Example measurements for SIR @ 18 upd/s:
		/// 1.99.20 => 30% CPU usage
		/// 1.99.22 with owner drawn and delayed scrolling => 25% CPU usage
		/// 1.99.22 with owner drawn without DrawItem() => 10% CPU usage
		/// 1.99.22 with normal drawn => 20% CPU usage
		/// 
		/// Double-buffered = <c>true</c> (form and control) doesn't make much difference either...
		///
		///
		/// FastListBox
		/// -----------
		///
		/// Fast and smooth :-)
		///
		/// CPU usage is about the same as above, however, FastListBox has no flickering at all
		/// whereas the standard ListBox has.
		///
		/// </remarks>
		private void listBox_Monitor_DrawItem(object sender, DrawItemEventArgs e)
		{
			unchecked
			{
				if (e.Index >= 0)
				{
					FastListBox flb = fastListBox_Monitor;

					e.DrawBackground();
					SizeF size = Drawing.DrawItem(flb.Items[e.Index] as Domain.DisplayLine, this.formatSettings, e.Graphics, e.Bounds, e.State);
					e.DrawFocusRectangle();

					int width  = (int)Math.Ceiling(size.Width);
					int height = (int)Math.Ceiling(size.Height);

					if ((width > 0) && (width > flb.HorizontalExtent))
						flb.HorizontalExtent = width;

					if ((height > 0) && (height != flb.ItemHeight))
						flb.ItemHeight = height;
				}
			}
		}

		private void listBox_Monitor_Leave(object sender, EventArgs e)
		{
			FastListBox flb = fastListBox_Monitor;
			flb.ClearSelected();
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			if (this.repositoryType != Domain.RepositoryType.None)
			{
				switch (this.repositoryType)
				{
					case Domain.RepositoryType.Tx:    this.imageInactive = Properties.Resources.Image_Monitor_Tx_28x28;    this.imageActive = Properties.Resources.Image_Monitor_Tx_28x28_Green;    break;
					case Domain.RepositoryType.Bidir: this.imageInactive = Properties.Resources.Image_Monitor_Bidir_28x28; this.imageActive = Properties.Resources.Image_Monitor_Bidir_28x28_Green; break;
					case Domain.RepositoryType.Rx:    this.imageInactive = Properties.Resources.Image_Monitor_Rx_28x28;    this.imageActive = Properties.Resources.Image_Monitor_Rx_28x28_Green;    break;
				}
				pictureBox_Monitor.BackgroundImage = this.imageInactive;

				// image blending
				switch (this.activityState)
				{
					case MonitorActivityState.Active:   this.imageOpacityState = OpacityState.Inactive; pictureBox_Monitor.Image = this.imageActive; break;
					case MonitorActivityState.Inactive: this.imageOpacityState = OpacityState.Inactive; pictureBox_Monitor.Image = null;         break;
					case MonitorActivityState.Pending:
					{
						if (this.imageOpacityState == OpacityState.Inactive)
						{
							if (this.activityStateOld == MonitorActivityState.Active)
							{
								pictureBox_Monitor.Image = this.imageActive;
								this.imageOpacity = MaxImageOpacity;
								this.imageOpacityState = OpacityState.Decrementing;
							}
							if (this.activityStateOld == MonitorActivityState.Inactive)
							{
								pictureBox_Monitor.Image = this.imageActive;
								this.imageOpacity = MinImageOpacity;
								this.imageOpacityState = OpacityState.Incrementing;
							}
						}
						break;
					}
				}
				this.activityStateOld = this.activityState;

				timer_Opacity.Enabled = (this.imageOpacityState != OpacityState.Inactive);
				panel_Picture.Visible = true;

				fastListBox_Monitor.BringToFront();
				fastListBox_Monitor.Top = panel_Picture.Height;
			}
			else
			{
				panel_Picture.Visible = false;
				fastListBox_Monitor.SendToBack();
			}

			SetFormatDependentControls();
			SetTimeStatusControls();
			SetCountStatusControls();
		}

		private void SetFormatDependentControls()
		{
			FastListBox flb = fastListBox_Monitor;

			flb.BeginUpdate();

			flb.Font = this.formatSettings.Font;
			flb.ItemHeight = this.formatSettings.Font.Height;
			flb.ScrollToBottomIfNoItemsSelected();
			flb.Invalidate();

			flb.EndUpdate();
		}

		private void SetCharReplaceDependentControls()
		{
			fastListBox_Monitor.Invalidate();
		}

		private void SetTimeStatusControls()
		{
			label_TimeStatus.Text = XTimeSpan.FormatTimeSpan(this.connectTime);
			label_TimeStatus.Visible = this.showTimeStatus;
		}

		private void SetCountStatusControls()
		{
			StringBuilder sb = new StringBuilder();
			switch (this.repositoryType)
			{
				case Domain.RepositoryType.Tx:
				{
					sb.Append(this.txByteCountStatus.ToString());
					sb.Append(" / ");
					sb.Append(this.txLineCountStatus.ToString());
					break;
				}
				case Domain.RepositoryType.Bidir:
				{
					sb.Append(this.txByteCountStatus.ToString());
					sb.Append(" / ");
					sb.Append(this.txLineCountStatus.ToString());
					sb.Append(Environment.NewLine);
					sb.Append(this.rxByteCountStatus.ToString());
					sb.Append(" / ");
					sb.Append(this.rxLineCountStatus.ToString());
					break;
				}
				case Domain.RepositoryType.Rx:
				{
					sb.Append(this.rxByteCountStatus.ToString());
					sb.Append(" / ");
					sb.Append(this.rxLineCountStatus.ToString());
					break;
				}
			}
			label_CountStatus.Text = sb.ToString();
			label_CountStatus.Visible = this.showCountStatus;
		}

		/// <summary>
		/// Adds an element to the list box.
		/// </summary>
		/// <remarks>
		/// Neither calls <see cref="ListBox.BeginUpdate()"/> nor <see cref="ListBox.EndUpdate()"/>.
		/// If performance requires it, the calling function must do so.
		/// </remarks>
		private void AddElementToListBox(Domain.DisplayElement element)
		{
			FastListBox flb = fastListBox_Monitor;

			// If first line, add element to a new line.
			if (flb.Items.Count <= 0)
			{
				flb.Items.Add(new Domain.DisplayLine(element));
			}
			else
			{
				// Get current line.
				int lastLineIndex = flb.Items.Count - 1;
				Domain.DisplayLine current = flb.Items[lastLineIndex] as Domain.DisplayLine;

				// If first element, add element to line.
				if (current.Count <= 0)
				{
					current.Add(element);
				}
				else
				{
					// If current line has ended, add element to a new line.
					// Otherwise, simply add element to current line.
					int lastElementIndex = current.Count - 1;
					if (current[lastElementIndex] is Domain.DisplayElement.LineBreak)
					{
						// Remove lines if maximum exceeded.
						while (flb.Items.Count >= (this.maxLineCount))
							flb.Items.RemoveAt(0);

						// Add element to a new line.
						flb.Items.Add(new Domain.DisplayLine(element));
					}
					else
					{
						current.Add(element);
					}
				}
			}
		}

		private void ClearListBox()
		{
			FastListBox flb = fastListBox_Monitor;
			flb.BeginUpdate();

			flb.Items.Clear();
			flb.HorizontalExtent = 0;

			flb.EndUpdate();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnCopyRequest(EventArgs e)
		{
			EventHelper.FireSync(CopyRequest, this, e);
		}

		protected virtual void OnPrintRequest(EventArgs e)
		{
			EventHelper.FireSync(PrintRequest, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
