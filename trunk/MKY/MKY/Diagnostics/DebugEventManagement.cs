﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace MKY.Diagnostics
{
	/// <summary>
	/// Provides static methods to help debugging events.
	/// </summary>
	/// <remarks>
	/// Based on http://www.codeproject.com/Articles/103542/Removing-Event-Handlers-using-Reflection.
	/// </remarks>
	public static class DebugEventManagement
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		[Conditional("DEBUG")]
		public static void DebugNotifyAllEventRemains(object obj)
		{
			DebugNotifyEventRemains(obj, null);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		[Conditional("DEBUG")]
		public static void DebugNotifyEventRemains(object obj, string eventName)
		{
			var sinks = EventHandlerHelper.GetEventSinks(obj, eventName);
			if (sinks.Count > 0)
			{
				StringBuilder sb = new StringBuilder(sinks.Count.ToString(CultureInfo.InvariantCulture));

				if (sinks.Count == 1)
					sb.Append(" remaining event sink in ");
				else
					sb.Append(" remaining event sinks in ");

				sb.Append("'" + obj.GetType().FullName + "'!");

				Debug.WriteLine(sb.ToString());
				Debug.Indent();

				foreach (var sink in sinks)
				{
					string target;
					try
					{
						target = sink.Value2.Target.ToString();
					}
					catch (Exception ex)
					{
						target = "Exception while retrieving object information!" + Environment.NewLine + ex.Message;
					}

					string method;
					try
					{
						method = sink.Value2.Method.ToString();
					}
					catch (Exception ex)
					{
						method = "Exception while retrieving object information!" + Environment.NewLine + ex.Message;
					}

					Debug.WriteLine(@"""" + target + @""" still references """ + method + @"""");
				}

				Debug.Unindent();
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
