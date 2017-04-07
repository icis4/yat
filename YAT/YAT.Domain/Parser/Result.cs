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
// YAT 2.0 Gamma 3 Development Version 1.99.53
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
	public class BytesResult : Result
	{
		private byte[] bytes;

		/// <summary></summary>
		public BytesResult(byte[] bytes)
		{
			this.bytes = bytes;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Source is an array, sink is an array, this class transports the array from source to sink, there's no purpose to use a ReadOnlyCollection here.")]
		public byte[] Bytes
		{
			get { return (this.bytes); }
		}
	}

	/// <remarks>
	/// So far, this type can only deal with integer values. As soon as floating point, boolean,
	/// enum or string values are required, this type will have to be extended accordingly.
	/// </remarks>
	public class KeywordResult : Result
	{
		private Keyword keyword;
		private int[] args;

		/// <summary></summary>
		public KeywordResult(Keyword keyword)
			: this(keyword, null)
		{
		}

		/// <summary></summary>
		public KeywordResult(Keyword keyword, int[] args)
		{
			this.keyword = keyword;
			this.args    = args;
		}

		/// <summary></summary>
		public Keyword Keyword
		{
			get { return (this.keyword); }
		}

		/// <summary></summary>
		public int[] Args
		{
			get { return (this.args); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
