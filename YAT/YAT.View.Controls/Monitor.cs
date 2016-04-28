//==================================================================================================
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

// \remind MKY 2013-05-25 (related to feature request #163)
// No feasible way to implement horizontal auto scroll found. There are Win32 API functions to move
// the position of the scroll bar itself, and to scroll rectangles, but it is not feasible to do the
// whole translation from .NET Windows.Forms to Win32. Giving up.

// Enable to continue working/testing with an automatic horizontally scrolling list box:
//#define ENABLE_HORIZONTAL_AUTO_SCROLL

#if (DEBUG)

	// Enable debugging of update management (incl. CPU performance measurement):
////#define DEBUG_UPDATE

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Security.Permissions;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

using YAT.View.Controls.ViewModel;

#endregion

namespace YAT.View.Controls
{
	/// <summary>
	/// This monitor implements a list box based terminal monitor in a speed optimized way.
	/// </summary>
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

		private const Domain.RepositoryType RepositoryTypeDefault = Domain.RepositoryType.None;

		// State:
		private const MonitorActivityState  ActivityStateDefault  = MonitorActivityState.Inactive;
		private const double MinImageOpacity       =  0.00; //   0%
		private const double MaxImageOpacity       =  1.00; // 100%
		private const double ImageOpacityIncrement = +0.10; // +10%
		private const double ImageOpacityDecrement = -0.10; // -10%

		// Lines:
		private const int MaxLineCountDefault = Domain.Settings.DisplaySettings.MaxLineCountDefault;

		// Line numbers:
		private const int VerticalScrollBarWidth = 18;
		private const int AdditionalMargin = 4;
		private const bool ShowLineNumbersDefault = false;

		// Status:
		private const bool ShowTimeStatusDefault = false;
		private const bool ShowDataStatusDefault = false;

		// Update:
		private const int DataStatusIntervalMs = 31; // Interval shall be quite short => fixed to 31 ms (a prime number) = approx. 32 updates per second.
		private readonly long DataStatusTickInterval = TimeoutToTicks(DataStatusIntervalMs);

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Domain.RepositoryType repositoryType = RepositoryTypeDefault;

		// State:
		private MonitorActivityState activityState = ActivityStateDefault;
		private MonitorActivityState activityStateOld = ActivityStateDefault;
		private Image imageInactive = null;
		private Image imageActive = null;
		private OpacityState imageOpacityState = OpacityState.Inactive;
		private double imageOpacity = MinImageOpacity;

		// Lines:
		private int maxLineCount = MaxLineCountDefault;
		private Model.Settings.FormatSettings formatSettings = new Model.Settings.FormatSettings();

		// Line numbers:
		private int initialLineNumberWidth;
		private int currentLineNumberWidth;
		private bool showLineNumbers = ShowLineNumbersDefault;

		// Status:
		private bool showTimeStatus = ShowTimeStatusDefault;
		private MonitorTimeStatusHelper timeStatusHelper;
		private bool showDataStatus = ShowDataStatusDefault;
		private MonitorDataStatusHelper dataStatusHelper;

		// Update:
		private List<object> pendingElementsAndLines = new List<object>(32); // Preset the initial capactiy to improve memory management, 32 is an arbitrary value.
		private bool performImmediateUpdate;
		private long monitorUpdateTickInterval;
		private long nextMonitorUpdateTickStamp; // Ticks as defined by 'Stopwatch'.
		private long nextDataStatusUpdateTickStamp; // Ticks as defined by 'Stopwatch'.

		// Note that 'Stopwatch' is used instead of 'DateTime.Now.Ticks' or 'Environment.TickCount'
		// as suggested in several online resources.

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Monitor()
		{
			InitializeComponent();

			timer_DataStatusUpdateTimeout.Interval = (DataStatusIntervalMs * 2); // Normal update shall have precedence over timeout.

			this.timeStatusHelper = new MonitorTimeStatusHelper();
			this.dataStatusHelper = new MonitorDataStatusHelper();

			this.timeStatusHelper.StatusTextChanged += timeStatusHelper_StatusTextChanged;
			this.dataStatusHelper.StatusTextChanged += dataStatusHelper_StatusTextChanged;

			// Attention:
			// Since the line number list box will display the vertical scroll bar automatically,
			// the line number list box is placed underneath the monitor list box and sized larger
			// than it would have to be.
			this.initialLineNumberWidth = EffectiveWidthToRequestedWidth(fastListBox_LineNumbers.Width);
			ResizeAndRelocateListBoxes(this.initialLineNumberWidth);

			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The repository type.")]
		[DefaultValue(RepositoryTypeDefault)]
		public virtual Domain.RepositoryType RepositoryType
		{
			get { return (this.repositoryType); }
			set
			{
				if (this.repositoryType != value)
				{
					this.repositoryType = value;
					this.dataStatusHelper.RepositoryType = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The activity state.")]
		[DefaultValue(ActivityStateDefault)]
		public virtual MonitorActivityState ActivityState
		{
			get { return (this.activityState); }
			set
			{
				if (this.activityState != value)
				{
					this.activityState = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The maxmimal number of lines to display.")]
		[DefaultValue(MaxLineCountDefault)]
		public virtual int MaxLineCount
		{
			get { return (this.maxLineCount); }
			set
			{
				if (this.maxLineCount != value)
				{
					this.maxLineCount = value;
					Reload();
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Model.Settings.FormatSettings FormatSettings
		{
			set
			{
				if (this.formatSettings != value)
				{
					bool fontHasChanged = (this.formatSettings.Font != value.Font);
					bool backHasChanged = (this.formatSettings.BackColor != value.BackColor);

					this.formatSettings = value;

					if (backHasChanged)
						fastListBox_Monitor.BackColor = this.formatSettings.BackColor;

					if (fontHasChanged)
					{
						// Directly apply the new settings to the list boxes. This ensures that
						// update is only done when required, as the update leads to move of list
						// box to top, and re-drawing. Both takes time, and impacts the monitor
						// behavior. Thus, only update if really needed.

						ListBox lb;
						Font f = this.formatSettings.Font;

						lb = fastListBox_LineNumbers;
						lb.BeginUpdate();
						lb.Font = (Font)f.Clone();
						lb.ItemHeight = f.Height;
						lb.Invalidate();
						lb.EndUpdate();

						lb = fastListBox_Monitor;
						lb.BeginUpdate();
						lb.Font = (Font)f.Clone();
						lb.ItemHeight = f.Height;
						lb.Invalidate();
						lb.EndUpdate();
					}
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("Show the line numbers.")]
		[DefaultValue(ShowLineNumbersDefault)]
		public virtual bool ShowLineNumbers
		{
			get { return (this.showLineNumbers); }
			set
			{
				if (this.showLineNumbers != value)
				{
					this.showLineNumbers = value;
					SetLineNumbersControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("Show the time status.")]
		[DefaultValue(ShowTimeStatusDefault)]
		public virtual bool ShowTimeStatus
		{
			get { return (this.showTimeStatus); }
			set
			{
				if (this.showTimeStatus != value)
				{
					this.showTimeStatus = value;
					SetTimeStatusVisible();
				}
			}
		}

		/// <summary></summary>
		/// <remarks>A default value of TimeSpan.Zero is not possible because it is not constant.</remarks>
		[Category("Monitor")]
		[Description("The active connection time.")]
		public virtual TimeSpan ActiveConnectTime
		{
			get { return (this.timeStatusHelper.ActiveConnectTime); }
			set { this.timeStatusHelper.ActiveConnectTime = value;  }
		}

		/// <summary></summary>
		/// <remarks>A default value of TimeSpan.Zero is not possible because it is not constant.</remarks>
		[Category("Monitor")]
		[Description("The total connection time.")]
		public virtual TimeSpan TotalConnectTime
		{
			get { return (this.timeStatusHelper.TotalConnectTime); }
			set { this.timeStatusHelper.TotalConnectTime = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("Show the data status.")]
		[DefaultValue(ShowDataStatusDefault)]
		public virtual bool ShowDataStatus
		{
			get { return (this.showDataStatus); }
			set
			{
				if (this.showDataStatus != value)
				{
					this.showDataStatus = value;
					SetDataStatusVisible();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Tx byte count.")]
		[DefaultValue(0)]
		public virtual int TxByteCount
		{
			get { return (this.dataStatusHelper.TxByteCount); }
			set { this.dataStatusHelper.TxByteCount = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Tx line count.")]
		[DefaultValue(0)]
		public virtual int TxLineCount
		{
			get { return (this.dataStatusHelper.TxLineCount); }
			set { this.dataStatusHelper.TxLineCount = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Rx byte count.")]
		[DefaultValue(0)]
		public virtual int RxByteCount
		{
			get { return (this.dataStatusHelper.RxByteCount); }
			set { this.dataStatusHelper.RxByteCount = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Rx line count.")]
		[DefaultValue(0)]
		public virtual int RxLineCount
		{
			get { return (this.dataStatusHelper.RxLineCount); }
			set { this.dataStatusHelper.RxLineCount = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Tx byte rate.")]
		[DefaultValue(0)]
		public virtual int TxByteRate
		{
			get { return (this.dataStatusHelper.TxByteRate); }
			set { this.dataStatusHelper.TxByteRate = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Tx line rate.")]
		[DefaultValue(0)]
		public virtual int TxLineRate
		{
			get { return (this.dataStatusHelper.TxLineRate); }
			set { this.dataStatusHelper.TxLineRate = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Rx byte rate.")]
		[DefaultValue(0)]
		public virtual int RxByteRate
		{
			get { return (this.dataStatusHelper.RxByteRate); }
			set { this.dataStatusHelper.RxByteRate = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Rx line rate.")]
		[DefaultValue(0)]
		public virtual int RxLineRate
		{
			get { return (this.dataStatusHelper.RxLineRate); }
			set { this.dataStatusHelper.RxLineRate = value;  }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Activate()
		{
			label_TimeStatus     .BackColor = SystemColors.GradientActiveCaption;
			label_TimeStatusEmpty.BackColor = SystemColors.GradientActiveCaption;

			label_DataStatus     .BackColor = SystemColors.GradientActiveCaption;
			label_DataStatusEmpty.BackColor = SystemColors.GradientActiveCaption;
		}

		/// <summary></summary>
		public virtual void Deactivate()
		{
			label_TimeStatus     .BackColor = SystemColors.Control;
			label_TimeStatusEmpty.BackColor = SystemColors.Control;

			label_DataStatus     .BackColor = SystemColors.Control;
			label_DataStatusEmpty.BackColor = SystemColors.Control;
		}

		/// <summary></summary>
		public virtual void AddElement(Domain.DisplayElement element)
		{
			AddElementsOrLines(element);
		}

		/// <summary></summary>
		public virtual void AddElements(List<Domain.DisplayElement> elements)
		{
			AddElementsOrLines(elements);
		}

		/// <summary></summary>
		public virtual void AddLine(Domain.DisplayLine line)
		{
			AddElementsOrLines(line);
		}

		/// <summary></summary>
		public virtual void AddLines(List<Domain.DisplayLine> lines)
		{
			AddElementsOrLines(lines);
		}

		/// <summary></summary>
		public virtual void Clear()
		{
			ClearAndResetListBoxes();
		}

		/// <summary></summary>
		public virtual void Reload(List<Domain.DisplayElement> elements)
		{
			Clear();
			AddElements(elements);
		}

		/// <summary></summary>
		public virtual void Reload(List<Domain.DisplayLine> lines)
		{
			Clear();
			AddLines(lines);
		}

		/// <summary></summary>
		public virtual void Reload()
		{
			ListBox lb = fastListBox_Monitor;

			// Retrieve lines from list box:
			List<Domain.DisplayLine> lines = new List<Domain.DisplayLine>(lb.Items.Count); // Preset the required capactiy to improve memory management.
			foreach (object item in lb.Items)
			{
				var line = (item as Domain.DisplayLine);
				if (line != null)
					lines.Add(line);
			}

			// Clear and perform reload:
			Reload(lines);
		}

		/// <summary></summary>
		public virtual void SelectAll()
		{
			ListBoxEx lb = fastListBox_Monitor;
			lb.BeginUpdate();
			lb.SelectAllIndices();
			lb.EndUpdate();
		}

		/// <summary></summary>
		public virtual void SelectNone()
		{
			ListBox lb = fastListBox_Monitor;
			lb.BeginUpdate();
			lb.ClearSelected();
			lb.EndUpdate();
		}

		/// <summary></summary>
		public virtual int SelectedLineCount
		{
			get { return (fastListBox_Monitor.SelectedItems.Count); }
		}

		/// <summary></summary>
		public virtual List<Domain.DisplayLine> SelectedLines
		{
			get
			{
				ListBox lb = fastListBox_Monitor;

				List<Domain.DisplayLine> selectedLines = new List<Domain.DisplayLine>(32); // Preset the initial capactiy to improve memory management, 32 is an arbitrary value.
				if (lb.SelectedItems.Count > 0)
				{
					foreach (int i in lb.SelectedIndices)
						selectedLines.Add(lb.Items[i] as Domain.DisplayLine);
				}
				else
				{
					for (int i = 0; i < lb.Items.Count; i++)
						selectedLines.Add(lb.Items[i] as Domain.DisplayLine);
				}

				return (selectedLines);
			}
		}

		/// <summary></summary>
		public virtual void SetTimeStatus(TimeSpan activeConnectTime, TimeSpan totalConnectTime)
		{
			this.timeStatusHelper.Set(activeConnectTime, totalConnectTime);
		}

		/// <summary></summary>
		public virtual void ResetTimeStatus()
		{
			this.timeStatusHelper.Reset();
		}

		/// <summary></summary>
		public virtual void SetDataCountStatus(int txByteCount, int txLineCount, int rxByteCount, int rxLineCount)
		{
			this.dataStatusHelper.SetCount(txByteCount, txLineCount, rxByteCount, rxLineCount);
		}

		/// <summary></summary>
		public virtual void SetDataRateStatus(int txByteRate, int txLineRate, int rxByteRate, int rxLineRate)
		{
			this.dataStatusHelper.SetRate(txByteRate, txLineRate, rxByteRate, rxLineRate);
		}

		/// <summary></summary>
		public virtual void ResetDataStatus()
		{
			this.dataStatusHelper.Reset();
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
			// Ctrl+A = Select All is implemented directly within here since it is a common shortcut.
			if (keyData == (Keys.Control | Keys.A))
			{
				SelectAll();
				return (true);
			}

			// Ctrl+Shift+N = Select None not needs to be implemented in here, it can be implemented
			// by the parent form as a normal menu shortcut.

			return (base.ProcessCmdKey(ref msg, keyData));
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		private void Monitor_Resize(object sender, EventArgs e)
		{
			const int IconDistance = 14; // 14 relates to half the size of the direction icon.
			int middle = (Width / 2);

			label_TimeStatus     .Width  = middle - IconDistance;
			label_TimeStatusEmpty.Width  = middle - IconDistance;

			label_DataStatus     .Left  = middle + IconDistance;
			label_DataStatusEmpty.Left  = middle + IconDistance;
			label_DataStatus     .Width = middle - IconDistance;
			label_DataStatusEmpty.Width = middle - IconDistance;
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		/// <remarks>
		/// Note that the 'MeasureItem' event measures the item height only and is not needed for 'OwnerDrawnFixed'.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to pass.")]
		private void fastListBox_LineNumbers_DrawItem(object sender, DrawItemEventArgs e)
		{
			unchecked
			{
				if (e.Index >= 0)
				{
					string lineNumberString = ((e.Index + 1).ToString(CultureInfo.CurrentCulture));

					ListBox lb = fastListBox_LineNumbers;
					int requestedWidth;

				////e.DrawBackground(); is not needed and actually draws a white background.
					MonitorRenderer.DrawAndMeasureLineNumber(lineNumberString, this.formatSettings,
					                                 e.Graphics, e.Bounds, out requestedWidth);
				////e.DrawFocusRectangle(); is not needed.

					// The item width is handled here.
					// The item height is set in SetFormatDependentControls().
					if ((requestedWidth > 0) && (requestedWidth > EffectiveWidthToRequestedWidth(lb.Width)))
						ResizeAndRelocateListBoxes(requestedWidth);
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int fastListBox_Monitor_DrawItem_lastTopIndex = ControlEx.InvalidIndex;

		/// <remarks>
		/// Note that the 'MeasureItem' event measures the item height only and is not needed for 'OwnerDrawnFixed'.
		/// 
		/// ListBox
		/// -------
		/// 
		/// Whether we like it or not, <see cref="System.Windows.Forms.ListBox.OnDrawItem"/> calls
		/// this method pretty often. Actually it's called twice each time a new line is added. In
		/// addition, another call is needed for the next still empty line. Thus:
		/// 1st line received => 3 calls to DrawItem() at index 0 | 0 | 1
		/// 2nd line received => 5                     at index 0 | 1 | 0 | 1 | 2
		/// 3rd line received => 7                     at index 0 | 1 | 2 | 0 | 1 | 2 | 3
		/// ...
		/// Nth line received => 2*N + 1               at index 0 | 1 | 2...N | 0 | 1 | 2...N | N+1
		/// 
		/// Each call takes a 0..2 ms. For 25 lines this results in something like:
		/// 51 x 2 ms = 100 ms per update!
		/// At least scrolling is handled properly, i.e. as soon as the listbox starts to scroll,
		/// the number of calls doesn't increase anymore.
		/// 
		/// Example measurements for SIR @ 18 samples per second:
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
		/// 
		/// Timed updated FastListBox
		/// -------------------------
		/// In case of really large data, the FastListBox still proved too slow. Thus, a timed
		/// update has been implemented to further improve the performance. Three approaches
		/// have been tried to implement such timed update:
		/// 1. More sophisticated handling within <see cref="fastListBox_Monitor_DrawItem"/>
		///    => Doesn't work because list box's back ground has already been drawn before
		///       this event is invoked, thus it just increases flickering...
		/// 2. More sophisticated handling within <see cref="FastListBox.OnPaintBackground"/>
		///    => Doesn't work because list box has already been cleaned to a black background
		///       before this event is invoked, thus it increases flickering too...
		/// 3. Temporarily suspending the adding of elements. The elements are then added upon
		///    the next update. See <see cref="MonitorUpdateHasToBePerformed()"/> for details.
		/// 
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't seem to be able to deal with abbreviations or extensions such as 'ms'...")]
		private void fastListBox_Monitor_DrawItem(object sender, DrawItemEventArgs e)
		{
			unchecked
			{
				if (e.Index >= 0)
				{
					ListBoxEx lb = fastListBox_Monitor;
					int requestedWidth;
					int drawnWidth;

					// Handle non-standard background:
					if (this.formatSettings.BackColor != SystemColors.Window) // Equals FormatSettings.DefaultBackColor
					{
						if ((e.State & DrawItemState.Selected) != DrawItemState.Selected) // Change only needed if item is not selected
							e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State, e.ForeColor, this.formatSettings.BackColor);
					}

					e.DrawBackground();

					MonitorRenderer.DrawAndMeasureLine((lb.Items[e.Index] as Domain.DisplayLine), this.formatSettings,
					                           e.Graphics, e.Bounds, e.State, out requestedWidth, out drawnWidth);

					e.DrawFocusRectangle();

					// The item width and horizontal extent is handled here.
					// The item height is set in SetFormatDependentControls().
					if ((requestedWidth > 0) && (requestedWidth > lb.HorizontalExtent))
						lb.HorizontalExtent = requestedWidth;

#if (ENABLE_HORIZONTAL_AUTO_SCROLL)
					// Perform horizontal auto scroll, but only on the last item.
					if (e.Index == (lb.Items.Count - 1))
						lb.HorizontalScrollToPosition(requestedWidth - e.Bounds.Width);
#endif
					// Check whether the top index has changed, if so, also scroll the line numbers.
					// This especially is the case when the monitor gets cleared, the top index will
					// become 0.
					if (fastListBox_Monitor_DrawItem_lastTopIndex != lb.TopIndex)
					{
						fastListBox_Monitor_DrawItem_lastTopIndex = lb.TopIndex;
						fastListBox_LineNumbers.VerticalScrollToIndex(lb.TopIndex);
					}
				}
			}
		}

		private void fastListBox_Monitor_Leave(object sender, EventArgs e)
		{
			fastListBox_Monitor.ClearSelected();
		}

		private void timer_MonitorUpdateTimeout_Tick(object sender, EventArgs e)
		{
			StopMonitorUpdateTimeout();
			UpdateFastListBoxWithPendingElementsAndLines();
		}

		private void timer_DataStatusUpdateTimeout_Tick(object sender, EventArgs e)
		{
			StopDataStatusUpdateTimeout();
			UpdateDataStatusText();
		}

		private void timer_TotalProcessorLoad_Tick(object sender, EventArgs e)
		{
			int totalProcessorLoad = (int)performanceCounter_TotalProcessorLoad.NextValue();
			WriteUpdateDebugMessage("CPU load = " + totalProcessorLoad.ToString(CultureInfo.InvariantCulture) + "%");
			CalculateUpdateRates(totalProcessorLoad);
		}

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
				// \fixme:
				// Don't know how to alter image opacity yet.
				pictureBox_Monitor.Image.Opacity = this.imageOpacity
#endif
				if (this.imageOpacity >= ((MaxImageOpacity - MinImageOpacity) / 2))
					pictureBox_Monitor.Image = this.imageActive;
				else
					pictureBox_Monitor.Image = null;
			}
		}

		#endregion

		#region Utilities Event Handlers
		//==========================================================================================
		// Utilities Event Handlers
		//==========================================================================================

		private void timeStatusHelper_StatusTextChanged(object sender, EventArgs e)
		{
			SetTimeStatusText();
		}

		private void dataStatusHelper_StatusTextChanged(object sender, EventArgs e)
		{
			SetDataStatusText();
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
					case Domain.RepositoryType.Tx:    this.imageInactive = Properties.Resources.Image_Monitor_Tx_28x28_Grey;    this.imageActive = Properties.Resources.Image_Monitor_Tx_28x28_Blue;          break;
					case Domain.RepositoryType.Bidir: this.imageInactive = Properties.Resources.Image_Monitor_Bidir_28x28_Grey; this.imageActive = Properties.Resources.Image_Monitor_Bidir_28x28_BluePurple; break;
					case Domain.RepositoryType.Rx:    this.imageInactive = Properties.Resources.Image_Monitor_Rx_28x28_Grey;    this.imageActive = Properties.Resources.Image_Monitor_Rx_28x28_Purple;        break;
				}
				pictureBox_Monitor.BackgroundImage = this.imageInactive;

				// Image blending:
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

				fastListBox_LineNumbers.Height = (Height - panel_Picture.Height);
				fastListBox_LineNumbers.Top = panel_Picture.Height;

				fastListBox_Monitor.Height = (Height - panel_Picture.Height);
				fastListBox_Monitor.Top = panel_Picture.Height;

				panel_Picture.Visible = true;
			}
			else
			{
				panel_Picture.Visible = false;

				fastListBox_LineNumbers.Top = 0;
				fastListBox_LineNumbers.Height = Height;

				fastListBox_Monitor.Top = 0;
				fastListBox_Monitor.Height = Height;
			}

			SetTimeStatusVisible();
			SetTimeStatusText();

			SetDataStatusVisible();
			SetDataStatusText();
		}

		private void AddElementsOrLines(object elementsOrLines)
		{
			this.pendingElementsAndLines.Add(elementsOrLines);

			// Either perform the update...
			// ...or arm the update timeout to ensure that update will be performed later:
			if (MonitorUpdateHasToBePerformed())
			{
				StopMonitorUpdateTimeout();
				UpdateFastListBoxWithPendingElementsAndLines();
			}
			else
			{
				StartMonitorUpdateTimeout(TicksToTimeout(this.monitorUpdateTickInterval) * 2); // Normal update shall have precedence over timeout.
			}
		}

		private void UpdateFastListBoxWithPendingElementsAndLines()
		{
			ListBoxEx lblin = fastListBox_LineNumbers;
			ListBoxEx lbmon = fastListBox_Monitor;

			lblin.BeginUpdate();
			lbmon.BeginUpdate();

			foreach (object obj in (this.pendingElementsAndLines))
			{
				{
					var element = (obj as Domain.DisplayElement);
					if (element != null)
					{
						AddElementToListBox(element);
						continue;
					}
				}
				{
					var elements = (obj as List<Domain.DisplayElement>);
					if (elements != null)
					{
						foreach (Domain.DisplayElement element in elements)
							AddElementToListBox(element);

						continue;
					}
				}
				{
					var line = (obj as Domain.DisplayLine);
					if (line != null)
					{
						foreach (Domain.DisplayElement element in line)
							AddElementToListBox(element);

						continue;
					}
				}
				{
					var lines = (obj as List<Domain.DisplayLine>);
					if (lines != null)
					{
						foreach (Domain.DisplayLine line in lines)
							foreach (Domain.DisplayElement element in line)
								AddElementToListBox(element);

						continue;
					}
				}
				throw (new NotSupportedException("Program execution should never get here, '" + obj.GetType() + "' is an invalid pending item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			} // foreach (object in pending)

			this.pendingElementsAndLines.Clear();

			// Calculate tick stamp of next update:
			this.nextMonitorUpdateTickStamp = (Stopwatch.GetTimestamp() + this.monitorUpdateTickInterval);

			if (lbmon.VerticalScrollToBottomIfNoItemButTheLastIsSelected())
				lblin.VerticalScrollToBottom();

			lblin.EndUpdate();
			lbmon.EndUpdate();
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
			ListBox lblin = fastListBox_LineNumbers;
			ListBox lbmon = fastListBox_Monitor;

			// If first line, add element to a new line:
			if (lbmon.Items.Count <= 0)
			{
				lblin.Items.Add(0);
				lbmon.Items.Add(new Domain.DisplayLine(element));
			}
			else
			{
				// Get current line:
				int lastLineIndex = lbmon.Items.Count - 1;
				Domain.DisplayLine current = (lbmon.Items[lastLineIndex] as Domain.DisplayLine);

				// If first element, add element to line:
				if (current.Count <= 0)
				{
					current.Add(element);
				}
				else
				{
					// If current line has ended, add element to a new line.
					// Otherwise, simply add element to current line:
					int lastElementIndex = current.Count - 1;
					if (current[lastElementIndex] is Domain.DisplayElement.LineBreak)
					{
						// Remove lines if maximum exceeded:
						while (lbmon.Items.Count >= (this.maxLineCount))
						{
							lblin.Items.RemoveAt(0);
							lbmon.Items.RemoveAt(0);
						}

						// Add element to a new line:
						lblin.Items.Add(0);
						lbmon.Items.Add(new Domain.DisplayLine(element));
					}
					else
					{
						current.Add(element);
					}
				}
			}
		}

		private void SetLineNumbersControls()
		{
			ResizeAndRelocateListBoxes(this.currentLineNumberWidth);
		}

		private void ClearAndResetListBoxes()
		{
			ListBox lb;
			
			lb = fastListBox_LineNumbers;
			lb.BeginUpdate();
			lb.Items.Clear();
			lb.EndUpdate();

			lb = fastListBox_Monitor;
			lb.BeginUpdate();
			lb.Items.Clear();
			lb.HorizontalExtent = 0;
			lb.EndUpdate();

			ResizeAndRelocateListBoxes(this.initialLineNumberWidth);
		}

		private void ResizeAndRelocateListBoxes(int requestedWidth)
		{
			fastListBox_LineNumbers.Visible = this.showLineNumbers;

			if (this.showLineNumbers)
			{
				int effectiveWidth = requestedWidth + VerticalScrollBarWidth + AdditionalMargin;
				fastListBox_LineNumbers.Width = effectiveWidth;
				fastListBox_LineNumbers.Invalidate();

				int effectiveLeft = requestedWidth + AdditionalMargin; // Hide the vertical scroll bar.
				fastListBox_Monitor.Left = effectiveLeft;
				fastListBox_Monitor.Width = (Width - effectiveLeft);
				fastListBox_Monitor.Invalidate();
			}
			else
			{
				fastListBox_Monitor.Left = 0;
				fastListBox_Monitor.Width = Width;
				fastListBox_Monitor.Invalidate();
			}

			this.currentLineNumberWidth = requestedWidth;
		}

		private static int EffectiveWidthToRequestedWidth(int effectiveWidth)
		{
			return (effectiveWidth - (VerticalScrollBarWidth + AdditionalMargin));
		}

		/// <remarks>Separated from <see cref="SetTimeStatusText"/> to improve performance.</remarks>
		private void SetTimeStatusVisible()
		{
			label_TimeStatus.Visible      =  this.showTimeStatus;
			label_TimeStatusEmpty.Visible = !this.showTimeStatus;
		}

		private void SetTimeStatusText()
		{
			label_TimeStatus.Text = this.timeStatusHelper.StatusText;
		}

		/// <remarks>Separated from <see cref="SetDataStatusText"/> to improve performance.</remarks>
		private void SetDataStatusVisible()
		{
			label_DataStatus.Visible      =  this.showDataStatus;
			label_DataStatusEmpty.Visible = !this.showDataStatus;
		}

		private void SetDataStatusText()
		{
			// Either perform the update...
			// ...or arm the update timeout to ensure that update will be performed later:
			if (DataStatusUpdateHasToBePerformed())
				UpdateDataStatusText();
			else
				StartDataStatusUpdateTimeout();
		}

		private void UpdateDataStatusText()
		{
			label_DataStatus.Text = this.dataStatusHelper.StatusText;

			// Calculate tick stamp of next update:
			this.nextDataStatusUpdateTickStamp = (Stopwatch.GetTimestamp() + DataStatusTickInterval);
		}

		/// <summary>
		/// The update rate is calculated reverse-proportional to the total CPU load:
		/// 
		///      update interval in ms
		///                 ^
		///      max = 2500 |-----------xxxx|
		///                 |           x   |
		///                 |           x   |
		///                 |          x    |
		///                 |       xx      |
		/// min = immediate |xxxxx          |
		///       (means 0) o-----------------> total CPU load in %
		///                 0  25  50  75  100
		/// 
		/// Up to 25%, the update is done immediately.
		/// Above 75%, the update is done every 2500 milliseconds.
		/// Quadratic inbetween, at y = x^2.
		/// 
		/// Rationale:
		///  - For better user expericence, interval shall gradually increase.
		///  - Even at high CPU load, there shall still be some updating.
		/// </summary>
		/// <param name="totalProcessorLoadInPercent">
		/// Load in %, i.e. values from 0.0 to 100.0.
		/// </param>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "What's wrong with 'inbetween'?")]
		private void CalculateUpdateRates(int totalProcessorLoadInPercent)
		{
			const int LowerLoad = 25; // %
			const int UpperLoad = 75; // %
		////const int LoadSpan  = 50; // %

			const int LowerInterval =   41; // Interval shall be quite short => fixed to 41 ms (a prime number) = approx. 24 updates per second.
			const int UpperInterval = 2500; // = 50*50 => eases calculation.

			if (totalProcessorLoadInPercent < LowerLoad)
			{
				this.monitorUpdateTickInterval = LowerInterval;
				this.performImmediateUpdate = true;

				WriteUpdateDebugMessage("Update interval is minimum:");
			}
			else if (totalProcessorLoadInPercent > UpperLoad)
			{
				this.monitorUpdateTickInterval = TimeoutToTicks(UpperInterval);
				this.performImmediateUpdate = false;

				WriteUpdateDebugMessage("Update interval is maximum:");
			}
			else
			{
				int x = (totalProcessorLoadInPercent - LowerLoad);
				int y = x * x;

				y = MKY.Int32Ex.Limit(y, LowerInterval, UpperInterval);

				this.monitorUpdateTickInterval = TimeoutToTicks(y);
				this.performImmediateUpdate = false;

				WriteUpdateDebugMessage("Update interval is calculated:");
			}

			WriteUpdateDebugMessage(" > " + this.monitorUpdateTickInterval.ToString(CultureInfo.InvariantCulture) + " ticks");
			WriteUpdateDebugMessage(" > " + TicksToTimeout(this.monitorUpdateTickInterval).ToString(CultureInfo.InvariantCulture) + " ms");
		}

		private static int TicksToTimeout(long ticks)
		{
			return ((int)(ticks * 1000 / Stopwatch.Frequency));
		}

		private static long TimeoutToTicks(int timeoutMs)
		{
			return (Stopwatch.Frequency * timeoutMs / 1000);
		}

		/// <summary>
		/// Either perform the update if immediate update is active (e.g. low data traffic)
		/// or if the tick interval has expired.
		/// </summary>
		private bool MonitorUpdateHasToBePerformed()
		{
			// Immediate update:
			if (this.performImmediateUpdate)
				return (true);

			// Calculate whether the update interval has expired:
			if (Stopwatch.GetTimestamp() >= this.nextMonitorUpdateTickStamp)
				return (true);

			return (false);
		}

		private void StopMonitorUpdateTimeout()
		{
			if (timer_MonitorUpdateTimeout.Enabled)
				timer_MonitorUpdateTimeout.Stop();
		}

		/// <param name="timeout">
		/// The value cannot be less than one.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The <paramref name="timeout"/> value is less than one.
		/// </exception>
		private void StartMonitorUpdateTimeout(int timeout)
		{
			if (!timer_MonitorUpdateTimeout.Enabled && (timeout > 0))
			{
				timer_MonitorUpdateTimeout.Interval = timeout;
				timer_MonitorUpdateTimeout.Start();
			}
		}

		/// <summary>
		/// Either perform the update if immediate update is active (e.g. low data traffic)
		/// or if the tick interval has expired.
		/// </summary>
		private bool DataStatusUpdateHasToBePerformed()
		{
			// Immediate update:
			if (this.performImmediateUpdate)
				return (true);

			// Calculate whether the update interval has expired:
			if (Stopwatch.GetTimestamp() >= this.nextDataStatusUpdateTickStamp)
				return (true);

			return (false);
		}

		private void StopDataStatusUpdateTimeout()
		{
			if (timer_DataStatusUpdateTimeout.Enabled)
				timer_DataStatusUpdateTimeout.Stop();
		}

		private void StartDataStatusUpdateTimeout()
		{
			if (!timer_DataStatusUpdateTimeout.Enabled)
				timer_DataStatusUpdateTimeout.Start();
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[Conditional("DEBUG_UPDATE")]
		protected virtual void WriteUpdateDebugMessage(string message)
		{
			Debug.WriteLine(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
