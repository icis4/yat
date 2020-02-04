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
#if USE_SCOTT_PLOT
using System.Collections.Generic;
#endif
using System.Drawing;
#if USE_OXY_PLOT
using System.Globalization;
using System.Linq;
#endif
using System.Windows.Forms;

#if USE_OXY_PLOT
using MKY;
#endif
using MKY.Collections.Specialized;

using YAT.Model.Types;

#endregion

namespace YAT.Model
{
	/// <summary>
	/// Defines the plot model.
	/// </summary>
	/// <remarks>
	/// Separated from plot view for...
	/// <list type="bullet">
	/// <item><description>...better decoupling updating the model (threadpool thread) and updating the view (main thread).</description></item>
	/// <item><description>...better decoupling model and view implementation (MVVM).</description></item>
	/// </list>
	/// </remarks>
	public class AutoActionPlotModel
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <summary></summary>
		public AutoAction Action { get; protected set; }

	#if USE_SCOTT_PLOT

		/// <summary></summary>
		public string Title { get; protected set; }

		/// <summary></summary>
		public string XLabel { get; protected set; }

		/// <summary></summary>
		public string YLabel { get; protected set; }

		/// <summary></summary>
		public Tuple<string, List<double>> XValues { get; protected set; }

		/// <summary></summary>
		public List<Tuple<string, List<double>>> YValues { get; protected set; }

	#endif

		/// <summary></summary>
		public HistogramDouble Histogram { get; protected set; }

	#if USE_SCOTT_PLOT

		/// <summary>
		/// Gets or sets a counter that indicates whether and how many times the plot has changed.
		/// </summary>
		public int UpdateCounter { get; protected set; } // = 0;

	#elif USE_OXY_PLOT

		/// <summary></summary>
		public OxyPlot.PlotModel OxyModel { get; protected set; }

	#endif

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

	#if USE_OXY_PLOT
		/// <summary></summary>
		public AutoActionPlotModel()
		{
			OxyModel = new OxyPlot.PlotModel();
		}
	#endif

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public void AddItem(AutoActionPlotItem pi, Color txColor, Color rxColor)
		{
			if (Action != pi.Action) {
				Action = pi.Action;

	#if USE_SCOTT_PLOT
				Clear(); // Ensure that X/Y or histogram tuples are consistent.
	#elif USE_OXY_PLOT
				Reset(); // Ensure that X/Y or histogram tuples are consistent.
	#endif
			}

	#if USE_SCOTT_PLOT
			Title = (AutoActionEx)pi.Action;
	#elif USE_OXY_PLOT
			OxyModel.Title = (AutoActionEx)pi.Action;
	#endif

			switch (pi.Action)
			{
				case AutoAction.PlotByteCountRate:   AddItemToPlotCountRate(     pi, txColor, rxColor, "bytes"               ); break;
				case AutoAction.PlotLineCountRate:   AddItemToPlotCountRate(     pi, txColor, rxColor, "lines"               ); break;
				case AutoAction.LineChartIndex:      AddItemToLineChartIndex(    pi,          rxColor                        ); break;
				case AutoAction.LineChartTime:       AddItemToLineChartTime(     pi,          rxColor                        ); break;
				case AutoAction.LineChartTimeStamp:  AddItemToLineChartTimeStamp(pi,          rxColor                        ); break;
				case AutoAction.ScatterPlot:         AddItemToScatterPlot(       pi,          rxColor                        ); break;
				case AutoAction.HistogramHorizontal: AddItemToHistogram(         pi,          rxColor, Orientation.Horizontal); break;
				case AutoAction.HistogramVertical:   AddItemToHistogram(         pi,          rxColor, Orientation.Vertical  ); break;
			}

	#if USE_SCOTT_PLOT
			IndicateUpdate();
	#elif USE_OXY_PLOT
			Invalidate(true); // Items have changed.
	#endif
		}

		/// <summary></summary>
		public virtual void Clear()
		{
			Histogram = null;
	#if USE_SCOTT_PLOT
			XValues = null;
			YValues = null;
	#elif USE_OXY_PLOT
			OxyModel.Series.Clear();
	#endif

	#if USE_SCOTT_PLOT
			IndicateUpdate();
	#elif USE_OXY_PLOT
			Invalidate(true); // Items have changed.
	#endif
		}

	#if USE_OXY_PLOT
		/// <summary></summary>
		public virtual void Reset()
		{
			Clear();

			OxyModel.Axes.Clear();

			Invalidate(false); // Items have not changed.
		}
	#endif

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		/// <summary></summary>
		protected virtual void AddItemToPlotCountRate(AutoActionPlotItem pi, Color txColor, Color rxColor, string content)
		{
	#if USE_SCOTT_PLOT
			XLabel = "Time Stamp";
			YLabel = "Value";

			AddItemToXAndY(pi);
	#elif USE_OXY_PLOT
			if (OxyModel.Axes.Count == 0)
			{
				OxyModel.Axes.Add(new OxyPlot.Axes.DateTimeAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Time Stamp",                                  AbsoluteMinimum = DateTime.Now.ToOADate() });
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis   { Position = OxyPlot.Axes.AxisPosition.Left,   Title = "Count", Unit = content,        Key = "Count", AbsoluteMinimum = 0.0 });
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis   { Position = OxyPlot.Axes.AxisPosition.Right,  Title = "Rate",  Unit = content + "/s", Key = "Rate",  AbsoluteMinimum = 0.0 });
			}

			for (int i = OxyModel.Series.Count; i < pi.YValues.Length; i++)
			{
				switch (i)
				{
					case 0: /* TxCount */ OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = "Tx Count [" + content + "]",       YAxisKey = "Count", Color = OxyPlotEx.ConvertTo(txColor)                                     }); break;
					case 1: /* TxRate  */ OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = "Tx Rate [" + content + "/s" + "]", YAxisKey = "Rate",  Color = OxyPlotEx.ConvertTo(txColor), LineStyle = OxyPlot.LineStyle.Dash }); break;
					case 2: /* RxCount */ OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = "Rx Count [" + content + "]",       YAxisKey = "Count", Color = OxyPlotEx.ConvertTo(rxColor)                                     }); break;
					case 3: /* RxRate  */ OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = "Rx Rate [" + content + "/s" + "]", YAxisKey = "Rate",  Color = OxyPlotEx.ConvertTo(rxColor), LineStyle = OxyPlot.LineStyle.Dash }); break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "Index " + i.ToString(CultureInfo.InvariantCulture) + " is a count/rate that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			// Directly adding data point is the best performing way to add items according to https://oxyplot.readthedocs.io/en/latest/guidelines/performance.html.
			for (int i = 0; i < pi.YValues.Length; i++)
			{
				var series = OxyModel.Series[i];
				var points = ((OxyPlot.Series.LineSeries)series).Points;
				                                                   //// Skip non-isochronuos samples. Such may happen because rate updates are enqueued fro two different locations in the terminal.
				if ((points.Count > 0) && (points[points.Count-1].X <= pi.XValue.Item2))
					points.Add(new OxyPlot.DataPoint(pi.XValue.Item2, pi.YValues[i].Item2));
			}
	#endif
		}

		/// <summary></summary>
		protected virtual void AddItemToLineChartIndex(AutoActionPlotItem pi, Color firstColor)
		{
	#if USE_SCOTT_PLOT
			XLabel = "Index";
			YLabel = "Value";

			AddItemToYOnly(pi);
	#elif USE_OXY_PLOT
			if (OxyModel.Axes.Count == 0)
			{
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Index", AbsoluteMinimum = 0.0 });
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left,   Title = "Value" });
			}

			for (int i = OxyModel.Series.Count; i < pi.YValues.Length; i++)
			{
				if (i == 0)
					OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Cross, Color = OxyPlotEx.ConvertTo(firstColor) });
				else
					OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Cross });
			}

			var x = 0;
			foreach (var series in OxyModel.Series)
			{
				var points = ((OxyPlot.Series.LineSeries)series).Points;
				if (x < (points.Count - 1))
					x = (points.Count - 1);
			}

			// Directly adding data point is the best performing way to add items according to https://oxyplot.readthedocs.io/en/latest/guidelines/performance.html.
			for (int i = 0; i < pi.YValues.Length; i++)
			{
				var series = OxyModel.Series[i];
				var points = ((OxyPlot.Series.LineSeries)series).Points;
				points.Add(new OxyPlot.DataPoint(x, pi.YValues[i].Item2));
			}
	#endif
		}

		/// <summary></summary>
		protected virtual void AddItemToLineChartTime(AutoActionPlotItem pi, Color firstColor)
		{
	#if USE_SCOTT_PLOT
			XLabel = "Time";
			YLabel = "Value";

			AddItemToXAndY(pi);
	#elif USE_OXY_PLOT
			if (OxyModel.Axes.Count == 0)
			{
				OxyModel.Axes.Add(new OxyPlot.Axes.DateTimeAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Time"  });
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis   { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Value" });
			}

			for (int i = OxyModel.Series.Count; i < pi.YValues.Length; i++)
			{
				if (i == 0)
					OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Cross, Color = OxyPlotEx.ConvertTo(firstColor) });
				else
					OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Cross });
			}

			// Directly adding data point is the best performing way to add items according to https://oxyplot.readthedocs.io/en/latest/guidelines/performance.html.
			for (int i = 0; i < pi.YValues.Length; i++)
			{
				var series = OxyModel.Series[i];
				var points = ((OxyPlot.Series.LineSeries)series).Points;
				points.Add(new OxyPlot.DataPoint(pi.XValue.Item2, pi.YValues[i].Item2));
			}
	#endif
		}

		/// <summary></summary>
		protected virtual void AddItemToLineChartTimeStamp(AutoActionPlotItem pi, Color firstColor)
		{
	#if USE_SCOTT_PLOT
			XLabel = "Time Stamp";
			YLabel = "Value";

			AddItemToXAndY(pi);
	#elif USE_OXY_PLOT
			if (OxyModel.Axes.Count == 0)
			{
				OxyModel.Axes.Add(new OxyPlot.Axes.DateTimeAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Time Stamp", AbsoluteMinimum = DateTime.Now.ToOADate() });
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis   { Position = OxyPlot.Axes.AxisPosition.Left,   Title = "Value" });
			}

			for (int i = OxyModel.Series.Count; i < pi.YValues.Length; i++)
			{
				if (i == 0)
					OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Cross, Color = OxyPlotEx.ConvertTo(firstColor) });
				else
					OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Cross });
			}

			// Directly adding data point is the best performing way to add items according to https://oxyplot.readthedocs.io/en/latest/guidelines/performance.html.
			for (int i = 0; i < pi.YValues.Length; i++)
			{
				var series = OxyModel.Series[i];
				var points = ((OxyPlot.Series.LineSeries)series).Points;
				points.Add(new OxyPlot.DataPoint(pi.XValue.Item2, pi.YValues[i].Item2));
			}
	#endif
		}

		/// <summary></summary>
		protected virtual void AddItemToScatterPlot(AutoActionPlotItem pi, Color firstColor)
		{
	#if USE_SCOTT_PLOT
			XLabel = "X Value";
			YLabel = "Y Value";

			AddItemToXAndY(pi);
	#elif USE_OXY_PLOT
			if (OxyModel.Axes.Count == 0)
			{
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis { PositionAtZeroCrossing = true, AxislineStyle = OxyPlot.LineStyle.Solid, TickStyle = OxyPlot.Axes.TickStyle.Crossing, Title = "X Value" });
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis { PositionAtZeroCrossing = true, AxislineStyle = OxyPlot.LineStyle.Solid, TickStyle = OxyPlot.Axes.TickStyle.Crossing, Title = "Y Value" });
			}

			for (int i = OxyModel.Series.Count; i < pi.YValues.Length; i++)
			{
				if (i == 0)
					OxyModel.Series.Add(new OxyPlot.Series.ScatterSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Square, MarkerFill = OxyPlotEx.ConvertTo(firstColor) });
				else
					OxyModel.Series.Add(new OxyPlot.Series.ScatterSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Square });
			}

			// Directly adding data point is the best performing way to add items according to https://oxyplot.readthedocs.io/en/latest/guidelines/performance.html.
			for (int i = 0; i < pi.YValues.Length; i++)
			{
				var series = OxyModel.Series[i];
				var points = ((OxyPlot.Series.ScatterSeries)series).Points;
				points.Add(new OxyPlot.Series.ScatterPoint(pi.XValue.Item2, pi.YValues[i].Item2));
			}
	#endif
		}

	#if USE_SCOTT_PLOT

		/// <summary></summary>
		protected virtual void AddItemToYOnly(AutoActionPlotItem pi)
		{
			if (YValues == null)
			{
				YValues = new List<Tuple<string, List<double>>>(pi.YValues.Length); // Preset the required capacity to improve memory management.
			}

			AddItemToY(pi);
		}

		/// <summary></summary>
		protected virtual void AddItemToXAndY(AutoActionPlotItem pi)
		{
			if ((XValues == null) || (YValues == null))
			{
				XValues = new Tuple<string, List<double>>(pi.XValue.Item1, new List<double>(1024)); // Add a new empty list.
				YValues = new List<Tuple<string, List<double>>>(pi.YValues.Length); // Preset the required capacity to improve memory management.
			}

			XValues.Item2.Add(pi.XValue.Item2);

			AddItemToY(pi);
		}

		/// <summary></summary>
		protected virtual void AddItemToY(AutoActionPlotItem pi)
		{
			for (int i = YValues.Count; i < pi.YValues.Length; i++)
			{
				string label = pi.YValues[i].Item1;

				List<double> values;
				if ((i == 0) || (YValues[0].Item2.Count == 0))
					values = new List<double>(1024); // Add a new empty list.
				else
					values = new List<double>(new double[YValues[0].Item2.Count]); // Add a new list filled with default values.

				YValues.Add(new Tuple<string, List<double>>(label, values));
			}

			for (int i = 0; i < YValues.Count; i++)
			{
				if (i < pi.YValues.Length)
					YValues[i].Item2.Add(pi.YValues[i].Item2);
				else
					YValues[i].Item2.Add(0); // Fill with default value.
			}
		}

	#endif

		/// <summary></summary>
		protected virtual void AddItemToHistogram(AutoActionPlotItem pi, Color color, Orientation orientation)
		{
	#if USE_SCOTT_PLOT
			XLabel = "Bins";
			YLabel = "Counts";
	#elif USE_OXY_PLOT
			if (OxyModel.Axes.Count == 0)
			{
				if (orientation == Orientation.Horizontal)
				{
					OxyModel.Axes.Add(new OxyPlot.Axes.CategoryAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Bins", GapWidth = 0.1, StringFormat = "G3" }); // Not using 'SuperExponentialFormat' for two reasons:
					OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis   { Position = OxyPlot.Axes.AxisPosition.Left,   Title = "Counts", AbsoluteMinimum = 0.0 });             // 1. Readability (always that format) 2. Performance (time consuming)
				}
				else
				{
					OxyModel.Axes.Add(new OxyPlot.Axes.CategoryAxis { Position = OxyPlot.Axes.AxisPosition.Left,   Title = "Bins", GapWidth = 0.1, StringFormat = "G3" }); // Not using 'SuperExponentialFormat' for two reasons:
					OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis   { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Counts", AbsoluteMinimum = 0.0 });             // 1. Readability (always that format) 2. Performance (time consuming)
				}
			}

			if (OxyModel.Series.Count == 0)
			{
				if (orientation == Orientation.Horizontal)
				{
					OxyModel.Series.Add(new OxyPlot.Series.ColumnSeries { Title = pi.YValues[0].Item1, FillColor = OxyPlotEx.ConvertTo(color) });
				}
				else
				{
					OxyModel.Series.Add(new OxyPlot.Series.BarSeries { Title = pi.YValues[0].Item1, FillColor = OxyPlotEx.ConvertTo(color) });
				}
			}
	#endif
			if (Histogram == null)
				Histogram = new HistogramDouble(256);

			foreach (var tuple in pi.YValues)
				Histogram.Add(tuple.Item2);

	#if USE_OXY_PLOT
			// While directly adding data point is the best performing way to add items according to https://oxyplot.readthedocs.io/en/latest/guidelines/performance.html,
			// this seems an overkill for the histogram, as it would have to notify about added as well as inserted bins and... Too complicated, simply recreate:

			var axis = (OxyPlot.Axes.CategoryAxis)(OxyModel.Axes[0]);
			var labels = axis.Labels;
			labels.Clear();
			labels.AddRange(Histogram.ValuesMidPoint.Select(x => FormatAxisValueUnderride(x, axis, OxyModel.ActualCulture))); // See method's remarks.

			var series = OxyModel.Series[0];
			int i = 0;
			if (orientation == Orientation.Horizontal)
			{
				var items = ((OxyPlot.Series.ColumnSeries)series).Items;
				items.Clear();
				items.AddRange(Histogram.Counts.Select(x => new OxyPlot.Series.ColumnItem(x, i++)));
			}
			else
			{
				var items = ((OxyPlot.Series.BarSeries)series).Items;
				items.Clear();
				items.AddRange(Histogram.Counts.Select(x => new OxyPlot.Series.BarItem(x, i++)));
			}
	#endif
		}

	#if USE_OXY_PLOT
		/// <remarks>
		/// This method partly replicates <see cref="OxyPlot.Axes.Axis.FormatValueOverride(double)"/>
		/// because it is not available on an <see cref="OxyPlot.Axes.CategoryAxis"/>.
		/// </remarks>
		protected static string FormatAxisValueUnderride(double value, OxyPlot.Axes.Axis axis, CultureInfo actualCulture)
		{
			string format = string.Concat("{0:", axis.ActualStringFormat ?? axis.StringFormat ?? string.Empty, "}");
			return (string.Format(actualCulture, format, value));
		}
	#endif

	#if USE_SCOTT_PLOT
		/// <summary></summary>
		protected virtual void IndicateUpdate()
		{
			unchecked // Loop-around is OK.
			{
				UpdateCounter++;
			}
		}
	#elif USE_OXY_PLOT
		/// <summary></summary>
		protected virtual void Invalidate(bool updateData)
		{
			OxyModel.InvalidatePlot(updateData);
		}
	#endif

		#endregion
	}

	#region Extension Methods
	//==============================================================================================
	// Extension Methods
	//==============================================================================================

	#if USE_OXY_PLOT
	/// <summary></summary>
	public static class OxyPlotEx
	{
		/// <summary></summary>
		public static OxyPlot.OxyColor ConvertTo(Color color)
		{
			return (OxyPlot.OxyColor.FromArgb(color.A, color.R, color.G, color.B));
		}

	}
	#endif
	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
