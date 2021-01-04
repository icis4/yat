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
// MKY Version 1.0.28 Development
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
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MKY.CommandLine
{
	/// <summary>
	/// Attribute to mark an option argument.
	/// </summary>
	/// <remarks>
	/// This class is based on the NUnit command line infrastructure. See <see cref="ArgsHandler"/> for details.
	/// Sealed to improve performance during reflection on custom attributes according to FxCop:CA1813.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class OptionArgAttribute : Attribute
	{
		private string[] names;
		private string[] shortNames;
		private string description;

		/// <summary>
		/// Gets or sets one or more names.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Public array getter/setter is required for ease of use of this argument handling infrastructure.")]
		public string[] Names
		{
			get { return (this.names); }
			set { this.names = value; }
		}

		/// <summary>
		/// Gets or sets a single name.
		/// </summary>
		public string Name
		{
			get
			{
				if      (this.names == null)
					return ("");
				else if (this.names.Length <= 0)
					return ("");
				else if (this.names.Length == 1)
					return (this.names[0]);

				// else
				var singleLine = new StringBuilder();
				for (int i = 0; i < this.names.Length; i++)
				{
					singleLine.Append(this.names[i]);

					if (i < (this.names.Length - 1))
						singleLine.Append(' ');
				}
				return (singleLine.ToString());
			}

			set
			{
				this.names = new string[] { value };
			}
		}

		/// <summary>
		/// Gets or sets one or more short names.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Public array getter/setter is required for ease of use of this argument handling infrastructure.")]
		public string[] ShortNames
		{
			get { return (this.shortNames); }
			set { this.shortNames = value;  }
		}

		/// <summary>
		/// Gets or sets a single short name.
		/// </summary>
		public string ShortName
		{
			get
			{
				if      (this.shortNames == null)
					return ("");
				else if (this.shortNames.Length <= 0)
					return ("");
				else if (this.shortNames.Length == 1)
					return (this.shortNames[0]);

				// else
				var singleLine = new StringBuilder();
				for (int i = 0; i < this.shortNames.Length; i++)
				{
					singleLine.Append(this.shortNames[i]);

					if (i < (this.shortNames.Length - 1))
						singleLine.Append(' ');
				}
				return (singleLine.ToString());
			}

			set
			{
				this.shortNames = new string[] { value };
			}
		}

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
