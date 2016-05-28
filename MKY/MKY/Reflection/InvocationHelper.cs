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
using System.Reflection;

namespace MKY.Reflection
{
	/// <summary>
	/// Invocation utility methods.
	/// </summary>
	public static class InvocationHelper
	{
		/// <summary>
		/// Attribute to select a static property of a generic type, i.e. kind of a static interface.
		/// </summary>
		[CLSCompliant(false)]
		[AttributeUsage(AttributeTargets.Class)]
		public sealed class StaticPropertyAttribute : Attribute
		{
			private string propertyName;
			private Type propertyType;

			/// <summary></summary>
			public StaticPropertyAttribute(string propertyName, Type propertyType)
			{
				this.propertyName = propertyName;
				this.propertyType = propertyType;
			}

			/// <summary></summary>
			public string PropertyName
			{
				get { return (this.propertyName); }
				set { this.propertyName = value;  }
			}

			/// <summary></summary>
			public Type PropertyType
			{
				get { return (this.propertyType); }
				set { this.propertyType = value;  }
			}
		}

		/// <summary>
		/// Invokes the given static property.
		/// </summary>
		public static TReturn InvokeStaticPropertyFromAttribute<TReturn>(Type type)
		{
			StaticPropertyAttribute[] atts = (StaticPropertyAttribute[])type.GetCustomAttributes(typeof(StaticPropertyAttribute), true);
			foreach (StaticPropertyAttribute att in atts)
			{
				if (att.PropertyType == typeof(TReturn))
				{
					PropertyInfo pi = type.GetProperty(att.PropertyName, BindingFlags.Static | BindingFlags.Public);
					if (pi != null)
						return ((TReturn)pi.GetValue(null, null)); // 'null' means static.
				}
			}

			return (default(TReturn));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
