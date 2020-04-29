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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Threading;

using MKY.Diagnostics;

namespace MKY
{
	/// <summary>
	/// Provides a thread-safe base implementation of the standard <see cref="IDisposable"/>
	/// as well as the extended <see cref="IDisposableEx"/> interfaces.
	/// </summary>
	/// <remarks>
	/// Based on Brian Lambert's "A simple and totally thread-safe implementation of IDisposable".
	/// </remarks>
	public abstract class DisposableBase : IDisposable, IDisposableEx
	{
		/// <summary>
		/// A value which indicates the disposable state.
		/// <list type="bullet">
		/// <item><description>0 indicates undisposed.</description></item>
		/// <item><description>1 indicates disposal is ongoing or completely disposed.</description></item>
		/// </list>
		/// </summary>
		/// <remarks>
		/// <c>int</c> rather than <c>bool</c> is required for thread-safe operations.
		/// </remarks>
		private int disposableState;

		/// <summary>
		/// Gets a value indicating whether disposal of object is neither ongoing nor has completed.
		/// </summary>
		/// <remarks>
		/// Being aware that "undisposed" is an uncommon term, it indeed seems the best matching
		/// because a) it contains "dispose" and thus links to <see cref="IDisposable"/> and
		/// <see cref="IDisposable.Dispose"/> and b) does not mislead to be limited to "completed"
		/// as "IsDisposed" would. Also note <see cref="System.Windows.Forms.Control.Disposing"/>
		/// and <see cref="System.Windows.Forms.Control.IsDisposed"/> which explicitly indicate the
		/// state of the disposal sequence, i.e. only either or can be <c>true</c>.
		/// </remarks>
		public bool IsUndisposed
		{
			get { return (Thread.VolatileRead(ref this.disposableState) == 0); }
		}

		/// <summary>
		/// Gets a value indicating whether disposal of object is either ongoing or has completed.
		/// </summary>
		/// <remarks>
		/// This is a convenience property to prevent hardly comprehensive and thus potentially
		/// error-prone code like <code>if (!IsUndisposed)</code>.
		/// </remarks>
		public bool IsInDisposal
		{
			get { return (!IsUndisposed); }
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing or releasing resources.
		/// </summary>
		public void Dispose()
		{
			// Attempt to move the disposable state from 0 to 1. If successful, we can be assured
			// that this thread is the first thread to do so, and can safely dispose of the object.
			if (Interlocked.CompareExchange(ref this.disposableState, 1, 0) == 0)
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose()"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected abstract void Dispose(bool disposing);

	#if (DEBUG)

		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		///
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		///
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~DisposableBase()
		{
			DebugEventManagement.DebugWriteAllEventRemains(this);

			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}

	#endif // DEBUG

		/// <summary>
		/// Asserts that disposal of object is neither ongoing nor has already completed.
		/// </summary>
		/// <remarks>
		/// Not named "AssertIsUndisposed" as that sounds more like "check whether assert is undisposed".
		/// </remarks>
		protected virtual void AssertUndisposed()
		{
			if (!IsUndisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object is being or has already been disposed!"));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
