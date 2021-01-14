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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using MKY.Collections.Generic;

namespace MKY
{
	/// <summary>
	/// Provides static methods to help with event handlers.
	/// </summary>
	/// <remarks>
	/// Based on http://www.codeproject.com/Articles/103542/Removing-Event-Handlers-using-Reflection.
	/// </remarks>
	public static class EventHandlerHelper
	{
		private static BindingFlags AllBindings
		{
			get { return (BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static); }
		}

		private static Dictionary<Type, List<FieldInfo>> staticEventFieldInfoCache = new Dictionary<Type, List<FieldInfo>>();
		private static object staticEventFieldInfoCacheSyncObj = new object();

		private static List<FieldInfo> GetEventFields(Type type)
		{
			lock (staticEventFieldInfoCacheSyncObj)
			{
				// Retrieve from cache if available:
				if (staticEventFieldInfoCache.ContainsKey(type))
					return (staticEventFieldInfoCache[type]);

				// Retrieve using reflection and add to cache:
				List<FieldInfo> fis = GetEventFieldsUsingReflection(type);
				staticEventFieldInfoCache.Add(type, fis);
				return (fis);
			}
		}

		private static List<FieldInfo> GetEventFieldsUsingReflection(Type type)
		{
			EventInfo[] eis = type.GetEvents(AllBindings);

			List<FieldInfo> fis = new List<FieldInfo>(eis.Length);

			foreach (EventInfo ei in eis)
			{
				Type dt = ei.DeclaringType;
				FieldInfo fi = dt.GetField(ei.Name, AllBindings);
				if (fi != null)
					fis.Add(fi);
			}

			return (fis);
		}

		private static EventHandlerList GetStaticEventHandlers(Type type, object obj)
		{
			MethodInfo mi = type.GetMethod("get_Events", AllBindings);
			if (mi != null)
				return ((EventHandlerList)mi.Invoke(obj, null));
			else
				return (null);
		}

		/// <summary>
		/// Gets all event sinks for the given event, or all events if <paramref name="eventName"/>
		/// is <c>null</c> or <see cref="string.Empty"/>.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="eventName">Name of the event.</param>
		/// <returns>All event sinks for the given object.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="obj"/> must be an object.</exception>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Why not?")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static ICollection<Pair<EventInfo, Delegate>> GetEventSinks(object obj, string eventName = null)
		{
			if (obj == null)
				throw (new ArgumentNullException("obj", "An object is required!"));

			var type = obj.GetType();
			var allEventFields = GetEventFields(type);
			var staticEventHandlers = GetStaticEventHandlers(type, obj);

			var sinks = new List<Pair<EventInfo, Delegate>>();

			foreach (FieldInfo fi in allEventFields)
			{
				if (!string.IsNullOrEmpty(eventName) && (StringEx.CompareOrdinalIgnoreCase(eventName, fi.Name) != 0))
					continue; // Not the desired event.

				EventInfo ei = type.GetEvent(fi.Name, AllBindings);
				if (ei != null)
				{
					if (fi.IsStatic)
					{
						if (staticEventHandlers != null)
						{
							object val = fi.GetValue(obj);
							Delegate staticDelegate = staticEventHandlers[val];
							if (staticDelegate != null)
							{
								foreach (Delegate del in staticDelegate.GetInvocationList())
									sinks.Add(new Pair<EventInfo, Delegate>(ei, del));
							}
						}
					}
					else // IsInstance
					{
						object val = fi.GetValue(obj);
						Delegate instanceDelegate = (val as Delegate);
						if (instanceDelegate != null)
						{
							foreach (Delegate del in instanceDelegate.GetInvocationList())
								sinks.Add(new Pair<EventInfo, Delegate>(ei, del));
						}
					}
				} // EventInfo
			} // foreach (FieldInfo)

			return (sinks);
		}

		/// <summary>
		/// Removes all event handlers from the given object.
		/// </summary>
		/// <param name="obj">The object.</param>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void RemoveAllEventHandlers(object obj)
		{
			RemoveEventHandler(obj, null);
		}

		/// <summary>
		/// Removes the event handler.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="eventName">Name of the event.</param>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void RemoveEventHandler(object obj, string eventName)
		{
			var sinks = GetEventSinks(obj, eventName);
			foreach (var sink in sinks)
				sink.Value1.RemoveEventHandler(obj, sink.Value2);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
