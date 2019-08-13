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
// MKY Version 1.0.19
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
using System.Diagnostics.CodeAnalysis;

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
		private int count;

		/// <summary></summary>
		public bool IsSettingControls
		{
			get
			{
				return (this.count > 0); // No need to use 'Interlocked.Read()' as access to
			}                            // 'Windows.Forms' must be synchronized anyway.
		}

		/// <summary></summary>
		public void Enter()
		{
			this.count++; // No need to use 'Interlocked.Increment()' as access to
		}                 // 'Windows.Forms' must be synchronized anyway.

		/// <summary></summary>
		public void Leave()
		{
			this.count--; // No need to use 'Interlocked.Decrement()' as access to
		                  // 'Windows.Forms' must be synchronized anyway.
			if (this.count < 0)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'SettingControlsHelper' count has fallen below 0!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Special operator for much easier use of this helper class.")]
		public static implicit operator bool(SettingControlsHelper isSettingControls)
		{
			return (isSettingControls.IsSettingControls);
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
			return (this.count);
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
			return (this.count.Equals(other.count));
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