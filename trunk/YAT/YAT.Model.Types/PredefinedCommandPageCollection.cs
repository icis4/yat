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
			foreach (var p in collection)
			{
				int spreadRatio = (int)(Math.Ceiling(((double)(p.Commands.Count)) / (double)(commandCapacityPerPage)));
				if (spreadRatio > 1) // e.g. 20:12 = 2, or 30:12 = 3, or 30:24 = 2.
					AddRange(CreateSpread(p, commandCapacityPerPage, spreadRatio));
				else
					Add(p);
			}
		}

		/// <summary></summary>
		public void InsertSpreaded(int index, IEnumerable<PredefinedCommandPage> collection, int commandCapacityPerPage)
		{
			foreach (var p in collection)
			{
				int spreadRatio = (int)(Math.Ceiling(((double)(p.Commands.Count)) / (double)(commandCapacityPerPage)));
				if (spreadRatio > 1) // e.g. 20:12 = 2, or 30:12 = 3, or 30:24 = 2.
					InsertRange(index, CreateSpread(p, commandCapacityPerPage, spreadRatio));
				else
					Insert(index, p);
			}
		}

		/// <summary></summary>
		protected static IEnumerable<PredefinedCommandPage> CreateSpread(PredefinedCommandPage page, int commandCapacityPerPage, int spreadRatio)
		{
			for (int i = 0; i < spreadRatio; i++)
			{
				var spreadPage = new PredefinedCommandPage(commandCapacityPerPage); // Preset the required capacity to improve memory management.

				if (spreadRatio <= 1)
					spreadPage.Name = page.Name;
				else
					spreadPage.Name = page.Name + string.Format(CultureInfo.CurrentUICulture, " ({0}/{1})", (i + 1), spreadRatio);

				for (int j = 0; j < commandCapacityPerPage; j++)
				{
					int cmdIdx = ((i * commandCapacityPerPage) + j);
					if (cmdIdx < page.Commands.Count)
						spreadPage.Commands.Add(page.Commands[cmdIdx]);
					else
						break;
				}

				spreadPage.RemoveTrailingCommands(); // May occur on intermediate spreads, should not occur on last spread.

				yield return (spreadPage);
			}
		}

		/// <summary></summary>
		public void AddMerged(IEnumerable<PredefinedCommandPage> collection, int commandCapacityPerPageOld, int commandCapacityPerPageNew)
		{
			int mergeRatio = (commandCapacityPerPageNew / commandCapacityPerPageOld);
			if (mergeRatio > 1) // e.g. 24:12 = 2, or 48:24 = 2, but 36:24 = 1 requires no merge.
				AddRange(CreateMerge(collection, commandCapacityPerPageOld, mergeRatio));
			else
				AddRange(collection);
		}

		/// <summary></summary>
		public void InsertMerged(int index, IEnumerable<PredefinedCommandPage> collection, int commandCapacityPerPageOld, int commandCapacityPerPageNew)
		{
			int mergeRatio = (commandCapacityPerPageNew / commandCapacityPerPageOld);
			if (mergeRatio > 1) // e.g. 24:12 = 2, or 48:24 = 2, but 36:24 = 1 requires no merge.
				InsertRange(index, CreateMerge(collection, commandCapacityPerPageOld, mergeRatio));
			else
				InsertRange(index, collection);
		}

		/// <summary></summary>
		protected static IEnumerable<PredefinedCommandPage> CreateMerge(IEnumerable<PredefinedCommandPage> collection, int commandCapacityPerPageOld, int mergeRatio)
		{
			var enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext()) // Collection contains (more) elements:
			{
				var mergePage = new PredefinedCommandPage(mergeRatio * commandCapacityPerPageOld); // Preset the required capacity to improve memory management.

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

				yield return (mergePage);
			}
		}

		/// <summary></summary>
		public void AddTruncated(IEnumerable<PredefinedCommandPage> collection, int commandCapacityPerPage)
		{
			foreach (var p in collection)
				Add(CreateTruncation(p, commandCapacityPerPage));
		}

		/// <summary></summary>
		public void InsertTruncated(int index, IEnumerable<PredefinedCommandPage> collection, int commandCapacityPerPage)
		{
			foreach (var p in collection)
				Insert(index, CreateTruncation(p, commandCapacityPerPage));
		}

		/// <summary></summary>
		protected static PredefinedCommandPage CreateTruncation(PredefinedCommandPage page, int commandCapacityPerPage)
		{
			var truncatedCount = Math.Min(page.Commands.Count, commandCapacityPerPage);
			var truncatedPage = new PredefinedCommandPage(commandCapacityPerPage, page.Name);
			truncatedPage.Commands.AddRange(page.Commands.Take(truncatedCount));
			return (truncatedPage);
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
