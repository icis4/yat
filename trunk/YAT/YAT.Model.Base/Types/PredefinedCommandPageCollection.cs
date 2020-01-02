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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#endregion

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

		/// <summary></summary>
		public const string StandardPageNamePrefix = "Page ";

		/// <remarks>Constant (and not a generated readonly) for simplicity.</remarks>
		public const string FirstPageNameDefault = "Page 1";

		private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

		/// <summary></summary>
		public static readonly Regex StandardPageNameRegex = new Regex(StandardPageNamePrefix + @"(?<pageId>\d+)", Options);

		/// <remarks>
		/// Must be implemented as property (instead of a readonly) since <see cref="PredefinedCommandPage"/>
		/// is a mutable reference type. Defining a readonly would correctly result in FxCop
		/// message CA2104 "DoNotDeclareReadOnlyMutableReferenceTypes" (Microsoft.Security).
		/// </remarks>
		public static PredefinedCommandPage DefaultPage
		{
			get { return (new PredefinedCommandPage(FirstPageNameDefault)); }
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

		/// <remarks>Explicitly name "Command" to clearly distinguish from list of pages.</remarks>
		public int MaxDefinedCommandCountPerPage
		{
			get
			{
				int max = 0;

				foreach (var p in this) {
					if (max < p.DefinedCommandCount) {
						max = p.DefinedCommandCount;
					}
				}

				return (max);
			}
		}

		/// <remarks>Explicitly name "Command" to clearly distinguish from list of pages.</remarks>
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
		public static string StandardPageName(int id)
		{
			var sb = new StringBuilder("Page ");

			sb.Append(id);

			return (sb.ToString());
		}

		/// <summary></summary>
		public static bool IsStandardPageName(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				var m = StandardPageNameRegex.Match(name);
				if (m.Success)
					return (true);
			}

			return (false);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spreaded", Justification = "'Spreaded' sounds more natural for a non-English guy like me, and symmetricity with 'Merged' and 'Truncated'.")]
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spreaded", Justification = "'Spreaded' sounds more natural for a non-English guy like me, and symmetricity with 'Merged' and 'Truncated'.")]
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

				if (spreadRatio <= 1) // Keep page name unchanged.
					spreadPage.Name = page.Name;
				else                  // Standard named pages could be renumbered on spread, but decided against to make spread obvious to user.
					spreadPage.Name = page.Name + string.Format(CultureInfo.CurrentCulture, " ({0}/{1})", (i + 1), spreadRatio);

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

					if (i == 0) // Standard named pages could be renumbered on merge, but decided against to make merge obvious to user.
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
			var truncatedPage = new PredefinedCommandPage(commandCapacityPerPage, page.Name); // Keep page name unchanged.
			truncatedPage.Commands.AddRange(page.Commands.Take(truncatedCount));
			return (truncatedPage);
		}

		/// <summary></summary>
		public void Renumber()
		{
			for (int i = 0; i < Count; i++)
				this[i].Name = StandardPageName(i + 1);
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
