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
// YAT Version 2.4.1
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Globalization;
using System.IO;

#endregion

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

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			// Dispose of managed resources:
			if (disposing)
			{
				if (this.contiguousWriter != null) {
					this.contiguousWriter.Dispose();
					this.contiguousWriter = null;
				}
			}

			base.Dispose(disposing);
		}

		#endregion

		/// <summary></summary>
		public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
		{
			AssertUndisposed();

			if (parseChar < 0)     // End of string to parse.
			{
				if (!TryWriteContiguous(parser, ref formatException))
					return (false);

				parser.HasFinished = true;
				ChangeState(parser, null);
			}                      // End of contiguous string.
			else if (parseChar == ')' && (parser.Mode != Mode.NoEscapes) && !parser.IsTopLevel)
			{
				if (!TryWriteContiguous(parser, ref formatException))
					return (false);

				parser.HasFinished = true;
				ChangeState(parser, null);
			}                      // Escape sequence.
			else if (parseChar == '\\' && (parser.Mode != Mode.NoEscapes))
			{
				if (!TryWriteContiguous(parser, ref formatException))
					return (false);

				// Commit bytes, create nested parser and then change state:
				parser.CommitPendingBytes();
				parser.NestedParser = parser.GetNestedParser(new EscapeState());
				ChangeState(parser, new NestedState());
			}                      // ASCII mnemonic sequence.
			else if (parseChar == '<' && ((parser.Mode & Mode.AsciiEscapes) != 0))
			{
				if (!TryWriteContiguous(parser, ref formatException))
					return (false);

				// Commit bytes, create nested parser and then change state:
				parser.CommitPendingBytes();
				parser.NestedParser = parser.GetNestedParser(new AsciiMnemonicState());
				ChangeState(parser, new NestedState());
			}                       // Keyword args.
			else if (parseChar == '(' && ((parser.Mode & Mode.KeywordEscapes) != 0) && parser.IsKeywordParser)
			{
				KeywordResult result;

				if (!TryParseContiguousToKeyword(parser, out result, ref formatException))
					return (false);

				// Nothing to commit yet, create nested parser and then change state:
				parser.NestedParser = parser.GetNestedParser(new KeywordArgState(result.Keyword));
				ChangeState(parser, new NestedState());
			}
			else                    // Compose contiguous string.
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
