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
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace MKY
{
	/// <summary>
	/// Helper to deal with exceptions.
	/// </summary>
	public class ExceptionHelper
	{
		/// <summary>
		/// Gets or sets the owner of this helper object.
		/// </summary>
		public string Owner { get; protected set; }

		private Dictionary<Type, bool> exceptionTypesToIgnore = new Dictionary<Type, bool>(); // Default initial capacity is OK.
		private object exceptionTypesToIgnoreSyncObj = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionHelper"/> class.
		/// </summary>
		/// <param name="owner">The owner of this helper object.</param>
		public ExceptionHelper(string owner)
		{
			Owner = owner;
		}

		/// <summary>
		/// Ignores the given exception type.
		/// </summary>
		public virtual void IgnoreExceptionType(Type type)
		{
			lock (exceptionTypesToIgnoreSyncObj)
			{
				if (exceptionTypesToIgnore != null)
				{
					if (exceptionTypesToIgnore.ContainsKey(type))
					{
						exceptionTypesToIgnore[type] = true;
						TraceMessage(type + " activated/re-activated in list of ignored exceptions.");
					}
					else
					{
						exceptionTypesToIgnore.Add(type, true);
						TraceMessage(type + " added to list of ignored exceptions.");
					}
				}
			}
		}

		/// <summary>
		/// No longer ignores the given exception type.
		/// </summary>
		public virtual void RevertIgnoredExceptionType(Type type)
		{
			lock (exceptionTypesToIgnoreSyncObj)
			{
				if (exceptionTypesToIgnore != null)
				{
					if (exceptionTypesToIgnore.Remove(type)) // MSDN: "If ... does not contain ... specified key ... no exception is thrown.
						TraceMessage(type + " removed from list of ignored exceptions.");
				}
			}
		}

		/// <summary>
		/// Evaluates whether the given <paramref name="type"/> is currently being ignored.
		/// </summary>
		/// <remarks>
		/// Used to prevent that multiple unhandled exceptions also generate multiple unhandled exception dialogs.
		/// Any exception of the same type of a type that <see cref="Type.IsAssignableFrom(Type)"/> will be ignored.
		/// </remarks>
		public virtual bool ExceptionTypeIsIgnored(Type type)
		{
			lock (exceptionTypesToIgnoreSyncObj)
			{
				if (exceptionTypesToIgnore != null)
				{
					foreach (var typeToIgnore in exceptionTypesToIgnore)
					{
						if (typeToIgnore.Value) // = type is activated.
						{
							if (typeToIgnore.Key.IsAssignableFrom(type)) // = base type is assignable from derived type?
							{
								TraceMessage(type + " is being ignored.");
								return (true);
							}
						}
					}
				}
			}

			TraceMessage(type + " is not being ignored (yet).");
			return (false);
		}

		/// <summary>
		/// Terminates the exception ignoring, e.g. during shutdown, or if the user requests to disable it.
		/// </summary>
		public virtual void TerminateExceptionIgnoring()
		{
			lock (exceptionTypesToIgnoreSyncObj)
			{
				exceptionTypesToIgnore = null;
				TraceMessage("No longer ignoring exceptions.");
			}
		}

		[Conditional("TRACE")]
		private void TraceMessage(string message)
		{
			Trace.WriteLine
			(
				string.Format
				(
					CultureInfo.InvariantCulture,
					" @ {0} @ Thread #{1} : {2} owned by {3} : {4}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.InvariantCulture),
					GetType(),
					(!string.IsNullOrEmpty(Owner) ? Owner : "<Undefined>"),
					message
				)
			);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
