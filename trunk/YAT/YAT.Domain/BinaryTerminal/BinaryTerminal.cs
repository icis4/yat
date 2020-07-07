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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;

using MKY;
using MKY.Text;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in YAT.Domain\BinaryTerminal for better separation of the implementation files.
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
	/// <item><description>Less simple-stupid-forwarder, e.g. directly raising events.</description></item>
	/// </list>
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
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
			InitializeProcess();
		}

		/// <summary></summary>
		public BinaryTerminal(Settings.TerminalSettings settings, Terminal terminal)
			: base(settings, terminal)
		{
			AttachBinaryTerminalSettings();
			InitializeProcess();
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			DetachBinaryTerminalSettings();

			// Dispose of managed resources:
			if (disposing)
			{
				// Nothing else to do (yet).
			}

			base.Dispose(disposing);
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual Settings.BinaryTerminalSettings BinaryTerminalSettings
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

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Parse
		//------------------------------------------------------------------------------------------
		// Parse
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Tries to parse <paramref name="s"/>, taking the current settings into account.
		/// </summary>
		protected override bool TryParse(string s, Radix defaultRadix, Parser.Mode parseMode, out Parser.Result[] result, out string textSuccessfullyParsed)
		{
			AssertUndisposed();

			using (var p = new Parser.Parser((EncodingEx)BinaryTerminalSettings.EncodingFixed, TerminalSettings.IO.Endianness, parseMode))
				return (p.TryParse(s, out result, out textSuccessfullyParsed, defaultRadix));
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/>, taking the current settings into account.
		/// </summary>
		public override bool TryParse(string s, Radix defaultRadix, out byte[] result)
		{
			AssertUndisposed();

			using (var p = new Parser.Parser((EncodingEx)BinaryTerminalSettings.EncodingFixed, TerminalSettings.IO.Endianness, TerminalSettings.Send.Text.ToParseMode()))
				return (p.TryParse(s, out result, defaultRadix));
		}

		#endregion

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

		/// <remarks>
		/// This method shall not be overridden as it accesses the quasi-private member
		/// <see cref="BinaryTerminalSettings"/>.
		/// </remarks>
		protected Settings.BinaryDisplaySettings GetBinaryDisplaySettings(IODirection dir)
		{
			switch (dir)
			{
				case IODirection.Tx:    return (BinaryTerminalSettings.TxDisplay);
				case IODirection.Rx:    return (BinaryTerminalSettings.RxDisplay);

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
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
			// AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

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
			// AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (indent + "> Type: BinaryTerminal" + Environment.NewLine + base.ToExtendedDiagnosticsString(indent));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
