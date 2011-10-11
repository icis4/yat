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
// YAT 2.0 Beta 4 Candidate 1 Development Version 1.99.27
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

using MKY;

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
		Clear,
		Delay,
		Eol,
		NoEol,
		OutputBreakOn,
		OutputBreakOff,
		OutputBreakToggle,
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum KeywordEx.
	/// </summary>
	public class KeywordEx : MKY.EnumEx
	{
		#region String Definitions

		private const string Clear_string = "Clear";
		private const string Delay_string = "Delay";
		private const string Eol_string = "EOL";
		private const string NoEol_string = "NoEOL";
		private const string OutputBreakOn_string = "OutputBreakOn";
		private const string OutputBreakOff_string = "OutputBreakOff";
		private const string OutputBreakToggle_string = "OutputBreakToggle";

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
				case Keyword.Clear: return (Clear_string);
				case Keyword.Delay: return (Delay_string);

				case Keyword.Eol:   return (Eol_string);
				case Keyword.NoEol: return (NoEol_string);

				case Keyword.OutputBreakOn:     return (OutputBreakOn_string);
				case Keyword.OutputBreakOff:    return (OutputBreakOff_string);
				case Keyword.OutputBreakToggle: return (OutputBreakToggle_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static KeywordEx[] GetItems()
		{
			List<KeywordEx> a = new List<KeywordEx>();
			a.Add(new KeywordEx(Keyword.Clear));
			a.Add(new KeywordEx(Keyword.Delay));
			a.Add(new KeywordEx(Keyword.Eol));
			a.Add(new KeywordEx(Keyword.NoEol));
			a.Add(new KeywordEx(Keyword.OutputBreakOn));
			a.Add(new KeywordEx(Keyword.OutputBreakOff));
			a.Add(new KeywordEx(Keyword.OutputBreakToggle));
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
			if      (StringEx.EqualsOrdinalIgnoreCase(keyword, Clear_string))
			{
				result = new KeywordEx(Keyword.Clear);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(keyword, Delay_string))
			{
				result = new KeywordEx(Keyword.Delay);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(keyword, Eol_string))
			{
				result = new KeywordEx(Keyword.Eol);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(keyword, NoEol_string))
			{
				result = new KeywordEx(Keyword.NoEol);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(keyword, OutputBreakOn_string))
			{
				result = new KeywordEx(Keyword.OutputBreakOn);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(keyword, OutputBreakOff_string))
			{
				result = new KeywordEx(Keyword.OutputBreakOff);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(keyword, OutputBreakToggle_string))
			{
				result = new KeywordEx(Keyword.OutputBreakToggle);
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
