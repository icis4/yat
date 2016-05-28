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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics;
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
		[Conditional("DEBUG")]
		public static void DebugNotifyAllEventRemains(object obj)
		{
			DebugNotifyEventRemains(obj, null);
		}

		/// <summary></summary>
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

				sb.Append(obj.GetType().FullName + "!");

				Debug.WriteLine(sb.ToString());
				Debug.Indent();

				foreach (var sink in sinks)
					Debug.WriteLine(sink.Value2.Target + " still references " + sink.Value2.Method);

				Debug.Unindent();
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
