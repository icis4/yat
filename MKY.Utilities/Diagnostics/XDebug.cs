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
	/// Provides static methods to write diagnostics output to <see cref="System.Diagnostics.Debug"/>.
	/// </summary>
	public static class XDebug
	{
		private static DebugWrapper _debugWrapper = new DebugWrapper();

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		/// <remarks>
		/// There also is a <see cref="System.Diagnostics.Trace"/> variant of this method available
		/// in <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
		/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
		/// output writers. Therefore, the two implementations use writer wrappers.
		/// </remarks>
		[Conditional("DEBUG")]
		public static void WriteException(object obj, Exception ex)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteException(_debugWrapper, obj, ex);
		#endif
		}

		/// <summary>
		/// Writes message and stack to <see cref="System.Diagnostics.Debug"/>.
		/// </summary>
		/// <remarks>
		/// There also is a <see cref="System.Diagnostics.Trace"/> variant of this method available
		/// in <see cref="MKY.Utilities.Diagnostics.XTrace"/>.
		/// Unfortunately, the Debug and Trace objects do not provide access to their underlying
		/// output writers. Therefore, the two implementations use writer wrappers.
		/// </remarks>
		[Conditional("DEBUG")]
		public static void WriteStack(object obj, string message)
		{
		#if (DEBUG)
			DiagnosticsWriterOutput.WriteStack(_debugWrapper, obj, message);
		#endif
		}
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
