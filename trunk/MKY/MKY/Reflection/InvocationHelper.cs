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
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize scope.")]
		[CLSCompliant(false)]
		[AttributeUsage(AttributeTargets.Class)]
		public sealed class StaticPropertyAttribute : Attribute
		{
			/// <summary></summary>
			public string PropertyName { get; }

			/// <summary></summary>
			public Type PropertyType { get; }

			/// <summary></summary>
			public StaticPropertyAttribute(string propertyName, Type propertyType)
			{
				PropertyName = propertyName;
				PropertyType = propertyType;
			}
		}

		/// <summary>
		/// Invokes the given static property.
		/// </summary>
		/// <typeparam name="TReturn">The return type of the method to invoke.</typeparam>
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
