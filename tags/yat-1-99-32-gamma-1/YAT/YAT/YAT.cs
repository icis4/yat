﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 1 Version 1.99.32
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
	/// Application main class of YAT for GUI operation.
	/// </summary>
	/// <remarks>
	/// This class is separated into its own .exe project for those who want to use YAT components
	/// within their own application context.
	/// Sealed to prevent FxCop "CA1052:StaticHolderTypesShouldBeSealed".
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
		/// The application's exit code according to <see cref="Controller.Main.Result"/>.
		/// </returns>
		/// <remarks>
		/// There must separate Windows.Forms application and console application projects to
		/// properly support running YAT from console as well as with GUI.
		/// 
		/// Windows.Forms application:
		/// In case of a Windows.Forms application, console output is not routed back to the
		/// console. Thus, the command line result, help text or errors are not visible.
		/// 
		/// Windows.Forms application with MKY.Win32.Console.Attach/Detach();
		/// Calling Console.Attach/Detach() solves the issue stated above, but only in case of
		/// directly calling YAT from the command line. It does not solve the issue when calling
		/// YAT from PowerShell. In case of PowerShell, an exception is thrown! Also, when
		/// requesting the command line help, it is output after another command line prompt.
		/// 
		/// Console application:
		/// In case of a console application, console output is properly handled but a console
		/// window appears when starting the application. That console window is not acceptable
		/// in normal (i.e. GUI) operation. What other application opens with a console window
		/// in the background?
		/// 
		/// None of the three approaches above is good enough for YAT. And no other approaches
		/// have been found, even after investing quite some time into online research and
		/// asking other .NET developers.
		/// 
		/// Note that this remark can also be found at YAT.YAT.Main().
		/// </remarks>
		[STAThread]
		private static int Main(string[] commandLineArgs)
		{
			Controller.Main main = new Controller.Main(commandLineArgs);
			Controller.Main.Result mainResult = main.Run();
			return ((int)mainResult);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================