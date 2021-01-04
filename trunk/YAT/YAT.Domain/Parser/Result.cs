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
// YAT Version 2.2.0 Development
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

using System.Diagnostics.CodeAnalysis;

namespace YAT.Domain.Parser
{
	/// <summary></summary>
	public abstract class Result
	{
	}

	/// <summary></summary>
	public class BytesResult : Result
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] Bytes { get; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes", Justification = "'Byte' not only is a type, it also tells the semantic.")]
		public BytesResult(byte[] bytes)
		{
			Bytes = bytes;
		}
	}

	/// <remarks>
	/// So far, this type can only deal with integer values. As soon as floating point, boolean,
	/// enum or string values are required, this type will have to be extended accordingly.
	/// </remarks>
	public class KeywordResult : Result
	{
		/// <summary></summary>
		public Keyword Keyword { get; }

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Performance is not an issue here, simplicity and ease of use is...")]
		public int[] Args { get; }

		/// <summary></summary>
		public KeywordResult(Keyword keyword)
			: this(keyword, null)
		{
		}

		/// <summary></summary>
		public KeywordResult(Keyword keyword, int[] args)
		{
			Keyword = keyword;
			Args    = args;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
