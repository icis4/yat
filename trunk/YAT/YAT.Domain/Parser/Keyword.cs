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

using MKY.Types;

namespace YAT.Domain.Parser
{
	#region Enum Keyword

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum Keyword
	{
		None,
		Delay,
		Eol,
		NoEol,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum KeywordEx.
	/// </summary>
	public class KeywordEx : EnumEx
	{
		#region String Definitions

		private const string Delay_string = "Delay";
		private const string Eol_string = "EOL";
		private const string NoEol_string = "NoEOL";

		#endregion

		/// <summary>Default is <see cref="Keyword.None"/>.</summary>
		public KeywordEx()
			: base(Keyword.None)
		{
		}

		/// <summary></summary>
		protected KeywordEx(Keyword keyword)
			: base(keyword)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((Keyword)UnderlyingEnum)
			{
				case Keyword.Delay: return (Delay_string);
				case Keyword.Eol:   return (Eol_string);
				case Keyword.NoEol: return (NoEol_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static KeywordEx[] GetItems()
		{
			List<KeywordEx> a = new List<KeywordEx>();
			a.Add(new KeywordEx(Keyword.Delay));
			a.Add(new KeywordEx(Keyword.Eol));
			a.Add(new KeywordEx(Keyword.NoEol));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static KeywordEx Parse(string keyword)
		{
			KeywordEx result;

			if (TryParse(keyword, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("keyword", keyword, "Invalid keyword."));
		}

		/// <summary></summary>
		public static bool TryParse(string keyword, out KeywordEx result)
		{
			if      (string.Compare(keyword, Delay_string, StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = new KeywordEx(Keyword.Delay);
				return (true);
			}
			else if (string.Compare(keyword, Eol_string, StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = new KeywordEx(Keyword.Eol);
				return (true);
			}
			else if (string.Compare(keyword, NoEol_string, StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = new KeywordEx(Keyword.NoEol);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator Keyword(KeywordEx keyword)
		{
			return ((Keyword)keyword.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator KeywordEx(Keyword keyword)
		{
			return (new KeywordEx(keyword));
		}

		/// <summary></summary>
		public static implicit operator int(KeywordEx keyword)
		{
			return (keyword.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator KeywordEx(int keyword)
		{
			return (new KeywordEx((Keyword)keyword));
		}

		/// <summary></summary>
		public static implicit operator string(KeywordEx keyword)
		{
			return (keyword.ToString());
		}

		/// <summary></summary>
		public static implicit operator KeywordEx(string keyword)
		{
			return (Parse(keyword));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
