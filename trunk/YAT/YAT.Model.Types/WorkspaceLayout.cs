//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using MKY;

using YAT.Utilities;

namespace YAT.Model.Types
{
	#region Enum WorkspaceLayout

	/// <summary>
	/// Specifies the layout of multiple document interface (MDI) child windows in an MDI parent window.
	/// </summary>
	public enum WorkspaceLayout
	{
		/// <summary>
		/// All child windows are manually positioned within the client region of the MDI parent form.
		/// </summary>
		Manual = -1,

		/// <summary>
		/// All child windows are cascaded within the client region of the MDI parent form.
		/// </summary>
		Cascade = MdiLayout.Cascade, // = 0

		/// <summary>
		/// All child windows are tiled horizontally within the client region of the MDI parent form.
		/// </summary>
		TileHorizontal = MdiLayout.TileHorizontal, // = 1

		/// <summary>
		/// All child windows are tiled vertically within the client region of the MDI parent form.
		/// </summary>
		TileVertical = MdiLayout.TileVertical // = 2

		// System.Windows.Forms.MdiLayout.ArrangeIcons is not supported by YAT.
	}

	#endregion

	/// <summary>
	/// Extended enum WorkspaceLayoutEx.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class WorkspaceLayoutEx : EnumEx
	{
		/// <summary>Default is <see cref="WorkspaceLayout.Manual"/>.</summary>
		public WorkspaceLayoutEx()
			: base(WorkspaceLayout.Manual)
		{
		}

		/// <summary></summary>
		protected WorkspaceLayoutEx(WorkspaceLayout layout)
			: base(layout)
		{
		}

		#region GetItems

		/// <summary></summary>
		public static WorkspaceLayoutEx[] GetItems()
		{
			List<WorkspaceLayoutEx> a = new List<WorkspaceLayoutEx>();
			a.Add(new WorkspaceLayoutEx(WorkspaceLayout.Manual));
			a.Add(new WorkspaceLayoutEx(WorkspaceLayout.Cascade));
			a.Add(new WorkspaceLayoutEx(WorkspaceLayout.TileHorizontal));
			a.Add(new WorkspaceLayoutEx(WorkspaceLayout.TileVertical));
			return (a.ToArray());
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator WorkspaceLayout(WorkspaceLayoutEx layout)
		{
			return ((WorkspaceLayout)layout.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator WorkspaceLayoutEx(WorkspaceLayout layout)
		{
			return (new WorkspaceLayoutEx(layout));
		}

		/// <summary></summary>
		public static implicit operator int(WorkspaceLayoutEx layout)
		{
			return (layout.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator WorkspaceLayoutEx(int layout)
		{
			return (new WorkspaceLayoutEx((WorkspaceLayout)layout));
		}

		/// <summary></summary>
		public static implicit operator MdiLayout(WorkspaceLayoutEx layout)
		{
			switch ((WorkspaceLayout)layout.UnderlyingEnum)
			{
				case WorkspaceLayout.Manual:         throw (new NotSupportedException("'Manual' is not supported by 'Windows.Forms.MdiLayout'!"));
				case WorkspaceLayout.Cascade:        return (MdiLayout.Cascade);
				case WorkspaceLayout.TileHorizontal: return (MdiLayout.TileHorizontal);
				case WorkspaceLayout.TileVertical:   return (MdiLayout.TileVertical);
				default:                             throw (new InvalidOperationException("Invalid workspace layout!"));
			}
		}

		/// <summary></summary>
		public static implicit operator WorkspaceLayoutEx(MdiLayout layout)
		{
			switch (layout)
			{
				case MdiLayout.Cascade:        return (new WorkspaceLayoutEx(WorkspaceLayout.Cascade));
				case MdiLayout.TileHorizontal: return (new WorkspaceLayoutEx(WorkspaceLayout.TileHorizontal));
				case MdiLayout.TileVertical:   return (new WorkspaceLayoutEx(WorkspaceLayout.TileVertical));
				case MdiLayout.ArrangeIcons:   throw (new NotSupportedException("'ArrangeIcons' is not supported by " + ApplicationInfo.ProductName + "!"));
				default:                       throw (new InvalidOperationException("Invalid MDI layout!"));
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
