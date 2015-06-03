//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.12
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

namespace MKY.Contracts
{
	/// <summary>
	/// Attribute to indicate the nature of a function call.
	/// </summary>
	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Delegate)]
	public sealed class CallingContractAttribute : Attribute
	{
		private bool isMainThread;
		private bool isSequential;

		/// <summary>
		/// Indicates that the function call is always performed on the main thread of the application.
		/// </summary>
		public bool IsAlwaysMainThread
		{
			get { return (this.isMainThread); }
			set { this.isMainThread = value;  }
		}

		/// <summary>
		/// Indicates that the function call is never performed on the main thread of the application.
		/// </summary>
		public bool IsNeverMainThread
		{
			get { return (!this.isMainThread); }
			set { this.isMainThread = !value;  }
		}

		/// <summary>
		/// Indicates that the function call is always performed sequential, that is the caller
		/// ensures that no race condition may occur.
		/// </summary>
		public bool IsAlwaysSequential
		{
			get { return (this.isSequential); }
			set { this.isSequential = value;  }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
