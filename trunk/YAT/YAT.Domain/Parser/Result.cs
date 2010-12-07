//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Domain.Parser
{
	/// <summary></summary>
	public abstract class Result
	{
	}

	/// <summary></summary>
	public class ByteArrayResult : Result
	{
		/// <summary></summary>
		public readonly byte[] ByteArray;

		/// <summary></summary>
		public ByteArrayResult(byte[] byteArray)
		{
			ByteArray = byteArray;
		}
	}

	/// <summary></summary>
	public class KeywordResult : Result
	{
		/// <summary></summary>
		public readonly Keyword Keyword;

		/// <summary></summary>
		public KeywordResult(Keyword keyword)
		{
			Keyword = keyword;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
