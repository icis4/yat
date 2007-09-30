using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace YAT.Domain.Parser
{
	/// <summary>
	/// Extends Parser with character substitution.
	/// </summary>
	public class SubstitutionParser : Parser
	{
		private CharSubstitution _substitution;

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
			_substitution = ((SubstitutionParser)parser)._substitution;
		}

		//------------------------------------------------------------------------------------------
		// Factory
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override Parser GetParser(ParserState parserState, Parser parser)
		{
			SubstitutionParser p = new SubstitutionParser(parserState, parser);
			return (p);
		}

		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public byte[] Parse(string s, CharSubstitution substitution)
		{
			string parsed;
			return (Parse(s, substitution, out parsed));
		}

		/// <summary></summary>
		public byte[] Parse(string s, CharSubstitution substitution, out string parsed)
		{
			_substitution = substitution;
			return (Parse(s, out parsed));
		}

		/// <summary></summary>
		public Result[] Parse(string s, CharSubstitution substitution, ParseMode mode)
		{
			string parsed;
			return (Parse(s, substitution, mode, out parsed));
		}

		/// <summary></summary>
		public Result[] Parse(string s, CharSubstitution substitution, ParseMode mode, out string parsed)
		{
			_substitution = substitution;
			return (Parse(s, mode, out parsed));
		}

		/// <summary></summary>
		public bool TryParse(string s, CharSubstitution substitution, out byte[] result)
		{
			string parsed;
			return (TryParse(s, substitution, out result, out parsed));
		}

		/// <summary></summary>
		public bool TryParse(string s, CharSubstitution substitution, out byte[] result, out string parsed)
		{
			_substitution = substitution;
			return (TryParse(s, out result, out parsed));
		}

		/// <summary></summary>
		protected override bool TryParseContiguousRadixToken(string token, Radix parseRadix, out byte[] result, ref FormatException formatException)
		{
			return (base.TryParseContiguousRadixToken(Substitute(token), parseRadix, out result, ref formatException));
		}

		private string Substitute(string token)
		{
			switch (_substitution)
			{
				case CharSubstitution.ToUpper: return (token.ToUpper());
				case CharSubstitution.ToLower: return (token.ToLower());
				default: return (token);
			}
		}
	}
}
