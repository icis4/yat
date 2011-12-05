//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
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
// Copyright © 2010-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;

namespace MKY.CommandLine
{
	/// <summary>
	/// Attribute to mark an option argument.
	/// </summary>
	/// <remarks>
	/// This class is based on the NUnit command line infrastructure.
	/// See <see cref="ArgsHandler"/> for details.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field)]
	public class OptionArgAttribute : Attribute
	{
		private string[] names;
		private string[] shortNames;
		private string description;

		/// <summary>
		/// Gets or sets one or more names.
		/// </summary>
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
				StringBuilder singleLine = new StringBuilder();
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
				StringBuilder singleLine = new StringBuilder();
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
