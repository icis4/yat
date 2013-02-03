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
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

namespace YAT.Model.Types
{
	/// <summary></summary>
	[Serializable]
	public class PredefinedCommandPageCollection : List<PredefinedCommandPage>
	{
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
			// Perfom a deep copy of the pages to break command references
			foreach (PredefinedCommandPage pcp in collection)
				Add(new PredefinedCommandPage(pcp));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
