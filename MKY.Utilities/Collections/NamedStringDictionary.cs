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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MKY.Utilities.Collections
{
	/// <summary>
	/// A collection containing one or more sets of test settings.
	/// </summary>
	[Serializable]
	public class NamedStringDictionary
	{
		private string name;
		private Dictionary<string, string> dictionary;

		/// <summary>
		/// Initializes a new instance of the <see cref="NamedStringDictionary"/> class.
		/// </summary>
		public NamedStringDictionary()
			: this("")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NamedStringDictionary"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public NamedStringDictionary(string name)
		{
			this.name = name;
			this.dictionary = new Dictionary<string, string>();
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[XmlElement("Name")]
		public string Name
		{
			get { return (this.name); }
			set { this.name = value;  }
		}

		/// <summary>
		/// Gets or sets the settings.
		/// </summary>
		/// <value>The settings.</value>
		[XmlElement("Settings")]
		public List<StringKeyValuePair> Settings
		{
			get
			{
				List<StringKeyValuePair> l = new List<StringKeyValuePair>(dictionary.Count);
				if (dictionary != null)
				{
					foreach (KeyValuePair<string, string> kvp in dictionary)
						l.Add(new StringKeyValuePair(kvp.Key, kvp.Value));
				}
				return (l);
			}
			set
			{
				dictionary.Clear();

				if (value != null)
				{
					foreach (StringKeyValuePair kvp in value)
						dictionary.Add(kvp.Key, kvp.Value);
				}
			}
		}

		/// <summary>
		/// Gets the keys.
		/// </summary>
		/// <value>The keys.</value>
		[XmlIgnore]
		public Dictionary<string, string>.KeyCollection Keys
		{
			get { return (this.dictionary.Keys); }
		}

		/// <summary>
		/// Gets the <see cref="System.String"/> with the specified key.
		/// </summary>
		[XmlIgnore]
		public string this[string key]
		{
			get { return (this.dictionary[key]); }
			set { this.dictionary[key] = value;  }
		}

		/// <summary>
		/// Clears all settings.
		/// </summary>
		public void Clear()
		{
			dictionary.Clear();
		}

		/// <summary>
		/// Adds the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void Add(string key, string value)
		{
			this.dictionary.Add(key, value);
		}

		/// <summary>
		/// Determines whether the specified key contains key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsKey(string key)
		{
			return (this.dictionary.ContainsKey(key));
		}

		/// <summary>
		/// Tries the get value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		/// <c>true</c> if the value at the specified key could be retrieved; otherwise, <c>false</c>.
		/// </returns>
		public bool TryGetValue(string key, out string value)
		{
			return (this.dictionary.TryGetValue(key, out value));
		}
	}
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.Test/SettingsCollection.cs $
//==================================================================================================
