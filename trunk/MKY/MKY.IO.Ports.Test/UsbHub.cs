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
// MKY Version 1.0.25
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

namespace MKY.IO.Ports.Test
{
	#region UsbHubSetting
	//==============================================================================================
	// UsbHubSetting
	//==============================================================================================

	/// <summary>Flags to enable/disable the outputs of the 'RS-232' USB Hub.</summary>
	[Flags]
	public enum UsbHubSettings
	{
		/// <summary>No outputs enabled.</summary>
		None = 0x00,

		/// <summary>All outputs enabled, i.e. "Out 1..4".</summary>
		All  = 0x0F,

	/////// <summary>All outputs enabled, i.e. "Out 1..6".</summary>
	////All  = 0x3F, \If1To6

		/// <remarks>
		/// Needed to workaround limitations of some device or windows drivers. Must be sequenced
		/// in reversed order, enumeration/configuration of higher devices otherwise may fail.
		/// </remarks>
		/// <summary>Upper pair of outputs, i.e. "Out 3+4".</summary>
		Step1 = 0x0C,

		/// <summary>Lower pair of outputs, i.e. "Out 1+2".</summary>
		Step2 = 0x03,

	/////// <summary>Upper pair of outputs, i.e. "Out 5+6".</summary>
	////Step1 = 0x30,

	/////// <summary>Middle pair of outputs, i.e. "Out 3+4".</summary>
	////Step2 = 0x0C,

	/////// <summary>Lower pair of outputs, i.e. "Out 1+2".</summary>
	////Step3 = 0x03, \If1To6

		/// <summary>Out 1.</summary>
		Out1 = 0x01,

		/// <summary>Out 2.</summary>
		Out2 = 0x02,

		/// <summary>Out 3.</summary>
		Out3 = 0x04,

		/// <summary>Out 4.</summary>
		Out4 = 0x08,

	/////// <summary>Out 5.</summary>
	////Out5 = 0x10,

	/////// <summary>Out 6.</summary>
	////Out6 = 0x20 \If1To6
	}

	#endregion

	#region UsbHubControl
	//==============================================================================================
	// UsbHubControl
	//==============================================================================================

	/// <summary></summary>
	public static class UsbHubControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <remarks>Requires system path.</remarks>
		private const string Executable = "USBHubControl.exe";

		/// <remarks>Makes not (yet) sense to put this into the test configuration.</remarks>
		private const string Serial = "A6YJ5A78";

		/// <remarks>Example: 001100.</remarks>
		private const int MaxBits = 6;

		/// <summary>Error message for convenience.</summary>
		public const string ErrorMessage = @"The required """ + Executable + @""" is not available, therefore this test is excluded. Ensure that the ""MCD Conline USB HUB"" drivers are installed, and ""\Tools\CommandLine\USBHubControl.exe"" has been added to the system's PATH.";

		/// <summary>Hub seems to also require some delay between two executions, otherwise execution will fail.</summary>
		/// <remarks>Same delay as in "MCD Conline USB HUB 6-Port Runtime Config.cmd".</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Conline' is a product name.")]
		private const int WaitForNextExecution = 3000;

		/// <summary>Delay until driver has been loaded, otherwise subsequent calls will fail.</summary>
		/// <remarks>Same delay as in "MCD Conline USB HUB 6-Port Runtime Config.cmd".</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Conline' is a product name.")]
		private const int WaitForDriverLoading = 6000;

		/// <summary>Unloading seems to also require some delay, otherwise subsequent calls will fail.</summary>
		/// <remarks>Same delay as in "MCD Conline USB HUB 6-Port Runtime Config.cmd".</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Conline' is a product name.")]
		private const int WaitForDriverUnloading = 2000;

		#endregion

		#region Variables
		//==========================================================================================
		// Variables
		//==========================================================================================

		/// <summary>
		/// Proxy of the currently active hub settings.
		/// Required to allow masking of single of multiple devices.
		/// </summary>
		/// <remarks>
		/// Assume that all used outputs are enabled at first.
		/// </remarks>
		private static UsbHubSettings staticProxy = UsbHubSettings.All;

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Probes the USB hub control.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool Probe()
		{
			return (FileEx.IsFindable(Executable));
		}

		/// <summary>
		/// Sets all outputs to the given setting.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool Set(UsbHubSettings setting)
		{
			DebugMessage("Setting   " + ToBinaryString(setting) + " mask.");

			UsbHubSettings disableMask = ( staticProxy & ~setting);
			UsbHubSettings enableMask  = (~staticProxy &  setting);

			if (disableMask != UsbHubSettings.None)
			{
				UsbHubSettings accumulated = (staticProxy & ~disableMask);
				if (!ExecuteControl(accumulated)) // No need to do steps here, disabling only.
					return (false);

				staticProxy = accumulated; // Update proxy.
				Thread.Sleep(WaitForDriverUnloading);
			}

			if (enableMask != UsbHubSettings.None)
			{
				UsbHubSettings accumulated = (staticProxy | enableMask);
				if (!SetInSteps(accumulated))
					return (false);

				// Updating proxy and waiting for driver is done in SetInSteps().
			}

			return (true);
		}

		private static bool SetInSteps(UsbHubSettings setting)
		{
			UsbHubSettings stepMask;

			stepMask = setting & UsbHubSettings.Step1;
			if (stepMask != UsbHubSettings.None)
			{
				UsbHubSettings accumulated = (staticProxy | stepMask);
				if (!ExecuteControl(accumulated))
					return (false);

				staticProxy = accumulated; // Update proxy.
				Thread.Sleep(WaitForDriverLoading);
			}

			stepMask = setting & UsbHubSettings.Step2;
			if (stepMask != UsbHubSettings.None)
			{
				UsbHubSettings accumulated = (staticProxy | stepMask);
				if (!ExecuteControl(accumulated))
					return (false);

				staticProxy = accumulated; // Update proxy.
				Thread.Sleep(WaitForDriverLoading);
			}

			////step = setting & UsbHubSetting.Step3; \If1To6
			////if (step != UsbHubSetting.None)
			////{
			////	UsbHubSetting accumulated = (staticProxy | step);
			////	if (!ExecuteControl(accumulated))
			////		return (false);
			////
			////	staticProxy = accumulated; // Update proxy.
			////	Thread.Sleep(WaitForDriverLoading);
			////}

			return (true);
		}

		/// <summary>
		/// Enables the given outputs.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool Enable(UsbHubSettings enableMask)
		{
			DebugMessage("Enabling  " + ToBinaryString(enableMask) + " mask. " +
			                      "Proxy was " + ToBinaryString(staticProxy) + " mask.");

			UsbHubSettings accumulated = (staticProxy | enableMask);
			if (!SetInSteps(accumulated))
				return (false);

			// Updating proxy and waiting for driver is done in SetInSteps().
			return (true);
		}

		/// <summary>
		/// Disables the given outputs.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool Disable(UsbHubSettings disableMask)
		{
			DebugMessage("Disabling " + ToBinaryString(disableMask) + " mask. " +
			                      "Proxy was " + ToBinaryString(staticProxy) + " mask.");

			UsbHubSettings accumulated = (staticProxy & ~disableMask);
			if (!ExecuteControl(accumulated)) // No need to do steps here, disabling only.
				return (false);

			staticProxy = accumulated; // Update proxy.
			Thread.Sleep(WaitForDriverUnloading);
			return (true);
		}

		/// <remarks>
		/// Intentionally using command line instead of MCD USB .NET component to avoid dependency
		/// to the MCD USB assembly (which would have to be installed on each computer testing YAT).
		///
		/// Execution requires approx 3 seconds.
		/// </remarks>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "That's OK, this method is only used for testing.")]
		private static bool ExecuteControl(UsbHubSettings setting)
		{
			string mask = ToBinaryString(setting);
			string args = Serial + " " + mask;

			Process p = new Process();
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.FileName = Executable;
			p.StartInfo.Arguments = args;
			p.StartInfo.CreateNoWindow = true;
			p.Start();

			DebugMessage("Executing " + mask + " mask...");
			p.WaitForExit(); // Execution requires approx 3 seconds.
			Thread.Sleep(WaitForNextExecution);

			if (p.ExitCode == 0)
			{
				DebugMessage("...successfully done.");
				return (true);
			}
			else
			{
				DebugMessage("...failed!");
				return (false);
			}
		}

		private static string ToBinaryString(UsbHubSettings setting)
		{
			return (StringEx.Right(ByteEx.ConvertToBinaryString((byte)setting), MaxBits));
		}

		[Conditional("DEBUG")]
		private static void DebugMessage(string message)
		{
			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					typeof(UsbHubControl),
					"",
					"[" + Serial + "]",
					message
				)
			);
		}

		#endregion
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
