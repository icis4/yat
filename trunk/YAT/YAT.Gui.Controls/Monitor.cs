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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

using YAT.Gui.Utilities;

#endregion

namespace YAT.Gui.Controls
{
	#region MonitorActivityState Enum
	//==================================================================================================
	// MonitorActivityState Enum
	//==================================================================================================

	/// <summary></summary>
	public enum MonitorActivityState
	{
		/// <summary></summary>
		Inactive,

		/// <summary></summary>
		Active,

		/// <summary></summary>
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

		// Line numbers:
		private const int VerticalScrollBarWidth = 18;
		private const int AdditionalMargin = 4;
		private const bool ShowLineNumbersDefault = false;

		// State:
		private const Domain.RepositoryType RepositoryTypeDefault = Domain.RepositoryType.None;
		private const MonitorActivityState  ActivityStateDefault  = MonitorActivityState.Inactive;

		// Image:
		private const double MinImageOpacity       =  0.00; //   0%
		private const double MaxImageOpacity       =  1.00; // 100%
		private const double ImageOpacityIncrement = +0.10; // +10%
		private const double ImageOpacityDecrement = -0.10; // -10%

		// Lines:
		private const int MaxLineCountDefault = Domain.Settings.DisplaySettings.MaxLineCountDefault;

		// Time status:
		private const bool ShowTimeStatusDefault = false;
		private const bool ShowCountAndRateStatusDefault = false;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// Line numbers:
		private int initialLineNumberWidth;
		private int currentLineNumberWidth;
		private bool showLineNumbers = ShowLineNumbersDefault;

		// State:
		private Domain.RepositoryType repositoryType = RepositoryTypeDefault;
		private MonitorActivityState activityState = ActivityStateDefault;
		private MonitorActivityState activityStateOld = ActivityStateDefault;

		// Image:
		private Image imageInactive = null;
		private Image imageActive = null;
		private OpacityState imageOpacityState = OpacityState.Inactive;
		private double imageOpacity = MinImageOpacity;

		// Lines:
		private int maxLineCount = MaxLineCountDefault;
		private Model.Settings.FormatSettings formatSettings = new Model.Settings.FormatSettings();

		// Time status:
		private bool showTimeStatus = ShowTimeStatusDefault;
		private TimeSpan connectTime;
		private TimeSpan totalConnectTime;

		// Count status:
		private bool showCountAndRateStatus = ShowCountAndRateStatusDefault;

		private int txByteCountStatus;
		private int rxByteCountStatus;
		private int txLineCountStatus;
		private int rxLineCountStatus;

		private int txByteRateStatus;
		private int rxByteRateStatus;
		private int txLineRateStatus;
		private int rxLineRateStatus;

		// Update:
		private long updateTickStamp;
		private long updateTickInterval;
		private bool performImmediateUpdate;
		private List<object> pendingElementsAndLines = new List<object>(32); // The initial capacity typically is 4, that's rather small.

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily", Justification = "The initialization of 'showLineNumbers' is not unnecesary, it is based on a constant that contains a default value!")]
		public Monitor()
		{
			InitializeComponent();

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
					this.formatSettings = value;
					SetFormatDependentControls();
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
					SetTimeStatusControls();
				}
			}
		}

		/// <summary></summary>
		/// <remarks>A default value of TimeSpan.Zero is not possible because it is not constant.</remarks>
		[Category("Monitor")]
		[Description("The connect time.")]
		public virtual TimeSpan ConnectTime
		{
			get { return (this.connectTime); }
			set
			{
				if (this.connectTime != value)
				{
					this.connectTime = value;
					SetTimeStatusControls();
				}
			}
		}

		/// <summary></summary>
		/// <remarks>A default value of TimeSpan.Zero is not possible because it is not constant.</remarks>
		[Category("Monitor")]
		[Description("The total connect time.")]
		public virtual TimeSpan TotalConnectTime
		{
			get { return (this.totalConnectTime); }
			set
			{
				if (this.totalConnectTime != value)
				{
					this.totalConnectTime = value;
					SetTimeStatusControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("Show the count and rate status.")]
		[DefaultValue(ShowCountAndRateStatusDefault)]
		public virtual bool ShowCountAndRateStatus
		{
			get { return (this.showCountAndRateStatus); }
			set
			{
				if (this.showCountAndRateStatus != value)
				{
					this.showCountAndRateStatus = value;
					SetCountAndRateStatusControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Tx byte count status.")]
		[DefaultValue(0)]
		public virtual int TxByteCountStatus
		{
			get { return (this.txByteCountStatus); }
			set
			{
				if (this.txByteCountStatus != value)
				{
					this.txByteCountStatus = value;
					SetCountAndRateStatusControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Tx line count status.")]
		[DefaultValue(0)]
		public virtual int TxLineCountStatus
		{
			get { return (this.txLineCountStatus); }
			set
			{
				if (this.txLineCountStatus != value)
				{
					this.txLineCountStatus = value;
					SetCountAndRateStatusControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Rx byte count status.")]
		[DefaultValue(0)]
		public virtual int RxByteCountStatus
		{
			get { return (this.rxByteCountStatus); }
			set
			{
				if (this.rxByteCountStatus != value)
				{
					this.rxByteCountStatus = value;
					SetCountAndRateStatusControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Rx line count status.")]
		[DefaultValue(0)]
		public virtual int RxLineCountStatus
		{
			get { return (this.rxLineCountStatus); }
			set
			{
				if (this.rxLineCountStatus != value)
				{
					this.rxLineCountStatus = value;
					SetCountAndRateStatusControls();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Tx byte rate status.")]
		[DefaultValue(0)]
		public virtual int TxByteRateStatus
		{
			get { return (this.txByteRateStatus); }
			set
			{
				if (this.txByteRateStatus != value)
				{
					this.txByteRateStatus = value;
					SetCountAndRateStatusControls();
					CalculateUpdateRate();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Tx line rate status.")]
		[DefaultValue(0)]
		public virtual int TxLineRateStatus
		{
			get { return (this.txLineRateStatus); }
			set
			{
				if (this.txLineRateStatus != value)
				{
					this.txLineRateStatus = value;
					SetCountAndRateStatusControls();
					CalculateUpdateRate();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Rx byte rate status.")]
		[DefaultValue(0)]
		public virtual int RxByteRateStatus
		{
			get { return (this.rxByteRateStatus); }
			set
			{
				if (this.rxByteRateStatus != value)
				{
					this.rxByteRateStatus = value;
					SetCountAndRateStatusControls();
					CalculateUpdateRate();
				}
			}
		}

		/// <summary></summary>
		[Category("Monitor")]
		[Description("The Rx line rate status.")]
		[DefaultValue(0)]
		public virtual int RxLineRateStatus
		{
			get { return (this.rxLineRateStatus); }
			set
			{
				if (this.rxLineRateStatus != value)
				{
					this.rxLineRateStatus = value;
					SetCountAndRateStatusControls();
					CalculateUpdateRate();
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

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
		public virtual void Reload()
		{
			ListBox lb = fastListBox_Monitor;

			// Retrieve lines from list box:
			List<Domain.DisplayLine> lines = new List<Domain.DisplayLine>();
			foreach (object item in lb.Items)
			{
				Domain.DisplayLine line = item as Domain.DisplayLine;
				lines.Add(line);
			}

			// Clear everything and perform reload:
			Clear();
			AddLines(lines);
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
		public virtual void ResetTimeStatus()
		{
			this.connectTime = TimeSpan.Zero;
			this.totalConnectTime = TimeSpan.Zero;

			SetTimeStatusControls();
		}

		/// <summary></summary>
		public virtual void ResetCountAndRateStatus()
		{
			this.txByteCountStatus = 0;
			this.txLineCountStatus = 0;
			this.rxByteCountStatus = 0;
			this.rxLineCountStatus = 0;

			this.txByteRateStatus = 0;
			this.txLineRateStatus = 0;
			this.rxByteRateStatus = 0;
			this.rxLineRateStatus = 0;

			SetCountAndRateStatusControls();
		}

		/// <summary></summary>
		public virtual void SelectAll()
		{
			ListBox lb = fastListBox_Monitor;
			lb.BeginUpdate();

			for (int i = 0; i < lb.Items.Count; i++)
				lb.SelectedIndex = i;

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

				List<Domain.DisplayLine> selectedLines = new List<Domain.DisplayLine>();
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

		#endregion

		#region Control Special Keys
		//==========================================================================================
		// Control Special Keys
		//==========================================================================================

		/// <summary></summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			// Ctrl-A = Select All is implemented directly within the monitor since it is a common shortcut.
			// Ctrl-Shift-N = Select None must not be implemented in here, but by the parent form instead.
			if (keyData == (Keys.Control | Keys.A))
			{
				SelectAll();
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
			const int IconDistance = 14; // 14 relates to half the size of the direction icon.
			int middle = (Width / 2);

			label_TimeStatus.Width  = middle - IconDistance;

			label_CountStatus.Left  = middle + IconDistance;
			label_CountStatus.Width = middle - IconDistance;
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
					string lineNumberString = ((e.Index + 1).ToString(CultureInfo.InvariantCulture));

					ListBox lb = fastListBox_LineNumbers;
					SizeF requestedSize;

				////e.DrawBackground(); is not needed and actually draws a white background.
					Drawing.DrawAndMeasureLineNumberString(lineNumberString, this.formatSettings,
					                                       e.Graphics, e.Bounds, out requestedSize);
				////e.DrawFocusRectangle(); is not needed.

					// Only handle the item width.
					// The item height is set in SetFormatDependentControls().
					int requestedWidth = (int)Math.Ceiling(requestedSize.Width);
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
		///    the next update. See <see cref="UpdateHasToBePerformed()"/> for details.
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
					ListBox lb = fastListBox_Monitor;
					SizeF requestedSize;
					SizeF drawnSize;

					e.DrawBackground();
					Drawing.DrawAndMeasureItem(lb.Items[e.Index] as Domain.DisplayLine, this.formatSettings,
					                           e.Graphics, e.Bounds, e.State, out requestedSize, out drawnSize);
					e.DrawFocusRectangle();

					// Only handle the item width and horizontal extent.
					// The item height is set in SetFormatDependentControls().
					int requestedWidth = (int)Math.Ceiling(requestedSize.Width);
					if ((requestedWidth > 0) && (requestedWidth > lb.HorizontalExtent))
						lb.HorizontalExtent = requestedWidth;

					// Check whether the top index has changed, if yes, also scroll the line numbers.
					if (this.fastListBox_Monitor_DrawItem_lastTopIndex != lb.TopIndex)
					{
						this.fastListBox_Monitor_DrawItem_lastTopIndex = lb.TopIndex;
						ListBoxEx.ScrollToIndex(fastListBox_LineNumbers, lb.TopIndex);
					}
				}
			}
		}

		private void fastListBox_Monitor_Leave(object sender, EventArgs e)
		{
			fastListBox_Monitor.ClearSelected();
		}

		/// <summary>
		/// Timeout to ensure that list box is updated even if updates were skipped to improve performance before.
		/// </summary>
		private void timer_UpdateTimeout_Tick(object sender, EventArgs e)
		{
			StopUpdateTimeout();
			UpdateFastListBoxWithPendingElementsAndLines();
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
					case Domain.RepositoryType.Tx:    this.imageInactive = Properties.Resources.Image_Monitor_Tx_28x28_Grey;    this.imageActive = Properties.Resources.Image_Monitor_Tx_28x28_Green;    break;
					case Domain.RepositoryType.Bidir: this.imageInactive = Properties.Resources.Image_Monitor_Bidir_28x28_Grey; this.imageActive = Properties.Resources.Image_Monitor_Bidir_28x28_Green; break;
					case Domain.RepositoryType.Rx:    this.imageInactive = Properties.Resources.Image_Monitor_Rx_28x28_Grey;    this.imageActive = Properties.Resources.Image_Monitor_Rx_28x28_Green;    break;
				}
				pictureBox_Monitor.BackgroundImage = this.imageInactive;

				// Image blending.
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

			SetFormatDependentControls();
			SetTimeStatusControls();
			SetCountAndRateStatusControls();
		}

		private void SetFormatDependentControls()
		{
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
			ListBoxEx.ScrollToBottomIfNoItemButTheLastIsSelected(lb);
			lb.Invalidate();
			lb.EndUpdate();
		}

		private void SetLineNumbersControls()
		{
			ResizeAndRelocateListBoxes(this.currentLineNumberWidth);
		}

		private void SetTimeStatusControls()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(TimeSpanEx.FormatTimeSpan(this.connectTime));
			sb.Append(Environment.NewLine);
			sb.Append(TimeSpanEx.FormatTimeSpan(this.totalConnectTime));

			label_TimeStatus.Text = sb.ToString();
			label_TimeStatus.Visible = this.showTimeStatus;
		}

		private void SetCountAndRateStatusControls()
		{
			StringBuilder sb = new StringBuilder();
			switch (this.repositoryType)
			{
				case Domain.RepositoryType.Tx:
				{
					AppendTxStatus(sb);
					break;
				}
				case Domain.RepositoryType.Bidir:
				{
					AppendTxStatus(sb);
					sb.Append(Environment.NewLine);
					AppendRxStatus(sb);
					break;
				}
				case Domain.RepositoryType.Rx:
				{
					AppendRxStatus(sb);
					break;
				}
			}

			label_CountStatus.Visible = this.showCountAndRateStatus;
			label_CountStatus.Text = sb.ToString();
		}

		private void AppendTxStatus(StringBuilder sb)
		{
			sb.Append(this.txByteCountStatus);
			sb.Append(" | ");
			sb.Append(this.txLineCountStatus);
			sb.Append(" @ ");
			sb.Append(this.txByteRateStatus);
			sb.Append("/s");  // " B/s" is not really readable, compare 1024/s vs. 1024 B/s,
			sb.Append(" | "); //   and consider that the values may be flickering.
			sb.Append(this.txLineRateStatus);
			sb.Append("/s");
		}

		private void AppendRxStatus(StringBuilder sb)
		{
			sb.Append(this.rxByteCountStatus);
			sb.Append(" | ");
			sb.Append(this.rxLineCountStatus);
			sb.Append(" @ ");
			sb.Append(this.rxByteRateStatus);
			sb.Append("/s");
			sb.Append(" | ");
			sb.Append(this.rxLineRateStatus);
			sb.Append("/s");
		}

		private void AddElementsOrLines(object elementsOrLines)
		{
			this.pendingElementsAndLines.Add(elementsOrLines);

			// Either perform the update...
			// ...or arm the update timeout to ensure that update will be performed later:
			if (UpdateHasToBePerformed())
				UpdateFastListBoxWithPendingElementsAndLines();
			else
				RestartUpdateTimeout(TicksToTimeout(this.updateTickInterval));
		}

		private void UpdateFastListBoxWithPendingElementsAndLines()
		{
			ListBox lblin = fastListBox_LineNumbers;
			ListBox lbmon = fastListBox_Monitor;

			lblin.BeginUpdate();
			lbmon.BeginUpdate();

			foreach (object obj in (this.pendingElementsAndLines))
			{
				{
					Domain.DisplayElement element = (obj as Domain.DisplayElement);
					if (element != null)
					{
						AddElementToListBox(element);
						continue;
					}
				}
				{
					List<Domain.DisplayElement> elements = (obj as List<Domain.DisplayElement>);
					if (elements != null)
					{
						foreach (Domain.DisplayElement element in elements)
							AddElementToListBox(element);

						continue;
					}
				}
				{
					Domain.DisplayLine line = (obj as Domain.DisplayLine);
					if (line != null)
					{
						foreach (Domain.DisplayElement element in line)
							AddElementToListBox(element);

						continue;
					}
				}
				{
					List<Domain.DisplayLine> lines = (obj as List<Domain.DisplayLine>);
					if (lines != null)
					{
						foreach (Domain.DisplayLine line in lines)
							foreach (Domain.DisplayElement element in line)
								AddElementToListBox(element);

						continue;
					}
				}
				throw (new InvalidOperationException("Invalid pending element(s) or line(s)"));
			}

			this.pendingElementsAndLines.Clear();

			// Keep tick stamp of update.
			this.updateTickStamp = DateTime.Now.Ticks;

			if (ListBoxEx.ScrollToBottomIfNoItemButTheLastIsSelected(lbmon))
				ListBoxEx.ScrollToBottom(lblin);

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

			// If first line, add element to a new line.
			if (lbmon.Items.Count <= 0)
			{
				lblin.Items.Add(0);
				lbmon.Items.Add(new Domain.DisplayLine(element));
			}
			else
			{
				// Get current line.
				int lastLineIndex = lbmon.Items.Count - 1;
				Domain.DisplayLine current = lbmon.Items[lastLineIndex] as Domain.DisplayLine;

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
						while (lbmon.Items.Count >= (this.maxLineCount))
						{
							lblin.Items.RemoveAt(0);
							lbmon.Items.RemoveAt(0);
						}

						// Add element to a new line.
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

		private static int TicksToTimeout(long ticks)
		{
			return ((int)(ticks / TimeSpan.TicksPerMillisecond));
		}

		private static long TimeoutToTicks(int timeout)
		{
			return ((long)timeout * TimeSpan.TicksPerMillisecond);
		}

		/// <summary>
		/// The update rate is calculated on a non-linear basis that represents the typical speeds
		/// of Rx/Tx data and the human eye. The function is defined as follows:
		/// 
		///    update interval in milliseconds
		///                 ^
		/// max = 1000      | ------------ x
		///                 |              |
		///                 |              |
		///                 |              |
		/// min = immediate | - x          |
		///       (means 0) o-----------------> data rate in bytes per second
		///                    100       1000
		/// 
		/// Thus, up to 100 bytes per second the update is done immediately.
		/// At 1000 bytes per second or more, the update is done once a second.
		/// Linear inbetween, for ease of implementation the 1:1 value is used.
		/// 
		/// An alternative solution would be to measure the effective duration of
		/// an update and then adjust the rate on the duration.Could be tried if
		/// the calculation applied now doesn't work well.
		/// </summary>
		private void CalculateUpdateRate()
		{
			int maxRate = Math.Max(this.txByteRateStatus, this.rxByteRateStatus);

			if (maxRate <= 100)
			{
				this.updateTickInterval = 0;
				this.performImmediateUpdate = true;
			}
			else if (maxRate >= 1000)
			{
				this.updateTickInterval = TimeoutToTicks(1000);
				this.performImmediateUpdate = false;
			}
			else
			{
				this.updateTickInterval = TimeoutToTicks(maxRate);
				this.performImmediateUpdate = false;
			}
		}

		/// <summary>
		/// Either perform the update if immediate update is active (e.g. low data traffic)
		/// or if the tick interval has expired.
		/// </summary>
		private bool UpdateHasToBePerformed()
		{
			// Immediate update.
			if (this.performImmediateUpdate)
				return (true);

			// Calculate whether the update has expired.
			if (DateTime.Now.Ticks >= (this.updateTickStamp + this.updateTickInterval))
				return (true);

			return (false);
		}

		private void StopUpdateTimeout()
		{
			timer_UpdateTimeout.Stop();
		}

		private void RestartUpdateTimeout(int timeout)
		{
			timer_UpdateTimeout.Stop();
			timer_UpdateTimeout.Interval = timeout;
			timer_UpdateTimeout.Start();
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
