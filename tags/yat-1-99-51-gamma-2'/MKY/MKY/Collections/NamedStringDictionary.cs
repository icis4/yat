//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.15
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace MKY.Collections
{
	/// <summary>
	/// A collection containing one or more sets of test settings.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Class actually implements a dictionary, but not using inheritance.")]
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
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("Settings")]
		public List<StringKeyValuePair> Settings
		{
			get
			{
				List<StringKeyValuePair> l = new List<StringKeyValuePair>(this.dictionary.Count);
				if (this.dictionary != null)
				{
					foreach (KeyValuePair<string, string> kvp in this.dictionary)
						l.Add(new StringKeyValuePair(kvp.Key, kvp.Value));
				}
				return (l);
			}
			set
			{
				this.dictionary.Clear();

				if (value != null)
				{
					foreach (StringKeyValuePair kvp in value)
						this.dictionary.Add(kvp.Key, kvp.Value);
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
			this.dictionary.Clear();
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
// $URL$
//==================================================================================================
