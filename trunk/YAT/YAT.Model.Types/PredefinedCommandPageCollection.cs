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
// YAT Version 2.0.1 Development
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
using System.Collections.Generic;
using System.Globalization;

namespace YAT.Model.Types
{
	/// <summary></summary>
	[Serializable]
	public class PredefinedCommandPageCollection : List<PredefinedCommandPage>
	{
		/// <remarks>Commands and pages are numberd 1..max, 0 indicates none/invalid.</remarks>
		public const int NoPageId = 0;

		/// <remarks>Commands and pages are numberd 1..max, 0 indicates none/invalid.</remarks>
		public const int FirstPageId = 1;

		/// <summary></summary>
		public const int MaxCapacity = int.MaxValue;

		/// <remarks>
		/// Must be implemented as property (instead of a readonly) since <see cref="PredefinedCommandPage"/>
		/// is a mutable reference type. Defining a readonly would correctly result in FxCop
		/// message CA2104 "DoNotDeclareReadOnlyMutableReferenceTypes" (Microsoft.Security).
		/// </remarks>
		public static PredefinedCommandPage DefaultPage
		{
			get { return (new PredefinedCommandPage("Page 1")); }
		}

		/// <summary></summary>
		public PredefinedCommandPageCollection()
			: base()
		{
		}

		/// <summary></summary>
		public PredefinedCommandPageCollection(int capacity)
			: base(capacity)
		{
		}

		/// <summary></summary>
		public PredefinedCommandPageCollection(IEnumerable<PredefinedCommandPage> collection)
			: base()
		{
			// Perfom a deep copy to break references:
			foreach (PredefinedCommandPage pcp in collection)
				Add(new PredefinedCommandPage(pcp));
		}

		/// <summary></summary>
		public int MaxCommandCountPerPage
		{
			get
			{
				int max = 0;

				foreach (var p in this) {
					if (p.Commands != null) {
						if (max < p.Commands.Count) {
							max = p.Commands.Count;
						}
					}
				}

				return (max);
			}
		}

		/// <summary></summary>
		public void AddSpreaded(IEnumerable<PredefinedCommandPage> collection, int commandCapacityPerPage)
		{
			// Attention:
			// Similar code exists in InsertSpreaded() further below.
			// Changes here may have to be applied there too.

			foreach (var p in collection)
			{
				int n = (int)(Math.Ceiling(((double)(p.Commands.Count)) / (commandCapacityPerPage)));
				for (int i = 0; i < n; i++)
				{
					var spreadPage = new PredefinedCommandPage();

					if (n <= 1)
						spreadPage.PageName = p.PageName;
					else
						spreadPage.PageName = p.PageName + string.Format(CultureInfo.CurrentUICulture, " ({0}/{1})", i, n);

					for (int j = 0; j < commandCapacityPerPage; j++)
					{
						int indexImported = ((i * commandCapacityPerPage) + j);
						spreadPage.Commands.Add(p.Commands[indexImported]);
					}

					Add(spreadPage);
				}
			}
		}

		/// <summary></summary>
		public void InsertSpreaded(int index, IEnumerable<PredefinedCommandPage> collection, int commandCapacityPerPage)
		{
			// Attention:
			// Similar code exists in AddSpreaded() further above.
			// Changes here may have to be applied there too.

			foreach (var p in collection)
			{
				int n = (int)(Math.Ceiling(((double)(p.Commands.Count)) / (commandCapacityPerPage)));
				for (int i = 0; i < n; i++)
				{
					var spreadPage = new PredefinedCommandPage();

					if (n <= 1)
						spreadPage.PageName = p.PageName;
					else
						spreadPage.PageName = p.PageName + string.Format(CultureInfo.CurrentUICulture, " ({0}/{1})", i, n);

					for (int j = 0; j < commandCapacityPerPage; j++)
					{
						int indexImported = ((i * commandCapacityPerPage) + j);
						spreadPage.Commands.Add(p.Commands[indexImported]);
					}

					Insert(index, spreadPage);
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
