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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2021 Matthias Kläy.
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Permissions;
using System.Text;
using System.Threading;

using MKY.IO;

#endregion

namespace MKY.Test.Devices
{
	#region UsbHubSetting
	//==============================================================================================
	// UsbHubSetting
	//==============================================================================================

	/// <summary>Flags to enable/disable the outputs of the 'RS-232' USB Hub.</summary>
	/// <remarks>Plural name is an FxCop requirement for <see cref="FlagsAttribute"/>.</remarks>
	[Flags]
	public enum UsbHubDevices
	{
		/// <summary>No device.</summary>
		None = 0x00,

		/// <summary>Hub 1.</summary>
		Hub1 = 0x01,

		/// <summary>Hub 2.</summary>
		Hub2 = 0x02,

		/// <summary>All devices.</summary>
		All = 0x03
	}

	/// <summary>Flags to enable/disable the outputs of the 'RS-232' USB Hub.</summary>
	/// <remarks>Plural name is an FxCop requirement for <see cref="FlagsAttribute"/>.</remarks>
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

		/// <remarks>Example: 001100.</remarks>
		private const int SettingBitCount = 6;

		/// <summary>Error message for convenience.</summary>
		public const string ErrorMessage = @"The required """ + Executable + @""" is not available, therefore this test is excluded. Ensure that the ""MCD Conline USB HUB"" drivers are installed, and ""\Tools\CommandLine\USBHubControl.exe"" has been added to the system's PATH.";

		/// <remarks>Same time-out as in "MCD Conline USB HUB 6-Port Runtime Config.cmd".</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Conline' is a product name.")]
		private const int ExecutionTimeout = 3000;

		/// <summary>Delay until driver has been loaded, otherwise subsequent calls will fail.</summary>
		/// <remarks>Same delay as in "MCD Conline USB HUB 6-Port Runtime Config.cmd".</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Conline' is a product name.")]
		private const int DriverLoadingWaitTime = 6000;

		/// <summary>Unloading seems to also require some delay, otherwise subsequent calls will fail.</summary>
		/// <remarks>Same delay as in "MCD Conline USB HUB 6-Port Runtime Config.cmd".</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Conline' is a product name.")]
		private const int DriverUnloadingWaitTime = 6000;

		#endregion

		#region Variables
		//==========================================================================================
		// Variables
		//==========================================================================================

		/// <summary>
		/// Proxy of the currently active hub setting.
		/// Required to allow masking of single of multiple devices.
		/// </summary>
		/// <remarks>
		/// Assume that all used outputs are enabled at first.
		/// </remarks>
		private static UsbHubSettings staticSettingProxy = UsbHubSettings.All;

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Determines whether the USB hub control executable is available.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool ExecutableIsAvailable
		{
			get { return (FileEx.IsFindable(Executable)); }
		}

		/// <summary>
		/// Determines whether the USB hub with the given serial string is available.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool Probe(string deviceSerial)
		{
			return (TryProbe(deviceSerial));
		}

		/// <summary>
		/// Sets all outputs to the given setting.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		public static bool Set(UsbHubDevices device, UsbHubSettings setting)
		{
			DebugMessage(device, "Setting   " + SettingToBinaryString(setting) + " mask.");

			UsbHubSettings disableMask = ( staticSettingProxy & ~setting);
			UsbHubSettings enableMask  = (~staticSettingProxy &  setting);

			if (disableMask != UsbHubSettings.None)
			{
				UsbHubSettings accumulated = (staticSettingProxy & ~disableMask);
				if (!TryConfigure(device, accumulated)) // No need to do steps here, disabling only.
					return (false);

				staticSettingProxy = accumulated; // Update proxy.
				Thread.Sleep(DriverUnloadingWaitTime);
			}

			if (enableMask != UsbHubSettings.None)
			{
				UsbHubSettings accumulated = (staticSettingProxy | enableMask);
				if (!SetInSteps(device, accumulated))
					return (false);

				// Updating proxy and waiting for driver is done in SetInSteps().
			}

			return (true);
		}

		private static bool SetInSteps(UsbHubDevices device, UsbHubSettings setting)
		{
			UsbHubSettings stepMask;

			stepMask = setting & UsbHubSettings.Step1;
			if (stepMask != UsbHubSettings.None)
			{
				UsbHubSettings accumulated = (staticSettingProxy | stepMask);
				if (!TryConfigure(device, accumulated))
					return (false);

				staticSettingProxy = accumulated; // Update proxy.
				Thread.Sleep(DriverLoadingWaitTime);
			}

			stepMask = setting & UsbHubSettings.Step2;
			if (stepMask != UsbHubSettings.None)
			{
				UsbHubSettings accumulated = (staticSettingProxy | stepMask);
				if (!TryConfigure(device, accumulated))
					return (false);

				staticSettingProxy = accumulated; // Update proxy.
				Thread.Sleep(DriverLoadingWaitTime);
			}

		////step = setting & UsbHubSetting.Step3; \If1To6
		////if (step != UsbHubSetting.None)
		////{
		////	UsbHubSetting accumulated = (staticProxy | step);
		////	if (!ExecuteControl(device, accumulated))
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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Just a debug message.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Just a debug message.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",              Justification = "Just a debug message.")]
		public static bool Enable(UsbHubDevices device, UsbHubSettings enableMask)
		{
			DebugMessage(device, "Enabling  " + SettingToBinaryString(enableMask) + " mask. " +
			                     "Proxy was " + SettingToBinaryString(staticSettingProxy) + " mask.");

			UsbHubSettings accumulated = (staticSettingProxy | enableMask);
			if (!SetInSteps(device, accumulated))
				return (false);

			// Updating proxy and waiting for driver is done in SetInSteps().
			return (true);
		}

		/// <summary>
		/// Disables the given outputs.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "Just a debug message.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Just a debug message.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",              Justification = "Just a debug message.")]
		public static bool Disable(UsbHubDevices device, UsbHubSettings disableMask)
		{
			DebugMessage(device, "Disabling " + SettingToBinaryString(disableMask) + " mask. " +
			                     "Proxy was " + SettingToBinaryString(staticSettingProxy) + " mask.");

			UsbHubSettings accumulated = (staticSettingProxy & ~disableMask);
			if (!TryConfigure(device, accumulated)) // No need to do steps here, disabling only.
				return (false);

			staticSettingProxy = accumulated; // Update proxy.
			Thread.Sleep(DriverUnloadingWaitTime);
			return (true);
		}

		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Solely used for testing purposes.")]
		private static bool TryProbe(string deviceSerial)
		{
			string arguments = deviceSerial + " ?";

			DebugMessage(deviceSerial, "Probing...");

			string outputAndErrorResult;
			if (TryExecute(arguments, out outputAndErrorResult))
			{
				// Evaluate result:
				if (TryParseSettingFromBinaryString(outputAndErrorResult))
				{
					DebugMessage(deviceSerial, "...successfully done.");
					return (true);
				}
				else
				{
					DebugMessage(deviceSerial, @"...succeeded to execute but """ + outputAndErrorResult + @"""!");
					return (false);
				}
			}
			else
			{
				DebugMessage(deviceSerial, "...failed to execute!");
				return (false);
			}
		}

		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Solely used for testing purposes.")]
		private static bool TryConfigure(UsbHubDevices device, UsbHubSettings setting)
		{
			string mask = SettingToBinaryString(setting);
			string deviceSerial = DeviceToSerialString(device);
			string arguments = deviceSerial + " " + mask;

			DebugMessage(device, @"Configuring setting of """ + mask + @"""...");

			string outputAndErrorResult;
			if (TryExecute(arguments, out outputAndErrorResult))
			{
				// Evaluate result:
				if (TryParseSettingFromBinaryString(outputAndErrorResult))
				{
					DebugMessage(device, "...successfully done.");
					return (true);
				}
				else
				{
					DebugMessage(device, @"...succeeded to execute but """ + outputAndErrorResult + @"""!");
					return (false);
				}
			}
			else
			{
				DebugMessage(device, "...failed to execute!");
				return (false);
			}
		}

		/// <remarks>
		/// Intentionally using command line instead of MCD USB .NET component to avoid dependency
		/// to the MCD USB assembly (which would have to be installed on each computer testing YAT).
		///
		/// Execution requires less than <see cref="ExecutionTimeout"/>.
		/// </remarks>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		private static bool TryExecute(string arguments, out string outputAndErrorResult)
		{
			var p = new Process();
			p.StartInfo.FileName = Executable;
			p.StartInfo.Arguments = arguments;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true; // Required to allow reading result below.
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.CreateNoWindow = true;
			p.Start();

			p.WaitForExit(ExecutionTimeout);
			if (p.ExitCode == 0)
			{
				// Retrieve result:
				var sb = new StringBuilder();

				while (!p.StandardOutput.EndOfStream)       // Just Append(), not AppendLine(), result will just be e.g....
					sb.Append(p.StandardOutput.ReadLine()); // 10111
				                                            // ...or...
				while (!p.StandardError.EndOfStream)        // FTDI open error: FT_DEVICE_NOT_FOUND
					sb.Append(p.StandardOutput.ReadLine()); // ...and anything else indicates an unexpected result.
				                                            // AppendLine() would result in e.g. "10111\r\n", i.e. the EOL would
				outputAndErrorResult = sb.ToString();       // have to be trimmed before forwarding to TryParseSettingFromBinaryString().
				return (true);
			}
			else
			{
				outputAndErrorResult = null;
				return (false);
			}
		}

		private static string SettingToBinaryString(UsbHubSettings setting)
		{
			return (StringEx.Right(ByteEx.ConvertToBinaryString((byte)setting), SettingBitCount));
		}

		private static bool TryParseSettingFromBinaryString(string s)
		{
			UsbHubSettings dummy;
			return (TryParseSettingFromBinaryString(s, out dummy));
		}

		private static bool TryParseSettingFromBinaryString(string s, out UsbHubSettings result)
		{
			ulong ulongResult;
			if (UInt64Ex.TryParseBinary(s, out ulongResult))
			{
				var byteResult = (byte)ulongResult;
				result = (UsbHubSettings)byteResult;
				return (true);
			}
			else
			{
				result = 0x00;
				return (false);
			}
		}

		private static string DeviceToSerialString(UsbHubDevices device)
		{
			switch (device)
			{
				case UsbHubDevices.None:
				case UsbHubDevices.All:  throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "This method requires a dedicated device, not '" + device + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "device"));

				case UsbHubDevices.Hub1: return (ConfigurationProvider.Configuration.UsbHub1);
				case UsbHubDevices.Hub2: return (ConfigurationProvider.Configuration.UsbHub2);

				default: throw (new ArgumentOutOfRangeException("device", device, MessageHelper.InvalidExecutionPreamble + "'" + device + "' identifies a device that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		internal static void DebugMessage(UsbHubDevices device, string message)
		{
			DebugMessage(device.ToString(), message);
		}

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		internal static void DebugMessage(string device, string message)
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
					"[" + device + "]",
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
