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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
	/// https://docs.microsoft.com/en-us/archive/blogs/blambert/a-simple-and-totally-thread-safe-implementation-of-idisposable
	///
	/// Saying hello to StyleCop ;-.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
	public abstract class DisposableBase : IDisposable, IDisposableEx
	{
		/// <summary>
		/// A value which indicates the disposable state.
		/// <list type="bullet">
		/// <item><description>0 indicates undisposed.</description></item>
		/// <item><description>1 indicates disposal is ongoing or has completed.</description></item>
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Undisposed", Justification = "See remarks.")]
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
		[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "This method does call Dispose(true) and then GC.SuppressFinalize(this), just a bit more clever than the basic IDisposable pattern.")]
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
			DebugMessage("Finalizing...");

			DebugEventManagement.DebugWriteAllEventRemains(this);

			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);

			DebugMessage("...successfully finalized.");
		}

	#endif // DEBUG

		/// <summary>
		/// Asserts that disposal of object is neither ongoing nor has already completed.
		/// </summary>
		/// <remarks>
		/// Not named "AssertIsUndisposed" as that sounds more like "check whether assert is undisposed".
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Undisposed", Justification = "See remarks at 'IsUndisposed'.")]
		protected virtual void AssertUndisposed()
		{
			if (!IsUndisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object is being or has already been disposed!"));
		}

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[Conditional("DEBUG")]
		private void DebugMessage(string format, params object[] args)
		{
			DebugMessage(string.Format(CultureInfo.CurrentCulture, format, args));
		}

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		private void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					GetType(),
					"",
					"",
					message
				)
			);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
