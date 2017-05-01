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

using System.Globalization;
using System.IO;

// This code is intentionally placed into the parser base namespace even though the file is located
// in a "\States" sub-directory. The sub-directory shall only group the implementation but not open
// another namespace. The state classes already contain "State" in their name.
namespace YAT.Domain.Parser
{
	/// <summary>
	/// This state is the default, it handles contiguous sequences. The state terminates when
	/// entering one of the other states.
	/// </summary>
	internal class DefaultState : ParserState
	{
		private StringWriter contiguousWriter;

		/// <summary></summary>
		public DefaultState()
		{
			this.contiguousWriter = new StringWriter(CultureInfo.InvariantCulture);
		}

		#region Disposal
		//--------------------------------------------------------------------------------------
		// Disposal
		//--------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					if (this.contiguousWriter != null)
						this.contiguousWriter.Dispose();
				}

				// Set state to disposed:
				this.contiguousWriter = null;
			}

			base.Dispose(disposing);
		}

		#endregion

		/// <summary></summary>
		public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
		{
			AssertNotDisposed();

			if ((parseChar < 0) ||                                 // End of parse string.
				(parseChar == ')' && !parser.IsTopLevel))
			{
				if (!TryWriteContiguous(parser, ref formatException))
					return (false);

				parser.HasFinished = true;
				ChangeState(parser, null);
			}
			else if (parseChar == '\\' && !parser.IsKeywordParser) // Escape sequence.
			{
				if (!TryWriteContiguous(parser, ref formatException))
					return (false);

				// Commit bytes, create nested parser and then change state:
				parser.CommitPendingBytes();
				parser.NestedParser = parser.GetNestedParser(new EscapeState());
				ChangeState(parser, new NestedState());
			}
			else if (parseChar == '<' && !parser.IsKeywordParser) // ASCII mnemonic sequence.
			{
				if (!TryWriteContiguous(parser, ref formatException))
					return (false);

				// Commit bytes, create nested parser and then change state:
				parser.CommitPendingBytes();
				parser.NestedParser = parser.GetNestedParser(new AsciiMnemonicState());
				ChangeState(parser, new NestedState());
			}
			else if (parseChar == '(' && parser.IsKeywordParser) // Keyword args.
			{
				KeywordResult result;

				if (!TryParseContiguousToKeyword(parser, out result, ref formatException))
					return (false);

				// Nothing to commit yet, create nested parser and then change state:
				parser.NestedParser = parser.GetNestedParser(new KeywordArgState(result.Keyword));
				ChangeState(parser, new NestedState());
			}
			else                                                 // Compose contiguous string.
			{
				this.contiguousWriter.Write((char)parseChar);

				if (parser.DoProbe)
				{   // Continuously probe the contiguous string to provide better error message:
					if (!ProbeContiguous(parser, ref formatException))
						return (false);
				}
			}

			return (true);
		}

		private bool TryWriteContiguous(Parser parser, ref FormatException formatException)
		{
			return (DoHandleContiguous(parser, ref formatException, true));
		}

		private bool ProbeContiguous(Parser parser, ref FormatException formatException)
		{
			return (DoHandleContiguous(parser, ref formatException, false));
		}

		private bool DoHandleContiguous(Parser parser, ref FormatException formatException, bool writeOnSuccess)
		{
			if (!parser.IsKeywordParser)
			{
				byte[] result;

				if (!TryParseContiguousToRadix(parser, out result, ref formatException))
					return (false);

				if (writeOnSuccess && (result != null) && (result.Length > 0))
				{
					foreach (byte b in result)
						parser.BytesWriter.WriteByte(b);

					parser.CommitPendingBytes();
				}
			}
			else // IsKeywordParser
			{
				KeywordResult result;

				if (!TryParseContiguousToKeyword(parser, out result, ref formatException))
					return (false);

				if (writeOnSuccess && (result != null) && (result.Keyword != Keyword.None))
					parser.CommitResult(result);
			}

			return (true);
		}

		private bool TryParseContiguousToRadix(Parser parser, out byte[] result, ref FormatException formatException)
		{
			result = null;

			var contiguousString = this.contiguousWriter.ToString();
			if (contiguousString.Length > 0)
			{
				if (!parser.TryParseContiguousRadix(contiguousString, parser.Radix, out result, ref formatException))
					return (false);
			}

			return (true);
		}

		private bool TryParseContiguousToKeyword(Parser parser, out KeywordResult result, ref FormatException formatException)
		{
			result = null;

			var contiguousString = this.contiguousWriter.ToString();
			if (contiguousString.Length > 0)
			{
				if (!parser.TryParseKeyword(contiguousString, out result, ref formatException))
					return (false);
			}

			return (true);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
