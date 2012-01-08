//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

namespace MKY
{
	/// <summary></summary>
	public static class UnusedEvent
	{
		/// <summary>
		/// Utility method that can be applied to unused events to prevent compiler warnings.
		/// </summary>
		public static void PreventCompilerWarning(EventHandler handler)
		{
			if (handler != null)
				return;

			// else return too...
		}

		/// <summary>
		/// Utility method that can be applied to unused events to prevent compiler warnings.
		/// </summary>
		public static void PreventCompilerWarning<T>(EventHandler<T> handler) where T : EventArgs
		{
			if (handler != null)
				return;

			// else return too...
		}
	}

	/// <summary></summary>
	public static class UnusedArg
	{
		/// <summary>
		/// Utility method that can be applied to unused arguments to prevent code analysis warnings (e.g. FxCop).
		/// </summary>
		/// <remarks>
		/// Prevent FxCop "CA1801:ReviewUnusedParameters".
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void PreventAnalysisWarning(object obj)
		{
			if (obj != null)
				return;

			// else return too...
		}
	}

	/// <summary></summary>
	public static class UnusedField
	{
		/// <summary>
		/// Utility method that can be applied to unused fields to prevent compiler warnings.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static void PreventCompilerWarning<T>(out T obj)
		{
			obj = default(T);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
