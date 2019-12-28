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
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using MKY;

using YAT.Model.Types;
using YAT.Settings.Application;

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class AutoActionPlot : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool plotUpdateIsSuspended; // = false;

		private Model.Types.AutoActionPlot plotType;
		private string plotTitle;
		private string plotXLabel;
		private string plotYLabel;
		private List<double> plotXValues = new List<double>(1024); // Preset the initial capacity to improve memory management; 1024 is an arbitrary value.
		private List<double> plotYValues = new List<double>(1024); // Preset the initial capacity to improve memory management; 1024 is an arbitrary value.
		private object plotSyncObj = new object();

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public AutoActionPlot()
		{
			InitializeComponent();
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public void AddItem(AutoActionPlotItem pi)
		{
			switch (pi.Type)
			{
				case Model.Types.AutoActionPlot.LineChartIndex:  AddItemToLineChartIndex(pi);  break;
				case Model.Types.AutoActionPlot.LineChartTime:   AddItemToLineChartTime(pi);   break;
				case Model.Types.AutoActionPlot.ScatterPlotXY:   AddItemToScatterPlotXY(pi);   break;
				case Model.Types.AutoActionPlot.ScatterPlotTime: AddItemToScatterPlotTime(pi); break;
				case Model.Types.AutoActionPlot.Histogram:       AddItemToHistogram(pi);       break;
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void timer_PlotUpdate_Tick(object sender, EventArgs e)
		{
			if (!this.plotUpdateIsSuspended)
				UpdatePlot();
		}

		private void scottPlot_MouseMoved(object sender, EventArgs e)
		{
			UpdateHover();
		}

		private void checkBox_ShowLegend_CheckedChanged(object sender, EventArgs e)
		{
			ApplicationSettings.RoamingUserSettings.Plot.ShowLegend = !ApplicationSettings.RoamingUserSettings.Plot.ShowLegend;
			ApplicationSettings.SaveRoamingUserSettings();

			UpdatePlot();
		}

		private void checkBox_ShowHover_CheckedChanged(object sender, EventArgs e)
		{
			ApplicationSettings.RoamingUserSettings.Plot.ShowHover = !ApplicationSettings.RoamingUserSettings.Plot.ShowHover;
			ApplicationSettings.SaveRoamingUserSettings();

			UpdateHover();
		}

		private void button_Pause_Click(object sender, EventArgs e)
		{
			this.plotUpdateIsSuspended = true;

			button_Pause   .Visible = false;
			button_Continue.Visible = true;
		}

		private void button_Continue_Click(object sender, EventArgs e)
		{
			this.plotUpdateIsSuspended = false;

			button_Continue.Visible = false;
			button_Pause   .Visible = true;
		}

		private void button_Clear_Click(object sender, EventArgs e)
		{
			ClearValues();
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

		private void AddItemToLineChartIndex(AutoActionPlotItem pi)
		{
			lock (this.plotSyncObj)
			{
				this.plotType = pi.Type;

				this.plotTitle = pi.Title;

				this.plotXLabel = pi.XCaption;
				this.plotYLabel = pi.YCaption;

				this.plotYValues.AddRange(pi.YValues); // PENDING: Chabis, multiple signals !!!
			}
		}

		private void AddItemToLineChartTime(AutoActionPlotItem pi)
		{
			lock (this.plotSyncObj)
			{
				this.plotType = pi.Type;

				this.plotTitle = pi.Title;

				this.plotXLabel = pi.XCaption;
				this.plotYLabel = pi.YCaption;

				this.plotXValues.AddRange(pi.XValues);
				this.plotYValues.AddRange(pi.YValues);
			}
		}

		private void AddItemToScatterPlotXY(AutoActionPlotItem pi)
		{
			lock (this.plotSyncObj)
			{
				this.plotType = pi.Type;

				this.plotTitle = pi.Title;

				this.plotXLabel = pi.XCaption;
				this.plotYLabel = pi.YCaption;

				this.plotXValues.AddRange(pi.XValues);
				this.plotYValues.AddRange(pi.YValues);
			}
		}

		private void AddItemToScatterPlotTime(AutoActionPlotItem pi)
		{
			lock (this.plotSyncObj)
			{
				this.plotType = pi.Type;

				this.plotTitle = pi.Title;

				this.plotXLabel = pi.XCaption;
				this.plotYLabel = pi.YCaption;

				this.plotXValues.AddRange(pi.XValues);
				this.plotYValues.AddRange(pi.YValues);
			}
		}

		private void AddItemToHistogram(AutoActionPlotItem pi)
		{
			lock (this.plotSyncObj)
			{
				this.plotType = pi.Type;

				this.plotTitle = pi.Title;

				this.plotXLabel = pi.XCaption;
				this.plotYLabel = pi.YCaption;

				this.plotXValues.AddRange(pi.XValues);
				this.plotYValues.AddRange(pi.YValues);

				// PENDING Make histo bins and counts
			}
		}

		private void ClearValues()
		{
			lock (this.plotSyncObj)
			{
				this.plotXValues.Clear();
				this.plotYValues.Clear();
			}
		}

		private void UpdatePlot()
		{
			scottPlot.plt.Clear();

			lock (this.plotSyncObj)
			{
				scottPlot.plt.Title(this.plotTitle);
				scottPlot.plt.XLabel(this.plotXLabel);
				scottPlot.plt.YLabel(this.plotYLabel);

				switch (this.plotType)
				{
					case Model.Types.AutoActionPlot.LineChartIndex:
						scottPlot.plt.PlotSignal(this.plotYValues.ToArray());
						break;

					case Model.Types.AutoActionPlot.LineChartTime:
						scottPlot.plt.Ticks(dateTimeX: true);
						scottPlot.plt.PlotScatter(this.plotXValues.ToArray(), this.plotYValues.ToArray());
						break;

					case Model.Types.AutoActionPlot.ScatterPlotXY:
						scottPlot.plt.PlotScatter(this.plotXValues.ToArray(), this.plotYValues.ToArray(), lineWidth: 0);
						break;

					case Model.Types.AutoActionPlot.ScatterPlotTime:
						scottPlot.plt.Ticks(dateTimeX: true);
						scottPlot.plt.PlotScatter(this.plotXValues.ToArray(), this.plotYValues.ToArray(), lineWidth: 0);
						break;

					case Model.Types.AutoActionPlot.Histogram:
						scottPlot.plt.PlotBar(this.plotXValues.ToArray(), this.plotYValues.ToArray(), barWidth: ToHistogramBarWidth(this.plotXValues));
						break;

					default:
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + this.plotType.ToString() + "' is a plot type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			scottPlot.plt.AxisAuto();
			scottPlot.plt.Legend(enableLegend: ApplicationSettings.RoamingUserSettings.Plot.ShowLegend);

			scottPlot.Render();
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

			switch (this.plotType)
			{
				case Model.Types.AutoActionPlot.LineChartIndex:
					UpdateHoverOnSignal(cursorPos);
					break;

				case Model.Types.AutoActionPlot.LineChartTime:
				case Model.Types.AutoActionPlot.ScatterPlotXY:
				case Model.Types.AutoActionPlot.ScatterPlotTime:
					UpdateHoverOnScatter(cursorPos);
					break;

				case Model.Types.AutoActionPlot.Histogram:
					UpdateHoverOnHistogram(cursorPos);
					break;

				default:
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + this.plotType.ToString() + "' is a plot type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			scottPlot.Render();
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

			if ((ApplicationSettings.RoamingUserSettings.Plot.ShowHover) && (closestDistance < 20))
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
