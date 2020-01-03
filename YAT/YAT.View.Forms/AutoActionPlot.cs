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
	#define DEBUG_UPDATE

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using MKY;
using MKY.Diagnostics;
using MKY.Math;

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

		private readonly long initialAndMinimumUpdateInterval; // = 0;

		private Model.Terminal model; // = null;

		private int lastUpdateCount; // = 0;
		private bool updateIsSuspended; // = false;
		private TimedMovingAverageInt64 updateSpanAvg10s;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public AutoActionPlot(Model.Terminal model)
		{
			InitializeComponent();

			this.initialAndMinimumUpdateInterval = timer_Update.Interval; // 73 ms (a prime number)

			checkBox_ShowLegend.Checked = ApplicationSettings.RoamingUserSettings.Plot.ShowLegend;

			this.model = model;

			this.updateSpanAvg10s = new TimedMovingAverageInt64(10000);
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void timer_Update_Tick(object sender, EventArgs e)
		{
			long updateSpanAvg10s;
			UpdatePlot(out updateSpanAvg10s); // No additional synchronization is needed, the System.Windows.Forms.Timer is synchronized.
		}

		private void scottPlot_MouseEntered(object sender, EventArgs e)
		{
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

		private void checkBox_ShowLegend_CheckedChanged(object sender, EventArgs e)
		{
			ApplicationSettings.RoamingUserSettings.Plot.ShowLegend = !ApplicationSettings.RoamingUserSettings.Plot.ShowLegend;
			ApplicationSettings.SaveRoamingUserSettings();

			UpdatePlot(); // Immediately update, don't wait for update ticker.
		}

		private void button_Clear_Click(object sender, EventArgs e)
		{
			Clear();
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

		private void Clear()
		{
			lock (this.model.AutoActionPlotModelSyncObj)
				this.model.AutoActionPlotModel.ClearAllItems();

			UpdatePlot(); // Immediately update, don't wait for update ticker.
		}

		private void UpdatePlot()
		{
			long updateSpanAvg10sDummy;
			UpdatePlot(out updateSpanAvg10sDummy);
		}

		private void UpdatePlot(out long updateSpanAvg10s)
		{
			updateSpanAvg10s = 0;

			if (!this.updateIsSuspended) // Only update when allowed.
			{
				lock (this.model.AutoActionPlotModelSyncObj)
				{
					var mdl = this.model.AutoActionPlotModel;
					if (this.lastUpdateCount != mdl.UpdateCounter) // Only update when needed.
					{
						var updateBegin = Stopwatch.GetTimestamp();

						scottPlot.plt.Clear();

						scottPlot.plt.Title(mdl.Title);

						scottPlot.plt.XLabel(mdl.XLabel);
						scottPlot.plt.YLabel(mdl.YLabel);

						switch (mdl.Action)
						{
							case AutoAction.LineChartIndex: {
								if (mdl.YValues != null) {
									foreach (var kvp in mdl.YValues) {
										scottPlot.plt.PlotSignal(kvp.Item2.ToArray(), label: kvp.Item1);
									}
								}
								break;
							}

							case AutoAction.LineChartTimeStamp: {
								scottPlot.plt.Ticks(dateTimeX: true);
								if ((mdl.XValues != null) && (mdl.YValues != null)) {
									foreach (var kvp in mdl.YValues) {
										scottPlot.plt.PlotScatter(mdl.XValues.Item2.ToArray(), kvp.Item2.ToArray(), label: kvp.Item1);
									}
								}
								break;
							}

							case AutoAction.ScatterPlotXY: {
								// PENDING
							//	scottPlot.plt.PlotScatter(this.plotXValues.ToArray(), this.plotYValues.ToArray(), lineWidth: 0);
								break;
							}

							case AutoAction.ScatterPlotTime: {
								// PENDING
							//	scottPlot.plt.Ticks(dateTimeX: true);
							//	scottPlot.plt.PlotScatter(this.plotXValues.ToArray(), this.plotYValues.ToArray(), lineWidth: 0);
								break;
							}

							case AutoAction.Histogram: {
								// PENDING
							//	scottPlot.plt.PlotBar(this.plotXValues.ToArray(), this.plotYValues.ToArray(), barWidth: ToHistogramBarWidth(this.plotXValues));
								break;
							}

							default: {
								throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + mdl.Action.ToString() + "' is a plot type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}
						}

						scottPlot.plt.AxisAuto();
						scottPlot.plt.Legend(enableLegend: ApplicationSettings.RoamingUserSettings.Plot.ShowLegend);

						scottPlot.Render();

						this.lastUpdateCount = mdl.UpdateCounter;

						var updateEnd = Stopwatch.GetTimestamp();
						var updateSpan = (updateEnd - updateBegin);
						updateSpanAvg10s = this.updateSpanAvg10s.EnqueueAndCalculate(updateSpan); // No additional synchronization is needed, all access to this form is synchronized onto the main thread.
						DebugUpdate("Update took " + StopwatchEx.TicksToTime(updateSpan) + " ms; in average (moving, 10 s) " + StopwatchEx.TicksToTime(updateSpanAvg10s) + " ms");
					}
				}
			}
		}

		private double? ToHistogramBarWidth(List<double> xValues)
		{
			if ((xValues != null) && (xValues.Count > 1))
				return (xValues[1] - xValues[0]);
			else
				return (null);
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

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG")]
		private void DebugMessage(string message)
		{
			Debug.WriteLine(message);
		}

		[Conditional("DEBUG_UPDATE")]
		private void DebugUpdate(string message)
		{
			DebugMessage(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
