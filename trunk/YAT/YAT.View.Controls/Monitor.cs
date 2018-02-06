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
// YAT 2.0 Almost Final Version 1.99.95
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

// \remind (MKY 2013-05-25) (related to feature request #163)
// No feasible way to implement horizontal auto scroll found. There are Win32 API functions to move
// the position of the scroll bar itself, and to scroll rectangles, but it is not feasible to do the
// whole translation from .NET Windows.Forms to Win32. Giving up.

// Enable to continue working/testing with an automatic horizontally scrolling list box:
//#define ENABLE_HORIZONTAL_AUTO_SCROLL

#if (DEBUG)

	// Enable debugging of update management (incl. CPU performance measurement):
////#define DEBUG_UPDATE               // The 'DebugEnabled' property is set for 'BiDir' monitor.

	// Enable debugging of vertical semi-auto scrolling:
////#define DEBUG_VERTICAL_AUTO_SCROLL // The 'DebugEnabled' property is set for 'BiDir' monitor.

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
using System.Text.RegularExpressions;
using System.Windows.Forms;

using MKY;
using MKY.Diagnostics;
using MKY.Windows.Forms;

using YAT.Format.Settings;
using YAT.Model.Types;

#endregion

namespace YAT.View.Controls
{
	/// <summary>
	/// This monitor implements a list box based terminal monitor in a speed optimized way.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
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

		private const bool ShowStatusPanelDefault = true;
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
		private const bool ShowBufferLineNumbersDefault = false;
		private const bool ShowTotalLineNumbersDefault = false;

		// Status:
		private const bool ShowTimeStatusDefault = false;
		private const bool ShowDataStatusDefault = false;

		// Copy of active line:
		private const bool ShowCopyOfActiveLineDefault = false;

		// Update:
		private const int DataStatusIntervalMs = 31; // Interval shall be quite short => fixed to 31 ms (a prime number) = approx. 32 updates per second.

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly long DataStatusTickInterval = StopwatchEx.TimeToTicks(DataStatusIntervalMs);

		// Debug:
	#if (DEBUG)
		private const bool DebugEnabledDefault = ListBoxEx.DebugEnabledDefault;
	#endif

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool showStatusPanel = ShowStatusPanelDefault;
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
		private FormatSettings formatSettings = new FormatSettings();

		// Line numbers:
		private long lineNumberOffset;
		private int initialLineNumberWidth;
		private int currentLineNumberWidth;
		private bool showBufferLineNumbers = ShowBufferLineNumbersDefault;
		private bool showTotalLineNumbers = ShowTotalLineNumbersDefault;

		// Status:
		private bool showTimeStatus = ShowTimeStatusDefault;
		private MonitorTimeStatusHelper timeStatusHelper;
		private bool showDataStatus = ShowDataStatusDefault;
		private MonitorDataStatusHelper dataStatusHelper;

		// Copy of active line:
		private bool showCopyOfActiveLine = ShowCopyOfActiveLineDefault;

		// Find:
		private string findPattern; // = null;
		private Regex findRegex; // = null;
		private bool isFirstFindOnEdit = true;
		private int findOnEditStartIndex = ControlEx.InvalidIndex;
		private int lastFindIndex = ListBox.NoMatches;

		// Update:
		private List<object> pendingElementsAndLines = new List<object>(32); // Preset the initial capacity to improve memory management, 32 is an arbitrary value.
		private bool performImmediateUpdate;
		private long monitorUpdateTickInterval;
		private long nextMonitorUpdateTickStamp; // Ticks as defined by 'Stopwatch'.
		private long nextDataStatusUpdateTickStamp; // Ticks as defined by 'Stopwatch'.

		// Note that 'Stopwatch' is used instead of 'DateTime.Now.Ticks' or 'Environment.TickCount'
		// as suggested in several online resources.

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the TextFocusState property is changed.")]
		public event EventHandler TextFocusedChanged;

		/// <summary></summary>
		[Category("Notification")]
		[Description("Event raised when the TextFind has changed.")]
		public event EventHandler FindChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification = "Well, any better idea on how to implement the monitor update timeout?")]
		public Monitor()
		{
			InitializeComponent();

			timer_DataStatusUpdateTimeout.Interval = (DataStatusIntervalMs * 2); // Synchronous update shall have precedence over timeout.

			this.timeStatusHelper = new MonitorTimeStatusHelper();
			this.dataStatusHelper = new MonitorDataStatusHelper();

			this.timeStatusHelper.StatusTextChanged += timeStatusHelper_StatusTextChanged;
			this.dataStatusHelper.StatusTextChanged += dataStatusHelper_StatusTextChanged;

			ApplyFont(); // Required to initialize 'ListBox.ItemHeight', e.g. with scale != 100% (96 DPI).

			// Attention:
			// Since the line number list box will display the vertical scroll bar automatically,
			// the line number list box is placed underneath the monitor list box and sized larger
			// than it would have to be.
			this.initialLineNumberWidth = EffectiveWidthToRequestedWidth(fastListBox_LineNumbers.Width);
			SetControls(this.initialLineNumberWidth);
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Category("Monitor")]
		[Description("Configures status panel visibility.")]
		[DefaultValue(ShowStatusPanelDefault)]
		public virtual bool ShowStatusPanel
		{
			get { return (this.showStatusPanel); }
			set
			{
				if (this.showStatusPanel != value)
				{
					this.showStatusPanel = value;
					SetControls();
				}
			}
		}

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
					SetActivityStateControls();
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
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Only setter required for initialization of control.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual FormatSettings FormatSettings
		{
			set
			{
				if (this.formatSettings != value)
				{
					bool fontHasChanged = (this.formatSettings.Font      != value.Font);
					bool backHasChanged = (this.formatSettings.BackColor != value.BackColor);

					this.formatSettings = value;

					if (backHasChanged)
						fastListBox_Monitor.BackColor = this.formatSettings.BackColor;

					if (fontHasChanged)
						ApplyFont();
					else
						fastListBox_Monitor.Invalidate(); // Required e.g. when enabling/disabling formatting.
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("Show buffer line numbers.")]
		[DefaultValue(ShowBufferLineNumbersDefault)]
		public virtual bool ShowBufferLineNumbers
		{
			get { return (this.showBufferLineNumbers); }
			set
			{
				if (this.showBufferLineNumbers != value)
				{
					this.showBufferLineNumbers = value;

					if (this.showBufferLineNumbers) // This option keeps the offset at 0.
					{
						this.lineNumberOffset = 0;
						fastListBox_LineNumbers.Invalidate();
					}

					SetControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("Show total line numbers.")]
		[DefaultValue(ShowTotalLineNumbersDefault)]
		public virtual bool ShowTotalLineNumbers
		{
			get { return (this.showTotalLineNumbers); }
			set
			{
				if (this.showTotalLineNumbers != value)
				{
					this.showTotalLineNumbers = value;
					SetControls();
				}
			}
		}

		/// <summary>
		/// Sets <see cref="ShowBufferLineNumbers"/> and <see cref="ShowTotalLineNumbers"/> at once.
		/// </summary>
		public virtual void SetLineNumbers(bool showBufferLineNumbers, bool showTotalLineNumbers)
		{
			bool hasChanged = false;

			if (this.showBufferLineNumbers != showBufferLineNumbers)
			{
				this.showBufferLineNumbers = showBufferLineNumbers;

				if (this.showBufferLineNumbers) // This option keeps the offset at 0.
				{
					this.lineNumberOffset = 0;
					fastListBox_LineNumbers.Invalidate();
				}

				hasChanged = true;
			}

			if (this.showTotalLineNumbers != showTotalLineNumbers)
			{
				this.showTotalLineNumbers = showTotalLineNumbers;
				hasChanged = true;
			}

			if (hasChanged)
			{
				SetControls();
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
		[DefaultValue(MonitorDataStatusHelper.CountDefault)]
		public virtual int TxByteCount
		{
			get { return (this.dataStatusHelper.TxByteCount); }
			set { this.dataStatusHelper.TxByteCount = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Tx line count.")]
		[DefaultValue(MonitorDataStatusHelper.CountDefault)]
		public virtual int TxLineCount
		{
			get { return (this.dataStatusHelper.TxLineCount); }
			set { this.dataStatusHelper.TxLineCount = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Rx byte count.")]
		[DefaultValue(MonitorDataStatusHelper.CountDefault)]
		public virtual int RxByteCount
		{
			get { return (this.dataStatusHelper.RxByteCount); }
			set { this.dataStatusHelper.RxByteCount = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Rx line count.")]
		[DefaultValue(MonitorDataStatusHelper.CountDefault)]
		public virtual int RxLineCount
		{
			get { return (this.dataStatusHelper.RxLineCount); }
			set { this.dataStatusHelper.RxLineCount = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Tx byte rate.")]
		[DefaultValue(MonitorDataStatusHelper.RateDefault)]
		public virtual int TxByteRate
		{
			get { return (this.dataStatusHelper.TxByteRate); }
			set { this.dataStatusHelper.TxByteRate = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Tx line rate.")]
		[DefaultValue(MonitorDataStatusHelper.RateDefault)]
		public virtual int TxLineRate
		{
			get { return (this.dataStatusHelper.TxLineRate); }
			set { this.dataStatusHelper.TxLineRate = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Rx byte rate.")]
		[DefaultValue(MonitorDataStatusHelper.RateDefault)]
		public virtual int RxByteRate
		{
			get { return (this.dataStatusHelper.RxByteRate); }
			set { this.dataStatusHelper.RxByteRate = value;  }
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Rx line rate.")]
		[DefaultValue(MonitorDataStatusHelper.RateDefault)]
		public virtual int RxLineRate
		{
			get { return (this.dataStatusHelper.RxLineRate); }
			set { this.dataStatusHelper.RxLineRate = value;  }
		}

		/// <remarks>
		/// Name only "Active" instead of "LastActive" for simplicity.
		/// </remarks>
		[Category("Monitor")]
		[Description("Show a copy of the active line.")]
		[DefaultValue(ShowCopyOfActiveLineDefault)]
		public virtual bool ShowCopyOfActiveLine
		{
			get { return (this.showCopyOfActiveLine); }
			set
			{
				if (this.showCopyOfActiveLine != value)
				{
					this.showCopyOfActiveLine = value;
					SetControls();
				}
			}
		}

		/// <summary></summary>
		[Browsable(false)]
		public virtual bool TextFocused
		{
			get { return (textBox_CopyOfActiveLine.Focused); }
		}

		/// <summary></summary>
		public virtual string FindPattern
		{
			get { return (this.findPattern); }
		}

		/// <summary></summary>
		public virtual Regex FindRegex
		{
			get { return (this.findRegex); }
		}

	#if (DEBUG)

		/// <remarks>
		/// Flag in a addition to configuration item to allow selective debugging of just a single
		/// monitor, e.g. the bidir monitor, to reduce debug output.
		/// </remarks>
		[Category("Scroll")]
		[Description("Enables or disables debugging.")]
		[DefaultValue(DebugEnabledDefault)]
		public virtual bool DebugEnabled
		{
			get { return (this.fastListBox_Monitor.DebugEnabled); }
			set { this.fastListBox_Monitor.DebugEnabled = value;  }
		}

	#endif // DEBUG

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
			this.pendingElementsAndLines.Clear();
			ClearAndResetListBoxes();
		}

		/// <summary></summary>
		protected virtual void Reload()
		{
			var lb = fastListBox_Monitor;

			// Retrieve lines from list box:
			var lines = new List<Domain.DisplayLine>(lb.Items.Count); // Preset the required capacity to improve memory management.
			foreach (object item in lb.Items)
			{
				var line = (item as Domain.DisplayLine);
				if (line != null)
					lines.Add(line);
			}

			// Perform reload:
			Reload(lines);
		}

		/// <summary></summary>
		protected virtual void Reload(List<Domain.DisplayLine> lines)
		{
			Clear();
			AddLines(lines);
		}

		/// <summary></summary>
		public virtual void SelectAll()
		{
			var lb = fastListBox_Monitor;
			lb.BeginUpdate();
			lb.SelectAll();
			lb.EndUpdate();
		}

		/// <summary></summary>
		public virtual void SelectNone()
		{
			var lb = fastListBox_Monitor;
			lb.BeginUpdate();
			lb.ClearSelected();
			lb.EndUpdate();
		}

		/// <summary></summary>
		public virtual void ResetFindOnEdit()
		{
			this.isFirstFindOnEdit = true;
			this.findOnEditStartIndex = ControlEx.InvalidIndex;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool TryFindOnEdit(string pattern, FindOptions options, out FindDirection resultingDirection)
		{
			if (this.isFirstFindOnEdit)
			{
				this.isFirstFindOnEdit = false;

				int nextStartIndex;
				if (TryGetNextStartIndex(out nextStartIndex))
					this.findOnEditStartIndex = nextStartIndex; // Find will start after current item.
				else
					this.findOnEditStartIndex = ControlEx.InvalidIndex; // Find will start at first item.
			}

			if (!string.IsNullOrEmpty(pattern))
			{
				PrepareFind(pattern, options);

				if (TryFindNext(this.findOnEditStartIndex))
				{
					resultingDirection = FindDirection.Forward;
					return (true);
				}

				if (TryFindPrevious(this.findOnEditStartIndex))
				{
					resultingDirection = FindDirection.Backward;
					return (true);
				}
			}

			var lb = fastListBox_Monitor;
			lb.ClearSelected();

			resultingDirection = FindDirection.Undetermined;
			return (false);
		}

		/// <summary></summary>
		public virtual bool TryFindNext(string pattern, FindOptions options)
		{
			this.isFirstFindOnEdit = true;

			PrepareFind(pattern, options);

			return (TryFindNext());
		}

		/// <summary></summary>
		public virtual bool TryFindPrevious(string pattern, FindOptions options)
		{
			this.isFirstFindOnEdit = true;

			PrepareFind(pattern, options);

			return (TryFindPrevious());
		}

		/// <summary></summary>
		protected virtual void PrepareFind(string pattern, FindOptions options)
		{
			this.findPattern = pattern;

			var regexOptions = RegexOptions.Singleline;
			if (!options.CaseSensitive)
				regexOptions |= RegexOptions.IgnoreCase;

			if (options.WholeWord) // Add the Regex word delimiter:
				pattern = string.Format(CultureInfo.CurrentUICulture, "{0}{1}{0}", @"\b", pattern);

			this.findRegex = new Regex(pattern, regexOptions);
		}

		/// <summary></summary>
		protected virtual bool TryFindNext()
		{
			int startIndex;
			if (!TryGetNextStartIndex(out startIndex))
				return (false);

			int findIndex;
			if (!TryFindNext(startIndex, out findIndex))
				return (false);

			this.lastFindIndex = findIndex;
			return (true);
		}

		/// <summary></summary>
		protected virtual bool TryFindPrevious()
		{
			int startIndex;
			if (!TryGetPreviousStartIndex(out startIndex))
				return (false);

			int findIndex;
			if (!TryFindPrevious(startIndex, out findIndex))
				return (false);

			this.lastFindIndex = findIndex;
			return (true);
		}

		/// <summary></summary>
		protected virtual bool TryFindNext(int startIndex)
		{
			int findIndex;
			return (TryFindNext(startIndex, out findIndex));
		}

		/// <summary></summary>
		protected virtual bool TryFindNext(int startIndex, out int findIndex)
		{
			var lb = fastListBox_Monitor;

			var i = lb.FindNext(this.findRegex, startIndex);
			if (i != ListBox.NoMatches)
			{
				lb.ClearSelected();
				lb.SetSelected(i, true);
				lb.TopIndex = Math.Max(i - (lb.TotalVisibleItemCount / 2), 0);

				findIndex = i;
				OnFindChanged(EventArgs.Empty);
				return (true);
			}

			findIndex = ListBox.NoMatches;
			OnFindChanged(EventArgs.Empty);
			return (false);
		}

		/// <summary></summary>
		protected virtual bool TryFindPrevious(int startIndex)
		{
			int findIndex;
			return (TryFindPrevious(startIndex, out findIndex));
		}

		/// <summary></summary>
		protected virtual bool TryFindPrevious(int startIndex, out int findIndex)
		{
			var lb = fastListBox_Monitor;

			var i = lb.FindPrevious(this.findRegex, startIndex);
			if (i != ListBox.NoMatches)
			{
				lb.ClearSelected();
				lb.SetSelected(i, true);
				lb.TopIndex = Math.Max(i - (lb.TotalVisibleItemCount / 2), 0);

				findIndex = i;
				OnFindChanged(EventArgs.Empty);
				return (true);
			}

			findIndex = ListBox.NoMatches;
			OnFindChanged(EventArgs.Empty);
			return (false);
		}

		/// <summary></summary>
		protected virtual bool TryGetCurrentStartIndex(out int result)
		{
			var lb = fastListBox_Monitor;

			if (lb.Items.Count > 0)
			{
				if (lb.LastSelectedIndex != ControlEx.InvalidIndex)
				{
					result = lb.LastSelectedIndex;
					if (result <= lb.LastIndex)
						return (true);
					else
						return (false);
				}

				if (this.lastFindIndex != ListBox.NoMatches)
				{
					result = this.lastFindIndex;
					if (result <= lb.LastIndex)
						return (true);
					else
						return (false);
				}

				result = ControlEx.InvalidIndex;
				return (true);
			}

			result = ControlEx.InvalidIndex;
			return (false);
		}

		/// <summary></summary>
		protected virtual bool TryGetNextStartIndex(out int result)
		{
			var lb = fastListBox_Monitor;

			int currentStartIndex;
			if (TryGetCurrentStartIndex(out currentStartIndex))
			{
				result = (currentStartIndex + 1);
				if (result <= lb.LastIndex)
					return (true);
				else
					return (false);
			}

			result = ControlEx.InvalidIndex;
			return (false);
		}

		/// <summary></summary>
		protected virtual bool TryGetPreviousStartIndex(out int result)
		{
			var lb = fastListBox_Monitor;

			int currentStartIndex;
			if (TryGetCurrentStartIndex(out currentStartIndex))
			{
				result = (currentStartIndex - 1);
				if (result >= lb.FirstIndex)
					return (true);
				else
					return (false);
			}

			result = ControlEx.InvalidIndex;
			return (false);
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
				var lb = fastListBox_Monitor;

				var selectedLines = new List<Domain.DisplayLine>(32); // Preset the initial capacity to improve memory management, 32 is an arbitrary value.
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
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "Why not? 'Byte' not only is a type, but also emphasizes a purpose.")]
		public virtual void SetDataCountStatus(int txByteCount, int txLineCount, int rxByteCount, int rxLineCount)
		{
			if ((txLineCount <= 0) && (rxLineCount <= 0))
			{
				this.lineNumberOffset = 0;
				fastListBox_LineNumbers.Invalidate();
			}

			this.dataStatusHelper.SetCount(txByteCount, txLineCount, rxByteCount, rxLineCount);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "Why not? 'Byte' not only is a type, but also emphasizes a purpose.")]
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

		private void fastListBox_Monitor_SelectedIndexChanged(object sender, EventArgs e)
		{
			var lb = fastListBox_Monitor;

			if (lb.SelectedItems.Count > 0)
				SetCopyOfActiveLine(lb.SelectedItems[0]);
		}

		/// <remarks>
		/// Note that the 'MeasureItem' event only measures the height and an item and is thus
		/// only needed for 'OwnerDrawnVariable' and not for 'OwnerDrawnFixed'.
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
					string lineNumberString = ((this.lineNumberOffset + e.Index + 1).ToString(CultureInfo.CurrentCulture));

					var lb = fastListBox_LineNumbers;
					int requestedWidth;

				////e.DrawBackground(); is not needed and actually draws a white background.
					MonitorRenderer.DrawAndMeasureLineNumber(lineNumberString, this.formatSettings,
					                                         e.Graphics, e.Bounds, out requestedWidth);
				////e.DrawFocusRectangle(); is not needed.

					// The item width is handled here.
					// The item height is set in the 'FormatSettings' property.
					if ((requestedWidth > 0) && (requestedWidth > EffectiveWidthToRequestedWidth(lb.Width)))
						ResizeAndRelocateControls(requestedWidth);
				}
			}
		}

		/// <remarks>
		/// Intentionally initializing to 0 and not ControlEx.InvalidIndex. Doing so would result in
		/// an unnecessary initial VerticalScrollToIndex() request.
		/// 
		/// This also matches to behavior of <see cref="ListBox.TopIndex"/>:
		/// 
		/// Initially, the item with the index position zero (0) is at the top of the visible region
		/// of the ListBox. If the contents of the ListBox have been scrolled, another item might be
		/// at the top of the control's display area.
		/// You can use this property to obtain the index within the ListBox.ObjectCollection for the
		/// ListBox of the item that is currently positioned at the top of the visible region of the
		/// control.
		/// You can also use this property to position an item in the list at the top of the visible
		/// region of the control.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int fastListBox_Monitor_DrawItem_lastTopIndex; // = 0;

		/// <remarks>
		/// Note that the 'MeasureItem' event is not needed for 'OwnerDrawnFixed' (item height only).
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
		/// 2. More sophisticated handling within additional 'FastListBox.OnPaintBackground'
		///    => Doesn't work because list box has already been cleaned to a black background
		///       before this event is invoked, thus it increases flickering too...
		/// 3. Temporarily suspending the adding of elements. The elements are then added upon
		///    the next update. See <see cref="MonitorUpdateHasToBePerformed()"/> for details.
		/// 
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'ms' is the proper abbreviation for milliseconds but StyleCop isn't able to deal with such abbreviations...")]
		private void fastListBox_Monitor_DrawItem(object sender, DrawItemEventArgs e)
		{
			unchecked
			{
				if (e.Index >= 0)
				{
				#if (ENABLE_HORIZONTAL_AUTO_SCROLL)
					ListBoxEx lbmon = fastListBox_Monitor;
				#else
					ListBox lbmon = fastListBox_Monitor;
				#endif
					int requestedWidth;

					if (this.formatSettings.FormattingEnabled)
					{
						// Handle non-standard background:
						if (this.formatSettings.BackColor != SystemColors.Window) // = 'FormatSettings.DefaultBackColor'.
						{
							if ((e.State & DrawItemState.Selected) == 0) // Change only needed if item is not selected.
								e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State, e.ForeColor, this.formatSettings.BackColor);
						}

						e.DrawBackground();

						var dl = (lbmon.Items[e.Index] as Domain.DisplayLine);
						MonitorRenderer.DrawAndMeasureLine(dl, this.formatSettings,
						                                   e.Graphics, e.Bounds, e.State, out requestedWidth);
						e.DrawFocusRectangle();
					}
					else
					{
						e.DrawBackground();

						var dl = (lbmon.Items[e.Index] as Domain.DisplayLine);
						MonitorRenderer.DrawAndMeasureLine(dl.Text, dl.Highlight, e.Font,
						                                   e.Graphics, e.Bounds, e.State, e.ForeColor, e.BackColor, out requestedWidth);
						e.DrawFocusRectangle();

						// Note that it makes no sense to replace the 'FastListBox' by a standard 'ListBox' or 'ListBoxEx'
						// to further increase the speed of this view control. Doing so would only reintroduce flickering!
						// Refer to the 'MKY.Windows.Forms.Test' test application for comparison of the variants.
					}

					// The item width and horizontal extent is handled below.
					// The item height is set in the 'FormatSettings' property.
					if (lbmon.HorizontalExtent < requestedWidth)
						lbmon.HorizontalExtent = requestedWidth;

				#if (ENABLE_HORIZONTAL_AUTO_SCROLL)
					// Perform horizontal auto scroll, but only on the last item:
					if (e.Index == lbmon.LastIndex)
						lbmon.HorizontalScrollToPosition(requestedWidth - e.Bounds.Width);
				#endif

					// Check whether the top index has changed, if so, also scroll the line numbers.
					// Especially applies when monitor gets cleared, the top index will become 0.
					if (fastListBox_Monitor_DrawItem_lastTopIndex != lbmon.TopIndex)
					{
						fastListBox_Monitor_DrawItem_lastTopIndex = lbmon.TopIndex;
						fastListBox_LineNumbers.VerticalScrollToIndex(lbmon.TopIndex);
					}
				}
			}
		}

		private void fastListBox_Monitor_Leave(object sender, EventArgs e)
		{
			fastListBox_Monitor.ClearSelected();
		}

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
		private void timer_MonitorUpdateTimeout_Tick(object sender, EventArgs e)
		{
			StopMonitorUpdateTimeout();
			UpdateFastListBoxWithPendingElementsAndLines();
		}

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
		private void timer_DataStatusUpdateTimeout_Tick(object sender, EventArgs e)
		{
			StopDataStatusUpdateTimeout();
			UpdateDataStatusText();
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int timer_ProcessorLoad_Tick_LastValue = 100;

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
		private void timer_ProcessorLoad_Tick(object sender, EventArgs e)
		{
			// Calculate average of last two samples:

			int currentValue = ProcessorLoad.Update();
			int averageValue = ((currentValue + timer_ProcessorLoad_Tick_LastValue) / 2);
			timer_ProcessorLoad_Tick_LastValue = currentValue;

			DebugUpdate("CPU load = " + averageValue.ToString(CultureInfo.CurrentCulture) + "% resulting in ");
			CalculateUpdateRates(averageValue);
		}

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
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

		private void textBox_CopyOfActiveLine_Enter(object sender, EventArgs e)
		{
			OnTextFocusedChanged(e);
		}

		private void textBox_CopyOfActiveLine_Leave(object sender, EventArgs e)
		{
			OnTextFocusedChanged(e);
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

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private static int EffectiveWidthToRequestedWidth(int effectiveWidth)
		{
			return (effectiveWidth - (VerticalScrollBarWidth + AdditionalMargin));
		}

		private void SetControls()
		{
			SetControls(this.currentLineNumberWidth);
		}

		private void SetControls(int requestedLineNumberWidth)
		{
			panel_Picture.Visible = ShowStatusPanel; // // Adjust monitor for compact view, e.g. in 'FormatSettings' dialog:

			SetRepositoryDependentControls();
			SetActivityStateControls();

			SetTimeStatusVisible();
			SetTimeStatusText();

			SetDataStatusVisible();
			SetDataStatusText();

			ResizeAndRelocateControls(requestedLineNumberWidth);
		}

		private void SetRepositoryDependentControls()
		{
			if (RepositoryType != Domain.RepositoryType.None)
			{
				switch (RepositoryType)
				{
					case Domain.RepositoryType.Tx:    this.imageInactive = Properties.Resources.Image_Monitor_Tx_28x28_Grey;    this.imageActive = Properties.Resources.Image_Monitor_Tx_28x28_Blue;          break;
					case Domain.RepositoryType.Bidir: this.imageInactive = Properties.Resources.Image_Monitor_Bidir_28x28_Grey; this.imageActive = Properties.Resources.Image_Monitor_Bidir_28x28_BluePurple; break;
					case Domain.RepositoryType.Rx:    this.imageInactive = Properties.Resources.Image_Monitor_Rx_28x28_Grey;    this.imageActive = Properties.Resources.Image_Monitor_Rx_28x28_Purple;        break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + RepositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				pictureBox_Monitor.BackgroundImage = this.imageInactive;
			}
		}

		private void SetActivityStateControls()
		{
			if (RepositoryType != Domain.RepositoryType.None)
			{
				switch (ActivityState)
				{
					case MonitorActivityState.Active:   this.imageOpacityState = OpacityState.Inactive; pictureBox_Monitor.Image = this.imageActive; break;
					case MonitorActivityState.Inactive: this.imageOpacityState = OpacityState.Inactive; pictureBox_Monitor.Image = null;             break;

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

				this.activityStateOld = ActivityState;

				timer_Opacity.Enabled = (this.imageOpacityState != OpacityState.Inactive);
			}
		}

		/// <summary>
		/// Applies the font to the list boxes.
		/// </summary>
		/// <remarks>
		/// Directly apply the new settings to the list boxes. This ensures that update is only done
		/// done when required, as the update leads to move of list box to top, and re-drawing. Both
		/// takes time and impacts the monitor behavior. Thus, only update if really needed.
		/// </remarks>
		private void ApplyFont()
		{
			var f = this.formatSettings.Font;

			var lb = fastListBox_LineNumbers;
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

			var tb = textBox_CopyOfActiveLine;
			tb.Font = (Font)f.Clone();
			tb.Invalidate();

			ResizeAndRelocateControls();
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
				StartMonitorUpdateTimeout(StopwatchEx.TicksToTime(this.monitorUpdateTickInterval) * 2); // Synchronous update shall have precedence over timeout.
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
						foreach (var element in elements)
							AddElementToListBox(element);

						continue;
					}
				}
				{
					var line = (obj as Domain.DisplayLine);
					if (line != null)
					{
						foreach (var element in line)
							AddElementToListBox(element);

						continue;
					}
				}
				{
					var lines = (obj as List<Domain.DisplayLine>);
					if (lines != null)
					{
						foreach (var line in lines)
							foreach (var element in line)
								AddElementToListBox(element);

						continue;
					}
				}

				// Kind of 'default':
				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + obj.GetType() + "' is a pending item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			} // foreach (object in pending)

			this.pendingElementsAndLines.Clear();

			// Calculate tick stamp of next update:
			unchecked
			{
				this.nextMonitorUpdateTickStamp = (Stopwatch.GetTimestamp() + this.monitorUpdateTickInterval); // Loop-around is OK.
			}

			if (!lbmon.UserIsScrolling) // Perform auto scroll.
			{
				if (lbmon.VerticalScrollToBottomIfNoVisibleItemOrOnlyOneOfTheLastItemsIsSelected())
					lblin.VerticalScrollToBottom(); // Scroll line numbers accordingly.
			}
			else // UserIsScrolling => Suspend auto scroll.
			{
				if (lbmon.VerticalScrollBarIsNearBottom) // Resume auto scroll.
				{
					if (lbmon.VerticalScrollToBottomIfNoVisibleItemOrOnlyOneOfTheLastItemsIsSelected())
						lblin.VerticalScrollToBottom(); // Scroll line numbers accordingly.
				}
			}

			lblin.EndUpdate();
			lbmon.EndUpdate();

			SetCopyOfActiveLine(lbmon.LastItem);
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
			var lblin = fastListBox_LineNumbers;
			var lbmon = fastListBox_Monitor;

			// If first line, add element to a new line:
			if (lbmon.Items.Count <= 0)
			{
				lblin.Items.Add(0);
				lbmon.Items.Add(new Domain.DisplayLine(element));
			}
			else
			{
				// Get current line:
				var current = (lbmon.LastItem as Domain.DisplayLine);

				// If first element, add element to line:
				if (current.Count <= 0)
				{
					current.Add(element);
				}
				else
				{
					// If a new line starts, add element to a new line.
					if (element is Domain.DisplayElement.LineStart)
					{
						// Remove lines if maximum exceeded:
						while (lbmon.Items.Count >= this.maxLineCount)
						{
							int newTopIndexToRestore = (lbmon.TopIndex - 1); // lbmon is master; decrement accounts for item that will be removed.
							DebugVerticalAutoScroll("Removing least recent item...");
							lblin.Items.RemoveAt(0); // Remove/RemoveAt() resets 'TopIndex' to 0!
							lbmon.Items.RemoveAt(0); // \remind (2017-11-05 / MKY) check if still needed after upgrade to .NET 4.0 or higher (FR#229)
							DebugVerticalAutoScroll("......restoring 'TopIndex'...");
							lblin.TopIndex = newTopIndexToRestore;
							lbmon.TopIndex = newTopIndexToRestore;
							DebugVerticalAutoScroll(".........................done");

							if (!this.showBufferLineNumbers) // This option would require the offset to stay at 0.
							{
								// Increment the offset independent on 'showTotalLineNumbers' to
								// have the indeed total value when the user enables the setting.

								unchecked
								{
									this.lineNumberOffset++; // Overflow is OK.
								}
							}
						}

						// Add element to a new line:
						DebugVerticalAutoScroll("Adding new item..............");
						lblin.Items.Add(0); // 0 = dummy value. 'null' is not valid.
						lbmon.Items.Add(new Domain.DisplayLine(element));
						DebugVerticalAutoScroll(".........................done");
					}
					else
					{
						current.Add(element);
					}
				}
			}
		}

		private void ClearAndResetListBoxes()
		{
			var lblin = fastListBox_LineNumbers;
			lblin.BeginUpdate();
			lblin.Items.Clear();
			lblin.EndUpdate();

			var lbmon = fastListBox_Monitor;
			lbmon.BeginUpdate();
			lbmon.Items.Clear();
			lbmon.NotifyCleared();
			lbmon.HorizontalExtent = 0;
			lbmon.EndUpdate();

			ResizeAndRelocateControls(this.initialLineNumberWidth);
		}

		private void ResizeAndRelocateControls()
		{
			ResizeAndRelocateControls(this.currentLineNumberWidth);
		}

		private void ResizeAndRelocateControls(int requestedLineNumberWidth)
		{
			// --- Width ---

			bool showLN = (this.showBufferLineNumbers || this.showTotalLineNumbers);

			fastListBox_LineNumbers.Visible = showLN;

			if (showLN)
			{
				int effectiveWidth = requestedLineNumberWidth + VerticalScrollBarWidth + AdditionalMargin;
				fastListBox_LineNumbers.Width = effectiveWidth;

				int effectiveLeft = requestedLineNumberWidth + AdditionalMargin; // Hide the vertical scroll
				fastListBox_Monitor.Left = effectiveLeft;                        // bar of the line numbers.
				fastListBox_Monitor.Width = (Width - effectiveLeft);

				textBox_CopyOfActiveLine.Left = effectiveLeft;
				textBox_CopyOfActiveLine.Width = (Width - effectiveLeft);
			}
			else
			{
				fastListBox_Monitor.Left = 0;
				fastListBox_Monitor.Width = Width;

				textBox_CopyOfActiveLine.Left = 0;
				textBox_CopyOfActiveLine.Width = Width;
			}

			this.currentLineNumberWidth = requestedLineNumberWidth;

			// --- Height ---

			var availableHeight = Height;
			var showAL = this.showCopyOfActiveLine;

			textBox_CopyOfActiveLine.Visible = showAL;

			if (showAL)
			{
				int activeLineHeight = (textBox_CopyOfActiveLine.Height + 1); // 1 aligns with 'SendPredefined'.
				textBox_CopyOfActiveLine.Top = (Height - activeLineHeight);

				availableHeight -= (activeLineHeight + 3); // 3 standard margin.
			}
			else
			{
				availableHeight -= 1; // 1 aligns with 'SendPredefined'.
			}

			if (ShowStatusPanel)
			{
				availableHeight -= panel_Picture.Height;

				fastListBox_LineNumbers.Height = availableHeight;
				fastListBox_LineNumbers.Top = panel_Picture.Height;

				fastListBox_Monitor.Height = availableHeight;
				fastListBox_Monitor.Top = panel_Picture.Height;
			}
			else
			{
				fastListBox_LineNumbers.Top = 0;
				fastListBox_LineNumbers.Height = availableHeight;

				fastListBox_Monitor.Top = 0;
				fastListBox_Monitor.Height = availableHeight;
			}
		}

		/// <remarks>
		/// Name only "Active" instead of "LastActive" for simplicity.
		/// </remarks>
		private void SetCopyOfActiveLine(object item)
		{
			var dl = item as Domain.DisplayLine;
			if (dl != null)
				textBox_CopyOfActiveLine.Text = dl.Text;
			else
				textBox_CopyOfActiveLine.Text = "";
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
			unchecked
			{
				this.nextDataStatusUpdateTickStamp = (Stopwatch.GetTimestamp() + DataStatusTickInterval); // Loop-around is OK.
			}
		}

		/// <summary>
		/// The update rate is calculated reverse-proportional to the total CPU load:
		/// 
		///      update interval in ms
		///                 ^
		///      max = 1250 |-------------xx|
		///                 |            x  |
		///                 |           x   |
		///                 |          x    |
		///                 |       xx      |
		/// min = immediate |xxxxx          |
		///       (means 0) o-----------------> total CPU load in %
		///                 0  25  50  75  100
		/// 
		/// Up to 25%, the update is done immediately.
		/// Above 95%, the update is done every 1250 milliseconds.
		/// Quadratic inbetween, at y = x^2.
		/// 
		/// Rationale:
		///  - For better user expericence, interval shall gradually increase.
		///  - Even at high CPU load, there shall still be some updating.
		/// </summary>
		/// <param name="processorLoadPercentage">
		/// Load in %, i.e. values from 0 to 100.
		/// </param>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Inbetween' is a correct English term.")]
		private void CalculateUpdateRates(int processorLoadPercentage)
		{
			const int LowerLoad = 25; // %
			const int UpperLoad = 95; // %

			const int LowerInterval =   41; // Interval shall be quite short => fixed to 41 ms (a prime number) = approx. 24 updates per second.
			const int UpperInterval = 1125; // = (75*75) / 5 => eases calculation.

			if (processorLoadPercentage < LowerLoad)
			{
				this.monitorUpdateTickInterval = StopwatchEx.TimeToTicks(LowerInterval);
				this.performImmediateUpdate = true;

				DebugUpdate("minimum update interval of ");
			}
			else if (processorLoadPercentage > UpperLoad)
			{
				this.monitorUpdateTickInterval = StopwatchEx.TimeToTicks(UpperInterval);
				this.performImmediateUpdate = false;

				DebugUpdate("maximum update interval of ");
			}
			else
			{
				int x = (processorLoadPercentage - LowerLoad); // x is max. 75%
				int y = (x * x) / 5;

				y = Int32Ex.Limit(y, LowerInterval, UpperInterval);

				this.monitorUpdateTickInterval = StopwatchEx.TimeToTicks(y);
				this.performImmediateUpdate = false;

				DebugUpdate("calculated update interval of ");
			}

			DebugUpdate(this.monitorUpdateTickInterval.ToString(CultureInfo.CurrentCulture) + " ticks = ");
			DebugUpdate(StopwatchEx.TicksToTime(this.monitorUpdateTickInterval).ToString(CultureInfo.CurrentCulture) + " ms");
			DebugUpdate(Environment.NewLine);
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

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTextFocusedChanged(EventArgs e)
		{
			EventHelper.RaiseSync(TextFocusedChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnFindChanged(EventArgs e)
		{
			EventHelper.RaiseSync(FindChanged, this, e);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[Conditional("DEBUG_UPDATE")]
		protected virtual void DebugUpdate(string message)
		{
		#if (DEBUG)
			if (DebugEnabled)
		#endif
			{
				Debug.WriteLine(message);
			}
		}

		/// <summary></summary>
		[Conditional("DEBUG_VERTICAL_AUTO_SCROLL")]
		protected virtual void DebugVerticalAutoScroll(string leadMessage)
		{
		#if (DEBUG)
			if (DebugEnabled)
		#endif
			{
				Debug.WriteLine
				(
					string.Format
					(
						CultureInfo.CurrentCulture,
						"{0} : ItemCount = {1} | FullyVisibleItemCount = {2} | TotalVisibleItemCount = {3} | TopIndex = {4} | BottomIndex = {5}",
						leadMessage,
						fastListBox_Monitor.Items.Count,
						fastListBox_Monitor.FullyVisibleItemCount,
						fastListBox_Monitor.TotalVisibleItemCount,
						fastListBox_Monitor.TopIndex,
						fastListBox_Monitor.BottomIndex
					)
				);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
