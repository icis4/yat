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

#if (DEBUG)

	// Enable debugging of plot update:
////#define DEBUG_UPDATE

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using MKY;
using MKY.Collections.Specialized;
using MKY.Diagnostics;
using MKY.Windows.Forms;

using YAT.Model.Types;
using YAT.Settings.Application;

#endregion

namespace YAT.View.Forms
{
	/// <remarks>
	/// Separate <see cref="Form"/> rather than integrated into <see cref="Terminal"/> for...
	/// <list type="bullet">
	/// <item><description>...allowing a user to minimize YAT while still showing the plot.</description></item>
	/// <item><description>...better decoupling monitor and plot update performance.</description></item>
	/// <item><description>...no adding even more to <see cref="Terminal"/>.</description></item>
	/// </list>
	/// Potential refinement of this behavior is tracked in https://sourceforge.net/p/y-a-terminal/feature-requests/391/.
	/// </remarks>
	public partial class AutoActionPlot : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isStartingUp = true;
		private bool isClosing = false;

		private readonly int initialAndMinimumUpdateInterval; // = 0;
		private readonly long lowerSpanTicks; // = 0;
		private readonly long upperSpanTicks; // = 0;

		private SettingControlsHelper isSettingControls;

		private Model.Terminal model; // = null;

		private int lastUpdateCount; // = 0;
		private bool updateIsSuspended; // = false;
		private TimedMovingAverageInt64 updateSpanAvg10s;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler DeactivateAutoAction;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public AutoActionPlot(Model.Terminal model)
		{
			this.model = model;

			InitializeComponent();

			// First do InitializeComponent() and UpdatePlot() related initialization:

			this.initialAndMinimumUpdateInterval = timer_Update.Interval; // 73 ms (a prime number)
			var lowerSpan = this.initialAndMinimumUpdateInterval / 10; // 7 ms
			this.lowerSpanTicks = StopwatchEx.TimeToTicks(lowerSpan);
			this.upperSpanTicks = StopwatchEx.TimeToTicks(lowerSpan + 75); // ms
			this.updateSpanAvg10s = new TimedMovingAverageInt64(10000);

			// Then initialization which is dependent on the intialization above:

			SetBackColor(this.model.SettingsRoot.Format.BackColor);

			this.isSettingControls.Enter();
			try
			{
				checkBox_ShowLegend.Checked = ApplicationSettings.RoamingUserSettings.Plot.ShowLegend;
			}
			finally
			{
				this.isSettingControls.Leave();
			}

			ApplyWindowSettingsAccordingToStartupState();
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

		private void AutoActionPlot_BackColorChanged(object sender, EventArgs e)
		{
			SetBackColor(BackColor);
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
			long updateSpanTicksAvg10s;
			if (UpdatePlot(out updateSpanTicksAvg10s)) // No additional synchronization is needed, the System.Windows.Forms.Timer is synchronized.
			{
				DebugUpdate("Update span (moving average) of " + updateSpanTicksAvg10s.ToString(CultureInfo.CurrentCulture) + " ticks = ");
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
		}

		private void scottPlot_MouseEntered(object sender, EventArgs e)
		{
			button_FitAxis.Enabled = true;
			label_UpdateSuspended.Visible = true;
			this.updateIsSuspended = true;
		}

		private void scottPlot_MouseLeft(object sender, EventArgs e)
		{
			this.updateIsSuspended = false;
			label_UpdateSuspended.Visible = false;
		}

		private void scottPlot_MouseMoved(object sender, EventArgs e)
		{
			UpdateHover();
		}

		private void button_FitAxis_Click(object sender, EventArgs e)
		{
			FitAxis();
		}

		private void checkBox_ShowLegend_CheckedChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			ApplicationSettings.RoamingUserSettings.Plot.ShowLegend = !ApplicationSettings.RoamingUserSettings.Plot.ShowLegend;
			ApplicationSettings.SaveRoamingUserSettings();

			UpdatePlot(true); // Immediately update, don't wait for update ticker.
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

		private void SetBackColor(Color backColor)
		{                          // 'dataBg' = inner part of plot, without title/axis/legend (= 'figBg').
			scottPlot.plt.Style(dataBg: backColor); // Back color is only appied to inner part same as
			UpdatePlot(true);                       // it is only applied to inner part of monitors.
		}

		private void FitAxis()
		{
			UpdatePlot(true); // Immediately update, don't wait for update ticker.
		}

		private void Clear()
		{
			lock (this.model.AutoActionPlotModelSyncObj)
				this.model.AutoActionPlotModel.ClearAllItems();

			UpdatePlot(true); // Immediately update, don't wait for update ticker.
		}

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

			lock (this.model.AutoActionPlotModelSyncObj)
			{
				var mdl = this.model.AutoActionPlotModel;
				if ((this.lastUpdateCount != mdl.UpdateCounter) || force) // Only update when needed.
				{
					button_FitAxis.Enabled = false; // AxisAuto() will be called further below.

					var txColor = this.model.SettingsRoot.Format.TxDataFormat.Color;
					var rxColor = this.model.SettingsRoot.Format.RxDataFormat.Color;

					scottPlot.plt.Clear();

					scottPlot.plt.Title(mdl.Title);

					scottPlot.plt.XLabel(mdl.XLabel);
					scottPlot.plt.YLabel(mdl.YLabel);

					switch (mdl.Action)
					{
						case AutoAction.PlotByteCountRate:  PlotCountRate(mdl, txColor, rxColor       ); break;
						case AutoAction.PlotLineCountRate:  PlotCountRate(mdl, txColor, rxColor       ); break;
						case AutoAction.LineChartIndex:     PlotSignal(   mdl,          rxColor       ); break;
						case AutoAction.LineChartTime:      PlotScatter(  mdl,          rxColor, true ); break;
						case AutoAction.LineChartTimeStamp: PlotScatter(  mdl,          rxColor, true ); break;
						case AutoAction.ScatterPlotXY:      PlotScatter(  mdl,          rxColor, false); break;
						case AutoAction.ScatterPlotTime:    PlotScatter(  mdl,          rxColor, true ); break;
						case AutoAction.Histogram:          PlotHistogram(mdl,          rxColor       ); break;

						default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + mdl.Action.ToString() + "' is a plot type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}

					scottPlot.plt.AxisAuto();
					scottPlot.plt.Legend(enableLegend: ApplicationSettings.RoamingUserSettings.Plot.ShowLegend);

					scottPlot.Render();

					this.lastUpdateCount = mdl.UpdateCounter;

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

		private void PlotCountRate(Model.AutoActionPlotModel mdl, Color txColor, Color rxColor)
		{
			scottPlot.plt.Ticks(dateTimeX: true);

			if ((mdl.XValues != null) && (mdl.YValues != null) && (mdl.YValues.Count > 0))
			{
				for (int i = 0; i < mdl.YValues.Count; i++)
				{
					switch (i)
					{
						case 0: /* TxCount */ scottPlot.plt.PlotScatter(mdl.XValues.Item2.ToArray(), mdl.YValues[i].Item2.ToArray(), color: txColor, label: mdl.YValues[i].Item1, markerShape: ScottPlot.MarkerShape.none);                                     break;
						case 1: /* TxRate  */ scottPlot.plt.PlotScatter(mdl.XValues.Item2.ToArray(), mdl.YValues[i].Item2.ToArray(), color: txColor, label: mdl.YValues[i].Item1, markerShape: ScottPlot.MarkerShape.none, lineStyle: ScottPlot.LineStyle.Dot); break;
						case 2: /* RxCount */ scottPlot.plt.PlotScatter(mdl.XValues.Item2.ToArray(), mdl.YValues[i].Item2.ToArray(), color: rxColor, label: mdl.YValues[i].Item1, markerShape: ScottPlot.MarkerShape.none);                                     break;
						case 3: /* RxRate  */ scottPlot.plt.PlotScatter(mdl.XValues.Item2.ToArray(), mdl.YValues[i].Item2.ToArray(), color: rxColor, label: mdl.YValues[i].Item1, markerShape: ScottPlot.MarkerShape.none, lineStyle: ScottPlot.LineStyle.Dot); break;

						default:  throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "Index " + i.ToString(CultureInfo.InvariantCulture) + " is a count/rate that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
		}


		private void PlotSignal(Model.AutoActionPlotModel mdl, Color rxColor)
		{
			if ((mdl.YValues != null) && (mdl.YValues.Count > 0))
			{
				var isFirst = true;
				foreach (var kvp in mdl.YValues)
				{
					if (isFirst)
					{
						isFirst = false;
						scottPlot.plt.PlotSignal(kvp.Item2.ToArray(), color: rxColor, label: kvp.Item1);
					}
					else
					{
						scottPlot.plt.PlotSignal(kvp.Item2.ToArray(), label: kvp.Item1);
					}
				}
			}
		}

		private void PlotScatter(Model.AutoActionPlotModel mdl, Color rxColor, bool dateTimeX)
		{
			if (dateTimeX)
				scottPlot.plt.Ticks(dateTimeX: true);

			if ((mdl.XValues != null) && (mdl.YValues != null) && (mdl.YValues.Count > 0))
			{
				var isFirst = true;
				foreach (var kvp in mdl.YValues)
				{
					if (isFirst)
					{
						isFirst = false;
						scottPlot.plt.PlotScatter(mdl.XValues.Item2.ToArray(), kvp.Item2.ToArray(), color: rxColor, lineWidth: 0, label: kvp.Item1);
					}
					else
					{
						scottPlot.plt.PlotScatter(mdl.XValues.Item2.ToArray(), kvp.Item2.ToArray(), lineWidth: 0, label: kvp.Item1);
					}
				}
			}
		}

		private void PlotHistogram(Model.AutoActionPlotModel mdl, Color rxColor)
		{
			if (mdl.Histogram != null)
			{
				scottPlot.plt.PlotBar(mdl.Histogram.ValuesMidPoint.ToArray(), mdl.Histogram.Counts.Select(x => (double)x).ToArray(), barWidth: mdl.Histogram.BinSize, color: rxColor, label: "All Captures");
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

			lock (this.model.AutoActionPlotModelSyncObj)
			{
				var mdl = this.model.AutoActionPlotModel;

				switch (mdl.Action)
				{
					case AutoAction.LineChartIndex:
						UpdateHoverOnSignal(cursorPos);
						break;

					case AutoAction.PlotByteCountRate:
					case AutoAction.PlotLineCountRate:
					case AutoAction.LineChartTime:
					case AutoAction.LineChartTimeStamp:
					case AutoAction.ScatterPlotXY:
					case AutoAction.ScatterPlotTime:
						UpdateHoverOnScatter(cursorPos);
						break;

					case AutoAction.Histogram:
						UpdateHoverOnHistogram(cursorPos);
						break;

					default:
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + mdl.Action.ToString() + "' is a plot type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

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

		[Conditional("DEBUG_UPDATE")]
		private void DebugUpdate(string message)
		{
			Debug.Write(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
