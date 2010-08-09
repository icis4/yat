//==================================================================================================
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.Test/SettingsCollection.cs $
// $Author: maettu_this $
// $Date: 2010-04-27 20:05:33 +0200 (Di, 27 Apr 2010) $
// $Revision: 294 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace MKY.Utilities.Collections
{
	/// <summary>
	/// Serializable string key/value-pair.
	/// </summary>
	/// <remarks>
	/// Required because <see cref="T:KeyValuePair`1"/> doesn't properly serialize
	/// its contents even though it is marked <see cref="SerializableAttribute"/>.
	/// </remarks>
	[Serializable]
	public struct StringKeyValuePair
	{
		private string key;
		private string value;

		/// <summary>
		/// Initializes a new instance of the <see cref="StringKeyValuePair"/> struct.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public StringKeyValuePair(string key, string value)
		{
			this.key = key;
			this.value = value;
		}

		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>The key.</value>
		[XmlElement("Key")]
		public string Key
		{
			get { return (this.key); }
			set { this.key = value;  }
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		[XmlElement("Value")]
		public string Value
		{
			get { return (this.value); }
			set { this.value = value;  }
		}
	}
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.Test/SettingsCollection.cs $
//==================================================================================================
