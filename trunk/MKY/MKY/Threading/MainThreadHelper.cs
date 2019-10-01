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
// MKY Version 1.0.27
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Threading;

namespace MKY.Threading
{
	/// <summary>
	/// Helper to deal with the applications main thread.
	/// </summary>
	public static class MainThreadHelper
	{
		/// <summary>
		/// The value that identifies an invalid thread identifier.
		/// </summary>
		public const int InvalidThreadId = -1;

		private static int staticMainThreadId = InvalidThreadId;

		/// <summary>
		/// Returns the main thread ID if <see cref="SetCurrentThread"/> has been called before;
		/// otherwise, <see cref="InvalidThreadId"/>.
		/// </summary>
		public static int MainThreadId
		{
			get { return (staticMainThreadId); }
		}

		/// <summary>
		/// Sets the current thread as the main thread.
		/// </summary>
		public static void SetCurrentThread()
		{
			staticMainThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		/// <summary>
		/// Returns whether this helper has been fully initialized, i.e. the main thread has been
		/// set by calling <see cref="SetCurrentThread"/>.
		/// </summary>
		/// <value>
		/// <c>true</c> if the helper has been fully initialized; otherwise, <c>false</c>.
		/// </value>
		public static bool IsInitialized
		{
			get
			{
				return (staticMainThreadId != InvalidThreadId);
			}
		}

		/// <summary>
		/// Returns whether the current thread is the main thread if <see cref="SetCurrentThread"/>
		/// has been called before; otherwise, <c>false</c>.
		/// </summary>
		/// <value>
		/// <c>true</c> if the current thread is the main thread; otherwise, <c>false</c>.
		/// </value>
		public static bool IsMainThread
		{
			get
			{
				if (staticMainThreadId != InvalidThreadId)
					return (Thread.CurrentThread.ManagedThreadId == staticMainThreadId);
				else
					return (false);

				// Do neither throw an exception nor output a debug message in case the ID is still
				// 'InvalidThreadId', because of the following reasons:
				//  > An application may not have any reason to use this helper, but other utilities
				//    such as the 'EventHelper' may still access this property.
				//  > An application may call 'SetCurrentThread()' in the "normal" case, but it
				//    may not be called by unit or sub-system test code.
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
