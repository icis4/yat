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
// MKY Version 1.0.26 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading;

using MKY.IO;

namespace MKY.Test.Devices
{
	#region UsbHubSetting
	//==============================================================================================
	// UsbHubSetting
	//==============================================================================================

	/// <summary>Flags to enable/disable the outputs of the 'RS-232' USB Hub.</summary>
	[Flags]
	public enum UsbHubDevice
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
	[Flags]
	public enum UsbHubSetting
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

		/// <remarks>Same timeout as in "MCD Conline USB HUB 6-Port Runtime Config.cmd".</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Conline' is a product name.")]
		private const int ExecutionTimeout = 3000;

	/////// <summary>Hub seems to also require some delay between two executions, otherwise execution will fail.</summary>
	/////// <remarks>Same delay as in "MCD Conline USB HUB 6-Port Runtime Config.cmd".</remarks>
	////[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Conline' is a product name.")]
	////private const int NextExecutionWaitTime = 3000; PENDING

		/// <summary>Delay until driver has been loaded, otherwise subsequent calls will fail.</summary>
		/// <remarks>Same delay as in "MCD Conline USB HUB 6-Port Runtime Config.cmd".</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Conline' is a product name.")]
		private const int DriverLoadingWaitTime = 6000;

		/// <summary>Unloading seems to also require some delay, otherwise subsequent calls will fail.</summary>
		/// <remarks>Same delay as in "MCD Conline USB HUB 6-Port Runtime Config.cmd".</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Conline' is a product name.")]
		private const int DriverUnloadingWaitTime = 2000;

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
		private static UsbHubSetting staticSettingProxy = UsbHubSetting.All;

	////private static DateTime staticLastExectionTimeStamp = DateTime.MinValue; PENDING

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
		public static bool Set(UsbHubDevice device, UsbHubSetting setting)
		{
			DebugMessage(device, "Setting   " + SettingToBinaryString(setting) + " mask.");

			UsbHubSetting disableMask = ( staticSettingProxy & ~setting);
			UsbHubSetting enableMask  = (~staticSettingProxy &  setting);

			if (disableMask != UsbHubSetting.None)
			{
				UsbHubSetting accumulated = (staticSettingProxy & ~disableMask);
				if (!TryConfigure(device, accumulated)) // No need to do steps here, disabling only.
					return (false);

				staticSettingProxy = accumulated; // Update proxy.
				Thread.Sleep(DriverUnloadingWaitTime);
			}

			if (enableMask != UsbHubSetting.None)
			{
				UsbHubSetting accumulated = (staticSettingProxy | enableMask);
				if (!SetInSteps(device, accumulated))
					return (false);

				// Updating proxy and waiting for driver is done in SetInSteps().
			}

			return (true);
		}

		private static bool SetInSteps(UsbHubDevice device, UsbHubSetting setting)
		{
			UsbHubSetting stepMask;

			stepMask = setting & UsbHubSetting.Step1;
			if (stepMask != UsbHubSetting.None)
			{
				UsbHubSetting accumulated = (staticSettingProxy | stepMask);
				if (!TryConfigure(device, accumulated))
					return (false);

				staticSettingProxy = accumulated; // Update proxy.
				Thread.Sleep(DriverLoadingWaitTime);
			}

			stepMask = setting & UsbHubSetting.Step2;
			if (stepMask != UsbHubSetting.None)
			{
				UsbHubSetting accumulated = (staticSettingProxy | stepMask);
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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Just a debug message.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Just a debug message.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Just a debug message.")]
		public static bool Enable(UsbHubDevice device, UsbHubSetting enableMask)
		{
			DebugMessage(device, "Enabling  " + SettingToBinaryString(enableMask) + " mask. " +
			                     "Proxy was " + SettingToBinaryString(staticSettingProxy) + " mask.");

			UsbHubSetting accumulated = (staticSettingProxy | enableMask);
			if (!SetInSteps(device, accumulated))
				return (false);

			// Updating proxy and waiting for driver is done in SetInSteps().
			return (true);
		}

		/// <summary>
		/// Disables the given outputs.
		/// </summary>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Just a debug message.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "Just a debug message.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Just a debug message.")]
		public static bool Disable(UsbHubDevice device, UsbHubSetting disableMask)
		{
			DebugMessage(device, "Disabling " + SettingToBinaryString(disableMask) + " mask. " +
			                     "Proxy was " + SettingToBinaryString(staticSettingProxy) + " mask.");

			UsbHubSetting accumulated = (staticSettingProxy & ~disableMask);
			if (!TryConfigure(device, accumulated)) // No need to do steps here, disabling only.
				return (false);

			staticSettingProxy = accumulated; // Update proxy.
			Thread.Sleep(DriverUnloadingWaitTime);
			return (true);
		}

		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
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
		private static bool TryConfigure(UsbHubDevice device, UsbHubSetting setting)
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
		/// Execution requires approx 3 seconds.
		/// </remarks>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		private static bool TryExecute(string arguments, out string outputAndErrorResult)
		{
		////var nextExecutionTimeStamp = (staticLastExectionTimeStamp + TimeSpan.FromMilliseconds(NextExecutionWaitTime));
		////var nextExecutionWaitDelta = (nextExecutionTimeStamp - DateTime.Now);
		////if (nextExecutionWaitDelta.TotalMilliseconds > 0)
		////	Thread.Sleep((int)Math.Ceiling(nextExecutionWaitDelta.TotalMilliseconds)); PENDING

			var p = new Process();
			p.StartInfo.FileName = Executable;
			p.StartInfo.Arguments = arguments;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true; // Required to allow reading result below.
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.CreateNoWindow = true;
			p.Start();

			p.WaitForExit(ExecutionTimeout);
		////staticLastExectionTimeStamp = DateTime.Now; PENDING
			if (p.ExitCode == 0)
			{
				// Retrieve result:
				var sb = new StringBuilder();

				while (!p.StandardOutput.EndOfStream)
					sb.AppendLine(p.StandardOutput.ReadLine());

				while (!p.StandardError.EndOfStream)
					sb.AppendLine(p.StandardOutput.ReadLine());

				outputAndErrorResult = sb.ToString();
				return (true);
			}
			else
			{
				outputAndErrorResult = null;
				return (false);
			}
		}

		private static string SettingToBinaryString(UsbHubSetting setting)
		{
			return (StringEx.Right(ByteEx.ConvertToBinaryString((byte)setting), SettingBitCount));
		}

		private static bool TryParseSettingFromBinaryString(string s)
		{
			UsbHubSetting dummy;
			return (TryParseSettingFromBinaryString(s, out dummy));
		}

		private static bool TryParseSettingFromBinaryString(string s, out UsbHubSetting result)
		{
			ulong ulongResult;
			if (UInt64Ex.TryParseBinary(s, out ulongResult))
			{
				var byteResult = (byte)ulongResult;
				result = (UsbHubSetting)byteResult;
				return (true);
			}
			else
			{
				result = 0x00;
				return (false);
			}
		}

		private static string DeviceToSerialString(UsbHubDevice device)
		{
			switch (device)
			{
				case UsbHubDevice.None:
				case UsbHubDevice.All:  throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "This method requires a dedicated device, not '" + device + "'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "device"));

				case UsbHubDevice.Hub1: return (ConfigurationProvider.Configuration.UsbHub1);
				case UsbHubDevice.Hub2: return (ConfigurationProvider.Configuration.UsbHub2);

				default: throw (new ArgumentOutOfRangeException("device", device, MessageHelper.InvalidExecutionPreamble + "'" + device + "' identifies a device that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		[Conditional("DEBUG")]
		private static void DebugMessage(UsbHubDevice device, string message)
		{
			DebugMessage(device.ToString(), message);
		}

		[Conditional("DEBUG")]
		private static void DebugMessage(string device, string message)
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
