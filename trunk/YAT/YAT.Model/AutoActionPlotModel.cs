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

using System;
using System.Collections.Generic;

namespace YAT.Model.Types
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

		/// <summary>
		/// Gets or sets a counter that indicates whether and how many times the plot has changed.
		/// </summary>
		public int UpdateCounter { get; protected set; } // = 0;

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public void AddItem(AutoActionPlotItem pi)
		{
			Action = pi.Action;

			Title = (AutoActionEx)pi.Action;

			switch (pi.Action)
			{
				case AutoAction.LineChartIndex:     AddItemToLineChartIndex(pi);     break;
				case AutoAction.LineChartTimeStamp: AddItemToLineChartTimeStamp(pi); break;
				case AutoAction.ScatterPlotXY:      AddItemToScatterPlotXY(pi);      break;
				case AutoAction.ScatterPlotTime:    AddItemToScatterPlotXTime(pi);   break;
				case AutoAction.Histogram:          AddItemToHistogram(pi);          break;
			}

			IndicateUpdate();
		}

		/// <summary></summary>
		public virtual void ClearAllItems()
		{
			XValues = null;
			YValues = null;

			IndicateUpdate();
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		/// <summary></summary>
		protected virtual void AddItemToLineChartIndex(AutoActionPlotItem pi)
		{
			XLabel = "Index";

			AddItemToLineChart(pi);
		}

		/// <summary></summary>
		protected virtual void AddItemToLineChartTimeStamp(AutoActionPlotItem pi)
		{
			XLabel = "Time Stamp";

			AddItemToLineChart(pi);
		}

		/// <summary></summary>
		protected virtual void AddItemToLineChart(AutoActionPlotItem pi)
		{
			YLabel = "Value";

			var vc = (pi as ValueCollectionAutoActionPlotItem);

			if ((XValues == null) || (YValues == null))
			{
				XValues = new Tuple<string, List<double>>(vc.XValue.Item1, new List<double>(1024)); // Add a new empty list.
				YValues = new List<Tuple<string, List<double>>>(vc.YValues.Length); // Preset the required capacity to improve memory management.
			}

			XValues.Item2.Add(vc.XValue.Item2);

			for (int i = YValues.Count; i < vc.YValues.Length; i++)
			{
				string label = vc.YValues[i].Item1;

				List<double> values;
				if ((i == 0) || (YValues[0].Item2.Count == 0))
					values = new List<double>(1024); // Add a new empty list.
				else
					values = new List<double>(new double[YValues[0].Item2.Count]); // Add a new list filled with default values.

				YValues.Add(new Tuple<string, List<double>>(label, values));
			}

			for (int i = 0; i < YValues.Count; i++)
			{
				if (i < vc.YValues.Length)
					YValues[i].Item2.Add(vc.YValues[i].Item2);
				else
					YValues[i].Item2.Add(0); // Fill with default value.
			}
		}

		/// <summary></summary>
		protected virtual void AddItemToScatterPlotXY(AutoActionPlotItem pi)
		{
			// PENDING
		}

		/// <summary></summary>
		protected virtual void AddItemToScatterPlotXTime(AutoActionPlotItem pi)
		{
			// PENDING
		}

		/// <summary></summary>
		protected virtual void AddItemToHistogram(AutoActionPlotItem pi)
		{
			// PENDING Make histo bins and counts (bins epsilon up to 1024, then equally distributed)
		}

		/// <summary></summary>
		protected virtual void IndicateUpdate()
		{
			unchecked // Loop-around is OK.
			{
				UpdateCounter++;
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
