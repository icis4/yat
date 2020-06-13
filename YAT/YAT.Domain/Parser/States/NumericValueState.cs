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

using System;
using System.Globalization;
using System.IO;

using MKY;

#endregion

// This code is intentionally placed into the parser base namespace even though the file is located
// in a "\States" sub-directory. The sub-directory shall only group the implementation but not open
// another namespace. The state classes already contain "State" in their name.
namespace YAT.Domain.Parser
{
	/// <summary>
	/// This state handles a sequence of numeric values in one of the supported radices. The
	/// sequence may consist of any number of subsequent values. The state terminates as soon as
	/// a non-supported character is found.
	/// </summary>
	/// <remarks>
	/// +/- signs are not allowed, neither are decimal points nor separators such as the apostrophe.
	/// </remarks>
	internal class NumericValueState : ParserState
	{
		private StringWriter valueWriter;

		/// <summary></summary>
		public NumericValueState()
			: this(CharEx.InvalidChar)
		{
		}

		/// <summary></summary>
		public NumericValueState(int parseChar)
		{
			this.valueWriter = new StringWriter(CultureInfo.InvariantCulture);

			if (parseChar != CharEx.InvalidChar)
				this.valueWriter.Write((char)parseChar);
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
				if (this.valueWriter != null) {
					this.valueWriter.Dispose();
					this.valueWriter = null;
				}
			}

			base.Dispose(disposing);
		}

		#endregion

		/// <summary></summary>
		public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
		{
			AssertUndisposed();

			switch (parser.Radix)
			{
				case Radix.Bin:
				{
					if ((parseChar >= '0') && (parseChar <= '1'))
					{
						this.valueWriter.Write((char)parseChar);
						return (true);
					}
					break;
				}

				case Radix.Oct:
				{
					if ((parseChar >= '0') && (parseChar <= '7'))
					{
						this.valueWriter.Write((char)parseChar);
						return (true);
					}
					break;
				}

				case Radix.Dec:
				{
					if ((parseChar >= '0') && (parseChar <= '9'))
					{
						this.valueWriter.Write((char)parseChar);
						return (true);
					}
					break;
				}

				case Radix.Hex:
				case Radix.Unicode:
				{
					if (((parseChar >= '0') && (parseChar <= '9')) ||
						((parseChar >= 'A') && (parseChar <= 'F')) ||
						((parseChar >= 'a') && (parseChar <= 'f')))
					{
						this.valueWriter.Write((char)parseChar);
						return (true);
					}
					break;
				}

				default:
				{
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + parser.Radix + "' radix is not supported for numeric values!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}

			// No more valid character found, try to process numeric value:
			byte[] result;
			if (parser.TryParseAndConvertContiguousNumericItem(this.valueWriter.ToString(), out result, ref formatException))
			{
				foreach (byte b in result)
					parser.BytesWriter.WriteByte(b);

				parser.CommitPendingBytes();

				parser.HasFinished = true;
				ChangeState(parser, null);
				return (false); // Return 'false' to indicate that the current 'parseChar' has not been processed yet!
			}
			else
			{
				parser.HasFinished = true;
				ChangeState(parser, null);
				return (false); // Return 'false' to indicate that the current 'parseChar' has not been processed yet!
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
