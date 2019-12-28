//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/YAT/YAT.View.Forms/Main.cs $
// $Revision: 2826 $
// $Date: 2019-12-12 00:06:45 +0100 (Do., 12 Dez 2019) $
// $Author: maettu_this $
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

using YAT.Model.Types;
using YAT.Settings.Application;

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	public partial class AutoActionPlot : Form
	{
		List<double> values = new List<double>(1024); // Preset the initial capacity to improve memory management; 1024 is an arbitrary value.

		/// <summary></summary>
		public AutoActionPlot()
		{
			InitializeComponent();
		}

		/// <summary></summary>
		public void AddItem(AutoActionPlotItem pi)
		{
			switch (pi.Type)
			{
				case Model.Types.AutoActionPlot.LineChartIndex: UpdateLineChartIndex(pi); break;
				case Model.Types.AutoActionPlot.LineChartTime:  UpdateLineChartTime(pi);  break;
				case Model.Types.AutoActionPlot.ScatterPlot:    UpdateScatterPlot(pi);    break;
				case Model.Types.AutoActionPlot.Histogram:      UpdateHistogram(pi);      break;
			}
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

		private void button_Reset_Click(object sender, EventArgs e)
		{
			ClearPlot();
		}

		private void button_Close_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <remarks>Based on 'ScottPlotDemos.PENDING'.</remarks>
		private void UpdateLineChartIndex(AutoActionPlotItem pi)
		{
			scottPlot.plt.Clear();
			scottPlot.plt.Title(pi.Title);
			scottPlot.plt.XLabel(pi.XCaption);
			scottPlot.plt.YLabel(pi.YCaption);

			var casted = (pi as MultiDoubleAutoActionPlotItem);
			this.values.AddRange(casted.YValues);

			scottPlot.plt.PlotSignal(this.values.ToArray());
			scottPlot.plt.AxisAuto();

			UpdatePlot();
		}

		/// <remarks>Based on 'ScottPlotDemos.PENDING'.</remarks>
		private void UpdateLineChartTime(AutoActionPlotItem pi)
		{
			// PENDING

			UpdatePlot();
		}

		/// <remarks>Based on 'ScottPlotDemos.PENDING'.</remarks>
		private void UpdateScatterPlot(AutoActionPlotItem pi)
		{
			// PENDING

			UpdatePlot();
		}

		/// <remarks>Based on 'ScottPlotDemos.PENDING'.</remarks>
		private void UpdateHistogram(AutoActionPlotItem pi)
		{
			// PENDING

			UpdatePlot();
		}

		private void UpdatePlot()
		{
			scottPlot.plt.Legend(enableLegend: ApplicationSettings.RoamingUserSettings.Plot.ShowLegend);

			scottPlot.Render();
		}

		private void ClearPlot()
		{
			this.values.Clear();

			scottPlot.plt.Clear(false);

			UpdatePlot();
		}

		/// <remarks>Taken from 'ScottPlotDemos.FormHoverValue'.</remarks>
		private void UpdateHover()
		{
			return; // PENDING

			var plottables = scottPlot.plt.GetPlottables();

			var scatterPlot      = (ScottPlot.PlottableScatter)plottables[0];
			var highlightScatter = (ScottPlot.PlottableScatter)plottables[1];
			var highlightText    = (ScottPlot.PlottableText)   plottables[2];

			// Get cursor/mouse position:
			var cursorPos = new Point(Cursor.Position.X, Cursor.Position.Y);

			// Adjust to position on ScottPlot:
			cursorPos.X -= this.PointToScreen(scottPlot.Location).X;
			cursorPos.Y -= this.PointToScreen(scottPlot.Location).Y;

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

			scottPlot.Render();
		}
	}
}

//==================================================================================================
// End of
// $URL: svn+ssh://maettu_this@svn.code.sf.net/p/y-a-terminal/code/trunk/YAT/YAT.View.Forms/Main.cs $
//==================================================================================================
