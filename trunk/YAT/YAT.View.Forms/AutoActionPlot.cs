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
// YAT Version 2.1.1 Development
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

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

// Select the plot library:
#define USE_SCOTT_PLOT
////#define USE_OXY_PLOT

#if (DEBUG) // PENDING: Remove whole section if not using OxyPlot

	// Enable debugging of plot update:
////#define DEBUG_UPDATE

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
#if USE_SCOTT_PLOT
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#endif
using System.Drawing;
#if USE_SCOTT_PLOT
using System.Globalization;
using System.Linq;
#endif
using System.Windows.Forms;

using MKY;
#if USE_SCOTT_PLOT
using MKY.Collections.Specialized;
using MKY.Diagnostics;
#endif
using MKY.Windows.Forms;

#if USE_OXY_PLOT
using YAT.Model;
#endif
using YAT.Model.Types;
using YAT.Settings.Application;

#endregion

namespace YAT.View.Forms
{
	/// <remarks>
	/// Separate <see cref="Form"/> rather than integrated into <see cref="Terminal"/> for...
	/// <list type="bullet">
	/// <item><description>...allowing a user to freely size and position the plot.</description></item>
	/// <item><description>...better decoupling monitor and plot update performance.</description></item>
	/// <item><description>...not adding even more to <see cref="Terminal"/>.</description></item>
	/// </list>
	/// Note that <see cref="Terminal"/> invokes this form using <see cref="Form.Show(IWin32Window)"/>,
	/// specifying the terminal as the owner. Advantages:
	/// <list type="bullet">
	/// <item><description>Behavior as described above.</description></item>
	/// <item><description>Keep plot on top of <see cref="Terminal"/> as well as <see cref="Main"/>.</description></item>
	/// <item><description>Thus possible to position the plot inside the <see cref="Main"/> window.</description></item>
	/// </list>
	/// Disdvantages:
	/// <list type="bullet">
	/// <item><description>Not possible to minimize YAT while still showing the plot.</description></item>
	/// </list>
	/// Potential refinement of this behavior is tracked in https://sourceforge.net/p/y-a-terminal/feature-requests/391/.
	/// </remarks>
	public partial class AutoActionPlot : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Color plotAreaBackColor = SystemColors.Control;

		private bool isStartingUp = true;
		private bool isClosing = false;

	#if USE_SCOTT_PLOT
		private readonly int initialAndMinimumUpdateInterval; // = 0;
		private readonly long lowerSpanTicks; // = 0;
		private readonly long upperSpanTicks; // = 0;
	#endif

		private SettingControlsHelper isSettingControls;

		private Model.Terminal terminal; // = null;

	////private AutoAction lastAction; // = [None]; \remind (2020-01-17 / MKY / FR#391)
	#if USE_SCOTT_PLOT
		private int lastUpdateCount; // = 0;
		private bool updateIsSuspended; // = false;
		private TimedMovingAverageInt64 updateSpanAvg10s;
	#endif

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the PlotAreaBackColor property is changed.")]
		public event EventHandler PlotAreaBackColorChanged;

	/////// <summary></summary>
	////[Category("Action")]
	////[Description("Event raised when the AutoAction is requested to be changed.")]
	////public event EventHandler<EventArgs<AutoAction>> ChangeAutoAction; \remind (2020-01-17 / MKY / FR#391) has been prepared but is yet deactivated for reasons described in FR#391.

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised when the AutoAction is requested to be deactivated.")]
		public event EventHandler DeactivateAutoAction;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public AutoActionPlot(Model.Terminal terminal)
		{
			this.terminal = terminal;

			InitializeComponent();

	#if USE_SCOTT_PLOT
			scottPlot.Visible = true;
			plotView.Visible = false;
	#elif USE_OXY_PLOT
			scottPlot.Visible = false;
			plotView.Visible = true;

			plotView.Model = this.terminal.AutoActionPlotModel.OxyModel;
		////plotView.Model.DefaultFont     = Font.Name;
		////plotView.Model.DefaultFontSize = Font.SizeInPoints;
		////plotView.Model.DefaultFont     = SystemFonts.DefaultFont.Name;
		////plotView.Model.DefaultFontSize = SystemFonts.DefaultFont.SizeInPoints;
			plotView.Model.DefaultFontSize = SystemFonts.DefaultFont.Height; // Best result for unknown reason, clarification PENDING.

			var controller = new OxyPlot.PlotController();
			controller.Bind(new OxyPlot.OxyMouseEnterGesture(), OxyPlot.PlotCommands.HoverPointsOnlyTrack);
			plotView.Controller = controller;
	#endif
			// First do InitializeComponent() and UpdatePlot() related initialization:

	#if USE_SCOTT_PLOT
			this.initialAndMinimumUpdateInterval = timer_Update.Interval; // 73 ms (a prime number)
			var lowerSpan = this.initialAndMinimumUpdateInterval / 10; // 7 ms
			this.lowerSpanTicks = StopwatchEx.TimeToTicks(lowerSpan);
			this.upperSpanTicks = StopwatchEx.TimeToTicks(lowerSpan + 75); // ms
			this.updateSpanAvg10s = new TimedMovingAverageInt64(10000);
	#endif

			// Then initialization which is dependent on the intialization above:

			PlotAreaBackColor = this.terminal.SettingsRoot.Format.BackColor;

			this.isSettingControls.Enter();
			try
			{
			////comboBox_PlotAction.Items.AddRange(AutoActionEx.GetLineScatterHistrogramPlotItems());
			////
			////var yatModel = this.terminal.AutoActionPlotModel; \remind (2020-01-17 / MKY / FR#391)
			////var action = (AutoActionEx)yatModel.Action;
			////ComboBoxHelper.Select(comboBox_PlotAction, action, action);
			////comboBox_PlotAction.Enabled = action.IsLineScatterHistogramPlot;

				checkBox_ShowLegend.Checked = ApplicationSettings.RoamingUserSettings.Plot.ShowLegend;
			}
			finally
			{
				this.isSettingControls.Leave();
			}

	#if USE_OXY_PLOT
			ApplyShowLegend();
	#endif

			ApplyWindowSettingsAccordingToStartupState();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Gets or sets the back color of the plot area.
		/// </summary>
		[Category("Appearance")]
		[Description("The back color of the plot area.")]
		[DefaultValue(typeof(SystemColors), "Control")]
		public Color PlotAreaBackColor
		{
			get { return (this.plotAreaBackColor); }
			set
			{
				if (this.plotAreaBackColor != value)
				{
					this.plotAreaBackColor = value;
					OnPlotAreaBackColorChanged(EventArgs.Empty);
				}
			}
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		private void AutoActionPlot_Shown(object sender, EventArgs e)
		{
			this.isStartingUp = false;
		}

		private void AutoActionPlot_PlotAreaBackColorChanged(object sender, EventArgs e)
		{
			ApplyPlotAreaBackColor();
		}

		private void AutoActionPlot_LocationChanged(object sender, EventArgs e)
		{
			if (!this.isStartingUp && !this.isClosing)
				UpdateWindowSettings(true);
		}

		private void AutoActionPlot_SizeChanged(object sender, EventArgs e)
		{
			if (!this.isStartingUp && !this.isClosing)
				UpdateWindowSettings(false);
		}

		private void AutoActionPlot_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Skip if WinForms has already determined to cancel:
			if (e.Cancel)
				return;

			this.isClosing = true;

			// Save window settings (which has not yet been done at UpdateWindowSettings()):
			SaveWindowSettings();
		}

		private void AutoActionPlot_FormClosed(object sender, FormClosedEventArgs e)
		{
	#if USE_OXY_PLOT
			plotView.Model = null; // Detach, required to potentially attach to model later again.
	#endif
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		/// <remarks>
		/// Opposed to <see cref="View.Controls.Monitor"/>, where update is mostly managed by
		/// <see cref="ListBox"/>, i.e. selective update of added items, the plot is completely
		/// rendered in <see cref="UpdatePlot(out long)"/>. Thus, an update strategy based on
		/// regular polling is used.
		/// </remarks>
		private void timer_Update_Tick(object sender, EventArgs e)
		{
	#if USE_SCOTT_PLOT // PENDING: Remove timer if not using ScottPlot.
			long updateSpanTicksAvg10s;
			if (UpdatePlot(out updateSpanTicksAvg10s)) // No additional synchronization is needed, the System.Windows.Forms.Timer is synchronized.
			{
				DebugUpdate(DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo));
				DebugUpdate(" : Plot update span (moving average) of " + updateSpanTicksAvg10s.ToString(CultureInfo.CurrentCulture) + " ticks = ");
				DebugUpdate(StopwatchEx.TicksToTime(updateSpanTicksAvg10s).ToString(CultureInfo.CurrentCulture) + " ms resulting in ");

				int interval = CalculateUpdateInterval(updateSpanTicksAvg10s);
				if (timer_Update.Interval != interval) {
					timer_Update.Interval = interval;

					DebugUpdate("changed.");
				}
				else {
					DebugUpdate("kept.");
				}

				DebugUpdate(Environment.NewLine);
			}
	#endif
		}

		private void scottPlot_MouseEntered(object sender, EventArgs e)
		{
	#if USE_SCOTT_PLOT
			button_ResetAxis.Enabled = true;
			label_UpdateSuspended.Visible = true;
			this.updateIsSuspended = true;
	#endif
		}

		private void scottPlot_MouseLeft(object sender, EventArgs e)
		{
	#if USE_SCOTT_PLOT
			this.updateIsSuspended = false;
			label_UpdateSuspended.Visible = false;
	#endif
		}

		private void scottPlot_MouseMoved(object sender, EventArgs e)
		{
	#if USE_SCOTT_PLOT
			UpdateHover();
	#endif
		}

		private void plotView_Paint(object sender, PaintEventArgs e)
		{
	#if USE_OXY_PLOT
			// Manage histogram bin labels:
			var oxyModel = plotView.Model;
			var yatModel = this.terminal.AutoActionPlotModel;
			switch (yatModel.Action)
			{
				case AutoAction.HistogramHorizontal:
				{
					if (yatModel.Histogram != null) // 'null' when no data yet or just cleared.
					{
						var widthPerBin = (0.8 * plotView.Width / yatModel.Histogram.BinCount);
						var binsPerStep = (widthPerBin / 48.0); // Arbitrary number ~ "8.88E88", chosen experimentally by resizing plot.

						var axis = (OxyPlot.Axes.CategoryAxis)(oxyModel.Axes[0]);
						axis.MajorStep = ToMajorStep(binsPerStep);

						var showCountLabels = (widthPerBin >= 32.0); // Arbitrary number ~ "8888".
						var series = (OxyPlot.Series.ColumnSeries)(oxyModel.Series[0]);
						series.LabelFormatString = (showCountLabels ? "{0}" : "");
					}
					break;
				}

				case AutoAction.HistogramVertical:
				{
					if (yatModel.Histogram != null) // 'null' when no data yet or just cleared.
					{
						var heightPerBin = (0.8 * plotView.Height / yatModel.Histogram.BinCount);
						var binsPerStep = (heightPerBin / plotView.Font.Height);

						var axis = (OxyPlot.Axes.CategoryAxis)(oxyModel.Axes[0]);
						axis.MajorStep = ToMajorStep(binsPerStep);

						var showCountLabels = (heightPerBin >= plotView.Font.Height);
						var series = (OxyPlot.Series.BarSeries)(oxyModel.Series[0]);
						series.LabelFormatString = (showCountLabels ? "{0}" : "");
					}
					break;
				}

				default:
				{
					break; // Nothing to do.
				}
			}
	#endif
		}

	#if USE_OXY_PLOT
		/// <summary>
		/// Calculates the major step of the histogram bin axis.
		/// </summary>
		protected double ToMajorStep(double binsPerStep)
		{
			if (binsPerStep >= 1.0)
			{
				return (1.0);
			}
			else
			{
				var log10 = (int)Math.Ceiling(Math.Log10(binsPerStep));
				var magnitude = Math.Pow(10, log10);
				var normalized = (binsPerStep / magnitude);

				if      (normalized >= 0.5)
					return (2.0 / magnitude);
				else if (normalized >= 0.2)
					return (5.0 / magnitude);
				else
					return (10.0 / magnitude);
			}
		}
	#endif

	////private void comboBox_PlotAction_SelectedIndexChanged(object sender, EventArgs e)
	////{
	////	if (this.isSettingControls) \remind (2020-01-17 / MKY / FR#391)
	////		return;
	////
	////	var action = (comboBox_PlotAction.SelectedItem as AutoActionEx);
	////	if (action != null)
	////		OnChangeAutoAction(new EventArgs<AutoAction>(action));
	////}

		private void button_ResetAxis_Click(object sender, EventArgs e)
		{
			ResetAxis();
		}

		private void checkBox_ShowLegend_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			ApplicationSettings.RoamingUserSettings.Plot.ShowLegend = !ApplicationSettings.RoamingUserSettings.Plot.ShowLegend;
			ApplicationSettings.SaveRoamingUserSettings();

	#if USE_SCOTT_PLOT
			UpdatePlot(true); // Immediately update, don't wait for update ticker.
	#elif USE_OXY_PLOT
			ApplyShowLegend();
	#endif
		}

		private void button_Clear_Click(object sender, EventArgs e)
		{
			Clear();
		}

		private void button_Deactivate_Click(object sender, EventArgs e)
		{
			OnDeactivateAutoAction(EventArgs.Empty);
		}

		private void button_Close_Click(object sender, EventArgs e)
		{
			Close();
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		private void ApplyWindowSettingsAccordingToStartupState()
		{
			// Attention:
			// Almost the same code exists in Main.ApplyWindowSettingsAccordingToStartupState().
			// Changes here likely have to be applied there too.

			if (ApplicationSettings.LocalUserSettingsSuccessfullyLoadedFromFile)
			{
				// Do not Suspend/ResumeLayout() when changing the form itself!

				// Window state:
				WindowState = ApplicationSettings.LocalUserSettings.PlotWindow.State;

				// Start position:
				var savedStartPosition = ApplicationSettings.LocalUserSettings.PlotWindow.StartPosition;
				var savedLocation      = ApplicationSettings.LocalUserSettings.PlotWindow.Location; // Note the issue/limitation described
				var savedSize          = ApplicationSettings.LocalUserSettings.PlotWindow.Size;     // in SaveWindowSettings() below.

				var savedBounds = new Rectangle(savedLocation, savedSize);
				var isWithinBounds = ScreenEx.IsWithinAnyBounds(savedBounds);
				if (isWithinBounds) // Restore saved settings if within bounds:
				{
					StartPosition = savedStartPosition;
					Location      = savedLocation;
					Size          = savedSize;
				}
				else // Let the operating system adjust the position if out of bounds:
				{
					StartPosition = FormStartPosition.WindowsDefaultBounds;
				}

				// Note that check must be done regardless of the window state, since the state may
				// be changed by the user at any time after the initial layout.
			}
		}

		/// <summary>
		/// Updates the window settings without saving it to the local user settings (yet).
		/// </summary>
		/// <remarks>
		/// Advantage: Prevents many save operations on resizing the form.
		/// Disadvantage: State gets lost if application crashes.
		/// </remarks>
		private void UpdateWindowSettings(bool setStartPositionToManual)
		{
			// Attention:
			// Almost the same code exists in Main.SaveWindowSettings().
			// Changes here likely have to be applied there too.

			if (setStartPositionToManual)
			{
				ApplicationSettings.LocalUserSettings.PlotWindow.StartPosition = FormStartPosition.Manual;
				StartPosition = ApplicationSettings.LocalUserSettings.PlotWindow.StartPosition;
			}

			ApplicationSettings.LocalUserSettings.PlotWindow.State = WindowState;

			if (WindowState == FormWindowState.Normal)
			{
				if (StartPosition == FormStartPosition.Manual)
					ApplicationSettings.LocalUserSettings.PlotWindow.Location = Location;

				ApplicationSettings.LocalUserSettings.PlotWindow.Size = Size;

				// Note the following issue/limitation:
				// Windows or WinForm seems to consider the shadow around a form to belong to the form,
				// i.e. a form that is placed at a screen's edge, may tell values outside the screen.
				//
				// Example with two screens [2] [1] (where 1 is the main screen, and both screens are 1920 × 1080)
				// and the main form placed at the upper left corner, spreading across the whole screen. This may
				// result in the following [LocalUserSettings] values:
				//
				//    <Location>
				//      <X>-1924</X>
				//      <Y>2</Y>
				//    </Location>
				//    <Size>
				//      <Width>1926</Width>
				//      <Height>480</Height>
				//    </Size>
				//
				// Location.X and Size.Width are outside the screen's dimensions even though the form is inside!
				// As a consequence, MKY.Windows.Forms.ScreenEx.IsWithinAnyBounds() will wrongly determine that
				// the form doesn't fit a screen and ApplyWindowSettingsAccordingToStartup() will fall back to
				// 'FormStartPosition.WindowsDefaultBounds'.
				//
				// Issue/limitation is considered very acceptable, neither bug filed nor added to release notes.
			}

			// Don't save right now, see remarks of this method as well as 'SaveWindowSettings()' below.
		}

		private void SaveWindowSettings()
		{
			if (ApplicationSettings.LocalUserSettings.PlotWindow.HaveChanged)
				ApplicationSettings.SaveLocalUserSettings();
		}

	#if USE_SCOTT_PLOT

		/// <summary>
		/// The update interval is calculated dependent on the time needed to update. Less than ~10%
		/// of the CPU load shall be consumed by the plot:
		///
		/// update interval in ms
		///            ^
		/// max = 1125 |--------------x|
		///            |             x |
		///            |            x  |
		///            |           x   |
		///            |        xx     |
		///            |     xx        |
		///   min = 73 |xxx            |
		///            o-----------------> time needed to update in ms
		///            0  7           82
		///
		/// Up to 7 ms, the update is done more or less immediately.
		/// Above 82 ms, the update is done every 1125 milliseconds.
		/// Quadratic inbetween, at y = x^2.
		///
		/// Rationale:
		///  - For better user expericence, interval shall gradually increase.
		///  - Even at high CPU load, there shall still be some updating.
		/// </summary>
		/// <param name="updateSpanTicksAvg10s">
		/// Ticks needed to update.
		/// </param>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Inbetween' is a correct English term.")]
		private int CalculateUpdateInterval(long updateSpanTicksAvg10s)
		{
			int lowerInterval = this.initialAndMinimumUpdateInterval; // 73 ms
			const int UpperInterval = 1125; // = (75*75) / 5 => eases calculation.

			int resultInterval;

			if (updateSpanTicksAvg10s < this.lowerSpanTicks)
			{
				resultInterval = lowerInterval;

				DebugUpdate("minimum update interval of ");
			}
			else if (updateSpanTicksAvg10s > this.upperSpanTicks)
			{
				resultInterval = UpperInterval;

				DebugUpdate("maximum update interval of ");
			}
			else
			{
				int x = StopwatchEx.TicksToTime(updateSpanTicksAvg10s - this.lowerSpanTicks); // Resulting x must be equivalent to max. 75 ms.
				int y = lowerInterval + ((x * x) / 5);

				y = Int32Ex.Limit(y, lowerInterval, UpperInterval); // 'min' and 'max' are fixed.

				resultInterval = y;

				DebugUpdate("calculated update interval of ");
			}

			DebugUpdate(resultInterval.ToString(CultureInfo.CurrentCulture) + " ms ");

			return (resultInterval);
		}

	#endif

		private void ApplyPlotAreaBackColor()
	#if USE_SCOTT_PLOT
		{                          // 'dataBg' = inner part of plot, without title/axis/legend (= 'figBg').
			scottPlot.plt.Style(dataBg: PlotAreaBackColor); // Color is only appied to inner part same as
			UpdatePlot(true);                               // it is only applied to inner part of monitors.
	#elif USE_OXY_PLOT
		{
			var model = plotView.Model;
			if (model != null)
			{
				var oxyColor = OxyPlotEx.ConvertTo(PlotAreaBackColor);

			////model.Background         = oxyColor; // Same as plotView.BackColor ?!?
				model.PlotAreaBackground = oxyColor; // Back color is only appied to inner part same as it is only applied to inner part of monitors.
			////model.LegendBackground is not configurable (yet).

				model.InvalidatePlot(false);
			}
	#endif
		}

		private void ResetAxis()
		{
	#if USE_SCOTT_PLOT
			UpdatePlot(true); // Immediately update, don't wait for update ticker.
	#elif USE_OXY_PLOT
			var model = plotView.Model;
			if (model != null)
			{
				model.ResetAllAxes();
				model.InvalidatePlot(false);
			}
	#endif
		}

	#if USE_OXY_PLOT
		private void ApplyShowLegend()
		{
			var model = plotView.Model;
			if (model != null)
			{
				model.IsLegendVisible = ApplicationSettings.RoamingUserSettings.Plot.ShowLegend;

				model.LegendPlacement   = OxyPlot.LegendPlacement.Outside;
				model.LegendPosition    = OxyPlot.LegendPosition.BottomLeft;
				model.LegendOrientation = OxyPlot.LegendOrientation.Horizontal;

				model.InvalidatePlot(false);
			}
		}
	#endif

		private void Clear()
		{
			lock (this.terminal.AutoActionPlotModelSyncObj)
				this.terminal.AutoActionPlotModel.Clear();

	#if USE_SCOTT_PLOT
			UpdatePlot(true); // Immediately update, don't wait for update ticker.
	#endif
		}

	#if USE_SCOTT_PLOT
		private void UpdatePlot(bool force)
		{
			long spanTicksAvg10sDummy;
			UpdatePlot(force, out spanTicksAvg10sDummy);
		}

		private bool UpdatePlot(out long spanTicksAvg10s)
		{
			return (UpdatePlot(false, out spanTicksAvg10s));
		}

		private bool UpdatePlot(bool force, out long spanTicksAvg10s)
		{
			spanTicksAvg10s = 0;

			if (this.updateIsSuspended && !force) // Only update when allowed.
				return (false);

			var beginTicks = Stopwatch.GetTimestamp(); // Measure including waiting for lock!

			lock (this.terminal.AutoActionPlotModelSyncObj)
			{
				var yatModel = this.terminal.AutoActionPlotModel;

				var doUpdate = ((this.lastUpdateCount != yatModel.UpdateCounter) || force); // Only update when needed.
			////var doUpdate = ((this.lastAction != yatModel.Action) || (this.lastUpdateCount != yatModel.UpdateCounter) || force); // Only update when needed.

			////if (this.lastAction != yatModel.Action) \remind (2020-01-17 / MKY / FR#391)
			////{
			////	this.isSettingControls.Enter();
			////	try
			////	{
			////		var action = (AutoActionEx)yatModel.Action;
			////		ComboBoxHelper.Select(comboBox_PlotAction, action, action);
			////		comboBox_PlotAction.Enabled = action.IsLineScatterHistogramPlot;
			////	}
			////	finally
			////	{
			////		this.isSettingControls.Leave();
			////	}
			////}

				if (doUpdate)
				{
					button_ResetAxis.Enabled = false; // AxisAuto() will be called further below.

					var txColor = this.terminal.SettingsRoot.Format.TxDataFormat.Color;
					var rxColor = this.terminal.SettingsRoot.Format.RxDataFormat.Color;

					scottPlot.plt.Clear();

					scottPlot.plt.Title(yatModel.Title);

					scottPlot.plt.XLabel(yatModel.XLabel);
					scottPlot.plt.YLabel(yatModel.YLabel);

					switch (yatModel.Action)
					{
						case AutoAction.PlotByteCountRate:   PlotCountRate(yatModel, txColor, rxColor                        ); break;
						case AutoAction.PlotLineCountRate:   PlotCountRate(yatModel, txColor, rxColor                        ); break;
						case AutoAction.LineChartIndex:      PlotSignal(   yatModel,          rxColor                        ); break;
						case AutoAction.LineChartTime:       PlotScatter(  yatModel,          rxColor, true,  true           ); break;
						case AutoAction.LineChartTimeStamp:  PlotScatter(  yatModel,          rxColor, true,  true           ); break;
						case AutoAction.ScatterPlot:         PlotScatter(  yatModel,          rxColor, false, false          ); break;
						case AutoAction.HistogramHorizontal: PlotHistogram(yatModel,          rxColor, Orientation.Horizontal); break;
						case AutoAction.HistogramVertical:   PlotHistogram(yatModel,          rxColor, Orientation.Vertical  ); break;

						default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + yatModel.Action.ToString() + "' is a plot type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}

					scottPlot.plt.AxisAuto();
					scottPlot.plt.Legend(enableLegend: ApplicationSettings.RoamingUserSettings.Plot.ShowLegend);

					scottPlot.Render();

				////this.lastAction = yatModel.Action; \remind (2020-01-17 / MKY / FR#391)
					this.lastUpdateCount = yatModel.UpdateCounter;

					var endTicks = Stopwatch.GetTimestamp();
					var spanTicks = (endTicks - beginTicks);
					spanTicksAvg10s = this.updateSpanAvg10s.EnqueueAndCalculate(spanTicks); // No additional synchronization is needed, all access to this form is synchronized onto the main thread.

					return (true);
				}
				else
				{
					return (false);
				}
			}
		}

		private void PlotCountRate(Model.AutoActionPlotModel yatModel, Color txColor, Color rxColor)
		{
			scottPlot.plt.Ticks(dateTimeX: true);

			if ((yatModel.XValues != null) && (yatModel.YValues != null) && (yatModel.YValues.Count > 0))
			{
				for (int i = 0; i < yatModel.YValues.Count; i++)
				{
					switch (i)
					{
						case 0: /* TxCount */ scottPlot.plt.PlotScatter(yatModel.XValues.Item2.ToArray(), yatModel.YValues[i].Item2.ToArray(), color: txColor, label: yatModel.YValues[i].Item1, markerShape: ScottPlot.MarkerShape.none);                                     break;
						case 1: /* TxRate  */ scottPlot.plt.PlotScatter(yatModel.XValues.Item2.ToArray(), yatModel.YValues[i].Item2.ToArray(), color: txColor, label: yatModel.YValues[i].Item1, markerShape: ScottPlot.MarkerShape.none, lineStyle: ScottPlot.LineStyle.Dot); break;
						case 2: /* RxCount */ scottPlot.plt.PlotScatter(yatModel.XValues.Item2.ToArray(), yatModel.YValues[i].Item2.ToArray(), color: rxColor, label: yatModel.YValues[i].Item1, markerShape: ScottPlot.MarkerShape.none);                                     break;
						case 3: /* RxRate  */ scottPlot.plt.PlotScatter(yatModel.XValues.Item2.ToArray(), yatModel.YValues[i].Item2.ToArray(), color: rxColor, label: yatModel.YValues[i].Item1, markerShape: ScottPlot.MarkerShape.none, lineStyle: ScottPlot.LineStyle.Dot); break;

						default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "Index " + i.ToString(CultureInfo.InvariantCulture) + " is a count/rate that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
		}

		private void PlotSignal(Model.AutoActionPlotModel yatModel, Color firstColor)
		{
			if ((yatModel.YValues != null) && (yatModel.YValues.Count > 0))
			{
				var isFirst = true;
				foreach (var kvp in yatModel.YValues)
				{
					if (isFirst)
					{
						isFirst = false;
						scottPlot.plt.PlotSignal(kvp.Item2.ToArray(), color: firstColor, label: kvp.Item1);
					}
					else
					{
						scottPlot.plt.PlotSignal(kvp.Item2.ToArray(), label: kvp.Item1);
					}
				}
			}
		}

		private void PlotScatter(Model.AutoActionPlotModel yatModel, Color firstColor, bool dateTimeX, bool drawLine)
		{
			if (dateTimeX)
				scottPlot.plt.Ticks(dateTimeX: true);

			if ((yatModel.XValues != null) && (yatModel.YValues != null) && (yatModel.YValues.Count > 0))
			{
				var lineWidth = (drawLine ? 1 : 0);

				var isFirst = true;
				foreach (var kvp in yatModel.YValues)
				{
					if (isFirst)
					{
						isFirst = false;
						scottPlot.plt.PlotScatter(yatModel.XValues.Item2.ToArray(), kvp.Item2.ToArray(), color: firstColor, lineWidth: lineWidth, label: kvp.Item1);
					}
					else
					{
						scottPlot.plt.PlotScatter(yatModel.XValues.Item2.ToArray(), kvp.Item2.ToArray(), lineWidth: lineWidth, label: kvp.Item1);
					}
				}
			}
		}

		private void PlotHistogram(Model.AutoActionPlotModel yatModel, Color color, Orientation orientation)
		{
			if (yatModel.Histogram != null)
			{
				if (orientation == Orientation.Horizontal)
					scottPlot.plt.PlotBar(yatModel.Histogram.ValuesMidPoint.ToArray(), yatModel.Histogram.Counts.Select(x => (double)x).ToArray(), barWidth: yatModel.Histogram.BinSize, color: color, label: "All Captures");
				else // PENDING, probably not supported by ScottPlot
					scottPlot.plt.PlotBar(yatModel.Histogram.ValuesMidPoint.ToArray(), yatModel.Histogram.Counts.Select(x => (double)x).ToArray(), barWidth: yatModel.Histogram.BinSize, color: color, label: "All Captures");
			}
		}

		/// <remarks>Based on 'ScottPlotDemos.FormHoverValue'.</remarks>
		private void UpdateHover()
		{
			// Get cursor/mouse position:
			var cursorPos = new Point(Cursor.Position.X, Cursor.Position.Y);

			// Adjust to position on ScottPlot:
			cursorPos.X -= this.PointToScreen(scottPlot.Location).X;
			cursorPos.Y -= this.PointToScreen(scottPlot.Location).Y;

			lock (this.terminal.AutoActionPlotModelSyncObj)
			{
				var yatModel = this.terminal.AutoActionPlotModel;

				switch (yatModel.Action)
				{
					case AutoAction.LineChartIndex:
						UpdateHoverOnSignal(cursorPos);
						break;

					case AutoAction.PlotByteCountRate:
					case AutoAction.PlotLineCountRate:
					case AutoAction.LineChartTime:
					case AutoAction.LineChartTimeStamp:
					case AutoAction.ScatterPlot:
						UpdateHoverOnScatter(cursorPos);
						break;

					case AutoAction.HistogramHorizontal:
					case AutoAction.HistogramVertical:
						UpdateHoverOnHistogram(cursorPos);
						break;

					default:
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + yatModel.Action.ToString() + "' is a plot type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				scottPlot.Render();
			}
		}

		/// <remarks>Based on 'ScottPlotDemos.FormHoverValue'.</remarks>
		private void UpdateHoverOnSignal(Point cursorPos)
		{
			var plottables = scottPlot.plt.GetPlottables();

			// PENDING
		}

		/// <remarks>Based on 'ScottPlotDemos.FormHoverValue'.</remarks>
		private void UpdateHoverOnScatter(Point cursorPos)
		{
			var plottables = scottPlot.plt.GetPlottables();

			// PENDING: How to deal with multiple signals ?!?

			return;

			var scatterPlot      = (ScottPlot.PlottableScatter)plottables[0];
			var highlightScatter = (ScottPlot.PlottableScatter)plottables[1];
			var highlightText    = (ScottPlot.PlottableText)   plottables[2];

			// Determine which point is closest to the cursor/mouse:
			int closestIndex = 0;
			double closestDistance = double.PositiveInfinity;
			for (int i = 0; i < scatterPlot.ys.Length; i++)
			{
				double dx = (cursorPos.X - scottPlot.plt.CoordinateToPixel(scatterPlot.xs[i], 0).X);
				double dy = (cursorPos.Y - scottPlot.plt.CoordinateToPixel(0, scatterPlot.ys[i]).Y);
				double distance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
				if (closestIndex < 0)
				{
					closestDistance = distance;
				}
				else if (distance < closestDistance)
				{
					closestDistance = distance;
					closestIndex = i;
				}
			}

			highlightText.x        = scatterPlot.xs[closestIndex];
			highlightText.y        = scatterPlot.ys[closestIndex];
			highlightScatter.xs[0] = scatterPlot.xs[closestIndex];
			highlightScatter.ys[0] = scatterPlot.ys[closestIndex];

			if (closestDistance < 20)
			{
				highlightText.text = string.Format
				(
					"   ({0}, {1})",
					Math.Round(scatterPlot.xs[closestIndex], 3),
					Math.Round(scatterPlot.ys[closestIndex], 3)
				);
				highlightScatter.markerSize = 10;
			}
			else
			{
				highlightText.text = "";
				highlightScatter.markerSize = 0;
			}
		}

		/// <remarks>Based on 'ScottPlotDemos.FormHoverValue'.</remarks>
		private void UpdateHoverOnHistogram(Point cursorPos)
		{
			var plottables = scottPlot.plt.GetPlottables();

			// PENDING
		}
	#endif

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnPlotAreaBackColorChanged(EventArgs e)
		{
			EventHelper.RaiseSync(PlotAreaBackColorChanged, this, e);
		}

	/////// <summary></summary>
	////protected virtual void OnChangeAutoAction(EventArgs<AutoAction> e)
	////{
	////	EventHelper.RaiseSync(ChangeAutoAction, this, e); \remind (2020-01-17 / MKY / FR#391)
	////}

		/// <summary></summary>
		protected virtual void OnDeactivateAutoAction(EventArgs e)
		{
			EventHelper.RaiseSync(DeactivateAutoAction, this, e);
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

	#if USE_SCOTT_PLOT // PENDING: Remove whole section if not using OxyPlot
		/// <remarks>Using <see cref="Debug.Write(string)"/> for manually composing line.</remarks>
		[Conditional("DEBUG_UPDATE")]
		private void DebugUpdate(string message)
		{
			Debug.Write(message);
		}
	#endif

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
