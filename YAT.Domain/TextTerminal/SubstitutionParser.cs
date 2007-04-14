using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HSR.YAT.Domain.Parser
{
	/// <summary>
	/// Extends Parser with character substitution.
	/// </summary>
	public class SubstitutionParser : Parser
	{
		private CharSubstitution _substitution;

		public SubstitutionParser()
			: base()
		{
		}

		public SubstitutionParser(Encoding encoding)
			: base(encoding)
		{
		}

		public SubstitutionParser(Radix defaultRadix)
			: base(defaultRadix)
		{
		}

		public SubstitutionParser(Encoding encoding, Radix defaultRadix)
			: base(encoding, defaultRadix)
		{
		}

		protected SubstitutionParser(ParserState parserState, Parser parser)
			: base(parserState, parser)
		{
			_substitution = ((SubstitutionParser)parser)._substitution;
		}

		//------------------------------------------------------------------------------------------
		// Factory
		//------------------------------------------------------------------------------------------

		protected override Parser GetParser(ParserState parserState, Parser parser)
		{
			SubstitutionParser p = new SubstitutionParser(parserState, parser);
			return (p);
		}

		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		public byte[] Parse(string s, CharSubstitution substitution)
		{
			string parsed;
			return (Parse(s, substitution, out parsed));
		}

		public byte[] Parse(string s, CharSubstitution substitution, out string parsed)
		{
			_substitution = substitution;
			return (Parse(s, out parsed));
		}

		public Result[] Parse(string s, CharSubstitution substitution, ParseMode mode)
		{
			string parsed;
			return (Parse(s, substitution, mode, out parsed));
		}

		public Result[] Parse(string s, CharSubstitution substitution, ParseMode mode, out string parsed)
		{
			_substitution = substitution;
			return (Parse(s, mode, out parsed));
		}

		public bool TryParse(string s, CharSubstitution substitution, out byte[] result)
		{
			string parsed;
			return (TryParse(s, substitution, out result, out parsed));
		}

		public bool TryParse(string s, CharSubstitution substitution, out byte[] result, out string parsed)
		{
			_substitution = substitution;
			return (TryParse(s, out result, out parsed));
		}

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
