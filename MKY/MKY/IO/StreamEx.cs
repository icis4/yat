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
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.IO;

namespace MKY.IO
{
	/// <summary>
	/// Utility methods for <see cref="System.IO.Stream"/>.
	/// </summary>
	public static class StreamEx
	{
		/// <summary>
		/// Result when reading the stream's end.
		/// </summary>
		public const int EndOfStream = -1;

		/// <summary>
		/// Result when stream length cannot be retrieved from a stream, e.g. if the stream is not seekable.
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
