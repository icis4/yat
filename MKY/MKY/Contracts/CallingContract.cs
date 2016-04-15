﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.Contracts
{
	/// <summary>
	/// Attribute to indicate the nature of a function call.
	/// </summary>
	/// <remarks>
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Delegate, AllowMultiple = true)]
	public sealed class CallingContractAttribute : Attribute
	{
		private bool isAlwaysMainThread;
		private bool isNeverMainThread;
		private bool isAlwaysSequential;
		private string isAlwaysSequentialIncluding;
		private string rationale;

		/// <summary>
		/// Indicates that the function call is always performed on the main thread of the application.
		/// </summary>
		public bool IsAlwaysMainThread
		{
			get { return (this.isAlwaysMainThread); }
			set
			{
				this.isAlwaysMainThread =  value;
				this.isNeverMainThread  = !value;
			}
		}

		/// <summary>
		/// Indicates that the function call is never performed on the main thread of the application.
		/// </summary>
		public bool IsNeverMainThread
		{
			get { return (this.isNeverMainThread); }
			set
			{
				this.isAlwaysMainThread = !value;
				this.isNeverMainThread  =  value;
			}
		}

		/// <summary>
		/// Indicates that the function call is always performed sequential, that is the caller
		/// ensures that no race condition may occur.
		/// </summary>
		public bool IsAlwaysSequential
		{
			get { return (this.isAlwaysSequential); }
			set { this.isAlwaysSequential = value;  }
		}

		/// <summary>
		/// Indicates that the function call is always performed sequential, that is the caller
		/// ensures that no race condition may occur, and the call is also sequentially synchronized
		/// with the given delegate.
		/// </summary>
		public string IsAlwaysSequentialIncluding
		{
			get { return (this.isAlwaysSequentialIncluding); }
			set
			{
				this.isAlwaysSequential = true;
				this.isAlwaysSequentialIncluding = value;
			}
		}

		/// <summary>
		/// The rationale for the calling contract.
		/// </summary>
		public string Rationale
		{
			get { return (this.rationale); }
			set { this.rationale = value;  }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
