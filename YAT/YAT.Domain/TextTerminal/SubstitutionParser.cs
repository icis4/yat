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

using System.Text;

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
		public SubstitutionParser(Endianess endianess)
			: base(endianess)
		{
		}

		/// <summary></summary>
		public SubstitutionParser(Encoding encoding)
			: base(encoding)
		{
		}

		/// <summary></summary>
		public SubstitutionParser(Endianess endianess, Encoding encoding)
			: base(endianess, encoding)
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
		public SubstitutionParser(Endianess endianess, Encoding encoding, Radix defaultRadix)
			: base(endianess, encoding, defaultRadix)
		{
		}

		/// <summary></summary>
		protected SubstitutionParser(ParserState parserState, Parser parser)
			: base(parserState, parser)
		{
			this.substitution = ((SubstitutionParser)parser).substitution;
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Nothing to do (yet).
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
		protected override Parser GetParser(ParserState parserState, Parser parser)
		{
			AssertNotDisposed();

			SubstitutionParser p = new SubstitutionParser(parserState, parser);
			return (p);
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual byte[] Parse(string s, CharSubstitution substitution)
		{
			// AssertNotDisposed() is called by base.Parse().

			string parsed;
			return (Parse(s, substitution, out parsed));
		}

		/// <summary></summary>
		public virtual byte[] Parse(string s, CharSubstitution substitution, out string parsed)
		{
			// AssertNotDisposed() is called by base.Parse().

			this.substitution = substitution;
			return (Parse(s, out parsed));
		}

		/// <summary></summary>
		public virtual Result[] Parse(string s, CharSubstitution substitution, ParseMode mode)
		{
			// AssertNotDisposed() is called by base.Parse().

			string parsed;
			return (Parse(s, substitution, mode, out parsed));
		}

		/// <summary></summary>
		public virtual Result[] Parse(string s, CharSubstitution substitution, ParseMode mode, out string parsed)
		{
			// AssertNotDisposed() is called by base.Parse().

			this.substitution = substitution;
			return (Parse(s, mode, out parsed));
		}

		/// <summary></summary>
		public virtual bool TryParse(string s, CharSubstitution substitution, out byte[] result)
		{
			// AssertNotDisposed() is called by base.TryParse().

			string parsed;
			return (TryParse(s, substitution, out result, out parsed));
		}

		/// <summary></summary>
		public virtual bool TryParse(string s, CharSubstitution substitution, out byte[] result, out string parsed)
		{
			// AssertNotDisposed() is called by base.TryParse().

			this.substitution = substitution;
			return (TryParse(s, out result, out parsed));
		}

		/// <summary></summary>
		protected override bool TryParseContiguousRadixToken(string token, Radix parseRadix, out byte[] result, ref FormatException formatException)
		{
			// AssertNotDisposed() is called by base.TryParseContiguousRadixToken().

			return (base.TryParseContiguousRadixToken(Substitute(token), parseRadix, out result, ref formatException));
		}

		private string Substitute(string token)
		{
			switch (this.substitution)
			{
				case CharSubstitution.ToUpper: return (token.ToUpper());
				case CharSubstitution.ToLower: return (token.ToLower());
				default: return (token);
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
