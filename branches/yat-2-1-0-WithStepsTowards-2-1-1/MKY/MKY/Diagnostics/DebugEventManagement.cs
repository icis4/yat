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
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace MKY.Diagnostics
{
	/// <summary>
	/// Provides static methods to help debugging event handler management.
	/// </summary>
	public static class DebugEventManagement
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		[Conditional("DEBUG")]
		public static void DebugWriteAllEventRemains(object obj)
		{
			DebugWriteEventRemains(obj, null);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		[Conditional("DEBUG")]
		public static void DebugWriteEventRemains(object obj, string eventName)
		{
			var sinks = EventHandlerHelper.GetEventSinks(obj, eventName);
			if (sinks.Count > 0)
			{
				var sb = new StringBuilder(sinks.Count.ToString(CultureInfo.CurrentCulture));

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
					if ((sink.Value2 != null) && (sink.Value2.Target != null))
					{
						try
						{
							target = sink.Value2.Target.ToString();
						}
						catch (Exception ex)
						{
							using (var sw = new StringWriter(CultureInfo.CurrentCulture))
							{
								AnyWriter.WriteException(sw, sink.Value2.Target.GetType(), ex, "Exception while retrieving target information!");
								target = sw.ToString();
							}
						}
					}
					else
					{
						target = "Unable to retrieve target information!";
					}

					string method;
					if ((sink.Value2 != null) && (sink.Value2.Method != null))
					{
						try
						{
							method = sink.Value2.Method.ToString();
						}
						catch (Exception ex)
						{
							using (var sw = new StringWriter(CultureInfo.CurrentCulture))
							{
								AnyWriter.WriteException(sw, sink.Value2.Method.GetType(), ex, "Exception while retrieving method information!");
								method = sw.ToString();
							}
						}
					}
					else
					{
						method = "Unable to retrieve method information!";
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
