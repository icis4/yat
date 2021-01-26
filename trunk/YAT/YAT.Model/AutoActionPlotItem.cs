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
// YAT Version 2.4.0
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

using System;
using System.Diagnostics.CodeAnalysis;

using YAT.Model.Types;

namespace YAT.Model
{
	/// <summary>
	/// Defines an item to plot.
	/// </summary>
	public class AutoActionPlotItem
	{
		/// <summary></summary>
		public AutoAction Action { get; }

		/// <summary></summary>
		public Tuple<string, double> XValue { get; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Don't care, straightforward implementation.")]
		public Tuple<string, double>[] YValues { get; }

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "'x' and 'y' are common terms for identifying the axes of a plot.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "x", Justification = "'x' and 'y' are common terms for identifying the axes of a plot.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "y", Justification = "'x' and 'y' are common terms for identifying the axes of a plot.")]
		public AutoActionPlotItem(AutoAction action, Tuple<string, double> xValue, Tuple<string, double>[] yValues)
		{
			Action  = action;
			XValue  = xValue;
			YValues = yValues;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
