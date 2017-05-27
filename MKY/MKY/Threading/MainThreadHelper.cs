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
// MKY Development Version 1.0.18
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

using System;
using System.Threading;

using MKY.Diagnostics;

namespace MKY.Threading
{
	/// <summary>
	/// Helper to deal with the applications main thread.
	/// </summary>
	public static class MainThreadHelper
	{
		private const int InvalidThreadId = -1;
		private static int staticMainThreadId = InvalidThreadId;

		/// <summary>
		/// Sets the current thread as the main thread.
		/// </summary>
		public static void SetCurrentThread()
		{
			staticMainThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		/// <summary>
		/// Returns whether the current thread is the main thread.
		/// </summary>
		/// <value>
		/// <c>true</c> if the current thread is the main thread; otherwise, <c>false</c>.
		/// </value>
		/// <exception cref="InvalidOperationException">
		/// Thrown if <see cref="SetCurrentThread"/> has not been called before getting this property.
		/// </exception>
		public static bool IsMainThread
		{
			get
			{
				if (staticMainThreadId != InvalidThreadId)
				{
					return (Thread.CurrentThread.ManagedThreadId == staticMainThreadId);
				}
				else
				{
					DebugEx.WriteStack(typeof(MainThreadHelper), "'MainThreadHelper.SetCurrentThread()' should be called before 'MainThreadHelper.IsMainThread' can be evaluated.");
					return (false);
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
