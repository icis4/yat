﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
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
		public SubstitutionParser()
			: base()
		{
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public SubstitutionParser(Endianness endianness)
			: base(endianness)
		{
		}

		/// <summary></summary>
		public SubstitutionParser(Encoding encoding)
			: base(encoding)
		{
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public SubstitutionParser(Endianness endianness, Encoding encoding)
			: base(endianness, encoding)
		{
		}

		/// <summary></summary>
		public SubstitutionParser(Radix defaultRadix)
			: base(defaultRadix)
		{
		}

		/// <summary></summary>
		public SubstitutionParser(Encoding encoding, Radix defaultRadix)
			: base(encoding, defaultRadix)
		{
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public SubstitutionParser(Endianness endianness, Encoding encoding, Radix defaultRadix)
			: base(endianness, encoding, defaultRadix)
		{
		}

		/// <summary></summary>
		internal SubstitutionParser(ParserState parserState, Parser parent)
			: base(parserState, parent)
		{
			this.substitution = ((SubstitutionParser)parent).substitution;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
				}
			}

			base.Dispose(disposing);
		}

		#endregion

		#endregion

		#region Factory
		//==========================================================================================
		// Factory
		//==========================================================================================

		/// <summary></summary>
		internal override Parser GetParser(ParserState parserState, Parser parent)
		{
			AssertNotDisposed();

			return (new SubstitutionParser(parserState, parent));
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual byte[] Parse(string s, CharSubstitution substitution)
		{
			// AssertNotDisposed() is called by 'Parse()' below.

			this.substitution = substitution;
			return (Parse(s));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool TryParse(string s, CharSubstitution substitution, out byte[] result)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			string successfullyParsed;
			return (TryParse(s, substitution, out result, out successfullyParsed));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool TryParse(string s, CharSubstitution substitution, Modes modes, out byte[] result)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			string successfullyParsed;
			return (TryParse(s, substitution, modes, out result, out successfullyParsed));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool TryParse(string s, CharSubstitution substitution, out byte[] result, out string successfullyParsed)
		{
			// AssertNotDisposed() is called by 'base.TryParse()' below.

			this.substitution = substitution;
			return (TryParse(s, out result, out successfullyParsed));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool TryParse(string s, CharSubstitution substitution, Modes modes, out byte[] result, out string successfullyParsed)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			this.substitution = substitution;
			return (TryParse(s, modes, out result, out successfullyParsed));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool TryParse(string s, CharSubstitution substitution, Modes modes, out Result[] result, out string successfullyParsed)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			this.substitution = substitution;
			return (TryParse(s, modes, out result, out successfullyParsed));
		}

		/// <summary></summary>
		internal override bool TryParseContiguousRadixItem(string item, Radix radix, out byte[] result, ref FormatException formatException)
		{
			// AssertNotDisposed() is called by 'base.TryParseContiguousRadixItem()' below.

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
