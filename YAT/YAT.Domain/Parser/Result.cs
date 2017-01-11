﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

namespace YAT.Domain.Parser
{
	/// <summary></summary>
	public abstract class Result
	{
	}

	/// <summary></summary>
	public class ByteArrayResult : Result
	{
		private byte[] byteArray;

		/// <summary></summary>
		public ByteArrayResult(byte[] byteArray)
		{
			this.byteArray = byteArray;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Source is an array, sink is an array, this class transports the array from source to sink, there's no purpose to use a ReadOnlyCollection here.")]
		public byte[] ByteArray
		{
			get { return (this.byteArray); }
		}
	}

	/// <summary></summary>
	public class KeywordResult : Result
	{
		private Keyword keyword;

		/// <summary></summary>
		public KeywordResult(Keyword keyword)
		{
			this.keyword = keyword;
		}

		/// <summary></summary>
		public Keyword Keyword
		{
			get { return (this.keyword); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
