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
// YAT Version 2.1.1 Development
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
#if (WITH_SCRIPTING)
using System.Collections.Generic;
#endif
using System.Diagnostics.CodeAnalysis;
#if (WITH_SCRIPTING)
using System.Globalization;
using System.Text;
#endif

using MKY;
using MKY.Diagnostics;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in the YAT.Domain\BinaryTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// <see cref="Terminal"/> implementation with binary semantics.
	/// </summary>
	/// <remarks>
	/// This class is implemented using partial classes separating sending/processing functionality.
	/// Using partial classes rather than aggregated sender, processor,... so far for these reasons:
	/// <list type="bullet">
	/// <item><description>Simpler for implementing text/binary specialization.</description></item>
	/// <item><description>Simpler for implementing synchronization among Tx and Rx.</description></item>
	/// <item><description>Less "Durchlauferhitzer", e.g. directly raising events.</description></item>
	/// </list>
	/// </remarks>
	public partial class BinaryTerminal : Terminal
	{
		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public BinaryTerminal(Settings.TerminalSettings settings)
			: base(settings)
		{
			AttachBinaryTerminalSettings();
		}

		/// <summary></summary>
		public BinaryTerminal(Settings.TerminalSettings settings, Terminal terminal)
			: base(settings, terminal)
		{
			AttachBinaryTerminalSettings();

			var casted = (terminal as BinaryTerminal);
			if (casted != null)
			{
				// Tx:
				{
					this.txUnidirBinaryLineState = new BinaryLineState(casted.txUnidirBinaryLineState);
					this.txBidirBinaryLineState  = new BinaryLineState(casted.txBidirBinaryLineState);
				}

				// Rx:
				{
					this.rxUnidirBinaryLineState = new BinaryLineState(casted.rxUnidirBinaryLineState);
					this.rxBidirBinaryLineState  = new BinaryLineState(casted.rxBidirBinaryLineState);
				}
			}
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					DetachBinaryTerminalSettings();
				}
			}

			base.Dispose(disposing);
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		private Settings.BinaryTerminalSettings BinaryTerminalSettings
		{
			get
			{
				if (TerminalSettings != null)
					return (TerminalSettings.BinaryTerminal);
				else
					return (null);
			}
		}

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region Settings
		//------------------------------------------------------------------------------------------
		// Settings
		//------------------------------------------------------------------------------------------

		private void AttachBinaryTerminalSettings()
		{
			if (BinaryTerminalSettings != null)
				BinaryTerminalSettings.Changed += BinaryTerminalSettings_Changed;
		}

		private void DetachBinaryTerminalSettings()
		{
			if (BinaryTerminalSettings != null)
				BinaryTerminalSettings.Changed -= BinaryTerminalSettings_Changed;
		}

		private void ApplyBinaryTerminalSettings()
		{
			RefreshRepositories();
		}

		#endregion

		#region Settings Events
		//------------------------------------------------------------------------------------------
		// Settings Events
		//------------------------------------------------------------------------------------------

		private void BinaryTerminalSettings_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			ApplyBinaryTerminalSettings();
		}

		#endregion

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging.

			return (ToExtendedDiagnosticsString()); // No 'real' ToString() method required yet.
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public override string ToExtendedDiagnosticsString(string indent = "")
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging.

			return (indent + "> Type: BinaryTerminal" + Environment.NewLine + base.ToExtendedDiagnosticsString(indent));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
