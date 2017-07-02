﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 3 Version 1.99.70
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;
using System.Windows.Forms;

using MKY.Windows.Forms;

namespace YAT.Test
{
	/// <summary>
	/// This is the test main dummy for the YAT test projects.
	/// </summary>
	/// <remarks>
	/// See "!-ReadMe.txt" for more information.
	/// </remarks>
	public static class TestMain
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			var sb = new StringBuilder();
			sb.AppendLine("Note that this console application is only a dummy for testing purposes.");
			sb.AppendLine();
			sb.Append    (@"See ""!-ReadMe.txt"" for more information.");

			MessageBoxEx.Show(sb.ToString(), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
