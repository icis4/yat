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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

namespace YAT
{
	/// <summary>
	/// Application main class of YAT.
	/// </summary>
	/// <remarks>
	/// This class is separated into its own .exe project for those who want to use YAT components
	/// within their own application context.
	/// Sealed to prevent FxCop "CA1052:StaticHolderTypesShouldBeSealeds".
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public sealed class YAT
	{
		/// <remarks>
		/// Prevent FxCop "CA1053:StaticHolderTypesShouldNotHaveConstructors".
		/// </remarks>
		private YAT()
		{
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// <param name="commandLineArgs">An array containing the command line arguments.</param>
		/// <returns>
		/// The application's exit code according to <see cref="Controller.MainResult"/>.
		/// </returns>
		/// <remarks>
		/// The YAT application must manually attach to the Win32 console since it is a windows
		/// forms application. Without attaching, no console is available and no command line text
		/// can be output.
		/// </remarks>
		[STAThread]
		private static int Main(string[] commandLineArgs)
		{
			Controller.Main main = new Controller.Main(commandLineArgs);
			Controller.MainResult result = main.Run();
			return ((int)result);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
