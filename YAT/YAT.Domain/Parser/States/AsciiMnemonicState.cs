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
// YAT Version 2.1.1 Development
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

using MKY;
using MKY.Text;

#endregion

// This code is intentionally placed into the parser base namespace even though the file is located
// in a "\States" sub-directory. The sub-directory shall only group the implementation but not open
// another namespace. The state classes already contain "State" in their name.
namespace YAT.Domain.Parser
{
	/// <summary>
	/// This state handles a sequence of ASCII mnemonics. The sequence may consist of any number
	/// of subsequent characters that form valid ASCII mnemonics. The state terminates with the
	/// closing '>'.
	/// </summary>
	internal class AsciiMnemonicState : ParserState
	{
		private StringWriter mnemonicWriter;

		/// <summary></summary>
		public AsciiMnemonicState()
		{
			this.mnemonicWriter = new StringWriter(CultureInfo.InvariantCulture);
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
					if (this.mnemonicWriter != null)
						this.mnemonicWriter.Dispose();
				}

				// Set state to disposed:
				this.mnemonicWriter = null;
			}

			base.Dispose(disposing);
		}

		#endregion

		/// <summary></summary>
		public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
		{
			AssertNotDisposed();

			if ((parseChar < 0) || (parseChar == '>')) // Process finished mnemonic string.
			{
				byte[] a;

				if (!TryParseContiguousAsciiMnemonic(parser, this.mnemonicWriter.ToString(), out a, ref formatException))
					return (false);

				foreach (byte b in a)
					parser.BytesWriter.WriteByte(b);

				parser.CommitPendingBytes();

				parser.HasFinished = true;
				ChangeState(parser, null);
			}
			else                                       // Compose contiguous string.
			{
				this.mnemonicWriter.Write((char)parseChar);
			}
			return (true);
		}

		/// <summary>
		/// Parses <paramref name="s"/> for ASCII mnemonics. <paramref name="s"/> will
		/// sequentially be parsed and converted mnemonics-by-mnemonics.
		/// </summary>
		/// <param name="parser">The parser to be used.</param>
		/// <param name="s">String to be parsed.</param>
		/// <param name="result">Array containing the resulting bytes.</param>
		/// <param name="formatException">Returned if invalid string format.</param>
		/// <returns>Byte array containing the values encoded in Encoding.Default.</returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
		private static bool TryParseContiguousAsciiMnemonic(Parser parser, string s, out byte[] result, ref FormatException formatException)
		{
			using (var bytes = new MemoryStream())
			{
				foreach (string item in s.Split())
				{
					if (item.Length > 0)
					{
						byte[] a;
						if (TryParseContiguousAsciiMnemonicItem(parser, item, out a, ref formatException))
						{
							bytes.Write(a, 0, a.Length);
						}
						else
						{
							result = null;
							return (false);
						}
					}
				}

				result = bytes.ToArray();
				return (true);
			} // using (MemoryStream)
		}

		/// <summary>
		/// Parses <paramref name="s"/> for ASCII mnemonics. <paramref name="s"/> will
		/// sequentially be parsed and converted mnemonics-by-mnemonics.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for recursion.")]
		private static bool TryParseContiguousAsciiMnemonicItem(Parser parser, string s, out byte[] result, ref FormatException formatException)
		{
			using (var bytes = new MemoryStream())
			{
				string remaining = s;
				bool success = true;

				while (remaining.Length > 0)
				{
					bool found = false;

					int from = Math.Min(Ascii.MnemonicMaxLength, remaining.Length);
					for (int i = from; i >= Ascii.MnemonicMinLength; i--) // Probe the max..min left-most characters for a valid ASCII mnemonic.
					{
						byte code;
						if (Ascii.TryParse(StringEx.Left(remaining, i), out code))
						{
							char c = Convert.ToChar(code);
							byte[] a = parser.GetBytes(c);
							bytes.Write(a, 0, a.Length);

							remaining = remaining.Remove(0, i);
							found = true;
							break; // Quit for-loop and continue within remaining string.
						}
					}

					if (!found)
					{
						success = false;
						break; // Quit while-loop.
					}
				}

				if (success)
				{
					result = bytes.ToArray();
					return (true);
				}
				else
				{
					var sb = new StringBuilder();

					if (remaining.Length != s.Length)
					{
						sb.Append(@"""");
						sb.Append(remaining);
						sb.Append(@""" of ");
					}

					sb.Append(@"""");
					sb.Append(s);
					sb.Append(@""" is an invalid ASCII mnemonic.");

					formatException = new FormatException(sb.ToString());
					result = null;
					return (false);
				}
			} // using (MemoryStream)
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
