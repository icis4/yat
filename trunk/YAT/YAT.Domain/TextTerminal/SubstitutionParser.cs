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

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in YAT.Domain\TextTerminal for better separation of the implementation files.
namespace YAT.Domain.Parser
{
	/// <summary>
	/// Extends Parser with character substitution.
	/// </summary>
	public class SubstitutionParser : Parser
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private CharSubstitution substitution;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public SubstitutionParser(CharSubstitution substitution, Encoding encoding, Endianness endianness, Mode modes)
			: base(encoding, endianness, modes)
		{
			this.substitution = substitution;
		}

		/// <summary></summary>
		internal SubstitutionParser(SubstitutionParser parent, ParserState parserState)
			: base(parent, parserState)
		{
			this.substitution = parent.substitution;
		}

		#endregion

		#region Factory
		//==========================================================================================
		// Factory
		//==========================================================================================

		/// <summary></summary>
		internal override Parser GetNestedParser(ParserState parserState)
		{
			AssertUndisposed();

			return (new SubstitutionParser(this, parserState));
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		internal override bool TryParseContiguousRadixItem(string item, Radix radix, out byte[] result, ref FormatException formatException)
		{
		////AssertUndisposed() is called by 'base.TryParseContiguousRadixItem()' below.

			return (base.TryParseContiguousRadixItem(Substitute(item), radix, out result, ref formatException));
		}

		private string Substitute(string item)
		{
			switch (this.substitution)
			{
				case CharSubstitution.ToUpper: return (item.ToUpper(CultureInfo.CurrentCulture));
				case CharSubstitution.ToLower: return (item.ToLower(CultureInfo.CurrentCulture));
				default:                       return (item);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
