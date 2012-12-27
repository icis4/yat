//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

using MKY.CommandLine;

#endregion

namespace YAT.Controller
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.")]
	public class CommandLineArgs : Model.CommandLineArgs
	{
		private const string SuppressionJustification = "Command line arguments based on 'MKY.CommandLine.ArgsHandler' must be public.";

		#region Public Fields = Command Line Arguments
		//==========================================================================================
		// Public Fields = Command Line Arguments
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "NoLogo", ShortName = "nl", Description = "Do not display title and copyright.")]
		public bool NoLogo;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[OptionArg(Name = "NoView", ShortName = "nv", Description = "Start the YAT application just on the console, without any GUI at all.")]
		public bool NoView;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = SuppressionJustification)]
		[CLSCompliant(false)]
		[OptionArg(Names = new string[] { "Help", "HelpText" }, ShortNames = new string[] { "h", "?" }, Description = "Display this help text.")]
		public bool HelpIsRequested;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public CommandLineArgs(string[] args)
			: base(args)
		{
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Gets a value indicating whether [show logo].
		/// </summary>
		public bool ShowLogo
		{
			get { return (!(NoLogo)); }
		}

		/// <summary>
		/// Gets a value indicating whether [show view].
		/// </summary>
		public bool ShowView
		{
			get { return (!(NoView)); }
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
