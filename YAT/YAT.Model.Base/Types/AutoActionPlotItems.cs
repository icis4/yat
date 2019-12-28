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

namespace YAT.Model.Types
{
	/// <summary>
	/// Defines an item to plot.
	/// </summary>
	public class AutoActionPlotItem
	{
		/// <summary></summary>
		public AutoActionPlot Type { get; }

		/// <summary></summary>
		public string Title { get; }

		/// <summary></summary>
		public string XCaption { get; }

		/// <summary></summary>
		public string YCaption { get; }

		/// <summary></summary>
		public double[] XValues { get; }

		/// <summary></summary>
		public double[] YValues { get; }

		/// <summary></summary>
		public AutoActionPlotItem(AutoActionPlot type, string title, string xCaption, string yCaption, double[] xValues, double[] yValues)
		{
			Type = type;

			Title = title;

			XCaption = xCaption;
			YCaption = yCaption;

			XValues = xValues;
			YValues = yValues;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
