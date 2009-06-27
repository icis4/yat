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
using System.IO;

namespace MKY.Utilities.Diagnostics
{
	/// <summary>
	/// Provides static methods to write diagnostics output to any <see cref="System.IO.TextWriter"/>.
	/// </summary>
	public static class AnyWriter
	{
		private static AnyWriterWrapper _anyWriterWrapper = new AnyWriterWrapper();

		/// <summary>
		/// Writes source, type, message and stack of the given exception and its inner exceptions
		/// to the given <see cref="System.IO.TextWriter"/>.
		/// </summary>
		public static void WriteException(TextWriter writer, object obj, Exception ex)
		{
			_anyWriterWrapper.SetWriter(writer);
			DiagnosticsWriterOutput.WriteException(_anyWriterWrapper, obj, ex);
			_anyWriterWrapper.SetWriter(null);
		}
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
