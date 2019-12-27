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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace YAT.Model.Types
{
	/// <summary>
	/// Defines an item to plot.
	/// </summary>
	public abstract class AutoActionPlotItem
	{
		/// <summary></summary>
		public AutoActionPlot Type { get; }

		/// <summary></summary>
		public AutoActionPlotItem(AutoActionPlot type)
		{
			Type = type;
		}
	}

	/// <summary>
	/// Defines an item to plot.
	/// </summary>
	public abstract class MultiValueAutoActionPlotItem : AutoActionPlotItem
	{
		/// <summary></summary>
		public MultiValueAutoActionPlotItem(AutoActionPlot type)
			: base(type)
		{
		}
	}

	/// <summary>
	/// Defines an item to plot.
	/// </summary>
	public class MultiDoubleAutoActionPlotItem : MultiValueAutoActionPlotItem
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Source is an array, sink is an array, this class transports the array from source to sink, there's no purpose to use a ReadOnlyCollection here.")]
		public IEnumerable<double> Values { get; }

		/// <summary></summary>
		public MultiDoubleAutoActionPlotItem(AutoActionPlot type, IEnumerable<double> values)
			: base(type)
		{
			Values = values;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
