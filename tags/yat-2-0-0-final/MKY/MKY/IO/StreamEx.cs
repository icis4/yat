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
// MKY Version 1.0.25
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MKY.IO
{
	/// <summary>
	/// Utility methods for <see cref="Stream"/>.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class StreamEx
	{
		/// <summary>
		/// Result when reading the stream's end.
		/// </summary>
		/// <remarks>
		/// Value corresponds to the value returned by <see cref="Stream.ReadByte()"/>
		/// and the other read functions if no more characters can be read from the stream.
		/// Value also corresponds to <see cref="CharEx.InvalidChar"/>.
		/// </remarks>
		public const int EndOfStream = -1;

		/// <summary>
		/// Result when stream length cannot be retrieved from a stream,
		/// e.g. if the stream is not seekable.
		/// </summary>
		public const long UnknownLength = -1;

		/// <summary>
		/// Returns the true length of available data in a stream.
		/// </summary>
		public static long RemainingLength(Stream stream)
		{
			if (stream.CanSeek)
				return (stream.Length - stream.Position);
			else
				return (UnknownLength);
		}

		/// <summary>
		/// Returns whether the given stream contains more data.
		/// </summary>
		public static bool HasRemainingData(Stream stream)
		{
			return (RemainingLength(stream) > 0);
		}

		/// <summary>
		/// Returns whether the given stream contains at least the given amount of data.
		/// </summary>
		public static bool AtLeastRemaining(Stream stream, int size)
		{
			return (RemainingLength(stream) >= size);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
