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

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using MKY.Threading;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// <see cref="System.Windows.Forms"/> utility methods.
	/// </summary>
	/// <remarks>
	/// Struct instead of class to allow same declaration as if this was just a simple bool.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Boolean just *is* 'bool'...")]
	public struct SettingControlsHelper : IEquatable<SettingControlsHelper>
	{
		private int counter; // = 0;

		/// <summary>
		/// Gets a value indicating whether the parent of this instance is currently setting the
		/// values of its <see cref="Control"/> objects.
		/// </summary>
		/// <value>
		/// <c>true</c> if the parent of this instance is setting controls; otherwise, <c>false</c>.
		/// </value>
		private bool IsSettingControls
		{
			get
			{
				DebugAssertIsMainThread("Property called from non-main thread!");

				return (this.counter > 0); // No need to use 'Interlocked.Read()' as access to
			}                              // 'Windows.Forms' must be synchronized anyway.
		}

		/// <summary>
		/// To be called each time before setting the values of the <see cref="Control"/> objects
		/// of the parent of this instance.
		/// </summary>
		public void Enter()
		{
			DebugAssertIsMainThread("Method called from non-main thread!");

			this.counter++; // No need to use 'Interlocked.Increment()' as access to
		}                   // 'Windows.Forms' must be synchronized anyway.

		/// <summary>
		/// To be called each time after setting the values of the <see cref="Control"/> objects
		/// of the parent of this instance.
		/// </summary>
		public void Leave()
		{
			DebugAssertIsMainThread("Method called from non-main thread!");

			this.counter--; // No need to use 'Interlocked.Decrement()' as access to
			                // 'Windows.Forms' must be synchronized anyway.
			if (this.counter < 0)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Counter has fallen below 0!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Special operator for much easier use of this helper class.")]
		public static implicit operator bool(SettingControlsHelper rhs)
		{
			return (rhs.IsSettingControls);
		}

		[Conditional("Debug")]
		private static void DebugAssertIsMainThread(string message)
		{
			if (MainThreadHelper.IsInitialized)
				Debug.Assert(MainThreadHelper.IsMainThread, message, "This helper requires that it is always called on the main thread, because no thread-synchronization is done.");
		////else
			////There is no way to assert whether this is the main thread.
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			return (this.counter);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is SettingControlsHelper)
				return (Equals((SettingControlsHelper)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SettingControlsHelper other)
		{
			return (this.counter.Equals(other.counter));
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(SettingControlsHelper lhs, SettingControlsHelper rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(SettingControlsHelper lhs, SettingControlsHelper rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
