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

using System;

namespace YAT.Model.Types
{
	/// <summary>
	/// Defines an item to plot.
	/// </summary>
	public abstract class AutoActionPlotItem
	{
		/// <summary></summary>
		public AutoAction Action { get; }

		/// <summary></summary>
		public AutoActionPlotItem(AutoAction action)
		{
			Action = action;
		}
	}

	/// <summary>
	/// Defines an item to plot.
	/// </summary>
	public class ValueCollectionAutoActionPlotItem : AutoActionPlotItem
	{
		/// <summary></summary>
		public Tuple<string, double> XValue { get; }

		/// <summary></summary>
		public Tuple<string, double>[] YValues { get; }

		/// <summary></summary>
		public ValueCollectionAutoActionPlotItem(AutoAction action, Tuple<string, double> xValue, Tuple<string, double>[] yValues)
			: base(action)
		{
			XValue = xValue;
			YValues = yValues;
		}
	}

	/// <summary>
	/// Defines an item to plot.
	/// </summary>
	public class ValuePairAutoActionPlotItem : AutoActionPlotItem
	{
		/// <summary></summary>
		public Tuple<string, double> XValue { get; }

		/// <summary></summary>
		public Tuple<string, double> YValue { get; }

		/// <summary></summary>
		public ValuePairAutoActionPlotItem(AutoAction action, Tuple<string, double> xValue, Tuple<string, double> yValue)
			: base(action)
		{
			XValue = xValue;
			YValue = yValue;
		}
	}

	/// <summary>
	/// Defines an item to plot.
	/// </summary>
	public class ValueTimeAutoActionPlotItem : AutoActionPlotItem
	{
		/// <summary></summary>
		public Tuple<string, DateTime> XValue { get; }

		/// <summary></summary>
		public Tuple<string, double> YValue { get; }

		/// <summary></summary>
		public ValueTimeAutoActionPlotItem(AutoAction action, Tuple<string, DateTime> xValue, Tuple<string, double> yValue)
			: base(action)
		{
			XValue = xValue;
			YValue = yValue;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
