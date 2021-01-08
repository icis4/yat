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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using MKY;
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
	/// <item><description>...better decoupling updating the model (thread pool thread) and updating the view (main thread).</description></item>
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

		/// <summary></summary>
		public HistogramDouble Histogram { get; protected set; }

		/// <summary></summary>
		public bool IsActive { get; protected set; } = true;

		/// <summary></summary>
		[CLSCompliant(false)]
		public OxyPlot.PlotModel OxyModel { get; protected set; }

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public AutoActionPlotModel()
		{
			OxyModel = new OxyPlot.PlotModel();
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Suspend()
		{
			IsActive = false;
		}

		/// <summary></summary>
		public virtual void Resume()
		{
			IsActive = true;
		}

		/// <summary></summary>
		public void AddItem(AutoActionPlotItem pi, Color txColor, Color rxColor)
		{
			if (!IsActive)
				return;

			if (Action != pi.Action) {
				Action = pi.Action;

				Reset(); // Ensure that X/Y or histogram tuples are consistent.
			}

			OxyModel.Title = (AutoActionEx)pi.Action;

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

			Invalidate(true); // Items have changed.
		}

		/// <summary></summary>
		public virtual void Clear()
		{
			Histogram = null;

			OxyModel.Series.Clear();

			Invalidate(true); // Items have changed.
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			Clear();

			OxyModel.Axes.Clear();

			Invalidate(false); // Items have not changed.
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		/// <summary></summary>
		protected virtual void AddItemToPlotCountRate(AutoActionPlotItem pi, Color txColor, Color rxColor, string content)
		{
			if (OxyModel.Axes.Count == 0)
			{
				OxyModel.Axes.Add(new OxyPlot.Axes.DateTimeAxis { MajorGridlineStyle = OxyPlot.LineStyle.Solid,                           Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Time Stamp",                                  AbsoluteMinimum = DateTime.Now.ToOADate() });
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis   { MajorGridlineStyle = OxyPlot.LineStyle.Solid,                           Position = OxyPlot.Axes.AxisPosition.Left,   Title = "Count", Unit = content,        Key = "Count", AbsoluteMinimum = 0.0 });
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis   { MajorGridlineStyle = OxyPlot.LineStyle.Dot, MajorGridlineThickness = 1, Position = OxyPlot.Axes.AxisPosition.Right,  Title = "Rate",  Unit = content + "/s", Key = "Rate",  AbsoluteMinimum = 0.0 });
			}

			for (int i = OxyModel.Series.Count; i < pi.YValues.Length; i++)
			{
				switch (i)
				{
					case 0: /* TxCount */ OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = "Tx Count [" + content + "]",       YAxisKey = "Count", Color = OxyPlotEx.ConvertTo(txColor)                                                         }); break;
					case 1: /* TxRate  */ OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = "Tx Rate [" + content + "/s" + "]", YAxisKey = "Rate",  Color = OxyPlotEx.ConvertTo(txColor), LineStyle = OxyPlot.LineStyle.Dot, StrokeThickness = 1 }); break;
					case 2: /* RxCount */ OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = "Rx Count [" + content + "]",       YAxisKey = "Count", Color = OxyPlotEx.ConvertTo(rxColor)                                                         }); break;
					case 3: /* RxRate  */ OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = "Rx Rate [" + content + "/s" + "]", YAxisKey = "Rate",  Color = OxyPlotEx.ConvertTo(rxColor), LineStyle = OxyPlot.LineStyle.Dot, StrokeThickness = 1 }); break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "Index " + i.ToString(CultureInfo.InvariantCulture) + " is a count/rate that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			// Directly adding data point is the best performing way to add items according
			// to https://oxyplot.readthedocs.io/en/latest/guidelines/performance.html:
			var x = pi.XValue.Item2;
			for (int i = 0; i < pi.YValues.Length; i++)
			{
				var y = pi.YValues[i].Item2;

				var series = OxyModel.Series[i];
				var points = ((OxyPlot.Series.LineSeries)series).Points;

				if ((points.Count > 0) && (points[points.Count - 1].X.Equals(0.0)))   // Add an addition 0 to properly indicate inactivity.
					points.Add(new OxyPlot.DataPoint(x, 0.0));                        // Applies to rate when inactive and to count on reset.

				if ((points.Count > 0) && (y.Equals(0.0)))                            // Add an addition value to properly indicate count.
					points.Add(new OxyPlot.DataPoint(x, points[points.Count - 1].Y)); // Applies to count on reset.
				                                                                           //// Skip non-isochronuos samples. Such may happen because rate updates are enqueued fro two different locations in the terminal.
				if ((points.Count == 0) || ((points.Count > 0) && (points[points.Count - 1].X <= x)))
					points.Add(new OxyPlot.DataPoint(x, y));
			}
		}

		/// <summary></summary>
		protected virtual void AddItemToLineChartIndex(AutoActionPlotItem pi, Color firstColor)
		{
			if (OxyModel.Axes.Count == 0)
			{
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis { MajorGridlineStyle = OxyPlot.LineStyle.Solid, Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Index", AbsoluteMinimum = 0.0 });
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis { MajorGridlineStyle = OxyPlot.LineStyle.Solid, Position = OxyPlot.Axes.AxisPosition.Left,   Title = "Value" });
			}

			for (int i = OxyModel.Series.Count; i < pi.YValues.Length; i++)
			{
				if (i == 0)
					OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Square, MarkerSize = 3, MarkerFill = OxyPlotEx.ConvertTo(firstColor), Color = OxyPlotEx.ConvertTo(firstColor) });
				else
					OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Square, MarkerSize = 3 });
			}

			var x = 0;
			foreach (var series in OxyModel.Series)
			{
				var points = ((OxyPlot.Series.LineSeries)series).Points;
				if (x < (points.Count - 1))
					x = (points.Count - 1);
			}

			int pointsTotalCount = 0;

			// Directly adding data point is the best performing way to add items according
			// to https://oxyplot.readthedocs.io/en/latest/guidelines/performance.html:
			for (int i = 0; i < pi.YValues.Length; i++)
			{
				var series = OxyModel.Series[i];
				var points = ((OxyPlot.Series.LineSeries)series).Points;
				points.Add(new OxyPlot.DataPoint(x, pi.YValues[i].Item2));

				pointsTotalCount += points.Count;
			}

			if (pointsTotalCount > 640) // Arbitrary number, based on FullHD Width / MarkerSize = 1920 / 3 = 640.
			{                           // Improve readability and performance on large number of points:
				foreach (var series in OxyModel.Series)
					((OxyPlot.Series.LineSeries)series).MarkerType = OxyPlot.MarkerType.None;
			}
		}

		/// <summary></summary>
		protected virtual void AddItemToLineChartTime(AutoActionPlotItem pi, Color firstColor)
		{
			if (OxyModel.Axes.Count == 0)
			{
				OxyModel.Axes.Add(new OxyPlot.Axes.DateTimeAxis { MajorGridlineStyle = OxyPlot.LineStyle.Solid, Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Time"  });
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis   { MajorGridlineStyle = OxyPlot.LineStyle.Solid, Position = OxyPlot.Axes.AxisPosition.Left,   Title = "Value" });
			}

			for (int i = OxyModel.Series.Count; i < pi.YValues.Length; i++)
			{
				if (i == 0)
					OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Square, MarkerSize = 3, MarkerFill = OxyPlotEx.ConvertTo(firstColor), Color = OxyPlotEx.ConvertTo(firstColor) });
				else
					OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Square, MarkerSize = 3 });
			}

			int pointsTotalCount = 0;

			// Directly adding data point is the best performing way to add items according
			// to https://oxyplot.readthedocs.io/en/latest/guidelines/performance.html:
			for (int i = 0; i < pi.YValues.Length; i++)
			{
				var series = OxyModel.Series[i];
				var points = ((OxyPlot.Series.LineSeries)series).Points;
				points.Add(new OxyPlot.DataPoint(pi.XValue.Item2, pi.YValues[i].Item2));

				pointsTotalCount += points.Count;
			}

			if (pointsTotalCount > 640) // Arbitrary number, based on FullHD Width / MarkerSize = 1920 / 3 = 640.
			{                           // Improve readability and performance on large number of points:
				foreach (var series in OxyModel.Series)
					((OxyPlot.Series.LineSeries)series).MarkerType = OxyPlot.MarkerType.None;
			}
		}

		/// <summary></summary>
		protected virtual void AddItemToLineChartTimeStamp(AutoActionPlotItem pi, Color firstColor)
		{
			if (OxyModel.Axes.Count == 0)
			{
				OxyModel.Axes.Add(new OxyPlot.Axes.DateTimeAxis { MajorGridlineStyle = OxyPlot.LineStyle.Solid, Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Time Stamp", AbsoluteMinimum = DateTime.Now.ToOADate() });
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis   { MajorGridlineStyle = OxyPlot.LineStyle.Solid, Position = OxyPlot.Axes.AxisPosition.Left,   Title = "Value" });
			}

			for (int i = OxyModel.Series.Count; i < pi.YValues.Length; i++)
			{
				if (i == 0)
					OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Square, MarkerSize = 3, MarkerFill = OxyPlotEx.ConvertTo(firstColor), Color = OxyPlotEx.ConvertTo(firstColor) });
				else
					OxyModel.Series.Add(new OxyPlot.Series.LineSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Square, MarkerSize = 3 });
			}

			int pointsTotalCount = 0;

			// Directly adding data point is the best performing way to add items according
			// to https://oxyplot.readthedocs.io/en/latest/guidelines/performance.html:
			for (int i = 0; i < pi.YValues.Length; i++)
			{
				var series = OxyModel.Series[i];
				var points = ((OxyPlot.Series.LineSeries)series).Points;
				points.Add(new OxyPlot.DataPoint(pi.XValue.Item2, pi.YValues[i].Item2));

				pointsTotalCount += points.Count;
			}

			if (pointsTotalCount > 640) // Arbitrary number, based on FullHD Width / MarkerSize = 1920 / 3 = 640.
			{                           // Improve readability and performance on large number of points:
				foreach (var series in OxyModel.Series)
					((OxyPlot.Series.LineSeries)series).MarkerType = OxyPlot.MarkerType.None;
			}
		}

		/// <summary></summary>
		protected virtual void AddItemToScatterPlot(AutoActionPlotItem pi, Color firstColor)
		{
			if (OxyModel.Axes.Count == 0)
			{
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis { MajorGridlineStyle = OxyPlot.LineStyle.Solid, Position = OxyPlot.Axes.AxisPosition.Bottom, PositionAtZeroCrossing = true, AxislineStyle = OxyPlot.LineStyle.Solid, TickStyle = OxyPlot.Axes.TickStyle.Crossing, Title = "X Value" });
				OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis { MajorGridlineStyle = OxyPlot.LineStyle.Solid, Position = OxyPlot.Axes.AxisPosition.Left,   PositionAtZeroCrossing = true, AxislineStyle = OxyPlot.LineStyle.Solid, TickStyle = OxyPlot.Axes.TickStyle.Crossing, Title = "Y Value" });
			}

			for (int i = OxyModel.Series.Count; i < pi.YValues.Length; i++)
			{
				if (i == 0)
					OxyModel.Series.Add(new OxyPlot.Series.ScatterSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Square, MarkerSize = 3, MarkerFill = OxyPlotEx.ConvertTo(firstColor) });
				else
					OxyModel.Series.Add(new OxyPlot.Series.ScatterSeries { Title = pi.YValues[i].Item1, MarkerType = OxyPlot.MarkerType.Square, MarkerSize = 3 });
			}

			int pointsTotalCount = 0;

			// Directly adding data point is the best performing way to add items according
			// to https://oxyplot.readthedocs.io/en/latest/guidelines/performance.html:
			for (int i = 0; i < pi.YValues.Length; i++)
			{
				var series = OxyModel.Series[i];
				var points = ((OxyPlot.Series.ScatterSeries)series).Points;
				points.Add(new OxyPlot.Series.ScatterPoint(pi.XValue.Item2, pi.YValues[i].Item2));

				pointsTotalCount += points.Count;
			}

			if (pointsTotalCount > 640) // Arbitrary number, based on FullHD Width / MarkerSize = 1920 / 3 = 640.
			{                           // Improve readability and performance on large number of points:
				foreach (var series in OxyModel.Series)
					((OxyPlot.Series.ScatterSeries)series).MarkerType = OxyPlot.MarkerType.None;
			}
		}

		/// <summary></summary>
		protected virtual void AddItemToHistogram(AutoActionPlotItem pi, Color color, Orientation orientation)
		{
			if (OxyModel.Axes.Count == 0)
			{
				if (orientation == Orientation.Horizontal)
				{
					OxyModel.Axes.Add(new OxyPlot.Axes.CategoryAxis { MajorGridlineStyle = OxyPlot.LineStyle.Solid, Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Bins", GapWidth = 0.1, StringFormat = "G3" }); // Not using 'SuperExponentialFormat' for two reasons:
					OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis   { MajorGridlineStyle = OxyPlot.LineStyle.Solid, Position = OxyPlot.Axes.AxisPosition.Left,   Title = "Counts", AbsoluteMinimum = 0.0 });             // 1. Readability (always that format) 2. Performance (time consuming)
				}
				else
				{
					OxyModel.Axes.Add(new OxyPlot.Axes.CategoryAxis { MajorGridlineStyle = OxyPlot.LineStyle.Solid, Position = OxyPlot.Axes.AxisPosition.Left,   Title = "Bins", GapWidth = 0.1, StringFormat = "G3" }); // Not using 'SuperExponentialFormat' for two reasons:
					OxyModel.Axes.Add(new OxyPlot.Axes.LinearAxis   { MajorGridlineStyle = OxyPlot.LineStyle.Solid, Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Counts", AbsoluteMinimum = 0.0 });             // 1. Readability (always that format) 2. Performance (time consuming)
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
					OxyModel.Series.Add(new OxyPlot.Series.BarSeries    { Title = pi.YValues[0].Item1, FillColor = OxyPlotEx.ConvertTo(color) });
				}
			}

			if (Histogram == null)
				Histogram = new HistogramDouble(256); // Arbitrary maximum number of bins, resulting in at least some pixels per bin.

			foreach (var tuple in pi.YValues)
				Histogram.Add(tuple.Item2);

			// While directly adding data point is the best performing way to add items according
			// to https://oxyplot.readthedocs.io/en/latest/guidelines/performance.html, this seems
			// an overkill for the histogram, as it would have to notify about added as well as
			// inserted bins and...and...and... Too complicated, simply recreate:

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
		}

		/// <remarks>
		/// This method partly replicates <see cref="OxyPlot.Axes.Axis.FormatValueOverride(double)"/>
		/// because it is not available on an <see cref="OxyPlot.Axes.CategoryAxis"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Underride", Justification = "Meant to be something like but opposite to 'override'.")]
		[CLSCompliant(false)]
		protected static string FormatAxisValueUnderride(double value, OxyPlot.Axes.Axis axis, CultureInfo actualCulture)
		{
			string format = string.Concat("{0:", axis.ActualStringFormat ?? axis.StringFormat ?? string.Empty, "}");
			return (string.Format(actualCulture, format, value));
		}

		/// <summary></summary>
		protected virtual void Invalidate(bool updateData)
		{
			OxyModel.InvalidatePlot(updateData);
		}

		#endregion
	}

	#region Extension Methods
	//==============================================================================================
	// Extension Methods
	//==============================================================================================

	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class OxyPlotEx
	{
		/// <summary></summary>
		[CLSCompliant(false)]
		public static OxyPlot.OxyColor ConvertTo(Color color)
		{
			return (OxyPlot.OxyColor.FromArgb(color.A, color.R, color.G, color.B));
		}
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
