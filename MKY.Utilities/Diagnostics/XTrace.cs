//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MKY.Utilities.Diagnostics
{
	/// <summary>
	/// Provides static methods to write diagnostics output to <see cref="System.Diagnostics.Trace"/>.
	/// </summary>
	public static class XTrace
	{
		private static TraceWrapper _traceWrapper = new TraceWrapper();

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		/// <remarks>
		/// There also is a <see cref="System.Diagnostics.Debug"/> variant of this method available
		/// in <see cref="MKY.Utilities.Diagnostics.XDebug"/>.
		/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
		/// output writers. Therefore, the two implementations use writer wrappers.
		/// </remarks>
		[Conditional("TRACE")]
		public static void WriteException(object obj, Exception ex)
		{
			DiagnosticsWriterOutput.WriteException(_traceWrapper, obj, ex);
		}

		/// <summary>
		/// Writes message and stack to <see cref="System.Diagnostics.Trace"/>.
		/// </summary>
		/// <remarks>
		/// There also is a <see cref="System.Diagnostics.Debug"/> variant of this method available
		/// in <see cref="MKY.Utilities.Diagnostics.XDebug"/>.
		/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
		/// output writers. Therefore, the two implementations use writer wrappers.
		/// </remarks>
		[Conditional("TRACE")]
		public static void WriteStack(object obj, string message)
		{
			DiagnosticsWriterOutput.WriteStack(_traceWrapper, obj, message);
		}
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
