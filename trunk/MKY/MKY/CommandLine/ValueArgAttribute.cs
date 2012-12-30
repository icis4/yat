﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.CommandLine
{
	/// <summary>
	/// Attribute to mark a value argument.
	/// </summary>
	/// <remarks>
	/// This class is based on the NUnit command line infrastructure. See <see cref="ArgsHandler"/> for details.
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class ValueArgAttribute : Attribute
	{
		private string description;

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		public string Description
		{
			get { return (this.description); }
			set { this.description = value;  }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
