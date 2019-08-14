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
using System.Linq;

namespace YAT.Model.Types
{
	/// <summary></summary>
	[Serializable]
	public class PredefinedCommandPageCollection : List<PredefinedCommandPage>
	{
		/// <remarks>Commands and pages are numbered 1..max, 0 indicates none/invalid.</remarks>
		public const int NoPageId = 0;

		/// <remarks>Commands and pages are numbered 1..max, 0 indicates none/invalid.</remarks>
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

		/// <remarks>Explicitly name "Command" to clearly distiguish from list of pages.</remarks>
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

		/// <remarks>Explicitly name "Command" to clearly distiguish from list of pages.</remarks>
		public int TotalDefinedCommandCount
		{
			get
			{
				int n = 0;

				foreach (var p in this) {
					n += p.DefinedCommandCount;
				}

				return (n);
			}
		}

		/// <summary>The number of pages linked to a file.</summary>
		public int LinkedToFilePathCount
		{
			get
			{
				int n = 0;

				foreach (var p in this) {
					if (p.IsLinkedToFilePath) {
						n++;
					}
				}

				return (n);
			}
		}

		/// <summary></summary>
		public void AddSpreaded(IEnumerable<PredefinedCommandPage> collection, int commandCapacityPerPage)
		{
			// Attention:
			// Similar code exists in InsertSpreaded() below.
			// Changes here may have to be applied there too.

			foreach (var p in collection)
			{
				int n = (int)(Math.Ceiling(((double)(p.Commands.Count)) / (commandCapacityPerPage)));
				for (int i = 0; i < n; i++)
				{
					var spreadPage = new PredefinedCommandPage();

					if (n <= 1)
						spreadPage.Name = p.Name;
					else
						spreadPage.Name = p.Name + string.Format(CultureInfo.CurrentUICulture, " ({0}/{1})", i, n);

					for (int j = 0; j < commandCapacityPerPage; j++)
					{
						int cmdIdx = ((i * commandCapacityPerPage) + j);
						spreadPage.Commands.Add(p.Commands[cmdIdx]);
					}

					Add(spreadPage);
				}
			}
		}

		/// <summary></summary>
		public void InsertSpreaded(int index, IEnumerable<PredefinedCommandPage> collection, int commandCapacityPerPage)
		{
			// Attention:
			// Similar code exists in AddSpreaded() above.
			// Changes here may have to be applied there too.

			foreach (var p in collection)
			{
				int n = (int)(Math.Ceiling(((double)(p.Commands.Count)) / (commandCapacityPerPage)));
				for (int i = 0; i < n; i++)
				{
					var spreadPage = new PredefinedCommandPage();

					if (n <= 1)
						spreadPage.Name = p.Name;
					else
						spreadPage.Name = p.Name + string.Format(CultureInfo.CurrentUICulture, " ({0}/{1})", i, n);

					for (int j = 0; j < commandCapacityPerPage; j++)
					{
						int cmdIdx = ((i * commandCapacityPerPage) + j);
						spreadPage.Commands.Add(p.Commands[cmdIdx]);
					}

					Insert(index, spreadPage);
				}
			}
		}

		/// <summary></summary>
		public void AddMerged(IEnumerable<PredefinedCommandPage> collection, int commandCapacityPerPageOld, int commandCapacityPerPageNew)
		{
			// Attention:
			// Similar code exists in InsertMerged() below.
			// Changes here may have to be applied there too.

			int mergeRatio = (commandCapacityPerPageNew / commandCapacityPerPageOld);
			if (mergeRatio > 1) // e.g. 24:12 = 2, or 48:24 = 2, but 36:24 = 1 requires no merge.
			{
				var enumerator = collection.GetEnumerator();
				while (enumerator.MoveNext()) // Collection contains (more) elements:
				{
					var mergePage = new PredefinedCommandPage();

					for (int i = 0; i < mergeRatio; i++)
					{
						var p = enumerator.Current;

						if (i == 0)
							mergePage.Name = p.Name;
						else
							mergePage.Name += (" + " + p.Name);

						int cmdIdx = 0;
						while (cmdIdx < p.Commands.Count)
						{
							mergePage.Commands.Add(p.Commands[cmdIdx]);
							cmdIdx++;
						}

						if ((cmdIdx < commandCapacityPerPageOld) && // Source page was not completely filled.
							(i != (mergeRatio - 1)))                // This is not the last page to merge.
						{
							while (cmdIdx < commandCapacityPerPageOld)
							{
								mergePage.Commands.Add(new Command()); // Fill-in empty commands.
								cmdIdx++;
							}
						}

						if (i < (mergeRatio - 1))
						{
							if (enumerator.MoveNext()) // Collection contains (more) elements:
								continue;
							else
								break;
						}
					}

					Add(mergePage);
				}
			}
			else
			{
				AddRange(collection);
			}
		}

		/// <summary></summary>
		public void InsertMerged(int index, IEnumerable<PredefinedCommandPage> collection, int commandCapacityPerPageOld, int commandCapacityPerPageNew)
		{
			// Attention:
			// Similar code exists in AddMerged() above.
			// Changes here may have to be applied there too.

			int mergeRatio = (commandCapacityPerPageNew / commandCapacityPerPageOld);
			if (mergeRatio > 1) // e.g. 24:12 = 2, or 48:24 = 2, but 36:24 = 1 requires no merge.
			{
				var enumerator = collection.GetEnumerator();
				while (enumerator.MoveNext()) // Collection contains (more) elements:
				{
					var mergePage = new PredefinedCommandPage();

					for (int i = 0; i < mergeRatio; i++)
					{
						var p = enumerator.Current;

						if (i == 0)
							mergePage.Name = p.Name;
						else
							mergePage.Name += (" + " + p.Name);

						int cmdIdx = 0;
						while (cmdIdx < p.Commands.Count)
						{
							mergePage.Commands.Add(p.Commands[cmdIdx]);
							cmdIdx++;
						}

						if ((cmdIdx < commandCapacityPerPageOld) && // Source page was not completely filled.
							(i != (mergeRatio - 1)))                // This is not the last page to merge.
						{
							while (cmdIdx < commandCapacityPerPageOld)
							{
								mergePage.Commands.Add(new Command()); // Fill-in empty commands.
								cmdIdx++;
							}
						}

						if (i < (mergeRatio - 1))
						{
							if (enumerator.MoveNext()) // Collection contains (more) elements:
								continue;
							else
								break;
						}
					}

					Insert(index, mergePage);
				}
			}
			else
			{
				InsertRange(index, collection);
			}
		}

		/// <summary></summary>
		public void UnlinkAll()
		{
			foreach (var p in this)
				p.Unlink();
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
